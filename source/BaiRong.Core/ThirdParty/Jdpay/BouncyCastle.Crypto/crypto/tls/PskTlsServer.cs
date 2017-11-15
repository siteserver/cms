using System;

using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class PskTlsServer
        :   AbstractTlsServer
    {
        protected TlsPskIdentityManager mPskIdentityManager;

        public PskTlsServer(TlsPskIdentityManager pskIdentityManager)
            :   this(new DefaultTlsCipherFactory(), pskIdentityManager)
        {
        }

        public PskTlsServer(TlsCipherFactory cipherFactory, TlsPskIdentityManager pskIdentityManager)
            :   base(cipherFactory)
        {
            this.mPskIdentityManager = pskIdentityManager;
        }

        protected virtual TlsEncryptionCredentials GetRsaEncryptionCredentials()
        {
            throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        protected virtual DHParameters GetDHParameters()
        {
            return DHStandardGroups.rfc7919_ffdhe2048;
        }

        protected override int[] GetCipherSuites()
        {
            return new int[]
            {
                CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA
            };
        }

        public override TlsCredentials GetCredentials()
        {
            int keyExchangeAlgorithm = TlsUtilities.GetKeyExchangeAlgorithm(mSelectedCipherSuite);

            switch (keyExchangeAlgorithm)
            {
                case KeyExchangeAlgorithm.DHE_PSK:
                case KeyExchangeAlgorithm.ECDHE_PSK:
                case KeyExchangeAlgorithm.PSK:
                    return null;

                case KeyExchangeAlgorithm.RSA_PSK:
                    return GetRsaEncryptionCredentials();

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
                case KeyExchangeAlgorithm.DHE_PSK:
                case KeyExchangeAlgorithm.ECDHE_PSK:
                case KeyExchangeAlgorithm.PSK:
                case KeyExchangeAlgorithm.RSA_PSK:
                    return CreatePskKeyExchange(keyExchangeAlgorithm);

                default:
                    /*
                     * Note: internal error here; the TlsProtocol implementation verifies that the
                     * server-selected cipher suite was in the list of client-offered cipher suites, so if
                     * we now can't produce an implementation, we shouldn't have offered it!
                     */
                    throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        protected virtual TlsKeyExchange CreatePskKeyExchange(int keyExchange)
        {
            return new TlsPskKeyExchange(keyExchange, mSupportedSignatureAlgorithms, null, mPskIdentityManager,
                GetDHParameters(), mNamedCurves, mClientECPointFormats, mServerECPointFormats);
        }
    }
}
