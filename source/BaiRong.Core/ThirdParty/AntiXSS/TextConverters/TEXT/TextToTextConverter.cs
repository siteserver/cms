// ***************************************************************
// <copyright file="TextToTextConverter.cs" company="Microsoft">
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
    using Data.Internal;
    using Globalization;
    using Test;


    internal class TextToTextConverter : IProducerConsumer, IDisposable
    {
        protected TextParser parser;
        protected bool endOfFile;

        protected TextOutput output;
#if DEBUG
        protected TestHtmlTrace trace;
#endif
        protected bool convertFragment;

        protected int lineLength;
        protected int newLines;
        protected int spaces;
        protected int nbsps;
        protected bool paragraphStarted;

        protected bool treatNbspAsBreakable;

        private bool started;

        protected Injection injection;


        public TextToTextConverter(
                    TextParser parser, 
                    TextOutput output,
                    Injection injection,
                    bool convertFragment,
                    bool treatNbspAsBreakable,
                    Stream traceStream, 
                    bool traceShowTokenNum, 
                    int traceStopOnTokenNum)
        {
#if DEBUG
            if (traceStream != null)
            {
                trace = new TestHtmlTrace(traceStream, traceShowTokenNum, traceStopOnTokenNum);
            }
#endif
            this.treatNbspAsBreakable = treatNbspAsBreakable;

            this.convertFragment = convertFragment;

            this.output = output;
            this.parser = parser;

            if (!this.convertFragment)
            {
                this.injection = injection;
            }
        }

        public void Initialize(string fragment)
        {
            parser.Initialize(fragment);

            endOfFile = false;

            lineLength = 0;
            newLines = 0;
            spaces = 0;
            nbsps = 0;
            paragraphStarted = false;
            started = false;
        }


        public void Run()
        {
            if (endOfFile)
            {
                // nothing to do after the end of file.
                return;
            }

            var tokenId = parser.Parse();

            if (TextTokenId.None == tokenId)
            {
                return;
            }

            Process(tokenId);
        }

        public bool Flush()
        {
            if (!endOfFile)
            {
                Run();
            }

            return endOfFile;
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
            if (!started)
            {
                if (!convertFragment)
                {
                    output.OpenDocument();

                    if (injection != null && injection.HaveHead)
                    {
                        injection.Inject(true, output);
                    }
                }

                output.SetQuotingLevel(0);
                started = true;
            }

            switch (tokenId)
            {
                case TextTokenId.Text:

                    OutputFragmentSimple(parser.Token);
                    break;

                case TextTokenId.EncodingChange:

                    if (!convertFragment)
                    {
                        if (output.OutputCodePageSameAsInput)
                        {
                            var codePage = parser.Token.Argument;

                            #if DEBUG
                            Encoding newOutputEncoding;
                            // we should have already checked this in parser before reporting change
                            InternalDebug.Assert(Charset.TryGetEncoding(codePage, out newOutputEncoding));
                            #endif

                            output.OutputEncoding = Charset.GetEncoding(codePage);
                        }
                    }
                    break;

                case TextTokenId.EndOfFile:

                    if (!convertFragment)
                    {
                        if (injection != null && injection.HaveTail)
                        {
                            if (!output.LineEmpty)
                            {
                                output.OutputNewLine();
                            }

                            injection.Inject(false, output);
                        }

                        output.Flush();
                    }
                    else
                    {
                        output.CloseParagraph();
                    }

                    endOfFile = true;

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

                                output.OutputNewLine();
                                break;

                        case RunTextType.Space:
                        case RunTextType.UnusualWhitespace:

                                output.OutputSpace(run.Length);
                                break;

                        case RunTextType.Tabulation:

                                output.OutputTabulation(run.Length);
                                break;

                        case RunTextType.Nbsp:

                                if (treatNbspAsBreakable)
                                {
                                    output.OutputSpace(run.Length);
                                }
                                else
                                {
                                    output.OutputNbsp(run.Length);
                                }
                                break;

                        case RunTextType.NonSpace:

                                // InternalDebug.Assert(run.IsNormal);
                                output.OutputNonspace(run.RawBuffer, run.RawOffset, run.RawLength, TextMapping.Unicode);
                                break;

                        default:

                                InternalDebug.Assert(false, "unexpected run text type");
                                break;
                    }
                }
                else if (run.IsSpecial && run.Kind == (uint)TextRunKind.QuotingLevel)
                {
                    output.SetQuotingLevel((ushort)run.Value);
                }
            }
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
#if DEBUG
            if (trace != null /*&& this.trace is IDisposable*/)
            {
                ((IDisposable)trace).Dispose();
            }
#endif
            parser = null;
            output = null;
#if DEBUG
            trace = null;
#endif
            GC.SuppressFinalize(this);
        }
    }
}
