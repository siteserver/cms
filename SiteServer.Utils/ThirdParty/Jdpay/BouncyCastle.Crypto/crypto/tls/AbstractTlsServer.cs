using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public abstract class AbstractTlsServer
        :   AbstractTlsPeer, TlsServer
    {
        protected TlsCipherFactory mCipherFactory;

        protected TlsServerContext mContext;

        protected ProtocolVersion mClientVersion;
        protected int[] mOfferedCipherSuites;
        protected byte[] mOfferedCompressionMethods;
        protected IDictionary mClientExtensions;

        protected bool mEncryptThenMacOffered;
        protected short mMaxFragmentLengthOffered;
        protected bool mTruncatedHMacOffered;
        protected IList mSupportedSignatureAlgorithms;
        protected bool mEccCipherSuitesOffered;
        protected int[] mNamedCurves;
        protected byte[] mClientECPointFormats, mServerECPointFormats;

        protected ProtocolVersion mServerVersion;
        protected int mSelectedCipherSuite;
        protected byte mSelectedCompressionMethod;
        protected IDictionary mServerExtensions;

        public AbstractTlsServer()
            :   this(new DefaultTlsCipherFactory())
        {
        }

        public AbstractTlsServer(TlsCipherFactory cipherFactory)
        {
            this.mCipherFactory = cipherFactory;
        }

        protected virtual bool AllowEncryptThenMac
        {
            get { return true; }
        }

        protected virtual bool AllowTruncatedHMac
        {
            get { return false; }
        }

        protected virtual IDictionary CheckServerExtensions()
        {
            return this.mServerExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(this.mServerExtensions);
        }

        protected abstract int[] GetCipherSuites();

        protected byte[] GetCompressionMethods()
        {
            return new byte[] { CompressionMethod.cls_null };
        }

        protected virtual ProtocolVersion MaximumVersion
        {
            get { return ProtocolVersion.TLSv11; }
        }

        protected virtual ProtocolVersion MinimumVersion
        {
            get { return ProtocolVersion.TLSv10; }
        }

        protected virtual bool SupportsClientEccCapabilities(int[] namedCurves, byte[] ecPointFormats)
        {
            // NOTE: BC supports all the current set of point formats so we don't check them here

            if (namedCurves == null)
            {
                /*
                 * RFC 4492 4. A client that proposes ECC cipher suites may choose not to include these
                 * extensions. In this case, the server is free to choose any one of the elliptic curves
                 * or point formats [...].
                 */
                return TlsEccUtilities.HasAnySupportedNamedCurves();
            }

            for (int i = 0; i < namedCurves.Length; ++i)
            {
                int namedCurve = namedCurves[i];
                if (NamedCurve.IsValid(namedCurve)
                    && (!NamedCurve.RefersToASpecificNamedCurve(namedCurve) || TlsEccUtilities.IsSupportedNamedCurve(namedCurve)))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void Init(TlsServerContext context)
        {
            this.mContext = context;
        }

        public virtual void NotifyClientVersion(ProtocolVersion clientVersion)
        {
            this.mClientVersion = clientVersion;
        }

        public virtual void NotifyFallback(bool isFallback)
        {
            /*
             * RFC 7507 3. If TLS_FALLBACK_SCSV appears in ClientHello.cipher_suites and the highest
             * protocol version supported by the server is higher than the version indicated in
             * ClientHello.client_version, the server MUST respond with a fatal inappropriate_fallback
             * alert [..].
             */
            if (isFallback && MaximumVersion.IsLaterVersionOf(mClientVersion))
                throw new TlsFatalAlert(AlertDescription.inappropriate_fallback);
        }

        public virtual void NotifyOfferedCipherSuites(int[] offeredCipherSuites)
        {
            this.mOfferedCipherSuites = offeredCipherSuites;
            this.mEccCipherSuitesOffered = TlsEccUtilities.ContainsEccCipherSuites(this.mOfferedCipherSuites);
        }

        public virtual void NotifyOfferedCompressionMethods(byte[] offeredCompressionMethods)
        {
            this.mOfferedCompressionMethods = offeredCompressionMethods;
        }

        public virtual void ProcessClientExtensions(IDictionary clientExtensions)
        {
            this.mClientExtensions = clientExtensions;

            if (clientExtensions != null)
            {
                this.mEncryptThenMacOffered = TlsExtensionsUtilities.HasEncryptThenMacExtension(clientExtensions);

                this.mMaxFragmentLengthOffered = TlsExtensionsUtilities.GetMaxFragmentLengthExtension(clientExtensions);
                if (mMaxFragmentLengthOffered >= 0 && !MaxFragmentLength.IsValid((byte)mMaxFragmentLengthOffered))
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                this.mTruncatedHMacOffered = TlsExtensionsUtilities.HasTruncatedHMacExtension(clientExtensions);

                this.mSupportedSignatureAlgorithms = TlsUtilities.GetSignatureAlgorithmsExtension(clientExtensions);
                if (this.mSupportedSignatureAlgorithms != null)
                {
                    /*
                     * RFC 5246 7.4.1.4.1. Note: this extension is not meaningful for TLS versions prior
                     * to 1.2. Clients MUST NOT offer it if they are offering prior versions.
                     */
                    if (!TlsUtilities.IsSignatureAlgorithmsExtensionAllowed(mClientVersion))
                        throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                }

                this.mNamedCurves = TlsEccUtilities.GetSupportedEllipticCurvesExtension(clientExtensions);
                this.mClientECPointFormats = TlsEccUtilities.GetSupportedPointFormatsExtension(clientExtensions);
            }

            /*
             * RFC 4429 4. The client MUST NOT include these extensions in the ClientHello message if it
             * does not propose any ECC cipher suites.
             * 
             * NOTE: This was overly strict as there may be ECC cipher suites that we don't recognize.
             * Also, draft-ietf-tls-negotiated-ff-dhe will be overloading the 'elliptic_curves'
             * extension to explicitly allow FFDHE (i.e. non-ECC) groups.
             */
            //if (!this.mEccCipherSuitesOffered && (this.mNamedCurves != null || this.mClientECPointFormats != null))
            //    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
        }

        public virtual ProtocolVersion GetServerVersion()
        {
            if (MinimumVersion.IsEqualOrEarlierVersionOf(mClientVersion))
            {
                ProtocolVersion maximumVersion = MaximumVersion;
                if (mClientVersion.IsEqualOrEarlierVersionOf(maximumVersion))
                {
                    return mServerVersion = mClientVersion;
                }
                if (mClientVersion.IsLaterVersionOf(maximumVersion))
                {
                    return mServerVersion = maximumVersion;
                }
            }
            throw new TlsFatalAlert(AlertDescription.protocol_version);
        }

        public virtual int GetSelectedCipherSuite()
        {
            /*
             * RFC 5246 7.4.3. In order to negotiate correctly, the server MUST check any candidate
             * cipher suites against the "signature_algorithms" extension before selecting them. This is
             * somewhat inelegant but is a compromise designed to minimize changes to the original
             * cipher suite design.
             */
            IList sigAlgs = TlsUtilities.GetUsableSignatureAlgorithms(this.mSupportedSignatureAlgorithms);

            /*
             * RFC 4429 5.1. A server that receives a ClientHello containing one or both of these
             * extensions MUST use the client's enumerated capabilities to guide its selection of an
             * appropriate cipher suite. One of the proposed ECC cipher suites must be negotiated only
             * if the server can successfully complete the handshake while using the curves and point
             * formats supported by the client [...].
             */
            bool eccCipherSuitesEnabled = SupportsClientEccCapabilities(this.mNamedCurves, this.mClientECPointFormats);

            int[] cipherSuites = GetCipherSuites();
            for (int i = 0; i < cipherSuites.Length; ++i)
            {
                int cipherSuite = cipherSuites[i];

                if (Arrays.Contains(this.mOfferedCipherSuites, cipherSuite)
                    && (eccCipherSuitesEnabled || !TlsEccUtilities.IsEccCipherSuite(cipherSuite))
                    && TlsUtilities.IsValidCipherSuiteForVersion(cipherSuite, mServerVersion)
                    && TlsUtilities.IsValidCipherSuiteForSignatureAlgorithms(cipherSuite, sigAlgs))
                {
                    return this.mSelectedCipherSuite = cipherSuite;
                }
            }
            throw new TlsFatalAlert(AlertDescription.handshake_failure);
        }

        public virtual byte GetSelectedCompressionMethod()
        {
            byte[] compressionMethods = GetCompressionMethods();
            for (int i = 0; i < compressionMethods.Length; ++i)
            {
                if (Arrays.Contains(mOfferedCompressionMethods, compressionMethods[i]))
                {
                    return this.mSelectedCompressionMethod = compressionMethods[i];
                }
            }
            throw new TlsFatalAlert(AlertDescription.handshake_failure);
        }

        // IDictionary is (Int32 -> byte[])
        public virtual IDictionary GetServerExtensions()
        {
            if (this.mEncryptThenMacOffered && AllowEncryptThenMac)
            {
                /*
                 * RFC 7366 3. If a server receives an encrypt-then-MAC request extension from a client
                 * and then selects a stream or Authenticated Encryption with Associated Data (AEAD)
                 * ciphersuite, it MUST NOT send an encrypt-then-MAC response extension back to the
                 * client.
                 */
                if (TlsUtilities.IsBlockCipherSuite(this.mSelectedCipherSuite))
                {
                    TlsExtensionsUtilities.AddEncryptThenMacExtension(CheckServerExtensions());
                }
            }

            if (this.mMaxFragmentLengthOffered >= 0
                && TlsUtilities.IsValidUint8(mMaxFragmentLengthOffered)
                && MaxFragmentLength.IsValid((byte)mMaxFragmentLengthOffered))
            {
                TlsExtensionsUtilities.AddMaxFragmentLengthExtension(CheckServerExtensions(), (byte)mMaxFragmentLengthOffered);
            }

            if (this.mTruncatedHMacOffered && AllowTruncatedHMac)
            {
                TlsExtensionsUtilities.AddTruncatedHMacExtension(CheckServerExtensions());
            }

            if (this.mClientECPointFormats != null && TlsEccUtilities.IsEccCipherSuite(this.mSelectedCipherSuite))
            {
                /*
                 * RFC 4492 5.2. A server that selects an ECC cipher suite in response to a ClientHello
                 * message including a Supported Point Formats Extension appends this extension (along
                 * with others) to its ServerHello message, enumerating the point formats it can parse.
                 */
                this.mServerECPointFormats = new byte[]{ ECPointFormat.uncompressed,
                    ECPointFormat.ansiX962_compressed_prime, ECPointFormat.ansiX962_compressed_char2, };

                TlsEccUtilities.AddSupportedPointFormatsExtension(CheckServerExtensions(), mServerECPointFormats);
            }

            return mServerExtensions;
        }

        public virtual IList GetServerSupplementalData()
        {
            return null;
        }

        public abstract TlsCredentials GetCredentials();

        public virtual CertificateStatus GetCertificateStatus()
        {
            return null;
        }

        public abstract TlsKeyExchange GetKeyExchange();

        public virtual CertificateRequest GetCertificateRequest()
        {
            return null;
        }

        public virtual void ProcessClientSupplementalData(IList clientSupplementalData)
        {
            if (clientSupplementalData != null)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
        }

        public virtual void NotifyClientCertificate(Certificate clientCertificate)
        {
            throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        public override TlsCompression GetCompression()
        {
            switch (mSelectedCompressionMethod)
            {
            case CompressionMethod.cls_null:
                return new TlsNullCompression();

            default:
                /*
                 * Note: internal error here; we selected the compression method, so if we now can't
                 * produce an implementation, we shouldn't have chosen it!
                 */
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        public override TlsCipher GetCipher()
        {
            int encryptionAlgorithm = TlsUtilities.GetEncryptionAlgorithm(mSelectedCipherSuite);
            int macAlgorithm = TlsUtilities.GetMacAlgorithm(mSelectedCipherSuite);

            return mCipherFactory.CreateCipher(mContext, encryptionAlgorithm, macAlgorithm);
        }

        public virtual NewSessionTicket GetNewSessionTicket()
        {
            /*
             * RFC 5077 3.3. If the server determines that it does not want to include a ticket after it
             * has included the SessionTicket extension in the ServerHello, then it sends a zero-length
             * ticket in the NewSessionTicket handshake message.
             */
            return new NewSessionTicket(0L, TlsUtilities.EmptyBytes);
        }
    }
}
