using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Modes
{
    /**
    * implements Cipher-Block-Chaining (CBC) mode on top of a simple cipher.
    */
    public class CbcBlockCipher
		: IBlockCipher
    {
        private byte[]			IV, cbcV, cbcNextV;
		private int				blockSize;
        private IBlockCipher	cipher;
        private bool			encrypting;

        /**
        * Basic constructor.
        *
        * @param cipher the block cipher to be used as the basis of chaining.
        */
        public CbcBlockCipher(
            IBlockCipher cipher)
        {
            this.cipher = cipher;
            this.blockSize = cipher.GetBlockSize();

            this.IV = new byte[blockSize];
            this.cbcV = new byte[blockSize];
            this.cbcNextV = new byte[blockSize];
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
        * Initialise the cipher and, possibly, the initialisation vector (IV).
        * If an IV isn't passed as part of the parameter, the IV will be all zeros.
        *
        * @param forEncryption if true the cipher is initialised for
        *  encryption, if false for decryption.
        * @param param the key and other data required by the cipher.
        * @exception ArgumentException if the parameters argument is
        * inappropriate.
        */
        public void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            bool oldEncrypting = this.encrypting;

            this.encrypting = forEncryption;

            if (parameters is ParametersWithIV)
            {
                ParametersWithIV ivParam = (ParametersWithIV)parameters;
                byte[]      iv = ivParam.GetIV();

                if (iv.Length != blockSize)
                {
                    throw new ArgumentException("initialisation vector must be the same length as block size");
                }

                Array.Copy(iv, 0, IV, 0, iv.Length);

				parameters = ivParam.Parameters;
            }

			Reset();

            // if null it's an IV changed only.
            if (parameters != null)
            {
                cipher.Init(encrypting, parameters);
            }
            else if (oldEncrypting != encrypting)
            {
                throw new ArgumentException("cannot change encrypting state without providing key.");
            }
        }

		/**
        * return the algorithm name and mode.
        *
        * @return the name of the underlying algorithm followed by "/CBC".
        */
        public string AlgorithmName
        {
            get { return cipher.AlgorithmName + "/CBC"; }
        }

		public bool IsPartialBlockOkay
		{
			get { return false; }
		}

		/**
        * return the block size of the underlying cipher.
        *
        * @return the block size of the underlying cipher.
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
            return (encrypting)
				?	EncryptBlock(input, inOff, output, outOff)
				:	DecryptBlock(input, inOff, output, outOff);
        }

        /**
        * reset the chaining vector back to the IV and reset the underlying
        * cipher.
        */
        public void Reset()
        {
            Array.Copy(IV, 0, cbcV, 0, IV.Length);
			Array.Clear(cbcNextV, 0, cbcNextV.Length);

            cipher.Reset();
        }

        /**
        * Do the appropriate chaining step for CBC mode encryption.
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
            byte[]      input,
            int         inOff,
            byte[]      outBytes,
            int         outOff)
        {
            if ((inOff + blockSize) > input.Length)
            {
                throw new DataLengthException("input buffer too short");
            }

            /*
            * XOR the cbcV and the input,
            * then encrypt the cbcV
            */
            for (int i = 0; i < blockSize; i++)
            {
                cbcV[i] ^= input[inOff + i];
            }

            int length = cipher.ProcessBlock(cbcV, 0, outBytes, outOff);

            /*
            * copy ciphertext to cbcV
            */
            Array.Copy(outBytes, outOff, cbcV, 0, cbcV.Length);

            return length;
        }

        /**
        * Do the appropriate chaining step for CBC mode decryption.
        *
        * @param in the array containing the data to be decrypted.
        * @param inOff offset into the in array the data starts at.
        * @param out the array the decrypted data will be copied into.
        * @param outOff the offset into the out array the output will start at.
        * @exception DataLengthException if there isn't enough data in in, or
        * space in out.
        * @exception InvalidOperationException if the cipher isn't initialised.
        * @return the number of bytes processed and produced.
        */
        private int DecryptBlock(
            byte[]      input,
            int         inOff,
            byte[]      outBytes,
            int         outOff)
        {
            if ((inOff + blockSize) > input.Length)
            {
                throw new DataLengthException("input buffer too short");
            }

            Array.Copy(input, inOff, cbcNextV, 0, blockSize);

            int length = cipher.ProcessBlock(input, inOff, outBytes, outOff);

            /*
            * XOR the cbcV and the output
            */
            for (int i = 0; i < blockSize; i++)
            {
                outBytes[outOff + i] ^= cbcV[i];
            }

            /*
            * swap the back up buffer into next position
            */
            byte[]  tmp;

            tmp = cbcV;
            cbcV = cbcNextV;
            cbcNextV = tmp;

            return length;
        }
    }

}
