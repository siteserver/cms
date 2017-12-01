using System;
using System.Collections;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Eac;
using Org.BouncyCastle.Asn1.Iana;
using Org.BouncyCastle.Asn1.Misc;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Cms
{
    internal class CmsSignedHelper
    {
        internal static readonly CmsSignedHelper Instance = new CmsSignedHelper();

        private static readonly string EncryptionECDsaWithSha1 = X9ObjectIdentifiers.ECDsaWithSha1.Id;
        private static readonly string EncryptionECDsaWithSha224 = X9ObjectIdentifiers.ECDsaWithSha224.Id;
        private static readonly string EncryptionECDsaWithSha256 = X9ObjectIdentifiers.ECDsaWithSha256.Id;
        private static readonly string EncryptionECDsaWithSha384 = X9ObjectIdentifiers.ECDsaWithSha384.Id;
        private static readonly string EncryptionECDsaWithSha512 = X9ObjectIdentifiers.ECDsaWithSha512.Id;

        private static readonly IDictionary encryptionAlgs = Platform.CreateHashtable();
        private static readonly IDictionary digestAlgs = Platform.CreateHashtable();
        private static readonly IDictionary digestAliases = Platform.CreateHashtable();

        private static readonly ISet noParams = new HashSet();
        private static readonly IDictionary ecAlgorithms = Platform.CreateHashtable();

        private static void AddEntries(DerObjectIdentifier oid, string digest, string encryption)
		{
			string alias = oid.Id;
			digestAlgs.Add(alias, digest);
			encryptionAlgs.Add(alias, encryption);
		}

		static CmsSignedHelper()
		{
			AddEntries(NistObjectIdentifiers.DsaWithSha224, "SHA224", "DSA");
			AddEntries(NistObjectIdentifiers.DsaWithSha256, "SHA256", "DSA");
			AddEntries(NistObjectIdentifiers.DsaWithSha384, "SHA384", "DSA");
			AddEntries(NistObjectIdentifiers.DsaWithSha512, "SHA512", "DSA");
			AddEntries(OiwObjectIdentifiers.DsaWithSha1, "SHA1", "DSA");
			AddEntries(OiwObjectIdentifiers.MD4WithRsa, "MD4", "RSA");
			AddEntries(OiwObjectIdentifiers.MD4WithRsaEncryption, "MD4", "RSA");
			AddEntries(OiwObjectIdentifiers.MD5WithRsa, "MD5", "RSA");
			AddEntries(OiwObjectIdentifiers.Sha1WithRsa, "SHA1", "RSA");
			AddEntries(PkcsObjectIdentifiers.MD2WithRsaEncryption, "MD2", "RSA");
			AddEntries(PkcsObjectIdentifiers.MD4WithRsaEncryption, "MD4", "RSA");
			AddEntries(PkcsObjectIdentifiers.MD5WithRsaEncryption, "MD5", "RSA");
			AddEntries(PkcsObjectIdentifiers.Sha1WithRsaEncryption, "SHA1", "RSA");
			AddEntries(PkcsObjectIdentifiers.Sha224WithRsaEncryption, "SHA224", "RSA");
			AddEntries(PkcsObjectIdentifiers.Sha256WithRsaEncryption, "SHA256", "RSA");
			AddEntries(PkcsObjectIdentifiers.Sha384WithRsaEncryption, "SHA384", "RSA");
			AddEntries(PkcsObjectIdentifiers.Sha512WithRsaEncryption, "SHA512", "RSA");
			AddEntries(X9ObjectIdentifiers.ECDsaWithSha1, "SHA1", "ECDSA");
			AddEntries(X9ObjectIdentifiers.ECDsaWithSha224, "SHA224", "ECDSA");
			AddEntries(X9ObjectIdentifiers.ECDsaWithSha256, "SHA256", "ECDSA");
			AddEntries(X9ObjectIdentifiers.ECDsaWithSha384, "SHA384", "ECDSA");
			AddEntries(X9ObjectIdentifiers.ECDsaWithSha512, "SHA512", "ECDSA");
			AddEntries(X9ObjectIdentifiers.IdDsaWithSha1, "SHA1", "DSA");
			AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_1, "SHA1", "ECDSA");
			AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_224, "SHA224", "ECDSA");
			AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_256, "SHA256", "ECDSA");
			AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_384, "SHA384", "ECDSA");
			AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_512, "SHA512", "ECDSA");
			AddEntries(EacObjectIdentifiers.id_TA_RSA_v1_5_SHA_1, "SHA1", "RSA");
			AddEntries(EacObjectIdentifiers.id_TA_RSA_v1_5_SHA_256, "SHA256", "RSA");
			AddEntries(EacObjectIdentifiers.id_TA_RSA_PSS_SHA_1, "SHA1", "RSAandMGF1");
			AddEntries(EacObjectIdentifiers.id_TA_RSA_PSS_SHA_256, "SHA256", "RSAandMGF1");

			encryptionAlgs.Add(X9ObjectIdentifiers.IdDsa.Id, "DSA");
			encryptionAlgs.Add(PkcsObjectIdentifiers.RsaEncryption.Id, "RSA");
			encryptionAlgs.Add(TeleTrusTObjectIdentifiers.TeleTrusTRsaSignatureAlgorithm, "RSA");
			encryptionAlgs.Add(X509ObjectIdentifiers.IdEARsa.Id, "RSA");
			encryptionAlgs.Add(CmsSignedGenerator.EncryptionRsaPss, "RSAandMGF1");
			encryptionAlgs.Add(CryptoProObjectIdentifiers.GostR3410x94.Id, "GOST3410");
			encryptionAlgs.Add(CryptoProObjectIdentifiers.GostR3410x2001.Id, "ECGOST3410");
			encryptionAlgs.Add("1.3.6.1.4.1.5849.1.6.2", "ECGOST3410");
			encryptionAlgs.Add("1.3.6.1.4.1.5849.1.1.5", "GOST3410");

			digestAlgs.Add(PkcsObjectIdentifiers.MD2.Id, "MD2");
			digestAlgs.Add(PkcsObjectIdentifiers.MD4.Id, "MD4");
			digestAlgs.Add(PkcsObjectIdentifiers.MD5.Id, "MD5");
			digestAlgs.Add(OiwObjectIdentifiers.IdSha1.Id, "SHA1");
			digestAlgs.Add(NistObjectIdentifiers.IdSha224.Id, "SHA224");
			digestAlgs.Add(NistObjectIdentifiers.IdSha256.Id, "SHA256");
			digestAlgs.Add(NistObjectIdentifiers.IdSha384.Id, "SHA384");
			digestAlgs.Add(NistObjectIdentifiers.IdSha512.Id, "SHA512");
			digestAlgs.Add(TeleTrusTObjectIdentifiers.RipeMD128.Id, "RIPEMD128");
			digestAlgs.Add(TeleTrusTObjectIdentifiers.RipeMD160.Id, "RIPEMD160");
			digestAlgs.Add(TeleTrusTObjectIdentifiers.RipeMD256.Id, "RIPEMD256");
			digestAlgs.Add(CryptoProObjectIdentifiers.GostR3411.Id,  "GOST3411");
			digestAlgs.Add("1.3.6.1.4.1.5849.1.2.1",  "GOST3411");

			digestAliases.Add("SHA1", new string[] { "SHA-1" });
			digestAliases.Add("SHA224", new string[] { "SHA-224" });
			digestAliases.Add("SHA256", new string[] { "SHA-256" });
			digestAliases.Add("SHA384", new string[] { "SHA-384" });
			digestAliases.Add("SHA512", new string[] { "SHA-512" });

            noParams.Add(CmsSignedGenerator.EncryptionDsa);
            //			noParams.Add(EncryptionECDsa);
            noParams.Add(EncryptionECDsaWithSha1);
            noParams.Add(EncryptionECDsaWithSha224);
            noParams.Add(EncryptionECDsaWithSha256);
            noParams.Add(EncryptionECDsaWithSha384);
            noParams.Add(EncryptionECDsaWithSha512);

            ecAlgorithms.Add(CmsSignedGenerator.DigestSha1, EncryptionECDsaWithSha1);
            ecAlgorithms.Add(CmsSignedGenerator.DigestSha224, EncryptionECDsaWithSha224);
            ecAlgorithms.Add(CmsSignedGenerator.DigestSha256, EncryptionECDsaWithSha256);
            ecAlgorithms.Add(CmsSignedGenerator.DigestSha384, EncryptionECDsaWithSha384);
            ecAlgorithms.Add(CmsSignedGenerator.DigestSha512, EncryptionECDsaWithSha512);
    }

		/**
        * Return the digest algorithm using one of the standard JCA string
        * representations rather than the algorithm identifier (if possible).
        */
        internal string GetDigestAlgName(
            string digestAlgOid)
        {
			string algName = (string)digestAlgs[digestAlgOid];

			if (algName != null)
			{
				return algName;
			}

			return digestAlgOid;
        }

    internal AlgorithmIdentifier GetEncAlgorithmIdentifier(
    DerObjectIdentifier encOid,
    Asn1Encodable sigX509Parameters)
    {
        if (noParams.Contains(encOid.Id))
        {
            return new AlgorithmIdentifier(encOid);
        }

        return new AlgorithmIdentifier(encOid, sigX509Parameters);
    }

    internal string[] GetDigestAliases(
			string algName)
		{
			string[] aliases = (string[]) digestAliases[algName];

			return aliases == null ? new String[0] : (string[]) aliases.Clone();
		}

		/**
        * Return the digest encryption algorithm using one of the standard
        * JCA string representations rather than the algorithm identifier (if
        * possible).
        */
        internal string GetEncryptionAlgName(
            string encryptionAlgOid)
        {
			string algName = (string) encryptionAlgs[encryptionAlgOid];

			if (algName != null)
			{
				return algName;
			}

			return encryptionAlgOid;
        }

		internal IDigest GetDigestInstance(
			string algorithm)
		{
			try
			{
				return DigestUtilities.GetDigest(algorithm);
			}
			catch (SecurityUtilityException e)
			{
				// This is probably superfluous on C#, since no provider infrastructure,
				// assuming DigestUtilities already knows all the aliases
				foreach (string alias in GetDigestAliases(algorithm))
				{
					try { return DigestUtilities.GetDigest(alias); }
					catch (SecurityUtilityException) {}
				}
				throw e;
			}
		}

		internal ISigner GetSignatureInstance(
			string algorithm)
		{
			return SignerUtilities.GetSigner(algorithm);
		}

		internal IX509Store CreateAttributeStore(
			string	type,
			Asn1Set	certSet)
		{
			IList certs = Platform.CreateArrayList();

			if (certSet != null)
			{
				foreach (Asn1Encodable ae in certSet)
				{
					try
					{
						Asn1Object obj = ae.ToAsn1Object();

						if (obj is Asn1TaggedObject)
						{
							Asn1TaggedObject tagged = (Asn1TaggedObject)obj;

							if (tagged.TagNo == 2)
							{
								certs.Add(
									new X509V2AttributeCertificate(
										Asn1Sequence.GetInstance(tagged, false).GetEncoded()));
							}
						}
					}
					catch (Exception ex)
					{
						throw new CmsException("can't re-encode attribute certificate!", ex);
					}
				}
			}

			try
			{
				return X509StoreFactory.Create(
					"AttributeCertificate/" + type,
					new X509CollectionStoreParameters(certs));
			}
			catch (ArgumentException e)
			{
				throw new CmsException("can't setup the X509Store", e);
			}
		}

		internal IX509Store CreateCertificateStore(
			string	type,
			Asn1Set	certSet)
		{
			IList certs = Platform.CreateArrayList();

			if (certSet != null)
			{
				AddCertsFromSet(certs, certSet);
			}

			try
			{
				return X509StoreFactory.Create(
					"Certificate/" + type,
					new X509CollectionStoreParameters(certs));
			}
			catch (ArgumentException e)
			{
				throw new CmsException("can't setup the X509Store", e);
			}
		}

		internal IX509Store CreateCrlStore(
			string	type,
			Asn1Set	crlSet)
		{
			IList crls = Platform.CreateArrayList();

			if (crlSet != null)
			{
				AddCrlsFromSet(crls, crlSet);
			}

			try
			{
				return X509StoreFactory.Create(
					"CRL/" + type,
					new X509CollectionStoreParameters(crls));
			}
			catch (ArgumentException e)
			{
				throw new CmsException("can't setup the X509Store", e);
			}
		}

		private void AddCertsFromSet(
			IList	certs,
			Asn1Set	certSet)
		{
			X509CertificateParser cf = new X509CertificateParser();

			foreach (Asn1Encodable ae in certSet)
			{
				try
				{
					Asn1Object obj = ae.ToAsn1Object();

					if (obj is Asn1Sequence)
					{
						// TODO Build certificate directly from sequence?
						certs.Add(cf.ReadCertificate(obj.GetEncoded()));
					}
				}
				catch (Exception ex)
				{
					throw new CmsException("can't re-encode certificate!", ex);
				}
			}
		}

		private void AddCrlsFromSet(
			IList	crls,
			Asn1Set	crlSet)
		{
			X509CrlParser cf = new X509CrlParser();

			foreach (Asn1Encodable ae in crlSet)
			{
				try
				{
					// TODO Build CRL directly from ae.ToAsn1Object()?
					crls.Add(cf.ReadCrl(ae.GetEncoded()));
				}
				catch (Exception ex)
				{
					throw new CmsException("can't re-encode CRL!", ex);
				}
			}
		}

		internal AlgorithmIdentifier FixAlgID(
			AlgorithmIdentifier algId)
		{
			if (algId.Parameters == null)
                return new AlgorithmIdentifier(algId.Algorithm, DerNull.Instance);

			return algId;
		}

        internal string GetEncOid(
            AsymmetricKeyParameter key,
            string digestOID)
        {
            string encOID = null;

            if (key is RsaKeyParameters)
            {
                if (!((RsaKeyParameters)key).IsPrivate)
                    throw new ArgumentException("Expected RSA private key");

                encOID = CmsSignedGenerator.EncryptionRsa;
            }
            else if (key is DsaPrivateKeyParameters)
            {
                if (digestOID.Equals(CmsSignedGenerator.DigestSha1))
                {
                    encOID = CmsSignedGenerator.EncryptionDsa;
                }
                else if (digestOID.Equals(CmsSignedGenerator.DigestSha224))
                {
                    encOID = NistObjectIdentifiers.DsaWithSha224.Id;
                }
                else if (digestOID.Equals(CmsSignedGenerator.DigestSha256))
                {
                    encOID = NistObjectIdentifiers.DsaWithSha256.Id;
                }
                else if (digestOID.Equals(CmsSignedGenerator.DigestSha384))
                {
                    encOID = NistObjectIdentifiers.DsaWithSha384.Id;
                }
                else if (digestOID.Equals(CmsSignedGenerator.DigestSha512))
                {
                    encOID = NistObjectIdentifiers.DsaWithSha512.Id;
                }
                else
                {
                    throw new ArgumentException("can't mix DSA with anything but SHA1/SHA2");
                }
            }
            else if (key is ECPrivateKeyParameters)
            {
                ECPrivateKeyParameters ecPrivKey = (ECPrivateKeyParameters)key;
                string algName = ecPrivKey.AlgorithmName;

                if (algName == "ECGOST3410")
                {
                    encOID = CmsSignedGenerator.EncryptionECGost3410;
                }
                else
                {
                    // TODO Should we insist on algName being one of "EC" or "ECDSA", as Java does?
                    encOID = (string)ecAlgorithms[digestOID];

                    if (encOID == null)
                        throw new ArgumentException("can't mix ECDSA with anything but SHA family digests");
                }
            }
            else if (key is Gost3410PrivateKeyParameters)
            {
                encOID = CmsSignedGenerator.EncryptionGost3410;
            }
            else
            {
                throw new ArgumentException("Unknown algorithm in CmsSignedGenerator.GetEncOid");
            }

            return encOID;
        }
    }
}
