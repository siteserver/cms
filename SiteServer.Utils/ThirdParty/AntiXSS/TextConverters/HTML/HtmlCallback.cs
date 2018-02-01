// ***************************************************************
// <copyright file="HtmlCallback.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.Collections.Generic;
    using Data.Internal;
    using Internal.Html;
    using Strings = CtsResources.TextConvertersStrings;


    internal struct HtmlTagParts
    {
        private HtmlToken.TagPartMajor major;
        private HtmlToken.TagPartMinor minor;

        internal HtmlTagParts(HtmlToken.TagPartMajor major, HtmlToken.TagPartMinor minor)
        {
            this.minor = minor;
            this.major = major;
        }

        internal void Reset()
        {
            minor = 0;
            major = 0;
        }

        public bool Begin => HtmlToken.TagPartMajor.Begin == (major & HtmlToken.TagPartMajor.Begin);
        public bool End => HtmlToken.TagPartMajor.End == (major & HtmlToken.TagPartMajor.End);
        public bool Complete => HtmlToken.TagPartMajor.Complete == major;

        public bool NameBegin => HtmlToken.TagPartMinor.BeginName == (minor & HtmlToken.TagPartMinor.BeginName);
        public bool Name => HtmlToken.TagPartMinor.ContinueName == (minor & HtmlToken.TagPartMinor.ContinueName);
        public bool NameEnd => HtmlToken.TagPartMinor.EndName == (minor & HtmlToken.TagPartMinor.EndName);
        public bool NameComplete => HtmlToken.TagPartMinor.CompleteName == (minor & HtmlToken.TagPartMinor.CompleteName);

        public bool Attributes => 0 != (minor & (HtmlToken.TagPartMinor.Attributes | HtmlToken.TagPartMinor.ContinueAttribute));

        public override string ToString()
        {
            return major.ToString() + " /" + minor.ToString() + "/";
        }
    }

    

    internal struct HtmlAttributeParts
    {
        private HtmlToken.AttrPartMajor major;
        private HtmlToken.AttrPartMinor minor;

        internal HtmlAttributeParts(HtmlToken.AttrPartMajor major, HtmlToken.AttrPartMinor minor)
        {
            this.minor = minor;
            this.major = major;
        }

        public bool Begin => HtmlToken.AttrPartMajor.Begin == (major & HtmlToken.AttrPartMajor.Begin);
        public bool End => HtmlToken.AttrPartMajor.End == (major & HtmlToken.AttrPartMajor.End);
        public bool Complete => HtmlToken.AttrPartMajor.Complete == major;

        public bool NameBegin => HtmlToken.AttrPartMinor.BeginName == (minor & HtmlToken.AttrPartMinor.BeginName);
        public bool Name => HtmlToken.AttrPartMinor.ContinueName == (minor & HtmlToken.AttrPartMinor.ContinueName);
        public bool NameEnd => HtmlToken.AttrPartMinor.EndName == (minor & HtmlToken.AttrPartMinor.EndName);
        public bool NameComplete => HtmlToken.AttrPartMinor.CompleteName == (minor & HtmlToken.AttrPartMinor.CompleteName);

        public bool ValueBegin => HtmlToken.AttrPartMinor.BeginValue == (minor & HtmlToken.AttrPartMinor.BeginValue);
        public bool Value => HtmlToken.AttrPartMinor.ContinueValue == (minor & HtmlToken.AttrPartMinor.ContinueValue);
        public bool ValueEnd => HtmlToken.AttrPartMinor.EndValue == (minor & HtmlToken.AttrPartMinor.EndValue);
        public bool ValueComplete => HtmlToken.AttrPartMinor.CompleteValue == (minor & HtmlToken.AttrPartMinor.CompleteValue);

        public override string ToString()
        {
            return major.ToString() + " /" + minor.ToString() + "/";
        }
    }

    

    
    
    
    
    
    internal delegate void HtmlTagCallback(HtmlTagContext tagContext, HtmlWriter htmlWriter);

    

    
    
    
    internal abstract class HtmlTagContext
    {
        internal enum TagWriteState
        {
            Undefined,
            Written,
            Deleted,
        }

        private TagWriteState writeState;

        private byte cookie;
        private bool valid;

        private bool invokeCallbackForEndTag;
        private bool deleteInnerContent;
        private bool deleteEndTag;

        private bool isEndTag;
        private bool isEmptyElementTag;

        private HtmlNameIndex tagNameIndex;

        private HtmlTagParts tagParts;

        private int attributeCount;

        internal HtmlTagContext()
        {
        }

        
        
        
        public bool IsEndTag       
        {
            get
            {
                AssertContextValid();
                return isEndTag;
            }
        }

        
        
        
        public bool IsEmptyElementTag   
        {
            get
            {
                AssertContextValid();
                return isEmptyElementTag;
            }
        }

        
        
        
        public HtmlTagId TagId
        {
            get
            {
                AssertContextValid();
                return HtmlNameData.names[(int)tagNameIndex].publicTagId;
            }
        }

        
        
        
        public string TagName
        {
            get
            {
                AssertContextValid();
                return GetTagNameImpl();
            }
        }

        internal HtmlNameIndex TagNameIndex
        {
            get
            {
                AssertContextValid();
                return tagNameIndex;
            }
        }

        internal HtmlTagParts TagParts
        {
            get
            {
                AssertContextValid();
                return tagParts;
            }
        }

        
        
        
        public AttributeCollection Attributes
        {
            get
            {
                AssertContextValid();
                return new AttributeCollection(this);
            }
        }

        
        

        
        
        
        

        internal bool IsInvokeCallbackForEndTag => invokeCallbackForEndTag;

        internal bool IsDeleteInnerContent => deleteInnerContent;

        internal bool IsDeleteEndTag => deleteEndTag;

        internal bool CopyPending
        {
            get
            {
                AssertContextValid();
                return GetCopyPendingStateImpl();
            }
        }

        
        
        
        public void WriteTag()
        {
            WriteTag(false);
        }

        
        
        
        
        public void WriteTag(bool copyInputAttributes)
        {
            AssertContextValid();

            if (writeState != TagWriteState.Undefined)
            {
                throw new InvalidOperationException(writeState == TagWriteState.Written ? Strings.CallbackTagAlreadyWritten : Strings.CallbackTagAlreadyDeleted);
            }

            deleteEndTag = false;

            

            WriteTagImpl(!isEndTag && copyInputAttributes);

            writeState = TagWriteState.Written;
        }

        
        
        
        public void DeleteTag()
        {
            DeleteTag(false);
        }

        
        
        
        
        public void DeleteTag(bool keepEndTag)
        {
            AssertContextValid();

            if (writeState != TagWriteState.Undefined)
            {
                throw new InvalidOperationException(writeState == TagWriteState.Written ? Strings.CallbackTagAlreadyWritten : Strings.CallbackTagAlreadyDeleted);
            }

            if (!isEndTag && !isEmptyElementTag)
            {
                deleteEndTag = !keepEndTag;
            }
            else
            {
                deleteEndTag = false;
            }

            DeleteTagImpl();

            writeState = TagWriteState.Deleted;
        }

        
        
        
        public void DeleteInnerContent()
        {
            AssertContextValid();

            if (!isEndTag && !isEmptyElementTag)
            {
                deleteInnerContent = true;
            }
        }

        
        
        
        public void InvokeCallbackForEndTag()
        {
            AssertContextValid();

            if (!isEndTag && !isEmptyElementTag)
            {
                invokeCallbackForEndTag = true;
            }
        }

        
        

        internal void InitializeTag(bool isEndTag, HtmlNameIndex tagNameIndex, bool droppedEndTag)
        {
            this.isEndTag = isEndTag;
            isEmptyElementTag = false;
            this.tagNameIndex = tagNameIndex;
            writeState = droppedEndTag ? TagWriteState.Deleted : TagWriteState.Undefined;

            
            invokeCallbackForEndTag = false;
            deleteInnerContent = false;
            deleteEndTag = !this.isEndTag;

            cookie = unchecked((byte)(cookie + 1));       
        }

        internal void InitializeFragment(bool isEmptyElementTag, int attributeCount, HtmlTagParts tagParts)
        {
            if (attributeCount >= 0x00FFFFFF)
            {
                
                throw new TextConvertersException();
            }

            this.isEmptyElementTag = isEmptyElementTag;
            this.tagParts = tagParts;
            this.attributeCount = attributeCount;

            cookie = unchecked((byte)(cookie + 1));       
            valid = true;
        }

        internal void UninitializeFragment()
        {
            valid = false;
        }

        
        

        internal virtual bool GetCopyPendingStateImpl()
        {
            return false;
        }

        internal abstract string GetTagNameImpl();

        internal abstract HtmlAttributeId GetAttributeNameIdImpl(int attributeIndex);

        internal abstract HtmlAttributeParts GetAttributePartsImpl(int attributeIndex);

        internal abstract string GetAttributeNameImpl(int attributeIndex);

        

        internal abstract string GetAttributeValueImpl(int attributeIndex);

        internal abstract int ReadAttributeValueImpl(int attributeIndex, char[] buffer, int offset, int count);

        internal abstract void WriteTagImpl(bool writeAttributes);

        internal virtual void DeleteTagImpl()
        {
        }

        internal abstract void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue);

        
        

        internal void AssertAttributeValid(int attributeIndexAndCookie)
        {
            
            if (!valid)
            {
                throw new InvalidOperationException(Strings.AttributeNotValidInThisState);
            }

            

            if (ExtractCookie(attributeIndexAndCookie) != cookie)
            {
                throw new InvalidOperationException(Strings.AttributeNotValidForThisContext);
            }

            

            var index = ExtractIndex(attributeIndexAndCookie);

            if (index < 0 || index >= attributeCount)
            {
                throw new InvalidOperationException(Strings.AttributeNotValidForThisContext);
            }
        }

        internal void AssertContextValid()
        {
            
            if (!valid)
            {
                throw new InvalidOperationException(Strings.ContextNotValidInThisState);
            }
        }

        
        

        internal static byte ExtractCookie(int attributeIndexAndCookie)
        {
            return (byte)((uint)attributeIndexAndCookie >> 24);
        }

        internal static int ExtractIndex(int attributeIndexAndCookie)
        {
            InternalDebug.Assert((attributeIndexAndCookie & 0x00FFFFFF) - 1 >= -1);
            return (attributeIndexAndCookie & 0x00FFFFFF) - 1;
        }

        internal static int ComposeIndexAndCookie(byte cookie, int attributeIndex)
        {
            InternalDebug.Assert(attributeIndex >= -1);
            return ((int)cookie << 24) + (attributeIndex + 1);
        }

        

        
        
        
        public struct AttributeCollection : IEnumerable<HtmlTagContextAttribute>, System.Collections.IEnumerable
        {
            private HtmlTagContext tagContext;

            internal AttributeCollection(HtmlTagContext tagContext)
            {
                this.tagContext = tagContext;
            }

            
            
            
            public int Count
            {
                get
                {
                    AssertValid();
                    return tagContext.attributeCount;
                }
            }

            
            
            
            
            
            public HtmlTagContextAttribute this[int index]
            {
                get
                {
                    AssertValid();
                    
                    if (index < 0 || index >= tagContext.attributeCount)
                    {
                        throw new ArgumentOutOfRangeException("index");
                    }

                    return new HtmlTagContextAttribute(tagContext, ComposeIndexAndCookie(tagContext.cookie, index));
                }
            }

            
            
            
            
            public Enumerator GetEnumerator()
            {
                AssertValid();
                return new Enumerator(tagContext);
            }

            IEnumerator<HtmlTagContextAttribute> IEnumerable<HtmlTagContextAttribute>.GetEnumerator()
            {
                AssertValid();
                return new Enumerator(tagContext);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                AssertValid();
                return new Enumerator(tagContext);
            }

            private void AssertValid()
            {
                if (tagContext == null)
                {
                    throw new InvalidOperationException(Strings.AttributeCollectionNotInitialized);
                }
            }

            

            
            
            
            public struct Enumerator : IEnumerator<HtmlTagContextAttribute>, System.Collections.IEnumerator
            {
                private HtmlTagContext tagContext;
                private int attributeIndexAndCookie;

                internal Enumerator(HtmlTagContext tagContext)
                {
                    this.tagContext = tagContext;
                    
                    attributeIndexAndCookie = ComposeIndexAndCookie(this.tagContext.cookie, -1);
                }

                
                
                
                public void Dispose()
                {
                }

                
                
                
                public HtmlTagContextAttribute Current => new HtmlTagContextAttribute(tagContext, attributeIndexAndCookie);

                Object System.Collections.IEnumerator.Current => new HtmlTagContextAttribute(tagContext, attributeIndexAndCookie);


                public bool MoveNext()
                {
                    if (ExtractIndex(attributeIndexAndCookie) < tagContext.attributeCount)
                    {
                        attributeIndexAndCookie ++;

                        return ExtractIndex(attributeIndexAndCookie) < tagContext.attributeCount;
                    }

                    return false;
                }

                void System.Collections.IEnumerator.Reset()
                {
                    
                    attributeIndexAndCookie = ComposeIndexAndCookie(tagContext.cookie, -1);
                }
            }
        }
    }

    

    
    
    
    internal struct HtmlTagContextAttribute
    {
        private HtmlTagContext tagContext;
        private int attributeIndexAndCookie;

        internal HtmlTagContextAttribute(HtmlTagContext tagContext, int attributeIndexAndCookie)
        {
            this.tagContext = tagContext;
            this.attributeIndexAndCookie = attributeIndexAndCookie;
#if DEBUG
            AssertValid();
#endif
        }

        
        
        
        public static readonly HtmlTagContextAttribute Null = new HtmlTagContextAttribute();

        
        
        
        public bool IsNull => tagContext == null;


        public HtmlAttributeId Id { get { AssertValid(); return tagContext.GetAttributeNameIdImpl(HtmlTagContext.ExtractIndex(attributeIndexAndCookie)); } }

        
        
        
        public string Name { get { AssertValid(); return tagContext.GetAttributeNameImpl(HtmlTagContext.ExtractIndex(attributeIndexAndCookie)); } }
        
        
        
        
        public string Value { get { AssertValid(); return tagContext.GetAttributeValueImpl(HtmlTagContext.ExtractIndex(attributeIndexAndCookie)); } }    

        internal HtmlAttributeParts Parts { get { AssertValid(); return tagContext.GetAttributePartsImpl(HtmlTagContext.ExtractIndex(attributeIndexAndCookie)); } }

        
        
        
        
        
        
        
        public int ReadValue(char[] buffer, int offset, int count)
        {
            AssertValid();
            return tagContext.ReadAttributeValueImpl(HtmlTagContext.ExtractIndex(attributeIndexAndCookie), buffer, offset, count);
        }

        
        
        
        public void Write()
        {
            AssertValid();
            tagContext.WriteAttributeImpl(HtmlTagContext.ExtractIndex(attributeIndexAndCookie), true, true);
        }

        
        
        
        public void WriteName()
        {
            AssertValid();
            tagContext.WriteAttributeImpl(HtmlTagContext.ExtractIndex(attributeIndexAndCookie), true, false);
        }

        
        
        
        public void WriteValue()
        {
            AssertValid();
            tagContext.WriteAttributeImpl(HtmlTagContext.ExtractIndex(attributeIndexAndCookie), false, true);
        }

        private void AssertValid()
        {
            if (tagContext == null)
            {
                throw new InvalidOperationException(Strings.AttributeNotInitialized);
            }

            tagContext.AssertAttributeValid(attributeIndexAndCookie);
        }

        
        
        public override string ToString()
        {
            
            return tagContext == null ? "null" : HtmlTagContext.ExtractIndex(attributeIndexAndCookie).ToString();
        }
    }

#if M5STUFF


    
    
    
    
    public interface IHtmlParsingCallback
    {
        
        
        
        
        
        
        
        

        bool EvaluateConditional(string conditional);
    }

    
    

    
    
    
    
    public enum HtmlFilterAction
    {
        
        NoAction,
        
        Drop,
        
        DropContainerOnly,
        
        DropContainerAndContent,
        
        EmptyValue,
        
        ReplaceValue,
    }

    
    
    
    
    public struct HtmlFilterContextAction
    {
        
        public HtmlFilterContextType contextType;

        
        public HtmlNameId nameId;
        
        public string name;

        
        public HtmlNameId containerNameId;
        
        public string containerName;

        
        public HtmlFilterAction action;
        
        public string replacementValue;

        
        public bool callbackOverride;
    }

    
    
    
    
    
    
    
    public class HtmlFilterTables
    {
        
        
        
        
        

        public HtmlFilterTables(HtmlFilterContextAction[] staticActions, bool mergeWithDefault)
        {
        }

        
        
    }

    
    
    
    
    public interface IImageExtractionCallback
    {
        
        
        
        
        
        

        Stream CreateImage(string imageType, out string linkUrl);
    }

#endif 
}
