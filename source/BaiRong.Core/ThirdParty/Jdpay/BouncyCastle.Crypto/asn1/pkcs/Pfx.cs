using System;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Asn1.Pkcs
{
    /**
     * the infamous Pfx from Pkcs12
     */
    public class Pfx
        : Asn1Encodable
    {
        private ContentInfo	contentInfo;
        private MacData		macData;

		public Pfx(
            Asn1Sequence seq)
        {
            BigInteger version = ((DerInteger) seq[0]).Value;
            if (version.IntValue != 3)
            {
                throw new ArgumentException("wrong version for PFX PDU");
            }

			contentInfo = ContentInfo.GetInstance(seq[1]);

			if (seq.Count == 3)
            {
                macData = MacData.GetInstance(seq[2]);
            }
        }

		public Pfx(
            ContentInfo	contentInfo,
            MacData		macData)
        {
            this.contentInfo = contentInfo;
            this.macData = macData;
        }

		public ContentInfo AuthSafe
		{
			get { return contentInfo; }
		}

		public MacData MacData
		{
			get { return macData; }
		}

		public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(
				new DerInteger(3), contentInfo);

			if (macData != null)
            {
                v.Add(macData);
            }

			return new BerSequence(v);
        }
    }
}
