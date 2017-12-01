using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    interface DtlsHandshakeRetransmit
    {
        /// <exception cref="IOException"/>
        void ReceivedHandshakeRecord(int epoch, byte[] buf, int off, int len);
    }
}
