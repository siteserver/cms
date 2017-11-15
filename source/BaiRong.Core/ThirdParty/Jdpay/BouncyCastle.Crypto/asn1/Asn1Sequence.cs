using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Asn1
{
    public abstract class Asn1Sequence
        : Asn1Object, IEnumerable
    {
        private readonly IList seq;

        /**
         * return an Asn1Sequence from the given object.
         *
         * @param obj the object we want converted.
         * @exception ArgumentException if the object cannot be converted.
         */
        public static Asn1Sequence GetInstance(
            object obj)
        {
            if (obj == null || obj is Asn1Sequence)
            {
                return (Asn1Sequence)obj;
            }
            else if (obj is Asn1SequenceParser)
            {
                return Asn1Sequence.GetInstance(((Asn1SequenceParser)obj).ToAsn1Object());
            }
            else if (obj is byte[])
            {
                try
                {
                    return Asn1Sequence.GetInstance(FromByteArray((byte[])obj));
                }
                catch (IOException e)
                {
                    throw new ArgumentException("failed to construct sequence from byte[]: " + e.Message);
                }
            }
            else if (obj is Asn1Encodable)
            {
                Asn1Object primitive = ((Asn1Encodable)obj).ToAsn1Object();
                
                if (primitive is Asn1Sequence)
                {
                    return (Asn1Sequence)primitive;
                }
            }

            throw new ArgumentException("Unknown object in GetInstance: " + Platform.GetTypeName(obj), "obj");
        }

        /**
         * Return an ASN1 sequence from a tagged object. There is a special
         * case here, if an object appears to have been explicitly tagged on
         * reading but we were expecting it to be implicitly tagged in the
         * normal course of events it indicates that we lost the surrounding
         * sequence - so we need to add it back (this will happen if the tagged
         * object is a sequence that contains other sequences). If you are
         * dealing with implicitly tagged sequences you really <b>should</b>
         * be using this method.
         *
         * @param obj the tagged object.
         * @param explicitly true if the object is meant to be explicitly tagged,
         *          false otherwise.
         * @exception ArgumentException if the tagged object cannot
         *          be converted.
         */
        public static Asn1Sequence GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            Asn1Object inner = obj.GetObject();

            if (explicitly)
            {
                if (!obj.IsExplicit())
                    throw new ArgumentException("object implicit - explicit expected.");

                return (Asn1Sequence) inner;
            }

            //
            // constructed object which appears to be explicitly tagged
            // when it should be implicit means we have to add the
            // surrounding sequence.
            //
            if (obj.IsExplicit())
            {
                if (obj is BerTaggedObject)
                {
                    return new BerSequence(inner);
                }

                return new DerSequence(inner);
            }

            if (inner is Asn1Sequence)
            {
                return (Asn1Sequence) inner;
            }

            throw new ArgumentException("Unknown object in GetInstance: " + Platform.GetTypeName(obj), "obj");
        }

        protected internal Asn1Sequence(
            int capacity)
        {
            seq = Platform.CreateArrayList(capacity);
        }

        public virtual IEnumerator GetEnumerator()
        {
            return seq.GetEnumerator();
        }

        [Obsolete("Use GetEnumerator() instead")]
        public IEnumerator GetObjects()
        {
            return GetEnumerator();
        }

        private class Asn1SequenceParserImpl
            : Asn1SequenceParser
        {
            private readonly Asn1Sequence outer;
            private readonly int max;
            private int index;

            public Asn1SequenceParserImpl(
                Asn1Sequence outer)
            {
                this.outer = outer;
                this.max = outer.Count;
            }

            public IAsn1Convertible ReadObject()
            {
                if (index == max)
                    return null;

                Asn1Encodable obj = outer[index++];

                if (obj is Asn1Sequence)
                    return ((Asn1Sequence)obj).Parser;

                if (obj is Asn1Set)
                    return ((Asn1Set)obj).Parser;

                // NB: Asn1OctetString implements Asn1OctetStringParser directly
//				if (obj is Asn1OctetString)
//					return ((Asn1OctetString)obj).Parser;

                return obj;
            }

            public Asn1Object ToAsn1Object()
            {
                return outer;
            }
        }

        public virtual Asn1SequenceParser Parser
        {
            get { return new Asn1SequenceParserImpl(this); }
        }

        /**
         * return the object at the sequence position indicated by index.
         *
         * @param index the sequence number (starting at zero) of the object
         * @return the object at the sequence position indicated by index.
         */
        public virtual Asn1Encodable this[int index]
        {
            get { return (Asn1Encodable) seq[index]; }
        }

        [Obsolete("Use 'object[index]' syntax instead")]
        public Asn1Encodable GetObjectAt(
            int index)
        {
             return this[index];
        }

        [Obsolete("Use 'Count' property instead")]
        public int Size
        {
            get { return Count; }
        }

        public virtual int Count
        {
            get { return seq.Count; }
        }

        protected override int Asn1GetHashCode()
        {
            int hc = Count;

            foreach (object o in this)
            {
                hc *= 17;
                if (o == null)
                {
                    hc ^= DerNull.Instance.GetHashCode();
                }
                else
                {
                    hc ^= o.GetHashCode();
                }
            }

            return hc;
        }

        protected override bool Asn1Equals(
            Asn1Object asn1Object)
        {
            Asn1Sequence other = asn1Object as Asn1Sequence;

            if (other == null)
                return false;

            if (Count != other.Count)
                return false;

            IEnumerator s1 = GetEnumerator();
            IEnumerator s2 = other.GetEnumerator();

            while (s1.MoveNext() && s2.MoveNext())
            {
                Asn1Object o1 = GetCurrent(s1).ToAsn1Object();
                Asn1Object o2 = GetCurrent(s2).ToAsn1Object();

                if (!o1.Equals(o2))
                    return false;
            }

            return true;
        }

        private Asn1Encodable GetCurrent(IEnumerator e)
        {
            Asn1Encodable encObj = (Asn1Encodable)e.Current;

            // unfortunately null was allowed as a substitute for DER null
            if (encObj == null)
                return DerNull.Instance;

            return encObj;
        }

        protected internal void AddObject(
            Asn1Encodable obj)
        {
            seq.Add(obj);
        }

        public override string ToString()
        {
            return CollectionUtilities.ToString(seq);
        }
    }
}
