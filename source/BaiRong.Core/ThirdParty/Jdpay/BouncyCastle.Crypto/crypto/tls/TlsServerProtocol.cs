using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class TlsServerProtocol
        :   TlsProtocol
    {
        protected TlsServer mTlsServer = null;
        internal TlsServerContextImpl mTlsServerContext = null;

        protected TlsKeyExchange mKeyExchange = null;
        protected TlsCredentials mServerCredentials = null;
        protected CertificateRequest mCertificateRequest = null;

        protected short mClientCertificateType = -1;
        protected TlsHandshakeHash mPrepareFinishHash = null;

        /**
         * Constructor for blocking mode.
         * @param stream The bi-directional stream of data to/from the client
         * @param output The stream of data to the client
         * @param secureRandom Random number generator for various cryptographic functions
         */
        public TlsServerProtocol(Stream stream, SecureRandom secureRandom)
            : base(stream, secureRandom)
        {
        }

        /**
         * Constructor for blocking mode.
         * @param input The stream of data from the client
         * @param output The stream of data to the client
         * @param secureRandom Random number generator for various cryptographic functions
         */
        public TlsServerProtocol(Stream input, Stream output, SecureRandom secureRandom)
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
        public TlsServerProtocol(SecureRandom secureRandom)
            : base(secureRandom)
        {
        }

        /**
         * Receives a TLS handshake in the role of server.<br/>
         * <br/>
         * In blocking mode, this will not return until the handshake is complete.
         * In non-blocking mode, use {@link TlsPeer#notifyHandshakeComplete()} to
         * receive a callback when the handshake is complete.
         *
         * @param tlsServer
         * @throws IOException If in blocking mode and handshake was not successful.
         */
        public virtual void Accept(TlsServer tlsServer)
        {
            if (tlsServer == null)
                throw new ArgumentNullException("tlsServer");
            if (this.mTlsServer != null)
                throw new InvalidOperationException("'Accept' can only be called once");

            this.mTlsServer = tlsServer;

            this.mSecurityParameters = new SecurityParameters();
            this.mSecurityParameters.entity = ConnectionEnd.server;

            this.mTlsServerContext = new TlsServerContextImpl(mSecureRandom, mSecurityParameters);

            this.mSecurityParameters.serverRandom = CreateRandomBlock(tlsServer.ShouldUseGmtUnixTime(),
                mTlsServerContext.NonceRandomGenerator);

            this.mTlsServer.Init(mTlsServerContext);
            this.mRecordStream.Init(mTlsServerContext);

            this.mRecordStream.SetRestrictReadVersion(false);

            BlockForHandshake();
        }

        protected override void CleanupHandshake()
        {
            base.CleanupHandshake();
        
            this.mKeyExchange = null;
            this.mServerCredentials = null;
            this.mCertificateRequest = null;
            this.mPrepareFinishHash = null;
        }

        protected override TlsContext Context
        {
            get { return mTlsServerContext; }
        }

        internal override AbstractTlsContext ContextAdmin
        {
            get { return mTlsServerContext; }
        }

        protected override TlsPeer Peer
        {
            get { return mTlsServer; }
        }

        protected override void HandleHandshakeMessage(byte type, MemoryStream buf)
        {
            switch (type)
            {
            case HandshakeType.client_hello:
            {
                switch (this.mConnectionState)
                {
                case CS_START:
                {
                    ReceiveClientHelloMessage(buf);
                    this.mConnectionState = CS_CLIENT_HELLO;

                    SendServerHelloMessage();
                    this.mConnectionState = CS_SERVER_HELLO;

                    mRecordStream.NotifyHelloComplete();

                    IList serverSupplementalData = mTlsServer.GetServerSupplementalData();
                    if (serverSupplementalData != null)
                    {
                        SendSupplementalDataMessage(serverSupplementalData);
                    }
                    this.mConnectionState = CS_SERVER_SUPPLEMENTAL_DATA;

                    this.mKeyExchange = mTlsServer.GetKeyExchange();
                    this.mKeyExchange.Init(Context);

                    this.mServerCredentials = mTlsServer.GetCredentials();

                    Certificate serverCertificate = null;

                    if (this.mServerCredentials == null)
                    {
                        this.mKeyExchange.SkipServerCredentials();
                    }
                    else
                    {
                        this.mKeyExchange.ProcessServerCredentials(this.mServerCredentials);

                        serverCertificate = this.mServerCredentials.Certificate;
                        SendCertificateMessage(serverCertificate);
                    }
                    this.mConnectionState = CS_SERVER_CERTIFICATE;

                    // TODO[RFC 3546] Check whether empty certificates is possible, allowed, or excludes CertificateStatus
                    if (serverCertificate == null || serverCertificate.IsEmpty)
                    {
                        this.mAllowCertificateStatus = false;
                    }

                    if (this.mAllowCertificateStatus)
                    {
                        CertificateStatus certificateStatus = mTlsServer.GetCertificateStatus();
                        if (certificateStatus != null)
                        {
                            SendCertificateStatusMessage(certificateStatus);
                        }
                    }

                    this.mConnectionState = CS_CERTIFICATE_STATUS;

                    byte[] serverKeyExchange = this.mKeyExchange.GenerateServerKeyExchange();
                    if (serverKeyExchange != null)
                    {
                        SendServerKeyExchangeMessage(serverKeyExchange);
                    }
                    this.mConnectionState = CS_SERVER_KEY_EXCHANGE;

                    if (this.mServerCredentials != null)
                    {
                        this.mCertificateRequest = mTlsServer.GetCertificateRequest();
                        if (this.mCertificateRequest != null)
                        {
                            if (TlsUtilities.IsTlsV12(Context) != (mCertificateRequest.SupportedSignatureAlgorithms != null))
                                throw new TlsFatalAlert(AlertDescription.internal_error);

                            this.mKeyExchange.ValidateCertificateRequest(mCertificateRequest);

                            SendCertificateRequestMessage(mCertificateRequest);

                            TlsUtilities.TrackHashAlgorithms(this.mRecordStream.HandshakeHash,
                                this.mCertificateRequest.SupportedSignatureAlgorithms);
                        }
                    }
                    this.mConnectionState = CS_CERTIFICATE_REQUEST;

                    SendServerHelloDoneMessage();
                    this.mConnectionState = CS_SERVER_HELLO_DONE;

                    this.mRecordStream.HandshakeHash.SealHashAlgorithms();

                    break;
                }
                case CS_END:
                {
                    RefuseRenegotiation();
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
                case CS_SERVER_HELLO_DONE:
                {
                    mTlsServer.ProcessClientSupplementalData(ReadSupplementalDataMessage(buf));
                    this.mConnectionState = CS_CLIENT_SUPPLEMENTAL_DATA;
                    break;
                }
                default:
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }
                break;
            }
            case HandshakeType.certificate:
            {
                switch (this.mConnectionState)
                {
                case CS_SERVER_HELLO_DONE:
                case CS_CLIENT_SUPPLEMENTAL_DATA:
                {
                    if (mConnectionState < CS_CLIENT_SUPPLEMENTAL_DATA)
                    {
                        mTlsServer.ProcessClientSupplementalData(null);
                    }

                    if (this.mCertificateRequest == null)
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    ReceiveCertificateMessage(buf);
                    this.mConnectionState = CS_CLIENT_CERTIFICATE;
                    break;
                }
                default:
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }
                break;
            }
            case HandshakeType.client_key_exchange:
            {
                switch (this.mConnectionState)
                {
                case CS_SERVER_HELLO_DONE:
                case CS_CLIENT_SUPPLEMENTAL_DATA:
                case CS_CLIENT_CERTIFICATE:
                {
                    if (mConnectionState < CS_CLIENT_SUPPLEMENTAL_DATA)
                    {
                        mTlsServer.ProcessClientSupplementalData(null);
                    }

                    if (mConnectionState < CS_CLIENT_CERTIFICATE)
                    {
                        if (this.mCertificateRequest == null)
                        {
                            this.mKeyExchange.SkipClientCredentials();
                        }
                        else
                        {
                            if (TlsUtilities.IsTlsV12(Context))
                            {
                                /*
                                 * RFC 5246 If no suitable certificate is available, the client MUST Send a
                                 * certificate message containing no certificates.
                                 * 
                                 * NOTE: In previous RFCs, this was SHOULD instead of MUST.
                                 */
                                throw new TlsFatalAlert(AlertDescription.unexpected_message);
                            }
                            else if (TlsUtilities.IsSsl(Context))
                            {
                                if (this.mPeerCertificate == null)
                                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                            }
                            else
                            {
                                NotifyClientCertificate(Certificate.EmptyChain);
                            }
                        }
                    }

                    ReceiveClientKeyExchangeMessage(buf);
                    this.mConnectionState = CS_CLIENT_KEY_EXCHANGE;
                    break;
                }
                default:
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }
                break;
            }
            case HandshakeType.certificate_verify:
            {
                switch (this.mConnectionState)
                {
                case CS_CLIENT_KEY_EXCHANGE:
                {
                    /*
                     * RFC 5246 7.4.8 This message is only sent following a client certificate that has
                     * signing capability (i.e., all certificates except those containing fixed
                     * Diffie-Hellman parameters).
                     */
                    if (!ExpectCertificateVerifyMessage())
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    ReceiveCertificateVerifyMessage(buf);
                    this.mConnectionState = CS_CERTIFICATE_VERIFY;

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
                case CS_CLIENT_KEY_EXCHANGE:
                case CS_CERTIFICATE_VERIFY:
                {
                    if (mConnectionState < CS_CERTIFICATE_VERIFY && ExpectCertificateVerifyMessage())
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);

                    ProcessFinishedMessage(buf);
                    this.mConnectionState = CS_CLIENT_FINISHED;

                    if (this.mExpectSessionTicket)
                    {
                        SendNewSessionTicketMessage(mTlsServer.GetNewSessionTicket());
                    }
                    this.mConnectionState = CS_SERVER_SESSION_TICKET;

                    SendChangeCipherSpecMessage();
                    SendFinishedMessage();
                    this.mConnectionState = CS_SERVER_FINISHED;

                    CompleteHandshake();
                    break;
                }
                default:
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }
                break;
            }
            case HandshakeType.hello_request:
            case HandshakeType.hello_verify_request:
            case HandshakeType.server_hello:
            case HandshakeType.server_key_exchange:
            case HandshakeType.certificate_request:
            case HandshakeType.server_hello_done:
            case HandshakeType.session_ticket:
            default:
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
            }
        }

        protected override void HandleAlertWarningMessage(byte alertDescription)
        {
            base.HandleAlertWarningMessage(alertDescription);

            switch (alertDescription)
            {
            case AlertDescription.no_certificate:
            {
                /*
                 * SSL 3.0 If the server has sent a certificate request Message, the client must send
                 * either the certificate message or a no_certificate alert.
                 */
                if (TlsUtilities.IsSsl(Context) && this.mCertificateRequest != null)
                {
                    switch (this.mConnectionState)
                    {
                    case CS_SERVER_HELLO_DONE:
                    case CS_CLIENT_SUPPLEMENTAL_DATA:
                    {
                        if (mConnectionState < CS_CLIENT_SUPPLEMENTAL_DATA)
                        {
                            mTlsServer.ProcessClientSupplementalData(null);
                        }

                        NotifyClientCertificate(Certificate.EmptyChain);
                        this.mConnectionState = CS_CLIENT_CERTIFICATE;
                        return;
                    }
                    }
                }
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
            }
            } 
        }

        protected virtual void NotifyClientCertificate(Certificate clientCertificate)
        {
            if (mCertificateRequest == null)
                throw new InvalidOperationException();
            if (mPeerCertificate != null)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);

            this.mPeerCertificate = clientCertificate;

            if (clientCertificate.IsEmpty)
            {
                this.mKeyExchange.SkipClientCredentials();
            }
            else
            {

                /*
                 * TODO RFC 5246 7.4.6. If the certificate_authorities list in the certificate request
                 * message was non-empty, one of the certificates in the certificate chain SHOULD be
                 * issued by one of the listed CAs.
                 */

                this.mClientCertificateType = TlsUtilities.GetClientCertificateType(clientCertificate,
                    this.mServerCredentials.Certificate);

                this.mKeyExchange.ProcessClientCertificate(clientCertificate);
            }

            /*
             * RFC 5246 7.4.6. If the client does not Send any certificates, the server MAY at its
             * discretion either continue the handshake without client authentication, or respond with a
             * fatal handshake_failure alert. Also, if some aspect of the certificate chain was
             * unacceptable (e.g., it was not signed by a known, trusted CA), the server MAY at its
             * discretion either continue the handshake (considering the client unauthenticated) or Send
             * a fatal alert.
             */
            this.mTlsServer.NotifyClientCertificate(clientCertificate);
        }

        protected virtual void ReceiveCertificateMessage(MemoryStream buf)
        {
            Certificate clientCertificate = Certificate.Parse(buf);

            AssertEmpty(buf);

            NotifyClientCertificate(clientCertificate);
        }

        protected virtual void ReceiveCertificateVerifyMessage(MemoryStream buf)
        {
            if (mCertificateRequest == null)
                throw new InvalidOperationException();

            DigitallySigned clientCertificateVerify = DigitallySigned.Parse(Context, buf);

            AssertEmpty(buf);

            // Verify the CertificateVerify message contains a correct signature.
            try
            {
                SignatureAndHashAlgorithm signatureAlgorithm = clientCertificateVerify.Algorithm;

                byte[] hash;
                if (TlsUtilities.IsTlsV12(Context))
                {
                    TlsUtilities.VerifySupportedSignatureAlgorithm(mCertificateRequest.SupportedSignatureAlgorithms, signatureAlgorithm);
                    hash = mPrepareFinishHash.GetFinalHash(signatureAlgorithm.Hash);
                }
                else
                {
                    hash = mSecurityParameters.SessionHash;
                }

                X509CertificateStructure x509Cert = mPeerCertificate.GetCertificateAt(0);
                SubjectPublicKeyInfo keyInfo = x509Cert.SubjectPublicKeyInfo;
                AsymmetricKeyParameter publicKey = PublicKeyFactory.CreateKey(keyInfo);

                TlsSigner tlsSigner = TlsUtilities.CreateTlsSigner((byte)mClientCertificateType);
                tlsSigner.Init(Context);
                if (!tlsSigner.VerifyRawSignature(signatureAlgorithm, clientCertificateVerify.Signature, publicKey, hash))
                    throw new TlsFatalAlert(AlertDescription.decrypt_error);
            }
            catch (TlsFatalAlert e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new TlsFatalAlert(AlertDescription.decrypt_error, e);
            }
        }

        protected virtual void ReceiveClientHelloMessage(MemoryStream buf)
        {
            ProtocolVersion client_version = TlsUtilities.ReadVersion(buf);
            mRecordStream.SetWriteVersion(client_version);

            if (client_version.IsDtls)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            byte[] client_random = TlsUtilities.ReadFully(32, buf);

            /*
             * TODO RFC 5077 3.4. If a ticket is presented by the client, the server MUST NOT attempt to
             * use the Session ID in the ClientHello for stateful session resumption.
             */
            byte[] sessionID = TlsUtilities.ReadOpaque8(buf);
            if (sessionID.Length > 32)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            /*
             * TODO RFC 5246 7.4.1.2. If the session_id field is not empty (implying a session
             * resumption request), this vector MUST include at least the cipher_suite from that
             * session.
             */
            int cipher_suites_length = TlsUtilities.ReadUint16(buf);
            if (cipher_suites_length < 2 || (cipher_suites_length & 1) != 0)
                throw new TlsFatalAlert(AlertDescription.decode_error);

            this.mOfferedCipherSuites = TlsUtilities.ReadUint16Array(cipher_suites_length / 2, buf);

            /*
             * TODO RFC 5246 7.4.1.2. If the session_id field is not empty (implying a session
             * resumption request), it MUST include the compression_method from that session.
             */
            int compression_methods_length = TlsUtilities.ReadUint8(buf);
            if (compression_methods_length < 1)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            this.mOfferedCompressionMethods = TlsUtilities.ReadUint8Array(compression_methods_length, buf);

            /*
             * TODO RFC 3546 2.3 If [...] the older session is resumed, then the server MUST ignore
             * extensions appearing in the client hello, and Send a server hello containing no
             * extensions.
             */
            this.mClientExtensions = ReadExtensions(buf);

            /*
             * TODO[session-hash]
             * 
             * draft-ietf-tls-session-hash-04 4. Clients and servers SHOULD NOT accept handshakes
             * that do not use the extended master secret [..]. (and see 5.2, 5.3)
             */
            this.mSecurityParameters.extendedMasterSecret = TlsExtensionsUtilities.HasExtendedMasterSecretExtension(mClientExtensions);

            ContextAdmin.SetClientVersion(client_version);

            mTlsServer.NotifyClientVersion(client_version);
            mTlsServer.NotifyFallback(Arrays.Contains(mOfferedCipherSuites, CipherSuite.TLS_FALLBACK_SCSV));

            mSecurityParameters.clientRandom = client_random;

            mTlsServer.NotifyOfferedCipherSuites(mOfferedCipherSuites);
            mTlsServer.NotifyOfferedCompressionMethods(mOfferedCompressionMethods);

            /*
             * RFC 5746 3.6. Server Behavior: Initial Handshake
             */
            {
                /*
                 * RFC 5746 3.4. The client MUST include either an empty "renegotiation_info" extension,
                 * or the TLS_EMPTY_RENEGOTIATION_INFO_SCSV signaling cipher suite value in the
                 * ClientHello. Including both is NOT RECOMMENDED.
                 */

                /*
                 * When a ClientHello is received, the server MUST check if it includes the
                 * TLS_EMPTY_RENEGOTIATION_INFO_SCSV SCSV. If it does, set the secure_renegotiation flag
                 * to TRUE.
                 */
                if (Arrays.Contains(mOfferedCipherSuites, CipherSuite.TLS_EMPTY_RENEGOTIATION_INFO_SCSV))
                {
                    this.mSecureRenegotiation = true;
                }

                /*
                 * The server MUST check if the "renegotiation_info" extension is included in the
                 * ClientHello.
                 */
                byte[] renegExtData = TlsUtilities.GetExtensionData(mClientExtensions, ExtensionType.renegotiation_info);
                if (renegExtData != null)
                {
                    /*
                     * If the extension is present, set secure_renegotiation flag to TRUE. The
                     * server MUST then verify that the length of the "renegotiated_connection"
                     * field is zero, and if it is not, MUST abort the handshake.
                     */
                    this.mSecureRenegotiation = true;

                    if (!Arrays.ConstantTimeAreEqual(renegExtData, CreateRenegotiationInfo(TlsUtilities.EmptyBytes)))
                        throw new TlsFatalAlert(AlertDescription.handshake_failure);
                }
            }

            mTlsServer.NotifySecureRenegotiation(this.mSecureRenegotiation);

            if (mClientExtensions != null)
            {
                // NOTE: Validates the padding extension data, if present
                TlsExtensionsUtilities.GetPaddingExtension(mClientExtensions);

                mTlsServer.ProcessClientExtensions(mClientExtensions);
            }
        }

        protected virtual void ReceiveClientKeyExchangeMessage(MemoryStream buf)
        {
            mKeyExchange.ProcessClientKeyExchange(buf);

            AssertEmpty(buf);

            if (TlsUtilities.IsSsl(Context))
            {
                EstablishMasterSecret(Context, mKeyExchange);
            }

            this.mPrepareFinishHash = mRecordStream.PrepareToFinish();
            this.mSecurityParameters.sessionHash = GetCurrentPrfHash(Context, mPrepareFinishHash, null);

            if (!TlsUtilities.IsSsl(Context))
            {
                EstablishMasterSecret(Context, mKeyExchange);
            }

            mRecordStream.SetPendingConnectionState(Peer.GetCompression(), Peer.GetCipher());
        }

        protected virtual void SendCertificateRequestMessage(CertificateRequest certificateRequest)
        {
            HandshakeMessage message = new HandshakeMessage(HandshakeType.certificate_request);

            certificateRequest.Encode(message);

            message.WriteToRecordStream(this);
        }

        protected virtual void SendCertificateStatusMessage(CertificateStatus certificateStatus)
        {
            HandshakeMessage message = new HandshakeMessage(HandshakeType.certificate_status);

            certificateStatus.Encode(message);

            message.WriteToRecordStream(this);
        }

        protected virtual void SendNewSessionTicketMessage(NewSessionTicket newSessionTicket)
        {
            if (newSessionTicket == null)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            HandshakeMessage message = new HandshakeMessage(HandshakeType.session_ticket);

            newSessionTicket.Encode(message);

            message.WriteToRecordStream(this);
        }

        protected virtual void SendServerHelloMessage()
        {
            HandshakeMessage message = new HandshakeMessage(HandshakeType.server_hello);

            {
                ProtocolVersion server_version = mTlsServer.GetServerVersion();
                if (!server_version.IsEqualOrEarlierVersionOf(Context.ClientVersion))
                    throw new TlsFatalAlert(AlertDescription.internal_error);

                mRecordStream.ReadVersion = server_version;
                mRecordStream.SetWriteVersion(server_version);
                mRecordStream.SetRestrictReadVersion(true);
                ContextAdmin.SetServerVersion(server_version);

                TlsUtilities.WriteVersion(server_version, message);
            }

            message.Write(this.mSecurityParameters.serverRandom);

            /*
             * The server may return an empty session_id to indicate that the session will not be cached
             * and therefore cannot be resumed.
             */
            TlsUtilities.WriteOpaque8(TlsUtilities.EmptyBytes, message);

            int selectedCipherSuite = mTlsServer.GetSelectedCipherSuite();
            if (!Arrays.Contains(mOfferedCipherSuites, selectedCipherSuite)
                || selectedCipherSuite == CipherSuite.TLS_NULL_WITH_NULL_NULL
                || CipherSuite.IsScsv(selectedCipherSuite)
                || !TlsUtilities.IsValidCipherSuiteForVersion(selectedCipherSuite, Context.ServerVersion))
            {
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
            mSecurityParameters.cipherSuite = selectedCipherSuite;

            byte selectedCompressionMethod = mTlsServer.GetSelectedCompressionMethod();
            if (!Arrays.Contains(mOfferedCompressionMethods, selectedCompressionMethod))
            {
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
            mSecurityParameters.compressionAlgorithm = selectedCompressionMethod;

            TlsUtilities.WriteUint16(selectedCipherSuite, message);
            TlsUtilities.WriteUint8(selectedCompressionMethod, message);

            this.mServerExtensions = mTlsServer.GetServerExtensions();

            /*
             * RFC 5746 3.6. Server Behavior: Initial Handshake
             */
            if (this.mSecureRenegotiation)
            {
                byte[] renegExtData = TlsUtilities.GetExtensionData(this.mServerExtensions, ExtensionType.renegotiation_info);
                bool noRenegExt = (null == renegExtData);

                if (noRenegExt)
                {
                    /*
                     * Note that Sending a "renegotiation_info" extension in response to a ClientHello
                     * containing only the SCSV is an explicit exception to the prohibition in RFC 5246,
                     * Section 7.4.1.4, on the server Sending unsolicited extensions and is only allowed
                     * because the client is signaling its willingness to receive the extension via the
                     * TLS_EMPTY_RENEGOTIATION_INFO_SCSV SCSV.
                     */

                    /*
                     * If the secure_renegotiation flag is set to TRUE, the server MUST include an empty
                     * "renegotiation_info" extension in the ServerHello message.
                     */
                    this.mServerExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(mServerExtensions);
                    this.mServerExtensions[ExtensionType.renegotiation_info] = CreateRenegotiationInfo(TlsUtilities.EmptyBytes);
                }
            }

            if (mSecurityParameters.extendedMasterSecret)
            {
                this.mServerExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(mServerExtensions);
                TlsExtensionsUtilities.AddExtendedMasterSecretExtension(mServerExtensions);
            }

            /*
             * TODO RFC 3546 2.3 If [...] the older session is resumed, then the server MUST ignore
             * extensions appearing in the client hello, and Send a server hello containing no
             * extensions.
             */

            if (this.mServerExtensions != null)
            {
                this.mSecurityParameters.encryptThenMac = TlsExtensionsUtilities.HasEncryptThenMacExtension(mServerExtensions);

                this.mSecurityParameters.maxFragmentLength = ProcessMaxFragmentLengthExtension(mClientExtensions,
                    mServerExtensions, AlertDescription.internal_error);

                this.mSecurityParameters.truncatedHMac = TlsExtensionsUtilities.HasTruncatedHMacExtension(mServerExtensions);

                /*
                 * TODO It's surprising that there's no provision to allow a 'fresh' CertificateStatus to be sent in
                 * a session resumption handshake.
                 */
                this.mAllowCertificateStatus = !mResumedSession
                    && TlsUtilities.HasExpectedEmptyExtensionData(mServerExtensions, ExtensionType.status_request,
                        AlertDescription.internal_error);

                this.mExpectSessionTicket = !mResumedSession
                    && TlsUtilities.HasExpectedEmptyExtensionData(mServerExtensions, ExtensionType.session_ticket,
                        AlertDescription.internal_error);

                WriteExtensions(message, this.mServerExtensions);
            }

            mSecurityParameters.prfAlgorithm = GetPrfAlgorithm(Context, mSecurityParameters.CipherSuite);

            /*
             * RFC 5246 7.4.9. Any cipher suite which does not explicitly specify verify_data_length has
             * a verify_data_length equal to 12. This includes all existing cipher suites.
             */
            mSecurityParameters.verifyDataLength = 12;

            ApplyMaxFragmentLengthExtension();

            message.WriteToRecordStream(this);
        }

        protected virtual void SendServerHelloDoneMessage()
        {
            byte[] message = new byte[4];
            TlsUtilities.WriteUint8(HandshakeType.server_hello_done, message, 0);
            TlsUtilities.WriteUint24(0, message, 1);

            WriteHandshakeMessage(message, 0, message.Length);
        }

        protected virtual void SendServerKeyExchangeMessage(byte[] serverKeyExchange)
        {
            HandshakeMessage message = new HandshakeMessage(HandshakeType.server_key_exchange, serverKeyExchange.Length);

            message.Write(serverKeyExchange);

            message.WriteToRecordStream(this);
        }

        protected virtual bool ExpectCertificateVerifyMessage()
        {
            return mClientCertificateType >= 0 && TlsUtilities.HasSigningCapability((byte)mClientCertificateType);
        }
    }
}
