using System;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Engines
{
    /**
    * An RC6 engine.
    */
    public class RC6Engine
		: IBlockCipher
    {
        private static readonly int wordSize = 32;
        private static readonly int bytesPerWord = wordSize / 8;

        /*
        * the number of rounds to perform
        */
        private static readonly int _noRounds = 20;

        /*
        * the expanded key array of size 2*(rounds + 1)
        */
        private int [] _S;

        /*
        * our "magic constants" for wordSize 32
        *
        * Pw = Odd((e-2) * 2^wordsize)
        * Qw = Odd((o-2) * 2^wordsize)
        *
        * where e is the base of natural logarithms (2.718281828...)
        * and o is the golden ratio (1.61803398...)
        */
        private static readonly int    P32 = unchecked((int) 0xb7e15163);
        private static readonly int    Q32 = unchecked((int) 0x9e3779b9);

        private static readonly int    LGW = 5;        // log2(32)

        private bool forEncryption;

        /**
        * Create an instance of the RC6 encryption algorithm
        * and set some defaults
        */
        public RC6Engine()
        {
//            _S            = null;
        }

        public virtual string AlgorithmName
        {
            get { return "RC6"; }
        }

        public virtual bool IsPartialBlockOkay
		{
			get { return false; }
		}

        public virtual int GetBlockSize()
        {
            return 4 * bytesPerWord;
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
            if (!(parameters is KeyParameter))
                throw new ArgumentException("invalid parameter passed to RC6 init - " + Platform.GetTypeName(parameters));

            this.forEncryption = forEncryption;

			KeyParameter p = (KeyParameter)parameters;
			SetKey(p.GetKey());
        }

        public virtual int ProcessBlock(
            byte[]	input,
            int		inOff,
            byte[]	output,
            int		outOff)
        {
			int blockSize = GetBlockSize();
			if (_S == null)
				throw new InvalidOperationException("RC6 engine not initialised");

            Check.DataLength(input, inOff, blockSize, "input buffer too short");
            Check.OutputLength(output, outOff, blockSize, "output buffer too short");

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
        * @param inKey the key to be used
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
            //   c = ceil(b/u), where u = wordSize/8 in little-endian order.
            //   In other words, we fill up L using u consecutive key bytes
            //   of K. Any unfilled byte positions in L are zeroed. In the
            //   case that b = c = 0, set c = 1 and L[0] = 0.
            //
            // compute number of dwords
            int c = (key.Length + (bytesPerWord - 1)) / bytesPerWord;
            if (c == 0)
            {
                c = 1;
            }
            int[]   L = new int[(key.Length + bytesPerWord - 1) / bytesPerWord];

            // load all key bytes into array of key dwords
            for (int i = key.Length - 1; i >= 0; i--)
            {
                L[i / bytesPerWord] = (L[i / bytesPerWord] << 8) + (key[i] & 0xff);
            }

            //
            // Phase 2:
            //   Key schedule is placed in a array of 2+2*ROUNDS+2 = 44 dwords.
            //   Initialize S to a particular fixed pseudo-random bit pattern
            //   using an arithmetic progression modulo 2^wordsize determined
            //   by the magic numbers, Pw & Qw.
            //
            _S            = new int[2+2*_noRounds+2];

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

            int A = 0;
            int B = 0;
            int ii = 0, jj = 0;

            for (int k = 0; k < iter; k++)
            {
                A = _S[ii] = RotateLeft(_S[ii] + A + B, 3);
                B =  L[jj] = RotateLeft( L[jj] + A + B, A+B);
                ii = (ii+1) % _S.Length;
                jj = (jj+1) %  L.Length;
            }
        }

        private int EncryptBlock(
            byte[]  input,
            int     inOff,
            byte[]  outBytes,
            int     outOff)
        {
            // load A,B,C and D registers from in.
            int A = BytesToWord(input, inOff);
            int B = BytesToWord(input, inOff + bytesPerWord);
            int C = BytesToWord(input, inOff + bytesPerWord*2);
            int D = BytesToWord(input, inOff + bytesPerWord*3);

            // Do pseudo-round #0: pre-whitening of B and D
            B += _S[0];
            D += _S[1];

            // perform round #1,#2 ... #ROUNDS of encryption
            for (int i = 1; i <= _noRounds; i++)
            {
                int t = 0,u = 0;

                t = B*(2*B+1);
                t = RotateLeft(t,5);

                u = D*(2*D+1);
                u = RotateLeft(u,5);

                A ^= t;
                A = RotateLeft(A,u);
                A += _S[2*i];

                C ^= u;
                C = RotateLeft(C,t);
                C += _S[2*i+1];

                int temp = A;
                A = B;
                B = C;
                C = D;
                D = temp;
            }
            // do pseudo-round #(ROUNDS+1) : post-whitening of A and C
            A += _S[2*_noRounds+2];
            C += _S[2*_noRounds+3];

            // store A, B, C and D registers to out
            WordToBytes(A, outBytes, outOff);
            WordToBytes(B, outBytes, outOff + bytesPerWord);
            WordToBytes(C, outBytes, outOff + bytesPerWord*2);
            WordToBytes(D, outBytes, outOff + bytesPerWord*3);

            return 4 * bytesPerWord;
        }

        private int DecryptBlock(
            byte[]  input,
            int     inOff,
            byte[]  outBytes,
            int     outOff)
        {
            // load A,B,C and D registers from out.
            int A = BytesToWord(input, inOff);
            int B = BytesToWord(input, inOff + bytesPerWord);
            int C = BytesToWord(input, inOff + bytesPerWord*2);
            int D = BytesToWord(input, inOff + bytesPerWord*3);

            // Undo pseudo-round #(ROUNDS+1) : post whitening of A and C
            C -= _S[2*_noRounds+3];
            A -= _S[2*_noRounds+2];

            // Undo round #ROUNDS, .., #2,#1 of encryption
            for (int i = _noRounds; i >= 1; i--)
            {
                int t=0,u = 0;

                int temp = D;
                D = C;
                C = B;
                B = A;
                A = temp;

                t = B*(2*B+1);
                t = RotateLeft(t, LGW);

                u = D*(2*D+1);
                u = RotateLeft(u, LGW);

                C -= _S[2*i+1];
                C = RotateRight(C,t);
                C ^= u;

                A -= _S[2*i];
                A = RotateRight(A,u);
                A ^= t;

            }
            // Undo pseudo-round #0: pre-whitening of B and D
            D -= _S[1];
            B -= _S[0];

            WordToBytes(A, outBytes, outOff);
            WordToBytes(B, outBytes, outOff + bytesPerWord);
            WordToBytes(C, outBytes, outOff + bytesPerWord*2);
            WordToBytes(D, outBytes, outOff + bytesPerWord*3);

            return 4 * bytesPerWord;
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
        * @param x word to rotate
        * @param y number of bits to rotate % wordSize
        */
        private int RotateLeft(int x, int y)
        {
            return ((int)((uint)(x << (y & (wordSize-1)))
				| ((uint) x >> (wordSize - (y & (wordSize-1))))));
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
        private int RotateRight(int x, int y) 
		{
            return ((int)(((uint) x >> (y & (wordSize-1)))
				| (uint)(x << (wordSize - (y & (wordSize-1))))));
        }

        private int BytesToWord(
            byte[]	src,
            int		srcOff)
        {
            int word = 0;

            for (int i = bytesPerWord - 1; i >= 0; i--)
            {
                word = (word << 8) + (src[i + srcOff] & 0xff);
            }

            return word;
        }

        private void WordToBytes(
            int		word,
            byte[]	dst,
            int		dstOff)
        {
            for (int i = 0; i < bytesPerWord; i++)
            {
                dst[i + dstOff] = (byte)word;
                word = (int) ((uint) word >> 8);
            }
        }
    }
}
