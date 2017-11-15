using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>(D)TLS DH key exchange.</summary>
    public class TlsDHKeyExchange
        :   AbstractTlsKeyExchange
    {
        protected TlsSigner mTlsSigner;
        protected DHParameters mDHParameters;

        protected AsymmetricKeyParameter mServerPublicKey;
        protected TlsAgreementCredentials mAgreementCredentials;

        protected DHPrivateKeyParameters mDHAgreePrivateKey;
        protected DHPublicKeyParameters mDHAgreePublicKey;

        public TlsDHKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, DHParameters dhParameters)
            :   base(keyExchange, supportedSignatureAlgorithms)
        {
            switch (keyExchange)
            {
            case KeyExchangeAlgorithm.DH_anon:
            case KeyExchangeAlgorithm.DH_RSA:
            case KeyExchangeAlgorithm.DH_DSS:
                this.mTlsSigner = null;
                break;
            case KeyExchangeAlgorithm.DHE_RSA:
                this.mTlsSigner = new TlsRsaSigner();
                break;
            case KeyExchangeAlgorithm.DHE_DSS:
                this.mTlsSigner = new TlsDssSigner();
                break;
            default:
                throw new InvalidOperationException("unsupported key exchange algorithm");
            }

            this.mDHParameters = dhParameters;
        }

        public override void Init(TlsContext context)
        {
            base.Init(context);

            if (this.mTlsSigner != null)
            {
                this.mTlsSigner.Init(context);
            }
        }

        public override void SkipServerCredentials()
        {
            if (mKeyExchange != KeyExchangeAlgorithm.DH_anon)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
        }

        public override void ProcessServerCertificate(Certificate serverCertificate)
        {
            if (mKeyExchange == KeyExchangeAlgorithm.DH_anon)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
            if (serverCertificate.IsEmpty)
                throw new TlsFatalAlert(AlertDescription.bad_certificate);

            X509CertificateStructure x509Cert = serverCertificate.GetCertificateAt(0);

            SubjectPublicKeyInfo keyInfo = x509Cert.SubjectPublicKeyInfo;
            try
            {
                this.mServerPublicKey = PublicKeyFactory.CreateKey(keyInfo);
            }
            catch (Exception e)
            {
                throw new TlsFatalAlert(AlertDescription.unsupported_certificate, e);
            }

            if (mTlsSigner == null)
            {
                try
                {
                    this.mDHAgreePublicKey = TlsDHUtilities.ValidateDHPublicKey((DHPublicKeyParameters)this.mServerPublicKey);
                    this.mDHParameters = ValidateDHParameters(mDHAgreePublicKey.Parameters);
                }
                catch (InvalidCastException e)
                {
                    throw new TlsFatalAlert(AlertDescription.certificate_unknown, e);
                }

                TlsUtilities.ValidateKeyUsage(x509Cert, KeyUsage.KeyAgreement);
            }
            else
            {
                if (!mTlsSigner.IsValidPublicKey(this.mServerPublicKey))
                {
                    throw new TlsFatalAlert(AlertDescription.certificate_unknown);
                }

                TlsUtilities.ValidateKeyUsage(x509Cert, KeyUsage.DigitalSignature);
            }

            base.ProcessServerCertificate(serverCertificate);
        }

        public override bool RequiresServerKeyExchange
        {
            get
            {
                switch (mKeyExchange)
                {
                case KeyExchangeAlgorithm.DH_anon:
                case KeyExchangeAlgorithm.DHE_DSS:
                case KeyExchangeAlgorithm.DHE_RSA:
                    return true;
                default:
                    return false;
                }
            }
        }

        public override byte[] GenerateServerKeyExchange()
        {
            if (!RequiresServerKeyExchange)
                return null;

            // DH_anon is handled here, DHE_* in a subclass

            MemoryStream buf = new MemoryStream();
            this.mDHAgreePrivateKey = TlsDHUtilities.GenerateEphemeralServerKeyExchange(mContext.SecureRandom,
                this.mDHParameters, buf);
            return buf.ToArray();
        }

        public override void ProcessServerKeyExchange(Stream input)
        {
            if (!RequiresServerKeyExchange)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);

            // DH_anon is handled here, DHE_* in a subclass

            ServerDHParams dhParams = ServerDHParams.Parse(input);

            this.mDHAgreePublicKey = TlsDHUtilities.ValidateDHPublicKey(dhParams.PublicKey);
            this.mDHParameters = ValidateDHParameters(mDHAgreePublicKey.Parameters);
        }

        public override void ValidateCertificateRequest(CertificateRequest certificateRequest)
        {
            if (mKeyExchange == KeyExchangeAlgorithm.DH_anon)
                throw new TlsFatalAlert(AlertDescription.handshake_failure);

            byte[] types = certificateRequest.CertificateTypes;
            for (int i = 0; i < types.Length; ++i)
            {
                switch (types[i])
                {
                case ClientCertificateType.rsa_sign:
                case ClientCertificateType.dss_sign:
                case ClientCertificateType.rsa_fixed_dh:
                case ClientCertificateType.dss_fixed_dh:
                case ClientCertificateType.ecdsa_sign:
                    break;
                default:
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                }
            }
        }

        public override void ProcessClientCredentials(TlsCredentials clientCredentials)
        {
            if (mKeyExchange == KeyExchangeAlgorithm.DH_anon)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            if (clientCredentials is TlsAgreementCredentials)
            {
                // TODO Validate client cert has matching parameters (see 'areCompatibleParameters')?

                this.mAgreementCredentials = (TlsAgreementCredentials)clientCredentials;
            }
            else if (clientCredentials is TlsSignerCredentials)
            {
                // OK
            }
            else
            {
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        public override void GenerateClientKeyExchange(Stream output)
        {
            /*
             * RFC 2246 7.4.7.2 If the client certificate already contains a suitable Diffie-Hellman
             * key, then Yc is implicit and does not need to be sent again. In this case, the Client Key
             * Exchange message will be sent, but will be empty.
             */
            if (mAgreementCredentials == null)
            {
                this.mDHAgreePrivateKey = TlsDHUtilities.GenerateEphemeralClientKeyExchange(mContext.SecureRandom,
                    mDHParameters, output);
            }
        }

        public override void ProcessClientCertificate(Certificate clientCertificate)
        {
            if (mKeyExchange == KeyExchangeAlgorithm.DH_anon)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);

            // TODO Extract the public key
            // TODO If the certificate is 'fixed', take the public key as dhAgreePublicKey
        }

        public override void ProcessClientKeyExchange(Stream input)
        {
            if (mDHAgreePublicKey != null)
            {
                // For dss_fixed_dh and rsa_fixed_dh, the key arrived in the client certificate
                return;
            }

            BigInteger Yc = TlsDHUtilities.ReadDHParameter(input);

            this.mDHAgreePublicKey = TlsDHUtilities.ValidateDHPublicKey(new DHPublicKeyParameters(Yc, mDHParameters));
        }

        public override byte[] GeneratePremasterSecret()
        {
            if (mAgreementCredentials != null)
            {
                return mAgreementCredentials.GenerateAgreement(mDHAgreePublicKey);
            }

            if (mDHAgreePrivateKey != null)
            {
                return TlsDHUtilities.CalculateDHBasicAgreement(mDHAgreePublicKey, mDHAgreePrivateKey);
            }

            throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        protected virtual int MinimumPrimeBits
        {
            get { return 1024; }
        }

        protected virtual DHParameters ValidateDHParameters(DHParameters parameters)
        {
            if (parameters.P.BitLength < MinimumPrimeBits)
                throw new TlsFatalAlert(AlertDescription.insufficient_security);

            return TlsDHUtilities.ValidateDHParameters(parameters);
        }
    }
}
