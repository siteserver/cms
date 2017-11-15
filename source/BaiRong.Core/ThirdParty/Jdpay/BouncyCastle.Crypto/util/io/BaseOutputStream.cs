using System;
using System.Diagnostics;
using System.IO;

namespace Org.BouncyCastle.Utilities.IO
{
    public abstract class BaseOutputStream : Stream
    {
		private bool closed;

		public sealed override bool CanRead { get { return false; } }
        public sealed override bool CanSeek { get { return false; } }
        public sealed override bool CanWrite { get { return !closed; } }

#if PORTABLE
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                closed = true;
            }
            base.Dispose(disposing);
        }
#else
        public override void Close()
        {
            closed = true;
            base.Close();
        }
#endif

        public override void Flush() { }
        public sealed override long Length { get { throw new NotSupportedException(); } }
        public sealed override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }
        public sealed override int Read(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
        public sealed override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
        public sealed override void SetLength(long value) { throw new NotSupportedException(); }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Debug.Assert(buffer != null);
            Debug.Assert(0 <= offset && offset <= buffer.Length);
            Debug.Assert(count >= 0);

            int end = offset + count;

            Debug.Assert(0 <= end && end <= buffer.Length);

            for (int i = offset; i < end; ++i)
            {
                this.WriteByte(buffer[i]);
            }
        }

		public virtual void Write(params byte[] buffer)
		{
			Write(buffer, 0, buffer.Length);
		}

        public override void WriteByte(byte b)
        {
            Write(new byte[]{ b }, 0, 1);
        }
    }
}
