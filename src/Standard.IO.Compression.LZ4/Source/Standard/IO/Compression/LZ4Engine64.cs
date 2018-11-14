using System.Runtime.CompilerServices;

namespace Standard.IO.Compression
{
#if BIT32
	internal unsafe class LZ4Engine32 : LZ4Engine
	{
		protected const int ArchSize = 4;
		protected const int StepSize = 4;
		protected const int HashUnit = 4;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected static uint ReadArch(void* p)
        {
            return *(uint*)p;
        }

		private static readonly uint[] DeBruijnBytePos = 
            {
			    0, 0, 3, 0, 3, 1, 3, 0,
			    3, 2, 2, 1, 3, 2, 0, 1,
			    3, 3, 1, 2, 2, 2, 2, 0,
			    3, 1, 2, 0, 1, 0, 1, 1
		    };

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected static uint NbCommonBytes(uint val)
        {
    		return DeBruijnBytePos[(uint)((int)val & -(int)val) * 0x077CB531U >> 27];
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static TableTypeT TableType(int inputSize)
        {
			return inputSize < Limit64k ? TableTypeT.ByU16 :
			    sizeof(byte*) == sizeof(uint) ? TableTypeT.ByPtr :
			    TableTypeT.ByU32;
        }
#else
    internal unsafe class LZ4Engine64 : LZ4Engine
    {
        protected const int ArchSize = 8;
        protected const int StepSize = 8;
        protected const int HashUnit = 8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static ulong ReadArch(void* p)
        {
            return *(ulong*)p;
        }

        private static readonly uint[] DeBruijnBytePos =
            {
                0, 0, 0, 0, 0, 1, 1, 2,
                0, 3, 1, 3, 1, 4, 2, 7,
                0, 2, 3, 6, 1, 5, 3, 5,
                1, 3, 4, 4, 2, 5, 6, 7,
                7, 0, 1, 2, 3, 3, 4, 6,
                2, 6, 5, 5, 3, 4, 5, 6,
                7, 1, 2, 4, 6, 4, 4, 5,
                7, 2, 6, 5, 7, 6, 7, 7
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static uint NbCommonBytes(ulong val)
        {
            return DeBruijnBytePos[(ulong)((long)val & -(long)val) * 0x0218A392CDABBD3Ful >> 58];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TableTypeT TableType(int inputSize)
        {
            return inputSize < Limit64k ? TableTypeT.ByU16 : TableTypeT.ByU32;
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static uint Count(byte* pIn, byte* pMatch, byte* pInLimit)
        {
            byte* pStart = pIn;

            if (pIn < pInLimit - (StepSize - 1))
            {
                ulong diff = ReadArch(pMatch) ^ ReadArch(pIn);
                if (diff != 0)
                    return NbCommonBytes(diff);

                pIn += StepSize;
                pMatch += StepSize;
            }

            while (pIn < pInLimit - (StepSize - 1))
            {
                ulong diff = ReadArch(pMatch) ^ ReadArch(pIn);
                if (diff != 0)
                    return (uint)(pIn + NbCommonBytes(diff) - pStart);

                pIn += StepSize;
                pMatch += StepSize;
            }

#if !BIT32
            if ((pIn < pInLimit - 3) && (LZ4MemoryHelper.Peek32(pMatch) == LZ4MemoryHelper.Peek32(pIn)))
            {
                pIn += 4;
                pMatch += 4;
            }
#endif

            if ((pIn < pInLimit - 1) && (LZ4MemoryHelper.Peek16(pMatch) == LZ4MemoryHelper.Peek16(pIn)))
            {
                pIn += 2;
                pMatch += 2;
            }

            if ((pIn < pInLimit) && (*pMatch == *pIn))
                pIn++;

            return (uint)(pIn - pStart);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint HashPosition(void* p, TableTypeT tableType)
        {
#if !BIT32
            if (tableType != TableTypeT.ByU16)
                return Hash5(ReadArch(p), tableType);
#endif
            return Hash4(LZ4MemoryHelper.Peek32(p), tableType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PutPosition(byte* p, void* tableBase, TableTypeT tableType, byte* srcBase)
        {
            PutPositionOnHash(p, HashPosition(p, tableType), tableBase, tableType, srcBase);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte* GetPosition(byte* p, void* tableBase, TableTypeT tableType, byte* srcBase)
        {
            return GetPositionOnHash(HashPosition(p, tableType), tableBase, tableType, srcBase);
        }

        public static int CompressGeneric(StreamT* cctx, byte* source, byte* dest, int inputSize, int maxOutputSize,
            LimitedOutputDirective outputLimited, TableTypeT tableType,
            DictDirective dict, DictIssueDirective dictIssue, uint acceleration)
        {
            byte* ip = source;
            byte* ibase;
            byte* lowLimit;
            byte* lowRefLimit = ip - cctx->DictSize;
            byte* dictionary = cctx->Dictionary;
            byte* dictEnd = dictionary + cctx->DictSize;
            long dictDelta = dictEnd - source;
            byte* anchor = source;
            byte* iend = ip + inputSize;
            byte* mflimit = iend - MFLimit;
            byte* matchlimit = iend - LastLiterals;

            byte* op = dest;
            byte* olimit = op + maxOutputSize;

            if (inputSize > MaxInputSize)
                return 0;

            switch (dict)
            {
                case DictDirective.WithPrefix64k:
                    ibase = source - cctx->CurrentOffset;
                    lowLimit = source - cctx->DictSize;
                    break;
                case DictDirective.UsingExtDict:
                    ibase = source - cctx->CurrentOffset;
                    lowLimit = source;
                    break;
                default:
                    ibase = source;
                    lowLimit = source;
                    break;
            }

            if ((tableType == TableTypeT.ByU16) && (inputSize >= Limit64k))
                return 0;

            if (inputSize < LZ4MinLength)
                goto _last_literals;

            PutPosition(ip, cctx->HashTable, tableType, ibase);
            ip++;
            uint forwardH = HashPosition(ip, tableType);

            for (; ; )
            {
                long refDelta = 0L;
                byte* match;
                byte* token;

                {
                    byte* forwardIp = ip;
                    uint step = 1u;
                    uint searchMatchNb = acceleration << SkipTrigger;
                    do
                    {
                        uint h = forwardH;
                        ip = forwardIp;
                        forwardIp += step;
                        step = searchMatchNb++ >> SkipTrigger;

                        if (forwardIp > mflimit)
                            goto _last_literals;

                        match = GetPositionOnHash(h, cctx->HashTable, tableType, ibase);
                        if (dict == DictDirective.UsingExtDict)
                        {
                            if (match < source)
                            {
                                refDelta = dictDelta;
                                lowLimit = dictionary;
                            }
                            else
                            {
                                refDelta = 0;
                                lowLimit = source;
                            }
                        }

                        forwardH = HashPosition(forwardIp, tableType);
                        PutPositionOnHash(ip, h, cctx->HashTable, tableType, ibase);
                    }
                    while ((dictIssue == DictIssueDirective.DictSmall) && (match < lowRefLimit) ||
                        (tableType != TableTypeT.ByU16) && (match + MaxDistance < ip) ||
                        (LZ4MemoryHelper.Peek32(match + refDelta) != LZ4MemoryHelper.Peek32(ip)));
                }

                while ((ip > anchor) && (match + refDelta > lowLimit) && (ip[-1] == match[refDelta - 1]))
                {
                    ip--;
                    match--;
                }

                {
                    uint litLength = (uint)(ip - anchor);
                    token = op++;
                    if ((outputLimited == LimitedOutputDirective.LimitedOutput) &&
                        (op + litLength + (2 + 1 + LastLiterals) + litLength / 255 > olimit))
                    {
                        return 0;
                    }

                    if (litLength >= RunMask)
                    {
                        int len = (int)(litLength - RunMask);

                        *token = (byte)(RunMask << MLBits);
                        for (; len >= 255; len -= 255)
                        {
                            *op++ = 255;
                        }

                        *op++ = (byte)len;
                    }
                    else
                    {
                        *token = (byte)(litLength << MLBits);
                    }

                    LZ4MemoryHelper.WildCopy(op, anchor, op + litLength);
                    op += litLength;
                }

                _next_match:
                LZ4MemoryHelper.Poke16(op, (ushort)(ip - match));
                op += 2;

                {
                    uint matchCode;

                    if ((dict == DictDirective.UsingExtDict) && (lowLimit == dictionary))
                    {
                        match += refDelta;
                        byte* limit = ip + (dictEnd - match);
                        if (limit > matchlimit)
                            limit = matchlimit;
                        matchCode = Count(ip + MinMatch, match + MinMatch, limit);
                        ip += MinMatch + matchCode;
                        if (ip == limit)
                        {
                            uint more = Count(ip, source, matchlimit);
                            matchCode += more;
                            ip += more;
                        }
                    }
                    else
                    {
                        matchCode = Count(ip + MinMatch, match + MinMatch, matchlimit);
                        ip += MinMatch + matchCode;
                    }

                    if ((outputLimited == LimitedOutputDirective.LimitedOutput) &&
                        (op + (1 + LastLiterals) + (matchCode >> 8) > olimit))
                    {
                        return 0;
                    }

                    if (matchCode >= MLMask)
                    {
                        *token += (byte)MLMask;
                        matchCode -= MLMask;
                        LZ4MemoryHelper.Poke32(op, 0xFFFFFFFF);
                        while (matchCode >= 4 * 255)
                        {
                            op += 4;
                            LZ4MemoryHelper.Poke32(op, 0xFFFFFFFF);
                            matchCode -= 4 * 255;
                        }

                        op += matchCode / 255;

                        *op++ = (byte)(matchCode % 255);
                    }
                    else
                    {
                        *token += (byte)matchCode;
                    }
                }

                anchor = ip;

                if (ip > mflimit)
                    break;

                PutPosition(ip - 2, cctx->HashTable, tableType, ibase);

                match = GetPosition(ip, cctx->HashTable, tableType, ibase);
                if (dict == DictDirective.UsingExtDict)
                {
                    if (match < source)
                    {
                        refDelta = dictDelta;
                        lowLimit = dictionary;
                    }
                    else
                    {
                        refDelta = 0;
                        lowLimit = source;
                    }
                }

                PutPosition(ip, cctx->HashTable, tableType, ibase);
                if ((dictIssue != DictIssueDirective.DictSmall || match >= lowRefLimit) &&
                    (match + MaxDistance >= ip) &&
                    (LZ4MemoryHelper.Peek32(match + refDelta) == LZ4MemoryHelper.Peek32(ip)))
                {
                    token = op++;
                    *token = 0;
                    goto _next_match;
                }

                forwardH = HashPosition(++ip, tableType);
            }

            _last_literals:
            {
                int lastRun = (int)(iend - anchor);
                if ((outputLimited == LimitedOutputDirective.LimitedOutput) &&
                    (op - dest + lastRun + 1 + (lastRun + 255 - RunMask) / 255 > (uint)maxOutputSize))
                {
                    return 0;
                }

                if (lastRun >= RunMask)
                {
                    int accumulator = (int)(lastRun - RunMask);

                    *op++ = (byte)(RunMask << MLBits);
                    for (; accumulator >= 255; accumulator -= 255)
                    {
                        *op++ = 255;
                    }
                    *op++ = (byte)accumulator;
                }
                else
                {
                    *op++ = (byte)(lastRun << MLBits);
                }

                LZ4MemoryHelper.Copy(op, anchor, lastRun);
                op += lastRun;
            }

            return (int)(op - dest);
        }

        public static int CompressFastExtState(StreamT* state, byte* source, byte* dest, int inputSize, int maxOutputSize, int acceleration)
        {
            ResetStream(state);
            if (acceleration < 1)
                acceleration = AccelerationDefault;

            LimitedOutputDirective limited = maxOutputSize >= CompressBound(inputSize)
                ? LimitedOutputDirective.NoLimit
                : LimitedOutputDirective.LimitedOutput;

            return CompressGeneric(
                state,
                source,
                dest,
                inputSize,
                limited == LimitedOutputDirective.NoLimit ? 0 : maxOutputSize,
                limited,
                TableType(inputSize),
                DictDirective.NoDict,
                DictIssueDirective.NoDictIssue,
                (uint)acceleration);
        }

        public static void ResetStream(StreamT* state)
        {
            LZ4MemoryHelper.Zero((byte*)state, sizeof(StreamT));
        }

        public static int CompressFast(byte* source, byte* dest, int inputSize, int maxOutputSize, int acceleration)
        {
            StreamT ctx;
            return CompressFastExtState(&ctx, source, dest, inputSize, maxOutputSize, acceleration);
        }

        public static int CompressDefault(byte* source, byte* dest, int inputSize, int maxOutputSize)
        {
            return CompressFast(source, dest, inputSize, maxOutputSize, 1);
        }

        private static int CompressDestSizeGeneric(StreamT* ctx, byte* src, byte* dst, int* srcSizePtr, int targetDstSize, TableTypeT tableType)
        {
            byte* ip = src;
            byte* srcBase = src;
            byte* lowLimit = src;
            byte* anchor = ip;
            byte* iend = ip + *srcSizePtr;
            byte* mflimit = iend - MFLimit;
            byte* matchlimit = iend - LastLiterals;

            byte* op = dst;
            byte* oend = op + targetDstSize;
            byte* oMaxLit = op + targetDstSize - 2 - 8 - 1;
            byte* oMaxMatch = op + targetDstSize - (LastLiterals + 1);
            byte* oMaxSeq = oMaxLit - 1;

            if (targetDstSize < 1)
                return 0;
            if (*srcSizePtr > MaxInputSize)
                return 0;
            if ((tableType == TableTypeT.ByU16) && (*srcSizePtr >= Limit64k))
                return 0;

            if (*srcSizePtr < LZ4MinLength)
                goto _last_literals; // Input too small, no compression (all literals)

            *srcSizePtr = 0;
            PutPosition(ip, ctx->HashTable, tableType, srcBase);
            ip++;
            uint forwardH = HashPosition(ip, tableType);

            for (; ; )
            {
                byte* match;
                byte* token;

                {
                    byte* forwardIp = ip;
                    uint step = 1u;
                    uint searchMatchNb = 1u << SkipTrigger;

                    do
                    {
                        uint h = forwardH;
                        ip = forwardIp;
                        forwardIp += step;
                        step = searchMatchNb++ >> SkipTrigger;

                        if (forwardIp > mflimit)
                            goto _last_literals;

                        match = GetPositionOnHash(h, ctx->HashTable, tableType, srcBase);
                        forwardH = HashPosition(forwardIp, tableType);
                        PutPositionOnHash(ip, h, ctx->HashTable, tableType, srcBase);
                    }
                    while ((tableType != TableTypeT.ByU16) && (match + MaxDistance < ip) ||
                        (LZ4MemoryHelper.Peek32(match) != LZ4MemoryHelper.Peek32(ip)));
                }

                while ((ip > anchor) && (match > lowLimit) && (ip[-1] == match[-1]))
                {
                    ip--;
                    match--;
                }

                {
                    uint litLength = (uint)(ip - anchor);
                    token = op++;
                    if (op + (litLength + 240) / 255 + litLength > oMaxLit)
                    {
                        op--;
                        goto _last_literals;
                    }

                    if (litLength >= RunMask)
                    {
                        uint len = litLength - RunMask;

                        *token = (byte)(RunMask << MLBits);
                        for (; len >= 255; len -= 255)
                        {
                            *op++ = 255;
                        }

                        *op++ = (byte)len;
                    }
                    else
                    {
                        *token = (byte)(litLength << MLBits);
                    }

                    LZ4MemoryHelper.WildCopy(op, anchor, op + litLength);
                    op += litLength;
                }

                _next_match:
                LZ4MemoryHelper.Poke16(op, (ushort)(ip - match));
                op += 2;

                {
                    int matchLength = (int)Count(ip + MinMatch, match + MinMatch, matchlimit);

                    if (op + (matchLength + 240) / 255 > oMaxMatch)
                    {
                        matchLength = (int)(15 - 1 + (oMaxMatch - op) * 255);
                    }

                    ip += MinMatch + matchLength;

                    if (matchLength >= MLMask)
                    {
                        *token += (byte)MLMask;
                        matchLength -= (int)MLMask;
                        while (matchLength >= 255)
                        {
                            matchLength -= 255;
                            *op++ = 255;
                        }

                        *op++ = (byte)matchLength;
                    }
                    else
                    {
                        *token += (byte)matchLength;
                    }
                }

                anchor = ip;

                if (ip > mflimit)
                    break;
                if (op > oMaxSeq)
                    break;

                PutPosition(ip - 2, ctx->HashTable, tableType, srcBase);

                match = GetPosition(ip, ctx->HashTable, tableType, srcBase);
                PutPosition(ip, ctx->HashTable, tableType, srcBase);
                if ((match + MaxDistance >= ip) && (LZ4MemoryHelper.Peek32(match) == LZ4MemoryHelper.Peek32(ip)))
                {
                    token = op++;
                    *token = 0;
                    goto _next_match;
                }

                forwardH = HashPosition(++ip, tableType);
            }

            _last_literals:
            {
                int lastRunSize = (int)(iend - anchor);
                if (op + 1 + (lastRunSize + 240) / 255 + lastRunSize > oend)
                {
                    lastRunSize = (int)(oend - op) - 1;
                    lastRunSize -= (lastRunSize + 240) / 255;
                }

                ip = anchor + lastRunSize;

                if (lastRunSize >= RunMask)
                {
                    long accumulator = lastRunSize - RunMask;

                    *op++ = (byte)(RunMask << MLBits);
                    for (; accumulator >= 255; accumulator -= 255)
                    {
                        *op++ = 255;
                    }

                    *op++ = (byte)accumulator;
                }
                else
                {
                    *op++ = (byte)(lastRunSize << MLBits);
                }

                LZ4MemoryHelper.Copy(op, anchor, lastRunSize);
                op += lastRunSize;
            }

            *srcSizePtr = (int)(ip - src);
            return (int)(op - dst);
        }

        public static int CompressDestSizeExtState(StreamT* state, byte* src, byte* dst, int* srcSizePtr, int targetDstSize)
        {
            ResetStream(state);
            return targetDstSize >= CompressBound(*srcSizePtr)
                ? CompressFastExtState(
                    state,
                    src,
                    dst,
                    *srcSizePtr,
                    targetDstSize,
                    1)
                : CompressDestSizeGeneric(
                    state,
                    src,
                    dst,
                    srcSizePtr,
                    targetDstSize,
                    TableType(*srcSizePtr));
        }

        public static int CompressDestSize(byte* src, byte* dst, int* srcSizePtr, int targetDstSize)
        {
            StreamT ctxBody;
            return CompressDestSizeExtState(&ctxBody, src, dst, srcSizePtr, targetDstSize);
        }

        //--------------------------------------------------------------------

        public static int LoadDict(StreamT* lz4dict, byte* dictionary, int dictSize)
        {
            StreamT* dict = lz4dict;
            byte* p = dictionary;
            byte* dictEnd = p + dictSize;

            if (dict->InitCheck != 0 || dict->CurrentOffset > 1 * GB)
                ResetStream(lz4dict);

            if (dictSize < HashUnit)
            {
                dict->Dictionary = null;
                dict->DictSize = 0;
                return 0;
            }

            if (dictEnd - p > 64 * KB)
                p = dictEnd - 64 * KB;
            dict->CurrentOffset += 64 * KB;
            byte* srcBase = p - dict->CurrentOffset;
            dict->Dictionary = p;
            dict->DictSize = (uint)(dictEnd - p);
            dict->CurrentOffset += dict->DictSize;

            while (p <= dictEnd - HashUnit)
            {
                PutPosition(p, dict->HashTable, TableTypeT.ByU32, srcBase);
                p += 3;
            }

            return (int)dict->DictSize;
        }

        public static int CompressFastContinue(StreamT* streamPtr, byte* source, byte* dest, int inputSize, int maxOutputSize, int acceleration)
        {
            byte* dictEnd = streamPtr->Dictionary + streamPtr->DictSize;

            byte* smallest = source;
            if (streamPtr->InitCheck != 0)
                return 0; // Uninitialized structure detected

            if (streamPtr->DictSize > 0 && smallest > dictEnd)
                smallest = dictEnd;

            RenormDictT(streamPtr, smallest);

            if (acceleration < 1)
                acceleration = AccelerationDefault;

            // Check overlapping input/dictionary space
            {
                byte* sourceEnd = source + inputSize;
                if (sourceEnd > streamPtr->Dictionary && sourceEnd < dictEnd)
                {
                    streamPtr->DictSize = (uint)(dictEnd - sourceEnd);
                    if (streamPtr->DictSize > 64 * KB)
                        streamPtr->DictSize = 64 * KB;

                    if (streamPtr->DictSize < 4)
                        streamPtr->DictSize = 0;

                    streamPtr->Dictionary = dictEnd - streamPtr->DictSize;
                }
            }

            DictIssueDirective dictIssue = (streamPtr->DictSize < 64 * KB) && (streamPtr->DictSize < streamPtr->CurrentOffset)
                ? DictIssueDirective.DictSmall
                : DictIssueDirective.NoDictIssue;

            DictDirective dictMode = dictEnd == source
                ? DictDirective.WithPrefix64k
                : DictDirective.UsingExtDict;

            int result = CompressGeneric(
                streamPtr,
                source,
                dest,
                inputSize,
                maxOutputSize,
                LimitedOutputDirective.LimitedOutput,
                TableTypeT.ByU32,
                dictMode,
                dictIssue,
                (uint)acceleration);

            if (dictMode == DictDirective.WithPrefix64k)
            {
                // prefix mode : source data follows dictionary
                streamPtr->DictSize += (uint)inputSize;
                streamPtr->CurrentOffset += (uint)inputSize;
            }
            else
            {
                // external dictionary mode
                streamPtr->Dictionary = source;
                streamPtr->DictSize = (uint)inputSize;
                streamPtr->CurrentOffset += (uint)inputSize;
            }

            return result;
        }
    }
}
