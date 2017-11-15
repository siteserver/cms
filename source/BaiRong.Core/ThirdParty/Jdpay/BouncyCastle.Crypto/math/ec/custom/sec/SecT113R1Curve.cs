using System;

using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecT113R1Curve
        : AbstractF2mCurve
    {
        private const int SecT113R1_DEFAULT_COORDS = COORD_LAMBDA_PROJECTIVE;

        protected readonly SecT113R1Point m_infinity;

        public SecT113R1Curve()
            : base(113, 9, 0, 0)
        {
            this.m_infinity = new SecT113R1Point(this, null, null);

            this.m_a = FromBigInteger(new BigInteger(1, Hex.Decode("003088250CA6E7C7FE649CE85820F7")));
            this.m_b = FromBigInteger(new BigInteger(1, Hex.Decode("00E8BEE4D3E2260744188BE0E9C723")));
            this.m_order = new BigInteger(1, Hex.Decode("0100000000000000D9CCEC8A39E56F"));
            this.m_cofactor = BigInteger.Two;

            this.m_coord = SecT113R1_DEFAULT_COORDS;
        }

        protected override ECCurve CloneCurve()
        {
            return new SecT113R1Curve();
        }

        public override bool SupportsCoordinateSystem(int coord)
        {
            switch (coord)
            {
            case COORD_LAMBDA_PROJECTIVE:
                return true;
            default:
                return false;
            }
        }

        public override ECPoint Infinity
        {
            get { return m_infinity; }
        }

        public override int FieldSize
        {
            get { return 113; }
        }

        public override ECFieldElement FromBigInteger(BigInteger x)
        {
            return new SecT113FieldElement(x);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
        {
            return new SecT113R1Point(this, x, y, withCompression);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
        {
            return new SecT113R1Point(this, x, y, zs, withCompression);
        }

        public override bool IsKoblitz
        {
            get { return false; }
        }

        public virtual int M
        {
            get { return 113; }
        }

        public virtual bool IsTrinomial
        {
            get { return true; }
        }

        public virtual int K1
        {
            get { return 9; }
        }

        public virtual int K2
        {
            get { return 0; }
        }

        public virtual int K3
        {
            get { return 0; }
        }
    }
}
