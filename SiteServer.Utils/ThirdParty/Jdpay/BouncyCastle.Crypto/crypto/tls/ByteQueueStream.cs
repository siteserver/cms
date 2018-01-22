using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class ByteQueueStream
        : Stream
    {
        private readonly ByteQueue buffer;

        public ByteQueueStream()
        {
            this.buffer = new ByteQueue();
        }

        public virtual int Available
        {
            get { return buffer.Available; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public virtual int Peek(byte[] buf)
        {
            int bytesToRead = System.Math.Min(buffer.Available, buf.Length);
            buffer.Read(buf, 0, bytesToRead, 0);
            return bytesToRead;
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public virtual int Read(byte[] buf)
        {
            return Read(buf, 0, buf.Length);
        }

        public override int Read(byte[] buf, int off, int len)
        {
            int bytesToRead = System.Math.Min(buffer.Available, len);
            buffer.RemoveData(buf, off, bytesToRead, 0);
            return bytesToRead;
        }

        public override int ReadByte()
        {
            if (buffer.Available == 0)
                return -1;

            return buffer.RemoveData(1, 0)[0] & 0xFF;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public virtual int Skip(int n)
        {
            int bytesToSkip = System.Math.Min(buffer.Available, n);
            buffer.RemoveData(bytesToSkip);
            return bytesToSkip;
        }

        public virtual void Write(byte[] buf)
        {
            buffer.AddData(buf, 0, buf.Length);
        }

        public override void Write(byte[] buf, int off, int len)
        {
            buffer.AddData(buf, off, len);
        }

        public override void WriteByte(byte b)
        {
            buffer.AddData(new byte[]{ b }, 0, 1);
        }
    }
}
