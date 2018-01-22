using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public abstract class AbstractTlsPeer
        :   TlsPeer
    {
        public virtual bool ShouldUseGmtUnixTime()
        {
            /*
             * draft-mathewson-no-gmtunixtime-00 2. For the reasons we discuss above, we recommend that
             * TLS implementors MUST by default set the entire value the ClientHello.Random and
             * ServerHello.Random fields, including gmt_unix_time, to a cryptographically random
             * sequence.
             */
            return false;
        }

        public virtual void NotifySecureRenegotiation(bool secureRenegotiation)
        {
            if (!secureRenegotiation)
            {
                /*
                 * RFC 5746 3.4/3.6. In this case, some clients/servers may want to terminate the handshake instead
                 * of continuing; see Section 4.1/4.3 for discussion.
                 */
                throw new TlsFatalAlert(AlertDescription.handshake_failure);
            }
        }

        public abstract TlsCompression GetCompression();

        public abstract TlsCipher GetCipher();

        public virtual void NotifyAlertRaised(byte alertLevel, byte alertDescription, string message, Exception cause)
        {
        }

        public virtual void NotifyAlertReceived(byte alertLevel, byte alertDescription)
        {
        }

        public virtual void NotifyHandshakeComplete()
        {
        }
    }
}
