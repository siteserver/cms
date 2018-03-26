// ***************************************************************
// <copyright file="ConverterUnicodeOutput.cs" company="Microsoft">
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

    
    internal class ConverterUnicodeOutput : ConverterOutput, IRestartable, IReusable, IDisposable
    {
        private const int FallbackExpansionMax = 16;

        private TextWriter pushSink;
        private ConverterReader pullSink;

        private bool endOfFile;

        private bool restartable;
        private bool canRestart;
        private bool isFirstChar = true;

        private TextCache cache = new TextCache();

        
        public ConverterUnicodeOutput(object destination, bool push, bool restartable)
        {
            if (push)
            {
                pushSink = destination as TextWriter;
                InternalDebug.Assert(pushSink != null);
            }
            else
            {
                pullSink = destination as ConverterReader;
                InternalDebug.Assert(pullSink != null);

                pullSink.SetSource(this);
            }

            this.restartable = canRestart = restartable;
        }

        
        private void Reinitialize()
        {
            endOfFile = false;
            cache.Reset();
            canRestart = restartable;
            isFirstChar = true;
        }

        
        bool IRestartable.CanRestart()
        {
            return canRestart;
        }

        
        void IRestartable.Restart()
        {
            InternalDebug.Assert(canRestart);

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
            if (pushSink != null)
            {
                if (newSourceOrDestination != null)
                {
                    
                    var newSink = newSourceOrDestination as TextWriter;

                    if (newSink == null)
                    {
                        throw new InvalidOperationException("cannot reinitialize this converter - new output should be a TextWriter object");
                    }

                    pushSink = newSink;
                }
            }

            Reinitialize();
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

            base.Dispose();
        }

        
        public override bool CanAcceptMore => canRestart || pullSink == null || cache.Length == 0;


        public override void Write(char[] buffer, int offset, int count, IFallback fallback)
        {
            InternalDebug.Assert(!endOfFile);

            byte unsafeAsciiMask = 0;
            var unsafeAsciiMap = fallback == null ? null : fallback.GetUnsafeAsciiMap(out unsafeAsciiMask);
            var hasUnsafeUnicode = fallback == null ? false : fallback.HasUnsafeUnicode();

            if (cache.Length != 0 || canRestart)
            {
                

                while (count != 0)
                {
                    char[] cacheBuffer;
                    int cacheOffset;
                    int cacheSpace;

                    if (fallback != null)
                    {
                        cache.GetBuffer(FallbackExpansionMax, out cacheBuffer, out cacheOffset, out cacheSpace);

                        var cacheOffsetStart = cacheOffset;

                        for (; 0 != count && cacheSpace != 0; count--, offset++)
                        {
                            var ch = buffer[offset];

                            if (IsUnsafeCharacter(ch, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, isFirstChar, fallback))
                            {
                                var cacheOffsetSave = cacheOffset;

                                if (!fallback.FallBackChar(ch, cacheBuffer, ref cacheOffset, cacheOffset + cacheSpace))
                                {
                                    
                                    

                                    break;
                                }
                                cacheSpace -= (cacheOffset - cacheOffsetSave);
                            }
                            else
                            {
                                

                                cacheBuffer[cacheOffset++] = ch;
                                cacheSpace--;
                            }
                            isFirstChar = false;
                        }

                        
                        
                        

                        cache.Commit(cacheOffset - cacheOffsetStart);
                    }
                    else
                    {
                        var minSpace = Math.Min(count, 256);

                        cache.GetBuffer(minSpace, out cacheBuffer, out cacheOffset, out cacheSpace);

                        var countToCopy = Math.Min(cacheSpace, count);

                        Buffer.BlockCopy(buffer, offset * 2, cacheBuffer, cacheOffset * 2, countToCopy * 2);

                        isFirstChar = false;
                        cache.Commit(countToCopy);

                        offset += countToCopy;
                        count -= countToCopy;
                    }
                }
            }
            else if (pullSink != null)
            {
                

                char[] pullBuffer;
                int pullOffset;
                int pullSpace;

                pullSink.GetOutputBuffer(out pullBuffer, out pullOffset, out pullSpace);

                if (pullSpace != 0)
                {
                    if (fallback != null)
                    {
                        var pullStartOffset = pullOffset;

                        for (; 0 != count && 0 != pullSpace; count--, offset++)
                        {
                            var ch = buffer[offset];

                            if (IsUnsafeCharacter(ch, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, isFirstChar, fallback))
                            {
                                var pullOffsetSave = pullOffset;

                                if (!fallback.FallBackChar(ch, pullBuffer, ref pullOffset, pullOffset + pullSpace))
                                {
                                    
                                    

                                    break;
                                }

                                pullSpace -= (pullOffset - pullOffsetSave);
                            }
                            else
                            {
                                

                                pullBuffer[pullOffset++] = ch;
                                pullSpace--;
                            }
                            isFirstChar = false;
                        }

                        pullSink.ReportOutput(pullOffset - pullStartOffset);
                    }
                    else
                    {
                        var countToCopy = Math.Min(pullSpace, count);

                        Buffer.BlockCopy(buffer, offset * 2, pullBuffer, pullOffset * 2, countToCopy * 2);

                        isFirstChar = false;
                        count -= countToCopy;
                        offset += countToCopy;

                        pullSink.ReportOutput(countToCopy);

                        pullOffset += countToCopy;
                        pullSpace -= countToCopy;
                    }
                }

                

                while (count != 0)
                {
                    char[] cacheBuffer;
                    int cacheOffset;
                    int cacheSpace;

                    if (fallback != null)
                    {
                        cache.GetBuffer(FallbackExpansionMax, out cacheBuffer, out cacheOffset, out cacheSpace);

                        var cacheOffsetStart = cacheOffset;

                        for (; 0 != count && cacheSpace != 0; count--, offset++)
                        {
                            var ch = buffer[offset];

                            if (IsUnsafeCharacter(ch, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, isFirstChar, fallback))
                            {
                                var cacheOffsetSave = cacheOffset;

                                if (!fallback.FallBackChar(ch, cacheBuffer, ref cacheOffset, cacheOffset + cacheSpace))
                                {
                                    
                                    

                                    break;
                                }

                                cacheSpace -= (cacheOffset - cacheOffsetSave);
                            }
                            else
                            {
                                

                                cacheBuffer[cacheOffset++] = ch;
                                cacheSpace--;
                            }
                            isFirstChar = false;
                        }

                        
                        
                        

                        cache.Commit(cacheOffset - cacheOffsetStart);
                    }
                    else
                    {
                        var minSpace = Math.Min(count, 256);

                        cache.GetBuffer(minSpace, out cacheBuffer, out cacheOffset, out cacheSpace);

                        var countToCopy = Math.Min(cacheSpace, count);

                        Buffer.BlockCopy(buffer, offset * 2, cacheBuffer, cacheOffset * 2, countToCopy * 2);
                        isFirstChar = false;

                        cache.Commit(countToCopy);

                        offset += countToCopy;
                        count -= countToCopy;
                    }
                }

                

                while (pullSpace != 0 && cache.Length != 0)
                {
                    
                    
                    
                    

                    char[] outputBuffer;
                    int outputOffset;
                    int outputCount;

                    cache.GetData(out outputBuffer, out outputOffset, out outputCount);

                    var countToCopy = Math.Min(outputCount, pullSpace);

                    Buffer.BlockCopy(outputBuffer, outputOffset * 2, pullBuffer, pullOffset * 2, countToCopy * 2);

                    cache.ReportRead(countToCopy);

                    pullSink.ReportOutput(countToCopy);

                    pullOffset += countToCopy;
                    pullSpace -= countToCopy;
                }
            }
            else
            {
                
                

                if (fallback != null)
                {
                    char[] cacheBuffer;
                    int cacheOffset;
                    int cacheSpace;

                    
                    
                    cache.GetBuffer(1024, out cacheBuffer, out cacheOffset, out cacheSpace);

                    var cacheOffsetStart = cacheOffset;
                    var cacheSpaceStart = cacheSpace;

                    while (count != 0)
                    {
                        for (; 0 != count && cacheSpace != 0; count--, offset++)
                        {
                            var ch = buffer[offset];

                            if (IsUnsafeCharacter(ch, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, isFirstChar, fallback))
                            {
                                var cacheOffsetSave = cacheOffset;

                                if (!fallback.FallBackChar(ch, cacheBuffer, ref cacheOffset, cacheOffset + cacheSpace))
                                {
                                    
                                    

                                    break;
                                }

                                cacheSpace -= (cacheOffset - cacheOffsetSave);
                            }
                            else
                            {
                                

                                cacheBuffer[cacheOffset++] = ch;
                                cacheSpace--;
                            }
                            isFirstChar = false;
                        }

                        if (cacheOffset - cacheOffsetStart != 0)
                        {
                            pushSink.Write(cacheBuffer, cacheOffsetStart, cacheOffset - cacheOffsetStart);

                            cacheOffset = cacheOffsetStart;
                            cacheSpace = cacheSpaceStart;
                        }
                    }
                }
                else
                {
                    if (count != 0)
                    {
                        pushSink.Write(buffer, offset, count);
                        isFirstChar = false;
                    }
                }
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

        
        public bool GetOutputChunk(out char[] chunkBuffer, out int chunkOffset, out int chunkLength)
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

        
        public void ReportOutput(int readCount)
        {
            InternalDebug.Assert(cache.Length >= readCount);

            cache.ReportRead(readCount);

            if (cache.Length == 0 && endOfFile)
            {
                pullSink.ReportEndOfFile();
            }
        }

        
        private bool FlushCached()
        {
            if (canRestart || cache.Length == 0)
            {
                return false;
            }

            

            char[] outputBuffer;
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

                    InternalDebug.Assert(outputCount != 0);
                }
            }
            else
            {
                pullSink.GetOutputBuffer(out outputBuffer, out outputOffset, out outputSpace);

                outputCount = cache.Read(outputBuffer, outputOffset, outputSpace);

                pullSink.ReportOutput(outputCount);

                if (cache.Length == 0 && endOfFile)
                {
                    pullSink.ReportEndOfFile();
                }
            }

            return true;
        }

        
        private static bool IsUnsafeCharacter(
            char ch, 
            byte[] unsafeAsciiMap, 
            byte unsafeAsciiMask,
            bool hasUnsafeUnicode,
            bool isFirstChar,
            IFallback fallback)
        {
            if (unsafeAsciiMap == null)
            {
                return false;
            }

            var result = ((ch >= unsafeAsciiMap.Length) ? false : (unsafeAsciiMap[(int)ch] & unsafeAsciiMask) != 0);
            result = result || (hasUnsafeUnicode && ch >= 0x7F && fallback.IsUnsafeUnicode(ch, isFirstChar));
            return result;
        }
    }
}

