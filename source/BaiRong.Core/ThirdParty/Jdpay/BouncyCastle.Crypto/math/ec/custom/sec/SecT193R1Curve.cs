using System;

using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecT193R1Curve
        : AbstractF2mCurve
    {
        private const int SecT193R1_DEFAULT_COORDS = COORD_LAMBDA_PROJECTIVE;

        protected readonly SecT193R1Point m_infinity;

        public SecT193R1Curve()
            : base(193, 15, 0, 0)
        {
            this.m_infinity = new SecT193R1Point(this, null, null);

            this.m_a = FromBigInteger(new BigInteger(1, Hex.Decode("0017858FEB7A98975169E171F77B4087DE098AC8A911DF7B01")));
            this.m_b = FromBigInteger(new BigInteger(1, Hex.Decode("00FDFB49BFE6C3A89FACADAA7A1E5BBC7CC1C2E5D831478814")));
            this.m_order = new BigInteger(1, Hex.Decode("01000000000000000000000000C7F34A778F443ACC920EBA49"));
            this.m_cofactor = BigInteger.Two;

            this.m_coord = SecT193R1_DEFAULT_COORDS;
        }

        protected override ECCurve CloneCurve()
        {
            return new SecT193R1Curve();
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
            get { return 193; }
        }

        public override ECFieldElement FromBigInteger(BigInteger x)
        {
            return new SecT193FieldElement(x);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
        {
            return new SecT193R1Point(this, x, y, withCompression);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
        {
            return new SecT193R1Point(this, x, y, zs, withCompression);
        }

        public override bool IsKoblitz
        {
            get { return false; }
        }

        public virtual int M
        {
            get { return 193; }
        }

        public virtual bool IsTrinomial
        {
            get { return true; }
        }

        public virtual int K1
        {
            get { return 15; }
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
