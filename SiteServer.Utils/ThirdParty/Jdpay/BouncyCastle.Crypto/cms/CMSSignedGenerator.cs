using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Cms
{
    public class DefaultDigestAlgorithmIdentifierFinder
    {
        private static readonly IDictionary digestOids = Platform.CreateHashtable();
        private static readonly IDictionary digestNameToOids = Platform.CreateHashtable();

        static DefaultDigestAlgorithmIdentifierFinder()
        {
            //
            // digests
            //
            digestOids.Add(OiwObjectIdentifiers.MD4WithRsaEncryption, PkcsObjectIdentifiers.MD4);
            digestOids.Add(OiwObjectIdentifiers.MD4WithRsa, PkcsObjectIdentifiers.MD4);
            digestOids.Add(OiwObjectIdentifiers.MD5WithRsa, PkcsObjectIdentifiers.MD5);
            digestOids.Add(OiwObjectIdentifiers.Sha1WithRsa, OiwObjectIdentifiers.IdSha1);
            digestOids.Add(OiwObjectIdentifiers.DsaWithSha1, OiwObjectIdentifiers.IdSha1);

            digestOids.Add(PkcsObjectIdentifiers.Sha224WithRsaEncryption, NistObjectIdentifiers.IdSha224);
            digestOids.Add(PkcsObjectIdentifiers.Sha256WithRsaEncryption, NistObjectIdentifiers.IdSha256);
            digestOids.Add(PkcsObjectIdentifiers.Sha384WithRsaEncryption, NistObjectIdentifiers.IdSha384);
            digestOids.Add(PkcsObjectIdentifiers.Sha512WithRsaEncryption, NistObjectIdentifiers.IdSha512);
            digestOids.Add(PkcsObjectIdentifiers.MD2WithRsaEncryption, PkcsObjectIdentifiers.MD2);
            digestOids.Add(PkcsObjectIdentifiers.MD4WithRsaEncryption, PkcsObjectIdentifiers.MD4);
            digestOids.Add(PkcsObjectIdentifiers.MD5WithRsaEncryption, PkcsObjectIdentifiers.MD5);
            digestOids.Add(PkcsObjectIdentifiers.Sha1WithRsaEncryption, OiwObjectIdentifiers.IdSha1);

            digestOids.Add(X9ObjectIdentifiers.ECDsaWithSha1, OiwObjectIdentifiers.IdSha1);
            digestOids.Add(X9ObjectIdentifiers.ECDsaWithSha224, NistObjectIdentifiers.IdSha224);
            digestOids.Add(X9ObjectIdentifiers.ECDsaWithSha256, NistObjectIdentifiers.IdSha256);
            digestOids.Add(X9ObjectIdentifiers.ECDsaWithSha384, NistObjectIdentifiers.IdSha384);
            digestOids.Add(X9ObjectIdentifiers.ECDsaWithSha512, NistObjectIdentifiers.IdSha512);
            digestOids.Add(X9ObjectIdentifiers.IdDsaWithSha1, OiwObjectIdentifiers.IdSha1);

            digestOids.Add(NistObjectIdentifiers.DsaWithSha224, NistObjectIdentifiers.IdSha224);
            digestOids.Add(NistObjectIdentifiers.DsaWithSha256, NistObjectIdentifiers.IdSha256);
            digestOids.Add(NistObjectIdentifiers.DsaWithSha384, NistObjectIdentifiers.IdSha384);
            digestOids.Add(NistObjectIdentifiers.DsaWithSha512, NistObjectIdentifiers.IdSha512);

            digestOids.Add(TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128, TeleTrusTObjectIdentifiers.RipeMD128);
            digestOids.Add(TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160, TeleTrusTObjectIdentifiers.RipeMD160);
            digestOids.Add(TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256, TeleTrusTObjectIdentifiers.RipeMD256);

            digestOids.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94, CryptoProObjectIdentifiers.GostR3411);
            digestOids.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001, CryptoProObjectIdentifiers.GostR3411);

            digestNameToOids.Add("SHA-1", OiwObjectIdentifiers.IdSha1);
            digestNameToOids.Add("SHA-224", NistObjectIdentifiers.IdSha224);
            digestNameToOids.Add("SHA-256", NistObjectIdentifiers.IdSha256);
            digestNameToOids.Add("SHA-384", NistObjectIdentifiers.IdSha384);
            digestNameToOids.Add("SHA-512", NistObjectIdentifiers.IdSha512);

            digestNameToOids.Add("SHA1", OiwObjectIdentifiers.IdSha1);
            digestNameToOids.Add("SHA224", NistObjectIdentifiers.IdSha224);
            digestNameToOids.Add("SHA256", NistObjectIdentifiers.IdSha256);
            digestNameToOids.Add("SHA384", NistObjectIdentifiers.IdSha384);
            digestNameToOids.Add("SHA512", NistObjectIdentifiers.IdSha512);

            digestNameToOids.Add("SHA3-224", NistObjectIdentifiers.IdSha3_224);
            digestNameToOids.Add("SHA3-256", NistObjectIdentifiers.IdSha3_256);
            digestNameToOids.Add("SHA3-384", NistObjectIdentifiers.IdSha3_384);
            digestNameToOids.Add("SHA3-512", NistObjectIdentifiers.IdSha3_512);

            digestNameToOids.Add("SHAKE-128", NistObjectIdentifiers.IdShake128);
            digestNameToOids.Add("SHAKE-256", NistObjectIdentifiers.IdShake256);

            digestNameToOids.Add("GOST3411", CryptoProObjectIdentifiers.GostR3411);

            digestNameToOids.Add("MD2", PkcsObjectIdentifiers.MD2);
            digestNameToOids.Add("MD4", PkcsObjectIdentifiers.MD4);
            digestNameToOids.Add("MD5", PkcsObjectIdentifiers.MD5);

            digestNameToOids.Add("RIPEMD128", TeleTrusTObjectIdentifiers.RipeMD128);
            digestNameToOids.Add("RIPEMD160", TeleTrusTObjectIdentifiers.RipeMD160);
            digestNameToOids.Add("RIPEMD256", TeleTrusTObjectIdentifiers.RipeMD256);
        }

        public AlgorithmIdentifier find(AlgorithmIdentifier sigAlgId)
        {
            AlgorithmIdentifier digAlgId;

            if (sigAlgId.Algorithm.Equals(PkcsObjectIdentifiers.IdRsassaPss))
            {
                digAlgId = RsassaPssParameters.GetInstance(sigAlgId.Parameters).HashAlgorithm;
            }
            else
            {
                digAlgId = new AlgorithmIdentifier((DerObjectIdentifier)digestOids[sigAlgId.Algorithm], DerNull.Instance);
            }

            return digAlgId;
        }

        public AlgorithmIdentifier find(String digAlgName)
        {
            return new AlgorithmIdentifier((DerObjectIdentifier)digestNameToOids[digAlgName], DerNull.Instance);
        }
    }

    public class CmsSignedGenerator
    {
        /**
        * Default type for the signed data.
        */
        public static readonly string Data = CmsObjectIdentifiers.Data.Id;

        public static readonly string DigestSha1 = OiwObjectIdentifiers.IdSha1.Id;
        public static readonly string DigestSha224 = NistObjectIdentifiers.IdSha224.Id;
        public static readonly string DigestSha256 = NistObjectIdentifiers.IdSha256.Id;
        public static readonly string DigestSha384 = NistObjectIdentifiers.IdSha384.Id;
        public static readonly string DigestSha512 = NistObjectIdentifiers.IdSha512.Id;
        public static readonly string DigestMD5 = PkcsObjectIdentifiers.MD5.Id;
        public static readonly string DigestGost3411 = CryptoProObjectIdentifiers.GostR3411.Id;
        public static readonly string DigestRipeMD128 = TeleTrusTObjectIdentifiers.RipeMD128.Id;
        public static readonly string DigestRipeMD160 = TeleTrusTObjectIdentifiers.RipeMD160.Id;
        public static readonly string DigestRipeMD256 = TeleTrusTObjectIdentifiers.RipeMD256.Id;

        public static readonly string EncryptionRsa = PkcsObjectIdentifiers.RsaEncryption.Id;
        public static readonly string EncryptionDsa = X9ObjectIdentifiers.IdDsaWithSha1.Id;
        public static readonly string EncryptionECDsa = X9ObjectIdentifiers.ECDsaWithSha1.Id;
        public static readonly string EncryptionRsaPss = PkcsObjectIdentifiers.IdRsassaPss.Id;
        public static readonly string EncryptionGost3410 = CryptoProObjectIdentifiers.GostR3410x94.Id;
        public static readonly string EncryptionECGost3410 = CryptoProObjectIdentifiers.GostR3410x2001.Id;

        internal IList _certs = Platform.CreateArrayList();
        internal IList _crls = Platform.CreateArrayList();
        internal IList _signers = Platform.CreateArrayList();
        internal IDictionary _digests = Platform.CreateHashtable();

        protected readonly SecureRandom rand;

        protected CmsSignedGenerator()
            : this(new SecureRandom())
        {
        }

        /// <summary>Constructor allowing specific source of randomness</summary>
        /// <param name="rand">Instance of <c>SecureRandom</c> to use.</param>
        protected CmsSignedGenerator(
            SecureRandom rand)
        {
            this.rand = rand;
        }

        internal protected virtual IDictionary GetBaseParameters(
            DerObjectIdentifier contentType,
            AlgorithmIdentifier digAlgId,
            byte[] hash)
        {
            IDictionary param = Platform.CreateHashtable();

            if (contentType != null)
            {
                param[CmsAttributeTableParameter.ContentType] = contentType;
            }

            param[CmsAttributeTableParameter.DigestAlgorithmIdentifier] = digAlgId;
            param[CmsAttributeTableParameter.Digest] = hash.Clone();

            return param;
        }

        internal protected virtual Asn1Set GetAttributeSet(
            Asn1.Cms.AttributeTable attr)
        {
            return attr == null
                ? null
                : new DerSet(attr.ToAsn1EncodableVector());
        }

        public void AddCertificates(
            IX509Store certStore)
        {
            CollectionUtilities.AddRange(_certs, CmsUtilities.GetCertificatesFromStore(certStore));
        }

        public void AddCrls(
            IX509Store crlStore)
        {
            CollectionUtilities.AddRange(_crls, CmsUtilities.GetCrlsFromStore(crlStore));
        }

        /**
		* Add the attribute certificates contained in the passed in store to the
		* generator.
		*
		* @param store a store of Version 2 attribute certificates
		* @throws CmsException if an error occurse processing the store.
		*/
        public void AddAttributeCertificates(
            IX509Store store)
        {
            try
            {
                foreach (IX509AttributeCertificate attrCert in store.GetMatches(null))
                {
                    _certs.Add(new DerTaggedObject(false, 2,
                        AttributeCertificate.GetInstance(Asn1Object.FromByteArray(attrCert.GetEncoded()))));
                }
            }
            catch (Exception e)
            {
                throw new CmsException("error processing attribute certs", e);
            }
        }

        /**
		 * Add a store of precalculated signers to the generator.
		 *
		 * @param signerStore store of signers
		 */
        public void AddSigners(
            SignerInformationStore signerStore)
        {
            foreach (SignerInformation o in signerStore.GetSigners())
            {
                _signers.Add(o);
                AddSignerCallback(o);
            }
        }

        /**
		 * Return a map of oids and byte arrays representing the digests calculated on the content during
		 * the last generate.
		 *
		 * @return a map of oids (as String objects) and byte[] representing digests.
		 */
        public IDictionary GetGeneratedDigests()
        {
            return Platform.CreateHashtable(_digests);
        }

        internal virtual void AddSignerCallback(
            SignerInformation si)
        {
        }

        internal static SignerIdentifier GetSignerIdentifier(X509Certificate cert)
        {
            return new SignerIdentifier(CmsUtilities.GetIssuerAndSerialNumber(cert));
        }

        internal static SignerIdentifier GetSignerIdentifier(byte[] subjectKeyIdentifier)
        {
            return new SignerIdentifier(new DerOctetString(subjectKeyIdentifier));
        }
    }
}
