using System;

using Org.BouncyCastle.Math.EC.Endo;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
    public class GlvMultiplier
        :   AbstractECMultiplier
    {
        protected readonly ECCurve curve;
        protected readonly GlvEndomorphism glvEndomorphism;

        public GlvMultiplier(ECCurve curve, GlvEndomorphism glvEndomorphism)
        {
            if (curve == null || curve.Order == null)
                throw new ArgumentException("Need curve with known group order", "curve");

            this.curve = curve;
            this.glvEndomorphism = glvEndomorphism;
        }

        protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
        {
            if (!curve.Equals(p.Curve))
                throw new InvalidOperationException();

            BigInteger n = p.Curve.Order;
            BigInteger[] ab = glvEndomorphism.DecomposeScalar(k.Mod(n));
            BigInteger a = ab[0], b = ab[1];

            ECPointMap pointMap = glvEndomorphism.PointMap;
            if (glvEndomorphism.HasEfficientPointMap)
            {
                return ECAlgorithms.ImplShamirsTrickWNaf(p, a, pointMap, b);
            }

            return ECAlgorithms.ImplShamirsTrickWNaf(p, a, pointMap.Map(p), b);
        }
    }
}
