namespace Org.BouncyCastle.Math.EC.Multiplier
{
    public class DoubleAddMultiplier
        : AbstractECMultiplier
    {
        /**
         * Joye's double-add algorithm.
         */
        protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
        {
            ECPoint[] R = new ECPoint[]{ p.Curve.Infinity, p };

            int n = k.BitLength;
            for (int i = 0; i < n; ++i)
            {
                int b = k.TestBit(i) ? 1 : 0;
                int bp = 1 - b;
                R[bp] = R[bp].TwicePlus(R[b]);
            }

            return R[0];
        }
    }
}
