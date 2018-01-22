using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>RFC 2246</summary>
    /// <remarks>
    /// Note that the values here are implementation-specific and arbitrary. It is recommended not to
    /// depend on the particular values (e.g. serialization).
    /// </remarks>
    public abstract class KeyExchangeAlgorithm
    {
        public const int NULL = 0;
        public const int RSA = 1;
        public const int RSA_EXPORT = 2;
        public const int DHE_DSS = 3;
        public const int DHE_DSS_EXPORT = 4;
        public const int DHE_RSA = 5;
        public const int DHE_RSA_EXPORT = 6;
        public const int DH_DSS = 7;
        public const int DH_DSS_EXPORT = 8;
        public const int DH_RSA = 9;
        public const int DH_RSA_EXPORT = 10;
        public const int DH_anon = 11;
        public const int DH_anon_EXPORT = 12;

        /*
         * RFC 4279
         */
        public const int PSK = 13;
        public const int DHE_PSK = 14;
        public const int RSA_PSK = 15;

        /*
         * RFC 4429
         */
        public const int ECDH_ECDSA = 16;
        public const int ECDHE_ECDSA = 17;
        public const int ECDH_RSA = 18;
        public const int ECDHE_RSA = 19;
        public const int ECDH_anon = 20;

        /*
         * RFC 5054
         */
        public const int SRP = 21;
        public const int SRP_DSS = 22;
        public const int SRP_RSA = 23;
    
        /*
         * RFC 5489
         */
        public const int ECDHE_PSK = 24;
    }
}
