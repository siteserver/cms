// ***************************************************************
// <copyright file="HtmlWriter.cs" company="Microsoft">
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
    using Internal.Html;
    using Strings = CtsResources.TextConvertersStrings;

    

    
    
    
    internal enum HtmlWriterState
    {
        
        Default,
        
        Tag,
        
        Attribute,
    }

    

    
    
    
    internal class HtmlWriter : IRestartable, IFallback, IDisposable, ITextSinkEx
    {
        private ConverterOutput output;
        private OutputState outputState;

        private bool filterHtml;

        private bool autoNewLines;

        private bool allowWspBeforeFollowingTag;
        private bool lastWhitespace;

        private int lineLength;
        private int longestLineLength;
        private int textLineLength;

        private int literalWhitespaceNesting;
        private bool literalTags;
        private bool literalEntities;
        private bool cssEscaping;

        private IFallback fallback;

        private HtmlNameIndex tagNameIndex;
        private HtmlNameIndex previousTagNameIndex;
        private bool isEndTag;
        private bool isEmptyScopeTag;

        private bool copyPending;

        internal enum OutputState
        {
            OutsideTag,
            TagStarted,
            WritingUnstructuredTagContent,
            WritingTagName,
            BeforeAttribute,
            WritingAttributeName,
            AfterAttributeName,
            WritingAttributeValue,
        }

        
        
        
        
        
        
        public HtmlWriter(Stream output, Encoding outputEncoding)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            if (outputEncoding == null)
            {
                throw new ArgumentNullException("outputEncoding");
            }

            this.output = new ConverterEncodingOutput(
                                    output,
                                    true,       
                                    false,      
                                    outputEncoding,
                                    false,  
                                    false,
                                    null);
            autoNewLines = true;
        }

        
        
        
        
        
        public HtmlWriter(TextWriter output)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            this.output = new ConverterUnicodeOutput(
                                    output,
                                    true,       
                                    false);     
            autoNewLines = true;
        }

        
        internal HtmlWriter(ConverterOutput output, bool filterHtml, bool autoNewLines)
        {
            this.output = output;
            this.filterHtml = filterHtml;
            this.autoNewLines = autoNewLines;
        }

        
        internal bool HasEncoding => output is ConverterEncodingOutput;


        internal bool CodePageSameAsInput
        {
            get
            {
                InternalDebug.Assert(output is ConverterEncodingOutput);
                return (output as ConverterEncodingOutput).CodePageSameAsInput;
            }
        }

        
        internal Encoding Encoding
        {
            get
            {
                InternalDebug.Assert(output is ConverterEncodingOutput);
                return (output as ConverterEncodingOutput).Encoding;
            }

            set
            {
                InternalDebug.Assert(output is ConverterEncodingOutput);
                (output as ConverterEncodingOutput).Encoding = value;
            }
        }

        
        internal bool CanAcceptMore => output.CanAcceptMore;


        internal bool IsTagOpen => outputState != OutputState.OutsideTag;


        internal int LineLength => lineLength;


        internal int LiteralWhitespaceNesting => literalWhitespaceNesting;


        public HtmlWriterState WriterState => outputState == OutputState.OutsideTag ? HtmlWriterState.Default :
            outputState < OutputState.WritingAttributeName ? HtmlWriterState.Tag :
                HtmlWriterState.Attribute;


        internal bool IsCopyPending => copyPending;


        public void Flush()
        {
            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }

            output.Flush();
        }

        
        internal void SetCopyPending(bool copyPending)
        {
            this.copyPending = copyPending;
        }

        
        
        
        
        
        public void WriteTag(HtmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            if (reader.TagId != HtmlTagId.Unknown)
            {
                WriteTagBegin(HtmlNameData.tagIndex[(int)reader.TagId], null, reader.TokenKind == HtmlTokenKind.EndTag, false, false);
            }
            else
            {
                WriteTagBegin(HtmlNameIndex.Unknown, null, reader.TokenKind == HtmlTokenKind.EndTag, false, false);
                reader.WriteTagNameTo(WriteTagName());
            }

            isEmptyScopeTag = (reader.TokenKind == HtmlTokenKind.EmptyElementTag);

            if (reader.TokenKind == HtmlTokenKind.StartTag ||
                reader.TokenKind == HtmlTokenKind.EmptyElementTag)
            {
                var attrReader = reader.AttributeReader;

                while (attrReader.ReadNext())
                {
                    if (attrReader.Id != HtmlAttributeId.Unknown)
                    {
                        OutputAttributeName(HtmlNameData.names[(int)HtmlNameData.attributeIndex[(int)attrReader.Id]].name);
                    }
                    else
                    {
                        attrReader.WriteNameTo(WriteAttributeName());
                    }

                    if (attrReader.HasValue)
                    {
                        attrReader.WriteValueTo(WriteAttributeValue());
                    }

                    OutputAttributeEnd();

                    outputState = OutputState.BeforeAttribute;
                }
            }
        }

        
        
        
        
        
        public void WriteStartTag(HtmlTagId id)
        {
            WriteTag(id, false);
        }

        
        
        
        
        
        public void WriteStartTag(string name)
        {
            WriteTag(name, false);
        }

        
        
        
        
        
        public void WriteEndTag(HtmlTagId id)
        {
            WriteTag(id, true);
            WriteTagEnd();             
        }

        
        
        
        
        
        public void WriteEndTag(string name)
        {
            WriteTag(name, true);
            WriteTagEnd();             
        }

        
        
        
        
        
        public void WriteEmptyElementTag(HtmlTagId id)
        {
            WriteTag(id, false);
            isEmptyScopeTag = true;
        }

        
        
        
        
        
        public void WriteEmptyElementTag(string name)
        {
            WriteTag(name, false);
            isEmptyScopeTag = true;
        }

        
        private void WriteTag(HtmlTagId id, bool isEndTag)
        {
            if ((int)id < 0 || (int)id >= HtmlNameData.tagIndex.Length)
            {
                throw new ArgumentException(Strings.TagIdInvalid, "id");
            }

            if (id == HtmlTagId.Unknown)
            {
                throw new ArgumentException(Strings.TagIdIsUnknown, "id");
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            WriteTagBegin(HtmlNameData.tagIndex[(int)id], null, isEndTag, false, false);
        }

        
        private void WriteTag(string name, bool isEndTag)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0)
            {
                throw new ArgumentException(Strings.TagNameIsEmpty, "name");
            }

            

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            var nameIndex = LookupName(name);

            if (nameIndex != HtmlNameIndex.Unknown)
            {
                
                name = null;
            }

            WriteTagBegin(nameIndex, name, isEndTag, false, false);
        }

        
        internal void WriteStartTag(HtmlNameIndex nameIndex)
        {
            WriteTagBegin(nameIndex, null, false, false, false);
        }

        
        internal void WriteEndTag(HtmlNameIndex nameIndex)
        {
            WriteTagBegin(nameIndex, null, true, false, false);
            WriteTagEnd();             
        }

        
        internal void WriteEmptyElementTag(HtmlNameIndex nameIndex)
        {
            WriteTagBegin(nameIndex, null, true, false, false);
            isEmptyScopeTag = true;
        }

        
        internal void WriteTagBegin(HtmlNameIndex nameIndex, string name, bool isEndTag, bool allowWspLeft, bool allowWspRight)
        {
            
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }
#if !BETTER_FUZZING
            
            
            
            
            
            
            
            
            if (literalTags && nameIndex >= HtmlNameIndex.Unknown && (!isEndTag || nameIndex != tagNameIndex))
            {
                throw new InvalidOperationException(Strings.CannotWriteOtherTagsInsideElement(HtmlNameData.names[(int)tagNameIndex].name));
            }
#endif
            var tagIndex = HtmlNameData.names[(int)nameIndex].tagIndex;

            if (nameIndex > HtmlNameIndex.Unknown)
            {
                

                isEmptyScopeTag = (HtmlDtd.tags[(int)tagIndex].scope == HtmlDtd.TagScope.EMPTY);

                if (isEndTag && isEmptyScopeTag)
                {
                    
                    
                    

                    if (HtmlDtd.tags[(int)tagIndex].unmatchedSubstitute != HtmlTagIndex._IMPLICIT_BEGIN)
                    {
                        

                        output.Write("<!-- </");
                        lineLength += 7;
                        if (nameIndex > HtmlNameIndex.Unknown)
                        {
                            output.Write(HtmlNameData.names[(int)nameIndex].name);
                            lineLength += HtmlNameData.names[(int)nameIndex].name.Length;
                        }
                        else
                        {
                            output.Write(name != null ? name : "???");
                            lineLength += name != null ? name.Length : 3;
                        }
                        output.Write("> ");
                        lineLength += 2;

                        
                        
                        
                        tagNameIndex = HtmlNameIndex._COMMENT;
                        outputState = OutputState.WritingUnstructuredTagContent;

                        return;
                    }

                    
                    

                    isEndTag = false;
                }
            }

            
            InternalDebug.Assert(0 == (HtmlDtd.tags[(int)tagIndex].literal & HtmlDtd.Literal.Entities) ||
                                0 != (HtmlDtd.tags[(int)tagIndex].literal & HtmlDtd.Literal.Tags));

            if (autoNewLines && literalWhitespaceNesting == 0)
            {
                var hadWhitespaceBeforeTag = lastWhitespace;
                var tagFill = HtmlDtd.tags[(int)tagIndex].fill;

                if (lineLength != 0)
                {
                    
                    
                    

                    var tagFmt = HtmlDtd.tags[(int)tagIndex].fmt;

                    if ((!isEndTag && tagFmt.LB == HtmlDtd.FmtCode.BRK) ||
                        (isEndTag && tagFmt.LE == HtmlDtd.FmtCode.BRK) ||
                        (lineLength > 80 &&
                        (lastWhitespace ||
                        allowWspBeforeFollowingTag ||
                        (!isEndTag && tagFill.LB == HtmlDtd.FillCode.EAT) ||
                        (isEndTag && tagFill.LE == HtmlDtd.FillCode.EAT))))
                    {
                        if (lineLength > longestLineLength)
                        {
                            longestLineLength = lineLength;
                        }

                        output.Write("\r\n");
                        lineLength = 0;
                        lastWhitespace = false;
                    }
                }

                allowWspBeforeFollowingTag = ((!isEndTag && tagFill.RB == HtmlDtd.FillCode.EAT) ||
                                                (isEndTag && tagFill.RE == HtmlDtd.FillCode.EAT) ||
                                                hadWhitespaceBeforeTag &&
                                                ((!isEndTag && tagFill.RB == HtmlDtd.FillCode.NUL) ||
                                                (isEndTag && tagFill.RE == HtmlDtd.FillCode.NUL))) &&
                                                (nameIndex != HtmlNameIndex.Body || !isEndTag);
            }

            if (lastWhitespace)
            {
                
                output.Write(' ');
                lineLength++;
                lastWhitespace = false;
            }

            if (HtmlDtd.tags[(int)tagIndex].blockElement || tagIndex == HtmlTagIndex.BR)
            {
                textLineLength = 0;
            }

            output.Write('<');
            lineLength++;

            if (nameIndex >= HtmlNameIndex.Unknown)
            {
                if (isEndTag)
                {
                    
                    if (0 != (HtmlDtd.tags[(int)tagIndex].literal & HtmlDtd.Literal.Tags))
                    {
                        literalTags = false;
                        
                        literalEntities = false;
                        cssEscaping = false;
                    }

                    if (HtmlDtd.tags[(int)tagIndex].contextTextType == HtmlDtd.ContextTextType.Literal)
                    {
                        literalWhitespaceNesting--;
                    }

                    output.Write('/');
                    lineLength++;
                }

                if (nameIndex != HtmlNameIndex.Unknown)
                {
                    output.Write(HtmlNameData.names[(int)nameIndex].name);
                    lineLength += HtmlNameData.names[(int)nameIndex].name.Length;

                    outputState = OutputState.BeforeAttribute;
                }
                else
                {
                    if (name != null)
                    {
                        output.Write(name);
                        lineLength += name.Length;

                        outputState = OutputState.BeforeAttribute;
                    }
                    else
                    {
                        
                        outputState = OutputState.TagStarted;
                    }

                    
                    
                    isEmptyScopeTag = false;
                }
            }
            else
            {
                previousTagNameIndex = tagNameIndex;

                if (nameIndex == HtmlNameIndex._COMMENT)
                {
                    output.Write("!--");
                    lineLength += 3;
                }
                else if (nameIndex == HtmlNameIndex._ASP)
                {
                    output.Write('%');
                    lineLength++;
                }
                else if (nameIndex == HtmlNameIndex._CONDITIONAL)
                {
                    output.Write("!--[");
                    lineLength += 4;
                }
                else if (nameIndex == HtmlNameIndex._DTD)
                {
                    output.Write('?');
                    lineLength++;
                }
                else 
                {
                    output.Write('!');
                    lineLength++;
                }

                
                outputState = OutputState.WritingUnstructuredTagContent;

                
                isEmptyScopeTag = true;
            }

            tagNameIndex = nameIndex;
            this.isEndTag = isEndTag;
        }

        
        internal void WriteTagEnd()
        {
            WriteTagEnd(isEmptyScopeTag);
        }

        
        internal void WriteTagEnd(bool emptyScopeTag)
        {
            InternalDebug.Assert(outputState != OutputState.OutsideTag);
            InternalDebug.Assert(!lastWhitespace);

            

            var tagIndex = HtmlNameData.names[(int)tagNameIndex].tagIndex;

            if (outputState > OutputState.BeforeAttribute)
            {
                

                OutputAttributeEnd();
            }

            if (tagNameIndex > HtmlNameIndex.Unknown)
            {
                output.Write('>');
                lineLength++;
            }
            else
            {
                if (tagNameIndex == HtmlNameIndex._COMMENT)
                {
                    output.Write("-->");
                    lineLength += 3;
                }
                else if (tagNameIndex == HtmlNameIndex._ASP)
                {
                    output.Write("%>");
                    lineLength += 2;
                }
                else if (tagNameIndex == HtmlNameIndex._CONDITIONAL)
                {
                    output.Write("]-->");
                    lineLength += 4;
                }
                else if (tagNameIndex == HtmlNameIndex.Unknown && emptyScopeTag)
                {
                    output.Write(" />");
                    lineLength += 3;
                }
                else
                {
                    output.Write('>');
                    lineLength++;
                }

                
                

                tagNameIndex = previousTagNameIndex;
            }

            if (isEndTag &&
                (tagIndex == HtmlTagIndex.LI || tagIndex == HtmlTagIndex.DD || tagIndex == HtmlTagIndex.DT))
            {
                
                
                
                
                
                lineLength = 0;
            }

            if (autoNewLines && literalWhitespaceNesting == 0)
            {
                
                
                

                var tagFmt = HtmlDtd.tags[(int)tagIndex].fmt;
                var tagFill = HtmlDtd.tags[(int)tagIndex].fill;

                if ((!isEndTag && tagFmt.RB == HtmlDtd.FmtCode.BRK) ||
                    (isEndTag && tagFmt.RE == HtmlDtd.FmtCode.BRK) ||
                        (lineLength > 80 &&
                        (allowWspBeforeFollowingTag ||
                        (!isEndTag && tagFill.RB == HtmlDtd.FillCode.EAT) ||
                        (isEndTag && tagFill.RE == HtmlDtd.FillCode.EAT))))
                {
                    if (lineLength > longestLineLength)
                    {
                        longestLineLength = lineLength;
                    }

                    output.Write("\r\n");
                    lineLength = 0;
                }
            }

            if (!isEndTag && !emptyScopeTag)
            {
                

                var literal = HtmlDtd.tags[(int)tagIndex].literal;

                if (0 != (literal & HtmlDtd.Literal.Tags))
                {
                    literalTags = true;
                    literalEntities = (0 != (literal & HtmlDtd.Literal.Entities));
                    cssEscaping = (tagIndex == HtmlTagIndex.Style);
                }

                if (HtmlDtd.tags[(int)tagIndex].contextTextType == HtmlDtd.ContextTextType.Literal)
                {
                    literalWhitespaceNesting++;
                }
            }

            outputState = OutputState.OutsideTag;
        }

        
        
        
        
        
        
        public void WriteAttribute(HtmlAttributeId id, string value)
        {
            if ((int)id < 0 || (int)id >= HtmlNameData.attributeIndex.Length)
            {
                throw new ArgumentException(Strings.AttributeIdInvalid, "id");
            }

            if (id == HtmlAttributeId.Unknown)
            {
                throw new ArgumentException(Strings.AttributeIdIsUnknown, "id");
            }

            if (outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (outputState > OutputState.BeforeAttribute)
            {
                OutputAttributeEnd();
            }

            OutputAttributeName(HtmlNameData.names[(int)HtmlNameData.attributeIndex[(int)id]].name);

            if (value != null)
            {
                OutputAttributeValue(value);
                OutputAttributeEnd();
            }

            
            outputState = OutputState.BeforeAttribute;
        }

        
        
        
        
        
        
        public void WriteAttribute(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0)
            {
                throw new ArgumentException(Strings.AttributeNameIsEmpty, "name");
            }

            

            
            if (outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (outputState > OutputState.BeforeAttribute)
            {
                OutputAttributeEnd();
            }

            OutputAttributeName(name);

            if (value != null)
            {
                OutputAttributeValue(value);
                OutputAttributeEnd();
            }

            
            outputState = OutputState.BeforeAttribute;
        }

        
        
        
        
        
        
        
        public void WriteAttribute(HtmlAttributeId id, char[] buffer, int index, int count)
        {
            if ((int)id < 0 || (int)id >= HtmlNameData.attributeIndex.Length)
            {
                throw new ArgumentException(Strings.AttributeIdInvalid, "id");
            }

            if (id == HtmlAttributeId.Unknown)
            {
                throw new ArgumentException(Strings.AttributeIdIsUnknown, "id");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (count < 0 || count > buffer.Length - index)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (outputState > OutputState.BeforeAttribute)
            {
                OutputAttributeEnd();
            }

            OutputAttributeName(HtmlNameData.names[(int)HtmlNameData.attributeIndex[(int)id]].name);

            OutputAttributeValue(buffer, index, count);

            OutputAttributeEnd();

            
            outputState = OutputState.BeforeAttribute;
        }

        
        
        
        
        
        
        
        
        public void WriteAttribute(string name, char[] buffer, int index, int count)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0)
            {
                throw new ArgumentException(Strings.AttributeNameIsEmpty, "name");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (count < 0 || count > buffer.Length - index)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (outputState > OutputState.BeforeAttribute)
            {
                OutputAttributeEnd();
            }

            OutputAttributeName(name);

            OutputAttributeValue(buffer, index, count);

            OutputAttributeEnd();

            
            outputState = OutputState.BeforeAttribute;
        }

        
        internal void WriteAttribute(HtmlNameIndex nameIndex, string value)
        {
            InternalDebug.Assert(nameIndex > HtmlNameIndex.Unknown && (int)nameIndex < HtmlNameData.names.Length);
            InternalDebug.Assert(outputState >= OutputState.WritingTagName);
            InternalDebug.Assert(!isEndTag);

            
            if (outputState > OutputState.BeforeAttribute)
            {
                OutputAttributeEnd();
            }

            OutputAttributeName(HtmlNameData.names[(int)nameIndex].name);

            if (value != null)
            {
                OutputAttributeValue(value);
                OutputAttributeEnd();
            }

            outputState = OutputState.BeforeAttribute;
        }

        internal void WriteAttribute(HtmlNameIndex nameIndex, BufferString value)
        {
            InternalDebug.Assert(nameIndex > HtmlNameIndex.Unknown && (int)nameIndex < HtmlNameData.names.Length);
            InternalDebug.Assert(outputState >= OutputState.WritingTagName);
            InternalDebug.Assert(!isEndTag);

            
            if (outputState > OutputState.BeforeAttribute)
            {
                OutputAttributeEnd();
            }

            OutputAttributeName(HtmlNameData.names[(int)nameIndex].name);
            OutputAttributeValue(value.Buffer, value.Offset, value.Length);
            OutputAttributeEnd();

            outputState = OutputState.BeforeAttribute;
        }

        
        
        
        
        
        public void WriteAttribute(HtmlAttributeReader attributeReader)
        {
            if (outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            attributeReader.WriteNameTo(WriteAttributeName());
            if (attributeReader.HasValue)
            {
                attributeReader.WriteValueTo(WriteAttributeValue());
            }
            OutputAttributeEnd();

            outputState = OutputState.BeforeAttribute;
        }

        
        
        
        
        
        
        public void WriteAttributeName(HtmlAttributeId id)
        {
            if ((int)id < 0 || (int)id >= HtmlNameData.attributeIndex.Length)
            {
                throw new ArgumentException(Strings.AttributeIdInvalid, "id");
            }

            if (id == HtmlAttributeId.Unknown)
            {
                throw new ArgumentException(Strings.AttributeIdIsUnknown, "id");
            }

            if (outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (outputState > OutputState.BeforeAttribute)
            {
                OutputAttributeEnd();
            }

            OutputAttributeName(HtmlNameData.names[(int)HtmlNameData.attributeIndex[(int)id]].name);
        }

        
        
        
        
        
        
        public void WriteAttributeName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0)
            {
                throw new ArgumentException(Strings.AttributeNameIsEmpty, "name");
            }

            if (outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (outputState > OutputState.BeforeAttribute)
            {
                OutputAttributeEnd();
            }

            OutputAttributeName(name);
        }


        
        internal void WriteAttributeName(HtmlNameIndex nameIndex)
        {
            InternalDebug.Assert(nameIndex > HtmlNameIndex.Unknown && (int)nameIndex < HtmlNameData.names.Length);
            InternalDebug.Assert(outputState >= OutputState.WritingTagName);
            InternalDebug.Assert(!isEndTag);

            
            if (outputState > OutputState.BeforeAttribute)
            {
                OutputAttributeEnd();
            }

            OutputAttributeName(HtmlNameData.names[(int)nameIndex].name);
        }

        
        
        
        
        
        public void WriteAttributeName(HtmlAttributeReader attributeReader)
        {
            if (outputState < OutputState.WritingTagName)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (isEndTag)
            {
                throw new InvalidOperationException(Strings.EndTagCannotHaveAttributes);
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            attributeReader.WriteNameTo(WriteAttributeName());
        }

        
        
        
        
        
        
        
        public void WriteAttributeValue(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (outputState < OutputState.TagStarted)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (outputState < OutputState.WritingAttributeName)
            {
                throw new InvalidOperationException(Strings.AttributeNotStarted);
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            OutputAttributeValue(value);
        }

        internal void WriteAttributeValue(BufferString value)
        {
            InternalDebug.Assert(!copyPending && outputState >= OutputState.WritingAttributeName);

            OutputAttributeValue(value.Buffer, value.Offset, value.Length);
        }

        
        
        
        
        
        
        
        
        
        public void WriteAttributeValue(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (count < 0 || count > buffer.Length - index)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (outputState < OutputState.TagStarted)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (outputState < OutputState.WritingAttributeName)
            {
                throw new InvalidOperationException(Strings.AttributeNotStarted);
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            OutputAttributeValue(buffer, index, count);
        }

        
        
        
        
        
        public void WriteAttributeValue(HtmlAttributeReader attributeReader)
        {
            if (outputState < OutputState.TagStarted)
            {
                throw new InvalidOperationException(Strings.TagNotStarted);
            }

            if (outputState < OutputState.WritingAttributeName)
            {
                throw new InvalidOperationException(Strings.AttributeNotStarted);
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            if (attributeReader.HasValue)
            {
                attributeReader.WriteValueTo(WriteAttributeValue());
            }
        }

        
        internal void WriteAttributeValueInternal(string value)
        {
            InternalDebug.Assert(value != null);
            InternalDebug.Assert(outputState >= OutputState.WritingAttributeName);

            OutputAttributeValue(value);
        }

        
        internal void WriteAttributeValueInternal(char[] buffer, int index, int count)
        {
            InternalDebug.Assert(buffer != null && index >= 0 && index < buffer.Length && count >= 0 && count <= buffer.Length - index);
            InternalDebug.Assert(outputState >= OutputState.WritingAttributeName);

            OutputAttributeValue(buffer, index, count);
        }

        
        
        
        
        
        public void WriteText(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }

            if (value.Length != 0)
            {
                if (lastWhitespace)
                {
                    OutputLastWhitespace(value[0]);
                }

                

                output.Write(value, this);
                lineLength += value.Length;
                textLineLength += value.Length;

                allowWspBeforeFollowingTag = false;
            }
        }

        
        
        
        
        
        
        
        public void WriteText(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (count < 0 || count > buffer.Length - index)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }

            

            WriteTextInternal(buffer, index, count);
        }

        
        internal void WriteText(char ch)
        {
            InternalDebug.Assert(!copyPending);

            
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }

            if (lastWhitespace)
            {
                OutputLastWhitespace(ch);
            }

            output.Write(ch, this);
            lineLength++;
            textLineLength++;
            allowWspBeforeFollowingTag = false;
        }

        
        
        
        
        
        public void WriteText(HtmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            reader.WriteTextTo(WriteText());
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        public void WriteMarkupText(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }

            if (lastWhitespace)
            {
                OutputLastWhitespace(value[0]);
            }

            

            output.Write(value, null);
            lineLength += value.Length;
            
            allowWspBeforeFollowingTag = false;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public void WriteMarkupText(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (count < 0 || count > buffer.Length - index)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }

            if (lastWhitespace)
            {
                OutputLastWhitespace(buffer[index]);
            }

            

            output.Write(buffer, index, count, null);
            lineLength += count;
            
            allowWspBeforeFollowingTag = false;
        }


        
        internal void WriteMarkupText(char ch)
        {
            
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }

            if (lastWhitespace)
            {
                OutputLastWhitespace(ch);
            }

            output.Write(ch, null);
            lineLength++;
            
            allowWspBeforeFollowingTag = false;
        }

        
        
        
        
        
        
        public void WriteMarkupText(HtmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (copyPending)
            {
                throw new InvalidOperationException(Strings.CannotWriteWhileCopyPending);
            }

            reader.WriteMarkupTextTo(WriteMarkupText());
        }

        
        internal ITextSinkEx WriteUnstructuredTagContent()
        {
            InternalDebug.Assert(outputState == OutputState.WritingUnstructuredTagContent);

            fallback = null;
            return this;
        }

        
        internal ITextSinkEx WriteTagName()
        {
            InternalDebug.Assert(outputState == OutputState.TagStarted ||
                                outputState == OutputState.WritingTagName);

            outputState = OutputState.WritingTagName;

            fallback = null;
            return this;
        }

        
        internal ITextSinkEx WriteAttributeName()
        {
            InternalDebug.Assert(outputState >= OutputState.WritingTagName);

            if (outputState != OutputState.WritingAttributeName)
            {
                if (outputState > OutputState.BeforeAttribute)
                {
                    OutputAttributeEnd();
                }

                
                

#if false



                if (this.lineLength > 255 && this.autoNewLines)
                {
                    
                    

                    if (this.lineLength > this.longestLineLength)
                    {
                        this.longestLineLength = this.lineLength;
                    }

                    this.output.Write("\r\n");
                    this.lineLength = 0;
                }
#endif
                output.Write(' ');     
                lineLength++;
            }

            outputState = OutputState.WritingAttributeName;

            fallback = null;
            return this;
        }

        
        internal ITextSinkEx WriteAttributeValue()
        {
            InternalDebug.Assert(outputState >= OutputState.WritingAttributeName);

            if (outputState != OutputState.WritingAttributeValue)
            {
                output.Write("=\"");
                lineLength += 2;
            }

            outputState = OutputState.WritingAttributeValue;

            fallback = this;
            return this;
        }

        
        internal ITextSinkEx WriteText()
        {
            
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }

            allowWspBeforeFollowingTag = false;

            if (lastWhitespace)
            {
                
                InternalDebug.Assert(ParseSupport.FarEastNonHanguelChar('\x3000'));
                OutputLastWhitespace('\x3000');
            }

            fallback = this;
            return this;
        }

        
        internal ITextSinkEx WriteMarkupText()
        {
            
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }

            if (lastWhitespace)
            {
                output.Write(' ');
                lineLength++;
                lastWhitespace = false;
            }

            fallback = null;
            return this;
        }

        
        internal void WriteNewLine()
        {
            WriteNewLine(false);
        }

        
        internal void WriteNewLine(bool optional)
        {
            
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }

            if (!optional || (lineLength != 0 && literalWhitespaceNesting == 0))
            {
                if (lineLength > longestLineLength)
                {
                    longestLineLength = lineLength;
                }

                output.Write("\r\n");
                lineLength = 0;
                lastWhitespace = false;
                allowWspBeforeFollowingTag = false;
            }
        }

        
        internal void WriteAutoNewLine()
        {
            WriteNewLine(false);
        }

        
        internal void WriteAutoNewLine(bool optional)
        {
            
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }

            if (autoNewLines && (!optional || (lineLength != 0 && literalWhitespaceNesting == 0)))
            {
                if (lineLength > longestLineLength)
                {
                    longestLineLength = lineLength;
                }

                output.Write("\r\n");
                lineLength = 0;
                lastWhitespace = false;
                allowWspBeforeFollowingTag = false;
            }
        }

        
        internal void WriteTabulation(int count)
        {
            WriteSpace((textLineLength / 8 * 8 + 8 * count) - textLineLength);
        }

        
        internal void WriteSpace(int count)
        {
            InternalDebug.Assert(outputState == OutputState.OutsideTag);

            if (literalWhitespaceNesting == 0)
            {
                if (lineLength == 0 && count == 1)
                {
                    
                    
                    output.Write('\xA0', this);
                    return;
                }

                if (lastWhitespace)
                {
                    lineLength++;
                    output.Write('\xA0', this);
                }

                lineLength += count - 1;
                textLineLength += count - 1;

                while (0 != --count)
                {
                    output.Write('\xA0', this);
                }

                lastWhitespace = true;
                allowWspBeforeFollowingTag = false;
            }
            else
            {
                while (0 != count--)
                {
                    output.Write(' ');
                }

                lineLength += count;
                textLineLength += count;

                lastWhitespace = false;
                allowWspBeforeFollowingTag = false;
            }
        }

        
        internal void WriteNbsp(int count)
        {
            InternalDebug.Assert(outputState == OutputState.OutsideTag);

            if (lastWhitespace)
            {
                OutputLastWhitespace('\xA0');
            }

            lineLength += count;
            textLineLength += count;
            while (0 != count--)
            {
                output.Write('\xA0', this);
            }

            allowWspBeforeFollowingTag = false;
        }

        
        internal void WriteTextInternal(char[] buffer, int index, int count)
        {
            InternalDebug.Assert(buffer != null);
            InternalDebug.Assert(index >= 0 && index <= buffer.Length);
            InternalDebug.Assert(count >= 0 && count <= buffer.Length - index);
            InternalDebug.Assert(!copyPending);

            InternalDebug.Assert(outputState == OutputState.OutsideTag);

            if (count != 0)
            {
                if (lastWhitespace)
                {
                    OutputLastWhitespace(buffer[index]);
                }

                InternalDebug.Assert(!lastWhitespace);

                output.Write(buffer, index, count, this);
                lineLength += count;
                textLineLength += count;

                allowWspBeforeFollowingTag = false;
            }
        }

        
        internal void StartTextChunk()
        {
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }

            lastWhitespace = false;
        }

        
        internal void EndTextChunk()
        {
            if (lastWhitespace)
            {
                OutputLastWhitespace('\n');
            }
        }

        
        internal void WriteCollapsedWhitespace()
        {
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }

            lastWhitespace = true;
            allowWspBeforeFollowingTag = false;
        }

        
        private void OutputLastWhitespace(char nextChar)
        {
            if (lineLength > 255 && autoNewLines)
            {
                if (lineLength > longestLineLength)
                {
                    longestLineLength = lineLength;
                }

                output.Write("\r\n");
                lineLength = 0;

                if (ParseSupport.FarEastNonHanguelChar(nextChar))
                {
                    
                    

                    output.Write(' ');
                    lineLength++;
                }
            }
            else
            {
                output.Write(' ');
                lineLength++;
            }

            textLineLength++;
            lastWhitespace = false;
        }

        
        private void OutputAttributeName(string name)
        {
            
#if false



            if (this.lineLength > 255 && this.autoNewLines)
            {
                
                

                if (this.lineLength > this.longestLineLength)
                {
                    this.longestLineLength = this.lineLength;
                }

                this.output.Write("\r\n");
                this.lineLength = 0;
            }
#endif
            output.Write(' ');     
            output.Write(name);
            lineLength += name.Length + 1;

            outputState = OutputState.AfterAttributeName;
        }

        
        private void OutputAttributeValue(string value)
        {
            InternalDebug.Assert(outputState > OutputState.BeforeAttribute);

            if (outputState < OutputState.WritingAttributeValue)
            {
                output.Write("=\"");
                lineLength += 2;
            }

            output.Write(value, this);
            lineLength += value.Length;

            outputState = OutputState.WritingAttributeValue;
        }

        
        private void OutputAttributeValue(char[] value, int index, int count)
        {
            InternalDebug.Assert(outputState > OutputState.BeforeAttribute);

            if (outputState < OutputState.WritingAttributeValue)
            {
                output.Write("=\"");
                lineLength += 2;
            }

            output.Write(value, index, count, this);
            lineLength += count;

            outputState = OutputState.WritingAttributeValue;
        }

        
        private void OutputAttributeEnd()
        {
            InternalDebug.Assert(outputState > OutputState.BeforeAttribute);

            if (outputState < OutputState.WritingAttributeValue)
            {
                output.Write("=\"");
                lineLength += 2;
            }

            
            output.Write('\"');
            lineLength++;
        }

        
        internal static HtmlNameIndex LookupName(string name)
        {
            if (name.Length <= HtmlNameData.MAX_NAME)
            {
                var hash = (short)(((uint)HashCode.CalculateLowerCase(name) ^ HtmlNameData.NAME_HASH_MODIFIER) % HtmlNameData.NAME_HASH_SIZE);

                

                var nameIndex = (int)HtmlNameData.nameHashTable[hash];

                if (nameIndex > 0)
                {
                    do
                    {
                        

                        var currentName = HtmlNameData.names[nameIndex].name;

                        if (currentName.Length == name.Length)
                        {
                            if (currentName[0] == ParseSupport.ToLowerCase(name[0]))
                            {
                                if (name.Length == 1 || currentName.Equals(name, StringComparison.OrdinalIgnoreCase))
                                {
                                    return (HtmlNameIndex)nameIndex;
                                }
                            }
                        }

                        
                        
                    }
                    while (HtmlNameData.names[++nameIndex].hash == hash);
                }
            }

            return HtmlNameIndex.Unknown;
        }

        
        bool IRestartable.CanRestart()
        {
            if (output is IRestartable)
            {
                return ((IRestartable)output).CanRestart();
            }

            return false;
        }

        
        void IRestartable.Restart()
        {
            if (output is IRestartable)
            {
                ((IRestartable)output).Restart();
            }

            

            allowWspBeforeFollowingTag = false;
            lastWhitespace = false;
            lineLength = 0;
            longestLineLength = 0;

            literalWhitespaceNesting = 0;
            literalTags = false;
            literalEntities = false;
            cssEscaping = false;

            tagNameIndex = HtmlNameIndex._NOTANAME;
            previousTagNameIndex = HtmlNameIndex._NOTANAME;

            isEndTag = false;
            isEmptyScopeTag = false;
            copyPending = false;

            outputState = OutputState.OutsideTag;
        }

        
        void IRestartable.DisableRestart()
        {
            if (output is IRestartable)
            {
                ((IRestartable)output).DisableRestart();
            }
        }

        
        byte[] IFallback.GetUnsafeAsciiMap(out byte unsafeAsciiMask)
        {
            if (literalEntities)
            {
                unsafeAsciiMask = 0x00;
                return null;
            }

            if (filterHtml)
            {
                
                unsafeAsciiMask = 0x01;
            }
            else
            {
                unsafeAsciiMask = 0x01;
            }

            return HtmlSupport.UnsafeAsciiMap;
        }

        
        bool IFallback.HasUnsafeUnicode()
        {
            return filterHtml;
        }

        bool IFallback.TreatNonAsciiAsUnsafe(string charset)
        {
            return filterHtml && charset.StartsWith("x-", StringComparison.OrdinalIgnoreCase);
        }
        
        bool IFallback.IsUnsafeUnicode(char ch, bool isFirstChar)
        {
            return filterHtml &&
                ((byte)(ch & 0xFF) == (byte)'<' ||
                (byte)((ch >> 8) & 0xFF) == (byte)'<' ||
                
                
                (!isFirstChar && ch == '\uFEFF') ||
                
                
                
                Char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.PrivateUse);
        }

        
        bool IFallback.FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int outputEnd)
        {
            if (literalEntities)
            {
                

                if (cssEscaping)
                {
                    

                    var value = (uint)ch;
                    var len = (value < 0x10) ? 1 : (value < 0x100) ? 2 : (value < 0x1000) ? 3 : 4;
                    if (outputEnd - outputBufferCount < len + 2)
                    {
                        return false;
                    }

                    outputBuffer[outputBufferCount++] = '\\';

                    var offset = outputBufferCount + len;
                    while (value != 0)
                    {
                        var digit = value & 0xF;
                        outputBuffer[--offset] = (char)(digit + (digit < 10 ? '0' : 'A' - 10));
                        value >>= 4;
                    }
                    outputBufferCount += len;

                    outputBuffer[outputBufferCount++] = ' ';
                }
                else
                {
                    

                    
                    
                    

                    if (outputEnd - outputBufferCount < 1)
                    {
                        return false;
                    }

                    outputBuffer[outputBufferCount++] = filterHtml ? '?' : ch;
                }
            }
            else
            {
                

                
                
                
                
                
                

                HtmlEntityIndex namedEntityId = 0;

                if (ch <= '>')
                {
                    if (ch == '>')
                    {
                        namedEntityId = HtmlEntityIndex.gt;
                    }
                    else if (ch == '<')
                    {
                        namedEntityId = HtmlEntityIndex.lt;
                    }
                    else if (ch == '&')
                    {
                        namedEntityId = HtmlEntityIndex.amp;
                    }
                    else if (ch == '\"')
                    {
                        namedEntityId = HtmlEntityIndex.quot;
                    }
                }
                else if ((char)0xA0 <= ch && ch <= (char)0xFF)
                {
                    namedEntityId = HtmlSupport.EntityMap[(int)ch - 0xA0];
                }

                if ((int)namedEntityId != 0)
                {
                    

                    var strQuote = HtmlNameData.entities[(int)namedEntityId].name;

                    if (outputEnd - outputBufferCount < strQuote.Length + 2)
                    {
                        return false;
                    }

                    outputBuffer[outputBufferCount++] = '&';
                    strQuote.CopyTo(0, outputBuffer, outputBufferCount, strQuote.Length);
                    outputBufferCount += strQuote.Length;
                    outputBuffer[outputBufferCount++] = ';';
                }
                else
                {
                    

                    var value = (uint)ch;
                    var len = (value < 10) ? 1 : (value < 100) ? 2 : (value < 1000) ? 3 : (value < 10000) ? 4 : 5;
                    if (outputEnd - outputBufferCount < len + 3)
                    {
                        return false;
                    }

                    outputBuffer[outputBufferCount++] = '&';
                    outputBuffer[outputBufferCount++] = '#';

                    var offset = outputBufferCount + len;
                    while (value != 0)
                    {
                        var digit = value % 10;
                        outputBuffer[--offset] = (char)(digit + '0');
                        value /= 10;
                    }
                    outputBufferCount += len;

                    outputBuffer[outputBufferCount++] = ';';
                }
            }

            return true;
        }

        
        bool ITextSink.IsEnough => false;

        void ITextSink.Write(char[] buffer, int offset, int count)
        {
            lineLength += count;
            textLineLength += count;
            output.Write(buffer, offset, count, fallback);
        }

        void ITextSink.Write(int ucs32Char)
        {
            lineLength++;
            textLineLength++;
            output.Write(ucs32Char, fallback);
        }

        
        void ITextSinkEx.Write(string text)
        {
            lineLength += text.Length;
            textLineLength += text.Length;
            output.Write(text, fallback);
        }

        void ITextSinkEx.WriteNewLine()
        {
            if (lineLength > longestLineLength)
            {
                longestLineLength = lineLength;
            }

            output.Write("\r\n");
            lineLength = 0;
        }

        
        

        
        public void Close()
        {
            Dispose(true);
        }

        
        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        
        
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (output != null)
                {
                    if (!copyPending)
                    {
                        Flush();
                    }

                    if (output is IDisposable)
                    {
                        ((IDisposable)output).Dispose();
                    }
                }

                GC.SuppressFinalize(this);
            }

            output = null;
        }
    }
}
