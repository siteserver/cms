using System;

using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecT409K1Curve
        : AbstractF2mCurve
    {
        private const int SecT409K1_DEFAULT_COORDS = COORD_LAMBDA_PROJECTIVE;

        protected readonly SecT409K1Point m_infinity;

        public SecT409K1Curve()
            : base(409, 87, 0, 0)
        {
            this.m_infinity = new SecT409K1Point(this, null, null);

            this.m_a = FromBigInteger(BigInteger.Zero);
            this.m_b = FromBigInteger(BigInteger.One);
            this.m_order = new BigInteger(1, Hex.Decode("7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE5F83B2D4EA20400EC4557D5ED3E3E7CA5B4B5C83B8E01E5FCF"));
            this.m_cofactor = BigInteger.ValueOf(4);

            this.m_coord = SecT409K1_DEFAULT_COORDS;
        }

        protected override ECCurve CloneCurve()
        {
            return new SecT409K1Curve();
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

        protected override ECMultiplier CreateDefaultMultiplier()
        {
            return new WTauNafMultiplier();
        }

        public override ECPoint Infinity
        {
            get { return m_infinity; }
        }

        public override int FieldSize
        {
            get { return 409; }
        }

        public override ECFieldElement FromBigInteger(BigInteger x)
        {
            return new SecT409FieldElement(x);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
        {
            return new SecT409K1Point(this, x, y, withCompression);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
        {
            return new SecT409K1Point(this, x, y, zs, withCompression);
        }

        public override bool IsKoblitz
        {
            get { return true; }
        }

        public virtual int M
        {
            get { return 409; }
        }

        public virtual bool IsTrinomial
        {
            get { return true; }
        }

        public virtual int K1
        {
            get { return 87; }
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
