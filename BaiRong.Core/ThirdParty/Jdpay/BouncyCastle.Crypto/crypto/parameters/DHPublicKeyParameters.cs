using System;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public class DHPublicKeyParameters
		: DHKeyParameters
    {
        private static BigInteger Validate(BigInteger y, DHParameters dhParams)
        {
            if (y == null)
                throw new ArgumentNullException("y");

            // TLS check
            if (y.CompareTo(BigInteger.Two) < 0 || y.CompareTo(dhParams.P.Subtract(BigInteger.Two)) > 0)
                throw new ArgumentException("invalid DH public key", "y");

            // we can't validate without Q.
            if (dhParams.Q != null
                && !y.ModPow(dhParams.Q, dhParams.P).Equals(BigInteger.One))
            {
                throw new ArgumentException("y value does not appear to be in correct group", "y");
            }

            return y;
        }

        private readonly BigInteger y;

		public DHPublicKeyParameters(
            BigInteger		y,
            DHParameters	parameters)
			: base(false, parameters)
        {
			this.y = Validate(y, parameters);
        }

		public DHPublicKeyParameters(
            BigInteger			y,
            DHParameters		parameters,
		    DerObjectIdentifier	algorithmOid)
			: base(false, parameters, algorithmOid)
        {
            this.y = Validate(y, parameters);
        }

        public virtual BigInteger Y
        {
            get { return y; }
        }

		public override bool Equals(
			object  obj)
        {
			if (obj == this)
				return true;

			DHPublicKeyParameters other = obj as DHPublicKeyParameters;

			if (other == null)
				return false;

			return Equals(other);
        }

		protected bool Equals(
			DHPublicKeyParameters other)
		{
			return y.Equals(other.y) && base.Equals(other);
		}

		public override int GetHashCode()
        {
            return y.GetHashCode() ^ base.GetHashCode();
        }
    }
}
