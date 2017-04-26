// ***************************************************************
// <copyright file="TestTextOutputTrace.cs" company="Microsoft">
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

#if DEBUG

    internal class TestTextOutputTrace : IDisposable
    {


        private StreamWriter writer;
        private StringBuilder sb = new StringBuilder();


        public TestTextOutputTrace(Stream stm)
        {
            writer = new StreamWriter(stm);
            // this.writer.WriteLine("Html: {0}, PlainText: {1}, Restartable: {2}", this.output.IsOutputHtml, this.output.IsOutputPlaintext, this.output is IRestartable);
        }

        public void WriteMessage(string message)
        {
            writer.WriteLine(message);
            writer.Flush();
        }

        public void OpenDocument()
        {
            writer.WriteLine("StartDocument()");
            writer.Flush();
        }

        public void CloseDocument()
        {
            writer.WriteLine("EndDocument()");
            writer.Flush();
        }

        public void SetQuotingLevel(int quottingLevel)
        {
            writer.WriteLine("SetQuotingLevel({0})", quottingLevel);
            writer.Flush();
        }

        public void CloseParagraph()
        {
            writer.WriteLine("EndParagraph()");
            writer.Flush();
        }

        public void OutputNewLine()
        {
            writer.WriteLine("NL");
            writer.Flush();
        }

        public void OutputTabulation(int count)
        {
            writer.WriteLine("TAB {0}", count);
            writer.Flush();
        }

        public void OutputSpace(int count)
        {
            writer.WriteLine("SPACE {0}", count);
            writer.Flush();
        }

        public void OutputNbsp(int count)
        {
            writer.WriteLine("NBSP {0}", count);
            writer.Flush();
        }

        public void OutputNonspace(char[] buffer, int offset, int count, TextMapping textMapping)
        {
            writer.WriteLine("TXT ({1}) \"{0}\"", SFromBuffer(buffer, offset, count), textMapping == TextMapping.Unicode ? "U" : "S");
            writer.Flush();
        }

        public void OutputNonspace(string text, TextMapping textMapping)
        {
            writer.WriteLine("TXT ({1}) \"{0}\"", SFromString(text, 0, text.Length), textMapping == TextMapping.Unicode ? "U" : "S");
            writer.Flush();
        }

        public void OutputNonspace(string text, int offset, int length, TextMapping textMapping)
        {
            writer.WriteLine("TXT ({1}) \"{0}\"", SFromString(text, offset, length), textMapping == TextMapping.Unicode ? "U" : "S");
            writer.Flush();
        }

        public void OutputNonspace(int ucs32Literal, TextMapping textMapping)
        {
            writer.WriteLine("TXT ({1}) \"{0}\"", SFromLiteral(ucs32Literal), textMapping == TextMapping.Unicode ? "U" : "S");
        }

        public void OpenAnchor(string url)
        {
            writer.WriteLine("OpenAnchor({0})", url ?? "<null>");
            writer.Flush();
        }

        public void CloseAnchor()
        {
            writer.WriteLine("CloseAnchor()");
            writer.Flush();
        }

        public void CancelAnchor()
        {
            writer.WriteLine("CancelAnchor()");
            writer.Flush();
        }

        public void OutputImage(string url, string alt, int width, int height)
        {
            writer.WriteLine("OutputImage({0}, {1}, {2}, {3})", url ?? "<null>", alt ?? "<null>", width, height);
            writer.Flush();
        }

        public void Flush()
        {
            writer.WriteLine("Flush()");
            writer.Flush();
        }


        void IDisposable.Dispose()
        {
            if (writer != null)
            {
                writer.Close();
            }

            GC.SuppressFinalize(this);
        }

        private string SFromBuffer(char[] buffer, int offset, int count)
        {
            sb.Remove(0, sb.Length);

            for (var i = offset; i < offset + count; i++)
            {
                if (buffer[i] > (char)0x7F || buffer[i] < (char)0x20)
                {
                    sb.AppendFormat("&#x{0:X};", (int)buffer[i]);
                }
                else
                {
                    sb.Append(buffer[i]);
                }
            }

            return sb.ToString();
        }

        private string SFromString(string fr, int offset, int length)
        {
            var nonAscii = false;

            for (var i = offset; i < length; i++)
            {
                if (fr[i] > (char)0x7F || fr[i] < (char)0x20)
                {
                    nonAscii = true;
                    break;
                }
            }

            if (!nonAscii)
            {
                return (offset == 0 && length == fr.Length) ? fr : fr.Substring(offset, length);
            }

            sb.Remove(0, sb.Length);

            for (var i = offset; i < length; i++)
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

        private string SFromLiteral(int ucs32Literal)
        {
            sb.Remove(0, sb.Length);
            sb.AppendFormat("&#x{0:X};", ucs32Literal);
            return sb.ToString();
        }
    }

#endif
}

