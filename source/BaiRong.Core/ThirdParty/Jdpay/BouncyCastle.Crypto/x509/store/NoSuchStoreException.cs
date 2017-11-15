using System;

namespace Org.BouncyCastle.X509.Store
{
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || PORTABLE)
    [Serializable]
#endif
    public class NoSuchStoreException
		: X509StoreException
	{
		public NoSuchStoreException()
		{
		}

		public NoSuchStoreException(
			string message)
			: base(message)
		{
		}

		public NoSuchStoreException(
			string		message,
			Exception	e)
			: base(message, e)
		{
		}
	}
}
