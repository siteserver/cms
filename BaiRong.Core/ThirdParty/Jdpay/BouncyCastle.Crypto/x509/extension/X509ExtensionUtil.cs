using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.X509.Extension
{
	public class X509ExtensionUtilities
	{
		public static Asn1Object FromExtensionValue(
			Asn1OctetString extensionValue)
		{
			return Asn1Object.FromByteArray(extensionValue.GetOctets());
		}

		public static ICollection GetIssuerAlternativeNames(
			X509Certificate cert)
		{
			Asn1OctetString extVal = cert.GetExtensionValue(X509Extensions.IssuerAlternativeName);

			return GetAlternativeName(extVal);
		}

		public static ICollection GetSubjectAlternativeNames(
			X509Certificate cert)
		{
			Asn1OctetString extVal = cert.GetExtensionValue(X509Extensions.SubjectAlternativeName);

			return GetAlternativeName(extVal);
		}

		private static ICollection GetAlternativeName(
			Asn1OctetString extVal)
		{
			IList temp = Platform.CreateArrayList();

			if (extVal != null)
			{
				try
				{
					Asn1Sequence seq = DerSequence.GetInstance(FromExtensionValue(extVal));

					foreach (Asn1Encodable primName in seq)
					{
                        IList list = Platform.CreateArrayList();
                        GeneralName genName = GeneralName.GetInstance(primName);

						list.Add(genName.TagNo);

						switch (genName.TagNo)
						{
							case GeneralName.EdiPartyName:
							case GeneralName.X400Address:
							case GeneralName.OtherName:
								list.Add(genName.Name.ToAsn1Object());
								break;
							case GeneralName.DirectoryName:
								list.Add(X509Name.GetInstance(genName.Name).ToString());
								break;
							case GeneralName.DnsName:
							case GeneralName.Rfc822Name:
							case GeneralName.UniformResourceIdentifier:
								list.Add(((IAsn1String)genName.Name).GetString());
								break;
							case GeneralName.RegisteredID:
								list.Add(DerObjectIdentifier.GetInstance(genName.Name).Id);
								break;
							case GeneralName.IPAddress:
								list.Add(DerOctetString.GetInstance(genName.Name).GetOctets());
								break;
							default:
								throw new IOException("Bad tag number: " + genName.TagNo);
						}

						temp.Add(list);
					}
				}
				catch (Exception e)
				{
					throw new CertificateParsingException(e.Message);
				}
			}

			return temp;
		}
	}
}
