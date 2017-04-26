/*
    (The MIT License)

    Copyright (C) 2012 wsky (wskyhx at gmail.com) and other contributors

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.IO;

namespace RemotingProtocolParser
{
    /// <summary>.Net Remoting Protocol stream read/write
    /// </summary>
    public abstract class ProtocolStreamHandle
    {
        protected int _contentLength = -1;
        protected Stream _source;
        protected ProtocolStreamHandle(Stream source)
        {
            this._source = source;
        }

        protected int ReadByte()
        {
            var b = this._source.ReadByte();
            if (b > -1)
                return b;
            else
                return -1;
        }
        protected void WriteByte(byte value)
        {
            this._source.WriteByte(value);
        }

        protected byte[] ReadBytes(int length)
        {
            //TODO:improve btye buffer
            //readBuffer as result buffer
            //HACK:.net remoting System.Runtime.Remoting.Channels。SocketHandler has best implementation
            //HACK:mono remoting write relatively simple. maybe have Memory fragmentation problem.
            var buffer = new byte[length];
            //buffer will pinned
            this._source.Read(buffer, 0, length);
            return buffer;
        }
        protected void WriteBytes(byte[] value)
        {
            this._source.Write(value, 0, value.Length);
        }

        /// <summary>read message content by content-length
        /// </summary>
        /// <returns></returns>
        public byte[] ReadContent()
        {
            if (this._contentLength == -1)
                throw new InvalidOperationException(
                    "You must call ReadContentLength first or ContentLength is invalid");
            return this.ReadBytes(this._contentLength);
        }
        /// <summary>write serialized message content by content-length
        /// </summary>
        /// <param name="value"></param>
        public void WriteContent(byte[] value)
        {
            if (value.Length != this._contentLength)
                throw new InvalidOperationException("value length must be equal to ContentLength");
            this.WriteBytes(value);
        }
    }
}