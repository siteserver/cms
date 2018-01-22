using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class SrpTlsServer
        :   AbstractTlsServer
    {
        protected TlsSrpIdentityManager mSrpIdentityManager;

        protected byte[] mSrpIdentity = null;
        protected TlsSrpLoginParameters mLoginParameters = null;

        public SrpTlsServer(TlsSrpIdentityManager srpIdentityManager)
            :   this(new DefaultTlsCipherFactory(), srpIdentityManager)
        {
        }

        public SrpTlsServer(TlsCipherFactory cipherFactory, TlsSrpIdentityManager srpIdentityManager)
            :   base(cipherFactory)
        {
            this.mSrpIdentityManager = srpIdentityManager;
        }

        protected virtual TlsSignerCredentials GetDsaSignerCredentials()
        {
            throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        protected virtual TlsSignerCredentials GetRsaSignerCredentials()
        {
            throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        protected override int[] GetCipherSuites()
        {
            return new int[]
            {
                CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_256_CBC_SHA,
                CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_256_CBC_SHA,
                CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_SRP_SHA_WITH_AES_256_CBC_SHA,
                CipherSuite.TLS_SRP_SHA_WITH_AES_128_CBC_SHA
            };
        }

        public override void ProcessClientExtensions(IDictionary clientExtensions)
        {
            base.ProcessClientExtensions(clientExtensions);

            this.mSrpIdentity = TlsSrpUtilities.GetSrpExtension(clientExtensions);
        }

        public override int GetSelectedCipherSuite()
        {
            int cipherSuite = base.GetSelectedCipherSuite();

            if (TlsSrpUtilities.IsSrpCipherSuite(cipherSuite))
            {
                if (mSrpIdentity != null)
                {
                    this.mLoginParameters = mSrpIdentityManager.GetLoginParameters(mSrpIdentity);
                }

                if (mLoginParameters == null)
                    throw new TlsFatalAlert(AlertDescription.unknown_psk_identity);
            }

            return cipherSuite;
        }

        public override TlsCredentials GetCredentials()
        {
            int keyExchangeAlgorithm = TlsUtilities.GetKeyExchangeAlgorithm(mSelectedCipherSuite);

            switch (keyExchangeAlgorithm)
            {
                case KeyExchangeAlgorithm.SRP:
                    return null;

                case KeyExchangeAlgorithm.SRP_DSS:
                    return GetDsaSignerCredentials();

                case KeyExchangeAlgorithm.SRP_RSA:
                    return GetRsaSignerCredentials();

                default:
                    /* Note: internal error here; selected a key exchange we don't implement! */
                    throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        public override TlsKeyExchange GetKeyExchange()
        {
            int keyExchangeAlgorithm = TlsUtilities.GetKeyExchangeAlgorithm(mSelectedCipherSuite);

            switch (keyExchangeAlgorithm)
            {
            case KeyExchangeAlgorithm.SRP:
            case KeyExchangeAlgorithm.SRP_DSS:
            case KeyExchangeAlgorithm.SRP_RSA:
                return CreateSrpKeyExchange(keyExchangeAlgorithm);

            default:
                /*
                 * Note: internal error here; the TlsProtocol implementation verifies that the
                 * server-selected cipher suite was in the list of client-offered cipher suites, so if
                 * we now can't produce an implementation, we shouldn't have offered it!
                 */
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        protected virtual TlsKeyExchange CreateSrpKeyExchange(int keyExchange)
        {
            return new TlsSrpKeyExchange(keyExchange, mSupportedSignatureAlgorithms, mSrpIdentity, mLoginParameters);
        }
    }
}
