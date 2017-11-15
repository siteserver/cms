using System;

using Org.BouncyCastle.Asn1.Pkcs;

namespace Org.BouncyCastle.Asn1.X509
{
    /**
     * The TbsCertificate object.
     * <pre>
     * TbsCertificate ::= Sequence {
     *      version          [ 0 ]  Version DEFAULT v1(0),
     *      serialNumber            CertificateSerialNumber,
     *      signature               AlgorithmIdentifier,
     *      issuer                  Name,
     *      validity                Validity,
     *      subject                 Name,
     *      subjectPublicKeyInfo    SubjectPublicKeyInfo,
     *      issuerUniqueID    [ 1 ] IMPLICIT UniqueIdentifier OPTIONAL,
     *      subjectUniqueID   [ 2 ] IMPLICIT UniqueIdentifier OPTIONAL,
     *      extensions        [ 3 ] Extensions OPTIONAL
     *      }
     * </pre>
     * <p>
     * Note: issuerUniqueID and subjectUniqueID are both deprecated by the IETF. This class
     * will parse them, but you really shouldn't be creating new ones.</p>
     */
	public class TbsCertificateStructure
		: Asn1Encodable
	{
		internal Asn1Sequence            seq;
		internal DerInteger              version;
		internal DerInteger              serialNumber;
		internal AlgorithmIdentifier     signature;
		internal X509Name                issuer;
		internal Time                    startDate, endDate;
		internal X509Name                subject;
		internal SubjectPublicKeyInfo    subjectPublicKeyInfo;
		internal DerBitString            issuerUniqueID;
		internal DerBitString            subjectUniqueID;
		internal X509Extensions          extensions;

		public static TbsCertificateStructure GetInstance(
			Asn1TaggedObject	obj,
			bool				explicitly)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static TbsCertificateStructure GetInstance(
			object obj)
		{
			if (obj is TbsCertificateStructure)
				return (TbsCertificateStructure) obj;

			if (obj != null)
				return new TbsCertificateStructure(Asn1Sequence.GetInstance(obj));

			return null;
		}

		internal TbsCertificateStructure(
			Asn1Sequence seq)
		{
			int seqStart = 0;

			this.seq = seq;

			//
			// some certficates don't include a version number - we assume v1
			//
			if (seq[0] is DerTaggedObject)
			{
				version = DerInteger.GetInstance((Asn1TaggedObject)seq[0], true);
			}
			else
			{
				seqStart = -1;          // field 0 is missing!
				version = new DerInteger(0);
			}

			serialNumber = DerInteger.GetInstance(seq[seqStart + 1]);

			signature = AlgorithmIdentifier.GetInstance(seq[seqStart + 2]);
			issuer = X509Name.GetInstance(seq[seqStart + 3]);

			//
			// before and after dates
			//
			Asn1Sequence  dates = (Asn1Sequence)seq[seqStart + 4];

			startDate = Time.GetInstance(dates[0]);
			endDate = Time.GetInstance(dates[1]);

			subject = X509Name.GetInstance(seq[seqStart + 5]);

			//
			// public key info.
			//
			subjectPublicKeyInfo = SubjectPublicKeyInfo.GetInstance(seq[seqStart + 6]);

			for (int extras = seq.Count - (seqStart + 6) - 1; extras > 0; extras--)
			{
				DerTaggedObject extra = (DerTaggedObject) seq[seqStart + 6 + extras];

				switch (extra.TagNo)
				{
					case 1:
						issuerUniqueID = DerBitString.GetInstance(extra, false);
						break;
					case 2:
						subjectUniqueID = DerBitString.GetInstance(extra, false);
						break;
					case 3:
						extensions = X509Extensions.GetInstance(extra);
						break;
				}
			}
		}

		public int Version
		{
			get { return version.Value.IntValue + 1; }
		}

		public DerInteger VersionNumber
		{
			get { return version; }
		}

		public DerInteger SerialNumber
		{
			get { return serialNumber; }
		}

		public AlgorithmIdentifier Signature
		{
			get { return signature; }
		}

		public X509Name Issuer
		{
			get { return issuer; }
		}

		public Time StartDate
		{
			get { return startDate; }
		}

		public Time EndDate
		{
			get { return endDate; }
		}

		public X509Name Subject
		{
			get { return subject; }
		}

		public SubjectPublicKeyInfo SubjectPublicKeyInfo
		{
			get { return subjectPublicKeyInfo; }
		}

		public DerBitString IssuerUniqueID
		{
			get { return issuerUniqueID; }
        }

		public DerBitString SubjectUniqueID
        {
			get { return subjectUniqueID; }
        }

		public X509Extensions Extensions
        {
			get { return extensions; }
        }

		public override Asn1Object ToAsn1Object()
        {
            return seq;
        }
    }
}
