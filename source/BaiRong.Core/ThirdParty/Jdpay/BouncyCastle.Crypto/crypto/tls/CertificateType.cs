using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    /**
     * RFC 6091 
     */
    public class CertificateType
    {
        public const byte X509 = 0;
        public const byte OpenPGP = 1;
    
        /*
         * RFC 7250
         */
        public const byte RawPublicKey = 2;
    }
}
