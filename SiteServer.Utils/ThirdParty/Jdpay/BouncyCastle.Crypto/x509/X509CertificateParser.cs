using System;
using System.Collections;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.X509
{
	/**
	 * class for dealing with X509 certificates.
	 * <p>
	 * At the moment this will deal with "-----BEGIN CERTIFICATE-----" to "-----END CERTIFICATE-----"
	 * base 64 encoded certs, as well as the BER binaries of certificates and some classes of PKCS#7
	 * objects.</p>
	 */
	public class X509CertificateParser
	{
		private static readonly PemParser PemCertParser = new PemParser("CERTIFICATE");

		private Asn1Set	sData;
		private int		sDataObjectCount;
		private Stream	currentStream;

		private X509Certificate ReadDerCertificate(
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

			return CreateX509Certificate(X509CertificateStructure.GetInstance(seq));
		}

		private X509Certificate GetCertificate()
		{
			if (sData != null)
			{
				while (sDataObjectCount < sData.Count)
				{
					object obj = sData[sDataObjectCount++];

					if (obj is Asn1Sequence)
					{
						return CreateX509Certificate(
							X509CertificateStructure.GetInstance(obj));
					}
				}
			}

			return null;
		}

		private X509Certificate ReadPemCertificate(
			Stream inStream)
		{
			Asn1Sequence seq = PemCertParser.ReadPemObject(inStream);

			return seq == null
				?	null
				:	CreateX509Certificate(X509CertificateStructure.GetInstance(seq));
		}

		protected virtual X509Certificate CreateX509Certificate(
			X509CertificateStructure c)
		{
			return new X509Certificate(c);
		}

		/// <summary>
		/// Create loading data from byte array.
		/// </summary>
		/// <param name="input"></param>
		public X509Certificate ReadCertificate(
			byte[] input)
		{
			return ReadCertificate(new MemoryStream(input, false));
		}

		/// <summary>
		/// Create loading data from byte array.
		/// </summary>
		/// <param name="input"></param>
		public ICollection ReadCertificates(
			byte[] input)
		{
			return ReadCertificates(new MemoryStream(input, false));
		}

		/**
		 * Generates a certificate object and initializes it with the data
		 * read from the input stream inStream.
		 */
		public X509Certificate ReadCertificate(
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
				throw new CertificateException("Failed to read certificate", e);
			}
		}

		/**
		 * Returns a (possibly empty) collection view of the certificates
		 * read from the given input stream inStream.
		 */
		public ICollection ReadCertificates(
			Stream inStream)
		{
			X509Certificate cert;
            IList certs = Platform.CreateArrayList();

			while ((cert = ReadCertificate(inStream)) != null)
			{
				certs.Add(cert);
			}

			return certs;
		}
	}
}
