using System;

using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecT163R2Curve
        : AbstractF2mCurve
    {
        private const int SecT163R2_DEFAULT_COORDS = COORD_LAMBDA_PROJECTIVE;

        protected readonly SecT163R2Point m_infinity;

        public SecT163R2Curve()
            : base(163, 3, 6, 7)
        {
            this.m_infinity = new SecT163R2Point(this, null, null);

            this.m_a = FromBigInteger(BigInteger.One);
            this.m_b = FromBigInteger(new BigInteger(1, Hex.Decode("020A601907B8C953CA1481EB10512F78744A3205FD")));
            this.m_order = new BigInteger(1, Hex.Decode("040000000000000000000292FE77E70C12A4234C33"));
            this.m_cofactor = BigInteger.Two;

            this.m_coord = SecT163R2_DEFAULT_COORDS;
        }

        protected override ECCurve CloneCurve()
        {
            return new SecT163R2Curve();
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
            get { return 163; }
        }

        public override ECFieldElement FromBigInteger(BigInteger x)
        {
            return new SecT163FieldElement(x);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
        {
            return new SecT163R2Point(this, x, y, withCompression);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
        {
            return new SecT163R2Point(this, x, y, zs, withCompression);
        }

        public override bool IsKoblitz
        {
            get { return false; }
        }

        public virtual int M
        {
            get { return 163; }
        }

        public virtual bool IsTrinomial
        {
            get { return false; }
        }

        public virtual int K1
        {
            get { return 3; }
        }

        public virtual int K2
        {
            get { return 6; }
        }

        public virtual int K3
        {
            get { return 7; }
        }
    }
}
