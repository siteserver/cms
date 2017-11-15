using System;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
    /**
     * Class implementing the NAF (Non-Adjacent Form) multiplication algorithm (right-to-left) using
     * mixed coordinates.
     */
    public class MixedNafR2LMultiplier 
        : AbstractECMultiplier
    {
        protected readonly int additionCoord, doublingCoord;

        /**
         * By default, addition will be done in Jacobian coordinates, and doubling will be done in
         * Modified Jacobian coordinates (independent of the original coordinate system of each point).
         */
        public MixedNafR2LMultiplier()
            : this(ECCurve.COORD_JACOBIAN, ECCurve.COORD_JACOBIAN_MODIFIED)
        {
        }

        public MixedNafR2LMultiplier(int additionCoord, int doublingCoord)
        {
            this.additionCoord = additionCoord;
            this.doublingCoord = doublingCoord;
        }

        protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
        {
            ECCurve curveOrig = p.Curve;

            ECCurve curveAdd = ConfigureCurve(curveOrig, additionCoord);
            ECCurve curveDouble = ConfigureCurve(curveOrig, doublingCoord);

            int[] naf = WNafUtilities.GenerateCompactNaf(k);

            ECPoint Ra = curveAdd.Infinity;
            ECPoint Td = curveDouble.ImportPoint(p);

            int zeroes = 0;
            for (int i = 0; i < naf.Length; ++i)
            {
                int ni = naf[i];
                int digit = ni >> 16;
                zeroes += ni & 0xFFFF;

                Td = Td.TimesPow2(zeroes);

                ECPoint Tj = curveAdd.ImportPoint(Td);
                if (digit < 0)
                {
                    Tj = Tj.Negate();
                }

                Ra = Ra.Add(Tj);

                zeroes = 1;
            }

            return curveOrig.ImportPoint(Ra);
        }

        protected virtual ECCurve ConfigureCurve(ECCurve c, int coord)
        {
            if (c.CoordinateSystem == coord)
                return c;

            if (!c.SupportsCoordinateSystem(coord))
                throw new ArgumentException("Coordinate system " + coord + " not supported by this curve", "coord");

            return c.Configure().SetCoordinateSystem(coord).Create();
        }
    }
}
