using System;

using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Macs
{
    /**
    * implements a Cipher-FeedBack (CFB) mode on top of a simple cipher.
    */
    class MacCFBBlockCipher
		: IBlockCipher
    {
        private byte[] IV;
        private byte[] cfbV;
        private byte[] cfbOutV;

		private readonly int blockSize;
        private readonly IBlockCipher cipher;

		/**
        * Basic constructor.
        *
        * @param cipher the block cipher to be used as the basis of the
        * feedback mode.
        * @param blockSize the block size in bits (note: a multiple of 8)
        */
        public MacCFBBlockCipher(
            IBlockCipher	cipher,
            int				bitBlockSize)
        {
            this.cipher = cipher;
            this.blockSize = bitBlockSize / 8;

            this.IV = new byte[cipher.GetBlockSize()];
            this.cfbV = new byte[cipher.GetBlockSize()];
            this.cfbOutV = new byte[cipher.GetBlockSize()];
        }

		/**
        * Initialise the cipher and, possibly, the initialisation vector (IV).
        * If an IV isn't passed as part of the parameter, the IV will be all zeros.
        * An IV which is too short is handled in FIPS compliant fashion.
        *
        * @param param the key and other data required by the cipher.
        * @exception ArgumentException if the parameters argument is
        * inappropriate.
        */
		public void Init(
			bool				forEncryption,
            ICipherParameters	parameters)
        {
			if (parameters is ParametersWithIV)
            {
                ParametersWithIV ivParam = (ParametersWithIV)parameters;
                byte[] iv = ivParam.GetIV();

                if (iv.Length < IV.Length)
                {
                    Array.Copy(iv, 0, IV, IV.Length - iv.Length, iv.Length);
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
        * return the algorithm name and mode.
        *
        * @return the name of the underlying algorithm followed by "/CFB"
        * and the block size in bits.
        */
        public string AlgorithmName
        {
			get { return cipher.AlgorithmName + "/CFB" + (blockSize * 8); }
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
            return blockSize;
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
            byte[]	outBytes,
            int		outOff)
        {
            if ((inOff + blockSize) > input.Length)
                throw new DataLengthException("input buffer too short");

			if ((outOff + blockSize) > outBytes.Length)
                throw new DataLengthException("output buffer too short");

			cipher.ProcessBlock(cfbV, 0, cfbOutV, 0);

            //
            // XOR the cfbV with the plaintext producing the cipher text
            //
            for (int i = 0; i < blockSize; i++)
            {
                outBytes[outOff + i] = (byte)(cfbOutV[i] ^ input[inOff + i]);
            }

			//
            // change over the input block.
            //
            Array.Copy(cfbV, blockSize, cfbV, 0, cfbV.Length - blockSize);
            Array.Copy(outBytes, outOff, cfbV, cfbV.Length - blockSize, blockSize);

			return blockSize;
        }

		/**
        * reset the chaining vector back to the IV and reset the underlying
        * cipher.
        */
        public void Reset()
        {
			IV.CopyTo(cfbV, 0);

			cipher.Reset();
        }

		public void GetMacBlock(
            byte[] mac)
        {
            cipher.ProcessBlock(cfbV, 0, mac, 0);
        }
    }

	public class CfbBlockCipherMac
		: IMac
    {
        private byte[] mac;
        private byte[] Buffer;
        private int bufOff;
        private MacCFBBlockCipher cipher;
        private IBlockCipherPadding padding;
        private int macSize;

		/**
        * create a standard MAC based on a CFB block cipher. This will produce an
        * authentication code half the length of the block size of the cipher, with
        * the CFB mode set to 8 bits.
        *
        * @param cipher the cipher to be used as the basis of the MAC generation.
        */
        public CfbBlockCipherMac(
            IBlockCipher cipher)
			: this(cipher, 8, (cipher.GetBlockSize() * 8) / 2, null)
		{
		}

		/**
        * create a standard MAC based on a CFB block cipher. This will produce an
        * authentication code half the length of the block size of the cipher, with
        * the CFB mode set to 8 bits.
        *
        * @param cipher the cipher to be used as the basis of the MAC generation.
        * @param padding the padding to be used.
        */
        public CfbBlockCipherMac(
            IBlockCipher		cipher,
            IBlockCipherPadding	padding)
			: this(cipher, 8, (cipher.GetBlockSize() * 8) / 2, padding)
		{
		}

		/**
        * create a standard MAC based on a block cipher with the size of the
        * MAC been given in bits. This class uses CFB mode as the basis for the
        * MAC generation.
        * <p>
        * Note: the size of the MAC must be at least 24 bits (FIPS Publication 81),
        * or 16 bits if being used as a data authenticator (FIPS Publication 113),
        * and in general should be less than the size of the block cipher as it reduces
        * the chance of an exhaustive attack (see Handbook of Applied Cryptography).
        * </p>
        * @param cipher the cipher to be used as the basis of the MAC generation.
        * @param cfbBitSize the size of an output block produced by the CFB mode.
        * @param macSizeInBits the size of the MAC in bits, must be a multiple of 8.
        */
        public CfbBlockCipherMac(
            IBlockCipher	cipher,
            int				cfbBitSize,
            int				macSizeInBits)
			: this(cipher, cfbBitSize, macSizeInBits, null)
		{
		}

		/**
        * create a standard MAC based on a block cipher with the size of the
        * MAC been given in bits. This class uses CFB mode as the basis for the
        * MAC generation.
        * <p>
        * Note: the size of the MAC must be at least 24 bits (FIPS Publication 81),
        * or 16 bits if being used as a data authenticator (FIPS Publication 113),
        * and in general should be less than the size of the block cipher as it reduces
        * the chance of an exhaustive attack (see Handbook of Applied Cryptography).
        * </p>
        * @param cipher the cipher to be used as the basis of the MAC generation.
        * @param cfbBitSize the size of an output block produced by the CFB mode.
        * @param macSizeInBits the size of the MAC in bits, must be a multiple of 8.
        * @param padding a padding to be used.
        */
        public CfbBlockCipherMac(
            IBlockCipher		cipher,
            int					cfbBitSize,
            int					macSizeInBits,
            IBlockCipherPadding	padding)
        {
            if ((macSizeInBits % 8) != 0)
                throw new ArgumentException("MAC size must be multiple of 8");

			mac = new byte[cipher.GetBlockSize()];

			this.cipher = new MacCFBBlockCipher(cipher, cfbBitSize);
            this.padding = padding;
            this.macSize = macSizeInBits / 8;

			Buffer = new byte[this.cipher.GetBlockSize()];
            bufOff = 0;
        }

		public string AlgorithmName
        {
            get { return cipher.AlgorithmName; }
        }

		public void Init(
            ICipherParameters parameters)
        {
            Reset();

			cipher.Init(true, parameters);
        }

		public int GetMacSize()
        {
            return macSize;
        }

		public void Update(
            byte input)
        {
            if (bufOff == Buffer.Length)
            {
				cipher.ProcessBlock(Buffer, 0, mac, 0);
                bufOff = 0;
            }

			Buffer[bufOff++] = input;
        }

		public void BlockUpdate(
            byte[]	input,
            int		inOff,
            int		len)
        {
            if (len < 0)
                throw new ArgumentException("Can't have a negative input length!");

			int blockSize = cipher.GetBlockSize();
            int resultLen = 0;
            int gapLen = blockSize - bufOff;

			if (len > gapLen)
            {
                Array.Copy(input, inOff, Buffer, bufOff, gapLen);

				resultLen += cipher.ProcessBlock(Buffer, 0, mac, 0);

				bufOff = 0;
                len -= gapLen;
                inOff += gapLen;

				while (len > blockSize)
                {
                    resultLen += cipher.ProcessBlock(input, inOff, mac, 0);

					len -= blockSize;
                    inOff += blockSize;
                }
            }

			Array.Copy(input, inOff, Buffer, bufOff, len);

			bufOff += len;
        }

		public int DoFinal(
            byte[]	output,
            int		outOff)
        {
            int blockSize = cipher.GetBlockSize();

            // pad with zeroes
            if (this.padding == null)
            {
                while (bufOff < blockSize)
                {
                    Buffer[bufOff++] = 0;
                }
            }
            else
            {
                padding.AddPadding(Buffer, bufOff);
            }

			cipher.ProcessBlock(Buffer, 0, mac, 0);

			cipher.GetMacBlock(mac);

			Array.Copy(mac, 0, output, outOff, macSize);

			Reset();

			return macSize;
        }

        /**
        * Reset the mac generator.
        */
        public void Reset()
        {
            // Clear the buffer.
			Array.Clear(Buffer, 0, Buffer.Length);
            bufOff = 0;

			// Reset the underlying cipher.
            cipher.Reset();
        }
    }

}
