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

namespace RemotingProtocolParser.TCP
{
    /// <summary>.Net Remoting Protocol (via TCP) Parser
    /// </summary>
    public class TcpProtocolHandle : ProtocolStreamHandle
    {
        private static readonly byte[] PREAMBLE = Encoding.ASCII.GetBytes(".NET");
        public TcpProtocolHandle() : this(new MemoryStream()) { }
        public TcpProtocolHandle(Stream source) : base(source) { }

        /// <summary>remoting protocol premable, expected value is ".NET".
        /// </summary>
        /// <returns></returns>
        public string ReadPreamble()
        {
            return Encoding.ASCII.GetString(new Byte[] { 
                (byte)this.ReadByte(), 
                (byte)this.ReadByte(), 
                (byte)this.ReadByte(), 
                (byte)this.ReadByte() });
        }
        public void WritePreamble()
        {
            this.WriteBytes(PREAMBLE);
        }

        /// <summary>remoting protocol majorVersion, expected value is "1".
        /// </summary>
        /// <returns></returns>
        public int ReadMajorVersion()
        {
            return this.ReadByte();
        }
        public void WriteMajorVersion()
        {
            this.WriteByte((byte)1);
        }

        /// <summary>remoting protocol minorVersion, expected value is "0".
        /// </summary>
        /// <returns></returns>
        public int ReadMinorVersion()
        {
            return this.ReadByte();
        }
        public void WriteMinorVersion()
        {
            this.WriteByte((byte)0);
        }
        
        /// <summary>remoting operation code, eg Request/OneWayRequest/Reply
        /// </summary>
        /// <returns></returns>
        public ushort ReadOperation()
        {
            return this.ReadUInt16();
        }
        /// <summary>write remoting operation code
        /// </summary>
        /// <param name="value">Request/OneWayRequest/Reply</param>
        public void WriteOperation(ushort value)
        {
            this.WriteUInt16(value);
        }

        /// <summary>Chunked or Fixed ContentLength. Only http channel support currently.
        /// </summary>
        /// <returns></returns>
        public ushort ReadContentDelimiter()
        {
            return this.ReadUInt16();
        }
        /// <summary>ContentLength=0, Chunked=1
        /// </summary>
        /// <param name="value"></param>
        public void WriteContentDelimiter(ushort value)
        {
            this.WriteUInt16(value);
        }

        /// <summary>get message content length
        /// </summary>
        /// <returns></returns>
        public int ReadContentLength()
        {
            return this._contentLength = this.ReadInt32();
        }
        public void WriteContentLength(int value)
        {
            this.WriteInt32(this._contentLength = value);
        }

        public IDictionary<string, object> ReadTransportHeaders()
        {
            var dict = new Dictionary<string, object>();
            ushort headerType = this.ReadUInt16();

            while (headerType != TcpHeaders.EndOfHeaders)
            {
                if (headerType == TcpHeaders.Custom)
                {
                    dict.Add(this.ReadCountedString(), this.ReadCountedString());
                }
                else if (headerType == TcpHeaders.RequestUri)
                {
                    this.ReadByte();//RequestUri-Format
                    dict.Add(TcpTransportHeader.RequestUri, this.ReadCountedString());
                }
                else if (headerType == TcpHeaders.StatusCode)
                {
                    this.ReadByte();//StatusCode-Format
                    dict.Add(TcpTransportHeader.StatusCode, this.ReadUInt16());
                    //HACK:error curse when StatusCode!=0
                    //if (code != 0) error = true;
                }
                else if (headerType == TcpHeaders.StatusPhrase)
                {
                    this.ReadByte();//StatusPhrase-Format
                    dict.Add(TcpTransportHeader.StatusPhrase, this.ReadCountedString());
                }
                else if (headerType == TcpHeaders.ContentType)
                {
                    this.ReadByte();//ContentType-Format
                    dict.Add(TcpTransportHeader.ContentType, this.ReadCountedString());
                }
                else if (this.ReadExtendedHeader(headerType, dict))
                {
                }
                else
                {
                    var headerFormat = (byte)ReadByte();

                    switch (headerFormat)
                    {
                        case TcpHeaderFormat.Void: break;
                        case TcpHeaderFormat.CountedString: this.ReadCountedString(); break;
                        case TcpHeaderFormat.Byte: this.ReadByte(); break;
                        case TcpHeaderFormat.UInt16: this.ReadUInt16(); break;
                        case TcpHeaderFormat.Int32: this.ReadInt32(); break;
                        default: throw new NotSupportedException();
                    }
                }

                headerType = this.ReadUInt16();
            }
            return dict;
        }
        /// <summary>write transport header. PS: "RequestUri" must be transport while request call
        /// </summary>
        /// <param name="headers"></param>
        public void WriteTransportHeaders(IDictionary<string, object> headers)
        {
            if (headers != null)
                foreach (var i in headers)
                {
                    if (i.Key.Equals(TcpTransportHeader.StatusCode, StringComparison.OrdinalIgnoreCase))
                        this.WriteStatusCodeHeader((ushort)i.Value);
                    else if (i.Key.Equals(TcpTransportHeader.StatusPhrase, StringComparison.OrdinalIgnoreCase))
                        this.WriteStatusPhraseHeader(i.Value.ToString());
                    else if (i.Key.Equals(TcpTransportHeader.ContentType, StringComparison.OrdinalIgnoreCase))
                        this.WriteContentTypeHeader(i.Value.ToString());
                    else if (i.Key.Equals(TcpTransportHeader.RequestUri, StringComparison.OrdinalIgnoreCase))
                        //Request-Uri must be transport while request call
                        this.WriteRequestUriHeader(i.Value.ToString());
                    else if (this.WriteExtendedHeader(i)) { }
                    else
                        this.WriteCustomHeader(i.Key, i.Value.ToString());
                }
            this.WriteUInt16(TcpHeaders.EndOfHeaders);
        }

        protected virtual bool ReadExtendedHeader(ushort headerType, IDictionary<String, Object> dict)
        {
            return false;
        }
        protected virtual bool WriteExtendedHeader(KeyValuePair<string, object> item)
        {
            return false;
        }

        protected ushort ReadUInt16()
        {
            return (ushort)(this.ReadByte() & 0xFF | this.ReadByte() << 8);
        }
        protected void WriteUInt16(ushort value)
        {
            this.WriteByte((byte)value);
            this.WriteByte((byte)(value >> 8));
        }

        protected int ReadInt32()
        {
            return (int)((this.ReadByte() & 0xFF)
                | this.ReadByte() << 8
                | this.ReadByte() << 16
                | this.ReadByte() << 24);
        }
        protected void WriteInt32(int value)
        {
            this.WriteByte((byte)value);
            this.WriteByte((byte)(value >> 8));
            this.WriteByte((byte)(value >> 16));
            this.WriteByte((byte)(value >> 24));
        }

        protected string ReadCountedString()
        {
            var format = (byte)this.ReadByte();
            int size = ReadInt32();

            if (size > 0)
            {
                byte[] data = this.ReadBytes(size);

                switch (format)
                {
                    case TcpStringFormat.Unicode:
                        return Encoding.Unicode.GetString(data);

                    case TcpStringFormat.UTF8:
                        return Encoding.UTF8.GetString(data);

                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                return null;
            }
        }
        protected void WriteCountedString(string value)
        {
            int strLength = 0;
            if (value != null)
                strLength = value.Length;

            if (strLength > 0)
            {
                byte[] strBytes = Encoding.UTF8.GetBytes(value);
                this.WriteByte(TcpStringFormat.UTF8);
                this.WriteInt32(strBytes.Length);
                this.WriteBytes(strBytes);
            }
            else
            {
                //just call it Unicode (doesn't matter since there is no data)
                this.WriteByte(TcpStringFormat.Unicode);
                this.WriteInt32(0);
            }
        }

        private void WriteRequestUriHeader(string value)
        {
            //value maybe "application/octet-stream"
            this.WriteUInt16(TcpHeaders.RequestUri);
            this.WriteByte(TcpHeaderFormat.CountedString);
            this.WriteCountedString(value);
        }
        private void WriteContentTypeHeader(string value)
        {
            this.WriteUInt16(TcpHeaders.ContentType);
            this.WriteByte(TcpHeaderFormat.CountedString);
            this.WriteCountedString(value);
        }
        private void WriteStatusCodeHeader(ushort value)
        {
            this.WriteUInt16(TcpHeaders.StatusCode);
            this.WriteByte(TcpHeaderFormat.UInt16);
            this.WriteUInt16(value);
        }
        private void WriteStatusPhraseHeader(string value)
        {
            this.WriteUInt16(TcpHeaders.StatusPhrase);
            this.WriteByte(TcpHeaderFormat.CountedString);
            this.WriteCountedString(value);
        }
        private void WriteCustomHeader(string name, string value)
        {
            this.WriteUInt16(TcpHeaders.Custom);
            this.WriteCountedString(name);
            this.WriteCountedString(value);
        }
    }
}
