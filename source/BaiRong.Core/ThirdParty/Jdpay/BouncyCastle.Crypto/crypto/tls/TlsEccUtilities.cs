using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.Field;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public abstract class TlsEccUtilities
    {
        private static readonly string[] CurveNames = new string[] { "sect163k1", "sect163r1", "sect163r2", "sect193r1",
            "sect193r2", "sect233k1", "sect233r1", "sect239k1", "sect283k1", "sect283r1", "sect409k1", "sect409r1",
            "sect571k1", "sect571r1", "secp160k1", "secp160r1", "secp160r2", "secp192k1", "secp192r1", "secp224k1",
            "secp224r1", "secp256k1", "secp256r1", "secp384r1", "secp521r1",
            "brainpoolP256r1", "brainpoolP384r1", "brainpoolP512r1"};

        public static void AddSupportedEllipticCurvesExtension(IDictionary extensions, int[] namedCurves)
        {
            extensions[ExtensionType.elliptic_curves] = CreateSupportedEllipticCurvesExtension(namedCurves);
        }

        public static void AddSupportedPointFormatsExtension(IDictionary extensions, byte[] ecPointFormats)
        {
            extensions[ExtensionType.ec_point_formats] = CreateSupportedPointFormatsExtension(ecPointFormats);
        }

        public static int[] GetSupportedEllipticCurvesExtension(IDictionary extensions)
        {
            byte[] extensionData = TlsUtilities.GetExtensionData(extensions, ExtensionType.elliptic_curves);
            return extensionData == null ? null : ReadSupportedEllipticCurvesExtension(extensionData);
        }

        public static byte[] GetSupportedPointFormatsExtension(IDictionary extensions)
        {
            byte[] extensionData = TlsUtilities.GetExtensionData(extensions, ExtensionType.ec_point_formats);
            return extensionData == null ? null : ReadSupportedPointFormatsExtension(extensionData);
        }

        public static byte[] CreateSupportedEllipticCurvesExtension(int[] namedCurves)
        {
            if (namedCurves == null || namedCurves.Length < 1)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            return TlsUtilities.EncodeUint16ArrayWithUint16Length(namedCurves);
        }

        public static byte[] CreateSupportedPointFormatsExtension(byte[] ecPointFormats)
        {
            if (ecPointFormats == null || !Arrays.Contains(ecPointFormats, ECPointFormat.uncompressed))
            {
                /*
                 * RFC 4492 5.1. If the Supported Point Formats Extension is indeed sent, it MUST
                 * contain the value 0 (uncompressed) as one of the items in the list of point formats.
                 */

                // NOTE: We add it at the end (lowest preference)
                ecPointFormats = Arrays.Append(ecPointFormats, ECPointFormat.uncompressed);
            }

            return TlsUtilities.EncodeUint8ArrayWithUint8Length(ecPointFormats);
        }

        public static int[] ReadSupportedEllipticCurvesExtension(byte[] extensionData)
        {
            if (extensionData == null)
                throw new ArgumentNullException("extensionData");

            MemoryStream buf = new MemoryStream(extensionData, false);

            int length = TlsUtilities.ReadUint16(buf);
            if (length < 2 || (length & 1) != 0)
                throw new TlsFatalAlert(AlertDescription.decode_error);

            int[] namedCurves = TlsUtilities.ReadUint16Array(length / 2, buf);

            TlsProtocol.AssertEmpty(buf);

            return namedCurves;
        }

        public static byte[] ReadSupportedPointFormatsExtension(byte[] extensionData)
        {
            byte[] ecPointFormats = TlsUtilities.DecodeUint8ArrayWithUint8Length(extensionData);
            if (!Arrays.Contains(ecPointFormats, ECPointFormat.uncompressed))
            {
                /*
                 * RFC 4492 5.1. If the Supported Point Formats Extension is indeed sent, it MUST
                 * contain the value 0 (uncompressed) as one of the items in the list of point formats.
                 */
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }
            return ecPointFormats;
        }

        public static string GetNameOfNamedCurve(int namedCurve)
        {
            return IsSupportedNamedCurve(namedCurve) ? CurveNames[namedCurve - 1] : null;
        }

        public static ECDomainParameters GetParametersForNamedCurve(int namedCurve)
        {
            string curveName = GetNameOfNamedCurve(namedCurve);
            if (curveName == null)
                return null;

            // Parameters are lazily created the first time a particular curve is accessed

            X9ECParameters ecP = CustomNamedCurves.GetByName(curveName);
            if (ecP == null)
            {
                ecP = ECNamedCurveTable.GetByName(curveName);
                if (ecP == null)
                    return null;
            }

            // It's a bit inefficient to do this conversion every time
            return new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H, ecP.GetSeed());
        }

        public static bool HasAnySupportedNamedCurves()
        {
            return CurveNames.Length > 0;
        }

        public static bool ContainsEccCipherSuites(int[] cipherSuites)
        {
            for (int i = 0; i < cipherSuites.Length; ++i)
            {
                if (IsEccCipherSuite(cipherSuites[i]))
                    return true;
            }
            return false;
        }

        public static bool IsEccCipherSuite(int cipherSuite)
        {
            switch (cipherSuite)
            {
            /*
             * RFC 4492
             */
            case CipherSuite.TLS_ECDH_ECDSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_AES_256_CBC_SHA:

            /*
             * RFC 5289
             */
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_GCM_SHA384:

            /*
             * RFC 5489
             */
            case CipherSuite.TLS_ECDHE_PSK_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA256:
            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA384:
            case CipherSuite.TLS_ECDHE_PSK_WITH_RC4_128_SHA:

            /*
             * RFC 6367
             */
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_CBC_SHA384:

            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_GCM_SHA384:

            case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:

            /*
             * RFC 7251
             */
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8:

            /*
             * draft-ietf-tls-chacha20-poly1305-04
             */
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256:

            /*
             * draft-zauner-tls-aes-ocb-04
             */
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_128_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_256_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_128_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_256_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_128_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_256_OCB:

                return true;

            default:
                return false;
            }
        }

        public static bool AreOnSameCurve(ECDomainParameters a, ECDomainParameters b)
        {
            return a != null && a.Equals(b);
        }

        public static bool IsSupportedNamedCurve(int namedCurve)
        {
            return (namedCurve > 0 && namedCurve <= CurveNames.Length);
        }

        public static bool IsCompressionPreferred(byte[] ecPointFormats, byte compressionFormat)
        {
            if (ecPointFormats == null)
                return false;

            for (int i = 0; i < ecPointFormats.Length; ++i)
            {
                byte ecPointFormat = ecPointFormats[i];
                if (ecPointFormat == ECPointFormat.uncompressed)
                    return false;
                if (ecPointFormat == compressionFormat)
                    return true;
            }
            return false;
        }

        public static byte[] SerializeECFieldElement(int fieldSize, BigInteger x)
        {
            return BigIntegers.AsUnsignedByteArray((fieldSize + 7) / 8, x);
        }

        public static byte[] SerializeECPoint(byte[] ecPointFormats, ECPoint point)
        {
            ECCurve curve = point.Curve;

            /*
             * RFC 4492 5.7. ...an elliptic curve point in uncompressed or compressed format. Here, the
             * format MUST conform to what the server has requested through a Supported Point Formats
             * Extension if this extension was used, and MUST be uncompressed if this extension was not
             * used.
             */
            bool compressed = false;
            if (ECAlgorithms.IsFpCurve(curve))
            {
                compressed = IsCompressionPreferred(ecPointFormats, ECPointFormat.ansiX962_compressed_prime);
            }
            else if (ECAlgorithms.IsF2mCurve(curve))
            {
                compressed = IsCompressionPreferred(ecPointFormats, ECPointFormat.ansiX962_compressed_char2);
            }
            return point.GetEncoded(compressed);
        }

        public static byte[] SerializeECPublicKey(byte[] ecPointFormats, ECPublicKeyParameters keyParameters)
        {
            return SerializeECPoint(ecPointFormats, keyParameters.Q);
        }

        public static BigInteger DeserializeECFieldElement(int fieldSize, byte[] encoding)
        {
            int requiredLength = (fieldSize + 7) / 8;
            if (encoding.Length != requiredLength)
                throw new TlsFatalAlert(AlertDescription.decode_error);
            return new BigInteger(1, encoding);
        }

        public static ECPoint DeserializeECPoint(byte[] ecPointFormats, ECCurve curve, byte[] encoding)
        {
            if (encoding == null || encoding.Length < 1)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            byte actualFormat;
            switch (encoding[0])
            {
            case 0x02: // compressed
            case 0x03: // compressed
            {
                if (ECAlgorithms.IsF2mCurve(curve))
                {
                    actualFormat = ECPointFormat.ansiX962_compressed_char2;
                }
                else if (ECAlgorithms.IsFpCurve(curve))
                {
                    actualFormat = ECPointFormat.ansiX962_compressed_prime;
                }
                else
                {
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                }
                break;
            }
            case 0x04: // uncompressed
            {
                actualFormat = ECPointFormat.uncompressed;
                break;
            }
            case 0x00: // infinity
            case 0x06: // hybrid
            case 0x07: // hybrid
            default:
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }

            if (actualFormat != ECPointFormat.uncompressed
                && (ecPointFormats == null || !Arrays.Contains(ecPointFormats, actualFormat)))
            {
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }

            return curve.DecodePoint(encoding);
        }

        public static ECPublicKeyParameters DeserializeECPublicKey(byte[] ecPointFormats, ECDomainParameters curve_params,
            byte[] encoding)
        {
            try
            {
                ECPoint Y = DeserializeECPoint(ecPointFormats, curve_params.Curve, encoding);
                return new ECPublicKeyParameters(Y, curve_params);
            }
            catch (Exception e)
            {
                throw new TlsFatalAlert(AlertDescription.illegal_parameter, e);
            }
        }

        public static byte[] CalculateECDHBasicAgreement(ECPublicKeyParameters publicKey, ECPrivateKeyParameters privateKey)
        {
            ECDHBasicAgreement basicAgreement = new ECDHBasicAgreement();
            basicAgreement.Init(privateKey);
            BigInteger agreementValue = basicAgreement.CalculateAgreement(publicKey);

            /*
             * RFC 4492 5.10. Note that this octet string (Z in IEEE 1363 terminology) as output by
             * FE2OSP, the Field Element to Octet String Conversion Primitive, has constant length for
             * any given field; leading zeros found in this octet string MUST NOT be truncated.
             */
            return BigIntegers.AsUnsignedByteArray(basicAgreement.GetFieldSize(), agreementValue);
        }

        public static AsymmetricCipherKeyPair GenerateECKeyPair(SecureRandom random, ECDomainParameters ecParams)
        {
            ECKeyPairGenerator keyPairGenerator = new ECKeyPairGenerator();
            keyPairGenerator.Init(new ECKeyGenerationParameters(ecParams, random));
            return keyPairGenerator.GenerateKeyPair();
        }

        public static ECPrivateKeyParameters GenerateEphemeralClientKeyExchange(SecureRandom random, byte[] ecPointFormats,
            ECDomainParameters ecParams, Stream output)
        {
            AsymmetricCipherKeyPair kp = GenerateECKeyPair(random, ecParams);

            ECPublicKeyParameters ecPublicKey = (ECPublicKeyParameters)kp.Public;
            WriteECPoint(ecPointFormats, ecPublicKey.Q, output);

            return (ECPrivateKeyParameters)kp.Private;
        }

        // TODO Refactor around ServerECDHParams before making this public
        internal static ECPrivateKeyParameters GenerateEphemeralServerKeyExchange(SecureRandom random, int[] namedCurves,
            byte[] ecPointFormats, Stream output)
        {
            /* First we try to find a supported named curve from the client's list. */
            int namedCurve = -1;
            if (namedCurves == null)
            {
                // TODO Let the peer choose the default named curve
                namedCurve = NamedCurve.secp256r1;
            }
            else
            {
                for (int i = 0; i < namedCurves.Length; ++i)
                {
                    int entry = namedCurves[i];
                    if (NamedCurve.IsValid(entry) && IsSupportedNamedCurve(entry))
                    {
                        namedCurve = entry;
                        break;
                    }
                }
            }

            ECDomainParameters ecParams = null;
            if (namedCurve >= 0)
            {
                ecParams = GetParametersForNamedCurve(namedCurve);
            }
            else
            {
                /* If no named curves are suitable, check if the client supports explicit curves. */
                if (Arrays.Contains(namedCurves, NamedCurve.arbitrary_explicit_prime_curves))
                {
                    ecParams = GetParametersForNamedCurve(NamedCurve.secp256r1);
                }
                else if (Arrays.Contains(namedCurves, NamedCurve.arbitrary_explicit_char2_curves))
                {
                    ecParams = GetParametersForNamedCurve(NamedCurve.sect283r1);
                }
            }

            if (ecParams == null)
            {
                /*
                 * NOTE: We shouldn't have negotiated ECDHE key exchange since we apparently can't find
                 * a suitable curve.
                 */
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }

            if (namedCurve < 0)
            {
                WriteExplicitECParameters(ecPointFormats, ecParams, output);
            }
            else
            {
                WriteNamedECParameters(namedCurve, output);
            }

            return GenerateEphemeralClientKeyExchange(random, ecPointFormats, ecParams, output);
        }

        public static ECPublicKeyParameters ValidateECPublicKey(ECPublicKeyParameters key)
        {
            // TODO Check RFC 4492 for validation
            return key;
        }

        public static int ReadECExponent(int fieldSize, Stream input)
        {
            BigInteger K = ReadECParameter(input);
            if (K.BitLength < 32)
            {
                int k = K.IntValue;
                if (k > 0 && k < fieldSize)
                {
                    return k;
                }
            }
            throw new TlsFatalAlert(AlertDescription.illegal_parameter);
        }

        public static BigInteger ReadECFieldElement(int fieldSize, Stream input)
        {
            return DeserializeECFieldElement(fieldSize, TlsUtilities.ReadOpaque8(input));
        }

        public static BigInteger ReadECParameter(Stream input)
        {
            // TODO Are leading zeroes okay here?
            return new BigInteger(1, TlsUtilities.ReadOpaque8(input));
        }

        public static ECDomainParameters ReadECParameters(int[] namedCurves, byte[] ecPointFormats, Stream input)
        {
            try
            {
                byte curveType = TlsUtilities.ReadUint8(input);

                switch (curveType)
                {
                case ECCurveType.explicit_prime:
                {
                    CheckNamedCurve(namedCurves, NamedCurve.arbitrary_explicit_prime_curves);

                    BigInteger prime_p = ReadECParameter(input);
                    BigInteger a = ReadECFieldElement(prime_p.BitLength, input);
                    BigInteger b = ReadECFieldElement(prime_p.BitLength, input);
                    byte[] baseEncoding = TlsUtilities.ReadOpaque8(input);
                    BigInteger order = ReadECParameter(input);
                    BigInteger cofactor = ReadECParameter(input);
                    ECCurve curve = new FpCurve(prime_p, a, b, order, cofactor);
                    ECPoint basePoint = DeserializeECPoint(ecPointFormats, curve, baseEncoding);
                    return new ECDomainParameters(curve, basePoint, order, cofactor);
                }
                case ECCurveType.explicit_char2:
                {
                    CheckNamedCurve(namedCurves, NamedCurve.arbitrary_explicit_char2_curves);

                    int m = TlsUtilities.ReadUint16(input);
                    byte basis = TlsUtilities.ReadUint8(input);
                    if (!ECBasisType.IsValid(basis))
                        throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                    int k1 = ReadECExponent(m, input), k2 = -1, k3 = -1;
                    if (basis == ECBasisType.ec_basis_pentanomial)
                    {
                        k2 = ReadECExponent(m, input);
                        k3 = ReadECExponent(m, input);
                    }

                    BigInteger a = ReadECFieldElement(m, input);
                    BigInteger b = ReadECFieldElement(m, input);
                    byte[] baseEncoding = TlsUtilities.ReadOpaque8(input);
                    BigInteger order = ReadECParameter(input);
                    BigInteger cofactor = ReadECParameter(input);

                    ECCurve curve = (basis == ECBasisType.ec_basis_pentanomial)
                        ? new F2mCurve(m, k1, k2, k3, a, b, order, cofactor)
                        : new F2mCurve(m, k1, a, b, order, cofactor);

                    ECPoint basePoint = DeserializeECPoint(ecPointFormats, curve, baseEncoding);

                    return new ECDomainParameters(curve, basePoint, order, cofactor);
                }
                case ECCurveType.named_curve:
                {
                    int namedCurve = TlsUtilities.ReadUint16(input);
                    if (!NamedCurve.RefersToASpecificNamedCurve(namedCurve))
                    {
                        /*
                         * RFC 4492 5.4. All those values of NamedCurve are allowed that refer to a
                         * specific curve. Values of NamedCurve that indicate support for a class of
                         * explicitly defined curves are not allowed here [...].
                         */
                        throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                    }

                    CheckNamedCurve(namedCurves, namedCurve);

                    return GetParametersForNamedCurve(namedCurve);
                }
                default:
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                }
            }
            catch (Exception e)
            {
                throw new TlsFatalAlert(AlertDescription.illegal_parameter, e);
            }
        }

        private static void CheckNamedCurve(int[] namedCurves, int namedCurve)
        {
            if (namedCurves != null && !Arrays.Contains(namedCurves, namedCurve))
            {
                /*
                 * RFC 4492 4. [...] servers MUST NOT negotiate the use of an ECC cipher suite
                 * unless they can complete the handshake while respecting the choice of curves
                 * and compression techniques specified by the client.
                 */
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }
        }

        public static void WriteECExponent(int k, Stream output)
        {
            BigInteger K = BigInteger.ValueOf(k);
            WriteECParameter(K, output);
        }

        public static void WriteECFieldElement(ECFieldElement x, Stream output)
        {
            TlsUtilities.WriteOpaque8(x.GetEncoded(), output);
        }

        public static void WriteECFieldElement(int fieldSize, BigInteger x, Stream output)
        {
            TlsUtilities.WriteOpaque8(SerializeECFieldElement(fieldSize, x), output);
        }

        public static void WriteECParameter(BigInteger x, Stream output)
        {
            TlsUtilities.WriteOpaque8(BigIntegers.AsUnsignedByteArray(x), output);
        }

        public static void WriteExplicitECParameters(byte[] ecPointFormats, ECDomainParameters ecParameters,
            Stream output)
        {
            ECCurve curve = ecParameters.Curve;

            if (ECAlgorithms.IsFpCurve(curve))
            {
                TlsUtilities.WriteUint8(ECCurveType.explicit_prime, output);

                WriteECParameter(curve.Field.Characteristic, output);
            }
            else if (ECAlgorithms.IsF2mCurve(curve))
            {
                IPolynomialExtensionField field = (IPolynomialExtensionField)curve.Field;
                int[] exponents = field.MinimalPolynomial.GetExponentsPresent();

                TlsUtilities.WriteUint8(ECCurveType.explicit_char2, output);

                int m = exponents[exponents.Length - 1];
                TlsUtilities.CheckUint16(m);
                TlsUtilities.WriteUint16(m, output);

                if (exponents.Length == 3)
                {
                    TlsUtilities.WriteUint8(ECBasisType.ec_basis_trinomial, output);
                    WriteECExponent(exponents[1], output);
                }
                else if (exponents.Length == 5)
                {
                    TlsUtilities.WriteUint8(ECBasisType.ec_basis_pentanomial, output);
                    WriteECExponent(exponents[1], output);
                    WriteECExponent(exponents[2], output);
                    WriteECExponent(exponents[3], output);
                }
                else
                {
                    throw new ArgumentException("Only trinomial and pentomial curves are supported");
                }
            }
            else
            {
                throw new ArgumentException("'ecParameters' not a known curve type");
            }

            WriteECFieldElement(curve.A, output);
            WriteECFieldElement(curve.B, output);
            TlsUtilities.WriteOpaque8(SerializeECPoint(ecPointFormats, ecParameters.G), output);
            WriteECParameter(ecParameters.N, output);
            WriteECParameter(ecParameters.H, output);
        }

        public static void WriteECPoint(byte[] ecPointFormats, ECPoint point, Stream output)
        {
            TlsUtilities.WriteOpaque8(SerializeECPoint(ecPointFormats, point), output);
        }

        public static void WriteNamedECParameters(int namedCurve, Stream output)
        {
            if (!NamedCurve.RefersToASpecificNamedCurve(namedCurve))
            {
                /*
                 * RFC 4492 5.4. All those values of NamedCurve are allowed that refer to a specific
                 * curve. Values of NamedCurve that indicate support for a class of explicitly defined
                 * curves are not allowed here [...].
                 */
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }

            TlsUtilities.WriteUint8(ECCurveType.named_curve, output);
            TlsUtilities.CheckUint16(namedCurve);
            TlsUtilities.WriteUint16(namedCurve, output);
        }
    }
}
