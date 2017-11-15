using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public interface TlsPeer
    {
        /// <summary>
        /// draft-mathewson-no-gmtunixtime-00 2. "If existing users of a TLS implementation may rely on
        /// gmt_unix_time containing the current time, we recommend that implementors MAY provide the
        /// ability to set gmt_unix_time as an option only, off by default."
        /// </summary>
        /// <returns>
        /// <code>true</code> if the current time should be used in the gmt_unix_time field of
        /// Random, or <code>false</code> if gmt_unix_time should contain a cryptographically
        /// random value.
        /// </returns>
        bool ShouldUseGmtUnixTime();

        /// <summary>
        /// Report whether the server supports secure renegotiation
        /// </summary>
        /// <remarks>
        /// The protocol handler automatically processes the relevant extensions
        /// </remarks>
        /// <param name="secureRenegotiation">
        /// A <see cref="System.Boolean"/>, true if the server supports secure renegotiation
        /// </param>
        /// <exception cref="IOException"></exception>
        void NotifySecureRenegotiation(bool secureRenegotiation);

        /// <summary>
        /// Return an implementation of <see cref="TlsCompression"/> to handle record compression.
        /// </summary>
        /// <returns>A <see cref="TlsCompression"/></returns>
        /// <exception cref="IOException"/>
        TlsCompression GetCompression();

        /// <summary>
        /// Return an implementation of <see cref="TlsCipher"/> to use for encryption/decryption.
        /// </summary>
        /// <returns>A <see cref="TlsCipher"/></returns>
        /// <exception cref="IOException"/>
        TlsCipher GetCipher();

        /// <summary>This method will be called when an alert is raised by the protocol.</summary>
        /// <param name="alertLevel"><see cref="AlertLevel"/></param>
        /// <param name="alertDescription"><see cref="AlertDescription"/></param>
        /// <param name="message">A human-readable message explaining what caused this alert. May be null.</param>
        /// <param name="cause">The <c>Exception</c> that caused this alert to be raised. May be null.</param>
        void NotifyAlertRaised(byte alertLevel, byte alertDescription, string message, Exception cause);

        /// <summary>This method will be called when an alert is received from the remote peer.</summary>
        /// <param name="alertLevel"><see cref="AlertLevel"/></param>
        /// <param name="alertDescription"><see cref="AlertDescription"/></param>
        void NotifyAlertReceived(byte alertLevel, byte alertDescription);

        /// <summary>Notifies the peer that the handshake has been successfully completed.</summary>
        /// <exception cref="IOException"></exception>
        void NotifyHandshakeComplete();
    }
}
