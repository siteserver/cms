using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public interface TlsClient
        :   TlsPeer
    {
        /// <summary>
        /// Called at the start of a new TLS session, before any other methods.
        /// </summary>
        /// <param name="context">
        /// A <see cref="TlsProtocolHandler"/>
        /// </param>
        void Init(TlsClientContext context);

        /// <summary>Return the session this client wants to resume, if any.</summary>
        /// <remarks>Note that the peer's certificate chain for the session (if any) may need to be periodically revalidated.</remarks>
        /// <returns>
        /// A <see cref="TlsSession"/> representing the resumable session to be used for this connection,
        /// or null to use a new session.
        /// </returns>
        TlsSession GetSessionToResume();

        /// <summary>
        /// Return the <see cref="ProtocolVersion"/> to use for the <c>TLSPlaintext.version</c> field prior to
        /// receiving the server version. NOTE: This method is <b>not</b> called for DTLS.
        /// </summary>
        /// <remarks>
        /// See RFC 5246 E.1.: "TLS clients that wish to negotiate with older servers MAY send any value
        /// {03,XX} as the record layer version number. Typical values would be {03,00}, the lowest
        /// version number supported by the client, and the value of ClientHello.client_version. No
        /// single value will guarantee interoperability with all old servers, but this is a complex
        /// topic beyond the scope of this document."
        /// </remarks>
        /// <returns>The <see cref="ProtocolVersion"/> to use.</returns>
        ProtocolVersion ClientHelloRecordLayerVersion { get; }

        ProtocolVersion ClientVersion { get; }

        bool IsFallback { get; }

        /// <summary>
        /// Get the list of cipher suites that this client supports.
        /// </summary>
        /// <returns>
        /// An array of <see cref="CipherSuite"/> values, each specifying a supported cipher suite.
        /// </returns>
        int[] GetCipherSuites();

        /// <summary>
        /// Get the list of compression methods that this client supports.
        /// </summary>
        /// <returns>
        /// An array of <see cref="CompressionMethod"/> values, each specifying a supported compression method.
        /// </returns>
        byte[] GetCompressionMethods();

        /// <summary>
        /// Get the (optional) table of client extensions to be included in (extended) client hello.
        /// </summary>
        /// <returns>
        /// A <see cref="IDictionary"/> (Int32 -> byte[]). May be null.
        /// </returns>
        /// <exception cref="IOException"></exception>
        IDictionary GetClientExtensions();

        /// <exception cref="IOException"></exception>
        void NotifyServerVersion(ProtocolVersion selectedVersion);

        /// <summary>
        /// Notifies the client of the session_id sent in the ServerHello.
        /// </summary>
        /// <param name="sessionID">An array of <see cref="System.Byte"/></param>
        void NotifySessionID(byte[] sessionID);

        /// <summary>
        /// Report the cipher suite that was selected by the server.
        /// </summary>
        /// <remarks>
        /// The protocol handler validates this value against the offered cipher suites
        /// <seealso cref="GetCipherSuites"/>
        /// </remarks>
        /// <param name="selectedCipherSuite">
        /// A <see cref="CipherSuite"/>
        /// </param>
        void NotifySelectedCipherSuite(int selectedCipherSuite);

        /// <summary>
        /// Report the compression method that was selected by the server.
        /// </summary>
        /// <remarks>
        /// The protocol handler validates this value against the offered compression methods
        /// <seealso cref="GetCompressionMethods"/>
        /// </remarks>
        /// <param name="selectedCompressionMethod">
        /// A <see cref="CompressionMethod"/>
        /// </param>
        void NotifySelectedCompressionMethod(byte selectedCompressionMethod);

        /// <summary>
        /// Report the extensions from an extended server hello.
        /// </summary>
        /// <remarks>
        /// Will only be called if we returned a non-null result from <see cref="GetClientExtensions"/>.
        /// </remarks>
        /// <param name="serverExtensions">
        /// A <see cref="IDictionary"/>  (Int32 -> byte[])
        /// </param>
        void ProcessServerExtensions(IDictionary serverExtensions);

        /// <param name="serverSupplementalData">A <see cref="IList">list</see> of <see cref="SupplementalDataEntry"/></param>
        /// <exception cref="IOException"/>
        void ProcessServerSupplementalData(IList serverSupplementalData);

        /// <summary>
        /// Return an implementation of <see cref="TlsKeyExchange"/> to negotiate the key exchange
        /// part of the protocol.
        /// </summary>
        /// <returns>
        /// A <see cref="TlsKeyExchange"/>
        /// </returns>
        /// <exception cref="IOException"/>
        TlsKeyExchange GetKeyExchange();

        /// <summary>
        /// Return an implementation of <see cref="TlsAuthentication"/> to handle authentication
        /// part of the protocol.
        /// </summary>
        /// <exception cref="IOException"/>
        TlsAuthentication GetAuthentication();

        /// <returns>A <see cref="IList">list</see> of <see cref="SupplementalDataEntry"/></returns>
        /// <exception cref="IOException"/>
        IList GetClientSupplementalData();

        /// <summary>RFC 5077 3.3. NewSessionTicket Handshake Message</summary>
        /// <remarks>
        /// This method will be called (only) when a NewSessionTicket handshake message is received. The
        /// ticket is opaque to the client and clients MUST NOT examine the ticket under the assumption
        /// that it complies with e.g. <i>RFC 5077 4. Recommended Ticket Construction</i>.
        /// </remarks>
        /// <param name="newSessionTicket">The <see cref="NewSessionTicket">ticket</see></param>
        /// <exception cref="IOException"/>
        void NotifyNewSessionTicket(NewSessionTicket newSessionTicket);
    }
}
