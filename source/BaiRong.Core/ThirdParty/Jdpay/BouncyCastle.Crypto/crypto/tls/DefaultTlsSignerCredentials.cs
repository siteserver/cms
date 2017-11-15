using System;
using System.IO;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class DefaultTlsSignerCredentials
        :   AbstractTlsSignerCredentials
    {
        protected readonly TlsContext mContext;
        protected readonly Certificate mCertificate;
        protected readonly AsymmetricKeyParameter mPrivateKey;
        protected readonly SignatureAndHashAlgorithm mSignatureAndHashAlgorithm;

        protected readonly TlsSigner mSigner;

        public DefaultTlsSignerCredentials(TlsContext context, Certificate certificate, AsymmetricKeyParameter privateKey)
            :   this(context, certificate, privateKey, null)
        {
        }

        public DefaultTlsSignerCredentials(TlsContext context, Certificate certificate, AsymmetricKeyParameter privateKey,
            SignatureAndHashAlgorithm signatureAndHashAlgorithm)
        {
            if (certificate == null)
                throw new ArgumentNullException("certificate");
            if (certificate.IsEmpty)
                throw new ArgumentException("cannot be empty", "clientCertificate");
            if (privateKey == null)
                throw new ArgumentNullException("privateKey");
            if (!privateKey.IsPrivate)
                throw new ArgumentException("must be private", "privateKey");
            if (TlsUtilities.IsTlsV12(context) && signatureAndHashAlgorithm == null)
                throw new ArgumentException("cannot be null for (D)TLS 1.2+", "signatureAndHashAlgorithm");

            if (privateKey is RsaKeyParameters)
            {
                mSigner = new TlsRsaSigner();
            }
            else if (privateKey is DsaPrivateKeyParameters)
            {
                mSigner = new TlsDssSigner();
            }
            else if (privateKey is ECPrivateKeyParameters)
            {
                mSigner = new TlsECDsaSigner();
            }
            else
            {
                throw new ArgumentException("type not supported: " + Platform.GetTypeName(privateKey), "privateKey");
            }

            this.mSigner.Init(context);

            this.mContext = context;
            this.mCertificate = certificate;
            this.mPrivateKey = privateKey;
            this.mSignatureAndHashAlgorithm = signatureAndHashAlgorithm;
        }

        public override Certificate Certificate
        {
            get { return mCertificate; }
        }

        /// <exception cref="IOException"></exception>
        public override byte[] GenerateCertificateSignature(byte[] hash)
        {
            try
            {
                if (TlsUtilities.IsTlsV12(mContext))
                {
                    return mSigner.GenerateRawSignature(mSignatureAndHashAlgorithm, mPrivateKey, hash);
                }
                else
                {
                    return mSigner.GenerateRawSignature(mPrivateKey, hash);
                }
            }
            catch (CryptoException e)
            {
                throw new TlsFatalAlert(AlertDescription.internal_error, e);
            }
        }

        public override SignatureAndHashAlgorithm SignatureAndHashAlgorithm
        {
            get { return mSignatureAndHashAlgorithm; }
        }
    }
}
