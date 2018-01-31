// ***************************************************************
// <copyright file="Token.cs" company="Microsoft">
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
    using Strings = CtsResources.TextConvertersStrings;

    

    internal enum TokenId : byte
    {
        None = 0,                
                                 
        EndOfFile,               
        Text,
        EncodingChange ,
    }

    

    internal enum RunType : uint
    {
        Invalid = 0,             
                                 

        Special = (1u << 30),    
                                 
                                 
                                 

        Normal = (2u << 30),     
                                 
                                 

        Literal = (3u << 30),    
                                 

        Mask = 0xC0000000u,      
    }

    

    internal enum RunTextType : uint
    {
        Unknown = 0,

        Space = (1 << 27),
        NewLine = (2 << 27),
        Tabulation = (3 << 27),
        UnusualWhitespace = (4 << 27),
        LastWhitespace = UnusualWhitespace,

        Nbsp = (5 << 27),
        NonSpace = (6 << 27),
        LastText = NonSpace,

        Last = LastText,

        Mask = 0x38000000u,
    }

    

    internal enum RunKind : uint
    {
        Invalid = 0,
        Text = (1u << 26),

        StartLexicalUnitFlag = (1u << 31),

        MajorKindMask = (0x1Fu << 26),
        MajorKindMaskWithStartLexicalUnitFlag = (0x3Fu << 26),
        MinorKindMask = (3u << 24),
        KindMask = (0xFFu << 24),
    }

    

    internal struct TokenRun
    {
        private Token token;
#if DEBUG
        private int index;
#endif

        internal TokenRun(Token token)
        {
            this.token = token;
#if DEBUG
            index = this.token.wholePosition.run;
#endif
        }

        
#if false
        public int Index
        {
            get { this.AssertCurrent(); return this.token.wholePosition.run; }
        }

        public bool IsInvalid
        {
            get { this.AssertCurrent(); return this.token.runList[this.token.wholePosition.run].Type == RunType.Invalid; }
        }

#endif
        public RunType Type
        {
            get { AssertCurrent(); return token.runList[token.wholePosition.run].Type; }
        }

        public bool IsTextRun
        {
            get { AssertCurrent(); return token.runList[token.wholePosition.run].Type >= RunType.Normal; }
        }

        public bool IsSpecial
        {
            get { AssertCurrent(); return token.runList[token.wholePosition.run].Type == RunType.Special; }
        }

        public bool IsNormal
        {
            get { AssertCurrent(); return token.runList[token.wholePosition.run].Type == RunType.Normal; }
        }

        public bool IsLiteral
        {
            get { AssertCurrent(); return token.runList[token.wholePosition.run].Type == RunType.Literal; }
        }

        public RunTextType TextType
        {
            get { AssertCurrent(); return token.runList[token.wholePosition.run].TextType; }
        }

        public char[] RawBuffer
        {
            get { AssertCurrent(); return token.buffer; }
        }

        public int RawOffset
        {
            get { AssertCurrent(); return token.wholePosition.runOffset; }
        }

        public int RawLength
        {
            get { AssertCurrent(); return token.runList[token.wholePosition.run].Length; }
        }

        public uint Kind
        {
            get { AssertCurrent(); return token.runList[token.wholePosition.run].Kind; }
        }

        public int Literal
        {
            get { AssertCurrent(); InternalDebug.Assert(IsLiteral); return token.runList[token.wholePosition.run].Value; }
        }

        public int Length
        {
            get
            {
                AssertCurrent();
                return IsNormal ? RawLength : IsLiteral ? Token.LiteralLength(Literal) : 0;
            }
        }

        public int Value
        {
            get { AssertCurrent(); return token.runList[token.wholePosition.run].Value; }
        }

        public char FirstChar
        {
            get { AssertCurrent(); return IsLiteral ? Token.LiteralFirstChar(Literal) : RawBuffer[RawOffset]; }
        }

        public char LastChar
        {
            get { AssertCurrent(); return IsLiteral ? Token.LiteralLastChar(Literal) : RawBuffer[RawOffset + RawLength - 1]; }
        }

        public bool IsAnyWhitespace => TextType <= RunTextType.LastWhitespace;


        public int ReadLiteral(char[] buffer)
        {
            AssertCurrent();
            InternalDebug.Assert(IsLiteral);
            InternalDebug.Assert(buffer.Length >= 2);

            var literalValue = token.runList[token.wholePosition.run].Value;
            var literalLength = Token.LiteralLength(literalValue);

            if (literalLength == 1)
            {
                buffer[0] = (char) literalValue;
                return 1;
            }

            InternalDebug.Assert(literalLength == 2);

            buffer[0] = Token.LiteralFirstChar(literalValue);
            buffer[1] = Token.LiteralLastChar(literalValue);
            return 2;
        }

        

        public string GetString(int maxSize)
        {
            Token.Fragment fragment;
            var run = token.wholePosition.run;
            var runList = token.runList;

            switch (runList[run].Type)
            {
                case RunType.Normal:

                        return new string(token.buffer, token.wholePosition.runOffset, Math.Min(maxSize, runList[run].Length));

                case RunType.Literal:

                        if (Length == 1)
                        {
                            return FirstChar.ToString();
                        }
                        
                        fragment = new Token.Fragment();

                        fragment.Initialize(run, token.wholePosition.runOffset);
                        fragment.tail = fragment.head + 1;

                        return token.GetString(ref fragment, maxSize);
            }

            return "";
        }

        

        [System.Diagnostics.Conditional("DEBUG")]
        private void AssertCurrent()
        {
#if DEBUG
            InternalDebug.Assert(index == token.wholePosition.run);
#endif
        }
    }

    internal enum CollapseWhitespaceState
    {
        NonSpace,
        Whitespace,
        NewLine
    }

    

    internal class Token
    {
        protected internal TokenId tokenId;
        protected internal int argument;                         

        protected internal char[] buffer;                        
        protected internal RunEntry[] runList;                   

        protected internal Fragment whole;                       
        protected internal FragmentPosition wholePosition;       

        
        private LowerCaseCompareSink compareSink;
        private LowerCaseSubstringSearchSink searchSink;
        private StringBuildSink stringBuildSink;

        

        public Token()
        {
            Reset();
        }

        

        public TokenId TokenId
        {
            get { return tokenId; }
            set { tokenId = value; }
        }

        public int Argument => argument;

        public bool IsEmpty => whole.tail == whole.head;

        public RunEnumerator Runs => new RunEnumerator(this);

        public TextReader Text => new TextReader(this);


        public bool IsWhitespaceOnly => IsWhitespaceOnlyImp(ref whole);

        protected internal bool IsWhitespaceOnlyImp(ref Fragment fragment)
        {
            var result = true;
            var run = fragment.head;

            while (run != fragment.tail)
            {
                if (runList[run].Type >= RunType.Normal)
                {
                    if (runList[run].TextType > RunTextType.LastWhitespace)
                    {
                        result = false;
                        break;
                    }
                }

                run ++;
            }

            return result;
        }

        internal static int LiteralLength(int literal)
        {
            return (literal > 0xFFFF) ? 2 : 1;
        }

        internal static char LiteralFirstChar(int literal)
        {
            return (literal > 0xFFFF) ? ParseSupport.HighSurrogateCharFromUcs4(literal) : (char)literal;
        }

        internal static char LiteralLastChar(int literal)
        {
            return (literal > 0xFFFF) ? ParseSupport.LowSurrogateCharFromUcs4(literal) : (char)literal;
        }

        
        
        

        protected internal int Read(ref Fragment fragment, ref FragmentPosition position, char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(count != 0);

            var startOffset = offset;
            var run = position.run;

            if (run == fragment.head - 1)
            {
                run = position.run = fragment.head;
            }

            if (run != fragment.tail)
            {
                var runOffset = position.runOffset;
                var runDeltaOffset = position.runDeltaOffset;

                do
                {
                    var runEntry = runList[run];

                    InternalDebug.Assert(count != 0);

                    if (runEntry.Type == RunType.Literal)
                    {
                        var literalLength = LiteralLength(runEntry.Value);

                        if (runDeltaOffset != literalLength)
                        {
                            if (literalLength == 1)
                            {
                                InternalDebug.Assert(runDeltaOffset == 0);

                                buffer[offset++] = (char) runEntry.Value;
                                count --;
                            }
                            else
                            {
                                InternalDebug.Assert(literalLength == 2);

                                if (runDeltaOffset != 0)
                                {
                                    InternalDebug.Assert(runDeltaOffset == 1);

                                    buffer[offset++] = LiteralLastChar(runEntry.Value);
                                    count --;
                                }
                                else
                                {
                                    buffer[offset++] = LiteralFirstChar(runEntry.Value);
                                    count --;

                                    if (count == 0)
                                    {
                                        runDeltaOffset = 1;
                                        break;
                                    }

                                    buffer[offset++] = LiteralLastChar(runEntry.Value);
                                    count --;
                                }
                            }
                        }
                    }
                    else if (runEntry.Type == RunType.Normal)
                    {
                        InternalDebug.Assert(runDeltaOffset >= 0 && runDeltaOffset < runEntry.Length);

                        var copyCount = Math.Min(count, runEntry.Length - runDeltaOffset);

                        InternalDebug.Assert(copyCount != 0);
                        
                        {
                            Buffer.BlockCopy(this.buffer, (runOffset + runDeltaOffset) * 2, buffer, offset * 2, copyCount * 2);

                            offset += copyCount;
                            count -= copyCount;

                            if (runDeltaOffset + copyCount != runEntry.Length)
                            {
                                runDeltaOffset += copyCount;
                                break;
                            }
                        }
                    }

                    runOffset += runEntry.Length;
                    runDeltaOffset = 0;
                }
                while (++run != fragment.tail && count != 0);

                position.run = run;
                position.runOffset = runOffset;
                position.runDeltaOffset = runDeltaOffset;
            }

            return offset - startOffset;
        }

        
        
        

        protected internal int ReadOriginal(ref Fragment fragment, ref FragmentPosition position, char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(count != 0);

            var startOffset = offset;
            var run = position.run;

            if (run == fragment.head - 1)
            {
                run = position.run = fragment.head;
            }

            if (run != fragment.tail)
            {
                var runOffset = position.runOffset;
                var runDeltaOffset = position.runDeltaOffset;

                do
                {
                    var runEntry = runList[run];

                    InternalDebug.Assert(count != 0);

                    if (runEntry.Type == RunType.Literal ||runEntry.Type == RunType.Normal)
                    {
                        InternalDebug.Assert(runDeltaOffset >= 0 && runDeltaOffset < runEntry.Length);

                        var copyCount = Math.Min(count, runEntry.Length - runDeltaOffset);

                        InternalDebug.Assert(copyCount != 0);
                        
                        {
                            Buffer.BlockCopy(this.buffer, (runOffset + runDeltaOffset) * 2, buffer, offset * 2, copyCount * 2);

                            offset += copyCount;
                            count -= copyCount;

                            if (runDeltaOffset + copyCount != runEntry.Length)
                            {
                                runDeltaOffset += copyCount;
                                break;
                            }
                        }
                    }

                    runOffset += runEntry.Length;
                    runDeltaOffset = 0;
                }
                while (++run != fragment.tail && count != 0);

                position.run = run;
                position.runOffset = runOffset;
                position.runDeltaOffset = runDeltaOffset;
            }

            return offset - startOffset;
        }

        protected internal int Read(LexicalUnit unit, ref FragmentPosition position, char[] buffer, int offset, int count)
        {
            InternalDebug.Assert(count != 0);

            var startOffset = offset;

            if (unit.head != -1)
            {
                var kind = runList[unit.head].MajorKind;  

                var run = position.run;

                if (run == unit.head - 1)
                {
                    run = position.run = unit.head;
                }

                var runEntry = runList[run];

                if (run == unit.head || runEntry.MajorKindPlusStartFlag == kind)
                {
                    var runOffset = position.runOffset;
                    var runDeltaOffset = position.runDeltaOffset;

                    do
                    {
                        InternalDebug.Assert(count != 0);

                        if (runEntry.Type == RunType.Literal)
                        {
                            var literalLength = LiteralLength(runEntry.Value);

                            if (runDeltaOffset != literalLength)
                            {
                                if (literalLength == 1)
                                {
                                    InternalDebug.Assert(runDeltaOffset == 0);

                                    buffer[offset++] = (char) runEntry.Value;
                                    count --;
                                }
                                else
                                {
                                    InternalDebug.Assert(literalLength == 2);

                                    if (runDeltaOffset != 0)
                                    {
                                        InternalDebug.Assert(runDeltaOffset == 1);

                                        buffer[offset++] = LiteralLastChar(runEntry.Value);
                                        count --;
                                    }
                                    else
                                    {
                                        buffer[offset++] = LiteralFirstChar(runEntry.Value);
                                        count --;

                                        if (count == 0)
                                        {
                                            runDeltaOffset = 1;
                                            break;
                                        }

                                        buffer[offset++] = LiteralLastChar(runEntry.Value);
                                        count --;
                                    }
                                }
                            }
                        }
                        else if (runEntry.Type == RunType.Normal)
                        {
                            InternalDebug.Assert(runDeltaOffset >= 0 && runDeltaOffset < runEntry.Length);

                            var copyCount = Math.Min(count, runEntry.Length - runDeltaOffset);

                            InternalDebug.Assert(copyCount != 0);
                            
                            {
                                Buffer.BlockCopy(this.buffer, (runOffset + runDeltaOffset) * 2, buffer, offset * 2, copyCount * 2);

                                offset += copyCount;
                                count -= copyCount;

                                if (runDeltaOffset + copyCount != runEntry.Length)
                                {
                                    runDeltaOffset += copyCount;
                                    break;
                                }
                            }
                        }

                        runOffset += runEntry.Length;
                        runDeltaOffset = 0;

                        runEntry = runList[++run];

                        
                        
                        
                    }
                    while (runEntry.MajorKindPlusStartFlag == kind && count != 0);

                    position.run = run;
                    position.runOffset = runOffset;
                    position.runDeltaOffset = runDeltaOffset;
                }
            }

            return offset - startOffset;
        }

        

        protected internal virtual void Rewind()
        {
            wholePosition.Rewind(whole);
        }

        
        

        protected internal int GetLength(ref Fragment fragment)
        {
            var run = fragment.head;
            var length = 0;

            if (run != fragment.tail)
            {
                do
                {
                    var runEntry = runList[run];

                    if (runEntry.Type == RunType.Normal)
                    {
                        length += runEntry.Length;
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        length += LiteralLength(runEntry.Value);
                    }
                }
                while (++run != fragment.tail);
            }

            return length;
        }

        
        

        protected internal int GetOriginalLength(ref Fragment fragment)
        {
            var run = fragment.head;
            var length = 0;

            if (run != fragment.tail)
            {
                do
                {
                    var runEntry = runList[run];

                    if (runEntry.Type == RunType.Normal || runEntry.Type == RunType.Literal)
                    {
                        length += runEntry.Length;
                    }
                }
                while (++run != fragment.tail);
            }

            return length;
        }

        protected internal int GetLength(LexicalUnit unit)
        {
            var run = unit.head;
            var length = 0;

            if (run != -1)  
            {
                var runEntry = runList[run];
                var kind = runEntry.MajorKind;  

                do
                {
                    if (runEntry.Type == RunType.Normal)
                    {
                        length += runEntry.Length;
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        length += LiteralLength(runEntry.Value);
                    }

                    runEntry = runList[++run];

                    
                    
                    
                }
                while (runEntry.MajorKindPlusStartFlag == kind);
            }

            return length;
        }

        

        protected internal bool IsFragmentEmpty(ref Fragment fragment)
        {
            var run = fragment.head;

            if (run != fragment.tail)
            {
                do
                {
                    var runEntry = runList[run];

                    if (runEntry.Type == RunType.Normal || runEntry.Type == RunType.Literal)
                    {
                        InternalDebug.Assert(0 != runEntry.Length);
                        return false;
                    }
                }
                while (++run != fragment.tail);
            }

            return true;
        }

        protected internal bool IsFragmentEmpty(LexicalUnit unit)
        {
            var run = unit.head;

            if (run != -1)  
            {
                var runEntry = runList[run];
                var kind = runEntry.MajorKind;  

                do
                {
                    if (runEntry.Type == RunType.Normal || runEntry.Type == RunType.Literal)
                    {
                        InternalDebug.Assert(0 != runEntry.Length);
                        return false;
                    }

                    runEntry = runList[++run];

                    
                    
                    
                }
                while (runEntry.MajorKindPlusStartFlag == kind);
            }

            return true;
        }

        

        protected internal bool IsContiguous(ref Fragment fragment)
        {
            
            return fragment.head + 1 == fragment.tail && runList[fragment.head].Type == RunType.Normal;
        }

        protected internal bool IsContiguous(LexicalUnit unit)
        {
            
            InternalDebug.Assert(unit.head >= 0 && unit.head + 1 <= whole.tail && unit.head + 1 < runList.Length);

            return runList[unit.head].Type == RunType.Normal &&
                    runList[unit.head].MajorKind != runList[unit.head + 1].MajorKindPlusStartFlag;
        }

        protected internal int CalculateHashLowerCase(Fragment fragment)
        {
            var run = fragment.head;

            if (run != fragment.tail)
            {
                var runOffset = fragment.headOffset;

                if (run + 1 == fragment.tail && runList[run].Type == RunType.Normal)
                {
                    
                    return HashCode.CalculateLowerCase(buffer, runOffset, runList[run].Length);
                }

                var hashCode = new HashCode(true);

                do
                {
                    var runEntry = runList[run];

                    if (runEntry.Type == RunType.Normal)
                    {
                        hashCode.AdvanceLowerCase(buffer, runOffset, runEntry.Length);
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        hashCode.AdvanceLowerCase(runEntry.Value);
                    }

                    runOffset += runEntry.Length;
                }
                while (++run != fragment.tail);

                return hashCode.FinalizeHash();
            }

            return HashCode.CalculateEmptyHash();
        }

        protected internal int CalculateHashLowerCase(LexicalUnit unit)
        {
            var run = unit.head;

            if (run != -1)
            {
                var runOffset = unit.headOffset;
                var runEntry = runList[run];
                var kind = runEntry.MajorKind;  

                if (runEntry.Type == RunType.Normal &&
                    kind != runList[run + 1].MajorKindPlusStartFlag)
                {
                    
                    return HashCode.CalculateLowerCase(buffer, runOffset, runEntry.Length);
                }

                var hashCode = new HashCode(true);

                do
                {
                    if (runEntry.Type == RunType.Normal)
                    {
                        hashCode.AdvanceLowerCase(buffer, runOffset, runEntry.Length);
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        hashCode.AdvanceLowerCase(runEntry.Value);
                    }

                    runOffset += runEntry.Length;

                    runEntry = runList[++run];

                    
                    
                    
                }
                while (runEntry.MajorKindPlusStartFlag == kind);

                return hashCode.FinalizeHash();
            }

            return HashCode.CalculateEmptyHash();
        }

        protected internal int CalculateHash(Fragment fragment)
        {
            var run = fragment.head;

            if (run != fragment.tail)
            {
                var runOffset = fragment.headOffset;

                if (run + 1 == fragment.tail && runList[run].Type == RunType.Normal)
                {
                    
                    return HashCode.Calculate(buffer, runOffset, runList[run].Length);
                }

                var hashCode = new HashCode(true);

                do
                {
                    var runEntry = runList[run];

                    if (runEntry.Type == RunType.Normal)
                    {
                        hashCode.Advance(buffer, runOffset, runEntry.Length);
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        hashCode.Advance(runEntry.Value);
                    }

                    runOffset += runEntry.Length;
                }
                while (++run != fragment.tail);

                return hashCode.FinalizeHash();
            }

            return HashCode.CalculateEmptyHash();
        }

        protected internal int CalculateHash(LexicalUnit unit)
        {
            var run = unit.head;

            if (run != -1)
            {
                var runOffset = unit.headOffset;
                var runEntry = runList[run];
                var kind = runEntry.MajorKind;  

                if (runEntry.Type == RunType.Normal &&
                    kind != runList[run + 1].MajorKindPlusStartFlag)
                {
                    
                    return HashCode.Calculate(buffer, runOffset, runEntry.Length);
                }

                var hashCode = new HashCode(true);

                do
                {
                    if (runEntry.Type == RunType.Normal)
                    {
                        hashCode.Advance(buffer, runOffset, runEntry.Length);
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        hashCode.Advance(runEntry.Value);
                    }

                    runOffset += runEntry.Length;

                    runEntry = runList[++run];

                    
                    
                    
                }
                while (runEntry.MajorKindPlusStartFlag == kind);

                return hashCode.FinalizeHash();
            }

            return HashCode.CalculateEmptyHash();
        }

        
        

        protected internal void WriteOriginalTo(ref Fragment fragment, ITextSink sink)
        {
            var run = fragment.head;

            if (run != fragment.tail)
            {
                var runOffset = fragment.headOffset;

                do
                {
                    var runEntry = runList[run];

                    if (runEntry.Type == RunType.Normal || runEntry.Type == RunType.Literal)
                    {
                        sink.Write(buffer, runOffset, runEntry.Length);
                    }

                    runOffset += runEntry.Length;
                }
                while (++run != fragment.tail && !sink.IsEnough);
            }
        }

        
        

        protected internal void WriteTo(ref Fragment fragment, ITextSink sink)
        {
            var run = fragment.head;

            if (run != fragment.tail)
            {
                var runOffset = fragment.headOffset;

                do
                {
                    var runEntry = runList[run];

                    if (runEntry.Type == RunType.Normal)
                    {
                        sink.Write(buffer, runOffset, runEntry.Length);
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        sink.Write(runEntry.Value);
                    }

                    runOffset += runEntry.Length;
                }
                while (++run != fragment.tail && !sink.IsEnough);
            }
        }

        protected internal void WriteTo(LexicalUnit unit, ITextSink sink)
        {
            var run = unit.head;

            if (run != -1)
            {
                var runOffset = unit.headOffset;

                var runEntry = runList[run];
                var kind = runEntry.MajorKind;  

                do
                {
                    if (runEntry.Type == RunType.Normal)
                    {
                        sink.Write(buffer, runOffset, runEntry.Length);
                    }
                    else if (runEntry.Type == RunType.Literal)
                    {
                        sink.Write(runEntry.Value);
                    }

                    runOffset += runEntry.Length;

                    runEntry = runList[++run];

                    
                    
                    
                }
                while (runEntry.MajorKindPlusStartFlag == kind && !sink.IsEnough);
            }
        }

        private static char[] staticCollapseWhitespace = { ' ', '\r', '\n' };

        protected internal void WriteToAndCollapseWhitespace(ref Fragment fragment, ITextSink sink, ref CollapseWhitespaceState collapseWhitespaceState)
        {
            var run = fragment.head;

            if (run != fragment.tail)
            {
                var runOffset = fragment.headOffset;

                
                if (runList[run].Type < RunType.Normal)
                {
                    SkipNonTextRuns(ref run, ref runOffset, fragment.tail);
                }

                while (run != fragment.tail && !sink.IsEnough)
                {
                    InternalDebug.Assert(runList[run].Type >= RunType.Normal);
                    

                    if (runList[run].TextType <= RunTextType.Nbsp)
                    {
                        if (runList[run].TextType == RunTextType.NewLine)
                        {
                            collapseWhitespaceState = CollapseWhitespaceState.NewLine;
                        }
                        else if (collapseWhitespaceState == CollapseWhitespaceState.NonSpace)
                        {
                            collapseWhitespaceState = CollapseWhitespaceState.Whitespace;
                        }
                    }
                    else
                    {
                        if (collapseWhitespaceState != CollapseWhitespaceState.NonSpace)
                        {
                            if (collapseWhitespaceState == CollapseWhitespaceState.NewLine)
                            {
                                
                                sink.Write(staticCollapseWhitespace, 1, 2);
                            }
                            else
                            {
                                
                                sink.Write(staticCollapseWhitespace, 0, 1);
                            }

                            collapseWhitespaceState = CollapseWhitespaceState.NonSpace;
                        }

                        if (runList[run].Type == RunType.Literal)
                        {
                            
                            sink.Write(runList[run].Value);
                        }
                        else
                        {
                            sink.Write(buffer, runOffset, runList[run].Length);
                        }
                    }

                    runOffset += runList[run].Length;

                    run ++;

                    
                    if (run != fragment.tail && runList[run].Type < RunType.Normal)
                    {
                        SkipNonTextRuns(ref run, ref runOffset, fragment.tail);
                    }

                }
            }
        }

        
        

        protected internal string GetString(ref Fragment fragment, int maxLength)
        {
            if (fragment.head == fragment.tail)
            {
                
                return string.Empty;
            }

            if (IsContiguous(ref fragment))
            {
                
                return new string(buffer, fragment.headOffset, GetLength(ref fragment));
            }

            if (IsFragmentEmpty(ref fragment))
            {
                
                return string.Empty;
            }

            if (stringBuildSink == null)
            {
                stringBuildSink = new StringBuildSink();
            }

            stringBuildSink.Reset(maxLength);

            WriteTo(ref fragment, stringBuildSink);

            return stringBuildSink.ToString();
        }

        protected internal string GetString(LexicalUnit unit, int maxLength)
        {
            if (IsFragmentEmpty(unit))
            {
                
                return string.Empty;
            }

            if (IsContiguous(unit))
            {
                
                return new string(buffer, unit.headOffset, GetLength(unit));
            }

            if (stringBuildSink == null)
            {
                stringBuildSink = new StringBuildSink();
            }

            stringBuildSink.Reset(maxLength);

            WriteTo(unit, stringBuildSink);

            return stringBuildSink.ToString();
        }

        
        
        
        
        
        
        

        protected internal bool CaseInsensitiveCompareEqual(ref Fragment fragment, string str)
        {
            if (compareSink == null)
            {
                compareSink = new LowerCaseCompareSink();
            }

            compareSink.Reset(str);

            WriteTo(ref fragment, compareSink);

            return compareSink.IsEqual;
        }

        protected internal bool CaseInsensitiveCompareEqual(LexicalUnit unit, string str)
        {
            if (compareSink == null)
            {
                compareSink = new LowerCaseCompareSink();
            }

            compareSink.Reset(str);

            WriteTo(unit, compareSink);

            return compareSink.IsEqual;
        }

        
        

        protected internal virtual bool CaseInsensitiveCompareRunEqual(int runOffset, string str, int strOffset)
        {
            var index = strOffset;

            while (index < str.Length)
            {
                InternalDebug.Assert(!ParseSupport.IsUpperCase(str[index]));

                if (ParseSupport.ToLowerCase(buffer[runOffset++]) != str[index++])
                {
                    return false;
                }
            }

            return true;
        }

        
        
        
        
        
        
        

        protected internal bool CaseInsensitiveContainsSubstring(ref Fragment fragment, string str)
        {
            if (searchSink == null)
            {
                searchSink = new LowerCaseSubstringSearchSink();
            }

            searchSink.Reset(str);

            WriteTo(ref fragment, searchSink);

            return searchSink.IsFound;
        }

        protected internal bool CaseInsensitiveContainsSubstring(LexicalUnit unit, string str)
        {
            if (searchSink == null)
            {
                searchSink = new LowerCaseSubstringSearchSink();
            }

            searchSink.Reset(str);

            WriteTo(unit, searchSink);

            return searchSink.IsFound;
        }

        

        protected internal void StripLeadingWhitespace(ref Fragment fragment)
        {
            var run = fragment.head;

            if (run != fragment.tail)
            {
                var runOffset = fragment.headOffset;
                CharClass charClass;

                
                if (runList[run].Type < RunType.Normal)
                {
                    SkipNonTextRuns(ref run, ref runOffset, fragment.tail);
                }

                if (run == fragment.tail)
                {
                    
                    return;
                }

                do
                {
                    InternalDebug.Assert(runList[run].Type >= RunType.Normal);

                    if (runList[run].Type == RunType.Literal)
                    {
                        if (runList[run].Value > 0xFFFF)
                        {
                            break;
                        }

                        charClass = ParseSupport.GetCharClass((char)runList[run].Value);
                        if (!ParseSupport.WhitespaceCharacter(charClass))
                        {
                            break;
                        }

                        
                    }
                    else
                    {
                        

                        var offset = runOffset;
                        while (offset < runOffset + runList[run].Length)
                        {
                            charClass = ParseSupport.GetCharClass(buffer[offset]);
                            if (!ParseSupport.WhitespaceCharacter(charClass))
                            {
                                break;
                            }

                            offset ++;
                        }

                        if (offset < runOffset + runList[run].Length)
                        {
                            runList[run].Length -= (offset - runOffset);
                            runOffset = offset;
                            break;
                        }

                        
                    }

                    runOffset += runList[run].Length;

                    run ++;

                    
                    if (run != fragment.tail && runList[run].Type < RunType.Normal)
                    {
                        SkipNonTextRuns(ref run, ref runOffset, fragment.tail);
                    }
                }
                while (run != fragment.tail);

                fragment.head = run;
                fragment.headOffset = runOffset;
            }
        }

        
        protected internal bool SkipLeadingWhitespace(LexicalUnit unit, ref FragmentPosition position)
        {
            var run = unit.head;

            if (run != -1)  
            {
                var runOffset = unit.headOffset;
                CharClass charClass;

                var runEntry = runList[run];
                var kind = runEntry.MajorKind;  
                var deltaOffset = 0;

                do
                {
                    if (runEntry.Type == RunType.Literal)
                    {
                        if (runEntry.Value > 0xFFFF)
                        {
                            break;
                        }

                        charClass = ParseSupport.GetCharClass((char)runEntry.Value);
                        if (!ParseSupport.WhitespaceCharacter(charClass))
                        {
                            break;
                        }

                        
                    }
                    else if (runEntry.Type == RunType.Normal)
                    {
                        var offset = runOffset;
                        while (offset < runOffset + runEntry.Length)
                        {
                            charClass = ParseSupport.GetCharClass(buffer[offset]);
                            if (!ParseSupport.WhitespaceCharacter(charClass))
                            {
                                break;
                            }

                            offset ++;
                        }

                        if (offset < runOffset + runEntry.Length)
                        {
                            deltaOffset = offset - runOffset;
                            break;
                        }

                        
                    }

                    runOffset += runEntry.Length;

                    runEntry = runList[++run];

                    
                    
                    
                }
                while (runEntry.MajorKindPlusStartFlag == kind);

                position.run = run;
                position.runOffset = runOffset;
                position.runDeltaOffset = deltaOffset;

                if (run == unit.head || runEntry.MajorKindPlusStartFlag == kind)
                {
                    
                    return true;
                }
            }

            
            return false;
        }

        

        protected internal bool MoveToNextRun(ref Fragment fragment, ref FragmentPosition position, bool skipInvalid)
        {
            InternalDebug.Assert(position.run >= fragment.head - 1 && position.run <= fragment.tail);

            var run = position.run;

            if (run != fragment.tail)
            {
                InternalDebug.Assert(run >= fragment.head || (position.runDeltaOffset == 0 && position.runOffset == fragment.headOffset));

                if (run >= fragment.head)
                {
                    
                    position.runOffset += runList[run].Length;
                    position.runDeltaOffset = 0;
                }

                run ++;

                if (skipInvalid)
                {
                    while (run != fragment.tail && runList[run].Type == RunType.Invalid)
                    {
                        position.runOffset += runList[run].Length;
                        run ++;
                    }
                }

                position.run = run;

                return (run != fragment.tail);
            }

            return false;
        }

        

        protected internal bool IsCurrentEof(ref FragmentPosition position)
        {
            var run = position.run;

            InternalDebug.Assert(runList[run].Type >= RunType.Normal);

            if (runList[run].Type == RunType.Literal)
            {
                return (position.runDeltaOffset == LiteralLength(runList[run].Value));
            }

            return (position.runDeltaOffset == runList[run].Length);
        }

        

        protected internal int ReadCurrent(ref FragmentPosition position, char[] buffer, int offset, int count)
        {
            var run = position.run;

            InternalDebug.Assert(runList[run].Type >= RunType.Normal);
            InternalDebug.Assert(count > 0);

            if (runList[run].Type == RunType.Literal)
            {
                var literalLength = LiteralLength(runList[run].Value);

                if (position.runDeltaOffset == literalLength)
                {
                    return 0;
                }

                if (literalLength == 1)
                {
                    InternalDebug.Assert(position.runDeltaOffset == 0);

                    buffer[offset] = (char) runList[run].Value;

                    position.runDeltaOffset ++;
                    return 1;
                }

                InternalDebug.Assert(literalLength == 2);

                if (position.runDeltaOffset != 0)
                {
                    InternalDebug.Assert(position.runDeltaOffset == 1);

                    buffer[offset] = LiteralLastChar(runList[run].Value);

                    position.runDeltaOffset ++;
                    return 1;
                }

                buffer[offset++] = LiteralFirstChar(runList[run].Value);
                count --;

                position.runDeltaOffset ++;

                if (count == 0)
                {
                    return 1;
                }

                buffer[offset] = LiteralLastChar(runList[run].Value);

                position.runDeltaOffset ++;
                return 2;
            }

            var copyCount = Math.Min(count, runList[run].Length - position.runDeltaOffset);

            if (copyCount != 0)
            {
                Buffer.BlockCopy(this.buffer, (position.runOffset + position.runDeltaOffset) * 2, buffer, offset * 2, copyCount * 2);

                position.runDeltaOffset += copyCount;
            }

            return copyCount;
        }

         

        internal void SkipNonTextRuns(ref int run, ref int runOffset, int tail)
        {
            InternalDebug.Assert(run != tail && runList[run].Type < RunType.Normal);

            do
            {
                
                
                runOffset += runList[run].Length;
                run ++;
            }
            while (run != tail && runList[run].Type < RunType.Normal);
        }

        internal void Reset()
        {
            tokenId = TokenId.None;
            argument = 0;

            whole.Reset();
            wholePosition.Reset();
        }

        

        public struct RunEnumerator
        {
            private Token token;
#if DEBUG
            private int index;
#endif
            

            internal RunEnumerator(Token token)
            {
                this.token = token;
#if DEBUG
                index = this.token.wholePosition.run;
#endif
            }

            

            public TokenRun Current
            {
                get
                { 
                    InternalDebug.Assert(token.wholePosition.run >= token.whole.head && token.wholePosition.run < token.whole.tail);
                    AssertCurrent();

                    return new TokenRun(token);
                }
            }

            public bool IsValidPosition => token.wholePosition.run >= token.whole.head && token.wholePosition.run < token.whole.tail;

            public int CurrentIndex => token.wholePosition.run;

            public int CurrentOffset => token.wholePosition.runOffset;


            public bool MoveNext()
            {
                AssertCurrent();

                var ret = token.MoveToNextRun(ref token.whole, ref token.wholePosition, false);
#if DEBUG
                index = token.wholePosition.run;
#endif
                return ret;
            }

            public bool MoveNext(bool skipInvalid)
            {
                AssertCurrent();

                var ret = token.MoveToNextRun(ref token.whole, ref token.wholePosition, skipInvalid);
#if DEBUG
                index = token.wholePosition.run;
#endif
                return ret;
            }

            public void Rewind()
            {
                AssertCurrent();

                token.wholePosition.Rewind(token.whole);
#if DEBUG
                index = token.wholePosition.run;
#endif
            }

            public RunEnumerator GetEnumerator()
            {
                return this;
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(token.wholePosition.run == index);
#endif
            }
        }

        

        public struct TextReader
        {
            private Token token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal TextReader(Token token)
            {
                this.token = token;
#if DEBUG
                position = this.token.wholePosition;
#endif
            }

            

            public int Length
            {
                get
                {
                    AssertCurrent();
                    return token.GetLength(ref token.whole);
                }
            }

            

            public int Read(char[] buffer, int offset, int count)
            {
                AssertCurrent();

                var countRead = token.Read(ref token.whole, ref token.wholePosition, buffer, offset, count);
#if DEBUG
                position = token.wholePosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                AssertCurrent();

                token.wholePosition.Rewind(token.whole);
#if DEBUG
                position = token.wholePosition;
#endif
            }

            public void WriteTo(ITextSink sink)
            {
                AssertCurrent();

                token.WriteTo(ref token.whole, sink);
            }

            public void WriteToAndCollapseWhitespace(ITextSink sink, ref CollapseWhitespaceState collapseWhitespaceState)
            {
                AssertCurrent();

                token.WriteToAndCollapseWhitespace(ref token.whole, sink, ref collapseWhitespaceState);
            }

            public void StripLeadingWhitespace()
            {
                token.StripLeadingWhitespace(ref token.whole);
                Rewind();
            }

            

            public int OriginalLength
            {
                get
                {
                    AssertCurrent();
                    return token.GetOriginalLength(ref token.whole);
                }
            }

            
            

            public int ReadOriginal(char[] buffer, int offset, int count)
            {
                AssertCurrent();

                var countRead = token.ReadOriginal(ref token.whole, ref token.wholePosition, buffer, offset, count);
#if DEBUG
                position = token.wholePosition;
#endif
                return countRead;
            }

            public void WriteOriginalTo(ITextSink sink)
            {
                AssertCurrent();

                token.WriteOriginalTo(ref token.whole, sink);
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(position.SameAs(token.wholePosition));
#endif
            }
        }

        

        internal struct RunEntry
        {
            internal const int MaxRunLength = 0x07FFFFFF;
            internal const int MaxRunValue = 0x00FFFFFF;

            private uint lengthAndType;                 
            private uint valueAndKind;                  

            public void Initialize(RunType type, RunTextType textType, uint kind, int length, int value)
            {
                InternalDebug.Assert(length <= MaxRunLength && value < MaxRunValue && (kind > MaxRunValue || kind == 0));

                lengthAndType = (uint)length | (uint)type | (uint)textType;
                valueAndKind = (uint)value | (uint)kind;
            }

            public void InitializeSentinel()
            {
                valueAndKind = (uint)(RunKind.Invalid | RunKind.StartLexicalUnitFlag);
                
            }

            public RunType Type => (RunType)(lengthAndType & (uint)RunType.Mask);
            public RunTextType TextType => (RunTextType)(lengthAndType & (uint)RunTextType.Mask);

            public int Length
            {
                get { return (int)(lengthAndType & 0x00FFFFFFu); }
                set { lengthAndType = (uint)value | (lengthAndType & 0xFF000000u); }
            }

            public uint Kind => (valueAndKind & 0xFF000000u);
            public uint MajorKindPlusStartFlag => (valueAndKind & 0xFC000000u);
            public uint MajorKind => (valueAndKind & 0x7C000000u);
            public int Value => (int)(valueAndKind & 0x00FFFFFFu);


            public override string ToString()
            {
                return Type.ToString() + " - " + TextType.ToString() + " - " + ((Kind & ~(uint)RunKind.StartLexicalUnitFlag) >> 26).ToString() + "/" + ((Kind >> 24)  & 3).ToString() + " (" + Length + ") = " + Value.ToString("X6");
            }
        }

        

        internal struct LexicalUnit
        {
            public int head;                            
            public int headOffset;                      
#if false
            public bool IsEmpty
            {
                get { return this.head == this.tail; }
            }
#endif
            public void Reset()
            {
                head = -1;
                headOffset = 0;
            }

            public void Initialize(int run, int offset)
            {
                head = run;
                headOffset = offset;
            }

            
            public override string ToString()
            {
                return head.ToString("X") + " / " + headOffset.ToString("X");
            }
        }

        

        internal struct Fragment
        {
            public int head;                            
            public int tail;                            
            public int headOffset;                      

            public bool IsEmpty => head == tail;

            public void Reset()
            {
                head = tail = headOffset = 0;
            }

            public void Initialize(int run, int offset)
            {
                head = tail = run;
                headOffset = offset;
            }

            
            public override string ToString()
            {
                return head.ToString("X") + " - " + tail.ToString("X") + " / " + headOffset.ToString("X");
            }
        }

        

        internal struct FragmentPosition
        {
            public int run;                             
            public int runOffset;                       
            public int runDeltaOffset;                  

            public void Reset()
            {
                
                run = -2;      
                runOffset = 0;
                runDeltaOffset = 0;
            }

            public void Rewind(LexicalUnit unit)
            {
                run = unit.head - 1;
                runOffset = unit.headOffset;
                runDeltaOffset = 0;
            }

            public void Rewind(Fragment fragment)
            {
                run = fragment.head - 1;
                runOffset = fragment.headOffset;
                runDeltaOffset = 0;
            }

            public bool SameAs(FragmentPosition pos2)
            {
                return run == pos2.run && runOffset == pos2.runOffset && runDeltaOffset == pos2.runDeltaOffset;
            }

            
            public override string ToString()
            {
                return run.ToString("X") + " / " + runOffset.ToString("X") + " + " + runDeltaOffset.ToString("X");
            }
        }

        
        

        private class LowerCaseCompareSink : ITextSink
        {
            private bool definitelyNotEqual;
            private int strIndex;
            private string str;

            public LowerCaseCompareSink()
            {
            }

            public bool IsEqual => !definitelyNotEqual && strIndex == str.Length;

            public bool IsEnough => definitelyNotEqual;

            public void Reset(string str)
            {
                this.str = str;
                strIndex = 0;
                definitelyNotEqual = false;
            }

            public void Write(char[] buffer, int offset, int count)
            {
                var end = offset + count;

                while (offset < end)
                {
                    if (strIndex == 0)
                    {
                        if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
                        {
                            
                            offset++;
                            continue;
                        }
                    }
                    else if (strIndex == str.Length)
                    {
                        if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
                        {
                            
                            offset++;
                            continue;
                        }

                        definitelyNotEqual = true;
                        break;
                    }

                    if (ParseSupport.ToLowerCase(buffer[offset]) != str[strIndex])
                    {
                        definitelyNotEqual = true;
                        break;
                    }

                    offset++;
                    strIndex++;
                }
            }

            public void Write(int ucs32Char)
            {
                if (LiteralLength(ucs32Char) != 1)
                {
                    
                    
                    definitelyNotEqual = true;
                    return;
                }

                if (strIndex == 0)
                {
                    if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)ucs32Char)))
                    {
                        
                        return;
                    }
                }
                else if (strIndex == str.Length)
                {
                    if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)ucs32Char)))
                    {
                        
                        return;
                    }

                    definitelyNotEqual = true;
                    return;
                }

                if (str[strIndex] != ParseSupport.ToLowerCase((char)ucs32Char))
                {
                    definitelyNotEqual = true;
                    return;
                }

                strIndex++;
            }
        }

        
        
        
        
        
        
        

        private class LowerCaseSubstringSearchSink : ITextSink
        {
            private bool found;
            private int strIndex;
            private string str;

            public LowerCaseSubstringSearchSink()
            {
            }

            public bool IsFound => found;

            public bool IsEnough => found;

            public void Reset(string str)
            {
                this.str = str;
                strIndex = 0;
                found = false;
            }

            public void Write(char[] buffer, int offset, int count)
            {
                InternalDebug.Assert(!found && strIndex < str.Length);

                var end = offset + count;

                while (offset < end && strIndex < str.Length)
                {
                    if (ParseSupport.ToLowerCase(buffer[offset]) == str[strIndex])
                    {
                        strIndex ++;
                    }
                    else
                    {
                        
                        strIndex = 0;
                    }

                    offset ++;
                }

                if (strIndex == str.Length)
                {
                    found = true;
                }
            }

            public void Write(int ucs32Char)
            {
                InternalDebug.Assert(!found && strIndex < str.Length);

                if (LiteralLength(ucs32Char) != 1 || 
                    str[strIndex] != ParseSupport.ToLowerCase((char)ucs32Char))
                {
                    
                    strIndex = 0;
                    return;
                }

                strIndex ++;

                if (strIndex == str.Length)
                {
                    found = true;
                }
            }
        }
    }
}

