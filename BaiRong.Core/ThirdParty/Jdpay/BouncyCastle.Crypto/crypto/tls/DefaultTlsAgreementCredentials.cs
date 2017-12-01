using System;
using System.IO;

using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class DefaultTlsAgreementCredentials
        : AbstractTlsAgreementCredentials
    {
        protected readonly Certificate mCertificate;
        protected readonly AsymmetricKeyParameter mPrivateKey;

        protected readonly IBasicAgreement mBasicAgreement;
        protected readonly bool mTruncateAgreement;

        public DefaultTlsAgreementCredentials(Certificate certificate, AsymmetricKeyParameter privateKey)
        {
            if (certificate == null)
                throw new ArgumentNullException("certificate");
            if (certificate.IsEmpty)
                throw new ArgumentException("cannot be empty", "certificate");
            if (privateKey == null)
                throw new ArgumentNullException("privateKey");
            if (!privateKey.IsPrivate)
                throw new ArgumentException("must be private", "privateKey");

            if (privateKey is DHPrivateKeyParameters)
            {
                mBasicAgreement = new DHBasicAgreement();
                mTruncateAgreement = true;
            }
            else if (privateKey is ECPrivateKeyParameters)
            {
                mBasicAgreement = new ECDHBasicAgreement();
                mTruncateAgreement = false;
            }
            else
            {
                throw new ArgumentException("type not supported: " + Platform.GetTypeName(privateKey), "privateKey");
            }

            this.mCertificate = certificate;
            this.mPrivateKey = privateKey;
        }

        public override Certificate Certificate
        {
            get { return mCertificate; }
        }

        /// <exception cref="IOException"></exception>
        public override byte[] GenerateAgreement(AsymmetricKeyParameter peerPublicKey)
        {
            mBasicAgreement.Init(mPrivateKey);
            BigInteger agreementValue = mBasicAgreement.CalculateAgreement(peerPublicKey);

            if (mTruncateAgreement)
            {
                return BigIntegers.AsUnsignedByteArray(agreementValue);
            }

            return BigIntegers.AsUnsignedByteArray(mBasicAgreement.GetFieldSize(), agreementValue);
        }
    }
}
