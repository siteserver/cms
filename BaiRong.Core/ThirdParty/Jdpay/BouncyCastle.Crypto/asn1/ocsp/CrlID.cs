using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.Ocsp
{
    public class CrlID
        : Asn1Encodable
    {
        private readonly DerIA5String		crlUrl;
        private readonly DerInteger			crlNum;
        private readonly DerGeneralizedTime	crlTime;

		// TODO Add GetInstance method(s) and amke this private?
		public CrlID(
            Asn1Sequence seq)
        {
			foreach (Asn1TaggedObject o in seq)
			{
				switch (o.TagNo)
                {
                case 0:
                    crlUrl = DerIA5String.GetInstance(o, true);
                    break;
                case 1:
                    crlNum = DerInteger.GetInstance(o, true);
                    break;
                case 2:
                    crlTime = DerGeneralizedTime.GetInstance(o, true);
                    break;
                default:
                    throw new ArgumentException("unknown tag number: " + o.TagNo);
                }
            }
        }

		public DerIA5String CrlUrl
		{
			get { return crlUrl; }
		}

		public DerInteger CrlNum
		{
			get { return crlNum; }
		}

		public DerGeneralizedTime CrlTime
		{
			get { return crlTime; }
		}

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         * CrlID ::= Sequence {
         *     crlUrl               [0]     EXPLICIT IA5String OPTIONAL,
         *     crlNum               [1]     EXPLICIT Integer OPTIONAL,
         *     crlTime              [2]     EXPLICIT GeneralizedTime OPTIONAL }
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector();

			if (crlUrl != null)
            {
                v.Add(new DerTaggedObject(true, 0, crlUrl));
            }

			if (crlNum != null)
            {
                v.Add(new DerTaggedObject(true, 1, crlNum));
            }

			if (crlTime != null)
            {
                v.Add(new DerTaggedObject(true, 2, crlTime));
            }

			return new DerSequence(v);
        }
    }
}
