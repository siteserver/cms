// ***************************************************************
// <copyright file="HtmlParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
    using System;
    using System.Text;
    using Data.Internal;
    using Globalization;


    internal interface IHtmlParser
    {
        

        HtmlToken Token { get; }

        HtmlTokenId Parse();
        void SetRestartConsumer(IRestartable restartConsumer);
    }

    

    internal class HtmlParser : IHtmlParser, IRestartable, IReusable, IDisposable
    {
        

        #pragma warning disable 0429 
        private const int ParseThresholdMax = (HtmlNameData.MAX_ENTITY_NAME > HtmlNameData.MAX_NAME ? HtmlNameData.MAX_ENTITY_NAME : HtmlNameData.MAX_NAME) + 2;
        #pragma warning restore 0429

        private ConverterInput input;
        private bool endOfFile;

        
        private bool literalTags;               
        private HtmlNameIndex literalTagNameId;           
        private bool literalEntities;           
        private bool plaintext;                 

        
        
        
        private bool parseConditionals = false;

        
        private ParseState parseState = ParseState.Text;

        private char[] parseBuffer;
        private int parseStart;
        private int parseCurrent;
        private int parseEnd;
        private int parseThreshold = 1;

        private bool slowParse = true;

        private char scanQuote;
        private char valueQuote;
        private CharClass lastCharClass;
        private int nameLength;

        
        private HtmlTokenBuilder tokenBuilder;
        private HtmlToken token;

        private IRestartable restartConsumer;
        private bool detectEncodingFromMetaTag;

        
        private short[] hashValuesTable;

        private bool rightMeta;
        private Encoding newEncoding;

        private SavedParserState savedState;

        
        public HtmlParser(
                ConverterInput input,
                bool detectEncodingFromMetaTag,
                bool preformatedText,
                int maxRuns,
                int maxAttrs,
                bool testBoundaryConditions)
        {
            this.input = input;
            this.detectEncodingFromMetaTag = detectEncodingFromMetaTag;
            input.SetRestartConsumer(this);

            tokenBuilder = new HtmlTokenBuilder(null, maxRuns, maxAttrs, testBoundaryConditions);

            token = tokenBuilder.Token;

            plaintext = preformatedText;
            literalEntities = preformatedText;
        }

        
        protected enum ParseState : byte
        {
            Text,
            TagStart,
            TagNamePrefix,
            TagName,
            TagWsp,
            AttrNameStart,
            AttrNamePrefix,
            AttrName,
            AttrWsp,
            AttrValueWsp,
            AttrValue,
            EmptyTagEnd,
            TagEnd,
            TagSkip,
            CommentStart,
            Comment,
            Conditional,
            CommentConditional,
            Bang,
            Dtd,
            Asp,
        }

        
        public HtmlToken Token => token;


        public void SetRestartConsumer(IRestartable restartConsumer)
        {
            this.restartConsumer = restartConsumer;
        }

        
        private void Reinitialize()
        {
            endOfFile = false;

            literalTags = false;
            literalTagNameId = HtmlNameIndex._NOTANAME;
            literalEntities = false;
            plaintext = false;

            parseState = ParseState.Text;

            parseBuffer = null;
            parseStart = 0;
            parseCurrent = 0;
            parseEnd = 0;
            parseThreshold = 1;

            slowParse = true;  

            scanQuote = '\0';
            valueQuote = '\0';
            lastCharClass = 0;
            nameLength = 0;

            tokenBuilder.Reset();
            tokenBuilder.MakeEmptyToken(HtmlTokenId.Restart);
        }

        
        public bool ParsingFragment => savedState != null && savedState.StateSaved;


        public void PushFragment(ConverterInput fragmentInput, bool literalTextInput)
        {
            if (savedState == null)
            {
                savedState = new SavedParserState();
            }

            savedState.PushState(this, fragmentInput, literalTextInput);
        }

        
        public void PopFragment()
        {
            savedState.PopState(this);
        }

        
        private class SavedParserState
        {
            private ConverterInput input;
            private bool endOfFile;
            private ParseState parseState;
            private bool slowParse;
            private bool literalTags;
            private HtmlNameIndex literalTagNameId;
            private bool literalEntities;
            private bool plaintext;
            private char[] parseBuffer;
            private int parseStart;
            private int parseCurrent;
            private int parseEnd;
            private int parseThreshold;

            public bool StateSaved => input != null;

            public void PushState(HtmlParser parser, ConverterInput newInput, bool literalTextInput)
            {
                InternalDebug.Assert(!StateSaved);
                InternalDebug.Assert(parser.parseState == ParseState.Text || parser.parseState == ParseState.TagStart);

                input = parser.input;
                endOfFile = parser.endOfFile;
                parseState = parser.parseState;
                slowParse = parser.slowParse;
                literalTags = parser.literalTags;
                literalTagNameId = parser.literalTagNameId;
                literalEntities = parser.literalEntities;
                plaintext = parser.plaintext;
                parseBuffer = parser.parseBuffer;
                parseStart = parser.parseStart;
                parseCurrent = parser.parseCurrent;
                parseEnd = parser.parseEnd;
                parseThreshold = parser.parseThreshold;

                parser.input = newInput;
                parser.endOfFile = false;
                parser.parseState = ParseState.Text;
                parser.slowParse = true;
                parser.literalTags = literalTextInput;
                parser.literalTagNameId = HtmlNameIndex.PlainText;
                parser.literalEntities = literalTextInput;
                parser.plaintext = literalTextInput;
                parser.parseBuffer = null;
                parser.parseStart = 0;
                parser.parseCurrent = 0;
                parser.parseEnd = 0;
                parser.parseThreshold = 1;
            }

            public void PopState(HtmlParser parser)
            {
                InternalDebug.Assert(StateSaved);
                InternalDebug.Assert(parser.endOfFile);

                parser.input = input;
                parser.endOfFile = endOfFile;
                parser.parseState = parseState;
                parser.slowParse = slowParse;
                parser.literalTags = literalTags;
                parser.literalTagNameId = literalTagNameId;
                parser.literalEntities = literalEntities;
                parser.plaintext = plaintext;
                parser.parseBuffer = parseBuffer;
                parser.parseStart = parseStart;
                parser.parseCurrent = parseCurrent;
                parser.parseEnd = parseEnd;
                parser.parseThreshold = parseThreshold;

                input = null;
                parseBuffer = null;
            }
        }

        
        bool IRestartable.CanRestart()
        {
            return restartConsumer != null && restartConsumer.CanRestart();
        }

        
        void IRestartable.Restart()
        {
            if (restartConsumer != null)
            {
                restartConsumer.Restart();
            }

            Reinitialize();
        }

        
        void IRestartable.DisableRestart()
        {
            if (restartConsumer != null)
            {
                restartConsumer.DisableRestart();
                restartConsumer = null;
            }
        }

        
        void IReusable.Initialize(object newSourceOrDestination)
        {
            InternalDebug.Assert(input is IReusable);

            ((IReusable)input).Initialize(newSourceOrDestination);

            Reinitialize();

            input.SetRestartConsumer(this);
        }

        
        public void Initialize(string fragment, bool preformatedText)
        {
            InternalDebug.Assert(input is ConverterBufferInput);

            (input as ConverterBufferInput).Initialize(fragment);

            Reinitialize();

            plaintext = preformatedText;
            literalEntities = preformatedText;
        }

        
        void IDisposable.Dispose()
        {
            if (input != null /*&& this.input is IDisposable*/)
            {
                ((IDisposable)input).Dispose();
            }

            input = null;
            restartConsumer = null;
            parseBuffer = null;
            token = null;
            tokenBuilder = null;
            hashValuesTable = null;

            GC.SuppressFinalize(this);
        }

        
        public HtmlTokenId Parse()
        {
            if (slowParse)
            {
                return ParseSlow();
            }

            
            
            
            
            
            
            InternalDebug.Assert(this.parseBuffer[this.parseCurrent] == '<' && parseState == ParseState.TagStart);

            if (tokenBuilder.Valid)
            {
                

                
                InternalDebug.Assert(!tokenBuilder.IncompleteTag);

                input.ReportProcessed(this.parseCurrent - parseStart);
                parseStart = this.parseCurrent;

                tokenBuilder.Reset();
            }

            
            

            var parseBuffer = this.parseBuffer;
            var parseCurrent = this.parseCurrent;

            var runStart = parseCurrent;
            var endTag = false;

            var ch = parseBuffer[++parseCurrent];

            if (ch == '/')
            {
                endTag = true;
                ch = parseBuffer[++parseCurrent];
            }

            var charClass = ParseSupport.GetCharClass(ch);

            if (!ParseSupport.AlphaCharacter(charClass))
            {
                
                goto StartSlowly;
            }

            tokenBuilder.StartTag(HtmlNameIndex.Unknown, runStart);
            if (endTag)
            {
                tokenBuilder.SetEndTag();
            }

            
            
            tokenBuilder.DebugPrepareToAddMoreRuns(6);

            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

            tokenBuilder.StartTagName();

            var nameLength = 0;

            runStart = parseCurrent;

            parseState = ParseState.TagNamePrefix;

            do
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }
            while (ParseSupport.HtmlSimpleTagNameCharacter(charClass));

            if (ch == ':')
            {
                
                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);

                
                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, parseCurrent, parseCurrent + 1);

                tokenBuilder.EndTagNamePrefix();

                nameLength = parseCurrent + 1 - runStart;
                runStart = parseCurrent + 1;

                
                do
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                while (ParseSupport.HtmlSimpleTagNameCharacter(charClass));

                parseState = ParseState.TagName;
            }

            if (parseCurrent != runStart)
            {
                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);
                nameLength += parseCurrent - runStart;
            }

            if (!ParseSupport.HtmlEndTagNameCharacter(charClass))
            {
                
                goto ContinueSlowly;
            }

            tokenBuilder.EndTagName(nameLength);

    ScanNextAttributeOrEnd:

            tokenBuilder.AssertPreparedToAddMoreRuns(2);

            

            if (ParseSupport.WhitespaceCharacter(charClass))
            {
                runStart = parseCurrent;

                do
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                while (ParseSupport.WhitespaceCharacter(charClass));

                tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, runStart, parseCurrent);
            }

    CheckEndOfTag:        

            tokenBuilder.AssertPreparedToAddMoreRuns(1);

            if (ch == '>' || (ch == '/' && parseBuffer[parseCurrent + 1] == '>'))
            {
                

                runStart = parseCurrent;

                if (ch == '/')
                {
                    parseCurrent ++;
                    tokenBuilder.SetEmptyScope();
                }
                parseCurrent ++;

                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, runStart, parseCurrent);
                tokenBuilder.EndTag(true);

                InternalDebug.Assert(scanQuote == '\0' && valueQuote == '\0');

                if (parseBuffer[parseCurrent] == '<')    
                {
                    parseState = ParseState.TagStart;
                }
                else
                {
                    parseState = ParseState.Text;
                    slowParse = true;
                }

                this.parseCurrent = parseCurrent;

                HandleSpecialTag();

                return token.TokenId;
            }

            parseState = ParseState.TagWsp;
            if (!ParseSupport.HtmlSimpleAttrNameCharacter(charClass) ||
                !tokenBuilder.CanAddAttribute() ||
                !tokenBuilder.PrepareToAddMoreRuns(11))
            {
                
                goto ContinueSlowly;
            }

            
            

            tokenBuilder.StartAttribute();

            nameLength = 0;

            runStart = parseCurrent;

            parseState = ParseState.AttrNamePrefix;

            do
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }
            while (ParseSupport.HtmlSimpleAttrNameCharacter(charClass));

            if (ch == ':')
            {
                
                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);

                
                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, parseCurrent, parseCurrent + 1);

                tokenBuilder.EndAttributeNamePrefix();

                nameLength = parseCurrent + 1 - runStart;
                runStart = parseCurrent + 1;

                
                do
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                while (ParseSupport.HtmlSimpleAttrNameCharacter(charClass));

                parseState = ParseState.AttrName;
            }

            if (parseCurrent != runStart)
            {
                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);
                nameLength += parseCurrent - runStart;
            }

            if (!ParseSupport.HtmlEndAttrNameCharacter(charClass))
            {
                
                goto ContinueSlowly;
            }

            tokenBuilder.EndAttributeName(nameLength);

            if (ParseSupport.WhitespaceCharacter(charClass))
            {
                runStart = parseCurrent;

                do
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                while (ParseSupport.WhitespaceCharacter(charClass));

                tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, runStart, parseCurrent);

                parseState = ParseState.AttrWsp;
                if (ParseSupport.InvalidUnicodeCharacter(charClass))
                {
                    
                    goto ContinueSlowly;
                }
            }

            if (ch != '=')
            {
                

                tokenBuilder.EndAttribute();

                goto CheckEndOfTag;
            }

            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrEqual, parseCurrent, parseCurrent + 1);

            
            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);

            if (ParseSupport.WhitespaceCharacter(charClass))
            {
                runStart = parseCurrent;

                do
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                while (ParseSupport.WhitespaceCharacter(charClass));

                tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, runStart, parseCurrent);

                parseState = ParseState.AttrValueWsp;
                if (ParseSupport.InvalidUnicodeCharacter(charClass))
                {
                    
                    goto ContinueSlowly;
                }
            }

            if (ParseSupport.QuoteCharacter(charClass))
            {
                valueQuote = ch;

                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, parseCurrent, parseCurrent + 1);

                tokenBuilder.StartValue();
                tokenBuilder.SetValueQuote(valueQuote);

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                if (ParseSupport.HtmlSimpleAttrQuotedValueCharacter(charClass))
                {
                    runStart = parseCurrent;

                    do
                    {
                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    while (ParseSupport.HtmlSimpleAttrQuotedValueCharacter(charClass));

                    
                    tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, runStart, parseCurrent);
                }

                if (ch != valueQuote)
                {
                    scanQuote = valueQuote;
                    parseState = ParseState.AttrValue;
                    
                    goto ContinueSlowly;
                }

                valueQuote = '\0';

                tokenBuilder.EndValue();

                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, parseCurrent, parseCurrent + 1);

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                tokenBuilder.EndAttribute();

                goto ScanNextAttributeOrEnd;
            }
            else if (ParseSupport.HtmlSimpleAttrUnquotedValueCharacter(charClass))
            {
                tokenBuilder.StartValue();

                runStart = parseCurrent;

                do
                {
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                while (ParseSupport.HtmlSimpleAttrUnquotedValueCharacter(charClass));

                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrValue, runStart, parseCurrent);

                parseState = ParseState.AttrValue;
                if (!ParseSupport.HtmlEndAttrUnquotedValueCharacter(charClass))
                {
                    
                    goto ContinueSlowly;
                }

                tokenBuilder.EndValue();

                tokenBuilder.EndAttribute();

                goto ScanNextAttributeOrEnd;
            }

            
            

            parseState = ParseState.AttrValueWsp;

    ContinueSlowly:

            this.parseCurrent = parseCurrent;
            lastCharClass = ParseSupport.GetCharClass(parseBuffer[parseCurrent - 1]);
            this.nameLength = nameLength;

    StartSlowly:

            slowParse = true;
            return ParseSlow();
        }

        
        public HtmlTokenId ParseSlow()
        {
            InternalDebug.Assert(parseCurrent >= parseStart);

            if (tokenBuilder.Valid)
            {
                

                if (tokenBuilder.IncompleteTag)
                {
                    var newBase = tokenBuilder.RewindTag();

                    
                    

                    InternalDebug.Assert(parseCurrent >= newBase);

                    input.ReportProcessed(newBase - parseStart);
                    parseStart = newBase;
                }
                else
                {
                    input.ReportProcessed(parseCurrent - parseStart);
                    parseStart = parseCurrent;

                    tokenBuilder.Reset();
                }
            }

            while (true)
            {
                InternalDebug.Assert(parseThreshold > 0);

                

                var forceFlushToken = false;

                if (parseCurrent + parseThreshold > parseEnd)
                {
                    
                    
                    
                    
                    

                    if (!endOfFile)
                    {
                        if (!input.ReadMore(ref parseBuffer, ref parseStart, ref parseCurrent, ref parseEnd))
                        {
                            

                            
                            InternalDebug.Assert(!tokenBuilder.Valid);

                            return HtmlTokenId.None;
                        }

                        
                        

                        tokenBuilder.BufferChanged(parseBuffer, parseStart);

                        var decodingInput = input as ConverterDecodingInput;

                        if (decodingInput != null && decodingInput.EncodingChanged)
                        {
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            

                            
                            decodingInput.EncodingChanged = false;

                            
                            
                            

                            
                            return tokenBuilder.MakeEmptyToken(HtmlTokenId.EncodingChange, decodingInput.Encoding.CodePage);
                        }

                        if (input.EndOfFile)
                        {
                            endOfFile = true;
                        }

                        
                        if (!endOfFile && parseEnd - parseStart < input.MaxTokenSize)
                        {
                            
                            continue;
                        }
                    }

                    
                    forceFlushToken = true;
                }

                
                InternalDebug.Assert(parseEnd > parseCurrent || forceFlushToken);

                

                

                var ch = parseBuffer[parseCurrent];
                var charClass = ParseSupport.GetCharClass(ch);

                if (ParseSupport.InvalidUnicodeCharacter(charClass) || parseThreshold > 1)
                {
                    
                    
                    
                    
                    

                    var aboveThreshold = SkipInvalidCharacters(ref ch, ref charClass, ref parseCurrent);

                    
                    
                    if (token.IsEmpty)
                    {
                        input.ReportProcessed(parseCurrent - parseStart);
                        parseStart = parseCurrent;

                        if (tokenBuilder.IncompleteTag)
                        {
                            
                            tokenBuilder.BufferChanged(parseBuffer, parseStart);
                        }
                    }

                    if (!aboveThreshold)
                    {
                        

                        if (!forceFlushToken)
                        {
                            
                            continue;
                        }

                        

                        if (parseCurrent == parseEnd && !tokenBuilder.IsStarted && endOfFile)
                        {
                            
                            break;
                        }

                        
                    }

                    
                    parseThreshold = 1;
                }

                if (ParseStateMachine(ch, charClass, forceFlushToken))
                {
                    return token.TokenId;
                }
            }

            return tokenBuilder.MakeEmptyToken(HtmlTokenId.EndOfFile);
        }

        
        public bool ParseStateMachine(char ch, CharClass charClass, bool forceFlushToken)
        {
            char chT;
            CharClass charClassT;

            var tokenBuilder = this.tokenBuilder;
            var parseBuffer = this.parseBuffer;
            var parseCurrent = this.parseCurrent;
            var parseEnd = this.parseEnd;

            var runStart = parseCurrent;

            InternalDebug.Assert(parseBuffer[parseCurrent] == ch);

            switch (parseState)
            {
                

                case ParseState.Text:

                        InternalDebug.Assert(!tokenBuilder.IsStarted);

                        if (ch == '<' && !plaintext)
                        {
                            
                            parseState = ParseState.TagStart;
                            goto case ParseState.TagStart;
                        }

                    ContinueText:

                        tokenBuilder.StartText(runStart);

                        
                        tokenBuilder.DebugPrepareToAddMoreRuns(3);

                        ParseText(ch, charClass, ref parseCurrent);

                        if (token.IsEmpty && !forceFlushToken)
                        {
                            
                            InternalDebug.Assert(parseCurrent == runStart);
                            InternalDebug.Assert(parseState != ParseState.Text || parseThreshold > 1);

                            tokenBuilder.Reset();     
                            slowParse = true;
                            break;  
                        }

                        

                        tokenBuilder.EndText();

                        this.parseCurrent = parseCurrent;

                        return true;

                

                case ParseState.TagStart:

                        InternalDebug.Assert(ch == '<');
                        InternalDebug.Assert(!tokenBuilder.IsStarted);

                        
                        tokenBuilder.DebugPrepareToAddMoreRuns(4);

                        chT = parseBuffer[parseCurrent + 1];        
                        charClassT = ParseSupport.GetCharClass(chT);

                        var endTag = false;

                        if (chT == '/')
                        {
                            

                            chT = parseBuffer[parseCurrent + 2];    
                            charClassT = ParseSupport.GetCharClass(chT);

                            if (ParseSupport.InvalidUnicodeCharacter(charClassT))
                            {
                                
                                
                                

                                if (!endOfFile || parseCurrent + 2 < parseEnd)
                                {
                                    

                                    parseThreshold = 3;    
                                    break;  
                                }

                                
                            }

                            parseCurrent ++;

                            endTag = true;
                        }
                        else if (!ParseSupport.AlphaCharacter(charClassT) || literalTags)
                        {
                            if (chT == '!')
                            {
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                

                                parseState = ParseState.CommentStart;
                                goto case ParseState.CommentStart;
                            }
                            else if (chT == '?' && !literalTags)
                            {
                                parseCurrent += 2;

                                tokenBuilder.StartTag(HtmlNameIndex._DTD, runStart);

                                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

                                tokenBuilder.StartTagText();

                                lastCharClass = charClassT;

                                ch = parseBuffer[parseCurrent];
                                charClass = ParseSupport.GetCharClass(ch);

                                runStart = parseCurrent;

                                parseState = ParseState.Dtd;
                                goto case ParseState.Dtd;
                            }
                            else if (chT == '%')
                            {
                                parseCurrent += 2;

                                tokenBuilder.StartTag(HtmlNameIndex._ASP, runStart);

                                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

                                tokenBuilder.StartTagText();

                                ch = parseBuffer[parseCurrent];
                                charClass = ParseSupport.GetCharClass(ch);

                                runStart = parseCurrent;

                                parseState = ParseState.Asp;
                                goto case ParseState.Asp;
                            }
                            else if (ParseSupport.InvalidUnicodeCharacter(charClassT))
                            {
                                

                                if (!endOfFile || parseCurrent + 1 < parseEnd)
                                {
                                    

                                    parseThreshold = 2;    
                                    break;  
                                }

                                
                            }

                            
                            
                            

                            
                            

                            
                            parseState = ParseState.Text;
                            goto ContinueText;
                        }

                        
                        

                        parseCurrent ++;

                        lastCharClass = charClass;

                        ch = chT;
                        charClass = charClassT;

                        tokenBuilder.StartTag(HtmlNameIndex.Unknown, runStart);
                        if (endTag)
                        {
                            tokenBuilder.SetEndTag();
                        }

                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

                        nameLength = 0;
                        tokenBuilder.StartTagName();

                        runStart = parseCurrent;

                        parseState = ParseState.TagNamePrefix;
                        goto case ParseState.TagNamePrefix;

                

                case ParseState.TagNamePrefix:

                        if (!tokenBuilder.PrepareToAddMoreRuns(2, runStart, HtmlRunKind.Name))
                        {
                            
                            
                            

                            goto SplitTag;
                        }

                        ch = ScanTagName(ch, ref charClass, ref parseCurrent, CharClass.HtmlTagNamePrefix);

                        if (parseCurrent != runStart)
                        {
                            nameLength += (parseCurrent - runStart);

                            if (literalTags && (nameLength > HtmlNameData.MAX_TAG_NAME || ch == '<'))
                            {
                                
                                
                                

                                goto RejectTag;
                            }

                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            goto HandleTagEOB;
                        }

                        if (ch != ':')
                        {
                            goto EndTagName;
                        }

                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, parseCurrent, parseCurrent + 1);
                        nameLength ++;

                        this.tokenBuilder.EndTagNamePrefix();

                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        runStart = parseCurrent;

                        parseState = ParseState.TagName;
                        goto case ParseState.TagName;

                

                case ParseState.TagName:

                        if (!tokenBuilder.PrepareToAddMoreRuns(1, runStart, HtmlRunKind.Name))
                        {
                            
                            
                            

                            goto SplitTag;
                        }

                        ch = ScanTagName(ch, ref charClass, ref parseCurrent, CharClass.HtmlTagName);

                        if (parseCurrent != runStart)
                        {
                            nameLength += (parseCurrent - runStart);

                            if (literalTags && (nameLength > HtmlNameData.MAX_TAG_NAME || ch == '<'))
                            {
                                
                                
                                

                                goto RejectTag;
                            }

                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            goto HandleTagEOB;
                        }

                EndTagName:

                        

                        tokenBuilder.EndTagName(nameLength);

                        InternalDebug.Assert(!literalTags || token.IsEndTag);

                        if (literalTags && token.NameIndex != literalTagNameId)
                        {
                            
                            

                            goto RejectTag;
                        }

                        

                        runStart = parseCurrent;

                        if (ch == '>')
                        {
                            parseState = ParseState.TagEnd;
                            goto case ParseState.TagEnd;
                        }
                        else if (ch == '/')
                        {
                            parseState = ParseState.EmptyTagEnd;
                            goto case ParseState.EmptyTagEnd;
                        }

                        
                        

                        lastCharClass = charClass;

                        parseState = ParseState.TagWsp;
                        goto case ParseState.TagWsp;

                

                case ParseState.TagWsp:

                        
                        if (!tokenBuilder.PrepareToAddMoreRuns(2, runStart, HtmlRunKind.TagWhitespace))
                        {
                            goto SplitTag;
                        }

                        ch = ScanWhitespace(ch, ref charClass, ref parseCurrent);

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            goto HandleTagEOB;
                        }

                        runStart = parseCurrent;

                        if (ch == '>')
                        {
                            parseState = ParseState.TagEnd;
                            goto case ParseState.TagEnd;
                        }
                        else if (ch == '/')
                        {
                            parseState = ParseState.EmptyTagEnd;
                            goto case ParseState.EmptyTagEnd;
                        }

                        parseState = ParseState.AttrNameStart;
                        goto case ParseState.AttrNameStart;

                case ParseState.AttrNameStart:

                        if (!tokenBuilder.CanAddAttribute() || !tokenBuilder.PrepareToAddMoreRuns(3, runStart, HtmlRunKind.Name))
                        {
                            goto SplitTag;
                        }

                        nameLength = 0;

                        tokenBuilder.StartAttribute();

                        parseState = ParseState.AttrNamePrefix;
                        goto case ParseState.AttrNamePrefix;

                

                case ParseState.AttrNamePrefix:

                        InternalDebug.Assert(valueQuote == '\0');

                        
                        if (!tokenBuilder.PrepareToAddMoreRuns(3, runStart, HtmlRunKind.Name))
                        {
                            goto SplitTag;
                        }

                        ch = ScanAttrName(ch, ref charClass, ref parseCurrent, CharClass.HtmlAttrNamePrefix);

                        if (parseCurrent != runStart)
                        {
                            nameLength += (parseCurrent - runStart);

                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            goto HandleTagEOB;
                        }

                        if (ch != ':')
                        {
                            goto EndAttributeName;
                        }

                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, parseCurrent, parseCurrent + 1);
                        nameLength ++;

                        this.tokenBuilder.EndAttributeNamePrefix();

                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        runStart = parseCurrent;

                        parseState = ParseState.AttrName;
                        goto case ParseState.AttrName;

                

                case ParseState.AttrName:

                        InternalDebug.Assert(valueQuote == '\0');

                        
                        if (!tokenBuilder.PrepareToAddMoreRuns(2, runStart, HtmlRunKind.Name))
                        {
                            goto SplitTag;
                        }

                        ch = ScanAttrName(ch, ref charClass, ref parseCurrent, CharClass.HtmlAttrName);

                        if (parseCurrent != runStart)
                        {
                            nameLength += (parseCurrent - runStart);

                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            goto HandleTagEOB;
                        }

                EndAttributeName:

                        tokenBuilder.EndAttributeName(nameLength);

                        runStart = parseCurrent;

                         if (ch == '=')
                        {
                            goto HandleAttrEqual;
                        }

                        InternalDebug.Assert(ch == '>' || ch == '/' || ParseSupport.WhitespaceCharacter(charClass));

                        lastCharClass = charClass;

                        parseState = ParseState.AttrWsp;
                        goto case ParseState.AttrWsp;

                

                case ParseState.AttrWsp:

                        
                        if (!tokenBuilder.PrepareToAddMoreRuns(2, runStart, HtmlRunKind.TagWhitespace))
                        {
                            goto SplitTag;
                        }

                        ch = ScanWhitespace(ch, ref charClass, ref parseCurrent);

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            goto HandleTagEOB;
                        }

                        runStart = parseCurrent;

                        if (ch != '=')
                        {
                            
                            tokenBuilder.EndAttribute();

                            InternalDebug.Assert(valueQuote == '\0');

                            if (ch == '>')
                            {
                                parseState = ParseState.TagEnd;
                                goto case ParseState.TagEnd;
                            }
                            else if (ch == '/')
                            {
                                parseState = ParseState.EmptyTagEnd;
                                goto case ParseState.EmptyTagEnd;
                            }

                            parseState = ParseState.AttrNameStart;
                            goto case ParseState.AttrNameStart;
                        }

                HandleAttrEqual:

                        InternalDebug.Assert(ch == '=');

                        lastCharClass = charClass;

                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrEqual, runStart, parseCurrent);

                        runStart = parseCurrent;

                        parseState = ParseState.AttrValueWsp;
                        goto case ParseState.AttrValueWsp;

                

                case ParseState.AttrValueWsp:

                        
                        if (!tokenBuilder.PrepareToAddMoreRuns(3, runStart, HtmlRunKind.TagWhitespace))
                        {
                            goto SplitTag;
                        }

                        ch = ScanWhitespace(ch, ref charClass, ref parseCurrent);

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            goto HandleTagEOB;
                        }

                        runStart = parseCurrent;

                        InternalDebug.Assert(valueQuote == '\0');

                        if (ParseSupport.QuoteCharacter(charClass))
                        {
                            if (ch == scanQuote)
                            {
                                scanQuote = '\0';
                            }
                            else if (scanQuote == '\0')
                            {
                                InternalDebug.Assert(ParseSupport.HtmlScanQuoteSensitiveCharacter(lastCharClass));
                                scanQuote = ch;
                            }

                            valueQuote = ch;

                            lastCharClass = charClass;

                            ch = parseBuffer[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);

                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, runStart, parseCurrent);

                            runStart = parseCurrent;
                        }

                        tokenBuilder.StartValue();

                        if (valueQuote != '\0')
                        {
                            tokenBuilder.SetValueQuote(valueQuote);
                        }

                        parseState = ParseState.AttrValue;
                        goto case ParseState.AttrValue;
                        
                

                case ParseState.AttrValue:

                        
                        if (!tokenBuilder.PrepareToAddMoreRuns(2, runStart, HtmlRunKind.AttrValue))  
                        {
                            goto SplitTag;
                        }

                        if (!ParseAttributeText(ch, charClass, ref parseCurrent))
                        {
                            goto SplitTag;
                        }

                        ch = parseBuffer[parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        if (ParseSupport.InvalidUnicodeCharacter(charClass) || parseThreshold > 1)
                        {
                            goto HandleTagEOB;
                        }

                        tokenBuilder.EndValue();

                        runStart = parseCurrent;

                        if (ch == valueQuote)
                        {
                            lastCharClass = charClass;

                            ch = parseBuffer[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);

                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, runStart, parseCurrent);

                            valueQuote = '\0';

                            runStart = parseCurrent;
                        }

                        tokenBuilder.EndAttribute();

                        InternalDebug.Assert(valueQuote == '\0');

                        if (ch == '>')
                        {
                            parseState = ParseState.TagEnd;
                            goto case ParseState.TagEnd;
                        }
                        else if (ch == '/')
                        {
                            parseState = ParseState.EmptyTagEnd;
                            goto case ParseState.EmptyTagEnd;
                        }

                        parseState = ParseState.TagWsp;
                        goto case ParseState.TagWsp;

                

                case ParseState.EmptyTagEnd:

                        InternalDebug.Assert(ch == '/');

                        if (!tokenBuilder.PrepareToAddMoreRuns(1, runStart, HtmlRunKind.TagWhitespace))
                        {
                            goto SplitTag;
                        }

                        chT = parseBuffer[parseCurrent + 1];        
                        charClassT = ParseSupport.GetCharClass(chT);

                        if (chT == '>')
                        {
                            

                            tokenBuilder.SetEmptyScope();

                            parseCurrent ++;

                            lastCharClass = charClass;

                            ch = chT;
                            charClass = charClassT;

                            parseState = ParseState.TagEnd;
                            goto case ParseState.TagEnd;
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClassT) && (!endOfFile || parseCurrent + 1 < parseEnd))
                        {
                            parseThreshold = 2;
                            goto HandleTagEOB;
                        }

                        

                        lastCharClass = charClass;

                        parseCurrent ++;

                        ch = chT;
                        charClass = charClassT;

                        runStart = parseCurrent;

                        parseState = ParseState.TagWsp;
                        goto case ParseState.TagWsp;

                

                case ParseState.TagEnd:

                        InternalDebug.Assert(ch == '>');

                        if (!tokenBuilder.PrepareToAddMoreRuns(1, runStart, HtmlRunKind.TagSuffix))
                        {
                            goto SplitTag;
                        }

                        lastCharClass = charClass;

                        parseCurrent ++;

                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, runStart, parseCurrent);

                        if (scanQuote != '\0')
                        {
                            runStart = parseCurrent;

                            ch = parseBuffer[parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);

                            parseState = ParseState.TagSkip;
                            goto case ParseState.TagSkip;
                        }

                        
                        tokenBuilder.EndTag(true);

                        InternalDebug.Assert(scanQuote == '\0' && valueQuote == '\0');

                        if (parseBuffer[parseCurrent] == '<')   
                        {
                            parseState = ParseState.TagStart;
                            slowParse = false;             
                        }
                        else
                        {
                            parseState = ParseState.Text;
                        }

                        this.parseCurrent = parseCurrent;

                        HandleSpecialTag();

                        return true;

                

                case ParseState.TagSkip:

                        

                        if (!tokenBuilder.PrepareToAddMoreRuns(1, runStart, HtmlRunKind.TagText))
                        {
                            goto SplitTag;
                        }

                        ch = ScanSkipTag(ch, ref charClass, ref parseCurrent);

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, runStart, parseCurrent);
                        }

                        if (ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            
                            goto HandleTagEOB;
                        }

                        InternalDebug.Assert(ch == '>' && scanQuote == '\0');

                        parseCurrent ++;

                        
                        tokenBuilder.EndTag(true);

                        InternalDebug.Assert(scanQuote == '\0' && valueQuote == '\0');

                        if (parseBuffer[parseCurrent] == '<')    
                        {
                            parseState = ParseState.TagStart;
                            slowParse = false;
                        }
                        else
                        {
                            parseState = ParseState.Text;
                        }

                        this.parseCurrent = parseCurrent;

                        HandleSpecialTag();

                        return true;

                
                

                HandleTagEOB:

                        if (!forceFlushToken || parseCurrent + parseThreshold < parseEnd)
                        {
                            

                            break;  
                        }

                        

                        if (endOfFile)
                        {
                            if (parseCurrent < parseEnd)
                            {
                                

                                if (ScanForInternalInvalidCharacters(parseCurrent))
                                {
                                    
                                    break;  
                                }

                                
                                
                                parseCurrent = parseEnd;
                            }

                            

                            if (token.IsTagBegin)
                            {
                                

                                goto RejectTag;
                            }

                            
                            

                            tokenBuilder.EndTag(true);

                            this.parseCurrent = parseCurrent;

                            HandleSpecialTag();

                            parseState = ParseState.Text;

                            return true;
                        }

                        
                        

                SplitTag:

                        if (literalTags && token.NameIndex == HtmlNameIndex.Unknown)
                        {
                            
                            
                            
                            InternalDebug.Assert(token.IsTagBegin);
                            goto RejectTag;
                        }

                        

                        tokenBuilder.EndTag(false);

                        this.parseCurrent = parseCurrent;

                        HandleSpecialTag();

                        return true;

                RejectTag:

                        

                        
                        
                        InternalDebug.Assert(token.IsTagBegin);

                        parseCurrent = parseStart;

                        scanQuote = valueQuote = '\0';

                        tokenBuilder.Reset();

                        runStart = parseCurrent;

                        

                        ch = parseBuffer[parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        InternalDebug.Assert(ch == '<');

                        parseState = ParseState.Text;
                        goto ContinueText;

                

                case ParseState.CommentStart:                   

                        
                        
                        tokenBuilder.DebugPrepareToAddMoreRuns(3);

                        InternalDebug.Assert(ch == '<');
                        InternalDebug.Assert(parseBuffer[parseCurrent + 1] == '!');
                        InternalDebug.Assert(!tokenBuilder.IsStarted);

                        

                        var pos = 2;

                        chT = parseBuffer[parseCurrent + pos];                

                        if (chT == '-')
                        {
                            pos ++;     

                            chT = parseBuffer[parseCurrent + pos];            

                            if (chT == '-')
                            {
                                

                                pos ++;     
                                
                                chT = parseBuffer[parseCurrent + pos];        

                                if (chT == '[' && parseConditionals)
                                {
                                    
                                    
                                    
                                    
                                    

                                    parseCurrent += 5;

                                    tokenBuilder.StartTag(HtmlNameIndex._CONDITIONAL, runStart);

                                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

                                    tokenBuilder.StartTagText();

                                    ch = parseBuffer[parseCurrent];
                                    charClass = ParseSupport.GetCharClass(ch);

                                    runStart = parseCurrent;

                                    parseState = ParseState.CommentConditional;
                                    goto case ParseState.CommentConditional;
                                }
                                else if (chT == '>')
                                {
                                    

                                    parseCurrent += 5;

                                    tokenBuilder.StartTag(HtmlNameIndex._COMMENT, runStart);

                                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent - 1);
                                    tokenBuilder.StartTagText();
                                    tokenBuilder.EndTagText();
                                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, parseCurrent - 1, parseCurrent);

                                    tokenBuilder.EndTag(true);

                                    InternalDebug.Assert(scanQuote == '\0' && valueQuote == '\0');

                                    parseState = ParseState.Text;
                                    
                                    this.parseCurrent = parseCurrent;

                                    return true;
                                }
                                else if (chT == '-')
                                {
                                    pos ++;     

                                    chT = parseBuffer[parseCurrent + pos];    

                                    if (chT == '>')
                                    {
                                        

                                        parseCurrent += 6;

                                        tokenBuilder.StartTag(HtmlNameIndex._COMMENT, runStart);

                                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent - 2);
                                        tokenBuilder.StartTagText();
                                        tokenBuilder.EndTagText();
                                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, parseCurrent - 2, parseCurrent);

                                        tokenBuilder.EndTag(true);

                                        InternalDebug.Assert(scanQuote == '\0' && valueQuote == '\0');

                                        parseState = ParseState.Text;
                                        
                                        this.parseCurrent = parseCurrent;

                                        return true;
                                    }
                                }

                                charClassT = ParseSupport.GetCharClass(chT);

                                if (!ParseSupport.InvalidUnicodeCharacter(charClassT))
                                {
                                    parseCurrent += 4;

                                    tokenBuilder.StartTag(HtmlNameIndex._COMMENT, runStart);

                                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

                                    tokenBuilder.StartTagText();

                                    ch = parseBuffer[parseCurrent];
                                    charClass = ParseSupport.GetCharClass(ch);

                                    runStart = parseCurrent;

                                    parseState = ParseState.Comment;
                                    goto case ParseState.Comment;
                                }
                            }
                        }
                        else if (chT == '[' && parseConditionals)
                        {
                            
                            
                            
                            
                            

                            parseCurrent += 3;

                            tokenBuilder.StartTag(HtmlNameIndex._CONDITIONAL, runStart);

                            tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

                            tokenBuilder.StartTagText();

                            ch = parseBuffer[parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);

                            runStart = parseCurrent;

                            parseState = ParseState.Conditional;
                            goto case ParseState.Conditional;
                        }

                        
                        

                        charClassT = ParseSupport.GetCharClass(chT);

                        if (ParseSupport.InvalidUnicodeCharacter(charClassT))
                        {
                            

                            

                            if (!endOfFile || parseCurrent + pos < parseEnd)
                            {
                                

                                parseThreshold = pos + 1;
                                break;  
                            }

                            

                            InternalDebug.Assert(ch == '<');

                            parseState = ParseState.Text;
                            goto ContinueText;
                        }

                        

                        if (literalTags)
                        {
                            
                            parseState = ParseState.Text;
                            goto ContinueText;
                        }

                        parseCurrent += 2;

                        tokenBuilder.StartTag(HtmlNameIndex._BANG, runStart);

                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, runStart, parseCurrent);

                        tokenBuilder.StartTagText();

                        lastCharClass = ParseSupport.GetCharClass('!');

                        ch = parseBuffer[parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        runStart = parseCurrent;

                        parseState = ParseState.Bang;
                        goto case ParseState.Bang;

                
                
                
                

                case ParseState.CommentConditional:             
                case ParseState.Conditional:                    
                case ParseState.Comment:                        
                case ParseState.Bang:                           
                case ParseState.Asp:                            
                case ParseState.Dtd:                            

                        if (!tokenBuilder.PrepareToAddMoreRuns(2, runStart, HtmlRunKind.TagText))
                        {
                            goto SplitTag;
                        }

                        while (!ParseSupport.InvalidUnicodeCharacter(charClass))
                        {
                            

                            if (ParseSupport.QuoteCharacter(charClass))
                            {
                                if (ch == scanQuote)
                                {
                                    scanQuote = '\0';
                                }
                                else if (scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(lastCharClass))
                                {
                                    scanQuote = ch;
                                }
                            }
                            else if (ParseSupport.HtmlSuffixCharacter(charClass))
                            {
                                int addToTextCnt, tagSuffixCnt;
                                bool endScan;

                                if (CheckSuffix(parseCurrent, ch, out addToTextCnt, out tagSuffixCnt, out endScan))
                                {
                                    if (!endScan)
                                    {
                                        

                                        InternalDebug.Assert(tagSuffixCnt == 0);

                                        parseCurrent += addToTextCnt;

                                        lastCharClass = charClass;
                                        InternalDebug.Assert(!ParseSupport.HtmlScanQuoteSensitiveCharacter(lastCharClass));

                                        ch = parseBuffer[parseCurrent];
                                        charClass = ParseSupport.GetCharClass(ch);

                                        continue;   
                                    }

                                    scanQuote = '\0';

                                    

                                    parseCurrent += addToTextCnt;

                                    if (parseCurrent != runStart)
                                    {
                                        tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, runStart, parseCurrent);
                                    }

                                    tokenBuilder.EndTagText();

                                    if (tagSuffixCnt != 0)
                                    {
                                        runStart = parseCurrent;

                                        parseCurrent += tagSuffixCnt;

                                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, runStart, parseCurrent);
                                    }

                                    tokenBuilder.EndTag(true);

                                    InternalDebug.Assert(scanQuote == '\0' && valueQuote == '\0');

                                    parseState = ParseState.Text;

                                    this.parseCurrent = parseCurrent;

                                    return true;
                                }

                                
                                
                                
                                

                                InternalDebug.Assert(tagSuffixCnt >= 1);

                                parseCurrent += addToTextCnt;
                                parseThreshold = tagSuffixCnt + 1;

                                break; 
                            }

                            lastCharClass = charClass;

                            ch = parseBuffer[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);
                        }

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, runStart, parseCurrent);

                            if (!tokenBuilder.PrepareToAddMoreRuns(2))
                            {
                                
                                goto SplitTag;
                            }
                        }

                        InternalDebug.Assert(ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(parseBuffer[parseCurrent + parseThreshold - 1])));

                        if (forceFlushToken && parseCurrent + parseThreshold > parseEnd)
                        {
                            
                            

                            if (endOfFile && parseCurrent < parseEnd)
                            {
                                
                                
                                tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, parseCurrent, parseEnd);

                                parseCurrent = parseEnd;
                            }

                            tokenBuilder.EndTag(endOfFile);

                            this.parseCurrent = parseCurrent;

                            return true;
                        }

                        
                        break;  

                

                default:
                        InternalDebug.Assert(false);  
                        this.parseCurrent = parseCurrent;
                        throw new TextConvertersException("internal error: invalid parse state");

            }

            this.parseCurrent = parseCurrent;
            return false;
        }

        
        private static void ProcessNumericEntityValue(int entityValue, out int literal)
        {
            if (entityValue < 0x10000)
            {
                if (0x80 <= entityValue && entityValue <= 0x9F)
                {
                    literal = ParseSupport.Latin1MappingInUnicodeControlArea(entityValue);
                }
                else if (ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass((char)entityValue)))
                {
                    literal = '?';
                }
                else
                {
                    literal = entityValue;
                }
            }
            else if (entityValue < 0x110000)
            {
                literal = entityValue;
            }
            else
            {
                literal = '?';
            }
        }

        
        private static bool FindEntityByHashName(short hash, char[] buffer, int nameOffset, int nameLength, out int entityValue)
        {
            entityValue = 0;

            var found = false;
            var nameIndex = HtmlNameData.entityHashTable[hash];

            if (nameIndex > 0)
            {
                do
                {
                    if (HtmlNameData.entities[(int)nameIndex].name.Length == nameLength)
                    {
                        int i;

                        

                        for (i = 0; i < nameLength; i++)
                        {
                            if (HtmlNameData.entities[(int)nameIndex].name[i] != (char)buffer[nameOffset + i])
                            {
                                break;
                            }
                        }

                        if (i == nameLength)
                        {
                            entityValue = (int)HtmlNameData.entities[(int)nameIndex].value;
                            found = true;
                            break;
                        }
                    }

                    
                    
                    nameIndex ++;
                }
                while (HtmlNameData.entities[(int)nameIndex].hash == hash);

            }
            
            return found;
        }

        
        private bool SkipInvalidCharacters(ref char ch, ref CharClass charClass, ref int parseCurrent)
        {
            

            var current = parseCurrent;
            var end = parseEnd;

            while (ParseSupport.InvalidUnicodeCharacter(charClass) && current < end)
            {
                ch = parseBuffer[++current];
                charClass = ParseSupport.GetCharClass(ch);
            }

            
            

            if (parseThreshold > 1 && current + 1 < end)
            {
                InternalDebug.Assert(!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(parseBuffer[current])));

                var dst = current + 1;
                var src = dst;

                while (src < end && dst < current + parseThreshold)
                {
                    var chT = parseBuffer[src];
                    var classT = ParseSupport.GetCharClass(chT);

                    if (!ParseSupport.InvalidUnicodeCharacter(classT))
                    {
                        if (src != dst)
                        {
                            InternalDebug.Assert(ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(parseBuffer[dst])));

                            parseBuffer[dst] = chT;        
                            parseBuffer[src] = '\0';       
                        }

                        dst ++;
                    }

                    src ++;
                }

                if (src == end)
                {
                    end = parseEnd = input.RemoveGap(dst, end);
                }
            }

            

            parseCurrent = current;

            
            return (current + parseThreshold <= end);
        }

        
        private char ScanTagName(char ch, ref CharClass charClass, ref int parseCurrent, CharClass acceptCharClassSet)
        {
            var parseBuffer = this.parseBuffer;

            while (ParseSupport.IsCharClassOneOf(charClass, acceptCharClassSet))
            {
                if (ParseSupport.QuoteCharacter(charClass))
                {
                    if (ch == scanQuote)
                    {
                        scanQuote = '\0';
                    }
                    else if (scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(lastCharClass))
                    {
                        scanQuote = ch;
                    }
                }
                else if (ch == '<' && literalTags)
                {
                    
                    

                    break;
                }

                lastCharClass = charClass;

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            return ch;
        }

        
        private char ScanAttrName(char ch, ref CharClass charClass, ref int parseCurrent, CharClass acceptCharClassSet)
        {
            var parseBuffer = this.parseBuffer;

            while (ParseSupport.IsCharClassOneOf(charClass, acceptCharClassSet))
            {
                if (ParseSupport.QuoteCharacter(charClass))
                {
                    if (ch == scanQuote)
                    {
                        scanQuote = '\0';
                    }
                    else if (scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(lastCharClass))
                    {
                        scanQuote = ch;
                    }

                    if (ch != '`')
                    {
                        
                        

                        parseBuffer[parseCurrent] = '?';
                    }
                }

                lastCharClass = charClass;

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            return ch;
        }

        
        private char ScanWhitespace(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var parseBuffer = this.parseBuffer;

            while (ParseSupport.WhitespaceCharacter(charClass))
            {
                lastCharClass = charClass;

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            return ch;
        }

        
        private char ScanText(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var parseBuffer = this.parseBuffer;

            while (ParseSupport.HtmlTextCharacter(charClass))
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            return ch;
        }

        
        private char ScanAttrValue(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var parseBuffer = this.parseBuffer;

            while (ParseSupport.HtmlAttrValueCharacter(charClass))
            {
                lastCharClass = charClass;
                
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            return ch;
        }

        
        private char ScanSkipTag(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            var parseBuffer = this.parseBuffer;

            while (!ParseSupport.InvalidUnicodeCharacter(charClass) && (ch != '>' || scanQuote != '\0'))
            {
                if (ParseSupport.QuoteCharacter(charClass))
                {
                    if (ch == scanQuote)
                    {
                        scanQuote = '\0';
                    }
                    else if (scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(lastCharClass))
                    {
                        scanQuote = ch;
                    }
                }

                lastCharClass = charClass;

                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            return ch;
        }

        
        private bool ScanForInternalInvalidCharacters(int parseCurrent)
        {
            var parseBuffer = this.parseBuffer;
            char ch;

            do
            {
                ch = parseBuffer[parseCurrent++];
            }
            while (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch)));

            parseCurrent--;

            InternalDebug.Assert(parseCurrent <= parseEnd);

            return parseCurrent < parseEnd;
        }

        
        private void ParseText(char ch, CharClass charClass, ref int parseCurrent)
        {
            var parseEnd = this.parseEnd;
            var parseBuffer = this.parseBuffer;
            var tokenBuilder = this.tokenBuilder;

            var firstRunStart = parseCurrent;
            var runStart = firstRunStart;

            
            tokenBuilder.AssertPreparedToAddMoreRuns(3);
            tokenBuilder.AssertCurrentRunPosition(runStart);

            do
            {
                ch = ScanText(ch, ref charClass, ref parseCurrent);

                if (ParseSupport.WhitespaceCharacter(charClass))
                {
                    if (parseCurrent != runStart)
                    {
                        

                        tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                        runStart = parseCurrent;
                    }

                    if (ch == ' ')
                    {
                        var chT = parseBuffer[parseCurrent + 1];
                        var charClassT = ParseSupport.GetCharClass(chT);

                        if (!ParseSupport.WhitespaceCharacter(charClassT))
                        {
                            

                            ch = chT;
                            charClass = charClassT;

                            parseCurrent ++;

                            tokenBuilder.AddTextRun(RunTextType.Space, runStart, parseCurrent);
                            runStart = parseCurrent;

                            continue;
                        }
                    }

                    

                    ParseWhitespace(ch, charClass, ref parseCurrent);

                    if (parseThreshold > 1)
                    {
                        
                        break;
                    }

                    ch = parseBuffer[parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                else if (ch == '<')
                {
                    
                    

                    if (plaintext || firstRunStart == parseCurrent)
                    {
                        

                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        
                        tokenBuilder.AssertPreparedToAddMoreRuns(3);
                        continue;
                    }

                    

                    if (parseCurrent != runStart)
                    {
                        
                        tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                    }

                    parseState = ParseState.TagStart;
                    slowParse = literalTags;

                    
                    break;
                }
                else if (ch == '&')
                {
                    if (literalEntities)
                    {
                        

                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);

                        
                        tokenBuilder.AssertPreparedToAddMoreRuns(3);
                        continue;
                    }

                    

                    int literal;
                    int consume;

                    if (DecodeEntity(parseCurrent, false, out literal, out consume))
                    {
                        InternalDebug.Assert(consume != 0);

                        if (consume == 1)
                        {
                            
                            

                            ch = parseBuffer[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);

                            
                            tokenBuilder.AssertPreparedToAddMoreRuns(3);
                            continue;
                        }

                        
                        

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                        }

                        
                        

                        if (literal <= 0xFFFF && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)literal)))
                        {
                            

                            switch ((char)literal)
                            {
                                case ' ':
                                        tokenBuilder.AddLiteralTextRun(RunTextType.Space, parseCurrent, parseCurrent + consume, literal);
                                        break;

                                case '\r':
                                        
                                        
                                        
                                        tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, parseCurrent, parseCurrent + consume, literal);
                                        break;

                                case '\n':
                                        tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, parseCurrent, parseCurrent + consume, literal);
                                        break;

                                case '\t':
                                        tokenBuilder.AddLiteralTextRun(RunTextType.Tabulation, parseCurrent, parseCurrent + consume, literal);
                                        break;

                                default:
                                        tokenBuilder.AddLiteralTextRun(RunTextType.UnusualWhitespace, parseCurrent, parseCurrent + consume, literal);
                                        break;
                            }
                        }
                        else
                        {
                            if (literal == 0xA0)
                            {
                                tokenBuilder.AddLiteralTextRun(RunTextType.Nbsp, parseCurrent, parseCurrent + consume, literal);
                            }
                            else
                            {
                                tokenBuilder.AddLiteralTextRun(RunTextType.NonSpace, parseCurrent, parseCurrent + consume, literal);
                            }
                        }

                        parseCurrent += consume;

                        ch = parseBuffer[parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    else
                    {
                        
                        

                        
                        

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                        }

                        
                        parseThreshold = HtmlNameData.MAX_ENTITY_NAME + 2;
                        
                        
                        break;
                    }
                }
                else if (ParseSupport.NbspCharacter(charClass))
                {
                    if (parseCurrent != runStart)
                    {
                        tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                    }

                    runStart = parseCurrent;

                    do
                    {
                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    while (ParseSupport.NbspCharacter(charClass));

                    tokenBuilder.AddTextRun(RunTextType.Nbsp, runStart, parseCurrent);
                }
                else
                {
                    InternalDebug.Assert(ParseSupport.InvalidUnicodeCharacter(charClass));

                    

                    if (parseCurrent != runStart)
                    {
                        tokenBuilder.AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                    }

                    if (parseCurrent >= parseEnd)
                    {
                        
                        break;
                    }

                    
                    

                    do
                    {
                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    while (ParseSupport.InvalidUnicodeCharacter(charClass) && parseCurrent < parseEnd);
                }

                
                runStart = parseCurrent;
            }
            while (tokenBuilder.PrepareToAddMoreRuns(3, runStart, HtmlRunKind.Text));
        }

        
        private bool ParseAttributeText(char ch, CharClass charClass, ref int parseCurrent)
        {
            var runStart = parseCurrent;
            var parseBuffer = this.parseBuffer;
            var tokenBuilder = this.tokenBuilder;
            int consume;

            while (true)
            {
                tokenBuilder.AssertCurrentRunPosition(runStart);
                tokenBuilder.AssertPreparedToAddMoreRuns(2);

                ch = ScanAttrValue(ch, ref charClass, ref parseCurrent);

                if (ParseSupport.QuoteCharacter(charClass))
                {
                    if (ch == scanQuote)
                    {
                        scanQuote = '\0';
                    }
                    else if (scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(lastCharClass))
                    {
                        scanQuote = ch;
                    }

                    lastCharClass = charClass;

                    if (ch == valueQuote)
                    {
                        

                        break;
                    }

                    
                    parseCurrent ++;
                }
                else if (ch == '&')
                {
                    

                    lastCharClass = charClass;

                    
                    

                    int literal;
                    
                    if (DecodeEntity(parseCurrent, true, out literal, out consume))
                    {
                        InternalDebug.Assert(consume != 0);

                        if (consume == 1)
                        {
                            
                            

                            ch = parseBuffer[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);

                            continue;
                        }

                        
                        

                        if (parseCurrent != runStart)
                        {
                            tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, runStart, parseCurrent);
                        }

                        tokenBuilder.AddLiteralRun(RunTextType.Unknown, HtmlRunKind.AttrValue, parseCurrent, parseCurrent + consume, literal);

                        parseCurrent += consume;

                        
                        if (!tokenBuilder.PrepareToAddMoreRuns(2))
                        {
                            return false;
                        }

                        runStart = parseCurrent;
                    }
                    else
                    {
                        
                        

                        
                        parseThreshold = HtmlNameData.MAX_ENTITY_NAME + 2;

                        break;
                    }
                }
                else if (ch == '>')
                {
                    lastCharClass = charClass;

                    if (valueQuote == '\0')
                    {
                        break;
                    }

                    if (scanQuote == '\0')
                    {
                        
                        InternalDebug.Assert(valueQuote != 0);

                        valueQuote = '\0';
                        break;
                    }

                    parseCurrent ++;
                }
                else if (ParseSupport.WhitespaceCharacter(charClass))
                {
                    lastCharClass = charClass;

                    if (valueQuote == '\0')
                    {
                        break;
                    }

                    
#if false
                    if (parseCurrent != runStart)
                    {
                        tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrValue, runStart, parseCurrent);
                    }

                    
                    this.ParseWhitespace(ch, charClass, ref parseCurrent);

                    runStart = parseCurrent;

                    if (this.parseThreshold > 1)
                    {
                        
                        break;
                    }

                    
                    if (!tokenBuilder.PrepareToAddMoreRuns(2))
                    {
                        return false;
                    }
#else
                    parseCurrent ++;
#endif
                }
                else
                {
                    InternalDebug.Assert(ParseSupport.InvalidUnicodeCharacter(charClass));
                    break;
                }

                ch = parseBuffer[parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }

            if (parseCurrent != runStart)
            {
                tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, runStart, parseCurrent);
            }

            return true;
        }

        
        private void ParseWhitespace(char ch, CharClass charClass, ref int parseCurrent)
        {
            CharClass charClassT;
            var runStart = parseCurrent;
            var parseBuffer = this.parseBuffer;
            var tokenBuilder = this.tokenBuilder;

            tokenBuilder.AssertPreparedToAddMoreRuns(1);
            tokenBuilder.AssertCurrentRunPosition(runStart);

            InternalDebug.Assert(ParseSupport.WhitespaceCharacter(charClass));

            

            do
            {
                switch (ch)
                {
                    case ' ':
                            do
                            {
                                ch = parseBuffer[++parseCurrent];
                            }
                            while (ch == ' ');

                            tokenBuilder.AddTextRun(RunTextType.Space, runStart, parseCurrent);
                            break;

                    case '\r':

                            if (parseBuffer[parseCurrent + 1] != '\n')
                            {
                                charClassT = ParseSupport.GetCharClass(parseBuffer[parseCurrent + 1]);

                                if (ParseSupport.InvalidUnicodeCharacter(charClassT))
                                {
                                    if (!endOfFile || parseCurrent + 1 < parseEnd)
                                    {
                                        
                                        
                                        

                                        parseThreshold = 2;
                                        break;
                                    }

                                    
                                }

                                
                            }
                            else
                            {
                                
                                parseCurrent ++;
                            }

                            ch = parseBuffer[++parseCurrent];

                            tokenBuilder.AddTextRun(RunTextType.NewLine, runStart, parseCurrent);
                            break;

                    case '\n':

                            

                            ch = parseBuffer[++parseCurrent];

                            tokenBuilder.AddTextRun(RunTextType.NewLine, runStart, parseCurrent);
                            break;

                    case '\t':

                            do
                            {
                                ch = parseBuffer[++parseCurrent];
                            }
                            while (ch == '\t');

                            tokenBuilder.AddTextRun(RunTextType.Tabulation, runStart, parseCurrent);
                            break;

                    default:

                            InternalDebug.Assert(ch == '\f' || ch == '\v');

                            do
                            {
                                ch = parseBuffer[++parseCurrent];
                            }
                            while (ch == '\f' || ch == '\v');

                            tokenBuilder.AddTextRun(RunTextType.UnusualWhitespace, runStart, parseCurrent);
                            break;
                }

                charClass = ParseSupport.GetCharClass(ch);

                runStart = parseCurrent;
            }
            while (ParseSupport.WhitespaceCharacter(charClass) && tokenBuilder.PrepareToAddMoreRuns(1) && parseThreshold == 1);
        }

        
        private bool CheckSuffix(int parseCurrent, char ch, out int addToTextCnt, out int tagSuffixCnt, out bool endScan)
        {
            InternalDebug.Assert(parseBuffer[parseCurrent] == ch);

            addToTextCnt = 1;
            tagSuffixCnt = 0;
            endScan = false;

            char chT;

            switch (parseState)
            {
                case ParseState.Asp:

                        if (ch != '%')
                        {
                            
                            return true;
                        }

                        chT = parseBuffer[parseCurrent + 1];

                        if (chT == '>')
                        {
                            

                            addToTextCnt = 0;
                            tagSuffixCnt = 2;
                            endScan = true;

                            return true;
                        }

                        if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(chT)))
                        {
                            
                            return true;
                        }

                        

                        addToTextCnt = 0;
                        tagSuffixCnt = 1;

                        return false;

                case ParseState.Bang:
                case ParseState.Dtd:

                        if (ch == '>' && scanQuote == '\0')
                        {
                            addToTextCnt = 0;
                            tagSuffixCnt = 1;
                            endScan = true;
                        }

                        return true;

                case ParseState.Comment:

                        if (ch != '-')
                        {
                            
                            return true;
                        }

                        var nonDash = parseCurrent;

                        do
                        {
                            chT = parseBuffer[++nonDash];
                        }
                        while (chT == '-');

                        if (chT == '>' && nonDash - parseCurrent >= 2)
                        {
                            

                            if (parseState == ParseState.CommentConditional)
                            {
                                parseState = ParseState.Comment;

                                tokenBuilder.AbortConditional(true);
                            }

                            addToTextCnt = nonDash - parseCurrent - 2;
                            tagSuffixCnt = 3;
                            endScan = true;

                            return true;
                        }

                        if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(chT)))
                        {
                            

                            addToTextCnt = nonDash - parseCurrent;

                            return true;
                        }

                        

                        addToTextCnt = (nonDash - parseCurrent > 2) ? (nonDash - parseCurrent - 2) : 0;
                        tagSuffixCnt = nonDash - parseCurrent - addToTextCnt;    

                        return false;

                case ParseState.Conditional:
                case ParseState.CommentConditional:

                        if (ch == '>')
                        {
                            
                            

                            parseState = (parseState == ParseState.CommentConditional) ? ParseState.Comment : ParseState.Bang;

                            tokenBuilder.AbortConditional(parseState == ParseState.Comment);

                            
                            return CheckSuffix(parseCurrent, ch, out addToTextCnt, out tagSuffixCnt, out endScan);
                        }

                        if (ch == '-' && parseState == ParseState.CommentConditional)
                        {
                            

                            goto case ParseState.Comment;
                        }

                        if (ch != ']')
                        {
                            return true;
                        }

                        

                        
                        
                        

                        chT = parseBuffer[parseCurrent + 1];

                        if (chT == '>')
                        {
                            

                            addToTextCnt = 0;
                            tagSuffixCnt = 2;
                            endScan = true;

                            return true;
                        }

                        var numOkChars = 1;

                        if (chT == '-')
                        {
                            numOkChars ++;

                            chT = parseBuffer[parseCurrent + 2];

                            if (chT == '-')
                            {
                                numOkChars ++;

                                chT = parseBuffer[parseCurrent + 3];

                                if (chT == '>')
                                {
                                    

                                    addToTextCnt = 0;
                                    tagSuffixCnt = 4;
                                    endScan = true;

                                    return true;
                                }
                            }
                        }

                        if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(chT)))
                        {
                            addToTextCnt = numOkChars;

                            return true;
                        }

                        addToTextCnt = 0;
                        tagSuffixCnt = numOkChars;

                        return false;

                default:
                        InternalDebug.Assert(false);
                        break;
            }

            return true;
        }

        
        private bool DecodeEntity(int parseCurrent, bool inAttribute, out int literal, out int consume)
        {
            

            
            
            
            
            
            
            

            

            
            
            
            
            
            

            
            
            

            var parseBuffer = this.parseBuffer;

            InternalDebug.Assert(parseBuffer[parseCurrent] == '&');

            char chT;
            CharClass charClassT;
            var entityStart = parseCurrent + 1;
            var entityCurrent = entityStart;
            var charCount = 0;
            var entityValue = 0;

            chT = parseBuffer[entityCurrent];
            charClassT = ParseSupport.GetCharClass(chT);

            if (chT == '#')
            {
                

                chT = parseBuffer[++entityCurrent];
                charClassT = ParseSupport.GetCharClass(chT);

                if (chT == 'x' || chT == 'X')
                {
                    

                    chT = parseBuffer[++entityCurrent];
                    charClassT = ParseSupport.GetCharClass(chT);

                    while (ParseSupport.HexCharacter(charClassT))
                    {
                        charCount ++;
                        entityValue = (entityValue << 4) + ParseSupport.CharToHex(chT);

                        chT = parseBuffer[++entityCurrent];
                        charClassT = ParseSupport.GetCharClass(chT);
                    }

                    
                    

                    
                    
                    
                    
                    

                    
                    

                    if (!ParseSupport.InvalidUnicodeCharacter(charClassT) || 
                        (endOfFile && entityCurrent >= parseEnd) ||
                        charCount > 6)            
                    {
                        if ((inAttribute || chT == ';') && entityValue != 0 && charCount <= 6)
                        {
                            ProcessNumericEntityValue(entityValue, out literal);

                            consume = entityCurrent - parseCurrent;

                            if (chT == ';')
                            {
                                consume ++;
                            }

                            return true;
                        }
                        else
                        {
                            
                            
                            

                            literal = 0;
                            consume = 1;

                            return true;
                        }
                    }

                    
                    

                }
                else
                {
                    

                    while (ParseSupport.NumericCharacter(charClassT))
                    {
                        charCount ++;
                        entityValue = (entityValue * 10) + ParseSupport.CharToDecimal(chT);

                        entityCurrent ++;

                        chT = parseBuffer[entityCurrent];
                        charClassT = ParseSupport.GetCharClass(chT);
                    }

                    
                    
                    

                    if (!ParseSupport.InvalidUnicodeCharacter(charClassT) || 
                        (endOfFile && entityCurrent >= parseEnd) ||
                        charCount > 7)
                    {
                        if (entityValue != 0 && charCount <= 7)
                        {
                            

                            ProcessNumericEntityValue(entityValue, out literal);

                            consume = entityCurrent - parseCurrent;

                            if (chT == ';')
                            {
                                
                                consume ++;
                            }

                            return true;
                        }
                        else
                        {
                            

                            literal = 0;
                            consume = 1;

                            return true;
                        }
                    }

                    
                    
                }
            }
            else
            {
                var hashValues = hashValuesTable;
                if (hashValues == null)
                {
                    hashValues = hashValuesTable = new short[HtmlNameData.MAX_ENTITY_NAME];
                }

                var hashCode = new HashCode(true);
                short hashValue;
                int entityValueT;

                while (ParseSupport.HtmlEntityCharacter(charClassT) && charCount < HtmlNameData.MAX_ENTITY_NAME)
                {
                    hashValue = (short)(((uint)hashCode.AdvanceAndFinalizeHash(chT) ^ HtmlNameData.ENTITY_HASH_MODIFIER) % HtmlNameData.ENTITY_HASH_SIZE);
                    hashValues[charCount++] = hashValue;

                    entityCurrent ++;

                    chT = parseBuffer[entityCurrent];
                    charClassT = ParseSupport.GetCharClass(chT);
                }

                if (!ParseSupport.InvalidUnicodeCharacter(charClassT) || (endOfFile && entityCurrent >= parseEnd))
                {
                    

                    
                    
                    
                    
                    

                    if (charCount > 1)      
                    {
                        if (FindEntityByHashName(hashValues[charCount - 1], parseBuffer, entityStart, charCount, out entityValueT) && 
                            (chT == ';' || entityValueT <= 255))
                        {
                            

                            entityValue = entityValueT;
                        }
                        else if (!inAttribute)
                        {
                            for (var i = charCount - 2; i >= 0; i--)
                            {
                                if (FindEntityByHashName(hashValues[i], parseBuffer, entityStart, i + 1, out entityValueT) && 
                                    entityValueT <= 255)
                                {
                                    

                                    entityValue = entityValueT;
                                    charCount = i + 1;
                                    break;
                                }
                            }
                        }

                        if (entityValue != 0)
                        {
                            
                            InternalDebug.Assert(entityValue <= 0xFFFF);

                            literal = entityValue;
                            consume = charCount + 1;

                            if (parseBuffer[entityStart + charCount] == ';')
                            {
                                consume ++;
                            }

                            return true;
                        }
                    }

                    

                    literal = 0;
                    consume = 1;

                    return true;
                }
            }

            literal = 0;
            consume = 0;

            return false;
        }

        
        private void HandleSpecialTag()
        {
            if (HtmlNameData.names[(int)token.NameIndex].literalTag)
            {
                literalTags = !token.IsEndTag;
                literalTagNameId = literalTags ? token.NameIndex : HtmlNameIndex.Unknown;

                if (HtmlNameData.names[(int)token.NameIndex].literalEnt)
                {
                    
                    
                    
                    
                    
                    
                    
                    
                    

                    literalEntities = !(token.IsEndTag || token.IsEmptyScope);
                }

                slowParse = slowParse || literalTags;
            }

            switch (token.NameIndex)
            {
                case HtmlNameIndex.Meta:

                        if (input is ConverterDecodingInput && detectEncodingFromMetaTag && ((IRestartable)this).CanRestart())
                        {
                            
                            

                            
                            
                            
                            
                            
                            

                            if (token.IsTagBegin)
                            {
                                rightMeta = false;
                                newEncoding = null;
                            }

                            token.Attributes.Rewind();

                            var attrToParse = -1;
                            var isHttpEquivContent = false;

                            foreach (var attr in token.Attributes)
                            {
                                if (attr.NameIndex == HtmlNameIndex.HttpEquiv)
                                {
                                    if (attr.Value.CaseInsensitiveCompareEqual("content-type") ||
                                        attr.Value.CaseInsensitiveCompareEqual("charset"))
                                    {
                                        rightMeta = true;
                                        if (attrToParse != -1)
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        
                                        break;
                                    }
                                }
                                else if (attr.NameIndex == HtmlNameIndex.Content)
                                {
                                    attrToParse = attr.Index;
                                    isHttpEquivContent = true;
                                    if (rightMeta)
                                    {
                                        break;
                                    }
                                }
                                else if (attr.NameIndex == HtmlNameIndex.Charset)
                                {
                                    attrToParse = attr.Index;
                                    isHttpEquivContent = false;
                                    rightMeta = true;
                                    break;
                                }
                            }

                            if (attrToParse != -1)
                            {
                                var strToParse = token.Attributes[attrToParse].Value.GetString(100);
                                var charset = CharsetFromString(strToParse, isHttpEquivContent);

                                if (charset != null)
                                {
                                    Charset.TryGetEncoding(charset, out newEncoding);

                                    
                                    
                                    
                                }
                            }

                            if (rightMeta && newEncoding != null)
                            {
                                (input as ConverterDecodingInput).RestartWithNewEncoding(newEncoding);
                            }

                            token.Attributes.Rewind();
                        }
                        break;

                    case HtmlNameIndex.PlainText:

                        if (!token.IsEndTag)
                        {
                            
                            
                            

                            plaintext = true;
                            literalEntities = true;

                            if (token.IsTagEnd)
                            {
                                
                                

                                parseState = ParseState.Text;
                            }
                        }

                        break;
            }
        }

        
        private static string CharsetFromString(string arg, bool lookForWordCharset)
        {
            var i = 0;

            while (i < arg.Length)
            {
                
                while (i < arg.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[i])))
                {
                    i ++;
                }

                if (i == arg.Length)
                {
                    break;
                }

                if (!lookForWordCharset || 
                    (arg.Length - i >= 7 && string.Equals(arg.Substring(i, 7), "charset", StringComparison.OrdinalIgnoreCase)))
                {
                    if (lookForWordCharset)
                    {
                        i = arg.IndexOf('=', i + 7);

                        if (i < 0)
                        {
                            break;
                        }

                        i ++;

                        
                        while (i < arg.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[i])))
                        {
                            i ++;
                        }

                        if (i == arg.Length)
                        {
                            break;
                        }
                    }

                    var j = i;

                    while (j < arg.Length && arg[j] != ';' && !ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[j])))
                    {
                        j ++;
                    }

                    return arg.Substring(i, j - i);
                }

                i = arg.IndexOf(';', i);

                if (i < 0)
                {
                    break;
                }

                i ++;
            }

            return null;
        }
    }
}

