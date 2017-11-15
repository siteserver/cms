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
    * This implementation is set to work with a 64 bit word size.</p>
    */
    public class RC564Engine
		: IBlockCipher
    {
        private static readonly int wordSize = 64;
        private static readonly int bytesPerWord = wordSize / 8;

        /*
        * the number of rounds to perform
        */
        private int _noRounds;

        /*
        * the expanded key array of size 2*(rounds + 1)
        */
        private long [] _S;

        /*
        * our "magic constants" for wordSize 62
        *
        * Pw = Odd((e-2) * 2^wordsize)
        * Qw = Odd((o-2) * 2^wordsize)
        *
        * where e is the base of natural logarithms (2.718281828...)
        * and o is the golden ratio (1.61803398...)
        */
        private static readonly long P64 = unchecked( (long) 0xb7e151628aed2a6bL);
        private static readonly long Q64 = unchecked( (long) 0x9e3779b97f4a7c15L);

        private bool forEncryption;

        /**
        * Create an instance of the RC5 encryption algorithm
        * and set some defaults
        */
        public RC564Engine()
        {
            _noRounds     = 12;
//            _S            = null;
        }

        public virtual string AlgorithmName
        {
            get { return "RC5-64"; }
        }

        public virtual bool IsPartialBlockOkay
		{
			get { return false; }
		}

        public virtual int GetBlockSize()
        {
            return 2 * bytesPerWord;
        }

        /**
        * initialise a RC5-64 cipher.
        *
        * @param forEncryption whether or not we are for encryption.
        * @param parameters the parameters required to set up the cipher.
        * @exception ArgumentException if the parameters argument is
        * inappropriate.
        */
        public virtual void Init(
            bool             forEncryption,
            ICipherParameters    parameters)
        {
            if (!(typeof(RC5Parameters).IsInstanceOfType(parameters)))
            {
                throw new ArgumentException("invalid parameter passed to RC564 init - " + Platform.GetTypeName(parameters));
            }

            RC5Parameters       p = (RC5Parameters)parameters;

            this.forEncryption = forEncryption;

            _noRounds     = p.Rounds;

            SetKey(p.GetKey());
        }

        public virtual int ProcessBlock(
            byte[]  input,
            int     inOff,
            byte[]  output,
            int     outOff)
        {
            return (forEncryption) ? EncryptBlock(input, inOff, output, outOff)
                                        : DecryptBlock(input, inOff, output, outOff);
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
            byte[]      key)
        {
            //
            // KEY EXPANSION:
            //
            // There are 3 phases to the key expansion.
            //
            // Phase 1:
            //   Copy the secret key K[0...b-1] into an array L[0..c-1] of
            //   c = ceil(b/u), where u = wordSize/8 in little-endian order.
            //   In other words, we fill up L using u consecutive key bytes
            //   of K. Any unfilled byte positions in L are zeroed. In the
            //   case that b = c = 0, set c = 1 and L[0] = 0.
            //
            long[]   L = new long[(key.Length + (bytesPerWord - 1)) / bytesPerWord];

            for (int i = 0; i != key.Length; i++)
            {
                L[i / bytesPerWord] += (long)(key[i] & 0xff) << (8 * (i % bytesPerWord));
            }

            //
            // Phase 2:
            //   Initialize S to a particular fixed pseudo-random bit pattern
            //   using an arithmetic progression modulo 2^wordsize determined
            //   by the magic numbers, Pw & Qw.
            //
            _S            = new long[2*(_noRounds + 1)];

            _S[0] = P64;
            for (int i=1; i < _S.Length; i++)
            {
                _S[i] = (_S[i-1] + Q64);
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

            long A = 0, B = 0;
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
        * @param  in      in byte buffer containing data to encrypt
        * @param  inOff   offset into src buffer
        * @param  out     out buffer where encrypted data is written
        * @param  outOff  offset into out buffer
        */
        private int EncryptBlock(
            byte[]  input,
            int     inOff,
            byte[]  outBytes,
            int     outOff)
        {
            long A = BytesToWord(input, inOff) + _S[0];
            long B = BytesToWord(input, inOff + bytesPerWord) + _S[1];

            for (int i = 1; i <= _noRounds; i++)
            {
                A = RotateLeft(A ^ B, B) + _S[2*i];
                B = RotateLeft(B ^ A, A) + _S[2*i+1];
            }

            WordToBytes(A, outBytes, outOff);
            WordToBytes(B, outBytes, outOff + bytesPerWord);

            return 2 * bytesPerWord;
        }

        private int DecryptBlock(
            byte[]  input,
            int     inOff,
            byte[]  outBytes,
            int     outOff)
        {
            long A = BytesToWord(input, inOff);
            long B = BytesToWord(input, inOff + bytesPerWord);

            for (int i = _noRounds; i >= 1; i--)
            {
                B = RotateRight(B - _S[2*i+1], A) ^ A;
                A = RotateRight(A - _S[2*i],   B) ^ B;
            }

            WordToBytes(A - _S[0], outBytes, outOff);
            WordToBytes(B - _S[1], outBytes, outOff + bytesPerWord);

            return 2 * bytesPerWord;
        }


        //////////////////////////////////////////////////////////////
        //
        // PRIVATE Helper Methods
        //
        //////////////////////////////////////////////////////////////

        /**
        * Perform a left "spin" of the word. The rotation of the given
        * word <em>x</em> is rotated left by <em>y</em> bits.
        * Only the <em>lg(wordSize)</em> low-order bits of <em>y</em>
        * are used to determine the rotation amount. Here it is
        * assumed that the wordsize used is a power of 2.
        *
        * @param  x  word to rotate
        * @param  y    number of bits to rotate % wordSize
        */
        private long RotateLeft(long x, long y) {
            return ((long) (    (ulong) (x << (int) (y & (wordSize-1))) |
                                ((ulong) x >> (int) (wordSize - (y & (wordSize-1)))))
                   );
        }

        /**
        * Perform a right "spin" of the word. The rotation of the given
        * word <em>x</em> is rotated left by <em>y</em> bits.
        * Only the <em>lg(wordSize)</em> low-order bits of <em>y</em>
        * are used to determine the rotation amount. Here it is
        * assumed that the wordsize used is a power of 2.
        *
        * @param x word to rotate
        * @param y number of bits to rotate % wordSize
        */
        private long RotateRight(long x, long y) {
            return ((long) (    ((ulong) x >> (int) (y & (wordSize-1))) |
                                (ulong) (x << (int) (wordSize - (y & (wordSize-1)))))
                   );
        }

        private long BytesToWord(
            byte[]  src,
            int     srcOff)
        {
            long    word = 0;

            for (int i = bytesPerWord - 1; i >= 0; i--)
            {
                word = (word << 8) + (src[i + srcOff] & 0xff);
            }

            return word;
        }

        private void WordToBytes(
            long    word,
            byte[]  dst,
            int     dstOff)
        {
            for (int i = 0; i < bytesPerWord; i++)
            {
                dst[i + dstOff] = (byte)word;
                word = (long) ((ulong) word >> 8);
            }
        }
    }
}
