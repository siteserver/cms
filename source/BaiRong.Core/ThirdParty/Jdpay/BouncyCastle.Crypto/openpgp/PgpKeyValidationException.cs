using System;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	/// <remarks>
	/// Thrown if the key checksum is invalid.
	/// </remarks>
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || PORTABLE)
    [Serializable]
#endif
    public class PgpKeyValidationException
		: PgpException
	{
		public PgpKeyValidationException() : base() {}
		public PgpKeyValidationException(string message) : base(message) {}
		public PgpKeyValidationException(string message, Exception exception) : base(message, exception) {}
	}
}
