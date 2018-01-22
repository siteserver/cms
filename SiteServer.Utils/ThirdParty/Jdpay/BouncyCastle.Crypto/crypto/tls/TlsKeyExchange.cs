using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>
    /// A generic interface for key exchange implementations in (D)TLS.
    /// </summary>
    public interface TlsKeyExchange
    {
        void Init(TlsContext context);

        /// <exception cref="IOException"/>
        void SkipServerCredentials();

        /// <exception cref="IOException"/>
        void ProcessServerCredentials(TlsCredentials serverCredentials);

        /// <exception cref="IOException"/>
        void ProcessServerCertificate(Certificate serverCertificate);

        bool RequiresServerKeyExchange { get; }

        /// <exception cref="IOException"/>
        byte[] GenerateServerKeyExchange();

        /// <exception cref="IOException"/>
        void SkipServerKeyExchange();

        /// <exception cref="IOException"/>
        void ProcessServerKeyExchange(Stream input);

        /// <exception cref="IOException"/>
        void ValidateCertificateRequest(CertificateRequest certificateRequest);

        /// <exception cref="IOException"/>
        void SkipClientCredentials();

        /// <exception cref="IOException"/>
        void ProcessClientCredentials(TlsCredentials clientCredentials);

        /// <exception cref="IOException"/>
        void ProcessClientCertificate(Certificate clientCertificate);

        /// <exception cref="IOException"/>
        void GenerateClientKeyExchange(Stream output);

        /// <exception cref="IOException"/>
        void ProcessClientKeyExchange(Stream input);

        /// <exception cref="IOException"/>
        byte[] GeneratePremasterSecret();
    }
}
