// ***************************************************************
// <copyright file="ConverterEncodingOutput.cs" company="Microsoft">
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
    using Globalization;    

    
    internal class ConverterEncodingOutput : ConverterOutput, IByteSource, IRestartable, IReusable
    {
        protected IResultsFeedback resultFeedback;

        private const int LineSpaceThreshold = 256;
        private const int SpaceThreshold = 32;

        private Stream pushSink;
        private ConverterStream pullSink;
        private bool endOfFile;

        private bool restartablePushSink;
        private long restartPosition;

        private bool encodingSameAsInput;

        private bool restartable;
        private bool canRestart;
        private bool lineModeEncoding;

        private int minCharsEncode;

        private char[] lineBuffer;
        private int lineBufferCount;
        private int lineBufferLastNL;

        private ByteCache cache = new ByteCache();

        private Encoding originalEncoding;
        private Encoding encoding;
        private Encoder encoder;
        private bool encodingCompleteUnicode;

        private CodePageMap codePageMap = new CodePageMap();

        private bool isFirstChar = true;

        
        public ConverterEncodingOutput(
            Stream destination,
            bool push,
            bool restartable,
            Encoding encoding,
            bool encodingSameAsInput,
            bool testBoundaryConditions,
            IResultsFeedback resultFeedback)
        {
            this.resultFeedback = resultFeedback;

            if (!push)
            {
                pullSink = destination as ConverterStream;
                InternalDebug.Assert(pullSink != null);

                pullSink.SetSource(this);
            }
            else
            {
                InternalDebug.Assert(destination.CanWrite);

                pushSink = destination;

                if (restartable && destination.CanSeek && destination.Position == destination.Length)
                {
                    restartablePushSink = true;
                    restartPosition = destination.Position;
                }
            }

            this.restartable = canRestart = restartable;

            lineBuffer = new char[4096];

            minCharsEncode = testBoundaryConditions ? 1 : 256;

            this.encodingSameAsInput = encodingSameAsInput;

            originalEncoding = encoding;
            ChangeEncoding(encoding);

            if (this.resultFeedback != null)
            {
                this.resultFeedback.Set(ConfigParameter.OutputEncoding, this.encoding);
            }
        }

        
        private void Reinitialize()
        {
            endOfFile = false;
            lineBufferCount = 0;
            lineBufferLastNL = 0;
            isFirstChar = true;

            cache.Reset();

            encoding = null;
            ChangeEncoding(originalEncoding);

            canRestart = restartable;
        }

        
        public Encoding Encoding
        {
            get { return encoding; }
            set
            {
                if (encoding != value)
                {
                    ChangeEncoding(value);

                    if (resultFeedback != null)
                    {
                        resultFeedback.Set(ConfigParameter.OutputEncoding, encoding);
                    }
                }
            }
        }

        
        public bool CodePageSameAsInput => encodingSameAsInput;


        bool IRestartable.CanRestart()
        {
            return canRestart;
        }

        
        void IRestartable.Restart()
        {
            InternalDebug.Assert(canRestart);

            if (pullSink == null && restartablePushSink)
            {
                
                pushSink.Position = restartPosition;
                pushSink.SetLength(restartPosition);
            }

            Reinitialize();

            canRestart = false;
        }

        
        void IRestartable.DisableRestart()
        {
            InternalDebug.Assert(canRestart);

            canRestart = false;

            FlushCached();
        }

        
        void IReusable.Initialize(object newSourceOrDestination)
        {
            restartablePushSink = false;

            if (pushSink != null)
            {
                if (newSourceOrDestination != null)
                {
                    
                    var newSink = newSourceOrDestination as Stream;

                    if (newSink == null || !newSink.CanWrite)
                    {
                        throw new InvalidOperationException("cannot reinitialize this converter - new output should be a writable Stream object");
                    }

                    pushSink = newSink;

                    if (restartable && newSink.CanSeek && newSink.Position == newSink.Length)
                    {
                        restartablePushSink = true;
                        restartPosition = newSink.Position;
                    }
                }
            }

            Reinitialize();
        }

        
        public override bool CanAcceptMore => canRestart || pullSink == null || cache.Length == 0;


        public override void Write(char[] buffer, int offset, int count, IFallback fallback)
        {
            if (fallback == null && !lineModeEncoding && lineBufferCount + count <= lineBuffer.Length - minCharsEncode)
            {
                if (count == 1)
                {
                    lineBuffer[lineBufferCount++] = buffer[offset];
                    return;
                }
                else if (count < 16)
                {
                    if (0 != (count & 8))
                    {
                        lineBuffer[lineBufferCount] = buffer[offset];
                        lineBuffer[lineBufferCount + 1] = buffer[offset + 1];
                        lineBuffer[lineBufferCount + 2] = buffer[offset + 2];
                        lineBuffer[lineBufferCount + 3] = buffer[offset + 3];
                        lineBuffer[lineBufferCount + 4] = buffer[offset + 4];
                        lineBuffer[lineBufferCount + 5] = buffer[offset + 5];
                        lineBuffer[lineBufferCount + 6] = buffer[offset + 6];
                        lineBuffer[lineBufferCount + 7] = buffer[offset + 7];
                        lineBufferCount += 8;
                        offset += 8;
                    }

                    if (0 != (count & 4))
                    {
                        lineBuffer[lineBufferCount] = buffer[offset];
                        lineBuffer[lineBufferCount + 1] = buffer[offset + 1];
                        lineBuffer[lineBufferCount + 2] = buffer[offset + 2];
                        lineBuffer[lineBufferCount + 3] = buffer[offset + 3];
                        lineBufferCount += 4;
                        offset += 4;
                    }

                    if (0 != (count & 2))
                    {
                        lineBuffer[lineBufferCount] = buffer[offset];
                        lineBuffer[lineBufferCount + 1] = buffer[offset + 1];
                        lineBufferCount += 2;
                        offset += 2;
                    }

                    if (0 != (count & 1))
                    {
                        lineBuffer[lineBufferCount++] = buffer[offset];
                    }

                    return;
                }
            }

            WriteComplete(buffer, offset, count, fallback);
        }

        public void WriteComplete(char[] buffer, int offset, int count, IFallback fallback)
        {
            InternalDebug.Assert(!endOfFile);
            InternalDebug.Assert(encoding != null);

            if (fallback != null || lineModeEncoding)
            {
                byte unsafeAsciiMask = 0;
                byte[] unsafeAsciiMap = null;
                uint unsafeAsciiMapLength = 0;
                var hasUnsafeUnicode = false;
                var treatNonAsciiAsUnsafe = false;

                if (fallback != null)
                {
                    unsafeAsciiMap = fallback.GetUnsafeAsciiMap(out unsafeAsciiMask);
                    if (unsafeAsciiMap != null)
                    {
                        unsafeAsciiMapLength = (uint)unsafeAsciiMap.Length;
                    }

                    hasUnsafeUnicode = fallback.HasUnsafeUnicode();
                    treatNonAsciiAsUnsafe = fallback.TreatNonAsciiAsUnsafe(encoding.WebName);
                }

                while (0 != count)
                {
                    
                    

                    for (; 0 != count && lineBufferCount != lineBuffer.Length; count--, offset++)
                    {
                        var ch = buffer[offset];

                        if (fallback != null)
                        {
                            if (((uint)ch < unsafeAsciiMapLength && (unsafeAsciiMap[(int)ch] & unsafeAsciiMask) != 0) ||
                                (!encodingCompleteUnicode && (ch >= 0x7F || ch < ' ') && codePageMap.IsUnsafeExtendedCharacter(ch)) ||
                                (hasUnsafeUnicode && ch >= 0x7F && (treatNonAsciiAsUnsafe || fallback.IsUnsafeUnicode(ch, isFirstChar))))
                            {
                                if (!fallback.FallBackChar(ch, lineBuffer, ref lineBufferCount, lineBuffer.Length))
                                {
                                    
                                    

                                    break;
                                }
                                isFirstChar = false;
                                continue;
                            }
                        }

                        

                        lineBuffer[lineBufferCount++] = ch;
                        isFirstChar = false;

                        if (lineModeEncoding)
                        {
                            if (ch == '\n' || ch == '\r')
                            {
                                
                                lineBufferLastNL = lineBufferCount;
                            }
                            else if (lineBufferLastNL > lineBuffer.Length - LineSpaceThreshold)
                            {
                                count--;
                                offset++;

                                break;
                            }
                        }
                    }

                    

                    
                    

                    if (lineModeEncoding &&
                        (lineBufferLastNL > lineBuffer.Length - LineSpaceThreshold ||
                        (lineBufferCount > lineBuffer.Length - SpaceThreshold &&
                        lineBufferLastNL != 0)))
                    {
                        

                        EncodeBuffer(lineBuffer, 0, lineBufferLastNL, false);

                        lineBufferCount -= lineBufferLastNL;

                        if (lineBufferCount != 0)
                        {
                            

                            Buffer.BlockCopy(lineBuffer, lineBufferLastNL * 2, lineBuffer, 0, lineBufferCount * 2);
                        }
                    }
                    else if (lineBufferCount > lineBuffer.Length - Math.Max(minCharsEncode, SpaceThreshold))
                    {
                        

                        EncodeBuffer(lineBuffer, 0, lineBufferCount, false);

                        lineBufferCount = 0;
                    }

                    lineBufferLastNL = 0;
                }
            }
            else
            {
                
                

                if (count > minCharsEncode)
                {
                    

                    if (lineBufferCount != 0)
                    {
                        
                        
                        
                        

                        EncodeBuffer(lineBuffer, 0, lineBufferCount, false);

                        lineBufferCount = 0;
                        lineBufferLastNL = 0;
                    }

                    
                    

                    EncodeBuffer(buffer, offset, count, false);
                }
                else
                {
                    InternalDebug.Assert(lineBufferCount + count <= lineBuffer.Length);

                    
                    
                    
                    

                    Buffer.BlockCopy(buffer, offset * 2, lineBuffer, lineBufferCount * 2, count * 2);
                    lineBufferCount += count;

                    if (lineBufferCount > lineBuffer.Length - minCharsEncode)
                    {
                        
                        
                        
                        

                        EncodeBuffer(lineBuffer, 0, lineBufferCount, false);

                        lineBufferCount = 0;
                        lineBufferLastNL = 0;
                    }
                }
            }
        }

        
        public override void Write(string text)
        {
            if (text.Length == 0)
            {
                return;
            }

            if (lineModeEncoding || lineBufferCount + text.Length > lineBuffer.Length - minCharsEncode)
            {
                
                Write(text, 0, text.Length);
                return;
            }

            

            if (text.Length <= 4)
            {
                
                

                var count = text.Length;

                lineBuffer[lineBufferCount++] = text[0];
                if (--count != 0)
                {
                    lineBuffer[lineBufferCount++] = text[1];
                    if (--count != 0)
                    {
                        lineBuffer[lineBufferCount++] = text[2];
                        if (--count != 0)
                        {
                            lineBuffer[lineBufferCount++] = text[3];
                        }
                    }
                }
            }
            else
            {
                text.CopyTo(0, lineBuffer, lineBufferCount, text.Length);
                lineBufferCount += text.Length;
            }
        }

        
        public override void Flush()
        {
            if (endOfFile)
            {
                return;
            }

            canRestart = false;

            FlushCached();

            

            EncodeBuffer(lineBuffer, 0, lineBufferCount, true);

            lineBufferCount = 0;
            lineBufferLastNL = 0;

            if (pullSink == null)
            {
                pushSink.Flush();
            }
            else
            {
                
                

                if (cache.Length == 0)
                {
                    pullSink.ReportEndOfFile();
                }
            }

            endOfFile = true;
        }

        
        bool IByteSource.GetOutputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkLength)
        {
            if (cache.Length == 0 || canRestart)
            {
                chunkBuffer = null;
                chunkOffset = 0;
                chunkLength = 0;
                return false;
            }

            cache.GetData(out chunkBuffer, out chunkOffset, out chunkLength);
            return true;
        }

        
        void IByteSource.ReportOutput(int readCount)
        {
            InternalDebug.Assert(cache.Length >= readCount);

            cache.ReportRead(readCount);

            if (cache.Length == 0 && endOfFile)
            {
                pullSink.ReportEndOfFile();
            }
        }

        
        protected override void Dispose()
        {
            if (cache != null && cache is IDisposable)
            {
                ((IDisposable)cache).Dispose();
            }

            cache = null;
            pushSink = null;
            pullSink = null;
            lineBuffer = null;
            encoding = null;
            encoder = null;
            codePageMap = null;

            base.Dispose();
        }

        
        private void EncodeBuffer(char[] buffer, int offset, int count, bool flush)
        {
            var maxSpaceRequired = encoding.GetMaxByteCount(count);

            byte[] outputBuffer, directBuffer = null;
            int outputOffset, directOffset = 0;
            int outputCount, directSpace = 0;
            var encodingToCache = true;

            
            

            if (canRestart || pullSink == null || cache.Length != 0)
            {
                

                cache.GetBuffer(maxSpaceRequired, out outputBuffer, out outputOffset);
            }
            else
            {
                

                pullSink.GetOutputBuffer(out directBuffer, out directOffset, out directSpace);

                if (directSpace >= maxSpaceRequired)
                {
                    

                    outputBuffer = directBuffer;
                    outputOffset = directOffset;

                    encodingToCache = false;
                }
                else
                {
                    cache.GetBuffer(maxSpaceRequired, out outputBuffer, out outputOffset);
                }
            }

            var encodedCount = encoder.GetBytes(buffer, offset, count, outputBuffer, outputOffset, flush);

            if (encodingToCache)
            {
                cache.Commit(encodedCount);

                if (pullSink == null)
                {
                    if (!canRestart || restartablePushSink)
                    {
                        

                        while (cache.Length != 0)
                        {
                            cache.GetData(out outputBuffer, out outputOffset, out outputCount);

                            pushSink.Write(outputBuffer, outputOffset, outputCount);

                            cache.ReportRead(outputCount);

                            InternalDebug.Assert(outputCount > 0);
                        }
                    }
                }
                else
                {
                    if (!canRestart)
                    {
                        encodedCount = cache.Read(directBuffer, directOffset, directSpace);

                        pullSink.ReportOutput(encodedCount);
                    }
                }
            }
            else
            {
                pullSink.ReportOutput(encodedCount);
            }
        }

        
        internal void ChangeEncoding(Encoding newEncoding)
        {
            

            if (encoding != null)
            {
                

                EncodeBuffer(lineBuffer, 0, lineBufferCount, true);

                lineBufferCount = 0;
                lineBufferLastNL = 0;
            }

            encoding = newEncoding;
            encoder = newEncoding.GetEncoder();
            var encodingCodePage = newEncoding.CodePage;

            if (encodingCodePage == 1200 ||        
                encodingCodePage == 1201 ||        
                encodingCodePage == 12000 ||       
                encodingCodePage == 12001 ||       
                encodingCodePage == 65000 ||       
                encodingCodePage == 65001 ||       
                encodingCodePage == 65005 ||       
                encodingCodePage == 65006 ||       
                encodingCodePage == 54936)         
            {
                lineModeEncoding = false;
                encodingCompleteUnicode = true;
                codePageMap.ChoseCodePage(1200);
            }
            else
            {
                encodingCompleteUnicode = false;
                codePageMap.ChoseCodePage(encodingCodePage);

                if (encodingCodePage == 50220 ||       
                    encodingCodePage == 50221 ||       
                    encodingCodePage == 50222 ||       
                    encodingCodePage == 50225 ||       
                    encodingCodePage == 50227 ||       
                    encodingCodePage == 50229 ||       
                    encodingCodePage == 52936)         
                {
                    
                    
                    
                    

                    lineModeEncoding = true;
                }
            }
        }

        
        private bool FlushCached()
        {
            if (canRestart || cache.Length == 0)
            {
                return false;
            }

            

            byte[] outputBuffer;
            int outputOffset;
            int outputSpace;
            int outputCount;

            if (pullSink == null)
            {
                

                while (cache.Length != 0)
                {
                    cache.GetData(out outputBuffer, out outputOffset, out outputCount);

                    pushSink.Write(outputBuffer, outputOffset, outputCount);

                    cache.ReportRead(outputCount);

                    InternalDebug.Assert(outputCount > 0);
                }
            }
            else
            {
                pullSink.GetOutputBuffer(out outputBuffer, out outputOffset, out outputSpace);

                outputCount = cache.Read(outputBuffer, outputOffset, outputSpace);

                pullSink.ReportOutput(outputCount);
            }

            return true;
        }
    }
}

