using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    /*
     * draft-ietf-tls-negotiated-ff-dhe-01
     */
    public abstract class FiniteFieldDheGroup
    {
        public const byte ffdhe2432 = 0;
        public const byte ffdhe3072 = 1;
        public const byte ffdhe4096 = 2;
        public const byte ffdhe6144 = 3;
        public const byte ffdhe8192 = 4;

        public static bool IsValid(byte group)
        {
            return group >= ffdhe2432 && group <= ffdhe8192;
        }
    }
}
