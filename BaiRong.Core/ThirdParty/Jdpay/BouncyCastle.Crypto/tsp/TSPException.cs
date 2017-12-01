using System;

namespace Org.BouncyCastle.Tsp
{
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || PORTABLE)
    [Serializable]
#endif
    public class TspException
		: Exception
	{
		public TspException()
		{
		}

		public TspException(
			string message)
			: base(message)
		{
		}

		public TspException(
			string		message,
			Exception	e)
			: base(message, e)
		{
		}
	}
}
