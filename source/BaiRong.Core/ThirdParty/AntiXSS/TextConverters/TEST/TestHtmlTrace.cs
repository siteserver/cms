// ***************************************************************
// <copyright file="TestHtmlTrace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Test
{
    using System;
    using System.IO;
    using System.Text;
    using Html;
    using Text;

#if DEBUG

    internal class TestHtmlTrace : IDisposable
    {
        private StreamWriter writer;

        private bool showTokenNum;
        private int stopOnTokenNum;
        private int tokenNum;

        private StringBuilder sb = new StringBuilder();

        public TestHtmlTrace(Stream stm, bool showTokenNum, int stopOnTokenNum)
        {
            this.showTokenNum = showTokenNum;
            this.stopOnTokenNum = stopOnTokenNum;
            writer = new StreamWriter( stm );

            writer.WriteLine("TRACE {0}", DateTime.UtcNow.ToString());
            writer.WriteLine();
        }

        public void WriteMessage(string message)
        {
            if (showTokenNum)
            {
                tokenNum ++;
                writer.Write("{0} ", tokenNum);
            }

            writer.WriteLine("!!! {0}", message);

            if (showTokenNum && stopOnTokenNum != 0 && tokenNum == stopOnTokenNum)
            {
                System.Diagnostics.Debugger.Launch();
                System.Diagnostics.Debugger.Break();
            }
        }

        public void TraceToken(Token token, int level)
        {
            var textToken = token as TextToken;
            var htmlToken = token as HtmlToken;

            if (showTokenNum)
            {
                tokenNum ++;
                writer.Write("{0} ", tokenNum);
            }

            switch ((HtmlTokenId)token.TokenId)
            {
                case HtmlTokenId.Tag:

                    writer.WriteLine("TAG/{6}    - {0}{1}{2} {3}{4} \"{5}\"", 
                                htmlToken.IsEndTag ? "/" : "",
                                SfromTagId((int)htmlToken.TagIndex),
                                SfromTagId((int)htmlToken.TagIndex) == SfromNameIndex(htmlToken.NameIndex) ? "" : " !!!",
                                SfromTagPart(htmlToken),
                                htmlToken.IsEmptyScope ? " empty" : "",
                                htmlToken.HasNameFragment ? SfromText(htmlToken.Name.GetString(1024)) : "",
                                level);

                    if (htmlToken.NameIndex < HtmlNameIndex.Unknown)
                    {
                        writer.WriteLine("  unstructured fragment: \"{0}\"", SfromText(htmlToken.UnstructuredContent.GetString(1024)));
                    }

                    if (htmlToken.Attributes.Count != 0)
                    {
                        htmlToken.Attributes.Rewind();
                        
                        var i = 0;

                        foreach (var attr in htmlToken.Attributes)
                        {
                            writer.WriteLine("  attr {0}: {1} \"{2}\" = \"{3}\"{4}", 
                                                i,
                                                SfromNameIndex(attr.NameIndex),
                                                SfromText(attr.Name.GetString(1024)),
                                                SfromText(attr.Value.GetString(1024)),
                                                SfromAttributePart(attr));
                            i++;
                        }

                        htmlToken.Attributes.Rewind();
                    }
                    break;

                case HtmlTokenId.Text:

                    writer.WriteLine("/{0}", level);

                    foreach (var run in token.Runs)
                    {
                        switch (run.TextType)
                        {
                            case RunTextType.Space:
                                writer.WriteLine("  Space ({0})", run.Length);
                                break;
                            case RunTextType.Tabulation:
                                writer.WriteLine("  Tabs ({0})", run.Length);
                                break;
                            case RunTextType.NewLine:
                                writer.WriteLine("  New Line");
                                break;
                            case RunTextType.UnusualWhitespace:
                                writer.WriteLine("  Unusual wsp ({0})", run.Length);
                                break;
                            case RunTextType.NonSpace:
                                writer.WriteLine("  Non-Whitespace text ({0} - \"{1}\")", run.Length, SfromText(run.GetString(1024)));
                                break;
                            case RunTextType.Nbsp:
                                writer.WriteLine("  Nbsp ({0})", run.Length);
                                break;
                            case RunTextType.Unknown:
                                writer.WriteLine("  Unknown ({0} - \"{1}\")", run.Length, SfromText(run.GetString(1024)));
                                break;
                            default:
                                writer.WriteLine("  ??? ({0} - \"{1}\")", run.Length, SfromText(run.GetString(1024)));
                                break;
                        }
                    }

                    token.Runs.Rewind();

                    break;

                case HtmlTokenId.OverlappedClose:
                case HtmlTokenId.OverlappedReopen:

                    writer.WriteLine("OVERLAPPED {0}/{2}    - {1}", (HtmlTokenId)token.TokenId == HtmlTokenId.OverlappedClose ? "CLOSE" : "REOPEN", htmlToken.Argument, level);

                    break;

                case HtmlTokenId.Restart:

                    writer.WriteLine("RESTART/{0}", level);

                    break;

                case HtmlTokenId.EncodingChange:

                    writer.WriteLine("ENCODING/{0}", level);

                    break;

                case HtmlTokenId.EndOfFile:

                    writer.WriteLine("EOF/{0}", level);
                    writer.Flush();
                    break;

                case HtmlTokenId.InjectionBegin:

                    writer.WriteLine("BEGIN INJECTION/{0}", level);
                    break;

                case HtmlTokenId.InjectionEnd:

                    writer.WriteLine("END INJECTION/{0}", level);
                    break;
            }

            writer.Flush();

            if (showTokenNum && stopOnTokenNum != 0 && tokenNum == stopOnTokenNum)
            {
                System.Diagnostics.Debugger.Launch();
                System.Diagnostics.Debugger.Break();
            }
        }

        public void Flush()
        {
            writer.Flush();
            // writer = null;
        }

#if PRIVATEBUILD_UNUSED

        public void Close()
        {
            writer.Close();
            writer = null;
        }

        private string SfromB(bool f)
        {
            return f ? "true" : "false";
        }
#endif

        private static string SfromTagPart(HtmlToken token)
        {
            return token.IsTagComplete ? "" : token.IsTagBegin ? "(begin)" : token.IsTagEnd ? "(end)" : "(middle)";
        }

        private static string SfromAttributePart(HtmlAttribute attr)
        {
            return attr.IsCompleteAttr ? "" : attr.IsAttrBegin ? " (begin)" : attr.IsAttrEnd ? " (end)" : " (cont)";
        }

        private static string SfromNameIndex(HtmlNameIndex nameId)
        {
            return HtmlNameData.names[(int)nameId].name;
        }

        private static string SfromTagId(int tagId)
        {
            if (tagId == (int)HtmlTagIndex.TC)
            {
                return "(Tc)";
            }
            return SfromNameIndex(HtmlDtd.tags[(int)tagId].nameIndex);
        }

        private string SfromText(string fr)
        {
            var nonAscii = false;

            for (var i=0; i<fr.Length; i++)
            {
                if (fr[i] > (char)0x7F || fr[i] < (char)0x20)
                {
                    nonAscii = true;
                    break;
                }
            }

            if (!nonAscii)
            {
                return fr;
            }

            sb.Remove(0, sb.Length);

            for (var i = 0; i < fr.Length; i++)
            {
                if (fr[i] > (char)0x7F || fr[i] < (char)0x20)
                {
                    // this.sb.Append('?');
                    sb.AppendFormat("&#x{0:X};", (int)fr[i]);
                }
                else
                {
                    sb.Append(fr[i]);
                }
            }

            return sb.ToString();
        }

        void IDisposable.Dispose()
        {
            writer.Close();
            writer = null;

            GC.SuppressFinalize(this);
        }
    }

#endif
}

