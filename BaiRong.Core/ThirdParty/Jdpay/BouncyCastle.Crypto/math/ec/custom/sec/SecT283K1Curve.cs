using System;

using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecT283K1Curve
        : AbstractF2mCurve
    {
        private const int SecT283K1_DEFAULT_COORDS = COORD_LAMBDA_PROJECTIVE;

        protected readonly SecT283K1Point m_infinity;

        public SecT283K1Curve()
            : base(283, 5, 7, 12)
        {
            this.m_infinity = new SecT283K1Point(this, null, null);

            this.m_a = FromBigInteger(BigInteger.Zero);
            this.m_b = FromBigInteger(BigInteger.One);
            this.m_order = new BigInteger(1, Hex.Decode("01FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE9AE2ED07577265DFF7F94451E061E163C61"));
            this.m_cofactor = BigInteger.ValueOf(4);

            this.m_coord = SecT283K1_DEFAULT_COORDS;
        }

        protected override ECCurve CloneCurve()
        {
            return new SecT283K1Curve();
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
            get { return 283; }
        }

        public override ECFieldElement FromBigInteger(BigInteger x)
        {
            return new SecT283FieldElement(x);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
        {
            return new SecT283K1Point(this, x, y, withCompression);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
        {
            return new SecT283K1Point(this, x, y, zs, withCompression);
        }

        public override bool IsKoblitz
        {
            get { return true; }
        }

        public virtual int M
        {
            get { return 283; }
        }

        public virtual bool IsTrinomial
        {
            get { return false; }
        }

        public virtual int K1
        {
            get { return 5; }
        }

        public virtual int K2
        {
            get { return 7; }
        }

        public virtual int K3
        {
            get { return 12; }
        }
    }
}
