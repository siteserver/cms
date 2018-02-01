// ***************************************************************
// <copyright file="HtmlToTextConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
    using System;
    using System.IO;
    using System.Text;
    using Data.Internal;
    using Globalization;
    using Text;
    
    using Format;

    

    internal class HtmlToTextConverter : IProducerConsumer, IRestartable, IReusable, IDisposable
    {
        

        private bool convertFragment;

        private IHtmlParser parser;
        private bool endOfFile;

        private TextOutput output;

        private HtmlToken token;

        private bool treatNbspAsBreakable;
        private bool outputImageLinks = true;
        private bool outputAnchorLinks = true;

        protected bool normalizedInput;

        private NormalizerContext normalizerContext;

        private TextMapping textMapping;

        
        private struct NormalizerContext
        {
            
            public char lastCh;
            public bool oneNL;
            public bool hasSpace;
            public bool eatSpace;
        }

        private bool lineStarted;
        private bool wideGap;
        private bool nextParagraphCloseWideGap = true;
        private bool afterFirstParagraph;
        private bool ignoreNextP;

        
        private int listLevel;
        private int listIndex;
        private bool listOrdered;

        private bool insideComment;
        private bool insidePre;
        private bool insideAnchor;

        private ScratchBuffer urlScratch;
        private int imageHeightPixels;
        private int imageWidthPixels;
        private ScratchBuffer imageAltText;

        private ScratchBuffer scratch;

        private Injection injection;

        private UrlCompareSink urlCompareSink;

        

        public HtmlToTextConverter(
                    IHtmlParser parser,
                    TextOutput output,
                    Injection injection,
                    bool convertFragment,
                    bool preformattedText,
                    bool testTreatNbspAsBreakable, 
                    Stream traceStream, 
                    bool traceShowTokenNum, 
                    int traceStopOnTokenNum)
        {

            normalizedInput = (parser is HtmlNormalizingParser);

            treatNbspAsBreakable = testTreatNbspAsBreakable;

            this.convertFragment = convertFragment;

            this.output = output;

            this.parser = parser;
            this.parser.SetRestartConsumer(this);

            if (!convertFragment)
            {
                this.injection = injection;

                this.output.OpenDocument();

                if (this.injection != null && this.injection.HaveHead)
                {
                    this.injection.Inject(true, this.output);
                }
            }
            else
            {
                insidePre = preformattedText;
            }
        }

        

        private void Reinitialize()
        {
            endOfFile = false;

            normalizerContext.hasSpace = false;
            normalizerContext.eatSpace = false;
            normalizerContext.oneNL = false;
            normalizerContext.lastCh = '\0';

            lineStarted = false;
            wideGap = false;
            nextParagraphCloseWideGap = true;
            afterFirstParagraph = false;
            ignoreNextP = false;

            insideComment = false;
            insidePre = false;
            insideAnchor = false;
            if (urlCompareSink != null)
            {
                urlCompareSink.Reset();
            }

            listLevel = 0;
            listIndex= 0;
            listOrdered = false;

            

            
            if (!convertFragment)
            {
                output.OpenDocument();

                if (injection != null)
                {
                    injection.Reset();

                    if (injection.HaveHead)
                    {
                        injection.Inject(true, output);
                    }
                }
            }

            textMapping = TextMapping.Unicode;

        }

        

        public void Run()
        {
            
            if (!endOfFile)
            {
                var tokenId = parser.Parse();

                if (HtmlTokenId.None != tokenId)
                {
                    Process(tokenId);
                }
            }
        }

        

        public bool Flush()
        {
            if (!endOfFile)
            {
                Run();
            }

            return endOfFile;
        }

        

        private void Process(HtmlTokenId tokenId)
        {
            token = parser.Token;

            switch (tokenId)
            {
                

                case HtmlTokenId.Tag:

                    if (token.TagIndex <= HtmlTagIndex.Unknown)
                    {
                        break;
                    }

                    var tagDef = GetTagDefinition(token.TagIndex);

                    if (normalizedInput)
                    {
                        if (!token.IsEndTag)
                        {
                            if (token.IsTagBegin)
                            {
                                
                                PushElement(tagDef);
                            }

                            ProcessStartTagAttributes(tagDef);
                        }
                        else
                        {
                            if (token.IsTagBegin)
                            {
                                PopElement(tagDef);
                            }
                        }
                    }
                    else
                    {
                        

                        
                        
                        
                        

                        if (!token.IsEndTag)
                        {
                            if (token.IsTagBegin)
                            {
                                LFillTagB(tagDef);
                                PushElement(tagDef);
                                RFillTagB(tagDef);
                            }

                            ProcessStartTagAttributes(tagDef);
                        }
                        else
                        {
                            if (token.IsTagBegin)
                            {
                                LFillTagE(tagDef);
                                PopElement(tagDef);
                                RFillTagE(tagDef);
                            }
                        }
                    }
                    break;

                

                case HtmlTokenId.Text:

                    if (!insideComment)
                    {
                        if (insideAnchor && urlCompareSink.IsActive)
                        {
                            token.Text.WriteTo(urlCompareSink);
                        }

                        if (insidePre)
                        {
                            ProcessPreformatedText();
                        }
                        else if (normalizedInput)
                        {
                            ProcessText();
                        }
                        else
                        {
                            NormalizeProcessText();
                        }
                    }
                    break;


                

                case HtmlTokenId.OverlappedClose:
                case HtmlTokenId.OverlappedReopen:

                    break;

                

                case HtmlTokenId.Restart:

                    break;

                

                case HtmlTokenId.EncodingChange:

                    if (output.OutputCodePageSameAsInput)
                    {
                        var codePage = token.Argument;

                        #if DEBUG
                        Encoding newOutputEncoding;
                        
                        InternalDebug.Assert(Charset.TryGetEncoding(codePage, out newOutputEncoding));
                        #endif

                        
                        
                        output.OutputEncoding = Charset.GetEncoding(codePage);
                    }

                    break;

                

                case HtmlTokenId.EndOfFile:

                    if (lineStarted)
                    {
                        output.OutputNewLine();
                        lineStarted = false;
                    }

                    if (!convertFragment)
                    {
                        if (injection != null && injection.HaveHead)
                        {
                            if (wideGap)
                            {
                                output.OutputNewLine();
                                wideGap = false;
                            }

                            injection.Inject(false, output);
                        }

                        output.CloseDocument();
                        output.Flush();
                    }
                    
                    endOfFile = true;
                    break;

            }
        }

        
        private void PushElement(HtmlDtd.TagDefinition tagDef)
        {
            switch (tagDef.tagIndex)
            {
                case HtmlTagIndex.Title:
                case HtmlTagIndex.Comment:
                case HtmlTagIndex.Script:
                case HtmlTagIndex.Style:
                
                case HtmlTagIndex.NoEmbed:
                case HtmlTagIndex.NoFrames:

                        insideComment = true;
                        break;

                case HtmlTagIndex.A:
                

                        if (insideAnchor)
                        {
                            
                            
                            

                            EndAnchor();
                        }
                        break;

                case HtmlTagIndex.Image:           
                case HtmlTagIndex.Img:

                        break;

                case HtmlTagIndex.TD:
                case HtmlTagIndex.TH:

                        if (lineStarted)
                        {
                            output.OutputTabulation(1);
                        }
                        break;

                case HtmlTagIndex.P:

                        if (!ignoreNextP)
                        {
                            EndParagraph(true);
                        }
                        nextParagraphCloseWideGap = true;
                        break;

                
                case HtmlTagIndex.BR:
                case HtmlTagIndex.Option:

                        EndLine();
                        break;

                
                case HtmlTagIndex.HR:

                        EndParagraph(false);
                        OutputText("________________________________");
                        EndParagraph(false);
                        break;

                
                case HtmlTagIndex.OL:
                case HtmlTagIndex.UL:
                case HtmlTagIndex.Dir:
                case HtmlTagIndex.Menu:

                        EndParagraph(listLevel == 0);

                        if (listLevel < 10)
                        {
                            listLevel ++;

                            if (listLevel == 1)
                            {
                                listIndex = 1;
                                listOrdered = (token.TagIndex == HtmlTagIndex.OL);
                            }
                        }
                        nextParagraphCloseWideGap = false;
                        break;

                case HtmlTagIndex.LI:

                        EndParagraph(false);

                        
                        OutputText("  ");

                        for (var i = 0; i < listLevel - 1; i++)
                        {
                            
                            OutputText("   ");
                        }

                        if (listLevel > 1 || !listOrdered)
                        {
                            OutputText("*");
                            output.OutputSpace(3);
                        }
                        else
                        {
                            var num = listIndex.ToString();

                            OutputText(num);

                            OutputText(".");
                            output.OutputSpace(num.Length == 1 ? 2 : 1);
                            listIndex ++;
                        }
                        break;

                
                case HtmlTagIndex.DL:              

                        EndParagraph(true);
                        break;

                case HtmlTagIndex.DT:              

                        if (lineStarted)
                        {
                            EndLine();
                        }
                        break;

                case HtmlTagIndex.DD:              

                        if (lineStarted)
                        {
                            EndLine();
                        }
                        break;

                
                case HtmlTagIndex.Pre:
                case HtmlTagIndex.PlainText:
                case HtmlTagIndex.Listing:
                case HtmlTagIndex.Xmp:

                        EndParagraph(true);
                        insidePre = true;
                        break;

                case HtmlTagIndex.Font:
                case HtmlTagIndex.Span:

                        break;

                default:
                        if (tagDef.blockElement)
                        {
                            EndParagraph(false);
                        }
                        break;
            }

            ignoreNextP = false;

            if (tagDef.tagIndex == HtmlTagIndex.LI)
            {
                
                ignoreNextP = true;
            }
        }

        private void ProcessStartTagAttributes(HtmlDtd.TagDefinition tagDef)
        {
            switch (tagDef.tagIndex)
            {
                case HtmlTagIndex.A:
                

                        if (outputAnchorLinks)
                        {
                            foreach (var attr in token.Attributes)
                            {
                                if (attr.NameIndex == HtmlNameIndex.Href)
                                {
                                    if (attr.IsAttrBegin)
                                    {
                                        urlScratch.Reset();
                                    }

                                    urlScratch.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);
                                    break;
                                }
                            }

                            if (token.IsTagEnd)
                            {
                                var url = urlScratch.BufferString;

                                url.TrimWhitespace();

                                if (url.Length != 0 && url[0] != '#' && url[0] != '?' && url[0] != ';')
                                {
                                    if (!lineStarted)
                                    {
                                        StartParagraphOrLine();
                                    }

                                    
                                    

                                    var urlString = url.ToString();

                                    if (urlString.IndexOf(' ') != -1)
                                    {
                                        urlString = urlString.Replace(" ", "%20");
                                    }

                                    output.OpenAnchor(urlString);
                                    insideAnchor = true;

                                    if (urlCompareSink == null)
                                    {
                                        urlCompareSink = new UrlCompareSink();
                                    }

                                    urlCompareSink.Initialize(urlString);
                                }

                                urlScratch.Reset();
                            }
                        }
                        break;

                case HtmlTagIndex.Image:           
                case HtmlTagIndex.Img:

                        

                        if (outputImageLinks)
                        {
                            foreach (var attr in token.Attributes)
                            {
                                if (attr.NameIndex == HtmlNameIndex.Src)
                                {
                                    if (attr.IsAttrBegin)
                                    {
                                        urlScratch.Reset();
                                    }

                                    urlScratch.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);
                                }
                                else if (attr.NameIndex == HtmlNameIndex.Alt)
                                {
                                    if (attr.IsAttrBegin)
                                    {
                                        imageAltText.Reset();
                                    }

                                    imageAltText.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);
                                }
                                else if (attr.NameIndex == HtmlNameIndex.Height)
                                {
                                    if (!attr.Value.IsEmpty)
                                    {
                                        PropertyValue value;

                                        if (attr.Value.IsContiguous)
                                        {
                                            value = HtmlSupport.ParseNumber(attr.Value.ContiguousBufferString, HtmlSupport.NumberParseFlags.Length);
                                        }
                                        else
                                        {
                                            scratch.Reset();
                                            scratch.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);
                                            value = HtmlSupport.ParseNumber(scratch.BufferString, HtmlSupport.NumberParseFlags.Length);
                                        }

                                        if (value.IsAbsRelLength)
                                        {
                                            imageHeightPixels = value.PixelsInteger;
                                            if (imageHeightPixels == 0)
                                            {
                                                imageHeightPixels = 1;
                                            }
                                        }
                                    }
                                }
                                else if (attr.NameIndex == HtmlNameIndex.Width)
                                {
                                    if (!attr.Value.IsEmpty)
                                    {
                                        PropertyValue value;

                                        if (attr.Value.IsContiguous)
                                        {
                                            value = HtmlSupport.ParseNumber(attr.Value.ContiguousBufferString, HtmlSupport.NumberParseFlags.Length);
                                        }
                                        else
                                        {
                                            scratch.Reset();
                                            scratch.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);
                                            value = HtmlSupport.ParseNumber(scratch.BufferString, HtmlSupport.NumberParseFlags.Length);
                                        }

                                        if (value.IsAbsRelLength)
                                        {
                                            imageWidthPixels = value.PixelsInteger;
                                            if (imageWidthPixels == 0)
                                            {
                                                imageWidthPixels = 1;
                                            }
                                        }
                                    }
                                }
                            }

                            

                            if (token.IsTagEnd)
                            {
                                string urlString = null;
                                string altString = null;

                                

                                var alt = imageAltText.BufferString;

                                alt.TrimWhitespace();

                                if (alt.Length != 0)
                                {
                                    altString = alt.ToString();
                                }

                                if (altString == null || output.ImageRenderingCallbackDefined)
                                {
                                    var url = urlScratch.BufferString;

                                    url.TrimWhitespace();

                                    if (url.Length != 0)
                                    {
                                        urlString = url.ToString();
                                    }
                                }

                                if (!lineStarted)
                                {
                                    StartParagraphOrLine();
                                }

                                output.OutputImage(urlString, altString, imageWidthPixels, imageHeightPixels);

                                urlScratch.Reset();
                                imageAltText.Reset();
                                imageHeightPixels = 0;
                                imageWidthPixels = 0;
                            }
                        }
                        break;

                case HtmlTagIndex.P:

                        
                        

                        if (token.Attributes.Find(HtmlNameIndex.Class) && token.Attributes.Current.Value.CaseInsensitiveCompareEqual("msonormal"))
                        {
                            
                            
                            

                            wideGap = false;
                            nextParagraphCloseWideGap = false;
                        }
                        break;

                case HtmlTagIndex.Font:

                        foreach (var attr in token.Attributes)
                        {
                            if (attr.NameIndex == HtmlNameIndex.Face)
                            {
                                scratch.Reset();
                                scratch.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);

                                var fontRecognizer = new RecognizeInterestingFontName();

                                for (var i = 0; i < scratch.Length && !fontRecognizer.IsRejected; i++)
                                {
                                    fontRecognizer.AddCharacter(scratch.Buffer[i]);
                                }

                                textMapping = fontRecognizer.TextMapping;
                                break;
                            }
                        }

                        break;


                case HtmlTagIndex.Span:

                        foreach (var attr in token.Attributes)
                        {
                            if (attr.NameIndex == HtmlNameIndex.Style)
                            {
                                scratch.Reset();
                                scratch.AppendHtmlAttributeValue(attr, HtmlSupport.MaxAttributeSize);

                                var fontRecognizer = new RecognizeInterestingFontNameInInlineStyle();

                                for (var i = 0; i < scratch.Length && !fontRecognizer.IsFinished; i++)
                                {
                                    fontRecognizer.AddCharacter(scratch.Buffer[i]);
                                }

                                textMapping = fontRecognizer.TextMapping;
                                break;
                            }
                        }

                        break;

            }
        }

        
        private void PopElement(HtmlDtd.TagDefinition tagDef)
        {
            switch (tagDef.tagIndex)
            {
                case HtmlTagIndex.Title:
                case HtmlTagIndex.Comment:
                case HtmlTagIndex.Script:
                case HtmlTagIndex.Style:
                
                case HtmlTagIndex.NoEmbed:
                case HtmlTagIndex.NoFrames:

                        insideComment = false;
                        break;

                case HtmlTagIndex.A:
                

                        if (insideAnchor)
                        {
                            EndAnchor();
                        }
                        break;

                case HtmlTagIndex.Image:           
                case HtmlTagIndex.Img:

                        break;

                case HtmlTagIndex.TD:
                case HtmlTagIndex.TH:

                        lineStarted = true;
                        break;

                case HtmlTagIndex.P:

                        EndParagraph(nextParagraphCloseWideGap);
                        nextParagraphCloseWideGap = true;
                        break;

                
                case HtmlTagIndex.BR:
                case HtmlTagIndex.Option:

                        EndLine();
                        break;

                
                case HtmlTagIndex.HR:

                        EndParagraph(false);
                        OutputText("________________________________");
                        EndParagraph(false);
                        break;

                
                case HtmlTagIndex.OL:
                case HtmlTagIndex.UL:
                case HtmlTagIndex.Dir:
                case HtmlTagIndex.Menu:

                        if (listLevel != 0)
                        {
                            listLevel --;
                        }

                        EndParagraph(listLevel == 0);
                        break;

                case HtmlTagIndex.DT:              

                        break;

                case HtmlTagIndex.DD:              

                        break;

                
                case HtmlTagIndex.Pre:
                case HtmlTagIndex.PlainText:
                case HtmlTagIndex.Listing:
                case HtmlTagIndex.Xmp:

                        EndParagraph(true);
                        insidePre = false;
                        break;

                case HtmlTagIndex.Font:
                case HtmlTagIndex.Span:

                        textMapping = TextMapping.Unicode;
                        break;

                default:
                        if (tagDef.blockElement)
                        {
                            EndParagraph(false);
                        }
                        break;
            }

            ignoreNextP = false;
        }

        
        private void ProcessText()
        {
            if (!lineStarted)
            {
                StartParagraphOrLine();
            }

            foreach (var run in token.Runs)
            {
                if (run.IsTextRun)
                {
                    if (run.IsAnyWhitespace)
                    {
                        output.OutputSpace(1);
                    }
                    else if (run.TextType == RunTextType.Nbsp)
                    {
                        if (treatNbspAsBreakable)
                        {
                            output.OutputSpace(run.Length);
                        }
                        else
                        {
                            output.OutputNbsp(run.Length);
                        }
                    }
                    else 
                    {
                        if (run.IsLiteral)
                        {
                            output.OutputNonspace(run.Literal, textMapping);
                        }
                        else
                        {
                            output.OutputNonspace(run.RawBuffer, run.RawOffset, run.RawLength, textMapping);
                        }
                    }
                }
            }
        }

        
        private void ProcessPreformatedText()
        {
            if (!lineStarted)
            {
                StartParagraphOrLine();
            }

            foreach (var run in token.Runs)
            {
                if (run.IsTextRun)
                {
                    if (run.IsAnyWhitespace)
                    {
                        switch (run.TextType)
                        {
                            case RunTextType.NewLine:

                                    
                                    output.OutputNewLine();
                                    break;

                            case RunTextType.Space:
                            default:

                                    if (treatNbspAsBreakable)
                                    {
                                        output.OutputSpace(run.Length);
                                    }
                                    else
                                    {
                                        output.OutputNbsp(run.Length);
                                    }
                                    break;

                            case RunTextType.Tabulation:
#if false
                                    int tabPosition = this.output.LineLength() / 8 * 8 + 8 * run.Length;
                                    int extraSpaces = tabPosition - this.output.LineLength();

                                    this.output.OutputSpace(extraSpaces);
#endif
                                    output.OutputTabulation(run.Length);
                                    break;
                        }
                    }
                    else if (run.TextType == RunTextType.Nbsp)
                    {
                        if (treatNbspAsBreakable)
                        {
                            output.OutputSpace(run.Length);
                        }
                        else
                        {
                            output.OutputNbsp(run.Length);
                        }
                    }
                    else 
                    {
                        if (run.IsLiteral)
                        {
                            output.OutputNonspace(run.Literal, textMapping);
                        }
                        else
                        {
                            output.OutputNonspace(run.RawBuffer, run.RawOffset, run.RawLength, textMapping);
                        }
                    }
                }
            }
        }

        
        private void NormalizeProcessText()
        {
            var runs = token.Runs;

            runs.MoveNext(true);

            while (runs.IsValidPosition)
            {
                var run = runs.Current;

                if (run.IsAnyWhitespace)
                {
                    

                    var cntWhitespaces = 0;

                    do
                    {
                        
                        
                        cntWhitespaces += runs.Current.TextType == RunTextType.NewLine ? 1 : 2;
                    }
                    while (runs.MoveNext(true) && (runs.Current.TextType <= RunTextType.LastWhitespace));

                    NormalizeAddSpace(cntWhitespaces == 1);
                }
                else if (run.TextType == RunTextType.Nbsp)
                {
                    NormalizeAddNbsp(run.Length);

                    runs.MoveNext(true);
                }
                else
                {
                    
                    NormalizeAddNonspace(run);

                    runs.MoveNext(true);
                }
            }
        }

         
        private void NormalizeAddNonspace(TokenRun run)
        {
            if (!lineStarted)
            {
                StartParagraphOrLine();
            }

            if (normalizerContext.hasSpace)
            {
                normalizerContext.hasSpace = false;

                

                if ('\0' == normalizerContext.lastCh || !normalizerContext.oneNL || !ParseSupport.TwoFarEastNonHanguelChars(normalizerContext.lastCh, run.FirstChar))
                {
                    output.OutputSpace(1);
                }
            }

            if (run.IsLiteral)
            {
                output.OutputNonspace(run.Literal, textMapping);
            }
            else
            {
                output.OutputNonspace(run.RawBuffer, run.RawOffset, run.RawLength, textMapping);
            }

            normalizerContext.eatSpace = false;
            normalizerContext.lastCh = run.LastChar;
            normalizerContext.oneNL = false;
        }

        
        private void NormalizeAddNbsp(int count)
        {
            if (!lineStarted)
            {
                StartParagraphOrLine();
            }

            if (normalizerContext.hasSpace)
            {
                normalizerContext.hasSpace = false;

                output.OutputSpace(1);
            }

            if (treatNbspAsBreakable)
            {
                output.OutputSpace(count);
            }
            else
            {
                output.OutputNbsp(count);
            }

            normalizerContext.eatSpace = false;
            normalizerContext.lastCh = '\xA0';
            normalizerContext.oneNL = false;
        }

        
        private void NormalizeAddSpace(bool oneNL)
        {
            InternalDebug.Assert(!insidePre);

            if (!normalizerContext.eatSpace && afterFirstParagraph)
            {
                normalizerContext.hasSpace = true;
            }

            if (normalizerContext.lastCh != '\0')
            {
                if (oneNL && !normalizerContext.oneNL)
                {
                    normalizerContext.oneNL = true;
                }
                else
                {
                    normalizerContext.lastCh = '\0';
                }
            }
        }

        
        
        
        

        
        private void LFillTagB(HtmlDtd.TagDefinition tagDef)
        {
            if (!insidePre)
            {
                LFill(tagDef.fill.LB);
            }
        }

        
        private void RFillTagB(HtmlDtd.TagDefinition tagDef)
        {
            if (!insidePre)
            {
                RFill(tagDef.fill.RB);
            }
        }

        
        private void LFillTagE(HtmlDtd.TagDefinition tagDef)
        {
            if (!insidePre)
            {
                LFill(tagDef.fill.LE);
            }
        }

        
        private void RFillTagE(HtmlDtd.TagDefinition tagDef)
        {
            if (!insidePre)
            {
                RFill(tagDef.fill.RE);
            }
        }

        
        private void LFill(HtmlDtd.FillCode codeLeft)
        {
            normalizerContext.lastCh = '\0';

            if (normalizerContext.hasSpace)
            {
                if (codeLeft == HtmlDtd.FillCode.PUT)
                {
                    if (!lineStarted)
                    {
                        StartParagraphOrLine();
                    }

                    output.OutputSpace(1);
                    normalizerContext.eatSpace = true;
                }

                normalizerContext.hasSpace = (codeLeft == HtmlDtd.FillCode.NUL);
            }
        }

        
        private void RFill(HtmlDtd.FillCode code)
        {
            if (code == HtmlDtd.FillCode.EAT)
            {
                normalizerContext.hasSpace = false;
                normalizerContext.eatSpace = true;
            }
            else if (code == HtmlDtd.FillCode.PUT)
            {
                normalizerContext.eatSpace = false;
            }
        }

        
        private static HtmlDtd.TagDefinition GetTagDefinition(HtmlTagIndex tagIndex)
        {
            return tagIndex != HtmlTagIndex._NULL ? HtmlDtd.tags[(int)tagIndex] : null;
        }

        

        private void EndAnchor()
        {
            if (!urlCompareSink.IsMatch)
            {
                if (!lineStarted)
                {
                    StartParagraphOrLine();
                }

                output.CloseAnchor();
            }
            else
            {
                output.CancelAnchor();
            }

            insideAnchor = false;
            urlCompareSink.Reset();
        }

        

        private void OutputText(string text)
        {
            if (!lineStarted)
            {
                StartParagraphOrLine();
            }

            output.OutputNonspace(text, textMapping);
        }

        

        private void StartParagraphOrLine()
        {
            InternalDebug.Assert(!lineStarted);

            if (wideGap)
            {
                if (afterFirstParagraph)
                {
                    output.OutputNewLine();
                }

                wideGap = false;
            }

            lineStarted = true;
            afterFirstParagraph = true;
        }

        

        private void EndLine()
        {
            output.OutputNewLine();
            lineStarted = false;
            wideGap = false;
        }

        

        private void EndParagraph(bool wideGap)
        {
            if (insideAnchor)
            {
                
                EndAnchor();
            }

            if (lineStarted)
            {
                output.OutputNewLine();
                lineStarted = false;
            }

            this.wideGap = (this.wideGap || wideGap);
        }

        

        void IDisposable.Dispose()
        {
            if (parser != null /*&& this.parser is IDisposable*/)
            {
                ((IDisposable)parser).Dispose();
            }

            if (!convertFragment && output != null && output is IDisposable)
            {
                ((IDisposable)output).Dispose();
            }

            if (token != null && token is IDisposable)
            {
                ((IDisposable)token).Dispose();
            }

            parser = null;
            output = null;

            token = null;

            GC.SuppressFinalize(this);
        }

        

        bool IRestartable.CanRestart()
        {
            return convertFragment || ((IRestartable)output).CanRestart();
        }

        

        void IRestartable.Restart()
        {
            InternalDebug.Assert(((IRestartable)this).CanRestart());

            if (!convertFragment)
            {
                ((IRestartable)output).Restart();
            }

            Reinitialize();
        }

        

        void IRestartable.DisableRestart()
        {
            if (!convertFragment)
            {
                ((IRestartable)output).DisableRestart();
            }
        }

        

        void IReusable.Initialize(object newSourceOrDestination)
        {
            InternalDebug.Assert(output is IReusable && parser is IReusable);

            ((IReusable)parser).Initialize(newSourceOrDestination);
            ((IReusable)output).Initialize(newSourceOrDestination);

            Reinitialize();

            parser.SetRestartConsumer(this);
        }

        

        public void Initialize(string fragment, bool preformatedText)
        {
            if (normalizedInput)
            {
                ((HtmlNormalizingParser)parser).Initialize(fragment, preformatedText);
            }
            else
            {
                ((HtmlParser)parser).Initialize(fragment, preformatedText);
            }

            if (!convertFragment)
            {
                ((IReusable)output).Initialize(null);
            }

            Reinitialize();
        }
    }
}

