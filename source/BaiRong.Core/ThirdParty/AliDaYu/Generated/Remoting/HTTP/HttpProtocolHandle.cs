/*
    (The MIT License)

    Copyright (C) 2012 wsky (wskyhx at gmail.com) and other contributors

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RemotingProtocolParser.HTTP
{
    /// <summary>.Net Remoting Protocol (via HTTP) Parser
    /// </summary>
    public class HttpProtocolHandle : ProtocolStreamHandle
    {
        private static byte[] HTTP_VERSION = Encoding.ASCII.GetBytes("HTTP/1.1");
        private static byte[] SPACE = Encoding.ASCII.GetBytes(" ");
        private static byte[] HEADER_SEPARATOE = new byte[] { (byte)':', (byte)' ' };
        private static byte[] END_OF_LINE = new byte[] { (byte)'\r', (byte)'\n' };

        public HttpProtocolHandle() : this(new MemoryStream()) { }
        public HttpProtocolHandle(Stream source) : base(source) { }

        /// <summary>eg: POST /remote.rem HTTP/1.1
        /// </summary>
        /// <returns></returns>
        public string ReadFirstLine()
        {
            return this.ReadToEndOfLine();
        }
        /// <summary>http request first line, eg: POST /remote.rem HTTP/1.1
        /// </summary>
        /// <param name="requestVerb">GET/POST</param>
        /// <param name="url"></param>
        public void WriteRequestFirstLine(string requestVerb, string url)
        {
            this.WriteBytes(Encoding.ASCII.GetBytes(requestVerb));
            this.WriteByte((byte)' ');
            this.WriteBytes(Encoding.ASCII.GetBytes(url));
            this.WriteByte((byte)' ');
            this.WriteBytes(HTTP_VERSION);
            this.WriteBytes(END_OF_LINE);
        }
        /// <summary>http response first line, eg: HTTP/1.1 200 ok
        /// </summary>
        /// <param name="httpStatusCode"></param>
        /// <param name="reasonPhrase"></param>
        public void WriteResponseFirstLine(string httpStatusCode, string reasonPhrase)
        {
            this.WriteBytes(HTTP_VERSION);
            this.WriteBytes(SPACE);
            this.WriteBytes(Encoding.ASCII.GetBytes(httpStatusCode));
            this.WriteByte((byte)' ');
            this.WriteBytes(Encoding.ASCII.GetBytes(reasonPhrase));
            this.WriteBytes(END_OF_LINE);
        }

        /// <summary>read all http headers
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> ReadHeaders()
        {
            var dict = new Dictionary<string, object>();
            while (true)
            {
                string header = this.ReadToEndOfLine();

                if (header.Length == 0)
                    break;

                int sep = header.IndexOf(":");
                string headerName = header.Substring(0, sep);
                //skip semi-colon and space?
                string headerValue = header.Substring(sep + 1 + 1);
                dict.Add(headerName, headerValue);

                if (headerName.Equals(HttpHeader.ContentLength, StringComparison.OrdinalIgnoreCase))
                {
                    this._contentLength = int.Parse(headerValue);
                }
            }
            return dict;
        }
        /// <summary>write http headers
        /// </summary>
        /// <param name="headers"></param>
        public void WriteHeaders(IDictionary<string, object> headers)
        {
            if (headers != null)
                foreach (var h in headers)
                {
                    if (h.Key.Equals(HttpHeader.ContentLength, StringComparison.OrdinalIgnoreCase))
                        this._contentLength = int.Parse(h.Value.ToString());
                    this.WriteHeader(h.Key, h.Value.ToString());
                }
            this.WriteBytes(END_OF_LINE);
        }

        private void WriteHeader(string name, string value)
        {
            this.WriteBytes(Encoding.ASCII.GetBytes(name));
            this.WriteBytes(HEADER_SEPARATOE);
            this.WriteBytes(Encoding.ASCII.GetBytes(value));
            this.WriteBytes(END_OF_LINE);
        }
        private string ReadToEndOfLine()
        {
            var str = this.ReadToChar('\r');
            if (this.ReadByte() == '\n')
                return str;
            else
                return null;
        }
        private string ReadToChar(char value)
        {
            byte[] strBytes = this.ReadToByte((byte)value);
            if (strBytes == null)
                return null;
            if (strBytes.Length == 0)
                return string.Empty;
            return Encoding.ASCII.GetString(strBytes);
        }
        private byte[] ReadToByte(byte b)
        {
            //TODO:should read more than want, design internal readed buffer
            //_source should not be networkstream while sync socket
            var readBytes = new byte[0];
            var find = false;
            while (!find)
            {
                var read = (byte)this._source.ReadByte();
                find = read == b;

                if (!find)
                {
                    var ret = new byte[readBytes.Length + 1];
                    Buffer.BlockCopy(readBytes, 0, ret, 0, readBytes.Length);
                    ret[ret.Length - 1] = read;
                    readBytes = ret;
                }
            }

            return readBytes;
        }
    }
}