using System;
using System.Text;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509
{
    public class CrlDistPoint
        : Asn1Encodable
    {
        internal readonly Asn1Sequence seq;

		public static CrlDistPoint GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
        }

		public static CrlDistPoint GetInstance(
            object obj)
        {
            if (obj is CrlDistPoint || obj == null)
            {
                return (CrlDistPoint) obj;
            }

			if (obj is Asn1Sequence)
            {
                return new CrlDistPoint((Asn1Sequence) obj);
            }

            throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		private CrlDistPoint(
            Asn1Sequence seq)
        {
            this.seq = seq;
        }

		public CrlDistPoint(
            DistributionPoint[] points)
        {
			seq = new DerSequence(points);
        }

		/**
         * Return the distribution points making up the sequence.
         *
         * @return DistributionPoint[]
         */
        public DistributionPoint[] GetDistributionPoints()
        {
            DistributionPoint[] dp = new DistributionPoint[seq.Count];

			for (int i = 0; i != seq.Count; ++i)
            {
                dp[i] = DistributionPoint.GetInstance(seq[i]);
            }

			return dp;
        }

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         * CrlDistPoint ::= Sequence SIZE {1..MAX} OF DistributionPoint
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            return seq;
        }

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
			string sep = Platform.NewLine;

			buf.Append("CRLDistPoint:");
			buf.Append(sep);
			DistributionPoint[] dp = GetDistributionPoints();
			for (int i = 0; i != dp.Length; i++)
			{
				buf.Append("    ");
				buf.Append(dp[i]);
				buf.Append(sep);
			}
			return buf.ToString();
		}
	}
}
