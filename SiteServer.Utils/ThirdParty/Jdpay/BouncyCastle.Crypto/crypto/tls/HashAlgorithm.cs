using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>RFC 5246 7.4.1.4.1</summary>
    public abstract class HashAlgorithm
    {
        public const byte none = 0;
        public const byte md5 = 1;
        public const byte sha1 = 2;
        public const byte sha224 = 3;
        public const byte sha256 = 4;
        public const byte sha384 = 5;
        public const byte sha512 = 6;

        public static string GetName(byte hashAlgorithm)
        {
            switch (hashAlgorithm)
            {
            case none:
                return "none";
            case md5:
                return "md5";
            case sha1:
                return "sha1";
            case sha224:
                return "sha224";
            case sha256:
                return "sha256";
            case sha384:
                return "sha384";
            case sha512:
                return "sha512";
            default:
                return "UNKNOWN";
            }
        }

        public static string GetText(byte hashAlgorithm)
        {
            return GetName(hashAlgorithm) + "(" + hashAlgorithm + ")";
        }

        public static bool IsPrivate(byte hashAlgorithm)
        {
            return 224 <= hashAlgorithm && hashAlgorithm <= 255;
        }
    }
}
