// ***************************************************************
// <copyright file="HtmlTextExtractionConverter.cs" company="Microsoft">
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


    internal class HtmlTextExtractionConverter : IProducerConsumer, IRestartable, IDisposable
    {
        

        private IHtmlParser parser;
        private bool endOfFile;

        private ConverterOutput output;

        private bool outputAnchorLinks = true;

        private bool insideComment;
        private bool insideAnchor;

        private CollapseWhitespaceState collapseWhitespaceState;

        

        public HtmlTextExtractionConverter(
                    IHtmlParser parser, 
                    ConverterOutput output,
                    Stream traceStream, 
                    bool traceShowTokenNum, 
                    int traceStopOnTokenNum)
        {

            this.output = output;

            this.parser = parser;
            this.parser.SetRestartConsumer(this);
        }

        

        public bool CanRestart()
        {
            return ((IRestartable)output).CanRestart();
        }

        

        public void Restart()
        {
            InternalDebug.Assert(CanRestart());

            ((IRestartable)output).Restart();

            

            endOfFile = false;

            insideComment = false;
            insideAnchor = false;
        }

        

        public void DisableRestart()
        {
            ((IRestartable)output).DisableRestart();
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

        
        

        void IDisposable.Dispose()
        {
            if (parser != null /*&& this.parser is IDisposable*/)
            {
                ((IDisposable)parser).Dispose();
            }

            if (output != null && output is IDisposable)
            {
                ((IDisposable)output).Dispose();
            }

            parser = null;
            output = null;

            GC.SuppressFinalize(this);
        }

        

        private void Process(HtmlTokenId tokenId)
        {
            var token = parser.Token;

            switch (tokenId)
            {
                

                case HtmlTokenId.Tag:

                    
                    

                    if (token.IsTagBegin)
                    {
                        

                        switch (token.TagIndex)
                        {
                            case HtmlTagIndex.Title:

                                    
                                    break;

                            case HtmlTagIndex.Comment:
                            case HtmlTagIndex.Script:
                            case HtmlTagIndex.Style:

                                    insideComment = !token.IsEndTag;
                                    break;

                            case HtmlTagIndex.A:

                                    if (outputAnchorLinks)
                                    {
                                        insideAnchor = !token.IsEndTag;
                                        if (!token.IsEndTag)
                                        {

                                        }
                                        else
                                        {
                                            
                                        }
                                    }
                                    break;

                            case HtmlTagIndex.Base:
                            case HtmlTagIndex.BaseFont:
                            case HtmlTagIndex.BGSound:
                            case HtmlTagIndex.Link:
                            case HtmlTagIndex.FrameSet:
                            case HtmlTagIndex.Frame:
                            case HtmlTagIndex.Iframe:

                                    
                                    break;

                            case HtmlTagIndex.Map:
                            case HtmlTagIndex.Div:
                            case HtmlTagIndex.P:
                            case HtmlTagIndex.H1:
                            case HtmlTagIndex.H2:
                            case HtmlTagIndex.H3:
                            case HtmlTagIndex.H4:
                            case HtmlTagIndex.H5:
                            case HtmlTagIndex.H6:
                            case HtmlTagIndex.Center:
                            case HtmlTagIndex.BlockQuote:
                            case HtmlTagIndex.Address:
                            case HtmlTagIndex.Marquee:
                            case HtmlTagIndex.BR:
                            
                            case HtmlTagIndex.HR:
                            
                            case HtmlTagIndex.Form:
                            case HtmlTagIndex.FieldSet:
                            case HtmlTagIndex.OptGroup:
                            case HtmlTagIndex.Select:
                            case HtmlTagIndex.Option:
                            
                            case HtmlTagIndex.OL:
                            case HtmlTagIndex.UL:
                            case HtmlTagIndex.Dir:
                            case HtmlTagIndex.Menu:
                            case HtmlTagIndex.LI:
                            case HtmlTagIndex.DL:              
                            case HtmlTagIndex.DT:              
                            case HtmlTagIndex.DD:              
                            
                            case HtmlTagIndex.Table:
                            case HtmlTagIndex.Caption:
                            case HtmlTagIndex.ColGroup:
                            case HtmlTagIndex.Col:
                            case HtmlTagIndex.Tbody:
                            case HtmlTagIndex.Thead:
                            case HtmlTagIndex.Tfoot:
                            case HtmlTagIndex.TR:
                            case HtmlTagIndex.TC:              
                            
                            case HtmlTagIndex.Pre:
                            case HtmlTagIndex.PlainText:
                            case HtmlTagIndex.Listing:

                                    collapseWhitespaceState = CollapseWhitespaceState.NewLine;
                                    break;

                            case HtmlTagIndex.TH:
                            case HtmlTagIndex.TD:

                                    if (!token.IsEndTag)
                                    {
                                        output.Write("\t");
                                    }
                                    break;

                            
                            case HtmlTagIndex.NoEmbed:
                            case HtmlTagIndex.NoFrames:

                                    insideComment = !token.IsEndTag;
                                    break;
                        }
                    }

                    
                    

                    switch (token.TagIndex)
                    {
                        case HtmlTagIndex.A:

                                if (!token.IsEndTag && outputAnchorLinks)
                                {
                                    
                                }
                                break;

                        case HtmlTagIndex.Area:

                                if (!token.IsEndTag && outputAnchorLinks)
                                {
                                    
                                }
                                break;

                        case HtmlTagIndex.Img:
                        case HtmlTagIndex.Image:

                                if (!token.IsEndTag && outputAnchorLinks)
                                {
                                    
                                }
                                break;
                    }

                    break;

                

                case HtmlTokenId.Text:

                    if (!insideComment)
                    {
                        token.Text.WriteToAndCollapseWhitespace(output, ref collapseWhitespaceState);
                    }
                    break;

                

                case HtmlTokenId.OverlappedClose:
                case HtmlTokenId.OverlappedReopen:

                    break;

                

                case HtmlTokenId.Restart:

                    break;

                

                case HtmlTokenId.EncodingChange:

                    var encodingOutput = output as ConverterEncodingOutput;
                    if (encodingOutput != null)
                    {
                        var codePage = token.Argument;

                        if (encodingOutput.CodePageSameAsInput)
                        {
                            #if DEBUG
                            Encoding newOutputEncoding;
                            
                            InternalDebug.Assert(Charset.TryGetEncoding(codePage, out newOutputEncoding));
                            #endif

                            
                            encodingOutput.Encoding = Charset.GetEncoding(codePage);
                        }
                    }
                    break;

                

                case HtmlTokenId.EndOfFile:

                    output.Write("\r\n");
                    output.Flush();
                    
                    endOfFile = true;
                    break;
            }
        }
    }
}

