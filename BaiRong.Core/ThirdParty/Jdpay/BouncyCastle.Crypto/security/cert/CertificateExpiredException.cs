using System;

namespace Org.BouncyCastle.Security.Certificates
{
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || PORTABLE)
    [Serializable]
#endif
    public class CertificateExpiredException : CertificateException
	{
		public CertificateExpiredException() : base() { }
		public CertificateExpiredException(string message) : base(message) { }
		public CertificateExpiredException(string message, Exception exception) : base(message, exception) { }
	}
}
