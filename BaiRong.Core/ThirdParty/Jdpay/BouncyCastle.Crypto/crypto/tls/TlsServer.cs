using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public interface TlsServer
        :   TlsPeer
    {
        void Init(TlsServerContext context);

        /// <exception cref="IOException"></exception>
        void NotifyClientVersion(ProtocolVersion clientVersion);

        /// <exception cref="IOException"></exception>
        void NotifyFallback(bool isFallback);

        /// <exception cref="IOException"></exception>
        void NotifyOfferedCipherSuites(int[] offeredCipherSuites);

        /// <exception cref="IOException"></exception>
        void NotifyOfferedCompressionMethods(byte[] offeredCompressionMethods);

        /// <param name="clientExtensions">A <see cref="IDictionary"/> (Int32 -> byte[]). Will never be null.</param>
        /// <exception cref="IOException"></exception>
        void ProcessClientExtensions(IDictionary clientExtensions);

        /// <exception cref="IOException"></exception>
        ProtocolVersion GetServerVersion();

        /// <exception cref="IOException"></exception>
        int GetSelectedCipherSuite();

        /// <exception cref="IOException"></exception>
        byte GetSelectedCompressionMethod();

        /// <summary>
        /// Get the (optional) table of server extensions to be included in (extended) server hello.
        /// </summary>
        /// <returns>
        /// A <see cref="IDictionary"/> (Int32 -> byte[]). May be null.
        /// </returns>
        /// <exception cref="IOException"></exception>
        IDictionary GetServerExtensions();

        /// <returns>
        /// A <see cref="IList"/> (<see cref="SupplementalDataEntry"/>). May be null.
        /// </returns>
        /// <exception cref="IOException"></exception>
        IList GetServerSupplementalData();

        /// <exception cref="IOException"></exception>
        TlsCredentials GetCredentials();

        /// <remarks>
        /// This method will be called (only) if the server included an extension of type
        /// "status_request" with empty "extension_data" in the extended server hello. See <i>RFC 3546
        /// 3.6. Certificate Status Request</i>. If a non-null <see cref="CertificateStatus"/> is returned, it
        /// is sent to the client as a handshake message of type "certificate_status".
        /// </remarks>
        /// <returns>A <see cref="CertificateStatus"/> to be sent to the client (or null for none).</returns>
        /// <exception cref="IOException"></exception>
        CertificateStatus GetCertificateStatus();

        /// <exception cref="IOException"></exception>
        TlsKeyExchange GetKeyExchange();

        /// <exception cref="IOException"></exception>
        CertificateRequest GetCertificateRequest();

        /// <param name="clientSupplementalData"><see cref="IList"/> (<see cref="SupplementalDataEntry"/>)</param>
        /// <exception cref="IOException"></exception>
        void ProcessClientSupplementalData(IList clientSupplementalData);

        /// <summary>
        /// Called by the protocol handler to report the client certificate, only if <c>GetCertificateRequest</c>
        /// returned non-null.
        /// </summary>
        /// <remarks>Note: this method is responsible for certificate verification and validation.</remarks>
        /// <param name="clientCertificate">the effective client certificate (may be an empty chain).</param>
        /// <exception cref="IOException"></exception>
        void NotifyClientCertificate(Certificate clientCertificate);

        /// <summary>RFC 5077 3.3. NewSessionTicket Handshake Message.</summary>
        /// <remarks>
        /// This method will be called (only) if a NewSessionTicket extension was sent by the server. See
        /// <i>RFC 5077 4. Recommended Ticket Construction</i> for recommended format and protection.
        /// </remarks>
        /// <returns>The <see cref="NewSessionTicket">ticket</see>)</returns>
        /// <exception cref="IOException"></exception>
        NewSessionTicket GetNewSessionTicket();
    }
}
