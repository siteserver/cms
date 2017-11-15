using System;

using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;

namespace Org.BouncyCastle.Crypto.Macs
{
    /**
    * standard CBC Block Cipher MAC - if no padding is specified the default of
    * pad of zeroes is used.
    */
    public class CbcBlockCipherMac
		: IMac
    {
        private byte[] buf;
        private int bufOff;
        private IBlockCipher cipher;
        private IBlockCipherPadding padding;
		private int macSize;

		/**
        * create a standard MAC based on a CBC block cipher. This will produce an
        * authentication code half the length of the block size of the cipher.
        *
        * @param cipher the cipher to be used as the basis of the MAC generation.
        */
        public CbcBlockCipherMac(
			IBlockCipher cipher)
			: this(cipher, (cipher.GetBlockSize() * 8) / 2, null)
		{
		}

		/**
        * create a standard MAC based on a CBC block cipher. This will produce an
        * authentication code half the length of the block size of the cipher.
        *
        * @param cipher the cipher to be used as the basis of the MAC generation.
        * @param padding the padding to be used to complete the last block.
        */
        public CbcBlockCipherMac(
            IBlockCipher		cipher,
            IBlockCipherPadding	padding)
        : this(cipher, (cipher.GetBlockSize() * 8) / 2, padding)
		{
		}

		/**
        * create a standard MAC based on a block cipher with the size of the
        * MAC been given in bits. This class uses CBC mode as the basis for the
        * MAC generation.
        * <p>
        * Note: the size of the MAC must be at least 24 bits (FIPS Publication 81),
        * or 16 bits if being used as a data authenticator (FIPS Publication 113),
        * and in general should be less than the size of the block cipher as it reduces
        * the chance of an exhaustive attack (see Handbook of Applied Cryptography).
        * </p>
        * @param cipher the cipher to be used as the basis of the MAC generation.
        * @param macSizeInBits the size of the MAC in bits, must be a multiple of 8.
        */
        public CbcBlockCipherMac(
            IBlockCipher	cipher,
            int				macSizeInBits)
			: this(cipher, macSizeInBits, null)
		{
		}

		/**
        * create a standard MAC based on a block cipher with the size of the
        * MAC been given in bits. This class uses CBC mode as the basis for the
        * MAC generation.
        * <p>
        * Note: the size of the MAC must be at least 24 bits (FIPS Publication 81),
        * or 16 bits if being used as a data authenticator (FIPS Publication 113),
        * and in general should be less than the size of the block cipher as it reduces
        * the chance of an exhaustive attack (see Handbook of Applied Cryptography).
        * </p>
        * @param cipher the cipher to be used as the basis of the MAC generation.
        * @param macSizeInBits the size of the MAC in bits, must be a multiple of 8.
        * @param padding the padding to be used to complete the last block.
        */
        public CbcBlockCipherMac(
            IBlockCipher		cipher,
            int					macSizeInBits,
            IBlockCipherPadding	padding)
        {
            if ((macSizeInBits % 8) != 0)
                throw new ArgumentException("MAC size must be multiple of 8");

			this.cipher = new CbcBlockCipher(cipher);
            this.padding = padding;
            this.macSize = macSizeInBits / 8;

			buf = new byte[cipher.GetBlockSize()];
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
			if (bufOff == buf.Length)
            {
				cipher.ProcessBlock(buf, 0, buf, 0);
                bufOff = 0;
            }

			buf[bufOff++] = input;
        }

        public void BlockUpdate(
            byte[]	input,
            int		inOff,
            int		len)
        {
            if (len < 0)
                throw new ArgumentException("Can't have a negative input length!");

			int blockSize = cipher.GetBlockSize();
            int gapLen = blockSize - bufOff;

            if (len > gapLen)
            {
                Array.Copy(input, inOff, buf, bufOff, gapLen);

                cipher.ProcessBlock(buf, 0, buf, 0);

                bufOff = 0;
                len -= gapLen;
                inOff += gapLen;

                while (len > blockSize)
                {
                    cipher.ProcessBlock(input, inOff, buf, 0);

                    len -= blockSize;
                    inOff += blockSize;
                }
            }

            Array.Copy(input, inOff, buf, bufOff, len);

            bufOff += len;
        }

        public int DoFinal(
            byte[]	output,
            int		outOff)
        {
            int blockSize = cipher.GetBlockSize();

            if (padding == null)
            {
                // pad with zeroes
                while (bufOff < blockSize)
                {
                    buf[bufOff++] = 0;
                }
            }
            else
            {
                if (bufOff == blockSize)
                {
                    cipher.ProcessBlock(buf, 0, buf, 0);
                    bufOff = 0;
                }

				padding.AddPadding(buf, bufOff);
            }

			cipher.ProcessBlock(buf, 0, buf, 0);

			Array.Copy(buf, 0, output, outOff, macSize);

			Reset();

			return macSize;
        }

		/**
        * Reset the mac generator.
        */
        public void Reset()
        {
            // Clear the buffer.
			Array.Clear(buf, 0, buf.Length);
			bufOff = 0;

			// Reset the underlying cipher.
            cipher.Reset();
        }
    }
}
