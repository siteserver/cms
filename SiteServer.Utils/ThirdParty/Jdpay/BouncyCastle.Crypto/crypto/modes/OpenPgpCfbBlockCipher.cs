using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Modes
{
    /**
    * Implements OpenPGP's rather strange version of Cipher-FeedBack (CFB) mode
    * on top of a simple cipher. This class assumes the IV has been prepended
    * to the data stream already, and just accomodates the reset after
    * (blockSize + 2) bytes have been read.
    * <p>
    * For further info see <a href="http://www.ietf.org/rfc/rfc2440.html">RFC 2440</a>.
	* </p>
    */
    public class OpenPgpCfbBlockCipher
        : IBlockCipher
    {
        private byte[] IV;
        private byte[] FR;
        private byte[] FRE;

		private readonly IBlockCipher cipher;
		private readonly int blockSize;

		private int count;
        private bool forEncryption;

		/**
        * Basic constructor.
        *
        * @param cipher the block cipher to be used as the basis of the
        * feedback mode.
        */
        public OpenPgpCfbBlockCipher(
            IBlockCipher cipher)
        {
            this.cipher = cipher;

            this.blockSize = cipher.GetBlockSize();
            this.IV = new byte[blockSize];
            this.FR = new byte[blockSize];
            this.FRE = new byte[blockSize];
        }

		/**
        * return the underlying block cipher that we are wrapping.
        *
        * @return the underlying block cipher that we are wrapping.
        */
        public IBlockCipher GetUnderlyingCipher()
        {
            return cipher;
        }

		/**
        * return the algorithm name and mode.
        *
        * @return the name of the underlying algorithm followed by "/PGPCFB"
        * and the block size in bits.
        */
        public string AlgorithmName
        {
            get { return cipher.AlgorithmName + "/OpenPGPCFB"; }
        }

		public bool IsPartialBlockOkay
		{
			get { return true; }
		}

		/**
        * return the block size we are operating at.
        *
        * @return the block size we are operating at (in bytes).
        */
        public int GetBlockSize()
        {
            return cipher.GetBlockSize();
        }

		/**
        * Process one block of input from the array in and write it to
        * the out array.
        *
        * @param in the array containing the input data.
        * @param inOff offset into the in array the data starts at.
        * @param out the array the output data will be copied into.
        * @param outOff the offset into the out array the output will start at.
        * @exception DataLengthException if there isn't enough data in in, or
        * space in out.
        * @exception InvalidOperationException if the cipher isn't initialised.
        * @return the number of bytes processed and produced.
        */
        public int ProcessBlock(
            byte[]	input,
            int		inOff,
            byte[]	output,
            int		outOff)
        {
            return (forEncryption) ? EncryptBlock(input, inOff, output, outOff) : DecryptBlock(input, inOff, output, outOff);
        }

		/**
        * reset the chaining vector back to the IV and reset the underlying
        * cipher.
        */
        public void Reset()
        {
            count = 0;

			Array.Copy(IV, 0, FR, 0, FR.Length);

			cipher.Reset();
        }

        /**
        * Initialise the cipher and, possibly, the initialisation vector (IV).
        * If an IV isn't passed as part of the parameter, the IV will be all zeros.
        * An IV which is too short is handled in FIPS compliant fashion.
        *
        * @param forEncryption if true the cipher is initialised for
        *  encryption, if false for decryption.
        * @param parameters the key and other data required by the cipher.
        * @exception ArgumentException if the parameters argument is
        * inappropriate.
        */
        public void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            this.forEncryption = forEncryption;

            if (parameters is ParametersWithIV)
            {
                ParametersWithIV ivParam = (ParametersWithIV)parameters;
                byte[] iv = ivParam.GetIV();

                if (iv.Length < IV.Length)
                {
                    // prepend the supplied IV with zeros (per FIPS PUB 81)
                    Array.Copy(iv, 0, IV, IV.Length - iv.Length, iv.Length);
                    for (int i = 0; i < IV.Length - iv.Length; i++)
                    {
                        IV[i] = 0;
                    }
                }
                else
                {
                    Array.Copy(iv, 0, IV, 0, IV.Length);
                }

                parameters = ivParam.Parameters;
            }

            Reset();

            cipher.Init(true, parameters);
        }

		/**
        * Encrypt one byte of data according to CFB mode.
        * @param data the byte to encrypt
        * @param blockOff offset in the current block
        * @returns the encrypted byte
        */
        private byte EncryptByte(byte data, int blockOff)
        {
            return (byte)(FRE[blockOff] ^ data);
        }

		/**
        * Do the appropriate processing for CFB IV mode encryption.
        *
        * @param in the array containing the data to be encrypted.
        * @param inOff offset into the in array the data starts at.
        * @param out the array the encrypted data will be copied into.
        * @param outOff the offset into the out array the output will start at.
        * @exception DataLengthException if there isn't enough data in in, or
        * space in out.
        * @exception InvalidOperationException if the cipher isn't initialised.
        * @return the number of bytes processed and produced.
        */
        private int EncryptBlock(
            byte[]	input,
            int		inOff,
            byte[]	outBytes,
            int		outOff)
        {
            if ((inOff + blockSize) > input.Length)
            {
                throw new DataLengthException("input buffer too short");
            }

            if ((outOff + blockSize) > outBytes.Length)
            {
                throw new DataLengthException("output buffer too short");
            }

            if (count > blockSize)
            {
                FR[blockSize - 2] = outBytes[outOff] = EncryptByte(input[inOff], blockSize - 2);
                FR[blockSize - 1] = outBytes[outOff + 1] = EncryptByte(input[inOff + 1], blockSize - 1);

                cipher.ProcessBlock(FR, 0, FRE, 0);

                for (int n = 2; n < blockSize; n++)
                {
					FR[n - 2] = outBytes[outOff + n] = EncryptByte(input[inOff + n], n - 2);
                }
            }
            else if (count == 0)
            {
                cipher.ProcessBlock(FR, 0, FRE, 0);

				for (int n = 0; n < blockSize; n++)
                {
					FR[n] = outBytes[outOff + n] = EncryptByte(input[inOff + n], n);
                }

				count += blockSize;
            }
            else if (count == blockSize)
            {
                cipher.ProcessBlock(FR, 0, FRE, 0);

                outBytes[outOff] = EncryptByte(input[inOff], 0);
                outBytes[outOff + 1] = EncryptByte(input[inOff + 1], 1);

                //
                // do reset
                //
                Array.Copy(FR, 2, FR, 0, blockSize - 2);
                Array.Copy(outBytes, outOff, FR, blockSize - 2, 2);

                cipher.ProcessBlock(FR, 0, FRE, 0);

                for (int n = 2; n < blockSize; n++)
                {
					FR[n - 2] = outBytes[outOff + n] = EncryptByte(input[inOff + n], n - 2);
                }

				count += blockSize;
            }

            return blockSize;
        }

        /**
        * Do the appropriate processing for CFB IV mode decryption.
        *
        * @param in the array containing the data to be decrypted.
        * @param inOff offset into the in array the data starts at.
        * @param out the array the encrypted data will be copied into.
        * @param outOff the offset into the out array the output will start at.
        * @exception DataLengthException if there isn't enough data in in, or
        * space in out.
        * @exception InvalidOperationException if the cipher isn't initialised.
        * @return the number of bytes processed and produced.
        */
        private int DecryptBlock(
            byte[]	input,
            int		inOff,
            byte[]	outBytes,
            int		outOff)
        {
            if ((inOff + blockSize) > input.Length)
            {
                throw new DataLengthException("input buffer too short");
            }

            if ((outOff + blockSize) > outBytes.Length)
            {
                throw new DataLengthException("output buffer too short");
            }

            if (count > blockSize)
            {
				byte inVal = input[inOff];
				FR[blockSize - 2] = inVal;
				outBytes[outOff] = EncryptByte(inVal, blockSize - 2);

				inVal = input[inOff + 1];
				FR[blockSize - 1] = inVal;
				outBytes[outOff + 1] = EncryptByte(inVal, blockSize - 1);

                cipher.ProcessBlock(FR, 0, FRE, 0);

                for (int n = 2; n < blockSize; n++)
                {
					inVal = input[inOff + n];
					FR[n - 2] = inVal;
					outBytes[outOff + n] = EncryptByte(inVal, n - 2);
				}
            }
            else if (count == 0)
            {
                cipher.ProcessBlock(FR, 0, FRE, 0);

                for (int n = 0; n < blockSize; n++)
                {
                    FR[n] = input[inOff + n];
                    outBytes[n] = EncryptByte(input[inOff + n], n);
                }

                count += blockSize;
            }
            else if (count == blockSize)
            {
                cipher.ProcessBlock(FR, 0, FRE, 0);

				byte inVal1 = input[inOff];
				byte inVal2 = input[inOff + 1];
				outBytes[outOff    ] = EncryptByte(inVal1, 0);
				outBytes[outOff + 1] = EncryptByte(inVal2, 1);

                Array.Copy(FR, 2, FR, 0, blockSize - 2);

				FR[blockSize - 2] = inVal1;
				FR[blockSize - 1] = inVal2;

                cipher.ProcessBlock(FR, 0, FRE, 0);

                for (int n = 2; n < blockSize; n++)
                {
					byte inVal = input[inOff + n];
					FR[n - 2] = inVal;
					outBytes[outOff + n] = EncryptByte(inVal, n - 2);
                }

                count += blockSize;
            }

            return blockSize;
        }
    }
}
