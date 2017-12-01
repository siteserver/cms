// ***************************************************************
// <copyright file="ConverterUnicodeInput.cs" company="Microsoft">
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

    
    internal class ConverterUnicodeInput : ConverterInput, IReusable, IDisposable
    {
        private TextReader pullSource;
        private ConverterWriter pushSource;

        private char[] parseBuffer;
        private int parseStart;
        private int parseEnd;

        private char[] pushChunkBuffer;
        private int pushChunkStart;
        private int pushChunkCount;
        private int pushChunkUsed;

        
        public ConverterUnicodeInput(
            object source,
            bool push,
            int maxParseToken,
            bool testBoundaryConditions,
            IProgressMonitor progressMonitor) :
            base(progressMonitor)
        {
            if (push)
            {
                InternalDebug.Assert(source is ConverterWriter);

                pushSource = source as ConverterWriter;
            }
            else
            {
                InternalDebug.Assert(source is TextReader);

                pullSource = source as TextReader;
            }

            maxTokenSize = maxParseToken;

            parseBuffer = new char[testBoundaryConditions ? 123 : 4096];

            if (pushSource != null)
            {
                pushSource.SetSink(this);
            }
        }

        
        private void Reinitialize()
        {
            parseStart = parseEnd = 0;
            pushChunkStart = 0;
            pushChunkCount = 0;
            pushChunkUsed = 0;
            pushChunkBuffer = null;
            endOfFile = false;
        }

        
        void IReusable.Initialize(object newSourceOrDestination)
        {
            if (pullSource != null)
            {
                

                if (newSourceOrDestination != null)
                {
                    var newSource = newSourceOrDestination as TextReader;

                    if (newSource == null)
                    {
                        throw new InvalidOperationException("cannot reinitialize this converter - new input should be a TextReader object");
                    }

                    pullSource = newSource;
                }
            }

            Reinitialize();
        }

        
        public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
        {
            InternalDebug.Assert((buffer == null && start == 0 && current == 0 && end == 0) ||
                                (buffer == parseBuffer &&
                                start == parseStart &&
                                end <= parseEnd &&
                                start <= current));

            var charactersProduced = parseEnd - end;   
                                                            
                                                            

            if (parseBuffer.Length - parseEnd <= 1 && !EnsureFreeSpace() && charactersProduced == 0)
            {
                
                
                
                
                
                
                
                

                return true;
            }

            

            while (!endOfFile)
            {
                

                if (parseBuffer.Length - parseEnd <= 1)
                {
                    
                    
                    
                    

                    InternalDebug.Assert(charactersProduced != 0);

                    break;
                }

                
                

                if (pushSource != null)
                {
                    

                    if (pushChunkCount == 0)
                    {
                        InternalDebug.Assert(pushChunkUsed == 0);

                        if (!pushSource.GetInputChunk(out pushChunkBuffer, out pushChunkStart, out pushChunkCount, out endOfFile))
                        {
                            
                            

                            InternalDebug.Assert(0 == pushChunkCount);
                            break;
                        }

                        
                        InternalDebug.Assert((pushChunkCount != 0) != endOfFile);
                    }

                    InternalDebug.Assert(pushChunkCount - pushChunkUsed != 0 || endOfFile);

                    if (pushChunkCount - pushChunkUsed != 0)
                    {
                        var charactersToAppend = Math.Min(pushChunkCount - pushChunkUsed, parseBuffer.Length - parseEnd - 1);

                        Buffer.BlockCopy(pushChunkBuffer, (pushChunkStart + pushChunkUsed) * 2, parseBuffer, parseEnd * 2, charactersToAppend * 2);

                        pushChunkUsed += charactersToAppend;

                        parseEnd += charactersToAppend;

                        parseBuffer[parseEnd] = '\0';     

                        charactersProduced += charactersToAppend;

                        if (pushChunkCount - pushChunkUsed == 0)
                        {
                            
                            
                            

                            pushSource.ReportRead(pushChunkCount);

                            pushChunkStart = 0;
                            pushChunkCount = 0;
                            pushChunkUsed = 0;
                            pushChunkBuffer = null;
                        }
                    }
                }
                else
                {
                    

                    var readCharactersCount = pullSource.Read(parseBuffer, parseEnd, parseBuffer.Length - parseEnd - 1);

                    if (readCharactersCount == 0)
                    {
                        endOfFile = true;
                    }
                    else
                    {
                        parseEnd += readCharactersCount;

                        parseBuffer[parseEnd] = '\0';     

                        charactersProduced += readCharactersCount;
                    }

                    if (progressMonitor != null)
                    {
                        progressMonitor.ReportProgress();
                    }
                }
            }

            buffer = parseBuffer;

            if (start != parseStart)
            {
                current = parseStart + (current - start);
                start = parseStart;
            }

            end = parseEnd;

            return charactersProduced != 0 || endOfFile;
        }

        
        public override void ReportProcessed(int processedSize)
        {
            InternalDebug.Assert(processedSize >= 0);
            InternalDebug.Assert(parseStart + processedSize <= parseEnd);

            parseStart += processedSize;
        }

        
        
        public override int RemoveGap(int gapBegin, int gapEnd)
        {
            
            
            
            InternalDebug.Assert(gapEnd <= parseEnd);

            if (gapEnd == parseEnd)
            {
                
                parseEnd = gapBegin;
                parseBuffer[gapBegin] = '\0';
                return gapBegin;
            }

            
            
            

            Buffer.BlockCopy(parseBuffer, gapEnd, parseBuffer, gapBegin, parseEnd - gapEnd);
            parseEnd = gapBegin + (parseEnd - gapEnd);
            parseBuffer[parseEnd] = '\0';
            return parseEnd;
        }


        
        public void GetInputBuffer(out char[] inputBuffer, out int inputOffset, out int inputCount, out int parseCount)
        {
            InternalDebug.Assert(parseBuffer.Length - parseEnd >= 1);

            inputBuffer = parseBuffer;
            inputOffset = parseEnd;
            inputCount = parseBuffer.Length - parseEnd - 1;
            parseCount = parseEnd - parseStart;
        }

        
        public void Commit(int inputCount)
        {
            parseEnd += inputCount;
            parseBuffer[parseEnd] = '\0';
        }

        
        protected override void Dispose()
        {
            pullSource = null;
            pushSource = null;
            parseBuffer = null;
            pushChunkBuffer = null;

            base.Dispose();
        }

        
        private bool EnsureFreeSpace()
        {
            
            InternalDebug.Assert(parseBuffer.Length - parseEnd <= 1);

            

            if (parseBuffer.Length - (parseEnd - parseStart) <= 1 ||
                (parseStart < 1 && 
                (long)parseBuffer.Length < (long)maxTokenSize + 1))
            {
                
                
                
                
                

                

                if ((long)parseBuffer.Length >= (long)maxTokenSize + 1)
                {
                    
                    return false;
                }

                

                long newSize = parseBuffer.Length * 2;

                if (newSize > (long)maxTokenSize + 1)
                {
                    newSize = (long)maxTokenSize + 1;
                }

                if (newSize > (long)Int32.MaxValue)
                {
                    
                    
                    
                    
                    
                    
                    
                    

                    newSize = (long)Int32.MaxValue;
                }

                var newBuffer = new char[(int)newSize];

                
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
    }
}

