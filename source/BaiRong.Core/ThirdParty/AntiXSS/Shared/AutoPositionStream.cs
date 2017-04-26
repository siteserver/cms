// ***************************************************************
// <copyright file="AutoPositionStream.cs" company="Microsoft">
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
    using Strings = CtsResources.SharedStrings;

    
    
    
    
    internal sealed class AutoPositionReadOnlyStream : Stream, ICloneableStream
    {
        private ReadableDataStorage storage;
        private long position;

        

        
        
        
        
        
        public AutoPositionReadOnlyStream(Stream wrapped, bool ownsStream)
        {
            storage = new ReadableDataStorageOnStream(wrapped, ownsStream);
            position = wrapped.Position;
        }

        
        
        
        
        private AutoPositionReadOnlyStream(AutoPositionReadOnlyStream original)
        {
            original.storage.AddRef();
            storage = original.storage;
            position = original.position;
        }

        

        
        
        
        public override bool CanRead => storage != null;


        public override bool CanWrite => false;


        public override bool CanSeek => storage != null;


        public override long Length
        {
            get
            { 
                if (storage == null)
                {
                    throw new ObjectDisposedException("AutoPositionReadOnlyStream");
                }

                return storage.Length; 
            }
        }

        
        
        
        
        
        
        
        
        
        public override long Position
        {
            get
            { 
                if (storage == null)
                {
                    throw new ObjectDisposedException("AutoPositionReadOnlyStream");
                }

                return position; 
            }
            set 
            { 
                if (storage == null)
                {
                    throw new ObjectDisposedException("AutoPositionReadOnlyStream");
                }
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", Strings.CannotSeekBeforeBeginning);
                }

                position = value; 
            }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (storage == null)
            {
                throw new ObjectDisposedException("AutoPositionReadOnlyStream");
            }

            var read = storage.Read(position, buffer, offset, count);
            position += read;
            return read;
        }

        
        
        
        
        
        
        
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        
        
        
        
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        
        
        
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (storage == null)
            {
                throw new ObjectDisposedException("AutoPositionReadOnlyStream");
            }

            switch (origin)
            {
                case SeekOrigin.Begin:
                {
                    break;
                }
                case SeekOrigin.Current:
                {
                    offset += position;
                    break;
                }
                case SeekOrigin.End:
                {
                    offset += Length;
                    break;
                }
                default:
                {
                    
                    throw new ArgumentException("origin");
                }
            }

            if (0 > offset)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.CannotSeekBeforeBeginning);
            }

            position = offset;

            return position;
        }

        
        
        
        public override void Close()
        {
            if (storage != null)
            {
                storage.Release();
                storage = null;
            }

            base.Close();
        }

        
        
        
        
        public Stream Clone()
        {
            if (storage == null)
            {
                throw new ObjectDisposedException("AutoPositionReadOnlyStream");
            }

            return new AutoPositionReadOnlyStream(this);
        }

        
        
        
        
        protected override void Dispose(bool disposing)
        {
            
            
            base.Dispose(disposing);
        }
    }
}
