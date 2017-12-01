using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto
{
	/**
	 * a wrapper for block ciphers with a single byte block size, so that they
	 * can be treated like stream ciphers.
	 */
	public class StreamBlockCipher
		: IStreamCipher
	{
		private readonly IBlockCipher cipher;
		private readonly byte[] oneByte = new byte[1];

		/**
		 * basic constructor.
		 *
		 * @param cipher the block cipher to be wrapped.
		 * @exception ArgumentException if the cipher has a block size other than
		 * one.
		 */
		public StreamBlockCipher(
			IBlockCipher cipher)
		{
			if (cipher == null)
				throw new ArgumentNullException("cipher");
			if (cipher.GetBlockSize() != 1)
				throw new ArgumentException("block cipher block size != 1.", "cipher");

			this.cipher = cipher;
		}

		/**
		 * initialise the underlying cipher.
		 *
		 * @param forEncryption true if we are setting up for encryption, false otherwise.
		 * @param param the necessary parameters for the underlying cipher to be initialised.
		 */
		public void Init(
			bool				forEncryption,
			ICipherParameters	parameters)
		{
			cipher.Init(forEncryption, parameters);
		}

		/**
		* return the name of the algorithm we are wrapping.
		*
		* @return the name of the algorithm we are wrapping.
		*/
		public string AlgorithmName
		{
			get { return cipher.AlgorithmName; }
		}

		/**
		* encrypt/decrypt a single byte returning the result.
		*
		* @param in the byte to be processed.
		* @return the result of processing the input byte.
		*/
		public byte ReturnByte(
			byte input)
		{
			oneByte[0] = input;

			cipher.ProcessBlock(oneByte, 0, oneByte, 0);

			return oneByte[0];
		}

		/**
		* process a block of bytes from in putting the result into out.
		*
		* @param in the input byte array.
		* @param inOff the offset into the in array where the data to be processed starts.
		* @param len the number of bytes to be processed.
		* @param out the output buffer the processed bytes go into.
		* @param outOff the offset into the output byte array the processed data stars at.
		* @exception DataLengthException if the output buffer is too small.
		*/
		public void ProcessBytes(
			byte[]	input,
			int		inOff,
			int		length,
			byte[]	output,
			int		outOff)
		{
			if (outOff + length > output.Length)
				throw new DataLengthException("output buffer too small in ProcessBytes()");

			for (int i = 0; i != length; i++)
			{
				cipher.ProcessBlock(input, inOff + i, output, outOff + i);
			}
		}

		/**
		* reset the underlying cipher. This leaves it in the same state
		* it was at after the last init (if there was one).
		*/
		public void Reset()
		{
			cipher.Reset();
		}
	}
}
