// ***************************************************************
// <copyright file="HtmlReader.cs" company="Microsoft">
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

    

    
    
    
    internal enum HtmlTokenKind
    {
        
        Text,

        
        StartTag,
        
        EndTag,
        
        
        
        
        
        
        
        
        EmptyElementTag,                

#if PRIVATEBUILD
        
        
        
        
        
        EmptyScopeMark,
#endif
        
        
        
        
        SpecialTag,

#if PRIVATEBUILD
        
        
        
        Restart,
#endif

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        OverlappedClose,
        
        OverlappedReopen,
    }

    
    
    
    
    internal class HtmlReader : IRestartable, IResultsFeedback, IDisposable
    {
        

        private Encoding inputEncoding;
        private bool detectEncodingFromByteOrderMark;
#if PRIVATEBUILD
        private bool detectEncodingFromMetaTag;
        private bool detectEncodingFromXmlDeclaration;
#endif
        private bool normalizeInputHtml;

        internal bool testBoundaryConditions = false;
        internal int inputBufferSize = 4096;

        private Stream testTraceStream = null;
        private bool testTraceShowTokenNum = true;
        private int testTraceStopOnTokenNum = 0;

        private Stream testNormalizerTraceStream = null;
        private bool testNormalizerTraceShowTokenNum = true;
        private int testNormalizerTraceStopOnTokenNum = 0;

        private bool locked;

        

        private object input;

        private IHtmlParser parser;

        private HtmlTokenId parserTokenId;
        private HtmlToken parserToken;

        private HtmlToken nextParserToken;

        private int depth;

        private StringBuildSink stringBuildSink;

        private int currentAttribute;

        private bool literalTags;

        private enum State : byte
        {
            Disposed,                   
            EndOfFile,                  
            Begin,                      

            Text,                       
            EndText,                    

            OverlappedClose,            
            OverlappedReopen,           

            SpecialTag,                 
            EndSpecialTag,              
#if PRIVATEBUILD
            EmptyScopeMark,             
#endif
            BeginTag,                   
            ReadTagName,                
            EndTagName,                 
            BeginAttribute,             
            ReadAttributeName,          
            EndAttributeName,           
            BeginAttributeValue,        
            ReadAttributeValue,         
            EndAttribute,               
            ReadTag,                    
            EndTag,                     
        }

        private State state;
        private HtmlTokenKind tokenKind;

        
        
        
        
        
        
        
        public HtmlReader(Stream input, Encoding inputEncoding)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (!input.CanRead)
            {
                throw new ArgumentException("input stream must support reading");
            }

            this.input = input;
            this.inputEncoding = inputEncoding;
            state = State.Begin;
        }

        
        
        
        
        
        
        public HtmlReader(TextReader input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            this.input = input;
            inputEncoding = Encoding.Unicode;
            state = State.Begin;
        }

        
        

        
        
        
        
        
        
        
        
        
        public Encoding InputEncoding
        {
            get { return inputEncoding; }
            set { AssertNotLocked(); inputEncoding = value; }
        }

        
        
        
        
        
        
        
        
        public bool DetectEncodingFromByteOrderMark
        {
            get { return detectEncodingFromByteOrderMark; }
            set { AssertNotLocked(); detectEncodingFromByteOrderMark = value; }
        }
#if PRIVATEBUILD
        
        
        

        
        
        
        
        
        
        
        public bool DetectEncodingFromMetaTag
        {
            get { return this.detectEncodingFromMetaTag; }
            set { this.AssertNotLocked(); this.detectEncodingFromMetaTag = value; }
        }

        
        
        
        public bool DetectEncodingFromXmlDeclaration
        {
            get { return this.detectEncodingFromXmlDeclaration; }
            set { this.AssertNotLocked(); this.detectEncodingFromXmlDeclaration = value; }
        }
#endif
        
        
        
        
        
        
        
        
        public bool NormalizeHtml
        {
            get { return normalizeInputHtml; }
            set { AssertNotLocked(); normalizeInputHtml = value; }
        }

        
        

        
        
        
        
        
        
        public HtmlTokenKind TokenKind
        {
            get
            {
                AssertInToken();
                return tokenKind;
            }
        }

        
        
        
        
        
        
        
        public bool ReadNextToken()
        {
            AssertNotDisposed();

            if (state == State.EndOfFile)
            {
                return false;
            }

            if (!locked)
            {
                InitializeAndLock();
            }

            if (state == State.Text)
            {
                InternalDebug.Assert(parserTokenId == HtmlTokenId.Text || (literalTags && parserTokenId == HtmlTokenId.Tag && parserToken.TagIndex < HtmlTagIndex.Unknown));

                

                do
                {
                    ParseToken();
                }
                while (parserTokenId == HtmlTokenId.Text ||
                    (literalTags && parserTokenId == HtmlTokenId.Tag && parserToken.TagIndex < HtmlTagIndex.Unknown));
            }
            else if (state >= State.SpecialTag)
            {
                InternalDebug.Assert(parserTokenId == HtmlTokenId.Tag);

                
                

                while (!parserToken.IsTagEnd)
                {
                    ParseToken();
                }
#if PRIVATEBUILD
                if (HtmlDtd.tags[(int)this.parserToken.TagIndex].scope != HtmlDtd.TagScope.EMPTY && this.parserToken.IsEmptyScope)
                {
                    

                    if (this.state != State.EmptyScopeMark)
                    {
                        

                        this.state = State.EmptyScopeMark;
                        this.tokenKind = HtmlTokenKind.EmptyScopeMark;
                        return true;
                    }
                }
#endif
                if (parserToken.TagIndex <= HtmlTagIndex.Unknown ||                                
                    parserToken.IsEndTag ||                                                        
                    HtmlDtd.tags[(int)parserToken.TagIndex].scope == HtmlDtd.TagScope.EMPTY ||     
                    parserToken.IsEmptyScope)
                {
                    
                }
                else
                {
                    
                    depth ++;
                }

                if (!parserToken.IsEndTag &&
                    0 != (HtmlDtd.tags[(int)parserToken.TagIndex].literal & HtmlDtd.Literal.Tags))
                {
                    literalTags = true;
                }

                ParseToken();
            }
            else
            {
                

                if (state == State.OverlappedClose)
                {
                    
                    depth -= parserToken.Argument;
                }

                ParseToken();
            }

            while (true)
            {
                switch (parserTokenId)
                {
                    case HtmlTokenId.Text:

                            state = State.Text;
                            tokenKind = HtmlTokenKind.Text;

                            parserToken.Text.Rewind();
                            break;

                    case HtmlTokenId.Tag:

                            InternalDebug.Assert(parserToken.IsTagBegin);

                            if (parserToken.TagIndex < HtmlTagIndex.Unknown)
                            {
                                if (literalTags)
                                {
                                    
                                    state = State.Text;
                                    tokenKind = HtmlTokenKind.Text;
                                }
                                else
                                {
                                    state = State.SpecialTag;
                                    tokenKind = HtmlTokenKind.SpecialTag;
                                }

                                parserToken.Text.Rewind();
                            }
                            else
                            {
                                if (parserToken.TagIndex == HtmlTagIndex.TC)
                                {
                                    
                                    
                                    

                                    InternalDebug.Assert(parserToken.IsTagEnd);

                                    ParseToken();
                                    continue;
                                }

                                if (parserToken.IsTagNameEmpty && parserToken.TagIndex == HtmlTagIndex.Unknown)
                                {
                                    
                                    
                                    
                                    InternalDebug.Assert(parserToken.IsEndTag);

                                    state = State.SpecialTag;
                                    tokenKind = HtmlTokenKind.SpecialTag;

                                    parserToken.Text.Rewind();
                                }
                                else
                                {
                                    state = State.BeginTag;

                                    if (parserToken.IsEndTag)
                                    {
                                        tokenKind = HtmlTokenKind.EndTag;

                                        if (0 != (HtmlDtd.tags[(int)parserToken.TagIndex].literal & HtmlDtd.Literal.Tags))
                                        {
                                            literalTags = false;
                                        }
                                    }
                                    else if (parserToken.TagIndex > HtmlTagIndex.Unknown &&
                                            HtmlDtd.tags[(int)parserToken.TagIndex].scope == HtmlDtd.TagScope.EMPTY)
                                    {
                                        tokenKind = HtmlTokenKind.EmptyElementTag;
                                    }
                                    else
                                    {
                                        tokenKind = HtmlTokenKind.StartTag;
                                    }

                                    parserToken.Text.Rewind();

                                    
                                    
                                    
                                    if (parserToken.IsEndTag && parserToken.TagIndex != HtmlTagIndex.Unknown)
                                    {
                                        depth --;
                                    }
                                }
                            }
                            break;

                    case HtmlTokenId.OverlappedClose:

                            

                            

                            state = State.OverlappedClose;
                            tokenKind = HtmlTokenKind.OverlappedClose;
                            break;

                    case HtmlTokenId.OverlappedReopen:

                            

                            
                            depth += parserToken.Argument;

                            state = State.OverlappedReopen;
                            tokenKind = HtmlTokenKind.OverlappedReopen;
                            break;

                    case HtmlTokenId.EncodingChange:

                            
                            
                            ParseToken();
                            continue;

                    case HtmlTokenId.Restart:

#if PRIVATEBUILD
                            
                            this.state = State.Begin;
                            this.tokenKind = HtmlTokenKind.Restart;
                            break;
#else
                            InternalDebug.Assert(false);
                            continue;
#endif
                    default: 

                            InternalDebug.Assert(parserTokenId == HtmlTokenId.EndOfFile);

                            state = State.EndOfFile;
                            return false;
                }

                break; 
            }

            return true;
        }

        
        
        
        
        
        
        
        
        
        public int Depth
        {
            get
            {
                AssertNotDisposed();
                return depth;
            }
        }

        
        
        
        
        
        public int OverlappedDepth
        {
            get
            {
                if (state != State.OverlappedClose && state != State.OverlappedReopen)
                {
                    AssertInToken();
                    throw new InvalidOperationException("Reader must be positioned on OverlappedClose or OverlappedReopen token");
                }

                InternalDebug.Assert(parserTokenId == HtmlTokenId.OverlappedClose || parserTokenId == HtmlTokenId.OverlappedReopen);
                InternalDebug.Assert(tokenKind == HtmlTokenKind.OverlappedClose || tokenKind == HtmlTokenKind.OverlappedReopen);

                return parserToken.Argument;
            }
        }

        
        
        
        
        
        
        
        public HtmlTagId TagId
        {
            get
            {
                AssertInTag();
                return HtmlNameData.names[(int)parserToken.NameIndex].publicTagId;
            }
        }

        
        
        
        
        
        
        
        public bool TagInjectedByNormalizer
        {
            get
            {
                AssertInTag();

                if (state != State.BeginTag)
                {
                    throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
                }

                InternalDebug.Assert(parserTokenId == HtmlTokenId.Tag);

                return parserToken.Argument == 1;
            }
        }

        
        
        
        
        
        
        
        
        
        public bool TagNameIsLong
        {
            get
            {
                AssertInTag();

                if (state != State.BeginTag)
                {
                    throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
                }

                InternalDebug.Assert(parserTokenId == HtmlTokenId.Tag);

                return parserToken.NameIndex == HtmlNameIndex.Unknown && parserToken.IsTagNameBegin && !parserToken.IsTagNameEnd;
            }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public string ReadTagName()
        {
            if (state != State.BeginTag)
            {
                AssertInTag();
                throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
            }

            InternalDebug.Assert(parserTokenId == HtmlTokenId.Tag && parserToken.IsTagBegin);

            string name;
            
            if (parserToken.NameIndex != HtmlNameIndex.Unknown)
            {
                
                name = HtmlNameData.names[(int)parserToken.NameIndex].name;
            }
            else
            {
                InternalDebug.Assert(parserTokenId == HtmlTokenId.Tag && parserToken.IsTagBegin && parserToken.NameIndex == HtmlNameIndex.Unknown);

                if (parserToken.IsTagNameEnd)
                {
                    return parserToken.Name.GetString(int.MaxValue);
                }

                InternalDebug.Assert(!parserToken.IsTagEnd);

                var sbSink = GetStringBuildSink();

                parserToken.Name.WriteTo(sbSink);

                do
                {
                    ParseToken();

                    parserToken.Name.WriteTo(sbSink);
                }
                while (!parserToken.IsTagNameEnd);

                name = sbSink.ToString();
            }

            
            state = State.EndTagName;

            
            return name;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        public int ReadTagName(char[] buffer, int offset, int count)
        {
            AssertInTag();
            if (state > State.EndTagName)
            {
                throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
            }

            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            var lengthRead = 0;

            if (state != State.EndTagName)
            {
                if (state == State.BeginTag)
                {
                    state = State.ReadTagName;
                    parserToken.Name.Rewind();
                }

                while (count != 0)
                {
                    var chunkLength = parserToken.Name.Read(buffer, offset, count);

                    if (chunkLength == 0)
                    {
                        if (parserToken.IsTagNameEnd)
                        {
                            state = State.EndTagName;
                            break;
                        }

                        ParseToken();
                        parserToken.Name.Rewind();
                        continue;
                    }

                    offset += chunkLength;
                    count -= chunkLength;
                    lengthRead += chunkLength;
                }
            }

            return lengthRead;
        }

        
        internal void WriteTagNameTo(ITextSink sink)
        {
            if (state != State.BeginTag)
            {
                AssertInTag();
                throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
            }

            while (true)
            {
                parserToken.Name.WriteTo(sink);

                if (parserToken.IsTagNameEnd)
                {
                    state = State.EndTagName;
                    break;
                }

                ParseToken();
            }
        }

        
        
        
        
        
        
        public HtmlAttributeReader AttributeReader
        {
            get
            {
                AssertInTag();
                if (state == State.ReadTag)
                {
                    throw new InvalidOperationException("Cannot read attributes after reading tag as a markup text");
                }

                return new HtmlAttributeReader(this);
            }
        }

        
        
        
        
        
        
        
        
        public int ReadText(char[] buffer, int offset, int count)
        {
            if (state == State.EndText)
            {
                return 0;
            }

            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            if (state != State.Text)
            {
                AssertInToken();
                throw new InvalidOperationException("Reader must be positioned on a Text token");
            }

            var lengthRead = 0;

            while (count != 0)
            {
                var chunkLength = parserToken.Text.Read(buffer, offset, count);

                if (chunkLength == 0)
                {
                    var nextToken = PreviewNextToken();
                    
                    if (nextToken != HtmlTokenId.Text &&
                        (!literalTags || nextToken != HtmlTokenId.Tag || nextParserToken.TagIndex >= HtmlTagIndex.Unknown))
                    {
                        state = State.EndText;
                        break;
                    }

                    ParseToken();
                    parserToken.Text.Rewind();
                    continue;
                }

                offset += chunkLength;
                count -= chunkLength;
                lengthRead += chunkLength;
            }

            return lengthRead;
        }

        
        internal void WriteTextTo(ITextSink sink)
        {
            if (state != State.Text)
            {
                AssertInToken();
                throw new InvalidOperationException("Reader must be positioned on a Text token");
            }

            while (true)
            {
                parserToken.Text.WriteTo(sink);

                var nextToken = PreviewNextToken();
                
                if (nextToken != HtmlTokenId.Text &&
                    (!literalTags || nextToken != HtmlTokenId.Tag || nextParserToken.TagIndex >= HtmlTagIndex.Unknown))
                {
                    state = State.EndText;
                    break;
                }

                ParseToken();
            }
        }

        
        
        
        
        
        
        
        
        
        
        public int ReadMarkupText(char[] buffer, int offset, int count)
        {
            if (state == State.EndTag || state == State.EndSpecialTag || state == State.EndText)
            {
                return 0;
            }

            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            if (state == State.BeginTag)
            {
                state = State.ReadTag;
            }
            else if (state != State.SpecialTag && state != State.ReadTag && state != State.Text)
            {
                AssertInToken();

                if (state > State.BeginTag)
                {
                    throw new InvalidOperationException("Cannot read tag content as markup text after accessing tag name or attributes");
                }

                throw new InvalidOperationException("Reader must be positioned on Text, StartTag, EndTag, EmptyElementTag or SpecialTag token");
            }

            var lengthRead = 0;

            while (count != 0)
            {
                var chunkLength = parserToken.Text.ReadOriginal(buffer, offset, count);

                if (chunkLength == 0)
                {
                    if (state == State.SpecialTag)
                    {
                        if (parserToken.IsTagEnd)
                        {
                            state = State.EndSpecialTag;
                            break;
                        }
                    }
                    else if (state == State.ReadTag)
                    {
                        if (parserToken.IsTagEnd)
                        {
                            state = State.EndTag;
                            break;
                        }
                    }
                    else 
                    {
                        var nextToken = PreviewNextToken();
                        
                        if (nextToken != HtmlTokenId.Text &&
                            (!literalTags || nextToken != HtmlTokenId.Tag || nextParserToken.TagIndex >= HtmlTagIndex.Unknown))
                        {
                            state = State.EndText;
                            break;
                        }
                    }

                    ParseToken();
                    parserToken.Text.Rewind();
                    continue;
                }

                offset += chunkLength;
                count -= chunkLength;
                lengthRead += chunkLength;
            }

            return lengthRead;
        }

        
        internal void WriteMarkupTextTo(ITextSink sink)
        {
            if (state == State.BeginTag)
            {
                state = State.ReadTag;
            }
            else if (state != State.SpecialTag && state != State.ReadTag && state != State.Text)
            {
                AssertInToken();

                if (state > State.BeginTag)
                {
                    throw new InvalidOperationException("Cannot read tag content as markup text after accessing tag name or attributes");
                }

                throw new InvalidOperationException("Reader must be positioned on Text, StartTag, EndTag, EmptyElementTag or SpecialTag token");
            }

            while (true)
            {
                parserToken.Text.WriteOriginalTo(sink);

                if (state == State.SpecialTag)
                {
                    if (parserToken.IsTagEnd)
                    {
                        state = State.EndSpecialTag;
                        break;
                    }
                }
                else if (state == State.ReadTag)
                {
                    if (parserToken.IsTagEnd)
                    {
                        state = State.EndTag;
                        break;
                    }
                }
                else 
                {
                    var nextToken = PreviewNextToken();
                    
                    if (nextToken != HtmlTokenId.Text &&
                        (!literalTags || nextToken != HtmlTokenId.Tag || nextParserToken.TagIndex >= HtmlTagIndex.Unknown))
                    {
                        state = State.EndText;
                        break;
                    }
                }

                ParseToken();
            }
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

                if (parser != null && parser is IDisposable)
                {
                    ((IDisposable)parser).Dispose();
                }

                if (input != null && input is IDisposable)
                {
                    ((IDisposable)input).Dispose();
                }

                GC.SuppressFinalize(this);
            }

            parser = null;
            input = null;
            stringBuildSink = null;
            parserToken = null;
            nextParserToken = null;
            state = State.Disposed;
        }

        

        internal HtmlReader SetInputEncoding(Encoding value)
        {
            InputEncoding = value;
            return this;
        }

        internal HtmlReader SetDetectEncodingFromByteOrderMark(bool value)
        {
            DetectEncodingFromByteOrderMark = value;
            return this;
        }

#if PRIVATEBUILD
        internal HtmlReader SetDetectEncodingFromMetaTag(bool value)
        {
            this.DetectEncodingFromMetaTag = value;
            return this;
        }
#endif
        internal HtmlReader SetNormalizeHtml(bool value)
        {
            NormalizeHtml = value;
            return this;
        }

        internal HtmlReader SetTestBoundaryConditions(bool value)
        {
            testBoundaryConditions = value;
            return this;
        }

        internal HtmlReader SetTestTraceStream(Stream value)
        {
            testTraceStream = value;
            return this;
        }

        internal HtmlReader SetTestTraceShowTokenNum(bool value)
        {
            testTraceShowTokenNum = value;
            return this;
        }

        internal HtmlReader SetTestTraceStopOnTokenNum(int value)
        {
            testTraceStopOnTokenNum = value;
            return this;
        }

        internal HtmlReader SetTestNormalizerTraceStream(Stream value)
        {
            testNormalizerTraceStream = value;
            return this;
        }

        internal HtmlReader SetTestNormalizerTraceShowTokenNum(bool value)
        {
            testNormalizerTraceShowTokenNum = value;
            return this;
        }

        internal HtmlReader SetTestNormalizerTraceStopOnTokenNum(int value)
        {
            testNormalizerTraceStopOnTokenNum = value;
            return this;
        }

        
        private void InitializeAndLock()
        {
            locked = true;

            ConverterInput converterInput;

            if (input is Stream)
            {
                if (inputEncoding == null)
                {
                    throw new InvalidOperationException(Strings.InputEncodingRequired);
                }

                converterInput = new ConverterDecodingInput(
                                    (Stream)input, 
                                    false,
                                    inputEncoding, 
                                    detectEncodingFromByteOrderMark, 
                                    
                                    TextConvertersDefaults.MaxTokenSize(testBoundaryConditions), 
                                    TextConvertersDefaults.MaxHtmlMetaRestartOffset(testBoundaryConditions), 
                                    
                                    inputBufferSize,                       
                                    testBoundaryConditions,
                                    this as IResultsFeedback,
                                    null); 
            }
            else
            {
                converterInput = new ConverterUnicodeInput(
                                    input, 
                                    false,
                                    
                                    TextConvertersDefaults.MaxTokenSize(testBoundaryConditions), 
                                    testBoundaryConditions,
                                    null); 

            }

            var preParser = new HtmlParser(
                                    converterInput,
#if PRIVATEBUILD
                                    this.detectEncodingFromMetaTag,
#else
                                    false,
#endif
                                    false, 
                                    
                                    TextConvertersDefaults.MaxTokenRuns(testBoundaryConditions),
                                    
                                    TextConvertersDefaults.MaxHtmlAttributes(testBoundaryConditions), 
                                    testBoundaryConditions);

            if (normalizeInputHtml)
            {
                parser = new HtmlNormalizingParser(
                                        preParser, 
                                        null, 
                                        false, 
                                        TextConvertersDefaults.MaxHtmlNormalizerNesting(testBoundaryConditions),
                                        testBoundaryConditions,
                                        testNormalizerTraceStream, 
                                        testNormalizerTraceShowTokenNum, 
                                        testNormalizerTraceStopOnTokenNum);
            }
            else
            {
                parser = preParser;
            }

#if PRIVATEBUILD
            if (this.detectEncodingFromMetaTag)
            {
                this.parser.SetRestartConsumer(this);
            }
#endif
        }

        
        bool IRestartable.CanRestart()
        {
            return false;
        }

        
        void IRestartable.Restart()
        {
        }

        
        void IRestartable.DisableRestart()
        {
        }

        
        void IResultsFeedback.Set(ConfigParameter parameterId, object val)
        {
            switch (parameterId)
            {
                case ConfigParameter.InputEncoding:
                        inputEncoding = (Encoding) val;
                        break;
            }
        }

        
        private void ParseToken()
        {
            if (nextParserToken != null)
            {
                parserToken = nextParserToken;
                parserTokenId = parserToken.TokenId;
                nextParserToken = null;
                return;
            }

            parserTokenId = parser.Parse();
            parserToken = parser.Token;

        }

        
        private HtmlTokenId PreviewNextToken()
        {
            if (nextParserToken == null)
            {
                parser.Parse();
                nextParserToken = parser.Token;

            }

            return nextParserToken.TokenId;
        }

        
        private StringBuildSink GetStringBuildSink()
        {
            if (stringBuildSink == null)
            {
                stringBuildSink = new StringBuildSink();
            }

            stringBuildSink.Reset(int.MaxValue);
            return stringBuildSink;
        }

        
        internal bool AttributeReader_ReadNextAttribute()
        {
            if (state == State.EndTag)
            {
                return false;
            }

            AssertInTag();

            if (state == State.ReadTag)
            {
                throw new InvalidOperationException("Cannot read attributes after reading tag as markup text");
            }

            do
            {
                if (state >= State.BeginTag &&
                    state < State.BeginAttribute)
                {
                    

                    while (parserToken.Attributes.Count == 0 && !parserToken.IsTagEnd)
                    {
                        ParseToken();
                    }

                    if (parserToken.Attributes.Count == 0)
                    {
                        InternalDebug.Assert(parserToken.IsTagEnd);

                        state = State.EndTag;
                        return false;
                    }

                    currentAttribute = 0;
                    InternalDebug.Assert(parserToken.Attributes[0].IsAttrBegin);

                    state = State.BeginAttribute;
                }
                else
                {
                    InternalDebug.Assert(currentAttribute < parserToken.Attributes.Count);

                    if (++currentAttribute == parserToken.Attributes.Count)
                    {
                        

                        if (parserToken.IsTagEnd)
                        {
                            state = State.EndTag;
                            return false;
                        }

                        while (true)
                        {
                            ParseToken();

                            InternalDebug.Assert(!parserToken.IsTagBegin);

                            if (parserToken.Attributes.Count != 0 && (parserToken.Attributes[0].IsAttrBegin || parserToken.Attributes.Count > 1))
                            {
                                
                                break;
                            }

                            
                            if (parserToken.IsTagEnd)
                            {
                                state = State.EndTag;
                                return false;
                            }
                        }

                        currentAttribute = 0;

                        if (!parserToken.Attributes[0].IsAttrBegin)
                        {
                            
                            
                            currentAttribute++;
                            InternalDebug.Assert(currentAttribute < parserToken.Attributes.Count);
                        }
                    }
                }

                
                
                
                if (!parserToken.Attributes[currentAttribute].IsAttrEmptyName)
                {
                    break;
                }
            }
            while (true);

            state = State.BeginAttribute;
            return true;
        }

        
        internal HtmlAttributeId AttributeReader_GetCurrentAttributeId()
        {
            AssertInAttribute();

            return HtmlNameData.names[(int)parserToken.Attributes[currentAttribute].NameIndex].publicAttributeId;
        }

        
        internal bool AttributeReader_CurrentAttributeNameIsLong()
        {
            if (state != State.BeginAttribute)
            {
                AssertInAttribute();
                throw new InvalidOperationException();
            }

            return parserToken.Attributes[currentAttribute].NameIndex == HtmlNameIndex.Unknown &&
                    parserToken.Attributes[currentAttribute].IsAttrBegin && !parserToken.Attributes[currentAttribute].IsAttrNameEnd;
        }

        
        internal string AttributeReader_ReadCurrentAttributeName()
        {
            if (state != State.BeginAttribute)
            {
                AssertInAttribute();
                throw new InvalidOperationException("Reader must be positioned at the beginning of attribute.");
            }

            InternalDebug.Assert(parserToken.Attributes[currentAttribute].IsAttrBegin);

            string name;
            
            if (parserToken.Attributes[currentAttribute].NameIndex != HtmlNameIndex.Unknown)
            {
                
                name = HtmlNameData.names[(int)parserToken.Attributes[currentAttribute].NameIndex].name;
            }
            else
            {
                InternalDebug.Assert(parserToken.Attributes[currentAttribute].IsAttrBegin && parserToken.Attributes[currentAttribute].NameIndex == HtmlNameIndex.Unknown);

                if (parserToken.Attributes[currentAttribute].IsAttrNameEnd)
                {
                    return parserToken.Attributes[currentAttribute].Name.GetString(int.MaxValue);
                }

                InternalDebug.Assert(!parserToken.IsTagEnd && !parserToken.Attributes[currentAttribute].IsAttrEnd);

                var sbSink = GetStringBuildSink();

                parserToken.Attributes[currentAttribute].Name.WriteTo(sbSink);

                do
                {
                    ParseToken();

                    InternalDebug.Assert(!parserToken.IsTagBegin && 
                        parserToken.Attributes.Count != 0 &&
                        !parserToken.Attributes[0].IsAttrBegin);

                    currentAttribute = 0;

                    parserToken.Attributes[currentAttribute].Name.WriteTo(sbSink);
                }
                while (!parserToken.Attributes[0].IsAttrNameEnd);

                name = sbSink.ToString();
            }

            
            state = State.EndAttributeName;

            
            return name;
        }

        
        internal int AttributeReader_ReadCurrentAttributeName(char[] buffer, int offset, int count)
        {
            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            if (state < State.BeginAttribute || state > State.EndAttributeName)
            {
                AssertInAttribute();
                throw new InvalidOperationException("Reader must be positioned at the beginning of attribute.");
            }

            var lengthRead = 0;

            if (state != State.EndAttributeName)
            {
                if (state == State.BeginAttribute)
                {
                    state = State.ReadAttributeName;
                    parserToken.Attributes[currentAttribute].Name.Rewind();
                }

                while (count != 0)
                {
                    var chunkLength = parserToken.Attributes[currentAttribute].Name.Read(buffer, offset, count);

                    if (chunkLength == 0)
                    {
                        if (parserToken.Attributes[currentAttribute].IsAttrNameEnd)
                        {
                            state = State.EndAttributeName;
                            break;
                        }

                        ParseToken();

                        InternalDebug.Assert(!parserToken.IsTagBegin && 
                            parserToken.Attributes.Count != 0 &&
                            !parserToken.Attributes[0].IsAttrBegin);

                        currentAttribute = 0;
                        parserToken.Attributes[currentAttribute].Name.Rewind();
                        continue;
                    }

                    offset += chunkLength;
                    count -= chunkLength;
                    lengthRead += chunkLength;
                }
            }

            return lengthRead;
        }

        
        internal void AttributeReader_WriteCurrentAttributeNameTo(ITextSink sink)
        {
            if (state != State.BeginAttribute)
            {
                AssertInAttribute();
                throw new InvalidOperationException("Reader must be positioned at the beginning of attribute.");
            }

            while (true)
            {
                parserToken.Attributes[currentAttribute].Name.WriteTo(sink);

                if (parserToken.Attributes[currentAttribute].IsAttrNameEnd)
                {
                    state = State.EndAttributeName;
                    break;
                }

                ParseToken();

                InternalDebug.Assert(!parserToken.IsTagBegin && 
                    parserToken.Attributes.Count != 0 &&
                    !parserToken.Attributes[0].IsAttrBegin);

                currentAttribute = 0;
            }
        }

        
        internal bool AttributeReader_CurrentAttributeHasValue()
        {
            if (state != State.BeginAttributeValue)
            {
                AssertInAttribute();

                if (state > State.BeginAttributeValue)
                {
                    throw new InvalidOperationException("Reader must be positioned before attribute value");
                }

                
                if (!SkipToAttributeValue())
                {
                    state = State.EndAttributeName;
                    return false;
                }

                state = State.BeginAttributeValue;
            }

            return true;
        }

        
        internal bool AttributeReader_CurrentAttributeValueIsLong()
        {
            if (state != State.BeginAttributeValue)
            {
                AssertInAttribute();

                if (state > State.BeginAttributeValue)
                {
                    throw new InvalidOperationException("Reader must be positioned before attribute value");
                }

                
                if (!SkipToAttributeValue())
                {
                    state = State.EndAttributeName;
                    return false;
                }

                state = State.BeginAttributeValue;
            }

            return parserToken.Attributes[currentAttribute].IsAttrValueBegin && !parserToken.Attributes[currentAttribute].IsAttrEnd;
        }

        
        internal string AttributeReader_ReadCurrentAttributeValue()
        {
            if (state != State.BeginAttributeValue)
            {
                AssertInAttribute();

                if (state > State.BeginAttributeValue)
                {
                    throw new InvalidOperationException("Reader must be positioned before attribute value");
                }

                
                if (!SkipToAttributeValue())
                {
                    state = State.EndAttribute;
                    return null;
                }

                
            }

            InternalDebug.Assert(parserToken.Attributes[currentAttribute].IsAttrValueBegin);
            
            InternalDebug.Assert(parserToken.Attributes[currentAttribute].IsAttrValueBegin);

            if (parserToken.Attributes[currentAttribute].IsAttrEnd)
            {
                return parserToken.Attributes[currentAttribute].Value.GetString(int.MaxValue);
            }

            InternalDebug.Assert(!parserToken.IsTagEnd && !parserToken.Attributes[currentAttribute].IsAttrEnd);

            var sbSink = GetStringBuildSink();

            parserToken.Attributes[currentAttribute].Value.WriteTo(sbSink);

            do
            {
                ParseToken();

                InternalDebug.Assert(!parserToken.IsTagBegin && 
                    parserToken.Attributes.Count != 0 &&
                    !parserToken.Attributes[0].IsAttrBegin);

                currentAttribute = 0;

                parserToken.Attributes[0].Value.WriteTo(sbSink);
            }
            while (!parserToken.Attributes[0].IsAttrEnd);

            
            state = State.EndAttribute;

            return sbSink.ToString();
        }

        
        internal int AttributeReader_ReadCurrentAttributeValue(char[] buffer, int offset, int count)
        {
            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Strings.OffsetOutOfRange);
            }

            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountOutOfRange);
            }

            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", Strings.CountTooLarge);
            }

            AssertInAttribute();

            if (state < State.BeginAttributeValue)
            {
                
                if (!SkipToAttributeValue())
                {
                    state = State.EndAttribute;
                    return 0;
                }

                state = State.BeginAttributeValue;
            }

            var lengthRead = 0;

            if (state != State.EndAttribute)
            {
                if (state == State.BeginAttributeValue)
                {
                    state = State.ReadAttributeValue;
                    parserToken.Attributes[currentAttribute].Value.Rewind();
                }

                while (count != 0)
                {
                    var chunkLength = parserToken.Attributes[currentAttribute].Value.Read(buffer, offset, count);

                    if (chunkLength == 0)
                    {
                        if (parserToken.Attributes[currentAttribute].IsAttrEnd)
                        {
                            state = State.EndAttribute;
                            break;
                        }

                        ParseToken();

                        InternalDebug.Assert(!parserToken.IsTagBegin && 
                            parserToken.Attributes.Count != 0 &&
                            !parserToken.Attributes[0].IsAttrBegin);

                        currentAttribute = 0;
                        parserToken.Attributes[currentAttribute].Value.Rewind();
                        continue;
                    }

                    offset += chunkLength;
                    count -= chunkLength;
                    lengthRead += chunkLength;
                }
            }

            return lengthRead;
        }

        
        internal void AttributeReader_WriteCurrentAttributeValueTo(ITextSink sink)
        {
            if (state != State.BeginAttributeValue)
            {
                AssertInAttribute();

                if (state > State.BeginAttributeValue)
                {
                    throw new InvalidOperationException("Reader must be positioned before attribute value");
                }

                
                if (!SkipToAttributeValue())
                {
                    state = State.EndAttribute;
                    return;
                }

                
            }

            while (true)
            {
                parserToken.Attributes[currentAttribute].Value.WriteTo(sink);

                if (parserToken.Attributes[currentAttribute].IsAttrEnd)
                {
                    state = State.EndAttribute;
                    break;
                }

                ParseToken();

                InternalDebug.Assert(!parserToken.IsTagBegin && 
                    parserToken.Attributes.Count != 0 &&
                    !parserToken.Attributes[0].IsAttrBegin);

                currentAttribute = 0;
            }
        }

        
        private bool SkipToAttributeValue()
        {
            if (!parserToken.Attributes[currentAttribute].IsAttrValueBegin)
            {
                if (parserToken.Attributes[currentAttribute].IsAttrEnd)
                {
                    
                    return false;
                }

                InternalDebug.Assert(!parserToken.IsTagEnd);

                do
                {
                    ParseToken();
                    InternalDebug.Assert(!parserToken.IsTagBegin && parserToken.Attributes.Count != 0 && !parserToken.Attributes[0].IsAttrBegin);
                }
                while (!parserToken.Attributes[0].IsAttrValueBegin && !parserToken.Attributes[0].IsAttrEnd);

                if (parserToken.Attributes[currentAttribute].IsAttrEnd)
                {
                    
                    return false;
                }
            }

            return true;
        }

        
        private void AssertNotLocked()
        {
            AssertNotDisposed();
            if (locked)
            {
                throw new InvalidOperationException("Cannot set reader properties after reading a first token");
            }
        }

        
        private void AssertNotDisposed()
        {
            if (state == State.Disposed)
            {
                throw new ObjectDisposedException("HtmlReader");
            }
        }

        
        private void AssertInToken()
        {
            if (state <= State.Begin)
            {
                AssertNotDisposed();
                throw new InvalidOperationException("Reader must be positioned inside a valid token");
            }
        }

        
        private void AssertInTag()
        {
            if (state < State.BeginTag)
            {
                AssertInToken();
                throw new InvalidOperationException("Reader must be positioned inside a StartTag, EndTag or EmptyElementTag token");
            }

            InternalDebug.Assert(parserTokenId == HtmlTokenId.Tag && parserToken.TagIndex >= HtmlTagIndex.Unknown);
        }

        
        private void AssertInAttribute()
        {
            if (state < State.BeginAttribute || state > State.EndAttribute)
            {
                AssertInTag();
                throw new InvalidOperationException("Reader must be positioned inside attribute");
            }

            InternalDebug.Assert(currentAttribute < parserToken.Attributes.Count);
        }
    }

    
    
    
    
    internal struct HtmlAttributeReader
    {
        private HtmlReader reader;

        
        internal HtmlAttributeReader(HtmlReader reader)
        {
            this.reader = reader;
        }

        
        
        
        public bool ReadNext()
        {
            return reader.AttributeReader_ReadNextAttribute();
        }

        
        
        
        
        
        
        
        public HtmlAttributeId Id => reader.AttributeReader_GetCurrentAttributeId();


        public bool NameIsLong => reader.AttributeReader_CurrentAttributeNameIsLong();


        public string ReadName()
        {
            return reader.AttributeReader_ReadCurrentAttributeName();
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public int ReadName(char[] buffer, int offset, int count)          
        {
            return reader.AttributeReader_ReadCurrentAttributeName(buffer, offset, count);
        }

        
        internal void WriteNameTo(ITextSink sink)
        {
            reader.AttributeReader_WriteCurrentAttributeNameTo(sink);
        }

        
        
        
        
        
        
        
        
        public bool HasValue => reader.AttributeReader_CurrentAttributeHasValue();


        public bool ValueIsLong => reader.AttributeReader_CurrentAttributeValueIsLong();


        public string ReadValue()
        {
            return reader.AttributeReader_ReadCurrentAttributeValue();
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public int ReadValue(char[] buffer, int offset, int count)         
        {
            return reader.AttributeReader_ReadCurrentAttributeValue(buffer, offset, count);
        }

        
        internal void WriteValueTo(ITextSink sink)
        {
            reader.AttributeReader_WriteCurrentAttributeValueTo(sink);
        }

        
    }
}
