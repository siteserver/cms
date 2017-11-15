using System;

using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecT571K1Curve
        : AbstractF2mCurve
    {
        private const int SecT571K1_DEFAULT_COORDS = COORD_LAMBDA_PROJECTIVE;

        protected readonly SecT571K1Point m_infinity;

        public SecT571K1Curve()
            : base(571, 2, 5, 10)
        {
            this.m_infinity = new SecT571K1Point(this, null, null);

            this.m_a = FromBigInteger(BigInteger.Zero);
            this.m_b = FromBigInteger(BigInteger.One);
            this.m_order = new BigInteger(1, Hex.Decode("020000000000000000000000000000000000000000000000000000000000000000000000131850E1F19A63E4B391A8DB917F4138B630D84BE5D639381E91DEB45CFE778F637C1001"));
            this.m_cofactor = BigInteger.ValueOf(4);

            this.m_coord = SecT571K1_DEFAULT_COORDS;
        }

        protected override ECCurve CloneCurve()
        {
            return new SecT571K1Curve();
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
            get { return 571; }
        }

        public override ECFieldElement FromBigInteger(BigInteger x)
        {
            return new SecT571FieldElement(x);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
        {
            return new SecT571K1Point(this, x, y, withCompression);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
        {
            return new SecT571K1Point(this, x, y, zs, withCompression);
        }

        public override bool IsKoblitz
        {
            get { return true; }
        }

        public virtual int M
        {
            get { return 571; }
        }

        public virtual bool IsTrinomial
        {
            get { return false; }
        }

        public virtual int K1
        {
            get { return 2; }
        }

        public virtual int K2
        {
            get { return 5; }
        }

        public virtual int K3
        {
            get { return 10; }
        }
    }
}
