using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <remarks>
    /// A queue for bytes.
    /// <p>
    /// This file could be more optimized.
    /// </p>
    /// </remarks>
    public class ByteQueue
    {
        /// <returns>The smallest number which can be written as 2^x which is bigger than i.</returns>
        public static int NextTwoPow(
            int i)
        {
            /*
            * This code is based of a lot of code I found on the Internet
            * which mostly referenced a book called "Hacking delight".
            *
            */
            i |= (i >> 1);
            i |= (i >> 2);
            i |= (i >> 4);
            i |= (i >> 8);
            i |= (i >> 16);
            return i + 1;
        }

        /**
         * The initial size for our buffer.
         */
        private const int DefaultCapacity = 1024;

        /**
         * The buffer where we store our data.
         */
        private byte[] databuf;

        /**
         * How many bytes at the beginning of the buffer are skipped.
         */
        private int skipped = 0;

        /**
         * How many bytes in the buffer are valid data.
         */
        private int available = 0;

        private bool readOnlyBuf = false;

        public ByteQueue()
            : this(DefaultCapacity)
        {
        }

        public ByteQueue(int capacity)
        {
            this.databuf = capacity == 0 ? TlsUtilities.EmptyBytes : new byte[capacity];
        }

        public ByteQueue(byte[] buf, int off, int len)
        {
            this.databuf = buf;
            this.skipped = off;
            this.available = len;
            this.readOnlyBuf = true;
        }

        /// <summary>Add some data to our buffer.</summary>
        /// <param name="data">A byte-array to read data from.</param>
        /// <param name="offset">How many bytes to skip at the beginning of the array.</param>
        /// <param name="len">How many bytes to read from the array.</param>
        public void AddData(
            byte[] data,
            int offset,
            int len)
        {
            if (readOnlyBuf)
                throw new InvalidOperationException("Cannot add data to read-only buffer");

            if ((skipped + available + len) > databuf.Length)
            {
                int desiredSize = ByteQueue.NextTwoPow(available + len);
                if (desiredSize > databuf.Length)
                {
                    byte[] tmp = new byte[desiredSize];
                    Array.Copy(databuf, skipped, tmp, 0, available);
                    databuf = tmp;
                }
                else
                {
                    Array.Copy(databuf, skipped, databuf, 0, available);
                }
                skipped = 0;
            }

            Array.Copy(data, offset, databuf, skipped + available, len);
            available += len;
        }

        /// <summary>The number of bytes which are available in this buffer.</summary>
        public int Available
        {
            get { return available; }
        }

        /// <summary>Copy some bytes from the beginning of the data to the provided <c cref="Stream">Stream</c>.</summary>
        /// <param name="output">The <c cref="Stream">Stream</c> to copy the bytes to.</param>
        /// <param name="length">How many bytes to copy.</param>
		/// <exception cref="InvalidOperationException">If insufficient data is available.</exception>
		/// <exception cref="IOException">If there is a problem copying the data.</exception>
        public void CopyTo(Stream output, int length)
        {
            if (length > available)
                throw new InvalidOperationException("Cannot copy " + length + " bytes, only got " + available);

            output.Write(databuf, skipped, length);
        }

        /// <summary>Read data from the buffer.</summary>
        /// <param name="buf">The buffer where the read data will be copied to.</param>
        /// <param name="offset">How many bytes to skip at the beginning of buf.</param>
        /// <param name="len">How many bytes to read at all.</param>
        /// <param name="skip">How many bytes from our data to skip.</param>
        public void Read(
            byte[]	buf,
            int		offset,
            int		len,
            int		skip)
        {
            if ((buf.Length - offset) < len)
            {
                throw new ArgumentException("Buffer size of " + buf.Length + " is too small for a read of " + len + " bytes");
            }
            if ((available - skip) < len)
            {
                throw new InvalidOperationException("Not enough data to read");
            }
            Array.Copy(databuf, skipped + skip, buf, offset, len);
        }

        /// <summary>Return a <c cref="MemoryStream">MemoryStream</c> over some bytes at the beginning of the data.</summary>
        /// <param name="length">How many bytes will be readable.</param>
        /// <returns>A <c cref="MemoryStream">MemoryStream</c> over the data.</returns>
		/// <exception cref="InvalidOperationException">If insufficient data is available.</exception>
        public MemoryStream ReadFrom(int length)
        {
            if (length > available)
                throw new InvalidOperationException("Cannot read " + length + " bytes, only got " + available);

            int position = skipped;

            available -= length;
            skipped += length;

            return new MemoryStream(databuf, position, length, false);
        }

        /// <summary>Remove some bytes from our data from the beginning.</summary>
        /// <param name="i">How many bytes to remove.</param>
        public void RemoveData(
            int i)
        {
            if (i > available)
            {
                throw new InvalidOperationException("Cannot remove " + i + " bytes, only got " + available);
            }

            /*
            * Skip the data.
            */
            available -= i;
            skipped += i;
        }

        public void RemoveData(byte[] buf, int off, int len, int skip)
        {
            Read(buf, off, len, skip);
            RemoveData(skip + len);
        }

        public byte[] RemoveData(int len, int skip)
        {
            byte[] buf = new byte[len];
            RemoveData(buf, 0, len, skip);
            return buf;
        }

        public void Shrink()
        {
            if (available == 0)
            {
                databuf = TlsUtilities.EmptyBytes;
                skipped = 0;
            }
            else
            {
                int desiredSize = ByteQueue.NextTwoPow(available);
                if (desiredSize < databuf.Length)
                {
                    byte[] tmp = new byte[desiredSize];
                    Array.Copy(databuf, skipped, tmp, 0, available);
                    databuf = tmp;
                    skipped = 0;
                }
            }
        }
    }
}
