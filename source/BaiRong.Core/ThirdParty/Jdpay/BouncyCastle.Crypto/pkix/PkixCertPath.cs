using System;
using System.Collections;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Pkix
{
	/**
	 * An immutable sequence of certificates (a certification path).<br />
	 * <br />
	 * This is an abstract class that defines the methods common to all CertPaths.
	 * Subclasses can handle different kinds of certificates (X.509, PGP, etc.).<br />
	 * <br />
	 * All CertPath objects have a type, a list of Certificates, and one or more
	 * supported encodings. Because the CertPath class is immutable, a CertPath
	 * cannot change in any externally visible way after being constructed. This
	 * stipulation applies to all public fields and methods of this class and any
	 * added or overridden by subclasses.<br />
	 * <br />
	 * The type is a string that identifies the type of Certificates in the
	 * certification path. For each certificate cert in a certification path
	 * certPath, cert.getType().equals(certPath.getType()) must be true.<br />
	 * <br />
	 * The list of Certificates is an ordered List of zero or more Certificates.
	 * This List and all of the Certificates contained in it must be immutable.<br />
	 * <br />
	 * Each CertPath object must support one or more encodings so that the object
	 * can be translated into a byte array for storage or transmission to other
	 * parties. Preferably, these encodings should be well-documented standards
	 * (such as PKCS#7). One of the encodings supported by a CertPath is considered
	 * the default encoding. This encoding is used if no encoding is explicitly
	 * requested (for the {@link #getEncoded()} method, for instance).<br />
	 * <br />
	 * All CertPath objects are also Serializable. CertPath objects are resolved
	 * into an alternate {@link CertPathRep} object during serialization. This
	 * allows a CertPath object to be serialized into an equivalent representation
	 * regardless of its underlying implementation.<br />
	 * <br />
	 * CertPath objects can be created with a CertificateFactory or they can be
	 * returned by other classes, such as a CertPathBuilder.<br />
	 * <br />
	 * By convention, X.509 CertPaths (consisting of X509Certificates), are ordered
	 * starting with the target certificate and ending with a certificate issued by
	 * the trust anchor. That is, the issuer of one certificate is the subject of
	 * the following one. The certificate representing the
	 * {@link TrustAnchor TrustAnchor} should not be included in the certification
	 * path. Unvalidated X.509 CertPaths may not follow these conventions. PKIX
	 * CertPathValidators will detect any departure from these conventions that
	 * cause the certification path to be invalid and throw a
	 * CertPathValidatorException.<br />
	 * <br />
	 * <strong>Concurrent Access</strong><br />
	 * <br />
	 * All CertPath objects must be thread-safe. That is, multiple threads may
	 * concurrently invoke the methods defined in this class on a single CertPath
	 * object (or more than one) with no ill effects. This is also true for the List
	 * returned by CertPath.getCertificates.<br />
	 * <br />
	 * Requiring CertPath objects to be immutable and thread-safe allows them to be
	 * passed around to various pieces of code without worrying about coordinating
	 * access. Providing this thread-safety is generally not difficult, since the
	 * CertPath and List objects in question are immutable.
	 *
	 * @see CertificateFactory
	 * @see CertPathBuilder
	 */
	/// <summary>
	/// CertPath implementation for X.509 certificates.
	/// </summary>
	public class PkixCertPath
//		: CertPath
	{
		internal static readonly IList certPathEncodings;

        static PkixCertPath()
        {
            IList encodings = Platform.CreateArrayList();
            encodings.Add("PkiPath");
            encodings.Add("PEM");
            encodings.Add("PKCS7");
            certPathEncodings = CollectionUtilities.ReadOnly(encodings);
        }

        private readonly IList certificates;

		/**
		 * @param certs
		 */
		private static IList SortCerts(
			IList certs)
		{
			if (certs.Count < 2)
				return certs;

			X509Name issuer = ((X509Certificate)certs[0]).IssuerDN;
			bool okay = true;

			for (int i = 1; i != certs.Count; i++)
			{
				X509Certificate cert = (X509Certificate)certs[i];

				if (issuer.Equivalent(cert.SubjectDN, true))
				{
					issuer = ((X509Certificate)certs[i]).IssuerDN;
				}
				else
				{
					okay = false;
					break;
				}
			}

			if (okay)
				return certs;

			// find end-entity cert
            IList retList = Platform.CreateArrayList(certs.Count);
            IList orig = Platform.CreateArrayList(certs);

			for (int i = 0; i < certs.Count; i++)
			{
				X509Certificate cert = (X509Certificate)certs[i];
				bool           found = false;

				X509Name subject = cert.SubjectDN;
				foreach (X509Certificate c in certs)
				{
					if (c.IssuerDN.Equivalent(subject, true))
					{
						found = true;
						break;
					}
				}

				if (!found)
				{
					retList.Add(cert);
					certs.RemoveAt(i);
				}
			}

			// can only have one end entity cert - something's wrong, give up.
			if (retList.Count > 1)
				return orig;

			for (int i = 0; i != retList.Count; i++)
			{
				issuer = ((X509Certificate)retList[i]).IssuerDN;

				for (int j = 0; j < certs.Count; j++)
				{
					X509Certificate c = (X509Certificate)certs[j];
					if (issuer.Equivalent(c.SubjectDN, true))
					{
						retList.Add(c);
						certs.RemoveAt(j);
						break;
					}
				}
			}

			// make sure all certificates are accounted for.
			if (certs.Count > 0)
				return orig;

			return retList;
		}

		/**
		 * Creates a CertPath of the specified type.
		 * This constructor is protected because most users should use
		 * a CertificateFactory to create CertPaths.
		 * @param type the standard name of the type of Certificatesin this path
		 **/
		public PkixCertPath(
			ICollection certificates)
//			: base("X.509")
		{
			this.certificates = SortCerts(Platform.CreateArrayList(certificates));
		}

		public PkixCertPath(
			Stream inStream)
			: this(inStream, "PkiPath")
		{
		}

		/**
		 * Creates a CertPath of the specified type.
		 * This constructor is protected because most users should use
		 * a CertificateFactory to create CertPaths.
		 *
		 * @param type the standard name of the type of Certificatesin this path
		 **/
		public PkixCertPath(
			Stream	inStream,
			string	encoding)
//			: base("X.509")
		{
            string upper = Platform.ToUpperInvariant(encoding);

            IList certs;
			try
			{
				if (upper.Equals(Platform.ToUpperInvariant("PkiPath")))
				{
					Asn1InputStream derInStream = new Asn1InputStream(inStream);
					Asn1Object derObject = derInStream.ReadObject();
					if (!(derObject is Asn1Sequence))
					{
						throw new CertificateException(
							"input stream does not contain a ASN1 SEQUENCE while reading PkiPath encoded data to load CertPath");
					}

                    certs = Platform.CreateArrayList();

                    foreach (Asn1Encodable ae in (Asn1Sequence)derObject)
                    {
                        byte[] derBytes = ae.GetEncoded(Asn1Encodable.Der);
                        Stream certInStream = new MemoryStream(derBytes, false);

                        // TODO Is inserting at the front important (list will be sorted later anyway)?
                        certs.Insert(0, new X509CertificateParser().ReadCertificate(certInStream));
					}
				}
                else if (upper.Equals("PKCS7") || upper.Equals("PEM"))
				{
                    certs = Platform.CreateArrayList(new X509CertificateParser().ReadCertificates(inStream));
				}
				else
				{
					throw new CertificateException("unsupported encoding: " + encoding);
				}
			}
			catch (IOException ex)
			{
				throw new CertificateException(
					"IOException throw while decoding CertPath:\n"
					+ ex.ToString());
			}

			this.certificates = SortCerts(certs);
		}

		/**
		 * Returns an iteration of the encodings supported by this
		 * certification path, with the default encoding
		 * first. Attempts to modify the returned Iterator via its
		 * remove method result in an UnsupportedOperationException.
		 *
		 * @return an Iterator over the names of the supported encodings (as Strings)
		 **/
		public virtual IEnumerable Encodings
		{
            get { return new EnumerableProxy(certPathEncodings); }
		}

		/**
		* Compares this certification path for equality with the specified object.
		* Two CertPaths are equal if and only if their types are equal and their
		* certificate Lists (and by implication the Certificates in those Lists)
		* are equal. A CertPath is never equal to an object that is not a CertPath.<br />
		* <br />
		* This algorithm is implemented by this method. If it is overridden, the
		* behavior specified here must be maintained.
		*
		* @param other
		*            the object to test for equality with this certification path
		*
		* @return true if the specified object is equal to this certification path,
		*         false otherwise
		*
		* @see Object#hashCode() Object.hashCode()
		*/
		public override bool Equals(
			object obj)
		{
			if (this == obj)
				return true;

			PkixCertPath other = obj as PkixCertPath;
			if (other == null)
				return false;

//			if (!this.Type.Equals(other.Type))
//				return false;

			//return this.Certificates.Equals(other.Certificates);

			// TODO Extract this to a utility class
			IList thisCerts = this.Certificates;
			IList otherCerts = other.Certificates;

			if (thisCerts.Count != otherCerts.Count)
				return false;

			IEnumerator e1 = thisCerts.GetEnumerator();
			IEnumerator e2 = thisCerts.GetEnumerator();

			while (e1.MoveNext())
			{
				e2.MoveNext();

				if (!Platform.Equals(e1.Current, e2.Current))
					return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			// FIXME?
			return this.Certificates.GetHashCode();
		}

		/**
		 * Returns the encoded form of this certification path, using
		 * the default encoding.
		 *
		 * @return the encoded bytes
		 * @exception CertificateEncodingException if an encoding error occurs
		 **/
		public virtual byte[] GetEncoded()
		{
			foreach (object enc in Encodings)
			{
				if (enc is string)
				{
					return GetEncoded((string)enc);
				}
			}
			return null;
		}

		/**
		 * Returns the encoded form of this certification path, using
		 * the specified encoding.
		 *
		 * @param encoding the name of the encoding to use
		 * @return the encoded bytes
		 * @exception CertificateEncodingException if an encoding error
		 * occurs or the encoding requested is not supported
		 *
		 */
		public virtual byte[] GetEncoded(
			string encoding)
		{
			if (Platform.EqualsIgnoreCase(encoding, "PkiPath"))
			{
				Asn1EncodableVector v = new Asn1EncodableVector();

				for (int i = certificates.Count - 1; i >= 0; i--)
				{
					v.Add(ToAsn1Object((X509Certificate) certificates[i]));
				}

				return ToDerEncoded(new DerSequence(v));
			}
            else if (Platform.EqualsIgnoreCase(encoding, "PKCS7"))
			{
				Asn1.Pkcs.ContentInfo encInfo = new Asn1.Pkcs.ContentInfo(
					PkcsObjectIdentifiers.Data, null);

				Asn1EncodableVector v = new Asn1EncodableVector();
				for (int i = 0; i != certificates.Count; i++)
				{
					v.Add(ToAsn1Object((X509Certificate)certificates[i]));
				}

				Asn1.Pkcs.SignedData sd = new Asn1.Pkcs.SignedData(
					new DerInteger(1),
					new DerSet(),
					encInfo,
					new DerSet(v),
					null,
					new DerSet());

				return ToDerEncoded(new Asn1.Pkcs.ContentInfo(PkcsObjectIdentifiers.SignedData, sd));
			}
            else if (Platform.EqualsIgnoreCase(encoding, "PEM"))
			{
				MemoryStream bOut = new MemoryStream();
				PemWriter pWrt = new PemWriter(new StreamWriter(bOut));

				try
				{
					for (int i = 0; i != certificates.Count; i++)
					{
						pWrt.WriteObject(certificates[i]);
					}

                    Platform.Dispose(pWrt.Writer);
				}
				catch (Exception)
				{
					throw new CertificateEncodingException("can't encode certificate for PEM encoded path");
				}

				return bOut.ToArray();
			}
			else
			{
				throw new CertificateEncodingException("unsupported encoding: " + encoding);
			}
		}

		/// <summary>
		/// Returns the list of certificates in this certification
		/// path.
		/// </summary>
		public virtual IList Certificates
		{
            get { return CollectionUtilities.ReadOnly(certificates); }
		}

		/**
		 * Return a DERObject containing the encoded certificate.
		 *
		 * @param cert the X509Certificate object to be encoded
		 *
		 * @return the DERObject
		 **/
		private Asn1Object ToAsn1Object(
			X509Certificate cert)
		{
			try
			{
				return Asn1Object.FromByteArray(cert.GetEncoded());
			}
			catch (Exception e)
			{
				throw new CertificateEncodingException("Exception while encoding certificate", e);
			}
		}

		private byte[] ToDerEncoded(Asn1Encodable obj)
		{
			try
			{
				return obj.GetEncoded(Asn1Encodable.Der);
			}
			catch (IOException e)
			{
				throw new CertificateEncodingException("Exception thrown", e);
			}
		}
	}
}
