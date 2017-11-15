using System;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Engines
{
    /**
    * A class that provides a basic International Data Encryption Algorithm (IDEA) engine.
    * <p>
    * This implementation is based on the "HOWTO: INTERNATIONAL DATA ENCRYPTION ALGORITHM"
    * implementation summary by Fauzan Mirza (F.U.Mirza@sheffield.ac.uk). (baring 1 typo at the
    * end of the mulinv function!).
    * </p>
    * <p>
    * It can be found at ftp://ftp.funet.fi/pub/crypt/cryptography/symmetric/idea/
    * </p>
    * <p>
    * Note 1: This algorithm is patented in the USA, Japan, and Europe including
    * at least Austria, France, Germany, Italy, Netherlands, Spain, Sweden, Switzerland
    * and the United Kingdom. Non-commercial use is free, however any commercial
    * products are liable for royalties. Please see
    * <a href="http://www.mediacrypt.com">www.mediacrypt.com</a> for
    * further details. This announcement has been included at the request of
    * the patent holders.
    * </p>
    * <p>
    * Note 2: Due to the requests concerning the above, this algorithm is now only
    * included in the extended assembly. It is not included in the default distributions.
    * </p>
    */
    public class IdeaEngine
        : IBlockCipher
    {
        private const int  BLOCK_SIZE = 8;
        private int[] workingKey;
        /**
        * standard constructor.
        */
        public IdeaEngine()
        {
        }
        /**
        * initialise an IDEA cipher.
        *
        * @param forEncryption whether or not we are for encryption.
        * @param parameters the parameters required to set up the cipher.
        * @exception ArgumentException if the parameters argument is
        * inappropriate.
        */
        public virtual void Init(
            bool				forEncryption,
            ICipherParameters	parameters)
        {
            if (!(parameters is KeyParameter))
                throw new ArgumentException("invalid parameter passed to IDEA init - " + Platform.GetTypeName(parameters));

            workingKey = GenerateWorkingKey(forEncryption,
                ((KeyParameter)parameters).GetKey());
        }

        public virtual string AlgorithmName
        {
            get { return "IDEA"; }
        }

        public virtual bool IsPartialBlockOkay
        {
            get { return false; }
        }

        public virtual int GetBlockSize()
        {
            return BLOCK_SIZE;
        }

        public virtual int ProcessBlock(
            byte[] input,
            int inOff,
            byte[] output,
            int outOff)
        {
            if (workingKey == null)
                throw new InvalidOperationException("IDEA engine not initialised");

            Check.DataLength(input, inOff, BLOCK_SIZE, "input buffer too short");
            Check.OutputLength(output, outOff, BLOCK_SIZE, "output buffer too short");

            IdeaFunc(workingKey, input, inOff, output, outOff);
            return BLOCK_SIZE;
        }
        public virtual void Reset()
        {
        }
        private static readonly int    MASK = 0xffff;
        private static readonly int    BASE = 0x10001;
        private int BytesToWord(
            byte[]  input,
            int     inOff)
        {
            return ((input[inOff] << 8) & 0xff00) + (input[inOff + 1] & 0xff);
        }
        private void WordToBytes(
            int     word,
            byte[]  outBytes,
            int     outOff)
        {
            outBytes[outOff] = (byte)((uint) word >> 8);
            outBytes[outOff + 1] = (byte)word;
        }
        /**
        * return x = x * y where the multiplication is done modulo
        * 65537 (0x10001) (as defined in the IDEA specification) and
        * a zero input is taken to be 65536 (0x10000).
        *
        * @param x the x value
        * @param y the y value
        * @return x = x * y
        */
        private int Mul(
            int x,
            int y)
        {
            if (x == 0)
            {
                x = (BASE - y);
            }
            else if (y == 0)
            {
                x = (BASE - x);
            }
            else
            {
                int     p = x * y;
                y = p & MASK;
                x = (int) ((uint) p >> 16);
                x = y - x + ((y < x) ? 1 : 0);
            }
            return x & MASK;
        }
        private void IdeaFunc(
            int[]   workingKey,
            byte[]  input,
            int     inOff,
            byte[]  outBytes,
            int     outOff)
        {
            int     x0, x1, x2, x3, t0, t1;
            int     keyOff = 0;
            x0 = BytesToWord(input, inOff);
            x1 = BytesToWord(input, inOff + 2);
            x2 = BytesToWord(input, inOff + 4);
            x3 = BytesToWord(input, inOff + 6);
            for (int round = 0; round < 8; round++)
            {
                x0 = Mul(x0, workingKey[keyOff++]);
                x1 += workingKey[keyOff++];
                x1 &= MASK;
                x2 += workingKey[keyOff++];
                x2 &= MASK;
                x3 = Mul(x3, workingKey[keyOff++]);
                t0 = x1;
                t1 = x2;
                x2 ^= x0;
                x1 ^= x3;
                x2 = Mul(x2, workingKey[keyOff++]);
                x1 += x2;
                x1 &= MASK;
                x1 = Mul(x1, workingKey[keyOff++]);
                x2 += x1;
                x2 &= MASK;
                x0 ^= x1;
                x3 ^= x2;
                x1 ^= t1;
                x2 ^= t0;
            }
            WordToBytes(Mul(x0, workingKey[keyOff++]), outBytes, outOff);
            WordToBytes(x2 + workingKey[keyOff++], outBytes, outOff + 2);  /* NB: Order */
            WordToBytes(x1 + workingKey[keyOff++], outBytes, outOff + 4);
            WordToBytes(Mul(x3, workingKey[keyOff]), outBytes, outOff + 6);
        }
        /**
        * The following function is used to expand the user key to the encryption
        * subkey. The first 16 bytes are the user key, and the rest of the subkey
        * is calculated by rotating the previous 16 bytes by 25 bits to the left,
        * and so on until the subkey is completed.
        */
        private int[] ExpandKey(
            byte[]  uKey)
        {
            int[]   key = new int[52];
            if (uKey.Length < 16)
            {
                byte[]  tmp = new byte[16];
                Array.Copy(uKey, 0, tmp, tmp.Length - uKey.Length, uKey.Length);
                uKey = tmp;
            }
            for (int i = 0; i < 8; i++)
            {
                key[i] = BytesToWord(uKey, i * 2);
            }
            for (int i = 8; i < 52; i++)
            {
                if ((i & 7) < 6)
                {
                    key[i] = ((key[i - 7] & 127) << 9 | key[i - 6] >> 7) & MASK;
                }
                else if ((i & 7) == 6)
                {
                    key[i] = ((key[i - 7] & 127) << 9 | key[i - 14] >> 7) & MASK;
                }
                else
                {
                    key[i] = ((key[i - 15] & 127) << 9 | key[i - 14] >> 7) & MASK;
                }
            }
            return key;
        }
        /**
        * This function computes multiplicative inverse using Euclid's Greatest
        * Common Divisor algorithm. Zero and one are self inverse.
        * <p>
        * i.e. x * MulInv(x) == 1 (modulo BASE)
        * </p>
        */
        private int MulInv(
            int x)
        {
            int t0, t1, q, y;

            if (x < 2)
            {
                return x;
            }
            t0 = 1;
            t1 = BASE / x;
            y  = BASE % x;
            while (y != 1)
            {
                q = x / y;
                x = x % y;
                t0 = (t0 + (t1 * q)) & MASK;
                if (x == 1)
                {
                    return t0;
                }
                q = y / x;
                y = y % x;
                t1 = (t1 + (t0 * q)) & MASK;
            }
            return (1 - t1) & MASK;
        }
        /**
        * Return the additive inverse of x.
        * <p>
        * i.e. x + AddInv(x) == 0
        * </p>
        */
        int AddInv(
            int x)
        {
            return (0 - x) & MASK;
        }

        /**
        * The function to invert the encryption subkey to the decryption subkey.
        * It also involves the multiplicative inverse and the additive inverse functions.
        */
        private int[] InvertKey(
            int[] inKey)
        {
            int     t1, t2, t3, t4;
            int     p = 52;                 /* We work backwards */
            int[]   key = new int[52];
            int     inOff = 0;

            t1 = MulInv(inKey[inOff++]);
            t2 = AddInv(inKey[inOff++]);
            t3 = AddInv(inKey[inOff++]);
            t4 = MulInv(inKey[inOff++]);
            key[--p] = t4;
            key[--p] = t3;
            key[--p] = t2;
            key[--p] = t1;

            for (int round = 1; round < 8; round++)
            {
                t1 = inKey[inOff++];
                t2 = inKey[inOff++];
                key[--p] = t2;
                key[--p] = t1;

                t1 = MulInv(inKey[inOff++]);
                t2 = AddInv(inKey[inOff++]);
                t3 = AddInv(inKey[inOff++]);
                t4 = MulInv(inKey[inOff++]);
                key[--p] = t4;
                key[--p] = t2; /* NB: Order */
                key[--p] = t3;
                key[--p] = t1;
            }
            t1 = inKey[inOff++];
            t2 = inKey[inOff++];
            key[--p] = t2;
            key[--p] = t1;

            t1 = MulInv(inKey[inOff++]);
            t2 = AddInv(inKey[inOff++]);
            t3 = AddInv(inKey[inOff++]);
            t4 = MulInv(inKey[inOff]);
            key[--p] = t4;
            key[--p] = t3;
            key[--p] = t2;
            key[--p] = t1;
            return key;
        }

        private int[] GenerateWorkingKey(
            bool forEncryption,
            byte[]  userKey)
        {
            if (forEncryption)
            {
                return ExpandKey(userKey);
            }
            else
            {
                return InvertKey(ExpandKey(userKey));
            }
        }
    }
}
