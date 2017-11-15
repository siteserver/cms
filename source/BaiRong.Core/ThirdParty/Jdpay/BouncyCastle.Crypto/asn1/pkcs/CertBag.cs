using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
    public class CertBag
        : Asn1Encodable
    {
//		private readonly Asn1Sequence seq;
        private readonly DerObjectIdentifier certID;
        private readonly Asn1Object certValue;

		public CertBag(
            Asn1Sequence seq)
        {
			if (seq.Count != 2)
				throw new ArgumentException("Wrong number of elements in sequence", "seq");

//			this.seq = seq;
            this.certID = DerObjectIdentifier.GetInstance(seq[0]);
            this.certValue = DerTaggedObject.GetInstance(seq[1]).GetObject();
        }

		public CertBag(
            DerObjectIdentifier	certID,
            Asn1Object			certValue)
        {
            this.certID = certID;
            this.certValue = certValue;
        }

		public DerObjectIdentifier CertID
		{
			get { return certID; }
		}

		public Asn1Object CertValue
		{
			get { return certValue; }
		}

		public override Asn1Object ToAsn1Object()
        {
			return new DerSequence(certID, new DerTaggedObject(0, certValue));
        }
    }
}
