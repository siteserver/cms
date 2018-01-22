using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>(D)TLS ECDH key exchange (see RFC 4492).</summary>
    public class TlsECDHKeyExchange
        :   AbstractTlsKeyExchange
    {
        protected TlsSigner mTlsSigner;
        protected int[] mNamedCurves;
        protected byte[] mClientECPointFormats, mServerECPointFormats;

        protected AsymmetricKeyParameter mServerPublicKey;
        protected TlsAgreementCredentials mAgreementCredentials;

        protected ECPrivateKeyParameters mECAgreePrivateKey;
        protected ECPublicKeyParameters mECAgreePublicKey;

        public TlsECDHKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, int[] namedCurves,
            byte[] clientECPointFormats, byte[] serverECPointFormats)
            :   base(keyExchange, supportedSignatureAlgorithms)
        {
            switch (keyExchange)
            {
            case KeyExchangeAlgorithm.ECDHE_RSA:
                this.mTlsSigner = new TlsRsaSigner();
                break;
            case KeyExchangeAlgorithm.ECDHE_ECDSA:
                this.mTlsSigner = new TlsECDsaSigner();
                break;
            case KeyExchangeAlgorithm.ECDH_anon:
            case KeyExchangeAlgorithm.ECDH_RSA:
            case KeyExchangeAlgorithm.ECDH_ECDSA:
                this.mTlsSigner = null;
                break;
            default:
                throw new InvalidOperationException("unsupported key exchange algorithm");
            }

            this.mNamedCurves = namedCurves;
            this.mClientECPointFormats = clientECPointFormats;
            this.mServerECPointFormats = serverECPointFormats;
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
            if (mKeyExchange != KeyExchangeAlgorithm.ECDH_anon)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
        }

        public override void ProcessServerCertificate(Certificate serverCertificate)
        {
            if (mKeyExchange == KeyExchangeAlgorithm.ECDH_anon)
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
                    this.mECAgreePublicKey = TlsEccUtilities.ValidateECPublicKey((ECPublicKeyParameters) this.mServerPublicKey);
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
                    throw new TlsFatalAlert(AlertDescription.certificate_unknown);

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
                case KeyExchangeAlgorithm.ECDH_anon:
                case KeyExchangeAlgorithm.ECDHE_ECDSA:
                case KeyExchangeAlgorithm.ECDHE_RSA:
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

            // ECDH_anon is handled here, ECDHE_* in a subclass

            MemoryStream buf = new MemoryStream();
            this.mECAgreePrivateKey = TlsEccUtilities.GenerateEphemeralServerKeyExchange(mContext.SecureRandom, mNamedCurves,
                mClientECPointFormats, buf);
            return buf.ToArray();
        }

        public override void ProcessServerKeyExchange(Stream input)
        {
            if (!RequiresServerKeyExchange)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);

            // ECDH_anon is handled here, ECDHE_* in a subclass

            ECDomainParameters curve_params = TlsEccUtilities.ReadECParameters(mNamedCurves, mClientECPointFormats, input);

            byte[] point = TlsUtilities.ReadOpaque8(input);

            this.mECAgreePublicKey = TlsEccUtilities.ValidateECPublicKey(TlsEccUtilities.DeserializeECPublicKey(
                mClientECPointFormats, curve_params, point));
        }

        public override void ValidateCertificateRequest(CertificateRequest certificateRequest)
        {
            if (mKeyExchange == KeyExchangeAlgorithm.ECDH_anon)
                throw new TlsFatalAlert(AlertDescription.handshake_failure);

            /*
             * RFC 4492 3. [...] The ECDSA_fixed_ECDH and RSA_fixed_ECDH mechanisms are usable with
             * ECDH_ECDSA and ECDH_RSA. Their use with ECDHE_ECDSA and ECDHE_RSA is prohibited because
             * the use of a long-term ECDH client key would jeopardize the forward secrecy property of
             * these algorithms.
             */
            byte[] types = certificateRequest.CertificateTypes;
            for (int i = 0; i < types.Length; ++i)
            {
                switch (types[i])
                {
                case ClientCertificateType.rsa_sign:
                case ClientCertificateType.dss_sign:
                case ClientCertificateType.ecdsa_sign:
                case ClientCertificateType.rsa_fixed_ecdh:
                case ClientCertificateType.ecdsa_fixed_ecdh:
                    break;
                default:
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                }
            }
        }

        public override void ProcessClientCredentials(TlsCredentials clientCredentials)
        {
            if (mKeyExchange == KeyExchangeAlgorithm.ECDH_anon)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            if (clientCredentials is TlsAgreementCredentials)
            {
                // TODO Validate client cert has matching parameters (see 'TlsEccUtilities.AreOnSameCurve')?

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
            if (mAgreementCredentials == null)
            {
                this.mECAgreePrivateKey = TlsEccUtilities.GenerateEphemeralClientKeyExchange(mContext.SecureRandom,
                    mServerECPointFormats, mECAgreePublicKey.Parameters, output);
            }
        }

        public override void ProcessClientCertificate(Certificate clientCertificate)
        {
            if (mKeyExchange == KeyExchangeAlgorithm.ECDH_anon)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);

            // TODO Extract the public key
            // TODO If the certificate is 'fixed', take the public key as mECAgreeClientPublicKey
        }

        public override void ProcessClientKeyExchange(Stream input)
        {
            if (mECAgreePublicKey != null)
            {
                // For ecdsa_fixed_ecdh and rsa_fixed_ecdh, the key arrived in the client certificate
                return;
            }

            byte[] point = TlsUtilities.ReadOpaque8(input);

            ECDomainParameters curve_params = this.mECAgreePrivateKey.Parameters;

            this.mECAgreePublicKey = TlsEccUtilities.ValidateECPublicKey(TlsEccUtilities.DeserializeECPublicKey(
                mServerECPointFormats, curve_params, point));
        }

        public override byte[] GeneratePremasterSecret()
        {
            if (mAgreementCredentials != null)
            {
                return mAgreementCredentials.GenerateAgreement(mECAgreePublicKey);
            }

            if (mECAgreePrivateKey != null)
            {
                return TlsEccUtilities.CalculateECDHBasicAgreement(mECAgreePublicKey, mECAgreePrivateKey);
            }

            throw new TlsFatalAlert(AlertDescription.internal_error);
        }
    }
}
