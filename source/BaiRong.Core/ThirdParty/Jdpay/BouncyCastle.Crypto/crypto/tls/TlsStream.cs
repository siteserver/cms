using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal class TlsStream
        : Stream
    {
        private readonly TlsProtocol handler;

        internal TlsStream(TlsProtocol handler)
        {
            this.handler = handler;
        }

        public override bool CanRead
        {
            get { return !handler.IsClosed; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return !handler.IsClosed; }
        }

#if PORTABLE
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                handler.Close();
            }
            base.Dispose(disposing);
        }
#else
        public override void Close()
        {
            handler.Close();
            base.Close();
        }
#endif

        public override void Flush()
        {
            handler.Flush();
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override int Read(byte[]	buf, int off, int len)
        {
            return this.handler.ReadApplicationData(buf, off, len);
        }

        public override int ReadByte()
        {
            byte[] buf = new byte[1];
            if (this.Read(buf, 0, 1) <= 0)
                return -1;
            return buf[0];
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buf, int off, int len)
        {
            this.handler.WriteData(buf, off, len);
        }

        public override void WriteByte(byte b)
        {
            this.handler.WriteData(new byte[] { b }, 0, 1);
        }
    }
}
