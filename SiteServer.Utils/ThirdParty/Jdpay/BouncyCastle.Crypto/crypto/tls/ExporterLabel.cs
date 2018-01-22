using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>RFC 5705</summary>
    public abstract class ExporterLabel
    {
        /*
         * RFC 5246
         */
        public const string client_finished = "client finished";
        public const string server_finished = "server finished";
        public const string master_secret = "master secret";
        public const string key_expansion = "key expansion";

        /*
         * RFC 5216
         */
        public const string client_EAP_encryption = "client EAP encryption";

        /*
         * RFC 5281
         */
        public const string ttls_keying_material = "ttls keying material";
        public const string ttls_challenge = "ttls challenge";

        /*
         * RFC 5764
         */
        public const string dtls_srtp = "EXTRACTOR-dtls_srtp";

        /*
         * draft-ietf-tls-session-hash-04
         */
        public static readonly string extended_master_secret = "extended master secret";
    }
}
