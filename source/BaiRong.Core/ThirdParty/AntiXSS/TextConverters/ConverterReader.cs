// ***************************************************************
// <copyright file="ConverterReader.cs" company="Microsoft">
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

    
    
    
    internal class ConverterReader : TextReader, IProgressMonitor
    {
        private ConverterUnicodeOutput sourceOutputObject;
        private IProducerConsumer producer;

        private bool madeProgress;
        private int maxLoopsWithoutProgress;

        private char[] writeBuffer;
        private int writeIndex;
        private int writeCount;

        private object source;

        private bool endOfFile;

        private bool inconsistentState;

        

        
        
        
        
        
        public ConverterReader(Stream sourceStream, TextConverter converter)
        {
            if (sourceStream == null)
            {
                throw new ArgumentNullException("sourceStream");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            if (!sourceStream.CanRead)
            {
                throw new ArgumentException(Strings.CannotReadFromSource, "sourceStream");
            }

            producer = converter.CreatePullChain(sourceStream, this);

            
            source = sourceStream;

            maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        
        
        
        
        
        public ConverterReader(TextReader sourceReader, TextConverter converter)
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

            
            source = sourceReader;

            maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        
        

        
        
        
        
        public override int Peek()
        {
            if (source == null)
            {
                throw new ObjectDisposedException("ConverterReader");
            }

            if (inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterReaderInInconsistentStare);
            }

            long loopsWithoutProgress = 0;

            char[] chunkBuffer;
            int chunkIndex;
            int chunkCount;

            
            
            inconsistentState = true;

            while (!endOfFile)
            {
                

                if (sourceOutputObject.GetOutputChunk(out chunkBuffer, out chunkIndex, out chunkCount))
                {
                    InternalDebug.Assert(chunkCount != 0);

                    
                    inconsistentState = false;

                    return (int)(ushort)chunkBuffer[chunkIndex];
                }

                producer.Run();

                if (madeProgress)
                {
                    madeProgress = false;
                    loopsWithoutProgress = 0;
                }
                else if (maxLoopsWithoutProgress == loopsWithoutProgress++)
                {
                    InternalDebug.Assert(false);
                    throw new TextConvertersException(Strings.TooManyIterationsToProduceOutput);
                }
            }

            
            inconsistentState = false;

            return -1;
        }
        
        
        
        
        
        public override int Read()
        {
            if (source == null)
            {
                throw new ObjectDisposedException("ConverterReader");
            }

            if (inconsistentState)
            {
                throw new InvalidOperationException(Strings.ConverterReaderInInconsistentStare);
            }

            long loopsWithoutProgress = 0;

            char[] chunkBuffer;
            int chunkIndex;
            int chunkCount;

            
            
            inconsistentState = true;

            while (!endOfFile)
            {
                

                if (sourceOutputObject.GetOutputChunk(out chunkBuffer, out chunkIndex, out chunkCount))
                {
                    InternalDebug.Assert(chunkCount != 0);

                    sourceOutputObject.ReportOutput(1);

                    
                    inconsistentState = false;

                    return (int)(ushort)chunkBuffer[chunkIndex];
                }

                producer.Run();

                if (madeProgress)
                {
                    madeProgress = false;
                    loopsWithoutProgress = 0;
                }
                else if (maxLoopsWithoutProgress == loopsWithoutProgress++)
                {
                    InternalDebug.Assert(false);
                    throw new TextConvertersException(Strings.TooManyIterationsToProduceOutput);
                }
            }

            
            inconsistentState = false;

            return -1;
        }

        
        
        
        
        
        
        
        public override int Read(char[] buffer, int index, int count)
        {
            if (source == null)
            {
                throw new ObjectDisposedException("ConverterReader");
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
                throw new InvalidOperationException(Strings.ConverterReaderInInconsistentStare);
            }

            var initialCount = count;

            
            

            char[] chunkBuffer;
            int chunkIndex;
            int chunkCount;

            while (count != 0 && sourceOutputObject.GetOutputChunk(out chunkBuffer, out chunkIndex, out chunkCount))
            {
                var charsRead = Math.Min(chunkCount, count);

                Buffer.BlockCopy(chunkBuffer, chunkIndex * 2, buffer, index * 2, charsRead * 2);

                index += charsRead;
                count -= charsRead;

                sourceOutputObject.ReportOutput(charsRead);
            }

            
            

            if (0 != count)
            {
                

                
                

                long loopsWithoutProgress = 0;

                writeBuffer = buffer;
                writeIndex = index;
                writeCount = count;

                
                
                inconsistentState = true;

                while (0 != writeCount && !endOfFile)
                {
                    

                    producer.Run();

                    if (madeProgress)
                    {
                        madeProgress = false;
                        loopsWithoutProgress = 0;
                    }
                    else if (maxLoopsWithoutProgress == loopsWithoutProgress++)
                    {
                        InternalDebug.Assert(false);
                        throw new TextConvertersException(Strings.TooManyIterationsToProduceOutput);
                    }
                }

                count = writeCount;

                writeBuffer = null;
                writeIndex = 0;
                writeCount = 0;

                
                inconsistentState = false;
            }

            return initialCount - count;
        }

        internal void SetSource(ConverterUnicodeOutput sourceOutputObject)
        {
            this.sourceOutputObject = sourceOutputObject;
        }

        internal void GetOutputBuffer(out char[] outputBuffer, out int outputIndex, out int outputCount)
        {
            InternalDebug.Assert(!endOfFile);

            outputBuffer = writeBuffer;
            outputIndex = writeIndex;
            outputCount = writeCount;
        }

        internal void ReportOutput(int outputCount)
        {
            InternalDebug.Assert(!endOfFile && outputCount <= writeCount);

            if (outputCount != 0)
            {
                writeCount -= outputCount;
                writeIndex += outputCount;

                madeProgress = true;
            }
        }

        internal void ReportEndOfFile()
        {
            endOfFile = true;
        }

        
        
        
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (source != null)
                {
                    if (source is Stream)
                    {
                        ((Stream)source).Close();
                    }
                    else
                    {
                        ((TextReader)source).Close();
                    }
                }
            }

            if (producer != null && producer is IDisposable)
            {
                ((IDisposable)producer).Dispose();
            }

            source = null;
            producer = null;
            sourceOutputObject = null;
            writeBuffer = null;

            base.Dispose(disposing);
        }

        void IProgressMonitor.ReportProgress()
        {
            madeProgress = true;
        }

        
        internal void Reuse(object newSource)
        {
            if (!(producer is IReusable))
            {
                throw new NotSupportedException("this converter is not reusable");
            }

            ((IReusable)producer).Initialize(newSource);

            source = newSource;

            writeBuffer = null;
            writeIndex = 0;
            writeCount = 0;

            endOfFile = false;
            inconsistentState = false;
        }
    }
}
