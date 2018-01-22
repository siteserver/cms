using System;
using System.Collections;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Pkcs
{
    public class AsymmetricKeyEntry
        : Pkcs12Entry
    {
        private readonly AsymmetricKeyParameter key;

		public AsymmetricKeyEntry(
            AsymmetricKeyParameter key)
			: base(Platform.CreateHashtable())
        {
            this.key = key;
        }

#if !(SILVERLIGHT || PORTABLE)
        [Obsolete]
        public AsymmetricKeyEntry(
            AsymmetricKeyParameter key,
            Hashtable attributes)
			: base(attributes)
        {
            this.key = key;
        }
#endif

        public AsymmetricKeyEntry(
            AsymmetricKeyParameter  key,
            IDictionary             attributes)
			: base(attributes)
        {
            this.key = key;
        }

		public AsymmetricKeyParameter Key
        {
            get { return this.key; }
        }

		public override bool Equals(object obj)
		{
			AsymmetricKeyEntry other = obj as AsymmetricKeyEntry;

			if (other == null)
				return false;

			return key.Equals(other.key);
		}

		public override int GetHashCode()
		{
			return ~key.GetHashCode();
		}
	}
}
