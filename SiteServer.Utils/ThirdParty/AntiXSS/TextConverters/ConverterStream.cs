// ***************************************************************
// <copyright file="ConverterStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.IO;
    using Data.Internal;
    using Strings = CtsResources.TextConvertersStrings;

    

    
    
    
    internal enum ConverterStreamAccess
    {
        
        Read,
        
        Write,
    }

    
    
    
    
    internal class ConverterStream : Stream, IProgressMonitor
    {
        

        private IProducerConsumer consumer;

        private int maxLoopsWithoutProgress;
        private bool madeProgress;

        private byte[] chunkToReadBuffer;
        private int chunkToReadOffset;
        private int chunkToReadCount;

        

        private IByteSource byteSource;
        private IProducerConsumer producer;

        private byte[] writeBuffer;
        private int writeOffset;
        private int writeCount;

        

        private object sourceOrDestination;

        private bool endOfFile;
        private bool inconsistentState;

        
        
        
        
        
        
        
        
        
        
        
        
        public ConverterStream(
            Stream stream,
            TextConverter converter,
            ConverterStreamAccess access)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            if (access < ConverterStreamAccess.Read || ConverterStreamAccess.Write < access)
            {
                throw new ArgumentException(Strings.AccessShouldBeReadOrWrite, "access");
            }

            if (access == ConverterStreamAccess.Read)
            {
                if (!stream.CanRead)
                {
                    throw new ArgumentException(Strings.CannotReadFromSource, "stream");
                }

                producer = converter.CreatePullChain(stream, this);
            }
            else
            {
                if (!stream.CanWrite)
                {
                    throw new ArgumentException(Strings.CannotWriteToDestination, "stream");
                }

                consumer = converter.CreatePushChain(this, stream);
            }

            sourceOrDestination = stream;

            maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        
        
        
        
        
        
        
        public ConverterStream(
            TextReader sourceReader,
            TextConverter converter)
        {
            if (sourceReader == null)
            {
                throw new ArgumentNullException("sourceReader");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            producer = converter.CreatePullChain(sourceReader, this);
            sourceOrDestination = sourceReader;

            maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        
        
        
        
        
        
        public ConverterStream(
            TextWriter destinationWriter,
            TextConverter converter)
        {
            if (destinationWriter == null)
            {
                throw new ArgumentNullException("destinationWriter");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            consumer = converter.CreatePushChain(this, destinationWriter);
            sourceOrDestination = destinationWriter;

            maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        
        

        
        
        
        
        public override bool CanRead => producer != null ? true : false;


        public override bool CanWrite => consumer != null ? true : false;


        public override bool CanSeek => false;


        public override long Length
        {
            get
            {
                throw new NotSupportedException(Strings.SeekUnsupported);
            }
        }

        
        
        
        
        public override long Position
        {
            get
            {
                throw new NotSupportedException(Strings.SeekUnsupported);
            }

            set
            {
                throw new NotSupportedException(Strings.SeekUnsupported);
            }
        }

        
        
        
        
        
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(Strings.SeekUnsupported);
        }

        
        
        
        
        public override void SetLength(long value)
        {
            throw new NotSupportedException(Strings.SeekUnsupported);
        }

        
        
        
        
        
        
        
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (sourceOrDestination == null)
            {
                throw new ObjectDisposedException("ConverterStream");
            }

            if (consumer == null)
            {
                throw new InvalidOperationException(Strings.WriteUnsupported);
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

            if (endOfFile)
            {
                throw new InvalidOperationException(Strings.WriteAfterFlush);
            }

            if (inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterStreamInInconsistentStare);
            }

            InternalDebug.Assert(consumer != null);

            chunkToReadBuffer = buffer;
            chunkToReadOffset = offset;
            chunkToReadCount = count;

            
            
            

            long loopsWithoutProgress = 0;

            
            
            inconsistentState = true;

            while (0 != chunkToReadCount)
            {
                
                consumer.Run();

                if (madeProgress)
                {
                    
                    loopsWithoutProgress = 0;
                    madeProgress = false;
                }
                else if (maxLoopsWithoutProgress == loopsWithoutProgress++)
                {
                    InternalDebug.Assert(false);
                    throw new TextConvertersException(Strings.TooManyIterationsToProcessInput);
                }
            }

            
            inconsistentState = false;

            chunkToReadBuffer = null;       
        }

        
        
        
        
        
        
        public override void Flush()
        {
            if (sourceOrDestination == null)
            {
                throw new ObjectDisposedException("ConverterStream");
            }

            if (consumer == null)
            {
                throw new InvalidOperationException(Strings.WriteUnsupported);
            }

            InternalDebug.Assert(consumer != null);

            endOfFile = true;

            if (!inconsistentState)
            {
                long loopsWithoutProgress = 0;

                
                
                inconsistentState = true;

                while (!consumer.Flush())
                {
                    if (madeProgress)
                    {
                        
                        loopsWithoutProgress = 0;
                        madeProgress = false;
                    }
                    else if (maxLoopsWithoutProgress == loopsWithoutProgress++)
                    {
                        InternalDebug.Assert(false);
                        throw new TextConvertersException(Strings.TooManyIterationsToFlushConverter);
                    }
                }

                
                inconsistentState = false;
            }

            if (sourceOrDestination is Stream)
            {
                ((Stream)sourceOrDestination).Flush();
            }
            else if (sourceOrDestination is TextWriter)
            {
                ((TextWriter)sourceOrDestination).Flush();
            }
        }

        
        
        
        
        
        

        public override void Close()
        {
            try
            {
                if (sourceOrDestination != null)
                {
                    if (consumer != null && !inconsistentState)
                    {
                        Flush();
                    }
                }

                
                
                
                if (producer != null && producer is IDisposable)
                {
                    ((IDisposable)producer).Dispose();
                }

                if (consumer != null && consumer is IDisposable)
                {
                    ((IDisposable)consumer).Dispose();
                }
            }
            finally
            {
                if (sourceOrDestination != null)
                {
                    if (sourceOrDestination is Stream)
                    {
                        ((Stream)sourceOrDestination).Close();
                    }
                    else if (sourceOrDestination is TextReader)
                    {
                        ((TextReader)sourceOrDestination).Close();
                    }
                    else
                    {
                        ((TextWriter)sourceOrDestination).Close();
                    }
                }
                sourceOrDestination = null;
                consumer = null;
                producer = null;
                chunkToReadBuffer = null;
                writeBuffer = null;
                byteSource = null;
            }
        }

        
        
        
        
        
        
        
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (sourceOrDestination == null)
            {
                throw new ObjectDisposedException("ConverterStream");
            }

            if (producer == null)
            {
                throw new InvalidOperationException(Strings.ReadUnsupported);
            }

            InternalDebug.Assert(producer != null);

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

            if (inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterStreamInInconsistentStare);
            }

            InternalDebug.Assert(producer != null);

            var initialCount = count;

            if (byteSource != null)
            {
                
                

                byte[] chunkBuffer;
                int chunkOffset;
                int chunkCount;

                while (count != 0 && byteSource.GetOutputChunk(out chunkBuffer, out chunkOffset, out chunkCount))
                {
                    var bytesRead = Math.Min(chunkCount, count);

                    Buffer.BlockCopy(chunkBuffer, chunkOffset, buffer, offset, bytesRead);

                    offset += bytesRead;
                    count -= bytesRead;

                    byteSource.ReportOutput(bytesRead);
                }
            }

            
            

            if (0 != count)
            {
                

                
                
                

                long loopsWithoutProgress = 0;

                writeBuffer = buffer;
                writeOffset = offset;
                writeCount = count;

                
                
                inconsistentState = true;

                while (0 != writeCount && !endOfFile)
                {
                    

                    producer.Run();

                    if (madeProgress)
                    {
                        
                        loopsWithoutProgress = 0;
                        madeProgress = false;
                    }
                    else if (maxLoopsWithoutProgress == loopsWithoutProgress++)
                    {
                        InternalDebug.Assert(false);
                        throw new TextConvertersException(Strings.TooManyIterationsToProduceOutput);
                    }
                }

                count = writeCount;     

                writeBuffer = null;
                writeOffset = 0;
                writeCount = 0;

                
                inconsistentState = false;
            }

            return initialCount - count;
        }

        
        
        
        
        
        internal void SetSource(IByteSource byteSource)
        {
            
            InternalDebug.Assert(producer == null && consumer == null);

            this.byteSource = byteSource;
        }

        
        
        
        
        
        
        
        internal void GetOutputBuffer(out byte[] outputBuffer, out int outputOffset, out int outputCount)
        {
            InternalDebug.Assert(producer != null);
            InternalDebug.Assert(!endOfFile);

            outputBuffer = writeBuffer;
            outputOffset = writeOffset;
            outputCount = writeCount;
        }

        
        
        
        
        
        internal void ReportOutput(int outputCount)
        {
            InternalDebug.Assert(producer != null);
            InternalDebug.Assert(!endOfFile && outputCount <= writeCount);

            if (outputCount != 0)
            {
                madeProgress = true;
                writeCount -= outputCount;
                writeOffset += outputCount;
            }
        }

        
        
        
        
        internal void ReportEndOfFile()
        {
            InternalDebug.Assert(producer != null);

            endOfFile = true;
        }

        
        
        
        
        
        
        
        
        
        internal bool GetInputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkCount, out bool eof)
        {
            chunkBuffer = chunkToReadBuffer;
            chunkOffset = chunkToReadOffset;
            chunkCount = chunkToReadCount;

            eof = endOfFile && 0 == chunkToReadCount;

            return 0 != chunkToReadCount || endOfFile;
        }

        
        
        
        
        
        internal void ReportRead(int readCount)
        {
            InternalDebug.Assert(readCount >= 0 && readCount <= chunkToReadCount);

            if (readCount != 0)
            {
                madeProgress = true;

                chunkToReadCount -= readCount;
                chunkToReadOffset += readCount;

                if (chunkToReadCount == 0)
                {
                    chunkToReadBuffer = null;
                    chunkToReadOffset = 0;
                }
            }
        }

        
        
        
        
        protected override void Dispose(bool disposing)
        {
            
            
            base.Dispose(disposing);
        }

        
        
        void IProgressMonitor.ReportProgress()
        {
            madeProgress = true;
        }

        
        internal void Reuse(object newSourceOrSink)
        {
            if (producer != null)
            {
                if (!(producer is IReusable))
                {
                    throw new NotSupportedException("this converter is not reusable");
                }

                ((IReusable)producer).Initialize(newSourceOrSink);
            }
            else
            {
                if (!(consumer is IReusable))
                {
                    throw new NotSupportedException("this converter is not reusable");
                }

                ((IReusable)consumer).Initialize(newSourceOrSink);
            }

            sourceOrDestination = newSourceOrSink;

            chunkToReadBuffer = null;
            chunkToReadOffset = 0;
            chunkToReadCount = 0;

            writeBuffer = null;
            writeOffset = 0;
            writeCount = 0;

            endOfFile = false;
            inconsistentState = false;
        }
    }
}

