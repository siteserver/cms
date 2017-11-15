using System;

using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecP256K1Curve
        : AbstractFpCurve
    {
        public static readonly BigInteger q = new BigInteger(1,
            Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F"));

        private const int SECP256K1_DEFAULT_COORDS = COORD_JACOBIAN;

        protected readonly SecP256K1Point m_infinity;

        public SecP256K1Curve()
            : base(q)
        {
            this.m_infinity = new SecP256K1Point(this, null, null);

            this.m_a = FromBigInteger(BigInteger.Zero);
            this.m_b = FromBigInteger(BigInteger.ValueOf(7));
            this.m_order = new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141"));
            this.m_cofactor = BigInteger.One;
            this.m_coord = SECP256K1_DEFAULT_COORDS;
        }

        protected override ECCurve CloneCurve()
        {
            return new SecP256K1Curve();
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
            return new SecP256K1FieldElement(x);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
        {
            return new SecP256K1Point(this, x, y, withCompression);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
        {
            return new SecP256K1Point(this, x, y, zs, withCompression);
        }
    }
}
