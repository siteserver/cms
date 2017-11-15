using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.X509
{
	/**
	* class to produce an X.509 Version 2 CRL.
	*/
	public class X509V2CrlGenerator
	{
		private readonly X509ExtensionsGenerator extGenerator = new X509ExtensionsGenerator();

		private V2TbsCertListGenerator	tbsGen;
		private DerObjectIdentifier		sigOID;
		private AlgorithmIdentifier		sigAlgId;
		private string					signatureAlgorithm;

		public X509V2CrlGenerator()
		{
			tbsGen = new V2TbsCertListGenerator();
		}

		/**
		* reset the generator
		*/
		public void Reset()
		{
			tbsGen = new V2TbsCertListGenerator();
			extGenerator.Reset();
		}

		/**
		* Set the issuer distinguished name - the issuer is the entity whose private key is used to sign the
		* certificate.
		*/
		public void SetIssuerDN(
			X509Name issuer)
		{
			tbsGen.SetIssuer(issuer);
		}

		public void SetThisUpdate(
			DateTime date)
		{
			tbsGen.SetThisUpdate(new Time(date));
		}

		public void SetNextUpdate(
			DateTime date)
		{
			tbsGen.SetNextUpdate(new Time(date));
		}

		/**
		* Reason being as indicated by CrlReason, i.e. CrlReason.KeyCompromise
		* or 0 if CrlReason is not to be used
		**/
		public void AddCrlEntry(
			BigInteger	userCertificate,
			DateTime	revocationDate,
			int			reason)
		{
			tbsGen.AddCrlEntry(new DerInteger(userCertificate), new Time(revocationDate), reason);
		}

		/**
		* Add a CRL entry with an Invalidity Date extension as well as a CrlReason extension.
		* Reason being as indicated by CrlReason, i.e. CrlReason.KeyCompromise
		* or 0 if CrlReason is not to be used
		**/
		public void AddCrlEntry(
			BigInteger	userCertificate,
			DateTime	revocationDate,
			int			reason,
			DateTime	invalidityDate)
		{
			tbsGen.AddCrlEntry(new DerInteger(userCertificate), new Time(revocationDate), reason, new DerGeneralizedTime(invalidityDate));
		}

		/**
		* Add a CRL entry with extensions.
		**/
		public void AddCrlEntry(
			BigInteger		userCertificate,
			DateTime		revocationDate,
			X509Extensions	extensions)
		{
			tbsGen.AddCrlEntry(new DerInteger(userCertificate), new Time(revocationDate), extensions);
		}

		/**
		* Add the CRLEntry objects contained in a previous CRL.
		*
		* @param other the X509Crl to source the other entries from.
		*/
		public void AddCrl(
			X509Crl other)
		{
			if (other == null)
				throw new ArgumentNullException("other");

			ISet revocations = other.GetRevokedCertificates();

			if (revocations != null)
			{
				foreach (X509CrlEntry entry in revocations)
				{
					try
					{
						tbsGen.AddCrlEntry(
							Asn1Sequence.GetInstance(
							Asn1Object.FromByteArray(entry.GetEncoded())));
					}
					catch (IOException e)
					{
						throw new CrlException("exception processing encoding of CRL", e);
					}
				}
			}
		}

        /// <summary>
        /// Set the signature algorithm that will be used to sign this CRL.
        /// </summary>
        /// <param name="signatureAlgorithm"/>
        [Obsolete("Not needed if Generate used with an ISignatureFactory")]
        public void SetSignatureAlgorithm(
			string signatureAlgorithm)
		{
			this.signatureAlgorithm = signatureAlgorithm;

			try
			{
				sigOID = X509Utilities.GetAlgorithmOid(signatureAlgorithm);
			}
			catch (Exception e)
			{
				throw new ArgumentException("Unknown signature type requested", e);
			}

			sigAlgId = X509Utilities.GetSigAlgID(sigOID, signatureAlgorithm);

			tbsGen.SetSignature(sigAlgId);
		}

		/**
		* add a given extension field for the standard extensions tag (tag 0)
		*/
		public void AddExtension(
			string			oid,
			bool			critical,
			Asn1Encodable	extensionValue)
		{
			extGenerator.AddExtension(new DerObjectIdentifier(oid), critical, extensionValue);
		}

		/**
		* add a given extension field for the standard extensions tag (tag 0)
		*/
		public void AddExtension(
			DerObjectIdentifier	oid,
			bool				critical,
			Asn1Encodable		extensionValue)
		{
			extGenerator.AddExtension(oid, critical, extensionValue);
		}

		/**
		* add a given extension field for the standard extensions tag (tag 0)
		*/
		public void AddExtension(
			string	oid,
			bool	critical,
			byte[]	extensionValue)
		{
			extGenerator.AddExtension(new DerObjectIdentifier(oid), critical, new DerOctetString(extensionValue));
		}

		/**
		* add a given extension field for the standard extensions tag (tag 0)
		*/
		public void AddExtension(
			DerObjectIdentifier	oid,
			bool				critical,
			byte[]				extensionValue)
		{
			extGenerator.AddExtension(oid, critical, new DerOctetString(extensionValue));
		}

        /// <summary>
        /// Generate an X.509 CRL, based on the current issuer and subject.
        /// </summary>
        /// <param name="privateKey">The private key of the issuer that is signing this certificate.</param>
        /// <returns>An X509Crl.</returns>
        [Obsolete("Use Generate with an ISignatureFactory")]
        public X509Crl Generate(
            AsymmetricKeyParameter privateKey)
        {
            return Generate(privateKey, null);
        }

        /// <summary>
        /// Generate an X.509 CRL, based on the current issuer and subject using the specified secure random.
        /// </summary>
        /// <param name="privateKey">The private key of the issuer that is signing this certificate.</param>
        /// <param name="random">Your Secure Random instance.</param>
        /// <returns>An X509Crl.</returns>
        [Obsolete("Use Generate with an ISignatureFactory")]
        public X509Crl Generate(
            AsymmetricKeyParameter privateKey,
            SecureRandom random)
        {
            return Generate(new Asn1SignatureFactory(signatureAlgorithm, privateKey, random));
        }

        /// <summary>
        /// Generate a new X509Crl using the passed in SignatureCalculator.
        /// </summary>
		/// <param name="signatureCalculatorFactory">A signature calculator factory with the necessary algorithm details.</param>
        /// <returns>An X509Crl.</returns>
        public X509Crl Generate(ISignatureFactory signatureCalculatorFactory)
        {
            tbsGen.SetSignature((AlgorithmIdentifier)signatureCalculatorFactory.AlgorithmDetails);

            TbsCertificateList tbsCertList = GenerateCertList();

            IStreamCalculator streamCalculator = signatureCalculatorFactory.CreateCalculator();

            byte[] encoded = tbsCertList.GetDerEncoded();

            streamCalculator.Stream.Write(encoded, 0, encoded.Length);

            Platform.Dispose(streamCalculator.Stream);

            return GenerateJcaObject(tbsCertList, (AlgorithmIdentifier)signatureCalculatorFactory.AlgorithmDetails, ((IBlockResult)streamCalculator.GetResult()).Collect());
        }

        private TbsCertificateList GenerateCertList()
		{
			if (!extGenerator.IsEmpty)
			{
				tbsGen.SetExtensions(extGenerator.Generate());
			}

			return tbsGen.GenerateTbsCertList();
		}

		private X509Crl GenerateJcaObject(
			TbsCertificateList	tbsCrl,
            AlgorithmIdentifier algId,
			byte[]				signature)
		{
			return new X509Crl(
				CertificateList.GetInstance(
					new DerSequence(tbsCrl, algId, new DerBitString(signature))));
		}

		/// <summary>
		/// Allows enumeration of the signature names supported by the generator.
		/// </summary>
		public IEnumerable SignatureAlgNames
		{
			get { return X509Utilities.GetAlgNames(); }
		}
	}
}
