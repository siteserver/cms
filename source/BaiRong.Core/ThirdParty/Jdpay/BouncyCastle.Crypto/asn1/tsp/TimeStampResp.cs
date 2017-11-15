using System;

using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Tsp
{
	public class TimeStampResp
		: Asn1Encodable
	{
		private readonly PkiStatusInfo	pkiStatusInfo;
		private readonly ContentInfo	timeStampToken;

		public static TimeStampResp GetInstance(
			object o)
		{
			if (o == null || o is TimeStampResp)
			{
				return (TimeStampResp) o;
			}

			if (o is Asn1Sequence)
			{
				return new TimeStampResp((Asn1Sequence) o);
			}

			throw new ArgumentException(
				"Unknown object in 'TimeStampResp' factory: " + Platform.GetTypeName(o));
		}

		private TimeStampResp(
			Asn1Sequence seq)
		{
			this.pkiStatusInfo = PkiStatusInfo.GetInstance(seq[0]);

			if (seq.Count > 1)
			{
				this.timeStampToken = ContentInfo.GetInstance(seq[1]);
			}
		}

		public TimeStampResp(
			PkiStatusInfo	pkiStatusInfo,
			ContentInfo		timeStampToken)
		{
			this.pkiStatusInfo = pkiStatusInfo;
			this.timeStampToken = timeStampToken;
		}

		public PkiStatusInfo Status
		{
			get { return pkiStatusInfo; }
		}

		public ContentInfo TimeStampToken
		{
			get { return timeStampToken; }
		}

		/**
		 * <pre>
		 * TimeStampResp ::= SEQUENCE  {
		 *   status                  PkiStatusInfo,
		 *   timeStampToken          TimeStampToken     OPTIONAL  }
		 * </pre>
		 */
		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector v = new Asn1EncodableVector(pkiStatusInfo);

			if (timeStampToken != null)
			{
				v.Add(timeStampToken);
			}

			return new DerSequence(v);
		}
	}
}
