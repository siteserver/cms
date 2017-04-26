// ***************************************************************
// <copyright file="ConverterBufferInput.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using Data.Internal;

    
    internal class ConverterBufferInput : ConverterInput, ITextSink, IDisposable
    {
        private const int DefaultMaxLength = 32 * 1024;
        private int maxLength;
        private string originalFragment;
        private char[] parseBuffer;

        
        public ConverterBufferInput(IProgressMonitor progressMonitor) :
            this(DefaultMaxLength, progressMonitor)
        {
        }

        
        public ConverterBufferInput(int maxLength, IProgressMonitor progressMonitor) :
            base(progressMonitor)
        {
            
            

            this.maxLength = maxLength;
        }

        
        public ConverterBufferInput(string fragment, IProgressMonitor progressMonitor) :
            this(DefaultMaxLength, fragment, progressMonitor)
        {
        }

        
        public ConverterBufferInput(int maxLength, string fragment, IProgressMonitor progressMonitor) :
            base(progressMonitor)
        {
            this.maxLength = maxLength;

            originalFragment = fragment;

            parseBuffer = new char[fragment.Length + 1];
            fragment.CopyTo(0, parseBuffer, 0, fragment.Length);
            parseBuffer[fragment.Length] = '\0';
            maxTokenSize = fragment.Length;
        }

#if PRIVATEBUILD
        
        public ConverterBufferInput(char[] fragment, int offset, int count, IProgressMonitor progressMonitor) :
            base(progressMonitor)
        {
            this.parseBuffer = new char[count + 1];
            Buffer.BlockCopy(fragment, offset * 2, this.parseBuffer, 0, count * 2);
            this.parseBuffer[count] = '\0';
            this.maxTokenSize = count;
        }
#endif

        
        public bool IsEnough => (maxTokenSize >= maxLength);


        public bool IsEmpty => (maxTokenSize == 0);


        public void Write(string str)
        {
            var count = PrepareToBuffer(str.Length);

            if (count > 0)
            {
                str.CopyTo(0, parseBuffer, maxTokenSize, count);

                maxTokenSize += count;
                parseBuffer[maxTokenSize] = '\0';
            }
        }

        
        public void Write(char[] buffer, int offset, int count)
        {
            count = PrepareToBuffer(count);

            if (count > 0)
            {
                Buffer.BlockCopy(buffer, offset * 2, parseBuffer, maxTokenSize * 2, count * 2);

                maxTokenSize += count;
                parseBuffer[maxTokenSize] = '\0';
            }
        }

        
        public void Write(int ucs32Char)
        {
            int count;

            if (ucs32Char > 0xFFFF)
            {
                count = PrepareToBuffer(2);
                if (count > 0)
                {
                    
                    
                    parseBuffer[maxTokenSize] = ParseSupport.HighSurrogateCharFromUcs4(ucs32Char);
                    parseBuffer[maxTokenSize + 1] = ParseSupport.LowSurrogateCharFromUcs4(ucs32Char);
                    maxTokenSize += count;
                    parseBuffer[maxTokenSize] = '\0';
                }
            }
            else
            {
                count = PrepareToBuffer(1);
                if (count > 0)
                {
                    
                    parseBuffer[maxTokenSize++] = (char)ucs32Char;
                    parseBuffer[maxTokenSize] = '\0';
                }
            }
        }

        
        public void Reset()
        {
            maxTokenSize = 0;
            endOfFile = false;
        }

        
        public void Initialize(string fragment)
        {
            if (originalFragment != fragment)
            {
                originalFragment = fragment;

                parseBuffer = new char[fragment.Length + 1];
                fragment.CopyTo(0, parseBuffer, 0, fragment.Length);
                parseBuffer[fragment.Length] = '\0';
                maxTokenSize = fragment.Length;
            }

            endOfFile = false;
        }

        
        public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
        {
            InternalDebug.Assert((buffer == null && start == 0 && current == 0 && end == 0) ||
                                (buffer == parseBuffer &&
                                
                                end <= maxTokenSize &&
                                start <= current &&
                                current <= end));

            if (buffer == null)
            {
                buffer = parseBuffer;
                start = 0;
                end = maxTokenSize;
                current = 0;

                if (end != 0)
                {
                    return true;
                }
            }

            endOfFile = true;
            return true;
        }

        
        public override void ReportProcessed(int processedSize)
        {
            InternalDebug.Assert(processedSize >= 0);
            progressMonitor.ReportProgress();
        }

        
        public override int RemoveGap(int gapBegin, int gapEnd)
        {
            
            

            
            
            

            parseBuffer[gapBegin] = '\0';
            return gapBegin;
        }


        
        protected override void Dispose()
        {
            parseBuffer = null;
            base.Dispose();
        }

        
        private int PrepareToBuffer(int count)
        {
            if (maxTokenSize + count > maxLength)
            {
                count = maxLength - maxTokenSize;
            }

            if (count > 0)
            {
                if (null == parseBuffer)
                {
                    InternalDebug.Assert(maxTokenSize == 0);
                    parseBuffer = new char[count + 1];
                }
                else if (parseBuffer.Length <= maxTokenSize + count)
                {
                    var oldBuffer = parseBuffer;

                    

                    var newLength = (maxTokenSize + count) * 2;
                    if (newLength > maxLength)
                    {
                        newLength = maxLength;
                    }
                    
                    parseBuffer = new char[newLength + 1];

                    if (maxTokenSize > 0)
                    {
                        Buffer.BlockCopy(oldBuffer, 0, parseBuffer, 0, maxTokenSize * 2);
                    }
                }
            }

            return count;
        }
    }
}

