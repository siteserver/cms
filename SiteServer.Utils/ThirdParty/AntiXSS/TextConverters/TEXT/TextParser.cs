// ***************************************************************
// <copyright file="TextParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ... 
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
    using System;
    using Data.Internal;


    internal class TextParser : IDisposable
    {

        protected ConverterInput input;
        protected bool endOfFile;

        protected char[] parseBuffer;
        protected int parseStart;
        protected int parseCurrent;
        protected int parseEnd;
        protected int parseThreshold = 1;

        // current token
        protected TextTokenBuilder tokenBuilder;
        protected TextToken token;

        protected bool unwrapFlowed;
        protected bool unwrapDelSpace;

        // line state
        protected bool lastSpace;
        protected int lineCount;

        // 2646 decoding state
        protected bool quotingExpected = true;
        protected int quotingLevel;
        protected int lastLineQuotingLevel;
        protected bool lastLineFlowed;
        protected bool signaturePossible = true;


        public TextParser(
                ConverterInput input,
                bool unwrapFlowed,
                bool unwrapDelSp,
                int maxRuns,
                bool testBoundaryConditions)
        {
            this.input = input;

            tokenBuilder = new TextTokenBuilder(null, maxRuns, testBoundaryConditions);

            token = tokenBuilder.Token;

            this.unwrapFlowed = unwrapFlowed;
            unwrapDelSpace = unwrapDelSp;
        }


        public TextToken Token
        {
            get 
            {
                InternalDebug.Assert(tokenBuilder.Valid);
                return token;
            }
        }


        public void Initialize(string fragment)
        {
            InternalDebug.Assert(input is ConverterBufferInput);

            (input as ConverterBufferInput).Initialize(fragment);

            endOfFile = false;

            parseBuffer = null;
            parseStart = 0;
            parseCurrent = 0;
            parseEnd = 0;
            parseThreshold = 1;

            tokenBuilder.Reset();

            lastSpace = false;
            lineCount = 0;

            quotingExpected = true;
            quotingLevel = 0;
            lastLineQuotingLevel = 0;
            lastLineFlowed = false;
            signaturePossible = true;
        }


        public TextTokenId Parse()
        {
            char ch, chT;
            CharClass charClass, charClassT;
            bool forceFlushToken;
            int runStart;

            if (tokenBuilder.Valid)
            {
                // start the new token

                input.ReportProcessed(parseCurrent - parseStart);
                parseStart = parseCurrent;

                tokenBuilder.Reset();

                InternalDebug.Assert(tokenBuilder.TotalLength == 0);
            }

            while (true)
            {
                InternalDebug.Assert(parseThreshold > 0);

                // try to read and decode more input data if necessary

                forceFlushToken = false;

                if (parseCurrent + parseThreshold > parseEnd)
                {
                    if (!endOfFile)
                    {
                        if (!input.ReadMore(ref parseBuffer, ref parseStart, ref parseCurrent, ref parseEnd))
                        {
                            // cannot decode more data until next input chunk is available

                            // we may have incomplete token at this point
                            InternalDebug.Assert(!tokenBuilder.Valid);

                            return TextTokenId.None;
                        }

                        // NOTE: in case of success, ReadMore can move the token in the buffer and / or
                        // switch to a new buffer

                        tokenBuilder.BufferChanged(parseBuffer, parseStart);

                        var decodingInput = input as ConverterDecodingInput;

                        if (decodingInput != null && decodingInput.EncodingChanged)
                        {


                            // reset the flag as required by ConverterInput protocol
                            decodingInput.EncodingChanged = false;

                            // signal encoding change to the caller
                            return tokenBuilder.MakeEmptyToken(TextTokenId.EncodingChange, decodingInput.Encoding.CodePage);
                        }

                        if (input.EndOfFile)
                        {
                            endOfFile = true;
                        }

                        if (!endOfFile && parseEnd - parseStart < input.MaxTokenSize)
                        {
                            // we have successfuly read "something", ensure this something is above threshold
                            continue;
                        }
                    }

                    // end of file or token is too long, need to flush the token as is (split)
                    forceFlushToken = true;
                }

                // we should have read something unless this is EOF
                InternalDebug.Assert(parseEnd > parseCurrent || forceFlushToken);

                // compact, so that the next character (or parseThreshold next characters) are valid.

                // get the next input character

                ch = parseBuffer[parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);

                if (ParseSupport.InvalidUnicodeCharacter(charClass) || parseThreshold > 1)
                {



                    while (ParseSupport.InvalidUnicodeCharacter(charClass) && parseCurrent < parseEnd)
                    {
                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }


                    if (parseThreshold > 1 && parseCurrent + 1 < parseEnd)
                    {
                        InternalDebug.Assert(parseCurrent == parseStart);
                        InternalDebug.Assert(!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(parseBuffer[parseCurrent])));

                        var src = parseCurrent + 1;
                        var dst = parseCurrent + 1;

                        while (src < parseEnd && dst < parseCurrent + parseThreshold)
                        {
                            chT = parseBuffer[src];
                            charClassT = ParseSupport.GetCharClass(chT);

                            if (!ParseSupport.InvalidUnicodeCharacter(charClassT))
                            {
                                if (src != dst)
                                {
                                    InternalDebug.Assert(ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(parseBuffer[dst])));

                                    parseBuffer[dst] = chT;        // move source character
                                    parseBuffer[src] = '\0';       // replace source character with invalid (zero)
                                }

                                dst ++;
                            }
                            
                            src ++;
                        }

                        if (src == parseEnd && parseCurrent + parseThreshold > dst)
                        {
                             Array.Copy(parseBuffer, parseCurrent, parseBuffer, parseEnd - (dst - parseCurrent), dst - parseCurrent);

                            parseCurrent = parseEnd - (dst - parseCurrent);

                            // reporting all invalid characters consumed
                            input.ReportProcessed(parseCurrent - parseStart);
                            parseStart = parseCurrent;
                        }
                    }


                    if (parseCurrent + parseThreshold > parseEnd)
                    {
                        // we still below threshold...

                        if (!forceFlushToken)
                        {
                            // go back and try to read more
                            continue;
                        }

                        // this is the end of file

                        if (parseCurrent == parseEnd && !tokenBuilder.IsStarted && endOfFile)
                        {
                            // EOF and token is empty, just return EOF token
                            break;
                        }

                        // this is the end of file, we cannot make it above threshold but still have some input data. 
                    }

                    // reset the threshold to its default value
                    parseThreshold = 1;
                }

                // now parse the buffer content

                runStart = parseCurrent;

                InternalDebug.Assert(!tokenBuilder.IsStarted);

                tokenBuilder.StartText(runStart);

                while (tokenBuilder.PrepareToAddMoreRuns(9, runStart, RunKind.Text))
                {
                    while (ParseSupport.TextUriCharacter(charClass))
                    {
                        ch = parseBuffer[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }

                    if (ParseSupport.TextNonUriCharacter(charClass))
                    {
                        if (parseCurrent != runStart)
                        {
                            // we have nonempty NWSP run

                            AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                        }

                        runStart = parseCurrent;

                        do
                        {
                            ch = parseBuffer[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);
                        }
                        while (ParseSupport.NbspCharacter(charClass));

                        AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                    }
                    else if (ParseSupport.WhitespaceCharacter(charClass))
                    {
                        if (parseCurrent != runStart)
                        {
                            // we have nonempty NWSP run

                            AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                        }

                        runStart = parseCurrent;

                        if (ch == ' ')
                        {
                            // ordinary space

                            chT = parseBuffer[parseCurrent + 1];
                            charClassT = ParseSupport.GetCharClass(chT);

                            if (!ParseSupport.WhitespaceCharacter(charClassT))
                            {
                                // add single space to text run

                                ch = chT;
                                charClass = charClassT;

                                parseCurrent ++;

                                AddTextRun(RunTextType.Space, runStart, parseCurrent);

                                runStart = parseCurrent;

                                continue;
                            }
                        }

                        // this is a potentially collapsable whitespace, accumulate whitespace run(s)  

                        ParseWhitespace(ch, charClass);

                        if (parseThreshold > 1)
                        {
                            // terminate the text parse loop to read more data
                            break;
                        }

                        runStart = parseCurrent;

                        ch = parseBuffer[parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    else if (ParseSupport.NbspCharacter(charClass))
                    {
                        if (parseCurrent != runStart)
                        {
                            AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                        }

                        runStart = parseCurrent;

                        do
                        {
                            ch = parseBuffer[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);
                        }
                        while (ParseSupport.NbspCharacter(charClass));

                        AddTextRun(RunTextType.Nbsp, runStart, parseCurrent);
                    }
                    else
                    {
                        InternalDebug.Assert(ParseSupport.InvalidUnicodeCharacter(charClass));

                        // finish the "non-whitespace" run

                        if (parseCurrent != runStart)
                        {
                            AddTextRun(RunTextType.NonSpace, runStart, parseCurrent);
                        }

                        if (parseCurrent >= parseEnd)
                        {
                            // end of available input (EOB), flush the current text token
                            break;
                        }

                        // this is just an embedded invalid character, skip any such invalid 
                        // characters and try to continue collecting text

                        do
                        {
                            ch = parseBuffer[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);
                        }
                        while (ParseSupport.InvalidUnicodeCharacter(charClass) && parseCurrent < parseEnd);
                    }

                    // prepare for a new run

                    runStart = parseCurrent;
                }

                if (token.IsEmpty)
                {
                    // text token is empty, we need more data

                    tokenBuilder.Reset();     // reset open text...

                    // reporting everything below parseCurrent consumed
                    input.ReportProcessed(parseCurrent - parseStart);
                    parseStart = parseCurrent;
                    continue;
                }

                // finish the current text token, return anything we have collected so far

                tokenBuilder.EndText();

                return (TextTokenId)token.TokenId;
            }

            return tokenBuilder.MakeEmptyToken(TextTokenId.EndOfFile);
        }



        void IDisposable.Dispose()
        {
            if (input != null /*&& this.input is IDisposable*/)
            {
                ((IDisposable)input).Dispose();
            }

            input = null;
            parseBuffer = null;
            token = null;
            tokenBuilder = null;

            GC.SuppressFinalize(this);
        }


        private void ParseWhitespace(char ch, CharClass charClass)
        {
            CharClass charClassT;
            var runStart = parseCurrent;

            // parse whitespace

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

                            AddTextRun(RunTextType.Space, runStart, parseCurrent);
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

                                    // EOF - there is no LF
                                }

                                // valid character that is not LF or EOF - no need to skip LF
                            }
                            else
                            {
                                // skip LF
                                parseCurrent ++;
                            }

                            ch = parseBuffer[++parseCurrent];

                            AddTextRun(RunTextType.NewLine, runStart, parseCurrent);
                            break;

                    case '\n':

                            // this is a standalone LF, count as CRLF

                            ch = parseBuffer[++parseCurrent];

                            AddTextRun(RunTextType.NewLine, runStart, parseCurrent);
                            break;

                    case '\t':

                            do
                            {
                                ch = parseBuffer[++parseCurrent];
                            }
                            while (ch == '\t');

                            AddTextRun(RunTextType.Tabulation, runStart, parseCurrent);
                            break;

                    default:

                            InternalDebug.Assert(ch == '\v' || ch == '\f');

                            do
                            {
                                ch = parseBuffer[++parseCurrent];
                            }
                            while (ch == '\v' || ch == '\f');

                            AddTextRun(RunTextType.UnusualWhitespace, runStart, parseCurrent);
                            break;
                }

                charClass = ParseSupport.GetCharClass(ch);

                runStart = parseCurrent;
            }
            while (ParseSupport.WhitespaceCharacter(charClass) && tokenBuilder.PrepareToAddMoreRuns(4, runStart, RunKind.Text) && parseThreshold == 1);
        }

        private void AddTextRun(RunTextType textType, int runStart, int runEnd)
        {
            if (!unwrapFlowed)
            {
                // just add the run
                tokenBuilder.AddTextRun(textType, runStart, runEnd);
                return;
            }

            // do unwrapping preprocessing
            AddTextRunUnwrap(textType, runStart, runEnd);
        }


        private void AddTextRunUnwrap(RunTextType textType, int runStart, int runEnd)
        {
            switch (textType)
            {
                case RunTextType.NewLine:

                        if (!lastSpace || (signaturePossible && lineCount == 3))
                        {
                            lastLineFlowed = false;
                            tokenBuilder.AddTextRun(textType, runStart, runEnd);
                        }
                        else
                        {
                            lastLineFlowed = true;
                        }

                        lineCount = 0;
                        lastSpace = false;
                        signaturePossible = true;
                        quotingExpected = true;

                        lastLineQuotingLevel = quotingLevel;
                        quotingLevel = 0;
                        break;

                case RunTextType.Space:
                case RunTextType.UnusualWhitespace:

                        if (quotingExpected)
                        {
                            InternalDebug.Assert(lineCount == 0);

                            runStart ++;    // skip first space at the beginning of line

                            // need to add invalid run to keep it contigous
                            tokenBuilder.SkipRunIfNecessary(runStart, RunKind.Text);

                            if (lastLineQuotingLevel != quotingLevel)
                            {
                                if (lastLineFlowed)
                                {
                                    tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, runStart, runStart, '\n');
                                }

                                tokenBuilder.AddSpecialRun(TextRunKind.QuotingLevel, runStart, quotingLevel);
                                lastLineQuotingLevel = quotingLevel;
                            }

                            quotingExpected = false;
                        }

                        InternalDebug.Assert(lineCount != 0 || lastLineQuotingLevel == quotingLevel);

                        if (runStart != runEnd)
                        {
                            lineCount += runEnd - runStart;
                            lastSpace = true;

                            tokenBuilder.AddTextRun(textType, runStart, runEnd);

                            if (lineCount != 3 || runEnd - runStart != 1)
                            {
                                signaturePossible = false;
                            }
                        }

                        break;

                case RunTextType.Tabulation:

                        if (quotingExpected)
                        {
                            InternalDebug.Assert(lineCount == 0);

                            if (lastLineQuotingLevel != quotingLevel)
                            {
                                if (lastLineFlowed)
                                {
                                    tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, runStart, runStart, '\n');
                                }

                                tokenBuilder.AddSpecialRun(TextRunKind.QuotingLevel, runStart, quotingLevel);
                                lastLineQuotingLevel = quotingLevel;
                            }

                            quotingExpected = false;
                        }

                        InternalDebug.Assert(lineCount != 0 || lastLineQuotingLevel == quotingLevel);

                        tokenBuilder.AddTextRun(textType, runStart, runEnd);

                        lineCount += runEnd - runStart;

                        lastSpace = false;
                        signaturePossible = false;

                        break;

                case RunTextType.Nbsp:
                case RunTextType.NonSpace:

                        if (quotingExpected)
                        {
                            InternalDebug.Assert(lineCount == 0);

                            while (runStart != runEnd && parseBuffer[runStart] == '>')
                            {
                                quotingLevel ++;
                                runStart ++;
                            }

                            // we may have skipped some stuff, in such case need to add invalid run to keep it contigous
                            tokenBuilder.SkipRunIfNecessary(runStart, RunKind.Text);

                            if (runStart != runEnd)
                            {
                                // end of quotation, check the quoting level

                                if (lastLineQuotingLevel != quotingLevel)
                                {
                                    if (lastLineFlowed)
                                    {
                                        tokenBuilder.AddLiteralTextRun(RunTextType.NewLine, runStart, runStart, '\n');
                                    }

                                    tokenBuilder.AddSpecialRun(TextRunKind.QuotingLevel, runStart, quotingLevel);
                                    lastLineQuotingLevel = quotingLevel;
                                }

                                quotingExpected = false;
                            }
                        }

                        if (runStart != runEnd)
                        {
                            InternalDebug.Assert(lineCount != 0 || lastLineQuotingLevel == quotingLevel);

                            tokenBuilder.AddTextRun(textType, runStart, runEnd);

                            lineCount += runEnd - runStart;
                            lastSpace = false;

                            if (lineCount > 2 || parseBuffer[runStart] != '-' || (runEnd - runStart == 2 && parseBuffer[runStart + 1] != '-'))
                            {
                                signaturePossible = false;
                            }
                        }

                        break;

                default:

                        InternalDebug.Assert(false, "unexpected run textType");
                        break;
            }
        }
    }
}

