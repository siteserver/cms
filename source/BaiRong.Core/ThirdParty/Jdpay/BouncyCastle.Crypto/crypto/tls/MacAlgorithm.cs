using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>RFC 2246</summary>
    /// <remarks>
    /// Note that the values here are implementation-specific and arbitrary. It is recommended not to
    /// depend on the particular values (e.g. serialization).
    /// </remarks>
    public abstract class MacAlgorithm
    {
        public const int cls_null = 0;
        public const int md5 = 1;
        public const int sha = 2;

        /*
         * RFC 5246
         */
        public const int hmac_md5 = md5;
        public const int hmac_sha1 = sha;
        public const int hmac_sha256 = 3;
        public const int hmac_sha384 = 4;
        public const int hmac_sha512 = 5;
    }
}
