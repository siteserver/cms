using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>(D)TLS PSK key exchange (RFC 4279).</summary>
    public class TlsPskKeyExchange
        :   AbstractTlsKeyExchange
    {
        protected TlsPskIdentity mPskIdentity;
        protected TlsPskIdentityManager mPskIdentityManager;

        protected DHParameters mDHParameters;
        protected int[] mNamedCurves;
        protected byte[] mClientECPointFormats, mServerECPointFormats;

        protected byte[] mPskIdentityHint = null;
        protected byte[] mPsk = null;

        protected DHPrivateKeyParameters mDHAgreePrivateKey = null;
        protected DHPublicKeyParameters mDHAgreePublicKey = null;

        protected ECPrivateKeyParameters mECAgreePrivateKey = null;
        protected ECPublicKeyParameters mECAgreePublicKey = null;

        protected AsymmetricKeyParameter mServerPublicKey = null;
        protected RsaKeyParameters mRsaServerPublicKey = null;
        protected TlsEncryptionCredentials mServerCredentials = null;
        protected byte[] mPremasterSecret;

        public TlsPskKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, TlsPskIdentity pskIdentity,
            TlsPskIdentityManager pskIdentityManager, DHParameters dhParameters, int[] namedCurves,
            byte[] clientECPointFormats, byte[] serverECPointFormats)
            :   base(keyExchange, supportedSignatureAlgorithms)
        {
            switch (keyExchange)
            {
            case KeyExchangeAlgorithm.DHE_PSK:
            case KeyExchangeAlgorithm.ECDHE_PSK:
            case KeyExchangeAlgorithm.PSK:
            case KeyExchangeAlgorithm.RSA_PSK:
                break;
            default:
                throw new InvalidOperationException("unsupported key exchange algorithm");
            }

            this.mPskIdentity = pskIdentity;
            this.mPskIdentityManager = pskIdentityManager;
            this.mDHParameters = dhParameters;
            this.mNamedCurves = namedCurves;
            this.mClientECPointFormats = clientECPointFormats;
            this.mServerECPointFormats = serverECPointFormats;
        }

        public override void SkipServerCredentials()
        {
            if (mKeyExchange == KeyExchangeAlgorithm.RSA_PSK)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
        }

        public override void ProcessServerCredentials(TlsCredentials serverCredentials)
        {
            if (!(serverCredentials is TlsEncryptionCredentials))
                throw new TlsFatalAlert(AlertDescription.internal_error);

            ProcessServerCertificate(serverCredentials.Certificate);

            this.mServerCredentials = (TlsEncryptionCredentials)serverCredentials;
        }

        public override byte[] GenerateServerKeyExchange()
        {
            this.mPskIdentityHint = mPskIdentityManager.GetHint();

            if (this.mPskIdentityHint == null && !RequiresServerKeyExchange)
                return null;

            MemoryStream buf = new MemoryStream();

            if (this.mPskIdentityHint == null)
            {
                TlsUtilities.WriteOpaque16(TlsUtilities.EmptyBytes, buf);
            }
            else
            {
                TlsUtilities.WriteOpaque16(this.mPskIdentityHint, buf);
            }

            if (this.mKeyExchange == KeyExchangeAlgorithm.DHE_PSK)
            {
                if (this.mDHParameters == null)
                    throw new TlsFatalAlert(AlertDescription.internal_error);

                this.mDHAgreePrivateKey = TlsDHUtilities.GenerateEphemeralServerKeyExchange(mContext.SecureRandom,
                    this.mDHParameters, buf);
            }
            else if (this.mKeyExchange == KeyExchangeAlgorithm.ECDHE_PSK)
            {
                this.mECAgreePrivateKey = TlsEccUtilities.GenerateEphemeralServerKeyExchange(mContext.SecureRandom,
                    mNamedCurves, mClientECPointFormats, buf);
            }

            return buf.ToArray();
        }

        public override void ProcessServerCertificate(Certificate serverCertificate)
        {
            if (mKeyExchange != KeyExchangeAlgorithm.RSA_PSK)
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

            // Sanity check the PublicKeyFactory
            if (this.mServerPublicKey.IsPrivate)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            this.mRsaServerPublicKey = ValidateRsaPublicKey((RsaKeyParameters)this.mServerPublicKey);

            TlsUtilities.ValidateKeyUsage(x509Cert, KeyUsage.KeyEncipherment);

            base.ProcessServerCertificate(serverCertificate);
        }

        public override bool RequiresServerKeyExchange
        {
            get
            {
                switch (mKeyExchange)
                {
                case KeyExchangeAlgorithm.DHE_PSK:
                case KeyExchangeAlgorithm.ECDHE_PSK:
                    return true;
                default:
                    return false;
                }
            }
        }

        public override void ProcessServerKeyExchange(Stream input)
        {
            this.mPskIdentityHint = TlsUtilities.ReadOpaque16(input);

            if (this.mKeyExchange == KeyExchangeAlgorithm.DHE_PSK)
            {
                ServerDHParams serverDHParams = ServerDHParams.Parse(input);

                this.mDHAgreePublicKey = TlsDHUtilities.ValidateDHPublicKey(serverDHParams.PublicKey);
                this.mDHParameters = mDHAgreePublicKey.Parameters;
            }
            else if (this.mKeyExchange == KeyExchangeAlgorithm.ECDHE_PSK)
            {
                ECDomainParameters ecParams = TlsEccUtilities.ReadECParameters(mNamedCurves, mClientECPointFormats, input);

                byte[] point = TlsUtilities.ReadOpaque8(input);

                this.mECAgreePublicKey = TlsEccUtilities.ValidateECPublicKey(TlsEccUtilities.DeserializeECPublicKey(
                    mClientECPointFormats, ecParams, point));
            }
        }

        public override void ValidateCertificateRequest(CertificateRequest certificateRequest)
        {
            throw new TlsFatalAlert(AlertDescription.unexpected_message);
        }

        public override void ProcessClientCredentials(TlsCredentials clientCredentials)
        {
            throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        public override void GenerateClientKeyExchange(Stream output)
        {
            if (mPskIdentityHint == null)
            {
                mPskIdentity.SkipIdentityHint();
            }
            else
            {
                mPskIdentity.NotifyIdentityHint(mPskIdentityHint);
            }

            byte[] psk_identity = mPskIdentity.GetPskIdentity();
            if (psk_identity == null)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            this.mPsk = mPskIdentity.GetPsk();
            if (mPsk == null)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            TlsUtilities.WriteOpaque16(psk_identity, output);

            mContext.SecurityParameters.pskIdentity = psk_identity;

            if (this.mKeyExchange == KeyExchangeAlgorithm.DHE_PSK)
            {
                this.mDHAgreePrivateKey = TlsDHUtilities.GenerateEphemeralClientKeyExchange(mContext.SecureRandom,
                    mDHParameters, output);
            }
            else if (this.mKeyExchange == KeyExchangeAlgorithm.ECDHE_PSK)
            {
                this.mECAgreePrivateKey = TlsEccUtilities.GenerateEphemeralClientKeyExchange(mContext.SecureRandom,
                    mServerECPointFormats, mECAgreePublicKey.Parameters, output);
            }
            else if (this.mKeyExchange == KeyExchangeAlgorithm.RSA_PSK)
            {
                this.mPremasterSecret = TlsRsaUtilities.GenerateEncryptedPreMasterSecret(mContext,
                    this.mRsaServerPublicKey, output);
            }
        }

        public override void ProcessClientKeyExchange(Stream input)
        {
            byte[] psk_identity = TlsUtilities.ReadOpaque16(input);

            this.mPsk = mPskIdentityManager.GetPsk(psk_identity);
            if (mPsk == null)
                throw new TlsFatalAlert(AlertDescription.unknown_psk_identity);

            mContext.SecurityParameters.pskIdentity = psk_identity;

            if (this.mKeyExchange == KeyExchangeAlgorithm.DHE_PSK)
            {
                BigInteger Yc = TlsDHUtilities.ReadDHParameter(input);

                this.mDHAgreePublicKey = TlsDHUtilities.ValidateDHPublicKey(new DHPublicKeyParameters(Yc, mDHParameters));
            }
            else if (this.mKeyExchange == KeyExchangeAlgorithm.ECDHE_PSK)
            {
                byte[] point = TlsUtilities.ReadOpaque8(input);

                ECDomainParameters curve_params = this.mECAgreePrivateKey.Parameters;

                this.mECAgreePublicKey = TlsEccUtilities.ValidateECPublicKey(TlsEccUtilities.DeserializeECPublicKey(
                    mServerECPointFormats, curve_params, point));
            }
            else if (this.mKeyExchange == KeyExchangeAlgorithm.RSA_PSK)
            {
                byte[] encryptedPreMasterSecret;
                if (TlsUtilities.IsSsl(mContext))
                {
                    // TODO Do any SSLv3 clients actually include the length?
                    encryptedPreMasterSecret = Streams.ReadAll(input);
                }
                else
                {
                    encryptedPreMasterSecret = TlsUtilities.ReadOpaque16(input);
                }

                this.mPremasterSecret = mServerCredentials.DecryptPreMasterSecret(encryptedPreMasterSecret);
            }
        }

        public override byte[] GeneratePremasterSecret()
        {
            byte[] other_secret = GenerateOtherSecret(mPsk.Length);

            MemoryStream buf = new MemoryStream(4 + other_secret.Length + mPsk.Length);
            TlsUtilities.WriteOpaque16(other_secret, buf);
            TlsUtilities.WriteOpaque16(mPsk, buf);

            Arrays.Fill(mPsk, (byte)0);
            this.mPsk = null;

            return buf.ToArray();
        }

        protected virtual byte[] GenerateOtherSecret(int pskLength)
        {
            if (this.mKeyExchange == KeyExchangeAlgorithm.DHE_PSK)
            {
                if (mDHAgreePrivateKey != null)
                {
                    return TlsDHUtilities.CalculateDHBasicAgreement(mDHAgreePublicKey, mDHAgreePrivateKey);
                }

                throw new TlsFatalAlert(AlertDescription.internal_error);
            }

            if (this.mKeyExchange == KeyExchangeAlgorithm.ECDHE_PSK)
            {
                if (mECAgreePrivateKey != null)
                {
                    return TlsEccUtilities.CalculateECDHBasicAgreement(mECAgreePublicKey, mECAgreePrivateKey);
                }

                throw new TlsFatalAlert(AlertDescription.internal_error);
            }

            if (this.mKeyExchange == KeyExchangeAlgorithm.RSA_PSK)
            {
                return this.mPremasterSecret;
            }

            return new byte[pskLength];
        }

        protected virtual RsaKeyParameters ValidateRsaPublicKey(RsaKeyParameters key)
        {
            // TODO What is the minimum bit length required?
            // key.Modulus.BitLength;

            if (!key.Exponent.IsProbablePrime(2))
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            return key;
        }
    }
}
