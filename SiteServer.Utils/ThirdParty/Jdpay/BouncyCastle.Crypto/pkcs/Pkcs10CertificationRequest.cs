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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto.Operators;

namespace Org.BouncyCastle.Pkcs
{
	/// <remarks>
	/// A class for verifying and creating Pkcs10 Certification requests.
	/// </remarks>
	/// <code>
	/// CertificationRequest ::= Sequence {
	///   certificationRequestInfo  CertificationRequestInfo,
	///   signatureAlgorithm        AlgorithmIdentifier{{ SignatureAlgorithms }},
	///   signature                 BIT STRING
	/// }
	///
	/// CertificationRequestInfo ::= Sequence {
	///   version             Integer { v1(0) } (v1,...),
	///   subject             Name,
	///   subjectPKInfo   SubjectPublicKeyInfo{{ PKInfoAlgorithms }},
	///   attributes          [0] Attributes{{ CRIAttributes }}
	///  }
	///
	///  Attributes { ATTRIBUTE:IOSet } ::= Set OF Attr{{ IOSet }}
	///
	///  Attr { ATTRIBUTE:IOSet } ::= Sequence {
	///    type    ATTRIBUTE.&amp;id({IOSet}),
	///    values  Set SIZE(1..MAX) OF ATTRIBUTE.&amp;Type({IOSet}{\@type})
	///  }
	/// </code>
	/// see <a href="http://www.rsasecurity.com/rsalabs/node.asp?id=2132"/>
	public class Pkcs10CertificationRequest
		: CertificationRequest
	{
		protected static readonly IDictionary algorithms = Platform.CreateHashtable();
        protected static readonly IDictionary exParams = Platform.CreateHashtable();
        protected static readonly IDictionary keyAlgorithms = Platform.CreateHashtable();
        protected static readonly IDictionary oids = Platform.CreateHashtable();
		protected static readonly ISet noParams = new HashSet();

		static Pkcs10CertificationRequest()
		{
			algorithms.Add("MD2WITHRSAENCRYPTION", PkcsObjectIdentifiers.MD2WithRsaEncryption);
			algorithms.Add("MD2WITHRSA", PkcsObjectIdentifiers.MD2WithRsaEncryption);
			algorithms.Add("MD5WITHRSAENCRYPTION", PkcsObjectIdentifiers.MD5WithRsaEncryption);
			algorithms.Add("MD5WITHRSA", PkcsObjectIdentifiers.MD5WithRsaEncryption);
			algorithms.Add("RSAWITHMD5", PkcsObjectIdentifiers.MD5WithRsaEncryption);
			algorithms.Add("SHA1WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha1WithRsaEncryption);
			algorithms.Add("SHA1WITHRSA", PkcsObjectIdentifiers.Sha1WithRsaEncryption);
			algorithms.Add("SHA224WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha224WithRsaEncryption);
			algorithms.Add("SHA224WITHRSA", PkcsObjectIdentifiers.Sha224WithRsaEncryption);
			algorithms.Add("SHA256WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha256WithRsaEncryption);
			algorithms.Add("SHA256WITHRSA", PkcsObjectIdentifiers.Sha256WithRsaEncryption);
			algorithms.Add("SHA384WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha384WithRsaEncryption);
			algorithms.Add("SHA384WITHRSA", PkcsObjectIdentifiers.Sha384WithRsaEncryption);
			algorithms.Add("SHA512WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha512WithRsaEncryption);
			algorithms.Add("SHA512WITHRSA", PkcsObjectIdentifiers.Sha512WithRsaEncryption);
			algorithms.Add("SHA1WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
			algorithms.Add("SHA224WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
			algorithms.Add("SHA256WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
			algorithms.Add("SHA384WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
			algorithms.Add("SHA512WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
			algorithms.Add("RSAWITHSHA1", PkcsObjectIdentifiers.Sha1WithRsaEncryption);
			algorithms.Add("RIPEMD128WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128);
			algorithms.Add("RIPEMD128WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128);
			algorithms.Add("RIPEMD160WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160);
			algorithms.Add("RIPEMD160WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160);
			algorithms.Add("RIPEMD256WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256);
			algorithms.Add("RIPEMD256WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256);
			algorithms.Add("SHA1WITHDSA", X9ObjectIdentifiers.IdDsaWithSha1);
			algorithms.Add("DSAWITHSHA1", X9ObjectIdentifiers.IdDsaWithSha1);
			algorithms.Add("SHA224WITHDSA", NistObjectIdentifiers.DsaWithSha224);
			algorithms.Add("SHA256WITHDSA", NistObjectIdentifiers.DsaWithSha256);
			algorithms.Add("SHA384WITHDSA", NistObjectIdentifiers.DsaWithSha384);
			algorithms.Add("SHA512WITHDSA", NistObjectIdentifiers.DsaWithSha512);
			algorithms.Add("SHA1WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha1);
			algorithms.Add("SHA224WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha224);
			algorithms.Add("SHA256WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha256);
			algorithms.Add("SHA384WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha384);
			algorithms.Add("SHA512WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha512);
			algorithms.Add("ECDSAWITHSHA1", X9ObjectIdentifiers.ECDsaWithSha1);
			algorithms.Add("GOST3411WITHGOST3410", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
			algorithms.Add("GOST3410WITHGOST3411", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
			algorithms.Add("GOST3411WITHECGOST3410", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
			algorithms.Add("GOST3411WITHECGOST3410-2001", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
			algorithms.Add("GOST3411WITHGOST3410-2001", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);

			//
			// reverse mappings
			//
            oids.Add(PkcsObjectIdentifiers.Sha1WithRsaEncryption, "SHA1WITHRSA");
            oids.Add(PkcsObjectIdentifiers.Sha224WithRsaEncryption, "SHA224WITHRSA");
            oids.Add(PkcsObjectIdentifiers.Sha256WithRsaEncryption, "SHA256WITHRSA");
            oids.Add(PkcsObjectIdentifiers.Sha384WithRsaEncryption, "SHA384WITHRSA");
            oids.Add(PkcsObjectIdentifiers.Sha512WithRsaEncryption, "SHA512WITHRSA");
            oids.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94, "GOST3411WITHGOST3410");
            oids.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001, "GOST3411WITHECGOST3410");

            oids.Add(PkcsObjectIdentifiers.MD5WithRsaEncryption, "MD5WITHRSA");
            oids.Add(PkcsObjectIdentifiers.MD2WithRsaEncryption, "MD2WITHRSA");
            oids.Add(X9ObjectIdentifiers.IdDsaWithSha1, "SHA1WITHDSA");
            oids.Add(X9ObjectIdentifiers.ECDsaWithSha1, "SHA1WITHECDSA");
            oids.Add(X9ObjectIdentifiers.ECDsaWithSha224, "SHA224WITHECDSA");
            oids.Add(X9ObjectIdentifiers.ECDsaWithSha256, "SHA256WITHECDSA");
            oids.Add(X9ObjectIdentifiers.ECDsaWithSha384, "SHA384WITHECDSA");
            oids.Add(X9ObjectIdentifiers.ECDsaWithSha512, "SHA512WITHECDSA");
            oids.Add(OiwObjectIdentifiers.MD5WithRsa, "MD5WITHRSA");
            oids.Add(OiwObjectIdentifiers.Sha1WithRsa, "SHA1WITHRSA");
            oids.Add(OiwObjectIdentifiers.DsaWithSha1, "SHA1WITHDSA");
            oids.Add(NistObjectIdentifiers.DsaWithSha224, "SHA224WITHDSA");
            oids.Add(NistObjectIdentifiers.DsaWithSha256, "SHA256WITHDSA");

			//
			// key types
			//
			keyAlgorithms.Add(PkcsObjectIdentifiers.RsaEncryption, "RSA");
			keyAlgorithms.Add(X9ObjectIdentifiers.IdDsa, "DSA");

			//
			// According to RFC 3279, the ASN.1 encoding SHALL (id-dsa-with-sha1) or MUST (ecdsa-with-SHA*) omit the parameters field.
			// The parameters field SHALL be NULL for RSA based signature algorithms.
			//
			noParams.Add(X9ObjectIdentifiers.ECDsaWithSha1);
			noParams.Add(X9ObjectIdentifiers.ECDsaWithSha224);
			noParams.Add(X9ObjectIdentifiers.ECDsaWithSha256);
			noParams.Add(X9ObjectIdentifiers.ECDsaWithSha384);
			noParams.Add(X9ObjectIdentifiers.ECDsaWithSha512);
			noParams.Add(X9ObjectIdentifiers.IdDsaWithSha1);
			noParams.Add(NistObjectIdentifiers.DsaWithSha224);
			noParams.Add(NistObjectIdentifiers.DsaWithSha256);

			//
			// RFC 4491
			//
			noParams.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
			noParams.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);

			//
			// explicit params
			//
			AlgorithmIdentifier sha1AlgId = new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1, DerNull.Instance);
			exParams.Add("SHA1WITHRSAANDMGF1", CreatePssParams(sha1AlgId, 20));

			AlgorithmIdentifier sha224AlgId = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha224, DerNull.Instance);
			exParams.Add("SHA224WITHRSAANDMGF1", CreatePssParams(sha224AlgId, 28));

			AlgorithmIdentifier sha256AlgId = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha256, DerNull.Instance);
			exParams.Add("SHA256WITHRSAANDMGF1", CreatePssParams(sha256AlgId, 32));

			AlgorithmIdentifier sha384AlgId = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha384, DerNull.Instance);
			exParams.Add("SHA384WITHRSAANDMGF1", CreatePssParams(sha384AlgId, 48));

			AlgorithmIdentifier sha512AlgId = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha512, DerNull.Instance);
			exParams.Add("SHA512WITHRSAANDMGF1", CreatePssParams(sha512AlgId, 64));
		}

		private static RsassaPssParameters CreatePssParams(
			AlgorithmIdentifier	hashAlgId,
			int					saltSize)
		{
			return new RsassaPssParameters(
				hashAlgId,
				new AlgorithmIdentifier(PkcsObjectIdentifiers.IdMgf1, hashAlgId),
				new DerInteger(saltSize),
				new DerInteger(1));
		}

		protected Pkcs10CertificationRequest()
		{
		}

		public Pkcs10CertificationRequest(
			byte[] encoded)
			: base((Asn1Sequence) Asn1Object.FromByteArray(encoded))
		{
		}

		public Pkcs10CertificationRequest(
			Asn1Sequence seq)
			: base(seq)
		{
		}

		public Pkcs10CertificationRequest(
			Stream input)
			: base((Asn1Sequence) Asn1Object.FromStream(input))
		{
        }

        /// <summary>
        /// Instantiate a Pkcs10CertificationRequest object with the necessary credentials.
        /// </summary>
        ///<param name="signatureAlgorithm">Name of Sig Alg.</param>
        /// <param name="subject">X509Name of subject eg OU="My unit." O="My Organisatioin" C="au" </param>
        /// <param name="publicKey">Public Key to be included in cert reqest.</param>
        /// <param name="attributes">ASN1Set of Attributes.</param>
        /// <param name="signingKey">Matching Private key for nominated (above) public key to be used to sign the request.</param>
        [Obsolete("Use constructor with an ISignatureFactory")]
        public Pkcs10CertificationRequest(
			string					signatureAlgorithm,
			X509Name				subject,
			AsymmetricKeyParameter	publicKey,
			Asn1Set					attributes,
			AsymmetricKeyParameter	signingKey)
		{
			if (signatureAlgorithm == null)
				throw new ArgumentNullException("signatureAlgorithm");
			if (subject == null)
				throw new ArgumentNullException("subject");
			if (publicKey == null)
				throw new ArgumentNullException("publicKey");
			if (publicKey.IsPrivate)
				throw new ArgumentException("expected public key", "publicKey");
			if (!signingKey.IsPrivate)
				throw new ArgumentException("key for signing must be private", "signingKey");

            init(new Asn1SignatureFactory(signatureAlgorithm, signingKey), subject, publicKey, attributes, signingKey);
		}

        /// <summary>
        /// Instantiate a Pkcs10CertificationRequest object with the necessary credentials.
        /// </summary>
        ///<param name="signatureCalculatorFactory">The factory for signature calculators to sign the PKCS#10 request with.</param>
        /// <param name="subject">X509Name of subject eg OU="My unit." O="My Organisatioin" C="au" </param>
        /// <param name="publicKey">Public Key to be included in cert reqest.</param>
        /// <param name="attributes">ASN1Set of Attributes.</param>
        /// <param name="signingKey">Matching Private key for nominated (above) public key to be used to sign the request.</param>
        public Pkcs10CertificationRequest(
            ISignatureFactory signatureCalculatorFactory,
            X509Name subject,
            AsymmetricKeyParameter publicKey,
            Asn1Set attributes,
            AsymmetricKeyParameter signingKey)
        {
            if (signatureCalculatorFactory == null)
                throw new ArgumentNullException("signatureCalculator");
            if (subject == null)
                throw new ArgumentNullException("subject");
            if (publicKey == null)
                throw new ArgumentNullException("publicKey");
            if (publicKey.IsPrivate)
                throw new ArgumentException("expected public key", "publicKey");
            if (!signingKey.IsPrivate)
                throw new ArgumentException("key for signing must be private", "signingKey");

            init(signatureCalculatorFactory, subject, publicKey, attributes, signingKey);
        }

        private void init(
            ISignatureFactory signatureCalculator, 
            X509Name subject,
            AsymmetricKeyParameter publicKey,
            Asn1Set attributes,
            AsymmetricKeyParameter signingKey)
        {
            this.sigAlgId = (AlgorithmIdentifier)signatureCalculator.AlgorithmDetails;

            SubjectPublicKeyInfo pubInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);

            this.reqInfo = new CertificationRequestInfo(subject, pubInfo, attributes);

            IStreamCalculator streamCalculator = signatureCalculator.CreateCalculator();

            byte[] reqInfoData = reqInfo.GetDerEncoded();

            streamCalculator.Stream.Write(reqInfoData, 0, reqInfoData.Length);

            Platform.Dispose(streamCalculator.Stream);

            // Generate Signature.
            sigBits = new DerBitString(((IBlockResult)streamCalculator.GetResult()).Collect());
        }

        //        internal Pkcs10CertificationRequest(
        //        	Asn1InputStream seqStream)
        //        {
        //			Asn1Sequence seq = (Asn1Sequence) seqStream.ReadObject();
        //            try
        //            {
        //                this.reqInfo = CertificationRequestInfo.GetInstance(seq[0]);
        //                this.sigAlgId = AlgorithmIdentifier.GetInstance(seq[1]);
        //                this.sigBits = (DerBitString) seq[2];
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new ArgumentException("Create From Asn1Sequence: " + ex.Message);
        //            }
        //        }

        /// <summary>
        /// Get the public key.
        /// </summary>
        /// <returns>The public key.</returns>
        public AsymmetricKeyParameter GetPublicKey()
		{
			return PublicKeyFactory.CreateKey(reqInfo.SubjectPublicKeyInfo);
		}

		/// <summary>
		/// Verify Pkcs10 Cert Request is valid.
		/// </summary>
		/// <returns>true = valid.</returns>
		public bool Verify()
		{
			return Verify(this.GetPublicKey());
		}

		public bool Verify(
			AsymmetricKeyParameter publicKey)
		{
            return Verify(new Asn1VerifierFactoryProvider(publicKey));
		}

        public bool Verify(
            IVerifierFactoryProvider verifierProvider)
        {
            return Verify(verifierProvider.CreateVerifierFactory(sigAlgId));
        }

        public bool Verify(
            IVerifierFactory verifier)
        {
            try
            {
                byte[] b = reqInfo.GetDerEncoded();

                IStreamCalculator streamCalculator = verifier.CreateCalculator();

                streamCalculator.Stream.Write(b, 0, b.Length);

                Platform.Dispose(streamCalculator.Stream);

                return ((IVerifier)streamCalculator.GetResult()).IsVerified(sigBits.GetOctets());
            }
            catch (Exception e)
            {
                throw new SignatureException("exception encoding TBS cert request", e);
            }
        }

        //        /// <summary>
        //        /// Get the Der Encoded Pkcs10 Certification Request.
        //        /// </summary>
        //        /// <returns>A byte array.</returns>
        //        public byte[] GetEncoded()
        //        {
        //        	return new CertificationRequest(reqInfo, sigAlgId, sigBits).GetDerEncoded();
        //        }

        // TODO Figure out how to set parameters on an ISigner
        private void SetSignatureParameters(
			ISigner			signature,
			Asn1Encodable	asn1Params)
		{
			if (asn1Params != null && !(asn1Params is Asn1Null))
			{
//				AlgorithmParameters sigParams = AlgorithmParameters.GetInstance(signature.getAlgorithm());
//
//				try
//				{
//					sigParams.init(asn1Params.ToAsn1Object().GetDerEncoded());
//				}
//				catch (IOException e)
//				{
//					throw new SignatureException("IOException decoding parameters: " + e.Message);
//				}

				if (Platform.EndsWith(signature.AlgorithmName, "MGF1"))
				{
					throw Platform.CreateNotImplementedException("signature algorithm with MGF1");

//					try
//					{
//						signature.setParameter(sigParams.getParameterSpec(PSSParameterSpec.class));
//					}
//					catch (GeneralSecurityException e)
//					{
//						throw new SignatureException("Exception extracting parameters: " + e.getMessage());
//					}
				}
			}
		}

		internal static string GetSignatureName(
			AlgorithmIdentifier sigAlgId)
		{
			Asn1Encodable asn1Params = sigAlgId.Parameters;

			if (asn1Params != null && !(asn1Params is Asn1Null))
			{
                if (sigAlgId.Algorithm.Equals(PkcsObjectIdentifiers.IdRsassaPss))
				{
					RsassaPssParameters rsaParams = RsassaPssParameters.GetInstance(asn1Params);
                    return GetDigestAlgName(rsaParams.HashAlgorithm.Algorithm) + "withRSAandMGF1";
				}
			}

            return sigAlgId.Algorithm.Id;
		}

		private static string GetDigestAlgName(
			DerObjectIdentifier digestAlgOID)
		{
			if (PkcsObjectIdentifiers.MD5.Equals(digestAlgOID))
			{
				return "MD5";
			}
			else if (OiwObjectIdentifiers.IdSha1.Equals(digestAlgOID))
			{
				return "SHA1";
			}
			else if (NistObjectIdentifiers.IdSha224.Equals(digestAlgOID))
			{
				return "SHA224";
			}
			else if (NistObjectIdentifiers.IdSha256.Equals(digestAlgOID))
			{
				return "SHA256";
			}
			else if (NistObjectIdentifiers.IdSha384.Equals(digestAlgOID))
			{
				return "SHA384";
			}
			else if (NistObjectIdentifiers.IdSha512.Equals(digestAlgOID))
			{
				return "SHA512";
			}
			else if (TeleTrusTObjectIdentifiers.RipeMD128.Equals(digestAlgOID))
			{
				return "RIPEMD128";
			}
			else if (TeleTrusTObjectIdentifiers.RipeMD160.Equals(digestAlgOID))
			{
				return "RIPEMD160";
			}
			else if (TeleTrusTObjectIdentifiers.RipeMD256.Equals(digestAlgOID))
			{
				return "RIPEMD256";
			}
			else if (CryptoProObjectIdentifiers.GostR3411.Equals(digestAlgOID))
			{
				return "GOST3411";
			}
			else
			{
				return digestAlgOID.Id;
			}
		}
	}
}
