using System;
using System.Collections;

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Pkcs
{
    public class X509CertificateEntry
        : Pkcs12Entry
    {
        private readonly X509Certificate cert;

		public X509CertificateEntry(
            X509Certificate cert)
			: base(Platform.CreateHashtable())
        {
            this.cert = cert;
        }

#if !(SILVERLIGHT || PORTABLE)
        [Obsolete]
        public X509CertificateEntry(
            X509Certificate	cert,
            Hashtable		attributes)
			: base(attributes)
        {
            this.cert = cert;
        }
#endif

        public X509CertificateEntry(
            X509Certificate cert,
            IDictionary     attributes)
			: base(attributes)
        {
            this.cert = cert;
        }

		public X509Certificate Certificate
        {
			get { return this.cert; }
        }

		public override bool Equals(object obj)
		{
			X509CertificateEntry other = obj as X509CertificateEntry;

			if (other == null)
				return false;

			return cert.Equals(other.cert);
		}

		public override int GetHashCode()
		{
			return ~cert.GetHashCode();
		}
	}
}
