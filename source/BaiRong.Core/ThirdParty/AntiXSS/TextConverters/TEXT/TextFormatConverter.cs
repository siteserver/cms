// ***************************************************************
// <copyright file="TextFormatConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
    using System;
    using System.IO;
    using System.Text;
    using Globalization;
    using Data.Internal;
    using Test;
    using Format;



    internal class TextFormatConverter : FormatConverter, IProducerConsumer, IDisposable
    {
        protected TextParser parser;

        private FormatOutput output;
#if DEBUG
        protected TestHtmlTrace trace;
#endif
        protected int lineLength;
        protected int newLines;
        protected int spaces;
        protected int nbsps;
        protected bool paragraphStarted;

        protected Injection injection;

 

        public TextFormatConverter(
                    TextParser parser, 
                    FormatOutput output,
                    Injection injection,
                    Stream traceStream, 
                    bool traceShowTokenNum, 
                    int traceStopOnTokenNum,
                    Stream formatConverterTraceStream) :
            base(formatConverterTraceStream)
        {
#if DEBUG
            if (traceStream != null)
            {
                trace = new TestHtmlTrace(traceStream, traceShowTokenNum, traceStopOnTokenNum);
            }
#endif
            this.parser = parser;

            this.output = output;

            if (this.output != null)
            {
                this.output.Initialize(
                        Store,
                        SourceFormat.Text,
                        "converted from text");
            }

            this.injection = injection;

            // open the document container
            InitializeDocument();

            if (this.injection != null && this.injection.HaveHead)
            {
                InternalDebug.Assert(this.output != null);
                // this.injection.Inject(true, this.output);
            }


            OpenContainer(FormatContainerType.Block, false);


            Last.SetProperty(PropertyPrecedence.NonStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Points, 10));
        }

        public TextFormatConverter(
                    TextParser parser, 
                    FormatStore store,
                    Injection injection,
                    Stream traceStream, 
                    bool traceShowTokenNum, 
                    int traceStopOnTokenNum,
                    Stream formatConverterTraceStream) :
            base(store, formatConverterTraceStream)
        {
#if DEBUG
            if (traceStream != null)
            {
                trace = new TestHtmlTrace(traceStream, traceShowTokenNum, traceStopOnTokenNum);
            }
#endif
            this.parser = parser;

            this.injection = injection;

            // open the document container
            InitializeDocument();

            // open the first paragraph container
            OpenContainer(FormatContainerType.Block, false);


            Last.SetProperty(PropertyPrecedence.NonStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Points, 10));
        }



        public void Initialize(string fragment)
        {
            parser.Initialize(fragment);

            lineLength = 0;
            newLines = 0;
            spaces = 0;
            nbsps = 0;
            paragraphStarted = false;
        }


        public override void Run()
        {
            if (output != null && MustFlush)
            {
                if (CanFlush)
                {
                    FlushOutput();
                }
            }
            else if (!EndOfFile)
            {
                var tokenId = parser.Parse();

                if (TextTokenId.None != tokenId)
                {
                    Process(tokenId);
                }
            }
        }

        /////////////////////////////////////////////////////////////

        public bool Flush()
        {
            Run();
            return EndOfFile && !MustFlush;
        }

        void IDisposable.Dispose()
        {
            if (parser != null /*&& this.parser is IDisposable*/)
            {
                ((IDisposable)parser).Dispose();
            }
#if DEBUG
            if (trace != null /*&& this.trace is IDisposable*/)
            {
                ((IDisposable)trace).Dispose();
            }
#endif
            parser = null;
#if DEBUG
            trace = null;
#endif
            GC.SuppressFinalize(this);
        }


        private bool CanFlush => output.CanAcceptMoreOutput;


        private bool FlushOutput()
        {
            InternalDebug.Assert(MustFlush);

            if (output.Flush())
            {
                MustFlush = false;
                return true;
            }

            return false;
        }


        protected void Process(TextTokenId tokenId)
        {
#if DEBUG
            if (trace != null)
            {
                trace.TraceToken(parser.Token, 0);

                if (tokenId == TextTokenId.EndOfFile)
                {
                    trace.Flush();
                }
            }
#endif
            switch (tokenId)
            {
                case TextTokenId.Text:

                    OutputFragmentSimple(parser.Token);
                    break;

                case TextTokenId.EncodingChange:

                    if (output != null && output.OutputCodePageSameAsInput)
                    {
                        var codePage = parser.Token.Argument;

                        #if DEBUG
                        Encoding newOutputEncoding;
                        
                        InternalDebug.Assert(Charset.TryGetEncoding(codePage, out newOutputEncoding));
                        #endif

                        
                        output.OutputEncoding = Charset.GetEncoding(codePage);
                    }
                    break;

                case TextTokenId.EndOfFile:

                    if (injection != null && injection.HaveTail)
                    {
                        AddLineBreak(1);
                        InternalDebug.Assert(output != null);
                        
                    }

                    // close the paragraph container
                    CloseContainer();

                    // close the document container
                    CloseAllContainersAndSetEOF();
                    break;
            }
        }

        
        private void OutputFragmentSimple(TextToken token)
        {
            foreach (var run in token.Runs)
            {
                if (run.IsTextRun)
                {
                    switch (run.TextType)
                    {
                        case RunTextType.NewLine:

                                AddLineBreak(1);
                                break;

                        case RunTextType.Space:
                        case RunTextType.UnusualWhitespace:

                                AddSpace(run.Length);
                                break;

                        case RunTextType.Tabulation:

                                AddTabulation(run.Length);
                                break;

                        case RunTextType.Nbsp:

                                AddNbsp(run.Length);
                                break;

                        case RunTextType.NonSpace:
                        case RunTextType.Unknown:

                                // InternalDebug.Assert(run.IsNormal);
                                AddNonSpaceText(run.RawBuffer, run.RawOffset, run.RawLength/*, TextMapping.Unicode*/);
                                break;

                        default:

                                InternalDebug.Assert(false, "unexpected run text type");
                                break;
                    }
                }
                else if (run.IsSpecial && run.Kind == (uint)TextRunKind.QuotingLevel)
                {
                    // consider: set QuotingLevelDelta property
                }
            }
        }
    }
}

