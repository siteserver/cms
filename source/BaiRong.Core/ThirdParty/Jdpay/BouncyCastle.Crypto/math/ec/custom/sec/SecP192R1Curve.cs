using System;

using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecP192R1Curve
        : AbstractFpCurve
    {
        public static readonly BigInteger q = new BigInteger(1,
            Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFF"));

        private const int SecP192R1_DEFAULT_COORDS = COORD_JACOBIAN;

        protected readonly SecP192R1Point m_infinity;

        public SecP192R1Curve()
            : base(q)
        {
            this.m_infinity = new SecP192R1Point(this, null, null);

            this.m_a = FromBigInteger(new BigInteger(1,
                Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFC")));
            this.m_b = FromBigInteger(new BigInteger(1,
                Hex.Decode("64210519E59C80E70FA7E9AB72243049FEB8DEECC146B9B1")));
            this.m_order = new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFF99DEF836146BC9B1B4D22831"));
            this.m_cofactor = BigInteger.One;

            this.m_coord = SecP192R1_DEFAULT_COORDS;
        }

        protected override ECCurve CloneCurve()
        {
            return new SecP192R1Curve();
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
            return new SecP192R1FieldElement(x);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
        {
            return new SecP192R1Point(this, x, y, withCompression);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
        {
            return new SecP192R1Point(this, x, y, zs, withCompression);
        }
    }
}
