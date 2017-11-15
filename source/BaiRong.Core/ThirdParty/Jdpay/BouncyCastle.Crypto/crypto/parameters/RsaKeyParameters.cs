using System;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class RsaKeyParameters
		: AsymmetricKeyParameter
    {
        // the value is the product of the 132 smallest primes from 3 to 751
        private static BigInteger SmallPrimesProduct = new BigInteger( 
            "8138E8A0FCF3A4E84A771D40FD305D7F4AA59306D7251DE54D98AF8FE95729A1" +
            "F73D893FA424CD2EDC8636A6C3285E022B0E3866A565AE8108EED8591CD4FE8D" +
            "2CE86165A978D719EBF647F362D33FCA29CD179FB42401CBAF3DF0C614056F9C" +
            "8F3CFD51E474AFB6BC6974F78DB8ABA8E9E517FDED658591AB7502BD41849462F",
            16);

        private static BigInteger Validate(BigInteger modulus)
        {
            if ((modulus.IntValue & 1) == 0)
                throw new ArgumentException("RSA modulus is even", "modulus");
            if (!modulus.Gcd(SmallPrimesProduct).Equals(BigInteger.One))
                throw new ArgumentException("RSA modulus has a small prime factor");

            // TODO: add additional primePower/Composite test - expensive!!

            return modulus;
        }

        private readonly BigInteger modulus;
        private readonly BigInteger exponent;

		public RsaKeyParameters(
            bool		isPrivate,
            BigInteger	modulus,
            BigInteger	exponent)
			: base(isPrivate)
        {
			if (modulus == null)
				throw new ArgumentNullException("modulus");
			if (exponent == null)
				throw new ArgumentNullException("exponent");
			if (modulus.SignValue <= 0)
				throw new ArgumentException("Not a valid RSA modulus", "modulus");
			if (exponent.SignValue <= 0)
				throw new ArgumentException("Not a valid RSA exponent", "exponent");
            if (!isPrivate && (exponent.IntValue & 1) == 0)
                throw new ArgumentException("RSA publicExponent is even", "exponent");

            this.modulus = Validate(modulus);
			this.exponent = exponent;
        }

		public BigInteger Modulus
        {
            get { return modulus; }
        }

		public BigInteger Exponent
        {
            get { return exponent; }
        }

		public override bool Equals(
			object obj)
        {
            RsaKeyParameters kp = obj as RsaKeyParameters;

			if (kp == null)
			{
				return false;
			}

			return kp.IsPrivate == this.IsPrivate
				&& kp.Modulus.Equals(this.modulus)
				&& kp.Exponent.Equals(this.exponent);
        }

		public override int GetHashCode()
        {
            return modulus.GetHashCode() ^ exponent.GetHashCode() ^ IsPrivate.GetHashCode();
        }
    }
}
