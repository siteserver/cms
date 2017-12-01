using System;
using System.IO;
using System.Collections;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.X509
{
	/// <summary>
	/// Class to Generate X509V1 Certificates.
	/// </summary>
	public class X509V1CertificateGenerator
	{
		private V1TbsCertificateGenerator   tbsGen;
		private DerObjectIdentifier         sigOID;
		private AlgorithmIdentifier         sigAlgId;
		private string                      signatureAlgorithm;

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public X509V1CertificateGenerator()
		{
			tbsGen = new V1TbsCertificateGenerator();
		}

		/// <summary>
		/// Reset the generator.
		/// </summary>
		public void Reset()
		{
			tbsGen = new V1TbsCertificateGenerator();
		}

		/// <summary>
		/// Set the certificate's serial number.
		/// </summary>
		/// <remarks>Make serial numbers long, if you have no serial number policy make sure the number is at least 16 bytes of secure random data.
		/// You will be surprised how ugly a serial number collision can get.</remarks>
		/// <param name="serialNumber">The serial number.</param>
		public void SetSerialNumber(
			BigInteger serialNumber)
		{
			if (serialNumber.SignValue <= 0)
			{
				throw new ArgumentException("serial number must be a positive integer", "serialNumber");
			}

			tbsGen.SetSerialNumber(new DerInteger(serialNumber));
		}

		/// <summary>
		/// Set the issuer distinguished name.
		/// The issuer is the entity whose private key is used to sign the certificate.
		/// </summary>
		/// <param name="issuer">The issuers DN.</param>
		public void SetIssuerDN(
			X509Name issuer)
		{
			tbsGen.SetIssuer(issuer);
		}

		/// <summary>
		/// Set the date that this certificate is to be valid from.
		/// </summary>
		/// <param name="date"/>
		public void SetNotBefore(
			DateTime date)
		{
			tbsGen.SetStartDate(new Time(date));
		}

		/// <summary>
		/// Set the date after which this certificate will no longer be valid.
		/// </summary>
		/// <param name="date"/>
		public void SetNotAfter(
			DateTime date)
		{
			tbsGen.SetEndDate(new Time(date));
		}

		/// <summary>
		/// Set the subject distinguished name.
		/// The subject describes the entity associated with the public key.
		/// </summary>
		/// <param name="subject"/>
		public void SetSubjectDN(
			X509Name subject)
		{
			tbsGen.SetSubject(subject);
		}

        /// <summary>
        /// Set the public key that this certificate identifies.
        /// </summary>
        /// <param name="publicKey"/>
		public void SetPublicKey(
			AsymmetricKeyParameter publicKey)
		{
			try
			{
				tbsGen.SetSubjectPublicKeyInfo(
					SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey));
			}
			catch (Exception e)
			{
				throw new ArgumentException("unable to process key - " + e.ToString());
			}
		}

		/// <summary>
		/// Set the signature algorithm that will be used to sign this certificate.
		/// This can be either a name or an OID, names are treated as case insensitive.
		/// </summary>
		/// <param name="signatureAlgorithm">string representation of the algorithm name</param>
		[Obsolete("Not needed if Generate used with an ISignatureFactory")]
		public void SetSignatureAlgorithm(
			string signatureAlgorithm)
		{
			this.signatureAlgorithm = signatureAlgorithm;

			try
			{
				sigOID = X509Utilities.GetAlgorithmOid(signatureAlgorithm);
			}
			catch (Exception)
			{
				throw new ArgumentException("Unknown signature type requested", "signatureAlgorithm");
			}

			sigAlgId = X509Utilities.GetSigAlgID(sigOID, signatureAlgorithm);

			tbsGen.SetSignature(sigAlgId);
		}

		/// <summary>
		/// Generate a new X509Certificate.
		/// </summary>
		/// <param name="privateKey">The private key of the issuer used to sign this certificate.</param>
		/// <returns>An X509Certificate.</returns>
		[Obsolete("Use Generate with an ISignatureFactory")]
		public X509Certificate Generate(
			AsymmetricKeyParameter privateKey)
		{
			return Generate(privateKey, null);
		}

        /// <summary>
        /// Generate a new X509Certificate specifying a SecureRandom instance that you would like to use.
        /// </summary>
        /// <param name="privateKey">The private key of the issuer used to sign this certificate.</param>
        /// <param name="random">The Secure Random you want to use.</param>
        /// <returns>An X509Certificate.</returns>
		[Obsolete("Use Generate with an ISignatureFactory")]
		public X509Certificate Generate(
			AsymmetricKeyParameter	privateKey,
			SecureRandom			random)
		{
			return Generate(new Asn1SignatureFactory(signatureAlgorithm, privateKey, random));
		}

		/// <summary>
		/// Generate a new X509Certificate using the passed in SignatureCalculator.
		/// </summary>
		/// <param name="signatureCalculatorFactory">A signature calculator factory with the necessary algorithm details.</param>
		/// <returns>An X509Certificate.</returns>
		public X509Certificate Generate(ISignatureFactory signatureCalculatorFactory)
		{
			tbsGen.SetSignature ((AlgorithmIdentifier)signatureCalculatorFactory.AlgorithmDetails);

			TbsCertificateStructure tbsCert = tbsGen.GenerateTbsCertificate();

            IStreamCalculator streamCalculator = signatureCalculatorFactory.CreateCalculator();

            byte[] encoded = tbsCert.GetDerEncoded();

            streamCalculator.Stream.Write(encoded, 0, encoded.Length);

            Platform.Dispose(streamCalculator.Stream);

            return GenerateJcaObject(tbsCert, (AlgorithmIdentifier)signatureCalculatorFactory.AlgorithmDetails, ((IBlockResult)streamCalculator.GetResult()).Collect());
		}

		private X509Certificate GenerateJcaObject(
			TbsCertificateStructure	tbsCert,
			AlgorithmIdentifier     sigAlg,
			byte[]					signature)
		{
			return new X509Certificate(
				new X509CertificateStructure(tbsCert, sigAlg, new DerBitString(signature)));
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
