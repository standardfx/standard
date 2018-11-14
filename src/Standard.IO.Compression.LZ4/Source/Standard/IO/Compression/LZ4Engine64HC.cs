using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if BIT32
using size_t = System.UInt32;
using reg_t = System.UInt32;
#else
using size_t = System.UInt64;
#endif

namespace Standard.IO.Compression
{
#if BIT32
	internal unsafe class LZ4Engine64HC: LZ4_32
	{
#else
    internal unsafe class LZ4Engine64HC : LZ4Engine64
    {
#endif
        private const int CLevelMin = 3;
        private const int CLevelDefault = 9;
        private const int CLevelOptMin = 10;
        private const int CLevelMax = 12;

        private const int DictionaryLogSize = 16;
        private const int MaxD = 1 << DictionaryLogSize;
        private const int MaxDMask = MaxD - 1;

        private const int HashLogHC = 15;
        private const int HashTableSizeHC = 1 << HashLogHC;
        private const int HashMask = HashTableSizeHC - 1;

        private const int OptimalML = (int)(MLMask - 1 + MinMatch);

        private const int OptNum = (1 << 12);

        [StructLayout(LayoutKind.Sequential)]
        public struct CCtxT // LZ4_streamHC_u
        {
            public fixed uint HashTable[HashTableSizeHC];
            public fixed ushort ChainTable[MaxD];
            // next block here to continue on current prefix
            public byte* End;
            // All index relative to this position
            public byte* BaseP;
            // alternate base for extDict
            public byte* DictBase;
            // deprecated
            public byte* InputBuffer;
            // below that point, need extDict
            public uint DictLimit;
            // below that point, no more dict
            public uint LowLimit;
            // index from which to continue dictionary update
            public uint NextToUpdate;
            public int CompressionLevel;
        }

        private struct OptimalT
        {
            public int Price;
            public int Off;
            public int Mlen;
            public int Litlen;
        }

        private struct MatchT
        {
            public int Off;
            public int Len;
        }

        private enum RepeatStateE
        {
            RepUntested,
            RepNot,
            RepConfirmed
        };

        private enum StratE
        {
            HC,
            Opt
        };

        private struct CParams
        {
            public readonly StratE Strat;
            public readonly System.UInt32 NbSearches;
            public readonly System.UInt32 TargetLength;

            public CParams(StratE strat, System.UInt32 nbSearches, System.UInt32 targetLength)
            {
                this.Strat = strat;
                this.NbSearches = nbSearches;
                this.TargetLength = targetLength;
            }
        }

        private static CParams[] ClTable =
            {
                new CParams(StratE.HC, 2, 16),   // 0, unused
			    new CParams(StratE.HC, 2, 16),   // 1, unused
			    new CParams(StratE.HC, 2, 16),   // 2, unused
			    new CParams(StratE.HC, 4, 16),   // 3
			    new CParams(StratE.HC, 8, 16),   // 4
			    new CParams(StratE.HC, 16, 16),  // 5
			    new CParams(StratE.HC, 32, 16),  // 6
			    new CParams(StratE.HC, 64, 16),  // 7
			    new CParams(StratE.HC, 128, 16), // 8
			    new CParams(StratE.HC, 256, 16), // 9
			    new CParams(StratE.Opt, 96, 64),       // 10==LZ4HC_CLEVEL_OPT_MIN
			    new CParams(StratE.Opt, 512, 128),     // 11
			    new CParams(StratE.Opt, 8192, OptNum), // 12==LZ4HC_CLEVEL_MAX
		    };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort DeltaNextU16(ushort* table, ushort pos)
        {
            return table[pos];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DeltaNextU16(ushort* table, ushort pos, ushort value)
        {
            table[pos] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint HashPtr(void* ptr)
        {
            return (LZ4MemoryHelper.Peek32(ptr) * 2654435761U) >> (MinMatch * 8 - HashLogHC);
        }

        public static void InitializeHC(CCtxT* hc4, byte* start)
        {
            LZ4MemoryHelper.Zero((byte*)hc4->HashTable, HashTableSizeHC * sizeof(uint));
            LZ4MemoryHelper.Fill((byte*)hc4->ChainTable, 0xFF, MaxD * sizeof(ushort));
            hc4->NextToUpdate = 64 * KB;
            hc4->BaseP = start - 64 * KB;
            hc4->End = start;
            hc4->DictBase = start - 64 * KB;
            hc4->DictLimit = 64 * KB;
            hc4->LowLimit = 64 * KB;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Insert(CCtxT* hc4, byte* ip)
        {
            ushort* chainTable = hc4->ChainTable;
            uint* hashTable = hc4->HashTable;
            byte* basep = hc4->BaseP;
            uint target = (uint)(ip - basep);
            uint idx = hc4->NextToUpdate;

            while (idx < target)
            {
                uint h = HashPtr(basep + idx);
                uint delta = idx - hashTable[h];
                if (delta > MaxDistance)
                    delta = MaxDistance;
                DeltaNextU16(chainTable, (ushort)idx, (ushort)delta);
                hashTable[h] = idx;
                idx++;
            }

            hc4->NextToUpdate = target;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CountBack(byte* ip, byte* match, byte* iMin, byte* mMin)
        {
            int back = 0;
            while ((ip + back > iMin) &&
                (match + back > mMin) &&
                (ip[back - 1] == match[back - 1]))
            {
                back--;
            }
            return back;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint CountPattern(byte* ip, byte* iEnd, uint pattern32)
        {
            byte* iStart = ip;
#if BIT32
			size_t pattern = pattern32;
#else
            size_t pattern = pattern32 | ((ulong)pattern32 << 32);
#endif

            while (ip < iEnd - (StepSize - 1))
            {
                size_t diff = ReadArch(ip) ^ pattern;
                if (diff != 0)
                {
                    ip += NbCommonBytes(diff);
                    return (uint)(ip - iStart);
                }

                ip += StepSize;
            }

            size_t patternByte = pattern;
            while ((ip < iEnd) && (*ip == (byte)patternByte))
            {
                ip++;
                patternByte >>= 8;
            }

            return (uint)(ip - iStart);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ReverseCountPattern(byte* ip, byte* iLow, uint pattern)
        {
            byte* iStart = ip;

            while (ip >= iLow + 4)
            {
                if (LZ4MemoryHelper.Peek32(ip - 4) != pattern)
                    break;

                ip -= 4;
            }

            {
                // works for any endianess
                byte* bytePtr = (byte*)&pattern + 3;
                while (ip > iLow)
                {
                    if (ip[-1] != *bytePtr)
                        break;

                    ip--;
                    bytePtr--;
                }
            }

            return (uint)(iStart - ip);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InsertAndGetWiderMatch(CCtxT* hc4, byte* ip, byte* iLowLimit, byte* iHighLimit, int longest,
            byte** matchpos, byte** startpos, int maxNbAttempts, int patternAnalysis)
        {
            ushort* chainTable = hc4->ChainTable;
            uint* hashTable = hc4->HashTable;
            byte* basep = hc4->BaseP;
            uint dictLimit = hc4->DictLimit;
            byte* lowPrefixPtr = basep + dictLimit;
            uint lowLimit = hc4->LowLimit + 64 * KB > (uint)(ip - basep)
                ? hc4->LowLimit
                : (uint)(ip - basep) - MaxDistance;
            byte* dictBase = hc4->DictBase;
            int delta = (int)(ip - iLowLimit);
            int nbAttempts = maxNbAttempts;
            uint pattern = LZ4MemoryHelper.Peek32(ip);
            RepeatStateE repeat = RepeatStateE.RepUntested;
            int srcPatternLength = 0;

            // First Match
            Insert(hc4, ip);
            uint matchIndex = hashTable[HashPtr(ip)];

            while ((matchIndex >= lowLimit) && (nbAttempts != 0))
            {
                nbAttempts--;
                if (matchIndex >= dictLimit)
                {
                    byte* matchPtr = basep + matchIndex;
                    if (*(iLowLimit + longest) == *(matchPtr - delta + longest))
                    {
                        if (LZ4MemoryHelper.Peek32(matchPtr) == pattern)
                        {
                            int mlt = MinMatch + (int)Count(ip + MinMatch, matchPtr + MinMatch, iHighLimit);
                            int back = 0;
                            while ((ip + back > iLowLimit) &&
                                (matchPtr + back > lowPrefixPtr) &&
                                (ip[back - 1] == matchPtr[back - 1]))
                            {
                                back--;
                            }

                            mlt -= back;

                            if (mlt > longest)
                            {
                                longest = mlt;
                                *matchpos = matchPtr + back;
                                *startpos = ip + back;
                            }
                        }
                    }
                }
                else
                {
                    byte* matchPtr = dictBase + matchIndex;
                    if (LZ4MemoryHelper.Peek32(matchPtr) == pattern)
                    {
                        int back = 0;
                        byte* vLimit = ip + (dictLimit - matchIndex);
                        if (vLimit > iHighLimit)
                            vLimit = iHighLimit;
                        int mlt = MinMatch + (int)Count(ip + MinMatch, matchPtr + MinMatch, vLimit);
                        if ((ip + mlt == vLimit) && (vLimit < iHighLimit))
                            mlt += (int)Count(ip + mlt, basep + dictLimit, iHighLimit);

                        while ((ip + back > iLowLimit) &&
                            (matchIndex + back > lowLimit) &&
                            (ip[back - 1] == matchPtr[back - 1]))
                        {
                            back--;
                        }

                        mlt -= back;
                        if (mlt > longest)
                        {
                            longest = mlt;
                            *matchpos = basep + matchIndex + back;
                            *startpos = ip + back;
                        }
                    }
                }

                {
                    ushort nextOffset = DeltaNextU16(chainTable, (ushort)matchIndex);
                    matchIndex -= nextOffset;
                    if ((patternAnalysis != 0) && (nextOffset == 1))
                    {
                        // may be a repeated pattern
                        if (repeat == RepeatStateE.RepUntested)
                        {
                            if (((pattern & 0xFFFF) == pattern >> 16) &
                                ((pattern & 0xFF) == pattern >> 24))
                            {
                                repeat = RepeatStateE.RepConfirmed;
                                srcPatternLength = (int)CountPattern(ip + 4, iHighLimit, pattern) + 4;
                            }
                            else
                            {
                                repeat = RepeatStateE.RepNot;
                            }
                        }

                        if ((repeat == RepeatStateE.RepConfirmed) && (matchIndex >= dictLimit))
                        {
                            byte* matchPtr = basep + matchIndex;
                            if (LZ4MemoryHelper.Peek32(matchPtr) == pattern)
                            {
                                int forwardPatternLength = (int)CountPattern(matchPtr + sizeof(uint), iHighLimit, pattern) + sizeof(uint);
                                byte* maxLowPtr = lowPrefixPtr + MaxDistance >= ip
                                    ? lowPrefixPtr
                                    : ip - MaxDistance;
                                int backLength = (int)ReverseCountPattern(matchPtr, maxLowPtr, pattern);
                                int currentSegmentLength = backLength + forwardPatternLength;

                                if ((currentSegmentLength >= srcPatternLength) &&
                                    (forwardPatternLength <= srcPatternLength))
                                {
                                    matchIndex += (uint)(forwardPatternLength - srcPatternLength);
                                }
                                else
                                {
                                    matchIndex -= (uint)backLength;
                                }
                            }
                        }
                    }
                }
            }

            return longest;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InsertAndFindBestMatch(CCtxT* hc4, byte* ip, byte* iLimit, byte** matchpos, int maxNbAttempts, int patternAnalysis)
        {
            byte* uselessPtr = ip;
            return InsertAndGetWiderMatch(
                hc4,
                ip,
                ip,
                iLimit,
                MinMatch - 1,
                matchpos,
                &uselessPtr,
                maxNbAttempts,
                patternAnalysis);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int EncodeSequence(byte** ip, byte** op, byte** anchor, int matchLength, byte* match, LimitedOutputDirective limit, byte* oend)
        {
            byte* token = (*op)++;

            size_t length = (size_t)(*ip - *anchor);
            if ((limit != LimitedOutputDirective.NoLimit) &&
                (*op + (length >> 8) + length + (2 + 1 + LastLiterals) > oend))
            {
                return 1;
            }

            if (length >= RunMask)
            {
                size_t len = length - RunMask;
                *token = (byte)(RunMask << MLBits);
                for (; len >= 255; len -= 255)
                {
                    *(*op)++ = 255;
                }
                *(*op)++ = (byte)len;
            }
            else
            {
                *token = (byte)(length << MLBits);
            }

            LZ4MemoryHelper.WildCopy(*op, *anchor, (*op) + length);

            *op += length;
            LZ4MemoryHelper.Poke16(*op, (ushort)(*ip - match));
            *op += 2;

            length = (size_t)(matchLength - MinMatch);
            if ((limit != LimitedOutputDirective.NoLimit) &&
                (*op + (length >> 8) + (1 + LastLiterals) > oend))
            {
                return 1;
            }

            if (length >= MLMask)
            {
                *token += (byte)MLMask;
                length -= MLMask;
                for (; length >= 510; length -= 510)
                {
                    *(*op)++ = 255;
                    *(*op)++ = 255;
                }

                if (length >= 255)
                {
                    length -= 255;
                    *(*op)++ = 255;
                }

                *(*op)++ = (byte)length;
            }
            else
            {
                *token += (byte)(length);
            }

            *ip += matchLength;
            *anchor = *ip;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LiteralsPrice(int litlen)
        {
            return litlen >= (int)RunMask
                ? litlen + (int)(1 + (litlen - RunMask) / 255)
                : litlen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SequencePrice(int litlen, int mlen)
        {
            int price = 1 + 2 + LiteralsPrice(litlen);
            return mlen >= (int)(MLMask + MinMatch)
                ? price + (int)(1 + (mlen - (MLMask + MinMatch)) / 255)
                : price;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static MatchT FindLongerMatch(CCtxT* ctx, byte* ip, byte* iHighLimit, int minLen, int nbSearches)
        {
            MatchT match;
            LZ4MemoryHelper.Zero((byte*)&match, sizeof(MatchT));
            byte* matchPtr = null;
            int matchLength = InsertAndGetWiderMatch(ctx, ip, ip, iHighLimit, minLen, &matchPtr, &ip, nbSearches, 1);
            if (matchLength <= minLen)
                return match;

            match.Len = matchLength;
            match.Off = (int)(ip - matchPtr);
            return match;
        }

        // -----------------------------------------------------------------

        private static int CompressOptimal(CCtxT* ctx, byte* source, byte* dst, int* srcSizePtr, int dstCapacity,
            int nbSearches, size_t sufficientLen, LimitedOutputDirective limit, int fullUpdate)
        {
            const int TrailingLiterals = 3;

            OptimalT* opt = stackalloc OptimalT[OptNum + TrailingLiterals];

            byte* ip = source;
            byte* anchor = ip;
            byte* iend = ip + *srcSizePtr;
            byte* mflimit = iend - MFLimit;
            byte* matchlimit = iend - LastLiterals;
            byte* op = dst;
            byte* opSaved;
            byte* oend = op + dstCapacity;

            *srcSizePtr = 0;
            if (limit == LimitedOutputDirective.LimitedDestSize)
                oend -= LastLiterals;
            if (sufficientLen >= OptNum)
                sufficientLen = OptNum - 1;

            // Main Loop
            while (ip < mflimit)
            {
                int llen = (int)(ip - anchor);
                int bestMlen, bestOff;
                int cur, lastMatchPos;

                MatchT firstMatch = FindLongerMatch(ctx, ip, matchlimit, MinMatch - 1, nbSearches);
                if (firstMatch.Len == 0)
                {
                    ip++;
                    continue;
                }

                if ((size_t)firstMatch.Len > sufficientLen)
                {
                    int firstML = firstMatch.Len;
                    byte* matchPos = ip - firstMatch.Off;
                    opSaved = op;
                    if (EncodeSequence(&ip, &op, &anchor, firstML, matchPos, limit, oend) != 0)
                        goto _dest_overflow;

                    continue;
                }

                // set prices for first positions (literals)
                {
                    int rPos;
                    for (rPos = 0; rPos < MinMatch; rPos++)
                    {
                        int cost = LiteralsPrice(llen + rPos);
                        opt[rPos].Mlen = 1;
                        opt[rPos].Off = 0;
                        opt[rPos].Litlen = llen + rPos;
                        opt[rPos].Price = cost;
                    }
                }
                // set prices using initial match
                {
                    int mlen = MinMatch;
                    int matchML = firstMatch.Len; // necessarily < sufficient_len < LZ4_OPT_NUM
                    int offset = firstMatch.Off;
                    for (; mlen <= matchML; mlen++)
                    {
                        int cost = SequencePrice(llen, mlen);
                        opt[mlen].Mlen = mlen;
                        opt[mlen].Off = offset;
                        opt[mlen].Litlen = llen;
                        opt[mlen].Price = cost;
                    }
                }
                lastMatchPos = firstMatch.Len;
                {
                    int addLit;
                    for (addLit = 1; addLit <= TrailingLiterals; addLit++)
                    {
                        opt[lastMatchPos + addLit].Mlen = 1;
                        opt[lastMatchPos + addLit].Off = 0;
                        opt[lastMatchPos + addLit].Litlen = addLit;
                        opt[lastMatchPos + addLit].Price = opt[lastMatchPos].Price + LiteralsPrice(addLit);
                    }
                }

                // check further positions
                for (cur = 1; cur < lastMatchPos; cur++)
                {
                    byte* curPtr = ip + cur;

                    if (curPtr >= mflimit)
                        break;

                    if (fullUpdate != 0)
                    {
                        if ((opt[cur + 1].Price <= opt[cur].Price) &&
                            (opt[cur + MinMatch].Price < opt[cur].Price + 3))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (opt[cur + 1].Price <= opt[cur].Price)
                            continue;
                    }

                    MatchT newMatch = FindLongerMatch(
                        ctx,
                        curPtr,
                        matchlimit,
                        fullUpdate != 0 ? MinMatch - 1 : lastMatchPos - cur,
                        nbSearches);

                    if (newMatch.Len == 0)
                        continue;

                    if ((size_t)newMatch.Len > sufficientLen || newMatch.Len + cur >= OptNum)
                    {
                        // immediate encoding
                        bestMlen = newMatch.Len;
                        bestOff = newMatch.Off;
                        lastMatchPos = cur + 1;
                        goto encode;
                    }

                    // before match : set price with literals at beginning
                    {
                        int baseLitlen = opt[cur].Litlen;
                        for (int litlen = 1; litlen < MinMatch; litlen++)
                        {
                            int price = opt[cur].Price - LiteralsPrice(baseLitlen) + LiteralsPrice(baseLitlen + litlen);
                            int pos = cur + litlen;
                            if (price >= opt[pos].Price)
                                continue;

                            opt[pos].Mlen = 1;
                            opt[pos].Off = 0;
                            opt[pos].Litlen = baseLitlen + litlen;
                            opt[pos].Price = price;
                        }
                    }

                    // set prices using match at position = cur
                    {
                        int matchML = newMatch.Len;
                        int ml = MinMatch;

                        for (; ml <= matchML; ml++)
                        {
                            int pos = cur + ml;
                            int offset = newMatch.Off;
                            int price;
                            int ll;
                            if (opt[cur].Mlen == 1)
                            {
                                ll = opt[cur].Litlen;
                                price = ((cur > ll) ? opt[cur - ll].Price : 0) + SequencePrice(ll, ml);
                            }
                            else
                            {
                                ll = 0;
                                price = opt[cur].Price + SequencePrice(0, ml);
                            }

                            if ((pos > lastMatchPos + TrailingLiterals) || (price <= opt[pos].Price))
                            {
                                if (ml == matchML && lastMatchPos < pos)
                                    lastMatchPos = pos;
                                opt[pos].Mlen = ml;
                                opt[pos].Off = offset;
                                opt[pos].Litlen = ll;
                                opt[pos].Price = price;
                            }
                        }
                    }

                    {
                        int addLit;
                        for (addLit = 1; addLit <= TrailingLiterals; addLit++)
                        {
                            opt[lastMatchPos + addLit].Mlen = 1;
                            opt[lastMatchPos + addLit].Off = 0;
                            opt[lastMatchPos + addLit].Litlen = addLit;
                            opt[lastMatchPos + addLit].Price = opt[lastMatchPos].Price + LiteralsPrice(addLit);
                        }
                    }
                }

                bestMlen = opt[lastMatchPos].Mlen;
                bestOff = opt[lastMatchPos].Off;
                cur = lastMatchPos - bestMlen;

                encode: // cur, last_match_pos, best_mlen, best_off must be set
                {
                    int candidate_pos = cur;
                    int selected_matchLength = bestMlen;
                    int selected_offset = bestOff;
                    while (true)
                    {
                        int next_matchLength = opt[candidate_pos].Mlen;
                        int next_offset = opt[candidate_pos].Off;
                        opt[candidate_pos].Mlen = selected_matchLength;
                        opt[candidate_pos].Off = selected_offset;
                        selected_matchLength = next_matchLength;
                        selected_offset = next_offset;
                        if (next_matchLength > candidate_pos)
                            break;

                        candidate_pos -= next_matchLength;
                    }
                }

                {
                    int rPos = 0;
                    while (rPos < lastMatchPos)
                    {
                        int ml = opt[rPos].Mlen;
                        int offset = opt[rPos].Off;
                        if (ml == 1)
                        {
                            ip++;
                            rPos++;
                            continue;
                        }

                        rPos += ml;
                        opSaved = op;
                        if (EncodeSequence(&ip, &op, &anchor, ml, ip - offset, limit, oend) != 0)
                            goto _dest_overflow;
                    }
                }
            }

            _last_literals:
            {
                size_t lastRunSize = (size_t)(iend - anchor);
                size_t litLength = (lastRunSize + 255 - RunMask) / 255;
                size_t totalSize = 1 + litLength + lastRunSize;
                if (limit == LimitedOutputDirective.LimitedDestSize)
                    oend += LastLiterals;
                if (limit != 0 && op + totalSize > oend)
                {
                    if (limit == LimitedOutputDirective.LimitedOutput)
                        return 0;

                    lastRunSize = (size_t)(oend - op) - 1;
                    litLength = (lastRunSize + 255 - RunMask) / 255;
                    lastRunSize -= litLength;
                }

                ip = anchor + lastRunSize;

                if (lastRunSize >= RunMask)
                {
                    size_t accumulator = lastRunSize - RunMask;
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

                LZ4MemoryHelper.Copy(op, anchor, (int)lastRunSize);
                op += lastRunSize;
            }

            // End
            *srcSizePtr = (int)(ip - source);
            return (int)(op - dst);

            _dest_overflow:
            if (limit != LimitedOutputDirective.LimitedDestSize)
                return 0;

            op = opSaved; // restore correct out pointer
            goto _last_literals;
        }

        private static int CompressHashChain(CCtxT* ctx, byte* source, byte* dest, int* srcSizePtr, int maxOutputSize, uint maxNbAttempts, LimitedOutputDirective limit)
        {
            int inputSize = *srcSizePtr;
            int patternAnalysis = maxNbAttempts > 64 ? 1 : 0; // levels 8+

            byte* ip = source;
            byte* anchor = ip;
            byte* iend = ip + inputSize;
            byte* mflimit = iend - MFLimit;
            byte* matchlimit = (iend - LastLiterals);

            byte* optr;
            byte* op = dest;
            byte* oend = op + maxOutputSize;

            byte* refPos = null;
            byte* start2 = null;
            byte* ref2 = null;
            byte* start3 = null;
            byte* ref3 = null;

            // init
            *srcSizePtr = 0;
            if (limit == LimitedOutputDirective.LimitedDestSize)
                oend -= LastLiterals; // Hack for support LZ4 format restriction
            if (inputSize < LZ4MinLength)
                goto _last_literals;  // Input too small, no compression (all literals)

            // Main Loop
            while (ip < mflimit)
            {
                int ml = InsertAndFindBestMatch(
                    ctx,
                    ip,
                    matchlimit,
                    &refPos,
                    (int)maxNbAttempts,
                    patternAnalysis);

                if (ml < MinMatch)
                {
                    ip++;
                    continue;
                }

                // saved, in case we would skip too much
                byte* start0 = ip;
                byte* ref0 = refPos;
                int ml0 = ml;

                _Search2:
                int ml2;
                if (ip + ml < mflimit)
                {
                    ml2 = InsertAndGetWiderMatch(
                        ctx,
                        ip + ml - 2,
                        ip + 0,
                        matchlimit,
                        ml,
                        &ref2,
                        &start2,
                        (int)maxNbAttempts,
                        patternAnalysis);
                }
                else
                {
                    ml2 = ml;
                }

                if (ml2 == ml)
                {
                    // No better match
                    optr = op;
                    if (EncodeSequence(&ip, &op, &anchor, ml, refPos, limit, oend) != 0)
                        goto _dest_overflow;

                    continue;
                }

                if (start0 < ip)
                {
                    if (start2 < ip + ml0)
                    {
                        // empirical
                        ip = start0;
                        refPos = ref0;
                        ml = ml0;
                    }
                }

                // Here, start0==ip
                if (start2 - ip < 3)
                {
                    // First Match too small : removed
                    ml = ml2;
                    ip = start2;
                    refPos = ref2;
                    goto _Search2;
                }

                _Search3:
                // At this stage, we have :
                //  ml2 > ml1, and
                //  ip1+3 <= ip2 (usually < ip1+ml1)
                if ((start2 - ip) < OptimalML)
                {
                    int newMl = ml;
                    if (newMl > OptimalML)
                        newMl = OptimalML;

                    if (ip + newMl > start2 + ml2 - MinMatch)
                        newMl = (int)(start2 - ip) + ml2 - MinMatch;
                    int correction = newMl - (int)(start2 - ip);
                    if (correction > 0)
                    {
                        start2 += correction;
                        ref2 += correction;
                        ml2 -= correction;
                    }
                }

                // Now, we have start2 = ip+new_ml, with new_ml = min(ml, OPTIMAL_ML=18)

                int ml3;
                if (start2 + ml2 < mflimit)
                {
                    ml3 = InsertAndGetWiderMatch(
                        ctx,
                        start2 + ml2 - 3,
                        start2,
                        matchlimit,
                        ml2,
                        &ref3,
                        &start3,
                        (int)maxNbAttempts,
                        patternAnalysis);
                }
                else
                {
                    ml3 = ml2;
                }

                if (ml3 == ml2)
                {
                    // No better match : 2 sequences to encode
                    // ip & ref are known; Now for ml
                    if (start2 < ip + ml)
                        ml = (int)(start2 - ip);
                    // Now, encode 2 sequences
                    optr = op;
                    if (EncodeSequence(&ip, &op, &anchor, ml, refPos, limit, oend) != 0)
                        goto _dest_overflow;

                    ip = start2;
                    optr = op;
                    if (EncodeSequence(&ip, &op, &anchor, ml2, ref2, limit, oend) != 0)
                        goto _dest_overflow;

                    continue;
                }

                if (start3 < ip + ml + 3)
                {
                    // Not enough space for match 2 : remove it
                    if (start3 >= (ip + ml))
                    {
                        // can write Seq1 immediately ==> Seq2 is removed, so Seq3 becomes Seq1
                        if (start2 < ip + ml)
                        {
                            int correction = (int)(ip + ml - start2);
                            start2 += correction;
                            ref2 += correction;
                            ml2 -= correction;
                            if (ml2 < MinMatch)
                            {
                                start2 = start3;
                                ref2 = ref3;
                                ml2 = ml3;
                            }
                        }

                        optr = op;
                        if (EncodeSequence(&ip, &op, &anchor, ml, refPos, limit, oend) != 0)
                            goto _dest_overflow;

                        ip = start3;
                        refPos = ref3;
                        ml = ml3;

                        start0 = start2;
                        ref0 = ref2;
                        ml0 = ml2;
                        goto _Search2;
                    }

                    start2 = start3;
                    ref2 = ref3;
                    ml2 = ml3;
                    goto _Search3;
                }

                // OK, now we have 3 ascending matches; let's write at least the first one
                // ip & ref are known; Now for ml

                if (start2 < ip + ml)
                {
                    if (start2 - ip < (int)MLMask)
                    {
                        if (ml > OptimalML)
                            ml = OptimalML;
                        if (ip + ml > start2 + ml2 - MinMatch)
                            ml = (int)(start2 - ip) + ml2 - MinMatch;
                        int correction = ml - (int)(start2 - ip);
                        if (correction > 0)
                        {
                            start2 += correction;
                            ref2 += correction;
                            ml2 -= correction;
                        }
                    }
                    else
                    {
                        ml = (int)(start2 - ip);
                    }
                }

                optr = op;
                if (EncodeSequence(&ip, &op, &anchor, ml, refPos, limit, oend) != 0)
                    goto _dest_overflow;

                ip = start2;
                refPos = ref2;
                ml = ml2;

                start2 = start3;
                ref2 = ref3;
                ml2 = ml3;

                goto _Search3;
            }

            _last_literals: // Encode last literals
            {
                size_t lastRunSize = (size_t)(iend - anchor); // literals
                size_t litLength = (lastRunSize + 255 - RunMask) / 255;
                size_t totalSize = 1 + litLength + lastRunSize;
                if (limit == LimitedOutputDirective.LimitedDestSize)
                    oend += LastLiterals; // restore correct value
                if ((limit != 0) && (op + totalSize > oend))
                {
                    if (limit == LimitedOutputDirective.LimitedOutput)
                        return 0; // Check output limit

                    // adapt lastRunSize to fill 'dest'
                    lastRunSize = (size_t)(oend - op) - 1;
                    litLength = (lastRunSize + 255 - RunMask) / 255;
                    lastRunSize -= litLength;
                }

                ip = anchor + lastRunSize;

                if (lastRunSize >= RunMask)
                {
                    size_t accumulator = lastRunSize - RunMask;

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

                LZ4MemoryHelper.Copy(op, anchor, (int)lastRunSize);
                op += lastRunSize;
            }

            // End
            *srcSizePtr = (int)(ip - source);
            return (int)(op - dest);

            _dest_overflow:
            if (limit != LimitedOutputDirective.LimitedDestSize)
                return 0;

            op = optr; // restore correct out pointer
            goto _last_literals;
        }

        private static int CompressGeneric(CCtxT* ctx, byte* src, byte* dst, int* srcSizePtr, int dstCapacity, int cLevel, LimitedOutputDirective limit)
        {
            if ((limit == LimitedOutputDirective.LimitedDestSize) && (dstCapacity < 1))
                return 0; // Impossible to store anything
            if (*srcSizePtr > MaxInputSize)
                return 0; // Unsupported input size (too large or negative)

            ctx->End += *srcSizePtr;
            if (cLevel < 1)
                cLevel = CLevelDefault; // note : convention is different from lz4frame, maybe something to review
            cLevel = Math.Min(CLevelMax, cLevel);

            CParams cParam = ClTable[cLevel];
            if (cParam.Strat == StratE.HC)
            {
                return CompressHashChain(
                    ctx,
                    src,
                    dst,
                    srcSizePtr,
                    dstCapacity,
                    cParam.NbSearches,
                    limit);
            }

            return CompressOptimal(
                ctx,
                src,
                dst,
                srcSizePtr,
                dstCapacity,
                (int)cParam.NbSearches,
                cParam.TargetLength,
                limit,
                cLevel == CLevelMax ? 1 : 0); // ultra mode
        }

        private static int CompressHCExtStateHC(CCtxT* ctx, byte* src, byte* dst, int srcSize, int dstCapacity, int compressionLevel)
        {
            if (((size_t)ctx & (size_t)(sizeof(void*) - 1)) != 0)
                return 0;

            InitializeHC(ctx, src);

            return CompressGeneric(
                ctx,
                src,
                dst,
                &srcSize,
                dstCapacity,
                compressionLevel,
                dstCapacity < CompressBound(srcSize)
                    ? LimitedOutputDirective.LimitedOutput
                    : LimitedOutputDirective.NoLimit);
        }

        private static CCtxT* AllocCtx()
        {
            return (CCtxT*)LZ4MemoryHelper.Alloc(sizeof(CCtxT));
        }

        private static void FreeCtx(CCtxT* context)
        {
            LZ4MemoryHelper.Free(context);
        }

        internal static int CompressHC(byte* src, byte* dst, int srcSize, int dstCapacity, int compressionLevel)
        {
            CCtxT* ptr = AllocCtx();
            try
            {
                return CompressHCExtStateHC(ptr, src, dst, srcSize, dstCapacity, compressionLevel);
            }
            finally
            {
                FreeCtx(ptr);
            }
        }

        private static int CompressHCDestSize(CCtxT* ctx, byte* source, byte* dest, int* sourceSizePtr, int targetDestSize, int cLevel)
        {
            InitializeHC(ctx, source);
            return CompressGeneric(
                ctx,
                source,
                dest,
                sourceSizePtr,
                targetDestSize,
                cLevel,
                LimitedOutputDirective.LimitedDestSize);
        }

        // initialization
        public static void ResetStreamHC(CCtxT* ctxPtr, int compressionLevel)
        {
            ctxPtr->BaseP = null;
            SetCompressionLevel(ctxPtr, compressionLevel);
        }

        public static void SetCompressionLevel(CCtxT* ctxPtr, int compressionLevel)
        {
            if (compressionLevel < 1)
                compressionLevel = 1;

            if (compressionLevel > CLevelMax)
                compressionLevel = CLevelMax;

            ctxPtr->CompressionLevel = compressionLevel;
        }

        private static int LoadDictHC(CCtxT* ctxPtr, byte* dictionary, int dictSize)
        {
            if (dictSize > 64 * KB)
            {
                dictionary += dictSize - 64 * KB;
                dictSize = 64 * KB;
            }

            InitializeHC(ctxPtr, dictionary);
            ctxPtr->End = dictionary + dictSize;

            if (dictSize >= 4)
                Insert(ctxPtr, ctxPtr->End - 3);

            return dictSize;
        }

        private static void SetExternalDict(CCtxT* ctxPtr, byte* newBlock)
        {
            if (ctxPtr->End >= ctxPtr->BaseP + 4)
                Insert(ctxPtr, ctxPtr->End - 3); // Referencing remaining dictionary content

            // Only one memory segment for extDict, so any previous extDict is lost at this stage
            ctxPtr->LowLimit = ctxPtr->DictLimit;
            ctxPtr->DictLimit = (uint)(ctxPtr->End - ctxPtr->BaseP);
            ctxPtr->DictBase = ctxPtr->BaseP;
            ctxPtr->BaseP = newBlock - ctxPtr->DictLimit;
            ctxPtr->End = newBlock;
            ctxPtr->NextToUpdate = ctxPtr->DictLimit; // match referencing will resume from there
        }

        private static int CompressHCContinueGeneric(CCtxT* ctxPtr, byte* src, byte* dst, int* srcSizePtr, int dstCapacity, LimitedOutputDirective limit)
        {
            // auto-init if forgotten
            if (ctxPtr->BaseP == null)
                InitializeHC(ctxPtr, src);

            // Check overflow
            if ((size_t)(ctxPtr->End - ctxPtr->BaseP) > 2 * GB)
            {
                size_t dictSize = (size_t)(ctxPtr->End - ctxPtr->BaseP) - ctxPtr->DictLimit;
                if (dictSize > 64 * KB)
                    dictSize = 64 * KB;
                LoadDictHC(ctxPtr, ctxPtr->End - dictSize, (int)dictSize);
            }

            // Check if blocks follow each other
            if (src != ctxPtr->End)
                SetExternalDict(ctxPtr, src);

            // Check overlapping input/dictionary space
            {
                byte* sourceEnd = src + *srcSizePtr;
                byte* dictBegin = ctxPtr->DictBase + ctxPtr->LowLimit;
                byte* dictEnd = ctxPtr->DictBase + ctxPtr->DictLimit;
                if (sourceEnd > dictBegin && src < dictEnd)
                {
                    if (sourceEnd > dictEnd)
                        sourceEnd = dictEnd;

                    ctxPtr->LowLimit = (uint)(sourceEnd - ctxPtr->DictBase);

                    if (ctxPtr->DictLimit - ctxPtr->LowLimit < 4)
                        ctxPtr->LowLimit = ctxPtr->DictLimit;
                }
            }

            return CompressGeneric(
                ctxPtr,
                src,
                dst,
                srcSizePtr,
                dstCapacity,
                ctxPtr->CompressionLevel,
                limit);
        }

        public static int CompressHCContinue(CCtxT* ctxPtr, byte* src, byte* dst, int srcSize, int dstCapacity)
        {
            return CompressHCContinueGeneric(
                ctxPtr,
                src,
                dst,
                &srcSize,
                dstCapacity,
                dstCapacity < CompressBound(srcSize)
                    ? LimitedOutputDirective.LimitedOutput
                    : LimitedOutputDirective.NoLimit);
        }

        private static int CompressHCContinueDestSize(CCtxT* ctxPtr, byte* src, byte* dst, int* srcSizePtr, int targetDestSize)
        {
            return CompressHCContinueGeneric(
                ctxPtr,
                src,
                dst,
                srcSizePtr,
                targetDestSize,
                LimitedOutputDirective.LimitedDestSize);
        }

        public static int SaveDictHC(CCtxT* streamHCPtr, byte* safeBuffer, int dictSize)
        {
            CCtxT* streamPtr = streamHCPtr;
            int prefixSize = (int)(streamPtr->End - (streamPtr->BaseP + streamPtr->DictLimit));

            if (dictSize > 64 * KB)
                dictSize = 64 * KB;
            if (dictSize < 4)
                dictSize = 0;
            if (dictSize > prefixSize)
                dictSize = prefixSize;

            LZ4MemoryHelper.Move(safeBuffer, streamPtr->End - dictSize, dictSize);
            uint endIndex = (uint)(streamPtr->End - streamPtr->BaseP);
            streamPtr->End = safeBuffer + dictSize;
            streamPtr->BaseP = streamPtr->End - endIndex;
            streamPtr->DictLimit = endIndex - (uint)dictSize;
            streamPtr->LowLimit = endIndex - (uint)dictSize;

            if (streamPtr->NextToUpdate < streamPtr->DictLimit)
                streamPtr->NextToUpdate = streamPtr->DictLimit;

            return dictSize;
        }
    }
}
