using System;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
    public class DerEnumerated
        : Asn1Object
    {
        private readonly byte[] bytes;

        /**
         * return an integer from the passed in object
         *
         * @exception ArgumentException if the object cannot be converted.
         */
        public static DerEnumerated GetInstance(
            object obj)
        {
            if (obj == null || obj is DerEnumerated)
            {
                return (DerEnumerated)obj;
            }

            throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
        }

        /**
         * return an Enumerated from a tagged object.
         *
         * @param obj the tagged object holding the object we want
         * @param explicitly true if the object is meant to be explicitly
         *              tagged false otherwise.
         * @exception ArgumentException if the tagged object cannot
         *               be converted.
         */
        public static DerEnumerated GetInstance(
            Asn1TaggedObject	obj,
            bool				isExplicit)
        {
            Asn1Object o = obj.GetObject();

            if (isExplicit || o is DerEnumerated)
            {
                return GetInstance(o);
            }

            return FromOctetString(((Asn1OctetString)o).GetOctets());
        }

        public DerEnumerated(
            int val)
        {
            bytes = BigInteger.ValueOf(val).ToByteArray();
        }

        public DerEnumerated(
            BigInteger val)
        {
            bytes = val.ToByteArray();
        }

        public DerEnumerated(
            byte[]   bytes)
        {
            if (bytes.Length > 1)
            {
                if (bytes[0] == 0 && (bytes[1] & 0x80) == 0)
                {
                    throw new ArgumentException("malformed enumerated");
                }
                if (bytes[0] == (byte)0xff && (bytes[1] & 0x80) != 0)
                {
                    throw new ArgumentException("malformed enumerated");
                }
            }
            this.bytes = Arrays.Clone(bytes);
        }

        public BigInteger Value
        {
            get { return new BigInteger(bytes); }
        }

        internal override void Encode(
            DerOutputStream derOut)
        {
            derOut.WriteEncoded(Asn1Tags.Enumerated, bytes);
        }

        protected override bool Asn1Equals(
            Asn1Object asn1Object)
        {
            DerEnumerated other = asn1Object as DerEnumerated;

            if (other == null)
                return false;

            return Arrays.AreEqual(this.bytes, other.bytes);
        }

        protected override int Asn1GetHashCode()
        {
            return Arrays.GetHashCode(bytes);
        }

        private static readonly DerEnumerated[] cache = new DerEnumerated[12];

        internal static DerEnumerated FromOctetString(byte[] enc)
        {
            if (enc.Length == 0)
            {
                throw new ArgumentException("ENUMERATED has zero length", "enc");
            }

            if (enc.Length == 1)
            {
                int value = enc[0];
                if (value < cache.Length)
                {
                    DerEnumerated cached = cache[value];
                    if (cached != null)
                    {
                        return cached;
                    }

                    return cache[value] = new DerEnumerated(Arrays.Clone(enc));
                }
            }

            return new DerEnumerated(Arrays.Clone(enc));
        }
    }
}
