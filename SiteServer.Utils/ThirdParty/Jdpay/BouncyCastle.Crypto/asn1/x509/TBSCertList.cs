using System;
using System.Collections;

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
	public class CrlEntry
		: Asn1Encodable
	{
		internal Asn1Sequence	seq;
		internal DerInteger		userCertificate;
		internal Time			revocationDate;
		internal X509Extensions	crlEntryExtensions;

		public CrlEntry(
			Asn1Sequence seq)
		{
			if (seq.Count < 2 || seq.Count > 3)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}

			this.seq = seq;

			userCertificate = DerInteger.GetInstance(seq[0]);
			revocationDate = Time.GetInstance(seq[1]);
		}

		public DerInteger UserCertificate
		{
			get { return userCertificate; }
		}

		public Time RevocationDate
		{
			get { return revocationDate; }
		}

		public X509Extensions Extensions
		{
			get
			{
				if (crlEntryExtensions == null && seq.Count == 3)
				{
					crlEntryExtensions = X509Extensions.GetInstance(seq[2]);
				}

				return crlEntryExtensions;
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			return seq;
		}
	}

	/**
     * PKIX RFC-2459 - TbsCertList object.
     * <pre>
     * TbsCertList  ::=  Sequence  {
     *      version                 Version OPTIONAL,
     *                                   -- if present, shall be v2
     *      signature               AlgorithmIdentifier,
     *      issuer                  Name,
     *      thisUpdate              Time,
     *      nextUpdate              Time OPTIONAL,
     *      revokedCertificates     Sequence OF Sequence  {
     *           userCertificate         CertificateSerialNumber,
     *           revocationDate          Time,
     *           crlEntryExtensions      Extensions OPTIONAL
     *                                         -- if present, shall be v2
     *                                }  OPTIONAL,
     *      crlExtensions           [0]  EXPLICIT Extensions OPTIONAL
     *                                         -- if present, shall be v2
     *                                }
     * </pre>
     */
    public class TbsCertificateList
        : Asn1Encodable
    {
		private class RevokedCertificatesEnumeration
			: IEnumerable
		{
			private readonly IEnumerable en;

			internal RevokedCertificatesEnumeration(
				IEnumerable en)
			{
				this.en = en;
			}

			public IEnumerator GetEnumerator()
			{
				return new RevokedCertificatesEnumerator(en.GetEnumerator());
			}

			private class RevokedCertificatesEnumerator
				: IEnumerator
			{
				private readonly IEnumerator e;

				internal RevokedCertificatesEnumerator(
					IEnumerator e)
				{
					this.e = e;
				}

				public bool MoveNext()
				{
					return e.MoveNext();
				}

				public void Reset()
				{
					e.Reset();
				}

				public object Current
				{
					get { return new CrlEntry(Asn1Sequence.GetInstance(e.Current)); }
				}
			}
		}

		internal Asn1Sequence			seq;
		internal DerInteger				version;
        internal AlgorithmIdentifier	signature;
        internal X509Name				issuer;
        internal Time					thisUpdate;
        internal Time					nextUpdate;
		internal Asn1Sequence			revokedCertificates;
		internal X509Extensions			crlExtensions;

		public static TbsCertificateList GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
        }

		public static TbsCertificateList GetInstance(
            object obj)
        {
            TbsCertificateList list = obj as TbsCertificateList;

			if (obj == null || list != null)
            {
                return list;
            }

			if (obj is Asn1Sequence)
            {
                return new TbsCertificateList((Asn1Sequence) obj);
            }

            throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
        }

		internal TbsCertificateList(
            Asn1Sequence seq)
        {
			if (seq.Count < 3 || seq.Count > 7)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}

			int seqPos = 0;

			this.seq = seq;

			if (seq[seqPos] is DerInteger)
            {
				version = DerInteger.GetInstance(seq[seqPos++]);
			}
            else
            {
                version = new DerInteger(0);
            }

			signature = AlgorithmIdentifier.GetInstance(seq[seqPos++]);
            issuer = X509Name.GetInstance(seq[seqPos++]);
            thisUpdate = Time.GetInstance(seq[seqPos++]);

			if (seqPos < seq.Count
                && (seq[seqPos] is DerUtcTime
                   || seq[seqPos] is DerGeneralizedTime
                   || seq[seqPos] is Time))
            {
                nextUpdate = Time.GetInstance(seq[seqPos++]);
            }

			if (seqPos < seq.Count
                && !(seq[seqPos] is DerTaggedObject))
            {
				revokedCertificates = Asn1Sequence.GetInstance(seq[seqPos++]);
			}

			if (seqPos < seq.Count
                && seq[seqPos] is DerTaggedObject)
            {
				crlExtensions = X509Extensions.GetInstance(seq[seqPos]);
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

		public AlgorithmIdentifier Signature
        {
            get { return signature; }
        }

		public X509Name Issuer
        {
            get { return issuer; }
        }

		public Time ThisUpdate
        {
            get { return thisUpdate; }
        }

		public Time NextUpdate
        {
            get { return nextUpdate; }
        }

		public CrlEntry[] GetRevokedCertificates()
        {
			if (revokedCertificates == null)
			{
				return new CrlEntry[0];
			}

			CrlEntry[] entries = new CrlEntry[revokedCertificates.Count];

			for (int i = 0; i < entries.Length; i++)
			{
				entries[i] = new CrlEntry(Asn1Sequence.GetInstance(revokedCertificates[i]));
			}

			return entries;
		}

		public IEnumerable GetRevokedCertificateEnumeration()
		{
			if (revokedCertificates == null)
			{
				return EmptyEnumerable.Instance;
			}

			return new RevokedCertificatesEnumeration(revokedCertificates);
		}

		public X509Extensions Extensions
        {
            get { return crlExtensions; }
        }

		public override Asn1Object ToAsn1Object()
        {
            return seq;
        }
    }
}
