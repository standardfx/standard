using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Standard.IO.Compression
{
    internal unsafe class LZ4Engine
    {
        // [StructLayout(LayoutKind.Sequential)]
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]

        protected const int MemoryUsage = 14;
        protected const int MaxInputSize = 0x7E000000;

        protected const int HashLog = MemoryUsage - 2;
        protected const int HashTableSize = 1 << MemoryUsage;
        protected const int HashSizeU32 = 1 << HashLog;

        protected const int AccelerationDefault = 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CompressBound(int isize)
        {
            return isize > MaxInputSize ? 0 : isize + isize / 255 + 16;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct StreamT
        {
            public fixed uint HashTable[HashSizeU32];
            public uint CurrentOffset;
            public uint InitCheck;
            public byte* Dictionary;
            // public byte* BufferStart; // obsolete, used for slideInputBuffer
            public uint DictSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct StreamDecodeT
        {
            public byte* ExternalDict;
            public uint ExtDictSize;
            public byte* PrefixEnd;
            public uint PrefixSize;
        }

        protected const int MinMatch = 4;

        protected const int WildCopyLength = 8;
        protected const int LastLiterals = 5;
        protected const int MFLimit = WildCopyLength + MinMatch;
        protected const int LZ4MinLength = MFLimit + 1;

        protected const int KB = 1 << 10;
        protected const int MB = 1 << 20;
        protected const uint GB = 1u << 30;

        protected const int MaxDLog = 16;
        protected const int MaxDistance = (1 << MaxDLog) - 1;

        protected const int MLBits = 4;
        protected const uint MLMask = (1U << MLBits) - 1;
        protected const int RunBits = 8 - MLBits;
        protected const uint RunMask = (1U << RunBits) - 1;

        protected const int Limit64k = 64 * KB + (MFLimit - 1);
        protected const int SkipTrigger = 6;

        public enum LimitedOutputDirective
        {
            NoLimit = 0,
            LimitedOutput = 1,
            LimitedDestSize = 2,
        }

        public enum TableTypeT
        {
            ByPtr = 0,
            ByU32 = 1,
            ByU16 = 2
        }

        public enum DictDirective
        {
            NoDict = 0,
            WithPrefix64k,
            UsingExtDict
        }

        public enum DictIssueDirective
        {
            NoDictIssue = 0,
            DictSmall
        }

        public enum EndConditionDirective
        {
            EndOnOutputSize = 0,
            EndOnInputSize = 1
        }

        public enum EarlyEndDirective
        {
            Full = 0,
            Partial = 1
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static uint Hash4(uint sequence, TableTypeT tableType)
        {
            int hashLog = tableType == TableTypeT.ByU16 ? HashLog + 1 : HashLog;
            return (sequence * 2654435761u) >> (MinMatch * 8 - hashLog);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static uint Hash5(ulong sequence, TableTypeT tableType)
        {
            int hashLog = tableType == TableTypeT.ByU16 ? HashLog + 1 : HashLog;
            return (uint)(((sequence << 24) * 889523592379ul) >> (64 - hashLog));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void PutPositionOnHash(byte* p, uint h, void* tableBase, TableTypeT tableType, byte* srcBase)
        {
            switch (tableType)
            {
                case TableTypeT.ByPtr:
                    ((byte**)tableBase)[h] = p;
                    return;
                case TableTypeT.ByU32:
                    ((uint*)tableBase)[h] = (uint)(p - srcBase);
                    return;
                default:
                    ((ushort*)tableBase)[h] = (ushort)(p - srcBase);
                    return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static byte* GetPositionOnHash(uint h, void* tableBase, TableTypeT tableType, byte* srcBase)
        {
            switch (tableType)
            {
                case TableTypeT.ByPtr:
                    return ((byte**)tableBase)[h];
                case TableTypeT.ByU32:
                    return ((uint*)tableBase)[h] + srcBase;
                default:
                    return ((ushort*)tableBase)[h] + srcBase;
            }
        }

        private static readonly uint[] _inc32table = { 0, 1, 2, 1, 0, 4, 4, 4 };
        private static readonly int[] _dec64table = { 0, 0, 0, -1, -4, 1, 2, 3 };

        public static int DecompressGeneric(byte* src, byte* dst, int srcSize, int outputSize,
            EndConditionDirective endOnInput, EarlyEndDirective partialDecoding,
            int targetOutputSize, DictDirective dict, byte* lowPrefix, byte* dictStart, int dictSize)
        {
            byte* ip = src;
            byte* iend = ip + srcSize;

            byte* op = dst;
            byte* oend = op + outputSize;
            byte* oexit = op + targetOutputSize;

            byte* dictEnd = dictStart + dictSize;

            bool safeDecode = endOnInput == EndConditionDirective.EndOnInputSize;
            bool checkOffset = safeDecode && dictSize < 64 * KB;

            if ((partialDecoding != EarlyEndDirective.Full) && (oexit > oend - MFLimit))
                oexit = oend - MFLimit;
            if ((endOnInput == EndConditionDirective.EndOnInputSize) && (outputSize == 0))
                return srcSize == 1 && *ip == 0 ? 0 : -1;
            if ((endOnInput != EndConditionDirective.EndOnInputSize) && (outputSize == 0))
                return *ip == 0 ? 1 : -1;

            for (; ; )
            {
                int length;
                uint token = *ip++;

                if ((ip + 14 + 2 <= iend) &&
                    (op + 14 + 18 <= oend) &&
                    (token < 15 << MLBits) &&
                    ((token & MLMask) != 15))
                {
                    int ll = (int)(token >> MLBits);
                    int off = LZ4MemoryHelper.Peek16(ip + ll);
                    byte* matchPtr = op + ll - off;
                    if (off >= 18 && matchPtr >= lowPrefix)
                    {
                        int ml = (int)((token & MLMask) + MinMatch);
                        LZ4MemoryHelper.Copy16(op, ip);
                        op += ll;
                        ip += ll + 2;
                        LZ4MemoryHelper.Copy18(op, matchPtr);
                        op += ml;
                        continue;
                    }
                }

                if ((length = (int)(token >> MLBits)) == RunMask)
                {
                    uint s;
                    do
                    {
                        s = *ip++;
                        length += (int)s;
                    }
                    while (
                        (endOnInput != EndConditionDirective.EndOnInputSize || ip < iend - RunMask) &&
                        (s == 255));

                    if (safeDecode && op + length < op)
                        goto _output_error;
                    if (safeDecode && ip + length < ip)
                        goto _output_error;
                }

                byte* cpy = op + length;
                if ((endOnInput == EndConditionDirective.EndOnInputSize) &&
                    ((cpy > (partialDecoding == EarlyEndDirective.Partial ? oexit : oend - MFLimit)) ||
                        (ip + length > iend - (2 + 1 + LastLiterals))) ||
                    (endOnInput != EndConditionDirective.EndOnInputSize) && (cpy > oend - WildCopyLength))
                {
                    if (partialDecoding == EarlyEndDirective.Partial)
                    {
                        if (cpy > oend)
                            goto _output_error;
                        if ((endOnInput == EndConditionDirective.EndOnInputSize) && (ip + length > iend))
                            goto _output_error;
                    }
                    else
                    {
                        if ((endOnInput != EndConditionDirective.EndOnInputSize) && (cpy != oend))
                            goto _output_error;

                        if ((endOnInput == EndConditionDirective.EndOnInputSize) &&
                            (ip + length != iend || cpy > oend))
                        {
                            goto _output_error;
                        }
                    }

                    LZ4MemoryHelper.Copy(op, ip, length);
                    ip += length;
                    op += length;
                    break;
                }

                LZ4MemoryHelper.WildCopy(op, ip, cpy);
                ip += length;
                op = cpy;

                int offset = LZ4MemoryHelper.Peek16(ip);
                ip += 2;
                byte* match = op - offset;
                if (checkOffset && match + dictSize < lowPrefix)
                    goto _output_error;

                LZ4MemoryHelper.Poke32(op, (uint)offset);

                length = (int)(token & MLMask);
                if (length == MLMask)
                {
                    uint s;
                    do
                    {
                        s = *ip++;
                        if ((endOnInput == EndConditionDirective.EndOnInputSize) && (ip > iend - LastLiterals))
                            goto _output_error;

                        length += (int)s;
                    }
                    while (s == 255);

                    if (safeDecode && (op + length < op))
                        goto _output_error;
                }

                length += MinMatch;

                if ((dict == DictDirective.UsingExtDict) && (match < lowPrefix))
                {
                    if (op + length > oend - LastLiterals)
                        goto _output_error;

                    if (length <= lowPrefix - match)
                    {
                        LZ4MemoryHelper.Move(op, dictEnd - (lowPrefix - match), length);
                        op += length;
                    }
                    else
                    {
                        int copySize = (int)(lowPrefix - match);
                        int restSize = length - copySize;
                        LZ4MemoryHelper.Copy(op, dictEnd - copySize, copySize);
                        op += copySize;
                        if (restSize > (int)(op - lowPrefix))
                        {
                            byte* endOfMatch = op + restSize;
                            byte* copyFrom = lowPrefix;
                            while (op < endOfMatch)
                            {
                                *op++ = *copyFrom++;
                            }
                        }
                        else
                        {
                            LZ4MemoryHelper.Copy(op, lowPrefix, restSize);
                            op += restSize;
                        }
                    }

                    continue;
                }

                cpy = op + length;
                if (offset < 8)
                {
                    op[0] = match[0];
                    op[1] = match[1];
                    op[2] = match[2];
                    op[3] = match[3];
                    match += _inc32table[offset];
                    LZ4MemoryHelper.Copy(op + 4, match, 4);
                    match -= _dec64table[offset];
                }
                else
                {
                    LZ4MemoryHelper.Copy8(op, match);
                    match += 8;
                }

                op += 8;

                if (cpy > oend - 12)
                {
                    byte* oCopyLimit = oend - (WildCopyLength - 1);
                    if (cpy > oend - LastLiterals)
                        goto _output_error;

                    if (op < oCopyLimit)
                    {
                        LZ4MemoryHelper.WildCopy(op, match, oCopyLimit);
                        match += oCopyLimit - op;
                        op = oCopyLimit;
                    }

                    while (op < cpy)
                    {
                        *op++ = *match++;
                    }
                }
                else
                {
                    LZ4MemoryHelper.Copy8(op, match);
                    if (length > 16)
                        LZ4MemoryHelper.WildCopy(op + 8, match + 8, cpy);
                }

                op = cpy; // correction
            }

            // end of decoding
            if (endOnInput == EndConditionDirective.EndOnInputSize)
                return (int)(op - dst); // Nb of output bytes decoded

            return (int)(ip - src); // Nb of input bytes read

            // Overflow error detected
            _output_error:
            return (int)-(ip - src) - 1;
        }

        public static int DecompressSafe(byte* source, byte* dest, int compressedSize, int maxDecompressedSize)
        {
            return DecompressGeneric(
                source,
                dest,
                compressedSize,
                maxDecompressedSize,
                EndConditionDirective.EndOnInputSize,
                EarlyEndDirective.Full,
                0,
                DictDirective.NoDict,
                dest,
                null,
                0);
        }

        public static int DecompressSafePartial(byte* source, byte* dest, int compressedSize, int targetOutputSize, int maxDecompressedSize)
        {
            return DecompressGeneric(
                source,
                dest,
                compressedSize,
                maxDecompressedSize,
                EndConditionDirective.EndOnInputSize,
                EarlyEndDirective.Partial,
                targetOutputSize,
                DictDirective.NoDict,
                dest,
                null,
                0);
        }

        public static int DecompressFast(byte* source, byte* dest, int originalSize)
        {
            return DecompressGeneric(
                source,
                dest,
                0,
                originalSize,
                EndConditionDirective.EndOnOutputSize,
                EarlyEndDirective.Full,
                0,
                DictDirective.WithPrefix64k,
                dest - 64 * KB,
                null,
                64 * KB);
        }

        public static int DecompressSafeContinue(StreamDecodeT* lz4sd, byte* source, byte* dest, int compressedSize, int maxOutputSize)
        {
            int result;

            if (lz4sd->PrefixEnd == dest)
            {
                result = DecompressGeneric(
                    source,
                    dest,
                    compressedSize,
                    maxOutputSize,
                    EndConditionDirective.EndOnInputSize,
                    EarlyEndDirective.Full,
                    0,
                    DictDirective.UsingExtDict,
                    lz4sd->PrefixEnd - lz4sd->PrefixSize,
                    lz4sd->ExternalDict,
                    (int)lz4sd->ExtDictSize);

                if (result <= 0)
                    return result;

                lz4sd->PrefixSize += (uint)result;
                lz4sd->PrefixEnd += result;
            }
            else
            {
                lz4sd->ExtDictSize = lz4sd->PrefixSize;
                lz4sd->ExternalDict = lz4sd->PrefixEnd - lz4sd->ExtDictSize;

                result = DecompressGeneric(
                    source,
                    dest,
                    compressedSize,
                    maxOutputSize,
                    EndConditionDirective.EndOnInputSize,
                    EarlyEndDirective.Full,
                    0,
                    DictDirective.UsingExtDict,
                    dest,
                    lz4sd->ExternalDict,
                    (int)lz4sd->ExtDictSize);

                if (result <= 0)
                    return result;

                lz4sd->PrefixSize = (uint)result;
                lz4sd->PrefixEnd = dest + result;
            }

            return result;
        }

        public static int DecompressFastContinue(StreamDecodeT* lz4sd, byte* source, byte* dest, int originalSize)
        {
            int result;

            if (lz4sd->PrefixEnd == dest)
            {
                result = DecompressGeneric(
                    source,
                    dest,
                    0,
                    originalSize,
                    EndConditionDirective.EndOnOutputSize,
                    EarlyEndDirective.Full,
                    0,
                    DictDirective.UsingExtDict,
                    lz4sd->PrefixEnd - lz4sd->PrefixSize,
                    lz4sd->ExternalDict,
                    (int)lz4sd->ExtDictSize);

                if (result <= 0)
                    return result;

                lz4sd->PrefixSize += (uint)originalSize;
                lz4sd->PrefixEnd += originalSize;
            }
            else
            {
                lz4sd->ExtDictSize = lz4sd->PrefixSize;
                lz4sd->ExternalDict = lz4sd->PrefixEnd - lz4sd->ExtDictSize;

                result = DecompressGeneric(
                    source,
                    dest,
                    0,
                    originalSize,
                    EndConditionDirective.EndOnOutputSize,
                    EarlyEndDirective.Full,
                    0,
                    DictDirective.UsingExtDict,
                    dest,
                    lz4sd->ExternalDict,
                    (int)lz4sd->ExtDictSize);

                if (result <= 0)
                    return result;

                lz4sd->PrefixSize = (uint)originalSize;
                lz4sd->PrefixEnd = dest + originalSize;
            }

            return result;
        }

        public static int DecompressUsingDictGeneric(byte* source, byte* dest, int compressedSize, int maxOutputSize, int safe, byte* dictStart, int dictSize)
        {
            if (dictSize == 0)
            {
                return DecompressGeneric(
                    source,
                    dest,
                    compressedSize,
                    maxOutputSize,
                    (EndConditionDirective)safe,
                    EarlyEndDirective.Full,
                    0,
                    DictDirective.NoDict,
                    dest,
                    null,
                    0);
            }

            if (dictStart + dictSize != dest)
            {
                return DecompressGeneric(
                    source,
                    dest,
                    compressedSize,
                    maxOutputSize,
                    (EndConditionDirective)safe,
                    EarlyEndDirective.Full,
                    0,
                    DictDirective.UsingExtDict,
                    dest,
                    dictStart,
                    dictSize);
            }

            if (dictSize >= 64 * KB - 1)
            {
                return DecompressGeneric(
                    source,
                    dest,
                    compressedSize,
                    maxOutputSize,
                    (EndConditionDirective)safe,
                    EarlyEndDirective.Full,
                    0,
                    DictDirective.WithPrefix64k,
                    dest - 64 * KB,
                    null,
                    0);
            }

            return DecompressGeneric(
                source,
                dest,
                compressedSize,
                maxOutputSize,
                (EndConditionDirective)safe,
                EarlyEndDirective.Full,
                0,
                DictDirective.NoDict,
                dest - dictSize,
                null,
                0);
        }

        public static int DecompressSafeUsingDict(byte* source, byte* dest, int compressedSize, int maxOutputSize, byte* dictStart, int dictSize)
        {
            return DecompressUsingDictGeneric(
                source,
                dest,
                compressedSize,
                maxOutputSize,
                1,
                dictStart,
                dictSize);
        }

        public static int DecompressFastUsingDict(byte* source, byte* dest, int originalSize, byte* dictStart, int dictSize)
        {
            return DecompressUsingDictGeneric(source, dest, 0, originalSize, 0, dictStart, dictSize);
        }

        public static int DecompressSafeForceExtDict(byte* source, byte* dest, int compressedSize, int maxOutputSize, byte* dictStart, int dictSize)
        {
            return DecompressGeneric(
                source,
                dest,
                compressedSize,
                maxOutputSize,
                EndConditionDirective.EndOnInputSize,
                EarlyEndDirective.Full,
                0,
                DictDirective.UsingExtDict,
                dest,
                dictStart,
                dictSize);
        }

        public static void RenormDictT(StreamT* dict, byte* src)
        {
            if ((dict->CurrentOffset <= 0x80000000) && (dict->CurrentOffset <= (ulong)src))
                return;

            uint delta = dict->CurrentOffset - 64 * KB;
            byte* dictEnd = dict->Dictionary + dict->DictSize;
            for (int i = 0; i < HashSizeU32; i++)
            {
                if (dict->HashTable[i] < delta)
                    dict->HashTable[i] = 0;
                else
                    dict->HashTable[i] -= delta;
            }

            dict->CurrentOffset = 64 * KB;
            if (dict->DictSize > 64 * KB)
                dict->DictSize = 64 * KB;
            dict->Dictionary = dictEnd - dict->DictSize;
        }

        public static int SaveDict(StreamT* dict, byte* safeBuffer, int dictSize)
        {
            byte* previousDictEnd = dict->Dictionary + dict->DictSize;

            if ((uint)dictSize > 64 * KB)
                dictSize = 64 * KB;
            if ((uint)dictSize > dict->DictSize)
                dictSize = (int)dict->DictSize;

            LZ4MemoryHelper.Move(safeBuffer, previousDictEnd - dictSize, dictSize);

            dict->Dictionary = safeBuffer;
            dict->DictSize = (uint)dictSize;

            return dictSize;
        }

        public static void SetStreamDecode(StreamDecodeT* lz4sd, byte* dictionary, int dictSize)
        {
            lz4sd->PrefixSize = (uint)dictSize;
            lz4sd->PrefixEnd = dictionary + dictSize;
            lz4sd->ExternalDict = null;
            lz4sd->ExtDictSize = 0;
        }
    }
}
