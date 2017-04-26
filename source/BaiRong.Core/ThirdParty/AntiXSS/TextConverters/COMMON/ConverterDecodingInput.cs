// ***************************************************************
// <copyright file="ConverterDecodingInput.cs" company="Microsoft">
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

    
    internal class ConverterDecodingInput : ConverterInput, IReusable
    {
        private IResultsFeedback resultFeedback;

        private Stream pullSource;
        private ConverterStream pushSource;
        private bool rawEndOfFile;

        private Encoding originalEncoding;
        private Encoding encoding;
        private Decoder decoder;
        private bool encodingChanged;

        private int minDecodeBytes;
        private int minDecodeChars;

        private char[] parseBuffer;
        private int parseStart;
        private int parseEnd;

        private int readFileOffset;
        private byte[] readBuffer;
        private int readCurrent;
        private int readEnd;

        private byte[] pushChunkBuffer;
        private int pushChunkStart;
        private int pushChunkCount;
        private int pushChunkUsed;

        private bool detectEncodingFromByteOrderMark;
        private byte[] preamble;

        private IRestartable restartConsumer;
        private int restartMax;
        private ByteCache restartCache;
        private bool restarting;

        
        public ConverterDecodingInput(
                    Stream source,
                    bool push,
                    Encoding encoding,
                    bool detectEncodingFromByteOrderMark,
                    int maxParseToken,
                    int restartMax,
                    int inputBufferSize,
                    bool testBoundaryConditions,
                    IResultsFeedback resultFeedback,
                    IProgressMonitor progressMonitor) :
            base(progressMonitor)
        {
            this.resultFeedback = resultFeedback;

            this.restartMax = restartMax;

            if (push)
            {
                InternalDebug.Assert(source is ConverterStream);

                pushSource = source as ConverterStream;
            }
            else
            {
                InternalDebug.Assert(source.CanRead);

                pullSource = source;
            }

            this.detectEncodingFromByteOrderMark = detectEncodingFromByteOrderMark;

            minDecodeBytes = testBoundaryConditions ? 1 : 64;

            originalEncoding = encoding;
            SetNewEncoding(encoding);

            
            
            
            InternalDebug.Assert(minDecodeBytes == 1 || minDecodeBytes >= Math.Max(4, preamble.Length));

            maxTokenSize = (maxParseToken == Int32.MaxValue) ? 
                        maxParseToken : 
                        testBoundaryConditions ? 
                            maxParseToken :
                            (maxParseToken + 1023) / 1024 * 1024;

            
            parseBuffer = new char[testBoundaryConditions ? 55 : Math.Min(4096, (long)maxTokenSize + (minDecodeChars + 1))];

            if (pushSource != null)
            {
                readBuffer = new byte[Math.Max(minDecodeBytes * 2, 8)];
            }
            else
            {
                var size = Math.Max(CalculateMaxBytes(parseBuffer.Length), inputBufferSize);

                readBuffer = new byte[size];
            }
        }

        
        private void Reinitialize()
        {
            parseStart = 0;
            parseEnd = 0;

            rawEndOfFile = false;

            SetNewEncoding(originalEncoding);

            encodingChanged = false;

            readFileOffset = 0;
            readCurrent = 0;
            readEnd = 0;

            pushChunkBuffer = null;
            pushChunkStart = 0;
            pushChunkCount = 0;
            pushChunkUsed = 0;

            if (restartCache != null)
            {
                restartCache.Reset();
            }
            restarting = false;

            endOfFile = false;
        }

        
        public Encoding Encoding => encoding;


        public bool EncodingChanged
        {
            
            get { return encodingChanged; }
            set { InternalDebug.Assert(value == false); encodingChanged = false; }
        }

        
        public override void SetRestartConsumer(IRestartable restartConsumer)
        {
            if (restartMax != 0 || restartConsumer == null)
            {
                this.restartConsumer = restartConsumer;
            }
        }

        
        public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
        {
            InternalDebug.Assert((buffer == null && start == 0 && current == 0 && end == 0) ||
                                (buffer == parseBuffer &&
                                start == parseStart &&
                                end == parseEnd &&
                                start <= current && current <= end));

            if (parseBuffer.Length - parseEnd <= minDecodeChars && !EnsureFreeSpace())
            {
                
                
                
                
                
                
                
                

                return true;
            }

            var charactersProduced = 0;

            

            while (!rawEndOfFile || readEnd - readCurrent != 0 || restarting)
            {
                

                if (parseBuffer.Length - parseEnd <= minDecodeChars)
                {
                    
                    
                    
                    

                    InternalDebug.Assert(charactersProduced != 0);

                    break;
                }

                
                

                if (readEnd - readCurrent >= 
                        (readFileOffset == 0 ? Math.Max(4, minDecodeBytes) : minDecodeBytes) || 
                    (rawEndOfFile && !restarting))
                {
                    
                    
                    

                    
                    
                    InternalDebug.Assert(readEnd - readCurrent != 0);

                    charactersProduced += DecodeFromBuffer(readBuffer, ref readCurrent, readEnd, readFileOffset + readCurrent, rawEndOfFile);
                }
                else
                {
                    
                    

                    if (restarting)
                    {
                        InternalDebug.Assert(readEnd - readCurrent == 0);

                        byte[] restartChunk;
                        int restartStart, restartStartSave;
                        int restartEnd;

                        if (!GetRestartChunk(out restartChunk, out restartStart, out restartEnd))
                        {
                            restarting = false;
                            continue;
                        }

                        restartStartSave = restartStart;

                        charactersProduced += DecodeFromBuffer(restartChunk, ref restartStart, restartEnd, readFileOffset, false);

                        readFileOffset += (restartStart - restartStartSave);

                        ReportRestartChunkUsed(restartStart - restartStartSave);
                    }
                    else if (pushSource != null)
                    {
                        

                        if (pushChunkCount == 0)
                        {
                            InternalDebug.Assert(pushChunkUsed == 0);

                            if (!pushSource.GetInputChunk(out pushChunkBuffer, out pushChunkStart, out pushChunkCount, out rawEndOfFile))
                            {
                                
                                

                                InternalDebug.Assert(0 == pushChunkCount);
                                break;
                            }

                            
                            InternalDebug.Assert((pushChunkCount != 0) != rawEndOfFile);
                        }
                        else if (pushChunkCount - pushChunkUsed == 0)
                        {
                            
                            

                            if (restartConsumer != null)
                            {
                                
                                BackupForRestart(pushChunkBuffer, pushChunkStart, pushChunkCount, readFileOffset, false);
                            }

                            pushSource.ReportRead(pushChunkCount);

                            readFileOffset += pushChunkCount;

                            pushChunkCount = 0;
                            pushChunkUsed = 0;

                            
                            
                            

                            
                            
                            InternalDebug.Assert(!pushSource.GetInputChunk(out pushChunkBuffer, out pushChunkStart, out pushChunkCount, out rawEndOfFile) && pushChunkCount == 0 && !rawEndOfFile);

                            break;
                        }

                        
                        

                        if (pushChunkCount - pushChunkUsed < (readFileOffset == 0 ? Math.Max(4, minDecodeBytes) : minDecodeBytes))
                        {
                            

                            if (pushChunkCount - pushChunkUsed != 0)
                            {
                                
                                
                                InternalDebug.Assert(readEnd - readCurrent + (pushChunkCount - pushChunkUsed) <= readBuffer.Length);

                                

                                if (readBuffer.Length - readEnd < (pushChunkCount - pushChunkUsed))
                                {
                                    

                                    if (restartConsumer != null)
                                    {
                                        
                                        BackupForRestart(readBuffer, 0, readCurrent, readFileOffset, false);
                                    }

                                    Buffer.BlockCopy(readBuffer, readCurrent, readBuffer, 0, readEnd - readCurrent);

                                    readFileOffset += readCurrent;

                                    readEnd = readEnd - readCurrent;
                                    readCurrent = 0;
                                }

                                if (pushChunkUsed != 0)
                                {
                                    InternalDebug.Assert(readEnd == 0);

                                    if (restartConsumer != null)
                                    {
                                        

                                        BackupForRestart(pushChunkBuffer, pushChunkStart, pushChunkUsed, readFileOffset + readEnd, false);
                                    }

                                    readFileOffset += pushChunkUsed;
                                }

                                Buffer.BlockCopy(pushChunkBuffer, pushChunkStart + pushChunkUsed, readBuffer, readEnd, pushChunkCount - pushChunkUsed);
                                readEnd += pushChunkCount - pushChunkUsed;

                                

                                pushSource.ReportRead(pushChunkCount);

                                pushChunkCount = 0;
                                pushChunkUsed = 0;

                                if (readEnd - readCurrent < (readFileOffset == 0 ? Math.Max(4, minDecodeBytes) : minDecodeBytes))
                                {
                                    
                                    break;
                                }
                            }

                            charactersProduced += DecodeFromBuffer(readBuffer, ref readCurrent, readEnd, readFileOffset + readCurrent, rawEndOfFile);
                        }
                        else if (readEnd - readCurrent != 0)
                        {
                            

                            if (readFileOffset == 0 && readCurrent == 0)
                            {
                                
                                

                                InternalDebug.Assert(pushChunkUsed == 0);
                                InternalDebug.Assert(readEnd - readCurrent < Math.Max(4, minDecodeBytes));
                                InternalDebug.Assert(pushChunkCount - pushChunkUsed >= Math.Max(4, minDecodeBytes));

                                var bytesToAppend = Math.Max(4, minDecodeBytes) - (readEnd - readCurrent);

                                Buffer.BlockCopy(pushChunkBuffer, pushChunkStart + pushChunkUsed, readBuffer, readEnd, bytesToAppend);
                                readEnd += bytesToAppend;

                                
                                
                                

                                pushSource.ReportRead(bytesToAppend);

                                pushChunkCount -= bytesToAppend;
                                pushChunkStart += bytesToAppend;
                            }

                            

                            charactersProduced += DecodeFromBuffer(readBuffer, ref readCurrent, readEnd, readFileOffset + readCurrent, false);
                        }

                        if (parseBuffer.Length - parseEnd > minDecodeChars && pushChunkCount - pushChunkUsed != 0 && readEnd - readCurrent == 0)
                        {
                            InternalDebug.Assert(!rawEndOfFile);

                            
                            
                            

                            if (readEnd != 0)
                            {
                                
                                

                                if (restartConsumer != null)
                                {
                                    BackupForRestart(readBuffer, 0, readCurrent, readFileOffset, false);
                                }

                                readFileOffset += readCurrent;

                                readEnd = 0;
                                readCurrent = 0;
                            }

                            var chunkUnusedStart = pushChunkStart + pushChunkUsed;

                            charactersProduced += DecodeFromBuffer(pushChunkBuffer, ref chunkUnusedStart, pushChunkStart + pushChunkCount, readFileOffset + pushChunkUsed, false);

                            

                            pushChunkUsed = chunkUnusedStart - pushChunkStart;
                        }
                    }
                    else
                    {
                        

                        
                        

                        if (readBuffer.Length - readEnd < minDecodeBytes)
                        {
                            

                            InternalDebug.Assert(readEnd - readCurrent < (readFileOffset == 0 ? Math.Max(4, minDecodeBytes) : minDecodeBytes));

                            if (restartConsumer != null)
                            {
                                
                                BackupForRestart(readBuffer, 0, readCurrent, readFileOffset, false);
                            }

                            Buffer.BlockCopy(readBuffer, readCurrent, readBuffer, 0, readEnd - readCurrent);

                            readFileOffset += readCurrent;

                            readEnd = readEnd - readCurrent;
                            readCurrent = 0;
                        }

                        var readCount = pullSource.Read(readBuffer, readEnd, readBuffer.Length - readEnd);

                        if (readCount == 0)
                        {
                            rawEndOfFile = true;
                        }
                        else
                        {
                            readEnd += readCount;
                            if (progressMonitor != null)
                            {
                                progressMonitor.ReportProgress();
                            }
                        }

                        charactersProduced += DecodeFromBuffer(readBuffer, ref readCurrent, readEnd, readFileOffset + readCurrent, rawEndOfFile);
                    }
                }
            }

            if (rawEndOfFile && readEnd - readCurrent == 0)
            {
                endOfFile = true;
            }

            if (buffer != parseBuffer)
            {
                buffer = parseBuffer;
            }

            if (start != parseStart)
            {
                current = parseStart + (current - start);
                start = parseStart;
            }

            end = parseEnd;

            return charactersProduced != 0 || endOfFile || encodingChanged;
        }

        
        public override void ReportProcessed(int processedSize)
        {
            InternalDebug.Assert(processedSize >= 0);
            InternalDebug.Assert(parseStart + processedSize <= parseEnd);

            parseStart += processedSize;
        }

        
        
        
        
        
        public override int RemoveGap(int gapBegin, int gapEnd)
        {
            
            
            
            InternalDebug.Assert(gapEnd == parseEnd);

            parseEnd = gapBegin;
            parseBuffer[gapBegin] = '\0';
            return gapBegin;
        }

        
        public bool RestartWithNewEncoding(Encoding newEncoding)
        {
            if (encoding.CodePage == newEncoding.CodePage)
            {
                

                if (restartConsumer != null)
                {
                    restartConsumer.DisableRestart();
                    restartConsumer = null;

                    if (restartCache != null)
                    {
                        restartCache.Reset();
                        restartCache = null;
                    }
                }

                return false;
            }

            if (restartConsumer == null || !restartConsumer.CanRestart())
            {
                return false;
            }

            restartConsumer.Restart();

            
            

            SetNewEncoding(newEncoding);

            encodingChanged = true;

            
            
            
            

            if (readEnd != 0 && readFileOffset != 0)
            {
                

                BackupForRestart(readBuffer, 0, readEnd, readFileOffset, true);

                readEnd = 0;

                readFileOffset = 0;
            }
            else
            {
                
            }

            readCurrent = 0;
            pushChunkUsed = 0;

            
            
            restartConsumer = null;

            
            parseStart = parseEnd = 0;

            
            
            
            restarting = restartCache != null && restartCache.Length != 0;

            return true;
        }

        
        private void SetNewEncoding(Encoding newEncoding)
        {
            encoding = newEncoding;
            decoder = encoding.GetDecoder();

            
            preamble = encoding.GetPreamble();
            InternalDebug.Assert(preamble != null);

            minDecodeChars = GetMaxCharCount(minDecodeBytes);

            if (resultFeedback != null)
            {
                resultFeedback.Set(ConfigParameter.InputEncoding, newEncoding);
            }
        }


        
        protected override void Dispose()
        {
            if (restartCache != null && restartCache is IDisposable)
            {
                ((IDisposable)restartCache).Dispose();
            }

            restartCache = null;
            pullSource = null;
            pushSource = null;
            parseBuffer = null;
            readBuffer = null;
            pushChunkBuffer = null;
            preamble = null;
            restartConsumer = null;

            base.Dispose();
        }

        
        private int DecodeFromBuffer(byte[] buffer, ref int start, int end, int fileOffset, bool flush)
        {
            
            
            

            var preambleLength = 0;

            if (fileOffset == 0)
            {
                
                
                

                if (detectEncodingFromByteOrderMark)
                {
                    
                    DetectEncoding(buffer, start, end);
                }

                
                InternalDebug.Assert(preamble != null);

                

                if (preamble.Length != 0 && end - start >= preamble.Length)
                {
                    int i;

                    for (i = 0; i < preamble.Length; i++)
                    {
                        if (preamble[i] != buffer[start + i])
                        {
                            break;
                        }
                    }

                    if (i == preamble.Length)
                    {
                        
                        start += preamble.Length;
                        preambleLength = preamble.Length;

                        if (restartConsumer != null)
                        {
                            
                            

                            restartConsumer.DisableRestart();
                            restartConsumer = null;
                        }
                    }
                }

                
                encodingChanged = true;

                
                preamble = null;
            }

            var bytesToDecode = end - start;

            if (GetMaxCharCount(bytesToDecode) >= parseBuffer.Length - parseEnd)
            {
                bytesToDecode = CalculateMaxBytes(parseBuffer.Length - parseEnd - 1);

                InternalDebug.Assert(bytesToDecode < end - start);
            }

            var charsDecoded = decoder.GetChars(buffer, start, bytesToDecode, parseBuffer, parseEnd);

            InternalDebug.Assert(charsDecoded <= parseBuffer.Length - parseEnd - 1);

            parseEnd += charsDecoded;

            parseBuffer[parseEnd] = '\0';     
                                                        
                                                        
            start += bytesToDecode;

            return bytesToDecode + preambleLength;
        }

        
        private bool EnsureFreeSpace()
        {
            
            InternalDebug.Assert(parseBuffer.Length - parseEnd <= minDecodeChars);

            

            if (parseBuffer.Length - (parseEnd - parseStart) < (minDecodeChars + 1) ||
                (parseStart < minDecodeChars &&
                (long)parseBuffer.Length < (long)maxTokenSize + (minDecodeChars + 1)))
            {
                
                
                
                
                

                

                if ((long)parseBuffer.Length >= (long)maxTokenSize + (minDecodeChars + 1))
                {
                    
                    return false;
                }

                

                long newSize = parseBuffer.Length * 2;

                if (newSize > (long)maxTokenSize + (minDecodeChars + 1))
                {
                    newSize = (long)maxTokenSize + (minDecodeChars + 1);
                }

                if (newSize > (long)Int32.MaxValue)
                {
                    
                    
                    
                    
                    
                    
                    
                    

                    newSize = (long)Int32.MaxValue;
                }

                if (newSize - (parseEnd - parseStart) < (minDecodeChars + 1))
                {
                    
                    
                    

                    return false;
                }

                char[] newBuffer;

                try
                {
                    newBuffer = new char[(int)newSize];
                }
                catch (OutOfMemoryException e)
                {
                    throw new TextConvertersException(Strings.TagTooLong, e);
                }

                
                Buffer.BlockCopy(parseBuffer, parseStart * 2, newBuffer, 0, (parseEnd - parseStart + 1) * 2);

                
                parseBuffer = newBuffer;

                parseEnd = (parseEnd - parseStart);
                parseStart = 0;
            }
            else
            {
                

                
                Buffer.BlockCopy(parseBuffer, parseStart * 2, parseBuffer, 0, (parseEnd - parseStart + 1) * 2);

                parseEnd = (parseEnd - parseStart);
                parseStart = 0;
            }

            return true;
        }        

        
        
        private int GetMaxCharCount(int byteCount)
        {
            if (encoding.CodePage == 65001)
            {
                InternalDebug.Assert(encoding.GetMaxCharCount(byteCount) == byteCount + 1, "when this assert fires, it means that we are back on Everett?");
                return byteCount + 1;
            }
            else if (encoding.CodePage == 54936)
            {
                InternalDebug.Assert(encoding.GetMaxCharCount(byteCount) == byteCount + 3, "when this assert fires, it means that we are back on Everett?");
                return byteCount + 3;
            }

            return encoding.GetMaxCharCount(byteCount);
        }

        
        private int CalculateMaxBytes(int charCount)
        {
            
            
            
            

            
            
            

            if (charCount == GetMaxCharCount(charCount))
            {
                
                return charCount;
            }

            if (charCount == GetMaxCharCount(charCount - 1))
            {
                
                return charCount - 1;
            }

            if (charCount == GetMaxCharCount(charCount - 3))
            {
                
                return charCount - 3;
            }

            

            var byteCountN = charCount - 4;
            var charCountN = GetMaxCharCount(byteCountN);

            
            
            

            var byteCount = (int)((float)byteCountN * (float)charCount / (float)charCountN);

            
            

            while (GetMaxCharCount(byteCount) < charCount)
            {
                byteCount ++;
            }

            do
            {
                byteCount --;
            }
            while (GetMaxCharCount(byteCount) > charCount);

            return byteCount;
        }

        
        private void DetectEncoding(byte[] buffer, int start, int end)
        {
            

            if (end - start < 2)
            {
                return;
            }

            Encoding newEncoding = null;

            if (buffer[start] == 0xFE && buffer[start + 1] == 0xFF)
            {
                
                newEncoding = Encoding.BigEndianUnicode;
            }
            else if (buffer[start] == 0xFF && buffer[start + 1] == 0xFE)
            {
                

                if (end - start >= 4 && 
                    buffer[start + 2] == 0 && 
                    buffer[start + 3] == 0) 
                {
                    newEncoding = Encoding.UTF32;
                }
                else 
                {
                    newEncoding = Encoding.Unicode;
                }
            }
            else if (end - start >= 3 && 
                    buffer[start] == 0xEF && 
                    buffer[start + 1] == 0xBB && 
                    buffer[start + 2] == 0xBF) 
            {
                
                newEncoding = Encoding.UTF8;
            }
            else if (end - start >= 4 && 
                    buffer[start] == 0 && 
                    buffer[start + 1] == 0 &&
                    buffer[start + 2] == 0xFE && 
                    buffer[start + 3] == 0xFF) 
            {
                
                newEncoding = new UTF32Encoding(true, true);
            }

            
            
            

            if (newEncoding != null)
            {
                encoding = newEncoding;
                decoder = encoding.GetDecoder();

                
                preamble = encoding.GetPreamble();

                minDecodeChars = GetMaxCharCount(minDecodeBytes);

                
                
                

                if (restartConsumer != null)
                {
                    
                    

                    restartConsumer.DisableRestart();
                    restartConsumer = null;
                }
            }
        }

        
        private void BackupForRestart(byte[] buffer, int offset, int count, int fileOffset, bool force)
        {
            InternalDebug.Assert(restartConsumer != null);

            if (!force && fileOffset > restartMax)
            {
                
                

                restartConsumer.DisableRestart();
                restartConsumer = null;

                preamble = null;
                return;
            }

            if (restartCache == null)
            {
                restartCache = new ByteCache();
            }

            byte[] cacheBuffer;
            int cacheOffset;

            restartCache.GetBuffer(count, out cacheBuffer, out cacheOffset);

            Buffer.BlockCopy(buffer, offset, cacheBuffer, cacheOffset, count);

            restartCache.Commit(count);
        }

        
        private bool GetRestartChunk(out byte[] restartChunk, out int restartStart, out int restartEnd)
        {
            InternalDebug.Assert(restartConsumer == null && restarting);

            if (restartCache.Length == 0)
            {
                restartChunk = null;
                restartStart = 0;
                restartEnd = 0;

                return false;
            }

            int outputCount;

            restartCache.GetData(out restartChunk, out restartStart, out outputCount);

            restartEnd = restartStart + outputCount;

            return true;
        }

        
        private void ReportRestartChunkUsed(int count)
        {
            InternalDebug.Assert(restartConsumer == null && restarting);
            InternalDebug.Assert(restartCache.Length >= count);

            restartCache.ReportRead(count);
        }

        
        void IReusable.Initialize(object newSourceOrDestination)
        {
            if (pullSource != null)
            {
                

                if (newSourceOrDestination != null)
                {
                    var newSource = newSourceOrDestination as Stream;

                    if (newSource == null || !newSource.CanRead)
                    {
                        throw new InvalidOperationException("cannot reinitialize this converter - new input should be a readable Stream object");
                    }

                    pullSource = newSource;
                }
            }

            Reinitialize();
        }
    }
}

