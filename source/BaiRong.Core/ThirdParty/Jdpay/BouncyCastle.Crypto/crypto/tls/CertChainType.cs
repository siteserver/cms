using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    /*
     * RFC 3546 3.3.
     */
    public abstract class CertChainType
    {
        public const byte individual_certs = 0;
        public const byte pkipath = 1;

        public static bool IsValid(byte certChainType)
        {
            return certChainType >= individual_certs && certChainType <= pkipath;
        }
    }
}
