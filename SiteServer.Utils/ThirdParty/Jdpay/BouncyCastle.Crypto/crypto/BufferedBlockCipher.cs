using System;
using System.Diagnostics;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto
{
	/**
	* A wrapper class that allows block ciphers to be used to process data in
	* a piecemeal fashion. The BufferedBlockCipher outputs a block only when the
	* buffer is full and more data is being added, or on a doFinal.
	* <p>
	* Note: in the case where the underlying cipher is either a CFB cipher or an
	* OFB one the last block may not be a multiple of the block size.
	* </p>
	*/
	public class BufferedBlockCipher
		: BufferedCipherBase
	{
		internal byte[]			buf;
		internal int			bufOff;
		internal bool			forEncryption;
		internal IBlockCipher	cipher;

		/**
		* constructor for subclasses
		*/
		protected BufferedBlockCipher()
		{
		}

		/**
		* Create a buffered block cipher without padding.
		*
		* @param cipher the underlying block cipher this buffering object wraps.
		* false otherwise.
		*/
		public BufferedBlockCipher(
			IBlockCipher cipher)
		{
			if (cipher == null)
				throw new ArgumentNullException("cipher");

			this.cipher = cipher;
			buf = new byte[cipher.GetBlockSize()];
			bufOff = 0;
		}

		public override string AlgorithmName
		{
			get { return cipher.AlgorithmName; }
		}

		/**
		* initialise the cipher.
		*
		* @param forEncryption if true the cipher is initialised for
		*  encryption, if false for decryption.
		* @param param the key and other data required by the cipher.
		* @exception ArgumentException if the parameters argument is
		* inappropriate.
		*/
		// Note: This doubles as the Init in the event that this cipher is being used as an IWrapper
		public override void Init(
			bool				forEncryption,
			ICipherParameters	parameters)
		{
			this.forEncryption = forEncryption;

            ParametersWithRandom pwr = parameters as ParametersWithRandom;
            if (pwr != null)
                parameters = pwr.Parameters;

            Reset();

			cipher.Init(forEncryption, parameters);
		}

		/**
		* return the blocksize for the underlying cipher.
		*
		* @return the blocksize for the underlying cipher.
		*/
		public override int GetBlockSize()
		{
			return cipher.GetBlockSize();
		}

		/**
		* return the size of the output buffer required for an update
		* an input of len bytes.
		*
		* @param len the length of the input.
		* @return the space required to accommodate a call to update
		* with len bytes of input.
		*/
		public override int GetUpdateOutputSize(
			int length)
		{
			int total = length + bufOff;
			int leftOver = total % buf.Length;
			return total - leftOver;
		}

		/**
		* return the size of the output buffer required for an update plus a
		* doFinal with an input of len bytes.
		*
		* @param len the length of the input.
		* @return the space required to accommodate a call to update and doFinal
		* with len bytes of input.
		*/
		public override int GetOutputSize(
			int length)
		{
			// Note: Can assume IsPartialBlockOkay is true for purposes of this calculation
			return length + bufOff;
		}

		/**
		* process a single byte, producing an output block if necessary.
		*
		* @param in the input byte.
		* @param out the space for any output that might be produced.
		* @param outOff the offset from which the output will be copied.
		* @return the number of output bytes copied to out.
		* @exception DataLengthException if there isn't enough space in out.
		* @exception InvalidOperationException if the cipher isn't initialised.
		*/
		public override int ProcessByte(
			byte	input,
			byte[]	output,
			int		outOff)
		{
			buf[bufOff++] = input;

			if (bufOff == buf.Length)
			{
				if ((outOff + buf.Length) > output.Length)
					throw new DataLengthException("output buffer too short");

				bufOff = 0;
				return cipher.ProcessBlock(buf, 0, output, outOff);
			}

			return 0;
		}

		public override byte[] ProcessByte(
			byte input)
		{
			int outLength = GetUpdateOutputSize(1);

			byte[] outBytes = outLength > 0 ? new byte[outLength] : null;

			int pos = ProcessByte(input, outBytes, 0);

			if (outLength > 0 && pos < outLength)
			{
				byte[] tmp = new byte[pos];
				Array.Copy(outBytes, 0, tmp, 0, pos);
				outBytes = tmp;
			}

			return outBytes;
		}

		public override byte[] ProcessBytes(
			byte[]	input,
			int		inOff,
			int		length)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			if (length < 1)
				return null;

			int outLength = GetUpdateOutputSize(length);

			byte[] outBytes = outLength > 0 ? new byte[outLength] : null;

			int pos = ProcessBytes(input, inOff, length, outBytes, 0);

			if (outLength > 0 && pos < outLength)
			{
				byte[] tmp = new byte[pos];
				Array.Copy(outBytes, 0, tmp, 0, pos);
				outBytes = tmp;
			}

			return outBytes;
		}

		/**
		* process an array of bytes, producing output if necessary.
		*
		* @param in the input byte array.
		* @param inOff the offset at which the input data starts.
		* @param len the number of bytes to be copied out of the input array.
		* @param out the space for any output that might be produced.
		* @param outOff the offset from which the output will be copied.
		* @return the number of output bytes copied to out.
		* @exception DataLengthException if there isn't enough space in out.
		* @exception InvalidOperationException if the cipher isn't initialised.
		*/
		public override int ProcessBytes(
			byte[]	input,
			int		inOff,
			int		length,
			byte[]	output,
			int		outOff)
		{
			if (length < 1)
			{
				if (length < 0)
					throw new ArgumentException("Can't have a negative input length!");

				return 0;
			}

			int blockSize = GetBlockSize();
			int outLength = GetUpdateOutputSize(length);

			if (outLength > 0)
			{
                Check.OutputLength(output, outOff, outLength, "output buffer too short");
			}

            int resultLen = 0;
			int gapLen = buf.Length - bufOff;
			if (length > gapLen)
			{
				Array.Copy(input, inOff, buf, bufOff, gapLen);
				resultLen += cipher.ProcessBlock(buf, 0, output, outOff);
				bufOff = 0;
				length -= gapLen;
				inOff += gapLen;
				while (length > buf.Length)
				{
					resultLen += cipher.ProcessBlock(input, inOff, output, outOff + resultLen);
					length -= blockSize;
					inOff += blockSize;
				}
			}
			Array.Copy(input, inOff, buf, bufOff, length);
			bufOff += length;
			if (bufOff == buf.Length)
			{
				resultLen += cipher.ProcessBlock(buf, 0, output, outOff + resultLen);
				bufOff = 0;
			}
			return resultLen;
		}

		public override byte[] DoFinal()
		{
			byte[] outBytes = EmptyBuffer;

			int length = GetOutputSize(0);
			if (length > 0)
			{
				outBytes = new byte[length];

				int pos = DoFinal(outBytes, 0);
				if (pos < outBytes.Length)
				{
					byte[] tmp = new byte[pos];
					Array.Copy(outBytes, 0, tmp, 0, pos);
					outBytes = tmp;
				}
			}
			else
			{
				Reset();
			}

			return outBytes;
		}

		public override byte[] DoFinal(
			byte[]	input,
			int		inOff,
			int		inLen)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			int length = GetOutputSize(inLen);

			byte[] outBytes = EmptyBuffer;

			if (length > 0)
			{
				outBytes = new byte[length];

				int pos = (inLen > 0)
					?	ProcessBytes(input, inOff, inLen, outBytes, 0)
					:	0;

				pos += DoFinal(outBytes, pos);

				if (pos < outBytes.Length)
				{
					byte[] tmp = new byte[pos];
					Array.Copy(outBytes, 0, tmp, 0, pos);
					outBytes = tmp;
				}
			}
			else
			{
				Reset();
			}

			return outBytes;
		}

		/**
		* Process the last block in the buffer.
		*
		* @param out the array the block currently being held is copied into.
		* @param outOff the offset at which the copying starts.
		* @return the number of output bytes copied to out.
		* @exception DataLengthException if there is insufficient space in out for
		* the output, or the input is not block size aligned and should be.
		* @exception InvalidOperationException if the underlying cipher is not
		* initialised.
		* @exception InvalidCipherTextException if padding is expected and not found.
		* @exception DataLengthException if the input is not block size
		* aligned.
		*/
		public override int DoFinal(
			byte[]	output,
			int		outOff)
		{
			try
			{
				if (bufOff != 0)
				{
                    Check.DataLength(!cipher.IsPartialBlockOkay, "data not block size aligned");
                    Check.OutputLength(output, outOff, bufOff, "output buffer too short for DoFinal()");

                    // NB: Can't copy directly, or we may write too much output
					cipher.ProcessBlock(buf, 0, buf, 0);
					Array.Copy(buf, 0, output, outOff, bufOff);
				}

				return bufOff;
			}
			finally
			{
				Reset();
			}
		}

		/**
		* Reset the buffer and cipher. After resetting the object is in the same
		* state as it was after the last init (if there was one).
		*/
		public override void Reset()
		{
			Array.Clear(buf, 0, buf.Length);
			bufOff = 0;

			cipher.Reset();
		}
	}
}
