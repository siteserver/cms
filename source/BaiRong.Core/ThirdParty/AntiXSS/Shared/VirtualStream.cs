// ***************************************************************
// <copyright file="VirtualStream.cs" company="Microsoft">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Security.Permissions;
    using Strings = CtsResources.SharedStrings;

    

    internal abstract class RefCountable
    {
        private int refCount;

        protected RefCountable()
        {
            
            refCount = 1;
        }

        public int RefCount => refCount;

        public void AddRef()
        {
            var rc = System.Threading.Interlocked.Increment(ref refCount);
            InternalDebug.Assert(rc > 1);
        }

        public void Release()
        {
            var rc = System.Threading.Interlocked.Decrement(ref refCount);
            InternalDebug.Assert(rc >= 0);
            if (rc == 0)
            {
                Destroy();
            }
        }

        protected virtual void Destroy()
        {
        }
    }

    

    internal abstract class DataStorage : RefCountable
    {
        protected bool isReadOnly;
        protected object readOnlyLock;

        protected DataStorage() : base()
        {
        }

        
        
        
        

        public abstract Stream OpenReadStream(long start, long end);

        
        
        
        
        
        
        

        public virtual long CopyContentToStream(
                            long start,
                            long end, 
                            Stream destStream,
                            ref byte[] scratchBuffer)
        {
            InternalDebug.Assert(0 <= start && start <= end);

            if (destStream == null && end != long.MaxValue)
            {
                
                
                
                
                return end - start;
            }

            
            using (var srcStream = OpenReadStream(start, end))
            {
                return CopyStreamToStream(srcStream, destStream, long.MaxValue, ref scratchBuffer);
            }
        }

        
        
        
        
        
        
        
        

        public static long CopyStreamToStream(
                            Stream srcStream,
                            Stream destStream,
                            long lengthToCopy,
                            ref byte[] scratchBuffer)
        {
            if (scratchBuffer == null || scratchBuffer.Length < 4096)
            {
                scratchBuffer = new byte[4096];
            }

            long written = 0;

            while (lengthToCopy != 0)
            {
                var toRead = (int)Math.Min(lengthToCopy, (long)scratchBuffer.Length);

                var read = srcStream.Read(scratchBuffer, 0, toRead);
                if (0 == read)
                {
                    

                    
                    
                    InternalDebug.Assert(lengthToCopy == long.MaxValue);
                    break;
                }

                if (destStream != null)
                {
                    destStream.Write(scratchBuffer, 0, read);
                }

                written += read;

                if (lengthToCopy != long.MaxValue)
                {
                    lengthToCopy -= read;
                }
            }

            return written;
        }

        public static Stream NewEmptyReadStream()
        {
            return new StreamOnReadableDataStorage(null, 0, 0);
        }

        
        
        internal virtual void SetReadOnly(bool makeReadOnly)
        {
            if (makeReadOnly == isReadOnly)
            {
                return;
            }

            if (makeReadOnly)
            {
                readOnlyLock = new Object();
            }
            else
            {
                readOnlyLock = null;
            }

            isReadOnly = makeReadOnly;
        }
    }

    

    internal abstract class StreamOnDataStorage : Stream
    {
        

        public abstract DataStorage Storage { get; }
        public abstract long Start { get; }
        public abstract long End { get; }

        
        
        
        
        protected override void Dispose(bool disposing)
        {
            
            
            base.Dispose(disposing);
        }
    }

    

    internal abstract class ReadableDataStorage : DataStorage
    {
        public ReadableDataStorage() : base()
        {
        }

        public abstract long Length { get; }

        public abstract int Read(long position, byte[] buffer, int offset, int count);

        public override Stream OpenReadStream(long start, long end)
        {
            return new StreamOnReadableDataStorage(this, start, end);
        }

        public override long CopyContentToStream(
                            long start,
                            long end, 
                            Stream destStream,
                            ref byte[] scratchBuffer)
        {
            
            

            if (scratchBuffer == null || scratchBuffer.Length < 4096)
            {
                scratchBuffer = new byte[4096];
            }

            long written = 0;
            var remaining = end == long.MaxValue ? long.MaxValue : end - start;

            while (remaining != 0)
            {
                var toRead = (int)Math.Min(remaining, (long)scratchBuffer.Length);

                var read = Read(start, scratchBuffer, 0, toRead);
                if (0 == read)
                {
                    
                    

                    
                    
                    break;
                }

                start += read;

                destStream.Write(scratchBuffer, 0, read);

                written += read;

                if (remaining != long.MaxValue)
                {
                    remaining -= read;
                }
            }

            return written;
        }
    }

    

    internal abstract class ReadableWritableDataStorage : ReadableDataStorage
    {
        public ReadableWritableDataStorage() : base()
        {
        }

        public abstract void Write(long position, byte[] buffer, int offset, int count);
        public abstract void SetLength(long length);

        public virtual StreamOnDataStorage OpenWriteStream(bool append)
        {
            
            

            if (append)
            {
                return new AppendStreamOnDataStorage(this);
            }

            return new ReadWriteStreamOnDataStorage(this);
        }

#if DEBUG
        private bool writeStreamOpen;

        
        internal void SignalWriteStreamOpen()
        {
            InternalDebug.Assert(!writeStreamOpen);
            writeStreamOpen = true;
        }

        
        internal void SignalWriteStreamClose()
        {
            InternalDebug.Assert(writeStreamOpen);
            writeStreamOpen = false;
        }
#endif
    }

    

    internal class StreamOnReadableDataStorage : StreamOnDataStorage, ICloneableStream
    {
        private ReadableDataStorage baseStorage;
        private long start;
        private long end;
        private long position;
        private bool disposed;

        public StreamOnReadableDataStorage(ReadableDataStorage baseStorage, long start, long end)
        {
            InternalDebug.Assert(baseStorage != null || (start == 0 && end == 0));
            InternalDebug.Assert(start >= 0 && start <= end);

            if (baseStorage != null)
            {
                baseStorage.AddRef();
                this.baseStorage = baseStorage;
            }

            this.start = start;
            this.end = end;
        }

        

        private StreamOnReadableDataStorage(ReadableDataStorage baseStorage, long start, long end, long position)
        {
            InternalDebug.Assert(baseStorage != null || (start == 0 && end == 0));
            InternalDebug.Assert(start >= 0 && start <= end);

            if (baseStorage != null)
            {
                baseStorage.AddRef();
                this.baseStorage = baseStorage;
            }

            this.start = start;
            this.end = end;
            this.position = position;
        }

        

        public override DataStorage Storage
        {
            get
            {
                ThrowIfDisposed();
                return baseStorage;
            }
        }

        public override long Start
        {
            get
            {
                ThrowIfDisposed();
                return start;
            }
        }

        public override long End
        {
            get
            {
                ThrowIfDisposed();
                return end;
            }
        }

        
        

        public override bool CanRead => !disposed;

        public override bool CanWrite => false;

        public override bool CanSeek => !disposed;

        public override long Length
        {
            get
            {
                ThrowIfDisposed();
                
                return end == long.MaxValue ? baseStorage.Length - start : end - start;
            }
        }

        public override long Position
        {
            get
            {
                ThrowIfDisposed();
                
                return position;
            }

            set
            {
                ThrowIfDisposed();
                
                InternalDebug.Assert(0 <= Position);

                if (value < 0)
                {
                    
                    throw new ArgumentOutOfRangeException("value", Strings.CannotSeekBeforeBeginning);
                }

                
                position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();

            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count < 0)
            {
                
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            var readTotal = 0;
            int read;

            if ((end == long.MaxValue || position < end - start) && count != 0)
            {
                if (end != long.MaxValue && count > end - start - position)
                {
                    count = (int)(end - start - position);
                }

                do
                {
                    read = baseStorage.Read(start + position, buffer, offset, count);

                    count -= read;
                    offset += read;

                    position += read;

                    readTotal += read;
                }
                while (count != 0 && read != 0);
            }

            return readTotal;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIfDisposed();

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
                    
                    throw new ArgumentException("Invalid Origin enumeration value", "origin");
                }
            }
            
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.CannotSeekBeforeBeginning);
            }
            
            position = offset;

            return position;
        }

        public override void Close()
        {
            if (baseStorage != null)
            {
                baseStorage.Release();
                baseStorage = null;
            }

            disposed = true;
            base.Close();
        }

        public Stream Clone()
        {
            ThrowIfDisposed();

            return new StreamOnReadableDataStorage(baseStorage, start, end, position);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("StreamOnReadableDataStorage");
            }
        }

        
        
        
        
        protected override void Dispose(bool disposing)
        {
            
            
            base.Dispose(disposing);
        }
    }

    

    internal class ReadableDataStorageOnStream : ReadableDataStorage
    {
        private Stream stream;
        private bool ownsStream;

        public ReadableDataStorageOnStream(Stream stream, bool ownsStream) : base()
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            InternalDebug.Assert(RefCount == 1);
            InternalDebug.Assert(stream.CanSeek && stream.CanRead);

            this.stream = stream;
            this.ownsStream = ownsStream;
        }

        public override long Length => stream.Length;

        public override int Read(long position, byte[] buffer, int offset, int count)
        {
            InternalDebug.Assert(RefCount > 0);

            if (isReadOnly)
            {
                lock (readOnlyLock)
                {
                    return InternalRead(position, buffer, offset, count);
                }
            }
            else
            {
                return InternalRead(position, buffer, offset, count);
            }
        }
        private int InternalRead(long position, byte[] buffer, int offset, int count)
        {
            stream.Position = position;
            return stream.Read(buffer, offset, count);
        }
#if false
        public override Stream OpenReadStream(long start, long end)
        {
            
            return new StreamOnReadableDataStorage(this, start, end);
        }
#endif
        protected override void Destroy()
        {
            InternalDebug.Assert(RefCount == 0);

            if (ownsStream)
            {
                stream.Close();
            }

            stream = null;

            base.Destroy();
        }
    }

    

    internal class ReadableWritableDataStorageOnStream : ReadableWritableDataStorage
    {
        protected Stream stream;
        protected bool ownsStream;

        public ReadableWritableDataStorageOnStream(Stream stream, bool ownsStream) : base()
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            InternalDebug.Assert(RefCount == 1);
            InternalDebug.Assert(stream.CanSeek && stream.CanRead && stream.CanWrite);

            this.stream = stream;
            this.ownsStream = ownsStream;
        }

        public override long Length => stream.Length;

        public override int Read(long position, byte[] buffer, int offset, int count)
        {
            InternalDebug.Assert(RefCount > 0);

            if (isReadOnly)
            {
                lock (readOnlyLock)
                {
                    return InternalRead(position, buffer, offset, count);
                }
            }
            else
            {
                return InternalRead(position, buffer, offset, count);
            }
        }
        private int InternalRead(long position, byte[] buffer, int offset, int count)
        {
            stream.Position = position;
            return stream.Read(buffer, offset, count);
        }

        public override void Write(long position, byte[] buffer, int offset, int count)
        {
            InternalDebug.Assert(RefCount > 0);

            if (isReadOnly)
            {
                throw new InvalidOperationException("Write to read-only DataStorage");
            }

            stream.Position = position;
            stream.Write(buffer, offset, count);
        }

        public override void SetLength(long length)
        {
            InternalDebug.Assert(RefCount > 0);

            if (isReadOnly)
            {
                throw new InvalidOperationException("Write to read-only DataStorage");
            }
            stream.SetLength(length);
        }
#if false
        public override Stream OpenReadStream(long start, long end)
        {
            
            return new StreamOnReadableDataStorage(this, start, end);
        }
#endif
        protected override void Destroy()
        {
            InternalDebug.Assert(RefCount == 0);

            if (ownsStream)
            {
                stream.Close();
            }

            stream = null;

            base.Destroy();
        }
    }






    internal class Func
    {

    }
    
    
    internal class TemporaryDataStorage : ReadableWritableDataStorage
    {
        private long totalLength = 0;
        private long filePosition = 0;
        private Stream fileStream;

        public static int defaultBufferBlockSize = 8 * 1024;                            
        public static int defaultBufferMaximumSize = defaultBufferBlockSize * 16;       
        public static string defaultPath = null;
        public static Action<byte[]> defaultReleaseBuffer = null;

        internal static volatile bool configured = false;  
        private static object configurationLockObject = new object();

        


        
        
        
        public override long Length => totalLength;


        protected override void Destroy()
        {
            InternalDebug.Assert(RefCount == 0);

            if (fileStream != null)
            {
                fileStream.Close();
                fileStream = null;
            }

            base.Destroy();
        }

        
        
        
        
        
        
        
        
        
        
        
        public override int Read(long position, byte[] buffer, int offset, int count)
        {
            var readTotal = 0;

           

            return readTotal;
        }

        private int InternalRead(long position, byte[] buffer, int offset, int count)
        {

            var readFromFile = fileStream.Read(buffer, offset, count);

            return readFromFile;
        }

        
        
        
        
        
        
        
        public override void Write(long position, byte[] buffer, int offset, int count)
        {
            if (isReadOnly)
            {
                throw new InvalidOperationException("Write to read-only DataStorage");
            }

            if (count != 0)
            {

                if (fileStream == null)
                {
                    fileStream = TempFileStream.CreateInstance();
                    filePosition = 0;
                }

                fileStream.Write(buffer, offset, count);

                position += count;

                if (position > totalLength)
                {
                    totalLength = position;
                }
            }
        }

        
        
        
        
        public override void SetLength(long length)
        {
            if (isReadOnly)
            {
                throw new InvalidOperationException("Write to read-only DataStorage");
            }

            totalLength = length;
        }

        internal static void RefreshConfiguration()
        {
            configured = false;
        }

        internal static string GetTempPath()
        {
            if (!configured)
            {
                Configure();
            }

            return TempFileStream.Path;
        }

        public static void Configure(
            int defaultMaximumSize,
            int defaultBlockSize,
            string defaultPath,
            Action<byte[]> defaultReleaseBuffer)
        {
            defaultBufferMaximumSize = defaultMaximumSize;
            defaultBufferBlockSize = defaultBlockSize;
            TemporaryDataStorage.defaultPath = defaultPath;
            TemporaryDataStorage.defaultReleaseBuffer = defaultReleaseBuffer;

            configured = false;
            Configure();
        }

        private static void Configure()
        {
            lock (configurationLockObject)
            {
                if (!configured)
                {
                    var maximumSize = defaultBufferMaximumSize;
                    var blockSize = defaultBufferBlockSize;
                    var path = TemporaryDataStorage.defaultPath;

                    if (maximumSize < blockSize || maximumSize % blockSize != 0)
                    {                        
                        maximumSize = defaultBufferMaximumSize;
                        blockSize = defaultBufferBlockSize;
                    }

                    defaultBufferMaximumSize = maximumSize;
                    defaultBufferBlockSize = blockSize;

                    var defaultPath = Path.GetTempPath();

                    if (path != null)
                    {
                        
                        path = ValidatePath(path);
                    }

                    if (path == null)
                    {
                        
                        

                        path = defaultPath;
                    }

                    TempFileStream.SetTemporaryPath(path);

                    configured = true;
                }
            }
        }

        private static readonly FileSystemAccessRule[] DirectoryAccessRules = new FileSystemAccessRule[]
        {
            new FileSystemAccessRule(
                new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
                FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.None,
                AccessControlType.Allow),
            new FileSystemAccessRule(
                new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
                FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.None,
                AccessControlType.Allow),
            new FileSystemAccessRule(
                new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null),
                FileSystemRights.FullControl & ~(FileSystemRights.ChangePermissions | FileSystemRights.TakeOwnership),
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.None,
                AccessControlType.Allow)
        };

        private static DirectorySecurity GetDirectorySecurity()
        {
            var security = new DirectorySecurity();

            
            
            security.SetAccessRuleProtection(true, false);

            
            security.SetOwner(WindowsIdentity.GetCurrent().User);

            
            for (var i = 0; i < DirectoryAccessRules.Length; i++)
            {
                security.AddAccessRule(DirectoryAccessRules[i]);
            }

            if (!WindowsIdentity.GetCurrent().User.IsWellKnown(WellKnownSidType.LocalSystemSid) &&
                !WindowsIdentity.GetCurrent().User.IsWellKnown(WellKnownSidType.NetworkServiceSid))
            {
                

                security.AddAccessRule(new FileSystemAccessRule(
                    WindowsIdentity.GetCurrent().User,
                    FileSystemRights.FullControl,
                    InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow));
            }

            return security;
        }


        
        
        [SuppressMessage("Microsoft.Security", "CA2103:ReviewImperativeSecurity")]
        
        private static string ValidatePath(string path)
        {
            

            try
            {
                if (Path.IsPathRooted(path))
                {
                    if (Directory.Exists(path))
                    {
                        
                    }
                    else
                    {
                        Directory.CreateDirectory(path, GetDirectorySecurity());
                    }

                    
                    
                    
                    
                    
                    
                    new FileIOPermission(FileIOPermissionAccess.Write, path).Demand();
                }
                else
                {
                    
                    path = null;
                }
            }
            catch (PathTooLongException /*exception*/)
            {
                
                path = null;
            }
            catch (DirectoryNotFoundException /*exception*/)
            {
                
                path = null;
            }
            catch (IOException /*exception*/)
            {
                
                path = null;
            }
            catch (UnauthorizedAccessException /*exception*/)
            {
                
                path = null;
            }
            catch (ArgumentException /*exception*/)
            {
                
                path = null;
            }
            catch (NotSupportedException /*exception*/)
            {
                
                path = null;
            }

            return path;
        }

        
        
        
      
    }

    

    internal class ReadWriteStreamOnDataStorage : StreamOnDataStorage, ICloneableStream
    {
        private ReadableWritableDataStorage storage;
        private long position;

        internal ReadWriteStreamOnDataStorage(ReadableWritableDataStorage storage)
        {
#if DEBUG
            
            
#endif
            storage.AddRef();

            this.storage = storage;
        }

        private ReadWriteStreamOnDataStorage(ReadableWritableDataStorage storage, long position)
        {
#if DEBUG
            
            
#endif
            storage.AddRef();

            this.storage = storage;
            this.position = position;
        }

        

        public override DataStorage Storage
        {
            get
            {
                if (storage == null)
                {
                    throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
                }

                return storage;
            }
        }

        public override long Start => 0;

        public override long End => long.MaxValue;


        public override bool CanRead
        {
            get
            { 
                if (storage == null)
                {
                    return false;
                }

                return true;
            }
        }

        public override bool CanWrite
        {
            get
            { 
                if (storage == null)
                {
                    return false;
                }

                return true;
            }
        }

        public override bool CanSeek
        {
            get
            { 
                if (storage == null)
                {
                    return false;
                }

                return true;
            }
        }

        public override long Length
        {
            get
            { 
                if (storage == null)
                {
                    throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
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
                    throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
                }

                return position; 
            }
            set 
            { 
                if (storage == null)
                {
                    throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
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
            int bytesRead;

            if (storage == null)
            {
                throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
            }

            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count < 0)
            {
                
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            bytesRead = storage.Read(position, buffer, offset, count);

            position += bytesRead;

            return bytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (storage == null)
            {
                throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
            }

            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            storage.Write(position, buffer, offset, count);

            position += count;
        }

        public override void SetLength(long value)
        {
            if (storage == null)
            {
                throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
            }

            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value", Strings.CannotSetNegativelength);
            }

            storage.SetLength(value);
        }

        public override void Flush()
        {
            if (storage == null)
            {
                throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (storage == null)
            {
                throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
            }

            switch (origin)
            {
                case SeekOrigin.Begin:
                {
                    position = offset;
                    break;
                }
                case SeekOrigin.Current:
                {
                    offset = position + offset;
                    break;
                }
                case SeekOrigin.End:
                {
                    offset = storage.Length + offset;
                    break;
                }
                default:
                {
                    
                    throw new ArgumentException("Invalid Origin enumeration value", "origin");
                }
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.CannotSeekBeforeBeginning);
            }

            position = offset;

            return position;
        }

        public override void Close()
        {
            if (null != storage)
            {
#if DEBUG
                
                
#endif
                storage.Release();
                storage = null;
            }
        }

        
        
        
        
        protected override void Dispose(bool disposing)
        {
            
            
            base.Dispose(disposing);
        }

        Stream ICloneableStream.Clone()
        {
            if (storage == null)
            {
                throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
            }

            return new ReadWriteStreamOnDataStorage(storage, position);
        }
    }

    

    internal class AppendStreamOnDataStorage : StreamOnDataStorage
    {
        private ReadableWritableDataStorage storage;
        private long position;

        public AppendStreamOnDataStorage(ReadableWritableDataStorage storage)
        {
#if DEBUG
            storage.SignalWriteStreamOpen();
#endif
            storage.AddRef();

            this.storage = storage;
            position = storage.Length;     
        }

        

        public override DataStorage Storage => storage;

        public override long Start => 0;


        public override long End
        {
            get
            {
                
                InternalDebug.Assert(storage.Length == 0 || storage.Length == position);
                return position;
            }
        }

        public ReadableWritableDataStorage ReadableWritableStorage => storage;


        public override bool CanRead => false;

        public override bool CanWrite => storage != null;

        public override bool CanSeek => false;

        public override long Length
        {
            get
            { 
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            { 
                throw new NotSupportedException();
            }
            set 
            { 
                throw new NotSupportedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (storage == null)
            {
                throw new ObjectDisposedException("AppendStreamOnDataStorage");
            }

            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            
            InternalDebug.Assert(storage.Length == 0 || storage.Length == position);

            storage.Write(position, buffer, offset, count);

            position += count;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            if (storage == null)
            {
                throw new ObjectDisposedException("AppendStreamOnDataStorage");
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void Close()
        {
            if (null != storage)
            {
#if DEBUG
                
                InternalDebug.Assert(storage.Length == 0 || storage.Length == position);
                storage.SignalWriteStreamClose();
#endif
                storage.Release();
                storage = null;
            }

            base.Close();
        }

        
        
        
        
        protected override void Dispose(bool disposing)
        {
            
            
            base.Dispose(disposing);
        }
    }
}

