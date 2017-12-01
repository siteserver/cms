using System;
using System.Collections;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509
{
    public class AttributeTable
    {
        private readonly IDictionary attributes;

        public AttributeTable(
            IDictionary attrs)
        {
            this.attributes = Platform.CreateHashtable(attrs);
        }

#if !(SILVERLIGHT || PORTABLE)
        [Obsolete]
        public AttributeTable(
            Hashtable attrs)
        {
            this.attributes = Platform.CreateHashtable(attrs);
        }
#endif

		public AttributeTable(
            Asn1EncodableVector v)
        {
            this.attributes = Platform.CreateHashtable(v.Count);

			for (int i = 0; i != v.Count; i++)
            {
                AttributeX509 a = AttributeX509.GetInstance(v[i]);

				attributes.Add(a.AttrType, a);
            }
        }

		public AttributeTable(
            Asn1Set s)
        {
            this.attributes = Platform.CreateHashtable(s.Count);

			for (int i = 0; i != s.Count; i++)
            {
                AttributeX509 a = AttributeX509.GetInstance(s[i]);

				attributes.Add(a.AttrType, a);
            }
        }

		public AttributeX509 Get(
            DerObjectIdentifier oid)
        {
            return (AttributeX509) attributes[oid];
        }

#if !(SILVERLIGHT || PORTABLE)
        [Obsolete("Use 'ToDictionary' instead")]
		public Hashtable ToHashtable()
        {
            return new Hashtable(attributes);
        }
#endif

        public IDictionary ToDictionary()
        {
            return Platform.CreateHashtable(attributes);
        }
    }
}
