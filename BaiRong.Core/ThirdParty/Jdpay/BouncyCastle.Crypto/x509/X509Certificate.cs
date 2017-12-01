using System;
using System.Collections;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Misc;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509.Extension;
using Org.BouncyCastle.Crypto.Operators;

namespace Org.BouncyCastle.X509
{
    /// <summary>
    /// An Object representing an X509 Certificate.
    /// Has static methods for loading Certificates encoded in many forms that return X509Certificate Objects.
    /// </summary>
    public class X509Certificate
		:	X509ExtensionBase
//		, PKCS12BagAttributeCarrier
    {
        private readonly X509CertificateStructure c;
//        private Hashtable pkcs12Attributes = new Hashtable();
//        private ArrayList pkcs12Ordering = new ArrayList();
		private readonly BasicConstraints basicConstraints;
		private readonly bool[] keyUsage;

		private bool hashValueSet;
		private int hashValue;

		protected X509Certificate()
		{
		}

		public X509Certificate(
			X509CertificateStructure c)
		{
			this.c = c;

			try
			{
				Asn1OctetString str = this.GetExtensionValue(new DerObjectIdentifier("2.5.29.19"));

				if (str != null)
				{
					basicConstraints = BasicConstraints.GetInstance(
						X509ExtensionUtilities.FromExtensionValue(str));
				}
			}
			catch (Exception e)
			{
				throw new CertificateParsingException("cannot construct BasicConstraints: " + e);
			}

			try
			{
				Asn1OctetString str = this.GetExtensionValue(new DerObjectIdentifier("2.5.29.15"));

				if (str != null)
				{
					DerBitString bits = DerBitString.GetInstance(
						X509ExtensionUtilities.FromExtensionValue(str));

					byte[] bytes = bits.GetBytes();
					int length = (bytes.Length * 8) - bits.PadBits;

					keyUsage = new bool[(length < 9) ? 9 : length];

					for (int i = 0; i != length; i++)
					{
//						keyUsage[i] = (bytes[i / 8] & (0x80 >>> (i % 8))) != 0;
						keyUsage[i] = (bytes[i / 8] & (0x80 >> (i % 8))) != 0;
					}
				}
				else
				{
					keyUsage = null;
				}
			}
			catch (Exception e)
			{
				throw new CertificateParsingException("cannot construct KeyUsage: " + e);
			}
		}

//		internal X509Certificate(
//			Asn1Sequence seq)
//        {
//            this.c = X509CertificateStructure.GetInstance(seq);
//        }

//		/// <summary>
//        /// Load certificate from byte array.
//        /// </summary>
//        /// <param name="encoded">Byte array containing encoded X509Certificate.</param>
//        public X509Certificate(
//            byte[] encoded)
//			: this((Asn1Sequence) new Asn1InputStream(encoded).ReadObject())
//		{
//        }
//
//        /// <summary>
//        /// Load certificate from Stream.
//        /// Must be positioned at start of certificate.
//        /// </summary>
//        /// <param name="input"></param>
//        public X509Certificate(
//            Stream input)
//			: this((Asn1Sequence) new Asn1InputStream(input).ReadObject())
//        {
//        }

		public virtual X509CertificateStructure CertificateStructure
		{
			get { return c; }
		}

		/// <summary>
        /// Return true if the current time is within the start and end times nominated on the certificate.
        /// </summary>
        /// <returns>true id certificate is valid for the current time.</returns>
        public virtual bool IsValidNow
        {
			get { return IsValid(DateTime.UtcNow); }
        }

		/// <summary>
        /// Return true if the nominated time is within the start and end times nominated on the certificate.
        /// </summary>
        /// <param name="time">The time to test validity against.</param>
        /// <returns>True if certificate is valid for nominated time.</returns>
        public virtual bool IsValid(
			DateTime time)
        {
            return time.CompareTo(NotBefore) >= 0 && time.CompareTo(NotAfter) <= 0;
        }

		/// <summary>
		/// Checks if the current date is within certificate's validity period.
		/// </summary>
		public virtual void CheckValidity()
		{
			this.CheckValidity(DateTime.UtcNow);
		}

		/// <summary>
		/// Checks if the given date is within certificate's validity period.
		/// </summary>
		/// <exception cref="CertificateExpiredException">if the certificate is expired by given date</exception>
		/// <exception cref="CertificateNotYetValidException">if the certificate is not yet valid on given date</exception>
		public virtual void CheckValidity(
			DateTime time)
		{
			if (time.CompareTo(NotAfter) > 0)
				throw new CertificateExpiredException("certificate expired on " + c.EndDate.GetTime());
			if (time.CompareTo(NotBefore) < 0)
				throw new CertificateNotYetValidException("certificate not valid until " + c.StartDate.GetTime());
		}

		/// <summary>
        /// Return the certificate's version.
        /// </summary>
        /// <returns>An integer whose value Equals the version of the cerficate.</returns>
        public virtual int Version
        {
            get { return c.Version; }
        }

		/// <summary>
        /// Return a <see cref="Org.BouncyCastle.Math.BigInteger">BigInteger</see> containing the serial number.
        /// </summary>
        /// <returns>The Serial number.</returns>
        public virtual BigInteger SerialNumber
        {
            get { return c.SerialNumber.Value; }
        }

		/// <summary>
        /// Get the Issuer Distinguished Name. (Who signed the certificate.)
        /// </summary>
        /// <returns>And X509Object containing name and value pairs.</returns>
//        public IPrincipal IssuerDN
		public virtual X509Name IssuerDN
		{
            get { return c.Issuer; }
        }

		/// <summary>
        /// Get the subject of this certificate.
        /// </summary>
        /// <returns>An X509Name object containing name and value pairs.</returns>
//        public IPrincipal SubjectDN
		public virtual X509Name SubjectDN
		{
            get { return c.Subject; }
        }

		/// <summary>
		/// The time that this certificate is valid from.
		/// </summary>
		/// <returns>A DateTime object representing that time in the local time zone.</returns>
		public virtual DateTime NotBefore
		{
			get { return c.StartDate.ToDateTime(); }
		}

		/// <summary>
        /// The time that this certificate is valid up to.
        /// </summary>
        /// <returns>A DateTime object representing that time in the local time zone.</returns>
        public virtual DateTime NotAfter
        {
			get { return c.EndDate.ToDateTime(); }
        }

		/// <summary>
		/// Return the Der encoded TbsCertificate data.
		/// This is the certificate component less the signature.
		/// To Get the whole certificate call the GetEncoded() member.
		/// </summary>
		/// <returns>A byte array containing the Der encoded Certificate component.</returns>
		public virtual byte[] GetTbsCertificate()
		{
			return c.TbsCertificate.GetDerEncoded();
		}

		/// <summary>
		/// The signature.
		/// </summary>
		/// <returns>A byte array containg the signature of the certificate.</returns>
		public virtual byte[] GetSignature()
		{
			return c.GetSignatureOctets();
		}

        /// <summary>
		/// A meaningful version of the Signature Algorithm. (EG SHA1WITHRSA)
		/// </summary>
		/// <returns>A sting representing the signature algorithm.</returns>
		public virtual string SigAlgName
		{
            get { return SignerUtilities.GetEncodingName(c.SignatureAlgorithm.Algorithm); }
		}

		/// <summary>
		/// Get the Signature Algorithms Object ID.
		/// </summary>
		/// <returns>A string containg a '.' separated object id.</returns>
		public virtual string SigAlgOid
		{
            get { return c.SignatureAlgorithm.Algorithm.Id; }
		}

		/// <summary>
		/// Get the signature algorithms parameters. (EG DSA Parameters)
		/// </summary>
		/// <returns>A byte array containing the Der encoded version of the parameters or null if there are none.</returns>
		public virtual byte[] GetSigAlgParams()
		{
			if (c.SignatureAlgorithm.Parameters != null)
			{
				return c.SignatureAlgorithm.Parameters.GetDerEncoded();
			}

			return null;
		}

		/// <summary>
		/// Get the issuers UID.
		/// </summary>
		/// <returns>A DerBitString.</returns>
		public virtual DerBitString IssuerUniqueID
		{
			get { return c.TbsCertificate.IssuerUniqueID; }
		}

		/// <summary>
		/// Get the subjects UID.
		/// </summary>
		/// <returns>A DerBitString.</returns>
		public virtual DerBitString SubjectUniqueID
		{
			get { return c.TbsCertificate.SubjectUniqueID; }
		}

		/// <summary>
		/// Get a key usage guidlines.
		/// </summary>
		public virtual bool[] GetKeyUsage()
		{
			return keyUsage == null ? null : (bool[]) keyUsage.Clone();
		}

		// TODO Replace with something that returns a list of DerObjectIdentifier
		public virtual IList GetExtendedKeyUsage()
		{
			Asn1OctetString str = this.GetExtensionValue(new DerObjectIdentifier("2.5.29.37"));

			if (str == null)
				return null;

			try
			{
				Asn1Sequence seq = Asn1Sequence.GetInstance(
					X509ExtensionUtilities.FromExtensionValue(str));

                IList list = Platform.CreateArrayList();

				foreach (DerObjectIdentifier oid in seq)
				{
					list.Add(oid.Id);
				}

				return list;
			}
			catch (Exception e)
			{
				throw new CertificateParsingException("error processing extended key usage extension", e);
			}
		}

		public virtual int GetBasicConstraints()
		{
			if (basicConstraints != null && basicConstraints.IsCA())
			{
				if (basicConstraints.PathLenConstraint == null)
				{
					return int.MaxValue;
				}

				return basicConstraints.PathLenConstraint.IntValue;
			}

			return -1;
		}

		public virtual ICollection GetSubjectAlternativeNames()
		{
			return GetAlternativeNames("2.5.29.17");
		}

		public virtual ICollection GetIssuerAlternativeNames()
		{
			return GetAlternativeNames("2.5.29.18");
		}

		protected virtual ICollection GetAlternativeNames(
			string oid)
		{
			Asn1OctetString altNames = GetExtensionValue(new DerObjectIdentifier(oid));

			if (altNames == null)
				return null;

			Asn1Object asn1Object = X509ExtensionUtilities.FromExtensionValue(altNames);

			GeneralNames gns = GeneralNames.GetInstance(asn1Object);

            IList result = Platform.CreateArrayList();
			foreach (GeneralName gn in gns.GetNames())
			{
                IList entry = Platform.CreateArrayList();
				entry.Add(gn.TagNo);
				entry.Add(gn.Name.ToString());
				result.Add(entry);
			}
			return result;
		}

		protected override X509Extensions GetX509Extensions()
		{
			return c.Version >= 3
				?	c.TbsCertificate.Extensions
				:	null;
		}

		/// <summary>
		/// Get the public key of the subject of the certificate.
		/// </summary>
		/// <returns>The public key parameters.</returns>
		public virtual AsymmetricKeyParameter GetPublicKey()
		{
			return PublicKeyFactory.CreateKey(c.SubjectPublicKeyInfo);
		}

		/// <summary>
		/// Return a Der encoded version of this certificate.
		/// </summary>
		/// <returns>A byte array.</returns>
		public virtual byte[] GetEncoded()
		{
			return c.GetDerEncoded();
		}

		public override bool Equals(
			object obj)
		{
			if (obj == this)
				return true;

			X509Certificate other = obj as X509Certificate;

			if (other == null)
				return false;

			return c.Equals(other.c);

			// NB: May prefer this implementation of Equals if more than one certificate implementation in play
//			return Arrays.AreEqual(this.GetEncoded(), other.GetEncoded());
		}

		public override int GetHashCode()
		{
			lock (this)
			{
				if (!hashValueSet)
				{
					hashValue = c.GetHashCode();
					hashValueSet = true;
				}
			}

			return hashValue;
		}

//		public void setBagAttribute(
//			DERObjectIdentifier oid,
//			DEREncodable        attribute)
//		{
//			pkcs12Attributes.put(oid, attribute);
//			pkcs12Ordering.addElement(oid);
//		}
//
//		public DEREncodable getBagAttribute(
//			DERObjectIdentifier oid)
//		{
//			return (DEREncodable)pkcs12Attributes.get(oid);
//		}
//
//		public Enumeration getBagAttributeKeys()
//		{
//			return pkcs12Ordering.elements();
//		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
			string nl = Platform.NewLine;

			buf.Append("  [0]         Version: ").Append(this.Version).Append(nl);
			buf.Append("         SerialNumber: ").Append(this.SerialNumber).Append(nl);
			buf.Append("             IssuerDN: ").Append(this.IssuerDN).Append(nl);
			buf.Append("           Start Date: ").Append(this.NotBefore).Append(nl);
			buf.Append("           Final Date: ").Append(this.NotAfter).Append(nl);
			buf.Append("            SubjectDN: ").Append(this.SubjectDN).Append(nl);
			buf.Append("           Public Key: ").Append(this.GetPublicKey()).Append(nl);
			buf.Append("  Signature Algorithm: ").Append(this.SigAlgName).Append(nl);

			byte[] sig = this.GetSignature();
			buf.Append("            Signature: ").Append(Hex.ToHexString(sig, 0, 20)).Append(nl);

			for (int i = 20; i < sig.Length; i += 20)
			{
				int len = System.Math.Min(20, sig.Length - i);
				buf.Append("                       ").Append(Hex.ToHexString(sig, i, len)).Append(nl);
			}

			X509Extensions extensions = c.TbsCertificate.Extensions;

			if (extensions != null)
			{
				IEnumerator e = extensions.ExtensionOids.GetEnumerator();

				if (e.MoveNext())
				{
					buf.Append("       Extensions: \n");
				}

				do
				{
					DerObjectIdentifier oid = (DerObjectIdentifier)e.Current;
					X509Extension ext = extensions.GetExtension(oid);

					if (ext.Value != null)
					{
						byte[] octs = ext.Value.GetOctets();
						Asn1Object obj = Asn1Object.FromByteArray(octs);
						buf.Append("                       critical(").Append(ext.IsCritical).Append(") ");
						try
						{
							if (oid.Equals(X509Extensions.BasicConstraints))
							{
								buf.Append(BasicConstraints.GetInstance(obj));
							}
							else if (oid.Equals(X509Extensions.KeyUsage))
							{
								buf.Append(KeyUsage.GetInstance(obj));
							}
							else if (oid.Equals(MiscObjectIdentifiers.NetscapeCertType))
							{
								buf.Append(new NetscapeCertType((DerBitString) obj));
							}
							else if (oid.Equals(MiscObjectIdentifiers.NetscapeRevocationUrl))
							{
								buf.Append(new NetscapeRevocationUrl((DerIA5String) obj));
							}
							else if (oid.Equals(MiscObjectIdentifiers.VerisignCzagExtension))
							{
								buf.Append(new VerisignCzagExtension((DerIA5String) obj));
							}
							else
							{
								buf.Append(oid.Id);
								buf.Append(" value = ").Append(Asn1Dump.DumpAsString(obj));
								//buf.Append(" value = ").Append("*****").Append(nl);
							}
						}
						catch (Exception)
						{
							buf.Append(oid.Id);
							//buf.Append(" value = ").Append(new string(Hex.encode(ext.getValue().getOctets()))).Append(nl);
							buf.Append(" value = ").Append("*****");
						}
					}

					buf.Append(nl);
				}
				while (e.MoveNext());
			}

			return buf.ToString();
		}

		/// <summary>
		/// Verify the certificate's signature using the nominated public key.
		/// </summary>
		/// <param name="key">An appropriate public key parameter object, RsaPublicKeyParameters, DsaPublicKeyParameters or ECDsaPublicKeyParameters</param>
		/// <returns>True if the signature is valid.</returns>
		/// <exception cref="Exception">If key submitted is not of the above nominated types.</exception>
		public virtual void Verify(
			AsymmetricKeyParameter key)
		{
			CheckSignature(new Asn1VerifierFactory(c.SignatureAlgorithm, key));
		}

        /// <summary>
        /// Verify the certificate's signature using a verifier created using the passed in verifier provider.
        /// </summary>
        /// <param name="verifierProvider">An appropriate provider for verifying the certificate's signature.</param>
        /// <returns>True if the signature is valid.</returns>
        /// <exception cref="Exception">If verifier provider is not appropriate or the certificate algorithm is invalid.</exception>
        public virtual void Verify(
            IVerifierFactoryProvider verifierProvider)
        {
            CheckSignature(verifierProvider.CreateVerifierFactory (c.SignatureAlgorithm));
        }

        protected virtual void CheckSignature(
			IVerifierFactory verifier)
		{
			if (!IsAlgIDEqual(c.SignatureAlgorithm, c.TbsCertificate.Signature))
				throw new CertificateException("signature algorithm in TBS cert not same as outer cert");

			Asn1Encodable parameters = c.SignatureAlgorithm.Parameters;

            IStreamCalculator streamCalculator = verifier.CreateCalculator();

			byte[] b = this.GetTbsCertificate();

			streamCalculator.Stream.Write(b, 0, b.Length);

            Platform.Dispose(streamCalculator.Stream);

            if (!((IVerifier)streamCalculator.GetResult()).IsVerified(this.GetSignature()))
			{
				throw new InvalidKeyException("Public key presented not for certificate signature");
			}
		}

		private static bool IsAlgIDEqual(AlgorithmIdentifier id1, AlgorithmIdentifier id2)
		{
            if (!id1.Algorithm.Equals(id2.Algorithm))
				return false;

			Asn1Encodable p1 = id1.Parameters;
			Asn1Encodable p2 = id2.Parameters;

			if ((p1 == null) == (p2 == null))
				return Platform.Equals(p1, p2);

			// Exactly one of p1, p2 is null at this point
			return p1 == null
				?	p2.ToAsn1Object() is Asn1Null
				:	p1.ToAsn1Object() is Asn1Null;
		}
	}
}
