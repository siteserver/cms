using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public abstract class AbstractTlsAgreementCredentials
        :   AbstractTlsCredentials, TlsAgreementCredentials
    {
        /// <exception cref="IOException"></exception>
        public abstract byte[] GenerateAgreement(AsymmetricKeyParameter peerPublicKey);
    }
}
