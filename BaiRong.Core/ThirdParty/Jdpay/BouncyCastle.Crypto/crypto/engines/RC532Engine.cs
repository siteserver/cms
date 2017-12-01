using System;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Engines
{
    /**
    * The specification for RC5 came from the <code>RC5 Encryption Algorithm</code>
    * publication in RSA CryptoBytes, Spring of 1995.
    * <em>http://www.rsasecurity.com/rsalabs/cryptobytes</em>.
    * <p>
    * This implementation has a word size of 32 bits.</p>
    */
    public class RC532Engine
		: IBlockCipher
    {
        /*
        * the number of rounds to perform
        */
        private int _noRounds;

        /*
        * the expanded key array of size 2*(rounds + 1)
        */
        private int [] _S;

        /*
        * our "magic constants" for 32 32
        *
        * Pw = Odd((e-2) * 2^wordsize)
        * Qw = Odd((o-2) * 2^wordsize)
        *
        * where e is the base of natural logarithms (2.718281828...)
        * and o is the golden ratio (1.61803398...)
        */
        private static readonly int P32 = unchecked((int) 0xb7e15163);
        private static readonly int Q32 = unchecked((int) 0x9e3779b9);

        private bool forEncryption;

        /**
        * Create an instance of the RC5 encryption algorithm
        * and set some defaults
        */
        public RC532Engine()
        {
            _noRounds     = 12;         // the default
//            _S            = null;
        }

        public virtual string AlgorithmName
        {
            get { return "RC5-32"; }
        }

        public virtual bool IsPartialBlockOkay
		{
			get { return false; }
		}

        public virtual int GetBlockSize()
        {
            return 2 * 4;
        }

		/**
        * initialise a RC5-32 cipher.
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
            if (typeof(RC5Parameters).IsInstanceOfType(parameters))
            {
                RC5Parameters p = (RC5Parameters)parameters;

                _noRounds = p.Rounds;

                SetKey(p.GetKey());
            }
            else if (typeof(KeyParameter).IsInstanceOfType(parameters))
            {
                KeyParameter p = (KeyParameter)parameters;

                SetKey(p.GetKey());
            }
            else
            {
                throw new ArgumentException("invalid parameter passed to RC532 init - " + Platform.GetTypeName(parameters));
            }

            this.forEncryption = forEncryption;
        }

        public virtual int ProcessBlock(
            byte[]	input,
            int		inOff,
            byte[]	output,
            int		outOff)
        {
            return (forEncryption)
				?	EncryptBlock(input, inOff, output, outOff)
				:	DecryptBlock(input, inOff, output, outOff);
        }

        public virtual void Reset()
        {
        }

        /**
        * Re-key the cipher.
        *
        * @param  key  the key to be used
        */
        private void SetKey(
            byte[] key)
        {
            //
            // KEY EXPANSION:
            //
            // There are 3 phases to the key expansion.
            //
            // Phase 1:
            //   Copy the secret key K[0...b-1] into an array L[0..c-1] of
            //   c = ceil(b/u), where u = 32/8 in little-endian order.
            //   In other words, we fill up L using u consecutive key bytes
            //   of K. Any unfilled byte positions in L are zeroed. In the
            //   case that b = c = 0, set c = 1 and L[0] = 0.
            //
            int[]   L = new int[(key.Length + (4 - 1)) / 4];

            for (int i = 0; i != key.Length; i++)
            {
                L[i / 4] += (key[i] & 0xff) << (8 * (i % 4));
            }

            //
            // Phase 2:
            //   Initialize S to a particular fixed pseudo-random bit pattern
            //   using an arithmetic progression modulo 2^wordsize determined
            //   by the magic numbers, Pw & Qw.
            //
            _S            = new int[2*(_noRounds + 1)];

            _S[0] = P32;
            for (int i=1; i < _S.Length; i++)
            {
                _S[i] = (_S[i-1] + Q32);
            }

            //
            // Phase 3:
            //   Mix in the user's secret key in 3 passes over the arrays S & L.
            //   The max of the arrays sizes is used as the loop control
            //
            int iter;

            if (L.Length > _S.Length)
            {
                iter = 3 * L.Length;
            }
            else
            {
                iter = 3 * _S.Length;
            }

            int A = 0, B = 0;
            int ii = 0, jj = 0;

            for (int k = 0; k < iter; k++)
            {
                A = _S[ii] = RotateLeft(_S[ii] + A + B, 3);
                B =  L[jj] = RotateLeft( L[jj] + A + B, A+B);
                ii = (ii+1) % _S.Length;
                jj = (jj+1) %  L.Length;
            }
        }

        /**
        * Encrypt the given block starting at the given offset and place
        * the result in the provided buffer starting at the given offset.
        *
        * @param  in     in byte buffer containing data to encrypt
        * @param  inOff  offset into src buffer
        * @param  out     out buffer where encrypted data is written
        * @param  outOff  offset into out buffer
        */
        private int EncryptBlock(
            byte[]  input,
            int     inOff,
            byte[]  outBytes,
            int     outOff)
        {
            int A = BytesToWord(input, inOff) + _S[0];
            int B = BytesToWord(input, inOff + 4) + _S[1];

            for (int i = 1; i <= _noRounds; i++)
            {
                A = RotateLeft(A ^ B, B) + _S[2*i];
                B = RotateLeft(B ^ A, A) + _S[2*i+1];
            }

            WordToBytes(A, outBytes, outOff);
            WordToBytes(B, outBytes, outOff + 4);

            return 2 * 4;
        }

        private int DecryptBlock(
            byte[]  input,
            int     inOff,
            byte[]  outBytes,
            int     outOff)
        {
            int A = BytesToWord(input, inOff);
            int B = BytesToWord(input, inOff + 4);

            for (int i = _noRounds; i >= 1; i--)
            {
                B = RotateRight(B - _S[2*i+1], A) ^ A;
                A = RotateRight(A - _S[2*i],   B) ^ B;
            }

            WordToBytes(A - _S[0], outBytes, outOff);
            WordToBytes(B - _S[1], outBytes, outOff + 4);

            return 2 * 4;
        }


        //////////////////////////////////////////////////////////////
        //
        // PRIVATE Helper Methods
        //
        //////////////////////////////////////////////////////////////

        /**
        * Perform a left "spin" of the word. The rotation of the given
        * word <em>x</em> is rotated left by <em>y</em> bits.
        * Only the <em>lg(32)</em> low-order bits of <em>y</em>
        * are used to determine the rotation amount. Here it is
        * assumed that the wordsize used is a power of 2.
        *
        * @param  x  word to rotate
        * @param  y    number of bits to rotate % 32
        */
        private int RotateLeft(int x, int y) {
            return ((int)  (  (uint) (x << (y & (32-1))) |
                              ((uint) x >> (32 - (y & (32-1)))) )
                   );
        }

        /**
        * Perform a right "spin" of the word. The rotation of the given
        * word <em>x</em> is rotated left by <em>y</em> bits.
        * Only the <em>lg(32)</em> low-order bits of <em>y</em>
        * are used to determine the rotation amount. Here it is
        * assumed that the wordsize used is a power of 2.
        *
        * @param  x  word to rotate
        * @param  y    number of bits to rotate % 32
        */
        private int RotateRight(int x, int y) {
            return ((int) (     ((uint) x >> (y & (32-1))) |
                                (uint) (x << (32 - (y & (32-1))))   )
                   );
        }

        private int BytesToWord(
            byte[]  src,
            int     srcOff)
        {
            return (src[srcOff] & 0xff) | ((src[srcOff + 1] & 0xff) << 8)
                | ((src[srcOff + 2] & 0xff) << 16) | ((src[srcOff + 3] & 0xff) << 24);
        }

        private void WordToBytes(
            int    word,
            byte[]  dst,
            int     dstOff)
        {
            dst[dstOff] = (byte)word;
            dst[dstOff + 1] = (byte)(word >> 8);
            dst[dstOff + 2] = (byte)(word >> 16);
            dst[dstOff + 3] = (byte)(word >> 24);
        }
    }
}
