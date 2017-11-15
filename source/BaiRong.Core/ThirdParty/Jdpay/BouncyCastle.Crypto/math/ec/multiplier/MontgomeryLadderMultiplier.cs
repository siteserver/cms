namespace Org.BouncyCastle.Math.EC.Multiplier
{
    public class MontgomeryLadderMultiplier 
        : AbstractECMultiplier
    {
        /**
         * Montgomery ladder.
         */
        protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
        {
            ECPoint[] R = new ECPoint[]{ p.Curve.Infinity, p };

            int n = k.BitLength;
            int i = n;
            while (--i >= 0)
            {
                int b = k.TestBit(i) ? 1 : 0;
                int bp = 1 - b;
                R[bp] = R[bp].Add(R[b]);
                R[b] = R[b].Twice();
            }
            return R[0];
        }
    }
}
