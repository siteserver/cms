// ***************************************************************
// <copyright file="SuppressCloseStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Internal
{
    using System;
    using System.IO;


    internal sealed class SuppressCloseStream : Stream, ICloneableStream
    {
        private Stream sourceStream;    

        
        public SuppressCloseStream(Stream sourceStream)
        {
            if (null == sourceStream)
            {
                throw new ArgumentNullException("sourceStream");
            }

            this.sourceStream = sourceStream;
        }

        public override bool CanRead => (null == sourceStream) ? false : sourceStream.CanRead;

        public override bool CanWrite => (null == sourceStream) ? false : sourceStream.CanWrite;


        public override bool CanSeek => (null == sourceStream) ? false : sourceStream.CanSeek;


        public override long Length
        {
            get 
            {
                AssertOpen();

                return sourceStream.Length; 
            }
        }

        
        public override long Position
        {
            get 
            {
                AssertOpen();

                return sourceStream.Position; 
            }

            set
            {
                AssertOpen();

                sourceStream.Position = value;
            }
        }

        
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            AssertOpen();

            return sourceStream.Read(buffer, offset, count);
        }

        
        
        public override void Write(byte[] buffer, int offset, int count)
        {
            AssertOpen();

            sourceStream.Write(buffer, offset, count);
        }

        
        
        public override void Flush()
        {          
            AssertOpen();

            sourceStream.Flush();
        }

        
        public override void SetLength(long value)
        {
            AssertOpen();

            sourceStream.SetLength(value);
        }

        
        public override long Seek(long offset, SeekOrigin origin)
        {
            AssertOpen();

            return sourceStream.Seek(offset, origin);
        }

        public override void Close()
        {
            if (null == sourceStream)
            {
                return;
            }

            sourceStream = null;
            base.Close();
        }

        
        public Stream Clone()
        {
            AssertOpen();

            if (CanWrite)
            {
                throw new NotSupportedException();
            }

            var cloneableStream = sourceStream as ICloneableStream;

            if (cloneableStream == null)
            {
                
                if (!sourceStream.CanSeek)
                {
                    throw new NotSupportedException();
                }

                sourceStream = new AutoPositionReadOnlyStream(sourceStream, false/*ownsStream*/);

                cloneableStream = sourceStream as ICloneableStream;
            }

            return new SuppressCloseStream(cloneableStream.Clone());
        }

        private void AssertOpen()
        {
            if (null == sourceStream)
            {
                throw new ObjectDisposedException("SuppressCloseStream");
            }
        }

        protected override void Dispose(bool disposing)
        {
            
            
            base.Dispose(disposing);
        }
    }
}

