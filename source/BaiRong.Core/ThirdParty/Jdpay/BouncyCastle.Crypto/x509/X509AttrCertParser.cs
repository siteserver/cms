using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.X509
{
	public class X509AttrCertParser
	{
		private static readonly PemParser PemAttrCertParser = new PemParser("ATTRIBUTE CERTIFICATE");

		private Asn1Set	sData;
		private int		sDataObjectCount;
		private Stream	currentStream;

		private IX509AttributeCertificate ReadDerCertificate(
			Asn1InputStream dIn)
		{
			Asn1Sequence seq = (Asn1Sequence)dIn.ReadObject();

			if (seq.Count > 1 && seq[0] is DerObjectIdentifier)
			{
				if (seq[0].Equals(PkcsObjectIdentifiers.SignedData))
				{
					sData = SignedData.GetInstance(
						Asn1Sequence.GetInstance((Asn1TaggedObject) seq[1], true)).Certificates;

					return GetCertificate();
				}
			}

//			return new X509V2AttributeCertificate(seq.getEncoded());
			return new X509V2AttributeCertificate(AttributeCertificate.GetInstance(seq));
		}

		private IX509AttributeCertificate GetCertificate()
		{
			if (sData != null)
			{
				while (sDataObjectCount < sData.Count)
				{
					object obj = sData[sDataObjectCount++];

					if (obj is Asn1TaggedObject && ((Asn1TaggedObject)obj).TagNo == 2)
					{
						//return new X509V2AttributeCertificate(
						//	Asn1Sequence.GetInstance((Asn1TaggedObject)obj, false).GetEncoded());
						return new X509V2AttributeCertificate(
							AttributeCertificate.GetInstance(
								Asn1Sequence.GetInstance((Asn1TaggedObject)obj, false)));
					}
				}
			}

			return null;
		}

		private IX509AttributeCertificate ReadPemCertificate(
			Stream inStream)
		{
			Asn1Sequence seq = PemAttrCertParser.ReadPemObject(inStream);

			return seq == null
				?	null
				//:	new X509V2AttributeCertificate(seq.getEncoded());
				:	new X509V2AttributeCertificate(AttributeCertificate.GetInstance(seq));
		}

		/// <summary>
		/// Create loading data from byte array.
		/// </summary>
		/// <param name="input"></param>
		public IX509AttributeCertificate ReadAttrCert(
			byte[] input)
		{
			return ReadAttrCert(new MemoryStream(input, false));
		}

		/// <summary>
		/// Create loading data from byte array.
		/// </summary>
		/// <param name="input"></param>
		public ICollection ReadAttrCerts(
			byte[] input)
		{
			return ReadAttrCerts(new MemoryStream(input, false));
		}

		/**
		 * Generates a certificate object and initializes it with the data
		 * read from the input stream inStream.
		 */
		public IX509AttributeCertificate ReadAttrCert(
			Stream inStream)
		{
			if (inStream == null)
				throw new ArgumentNullException("inStream");
			if (!inStream.CanRead)
				throw new ArgumentException("inStream must be read-able", "inStream");

			if (currentStream == null)
			{
				currentStream = inStream;
				sData = null;
				sDataObjectCount = 0;
			}
			else if (currentStream != inStream) // reset if input stream has changed
			{
				currentStream = inStream;
				sData = null;
				sDataObjectCount = 0;
			}

			try
			{
				if (sData != null)
				{
					if (sDataObjectCount != sData.Count)
					{
						return GetCertificate();
					}

					sData = null;
					sDataObjectCount = 0;
					return null;
				}

				PushbackStream pis = new PushbackStream(inStream);
				int tag = pis.ReadByte();

				if (tag < 0)
					return null;

				pis.Unread(tag);

				if (tag != 0x30)  // assume ascii PEM encoded.
				{
					return ReadPemCertificate(pis);
				}

				return ReadDerCertificate(new Asn1InputStream(pis));
			}
			catch (Exception e)
			{
				throw new CertificateException(e.ToString());
			}
		}

		/**
		 * Returns a (possibly empty) collection view of the certificates
		 * read from the given input stream inStream.
		 */
		public ICollection ReadAttrCerts(
			Stream inStream)
		{
			IX509AttributeCertificate attrCert;
            IList attrCerts = Platform.CreateArrayList();

			while ((attrCert = ReadAttrCert(inStream)) != null)
			{
				attrCerts.Add(attrCert);
			}

			return attrCerts;
		}
	}
}