/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements. See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

/*
 * This package is based on the work done by Keiron Liddle, Aftex Software
 * <keiron@aftexsw.com> to whom the Ant project is very grateful for his
 * great code.
 */

using System;
using System.IO;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Apache.Bzip2
{
	/**
    * An input stream that decompresses from the BZip2 format (with the file
    * header chars) to be read as any other stream.
    *
    * @author <a href="mailto:keiron@aftexsw.com">Keiron Liddle</a>
    *
    * <b>NB:</b> note this class has been modified to read the leading BZ from the
    * start of the BZIP2 stream to make it compatible with other PGP programs.
    */
    public class CBZip2InputStream : Stream 
	{
        private static void Cadvise() {
            //System.out.Println("CRC Error");
            //throw new CCoruptionError();
        }

//        private static void BadBGLengths() {
//            Cadvise();
//        }
//
//        private static void BitStreamEOF() {
//            Cadvise();
//        }

        private static void CompressedStreamEOF() {
            Cadvise();
        }

        private void MakeMaps() {
            int i;
            nInUse = 0;
            for (i = 0; i < 256; i++) {
                if (inUse[i]) {
                    seqToUnseq[nInUse] = (char) i;
                    unseqToSeq[i] = (char) nInUse;
                    nInUse++;
                }
            }
        }

        /*
        index of the last char in the block, so
        the block size == last + 1.
        */
        private int  last;

        /*
        index in zptr[] of original string after sorting.
        */
        private int  origPtr;

        /*
        always: in the range 0 .. 9.
        The current block size is 100000 * this number.
        */
        private int blockSize100k;

        private bool blockRandomised;

        private int bsBuff;
        private int bsLive;
        private CRC mCrc = new CRC();

        private bool[] inUse = new bool[256];
        private int nInUse;

        private char[] seqToUnseq = new char[256];
        private char[] unseqToSeq = new char[256];

        private char[] selector = new char[BZip2Constants.MAX_SELECTORS];
        private char[] selectorMtf = new char[BZip2Constants.MAX_SELECTORS];

        private int[] tt;
        private char[] ll8;

        /*
        freq table collected to save a pass over the data
        during decompression.
        */
        private int[] unzftab = new int[256];

        private int[][] limit = InitIntArray(BZip2Constants.N_GROUPS, BZip2Constants.MAX_ALPHA_SIZE);
        private int[][] basev = InitIntArray(BZip2Constants.N_GROUPS, BZip2Constants.MAX_ALPHA_SIZE);
        private int[][] perm = InitIntArray(BZip2Constants.N_GROUPS, BZip2Constants.MAX_ALPHA_SIZE);
        private int[] minLens = new int[BZip2Constants.N_GROUPS];

        private Stream bsStream;

        private bool streamEnd = false;

        private int currentChar = -1;

        private const int START_BLOCK_STATE = 1;
        private const int RAND_PART_A_STATE = 2;
        private const int RAND_PART_B_STATE = 3;
        private const int RAND_PART_C_STATE = 4;
        private const int NO_RAND_PART_A_STATE = 5;
        private const int NO_RAND_PART_B_STATE = 6;
        private const int NO_RAND_PART_C_STATE = 7;

        private int currentState = START_BLOCK_STATE;

        private int storedBlockCRC, storedCombinedCRC;
        private int computedBlockCRC, computedCombinedCRC;

        int i2, count, chPrev, ch2;
        int i, tPos;
        int rNToGo = 0;
        int rTPos  = 0;
        int j2;
        char z;

        public CBZip2InputStream(Stream zStream) {
            ll8 = null;
            tt = null;
            BsSetStream(zStream);
            Initialize();
            InitBlock();
            SetupBlock();
        }

        internal static int[][] InitIntArray(int n1, int n2) {
            int[][] a = new int[n1][];
            for (int k = 0; k < n1; ++k) {
                a[k] = new int[n2];
            }
            return a;
        }

        internal static char[][] InitCharArray(int n1, int n2) {
            char[][] a = new char[n1][];
            for (int k = 0; k < n1; ++k) {
                a[k] = new char[n2];
            }
            return a;
        }

        public override int ReadByte() {
            if (streamEnd) {
                return -1;
            } else {
                int retChar = currentChar;
                switch (currentState) {
                case START_BLOCK_STATE:
                    break;
                case RAND_PART_A_STATE:
                    break;
                case RAND_PART_B_STATE:
                    SetupRandPartB();
                    break;
                case RAND_PART_C_STATE:
                    SetupRandPartC();
                    break;
                case NO_RAND_PART_A_STATE:
                    break;
                case NO_RAND_PART_B_STATE:
                    SetupNoRandPartB();
                    break;
                case NO_RAND_PART_C_STATE:
                    SetupNoRandPartC();
                    break;
                default:
                    break;
                }
                return retChar;
            }
        }

        private void Initialize() {
            char magic3, magic4;
            magic3 = BsGetUChar();
            magic4 = BsGetUChar();
            if (magic3 != 'B' && magic4 != 'Z')
            {
                throw new IOException("Not a BZIP2 marked stream");
            }
            magic3 = BsGetUChar();
            magic4 = BsGetUChar();
            if (magic3 != 'h' || magic4 < '1' || magic4 > '9') {
                BsFinishedWithStream();
                streamEnd = true;
                return;
            }

            SetDecompressStructureSizes(magic4 - '0');
            computedCombinedCRC = 0;
        }

        private void InitBlock() {
            char magic1, magic2, magic3, magic4;
            char magic5, magic6;
            magic1 = BsGetUChar();
            magic2 = BsGetUChar();
            magic3 = BsGetUChar();
            magic4 = BsGetUChar();
            magic5 = BsGetUChar();
            magic6 = BsGetUChar();
            if (magic1 == 0x17 && magic2 == 0x72 && magic3 == 0x45
                && magic4 == 0x38 && magic5 == 0x50 && magic6 == 0x90) {
                Complete();
                return;
            }

            if (magic1 != 0x31 || magic2 != 0x41 || magic3 != 0x59
                || magic4 != 0x26 || magic5 != 0x53 || magic6 != 0x59) {
                BadBlockHeader();
                streamEnd = true;
                return;
            }

            storedBlockCRC = BsGetInt32();

            if (BsR(1) == 1) {
                blockRandomised = true;
            } else {
                blockRandomised = false;
            }

            //        currBlockNo++;
            GetAndMoveToFrontDecode();

            mCrc.InitialiseCRC();
            currentState = START_BLOCK_STATE;
        }

        private void EndBlock() {
            computedBlockCRC = mCrc.GetFinalCRC();
            /* A bad CRC is considered a fatal error. */
            if (storedBlockCRC != computedBlockCRC) {
                CrcError();
            }

            computedCombinedCRC = (computedCombinedCRC << 1)
                | (int)(((uint)computedCombinedCRC) >> 31);
            computedCombinedCRC ^= computedBlockCRC;
        }

        private void Complete() {
            storedCombinedCRC = BsGetInt32();
            if (storedCombinedCRC != computedCombinedCRC) {
                CrcError();
            }

            BsFinishedWithStream();
            streamEnd = true;
        }

        private static void BlockOverrun() {
            Cadvise();
        }

        private static void BadBlockHeader() {
            Cadvise();
        }

        private static void CrcError() {
            Cadvise();
        }

        private void BsFinishedWithStream() {
            try {
                if (this.bsStream != null) {
                    Platform.Dispose(this.bsStream);
                    this.bsStream = null;
                }
            } catch {
                //ignore
            }
        }

		private void BsSetStream(Stream f) {
            bsStream = f;
            bsLive = 0;
            bsBuff = 0;
        }

        private int BsR(int n) {
            int v;
            while (bsLive < n) {
                int zzi;
                char thech = '\0';
                try {
                    thech = (char) bsStream.ReadByte();
                } catch (IOException) {
                    CompressedStreamEOF();
                }
                if (thech == '\uffff') {
                    CompressedStreamEOF();
                }
                zzi = thech;
                bsBuff = (bsBuff << 8) | (zzi & 0xff);
                bsLive += 8;
            }

            v = (bsBuff >> (bsLive - n)) & ((1 << n) - 1);
            bsLive -= n;
            return v;
        }

        private char BsGetUChar() {
            return (char) BsR(8);
        }

        private int BsGetint() {
            int u = 0;
            u = (u << 8) | BsR(8);
            u = (u << 8) | BsR(8);
            u = (u << 8) | BsR(8);
            u = (u << 8) | BsR(8);
            return u;
        }

        private int BsGetIntVS(int numBits) {
            return (int) BsR(numBits);
        }

        private int BsGetInt32() {
            return (int) BsGetint();
        }

        private void HbCreateDecodeTables(int[] limit, int[] basev,
                                        int[] perm, char[] length,
                                        int minLen, int maxLen, int alphaSize) {
            int pp, i, j, vec;

            pp = 0;
            for (i = minLen; i <= maxLen; i++) {
                for (j = 0; j < alphaSize; j++) {
                    if (length[j] == i) {
                        perm[pp] = j;
                        pp++;
                    }
                }
            }

            for (i = 0; i < BZip2Constants.MAX_CODE_LEN; i++) {
                basev[i] = 0;
            }
            for (i = 0; i < alphaSize; i++) {
                basev[length[i] + 1]++;
            }

            for (i = 1; i < BZip2Constants.MAX_CODE_LEN; i++) {
                basev[i] += basev[i - 1];
            }

            for (i = 0; i < BZip2Constants.MAX_CODE_LEN; i++) {
                limit[i] = 0;
            }
            vec = 0;

            for (i = minLen; i <= maxLen; i++) {
                vec += (basev[i + 1] - basev[i]);
                limit[i] = vec - 1;
                vec <<= 1;
            }
            for (i = minLen + 1; i <= maxLen; i++) {
                basev[i] = ((limit[i - 1] + 1) << 1) - basev[i];
            }
        }

        private void RecvDecodingTables() {
            char[][] len = InitCharArray(BZip2Constants.N_GROUPS, BZip2Constants.MAX_ALPHA_SIZE);
            int i, j, t, nGroups, nSelectors, alphaSize;
            int minLen, maxLen;
            bool[] inUse16 = new bool[16];

            /* Receive the mapping table */
            for (i = 0; i < 16; i++) {
                if (BsR(1) == 1) {
                    inUse16[i] = true;
                } else {
                    inUse16[i] = false;
                }
            }

            for (i = 0; i < 256; i++) {
                inUse[i] = false;
            }

            for (i = 0; i < 16; i++) {
                if (inUse16[i]) {
                    for (j = 0; j < 16; j++) {
                        if (BsR(1) == 1) {
                            inUse[i * 16 + j] = true;
                        }
                    }
                }
            }

            MakeMaps();
            alphaSize = nInUse + 2;

            /* Now the selectors */
            nGroups = BsR(3);
            nSelectors = BsR(15);
            for (i = 0; i < nSelectors; i++) {
                j = 0;
                while (BsR(1) == 1) {
                    j++;
                }
                selectorMtf[i] = (char) j;
            }

            /* Undo the MTF values for the selectors. */
            {
                char[] pos = new char[BZip2Constants.N_GROUPS];
                char tmp, v;
                for (v = '\0'; v < nGroups; v++) {
                    pos[v] = v;
                }

                for (i = 0; i < nSelectors; i++) {
                    v = selectorMtf[i];
                    tmp = pos[v];
                    while (v > 0) {
                        pos[v] = pos[v - 1];
                        v--;
                    }
                    pos[0] = tmp;
                    selector[i] = tmp;
                }
            }

            /* Now the coding tables */
            for (t = 0; t < nGroups; t++) {
                int curr = BsR(5);
                for (i = 0; i < alphaSize; i++) {
                    while (BsR(1) == 1) {
                        if (BsR(1) == 0) {
                            curr++;
                        } else {
                            curr--;
                        }
                    }
                    len[t][i] = (char) curr;
                }
            }

            /* Create the Huffman decoding tables */
            for (t = 0; t < nGroups; t++) {
                minLen = 32;
                maxLen = 0;
                for (i = 0; i < alphaSize; i++) {
                    if (len[t][i] > maxLen) {
                        maxLen = len[t][i];
                    }
                    if (len[t][i] < minLen) {
                        minLen = len[t][i];
                    }
                }
                HbCreateDecodeTables(limit[t], basev[t], perm[t], len[t], minLen,
                                    maxLen, alphaSize);
                minLens[t] = minLen;
            }
        }

        private void GetAndMoveToFrontDecode() {
            char[] yy = new char[256];
            int i, j, nextSym, limitLast;
            int EOB, groupNo, groupPos;

            limitLast = BZip2Constants.baseBlockSize * blockSize100k;
            origPtr = BsGetIntVS(24);

            RecvDecodingTables();
            EOB = nInUse + 1;
            groupNo = -1;
            groupPos = 0;

            /*
            Setting up the unzftab entries here is not strictly
            necessary, but it does save having to do it later
            in a separate pass, and so saves a block's worth of
            cache misses.
            */
            for (i = 0; i <= 255; i++) {
                unzftab[i] = 0;
            }

            for (i = 0; i <= 255; i++) {
                yy[i] = (char) i;
            }

            last = -1;

            {
                int zt, zn, zvec, zj;
                if (groupPos == 0) {
                    groupNo++;
                    groupPos = BZip2Constants.G_SIZE;
                }
                groupPos--;
                zt = selector[groupNo];
                zn = minLens[zt];
                zvec = BsR(zn);
                while (zvec > limit[zt][zn]) {
                    zn++;
                    {
                        {
                            while (bsLive < 1) {
                                int zzi;
                                char thech = '\0';
                                try {
                                    thech = (char) bsStream.ReadByte();
                                } catch (IOException) {
                                    CompressedStreamEOF();
                                }
                                if (thech == '\uffff') {
                                    CompressedStreamEOF();
                                }
                                zzi = thech;
                                bsBuff = (bsBuff << 8) | (zzi & 0xff);
                                bsLive += 8;
                            }
                        }
                        zj = (bsBuff >> (bsLive - 1)) & 1;
                        bsLive--;
                    }
                    zvec = (zvec << 1) | zj;
                }
                nextSym = perm[zt][zvec - basev[zt][zn]];
            }

            while (true) {

                if (nextSym == EOB) {
                    break;
                }

                if (nextSym == BZip2Constants.RUNA || nextSym == BZip2Constants.RUNB) {
                    char ch;
                    int s = -1;
                    int N = 1;
                    do {
                        if (nextSym == BZip2Constants.RUNA) {
                            s = s + (0 + 1) * N;
                        } else if (nextSym == BZip2Constants.RUNB) {
                            s = s + (1 + 1) * N;
                            }
                        N = N * 2;
                        {
                            int zt, zn, zvec, zj;
                            if (groupPos == 0) {
                                groupNo++;
                                groupPos = BZip2Constants.G_SIZE;
                            }
                            groupPos--;
                            zt = selector[groupNo];
                            zn = minLens[zt];
                            zvec = BsR(zn);
                            while (zvec > limit[zt][zn]) {
                                zn++;
                                {
                                    {
                                        while (bsLive < 1) {
                                            int zzi;
                                            char thech = '\0';
                                            try {
                                                thech = (char) bsStream.ReadByte();
                                            } catch (IOException) {
                                                CompressedStreamEOF();
                                            }
                                            if (thech == '\uffff') {
                                                CompressedStreamEOF();
                                            }
                                            zzi = thech;
                                            bsBuff = (bsBuff << 8) | (zzi & 0xff);
                                            bsLive += 8;
                                        }
                                    }
                                    zj = (bsBuff >> (bsLive - 1)) & 1;
                                    bsLive--;
                                }
                                zvec = (zvec << 1) | zj;
                            }
                            nextSym = perm[zt][zvec - basev[zt][zn]];
                        }
                    } while (nextSym == BZip2Constants.RUNA || nextSym == BZip2Constants.RUNB);

                    s++;
                    ch = seqToUnseq[yy[0]];
                    unzftab[ch] += s;

                    while (s > 0) {
                        last++;
                        ll8[last] = ch;
                        s--;
                    }

                    if (last >= limitLast) {
                        BlockOverrun();
                    }
                    continue;
                } else {
                    char tmp;
                    last++;
                    if (last >= limitLast) {
                        BlockOverrun();
                    }

                    tmp = yy[nextSym - 1];
                    unzftab[seqToUnseq[tmp]]++;
                    ll8[last] = seqToUnseq[tmp];

                    /*
                    This loop is hammered during decompression,
                    hence the unrolling.

                    for (j = nextSym-1; j > 0; j--) yy[j] = yy[j-1];
                    */

                    j = nextSym - 1;
                    for (; j > 3; j -= 4) {
                        yy[j]     = yy[j - 1];
                        yy[j - 1] = yy[j - 2];
                        yy[j - 2] = yy[j - 3];
                        yy[j - 3] = yy[j - 4];
                    }
                    for (; j > 0; j--) {
                        yy[j] = yy[j - 1];
                    }

                    yy[0] = tmp;
                    {
                        int zt, zn, zvec, zj;
                        if (groupPos == 0) {
                            groupNo++;
                            groupPos = BZip2Constants.G_SIZE;
                        }
                        groupPos--;
                        zt = selector[groupNo];
                        zn = minLens[zt];
                        zvec = BsR(zn);
                        while (zvec > limit[zt][zn]) {
                            zn++;
                            {
                                {
                                    while (bsLive < 1) {
                                        int zzi;
                                        char thech = '\0';
                                        try {
                                            thech = (char) bsStream.ReadByte();
                                        } catch (IOException) {
                                            CompressedStreamEOF();
                                        }
                                        zzi = thech;
                                        bsBuff = (bsBuff << 8) | (zzi & 0xff);
                                        bsLive += 8;
                                    }
                                }
                                zj = (bsBuff >> (bsLive - 1)) & 1;
                                bsLive--;
                            }
                            zvec = (zvec << 1) | zj;
                        }
                        nextSym = perm[zt][zvec - basev[zt][zn]];
                    }
                    continue;
                }
            }
        }

        private void SetupBlock() {
            int[] cftab = new int[257];
            char ch;

            cftab[0] = 0;
            for (i = 1; i <= 256; i++) {
                cftab[i] = unzftab[i - 1];
            }
            for (i = 1; i <= 256; i++) {
                cftab[i] += cftab[i - 1];
            }

            for (i = 0; i <= last; i++) {
                ch = (char) ll8[i];
                tt[cftab[ch]] = i;
                cftab[ch]++;
            }
            cftab = null;

            tPos = tt[origPtr];

            count = 0;
            i2 = 0;
            ch2 = 256;   /* not a char and not EOF */

            if (blockRandomised) {
                rNToGo = 0;
                rTPos = 0;
                SetupRandPartA();
            } else {
                SetupNoRandPartA();
            }
        }

        private void SetupRandPartA() {
            if (i2 <= last) {
                chPrev = ch2;
                ch2 = ll8[tPos];
                tPos = tt[tPos];
                if (rNToGo == 0) {
                    rNToGo = BZip2Constants.rNums[rTPos];
                    rTPos++;
                    if (rTPos == 512) {
                        rTPos = 0;
                    }
                }
                rNToGo--;
                ch2 ^= (int) ((rNToGo == 1) ? 1 : 0);
                i2++;

                currentChar = ch2;
                currentState = RAND_PART_B_STATE;
                mCrc.UpdateCRC(ch2);
            } else {
                EndBlock();
                InitBlock();
                SetupBlock();
            }
        }

        private void SetupNoRandPartA() {
            if (i2 <= last) {
                chPrev = ch2;
                ch2 = ll8[tPos];
                tPos = tt[tPos];
                i2++;

                currentChar = ch2;
                currentState = NO_RAND_PART_B_STATE;
                mCrc.UpdateCRC(ch2);
            } else {
                EndBlock();
                InitBlock();
                SetupBlock();
            }
        }

        private void SetupRandPartB() {
            if (ch2 != chPrev) {
                currentState = RAND_PART_A_STATE;
                count = 1;
                SetupRandPartA();
            } else {
                count++;
                if (count >= 4) {
                    z = ll8[tPos];
                    tPos = tt[tPos];
                    if (rNToGo == 0) {
                        rNToGo = BZip2Constants.rNums[rTPos];
                        rTPos++;
                        if (rTPos == 512) {
                            rTPos = 0;
                        }
                    }
                    rNToGo--;
                    z ^= (char)((rNToGo == 1) ? 1 : 0);
                    j2 = 0;
                    currentState = RAND_PART_C_STATE;
                    SetupRandPartC();
                } else {
                    currentState = RAND_PART_A_STATE;
                    SetupRandPartA();
                }
            }
        }

        private void SetupRandPartC() {
            if (j2 < (int) z) {
                currentChar = ch2;
                mCrc.UpdateCRC(ch2);
                j2++;
            } else {
                currentState = RAND_PART_A_STATE;
                i2++;
                count = 0;
                SetupRandPartA();
            }
        }

        private void SetupNoRandPartB() {
            if (ch2 != chPrev) {
                currentState = NO_RAND_PART_A_STATE;
                count = 1;
                SetupNoRandPartA();
            } else {
                count++;
                if (count >= 4) {
                    z = ll8[tPos];
                    tPos = tt[tPos];
                    currentState = NO_RAND_PART_C_STATE;
                    j2 = 0;
                    SetupNoRandPartC();
                } else {
                    currentState = NO_RAND_PART_A_STATE;
                    SetupNoRandPartA();
                }
            }
        }

        private void SetupNoRandPartC() {
            if (j2 < (int) z) {
                currentChar = ch2;
                mCrc.UpdateCRC(ch2);
                j2++;
            } else {
                currentState = NO_RAND_PART_A_STATE;
                i2++;
                count = 0;
                SetupNoRandPartA();
            }
        }

        private void SetDecompressStructureSizes(int newSize100k) {
            if (!(0 <= newSize100k && newSize100k <= 9 && 0 <= blockSize100k
                && blockSize100k <= 9)) {
                // throw new IOException("Invalid block size");
            }

            blockSize100k = newSize100k;

            if (newSize100k == 0) {
                return;
            }

            int n = BZip2Constants.baseBlockSize * newSize100k;
            ll8 = new char[n];
            tt = new int[n];
        }
    
        public override void Flush() {
        }
    
        public override int Read(byte[] buffer, int offset, int count) {
            int c = -1;
            int k;
            for (k = 0; k < count; ++k) {
                c = ReadByte();
                if (c == -1)
                    break;
                buffer[k + offset] = (byte)c;
            }
            return k;
        }
    
        public override long Seek(long offset, SeekOrigin origin) {
            return 0;
        }
    
        public override void SetLength(long value) {
        }
    
        public override void Write(byte[] buffer, int offset, int count) {
        }
    
        public override bool CanRead {
            get {
                return true;
            }
        }
    
        public override bool CanSeek {
            get {
                return false;
            }
        }
    
        public override bool CanWrite {
            get {
                return false;
            }
        }
    
        public override long Length {
            get {
                return 0;
            }
        }
    
        public override long Position {
            get {
                return 0;
            }
            set {
            }
        }
    }
}