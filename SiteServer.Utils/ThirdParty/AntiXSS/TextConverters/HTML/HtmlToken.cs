// ***************************************************************
// <copyright file="HtmlToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
    using System;
    using Data.Internal;

    

    internal enum HtmlTokenId : byte
    {
        None = TokenId.None,
        EndOfFile = TokenId.EndOfFile,
        Text = TokenId.Text,
        EncodingChange = TokenId.EncodingChange,

        Tag = TokenId.EncodingChange + 1,

        Restart,
        OverlappedClose,                
        OverlappedReopen,               

        InjectionBegin,
        InjectionEnd,
    }

    

    internal enum HtmlLexicalUnit : uint
    {
        Invalid = RunKind.Invalid,
        Text = RunKind.Text,

        TagPrefix = (2 << 26),                      
        TagSuffix = (3 << 26),                      
        Name = (4 << 26),
        TagWhitespace = (5 << 26),                  
        AttrEqual = (6 << 26),
        AttrQuote = (7 << 26),
        AttrValue = (8 << 26),
        TagText = (9 << 26),                        
    }

    

    internal enum HtmlRunKind : uint
    {
        Invalid = RunKind.Invalid,
        Text = RunKind.Text,

        TagPrefix = HtmlLexicalUnit.TagPrefix,
        TagSuffix = HtmlLexicalUnit.TagSuffix,
        Name = HtmlLexicalUnit.Name,
        NamePrefixDelimiter = HtmlLexicalUnit.Name + (1 << 24),     
        TagWhitespace = HtmlLexicalUnit.TagWhitespace,
        AttrEqual = HtmlLexicalUnit.AttrEqual,
        AttrQuote = HtmlLexicalUnit.AttrQuote,
        AttrValue = HtmlLexicalUnit.AttrValue,
        TagText = HtmlLexicalUnit.TagText,                          
    }

    

    internal class HtmlToken : Token
    {
        protected internal HtmlTagIndex tagIndex;
        protected internal HtmlTagIndex originalTagIndex;
        protected internal HtmlNameIndex nameIndex;                  

        protected internal TagFlags flags;                           
        protected internal TagPartMajor partMajor;                   
        protected internal TagPartMinor partMinor;                   

        protected internal LexicalUnit unstructured;                 
        protected internal FragmentPosition unstructuredPosition;    

        protected internal LexicalUnit name;
        protected internal LexicalUnit localName;
        protected internal FragmentPosition namePosition;            

        protected internal AttributeEntry[] attributeList;           
        protected internal int attributeTail;                        

        protected internal int currentAttribute;                     
        protected internal FragmentPosition attrNamePosition;        
        protected internal FragmentPosition attrValuePosition;       

        

        public HtmlToken()
        {
            Reset();
        }

        

        [Flags]
        public enum TagFlags : byte
        {
            None = 0,

            
            EmptyTagName = 0x08,

            EndTag = 0x10,
            EmptyScope = 0x20,

            AllowWspLeft = 0x40,
            AllowWspRight = 0x80,
        }

        

        public enum TagPartMajor : byte
        {
            None = 0,
            Begin = 0x01 | Continue,
            Continue = 0x02,
            End = Continue | 0x04,
            Complete = Begin | End,
        }

        

        public enum TagPartMinor : byte
        {
            Empty = 0,

            
            BeginName = 0x01 | ContinueName,
            ContinueName = 0x02,
            EndName = ContinueName | 0x04,
            EndNameWithAttributes = EndName | Attributes,
            CompleteName = BeginName | EndName,
            CompleteNameWithAttributes = CompleteName | Attributes,

            
            BeginAttribute = 0x08 | ContinueAttribute,
            ContinueAttribute = 0x10,
            EndAttribute = ContinueAttribute | 0x20,
            EndAttributeWithOtherAttributes = EndAttribute | Attributes,
            AttributePartMask = BeginAttribute | EndAttribute,

            
            Attributes = 0x80,
        }

        

        public enum AttrPartMajor : byte
        {
            None = 0,
            Begin = TagPartMinor.BeginAttribute,        
            Continue = TagPartMinor.ContinueAttribute,  
            End = TagPartMinor.EndAttribute,            
            Complete = Begin | End,

            
            EmptyName = 0x01,
            ValueQuoted = 0x40,
            Deleted = 0x80,
            MaskOffFlags = Complete,
        }

        

        public enum AttrPartMinor : byte
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

        

        public new HtmlTokenId TokenId
        {
            get { return (HtmlTokenId)base.TokenId; }
            set { base.TokenId = (TokenId)value; }
        }

        public TagFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        public bool IsEndTag => 0 != (flags & TagFlags.EndTag);

        public bool IsEmptyScope => 0 != (flags & TagFlags.EmptyScope);

        public TagPartMajor MajorPart => partMajor;

        public TagPartMinor MinorPart => partMinor;

        public bool IsTagComplete => partMajor == TagPartMajor.Complete;

        public bool IsTagBegin => (partMajor & TagPartMajor.Begin) == TagPartMajor.Begin;

        public bool IsTagEnd => (partMajor & TagPartMajor.End) == TagPartMajor.End;

        public bool IsTagNameEmpty => 0 != (flags & TagFlags.EmptyTagName);

        public bool IsTagNameBegin => (partMinor & TagPartMinor.BeginName) == TagPartMinor.BeginName;

        public bool IsTagNameEnd => (partMinor & TagPartMinor.EndName) == TagPartMinor.EndName;

        public bool HasNameFragment => !IsFragmentEmpty(name);
#if false
        public bool HasUnstructuredContentFragment
        {
            get { return !this.IsFragmentEmpty(this.unstructured); }
        }
#endif
        public HtmlNameIndex NameIndex => nameIndex;

        public TagNameTextReader Name => new TagNameTextReader(this);

        public TagUnstructuredContentTextReader UnstructuredContent => new TagUnstructuredContentTextReader(this);

        public HtmlTagIndex TagIndex => tagIndex;

        public HtmlTagIndex OriginalTagId => originalTagIndex;

        public bool IsAllowWspLeft => (flags & TagFlags.AllowWspLeft) == TagFlags.AllowWspLeft;

        public bool IsAllowWspRight => (flags & TagFlags.AllowWspRight) == TagFlags.AllowWspRight;

        public AttributeEnumerator Attributes => new AttributeEnumerator(this);

        internal new void Reset()
        {
            tagIndex = originalTagIndex = HtmlTagIndex._NULL;
            nameIndex = HtmlNameIndex._NOTANAME;
            flags = TagFlags.None;
            partMajor = TagPartMajor.None;
            partMinor = TagPartMinor.Empty;

            name.Reset();
            unstructured.Reset();

            
            namePosition.Reset();
            unstructuredPosition.Reset();

            attributeTail = 0;
            currentAttribute = -1;

            
            attrNamePosition.Reset();
            attrValuePosition.Reset();
        }

        

        public struct AttributeEnumerator
        {
            private HtmlToken token;
#if DEBUG
            private int index;
#endif
            

            internal AttributeEnumerator(HtmlToken token)
            {
                this.token = token;
#if DEBUG
                index = this.token.currentAttribute;
#endif
            }

            

            public int Count => token.attributeTail;

            public HtmlAttribute Current
            {
                get
                { 
                    InternalDebug.Assert(token.currentAttribute >= 0 && token.currentAttribute < token.attributeTail);
                    AssertCurrent();

                    return new HtmlAttribute(token);
                }
            }

            public int CurrentIndex => token.currentAttribute;

            public HtmlAttribute this[int i]
            {
                get
                { 
                    InternalDebug.Assert(i >= 0 && i < token.attributeTail);

                    if (i != token.currentAttribute)
                    {
                        token.attrNamePosition.Rewind(token.attributeList[i].name);
                        token.attrValuePosition.Rewind(token.attributeList[i].value);
                    }

                    token.currentAttribute = i;

                    return new HtmlAttribute(token);
                }
            }

            

            public bool MoveNext()
            {
                InternalDebug.Assert(token.currentAttribute >= -1 && token.currentAttribute <= token.attributeTail);
                AssertCurrent();

                if (token.currentAttribute != token.attributeTail)
                {
                    token.currentAttribute ++;

                    if (token.currentAttribute != token.attributeTail)
                    {
                        token.attrNamePosition.Rewind(token.attributeList[token.currentAttribute].name);
                        token.attrValuePosition.Rewind(token.attributeList[token.currentAttribute].value);
                    }
#if DEBUG
                    index = token.currentAttribute;
#endif
                }

                return (token.currentAttribute != token.attributeTail);
            }

            public void Rewind()
            {
                AssertCurrent();

                token.currentAttribute = -1;
#if DEBUG
                index = token.currentAttribute;
#endif
            }

            public AttributeEnumerator GetEnumerator()
            {
                return this;
            }

            public bool Find(HtmlNameIndex nameIndex)
            {
                for (var i = 0; i < token.attributeTail; i++)
                {
                    if (token.attributeList[i].nameIndex == nameIndex)
                    {
                        token.currentAttribute = i;

                        token.attrNamePosition.Rewind(token.attributeList[i].name);
                        token.attrValuePosition.Rewind(token.attributeList[i].value);

                        return true;
                    }
                }

                return false;
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(token.currentAttribute == index);
#endif
            }
        }

        

        public struct TagUnstructuredContentTextReader
        {
            private HtmlToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal TagUnstructuredContentTextReader(HtmlToken token)
            {
                InternalDebug.Assert(token.TokenId == HtmlTokenId.Tag);

                this.token = token;
#if DEBUG
                position = this.token.unstructuredPosition;
#endif
            }
#if false
            public int Length
            {
                get { return this.token.GetLength(this.token.unstructured); }
            }

            

            public int Read(char[] buffer, int offset, int count)
            {
                this.AssertCurrent();

                int countRead = this.token.Read(this.token.unstructured, ref this.token.unstructuredPosition, buffer, offset, count);
#if DEBUG
                this.position = this.token.unstructuredPosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                this.token.unstructuredPosition.Rewind(this.token.unstructured);
            }
#endif
            public void WriteTo(ITextSink sink)
            {
                token.WriteTo(token.unstructured, sink);
            }

            public string GetString(int maxSize)
            {
                return token.GetString(token.unstructured, maxSize);
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(position.SameAs(token.unstructuredPosition));
#endif
            }
        }

        

        public struct TagNameTextReader
        {
            private HtmlToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal TagNameTextReader(HtmlToken token)
            {
                InternalDebug.Assert(token.TokenId == HtmlTokenId.Tag);

                this.token = token;
#if DEBUG
                position = this.token.namePosition;
#endif
            }

            public int Length => token.GetLength(token.name);


            public int Read(char[] buffer, int offset, int count)
            {
                AssertCurrent();

                var countRead = token.Read(token.name, ref token.namePosition, buffer, offset, count);
#if DEBUG
                position = token.namePosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                token.namePosition.Rewind(token.name);
            }

            public void WriteTo(ITextSink sink)
            {
                token.WriteTo(token.name, sink);
            }

            public string GetString(int maxSize)
            {
                return token.GetString(token.name, maxSize);
            }

            public void MakeEmpty()
            {
                token.name.Reset();
                Rewind();
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(position.SameAs(token.namePosition));
#endif
            }
        }

        

        public struct AttributeNameTextReader
        {
            private HtmlToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal AttributeNameTextReader(HtmlToken token)
            {
                InternalDebug.Assert(token.TokenId == HtmlTokenId.Tag);

                this.token = token;
#if DEBUG
                position = this.token.attrNamePosition;
#endif
            }

            public int Length => token.GetLength(token.attributeList[token.currentAttribute].name);


            public int Read(char[] buffer, int offset, int count)
            {
                AssertCurrent();

                var countRead = token.Read(token.attributeList[token.currentAttribute].name, ref token.attrNamePosition, buffer, offset, count);
#if DEBUG
                position = token.attrNamePosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                token.attrNamePosition.Rewind(token.attributeList[token.currentAttribute].name);
            }

            public void WriteTo(ITextSink sink)
            {
                token.WriteTo(token.attributeList[token.currentAttribute].name, sink);
            }

            public string GetString(int maxSize)
            {
                return token.GetString(token.attributeList[token.currentAttribute].name, maxSize);
            }

            public void MakeEmpty()
            {
                token.attributeList[token.currentAttribute].name.Reset();
                token.attrNamePosition.Rewind(token.attributeList[token.currentAttribute].name);
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(position.SameAs(token.attrNamePosition));
#endif
            }
        }

        

        public struct AttributeValueTextReader
        {
            private HtmlToken token;
#if DEBUG
            private FragmentPosition position;
#endif
            

            internal AttributeValueTextReader(HtmlToken token)
            {
                InternalDebug.Assert(token.TokenId == HtmlTokenId.Tag);

                this.token = token;
#if DEBUG
                position = this.token.attrValuePosition;
#endif
            }

            public int Length => token.GetLength(token.attributeList[token.currentAttribute].value);

            public bool IsEmpty => token.IsFragmentEmpty(token.attributeList[token.currentAttribute].value);

            public bool IsContiguous => token.IsContiguous(token.attributeList[token.currentAttribute].value);

            public BufferString ContiguousBufferString
            {
                get
                {
                    InternalDebug.Assert(!IsEmpty && IsContiguous);
                    return new BufferString(
                                token.buffer,
                                token.attributeList[token.currentAttribute].value.headOffset,
                                token.runList[token.attributeList[token.currentAttribute].value.head].Length);
                }
            }

            

            public int Read(char[] buffer, int offset, int count)
            {
                AssertCurrent();

                var countRead = token.Read(token.attributeList[token.currentAttribute].value, ref token.attrValuePosition, buffer, offset, count);
#if DEBUG
                position = token.attrValuePosition;
#endif
                return countRead;
            }

            public void Rewind()
            {
                token.attrValuePosition.Rewind(token.attributeList[token.currentAttribute].value);
            }

            public void WriteTo(ITextSink sink)
            {
                token.WriteTo(token.attributeList[token.currentAttribute].value, sink);
            }

            public string GetString(int maxSize)
            {
                return token.GetString(token.attributeList[token.currentAttribute].value, maxSize);
            }

            

            public bool CaseInsensitiveCompareEqual(string str)
            {
                return token.CaseInsensitiveCompareEqual(token.attributeList[token.currentAttribute].value, str);
            }

            public bool CaseInsensitiveContainsSubstring(string str)
            {
                return token.CaseInsensitiveContainsSubstring(token.attributeList[token.currentAttribute].value, str);
            }

            
            public bool SkipLeadingWhitespace()
            {
                return token.SkipLeadingWhitespace(token.attributeList[token.currentAttribute].value, ref token.attrValuePosition);
            }

            public void MakeEmpty()
            {
                token.attributeList[token.currentAttribute].value.Reset();
                Rewind();
            }

            

            [System.Diagnostics.Conditional("DEBUG")]
            private void AssertCurrent()
            {
#if DEBUG
                InternalDebug.Assert(position.SameAs(token.attrValuePosition));
#endif
            }
        }

        

        protected internal struct AttributeEntry
        {
            public HtmlNameIndex nameIndex;
            public byte quoteChar;              
            public AttrPartMajor partMajor;
            public AttrPartMinor partMinor;

            public LexicalUnit name;
            public LexicalUnit localName;
            public LexicalUnit value;

            

            public bool IsCompleteAttr => MajorPart == AttrPartMajor.Complete;

            public bool IsAttrBegin => (partMajor & AttrPartMajor.Begin) == AttrPartMajor.Begin;

            public bool IsAttrEnd => (partMajor & AttrPartMajor.End) == AttrPartMajor.End;

            public bool IsAttrEmptyName => (partMajor & AttrPartMajor.EmptyName) == AttrPartMajor.EmptyName;

            public bool IsAttrNameEnd => (partMinor & AttrPartMinor.EndName) == AttrPartMinor.EndName;

            public bool IsAttrValueBegin => (partMinor & AttrPartMinor.BeginValue) == AttrPartMinor.BeginValue;

            public AttrPartMajor MajorPart => partMajor & AttrPartMajor.MaskOffFlags;

            public AttrPartMinor MinorPart
            {
                get { return partMinor; }
                set { partMinor = value; }
            }

            public bool IsAttrValueQuoted
            {
                get { return (partMajor & AttrPartMajor.ValueQuoted) == AttrPartMajor.ValueQuoted; }
                set { partMajor = value ? (partMajor | AttrPartMajor.ValueQuoted) : (partMajor & ~AttrPartMajor.ValueQuoted); }
            }

            public bool IsAttrDeleted
            {
                get { return (partMajor & AttrPartMajor.Deleted) == AttrPartMajor.Deleted; }
                set { partMajor = value ? (partMajor | AttrPartMajor.Deleted) : (partMajor & ~AttrPartMajor.Deleted); }
            }
        }
    }

    

    internal struct HtmlAttribute
    {
        private HtmlToken token;
#if DEBUG
        private int index;
#endif
        

        internal HtmlAttribute(HtmlToken token)
        {
            this.token = token;
#if DEBUG
            index = this.token.currentAttribute;
#endif
        }

        public bool IsNull => token == null;


        public int Index
        {
            get { AssertCurrent(); return token.currentAttribute; }
        }

        public HtmlToken.AttrPartMajor MajorPart
        {
            get { AssertCurrent(); return token.attributeList[token.currentAttribute].MajorPart; }
        }

        public HtmlToken.AttrPartMinor MinorPart
        {
            get { AssertCurrent(); return token.attributeList[token.currentAttribute].MinorPart; }
        }

        public bool IsCompleteAttr
        {
            get { AssertCurrent(); return token.attributeList[token.currentAttribute].IsCompleteAttr; }
        }

        public bool IsAttrBegin
        {
            get { AssertCurrent(); return token.attributeList[token.currentAttribute].IsAttrBegin; }
        }

        public bool IsAttrEmptyName
        {
            get { AssertCurrent(); return token.attributeList[token.currentAttribute].IsAttrEmptyName; }
        }

        public bool IsAttrEnd
        {
            get { AssertCurrent(); return token.attributeList[token.currentAttribute].IsAttrEnd; }
        }

        public bool IsAttrNameEnd
        {
            get { AssertCurrent(); return token.attributeList[token.currentAttribute].IsAttrNameEnd; }
        }

        public bool IsDeleted
        {
            get { AssertCurrent(); return token.attributeList[token.currentAttribute].IsAttrDeleted; }
        }

        public bool IsAttrValueBegin
        {
            get { AssertCurrent(); return token.attributeList[token.currentAttribute].IsAttrValueBegin; }
        }

        public bool IsAttrValueQuoted
        {
            get { AssertCurrent(); return token.attributeList[token.currentAttribute].IsAttrValueQuoted; }
        }

        public HtmlNameIndex NameIndex
        {
            get { AssertCurrent(); return token.attributeList[token.currentAttribute].nameIndex; }
        }

        public char QuoteChar
        {
            get { AssertCurrent(); return (char) token.attributeList[token.currentAttribute].quoteChar; }
        }

        public bool HasNameFragment
        {
            get { AssertCurrent(); return !token.IsFragmentEmpty(token.attributeList[token.currentAttribute].name); }
        }

        public HtmlToken.AttributeNameTextReader Name 
        {
            get { AssertCurrent(); return new HtmlToken.AttributeNameTextReader(token); }
        }

        public bool HasValueFragment
        {
            get { AssertCurrent(); return !token.IsFragmentEmpty(token.attributeList[token.currentAttribute].value); }
        }

        public HtmlToken.AttributeValueTextReader Value
        {
            get { AssertCurrent(); return new HtmlToken.AttributeValueTextReader(token); }
        }

        public void SetMinorPart(HtmlToken.AttrPartMinor newMinorPart)
        {
            AssertCurrent(); 
            token.attributeList[token.currentAttribute].MinorPart = newMinorPart;
        }

        

        [System.Diagnostics.Conditional("DEBUG")]
        private void AssertCurrent()
        {
            
            
#if DEBUG
            InternalDebug.Assert(token.currentAttribute == index);
#endif
        }
    }
}

