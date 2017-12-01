using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class DtlsServerProtocol
        :   DtlsProtocol
    {
        protected bool mVerifyRequests = true;

        public DtlsServerProtocol(SecureRandom secureRandom)
            :   base(secureRandom)
        {
        }

        public virtual bool VerifyRequests
        {
            get { return mVerifyRequests; }
            set { this.mVerifyRequests = value; }
        }

        public virtual DtlsTransport Accept(TlsServer server, DatagramTransport transport)
        {
            if (server == null)
                throw new ArgumentNullException("server");
            if (transport == null)
                throw new ArgumentNullException("transport");

            SecurityParameters securityParameters = new SecurityParameters();
            securityParameters.entity = ConnectionEnd.server;

            ServerHandshakeState state = new ServerHandshakeState();
            state.server = server;
            state.serverContext = new TlsServerContextImpl(mSecureRandom, securityParameters);

            securityParameters.serverRandom = TlsProtocol.CreateRandomBlock(server.ShouldUseGmtUnixTime(),
                state.serverContext.NonceRandomGenerator);

            server.Init(state.serverContext);

            DtlsRecordLayer recordLayer = new DtlsRecordLayer(transport, state.serverContext, server, ContentType.handshake);

            // TODO Need to handle sending of HelloVerifyRequest without entering a full connection

            try
            {
                return ServerHandshake(state, recordLayer);
            }
            catch (TlsFatalAlert fatalAlert)
            {
                AbortServerHandshake(state, recordLayer, fatalAlert.AlertDescription);
                throw fatalAlert;
            }
            catch (IOException e)
            {
                AbortServerHandshake(state, recordLayer, AlertDescription.internal_error);
                throw e;
            }
            catch (Exception e)
            {
                AbortServerHandshake(state, recordLayer, AlertDescription.internal_error);
                throw new TlsFatalAlert(AlertDescription.internal_error, e);
            }
            finally
            {
                securityParameters.Clear();
            }
        }

        internal virtual void AbortServerHandshake(ServerHandshakeState state, DtlsRecordLayer recordLayer, byte alertDescription)
        {
            recordLayer.Fail(alertDescription);
            InvalidateSession(state);
        }

        internal virtual DtlsTransport ServerHandshake(ServerHandshakeState state, DtlsRecordLayer recordLayer)
        {
            SecurityParameters securityParameters = state.serverContext.SecurityParameters;
            DtlsReliableHandshake handshake = new DtlsReliableHandshake(state.serverContext, recordLayer);

            DtlsReliableHandshake.Message clientMessage = handshake.ReceiveMessage();

            // NOTE: DTLSRecordLayer requires any DTLS version, we don't otherwise constrain this
            //ProtocolVersion recordLayerVersion = recordLayer.ReadVersion;

            if (clientMessage.Type == HandshakeType.client_hello)
            {
                ProcessClientHello(state, clientMessage.Body);
            }
            else
            {
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
            }

            {
                byte[] serverHelloBody = GenerateServerHello(state);

                ApplyMaxFragmentLengthExtension(recordLayer, securityParameters.maxFragmentLength);

                ProtocolVersion recordLayerVersion = state.serverContext.ServerVersion;
                recordLayer.ReadVersion = recordLayerVersion;
                recordLayer.SetWriteVersion(recordLayerVersion);

                handshake.SendMessage(HandshakeType.server_hello, serverHelloBody);
            }

            handshake.NotifyHelloComplete();

            IList serverSupplementalData = state.server.GetServerSupplementalData();
            if (serverSupplementalData != null)
            {
                byte[] supplementalDataBody = GenerateSupplementalData(serverSupplementalData);
                handshake.SendMessage(HandshakeType.supplemental_data, supplementalDataBody);
            }

            state.keyExchange = state.server.GetKeyExchange();
            state.keyExchange.Init(state.serverContext);

            state.serverCredentials = state.server.GetCredentials();

            Certificate serverCertificate = null;

            if (state.serverCredentials == null)
            {
                state.keyExchange.SkipServerCredentials();
            }
            else
            {
                state.keyExchange.ProcessServerCredentials(state.serverCredentials);

                serverCertificate = state.serverCredentials.Certificate;
                byte[] certificateBody = GenerateCertificate(serverCertificate);
                handshake.SendMessage(HandshakeType.certificate, certificateBody);
            }

            // TODO[RFC 3546] Check whether empty certificates is possible, allowed, or excludes CertificateStatus
            if (serverCertificate == null || serverCertificate.IsEmpty)
            {
                state.allowCertificateStatus = false;
            }

            if (state.allowCertificateStatus)
            {
                CertificateStatus certificateStatus = state.server.GetCertificateStatus();
                if (certificateStatus != null)
                {
                    byte[] certificateStatusBody = GenerateCertificateStatus(state, certificateStatus);
                    handshake.SendMessage(HandshakeType.certificate_status, certificateStatusBody);
                }
            }

            byte[] serverKeyExchange = state.keyExchange.GenerateServerKeyExchange();
            if (serverKeyExchange != null)
            {
                handshake.SendMessage(HandshakeType.server_key_exchange, serverKeyExchange);
            }

            if (state.serverCredentials != null)
            {
                state.certificateRequest = state.server.GetCertificateRequest();
                if (state.certificateRequest != null)
                {
                    if (TlsUtilities.IsTlsV12(state.serverContext) != (state.certificateRequest.SupportedSignatureAlgorithms != null))
                        throw new TlsFatalAlert(AlertDescription.internal_error);

                    state.keyExchange.ValidateCertificateRequest(state.certificateRequest);

                    byte[] certificateRequestBody = GenerateCertificateRequest(state, state.certificateRequest);
                    handshake.SendMessage(HandshakeType.certificate_request, certificateRequestBody);

                    TlsUtilities.TrackHashAlgorithms(handshake.HandshakeHash,
                        state.certificateRequest.SupportedSignatureAlgorithms);
                }
            }

            handshake.SendMessage(HandshakeType.server_hello_done, TlsUtilities.EmptyBytes);

            handshake.HandshakeHash.SealHashAlgorithms();

            clientMessage = handshake.ReceiveMessage();

            if (clientMessage.Type == HandshakeType.supplemental_data)
            {
                ProcessClientSupplementalData(state, clientMessage.Body);
                clientMessage = handshake.ReceiveMessage();
            }
            else
            {
                state.server.ProcessClientSupplementalData(null);
            }

            if (state.certificateRequest == null)
            {
                state.keyExchange.SkipClientCredentials();
            }
            else
            {
                if (clientMessage.Type == HandshakeType.certificate)
                {
                    ProcessClientCertificate(state, clientMessage.Body);
                    clientMessage = handshake.ReceiveMessage();
                }
                else
                {
                    if (TlsUtilities.IsTlsV12(state.serverContext))
                    {
                        /*
                         * RFC 5246 If no suitable certificate is available, the client MUST send a
                         * certificate message containing no certificates.
                         * 
                         * NOTE: In previous RFCs, this was SHOULD instead of MUST.
                         */
                        throw new TlsFatalAlert(AlertDescription.unexpected_message);
                    }

                    NotifyClientCertificate(state, Certificate.EmptyChain);
                }
            }

            if (clientMessage.Type == HandshakeType.client_key_exchange)
            {
                ProcessClientKeyExchange(state, clientMessage.Body);
            }
            else
            {
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
            }

            TlsHandshakeHash prepareFinishHash = handshake.PrepareToFinish();
            securityParameters.sessionHash = TlsProtocol.GetCurrentPrfHash(state.serverContext, prepareFinishHash, null);

            TlsProtocol.EstablishMasterSecret(state.serverContext, state.keyExchange);
            recordLayer.InitPendingEpoch(state.server.GetCipher());

            /*
             * RFC 5246 7.4.8 This message is only sent following a client certificate that has signing
             * capability (i.e., all certificates except those containing fixed Diffie-Hellman
             * parameters).
             */
            if (ExpectCertificateVerifyMessage(state))
            {
                byte[] certificateVerifyBody = handshake.ReceiveMessageBody(HandshakeType.certificate_verify);
                ProcessCertificateVerify(state, certificateVerifyBody, prepareFinishHash);
            }

            // NOTE: Calculated exclusive of the actual Finished message from the client
            byte[] expectedClientVerifyData = TlsUtilities.CalculateVerifyData(state.serverContext, ExporterLabel.client_finished,
                TlsProtocol.GetCurrentPrfHash(state.serverContext, handshake.HandshakeHash, null));
            ProcessFinished(handshake.ReceiveMessageBody(HandshakeType.finished), expectedClientVerifyData);

            if (state.expectSessionTicket)
            {
                NewSessionTicket newSessionTicket = state.server.GetNewSessionTicket();
                byte[] newSessionTicketBody = GenerateNewSessionTicket(state, newSessionTicket);
                handshake.SendMessage(HandshakeType.session_ticket, newSessionTicketBody);
            }

            // NOTE: Calculated exclusive of the Finished message itself
            byte[] serverVerifyData = TlsUtilities.CalculateVerifyData(state.serverContext, ExporterLabel.server_finished,
                TlsProtocol.GetCurrentPrfHash(state.serverContext, handshake.HandshakeHash, null));
            handshake.SendMessage(HandshakeType.finished, serverVerifyData);

            handshake.Finish();

            state.server.NotifyHandshakeComplete();

            return new DtlsTransport(recordLayer);
        }

        protected virtual void InvalidateSession(ServerHandshakeState state)
        {
            if (state.sessionParameters != null)
            {
                state.sessionParameters.Clear();
                state.sessionParameters = null;
            }

            if (state.tlsSession != null)
            {
                state.tlsSession.Invalidate();
                state.tlsSession = null;
            }
        }

        protected virtual byte[] GenerateCertificateRequest(ServerHandshakeState state, CertificateRequest certificateRequest)
        {
            MemoryStream buf = new MemoryStream();
            certificateRequest.Encode(buf);
            return buf.ToArray();
        }

        protected virtual byte[] GenerateCertificateStatus(ServerHandshakeState state, CertificateStatus certificateStatus)
        {
            MemoryStream buf = new MemoryStream();
            certificateStatus.Encode(buf);
            return buf.ToArray();
        }

        protected virtual byte[] GenerateNewSessionTicket(ServerHandshakeState state, NewSessionTicket newSessionTicket)
        {
            MemoryStream buf = new MemoryStream();
            newSessionTicket.Encode(buf);
            return buf.ToArray();
        }

        protected virtual byte[] GenerateServerHello(ServerHandshakeState state)
        {
            SecurityParameters securityParameters = state.serverContext.SecurityParameters;

            MemoryStream buf = new MemoryStream();

            {
                ProtocolVersion server_version = state.server.GetServerVersion();
                if (!server_version.IsEqualOrEarlierVersionOf(state.serverContext.ClientVersion))
                    throw new TlsFatalAlert(AlertDescription.internal_error);

                // TODO Read RFCs for guidance on the expected record layer version number
                // recordStream.setReadVersion(server_version);
                // recordStream.setWriteVersion(server_version);
                // recordStream.setRestrictReadVersion(true);
                state.serverContext.SetServerVersion(server_version);

                TlsUtilities.WriteVersion(state.serverContext.ServerVersion, buf);
            }

            buf.Write(securityParameters.ServerRandom, 0, securityParameters.ServerRandom.Length);

            /*
             * The server may return an empty session_id to indicate that the session will not be cached
             * and therefore cannot be resumed.
             */
            TlsUtilities.WriteOpaque8(TlsUtilities.EmptyBytes, buf);

            int selectedCipherSuite = state.server.GetSelectedCipherSuite();
            if (!Arrays.Contains(state.offeredCipherSuites, selectedCipherSuite)
                || selectedCipherSuite == CipherSuite.TLS_NULL_WITH_NULL_NULL
                || CipherSuite.IsScsv(selectedCipherSuite)
                || !TlsUtilities.IsValidCipherSuiteForVersion(selectedCipherSuite, state.serverContext.ServerVersion))
            {
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
            ValidateSelectedCipherSuite(selectedCipherSuite, AlertDescription.internal_error);
            securityParameters.cipherSuite = selectedCipherSuite;

            byte selectedCompressionMethod = state.server.GetSelectedCompressionMethod();
            if (!Arrays.Contains(state.offeredCompressionMethods, selectedCompressionMethod))
                throw new TlsFatalAlert(AlertDescription.internal_error);
            securityParameters.compressionAlgorithm = selectedCompressionMethod;

            TlsUtilities.WriteUint16(selectedCipherSuite, buf);
            TlsUtilities.WriteUint8(selectedCompressionMethod, buf);

            state.serverExtensions = state.server.GetServerExtensions();

            /*
             * RFC 5746 3.6. Server Behavior: Initial Handshake
             */
            if (state.secure_renegotiation)
            {
                byte[] renegExtData = TlsUtilities.GetExtensionData(state.serverExtensions, ExtensionType.renegotiation_info);
                bool noRenegExt = (null == renegExtData);

                if (noRenegExt)
                {
                    /*
                     * Note that sending a "renegotiation_info" extension in response to a ClientHello
                     * containing only the SCSV is an explicit exception to the prohibition in RFC 5246,
                     * Section 7.4.1.4, on the server sending unsolicited extensions and is only allowed
                     * because the client is signaling its willingness to receive the extension via the
                     * TLS_EMPTY_RENEGOTIATION_INFO_SCSV SCSV.
                     */

                    /*
                     * If the secure_renegotiation flag is set to TRUE, the server MUST include an empty
                     * "renegotiation_info" extension in the ServerHello message.
                     */
                    state.serverExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(state.serverExtensions);
                    state.serverExtensions[ExtensionType.renegotiation_info] = TlsProtocol.CreateRenegotiationInfo(TlsUtilities.EmptyBytes);
                }
            }

            if (securityParameters.extendedMasterSecret)
            {
                state.serverExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(state.serverExtensions);
                TlsExtensionsUtilities.AddExtendedMasterSecretExtension(state.serverExtensions);
            }

            /*
             * TODO RFC 3546 2.3 If [...] the older session is resumed, then the server MUST ignore
             * extensions appearing in the client hello, and send a server hello containing no
             * extensions.
             */

            if (state.serverExtensions != null)
            {
                securityParameters.encryptThenMac = TlsExtensionsUtilities.HasEncryptThenMacExtension(state.serverExtensions);

                securityParameters.maxFragmentLength = EvaluateMaxFragmentLengthExtension(state.resumedSession,
                    state.clientExtensions, state.serverExtensions, AlertDescription.internal_error);

                securityParameters.truncatedHMac = TlsExtensionsUtilities.HasTruncatedHMacExtension(state.serverExtensions);

                /*
                 * TODO It's surprising that there's no provision to allow a 'fresh' CertificateStatus to be sent in
                 * a session resumption handshake.
                 */
                state.allowCertificateStatus = !state.resumedSession
                    && TlsUtilities.HasExpectedEmptyExtensionData(state.serverExtensions, ExtensionType.status_request,
                        AlertDescription.internal_error);

                state.expectSessionTicket = !state.resumedSession
                    && TlsUtilities.HasExpectedEmptyExtensionData(state.serverExtensions, ExtensionType.session_ticket,
                        AlertDescription.internal_error);

                TlsProtocol.WriteExtensions(buf, state.serverExtensions);
            }

            securityParameters.prfAlgorithm = TlsProtocol.GetPrfAlgorithm(state.serverContext,
                securityParameters.CipherSuite);

            /*
             * RFC 5246 7.4.9. Any cipher suite which does not explicitly specify verify_data_length
             * has a verify_data_length equal to 12. This includes all existing cipher suites.
             */
            securityParameters.verifyDataLength = 12;

            return buf.ToArray();
        }

        protected virtual void NotifyClientCertificate(ServerHandshakeState state, Certificate clientCertificate)
        {
            if (state.certificateRequest == null)
                throw new InvalidOperationException();

            if (state.clientCertificate != null)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);

            state.clientCertificate = clientCertificate;

            if (clientCertificate.IsEmpty)
            {
                state.keyExchange.SkipClientCredentials();
            }
            else
            {

                /*
                 * TODO RFC 5246 7.4.6. If the certificate_authorities list in the certificate request
                 * message was non-empty, one of the certificates in the certificate chain SHOULD be
                 * issued by one of the listed CAs.
                 */

                state.clientCertificateType = TlsUtilities.GetClientCertificateType(clientCertificate,
                    state.serverCredentials.Certificate);

                state.keyExchange.ProcessClientCertificate(clientCertificate);
            }

            /*
             * RFC 5246 7.4.6. If the client does not send any certificates, the server MAY at its
             * discretion either continue the handshake without client authentication, or respond with a
             * fatal handshake_failure alert. Also, if some aspect of the certificate chain was
             * unacceptable (e.g., it was not signed by a known, trusted CA), the server MAY at its
             * discretion either continue the handshake (considering the client unauthenticated) or send
             * a fatal alert.
             */
            state.server.NotifyClientCertificate(clientCertificate);
        }

        protected virtual void ProcessClientCertificate(ServerHandshakeState state, byte[] body)
        {
            MemoryStream buf = new MemoryStream(body, false);

            Certificate clientCertificate = Certificate.Parse(buf);

            TlsProtocol.AssertEmpty(buf);

            NotifyClientCertificate(state, clientCertificate);
        }

        protected virtual void ProcessCertificateVerify(ServerHandshakeState state, byte[] body, TlsHandshakeHash prepareFinishHash)
        {
            if (state.certificateRequest == null)
                throw new InvalidOperationException();

            MemoryStream buf = new MemoryStream(body, false);

            TlsServerContextImpl context = state.serverContext;
            DigitallySigned clientCertificateVerify = DigitallySigned.Parse(context, buf);

            TlsProtocol.AssertEmpty(buf);

            // Verify the CertificateVerify message contains a correct signature.
            try
            {
                SignatureAndHashAlgorithm signatureAlgorithm = clientCertificateVerify.Algorithm;

                byte[] hash;
                if (TlsUtilities.IsTlsV12(context))
                {
                    TlsUtilities.VerifySupportedSignatureAlgorithm(state.certificateRequest.SupportedSignatureAlgorithms, signatureAlgorithm);
                    hash = prepareFinishHash.GetFinalHash(signatureAlgorithm.Hash);
                }
                else
                {
                    hash = context.SecurityParameters.SessionHash;
                }

                X509CertificateStructure x509Cert = state.clientCertificate.GetCertificateAt(0);
                SubjectPublicKeyInfo keyInfo = x509Cert.SubjectPublicKeyInfo;
                AsymmetricKeyParameter publicKey = PublicKeyFactory.CreateKey(keyInfo);

                TlsSigner tlsSigner = TlsUtilities.CreateTlsSigner((byte)state.clientCertificateType);
                tlsSigner.Init(context);
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

        protected virtual void ProcessClientHello(ServerHandshakeState state, byte[] body)
        {
            MemoryStream buf = new MemoryStream(body, false);

            // TODO Read RFCs for guidance on the expected record layer version number
            ProtocolVersion client_version = TlsUtilities.ReadVersion(buf);
            if (!client_version.IsDtls)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            /*
             * Read the client random
             */
            byte[] client_random = TlsUtilities.ReadFully(32, buf);

            byte[] sessionID = TlsUtilities.ReadOpaque8(buf);
            if (sessionID.Length > 32)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            // TODO RFC 4347 has the cookie length restricted to 32, but not in RFC 6347
            byte[] cookie = TlsUtilities.ReadOpaque8(buf);

            int cipher_suites_length = TlsUtilities.ReadUint16(buf);
            if (cipher_suites_length < 2 || (cipher_suites_length & 1) != 0)
            {
                throw new TlsFatalAlert(AlertDescription.decode_error);
            }

            /*
             * NOTE: "If the session_id field is not empty (implying a session resumption request) this
             * vector must include at least the cipher_suite from that session."
             */
            state.offeredCipherSuites = TlsUtilities.ReadUint16Array(cipher_suites_length / 2, buf);

            int compression_methods_length = TlsUtilities.ReadUint8(buf);
            if (compression_methods_length < 1)
            {
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }

            state.offeredCompressionMethods = TlsUtilities.ReadUint8Array(compression_methods_length, buf);

            /*
             * TODO RFC 3546 2.3 If [...] the older session is resumed, then the server MUST ignore
             * extensions appearing in the client hello, and send a server hello containing no
             * extensions.
             */
            state.clientExtensions = TlsProtocol.ReadExtensions(buf);

            TlsServerContextImpl context = state.serverContext;
            SecurityParameters securityParameters = context.SecurityParameters;

            /*
             * TODO[session-hash]
             * 
             * draft-ietf-tls-session-hash-04 4. Clients and servers SHOULD NOT accept handshakes
             * that do not use the extended master secret [..]. (and see 5.2, 5.3)
             */
            securityParameters.extendedMasterSecret = TlsExtensionsUtilities.HasExtendedMasterSecretExtension(state.clientExtensions);

            context.SetClientVersion(client_version);

            state.server.NotifyClientVersion(client_version);
            state.server.NotifyFallback(Arrays.Contains(state.offeredCipherSuites, CipherSuite.TLS_FALLBACK_SCSV));

            securityParameters.clientRandom = client_random;

            state.server.NotifyOfferedCipherSuites(state.offeredCipherSuites);
            state.server.NotifyOfferedCompressionMethods(state.offeredCompressionMethods);

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
                if (Arrays.Contains(state.offeredCipherSuites, CipherSuite.TLS_EMPTY_RENEGOTIATION_INFO_SCSV))
                {
                    state.secure_renegotiation = true;
                }

                /*
                 * The server MUST check if the "renegotiation_info" extension is included in the
                 * ClientHello.
                 */
                byte[] renegExtData = TlsUtilities.GetExtensionData(state.clientExtensions, ExtensionType.renegotiation_info);
                if (renegExtData != null)
                {
                    /*
                     * If the extension is present, set secure_renegotiation flag to TRUE. The
                     * server MUST then verify that the length of the "renegotiated_connection"
                     * field is zero, and if it is not, MUST abort the handshake.
                     */
                    state.secure_renegotiation = true;

                    if (!Arrays.ConstantTimeAreEqual(renegExtData, TlsProtocol.CreateRenegotiationInfo(TlsUtilities.EmptyBytes)))
                        throw new TlsFatalAlert(AlertDescription.handshake_failure);
                }
            }

            state.server.NotifySecureRenegotiation(state.secure_renegotiation);

            if (state.clientExtensions != null)
            {
                // NOTE: Validates the padding extension data, if present
                TlsExtensionsUtilities.GetPaddingExtension(state.clientExtensions);

                state.server.ProcessClientExtensions(state.clientExtensions);
            }
        }

        protected virtual void ProcessClientKeyExchange(ServerHandshakeState state, byte[] body)
        {
            MemoryStream buf = new MemoryStream(body, false);

            state.keyExchange.ProcessClientKeyExchange(buf);

            TlsProtocol.AssertEmpty(buf);
        }

        protected virtual void ProcessClientSupplementalData(ServerHandshakeState state, byte[] body)
        {
            MemoryStream buf = new MemoryStream(body, false);
            IList clientSupplementalData = TlsProtocol.ReadSupplementalDataMessage(buf);
            state.server.ProcessClientSupplementalData(clientSupplementalData);
        }

        protected virtual bool ExpectCertificateVerifyMessage(ServerHandshakeState state)
        {
            return state.clientCertificateType >= 0 && TlsUtilities.HasSigningCapability((byte)state.clientCertificateType);
        }

        protected internal class ServerHandshakeState
        {
            internal TlsServer server = null;
            internal TlsServerContextImpl serverContext = null;
            internal TlsSession tlsSession = null;
            internal SessionParameters sessionParameters = null;
            internal SessionParameters.Builder sessionParametersBuilder = null;
            internal int[] offeredCipherSuites = null;
            internal byte[] offeredCompressionMethods = null;
            internal IDictionary clientExtensions = null;
            internal IDictionary serverExtensions = null;
            internal bool resumedSession = false;
            internal bool secure_renegotiation = false;
            internal bool allowCertificateStatus = false;
            internal bool expectSessionTicket = false;
            internal TlsKeyExchange keyExchange = null;
            internal TlsCredentials serverCredentials = null;
            internal CertificateRequest certificateRequest = null;
            internal short clientCertificateType = -1;
            internal Certificate clientCertificate = null;
        }
    }
}
