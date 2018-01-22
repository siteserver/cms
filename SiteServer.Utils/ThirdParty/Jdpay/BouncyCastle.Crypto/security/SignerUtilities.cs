using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Security
{
    /// <summary>
    ///  Signer Utility class contains methods that can not be specifically grouped into other classes.
    /// </summary>
    public sealed class SignerUtilities
    {
        private SignerUtilities()
        {
        }

        internal static readonly IDictionary algorithms = Platform.CreateHashtable();
        internal static readonly IDictionary oids = Platform.CreateHashtable();

        static SignerUtilities()
        {
            algorithms["MD2WITHRSA"] = "MD2withRSA";
            algorithms["MD2WITHRSAENCRYPTION"] = "MD2withRSA";
            algorithms[PkcsObjectIdentifiers.MD2WithRsaEncryption.Id] = "MD2withRSA";

            algorithms["MD4WITHRSA"] = "MD4withRSA";
            algorithms["MD4WITHRSAENCRYPTION"] = "MD4withRSA";
            algorithms[PkcsObjectIdentifiers.MD4WithRsaEncryption.Id] = "MD4withRSA";
            algorithms[OiwObjectIdentifiers.MD4WithRsa.Id] = "MD4withRSA";
			algorithms[OiwObjectIdentifiers.MD4WithRsaEncryption.Id] = "MD4withRSA";

			algorithms["MD5WITHRSA"] = "MD5withRSA";
            algorithms["MD5WITHRSAENCRYPTION"] = "MD5withRSA";
            algorithms[PkcsObjectIdentifiers.MD5WithRsaEncryption.Id] = "MD5withRSA";
            algorithms[OiwObjectIdentifiers.MD5WithRsa.Id] = "MD5withRSA";

            algorithms["SHA1WITHRSA"] = "SHA-1withRSA";
            algorithms["SHA1WITHRSAENCRYPTION"] = "SHA-1withRSA";
            algorithms["SHA-1WITHRSA"] = "SHA-1withRSA";
            algorithms[PkcsObjectIdentifiers.Sha1WithRsaEncryption.Id] = "SHA-1withRSA";
            algorithms[OiwObjectIdentifiers.Sha1WithRsa.Id] = "SHA-1withRSA";

            algorithms["SHA224WITHRSA"] = "SHA-224withRSA";
            algorithms["SHA224WITHRSAENCRYPTION"] = "SHA-224withRSA";
            algorithms[PkcsObjectIdentifiers.Sha224WithRsaEncryption.Id] = "SHA-224withRSA";
            algorithms["SHA-224WITHRSA"] = "SHA-224withRSA";

            algorithms["SHA256WITHRSA"] = "SHA-256withRSA";
            algorithms["SHA256WITHRSAENCRYPTION"] = "SHA-256withRSA";
            algorithms[PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id] = "SHA-256withRSA";
            algorithms["SHA-256WITHRSA"] = "SHA-256withRSA";

            algorithms["SHA384WITHRSA"] = "SHA-384withRSA";
            algorithms["SHA384WITHRSAENCRYPTION"] = "SHA-384withRSA";
            algorithms[PkcsObjectIdentifiers.Sha384WithRsaEncryption.Id] = "SHA-384withRSA";
            algorithms["SHA-384WITHRSA"] = "SHA-384withRSA";

            algorithms["SHA512WITHRSA"] = "SHA-512withRSA";
            algorithms["SHA512WITHRSAENCRYPTION"] = "SHA-512withRSA";
            algorithms[PkcsObjectIdentifiers.Sha512WithRsaEncryption.Id] = "SHA-512withRSA";
            algorithms["SHA-512WITHRSA"] = "SHA-512withRSA";

            algorithms["PSSWITHRSA"] = "PSSwithRSA";
            algorithms["RSASSA-PSS"] = "PSSwithRSA";
            algorithms[PkcsObjectIdentifiers.IdRsassaPss.Id] = "PSSwithRSA";
            algorithms["RSAPSS"] = "PSSwithRSA";

            algorithms["SHA1WITHRSAANDMGF1"] = "SHA-1withRSAandMGF1";
            algorithms["SHA-1WITHRSAANDMGF1"] = "SHA-1withRSAandMGF1";
            algorithms["SHA1WITHRSA/PSS"] = "SHA-1withRSAandMGF1";
            algorithms["SHA-1WITHRSA/PSS"] = "SHA-1withRSAandMGF1";

            algorithms["SHA224WITHRSAANDMGF1"] = "SHA-224withRSAandMGF1";
            algorithms["SHA-224WITHRSAANDMGF1"] = "SHA-224withRSAandMGF1";
            algorithms["SHA224WITHRSA/PSS"] = "SHA-224withRSAandMGF1";
            algorithms["SHA-224WITHRSA/PSS"] = "SHA-224withRSAandMGF1";

            algorithms["SHA256WITHRSAANDMGF1"] = "SHA-256withRSAandMGF1";
            algorithms["SHA-256WITHRSAANDMGF1"] = "SHA-256withRSAandMGF1";
            algorithms["SHA256WITHRSA/PSS"] = "SHA-256withRSAandMGF1";
            algorithms["SHA-256WITHRSA/PSS"] = "SHA-256withRSAandMGF1";

            algorithms["SHA384WITHRSAANDMGF1"] = "SHA-384withRSAandMGF1";
            algorithms["SHA-384WITHRSAANDMGF1"] = "SHA-384withRSAandMGF1";
            algorithms["SHA384WITHRSA/PSS"] = "SHA-384withRSAandMGF1";
            algorithms["SHA-384WITHRSA/PSS"] = "SHA-384withRSAandMGF1";

            algorithms["SHA512WITHRSAANDMGF1"] = "SHA-512withRSAandMGF1";
            algorithms["SHA-512WITHRSAANDMGF1"] = "SHA-512withRSAandMGF1";
            algorithms["SHA512WITHRSA/PSS"] = "SHA-512withRSAandMGF1";
            algorithms["SHA-512WITHRSA/PSS"] = "SHA-512withRSAandMGF1";

            algorithms["RIPEMD128WITHRSA"] = "RIPEMD128withRSA";
            algorithms["RIPEMD128WITHRSAENCRYPTION"] = "RIPEMD128withRSA";
            algorithms[TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128.Id] = "RIPEMD128withRSA";

            algorithms["RIPEMD160WITHRSA"] = "RIPEMD160withRSA";
            algorithms["RIPEMD160WITHRSAENCRYPTION"] = "RIPEMD160withRSA";
            algorithms[TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160.Id] = "RIPEMD160withRSA";

            algorithms["RIPEMD256WITHRSA"] = "RIPEMD256withRSA";
            algorithms["RIPEMD256WITHRSAENCRYPTION"] = "RIPEMD256withRSA";
            algorithms[TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256.Id] = "RIPEMD256withRSA";

            algorithms["NONEWITHRSA"] = "RSA";
            algorithms["RSAWITHNONE"] = "RSA";
            algorithms["RAWRSA"] = "RSA";

            algorithms["RAWRSAPSS"] = "RAWRSASSA-PSS";
            algorithms["NONEWITHRSAPSS"] = "RAWRSASSA-PSS";
            algorithms["NONEWITHRSASSA-PSS"] = "RAWRSASSA-PSS";

            algorithms["NONEWITHDSA"] = "NONEwithDSA";
            algorithms["DSAWITHNONE"] = "NONEwithDSA";
            algorithms["RAWDSA"] = "NONEwithDSA";

            algorithms["DSA"] = "SHA-1withDSA";
            algorithms["DSAWITHSHA1"] = "SHA-1withDSA";
            algorithms["DSAWITHSHA-1"] = "SHA-1withDSA";
            algorithms["SHA/DSA"] = "SHA-1withDSA";
            algorithms["SHA1/DSA"] = "SHA-1withDSA";
            algorithms["SHA-1/DSA"] = "SHA-1withDSA";
            algorithms["SHA1WITHDSA"] = "SHA-1withDSA";
            algorithms["SHA-1WITHDSA"] = "SHA-1withDSA";
            algorithms[X9ObjectIdentifiers.IdDsaWithSha1.Id] = "SHA-1withDSA";
            algorithms[OiwObjectIdentifiers.DsaWithSha1.Id] = "SHA-1withDSA";

            algorithms["DSAWITHSHA224"] = "SHA-224withDSA";
            algorithms["DSAWITHSHA-224"] = "SHA-224withDSA";
            algorithms["SHA224/DSA"] = "SHA-224withDSA";
            algorithms["SHA-224/DSA"] = "SHA-224withDSA";
            algorithms["SHA224WITHDSA"] = "SHA-224withDSA";
            algorithms["SHA-224WITHDSA"] = "SHA-224withDSA";
            algorithms[NistObjectIdentifiers.DsaWithSha224.Id] = "SHA-224withDSA";

            algorithms["DSAWITHSHA256"] = "SHA-256withDSA";
            algorithms["DSAWITHSHA-256"] = "SHA-256withDSA";
            algorithms["SHA256/DSA"] = "SHA-256withDSA";
            algorithms["SHA-256/DSA"] = "SHA-256withDSA";
            algorithms["SHA256WITHDSA"] = "SHA-256withDSA";
            algorithms["SHA-256WITHDSA"] = "SHA-256withDSA";
            algorithms[NistObjectIdentifiers.DsaWithSha256.Id] = "SHA-256withDSA";

            algorithms["DSAWITHSHA384"] = "SHA-384withDSA";
            algorithms["DSAWITHSHA-384"] = "SHA-384withDSA";
            algorithms["SHA384/DSA"] = "SHA-384withDSA";
            algorithms["SHA-384/DSA"] = "SHA-384withDSA";
            algorithms["SHA384WITHDSA"] = "SHA-384withDSA";
            algorithms["SHA-384WITHDSA"] = "SHA-384withDSA";
            algorithms[NistObjectIdentifiers.DsaWithSha384.Id] = "SHA-384withDSA";

            algorithms["DSAWITHSHA512"] = "SHA-512withDSA";
            algorithms["DSAWITHSHA-512"] = "SHA-512withDSA";
            algorithms["SHA512/DSA"] = "SHA-512withDSA";
            algorithms["SHA-512/DSA"] = "SHA-512withDSA";
            algorithms["SHA512WITHDSA"] = "SHA-512withDSA";
            algorithms["SHA-512WITHDSA"] = "SHA-512withDSA";
            algorithms[NistObjectIdentifiers.DsaWithSha512.Id] = "SHA-512withDSA";

            algorithms["NONEWITHECDSA"] = "NONEwithECDSA";
            algorithms["ECDSAWITHNONE"] = "NONEwithECDSA";

            algorithms["ECDSA"] = "SHA-1withECDSA";
            algorithms["SHA1/ECDSA"] = "SHA-1withECDSA";
            algorithms["SHA-1/ECDSA"] = "SHA-1withECDSA";
            algorithms["ECDSAWITHSHA1"] = "SHA-1withECDSA";
            algorithms["ECDSAWITHSHA-1"] = "SHA-1withECDSA";
            algorithms["SHA1WITHECDSA"] = "SHA-1withECDSA";
            algorithms["SHA-1WITHECDSA"] = "SHA-1withECDSA";
            algorithms[X9ObjectIdentifiers.ECDsaWithSha1.Id] = "SHA-1withECDSA";
            algorithms[TeleTrusTObjectIdentifiers.ECSignWithSha1.Id] = "SHA-1withECDSA";

            algorithms["SHA224/ECDSA"] = "SHA-224withECDSA";
            algorithms["SHA-224/ECDSA"] = "SHA-224withECDSA";
            algorithms["ECDSAWITHSHA224"] = "SHA-224withECDSA";
            algorithms["ECDSAWITHSHA-224"] = "SHA-224withECDSA";
            algorithms["SHA224WITHECDSA"] = "SHA-224withECDSA";
            algorithms["SHA-224WITHECDSA"] = "SHA-224withECDSA";
            algorithms[X9ObjectIdentifiers.ECDsaWithSha224.Id] = "SHA-224withECDSA";

            algorithms["SHA256/ECDSA"] = "SHA-256withECDSA";
            algorithms["SHA-256/ECDSA"] = "SHA-256withECDSA";
            algorithms["ECDSAWITHSHA256"] = "SHA-256withECDSA";
            algorithms["ECDSAWITHSHA-256"] = "SHA-256withECDSA";
            algorithms["SHA256WITHECDSA"] = "SHA-256withECDSA";
            algorithms["SHA-256WITHECDSA"] = "SHA-256withECDSA";
            algorithms[X9ObjectIdentifiers.ECDsaWithSha256.Id] = "SHA-256withECDSA";

            algorithms["SHA384/ECDSA"] = "SHA-384withECDSA";
            algorithms["SHA-384/ECDSA"] = "SHA-384withECDSA";
            algorithms["ECDSAWITHSHA384"] = "SHA-384withECDSA";
            algorithms["ECDSAWITHSHA-384"] = "SHA-384withECDSA";
            algorithms["SHA384WITHECDSA"] = "SHA-384withECDSA";
            algorithms["SHA-384WITHECDSA"] = "SHA-384withECDSA";
            algorithms[X9ObjectIdentifiers.ECDsaWithSha384.Id] = "SHA-384withECDSA";

            algorithms["SHA512/ECDSA"] = "SHA-512withECDSA";
            algorithms["SHA-512/ECDSA"] = "SHA-512withECDSA";
            algorithms["ECDSAWITHSHA512"] = "SHA-512withECDSA";
            algorithms["ECDSAWITHSHA-512"] = "SHA-512withECDSA";
            algorithms["SHA512WITHECDSA"] = "SHA-512withECDSA";
            algorithms["SHA-512WITHECDSA"] = "SHA-512withECDSA";
            algorithms[X9ObjectIdentifiers.ECDsaWithSha512.Id] = "SHA-512withECDSA";

            algorithms["RIPEMD160/ECDSA"] = "RIPEMD160withECDSA";
            algorithms["ECDSAWITHRIPEMD160"] = "RIPEMD160withECDSA";
            algorithms["RIPEMD160WITHECDSA"] = "RIPEMD160withECDSA";
            algorithms[TeleTrusTObjectIdentifiers.ECSignWithRipeMD160.Id] = "RIPEMD160withECDSA";

            algorithms["GOST-3410"] = "GOST3410";
            algorithms["GOST-3410-94"] = "GOST3410";
            algorithms["GOST3411WITHGOST3410"] = "GOST3410";
            algorithms[CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94.Id] = "GOST3410";

            algorithms["ECGOST-3410"] = "ECGOST3410";
            algorithms["ECGOST-3410-2001"] = "ECGOST3410";
            algorithms["GOST3411WITHECGOST3410"] = "ECGOST3410";
            algorithms[CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001.Id] = "ECGOST3410";



            oids["MD2withRSA"] = PkcsObjectIdentifiers.MD2WithRsaEncryption;
            oids["MD4withRSA"] = PkcsObjectIdentifiers.MD4WithRsaEncryption;
            oids["MD5withRSA"] = PkcsObjectIdentifiers.MD5WithRsaEncryption;

            oids["SHA-1withRSA"] = PkcsObjectIdentifiers.Sha1WithRsaEncryption;
            oids["SHA-224withRSA"] = PkcsObjectIdentifiers.Sha224WithRsaEncryption;
            oids["SHA-256withRSA"] = PkcsObjectIdentifiers.Sha256WithRsaEncryption;
            oids["SHA-384withRSA"] = PkcsObjectIdentifiers.Sha384WithRsaEncryption;
            oids["SHA-512withRSA"] = PkcsObjectIdentifiers.Sha512WithRsaEncryption;

            oids["PSSwithRSA"] = PkcsObjectIdentifiers.IdRsassaPss;
            oids["SHA-1withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
            oids["SHA-224withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
            oids["SHA-256withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
            oids["SHA-384withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
            oids["SHA-512withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;

            oids["RIPEMD128withRSA"] = TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128;
            oids["RIPEMD160withRSA"] = TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160;
            oids["RIPEMD256withRSA"] = TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256;

            oids["SHA-1withDSA"] = X9ObjectIdentifiers.IdDsaWithSha1;

            oids["SHA-1withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha1;
            oids["SHA-224withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha224;
            oids["SHA-256withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha256;
            oids["SHA-384withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha384;
            oids["SHA-512withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha512;

            oids["GOST3410"] = CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94;
            oids["ECGOST3410"] = CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001;
        }

        /// <summary>
        /// Returns an ObjectIdentifier for a given encoding.
        /// </summary>
        /// <param name="mechanism">A string representation of the encoding.</param>
        /// <returns>A DerObjectIdentifier, null if the OID is not available.</returns>
        // TODO Don't really want to support this
        public static DerObjectIdentifier GetObjectIdentifier(
            string mechanism)
        {
            if (mechanism == null)
                throw new ArgumentNullException("mechanism");

            mechanism = Platform.ToUpperInvariant(mechanism);
            string aliased = (string) algorithms[mechanism];

            if (aliased != null)
                mechanism = aliased;

            return (DerObjectIdentifier) oids[mechanism];
        }

        public static ICollection Algorithms
        {
            get { return oids.Keys; }
        }

        public static Asn1Encodable GetDefaultX509Parameters(
            DerObjectIdentifier id)
        {
            return GetDefaultX509Parameters(id.Id);
        }

        public static Asn1Encodable GetDefaultX509Parameters(
            string algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");

            algorithm = Platform.ToUpperInvariant(algorithm);

            string mechanism = (string) algorithms[algorithm];

            if (mechanism == null)
                mechanism = algorithm;

            if (mechanism == "PSSwithRSA")
            {
                // TODO The Sha1Digest here is a default. In JCE version, the actual digest
                // to be used can be overridden by subsequent parameter settings.
                return GetPssX509Parameters("SHA-1");
            }

            if (Platform.EndsWith(mechanism, "withRSAandMGF1"))
            {
                string digestName = mechanism.Substring(0, mechanism.Length - "withRSAandMGF1".Length);
                return GetPssX509Parameters(digestName);
            }

            return DerNull.Instance;
        }

        private static Asn1Encodable GetPssX509Parameters(
            string	digestName)
        {
            AlgorithmIdentifier hashAlgorithm = new AlgorithmIdentifier(
                DigestUtilities.GetObjectIdentifier(digestName), DerNull.Instance);

            // TODO Is it possible for the MGF hash alg to be different from the PSS one?
            AlgorithmIdentifier maskGenAlgorithm = new AlgorithmIdentifier(
                PkcsObjectIdentifiers.IdMgf1, hashAlgorithm);

            int saltLen = DigestUtilities.GetDigest(digestName).GetDigestSize();
            return new RsassaPssParameters(hashAlgorithm, maskGenAlgorithm,
                new DerInteger(saltLen), new DerInteger(1));
        }

        public static ISigner GetSigner(
            DerObjectIdentifier id)
        {
            return GetSigner(id.Id);
        }

        public static ISigner GetSigner(
            string algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");

            algorithm = Platform.ToUpperInvariant(algorithm);

            string mechanism = (string) algorithms[algorithm];

            if (mechanism == null)
                mechanism = algorithm;

            if (mechanism.Equals("RSA"))
            {
                return (new RsaDigestSigner(new NullDigest(), (AlgorithmIdentifier)null));
            }
            if (mechanism.Equals("MD2withRSA"))
            {
                return (new RsaDigestSigner(new MD2Digest()));
            }
            if (mechanism.Equals("MD4withRSA"))
            {
                return (new RsaDigestSigner(new MD4Digest()));
            }
            if (mechanism.Equals("MD5withRSA"))
            {
                return (new RsaDigestSigner(new MD5Digest()));
            }
            if (mechanism.Equals("SHA-1withRSA"))
            {
                return (new RsaDigestSigner(new Sha1Digest()));
            }
            if (mechanism.Equals("SHA-224withRSA"))
            {
                return (new RsaDigestSigner(new Sha224Digest()));
            }
            if (mechanism.Equals("SHA-256withRSA"))
            {
                return (new RsaDigestSigner(new Sha256Digest()));
            }
            if (mechanism.Equals("SHA-384withRSA"))
            {
                return (new RsaDigestSigner(new Sha384Digest()));
            }
            if (mechanism.Equals("SHA-512withRSA"))
            {
                return (new RsaDigestSigner(new Sha512Digest()));
            }
            if (mechanism.Equals("RIPEMD128withRSA"))
            {
                return (new RsaDigestSigner(new RipeMD128Digest()));
            }
            if (mechanism.Equals("RIPEMD160withRSA"))
            {
                return (new RsaDigestSigner(new RipeMD160Digest()));
            }
            if (mechanism.Equals("RIPEMD256withRSA"))
            {
                return (new RsaDigestSigner(new RipeMD256Digest()));
            }

            if (mechanism.Equals("RAWRSASSA-PSS"))
            {
                // TODO Add support for other parameter settings
                return PssSigner.CreateRawSigner(new RsaBlindedEngine(), new Sha1Digest());
            }
            if (mechanism.Equals("PSSwithRSA"))
            {
                // TODO The Sha1Digest here is a default. In JCE version, the actual digest
                // to be used can be overridden by subsequent parameter settings.
                return (new PssSigner(new RsaBlindedEngine(), new Sha1Digest()));
            }
            if (mechanism.Equals("SHA-1withRSAandMGF1"))
            {
                return (new PssSigner(new RsaBlindedEngine(), new Sha1Digest()));
            }
            if (mechanism.Equals("SHA-224withRSAandMGF1"))
            {
                return (new PssSigner(new RsaBlindedEngine(), new Sha224Digest()));
            }
            if (mechanism.Equals("SHA-256withRSAandMGF1"))
            {
                return (new PssSigner(new RsaBlindedEngine(), new Sha256Digest()));
            }
            if (mechanism.Equals("SHA-384withRSAandMGF1"))
            {
                return (new PssSigner(new RsaBlindedEngine(), new Sha384Digest()));
            }
            if (mechanism.Equals("SHA-512withRSAandMGF1"))
            {
                return (new PssSigner(new RsaBlindedEngine(), new Sha512Digest()));
            }

            if (mechanism.Equals("NONEwithDSA"))
            {
                return (new DsaDigestSigner(new DsaSigner(), new NullDigest()));
            }
            if (mechanism.Equals("SHA-1withDSA"))
            {
                return (new DsaDigestSigner(new DsaSigner(), new Sha1Digest()));
            }
            if (mechanism.Equals("SHA-224withDSA"))
            {
                return (new DsaDigestSigner(new DsaSigner(), new Sha224Digest()));
            }
            if (mechanism.Equals("SHA-256withDSA"))
            {
                return (new DsaDigestSigner(new DsaSigner(), new Sha256Digest()));
            }
            if (mechanism.Equals("SHA-384withDSA"))
            {
                return (new DsaDigestSigner(new DsaSigner(), new Sha384Digest()));
            }
            if (mechanism.Equals("SHA-512withDSA"))
            {
                return (new DsaDigestSigner(new DsaSigner(), new Sha512Digest()));
            }

            if (mechanism.Equals("NONEwithECDSA"))
            {
                return (new DsaDigestSigner(new ECDsaSigner(), new NullDigest()));
            }
            if (mechanism.Equals("SHA-1withECDSA"))
            {
                return (new DsaDigestSigner(new ECDsaSigner(), new Sha1Digest()));
            }
            if (mechanism.Equals("SHA-224withECDSA"))
            {
                return (new DsaDigestSigner(new ECDsaSigner(), new Sha224Digest()));
            }
            if (mechanism.Equals("SHA-256withECDSA"))
            {
                return (new DsaDigestSigner(new ECDsaSigner(), new Sha256Digest()));
            }
            if (mechanism.Equals("SHA-384withECDSA"))
            {
                return (new DsaDigestSigner(new ECDsaSigner(), new Sha384Digest()));
            }
            if (mechanism.Equals("SHA-512withECDSA"))
            {
                return (new DsaDigestSigner(new ECDsaSigner(), new Sha512Digest()));
            }

            if (mechanism.Equals("RIPEMD160withECDSA"))
            {
                return (new DsaDigestSigner(new ECDsaSigner(), new RipeMD160Digest()));
            }

            if (mechanism.Equals("SHA1WITHECNR"))
            {
                return (new DsaDigestSigner(new ECNRSigner(), new Sha1Digest()));
            }
            if (mechanism.Equals("SHA224WITHECNR"))
            {
                return (new DsaDigestSigner(new ECNRSigner(), new Sha224Digest()));
            }
            if (mechanism.Equals("SHA256WITHECNR"))
            {
                return (new DsaDigestSigner(new ECNRSigner(), new Sha256Digest()));
            }
            if (mechanism.Equals("SHA384WITHECNR"))
            {
                return (new DsaDigestSigner(new ECNRSigner(), new Sha384Digest()));
            }
            if (mechanism.Equals("SHA512WITHECNR"))
            {
                return (new DsaDigestSigner(new ECNRSigner(), new Sha512Digest()));
            }

            if (mechanism.Equals("GOST3410"))
            {
                return new Gost3410DigestSigner(new Gost3410Signer(), new Gost3411Digest());
            }
            if (mechanism.Equals("ECGOST3410"))
            {
                return new Gost3410DigestSigner(new ECGost3410Signer(), new Gost3411Digest());
            }

            if (mechanism.Equals("SHA1WITHRSA/ISO9796-2"))
            {
                return new Iso9796d2Signer(new RsaBlindedEngine(), new Sha1Digest(), true);
            }
            if (mechanism.Equals("MD5WITHRSA/ISO9796-2"))
            {
                return new Iso9796d2Signer(new RsaBlindedEngine(), new MD5Digest(), true);
            }
            if (mechanism.Equals("RIPEMD160WITHRSA/ISO9796-2"))
            {
                return new Iso9796d2Signer(new RsaBlindedEngine(), new RipeMD160Digest(), true);
            }

            if (Platform.EndsWith(mechanism, "/X9.31"))
            {
                string x931 = mechanism.Substring(0, mechanism.Length - "/X9.31".Length);
                int withPos = Platform.IndexOf(x931, "WITH");
                if (withPos > 0)
                {
                    int endPos = withPos + "WITH".Length;

                    string digestName = x931.Substring(0, withPos);
                    IDigest digest = DigestUtilities.GetDigest(digestName);

                    string cipherName = x931.Substring(endPos, x931.Length - endPos);
                    if (cipherName.Equals("RSA"))
                    {
                        IAsymmetricBlockCipher cipher = new RsaBlindedEngine();
                        return new X931Signer(cipher, digest);
                    }
                }
            }

            throw new SecurityUtilityException("Signer " + algorithm + " not recognised.");
        }

        public static string GetEncodingName(
            DerObjectIdentifier oid)
        {
            return (string) algorithms[oid.Id];
        }
    }
}
