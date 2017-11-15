using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public abstract class AbstractTlsKeyExchange
        :   TlsKeyExchange
    {
        protected readonly int mKeyExchange;
        protected IList mSupportedSignatureAlgorithms;

        protected TlsContext mContext;

        protected AbstractTlsKeyExchange(int keyExchange, IList supportedSignatureAlgorithms)
        {
            this.mKeyExchange = keyExchange;
            this.mSupportedSignatureAlgorithms = supportedSignatureAlgorithms;
        }

        protected virtual DigitallySigned ParseSignature(Stream input)
        {
            DigitallySigned signature = DigitallySigned.Parse(mContext, input);
            SignatureAndHashAlgorithm signatureAlgorithm = signature.Algorithm;
            if (signatureAlgorithm != null)
            {
                TlsUtilities.VerifySupportedSignatureAlgorithm(mSupportedSignatureAlgorithms, signatureAlgorithm);
            }
            return signature;
        }

        public virtual void Init(TlsContext context)
        {
            this.mContext = context;

            ProtocolVersion clientVersion = context.ClientVersion;

            if (TlsUtilities.IsSignatureAlgorithmsExtensionAllowed(clientVersion))
            {
                /*
                 * RFC 5246 7.4.1.4.1. If the client does not send the signature_algorithms extension,
                 * the server MUST do the following:
                 * 
                 * - If the negotiated key exchange algorithm is one of (RSA, DHE_RSA, DH_RSA, RSA_PSK,
                 * ECDH_RSA, ECDHE_RSA), behave as if client had sent the value {sha1,rsa}.
                 * 
                 * - If the negotiated key exchange algorithm is one of (DHE_DSS, DH_DSS), behave as if
                 * the client had sent the value {sha1,dsa}.
                 * 
                 * - If the negotiated key exchange algorithm is one of (ECDH_ECDSA, ECDHE_ECDSA),
                 * behave as if the client had sent value {sha1,ecdsa}.
                 */
                if (this.mSupportedSignatureAlgorithms == null)
                {
                    switch (mKeyExchange)
                    {
                    case KeyExchangeAlgorithm.DH_DSS:
                    case KeyExchangeAlgorithm.DHE_DSS:
                    case KeyExchangeAlgorithm.SRP_DSS:
                    {
                        this.mSupportedSignatureAlgorithms = TlsUtilities.GetDefaultDssSignatureAlgorithms();
                        break;
                    }

                    case KeyExchangeAlgorithm.ECDH_ECDSA:
                    case KeyExchangeAlgorithm.ECDHE_ECDSA:
                    {
                        this.mSupportedSignatureAlgorithms = TlsUtilities.GetDefaultECDsaSignatureAlgorithms();
                        break;
                    }

                    case KeyExchangeAlgorithm.DH_RSA:
                    case KeyExchangeAlgorithm.DHE_RSA:
                    case KeyExchangeAlgorithm.ECDH_RSA:
                    case KeyExchangeAlgorithm.ECDHE_RSA:
                    case KeyExchangeAlgorithm.RSA:
                    case KeyExchangeAlgorithm.RSA_PSK:
                    case KeyExchangeAlgorithm.SRP_RSA:
                    {
                        this.mSupportedSignatureAlgorithms = TlsUtilities.GetDefaultRsaSignatureAlgorithms();
                        break;
                    }

                    case KeyExchangeAlgorithm.DHE_PSK:
                    case KeyExchangeAlgorithm.ECDHE_PSK:
                    case KeyExchangeAlgorithm.PSK:
                    case KeyExchangeAlgorithm.SRP:
                        break;

                    default:
                        throw new InvalidOperationException("unsupported key exchange algorithm");
                    }
                }

            }
            else if (this.mSupportedSignatureAlgorithms != null)
            {
                throw new InvalidOperationException("supported_signature_algorithms not allowed for " + clientVersion);
            }
        }

        public abstract void SkipServerCredentials();

        public virtual void ProcessServerCertificate(Certificate serverCertificate)
        {
            if (mSupportedSignatureAlgorithms == null)
            {
                /*
                 * TODO RFC 2246 7.4.2. Unless otherwise specified, the signing algorithm for the
                 * certificate must be the same as the algorithm for the certificate key.
                 */
            }
            else
            {
                /*
                 * TODO RFC 5246 7.4.2. If the client provided a "signature_algorithms" extension, then
                 * all certificates provided by the server MUST be signed by a hash/signature algorithm
                 * pair that appears in that extension.
                 */
            }
        }

        public virtual void ProcessServerCredentials(TlsCredentials serverCredentials)
        {
            ProcessServerCertificate(serverCredentials.Certificate);
        }

        public virtual bool RequiresServerKeyExchange
        {
            get { return false; }
        }

        public virtual byte[] GenerateServerKeyExchange()
        {
            if (RequiresServerKeyExchange)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            return null;
        }

        public virtual void SkipServerKeyExchange()
        {
            if (RequiresServerKeyExchange)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
        }

        public virtual void ProcessServerKeyExchange(Stream input)
        {
            if (!RequiresServerKeyExchange)
            {
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
            }
        }

        public abstract void ValidateCertificateRequest(CertificateRequest certificateRequest);

        public virtual void SkipClientCredentials()
        {
        }

        public abstract void ProcessClientCredentials(TlsCredentials clientCredentials);

        public virtual void ProcessClientCertificate(Certificate clientCertificate)
        {
        }

        public abstract void GenerateClientKeyExchange(Stream output);

        public virtual void ProcessClientKeyExchange(Stream input)
        {
            // Key exchange implementation MUST support client key exchange
            throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        public abstract byte[] GeneratePremasterSecret();
    }
}
