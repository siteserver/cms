using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Net;
using System.Xml;
using System.Globalization;

namespace BaiRong.Core.Text.Sgml
{
    public enum LiteralType
    {
        CDATA, SDATA, PI
    };

    public class Entity
    {
        public const Char EOF = (char)65535;
        public string Proxy;
        public string Name;
        public bool Internal;
        public string PublicId;
        public string Uri;
        public string Literal;
        public LiteralType LiteralType;
        public Entity Parent;
        public bool Html;
        public int Line;
        public char Lastchar;
        public bool IsWhitespace;

        Encoding encoding;
        Uri resolvedUri;
        TextReader stm;
        bool weOwnTheStream;
        int lineStart;
        int absolutePos;

        public Entity(string name, string pubid, string uri, string proxy)
        {
            Name = name;
            PublicId = pubid;
            Uri = uri;
            Proxy = proxy;
            Html = (name != null && StringUtilities.EqualsIgnoreCase(name, "html"));
        }

        public Entity(string name, string literal)
        {
            Name = name;
            Literal = literal;
            Internal = true;
        }

        public Entity(string name, Uri baseUri, TextReader stm, string proxy)
        {
            Name = name;
            Internal = true;
            this.stm = stm;
            resolvedUri = baseUri;
            Proxy = proxy;
            Html = (string.Compare(name, "html", true, CultureInfo.InvariantCulture) == 0);
        }

        public Uri ResolvedUri
        {
            get
            {
                if (resolvedUri != null) return resolvedUri;
                else if (Parent != null) return Parent.ResolvedUri;
                return null;
            }
        }

        public int LinePosition => absolutePos - lineStart + 1;

        public char ReadChar()
        {
            var ch = (char)stm.Read();
            if (ch == 0)
            {
                // convert nulls to whitespace, since they are not valid in XML anyway.
                ch = ' ';
            }
            absolutePos++;
            if (ch == 0xa)
            {
                IsWhitespace = true;
                lineStart = absolutePos + 1;
                Line++;
            }
            else if (ch == ' ' || ch == '\t')
            {
                IsWhitespace = true;
                if (Lastchar == 0xd)
                {
                    lineStart = absolutePos;
                    Line++;
                }
            }
            else if (ch == 0xd)
            {
                IsWhitespace = true;
            }
            else
            {
                IsWhitespace = false;
                if (Lastchar == 0xd)
                {
                    Line++;
                    lineStart = absolutePos;
                }
            }
            Lastchar = ch;
            return ch;
        }

        public void Open(Entity parent, Uri baseUri)
        {
            Parent = parent;
            if (parent != null) Html = parent.Html;
            Line = 1;
            if (Internal)
            {
                if (Literal != null)
                    stm = new StringReader(Literal);
            }
            else if (Uri == null)
            {
                Error("Unresolvable entity '{0}'", Name);
            }
            else
            {
                if (baseUri != null)
                {
                    resolvedUri = new Uri(baseUri, Uri);
                }
                else
                {
                    resolvedUri = new Uri(Uri);
                }

                Stream stream = null;
                var e = Encoding.Default;
                switch (resolvedUri.Scheme)
                {
                    case "file":
                        {
                            var path = resolvedUri.LocalPath;
                            stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                        }
                        break;
                    default:
                        //Console.WriteLine("Fetching:" + ResolvedUri.AbsoluteUri);
                        var wr = (HttpWebRequest)WebRequest.Create(ResolvedUri);
                        wr.UserAgent = "Mozilla/4.0 (compatible;);";
                        wr.Timeout = 10000; // in case this is running in an ASPX page.
                        if (Proxy != null) wr.Proxy = new WebProxy(Proxy);
                        wr.PreAuthenticate = false;
                        // Pass the credentials of the process. 
                        wr.Credentials = CredentialCache.DefaultCredentials;

                        var resp = wr.GetResponse();
                        var actual = resp.ResponseUri;
                        if (actual.AbsoluteUri != resolvedUri.AbsoluteUri)
                        {
                            resolvedUri = actual;
                        }
                        var contentType = resp.ContentType.ToLower();
                        var mimeType = contentType;
                        var i = contentType.IndexOf(';');
                        if (i >= 0)
                        {
                            mimeType = contentType.Substring(0, i);
                        }
                        if (StringUtilities.EqualsIgnoreCase(mimeType, "text/html"))
                        {
                            Html = true;
                        }

                        i = contentType.IndexOf("charset");
                        e = Encoding.Default;
                        if (i >= 0)
                        {
                            var j = contentType.IndexOf("=", i);
                            var k = contentType.IndexOf(";", j);
                            if (k < 0) k = contentType.Length;
                            if (j > 0)
                            {
                                j++;
                                var charset = contentType.Substring(j, k - j).Trim();
                                try
                                {
                                    e = Encoding.GetEncoding(charset);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        stream = resp.GetResponseStream();
                        break;

                }
                weOwnTheStream = true;
                var html = new HtmlStream(stream, e);
                encoding = html.Encoding;
                stm = html;
            }
        }

        public Encoding GetEncoding()
        {
            return encoding;
        }

        public void Close()
        {
            if (weOwnTheStream)
                stm.Close();
        }

        public char SkipWhitespace()
        {
            var ch = Lastchar;
            while (ch != EOF && (ch == ' ' || ch == '\r' || ch == '\n' || ch == '\t'))
            {
                ch = ReadChar();
            }
            return ch;
        }

        public string ScanToken(StringBuilder sb, string term, bool nmtoken)
        {
            sb.Length = 0;
            var ch = Lastchar;
            if (nmtoken && ch != '_' && !Char.IsLetter(ch))
            {
                throw new Exception(
                    $"Invalid name start character '{ch}'");
            }
            while (ch != EOF && term.IndexOf(ch) < 0)
            {
                if (!nmtoken || ch == '_' || ch == '.' || ch == '-' || ch == ':' || Char.IsLetterOrDigit(ch))
                {
                    sb.Append(ch);
                }
                else
                {
                    throw new Exception(
                        $"Invalid name character '{ch}'");
                }
                ch = ReadChar();
            }
            return sb.ToString();
        }

        public string ScanLiteral(StringBuilder sb, char quote)
        {
            sb.Length = 0;
            var ch = ReadChar();
            while (ch != EOF && ch != quote)
            {
                if (ch == '&')
                {
                    ch = ReadChar();
                    if (ch == '#')
                    {
                        var charent = ExpandCharEntity();
                        sb.Append(charent);
                        ch = Lastchar;
                    }
                    else
                    {
                        sb.Append('&');
                        sb.Append(ch);
                        ch = ReadChar();
                    }
                }
                else
                {
                    sb.Append(ch);
                    ch = ReadChar();
                }
            }
            ReadChar(); // consume end quote.           
            return sb.ToString();
        }

        public string ScanToEnd(StringBuilder sb, string type, string terminators)
        {
            if (sb != null) sb.Length = 0;
            var start = Line;
            // This method scans over a chunk of text looking for the
            // termination sequence specified by the 'terminators' parameter.
            var ch = ReadChar();
            var state = 0;
            var next = terminators[state];
            while (ch != EOF)
            {
                if (ch == next)
                {
                    state++;
                    if (state >= terminators.Length)
                    {
                        // found it!
                        break;
                    }
                    next = terminators[state];
                }
                else if (state > 0)
                {
                    // char didn't match, so go back and see how much does still match.
                    var i = state - 1;
                    var newstate = 0;
                    while (i >= 0 && newstate == 0)
                    {
                        if (terminators[i] == ch)
                        {
                            // character is part of the terminators pattern, ok, so see if we can
                            // match all the way back to the beginning of the pattern.
                            var j = 1;
                            while (i - j >= 0)
                            {
                                if (terminators[i - j] != terminators[state - j])
                                    break;
                                j++;
                            }
                            if (j > i)
                            {
                                newstate = i + 1;
                            }
                        }
                        else
                        {
                            i--;
                        }
                    }
                    if (sb != null)
                    {
                        i = (i < 0) ? 1 : 0;
                        for (var k = 0; k <= state - newstate - i; k++)
                        {
                            sb.Append(terminators[k]);
                        }
                        if (i > 0) // see if we've matched this char or not
                            sb.Append(ch); // if not then append it to buffer.
                    }
                    state = newstate;
                    next = terminators[newstate];
                }
                else
                {
                    if (sb != null) sb.Append(ch);
                }
                ch = ReadChar();
            }
            if (ch == 0) Error(type + " starting on line {0} was never closed", start);
            ReadChar(); // consume last char in termination sequence.
            if (sb != null) return sb.ToString();
            return "";
        }

        public string ExpandCharEntity()
        {
            var ch = ReadChar();
            var v = 0;
            if (ch == 'x')
            {
                ch = ReadChar();
                for (; ch != EOF && ch != ';'; ch = ReadChar())
                {
                    var p = 0;
                    if (ch >= '0' && ch <= '9')
                    {
                        p = (int)(ch - '0');
                    }
                    else if (ch >= 'a' && ch <= 'f')
                    {
                        p = (int)(ch - 'a') + 10;
                    }
                    else if (ch >= 'A' && ch <= 'F')
                    {
                        p = (int)(ch - 'A') + 10;
                    }
                    else
                    {
                        break;//we must be done!
                        //Error("Hex digit out of range '{0}'", (int)ch);
                    }
                    v = (v * 16) + p;
                }
            }
            else
            {
                for (; ch != EOF && ch != ';'; ch = ReadChar())
                {
                    if (ch >= '0' && ch <= '9')
                    {
                        v = (v * 10) + (int)(ch - '0');
                    }
                    else
                    {
                        break; // we must be done!
                        //Error("Decimal digit out of range '{0}'", (int)ch);
                    }
                }
            }
            if (ch == 0)
            {
                Error("Premature {0} parsing entity reference", ch);
            }
            else if (ch == ';')
            {
                ReadChar();
            }
            // HACK ALERT: IE and Netscape map the unicode characters 
            if (Html && v >= 0x80 & v <= 0x9F)
            {
                // This range of control characters is mapped to Windows-1252!
                var size = CtrlMap.Length;
                var i = v - 0x80;
                var unicode = CtrlMap[i];
                return Convert.ToChar(unicode).ToString();
            }
            return Convert.ToChar(v).ToString();
        }

        static int[] CtrlMap = new int[] {
                                             // This is the windows-1252 mapping of the code points 0x80 through 0x9f.
                                             8364, 129, 8218, 402, 8222, 8230, 8224, 8225, 710, 8240, 352, 8249, 338, 141,
                                             381, 143, 144, 8216, 8217, 8220, 8221, 8226, 8211, 8212, 732, 8482, 353, 8250, 
                                             339, 157, 382, 376
                                         };

        public void Error(string msg)
        {
            throw new Exception(msg);
        }

        public void Error(string msg, char ch)
        {
            var str = (ch == EOF) ? "EOF" : Char.ToString(ch);
            throw new Exception(String.Format(msg, str));
        }

        public void Error(string msg, int x)
        {
            throw new Exception(String.Format(msg, x));
        }

        public void Error(string msg, string arg)
        {
            throw new Exception(String.Format(msg, arg));
        }

        public string Context()
        {
            var p = this;
            var sb = new StringBuilder();
            while (p != null)
            {
                string msg;
                if (p.Internal)
                {
                    msg = $"\nReferenced on line {p.Line}, position {p.LinePosition} of internal entity '{p.Name}'";
                }
                else
                {
                    msg =
                        $"\nReferenced on line {p.Line}, position {p.LinePosition} of '{p.Name}' entity at [{p.ResolvedUri.AbsolutePath}]";
                }
                sb.Append(msg);
                p = p.Parent;
            }
            return sb.ToString();
        }

        public static bool IsLiteralType(string token)
        {
            return (token == "CDATA" || token == "SDATA" || token == "PI");
        }

        public void SetLiteralType(string token)
        {
            switch (token)
            {
                case "CDATA":
                    LiteralType = LiteralType.CDATA;
                    break;
                case "SDATA":
                    LiteralType = LiteralType.SDATA;
                    break;
                case "PI":
                    LiteralType = LiteralType.PI;
                    break;
            }
        }
    }

    // This class decodes an HTML/XML stream correctly.
    internal class HtmlStream : TextReader
    {
        Stream stm;
        byte[] rawBuffer;
        int rawPos;
        int rawUsed;
        Encoding encoding;
        Decoder decoder;
        char[] buffer;
        int used;
        int pos;
        private const int BUFSIZE = 16384;
        private const int EOF = -1;

        public HtmlStream(Stream stm, Encoding defaultEncoding)
        {
            if (defaultEncoding == null) defaultEncoding = Encoding.UTF8; // default is UTF8
            if (!stm.CanSeek)
            {
                // Need to be able to seek to sniff correctly.
                stm = CopyToMemoryStream(stm);
            }
            this.stm = stm;
            rawBuffer = new Byte[BUFSIZE];
            rawUsed = stm.Read(rawBuffer, 0, 4); // maximum byte order mark
            buffer = new char[BUFSIZE];

            // Check byte order marks
            decoder = AutoDetectEncoding(rawBuffer, ref rawPos, rawUsed);
            var bom = rawPos;
            if (decoder == null)
            {
                decoder = defaultEncoding.GetDecoder();
                rawUsed += stm.Read(rawBuffer, 4, BUFSIZE - 4);
                DecodeBlock();
                // Now sniff to see if there is an XML declaration or HTML <META> tag.
                var sd = SniffEncoding();
                if (sd != null)
                {
                    decoder = sd;
                }
            }

            // Reset to get ready for Read()
            this.stm.Seek(0, SeekOrigin.Begin);
            pos = used = 0;
            // skip bom
            if (bom > 0)
            {
                stm.Read(rawBuffer, 0, bom);
            }
            rawPos = rawUsed = 0;

        }

        public Encoding Encoding => encoding;

        Stream CopyToMemoryStream(Stream s)
        {
            var size = 100000; // large heap is more efficient
            var buffer = new byte[size];
            int len;
            var r = new MemoryStream();
            while ((len = s.Read(buffer, 0, size)) > 0)
            {
                r.Write(buffer, 0, len);
            }
            r.Seek(0, SeekOrigin.Begin);
            s.Close();
            return r;
        }

        internal void DecodeBlock()
        {
            // shift current chars to beginning.
            if (pos > 0)
            {
                if (pos < used)
                {
                    Array.Copy(buffer, pos, buffer, 0, used - pos);
                }
                used -= pos;
                pos = 0;
            }
            var len = decoder.GetCharCount(rawBuffer, rawPos, rawUsed - rawPos);
            var available = buffer.Length - used;
            if (available < len)
            {
                var newbuf = new char[buffer.Length + len];
                Array.Copy(buffer, pos, newbuf, 0, used - pos);
                buffer = newbuf;
            }
            used = pos + decoder.GetChars(rawBuffer, rawPos, rawUsed - rawPos, buffer, pos);
            rawPos = rawUsed; // consumed the whole buffer!
        }
        internal static Decoder AutoDetectEncoding(byte[] buffer, ref int index, int length)
        {
            if (4 <= (length - index))
            {
                var w = (uint)buffer[index + 0] << 24 | (uint)buffer[index + 1] << 16 | (uint)buffer[index + 2] << 8 | (uint)buffer[index + 3];
                // see if it's a 4-byte encoding
                switch (w)
                {
                    case 0xfefffeff:
                        index += 4;
                        return new Ucs4DecoderBigEngian();

                    case 0xfffefffe:
                        index += 4;
                        return new Ucs4DecoderLittleEndian();

                    case 0x3c000000:
                        goto case 0xfefffeff;

                    case 0x0000003c:
                        goto case 0xfffefffe;
                }
                w >>= 8;
                if (w == 0xefbbbf)
                {
                    index += 3;
                    return Encoding.UTF8.GetDecoder();
                }
                w >>= 8;
                switch (w)
                {
                    case 0xfeff:
                        index += 2;
                        return Encoding.BigEndianUnicode.GetDecoder();

                    case 0xfffe:
                        index += 2;
                        return new UnicodeEncoding(false, false).GetDecoder();

                    case 0x3c00:
                        goto case 0xfeff;

                    case 0x003c:
                        goto case 0xfffe;
                }
            }
            return null;
        }
        private int ReadChar()
        {
            // Read only up to end of current buffer then stop.
            if (pos < used) return buffer[pos++];
            return EOF;
        }
        private int PeekChar()
        {
            var ch = ReadChar();
            if (ch != EOF)
            {
                pos--;
            }
            return ch;
        }
        private bool SniffPattern(string pattern)
        {
            var ch = PeekChar();
            if (ch != pattern[0]) return false;
            for (int i = 0, n = pattern.Length; ch != EOF && i < n; i++)
            {
                ch = ReadChar();
                var m = pattern[i];
                if (ch != m)
                {
                    return false;
                }
            }
            return true;
        }
        private void SniffWhitespace()
        {
            var ch = (char)PeekChar();
            while (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n')
            {
                var i = pos;
                ch = (char)ReadChar();
                if (ch != ' ' && ch != '\t' && ch != '\r' && ch != '\n')
                    pos = i;
            }
        }

        private string SniffLiteral()
        {
            var quoteChar = PeekChar();
            if (quoteChar == '\'' || quoteChar == '"')
            {
                ReadChar();// consume quote char
                var i = pos;
                var ch = ReadChar();
                while (ch != EOF && ch != quoteChar)
                {
                    ch = ReadChar();
                }
                return (pos > i) ? new string(buffer, i, pos - i - 1) : "";
            }
            return null;
        }
        private string SniffAttribute(string name)
        {
            SniffWhitespace();
            var id = SniffName();
            if (name == id)
            {
                SniffWhitespace();
                if (SniffPattern("="))
                {
                    SniffWhitespace();
                    return SniffLiteral();
                }
            }
            return null;
        }
        private string SniffAttribute(out string name)
        {
            SniffWhitespace();
            name = SniffName();
            if (name != null)
            {
                SniffWhitespace();
                if (SniffPattern("="))
                {
                    SniffWhitespace();
                    return SniffLiteral();
                }
            }
            return null;
        }
        private void SniffTerminator(string term)
        {
            var ch = ReadChar();
            var i = 0;
            var n = term.Length;
            while (i < n && ch != EOF)
            {
                if (term[i] == ch)
                {
                    i++;
                    if (i == n) break;
                }
                else
                {
                    i = 0; // reset.
                }
                ch = ReadChar();
            }
        }
        internal Decoder SniffEncoding()
        {
            Decoder decoder = null;
            if (SniffPattern("<?xml"))
            {
                var version = SniffAttribute("version");
                if (version != null)
                {
                    var encoding = SniffAttribute("encoding");
                    if (encoding != null)
                    {
                        try
                        {
                            var enc = Encoding.GetEncoding(encoding);
                            if (enc != null)
                            {
                                this.encoding = enc;
                                return enc.GetDecoder();
                            }
                        }
                        catch (Exception)
                        {
                            // oh well then.
                        }
                    }
                    SniffTerminator(">");
                }
            }
            if (decoder == null)
            {
                return SniffMeta();
            }
            return null;
        }

        internal Decoder SniffMeta()
        {
            var i = ReadChar();
            while (i != EOF)
            {
                var ch = (char)i;
                if (ch == '<')
                {
                    var name = SniffName();
                    if (name != null && StringUtilities.EqualsIgnoreCase(name, "meta"))
                    {
                        string httpequiv = null;
                        string content = null;
                        while (true)
                        {
                            var value = SniffAttribute(out name);
                            if (name == null)
                            {
                                break;
                            }
                            if (StringUtilities.EqualsIgnoreCase(name, "http-equiv"))
                            {
                                httpequiv = value;
                            }
                            else if (StringUtilities.EqualsIgnoreCase(name, "content"))
                            {
                                content = value;
                            }
                        }
                        if (httpequiv != null && StringUtilities.EqualsIgnoreCase(httpequiv, "content-type") && content != null)
                        {
                            var j = content.IndexOf("charset");
                            if (j >= 0)
                            {
                                //charset=utf-8
                                j = content.IndexOf("=", j);
                                if (j >= 0)
                                {
                                    j++;
                                    var k = content.IndexOf(";", j);
                                    if (k < 0) k = content.Length;
                                    var charset = content.Substring(j, k - j).Trim();
                                    try
                                    {
                                        var e = Encoding.GetEncoding(charset);
                                        encoding = e;
                                        return e.GetDecoder();
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
                i = ReadChar();

            }
            return null;
        }

        internal string SniffName()
        {
            var c = PeekChar();
            if (c == EOF)
                return null;
            var ch = (char)c;
            var start = pos;
            while (pos < used - 1 && (Char.IsLetterOrDigit(ch) || ch == '-' || ch == '_' || ch == ':'))
            {
                ch = buffer[++pos];
            }
            if (start == pos) return null;
            return new string(buffer, start, pos - start);
        }

        internal void SkipWhitespace()
        {
            var ch = (char)PeekChar();
            while (pos < used - 1 && (ch == ' ' || ch == '\r' || ch == '\n'))
            {
                ch = buffer[++pos];
            }
        }
        internal void SkipTo(char what)
        {
            var ch = (char)PeekChar();
            while (pos < used - 1 && (ch != what))
            {
                ch = buffer[++pos];
            }
        }
        internal string ParseAttribute()
        {
            SkipTo('=');
            if (pos < used)
            {
                pos++;
                SkipWhitespace();
                if (pos < used)
                {
                    var quote = buffer[pos];
                    pos++;
                    var start = pos;
                    SkipTo(quote);
                    if (pos < used)
                    {
                        var result = new string(buffer, start, pos - start);
                        pos++;
                        return result;
                    }
                }
            }
            return null;
        }
        public override int Peek()
        {
            var result = Read();
            if (result != EOF)
            {
                pos--;
            }
            return result;
        }
        public override int Read()
        {
            if (pos == used)
            {
                rawUsed = stm.Read(rawBuffer, 0, rawBuffer.Length);
                rawPos = 0;
                if (rawUsed == 0) return EOF;
                DecodeBlock();
            }
            if (pos < used) return buffer[pos++];
            return -1;
        }
        public override int Read(char[] buffer, int start, int length)
        {
            if (pos == used)
            {
                rawUsed = stm.Read(rawBuffer, 0, rawBuffer.Length);
                rawPos = 0;
                if (rawUsed == 0) return -1;
                DecodeBlock();
            }
            if (pos < used)
            {
                length = Math.Min(used - pos, length);
                Array.Copy(this.buffer, pos, buffer, start, length);
                pos += length;
                return length;
            }
            return 0;
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            return Read(buffer, index, count);
        }
        // Read up to end of line, or full buffer, whichever comes first.
        public int ReadLine(char[] buffer, int start, int length)
        {
            var i = 0;
            var ch = ReadChar();
            while (ch != EOF)
            {
                buffer[i + start] = (char)ch;
                i++;
                if (i + start == length)
                    break; // buffer is full

                if (ch == '\r')
                {
                    if (PeekChar() == '\n')
                    {
                        ch = ReadChar();
                        buffer[i + start] = (char)ch;
                        i++;
                    }
                    break;
                }
                else if (ch == '\n')
                {
                    break;
                }
                ch = ReadChar();
            }
            return i;
        }

        public override string ReadToEnd()
        {
            var buffer = new char[100000]; // large block heap is more efficient
            var len = 0;
            var sb = new StringBuilder();
            while ((len = Read(buffer, 0, buffer.Length)) > 0)
            {
                sb.Append(buffer, 0, len);
            }
            return sb.ToString();
        }
        public override void Close()
        {
            stm.Close();
        }
    }
    internal abstract class Ucs4Decoder : Decoder
    {
        internal byte[] temp = new byte[4];
        internal int tempBytes = 0;
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return (count + tempBytes) / 4;
        }
        internal abstract int GetFullChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex);
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            var i = tempBytes;

            if (tempBytes > 0)
            {
                for (; i < 4; i++)
                {
                    temp[i] = bytes[byteIndex];
                    byteIndex++;
                    byteCount--;
                }
                i = 1;
                GetFullChars(temp, 0, 4, chars, charIndex);
                charIndex++;
            }
            else
                i = 0;
            i = GetFullChars(bytes, byteIndex, byteCount, chars, charIndex) + i;

            var j = (tempBytes + byteCount) % 4;
            byteCount += byteIndex;
            byteIndex = byteCount - j;
            tempBytes = 0;

            if (byteIndex >= 0)
                for (; byteIndex < byteCount; byteIndex++)
                {
                    temp[tempBytes] = bytes[byteIndex];
                    tempBytes++;
                }
            return i;
        }
        internal char UnicodeToUTF16(UInt32 code)
        {
            byte lowerByte, higherByte;
            lowerByte = (byte)(0xD7C0 + (code >> 10));
            higherByte = (byte)(0xDC00 | code & 0x3ff);
            return ((char)((higherByte << 8) | lowerByte));
        }
    }
    internal class Ucs4DecoderBigEngian : Ucs4Decoder
    {
        internal override int GetFullChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            UInt32 code;
            int i, j;
            byteCount += byteIndex;
            for (i = byteIndex, j = charIndex; i + 3 < byteCount; )
            {
                code = (UInt32)(((bytes[i + 3]) << 24) | (bytes[i + 2] << 16) | (bytes[i + 1] << 8) | (bytes[i]));
                if (code > 0x10FFFF)
                {
                    throw new Exception("Invalid character 0x" + code.ToString("x") + " in encoding");
                }
                else if (code > 0xFFFF)
                {
                    chars[j] = UnicodeToUTF16(code);
                    j++;
                }
                else
                {
                    if (code >= 0xD800 && code <= 0xDFFF)
                    {
                        throw new Exception("Invalid character 0x" + code.ToString("x") + " in encoding");
                    }
                    else
                    {
                        chars[j] = (char)code;
                    }
                }
                j++;
                i += 4;
            }
            return j - charIndex;
        }
    };
    internal class Ucs4DecoderLittleEndian : Ucs4Decoder
    {
        internal override int GetFullChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            UInt32 code;
            int i, j;
            byteCount += byteIndex;
            for (i = byteIndex, j = charIndex; i + 3 < byteCount; )
            {
                code = (UInt32)(((bytes[i]) << 24) | (bytes[i + 1] << 16) | (bytes[i + 2] << 8) | (bytes[i + 3]));
                if (code > 0x10FFFF)
                {
                    throw new Exception("Invalid character 0x" + code.ToString("x") + " in encoding");
                }
                else if (code > 0xFFFF)
                {
                    chars[j] = UnicodeToUTF16(code);
                    j++;
                }
                else
                {
                    if (code >= 0xD800 && code <= 0xDFFF)
                    {
                        throw new Exception("Invalid character 0x" + code.ToString("x") + " in encoding");
                    }
                    else
                    {
                        chars[j] = (char)code;
                    }
                }
                j++;
                i += 4;
            }
            return j - charIndex;
        }
    }

    public class ElementDecl
    {
        public ElementDecl(string name, bool sto, bool eto, ContentModel cm, string[] inclusions, string[] exclusions)
        {
            Name = name;
            StartTagOptional = sto;
            EndTagOptional = eto;
            ContentModel = cm;
            Inclusions = inclusions;
            Exclusions = exclusions;
        }
        public string Name;
        public bool StartTagOptional;
        public bool EndTagOptional;
        public ContentModel ContentModel;
        public string[] Inclusions;
        public string[] Exclusions;
        public AttList AttList;

        public AttDef FindAttribute(string name)
        {
            return AttList[name.ToUpper()];
        }

        public void AddAttDefs(AttList list)
        {
            if (AttList == null)
            {
                AttList = list;
            }
            else
            {
                foreach (AttDef a in list)
                {
                    if (AttList[a.Name] == null)
                    {
                        AttList.Add(a);
                    }
                }
            }
        }

        public bool CanContain(string name, SgmlDtd dtd)
        {
            // return true if this element is allowed to contain the given element.
            if (Exclusions != null)
            {
                foreach (var s in Exclusions)
                {
                    if ((object)s == (object)name) // XmlNameTable optimization
                        return false;
                }
            }
            if (Inclusions != null)
            {
                foreach (var s in Inclusions)
                {
                    if ((object)s == (object)name) // XmlNameTable optimization
                        return true;
                }
            }
            return ContentModel.CanContain(name, dtd);
        }
    }

    public enum DeclaredContent
    {
        Default, CDATA, RCDATA, EMPTY
    }

    public class ContentModel
    {
        public DeclaredContent DeclaredContent;
        public int CurrentDepth;
        public Group Model;

        public ContentModel()
        {
            Model = new Group(null);
        }

        public void PushGroup()
        {
            Model = new Group(Model);
            CurrentDepth++;
        }

        public int PopGroup()
        {
            if (CurrentDepth == 0) return -1;
            CurrentDepth--;
            Model.Parent.AddGroup(Model);
            Model = Model.Parent;
            return CurrentDepth;
        }

        public void AddSymbol(string sym)
        {
            Model.AddSymbol(sym);
        }

        public void AddConnector(char c)
        {
            Model.AddConnector(c);
        }

        public void AddOccurrence(char c)
        {
            Model.AddOccurrence(c);
        }

        public void SetDeclaredContent(string dc)
        {
            switch (dc)
            {
                case "EMPTY":
                    DeclaredContent = DeclaredContent.EMPTY;
                    break;
                case "RCDATA":
                    DeclaredContent = DeclaredContent.RCDATA;
                    break;
                case "CDATA":
                    DeclaredContent = DeclaredContent.CDATA;
                    break;
                default:
                    throw new Exception(
                        $"Declared content type '{dc}' is not supported");
            }
        }

        public bool CanContain(string name, SgmlDtd dtd)
        {
            if (DeclaredContent != DeclaredContent.Default)
                return false; // empty or text only node.
            return Model.CanContain(name, dtd);
        }
    }

    public enum GroupType
    {
        None, And, Or, Sequence
    };

    public enum Occurrence
    {
        Required, Optional, ZeroOrMore, OneOrMore
    }

    public class Group
    {
        public Group Parent;
        public ArrayList Members;
        public GroupType GroupType;
        public Occurrence Occurrence;
        public bool Mixed;

        public bool TextOnly => Mixed && Members.Count == 0;

        public Group(Group parent)
        {
            Parent = parent;
            Members = new ArrayList();
            GroupType = GroupType.None;
            Occurrence = Occurrence.Required;
        }
        public void AddGroup(Group g)
        {
            Members.Add(g);
        }
        public void AddSymbol(string sym)
        {
            if (sym == "#PCDATA")
            {
                Mixed = true;
            }
            else
            {
                Members.Add(sym);
            }
        }
        public void AddConnector(char c)
        {
            if (!Mixed && Members.Count == 0)
            {
                throw new Exception(
                    $"Missing token before connector '{c}'."
                );
            }
            var gt = GroupType.None;
            switch (c)
            {
                case ',':
                    gt = GroupType.Sequence;
                    break;
                case '|':
                    gt = GroupType.Or;
                    break;
                case '&':
                    gt = GroupType.And;
                    break;
            }
            if (GroupType != GroupType.None && GroupType != gt)
            {
                throw new Exception(
                    $"Connector '{c}' is inconsistent with {GroupType.ToString()} group."
                );
            }
            GroupType = gt;
        }

        public void AddOccurrence(char c)
        {
            var o = Occurrence.Required;
            switch (c)
            {
                case '?':
                    o = Occurrence.Optional;
                    break;
                case '+':
                    o = Occurrence.OneOrMore;
                    break;
                case '*':
                    o = Occurrence.ZeroOrMore;
                    break;
            }
            Occurrence = o;
        }

        // Rough approximation - this is really assuming an "Or" group
        public bool CanContain(string name, SgmlDtd dtd)
        {
            // Do a simple search of members.
            foreach (var obj in Members)
            {
                if (obj is String)
                {
                    if (obj == (object)name) // XmlNameTable optimization
                        return true;
                }
            }
            // didn't find it, so do a more expensive search over child elements
            // that have optional start tags and over child groups.
            foreach (var obj in Members)
            {
                if (obj is String)
                {
                    var s = (string)obj;
                    var e = dtd.FindElement(s);
                    if (e != null)
                    {
                        if (e.StartTagOptional)
                        {
                            // tricky case, the start tag is optional so element may be
                            // allowed inside this guy!
                            if (e.CanContain(name, dtd))
                                return true;
                        }
                    }
                }
                else
                {
                    var m = (Group)obj;
                    if (m.CanContain(name, dtd))
                        return true;
                }
            }
            return false;
        }
    }

    public enum AttributeType
    {
        DEFAULT, CDATA, ENTITY, ENTITIES, ID, IDREF, IDREFS, NAME, NAMES, NMTOKEN, NMTOKENS,
        NUMBER, NUMBERS, NUTOKEN, NUTOKENS, NOTATION, ENUMERATION
    }

    public enum AttributePresence
    {
        DEFAULT, FIXED, REQUIRED, IMPLIED
    }

    public class AttDef
    {
        public string Name;
        public AttributeType Type;
        public string[] EnumValues;
        public string Default;
        public AttributePresence Presence;

        public AttDef(string name)
        {
            Name = name;
        }


        public void SetType(string type)
        {
            switch (type)
            {
                case "CDATA":
                    Type = AttributeType.CDATA;
                    break;
                case "ENTITY":
                    Type = AttributeType.ENTITY;
                    break;
                case "ENTITIES":
                    Type = AttributeType.ENTITIES;
                    break;
                case "ID":
                    Type = AttributeType.ID;
                    break;
                case "IDREF":
                    Type = AttributeType.IDREF;
                    break;
                case "IDREFS":
                    Type = AttributeType.IDREFS;
                    break;
                case "NAME":
                    Type = AttributeType.NAME;
                    break;
                case "NAMES":
                    Type = AttributeType.NAMES;
                    break;
                case "NMTOKEN":
                    Type = AttributeType.NMTOKEN;
                    break;
                case "NMTOKENS":
                    Type = AttributeType.NMTOKENS;
                    break;
                case "NUMBER":
                    Type = AttributeType.NUMBER;
                    break;
                case "NUMBERS":
                    Type = AttributeType.NUMBERS;
                    break;
                case "NUTOKEN":
                    Type = AttributeType.NUTOKEN;
                    break;
                case "NUTOKENS":
                    Type = AttributeType.NUTOKENS;
                    break;
                default:
                    throw new Exception("Attribute type '" + type + "' is not supported");
            }
        }

        public bool SetPresence(string token)
        {
            var hasDefault = true;
            if (token == "FIXED")
            {
                Presence = AttributePresence.FIXED;
            }
            else if (token == "REQUIRED")
            {
                Presence = AttributePresence.REQUIRED;
                hasDefault = false;
            }
            else if (token == "IMPLIED")
            {
                Presence = AttributePresence.IMPLIED;
                hasDefault = false;
            }
            else
            {
                throw new Exception($"Attribute value '{token}' not supported");
            }
            return hasDefault;
        }
    }

    public class AttList : IEnumerable
    {
        Hashtable AttDefs;

        public AttList()
        {
            AttDefs = new Hashtable();
        }

        public void Add(AttDef a)
        {
            AttDefs.Add(a.Name, a);
        }

        public AttDef this[string name] => (AttDef)AttDefs[name];

        public IEnumerator GetEnumerator()
        {
            return AttDefs.Values.GetEnumerator();
        }
    }

    public class SgmlDtd
    {
        public string Name;

        Hashtable elements;
        Hashtable pentities;
        Hashtable entities;
        StringBuilder sb;
        Entity current;
        XmlNameTable nameTable;

        public SgmlDtd(string name, XmlNameTable nt)
        {
            nameTable = nt;
            Name = name;
            elements = new Hashtable();
            pentities = new Hashtable();
            entities = new Hashtable();
            sb = new StringBuilder();
        }

        public XmlNameTable NameTable => nameTable;

        public static SgmlDtd Parse(Uri baseUri, string name, string pubid, string url, string subset, string proxy, XmlNameTable nt)
        {
            var dtd = new SgmlDtd(name, nt);
            if (url != null && url != string.Empty)
            {
                dtd.PushEntity(baseUri, new Entity(dtd.Name, pubid, url, proxy));
            }
            if (subset != null && subset != string.Empty)
            {
                dtd.PushEntity(baseUri, new Entity(name, subset));
            }
            try
            {
                dtd.Parse();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + dtd.current.Context());
            }
            return dtd;
        }
        public static SgmlDtd Parse(Uri baseUri, string name, string pubid, TextReader input, string subset, string proxy, XmlNameTable nt)
        {
            var dtd = new SgmlDtd(name, nt);
            dtd.PushEntity(baseUri, new Entity(dtd.Name, baseUri, input, proxy));
            if (subset != null && subset != string.Empty)
            {
                dtd.PushEntity(baseUri, new Entity(name, subset));
            }
            try
            {
                dtd.Parse();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + dtd.current.Context());
            }
            return dtd;
        }

        public Entity FindEntity(string name)
        {
            return (Entity)entities[name];
        }

        public ElementDecl FindElement(string name)
        {
            return (ElementDecl)elements[name.ToUpper()];
        }

        //-------------------------------- Parser -------------------------
        void PushEntity(Uri baseUri, Entity e)
        {
            e.Open(current, baseUri);
            current = e;
            current.ReadChar();
        }

        void PopEntity()
        {
            if (current != null) current.Close();
            if (current.Parent != null)
            {
                current = current.Parent;
            }
            else
            {
                current = null;
            }
        }

        void Parse()
        {
            var ch = current.Lastchar;
            while (true)
            {
                switch (ch)
                {
                    case Entity.EOF:
                        PopEntity();
                        if (current == null)
                            return;
                        ch = current.Lastchar;
                        break;
                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t':
                        ch = current.ReadChar();
                        break;
                    case '<':
                        ParseMarkup();
                        ch = current.ReadChar();
                        break;
                    case '%':
                        var e = ParseParameterEntity(WhiteSpace);
                        try
                        {
                            PushEntity(current.ResolvedUri, e);
                        }
                        catch (Exception ex)
                        {
                            // bugbug - need an error log.
                            Console.WriteLine(ex.Message + current.Context());
                        }
                        ch = current.Lastchar;
                        break;
                    default:
                        current.Error("Unexpected character '{0}'", ch);
                        break;
                }
            }
        }

        void ParseMarkup()
        {
            var ch = current.ReadChar();
            if (ch != '!')
            {
                current.Error("Found '{0}', but expecing declaration starting with '<!'");
                return;
            }
            ch = current.ReadChar();
            if (ch == '-')
            {
                ch = current.ReadChar();
                if (ch != '-') current.Error("Expecting comment '<!--' but found {0}", ch);
                current.ScanToEnd(sb, "Comment", "-->");
            }
            else if (ch == '[')
            {
                ParseMarkedSection();
            }
            else
            {
                var token = current.ScanToken(sb, WhiteSpace, true);
                switch (token)
                {
                    case "ENTITY":
                        ParseEntity();
                        break;
                    case "ELEMENT":
                        ParseElementDecl();
                        break;
                    case "ATTLIST":
                        ParseAttList();
                        break;
                    default:
                        current.Error("Invalid declaration '<!{0}'.  Expecting 'ENTITY', 'ELEMENT' or 'ATTLIST'.", token);
                        break;
                }
            }
        }

        char ParseDeclComments()
        {
            var ch = current.Lastchar;
            while (ch == '-')
            {
                ch = ParseDeclComment(true);
            }
            return ch;
        }

        char ParseDeclComment(bool full)
        {
            var start = current.Line;
            // -^-...--
            // This method scans over a comment inside a markup declaration.
            var ch = current.ReadChar();
            if (full && ch != '-') current.Error("Expecting comment delimiter '--' but found {0}", ch);
            current.ScanToEnd(sb, "Markup Comment", "--");
            return current.SkipWhitespace();
        }

        void ParseMarkedSection()
        {
            // <![^ name [ ... ]]>
            current.ReadChar(); // move to next char.
            var name = ScanName("[");
            if (name == "INCLUDE")
            {
                ParseIncludeSection();
            }
            else if (name == "IGNORE")
            {
                ParseIgnoreSection();
            }
            else
            {
                current.Error("Unsupported marked section type '{0}'", name);
            }
        }

        void ParseIncludeSection()
        {
            throw new NotImplementedException("Include Section");
        }

        void ParseIgnoreSection()
        {
            var start = current.Line;
            // <!-^-...-->
            var ch = current.SkipWhitespace();
            if (ch != '[') current.Error("Expecting '[' but found {0}", ch);
            current.ScanToEnd(sb, "Conditional Section", "]]>");
        }

        string ScanName(string term)
        {
            // skip whitespace, scan name (which may be parameter entity reference
            // which is then expanded to a name)
            var ch = current.SkipWhitespace();
            if (ch == '%')
            {
                var e = ParseParameterEntity(term);
                ch = current.Lastchar;
                // bugbug - need to support external and nested parameter entities
                if (!e.Internal) throw new NotSupportedException("External parameter entity resolution");
                return e.Literal.Trim();
            }
            else
            {
                return current.ScanToken(sb, term, true);
            }
        }

        Entity ParseParameterEntity(string term)
        {
            // almost the same as this.current.ScanToken, except we also terminate on ';'
            var ch = current.ReadChar();
            var name = current.ScanToken(sb, ";" + term, false);
            name = nameTable.Add(name);
            if (current.Lastchar == ';')
                current.ReadChar();
            var e = GetParameterEntity(name);
            return e;
        }

        Entity GetParameterEntity(string name)
        {
            var e = (Entity)pentities[name];
            if (e == null) current.Error("Reference to undefined parameter entity '{0}'", name);
            return e;
        }

        static string WhiteSpace = " \r\n\t";

        void ParseEntity()
        {
            var ch = current.SkipWhitespace();
            var pe = (ch == '%');
            if (pe)
            {
                // parameter entity.
                current.ReadChar(); // move to next char
                ch = current.SkipWhitespace();
            }
            var name = current.ScanToken(sb, WhiteSpace, true);
            name = nameTable.Add(name);
            ch = current.SkipWhitespace();
            Entity e = null;
            if (ch == '"' || ch == '\'')
            {
                var literal = current.ScanLiteral(sb, ch);
                e = new Entity(name, literal);
            }
            else
            {
                string pubid = null;
                string extid = null;
                var tok = current.ScanToken(sb, WhiteSpace, true);
                if (Entity.IsLiteralType(tok))
                {
                    ch = current.SkipWhitespace();
                    var literal = current.ScanLiteral(sb, ch);
                    e = new Entity(name, literal);
                    e.SetLiteralType(tok);
                }
                else
                {
                    extid = tok;
                    if (extid == "PUBLIC")
                    {
                        ch = current.SkipWhitespace();
                        if (ch == '"' || ch == '\'')
                        {
                            pubid = current.ScanLiteral(sb, ch);
                        }
                        else
                        {
                            current.Error("Expecting public identifier literal but found '{0}'", ch);
                        }
                    }
                    else if (extid != "SYSTEM")
                    {
                        current.Error("Invalid external identifier '{0}'.  Expecing 'PUBLIC' or 'SYSTEM'.", extid);
                    }
                    string uri = null;
                    ch = current.SkipWhitespace();
                    if (ch == '"' || ch == '\'')
                    {
                        uri = current.ScanLiteral(sb, ch);
                    }
                    else if (ch != '>')
                    {
                        current.Error("Expecting system identifier literal but found '{0}'", ch);
                    }
                    e = new Entity(name, pubid, uri, current.Proxy);
                }
            }
            ch = current.SkipWhitespace();
            if (ch == '-')
                ch = ParseDeclComments();
            if (ch != '>')
            {
                current.Error("Expecting end of entity declaration '>' but found '{0}'", ch);
            }
            if (pe) pentities.Add(e.Name, e);
            else entities.Add(e.Name, e);
        }

        void ParseElementDecl()
        {
            var ch = current.SkipWhitespace();
            var names = ParseNameGroup(ch, true);
            ch = Char.ToUpper(current.SkipWhitespace());
            var sto = false;
            var eto = false;
            if (ch == 'O' || ch == '-')
            {
                sto = (ch == 'O'); // start tag optional?   
                current.ReadChar();
                ch = Char.ToUpper(current.SkipWhitespace());
                if (ch == 'O' || ch == '-')
                {
                    eto = (ch == 'O'); // end tag optional? 
                    ch = current.ReadChar();
                }
            }
            ch = current.SkipWhitespace();
            var cm = ParseContentModel(ch);
            ch = current.SkipWhitespace();

            string[] exclusions = null;
            string[] inclusions = null;

            if (ch == '-')
            {
                ch = current.ReadChar();
                if (ch == '(')
                {
                    exclusions = ParseNameGroup(ch, true);
                    ch = current.SkipWhitespace();
                }
                else if (ch == '-')
                {
                    ch = ParseDeclComment(false);
                }
                else
                {
                    current.Error("Invalid syntax at '{0}'", ch);
                }
            }

            if (ch == '-')
                ch = ParseDeclComments();

            if (ch == '+')
            {
                ch = current.ReadChar();
                if (ch != '(')
                {
                    current.Error("Expecting inclusions name group", ch);
                }
                inclusions = ParseNameGroup(ch, true);
                ch = current.SkipWhitespace();
            }

            if (ch == '-')
                ch = ParseDeclComments();


            if (ch != '>')
            {
                current.Error("Expecting end of ELEMENT declaration '>' but found '{0}'", ch);
            }

            foreach (var name in names)
            {
                var atom = name.ToUpper();
                atom = nameTable.Add(name);
                elements.Add(atom, new ElementDecl(atom, sto, eto, cm, inclusions, exclusions));
            }
        }

        static string ngterm = " \r\n\t|,)";
        string[] ParseNameGroup(char ch, bool nmtokens)
        {
            var names = new ArrayList();
            if (ch == '(')
            {
                ch = current.ReadChar();
                ch = current.SkipWhitespace();
                while (ch != ')')
                {
                    // skip whitespace, scan name (which may be parameter entity reference
                    // which is then expanded to a name)                    
                    ch = current.SkipWhitespace();
                    if (ch == '%')
                    {
                        var e = ParseParameterEntity(ngterm);
                        PushEntity(current.ResolvedUri, e);
                        ParseNameList(names, nmtokens);
                        PopEntity();
                        ch = current.Lastchar;
                    }
                    else
                    {
                        var token = current.ScanToken(sb, ngterm, nmtokens);
                        token = token.ToUpper();
                        var atom = nameTable.Add(token);
                        names.Add(atom);
                    }
                    ch = current.SkipWhitespace();
                    if (ch == '|' || ch == ',') ch = current.ReadChar();
                }
                current.ReadChar(); // consume ')'
            }
            else
            {
                var name = current.ScanToken(sb, WhiteSpace, nmtokens);
                name = name.ToUpper();
                name = nameTable.Add(name);
                names.Add(name);
            }
            return (string[])names.ToArray(typeof(String));
        }

        void ParseNameList(ArrayList names, bool nmtokens)
        {
            var ch = current.Lastchar;
            ch = current.SkipWhitespace();
            while (ch != Entity.EOF)
            {
                string name;
                if (ch == '%')
                {
                    var e = ParseParameterEntity(ngterm);
                    PushEntity(current.ResolvedUri, e);
                    ParseNameList(names, nmtokens);
                    PopEntity();
                    ch = current.Lastchar;
                }
                else
                {
                    name = current.ScanToken(sb, ngterm, true);
                    name = name.ToUpper();
                    name = nameTable.Add(name);
                    names.Add(name);
                }
                ch = current.SkipWhitespace();
                if (ch == '|')
                {
                    ch = current.ReadChar();
                    ch = current.SkipWhitespace();
                }
            }
        }

        static string dcterm = " \r\n\t>";
        ContentModel ParseContentModel(char ch)
        {
            var cm = new ContentModel();
            if (ch == '(')
            {
                current.ReadChar();
                ParseModel(')', cm);
                ch = current.ReadChar();
                if (ch == '?' || ch == '+' || ch == '*')
                {
                    cm.AddOccurrence(ch);
                    current.ReadChar();
                }
            }
            else if (ch == '%')
            {
                var e = ParseParameterEntity(dcterm);
                PushEntity(current.ResolvedUri, e);
                cm = ParseContentModel(current.Lastchar);
                PopEntity(); // bugbug should be at EOF.
            }
            else
            {
                var dc = ScanName(dcterm);
                cm.SetDeclaredContent(dc);
            }
            return cm;
        }

        static string cmterm = " \r\n\t,&|()?+*";
        void ParseModel(char cmt, ContentModel cm)
        {
            // Called when part of the model is made up of the contents of a parameter entity
            var depth = cm.CurrentDepth;
            var ch = current.Lastchar;
            ch = current.SkipWhitespace();
            while (ch != cmt || cm.CurrentDepth > depth) // the entity must terminate while inside the content model.
            {
                if (ch == Entity.EOF)
                {
                    current.Error("Content Model was not closed");
                }
                if (ch == '%')
                {
                    var e = ParseParameterEntity(cmterm);
                    PushEntity(current.ResolvedUri, e);
                    ParseModel(Entity.EOF, cm);
                    PopEntity();
                    ch = current.SkipWhitespace();
                }
                else if (ch == '(')
                {
                    cm.PushGroup();
                    current.ReadChar();// consume '('
                    ch = current.SkipWhitespace();
                }
                else if (ch == ')')
                {
                    ch = current.ReadChar();// consume ')'
                    if (ch == '*' || ch == '+' || ch == '?')
                    {
                        cm.AddOccurrence(ch);
                        ch = current.ReadChar();
                    }
                    if (cm.PopGroup() < depth)
                    {
                        current.Error("Parameter entity cannot close a paren outside it's own scope");
                    }
                    ch = current.SkipWhitespace();
                }
                else if (ch == ',' || ch == '|' || ch == '&')
                {
                    cm.AddConnector(ch);
                    current.ReadChar(); // skip connector
                    ch = current.SkipWhitespace();
                }
                else
                {
                    string token;
                    if (ch == '#')
                    {
                        ch = current.ReadChar();
                        token = "#" + current.ScanToken(sb, cmterm, true); // since '#' is not a valid name character.
                    }
                    else
                    {
                        token = current.ScanToken(sb, cmterm, true);
                    }
                    token = token.ToUpper();
                    token = nameTable.Add(token);// atomize it.
                    ch = current.Lastchar;
                    if (ch == '?' || ch == '+' || ch == '*')
                    {
                        cm.PushGroup();
                        cm.AddSymbol(token);
                        cm.AddOccurrence(ch);
                        cm.PopGroup();
                        current.ReadChar(); // skip connector
                        ch = current.SkipWhitespace();
                    }
                    else
                    {
                        cm.AddSymbol(token);
                        ch = current.SkipWhitespace();
                    }
                }
            }
        }

        void ParseAttList()
        {
            var ch = current.SkipWhitespace();
            var names = ParseNameGroup(ch, true);
            var attlist = new AttList();
            ParseAttList(attlist, '>');
            foreach (var name in names)
            {
                var e = (ElementDecl)elements[name];
                if (e == null)
                {
                    current.Error("ATTLIST references undefined ELEMENT {0}", name);
                }
                e.AddAttDefs(attlist);
            }
        }

        static string peterm = " \t\r\n>";
        void ParseAttList(AttList list, char term)
        {
            var ch = current.SkipWhitespace();
            while (ch != term)
            {
                if (ch == '%')
                {
                    var e = ParseParameterEntity(peterm);
                    PushEntity(current.ResolvedUri, e);
                    ParseAttList(list, Entity.EOF);
                    PopEntity();
                    ch = current.SkipWhitespace();
                }
                else if (ch == '-')
                {
                    ch = ParseDeclComments();
                }
                else
                {
                    var a = ParseAttDef(ch);
                    list.Add(a);
                }
                ch = current.SkipWhitespace();
            }
        }

        AttDef ParseAttDef(char ch)
        {
            ch = current.SkipWhitespace();
            var name = ScanName(WhiteSpace);
            name = name.ToUpper();
            name = nameTable.Add(name);
            var attdef = new AttDef(name);

            ch = current.SkipWhitespace();
            if (ch == '-')
                ch = ParseDeclComments();

            ParseAttType(ch, attdef);

            ch = current.SkipWhitespace();
            if (ch == '-')
                ch = ParseDeclComments();

            ParseAttDefault(ch, attdef);

            ch = current.SkipWhitespace();
            if (ch == '-')
                ch = ParseDeclComments();

            return attdef;

        }

        void ParseAttType(char ch, AttDef attdef)
        {
            if (ch == '%')
            {
                var e = ParseParameterEntity(WhiteSpace);
                PushEntity(current.ResolvedUri, e);
                ParseAttType(current.Lastchar, attdef);
                PopEntity(); // bugbug - are we at the end of the entity?
                ch = current.Lastchar;
                return;
            }

            if (ch == '(')
            {
                attdef.EnumValues = ParseNameGroup(ch, false);
                attdef.Type = AttributeType.ENUMERATION;
            }
            else
            {
                var token = ScanName(WhiteSpace);
                if (token == "NOTATION")
                {
                    ch = current.SkipWhitespace();
                    if (ch != '(')
                    {
                        current.Error("Expecting name group '(', but found '{0}'", ch);
                    }
                    attdef.Type = AttributeType.NOTATION;
                    attdef.EnumValues = ParseNameGroup(ch, true);
                }
                else
                {
                    attdef.SetType(token);
                }
            }
        }

        void ParseAttDefault(char ch, AttDef attdef)
        {
            if (ch == '%')
            {
                var e = ParseParameterEntity(WhiteSpace);
                PushEntity(current.ResolvedUri, e);
                ParseAttDefault(current.Lastchar, attdef);
                PopEntity(); // bugbug - are we at the end of the entity?
                ch = current.Lastchar;
                return;
            }

            var hasdef = true;
            if (ch == '#')
            {
                current.ReadChar();
                var token = current.ScanToken(sb, WhiteSpace, true);
                hasdef = attdef.SetPresence(token);
                ch = current.SkipWhitespace();
            }
            if (hasdef)
            {
                if (ch == '\'' || ch == '"')
                {
                    var lit = current.ScanLiteral(sb, ch);
                    attdef.Default = lit;
                    ch = current.SkipWhitespace();
                }
                else
                {
                    var name = current.ScanToken(sb, WhiteSpace, false);
                    name = name.ToUpper();
                    name = nameTable.Add(name);
                    attdef.Default = name; // bugbug - must be one of the enumerated names.
                    ch = current.SkipWhitespace();
                }
            }
        }
    }

    class StringUtilities
    {
        public static bool EqualsIgnoreCase(string a, string b)
        {
            return string.Compare(a, b, true, CultureInfo.InvariantCulture) == 0;
        }
    }
}
