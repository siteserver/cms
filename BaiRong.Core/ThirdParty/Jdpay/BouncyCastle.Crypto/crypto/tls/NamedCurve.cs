using System;

using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>
    /// RFC 4492 5.1.1
    /// The named curves defined here are those specified in SEC 2 [13]. Note that many of
    /// these curves are also recommended in ANSI X9.62 [7] and FIPS 186-2 [11]. Values 0xFE00
    /// through 0xFEFF are reserved for private use. Values 0xFF01 and 0xFF02 indicate that the
    /// client supports arbitrary prime and characteristic-2 curves, respectively (the curve
    /// parameters must be encoded explicitly in ECParameters).
    /// </summary>
    public abstract class NamedCurve
    {
        public const int sect163k1 = 1;
        public const int sect163r1 = 2;
        public const int sect163r2 = 3;
        public const int sect193r1 = 4;
        public const int sect193r2 = 5;
        public const int sect233k1 = 6;
        public const int sect233r1 = 7;
        public const int sect239k1 = 8;
        public const int sect283k1 = 9;
        public const int sect283r1 = 10;
        public const int sect409k1 = 11;
        public const int sect409r1 = 12;
        public const int sect571k1 = 13;
        public const int sect571r1 = 14;
        public const int secp160k1 = 15;
        public const int secp160r1 = 16;
        public const int secp160r2 = 17;
        public const int secp192k1 = 18;
        public const int secp192r1 = 19;
        public const int secp224k1 = 20;
        public const int secp224r1 = 21;
        public const int secp256k1 = 22;
        public const int secp256r1 = 23;
        public const int secp384r1 = 24;
        public const int secp521r1 = 25;
    
        /*
         * RFC 7027
         */
        public const int brainpoolP256r1 = 26;
        public const int brainpoolP384r1 = 27;
        public const int brainpoolP512r1 = 28;

        /*
         * reserved (0xFE00..0xFEFF)
         */

        public const int arbitrary_explicit_prime_curves = 0xFF01;
        public const int arbitrary_explicit_char2_curves = 0xFF02;

        public static bool IsValid(int namedCurve)
        {
            return (namedCurve >= sect163k1 && namedCurve <= brainpoolP512r1)
                || (namedCurve >= arbitrary_explicit_prime_curves && namedCurve <= arbitrary_explicit_char2_curves);
        }

        public static bool RefersToASpecificNamedCurve(int namedCurve)
        {
            switch (namedCurve)
            {
            case arbitrary_explicit_prime_curves:
            case arbitrary_explicit_char2_curves:
                return false;
            default:
                return true;
            }
        }
    }
}
