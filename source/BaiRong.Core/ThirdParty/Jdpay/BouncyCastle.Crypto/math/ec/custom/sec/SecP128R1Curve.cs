using System;

using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecP128R1Curve
        : AbstractFpCurve
    {
        public static readonly BigInteger q = new BigInteger(1,
            Hex.Decode("FFFFFFFDFFFFFFFFFFFFFFFFFFFFFFFF"));

        private const int SecP128R1_DEFAULT_COORDS = COORD_JACOBIAN;

        protected readonly SecP128R1Point m_infinity;

        public SecP128R1Curve()
            : base(q)
        {
            this.m_infinity = new SecP128R1Point(this, null, null);

            this.m_a = FromBigInteger(new BigInteger(1,
                Hex.Decode("FFFFFFFDFFFFFFFFFFFFFFFFFFFFFFFC")));
            this.m_b = FromBigInteger(new BigInteger(1,
                Hex.Decode("E87579C11079F43DD824993C2CEE5ED3")));
            this.m_order = new BigInteger(1, Hex.Decode("FFFFFFFE0000000075A30D1B9038A115"));
            this.m_cofactor = BigInteger.One;

            this.m_coord = SecP128R1_DEFAULT_COORDS;
        }

        protected override ECCurve CloneCurve()
        {
            return new SecP128R1Curve();
        }

        public override bool SupportsCoordinateSystem(int coord)
        {
            switch (coord)
            {
            case COORD_JACOBIAN:
                return true;
            default:
                return false;
            }
        }

        public virtual BigInteger Q
        {
            get { return q; }
        }

        public override ECPoint Infinity
        {
            get { return m_infinity; }
        }

        public override int FieldSize
        {
            get { return q.BitLength; }
        }

        public override ECFieldElement FromBigInteger(BigInteger x)
        {
            return new SecP128R1FieldElement(x);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
        {
            return new SecP128R1Point(this, x, y, withCompression);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
        {
            return new SecP128R1Point(this, x, y, zs, withCompression);
        }
    }
}
