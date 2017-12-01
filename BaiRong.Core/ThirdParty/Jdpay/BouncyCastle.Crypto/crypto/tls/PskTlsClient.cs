using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class PskTlsClient
        :   AbstractTlsClient
    {
        protected TlsPskIdentity mPskIdentity;

        public PskTlsClient(TlsPskIdentity pskIdentity)
            :   this(new DefaultTlsCipherFactory(), pskIdentity)
        {
        }

        public PskTlsClient(TlsCipherFactory cipherFactory, TlsPskIdentity pskIdentity)
            :   base(cipherFactory)
        {
            this.mPskIdentity = pskIdentity;
        }

        public override int[] GetCipherSuites()
        {
            return new int[]
            {
                CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA
            };
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

        public override TlsAuthentication GetAuthentication()
        {
            /*
             * Note: This method is not called unless a server certificate is sent, which may be the
             * case e.g. for RSA_PSK key exchange.
             */
            throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        protected virtual TlsKeyExchange CreatePskKeyExchange(int keyExchange)
        {
            return new TlsPskKeyExchange(keyExchange, mSupportedSignatureAlgorithms, mPskIdentity, null, null, mNamedCurves,
                mClientECPointFormats, mServerECPointFormats);
        }
    }
}
