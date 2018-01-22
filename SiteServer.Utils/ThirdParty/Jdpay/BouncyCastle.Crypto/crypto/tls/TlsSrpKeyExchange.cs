using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>(D)TLS SRP key exchange (RFC 5054).</summary>
    public class TlsSrpKeyExchange
        :   AbstractTlsKeyExchange
    {
        protected static TlsSigner CreateSigner(int keyExchange)
        {
            switch (keyExchange)
            {
                case KeyExchangeAlgorithm.SRP:
                    return null;
                case KeyExchangeAlgorithm.SRP_RSA:
                    return new TlsRsaSigner();
                case KeyExchangeAlgorithm.SRP_DSS:
                    return new TlsDssSigner();
                default:
                    throw new ArgumentException("unsupported key exchange algorithm");
            }
        }

        protected TlsSigner mTlsSigner;
        protected TlsSrpGroupVerifier mGroupVerifier;
        protected byte[] mIdentity;
        protected byte[] mPassword;

        protected AsymmetricKeyParameter mServerPublicKey = null;

        protected Srp6GroupParameters mSrpGroup = null;
        protected Srp6Client mSrpClient = null;
        protected Srp6Server mSrpServer = null;
        protected BigInteger mSrpPeerCredentials = null;
        protected BigInteger mSrpVerifier = null;
        protected byte[] mSrpSalt = null;

        protected TlsSignerCredentials mServerCredentials = null;

        [Obsolete("Use constructor taking an explicit 'groupVerifier' argument")]
        public TlsSrpKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, byte[] identity, byte[] password)
            :   this(keyExchange, supportedSignatureAlgorithms, new DefaultTlsSrpGroupVerifier(), identity, password)
        {
        }

        public TlsSrpKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, TlsSrpGroupVerifier groupVerifier,
            byte[] identity, byte[] password)
            :   base(keyExchange, supportedSignatureAlgorithms)
        {
            this.mTlsSigner = CreateSigner(keyExchange);
            this.mGroupVerifier = groupVerifier;
            this.mIdentity = identity;
            this.mPassword = password;
            this.mSrpClient = new Srp6Client();
        }

        public TlsSrpKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, byte[] identity,
            TlsSrpLoginParameters loginParameters)
            :   base(keyExchange, supportedSignatureAlgorithms)
        {
            this.mTlsSigner = CreateSigner(keyExchange);
            this.mIdentity = identity;
            this.mSrpServer = new Srp6Server();
            this.mSrpGroup = loginParameters.Group;
            this.mSrpVerifier = loginParameters.Verifier;
            this.mSrpSalt = loginParameters.Salt;
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
            if (mTlsSigner != null)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
        }

        public override void ProcessServerCertificate(Certificate serverCertificate)
        {
            if (mTlsSigner == null)
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

            if (!mTlsSigner.IsValidPublicKey(this.mServerPublicKey))
                throw new TlsFatalAlert(AlertDescription.certificate_unknown);

            TlsUtilities.ValidateKeyUsage(x509Cert, KeyUsage.DigitalSignature);

            base.ProcessServerCertificate(serverCertificate);
        }

        public override void ProcessServerCredentials(TlsCredentials serverCredentials)
        {
            if ((mKeyExchange == KeyExchangeAlgorithm.SRP) || !(serverCredentials is TlsSignerCredentials))
                throw new TlsFatalAlert(AlertDescription.internal_error);

            ProcessServerCertificate(serverCredentials.Certificate);

            this.mServerCredentials = (TlsSignerCredentials)serverCredentials;
        }

        public override bool RequiresServerKeyExchange
        {
            get { return true; }
        }

        public override byte[] GenerateServerKeyExchange()
        {
            mSrpServer.Init(mSrpGroup, mSrpVerifier, TlsUtilities.CreateHash(HashAlgorithm.sha1), mContext.SecureRandom);
            BigInteger B = mSrpServer.GenerateServerCredentials();

            ServerSrpParams srpParams = new ServerSrpParams(mSrpGroup.N, mSrpGroup.G, mSrpSalt, B);

            DigestInputBuffer buf = new DigestInputBuffer();

            srpParams.Encode(buf);

            if (mServerCredentials != null)
            {
                /*
                 * RFC 5246 4.7. digitally-signed element needs SignatureAndHashAlgorithm from TLS 1.2
                 */
                SignatureAndHashAlgorithm signatureAndHashAlgorithm = TlsUtilities.GetSignatureAndHashAlgorithm(
                    mContext, mServerCredentials);

                IDigest d = TlsUtilities.CreateHash(signatureAndHashAlgorithm);

                SecurityParameters securityParameters = mContext.SecurityParameters;
                d.BlockUpdate(securityParameters.clientRandom, 0, securityParameters.clientRandom.Length);
                d.BlockUpdate(securityParameters.serverRandom, 0, securityParameters.serverRandom.Length);
                buf.UpdateDigest(d);

                byte[] hash = new byte[d.GetDigestSize()];
                d.DoFinal(hash, 0);

                byte[] signature = mServerCredentials.GenerateCertificateSignature(hash);

                DigitallySigned signed_params = new DigitallySigned(signatureAndHashAlgorithm, signature);
                signed_params.Encode(buf);
            }

            return buf.ToArray();
        }

        public override void ProcessServerKeyExchange(Stream input)
        {
            SecurityParameters securityParameters = mContext.SecurityParameters;

            SignerInputBuffer buf = null;
            Stream teeIn = input;

            if (mTlsSigner != null)
            {
                buf = new SignerInputBuffer();
                teeIn = new TeeInputStream(input, buf);
            }

            ServerSrpParams srpParams = ServerSrpParams.Parse(teeIn);

            if (buf != null)
            {
                DigitallySigned signed_params = ParseSignature(input);

                ISigner signer = InitVerifyer(mTlsSigner, signed_params.Algorithm, securityParameters);
                buf.UpdateSigner(signer);
                if (!signer.VerifySignature(signed_params.Signature))
                    throw new TlsFatalAlert(AlertDescription.decrypt_error);
            }

            this.mSrpGroup = new Srp6GroupParameters(srpParams.N, srpParams.G);

            if (!mGroupVerifier.Accept(mSrpGroup))
                throw new TlsFatalAlert(AlertDescription.insufficient_security);

            this.mSrpSalt = srpParams.S;

            /*
             * RFC 5054 2.5.3: The client MUST abort the handshake with an "illegal_parameter" alert if
             * B % N = 0.
             */
            try
            {
                this.mSrpPeerCredentials = Srp6Utilities.ValidatePublicValue(mSrpGroup.N, srpParams.B);
            }
            catch (CryptoException e)
            {
                throw new TlsFatalAlert(AlertDescription.illegal_parameter, e);
            }

            this.mSrpClient.Init(mSrpGroup, TlsUtilities.CreateHash(HashAlgorithm.sha1), mContext.SecureRandom);
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
            BigInteger A = mSrpClient.GenerateClientCredentials(mSrpSalt, mIdentity, mPassword);
            TlsSrpUtilities.WriteSrpParameter(A, output);

            mContext.SecurityParameters.srpIdentity = Arrays.Clone(mIdentity);
        }

        public override void ProcessClientKeyExchange(Stream input)
        {
            /*
             * RFC 5054 2.5.4: The server MUST abort the handshake with an "illegal_parameter" alert if
             * A % N = 0.
             */
            try
            {
                this.mSrpPeerCredentials = Srp6Utilities.ValidatePublicValue(mSrpGroup.N, TlsSrpUtilities.ReadSrpParameter(input));
            }
            catch (CryptoException e)
            {
                throw new TlsFatalAlert(AlertDescription.illegal_parameter, e);
            }

            mContext.SecurityParameters.srpIdentity = Arrays.Clone(mIdentity);
        }

        public override byte[] GeneratePremasterSecret()
        {
            try
            {
                BigInteger S = mSrpServer != null
                    ?   mSrpServer.CalculateSecret(mSrpPeerCredentials)
                    :   mSrpClient.CalculateSecret(mSrpPeerCredentials);

                // TODO Check if this needs to be a fixed size
                return BigIntegers.AsUnsignedByteArray(S);
            }
            catch (CryptoException e)
            {
                throw new TlsFatalAlert(AlertDescription.illegal_parameter, e);
            }
        }

        protected virtual ISigner InitVerifyer(TlsSigner tlsSigner, SignatureAndHashAlgorithm algorithm,
            SecurityParameters securityParameters)
        {
            ISigner signer = tlsSigner.CreateVerifyer(algorithm, this.mServerPublicKey);
            signer.BlockUpdate(securityParameters.clientRandom, 0, securityParameters.clientRandom.Length);
            signer.BlockUpdate(securityParameters.serverRandom, 0, securityParameters.serverRandom.Length);
            return signer;
        }
    }
}
