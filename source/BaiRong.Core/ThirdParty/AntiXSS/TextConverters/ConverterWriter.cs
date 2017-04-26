// ***************************************************************
// <copyright file="ConverterWriter.cs" company="Microsoft">
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
    using System.Text;
    using Data.Internal;
    using Strings = CtsResources.TextConvertersStrings;

    
    
    
    internal class ConverterWriter : TextWriter, IProgressMonitor
    {
        private ConverterUnicodeInput sinkInputObject;
        private IProducerConsumer consumer;

        private bool madeProgress;
        private int maxLoopsWithoutProgress;

        private char[] chunkToReadBuffer;
        private int chunkToReadIndex;
        private int chunkToReadCount;

        private object destination;

        private bool endOfFile;

        private bool inconsistentState;

        private bool boundaryTesting;

        
        
        
        
        
        public ConverterWriter(Stream destinationStream, TextConverter converter)
        {
            if (destinationStream == null)
            {
                throw new ArgumentNullException("destinationStream");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            if (!destinationStream.CanWrite)
            {
                throw new ArgumentException(Strings.CannotWriteToDestination, "destinationStream");
            }

            consumer = converter.CreatePushChain(this, destinationStream);

            
            destination = destinationStream;

            boundaryTesting = converter.TestBoundaryConditions;

            maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        

        
        
        
        
        
        public ConverterWriter(TextWriter destinationWriter, TextConverter converter)
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

            
            destination = destinationWriter;

            boundaryTesting = converter.TestBoundaryConditions;

            maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        
        
        
        
        
        public override Encoding Encoding => null;


        public override void Flush() 
        {
            if (destination == null)
            {
                throw new ObjectDisposedException("ConverterWriter");
            }

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

            if (destination is Stream)
            {
                ((Stream)destination).Flush();
            }
            else
            {
                ((TextWriter)destination).Flush();
            }
        }

        
        
        
        
        
        public override void Write(char value)
        {
            if (destination == null)
            {
                throw new ObjectDisposedException("ConverterWriter");
            }

            if (inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterWriterInInconsistentStare);
            }

            

            var parseCount = 10000;

            if (!boundaryTesting)
            {
                char[] inputBuffer;
                int inputIndex;
                int inputCount;

                sinkInputObject.GetInputBuffer(out inputBuffer, out inputIndex, out inputCount, out parseCount);

                if (inputCount >= 1)
                {
                    inputBuffer[inputIndex] = value;
                    sinkInputObject.Commit(1);
                    return;
                }
            }

            

            var buffer = new char[]
            {
                value
            };

            WriteBig(buffer, 0, 1, parseCount);
        }

        
        
        
        
        
        public override void Write(char[] buffer)
        {
            if (destination == null)
            {
                throw new ObjectDisposedException("ConverterWriter");
            }

            if (inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterWriterInInconsistentStare);
            }

            
            
            if (buffer == null)
            {
                return;
            }

            

            var parseCount = 10000;

            if (!boundaryTesting)
            {
                char[] inputBuffer;
                int inputIndex;
                int inputCount;

                sinkInputObject.GetInputBuffer(out inputBuffer, out inputIndex, out inputCount, out parseCount);

                if (inputCount >= buffer.Length)
                {
                    Buffer.BlockCopy(buffer, 0, inputBuffer, inputIndex * 2, buffer.Length * 2);
                    sinkInputObject.Commit(buffer.Length);
                    return;
                }
            }

            

            WriteBig(buffer, 0, buffer.Length, parseCount);
        }

        
        
        
        
        
        
        
        public override void Write(char[] buffer, int index, int count)
        {
            if (destination == null)
            {
                throw new ObjectDisposedException("ConverterWriter");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index", Strings.IndexOutOfRange);
            }

            if (count < 0 || count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (buffer.Length - index < count)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            if (inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterWriterInInconsistentStare);
            }

            

            var parseCount = 10000;

            if (!boundaryTesting)
            {
                char[] inputBuffer;
                int inputIndex;
                int inputCount;

                sinkInputObject.GetInputBuffer(out inputBuffer, out inputIndex, out inputCount, out parseCount);

                if (inputCount >= count)
                {
                    Buffer.BlockCopy(buffer, index * 2, inputBuffer, inputIndex * 2, count * 2);
                    sinkInputObject.Commit(count);
                    return;
                }
            }

            

            WriteBig(buffer, index, count, parseCount);
        }

        
        
        
        
        
        public override void Write(string value)
        {
            if (destination == null)
            {
                throw new ObjectDisposedException("ConverterWriter");
            }

            if (inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterWriterInInconsistentStare);
            }

            if (value == null)
            {
                return;
            }

            

            var parseCount = 10000;

            if (!boundaryTesting)
            {
                char[] inputBuffer;
                int inputIndex;
                int inputCount;

                sinkInputObject.GetInputBuffer(out inputBuffer, out inputIndex, out inputCount, out parseCount);

                if (inputCount >= value.Length)
                {
                    value.CopyTo(0, inputBuffer, inputIndex, value.Length);
                    sinkInputObject.Commit(value.Length);
                    return;
                }
            }

            

            var buffer = value.ToCharArray();

            WriteBig(buffer, 0, value.Length, parseCount);
        }

        
        
        
        
        
        public override void WriteLine(string value)
        {
            
            

            Write(value);
            WriteLine();
        }

        
        
        
        
        
        internal void SetSink(ConverterUnicodeInput sinkInputObject)
        {
            this.sinkInputObject = sinkInputObject;
        }

        
        
        
        
        
        
        
        
        
        internal bool GetInputChunk(out char[] chunkBuffer, out int chunkIndex, out int chunkCount, out bool eof)
        {
            chunkBuffer = chunkToReadBuffer;
            chunkIndex = chunkToReadIndex;
            chunkCount = chunkToReadCount;

            eof = endOfFile && 0 == chunkToReadCount;

            return 0 != chunkToReadCount || endOfFile;
        }

        
        
        
        
        
        internal void ReportRead(int readCount)
        {
            InternalDebug.Assert(readCount <= chunkToReadCount);

            if (readCount != 0)
            {
                chunkToReadCount -= readCount;
                chunkToReadIndex += readCount;

                if (chunkToReadCount == 0)
                {
                    chunkToReadBuffer = null;
                    chunkToReadIndex = 0;
                }

                madeProgress = true;
            }
        }

        
        
        
        
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (destination != null)
                {
                    if (!inconsistentState)
                    {
                        Flush();
                    }

                    if (destination is Stream)
                    {
                        ((Stream)destination).Close();
                    }
                    else
                    {
                        ((TextWriter)destination).Close();
                    }
                }
            }

            if (consumer != null && consumer is IDisposable)
            {
                ((IDisposable)consumer).Dispose();
            }

            destination = null;
            consumer = null;
            sinkInputObject = null;
            chunkToReadBuffer = null;

            base.Dispose(disposing);
        }

        
        
        
        
        
        
        
        
        private void WriteBig(char[] buffer, int index, int count, int parseCount)
        {
            chunkToReadBuffer = buffer;
            chunkToReadIndex = index;
            chunkToReadCount = count;

            
            
            
            
            
            
            
            long loopsWithoutProgress = 0;

            
            
            inconsistentState = true;

            while (0 != chunkToReadCount)
            {
                
                consumer.Run();

                if (madeProgress)
                {
                    madeProgress = false;
                    loopsWithoutProgress = 0;
                }
                else if (maxLoopsWithoutProgress == loopsWithoutProgress++)
                {
                    InternalDebug.Assert(false);
                    throw new TextConvertersException(Strings.TooManyIterationsToProcessInput);
                }
            }

            
            inconsistentState = false;
        }

        
        
        void IProgressMonitor.ReportProgress()
        {
            madeProgress = true;
        }

        
        internal void Reuse(object newSink)
        {
            if (!(consumer is IReusable))
            {
                throw new NotSupportedException("this converter is not reusable");
            }

            ((IReusable)consumer).Initialize(newSink);

            destination = newSink;

            chunkToReadBuffer = null;
            chunkToReadIndex = 0;
            chunkToReadCount = 0;

            endOfFile = false;
            inconsistentState = false;
        }
    }
}
