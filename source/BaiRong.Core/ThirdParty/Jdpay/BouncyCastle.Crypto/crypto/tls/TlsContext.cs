using System;

using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls
{
    public interface TlsContext
    {
        IRandomGenerator NonceRandomGenerator { get; }

        SecureRandom SecureRandom { get; }

        SecurityParameters SecurityParameters { get; }

        bool IsServer { get; }

        ProtocolVersion ClientVersion { get; }

        ProtocolVersion ServerVersion { get; }

        /**
         * Used to get the resumable session, if any, used by this connection. Only available after the
         * handshake has successfully completed.
         * 
         * @return A {@link TlsSession} representing the resumable session used by this connection, or
         *         null if no resumable session available.
         * @see TlsPeer#NotifyHandshakeComplete()
         */
        TlsSession ResumableSession { get; }

        object UserObject { get; set; }

        /**
         * Export keying material according to RFC 5705: "Keying Material Exporters for TLS".
         *
         * @param asciiLabel    indicates which application will use the exported keys.
         * @param context_value allows the application using the exporter to mix its own data with the TLS PRF for
         *                      the exporter output.
         * @param length        the number of bytes to generate
         * @return a pseudorandom bit string of 'length' bytes generated from the master_secret.
         */
        byte[] ExportKeyingMaterial(string asciiLabel, byte[] context_value, int length);
    }
}
