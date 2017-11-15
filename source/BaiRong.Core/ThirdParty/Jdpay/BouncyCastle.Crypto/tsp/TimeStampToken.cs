using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Tsp
{
	public class TimeStampToken
	{
		private readonly CmsSignedData		tsToken;
		private readonly SignerInformation	tsaSignerInfo;
//		private readonly DateTime			genTime;
		private readonly TimeStampTokenInfo	tstInfo;
		private readonly CertID				certID;

		public TimeStampToken(
			Asn1.Cms.ContentInfo contentInfo)
			: this(new CmsSignedData(contentInfo))
		{
		}

		public TimeStampToken(
			CmsSignedData signedData)
		{
			this.tsToken = signedData;

			if (!this.tsToken.SignedContentType.Equals(PkcsObjectIdentifiers.IdCTTstInfo))
			{
				throw new TspValidationException("ContentInfo object not for a time stamp.");
			}

			ICollection signers = tsToken.GetSignerInfos().GetSigners();

			if (signers.Count != 1)
			{
				throw new ArgumentException("Time-stamp token signed by "
					+ signers.Count
					+ " signers, but it must contain just the TSA signature.");
			}


			IEnumerator signerEnum = signers.GetEnumerator();

			signerEnum.MoveNext();
			tsaSignerInfo = (SignerInformation) signerEnum.Current;

			try
			{
				CmsProcessable content = tsToken.SignedContent;
				MemoryStream bOut = new MemoryStream();

				content.Write(bOut);

				this.tstInfo = new TimeStampTokenInfo(
					TstInfo.GetInstance(
						Asn1Object.FromByteArray(bOut.ToArray())));

				Asn1.Cms.Attribute attr = tsaSignerInfo.SignedAttributes[
					PkcsObjectIdentifiers.IdAASigningCertificate];

//				if (attr == null)
//				{
//					throw new TspValidationException(
//						"no signing certificate attribute found, time stamp invalid.");
//				}
//
//				SigningCertificate signCert = SigningCertificate.GetInstance(
//					attr.AttrValues[0]);
//
//				this.certID = EssCertID.GetInstance(signCert.GetCerts()[0]);

				if (attr != null)
				{
					SigningCertificate signCert = SigningCertificate.GetInstance(attr.AttrValues[0]);

					this.certID = new CertID(EssCertID.GetInstance(signCert.GetCerts()[0]));
				}
				else
				{
					attr = tsaSignerInfo.SignedAttributes[PkcsObjectIdentifiers.IdAASigningCertificateV2];

					if (attr == null)
						throw new TspValidationException("no signing certificate attribute found, time stamp invalid.");

					SigningCertificateV2 signCertV2 = SigningCertificateV2.GetInstance(attr.AttrValues[0]);

					this.certID = new CertID(EssCertIDv2.GetInstance(signCertV2.GetCerts()[0]));
				}
			}
			catch (CmsException e)
			{
				throw new TspException(e.Message, e.InnerException);
			}
		}

		public TimeStampTokenInfo TimeStampInfo
		{
			get { return tstInfo; }
		}

		public SignerID SignerID
		{
			get { return tsaSignerInfo.SignerID; }
		}

		public Asn1.Cms.AttributeTable SignedAttributes
		{
			get { return tsaSignerInfo.SignedAttributes; }
		}

		public Asn1.Cms.AttributeTable UnsignedAttributes
		{
			get { return tsaSignerInfo.UnsignedAttributes; }
		}

		public IX509Store GetCertificates(
			string type)
		{
			return tsToken.GetCertificates(type);
		}

		public IX509Store GetCrls(
			string type)
		{
			return tsToken.GetCrls(type);
		}

	    public IX509Store GetAttributeCertificates(
			string type)
	    {
	        return tsToken.GetAttributeCertificates(type);
	    }

		/**
		 * Validate the time stamp token.
		 * <p>
		 * To be valid the token must be signed by the passed in certificate and
		 * the certificate must be the one referred to by the SigningCertificate
		 * attribute included in the hashed attributes of the token. The
		 * certificate must also have the ExtendedKeyUsageExtension with only
		 * KeyPurposeID.IdKPTimeStamping and have been valid at the time the
		 * timestamp was created.
		 * </p>
		 * <p>
		 * A successful call to validate means all the above are true.
		 * </p>
		 */
		public void Validate(
			X509Certificate cert)
		{
			try
			{
				byte[] hash = DigestUtilities.CalculateDigest(
					certID.GetHashAlgorithmName(), cert.GetEncoded());

				if (!Arrays.ConstantTimeAreEqual(certID.GetCertHash(), hash))
				{
					throw new TspValidationException("certificate hash does not match certID hash.");
				}

				if (certID.IssuerSerial != null)
				{
					if (!certID.IssuerSerial.Serial.Value.Equals(cert.SerialNumber))
					{
						throw new TspValidationException("certificate serial number does not match certID for signature.");
					}

					GeneralName[] names = certID.IssuerSerial.Issuer.GetNames();
					X509Name principal = PrincipalUtilities.GetIssuerX509Principal(cert);
					bool found = false;

					for (int i = 0; i != names.Length; i++)
					{
						if (names[i].TagNo == 4
							&& X509Name.GetInstance(names[i].Name).Equivalent(principal))
						{
							found = true;
							break;
						}
					}

					if (!found)
					{
						throw new TspValidationException("certificate name does not match certID for signature. ");
					}
				}

				TspUtil.ValidateCertificate(cert);

				cert.CheckValidity(tstInfo.GenTime);

				if (!tsaSignerInfo.Verify(cert))
				{
					throw new TspValidationException("signature not created by certificate.");
				}
			}
			catch (CmsException e)
			{
				if (e.InnerException != null)
				{
					throw new TspException(e.Message, e.InnerException);
				}

				throw new TspException("CMS exception: " + e, e);
			}
			catch (CertificateEncodingException e)
			{
				throw new TspException("problem processing certificate: " + e, e);
			}
			catch (SecurityUtilityException e)
			{
				throw new TspException("cannot find algorithm: " + e.Message, e);
			}
		}

		/**
		 * Return the underlying CmsSignedData object.
		 *
		 * @return the underlying CMS structure.
		 */
		public CmsSignedData ToCmsSignedData()
		{
			return tsToken;
		}

		/**
		 * Return a ASN.1 encoded byte stream representing the encoded object.
		 *
		 * @throws IOException if encoding fails.
		 */
		public byte[] GetEncoded()
		{
			return tsToken.GetEncoded();
		}


		// perhaps this should be done using an interface on the ASN.1 classes...
		private class CertID
		{
			private EssCertID certID;
			private EssCertIDv2 certIDv2;

			internal CertID(EssCertID certID)
			{
				this.certID = certID;
				this.certIDv2 = null;
			}

			internal CertID(EssCertIDv2 certID)
			{
				this.certIDv2 = certID;
				this.certID = null;
			}

			public string GetHashAlgorithmName()
			{
				if (certID != null)
					return "SHA-1";

                if (NistObjectIdentifiers.IdSha256.Equals(certIDv2.HashAlgorithm.Algorithm))
					return "SHA-256";

                return certIDv2.HashAlgorithm.Algorithm.Id;
			}

			public AlgorithmIdentifier GetHashAlgorithm()
			{
				return (certID != null)
					?	new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1)
					:	certIDv2.HashAlgorithm;
			}

			public byte[] GetCertHash()
			{
				return certID != null
					?	certID.GetCertHash()
					:	certIDv2.GetCertHash();
			}

			public IssuerSerial IssuerSerial
			{
				get
				{
					return certID != null
						?	certID.IssuerSerial
						:	certIDv2.IssuerSerial;
				}
			}
		}
	}
}
