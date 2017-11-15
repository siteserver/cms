using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class TlsClientProtocol
        :   TlsProtocol
    {
        protected TlsClient mTlsClient = null;
        internal TlsClientContextImpl mTlsClientContext = null;

        protected byte[] mSelectedSessionID = null;

        protected TlsKeyExchange mKeyExchange = null;
        protected TlsAuthentication mAuthentication = null;

        protected CertificateStatus mCertificateStatus = null;
        protected CertificateRequest mCertificateRequest = null;

        /**
         * Constructor for blocking mode.
         * @param stream The bi-directional stream of data to/from the server
         * @param secureRandom Random number generator for various cryptographic functions
         */
        public TlsClientProtocol(Stream stream, SecureRandom secureRandom)
            : base(stream, secureRandom)
        {
        }

        /**
         * Constructor for blocking mode.
         * @param input The stream of data from the server
         * @param output The stream of data to the server
         * @param secureRandom Random number generator for various cryptographic functions
         */
        public TlsClientProtocol(Stream input, Stream output, SecureRandom secureRandom)
            : base(input, output, secureRandom)
        {
        }

        /**
         * Constructor for non-blocking mode.<br/>
         * <br/>
         * When data is received, use {@link #offerInput(java.nio.ByteBuffer)} to
         * provide the received ciphertext, then use
         * {@link #readInput(byte[], int, int)} to read the corresponding cleartext.<br/>
         * <br/>
         * Similarly, when data needs to be sent, use
         * {@link #offerOutput(byte[], int, int)} to provide the cleartext, then use
         * {@link #readOutput(byte[], int, int)} to get the corresponding
         * ciphertext.
         * 
         * @param secureRandom
         *            Random number generator for various cryptographic functions
         */
        public TlsClientProtocol(SecureRandom secureRandom)
            : base(secureRandom)
        {
        }

        /**
         * Initiates a TLS handshake in the role of client.<br/>
         * <br/>
         * In blocking mode, this will not return until the handshake is complete.
         * In non-blocking mode, use {@link TlsPeer#NotifyHandshakeComplete()} to
         * receive a callback when the handshake is complete.
         *
         * @param tlsClient The {@link TlsClient} to use for the handshake.
         * @throws IOException If in blocking mode and handshake was not successful.
         */
        public virtual void Connect(TlsClient tlsClient)
        {
            if (tlsClient == null)
                throw new ArgumentNullException("tlsClient");
            if (this.mTlsClient != null)
                throw new InvalidOperationException("'Connect' can only be called once");

            this.mTlsClient = tlsClient;

            this.mSecurityParameters = new SecurityParameters();
            this.mSecurityParameters.entity = ConnectionEnd.client;

            this.mTlsClientContext = new TlsClientContextImpl(mSecureRandom, mSecurityParameters);

            this.mSecurityParameters.clientRandom = CreateRandomBlock(tlsClient.ShouldUseGmtUnixTime(),
                mTlsClientContext.NonceRandomGenerator);

            this.mTlsClient.Init(mTlsClientContext);
            this.mRecordStream.Init(mTlsClientContext);

            TlsSession sessionToResume = tlsClient.GetSessionToResume();
            if (sessionToResume != null && sessionToResume.IsResumable)
            {
                SessionParameters sessionParameters = sessionToResume.ExportSessionParameters();
                if (sessionParameters != null)
                {
                    this.mTlsSession = sessionToResume;
                    this.mSessionParameters = sessionParameters;
                }
            }

            SendClientHelloMessage();
            this.mConnectionState = CS_CLIENT_HELLO;

            BlockForHandshake();
        }

        protected override void CleanupHandshake()
        {
            base.CleanupHandshake();

            this.mSelectedSessionID = null;
            this.mKeyExchange = null;
            this.mAuthentication = null;
            this.mCertificateStatus = null;
            this.mCertificateRequest = null;
        }

        protected override TlsContext Context
        {
            get { return mTlsClientContext; }
        }

        internal override AbstractTlsContext ContextAdmin
        {
            get { return mTlsClientContext; }
        }

        protected override TlsPeer Peer
        {
            get { return mTlsClient; }
        }

        protected override void HandleHandshakeMessage(byte type, MemoryStream buf)
        {
            if (this.mResumedSession)
            {
                if (type != HandshakeType.finished || this.mConnectionState != CS_SERVER_HELLO)
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);

                ProcessFinishedMessage(buf);
                this.mConnectionState = CS_SERVER_FINISHED;

                SendChangeCipherSpecMessage();
                SendFinishedMessage();
                this.mConnectionState = CS_CLIENT_FINISHED;

                CompleteHandshake();
                return;
            }

            switch (type)
            {
            case HandshakeType.certificate:
            {
                switch (this.mConnectionState)
                {
                case CS_SERVER_HELLO:
                case CS_SERVER_SUPPLEMENTAL_DATA:
                {
                    if (this.mConnectionState == CS_SERVER_HELLO)
                    {
                        HandleSupplementalData(null);
                    }

                    // Parse the Certificate message and Send to cipher suite

                    this.mPeerCertificate = Certificate.Parse(buf);

                    AssertEmpty(buf);

                    // TODO[RFC 3546] Check whether empty certificates is possible, allowed, or excludes CertificateStatus
                    if (this.mPeerCertificate == null || this.mPeerCertificate.IsEmpty)
                    {
                        this.mAllowCertificateStatus = false;
                    }

                    this.mKeyExchange.ProcessServerCertificate(this.mPeerCertificate);

                    this.mAuthentication = mTlsClient.GetAuthentication();
                    this.mAuthentication.NotifyServerCertificate(this.mPeerCertificate);

                    break;
                }
                default:
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }

                this.mConnectionState = CS_SERVER_CERTIFICATE;
                break;
            }
            case HandshakeType.certificate_status:
            {
                switch (this.mConnectionState)
                {
                case CS_SERVER_CERTIFICATE:
                {
                    if (!this.mAllowCertificateStatus)
                    {
                        /*
                         * RFC 3546 3.6. If a server returns a "CertificateStatus" message, then the
                         * server MUST have included an extension of type "status_request" with empty
                         * "extension_data" in the extended server hello..
                         */
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);
                    }

                    this.mCertificateStatus = CertificateStatus.Parse(buf);

                    AssertEmpty(buf);

                    // TODO[RFC 3546] Figure out how to provide this to the client/authentication.

                    this.mConnectionState = CS_CERTIFICATE_STATUS;
                    break;
                }
                default:
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }
                break;
            }
            case HandshakeType.finished:
            {
                switch (this.mConnectionState)
                {
                case CS_CLIENT_FINISHED:
                case CS_SERVER_SESSION_TICKET:
                {
                    if (this.mConnectionState == CS_CLIENT_FINISHED && this.mExpectSessionTicket)
                    {
                        /*
                         * RFC 5077 3.3. This message MUST be sent if the server included a
                         * SessionTicket extension in the ServerHello.
                         */
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);
                    }

                    ProcessFinishedMessage(buf);
                    this.mConnectionState = CS_SERVER_FINISHED;

                    CompleteHandshake();
                    break;
                }
                default:
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }
                break;
            }
            case HandshakeType.server_hello:
            {
                switch (this.mConnectionState)
                {
                case CS_CLIENT_HELLO:
                {
                    ReceiveServerHelloMessage(buf);
                    this.mConnectionState = CS_SERVER_HELLO;

                    this.mRecordStream.NotifyHelloComplete();

                    ApplyMaxFragmentLengthExtension();

                    if (this.mResumedSession)
                    {
                        this.mSecurityParameters.masterSecret = Arrays.Clone(this.mSessionParameters.MasterSecret);
                        this.mRecordStream.SetPendingConnectionState(Peer.GetCompression(), Peer.GetCipher());
                    }
                    else
                    {
                        InvalidateSession();

                        if (this.mSelectedSessionID.Length > 0)
                        {
                            this.mTlsSession = new TlsSessionImpl(this.mSelectedSessionID, null);
                        }
                    }

                    break;
                }
                default:
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }
                break;
            }
            case HandshakeType.supplemental_data:
            {
                switch (this.mConnectionState)
                {
                case CS_SERVER_HELLO:
                {
                    HandleSupplementalData(ReadSupplementalDataMessage(buf));
                    break;
                }
                default:
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }
                break;
            }
            case HandshakeType.server_hello_done:
            {
                switch (this.mConnectionState)
                {
                case CS_SERVER_HELLO:
                case CS_SERVER_SUPPLEMENTAL_DATA:
                case CS_SERVER_CERTIFICATE:
                case CS_CERTIFICATE_STATUS:
                case CS_SERVER_KEY_EXCHANGE:
                case CS_CERTIFICATE_REQUEST:
                {
                    if (mConnectionState < CS_SERVER_SUPPLEMENTAL_DATA)
                    {
                        HandleSupplementalData(null);
                    }

                    if (mConnectionState < CS_SERVER_CERTIFICATE)
                    {
                        // There was no server certificate message; check it's OK
                        this.mKeyExchange.SkipServerCredentials();
                        this.mAuthentication = null;
                    }

                    if (mConnectionState < CS_SERVER_KEY_EXCHANGE)
                    {
                        // There was no server key exchange message; check it's OK
                        this.mKeyExchange.SkipServerKeyExchange();
                    }

                    AssertEmpty(buf);

                    this.mConnectionState = CS_SERVER_HELLO_DONE;

                    this.mRecordStream.HandshakeHash.SealHashAlgorithms();

                    IList clientSupplementalData = mTlsClient.GetClientSupplementalData();
                    if (clientSupplementalData != null)
                    {
                        SendSupplementalDataMessage(clientSupplementalData);
                    }
                    this.mConnectionState = CS_CLIENT_SUPPLEMENTAL_DATA;

                    TlsCredentials clientCreds = null;
                    if (mCertificateRequest == null)
                    {
                        this.mKeyExchange.SkipClientCredentials();
                    }
                    else
                    {
                        clientCreds = this.mAuthentication.GetClientCredentials(mCertificateRequest);

                        if (clientCreds == null)
                        {
                            this.mKeyExchange.SkipClientCredentials();

                            /*
                             * RFC 5246 If no suitable certificate is available, the client MUST Send a
                             * certificate message containing no certificates.
                             * 
                             * NOTE: In previous RFCs, this was SHOULD instead of MUST.
                             */
                            SendCertificateMessage(Certificate.EmptyChain);
                        }
                        else
                        {
                            this.mKeyExchange.ProcessClientCredentials(clientCreds);

                            SendCertificateMessage(clientCreds.Certificate);
                        }
                    }

                    this.mConnectionState = CS_CLIENT_CERTIFICATE;

                    /*
                     * Send the client key exchange message, depending on the key exchange we are using
                     * in our CipherSuite.
                     */
                    SendClientKeyExchangeMessage();
                    this.mConnectionState = CS_CLIENT_KEY_EXCHANGE;

                    if (TlsUtilities.IsSsl(Context))
                    {
                        EstablishMasterSecret(Context, mKeyExchange);
                    }

                    TlsHandshakeHash prepareFinishHash = mRecordStream.PrepareToFinish();
                    this.mSecurityParameters.sessionHash = GetCurrentPrfHash(Context, prepareFinishHash, null);

                    if (!TlsUtilities.IsSsl(Context))
                    {
                        EstablishMasterSecret(Context, mKeyExchange);
                    }

                    mRecordStream.SetPendingConnectionState(Peer.GetCompression(), Peer.GetCipher());

                    if (clientCreds != null && clientCreds is TlsSignerCredentials)
                    {
                        TlsSignerCredentials signerCredentials = (TlsSignerCredentials)clientCreds;

                        /*
                         * RFC 5246 4.7. digitally-signed element needs SignatureAndHashAlgorithm from TLS 1.2
                         */
                        SignatureAndHashAlgorithm signatureAndHashAlgorithm = TlsUtilities.GetSignatureAndHashAlgorithm(
                            Context, signerCredentials);

                        byte[] hash;
                        if (signatureAndHashAlgorithm == null)
                        {
                            hash = mSecurityParameters.SessionHash;
                        }
                        else
                        {
                            hash = prepareFinishHash.GetFinalHash(signatureAndHashAlgorithm.Hash);
                        }

                        byte[] signature = signerCredentials.GenerateCertificateSignature(hash);
                        DigitallySigned certificateVerify = new DigitallySigned(signatureAndHashAlgorithm, signature);
                        SendCertificateVerifyMessage(certificateVerify);

                        this.mConnectionState = CS_CERTIFICATE_VERIFY;
                    }

                    SendChangeCipherSpecMessage();
                    SendFinishedMessage();
                    break;
                }
                default:
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }

                this.mConnectionState = CS_CLIENT_FINISHED;
                break;
            }
            case HandshakeType.server_key_exchange:
            {
                switch (this.mConnectionState)
                {
                case CS_SERVER_HELLO:
                case CS_SERVER_SUPPLEMENTAL_DATA:
                case CS_SERVER_CERTIFICATE:
                case CS_CERTIFICATE_STATUS:
                {
                    if (mConnectionState < CS_SERVER_SUPPLEMENTAL_DATA)
                    {
                        HandleSupplementalData(null);
                    }

                    if (mConnectionState < CS_SERVER_CERTIFICATE)
                    {
                        // There was no server certificate message; check it's OK
                        this.mKeyExchange.SkipServerCredentials();
                        this.mAuthentication = null;
                    }

                    this.mKeyExchange.ProcessServerKeyExchange(buf);

                    AssertEmpty(buf);
                    break;
                }
                default:
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }

                this.mConnectionState = CS_SERVER_KEY_EXCHANGE;
                break;
            }
            case HandshakeType.certificate_request:
            {
                switch (this.mConnectionState)
                {
                case CS_SERVER_CERTIFICATE:
                case CS_CERTIFICATE_STATUS:
                case CS_SERVER_KEY_EXCHANGE:
                {
                    if (this.mConnectionState != CS_SERVER_KEY_EXCHANGE)
                    {
                        // There was no server key exchange message; check it's OK
                        this.mKeyExchange.SkipServerKeyExchange();
                    }

                    if (this.mAuthentication == null)
                    {
                        /*
                         * RFC 2246 7.4.4. It is a fatal handshake_failure alert for an anonymous server
                         * to request client identification.
                         */
                        throw new TlsFatalAlert(AlertDescription.handshake_failure);
                    }

                    this.mCertificateRequest = CertificateRequest.Parse(Context, buf);

                    AssertEmpty(buf);

                    this.mKeyExchange.ValidateCertificateRequest(this.mCertificateRequest);

                    /*
                     * TODO Give the client a chance to immediately select the CertificateVerify hash
                     * algorithm here to avoid tracking the other hash algorithms unnecessarily?
                     */
                    TlsUtilities.TrackHashAlgorithms(this.mRecordStream.HandshakeHash,
                        this.mCertificateRequest.SupportedSignatureAlgorithms);

                    break;
                }
                default:
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }

                this.mConnectionState = CS_CERTIFICATE_REQUEST;
                break;
            }
            case HandshakeType.session_ticket:
            {
                switch (this.mConnectionState)
                {
                case CS_CLIENT_FINISHED:
                {
                    if (!this.mExpectSessionTicket)
                    {
                        /*
                         * RFC 5077 3.3. This message MUST NOT be sent if the server did not include a
                         * SessionTicket extension in the ServerHello.
                         */
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);
                    }

                    /*
                     * RFC 5077 3.4. If the client receives a session ticket from the server, then it
                     * discards any Session ID that was sent in the ServerHello.
                     */
                    InvalidateSession();

                    ReceiveNewSessionTicketMessage(buf);
                    break;
                }
                default:
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }

                this.mConnectionState = CS_SERVER_SESSION_TICKET;
                break;
            }
            case HandshakeType.hello_request:
            {
                AssertEmpty(buf);

                /*
                 * RFC 2246 7.4.1.1 Hello request This message will be ignored by the client if the
                 * client is currently negotiating a session. This message may be ignored by the client
                 * if it does not wish to renegotiate a session, or the client may, if it wishes,
                 * respond with a no_renegotiation alert.
                 */
                if (this.mConnectionState == CS_END)
                {
                    RefuseRenegotiation();
                }
                break;
            }
            case HandshakeType.client_hello:
            case HandshakeType.client_key_exchange:
            case HandshakeType.certificate_verify:
            case HandshakeType.hello_verify_request:
            default:
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
            }
        }

        protected virtual void HandleSupplementalData(IList serverSupplementalData)
        {
            this.mTlsClient.ProcessServerSupplementalData(serverSupplementalData);
            this.mConnectionState = CS_SERVER_SUPPLEMENTAL_DATA;

            this.mKeyExchange = mTlsClient.GetKeyExchange();
            this.mKeyExchange.Init(Context);
        }

        protected virtual void ReceiveNewSessionTicketMessage(MemoryStream buf)
        {
            NewSessionTicket newSessionTicket = NewSessionTicket.Parse(buf);

            AssertEmpty(buf);

            mTlsClient.NotifyNewSessionTicket(newSessionTicket);
        }

        protected virtual void ReceiveServerHelloMessage(MemoryStream buf)
        {
            {
                ProtocolVersion server_version = TlsUtilities.ReadVersion(buf);
                if (server_version.IsDtls)
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                // Check that this matches what the server is Sending in the record layer
                if (!server_version.Equals(this.mRecordStream.ReadVersion))
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                ProtocolVersion client_version = Context.ClientVersion;
                if (!server_version.IsEqualOrEarlierVersionOf(client_version))
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                this.mRecordStream.SetWriteVersion(server_version);
                ContextAdmin.SetServerVersion(server_version);
                this.mTlsClient.NotifyServerVersion(server_version);
            }

            /*
             * Read the server random
             */
            this.mSecurityParameters.serverRandom = TlsUtilities.ReadFully(32, buf);

            this.mSelectedSessionID = TlsUtilities.ReadOpaque8(buf);
            if (this.mSelectedSessionID.Length > 32)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            this.mTlsClient.NotifySessionID(this.mSelectedSessionID);
            this.mResumedSession = this.mSelectedSessionID.Length > 0 && this.mTlsSession != null
                && Arrays.AreEqual(this.mSelectedSessionID, this.mTlsSession.SessionID);

            /*
             * Find out which CipherSuite the server has chosen and check that it was one of the offered
             * ones, and is a valid selection for the negotiated version.
             */
            int selectedCipherSuite = TlsUtilities.ReadUint16(buf);
            if (!Arrays.Contains(this.mOfferedCipherSuites, selectedCipherSuite)
                || selectedCipherSuite == CipherSuite.TLS_NULL_WITH_NULL_NULL
                || CipherSuite.IsScsv(selectedCipherSuite)
                || !TlsUtilities.IsValidCipherSuiteForVersion(selectedCipherSuite, Context.ServerVersion))
            {
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }
            this.mTlsClient.NotifySelectedCipherSuite(selectedCipherSuite);

            /*
             * Find out which CompressionMethod the server has chosen and check that it was one of the
             * offered ones.
             */
            byte selectedCompressionMethod = TlsUtilities.ReadUint8(buf);
            if (!Arrays.Contains(this.mOfferedCompressionMethods, selectedCompressionMethod))
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            this.mTlsClient.NotifySelectedCompressionMethod(selectedCompressionMethod);

            /*
             * RFC3546 2.2 The extended server hello message format MAY be sent in place of the server
             * hello message when the client has requested extended functionality via the extended
             * client hello message specified in Section 2.1. ... Note that the extended server hello
             * message is only sent in response to an extended client hello message. This prevents the
             * possibility that the extended server hello message could "break" existing TLS 1.0
             * clients.
             */
            this.mServerExtensions = ReadExtensions(buf);

            /*
             * RFC 3546 2.2 Note that the extended server hello message is only sent in response to an
             * extended client hello message.
             * 
             * However, see RFC 5746 exception below. We always include the SCSV, so an Extended Server
             * Hello is always allowed.
             */
            if (this.mServerExtensions != null)
            {
                foreach (int extType in this.mServerExtensions.Keys)
                {
                    /*
                     * RFC 5746 3.6. Note that Sending a "renegotiation_info" extension in response to a
                     * ClientHello containing only the SCSV is an explicit exception to the prohibition
                     * in RFC 5246, Section 7.4.1.4, on the server Sending unsolicited extensions and is
                     * only allowed because the client is signaling its willingness to receive the
                     * extension via the TLS_EMPTY_RENEGOTIATION_INFO_SCSV SCSV.
                     */
                    if (extType == ExtensionType.renegotiation_info)
                        continue;

                    /*
                     * RFC 5246 7.4.1.4 An extension type MUST NOT appear in the ServerHello unless the
                     * same extension type appeared in the corresponding ClientHello. If a client
                     * receives an extension type in ServerHello that it did not request in the
                     * associated ClientHello, it MUST abort the handshake with an unsupported_extension
                     * fatal alert.
                     */
                    if (null == TlsUtilities.GetExtensionData(this.mClientExtensions, extType))
                        throw new TlsFatalAlert(AlertDescription.unsupported_extension);

                    /*
                     * RFC 3546 2.3. If [...] the older session is resumed, then the server MUST ignore
                     * extensions appearing in the client hello, and Send a server hello containing no
                     * extensions[.]
                     */
                    if (this.mResumedSession)
                    {
                        // TODO[compat-gnutls] GnuTLS test server Sends server extensions e.g. ec_point_formats
                        // TODO[compat-openssl] OpenSSL test server Sends server extensions e.g. ec_point_formats
                        // TODO[compat-polarssl] PolarSSL test server Sends server extensions e.g. ec_point_formats
    //                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                    }
                }
            }

            /*
             * RFC 5746 3.4. Client Behavior: Initial Handshake
             */
            {
                /*
                 * When a ServerHello is received, the client MUST check if it includes the
                 * "renegotiation_info" extension:
                 */
                byte[] renegExtData = TlsUtilities.GetExtensionData(this.mServerExtensions, ExtensionType.renegotiation_info);
                if (renegExtData != null)
                {
                    /*
                     * If the extension is present, set the secure_renegotiation flag to TRUE. The
                     * client MUST then verify that the length of the "renegotiated_connection"
                     * field is zero, and if it is not, MUST abort the handshake (by Sending a fatal
                     * handshake_failure alert).
                     */
                    this.mSecureRenegotiation = true;

                    if (!Arrays.ConstantTimeAreEqual(renegExtData, CreateRenegotiationInfo(TlsUtilities.EmptyBytes)))
                        throw new TlsFatalAlert(AlertDescription.handshake_failure);
                }
            }

            // TODO[compat-gnutls] GnuTLS test server fails to Send renegotiation_info extension when resuming
            this.mTlsClient.NotifySecureRenegotiation(this.mSecureRenegotiation);

            IDictionary sessionClientExtensions = mClientExtensions, sessionServerExtensions = mServerExtensions;
            if (this.mResumedSession)
            {
                if (selectedCipherSuite != this.mSessionParameters.CipherSuite
                    || selectedCompressionMethod != this.mSessionParameters.CompressionAlgorithm)
                {
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                }

                sessionClientExtensions = null;
                sessionServerExtensions = this.mSessionParameters.ReadServerExtensions();
            }

            this.mSecurityParameters.cipherSuite = selectedCipherSuite;
            this.mSecurityParameters.compressionAlgorithm = selectedCompressionMethod;

            if (sessionServerExtensions != null)
            {
                {
                    /*
                     * RFC 7366 3. If a server receives an encrypt-then-MAC request extension from a client
                     * and then selects a stream or Authenticated Encryption with Associated Data (AEAD)
                     * ciphersuite, it MUST NOT send an encrypt-then-MAC response extension back to the
                     * client.
                     */
                    bool serverSentEncryptThenMAC = TlsExtensionsUtilities.HasEncryptThenMacExtension(sessionServerExtensions);
                    if (serverSentEncryptThenMAC && !TlsUtilities.IsBlockCipherSuite(selectedCipherSuite))
                        throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                    this.mSecurityParameters.encryptThenMac = serverSentEncryptThenMAC;
                }

                this.mSecurityParameters.extendedMasterSecret = TlsExtensionsUtilities.HasExtendedMasterSecretExtension(sessionServerExtensions);

                this.mSecurityParameters.maxFragmentLength = ProcessMaxFragmentLengthExtension(sessionClientExtensions,
                    sessionServerExtensions, AlertDescription.illegal_parameter);

                this.mSecurityParameters.truncatedHMac = TlsExtensionsUtilities.HasTruncatedHMacExtension(sessionServerExtensions);

                /*
                 * TODO It's surprising that there's no provision to allow a 'fresh' CertificateStatus to be sent in
                 * a session resumption handshake.
                 */
                this.mAllowCertificateStatus = !this.mResumedSession
                    && TlsUtilities.HasExpectedEmptyExtensionData(sessionServerExtensions, ExtensionType.status_request,
                        AlertDescription.illegal_parameter);

                this.mExpectSessionTicket = !this.mResumedSession
                    && TlsUtilities.HasExpectedEmptyExtensionData(sessionServerExtensions, ExtensionType.session_ticket,
                        AlertDescription.illegal_parameter);
            }

            /*
             * TODO[session-hash]
             * 
             * draft-ietf-tls-session-hash-04 4. Clients and servers SHOULD NOT accept handshakes
             * that do not use the extended master secret [..]. (and see 5.2, 5.3)
             */

            if (sessionClientExtensions != null)
            {
                this.mTlsClient.ProcessServerExtensions(sessionServerExtensions);
            }

            this.mSecurityParameters.prfAlgorithm = GetPrfAlgorithm(Context, this.mSecurityParameters.CipherSuite);

            /*
             * RFC 5246 7.4.9. Any cipher suite which does not explicitly specify
             * verify_data_length has a verify_data_length equal to 12. This includes all
             * existing cipher suites.
             */
            this.mSecurityParameters.verifyDataLength = 12;
        }

        protected virtual void SendCertificateVerifyMessage(DigitallySigned certificateVerify)
        {
            HandshakeMessage message = new HandshakeMessage(HandshakeType.certificate_verify);

            certificateVerify.Encode(message);

            message.WriteToRecordStream(this);
        }

        protected virtual void SendClientHelloMessage()
        {
            this.mRecordStream.SetWriteVersion(this.mTlsClient.ClientHelloRecordLayerVersion);

            ProtocolVersion client_version = this.mTlsClient.ClientVersion;
            if (client_version.IsDtls)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            ContextAdmin.SetClientVersion(client_version);

            /*
             * TODO RFC 5077 3.4. When presenting a ticket, the client MAY generate and include a
             * Session ID in the TLS ClientHello.
             */
            byte[] session_id = TlsUtilities.EmptyBytes;
            if (this.mTlsSession != null)
            {
                session_id = this.mTlsSession.SessionID;
                if (session_id == null || session_id.Length > 32)
                {
                    session_id = TlsUtilities.EmptyBytes;
                }
            }

            bool fallback = this.mTlsClient.IsFallback;

            this.mOfferedCipherSuites = this.mTlsClient.GetCipherSuites();

            this.mOfferedCompressionMethods = this.mTlsClient.GetCompressionMethods();

            if (session_id.Length > 0 && this.mSessionParameters != null)
            {
                if (!Arrays.Contains(this.mOfferedCipherSuites, mSessionParameters.CipherSuite)
                    || !Arrays.Contains(this.mOfferedCompressionMethods, mSessionParameters.CompressionAlgorithm))
                {
                    session_id = TlsUtilities.EmptyBytes;
                }
            }

            this.mClientExtensions = this.mTlsClient.GetClientExtensions();

            HandshakeMessage message = new HandshakeMessage(HandshakeType.client_hello);

            TlsUtilities.WriteVersion(client_version, message);

            message.Write(this.mSecurityParameters.ClientRandom);

            TlsUtilities.WriteOpaque8(session_id, message);

            // Cipher Suites (and SCSV)
            {
                /*
                 * RFC 5746 3.4. The client MUST include either an empty "renegotiation_info" extension,
                 * or the TLS_EMPTY_RENEGOTIATION_INFO_SCSV signaling cipher suite value in the
                 * ClientHello. Including both is NOT RECOMMENDED.
                 */
                byte[] renegExtData = TlsUtilities.GetExtensionData(mClientExtensions, ExtensionType.renegotiation_info);
                bool noRenegExt = (null == renegExtData);

                bool noRenegScsv = !Arrays.Contains(mOfferedCipherSuites, CipherSuite.TLS_EMPTY_RENEGOTIATION_INFO_SCSV);

                if (noRenegExt && noRenegScsv)
                {
                    // TODO Consider whether to default to a client extension instead
    //                this.mClientExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(this.mClientExtensions);
    //                this.mClientExtensions[ExtensionType.renegotiation_info] = CreateRenegotiationInfo(TlsUtilities.EmptyBytes);
                    this.mOfferedCipherSuites = Arrays.Append(mOfferedCipherSuites, CipherSuite.TLS_EMPTY_RENEGOTIATION_INFO_SCSV);
                }

                /*
                 * RFC 7507 4. If a client sends a ClientHello.client_version containing a lower value
                 * than the latest (highest-valued) version supported by the client, it SHOULD include
                 * the TLS_FALLBACK_SCSV cipher suite value in ClientHello.cipher_suites [..]. (The
                 * client SHOULD put TLS_FALLBACK_SCSV after all cipher suites that it actually intends
                 * to negotiate.)
                 */
                if (fallback && !Arrays.Contains(mOfferedCipherSuites, CipherSuite.TLS_FALLBACK_SCSV))
                {
                    this.mOfferedCipherSuites = Arrays.Append(mOfferedCipherSuites, CipherSuite.TLS_FALLBACK_SCSV);
                }

                TlsUtilities.WriteUint16ArrayWithUint16Length(mOfferedCipherSuites, message);
            }

            TlsUtilities.WriteUint8ArrayWithUint8Length(mOfferedCompressionMethods, message);

            if (mClientExtensions != null)
            {
                WriteExtensions(message, mClientExtensions);
            }

            message.WriteToRecordStream(this);
        }

        protected virtual void SendClientKeyExchangeMessage()
        {
            HandshakeMessage message = new HandshakeMessage(HandshakeType.client_key_exchange);

            this.mKeyExchange.GenerateClientKeyExchange(message);

            message.WriteToRecordStream(this);
        }
    }
}
