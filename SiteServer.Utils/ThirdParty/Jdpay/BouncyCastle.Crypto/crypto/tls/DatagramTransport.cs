using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public interface DatagramTransport
    {
        /// <exception cref="IOException"/>
        int GetReceiveLimit();

        /// <exception cref="IOException"/>
        int GetSendLimit();

        /// <exception cref="IOException"/>
        int Receive(byte[] buf, int off, int len, int waitMillis);

        /// <exception cref="IOException"/>
        void Send(byte[] buf, int off, int len);

        /// <exception cref="IOException"/>
        void Close();
    }
}
