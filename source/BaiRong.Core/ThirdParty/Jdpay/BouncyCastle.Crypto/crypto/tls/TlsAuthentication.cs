using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public interface TlsAuthentication
	{
		/// <summary>
		/// Called by the protocol handler to report the server certificate.
		/// </summary>
		/// <remarks>
		/// This method is responsible for certificate verification and validation
		/// </remarks>
		/// <param name="serverCertificate">The server <see cref="Certificate"/> received</param>
		/// <exception cref="IOException"></exception>
		void NotifyServerCertificate(Certificate serverCertificate);

		/// <summary>
		/// Return client credentials in response to server's certificate request
		/// </summary>
		/// <param name="certificateRequest">
		/// A <see cref="CertificateRequest"/> containing server certificate request details
		/// </param>
		/// <returns>
		/// A <see cref="TlsCredentials"/> to be used for client authentication
		/// (or <c>null</c> for no client authentication)
		/// </returns>
		/// <exception cref="IOException"></exception>
		TlsCredentials GetClientCredentials(CertificateRequest certificateRequest);
	}
}
