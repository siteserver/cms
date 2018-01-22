using System;
using System.Diagnostics;

using Org.BouncyCastle.Crypto.Engines;

namespace Org.BouncyCastle.Crypto
{
    /**
    * a buffer wrapper for an asymmetric block cipher, allowing input
    * to be accumulated in a piecemeal fashion until final processing.
    */
    public class BufferedAsymmetricBlockCipher
		: BufferedCipherBase
    {
		private readonly IAsymmetricBlockCipher cipher;

		private byte[] buffer;
		private int bufOff;

		/**
        * base constructor.
        *
        * @param cipher the cipher this buffering object wraps.
        */
        public BufferedAsymmetricBlockCipher(
            IAsymmetricBlockCipher cipher)
        {
            this.cipher = cipher;
		}

		/**
        * return the amount of data sitting in the buffer.
        *
        * @return the amount of data sitting in the buffer.
        */
        internal int GetBufferPosition()
        {
            return bufOff;
        }

		public override string AlgorithmName
        {
            get { return cipher.AlgorithmName; }
        }

		public override int GetBlockSize()
        {
			return cipher.GetInputBlockSize();
        }

		public override int GetOutputSize(
			int length)
		{
			return cipher.GetOutputBlockSize();
		}

		public override int GetUpdateOutputSize(
			int length)
		{
			return 0;
		}

		/**
        * initialise the buffer and the underlying cipher.
        *
        * @param forEncryption if true the cipher is initialised for
        *  encryption, if false for decryption.
        * @param param the key and other data required by the cipher.
        */
        public override void Init(
            bool				forEncryption,
            ICipherParameters	parameters)
        {
			Reset();

			cipher.Init(forEncryption, parameters);

			//
			// we allow for an extra byte where people are using their own padding
			// mechanisms on a raw cipher.
			//
			this.buffer = new byte[cipher.GetInputBlockSize() + (forEncryption ? 1 : 0)];
			this.bufOff = 0;
        }

		public override byte[] ProcessByte(
			byte input)
		{
			if (bufOff >= buffer.Length)
				throw new DataLengthException("attempt to process message to long for cipher");

			buffer[bufOff++] = input;
			return null;
		}

		public override byte[] ProcessBytes(
			byte[]	input,
			int		inOff,
			int		length)
		{
			if (length < 1)
				return null;

			if (input == null)
				throw new ArgumentNullException("input");
			if (bufOff + length > buffer.Length)
				throw new DataLengthException("attempt to process message to long for cipher");

			Array.Copy(input, inOff, buffer, bufOff, length);
			bufOff += length;
			return null;
		}

		/**
        * process the contents of the buffer using the underlying
        * cipher.
        *
        * @return the result of the encryption/decryption process on the
        * buffer.
        * @exception InvalidCipherTextException if we are given a garbage block.
        */
        public override byte[] DoFinal()
        {
			byte[] outBytes = bufOff > 0
				?	cipher.ProcessBlock(buffer, 0, bufOff)
				:	EmptyBuffer;

			Reset();

			return outBytes;
        }

		public override byte[] DoFinal(
			byte[]	input,
			int		inOff,
			int		length)
		{
			ProcessBytes(input, inOff, length);
			return DoFinal();
		}

		/// <summary>Reset the buffer</summary>
        public override void Reset()
        {
			if (buffer != null)
			{
				Array.Clear(buffer, 0, buffer.Length);
				bufOff = 0;
			}
        }
    }
}
