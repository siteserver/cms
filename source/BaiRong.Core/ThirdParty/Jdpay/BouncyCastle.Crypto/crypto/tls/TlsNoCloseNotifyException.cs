using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>
    /// This exception will be thrown(only) when the connection is closed by the peer without sending a
    /// <code cref="AlertDescription.close_notify">close_notify</code> warning alert.
    /// </summary>
    /// <remarks>
    /// If this happens, the TLS protocol cannot rule out truncation of the connection data (potentially
    /// malicious). It may be possible to check for truncation via some property of a higher level protocol
    /// built upon TLS, e.g.the Content-Length header for HTTPS.
    /// </remarks>
    public class TlsNoCloseNotifyException
        :   EndOfStreamException
    {
        public TlsNoCloseNotifyException()
            : base("No close_notify alert received before connection closed")
        {
        }
    }
}
