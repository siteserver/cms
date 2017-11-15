using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Crypto.Tls
{
    public abstract class TlsDHUtilities
    {
        internal static readonly BigInteger Two = BigInteger.Two;

        /*
         * TODO[draft-ietf-tls-negotiated-ff-dhe-01] Move these groups to DHStandardGroups once reaches RFC
         */
        private static BigInteger FromHex(String hex)
        {
            return new BigInteger(1, Hex.Decode(hex));
        }

        private static DHParameters FromSafeP(String hexP)
        {
            BigInteger p = FromHex(hexP), q = p.ShiftRight(1);
            return new DHParameters(p, Two, q);
        }

        private static readonly string draft_ffdhe2432_p =
              "FFFFFFFFFFFFFFFFADF85458A2BB4A9AAFDC5620273D3CF1"
            + "D8B9C583CE2D3695A9E13641146433FBCC939DCE249B3EF9"
            + "7D2FE363630C75D8F681B202AEC4617AD3DF1ED5D5FD6561"
            + "2433F51F5F066ED0856365553DED1AF3B557135E7F57C935"
            + "984F0C70E0E68B77E2A689DAF3EFE8721DF158A136ADE735"
            + "30ACCA4F483A797ABC0AB182B324FB61D108A94BB2C8E3FB"
            + "B96ADAB760D7F4681D4F42A3DE394DF4AE56EDE76372BB19"
            + "0B07A7C8EE0A6D709E02FCE1CDF7E2ECC03404CD28342F61"
            + "9172FE9CE98583FF8E4F1232EEF28183C3FE3B1B4C6FAD73"
            + "3BB5FCBC2EC22005C58EF1837D1683B2C6F34A26C1B2EFFA"
            + "886B4238611FCFDCDE355B3B6519035BBC34F4DEF99C0238"
            + "61B46FC9D6E6C9077AD91D2691F7F7EE598CB0FAC186D91C"
            + "AEFE13098533C8B3FFFFFFFFFFFFFFFF";
        internal static readonly DHParameters draft_ffdhe2432 = FromSafeP(draft_ffdhe2432_p);

        private static readonly string draft_ffdhe3072_p =
              "FFFFFFFFFFFFFFFFADF85458A2BB4A9AAFDC5620273D3CF1"
            + "D8B9C583CE2D3695A9E13641146433FBCC939DCE249B3EF9"
            + "7D2FE363630C75D8F681B202AEC4617AD3DF1ED5D5FD6561"
            + "2433F51F5F066ED0856365553DED1AF3B557135E7F57C935"
            + "984F0C70E0E68B77E2A689DAF3EFE8721DF158A136ADE735"
            + "30ACCA4F483A797ABC0AB182B324FB61D108A94BB2C8E3FB"
            + "B96ADAB760D7F4681D4F42A3DE394DF4AE56EDE76372BB19"
            + "0B07A7C8EE0A6D709E02FCE1CDF7E2ECC03404CD28342F61"
            + "9172FE9CE98583FF8E4F1232EEF28183C3FE3B1B4C6FAD73"
            + "3BB5FCBC2EC22005C58EF1837D1683B2C6F34A26C1B2EFFA"
            + "886B4238611FCFDCDE355B3B6519035BBC34F4DEF99C0238"
            + "61B46FC9D6E6C9077AD91D2691F7F7EE598CB0FAC186D91C"
            + "AEFE130985139270B4130C93BC437944F4FD4452E2D74DD3"
            + "64F2E21E71F54BFF5CAE82AB9C9DF69EE86D2BC522363A0D"
            + "ABC521979B0DEADA1DBF9A42D5C4484E0ABCD06BFA53DDEF"
            + "3C1B20EE3FD59D7C25E41D2B66C62E37FFFFFFFFFFFFFFFF";
        internal static readonly DHParameters draft_ffdhe3072 = FromSafeP(draft_ffdhe3072_p);

        private static readonly string draft_ffdhe4096_p =
              "FFFFFFFFFFFFFFFFADF85458A2BB4A9AAFDC5620273D3CF1"
            + "D8B9C583CE2D3695A9E13641146433FBCC939DCE249B3EF9"
            + "7D2FE363630C75D8F681B202AEC4617AD3DF1ED5D5FD6561"
            + "2433F51F5F066ED0856365553DED1AF3B557135E7F57C935"
            + "984F0C70E0E68B77E2A689DAF3EFE8721DF158A136ADE735"
            + "30ACCA4F483A797ABC0AB182B324FB61D108A94BB2C8E3FB"
            + "B96ADAB760D7F4681D4F42A3DE394DF4AE56EDE76372BB19"
            + "0B07A7C8EE0A6D709E02FCE1CDF7E2ECC03404CD28342F61"
            + "9172FE9CE98583FF8E4F1232EEF28183C3FE3B1B4C6FAD73"
            + "3BB5FCBC2EC22005C58EF1837D1683B2C6F34A26C1B2EFFA"
            + "886B4238611FCFDCDE355B3B6519035BBC34F4DEF99C0238"
            + "61B46FC9D6E6C9077AD91D2691F7F7EE598CB0FAC186D91C"
            + "AEFE130985139270B4130C93BC437944F4FD4452E2D74DD3"
            + "64F2E21E71F54BFF5CAE82AB9C9DF69EE86D2BC522363A0D"
            + "ABC521979B0DEADA1DBF9A42D5C4484E0ABCD06BFA53DDEF"
            + "3C1B20EE3FD59D7C25E41D2B669E1EF16E6F52C3164DF4FB"
            + "7930E9E4E58857B6AC7D5F42D69F6D187763CF1D55034004"
            + "87F55BA57E31CC7A7135C886EFB4318AED6A1E012D9E6832"
            + "A907600A918130C46DC778F971AD0038092999A333CB8B7A"
            + "1A1DB93D7140003C2A4ECEA9F98D0ACC0A8291CDCEC97DCF"
            + "8EC9B55A7F88A46B4DB5A851F44182E1C68A007E5E655F6A"
            + "FFFFFFFFFFFFFFFF";
        internal static readonly DHParameters draft_ffdhe4096 = FromSafeP(draft_ffdhe4096_p);

        private static readonly string draft_ffdhe6144_p =
              "FFFFFFFFFFFFFFFFADF85458A2BB4A9AAFDC5620273D3CF1"
            + "D8B9C583CE2D3695A9E13641146433FBCC939DCE249B3EF9"
            + "7D2FE363630C75D8F681B202AEC4617AD3DF1ED5D5FD6561"
            + "2433F51F5F066ED0856365553DED1AF3B557135E7F57C935"
            + "984F0C70E0E68B77E2A689DAF3EFE8721DF158A136ADE735"
            + "30ACCA4F483A797ABC0AB182B324FB61D108A94BB2C8E3FB"
            + "B96ADAB760D7F4681D4F42A3DE394DF4AE56EDE76372BB19"
            + "0B07A7C8EE0A6D709E02FCE1CDF7E2ECC03404CD28342F61"
            + "9172FE9CE98583FF8E4F1232EEF28183C3FE3B1B4C6FAD73"
            + "3BB5FCBC2EC22005C58EF1837D1683B2C6F34A26C1B2EFFA"
            + "886B4238611FCFDCDE355B3B6519035BBC34F4DEF99C0238"
            + "61B46FC9D6E6C9077AD91D2691F7F7EE598CB0FAC186D91C"
            + "AEFE130985139270B4130C93BC437944F4FD4452E2D74DD3"
            + "64F2E21E71F54BFF5CAE82AB9C9DF69EE86D2BC522363A0D"
            + "ABC521979B0DEADA1DBF9A42D5C4484E0ABCD06BFA53DDEF"
            + "3C1B20EE3FD59D7C25E41D2B669E1EF16E6F52C3164DF4FB"
            + "7930E9E4E58857B6AC7D5F42D69F6D187763CF1D55034004"
            + "87F55BA57E31CC7A7135C886EFB4318AED6A1E012D9E6832"
            + "A907600A918130C46DC778F971AD0038092999A333CB8B7A"
            + "1A1DB93D7140003C2A4ECEA9F98D0ACC0A8291CDCEC97DCF"
            + "8EC9B55A7F88A46B4DB5A851F44182E1C68A007E5E0DD902"
            + "0BFD64B645036C7A4E677D2C38532A3A23BA4442CAF53EA6"
            + "3BB454329B7624C8917BDD64B1C0FD4CB38E8C334C701C3A"
            + "CDAD0657FCCFEC719B1F5C3E4E46041F388147FB4CFDB477"
            + "A52471F7A9A96910B855322EDB6340D8A00EF092350511E3"
            + "0ABEC1FFF9E3A26E7FB29F8C183023C3587E38DA0077D9B4"
            + "763E4E4B94B2BBC194C6651E77CAF992EEAAC0232A281BF6"
            + "B3A739C1226116820AE8DB5847A67CBEF9C9091B462D538C"
            + "D72B03746AE77F5E62292C311562A846505DC82DB854338A"
            + "E49F5235C95B91178CCF2DD5CACEF403EC9D1810C6272B04"
            + "5B3B71F9DC6B80D63FDD4A8E9ADB1E6962A69526D43161C1"
            + "A41D570D7938DAD4A40E329CD0E40E65FFFFFFFFFFFFFFFF";
        internal static readonly DHParameters draft_ffdhe6144 = FromSafeP(draft_ffdhe6144_p);

        private static readonly string draft_ffdhe8192_p =
              "FFFFFFFFFFFFFFFFADF85458A2BB4A9AAFDC5620273D3CF1"
            + "D8B9C583CE2D3695A9E13641146433FBCC939DCE249B3EF9"
            + "7D2FE363630C75D8F681B202AEC4617AD3DF1ED5D5FD6561"
            + "2433F51F5F066ED0856365553DED1AF3B557135E7F57C935"
            + "984F0C70E0E68B77E2A689DAF3EFE8721DF158A136ADE735"
            + "30ACCA4F483A797ABC0AB182B324FB61D108A94BB2C8E3FB"
            + "B96ADAB760D7F4681D4F42A3DE394DF4AE56EDE76372BB19"
            + "0B07A7C8EE0A6D709E02FCE1CDF7E2ECC03404CD28342F61"
            + "9172FE9CE98583FF8E4F1232EEF28183C3FE3B1B4C6FAD73"
            + "3BB5FCBC2EC22005C58EF1837D1683B2C6F34A26C1B2EFFA"
            + "886B4238611FCFDCDE355B3B6519035BBC34F4DEF99C0238"
            + "61B46FC9D6E6C9077AD91D2691F7F7EE598CB0FAC186D91C"
            + "AEFE130985139270B4130C93BC437944F4FD4452E2D74DD3"
            + "64F2E21E71F54BFF5CAE82AB9C9DF69EE86D2BC522363A0D"
            + "ABC521979B0DEADA1DBF9A42D5C4484E0ABCD06BFA53DDEF"
            + "3C1B20EE3FD59D7C25E41D2B669E1EF16E6F52C3164DF4FB"
            + "7930E9E4E58857B6AC7D5F42D69F6D187763CF1D55034004"
            + "87F55BA57E31CC7A7135C886EFB4318AED6A1E012D9E6832"
            + "A907600A918130C46DC778F971AD0038092999A333CB8B7A"
            + "1A1DB93D7140003C2A4ECEA9F98D0ACC0A8291CDCEC97DCF"
            + "8EC9B55A7F88A46B4DB5A851F44182E1C68A007E5E0DD902"
            + "0BFD64B645036C7A4E677D2C38532A3A23BA4442CAF53EA6"
            + "3BB454329B7624C8917BDD64B1C0FD4CB38E8C334C701C3A"
            + "CDAD0657FCCFEC719B1F5C3E4E46041F388147FB4CFDB477"
            + "A52471F7A9A96910B855322EDB6340D8A00EF092350511E3"
            + "0ABEC1FFF9E3A26E7FB29F8C183023C3587E38DA0077D9B4"
            + "763E4E4B94B2BBC194C6651E77CAF992EEAAC0232A281BF6"
            + "B3A739C1226116820AE8DB5847A67CBEF9C9091B462D538C"
            + "D72B03746AE77F5E62292C311562A846505DC82DB854338A"
            + "E49F5235C95B91178CCF2DD5CACEF403EC9D1810C6272B04"
            + "5B3B71F9DC6B80D63FDD4A8E9ADB1E6962A69526D43161C1"
            + "A41D570D7938DAD4A40E329CCFF46AAA36AD004CF600C838"
            + "1E425A31D951AE64FDB23FCEC9509D43687FEB69EDD1CC5E"
            + "0B8CC3BDF64B10EF86B63142A3AB8829555B2F747C932665"
            + "CB2C0F1CC01BD70229388839D2AF05E454504AC78B758282"
            + "2846C0BA35C35F5C59160CC046FD8251541FC68C9C86B022"
            + "BB7099876A460E7451A8A93109703FEE1C217E6C3826E52C"
            + "51AA691E0E423CFC99E9E31650C1217B624816CDAD9A95F9"
            + "D5B8019488D9C0A0A1FE3075A577E23183F81D4A3F2FA457"
            + "1EFC8CE0BA8A4FE8B6855DFE72B0A66EDED2FBABFBE58A30"
            + "FAFABE1C5D71A87E2F741EF8C1FE86FEA6BBFDE530677F0D"
            + "97D11D49F7A8443D0822E506A9F4614E011E2A94838FF88C"
            + "D68C8BB7C5C6424CFFFFFFFFFFFFFFFF";
        internal static readonly DHParameters draft_ffdhe8192 = FromSafeP(draft_ffdhe8192_p);

    
        public static void AddNegotiatedDheGroupsClientExtension(IDictionary extensions, byte[] dheGroups)
        {
            extensions[ExtensionType.negotiated_ff_dhe_groups] = CreateNegotiatedDheGroupsClientExtension(dheGroups);
        }

        public static void AddNegotiatedDheGroupsServerExtension(IDictionary extensions, byte dheGroup)
        {
            extensions[ExtensionType.negotiated_ff_dhe_groups] = CreateNegotiatedDheGroupsServerExtension(dheGroup);
        }

        public static byte[] GetNegotiatedDheGroupsClientExtension(IDictionary extensions)
        {
            byte[] extensionData = TlsUtilities.GetExtensionData(extensions, ExtensionType.negotiated_ff_dhe_groups);
            return extensionData == null ? null : ReadNegotiatedDheGroupsClientExtension(extensionData);
        }

        public static short GetNegotiatedDheGroupsServerExtension(IDictionary extensions)
        {
            byte[] extensionData = TlsUtilities.GetExtensionData(extensions, ExtensionType.negotiated_ff_dhe_groups);
            return extensionData == null ? (short)-1 : (short)ReadNegotiatedDheGroupsServerExtension(extensionData);
        }

        public static byte[] CreateNegotiatedDheGroupsClientExtension(byte[] dheGroups)
        {
            if (dheGroups == null || dheGroups.Length < 1 || dheGroups.Length > 255)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            return TlsUtilities.EncodeUint8ArrayWithUint8Length(dheGroups);
        }

        public static byte[] CreateNegotiatedDheGroupsServerExtension(byte dheGroup)
        {
            return TlsUtilities.EncodeUint8(dheGroup);
        }

        public static byte[] ReadNegotiatedDheGroupsClientExtension(byte[] extensionData)
        {
            byte[] dheGroups = TlsUtilities.DecodeUint8ArrayWithUint8Length(extensionData);
            if (dheGroups.Length < 1)
                throw new TlsFatalAlert(AlertDescription.decode_error);
            return dheGroups;
        }

        public static byte ReadNegotiatedDheGroupsServerExtension(byte[] extensionData)
        {
            return TlsUtilities.DecodeUint8(extensionData);
        }

        public static DHParameters GetParametersForDHEGroup(short dheGroup)
        {
            switch (dheGroup)
            {
            case FiniteFieldDheGroup.ffdhe2432:
                return draft_ffdhe2432;
            case FiniteFieldDheGroup.ffdhe3072:
                return draft_ffdhe3072;
            case FiniteFieldDheGroup.ffdhe4096:
                return draft_ffdhe4096;
            case FiniteFieldDheGroup.ffdhe6144:
                return draft_ffdhe6144;
            case FiniteFieldDheGroup.ffdhe8192:
                return draft_ffdhe8192;
            default:
                return null;
            }
        }

        public static bool ContainsDheCipherSuites(int[] cipherSuites)
        {
            for (int i = 0; i < cipherSuites.Length; ++i)
            {
                if (IsDheCipherSuite(cipherSuites[i]))
                    return true;
            }
            return false;
        }

        public static bool IsDheCipherSuite(int cipherSuite)
        {
            switch (cipherSuite)
            {
            /*
             * RFC 2246
             */
            case CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_DES_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_EXPORT_WITH_DES40_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_DES_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA:

            /*
             * RFC 3268
             */
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA:

            /*
             * RFC 5932
             */
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA256:

            /*
             * RFC 4162
             */
            case CipherSuite.TLS_DHE_DSS_WITH_SEED_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_SEED_CBC_SHA:

            /*
             * RFC 4279
             */
            case CipherSuite.TLS_DHE_PSK_WITH_RC4_128_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA:

            /*
             * RFC 4785
             */
            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA:

            /*
             * RFC 5246
             */
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256:

            /*
             * RFC 5288
             */
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_GCM_SHA384:

            /*
             * RFC 5487
             */
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA384:

            /*
             * RFC 6367
             */
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:

            /*
             * RFC 6655
             */
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM:
            case CipherSuite.TLS_PSK_DHE_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_PSK_DHE_WITH_AES_256_CCM_8:

            /*
             * draft-ietf-tls-chacha20-poly1305-04
             */
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256:

            /*
             * draft-zauner-tls-aes-ocb-04
             */
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_128_OCB:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_256_OCB:
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_128_OCB:
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_256_OCB:

                return true;

            default:
                return false;
            }
        }

        public static bool AreCompatibleParameters(DHParameters a, DHParameters b)
        {
            return a.P.Equals(b.P) && a.G.Equals(b.G)
                && (a.Q == null || b.Q == null || a.Q.Equals(b.Q));
        }

        public static byte[] CalculateDHBasicAgreement(DHPublicKeyParameters publicKey,
            DHPrivateKeyParameters privateKey)
        {
            DHBasicAgreement basicAgreement = new DHBasicAgreement();
            basicAgreement.Init(privateKey);
            BigInteger agreementValue = basicAgreement.CalculateAgreement(publicKey);

            /*
             * RFC 5246 8.1.2. Leading bytes of Z that contain all zero bits are stripped before it is
             * used as the pre_master_secret.
             */
            return BigIntegers.AsUnsignedByteArray(agreementValue);
        }

        public static AsymmetricCipherKeyPair GenerateDHKeyPair(SecureRandom random, DHParameters dhParams)
        {
            DHBasicKeyPairGenerator dhGen = new DHBasicKeyPairGenerator();
            dhGen.Init(new DHKeyGenerationParameters(random, dhParams));
            return dhGen.GenerateKeyPair();
        }

        public static DHPrivateKeyParameters GenerateEphemeralClientKeyExchange(SecureRandom random,
            DHParameters dhParams, Stream output)
        {
            AsymmetricCipherKeyPair kp = GenerateDHKeyPair(random, dhParams);

            DHPublicKeyParameters dhPublic = (DHPublicKeyParameters)kp.Public;
            WriteDHParameter(dhPublic.Y, output);

            return (DHPrivateKeyParameters)kp.Private;
        }

        public static DHPrivateKeyParameters GenerateEphemeralServerKeyExchange(SecureRandom random,
            DHParameters dhParams, Stream output)
        {
            AsymmetricCipherKeyPair kp = GenerateDHKeyPair(random, dhParams);

            DHPublicKeyParameters dhPublic = (DHPublicKeyParameters)kp.Public;
            new ServerDHParams(dhPublic).Encode(output);

            return (DHPrivateKeyParameters)kp.Private;
        }

        public static DHParameters ValidateDHParameters(DHParameters parameters)
        {
            BigInteger p = parameters.P;
            BigInteger g = parameters.G;

            if (!p.IsProbablePrime(2))
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            if (g.CompareTo(Two) < 0 || g.CompareTo(p.Subtract(Two)) > 0)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);


            return parameters;
        }

        public static DHPublicKeyParameters ValidateDHPublicKey(DHPublicKeyParameters key)
        {
            DHParameters parameters = ValidateDHParameters(key.Parameters);

            BigInteger Y = key.Y;
            if (Y.CompareTo(Two) < 0 || Y.CompareTo(parameters.P.Subtract(Two)) > 0)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            // TODO See RFC 2631 for more discussion of Diffie-Hellman validation

            return key;
        }

        public static BigInteger ReadDHParameter(Stream input)
        {
            return new BigInteger(1, TlsUtilities.ReadOpaque16(input));
        }

        public static void WriteDHParameter(BigInteger x, Stream output)
        {
            TlsUtilities.WriteOpaque16(BigIntegers.AsUnsignedByteArray(x), output);
        }
    }
}
