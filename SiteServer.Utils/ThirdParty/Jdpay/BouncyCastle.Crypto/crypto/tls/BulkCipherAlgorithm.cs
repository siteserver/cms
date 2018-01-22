using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>RFC 2246</summary>
    /// <remarks>
    /// Note that the values here are implementation-specific and arbitrary. It is recommended not to
    /// depend on the particular values (e.g. serialization).
    /// </remarks>
    public abstract class BulkCipherAlgorithm
    {
        public const int cls_null = 0;
        public const int rc4 = 1;
        public const int rc2 = 2;
        public const int des = 3;
        public const int cls_3des = 4;
        public const int des40 = 5;

        /*
         * RFC 4346
         */
        public const int aes = 6;
        public const int idea = 7;
    }
}
