using System;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class Evidence
		: Asn1Encodable, IAsn1Choice
	{
		private TimeStampTokenEvidence tstEvidence;

		public Evidence(TimeStampTokenEvidence tstEvidence)
		{
			this.tstEvidence = tstEvidence;
		}

		private Evidence(Asn1TaggedObject tagged)
		{
			if (tagged.TagNo == 0)
			{
				this.tstEvidence = TimeStampTokenEvidence.GetInstance(tagged, false);
			}
		}

		public static Evidence GetInstance(object obj)
		{
			if (obj is Evidence)
				return (Evidence)obj;

			if (obj is Asn1TaggedObject)
				return new Evidence(Asn1TaggedObject.GetInstance(obj));

			throw new ArgumentException("Unknown object in GetInstance: " + Platform.GetTypeName(obj), "obj");
		}

		public virtual TimeStampTokenEvidence TstEvidence
		{
			get { return tstEvidence; }
		}

		public override Asn1Object ToAsn1Object()
		{
			if (tstEvidence != null)
				return new DerTaggedObject(false, 0, tstEvidence);

			return null;
		}
	}
}
