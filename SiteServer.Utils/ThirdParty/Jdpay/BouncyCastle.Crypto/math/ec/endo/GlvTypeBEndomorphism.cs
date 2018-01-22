using System;

namespace Org.BouncyCastle.Math.EC.Endo
{
    public class GlvTypeBEndomorphism
        :   GlvEndomorphism
    {
        protected readonly ECCurve m_curve;
        protected readonly GlvTypeBParameters m_parameters;
        protected readonly ECPointMap m_pointMap;

        public GlvTypeBEndomorphism(ECCurve curve, GlvTypeBParameters parameters)
        {
            this.m_curve = curve;
            this.m_parameters = parameters;
            this.m_pointMap = new ScaleXPointMap(curve.FromBigInteger(parameters.Beta));
        }

        public virtual BigInteger[] DecomposeScalar(BigInteger k)
        {
            int bits = m_parameters.Bits;
            BigInteger b1 = CalculateB(k, m_parameters.G1, bits);
            BigInteger b2 = CalculateB(k, m_parameters.G2, bits);

            BigInteger[] v1 = m_parameters.V1, v2 = m_parameters.V2;
            BigInteger a = k.Subtract((b1.Multiply(v1[0])).Add(b2.Multiply(v2[0])));
            BigInteger b = (b1.Multiply(v1[1])).Add(b2.Multiply(v2[1])).Negate();

            return new BigInteger[]{ a, b };
        }

        public virtual ECPointMap PointMap
        {
            get { return m_pointMap; }
        }

        public virtual bool HasEfficientPointMap
        {
            get { return true; }
        }

        protected virtual BigInteger CalculateB(BigInteger k, BigInteger g, int t)
        {
            bool negative = (g.SignValue < 0);
            BigInteger b = k.Multiply(g.Abs());
            bool extra = b.TestBit(t - 1);
            b = b.ShiftRight(t);
            if (extra)
            {
                b = b.Add(BigInteger.One);
            }
            return negative ? b.Negate() : b;
        }
    }
}
