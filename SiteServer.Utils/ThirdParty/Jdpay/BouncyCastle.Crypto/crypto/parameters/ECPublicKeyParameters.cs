using System;
using System.Globalization;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math.EC;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public class ECPublicKeyParameters
        : ECKeyParameters
    {
        private static ECPoint Validate(ECPoint q)
        {
            if (q == null)
                throw new ArgumentNullException("q");
            if (q.IsInfinity)
                throw new ArgumentException("point at infinity", "q");

            q = q.Normalize();

            if (!q.IsValid())
                throw new ArgumentException("point not on curve", "q");

            return q;
        }

        private readonly ECPoint q;

        public ECPublicKeyParameters(
            ECPoint				q,
            ECDomainParameters	parameters)
            : this("EC", q, parameters)
        {
        }

        [Obsolete("Use version with explicit 'algorithm' parameter")]
        public ECPublicKeyParameters(
            ECPoint				q,
            DerObjectIdentifier publicKeyParamSet)
            : base("ECGOST3410", false, publicKeyParamSet)
        {
            if (q == null)
                throw new ArgumentNullException("q");

            this.q = Validate(q);
        }

        public ECPublicKeyParameters(
            string				algorithm,
            ECPoint				q,
            ECDomainParameters	parameters)
            : base(algorithm, false, parameters)
        {
            if (q == null)
                throw new ArgumentNullException("q");

            this.q = Validate(q);
        }

        public ECPublicKeyParameters(
            string				algorithm,
            ECPoint				q,
            DerObjectIdentifier publicKeyParamSet)
            : base(algorithm, false, publicKeyParamSet)
        {
            if (q == null)
                throw new ArgumentNullException("q");

            this.q = Validate(q);
        }

        public ECPoint Q
        {
            get { return q; }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;

            ECPublicKeyParameters other = obj as ECPublicKeyParameters;

            if (other == null)
                return false;

            return Equals(other);
        }

        protected bool Equals(
            ECPublicKeyParameters other)
        {
            return q.Equals(other.q) && base.Equals(other);
        }

        public override int GetHashCode()
        {
            return q.GetHashCode() ^ base.GetHashCode();
        }
    }
}
