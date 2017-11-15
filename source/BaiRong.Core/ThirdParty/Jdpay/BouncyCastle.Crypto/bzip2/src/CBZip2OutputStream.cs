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
    * An output stream that compresses into the BZip2 format (with the file
    * header chars) into another stream.
    *
    * @author <a href="mailto:keiron@aftexsw.com">Keiron Liddle</a>
    *
    * TODO:    Update to BZip2 1.0.1
    * <b>NB:</b> note this class has been modified to add a leading BZ to the
    * start of the BZIP2 stream to make it compatible with other PGP programs.
    */
    public class CBZip2OutputStream : Stream 
	{
        protected const int SETMASK = (1 << 21);
        protected const int CLEARMASK = (~SETMASK);
        protected const int GREATER_ICOST = 15;
        protected const int LESSER_ICOST = 0;
        protected const int SMALL_THRESH = 20;
        protected const int DEPTH_THRESH = 10;

        /*
        If you are ever unlucky/improbable enough
        to get a stack overflow whilst sorting,
        increase the following constant and try
        again.  In practice I have never seen the
        stack go above 27 elems, so the following
        limit seems very generous.
        */
        protected const int QSORT_STACK_SIZE = 1000;
        private bool finished;

        private static void Panic() {
            //System.out.Println("panic");
            //throw new CError();
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

        protected static void HbMakeCodeLengths(char[] len, int[] freq,
                                                int alphaSize, int maxLen) {
            /*
            Nodes and heap entries run from 1.  Entry 0
            for both the heap and nodes is a sentinel.
            */
            int nNodes, nHeap, n1, n2, i, j, k;
            bool  tooLong;

            int[] heap = new int[BZip2Constants.MAX_ALPHA_SIZE + 2];
            int[] weight = new int[BZip2Constants.MAX_ALPHA_SIZE * 2];
            int[] parent = new int[BZip2Constants.MAX_ALPHA_SIZE * 2];

            for (i = 0; i < alphaSize; i++) {
                weight[i + 1] = (freq[i] == 0 ? 1 : freq[i]) << 8;
            }

            while (true) {
                nNodes = alphaSize;
                nHeap = 0;

                heap[0] = 0;
                weight[0] = 0;
                parent[0] = -2;

                for (i = 1; i <= alphaSize; i++) {
                    parent[i] = -1;
                    nHeap++;
                    heap[nHeap] = i;
                    {
                        int zz, tmp;
                        zz = nHeap;
                        tmp = heap[zz];
                        while (weight[tmp] < weight[heap[zz >> 1]]) {
                            heap[zz] = heap[zz >> 1];
                            zz >>= 1;
                        }
                        heap[zz] = tmp;
                    }
                }
                if (!(nHeap < (BZip2Constants.MAX_ALPHA_SIZE + 2))) {
                    Panic();
                }

                while (nHeap > 1) {
                    n1 = heap[1];
                    heap[1] = heap[nHeap];
                    nHeap--;
                    {
                        int zz = 0, yy = 0, tmp = 0;
                        zz = 1;
                        tmp = heap[zz];
                        while (true) {
                            yy = zz << 1;
                            if (yy > nHeap) {
                                break;
                            }
                            if (yy < nHeap
                                && weight[heap[yy + 1]] < weight[heap[yy]]) {
                                yy++;
                            }
                            if (weight[tmp] < weight[heap[yy]]) {
                                break;
                            }
                            heap[zz] = heap[yy];
                            zz = yy;
                        }
                        heap[zz] = tmp;
                    }
                    n2 = heap[1];
                    heap[1] = heap[nHeap];
                    nHeap--;
                    {
                        int zz = 0, yy = 0, tmp = 0;
                        zz = 1;
                        tmp = heap[zz];
                        while (true) {
                            yy = zz << 1;
                            if (yy > nHeap) {
                                break;
                            }
                            if (yy < nHeap
                                && weight[heap[yy + 1]] < weight[heap[yy]]) {
                                yy++;
                            }
                            if (weight[tmp] < weight[heap[yy]]) {
                                break;
                            }
                            heap[zz] = heap[yy];
                            zz = yy;
                        }
                        heap[zz] = tmp;
                    }
                    nNodes++;
                    parent[n1] = parent[n2] = nNodes;

                    weight[nNodes] = (int)((uint)((weight[n1] & 0xffffff00)
                                    + (weight[n2] & 0xffffff00))
                        | (uint)(1 + (((weight[n1] & 0x000000ff) >
                                (weight[n2] & 0x000000ff)) ?
                                (weight[n1] & 0x000000ff) :
                                (weight[n2] & 0x000000ff))));

                    parent[nNodes] = -1;
                    nHeap++;
                    heap[nHeap] = nNodes;
                    {
                        int zz = 0, tmp = 0;
                        zz = nHeap;
                        tmp = heap[zz];
                        while (weight[tmp] < weight[heap[zz >> 1]]) {
                            heap[zz] = heap[zz >> 1];
                            zz >>= 1;
                        }
                        heap[zz] = tmp;
                    }
                }
                if (!(nNodes < (BZip2Constants.MAX_ALPHA_SIZE * 2))) {
                    Panic();
                }

                tooLong = false;
                for (i = 1; i <= alphaSize; i++) {
                    j = 0;
                    k = i;
                    while (parent[k] >= 0) {
                        k = parent[k];
                        j++;
                    }
                    len[i - 1] = (char) j;
                    if (j > maxLen) {
                        tooLong = true;
                    }
                }

                if (!tooLong) {
                    break;
                }

                for (i = 1; i < alphaSize; i++) {
                    j = weight[i] >> 8;
                    j = 1 + (j / 2);
                    weight[i] = j << 8;
                }
            }
        }

        /*
        index of the last char in the block, so
        the block size == last + 1.
        */
        int last;

        /*
        index in zptr[] of original string after sorting.
        */
        int origPtr;

        /*
        always: in the range 0 .. 9.
        The current block size is 100000 * this number.
        */
        int blockSize100k;

        bool blockRandomised;

        int bytesOut;
        int bsBuff;
        int bsLive;
        CRC mCrc = new CRC();

        private bool[] inUse = new bool[256];
        private int nInUse;

        private char[] seqToUnseq = new char[256];
        private char[] unseqToSeq = new char[256];

        private char[] selector = new char[BZip2Constants.MAX_SELECTORS];
        private char[] selectorMtf = new char[BZip2Constants.MAX_SELECTORS];

        private char[] block;
        private int[] quadrant;
        private int[] zptr;
        private short[] szptr;
        private int[] ftab;

        private int nMTF;

        private int[] mtfFreq = new int[BZip2Constants.MAX_ALPHA_SIZE];

        /*
        * Used when sorting.  If too many long comparisons
        * happen, we stop sorting, randomise the block
        * slightly, and try again.
        */
        private int workFactor;
        private int workDone;
        private int workLimit;
        private bool firstAttempt;
        private int nBlocksRandomised;

        private int currentChar = -1;
        private int runLength = 0;

        public CBZip2OutputStream(Stream inStream) : this(inStream, 9) {
        }

        public CBZip2OutputStream(Stream inStream, int inBlockSize)
            {
            block = null;
            quadrant = null;
            zptr = null;
            ftab = null;

            inStream.WriteByte((byte)'B');
            inStream.WriteByte((byte)'Z');

            BsSetStream(inStream);

            workFactor = 50;
            if (inBlockSize > 9) {
                inBlockSize = 9;
            }
            if (inBlockSize < 1) {
                inBlockSize = 1;
            }
            blockSize100k = inBlockSize;
            AllocateCompressStructures();
            Initialize();
            InitBlock();
        }

        /**
        *
        * modified by Oliver Merkel, 010128
        *
        */
        public override void WriteByte(byte bv) {
            int b = (256 + bv) % 256;
            if (currentChar != -1) {
                if (currentChar == b) {
                    runLength++;
                    if (runLength > 254) {
                        WriteRun();
                        currentChar = -1;
                        runLength = 0;
                    }
                } else {
                    WriteRun();
                    runLength = 1;
                    currentChar = b;
                }
            } else {
                currentChar = b;
                runLength++;
            }
        }

        private void WriteRun() {
            if (last < allowableBlockSize) {
                inUse[currentChar] = true;
                for (int i = 0; i < runLength; i++) {
                    mCrc.UpdateCRC((char) currentChar);
                }
                switch (runLength) {
                case 1:
                    last++;
                    block[last + 1] = (char) currentChar;
                    break;
                case 2:
                    last++;
                    block[last + 1] = (char) currentChar;
                    last++;
                    block[last + 1] = (char) currentChar;
                    break;
                case 3:
                    last++;
                    block[last + 1] = (char) currentChar;
                    last++;
                    block[last + 1] = (char) currentChar;
                    last++;
                    block[last + 1] = (char) currentChar;
                    break;
                default:
                    inUse[runLength - 4] = true;
                    last++;
                    block[last + 1] = (char) currentChar;
                    last++;
                    block[last + 1] = (char) currentChar;
                    last++;
                    block[last + 1] = (char) currentChar;
                    last++;
                    block[last + 1] = (char) currentChar;
                    last++;
                    block[last + 1] = (char) (runLength - 4);
                    break;
                }
            } else {
                EndBlock();
                InitBlock();
                WriteRun();
            }
        }

        bool closed = false;

//        protected void Finalize() {
//            Close();
//        }

#if PORTABLE
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (closed)
                    return;

                Finish();
                closed = true;
                Platform.Dispose(this.bsStream);
            }
            base.Dispose(disposing);
        }
#else
        public override void Close() {
            if (closed)
                return;

            Finish();

            closed = true;
            Platform.Dispose(this.bsStream);

            base.Close();
        }
#endif

        public void Finish() {
            if (finished) {
                return;
            }

            if (runLength > 0) {
                WriteRun();
            }
            currentChar = -1;
            EndBlock();
            EndCompression();
            finished = true;
            Flush();
        }
        
        public override void Flush() {
            bsStream.Flush();
        }

        private int blockCRC, combinedCRC;

        private void Initialize() {
            bytesOut = 0;
            nBlocksRandomised = 0;

            /* Write `magic' bytes h indicating file-format == huffmanised,
            followed by a digit indicating blockSize100k.
            */
            BsPutUChar('h');
            BsPutUChar('0' + blockSize100k);

            combinedCRC = 0;
        }

        private int allowableBlockSize;

        private void InitBlock() {
            //        blockNo++;
            mCrc.InitialiseCRC();
            last = -1;
            //        ch = 0;

            for (int i = 0; i < 256; i++) {
                inUse[i] = false;
            }

            /* 20 is just a paranoia constant */
            allowableBlockSize = BZip2Constants.baseBlockSize * blockSize100k - 20;
        }

        private void EndBlock() {
            blockCRC = mCrc.GetFinalCRC();
            combinedCRC = (combinedCRC << 1) | (int)(((uint)combinedCRC) >> 31);
            combinedCRC ^= blockCRC;

            /* sort the block and establish posn of original string */
            DoReversibleTransformation();

            /*
            A 6-byte block header, the value chosen arbitrarily
            as 0x314159265359 :-).  A 32 bit value does not really
            give a strong enough guarantee that the value will not
            appear by chance in the compressed datastream.  Worst-case
            probability of this event, for a 900k block, is about
            2.0e-3 for 32 bits, 1.0e-5 for 40 bits and 4.0e-8 for 48 bits.
            For a compressed file of size 100Gb -- about 100000 blocks --
            only a 48-bit marker will do.  NB: normal compression/
            decompression do *not* rely on these statistical properties.
            They are only important when trying to recover blocks from
            damaged files.
            */
            BsPutUChar(0x31);
            BsPutUChar(0x41);
            BsPutUChar(0x59);
            BsPutUChar(0x26);
            BsPutUChar(0x53);
            BsPutUChar(0x59);

            /* Now the block's CRC, so it is in a known place. */
            BsPutint(blockCRC);

            /* Now a single bit indicating randomisation. */
            if (blockRandomised) {
                BsW(1, 1);
                nBlocksRandomised++;
            } else {
                BsW(1, 0);
            }

            /* Finally, block's contents proper. */
            MoveToFrontCodeAndSend();
        }

        private void EndCompression() {
            /*
            Now another magic 48-bit number, 0x177245385090, to
            indicate the end of the last block.  (Sqrt(pi), if
            you want to know.  I did want to use e, but it contains
            too much repetition -- 27 18 28 18 28 46 -- for me
            to feel statistically comfortable.  Call me paranoid.)
            */
            BsPutUChar(0x17);
            BsPutUChar(0x72);
            BsPutUChar(0x45);
            BsPutUChar(0x38);
            BsPutUChar(0x50);
            BsPutUChar(0x90);

            BsPutint(combinedCRC);

            BsFinishedWithStream();
        }

        private void HbAssignCodes(int[] code, char[] length, int minLen,
                                    int maxLen, int alphaSize) {
            int n, vec, i;

            vec = 0;
            for (n = minLen; n <= maxLen; n++) {
                for (i = 0; i < alphaSize; i++) {
                    if (length[i] == n) {
                        code[i] = vec;
                        vec++;
                    }
                };
                vec <<= 1;
            }
        }

        private void BsSetStream(Stream f) {
            bsStream = f;
            bsLive = 0;
            bsBuff = 0;
            bytesOut = 0;
        }

        private void BsFinishedWithStream() {
            while (bsLive > 0) {
                int ch = (bsBuff >> 24);
                try {
                    bsStream.WriteByte((byte)ch); // write 8-bit
                } catch (IOException e) {
                    throw  e;
                }
                bsBuff <<= 8;
                bsLive -= 8;
                bytesOut++;
            }
        }

        private void BsW(int n, int v) {
            while (bsLive >= 8) {
                int ch = (bsBuff >> 24);
                try {
                    bsStream.WriteByte((byte)ch); // write 8-bit
                } catch (IOException e) {
                    throw e;
                }
                bsBuff <<= 8;
                bsLive -= 8;
                bytesOut++;
            }
            bsBuff |= (v << (32 - bsLive - n));
            bsLive += n;
        }

        private void BsPutUChar(int c) {
            BsW(8, c);
        }

        private void BsPutint(int u) {
            BsW(8, (u >> 24) & 0xff);
            BsW(8, (u >> 16) & 0xff);
            BsW(8, (u >>  8) & 0xff);
            BsW(8,  u        & 0xff);
        }

        private void BsPutIntVS(int numBits, int c) {
            BsW(numBits, c);
        }

        private void SendMTFValues() {
            char[][] len = CBZip2InputStream.InitCharArray(BZip2Constants.N_GROUPS, BZip2Constants.MAX_ALPHA_SIZE);

            int v, t, i, j, gs, ge, totc, bt, bc, iter;
            int nSelectors = 0, alphaSize, minLen, maxLen, selCtr;
            int nGroups;

            alphaSize = nInUse + 2;
            for (t = 0; t < BZip2Constants.N_GROUPS; t++) {
                for (v = 0; v < alphaSize; v++) {
                    len[t][v] = (char) GREATER_ICOST;
                }
            }

            /* Decide how many coding tables to use */
            if (nMTF <= 0) {
                Panic();
            }

            if (nMTF < 200) {
                nGroups = 2;
            } else if (nMTF < 600) {
                nGroups = 3;
            } else if (nMTF < 1200) {
                nGroups = 4;
            } else if (nMTF < 2400) {
                nGroups = 5;
            } else {
                nGroups = 6;
            }

            /* Generate an initial set of coding tables */ {
                int nPart, remF, tFreq, aFreq;

                nPart = nGroups;
                remF  = nMTF;
                gs = 0;
                while (nPart > 0) {
                    tFreq = remF / nPart;
                    ge = gs - 1;
                    aFreq = 0;
                    while (aFreq < tFreq && ge < alphaSize - 1) {
                        ge++;
                        aFreq += mtfFreq[ge];
                    }

                    if (ge > gs && nPart != nGroups && nPart != 1
                        && ((nGroups - nPart) % 2 == 1)) {
                        aFreq -= mtfFreq[ge];
                        ge--;
                    }

                    for (v = 0; v < alphaSize; v++) {
                        if (v >= gs && v <= ge) {
                            len[nPart - 1][v] = (char) LESSER_ICOST;
                        } else {
                            len[nPart - 1][v] = (char) GREATER_ICOST;
                        }
                    }

                    nPart--;
                    gs = ge + 1;
                    remF -= aFreq;
                }
            }

            int[][] rfreq = CBZip2InputStream.InitIntArray(BZip2Constants.N_GROUPS, BZip2Constants.MAX_ALPHA_SIZE);
            int[] fave = new int[BZip2Constants.N_GROUPS];
            short[] cost = new short[BZip2Constants.N_GROUPS];
            /*
            Iterate up to N_ITERS times to improve the tables.
            */
            for (iter = 0; iter < BZip2Constants.N_ITERS; iter++) {
                for (t = 0; t < nGroups; t++) {
                    fave[t] = 0;
                }

                for (t = 0; t < nGroups; t++) {
                    for (v = 0; v < alphaSize; v++) {
                        rfreq[t][v] = 0;
                    }
                }

                nSelectors = 0;
                totc = 0;
                gs = 0;
                while (true) {

                    /* Set group start & end marks. */
                    if (gs >= nMTF) {
                        break;
                    }
                    ge = gs + BZip2Constants.G_SIZE - 1;
                    if (ge >= nMTF) {
                        ge = nMTF - 1;
                    }

                    /*
                    Calculate the cost of this group as coded
                    by each of the coding tables.
                    */
                    for (t = 0; t < nGroups; t++) {
                        cost[t] = 0;
                    }

                    if (nGroups == 6) {
                        short cost0, cost1, cost2, cost3, cost4, cost5;
                        cost0 = cost1 = cost2 = cost3 = cost4 = cost5 = 0;
                        for (i = gs; i <= ge; i++) {
                            short icv = szptr[i];
                            cost0 += (short)len[0][icv];
                            cost1 += (short)len[1][icv];
                            cost2 += (short)len[2][icv];
                            cost3 += (short)len[3][icv];
                            cost4 += (short)len[4][icv];
                            cost5 += (short)len[5][icv];
                        }
                        cost[0] = cost0;
                        cost[1] = cost1;
                        cost[2] = cost2;
                        cost[3] = cost3;
                        cost[4] = cost4;
                        cost[5] = cost5;
                    } else {
                        for (i = gs; i <= ge; i++) {
                            short icv = szptr[i];
                            for (t = 0; t < nGroups; t++) {
                                cost[t] += (short)len[t][icv];
                            }
                        }
                    }

                    /*
                    Find the coding table which is best for this group,
                    and record its identity in the selector table.
                    */
                    bc = 999999999;
                    bt = -1;
                    for (t = 0; t < nGroups; t++) {
                        if (cost[t] < bc) {
                            bc = cost[t];
                            bt = t;
                        }
                    };
                    totc += bc;
                    fave[bt]++;
                    selector[nSelectors] = (char) bt;
                    nSelectors++;

                    /*
                    Increment the symbol frequencies for the selected table.
                    */
                    for (i = gs; i <= ge; i++) {
                        rfreq[bt][szptr[i]]++;
                    }

                    gs = ge + 1;
                }

                /*
                Recompute the tables based on the accumulated frequencies.
                */
                for (t = 0; t < nGroups; t++) {
                    HbMakeCodeLengths(len[t], rfreq[t], alphaSize, 20);
                }
            }

            rfreq = null;
            fave = null;
            cost = null;

            if (!(nGroups < 8)) {
                Panic();
            }
            if (!(nSelectors < 32768 && nSelectors <= (2 + (900000 / BZip2Constants.G_SIZE)))) {
                Panic();
            }


            /* Compute MTF values for the selectors. */
            {
                char[] pos = new char[BZip2Constants.N_GROUPS];
                char ll_i, tmp2, tmp;
                for (i = 0; i < nGroups; i++) {
                    pos[i] = (char) i;
                }
                for (i = 0; i < nSelectors; i++) {
                    ll_i = selector[i];
                    j = 0;
                    tmp = pos[j];
                    while (ll_i != tmp) {
                        j++;
                        tmp2 = tmp;
                        tmp = pos[j];
                        pos[j] = tmp2;
                    }
                    pos[0] = tmp;
                    selectorMtf[i] = (char) j;
                }
            }

            int[][] code = CBZip2InputStream.InitIntArray(BZip2Constants.N_GROUPS, BZip2Constants.MAX_ALPHA_SIZE);

            /* Assign actual codes for the tables. */
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
                if (maxLen > 20) {
                    Panic();
                }
                if (minLen < 1) {
                    Panic();
                }
                HbAssignCodes(code[t], len[t], minLen, maxLen, alphaSize);
            }

            /* Transmit the mapping table. */
            {
                bool[] inUse16 = new bool[16];
                for (i = 0; i < 16; i++) {
                    inUse16[i] = false;
                    for (j = 0; j < 16; j++) {
                        if (inUse[i * 16 + j]) {
                            inUse16[i] = true;
                        }
                    }
                }

				for (i = 0; i < 16; i++) {
                    if (inUse16[i]) {
                        BsW(1, 1);
                    } else {
                        BsW(1, 0);
                    }
                }

                for (i = 0; i < 16; i++) {
                    if (inUse16[i]) {
                        for (j = 0; j < 16; j++) {
                            if (inUse[i * 16 + j]) {
                                BsW(1, 1);
                            } else {
                                BsW(1, 0);
                            }
                        }
                    }
                }

            }

            /* Now the selectors. */
            BsW(3, nGroups);
            BsW(15, nSelectors);
            for (i = 0; i < nSelectors; i++) {
                for (j = 0; j < selectorMtf[i]; j++) {
                    BsW(1, 1);
                }
                BsW(1, 0);
            }

            /* Now the coding tables. */
            for (t = 0; t < nGroups; t++) {
                int curr = len[t][0];
                BsW(5, curr);
                for (i = 0; i < alphaSize; i++) {
                    while (curr < len[t][i]) {
                        BsW(2, 2);
                        curr++; /* 10 */
                    }
                    while (curr > len[t][i]) {
                        BsW(2, 3);
                        curr--; /* 11 */
                    }
                    BsW(1, 0);
                }
            }

            /* And finally, the block data proper */
            selCtr = 0;
            gs = 0;
            while (true) {
                if (gs >= nMTF) {
                    break;
                }
                ge = gs + BZip2Constants.G_SIZE - 1;
                if (ge >= nMTF) {
                    ge = nMTF - 1;
                }
                for (i = gs; i <= ge; i++) {
                    BsW(len[selector[selCtr]][szptr[i]],
                        code[selector[selCtr]][szptr[i]]);
                }

                gs = ge + 1;
                selCtr++;
            }
            if (!(selCtr == nSelectors)) {
                Panic();
            }
        }

        private void MoveToFrontCodeAndSend() {
            BsPutIntVS(24, origPtr);
            GenerateMTFValues();
            SendMTFValues();
        }

        private Stream bsStream;

        private void SimpleSort(int lo, int hi, int d) {
            int i, j, h, bigN, hp;
            int v;

            bigN = hi - lo + 1;
            if (bigN < 2) {
                return;
            }

            hp = 0;
            while (incs[hp] < bigN) {
                hp++;
            }
            hp--;

            for (; hp >= 0; hp--) {
                h = incs[hp];

                i = lo + h;
                while (true) {
                    /* copy 1 */
                    if (i > hi) {
                        break;
                    }
                    v = zptr[i];
                    j = i;
                    while (FullGtU(zptr[j - h] + d, v + d)) {
                        zptr[j] = zptr[j - h];
                        j = j - h;
                        if (j <= (lo + h - 1)) {
                            break;
                        }
                    }
                    zptr[j] = v;
                    i++;

                    /* copy 2 */
                    if (i > hi) {
                        break;
                    }
                    v = zptr[i];
                    j = i;
                    while (FullGtU(zptr[j - h] + d, v + d)) {
                        zptr[j] = zptr[j - h];
                        j = j - h;
                        if (j <= (lo + h - 1)) {
                            break;
                        }
                    }
                    zptr[j] = v;
                    i++;

                    /* copy 3 */
                    if (i > hi) {
                        break;
                    }
                    v = zptr[i];
                    j = i;
                    while (FullGtU(zptr[j - h] + d, v + d)) {
                        zptr[j] = zptr[j - h];
                        j = j - h;
                        if (j <= (lo + h - 1)) {
                            break;
                        }
                    }
                    zptr[j] = v;
                    i++;

                    if (workDone > workLimit && firstAttempt) {
                        return;
                    }
                }
            }
        }

        private void Vswap(int p1, int p2, int n) {
            int temp = 0;
            while (n > 0) {
                temp = zptr[p1];
                zptr[p1] = zptr[p2];
                zptr[p2] = temp;
                p1++;
                p2++;
                n--;
            }
        }

        private char Med3(char a, char b, char c) {
            char t;
            if (a > b) {
                t = a;
                a = b;
                b = t;
            }
            if (b > c) {
                t = b;
                b = c;
                c = t;
            }
            if (a > b) {
                b = a;
            }
            return b;
        }

        internal class StackElem {
            internal int ll;
            internal int hh;
            internal int dd;
        }

        private void QSort3(int loSt, int hiSt, int dSt) {
            int unLo, unHi, ltLo, gtHi, med, n, m;
            int sp, lo, hi, d;
            StackElem[] stack = new StackElem[QSORT_STACK_SIZE];
            for (int count = 0; count < QSORT_STACK_SIZE; count++) {
                stack[count] = new StackElem();
            }

            sp = 0;

            stack[sp].ll = loSt;
            stack[sp].hh = hiSt;
            stack[sp].dd = dSt;
            sp++;

            while (sp > 0) {
                if (sp >= QSORT_STACK_SIZE) {
                    Panic();
                }

                sp--;
                lo = stack[sp].ll;
                hi = stack[sp].hh;
                d = stack[sp].dd;

                if (hi - lo < SMALL_THRESH || d > DEPTH_THRESH) {
                    SimpleSort(lo, hi, d);
                    if (workDone > workLimit && firstAttempt) {
                        return;
                    }
                    continue;
                }

                med = Med3(block[zptr[lo] + d + 1],
                        block[zptr[hi            ] + d  + 1],
                        block[zptr[(lo + hi) >> 1] + d + 1]);

                unLo = ltLo = lo;
                unHi = gtHi = hi;

                while (true) {
                    while (true) {
                        if (unLo > unHi) {
                            break;
                        }
                        n = ((int) block[zptr[unLo] + d + 1]) - med;
                        if (n == 0) {
                            int temp = 0;
                            temp = zptr[unLo];
                            zptr[unLo] = zptr[ltLo];
                            zptr[ltLo] = temp;
                            ltLo++;
                            unLo++;
                            continue;
                        };
                        if (n >  0) {
                            break;
                        }
                        unLo++;
                    }
                    while (true) {
                        if (unLo > unHi) {
                            break;
                        }
                        n = ((int) block[zptr[unHi] + d + 1]) - med;
                        if (n == 0) {
                            int temp = 0;
                            temp = zptr[unHi];
                            zptr[unHi] = zptr[gtHi];
                            zptr[gtHi] = temp;
                            gtHi--;
                            unHi--;
                            continue;
                        };
                        if (n <  0) {
                            break;
                        }
                        unHi--;
                    }
                    if (unLo > unHi) {
                        break;
                    }
                    int tempx = zptr[unLo];
                    zptr[unLo] = zptr[unHi];
                    zptr[unHi] = tempx;
                    unLo++;
                    unHi--;
                }

                if (gtHi < ltLo) {
                    stack[sp].ll = lo;
                    stack[sp].hh = hi;
                    stack[sp].dd = d + 1;
                    sp++;
                    continue;
                }

                n = ((ltLo - lo) < (unLo - ltLo)) ? (ltLo - lo) : (unLo - ltLo);
                Vswap(lo, unLo - n, n);
                m = ((hi - gtHi) < (gtHi - unHi)) ? (hi - gtHi) : (gtHi - unHi);
                Vswap(unLo, hi - m + 1, m);

                n = lo + unLo - ltLo - 1;
                m = hi - (gtHi - unHi) + 1;

                stack[sp].ll = lo;
                stack[sp].hh = n;
                stack[sp].dd = d;
                sp++;

                stack[sp].ll = n + 1;
                stack[sp].hh = m - 1;
                stack[sp].dd = d + 1;
                sp++;

                stack[sp].ll = m;
                stack[sp].hh = hi;
                stack[sp].dd = d;
                sp++;
            }
        }

        private void MainSort() {
            int i, j, ss, sb;
            int[] runningOrder = new int[256];
            int[] copy = new int[256];
            bool[] bigDone = new bool[256];
            int c1, c2;
            int numQSorted;

            /*
            In the various block-sized structures, live data runs
            from 0 to last+NUM_OVERSHOOT_BYTES inclusive.  First,
            set up the overshoot area for block.
            */

            //   if (verbosity >= 4) fprintf ( stderr, "   sort initialise ...\n" );
            for (i = 0; i < BZip2Constants.NUM_OVERSHOOT_BYTES; i++) {
                block[last + i + 2] = block[(i % (last + 1)) + 1];
            }
            for (i = 0; i <= last + BZip2Constants.NUM_OVERSHOOT_BYTES; i++) {
                quadrant[i] = 0;
            }

            block[0] = (char) (block[last + 1]);

            if (last < 4000) {
                /*
                Use SimpleSort(), since the full sorting mechanism
                has quite a large constant overhead.
                */
                for (i = 0; i <= last; i++) {
                    zptr[i] = i;
                }
                firstAttempt = false;
                workDone = workLimit = 0;
                SimpleSort(0, last, 0);
            } else {
                numQSorted = 0;
                for (i = 0; i <= 255; i++) {
                    bigDone[i] = false;
                }

                for (i = 0; i <= 65536; i++) {
                    ftab[i] = 0;
                }

                c1 = block[0];
                for (i = 0; i <= last; i++) {
                    c2 = block[i + 1];
                    ftab[(c1 << 8) + c2]++;
                    c1 = c2;
                }

                for (i = 1; i <= 65536; i++) {
                    ftab[i] += ftab[i - 1];
                }

                c1 = block[1];
                for (i = 0; i < last; i++) {
                    c2 = block[i + 2];
                    j = (c1 << 8) + c2;
                    c1 = c2;
                    ftab[j]--;
                    zptr[ftab[j]] = i;
                }

                j = ((block[last + 1]) << 8) + (block[1]);
                ftab[j]--;
                zptr[ftab[j]] = last;

                /*
                Now ftab contains the first loc of every small bucket.
                Calculate the running order, from smallest to largest
                big bucket.
                */

                for (i = 0; i <= 255; i++) {
                    runningOrder[i] = i;
                }

                {
                    int vv;
                    int h = 1;
                    do {
                        h = 3 * h + 1;
                    }
                    while (h <= 256);
                    do {
                        h = h / 3;
                        for (i = h; i <= 255; i++) {
                            vv = runningOrder[i];
                            j = i;
                            while ((ftab[((runningOrder[j - h]) + 1) << 8]
                                    - ftab[(runningOrder[j - h]) << 8]) >
                                (ftab[((vv) + 1) << 8] - ftab[(vv) << 8])) {
                                runningOrder[j] = runningOrder[j - h];
                                j = j - h;
                                if (j <= (h - 1)) {
                                    break;
                                }
                            }
                            runningOrder[j] = vv;
                        }
                    } while (h != 1);
                }

                /*
                The main sorting loop.
                */
                for (i = 0; i <= 255; i++) {

                    /*
                    Process big buckets, starting with the least full.
                    */
                    ss = runningOrder[i];

                    /*
                    Complete the big bucket [ss] by quicksorting
                    any unsorted small buckets [ss, j].  Hopefully
                    previous pointer-scanning phases have already
                    completed many of the small buckets [ss, j], so
                    we don't have to sort them at all.
                    */
                    for (j = 0; j <= 255; j++) {
                        sb = (ss << 8) + j;
                        if (!((ftab[sb] & SETMASK) == SETMASK)) {
                            int lo = ftab[sb] & CLEARMASK;
                            int hi = (ftab[sb + 1] & CLEARMASK) - 1;
                            if (hi > lo) {
                                QSort3(lo, hi, 2);
                                numQSorted += (hi - lo + 1);
                                if (workDone > workLimit && firstAttempt) {
                                    return;
                                }
                            }
                            ftab[sb] |= SETMASK;
                        }
                    }

                    /*
                    The ss big bucket is now done.  Record this fact,
                    and update the quadrant descriptors.  Remember to
                    update quadrants in the overshoot area too, if
                    necessary.  The "if (i < 255)" test merely skips
                    this updating for the last bucket processed, since
                    updating for the last bucket is pointless.
                    */
                    bigDone[ss] = true;

                    if (i < 255) {
                        int bbStart  = ftab[ss << 8] & CLEARMASK;
                        int bbSize   = (ftab[(ss + 1) << 8] & CLEARMASK) - bbStart;
                        int shifts   = 0;

                        while ((bbSize >> shifts) > 65534) {
                            shifts++;
                        }

                        for (j = 0; j < bbSize; j++) {
                            int a2update = zptr[bbStart + j];
                            int qVal = (j >> shifts);
                            quadrant[a2update] = qVal;
                            if (a2update < BZip2Constants.NUM_OVERSHOOT_BYTES) {
                                quadrant[a2update + last + 1] = qVal;
                            }
                        }

                        if (!(((bbSize - 1) >> shifts) <= 65535)) {
                            Panic();
                        }
                    }

                    /*
                    Now scan this big bucket so as to synthesise the
                    sorted order for small buckets [t, ss] for all t != ss.
                    */
                    for (j = 0; j <= 255; j++) {
                        copy[j] = ftab[(j << 8) + ss] & CLEARMASK;
                    }

                    for (j = ftab[ss << 8] & CLEARMASK;
                        j < (ftab[(ss + 1) << 8] & CLEARMASK); j++) {
                        c1 = block[zptr[j]];
                        if (!bigDone[c1]) {
                            zptr[copy[c1]] = zptr[j] == 0 ? last : zptr[j] - 1;
                            copy[c1]++;
                        }
                    }

                    for (j = 0; j <= 255; j++) {
                        ftab[(j << 8) + ss] |= SETMASK;
                    }
                }
            }
        }

        private void RandomiseBlock() {
            int i;
            int rNToGo = 0;
            int rTPos  = 0;
            for (i = 0; i < 256; i++) {
                inUse[i] = false;
            }

            for (i = 0; i <= last; i++) {
                if (rNToGo == 0) {
                    rNToGo = (char) BZip2Constants.rNums[rTPos];
                    rTPos++;
                    if (rTPos == 512) {
                        rTPos = 0;
                    }
                }
                rNToGo--;
                block[i + 1] ^= (char)((rNToGo == 1) ? 1 : 0);
                // handle 16 bit signed numbers
                block[i + 1] &= (char)0xFF;

                inUse[block[i + 1]] = true;
            }
        }

        private void DoReversibleTransformation() {
            int i;

            workLimit = workFactor * last;
            workDone = 0;
            blockRandomised = false;
            firstAttempt = true;

            MainSort();

            if (workDone > workLimit && firstAttempt) {
                RandomiseBlock();
                workLimit = workDone = 0;
                blockRandomised = true;
                firstAttempt = false;
                MainSort();
            }

            origPtr = -1;
            for (i = 0; i <= last; i++) {
                if (zptr[i] == 0) {
                    origPtr = i;
                    break;
                }
            };

            if (origPtr == -1) {
                Panic();
            }
        }

        private bool FullGtU(int i1, int i2) {
            int k;
            char c1, c2;
            int s1, s2;

            c1 = block[i1 + 1];
            c2 = block[i2 + 1];
            if (c1 != c2) {
                return (c1 > c2);
            }
            i1++;
            i2++;

            c1 = block[i1 + 1];
            c2 = block[i2 + 1];
            if (c1 != c2) {
                return (c1 > c2);
            }
            i1++;
            i2++;

            c1 = block[i1 + 1];
            c2 = block[i2 + 1];
            if (c1 != c2) {
                return (c1 > c2);
            }
            i1++;
            i2++;

            c1 = block[i1 + 1];
            c2 = block[i2 + 1];
            if (c1 != c2) {
                return (c1 > c2);
            }
            i1++;
            i2++;

            c1 = block[i1 + 1];
            c2 = block[i2 + 1];
            if (c1 != c2) {
                return (c1 > c2);
            }
            i1++;
            i2++;

            c1 = block[i1 + 1];
            c2 = block[i2 + 1];
            if (c1 != c2) {
                return (c1 > c2);
            }
            i1++;
            i2++;

            k = last + 1;

            do {
                c1 = block[i1 + 1];
                c2 = block[i2 + 1];
                if (c1 != c2) {
                    return (c1 > c2);
                }
                s1 = quadrant[i1];
                s2 = quadrant[i2];
                if (s1 != s2) {
                    return (s1 > s2);
                }
                i1++;
                i2++;

                c1 = block[i1 + 1];
                c2 = block[i2 + 1];
                if (c1 != c2) {
                    return (c1 > c2);
                }
                s1 = quadrant[i1];
                s2 = quadrant[i2];
                if (s1 != s2) {
                    return (s1 > s2);
                }
                i1++;
                i2++;

                c1 = block[i1 + 1];
                c2 = block[i2 + 1];
                if (c1 != c2) {
                    return (c1 > c2);
                }
                s1 = quadrant[i1];
                s2 = quadrant[i2];
                if (s1 != s2) {
                    return (s1 > s2);
                }
                i1++;
                i2++;

                c1 = block[i1 + 1];
                c2 = block[i2 + 1];
                if (c1 != c2) {
                    return (c1 > c2);
                }
                s1 = quadrant[i1];
                s2 = quadrant[i2];
                if (s1 != s2) {
                    return (s1 > s2);
                }
                i1++;
                i2++;

                if (i1 > last) {
                    i1 -= last;
                    i1--;
                };
                if (i2 > last) {
                    i2 -= last;
                    i2--;
                };

                k -= 4;
                workDone++;
            } while (k >= 0);

            return false;
        }

        /*
        Knuth's increments seem to work better
        than Incerpi-Sedgewick here.  Possibly
        because the number of elems to sort is
        usually small, typically <= 20.
        */
        private int[] incs = { 1, 4, 13, 40, 121, 364, 1093, 3280,
                            9841, 29524, 88573, 265720,
                            797161, 2391484 };

        private void AllocateCompressStructures() {
            int n = BZip2Constants.baseBlockSize * blockSize100k;
            block = new char[(n + 1 + BZip2Constants.NUM_OVERSHOOT_BYTES)];
            quadrant = new int[(n + BZip2Constants.NUM_OVERSHOOT_BYTES)];
            zptr = new int[n];
            ftab = new int[65537];

            if (block == null || quadrant == null || zptr == null
                || ftab == null) {
                //int totalDraw = (n + 1 + NUM_OVERSHOOT_BYTES) + (n + NUM_OVERSHOOT_BYTES) + n + 65537;
                //compressOutOfMemory ( totalDraw, n );
            }

            /*
            The back end needs a place to store the MTF values
            whilst it calculates the coding tables.  We could
            put them in the zptr array.  However, these values
            will fit in a short, so we overlay szptr at the
            start of zptr, in the hope of reducing the number
            of cache misses induced by the multiple traversals
            of the MTF values when calculating coding tables.
            Seems to improve compression speed by about 1%.
            */
            //    szptr = zptr;


            szptr = new short[2 * n];
        }

        private void GenerateMTFValues() {
            char[] yy = new char[256];
            int  i, j;
            char tmp;
            char tmp2;
            int zPend;
            int wr;
            int EOB;

            MakeMaps();
            EOB = nInUse + 1;

            for (i = 0; i <= EOB; i++) {
                mtfFreq[i] = 0;
            }

            wr = 0;
            zPend = 0;
            for (i = 0; i < nInUse; i++) {
                yy[i] = (char) i;
            }


            for (i = 0; i <= last; i++) {
                char ll_i;

                ll_i = unseqToSeq[block[zptr[i]]];

                j = 0;
                tmp = yy[j];
                while (ll_i != tmp) {
                    j++;
                    tmp2 = tmp;
                    tmp = yy[j];
                    yy[j] = tmp2;
                };
                yy[0] = tmp;

                if (j == 0) {
                    zPend++;
                } else {
                    if (zPend > 0) {
                        zPend--;
                        while (true) {
                            switch (zPend % 2) {
                            case 0:
                                szptr[wr] = (short) BZip2Constants.RUNA;
                                wr++;
                                mtfFreq[BZip2Constants.RUNA]++;
                                break;
                            case 1:
                                szptr[wr] = (short) BZip2Constants.RUNB;
                                wr++;
                                mtfFreq[BZip2Constants.RUNB]++;
                                break;
                            };
                            if (zPend < 2) {
                                break;
                            }
                            zPend = (zPend - 2) / 2;
                        };
                        zPend = 0;
                    }
                    szptr[wr] = (short) (j + 1);
                    wr++;
                    mtfFreq[j + 1]++;
                }
            }

            if (zPend > 0) {
                zPend--;
                while (true) {
                    switch (zPend % 2) {
                    case 0:
                        szptr[wr] = (short) BZip2Constants.RUNA;
                        wr++;
                        mtfFreq[BZip2Constants.RUNA]++;
                        break;
                    case 1:
                        szptr[wr] = (short) BZip2Constants.RUNB;
                        wr++;
                        mtfFreq[BZip2Constants.RUNB]++;
                        break;
                    }
                    if (zPend < 2) {
                        break;
                    }
                    zPend = (zPend - 2) / 2;
                }
            }

            szptr[wr] = (short) EOB;
            wr++;
            mtfFreq[EOB]++;

            nMTF = wr;
        }

        public override int Read(byte[] buffer, int offset, int count) {
            return 0;
        }
    
        public override long Seek(long offset, SeekOrigin origin) {
            return 0;
        }
    
        public override void SetLength(long value) {
        }
    
        public override void Write(byte[] buffer, int offset, int count) {
            for (int k = 0; k < count; ++k) {
                WriteByte(buffer[k + offset]);
            }
        }
    
        public override bool CanRead {
            get {
                return false;
            }
        }
    
        public override bool CanSeek {
            get {
                return false;
            }
        }
    
        public override bool CanWrite {
            get {
                return true;
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