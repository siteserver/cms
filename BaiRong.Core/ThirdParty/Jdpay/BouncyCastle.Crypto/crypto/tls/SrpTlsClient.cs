using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class SrpTlsClient
        :   AbstractTlsClient
    {
        protected TlsSrpGroupVerifier mGroupVerifier;

        protected byte[] mIdentity;
        protected byte[] mPassword;

        public SrpTlsClient(byte[] identity, byte[] password)
            :   this(new DefaultTlsCipherFactory(), new DefaultTlsSrpGroupVerifier(), identity, password)
        {
        }

        public SrpTlsClient(TlsCipherFactory cipherFactory, byte[] identity, byte[] password)
            :   this(cipherFactory, new DefaultTlsSrpGroupVerifier(), identity, password)
        {
        }

        public SrpTlsClient(TlsCipherFactory cipherFactory, TlsSrpGroupVerifier groupVerifier,
            byte[] identity, byte[] password)
            :   base(cipherFactory)
        {
            this.mGroupVerifier = groupVerifier;
            this.mIdentity = Arrays.Clone(identity);
            this.mPassword = Arrays.Clone(password);
        }

        protected virtual bool RequireSrpServerExtension
        {
            // No explicit guidance in RFC 5054; by default an (empty) extension from server is optional
            get { return false; }
        }

        public override int[] GetCipherSuites()
        {
            return new int[]
            {
                CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_128_CBC_SHA
            };
        }

        public override IDictionary GetClientExtensions()
        {
            IDictionary clientExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(base.GetClientExtensions());
            TlsSrpUtilities.AddSrpExtension(clientExtensions, this.mIdentity);
            return clientExtensions;
        }

        public override void ProcessServerExtensions(IDictionary serverExtensions)
        {
            if (!TlsUtilities.HasExpectedEmptyExtensionData(serverExtensions, ExtensionType.srp,
                AlertDescription.illegal_parameter))
            {
                if (RequireSrpServerExtension)
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }

            base.ProcessServerExtensions(serverExtensions);
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

        public override TlsAuthentication GetAuthentication()
        {
            /*
             * Note: This method is not called unless a server certificate is sent, which may be the
             * case e.g. for SRP_DSS or SRP_RSA key exchange.
             */
            throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        protected virtual TlsKeyExchange CreateSrpKeyExchange(int keyExchange)
        {
            return new TlsSrpKeyExchange(keyExchange, mSupportedSignatureAlgorithms, mGroupVerifier, mIdentity, mPassword);
        }
    }
}
