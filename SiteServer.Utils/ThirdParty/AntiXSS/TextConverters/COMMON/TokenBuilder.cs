// ***************************************************************
// <copyright file="TokenBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal
{
    using System;
    using System.Diagnostics;
    using Data.Internal;

    

    internal abstract class TokenBuilder
    {
        protected const byte BuildStateInitialized = 0;
        protected const byte BuildStateEnded = 5;
        protected const byte FirstStarted = 10;
        protected const byte BuildStateText = FirstStarted;

        protected byte state;

        protected Token token;

        protected int maxRuns;

        protected int tailOffset;

        protected bool tokenValid;              

#if DEBUG
        private int preparedForNumRuns;
#endif

        

        public TokenBuilder(Token token, char[] buffer, int maxRuns, bool testBoundaryConditions)
        {
            var initialRuns = 64;

            if (!testBoundaryConditions)
            {
                this.maxRuns = maxRuns;
            }
            else
            {
                this.maxRuns = 55;
                initialRuns = 6 + 1;
            }

            

            this.token = token;

            this.token.buffer = buffer;
            this.token.runList = new Token.RunEntry[initialRuns];

            
        }

        

        public Token Token => token;


        public bool IsStarted => state != BuildStateInitialized;


        public bool Valid => tokenValid;

#if PRIVATEBUILD_UNUSED
        

        public char[] Buffer
        {
            get { return this.token.buffer; }
            set { this.token.buffer = value; }
        }
#endif

#if PRIVATEBUILD_UNUSED
        

        public int BaseOffset
        {
            get { return this.token.whole.headOffset; }
            set { this.token.whole.headOffset = value; }
        }
#endif

        

        public int TotalLength => tailOffset - token.whole.headOffset;


        public void BufferChanged(char[] newBuffer, int newBase)
        {
            if (newBuffer != token.buffer || newBase != token.whole.headOffset)
            {
                

                
                token.buffer = newBuffer;

                if (newBase != token.whole.headOffset)
                {
                    

                    var delta = newBase - token.whole.headOffset;

                    Rebase(delta);
                }
            }
        }

        

        public virtual void Reset()
        {
            InternalDebug.Assert(state != BuildStateInitialized || (token.whole.headOffset == 0 && tailOffset == 0));

            if (state > BuildStateInitialized)
            {
                token.Reset();

                tailOffset = 0;

                tokenValid = false;

                state = BuildStateInitialized;
            }
        }

        

        public TokenId MakeEmptyToken(TokenId tokenId)
        {
            InternalDebug.Assert(state == BuildStateInitialized && token.IsEmpty && !tokenValid);

            token.tokenId = tokenId;

            state = BuildStateEnded;
            tokenValid = true;

            return tokenId;
        }

        

        public TokenId MakeEmptyToken(TokenId tokenId, int argument)
        {
            InternalDebug.Assert(state == BuildStateInitialized && token.IsEmpty && !tokenValid);

            token.tokenId = tokenId;
            token.argument = argument;

            state = BuildStateEnded;
            tokenValid = true;

            return tokenId;
        }

        

        public void StartText(int baseOffset)
        {
            InternalDebug.Assert(state == BuildStateInitialized && token.IsEmpty && !tokenValid);

            token.tokenId = TokenId.Text;

            state = BuildStateText;

            token.whole.headOffset = baseOffset;
            tailOffset = baseOffset;
        }

        

        public void EndText()
        {
            InternalDebug.Assert(state == BuildStateText);

            state = BuildStateEnded;

            tokenValid = true;

            token.wholePosition.Rewind(token.whole);

            
            AddSentinelRun();
        }

        

        public void SkipRunIfNecessary(int start, uint skippedRunKind)
        {
#if DEBUG
            InternalDebug.Assert(preparedForNumRuns > 0);
#endif
            
            if (start != tailOffset)
            {
                AddInvalidRun(start, skippedRunKind);
            }
        }

        

        public bool PrepareToAddMoreRuns(int numRuns, int start, uint skippedRunKind)
        {
#if DEBUG
            preparedForNumRuns = numRuns;
#endif
            
            return (start == tailOffset && token.whole.tail + numRuns < token.runList.Length) || 
                            SlowPrepareToAddMoreRuns(numRuns, start, skippedRunKind);
        }

        public bool SlowPrepareToAddMoreRuns(int numRuns, int start, uint skippedRunKind)
        {
            InternalDebug.Assert(start >= tailOffset);

            if (start != tailOffset)
            {
                numRuns ++;
            }

            if (token.whole.tail + numRuns < token.runList.Length || ExpandRunsArray(numRuns))
            {
                if (start != tailOffset)
                {
#if DEBUG
                    preparedForNumRuns ++;
#endif
                    AddInvalidRun(start, skippedRunKind);
                }

                return true;
            }

            return false;
        }

        

        public bool PrepareToAddMoreRuns(int numRuns)
        {
#if DEBUG
            preparedForNumRuns = numRuns;
#endif
            return token.whole.tail + numRuns < token.runList.Length || ExpandRunsArray(numRuns);
        }

        

        [Conditional("DEBUG")]
        public void AssertPreparedToAddMoreRuns(int numRuns)
        {
#if DEBUG
            InternalDebug.Assert(numRuns <= preparedForNumRuns);
#endif
        }

        [Conditional("DEBUG")]
        public void AssertCanAddMoreRuns(int numRuns)
        {
            InternalDebug.Assert(token.whole.tail + numRuns <= token.runList.Length);
        }

        [Conditional("DEBUG")]
        public void AssertCurrentRunPosition(int position)
        {
            InternalDebug.Assert(position == tailOffset);
        }

        [Conditional("DEBUG")]
        public void DebugPrepareToAddMoreRuns(int numRuns)
        {
            InternalDebug.Assert(token.whole.head == 0 && token.whole.tail == 0);
            InternalDebug.Assert(token.whole.tail + numRuns + 1 <= token.runList.Length);
#if DEBUG
            preparedForNumRuns = numRuns;
#endif
        }

        

        public void AddTextRun(RunTextType textType, int start, int end)
        {
            InternalDebug.Assert(start == tailOffset);
            AddRun(RunType.Normal, textType, (uint)RunKind.Text, start, end, 0);
        }

        

        public void AddLiteralTextRun(RunTextType textType, int start, int end, int literal)
        {
            InternalDebug.Assert(start == tailOffset);
            AddRun(RunType.Literal, textType, (uint)RunKind.Text, start, end, literal);
        }

        

        public void AddSpecialRun(RunKind kind, int startEnd, int value)
        {
            InternalDebug.Assert(startEnd == tailOffset);
            AddRun(RunType.Special, RunTextType.Unknown, (uint)kind, tailOffset, startEnd, value);
        }

        

        internal void AddRun(RunType type, RunTextType textType, uint kind, int start, int end, int value)
        {
#if DEBUG
            InternalDebug.Assert(preparedForNumRuns > 0);
            preparedForNumRuns --;
#endif
            InternalDebug.Assert(state >= FirstStarted);
            InternalDebug.Assert(end >= start);
            InternalDebug.Assert(token.whole.head != token.whole.tail || tailOffset == token.whole.headOffset);
            InternalDebug.Assert(token.whole.tail + 1 < token.runList.Length);
            InternalDebug.Assert(start == tailOffset);

            token.runList[token.whole.tail++].Initialize(type, textType, kind, end - start, value);
            tailOffset = end;
        }

        

        internal void AddInvalidRun(int offset, uint kind)
        {
#if DEBUG
            InternalDebug.Assert(preparedForNumRuns > 0);
            preparedForNumRuns --;
#endif
            InternalDebug.Assert(offset > tailOffset);
            InternalDebug.Assert(token.whole.tail + 1 < token.runList.Length);

            token.runList[token.whole.tail++].Initialize(RunType.Invalid, RunTextType.Unknown, kind, offset - tailOffset, 0);
            tailOffset = offset;
        }

        

        internal void AddNullRun(uint kind)
        {
#if DEBUG
            InternalDebug.Assert(preparedForNumRuns > 0);
            preparedForNumRuns --;
#endif
            InternalDebug.Assert(token.whole.tail + 1 < token.runList.Length);

            token.runList[token.whole.tail++].Initialize(RunType.Invalid, RunTextType.Unknown, kind, 0, 0);
        }

        

        internal void AddSentinelRun()
        {
            
            InternalDebug.Assert(token.whole.tail + 1 <= token.runList.Length);

            token.runList[token.whole.tail].InitializeSentinel();
        }

        

        protected virtual void Rebase(int deltaOffset)
        {
            token.whole.headOffset += deltaOffset;
            token.wholePosition.runOffset += deltaOffset;

            tailOffset += deltaOffset;
        }

        

        private bool ExpandRunsArray(int numRuns)
        {
            int newSize;

            newSize = Math.Min(maxRuns, Math.Max(token.runList.Length * 2, token.whole.tail + numRuns + 1));

            if (newSize - token.whole.tail < numRuns + 1)
            {
                return false;
            }

            var newRuns = new Token.RunEntry[newSize];
            Array.Copy(token.runList, 0, newRuns, 0, token.whole.tail);
            token.runList = newRuns;

            return true;
        }
    }
}

