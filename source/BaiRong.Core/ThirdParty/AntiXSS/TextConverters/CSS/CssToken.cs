// ***************************************************************
// <copyright file="CssToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Css
{
    using Data.Internal;
    using Html;

    

    internal enum CssTokenId : byte
    {
        None = TokenId.None,
        EndOfFile = TokenId.EndOfFile,
        AtRule = TokenId.EncodingChange + 1,
        Declarations,
        RuleSet,
    }

    

    internal enum CssRunKind : uint
    {
        Invalid = RunKind.Invalid,
        Comment = RunKind.Invalid,

        Text = RunKind.Text,

        Space = (11u << 24),

        SimpleSelector = (12u << 24),
        Identifier = (13u << 24),
        Delimiter = (14u << 24),
        AtRuleName = (15u << 24),

        SelectorName = (16u << 24),
        SelectorCombinatorOrComma = (17u << 24),

        
        SelectorPseudoStart = (18u << 24),
        SelectorPseudo = (19u << 24),
        SelectorPseudoArg = (20u << 24),
        SelectorClassStart = (21u << 24),
        SelectorClass = (22u << 24),
        SelectorHashStart = (23u << 24),
        SelectorHash = (24u << 24),
        SelectorAttribStart = (25u << 24),
        SelectorAttribName = (26u << 24),
        SelectorAttribEquals = (27u << 24),
        SelectorAttribIncludes = (28u << 24),
        SelectorAttribDashmatch = (29u << 24),
        SelectorAttribIdentifier = (30u << 24),
        SelectorAttribString = (31u << 24),
        SelectorAttribEnd = (32u << 24),

        PropertyName = (40u << 24),
        PropertyColon = (41u << 24),

        
        ImportantStart = (42u << 24),
        Important = (43u << 24),
        Operator = (44u << 24),
        UnaryOperator = (45u << 24),
        Dot = (46u << 24),
        Percent = (47u << 24),
        Metrics = (48u << 24),
        TermIdentifier = (49u << 24),
        UnicodeRange = (50u << 24),
        FunctionStart = (51u << 24),
        FunctionEnd = (52u << 24),
        HexColorStart = (53u << 24),
        HexColor = (54u << 24),
        String = (55u << 24),
        Numeric = (56u << 24),
        Url = (58u << 24),

        PropertySemicolon = (57u << 24),

        
        PageIdent = (70u << 24),
        PagePseudoStart = (71u << 24),
        PagePseudo = (72u << 24),

    }

    

    internal enum CssSelectorClassType : byte
    {
        Regular,
        Pseudo,
        Hash,
        Attrib,
    }

    

    internal enum CssSelectorCombinator : byte
    {
        None, 
        Descendant, 
        Adjacent, 
        Child, 
    }

    

    internal class CssToken : Token
    {
        protected internal PropertyListPartMajor partMajor;         
        protected internal PropertyListPartMinor partMinor;         

        protected internal PropertyEntry[] propertyList;            
        protected internal int propertyHead;                        
        protected internal int propertyTail;                        
        protected internal int currentProperty;                     

        protected internal FragmentPosition propertyNamePosition;   
        protected internal FragmentPosition propertyValuePosition;  

        protected internal SelectorEntry[] selectorList;            
        protected internal int selectorHead;                        
        protected internal int selectorTail;                        
        protected internal int currentSelector;                     

        protected internal FragmentPosition selectorNamePosition;   
        protected internal FragmentPosition selectorClassPosition;  

        

        public enum PropertyListPartMajor : byte
        {
            None = 0,
            Begin = 0x01 | Continue,
            Continue = 0x02,
            End = Continue | 0x04,
            Complete = Begin | End,
        }

        

        public enum PropertyListPartMinor : byte
        {
            Empty = 0,

            
            BeginProperty = 0x08 | ContinueProperty,
            ContinueProperty = 0x10,
            EndProperty = ContinueProperty | 0x20,
            EndPropertyWithOtherProperties = EndProperty | Properties,
            PropertyPartMask = BeginProperty | EndProperty,

            
            Properties = 0x80,
        }

        

        public enum PropertyPartMajor : byte
        {
            None = 0,
            Begin = 0x01 | Continue,
            Continue = 0x02,
            End = Continue | 0x04,
            Complete = Begin | End,

            
            ValueQuoted = 0x40,
            Deleted = 0x80,
            MaskOffFlags = Complete,
        }

        

        public enum PropertyPartMinor : byte
        {
            Empty = 0,

            BeginName = 0x01 | ContinueName,
            ContinueName = 0x02,
            EndName = ContinueName | 0x04,
            EndNameWithBeginValue = EndName | BeginValue,
            EndNameWithCompleteValue = EndName | CompleteValue,
            CompleteName = BeginName | EndName,
            CompleteNameWithBeginValue = CompleteName | BeginValue,
            CompleteNameWithCompleteValue = CompleteName | CompleteValue,

            BeginValue = 0x08 | ContinueValue,
            ContinueValue = 0x10,
            EndValue = ContinueValue | 0x20,
            CompleteValue = BeginValue | EndValue,
        }

        public CssToken()
        {
            Reset();
        }

        public new CssTokenId TokenId => (CssTokenId)base.TokenId;

        public PropertyListPartMajor MajorPart => partMajor;

        public PropertyListPartMinor MinorPart => partMinor;

        public bool IsPropertyListBegin => (partMajor & PropertyListPartMajor.Begin) == PropertyListPartMajor.Begin;

        public bool IsPropertyListEnd => (partMajor & PropertyListPartMajor.End) == PropertyListPartMajor.End;

        public PropertyEnumerator Properties => new PropertyEnumerator(this);

        public SelectorEnumerator Selectors => new SelectorEnumerator(this);

        internal new void Reset()
        {
            partMajor = PropertyListPartMajor.None;
            partMinor = PropertyListPartMinor.Empty;

            propertyHead = propertyTail = 0;
            currentProperty = -1;

            selectorHead = selectorTail = 0;
            currentSelector = -1;
        }

        
        
        
        
        
        protected internal void WriteEscapedOriginalTo(ref Fragment fragment, ITextSink sink)
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
                        EscapeAndWriteBuffer(
                            buffer,
                            runOffset,
                            runEntry.Length,
                            sink);
                    }

                    runOffset += runEntry.Length;
                }
                while (++run != fragment.tail && !sink.IsEnough);
            }
        }

        private void EscapeAndWriteBuffer(
            char[] buffer,
            int offset,
            int length,
            ITextSink sink)
        {
            var lastIdx = offset;
            for (var idx = offset; idx < offset + length; )
            {
                var ch = buffer[idx];

                
                
                if (ch == '>' || ch == '<')
                {
                    
                    
                    if (idx - lastIdx > 0)
                    {
                        sink.Write(buffer, lastIdx, idx - lastIdx);
                    }

                    
                    
                    var value = (uint)ch;
                    var escapeSequenceBuffer = new char[4];
                    escapeSequenceBuffer[0] = '\\';
                    escapeSequenceBuffer[3] = ' ';
                    for (var i = 2; i > 0; --i)
                    {
                        var digit = value & 0xF;
                        escapeSequenceBuffer[i] = (char)(digit + (digit < 10 ? '0' : 'A' - 10));
                        value >>= 4;
                    }
                    sink.Write(escapeSequenceBuffer, 0, 4);

                    lastIdx = ++idx;
                }
                else
                {
                    
                    
                    AttemptUnescape(buffer, offset + length, ref ch, ref idx);
                    ++idx;
                }
            }
            
            
            sink.Write(buffer, lastIdx, length - (lastIdx - offset));
        }

        
        
        
        
        
        
        
        internal static bool AttemptUnescape(char[] parseBuffer, int parseEnd, ref char ch, ref int parseCurrent)
        {
            if (ch != '\\' || parseCurrent == parseEnd)
            {
                return false;
            }

            ch = parseBuffer[++parseCurrent];
            var charClass = ParseSupport.GetCharClass(ch);

            var end = parseCurrent + 6;
            end = end < parseEnd ? end : parseEnd;

            if (ParseSupport.HexCharacter(charClass))
            {
                
                
                var result = 0;
                while (true)
                {
                    result <<= 4;
                    result |= ParseSupport.CharToHex(ch);

                    if (parseCurrent == end)
                    {
                        break;
                    }
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    if (!ParseSupport.HexCharacter(charClass))
                    {
                        if (ch == '\r' && parseCurrent != parseEnd)
                        {
                            ch = parseBuffer[++parseCurrent];
                            if (ch == '\n')
                            {
                                charClass = ParseSupport.GetCharClass(ch);
                            }
                            else
                            {
                                --parseCurrent;
                            }
                        }
                        if (ch != ' ' && ch != '\t' && ch != '\r' && ch != '\n' && ch != '\f')
                        {
                            --parseCurrent;
                        }
                        break;
                    }
                }

                ch = (char)result;
                return true;
            }

            if (ch >= ' ' && ch != (char)0x7F)
            {
                
                
                return true;
            }

            
            
            --parseCurrent;
            ch = '\\';

            return false;
        }

        

        public struct PropertyEnumerator
        {
            private CssToken token;
#if DEBUG
            private int index;
#endif
            

            internal PropertyEnumerator(CssToken token)
            {
                this.token = token;
#if DEBUG
                index = this.token.currentProperty;
#endif
            }

            

            public int Count => token.propertyTail - token.propertyHead;

            public int ValidCount
            {
                get
                {
                    var count = 0;
                    for (var i = token.propertyHead; i < token.propertyTail; i++)
                    {
                        if (!token.propertyList[i].IsPropertyDeleted)
                        {
                            count++;
                        }
                    }
                    return count;
                }
            }

            public CssProperty Current
            {
                get
                {
                    InternalDebug.Assert(token.currentProperty >= token.propertyHead && token.currentProperty < token.propertyTail);
                    AssertCurrent();

                    return new CssProperty(token);
                }
            }

            public int CurrentIndex => token.currentProperty;

            public CssProperty this[int i]
            {
                get
                {
                    InternalDebug.Assert(i >= token.propertyHead && i < token.propertyTail);

                    token.currentProperty = i;

                    token.propertyNamePosition.Rewind(token.propertyList[i].name);
                    token.propertyValuePosition.Rewind(token.propertyList[i].value);

                    return new CssProperty(token);
                }
            }

            

            public bool MoveNext()
            {
                InternalDebug.Assert(token.currentProperty >= token.propertyHead - 1 && token.currentProperty <= token.propertyTail);
                AssertCurrent();

                if (token.currentProperty != token.propertyTail)
                {
                    token.currentProperty++;

                    if (token.currentProperty != token.propertyTail)
                    {
                        token.propertyNamePosition.Rewind(token.propertyList[token.currentProperty].name);
                        token.propertyValuePosition.Rewind(token.propertyList[token.currentProperty].value);
                    }
#if DEBUG
                    index = token.currentProperty;
#endif
                }

                return (token.currentProperty != token.propertyTail);
            }

            public void Rewind()
            {
                AssertCurrent();

                token.currentProperty = token.propertyHead - 1;
#if DEBUG
                index = token.currentProperty;
#endif
            }

            public PropertyEnumerator GetEnumerator()
            {
                return this;
            }

            public bool Find(CssNameIndex nameId)
            {
                for (var i = token.propertyHead; i < token.propertyTail; i++)
                {
                    if (token.propertyList[i].nameId == nameId)
                    {
                        token.currentProperty = i;

                        token.propertyNamePosition.Rewind(token.propertyList[i].name);
                        token.propertyValuePosition.Rewind(token.propertyList[i].value);

                        return true;
                    }
                }

                return false;
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(token.currentProperty == index);
#endif
            }
        }

        

        public struct PropertyNameTextReader
        {
            private CssToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal PropertyNameTextReader(CssToken token)
            {
                

                this.token = token;
#if DEBUG
                position = this.token.propertyNamePosition;
#endif
            }

            public int Length => token.GetLength(ref token.propertyList[token.currentProperty].name);


            public int Read(char[] buffer, int offset, int count)
            {
                AssertCurrent();

                var countRead = token.Read(ref token.propertyList[token.currentProperty].name, ref token.propertyNamePosition, buffer, offset, count);
#if DEBUG
                position = token.propertyNamePosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                token.propertyNamePosition.Rewind(token.propertyList[token.currentProperty].name);
            }

            public void WriteTo(ITextSink sink)
            {
                token.WriteTo(ref token.propertyList[token.currentProperty].name, sink);
            }

            public void WriteOriginalTo(ITextSink sink)
            {
                token.WriteOriginalTo(ref token.propertyList[token.currentProperty].name, sink);
            }

            public string GetString(int maxSize)
            {
                return token.GetString(ref token.propertyList[token.currentProperty].name, maxSize);
            }

            public void MakeEmpty()
            {
                token.propertyList[token.currentProperty].name.Reset();
                Rewind();
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(position.SameAs(token.propertyNamePosition));
#endif
            }
        }

        

        public struct PropertyValueTextReader
        {
            private CssToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal PropertyValueTextReader(CssToken token)
            {
                

                this.token = token;
#if DEBUG
                position = this.token.propertyValuePosition;
#endif
            }

            public int Length => token.GetLength(ref token.propertyList[token.currentProperty].value);

            public bool IsEmpty => token.IsFragmentEmpty(ref token.propertyList[token.currentProperty].value);

            public bool IsContiguous => token.IsContiguous(ref token.propertyList[token.currentProperty].value);

            public BufferString ContiguousBufferString
            {
                get
                {
                    InternalDebug.Assert(IsContiguous);
                    return new BufferString(
                                token.buffer,
                                token.propertyList[token.currentProperty].value.headOffset,
                                token.runList[token.propertyList[token.currentProperty].value.head].Length);
                }
            }

            

            public int Read(char[] buffer, int offset, int count)
            {
                AssertCurrent();

                var countRead = token.Read(ref token.propertyList[token.currentProperty].value, ref token.propertyValuePosition, buffer, offset, count);
#if DEBUG
                position = token.propertyValuePosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                token.propertyValuePosition.Rewind(token.propertyList[token.currentProperty].value);
            }

            public void WriteTo(ITextSink sink)
            {
                token.WriteTo(ref token.propertyList[token.currentProperty].value, sink);
            }

            public void WriteOriginalTo(ITextSink sink)
            {
                token.WriteOriginalTo(ref token.propertyList[token.currentProperty].value, sink);
            }

            public void WriteEscapedOriginalTo(ITextSink sink)
            {
                token.WriteEscapedOriginalTo(ref token.propertyList[token.currentProperty].value, sink);
            }

            public string GetString(int maxSize)
            {
                return token.GetString(ref token.propertyList[token.currentProperty].value, maxSize);
            }

            

            public bool CaseInsensitiveCompareEqual(string str)
            {
                return token.CaseInsensitiveCompareEqual(ref token.propertyList[token.currentProperty].value, str);
            }

            public bool CaseInsensitiveContainsSubstring(string str)
            {
                return token.CaseInsensitiveContainsSubstring(ref token.propertyList[token.currentProperty].value, str);
            }

            public void MakeEmpty()
            {
                token.propertyList[token.currentProperty].value.Reset();
                Rewind();
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(position.SameAs(token.propertyValuePosition));
#endif
            }
        }

        

        protected internal struct PropertyEntry
        {
            public CssNameIndex nameId;
            public byte quoteChar;              
            public PropertyPartMajor partMajor;
            public PropertyPartMinor partMinor;

            public Fragment name;
            public Fragment value;

            

            public bool IsCompleteProperty => MajorPart == PropertyPartMajor.Complete;

            public bool IsPropertyBegin => (partMajor & PropertyPartMajor.Begin) == PropertyPartMajor.Begin;

            public bool IsPropertyEnd => (partMajor & PropertyPartMajor.End) == PropertyPartMajor.End;

            public bool IsPropertyNameEnd => (partMinor & PropertyPartMinor.EndName) == PropertyPartMinor.EndName;

            public bool IsPropertyValueBegin => (partMinor & PropertyPartMinor.BeginValue) == PropertyPartMinor.BeginValue;

            public PropertyPartMajor MajorPart => partMajor & PropertyPartMajor.MaskOffFlags;

            public PropertyPartMinor MinorPart
            {
                get { return partMinor; }
                set { partMinor = value; }
            }

            public bool IsPropertyValueQuoted
            {
                get { return (partMajor & PropertyPartMajor.ValueQuoted) == PropertyPartMajor.ValueQuoted; }
                set { partMajor = value ? (partMajor | PropertyPartMajor.ValueQuoted) : (partMajor & ~PropertyPartMajor.ValueQuoted); }
            }

            public bool IsPropertyDeleted
            {
                get { return (partMajor & PropertyPartMajor.Deleted) == PropertyPartMajor.Deleted; }
                set { partMajor = value ? (partMajor | PropertyPartMajor.Deleted) : (partMajor & ~PropertyPartMajor.Deleted); }
            }
        }

        

        public struct SelectorEnumerator
        {
            private CssToken token;
#if DEBUG
            private int index;
#endif
            

            internal SelectorEnumerator(CssToken token)
            {
                this.token = token;
#if DEBUG
                index = this.token.currentSelector;
#endif
            }

            

            public int Count => token.selectorTail - token.selectorHead;

            public int ValidCount
            {
                get
                {
                    var count = 0;
                    for (var i = token.selectorHead; i < token.selectorTail; i++)
                    {
                        if (!token.selectorList[i].IsSelectorDeleted)
                        {
                            count++;
                        }
                    }
                    return count;
                }
            }

            public CssSelector Current
            {
                get
                {
                    InternalDebug.Assert(token.currentSelector >= token.selectorHead && token.currentSelector < token.selectorTail);
                    AssertCurrent();

                    return new CssSelector(token);
                }
            }

            public int CurrentIndex => token.currentSelector;

            public CssSelector this[int i]
            {
                get
                {
                    InternalDebug.Assert(i >= token.selectorHead && i < token.selectorTail);

                    token.currentSelector = i;

                    token.selectorNamePosition.Rewind(token.selectorList[i].name);
                    token.selectorClassPosition.Rewind(token.selectorList[i].className);

                    return new CssSelector(token);
                }
            }

            

            public bool MoveNext()
            {
                InternalDebug.Assert(token.currentSelector >= token.selectorHead - 1 && token.currentSelector <= token.selectorTail);
                AssertCurrent();

                if (token.currentSelector != token.selectorTail)
                {
                    token.currentSelector++;

                    if (token.currentSelector != token.selectorTail)
                    {
                        token.selectorNamePosition.Rewind(token.selectorList[token.currentSelector].name);
                        token.selectorClassPosition.Rewind(token.selectorList[token.currentSelector].className);
                    }
#if DEBUG
                    index = token.currentSelector;
#endif
                }

                return (token.currentSelector != token.selectorTail);
            }

            public void Rewind()
            {
                AssertCurrent();

                token.currentSelector = token.selectorHead - 1;
#if DEBUG
                index = token.currentSelector;
#endif
            }

            public SelectorEnumerator GetEnumerator()
            {
                return this;
            }

            public bool Find(HtmlNameIndex nameId)
            {
                for (var i = token.selectorHead; i < token.selectorTail; i++)
                {
                    if (token.selectorList[i].nameId == nameId)
                    {
                        token.currentSelector = i;

                        token.selectorNamePosition.Rewind(token.selectorList[i].name);
                        token.selectorClassPosition.Rewind(token.selectorList[i].className);

                        return true;
                    }
                }

                return false;
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(token.currentSelector == index);
#endif
            }
        }

        

        public struct SelectorNameTextReader
        {
            private CssToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal SelectorNameTextReader(CssToken token)
            {
                

                this.token = token;
#if DEBUG
                position = this.token.selectorNamePosition;
#endif
            }

            public int Length => token.GetLength(ref token.selectorList[token.currentSelector].name);


            public int Read(char[] buffer, int offset, int count)
            {
                AssertCurrent();

                var countRead = token.Read(ref token.selectorList[token.currentSelector].name, ref token.selectorNamePosition, buffer, offset, count);
#if DEBUG
                position = token.selectorNamePosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                token.selectorNamePosition.Rewind(token.selectorList[token.currentSelector].name);
            }

            public void WriteTo(ITextSink sink)
            {
                token.WriteTo(ref token.selectorList[token.currentSelector].name, sink);
            }

            public void WriteOriginalTo(ITextSink sink)
            {
                token.WriteOriginalTo(ref token.selectorList[token.currentSelector].name, sink);
            }

            public string GetString(int maxSize)
            {
                return token.GetString(ref token.selectorList[token.currentSelector].name, maxSize);
            }

            public void MakeEmpty()
            {
                token.selectorList[token.currentSelector].name.Reset();
                Rewind();
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(position.SameAs(token.selectorNamePosition));
#endif
            }
        }

        

        public struct SelectorClassTextReader
        {
            private CssToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal SelectorClassTextReader(CssToken token)
            {
                

                this.token = token;
#if DEBUG
                position = this.token.selectorClassPosition;
#endif
            }

            public int Length => token.GetLength(ref token.selectorList[token.currentSelector].className);


            public int Read(char[] buffer, int offset, int count)
            {
                AssertCurrent();

                var countRead = token.Read(ref token.selectorList[token.currentSelector].className, ref token.selectorClassPosition, buffer, offset, count);
#if DEBUG
                position = token.selectorClassPosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                token.selectorClassPosition.Rewind(token.selectorList[token.currentSelector].className);
            }

            public void WriteTo(ITextSink sink)
            {
                token.WriteTo(ref token.selectorList[token.currentSelector].className, sink);
            }

            public void WriteOriginalTo(ITextSink sink)
            {
                token.WriteEscapedOriginalTo(ref token.selectorList[token.currentSelector].className, sink);
            }

            public string GetString(int maxSize)
            {
                return token.GetString(ref token.selectorList[token.currentSelector].className, maxSize);
            }

            

            public bool CaseInsensitiveCompareEqual(string str)
            {
                return token.CaseInsensitiveCompareEqual(ref token.selectorList[token.currentSelector].className, str);
            }

            public bool CaseInsensitiveContainsSubstring(string str)
            {
                return token.CaseInsensitiveContainsSubstring(ref token.selectorList[token.currentSelector].className, str);
            }

            public void MakeEmpty()
            {
                token.selectorList[token.currentSelector].className.Reset();
                Rewind();
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(position.SameAs(token.selectorClassPosition));
#endif
            }
        }

        

        protected internal struct SelectorEntry
        {
            public HtmlNameIndex nameId;

            public bool deleted;

            public Fragment name;
            public Fragment className;
            public CssSelectorClassType classType;
            public CssSelectorCombinator combinator;

            

            public bool IsSelectorDeleted
            {
                get { return deleted; }
                set { deleted = value; }
            }
        }
    }

    

    internal struct CssSelector
    {
        private CssToken token;
#if DEBUG
        private int index;
#endif
        

        internal CssSelector(CssToken token)
        {
            this.token = token;
#if DEBUG
            index = this.token.currentSelector;
#endif
        }

        

        public int Index
        {
            get { AssertCurrent(); return token.currentSelector; }
        }

        public bool IsDeleted
        {
            get { AssertCurrent(); return token.selectorList[token.currentSelector].IsSelectorDeleted; }
        }

        public HtmlNameIndex NameId
        {
            get { AssertCurrent(); return token.selectorList[token.currentSelector].nameId; }
        }

        public bool HasNameFragment
        {
            get { AssertCurrent(); return !token.selectorList[token.currentSelector].name.IsEmpty; }
        }

        public CssToken.SelectorNameTextReader Name
        {
            get { AssertCurrent(); return new CssToken.SelectorNameTextReader(token); }
        }

        public bool HasClassFragment
        {
            get { AssertCurrent(); return !token.selectorList[token.currentSelector].className.IsEmpty; }
        }

        public CssToken.SelectorClassTextReader ClassName
        {
            get { AssertCurrent(); return new CssToken.SelectorClassTextReader(token); }
        }

        public CssSelectorClassType ClassType
        {
            get { AssertCurrent(); return token.selectorList[token.currentSelector].classType; }
        }

        public bool IsSimple
        {
            get
            {
                AssertCurrent();

                
                
                return (token.selectorList[token.currentSelector].combinator == CssSelectorCombinator.None &&
                    ((token.selectorTail == token.currentSelector + 1) ||
                    (token.selectorList[token.currentSelector + 1].combinator == CssSelectorCombinator.None)));
            }
        }

        public CssSelectorCombinator Combinator
        {
            get { AssertCurrent(); return token.selectorList[token.currentSelector].combinator; }
        }

        

        [System.Diagnostics.Conditional("DEBUG")]
        private void AssertCurrent()
        {
            
            
#if DEBUG
            InternalDebug.Assert(token.currentSelector == index);
#endif
        }
    }

    

    internal struct CssProperty
    {
        private CssToken token;
#if DEBUG
        private int index;
#endif
        

        internal CssProperty(CssToken token)
        {
            this.token = token;
#if DEBUG
            index = this.token.currentProperty;
#endif
        }

        

        public int Index
        {
            get { AssertCurrent(); return token.currentProperty; }
        }

        public bool IsCompleteProperty
        {
            get { AssertCurrent(); return token.propertyList[token.currentProperty].IsCompleteProperty; }
        }

        public bool IsPropertyBegin
        {
            get { AssertCurrent(); return token.propertyList[token.currentProperty].IsPropertyBegin; }
        }

        public bool IsPropertyEnd
        {
            get { AssertCurrent(); return token.propertyList[token.currentProperty].IsPropertyEnd; }
        }

        public bool IsPropertyNameEnd
        {
            get { AssertCurrent(); return token.propertyList[token.currentProperty].IsPropertyNameEnd; }
        }

        public bool IsDeleted
        {
            get { AssertCurrent(); return token.propertyList[token.currentProperty].IsPropertyDeleted; }
        }

        public bool IsPropertyValueQuoted
        {
            get { AssertCurrent(); return token.propertyList[token.currentProperty].IsPropertyValueQuoted; }
        }

        public CssNameIndex NameId
        {
            get { AssertCurrent(); return token.propertyList[token.currentProperty].nameId; }
        }

        public char QuoteChar
        {
            get { AssertCurrent(); return (char)token.propertyList[token.currentProperty].quoteChar; }
        }

        public bool HasNameFragment
        {
            get { AssertCurrent(); return !token.propertyList[token.currentProperty].name.IsEmpty; }
        }

        public CssToken.PropertyNameTextReader Name
        {
            get { AssertCurrent(); return new CssToken.PropertyNameTextReader(token); }
        }

        public bool HasValueFragment
        {
            get { AssertCurrent(); return !token.propertyList[token.currentProperty].value.IsEmpty; }
        }

        public CssToken.PropertyValueTextReader Value
        {
            get { AssertCurrent(); return new CssToken.PropertyValueTextReader(token); }
        }

        public void SetMinorPart(CssToken.PropertyPartMinor newMinorPart)
        {
            AssertCurrent();
            token.propertyList[token.currentProperty].MinorPart = newMinorPart;
        }

        

        [System.Diagnostics.Conditional("DEBUG")]
        private void AssertCurrent()
        {
            
            
#if DEBUG
            InternalDebug.Assert(token.currentProperty == index);
#endif
        }
    }
}

