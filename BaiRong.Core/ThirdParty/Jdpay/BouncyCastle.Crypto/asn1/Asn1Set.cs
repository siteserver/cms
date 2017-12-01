using System;
using System.Collections;
using System.IO;

#if PORTABLE
using System.Collections.Generic;
using System.Linq;
#endif

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Asn1
{
    abstract public class Asn1Set
        : Asn1Object, IEnumerable
    {
        private readonly IList _set;

        /**
         * return an ASN1Set from the given object.
         *
         * @param obj the object we want converted.
         * @exception ArgumentException if the object cannot be converted.
         */
        public static Asn1Set GetInstance(
            object obj)
        {
            if (obj == null || obj is Asn1Set)
            {
                return (Asn1Set)obj;
            }
            else if (obj is Asn1SetParser)
            {
                return Asn1Set.GetInstance(((Asn1SetParser)obj).ToAsn1Object());
            }
            else if (obj is byte[])
            {
                try
                {
                    return Asn1Set.GetInstance(FromByteArray((byte[])obj));
                }
                catch (IOException e)
                {
                    throw new ArgumentException("failed to construct set from byte[]: " + e.Message);
                }
            }
            else if (obj is Asn1Encodable)
            {
                Asn1Object primitive = ((Asn1Encodable)obj).ToAsn1Object();

                if (primitive is Asn1Set)
                {
                    return (Asn1Set)primitive;
                }
            }

            throw new ArgumentException("Unknown object in GetInstance: " + Platform.GetTypeName(obj), "obj");
        }

        /**
         * Return an ASN1 set from a tagged object. There is a special
         * case here, if an object appears to have been explicitly tagged on
         * reading but we were expecting it to be implicitly tagged in the
         * normal course of events it indicates that we lost the surrounding
         * set - so we need to add it back (this will happen if the tagged
         * object is a sequence that contains other sequences). If you are
         * dealing with implicitly tagged sets you really <b>should</b>
         * be using this method.
         *
         * @param obj the tagged object.
         * @param explicitly true if the object is meant to be explicitly tagged
         *          false otherwise.
         * @exception ArgumentException if the tagged object cannot
         *          be converted.
         */
        public static Asn1Set GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            Asn1Object inner = obj.GetObject();

            if (explicitly)
            {
                if (!obj.IsExplicit())
                    throw new ArgumentException("object implicit - explicit expected.");

                return (Asn1Set) inner;
            }

            //
            // constructed object which appears to be explicitly tagged
            // and it's really implicit means we have to add the
            // surrounding sequence.
            //
            if (obj.IsExplicit())
            {
                return new DerSet(inner);
            }

            if (inner is Asn1Set)
            {
                return (Asn1Set) inner;
            }

            //
            // in this case the parser returns a sequence, convert it
            // into a set.
            //
            if (inner is Asn1Sequence)
            {
                Asn1EncodableVector v = new Asn1EncodableVector();
                Asn1Sequence s = (Asn1Sequence) inner;

                foreach (Asn1Encodable ae in s)
                {
                    v.Add(ae);
                }

                // TODO Should be able to construct set directly from sequence?
                return new DerSet(v, false);
            }

            throw new ArgumentException("Unknown object in GetInstance: " + Platform.GetTypeName(obj), "obj");
        }

        protected internal Asn1Set(
            int capacity)
        {
            _set = Platform.CreateArrayList(capacity);
        }

        public virtual IEnumerator GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        [Obsolete("Use GetEnumerator() instead")]
        public IEnumerator GetObjects()
        {
            return GetEnumerator();
        }

        /**
         * return the object at the set position indicated by index.
         *
         * @param index the set number (starting at zero) of the object
         * @return the object at the set position indicated by index.
         */
        public virtual Asn1Encodable this[int index]
        {
            get { return (Asn1Encodable) _set[index]; }
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
            get { return _set.Count; }
        }

        public virtual Asn1Encodable[] ToArray()
        {
            Asn1Encodable[] values = new Asn1Encodable[this.Count];
            for (int i = 0; i < this.Count; ++i)
            {
                values[i] = this[i];
            }
            return values;
        }

        private class Asn1SetParserImpl
            : Asn1SetParser
        {
            private readonly Asn1Set outer;
            private readonly int max;
            private int index;

            public Asn1SetParserImpl(
                Asn1Set outer)
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

            public virtual Asn1Object ToAsn1Object()
            {
                return outer;
            }
        }

        public Asn1SetParser Parser
        {
            get { return new Asn1SetParserImpl(this); }
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
            Asn1Set other = asn1Object as Asn1Set;

            if (other == null)
                return false;

            if (Count != other.Count)
            {
                return false;
            }

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

        protected internal void Sort()
        {
            if (_set.Count < 2)
                return;

#if PORTABLE
            var sorted = _set.Cast<Asn1Encodable>()
                             .Select(a => new { Item = a, Key = a.GetEncoded(Asn1Encodable.Der) })
                             .OrderBy(t => t.Key, new DerComparer())
                             .Select(t => t.Item)
                             .ToList();

            for (int i = 0; i < _set.Count; ++i)
            {
                _set[i] = sorted[i];
            }
#else
            Asn1Encodable[] items = new Asn1Encodable[_set.Count];
            byte[][] keys = new byte[_set.Count][];

            for (int i = 0; i < _set.Count; ++i)
            {
                Asn1Encodable item = (Asn1Encodable)_set[i];
                items[i] = item;
                keys[i] = item.GetEncoded(Asn1Encodable.Der);
            }

            Array.Sort(keys, items, new DerComparer());

            for (int i = 0; i < _set.Count; ++i)
            {
                _set[i] = items[i];
            }
#endif
        }

        protected internal void AddObject(Asn1Encodable obj)
        {
            _set.Add(obj);
        }

        public override string ToString()
        {
            return CollectionUtilities.ToString(_set);
        }

#if PORTABLE
        private class DerComparer
            : IComparer<byte[]>
        {
            public int Compare(byte[] x, byte[] y)
            {
                byte[] a = x, b = y;
#else
        private class DerComparer
            : IComparer
        {
            public int Compare(object x, object y)
            {
                byte[] a = (byte[])x, b = (byte[])y;
#endif
                int len = System.Math.Min(a.Length, b.Length);
                for (int i = 0; i != len; ++i)
                {
                    byte ai = a[i], bi = b[i];
                    if (ai != bi)
                        return ai < bi ? -1 : 1;
                }
                if (a.Length > b.Length)
                    return AllZeroesFrom(a, len) ? 0 : 1;
                if (a.Length < b.Length)
                    return AllZeroesFrom(b, len) ? 0 : -1;
                return 0;
            }

            private bool AllZeroesFrom(byte[] bs, int pos)
            {
                while (pos < bs.Length)
                {
                    if (bs[pos++] != 0)
                        return false;
                }
                return true;
            }
        }
    }
}
