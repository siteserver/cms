using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class DtlsClientProtocol
        :   DtlsProtocol
    {
        public DtlsClientProtocol(SecureRandom secureRandom)
            :   base(secureRandom)
        {
        }

        public virtual DtlsTransport Connect(TlsClient client, DatagramTransport transport)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (transport == null)
                throw new ArgumentNullException("transport");

            SecurityParameters securityParameters = new SecurityParameters();
            securityParameters.entity = ConnectionEnd.client;

            ClientHandshakeState state = new ClientHandshakeState();
            state.client = client;
            state.clientContext = new TlsClientContextImpl(mSecureRandom, securityParameters);

            securityParameters.clientRandom = TlsProtocol.CreateRandomBlock(client.ShouldUseGmtUnixTime(),
                state.clientContext.NonceRandomGenerator);

            client.Init(state.clientContext);

            DtlsRecordLayer recordLayer = new DtlsRecordLayer(transport, state.clientContext, client, ContentType.handshake);

            TlsSession sessionToResume = state.client.GetSessionToResume();
            if (sessionToResume != null && sessionToResume.IsResumable)
            {
                SessionParameters sessionParameters = sessionToResume.ExportSessionParameters();
                if (sessionParameters != null)
                {
                    state.tlsSession = sessionToResume;
                    state.sessionParameters = sessionParameters;
                }
            }

            try
            {
                return ClientHandshake(state, recordLayer);
            }
            catch (TlsFatalAlert fatalAlert)
            {
                AbortClientHandshake(state, recordLayer, fatalAlert.AlertDescription);
                throw fatalAlert;
            }
            catch (IOException e)
            {
                AbortClientHandshake(state, recordLayer, AlertDescription.internal_error);
                throw e;
            }
            catch (Exception e)
            {
                AbortClientHandshake(state, recordLayer, AlertDescription.internal_error);
                throw new TlsFatalAlert(AlertDescription.internal_error, e);
            }
            finally
            {
                securityParameters.Clear();
            }
        }

        internal virtual void AbortClientHandshake(ClientHandshakeState state, DtlsRecordLayer recordLayer, byte alertDescription)
        {
            recordLayer.Fail(alertDescription);
            InvalidateSession(state);
        }

        internal virtual DtlsTransport ClientHandshake(ClientHandshakeState state, DtlsRecordLayer recordLayer)
        {
            SecurityParameters securityParameters = state.clientContext.SecurityParameters;
            DtlsReliableHandshake handshake = new DtlsReliableHandshake(state.clientContext, recordLayer);

            byte[] clientHelloBody = GenerateClientHello(state, state.client);

            recordLayer.SetWriteVersion(ProtocolVersion.DTLSv10);

            handshake.SendMessage(HandshakeType.client_hello, clientHelloBody);

            DtlsReliableHandshake.Message serverMessage = handshake.ReceiveMessage();

            while (serverMessage.Type == HandshakeType.hello_verify_request)
            {
                ProtocolVersion recordLayerVersion = recordLayer.ReadVersion;
                ProtocolVersion client_version = state.clientContext.ClientVersion;

                /*
                 * RFC 6347 4.2.1 DTLS 1.2 server implementations SHOULD use DTLS version 1.0 regardless of
                 * the version of TLS that is expected to be negotiated. DTLS 1.2 and 1.0 clients MUST use
                 * the version solely to indicate packet formatting (which is the same in both DTLS 1.2 and
                 * 1.0) and not as part of version negotiation.
                 */
                if (!recordLayerVersion.IsEqualOrEarlierVersionOf(client_version))
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                recordLayer.ReadVersion = null;

                byte[] cookie = ProcessHelloVerifyRequest(state, serverMessage.Body);
                byte[] patched = PatchClientHelloWithCookie(clientHelloBody, cookie);

                handshake.ResetHandshakeMessagesDigest();
                handshake.SendMessage(HandshakeType.client_hello, patched);

                serverMessage = handshake.ReceiveMessage();
            }

            if (serverMessage.Type == HandshakeType.server_hello)
            {
                ProtocolVersion recordLayerVersion = recordLayer.ReadVersion;
                ReportServerVersion(state, recordLayerVersion);
                recordLayer.SetWriteVersion(recordLayerVersion);

                ProcessServerHello(state, serverMessage.Body);
            }
            else
            {
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
            }

            handshake.NotifyHelloComplete();

            ApplyMaxFragmentLengthExtension(recordLayer, securityParameters.maxFragmentLength);

            if (state.resumedSession)
            {
                securityParameters.masterSecret = Arrays.Clone(state.sessionParameters.MasterSecret);
                recordLayer.InitPendingEpoch(state.client.GetCipher());

                // NOTE: Calculated exclusive of the actual Finished message from the server
                byte[] resExpectedServerVerifyData = TlsUtilities.CalculateVerifyData(state.clientContext, ExporterLabel.server_finished,
                    TlsProtocol.GetCurrentPrfHash(state.clientContext, handshake.HandshakeHash, null));
                ProcessFinished(handshake.ReceiveMessageBody(HandshakeType.finished), resExpectedServerVerifyData);

                // NOTE: Calculated exclusive of the Finished message itself
                byte[] resClientVerifyData = TlsUtilities.CalculateVerifyData(state.clientContext, ExporterLabel.client_finished,
                    TlsProtocol.GetCurrentPrfHash(state.clientContext, handshake.HandshakeHash, null));
                handshake.SendMessage(HandshakeType.finished, resClientVerifyData);

                handshake.Finish();

                state.clientContext.SetResumableSession(state.tlsSession);

                state.client.NotifyHandshakeComplete();

                return new DtlsTransport(recordLayer);
            }

            InvalidateSession(state);

            if (state.selectedSessionID.Length > 0)
            {
                state.tlsSession = new TlsSessionImpl(state.selectedSessionID, null);
            }

            serverMessage = handshake.ReceiveMessage();

            if (serverMessage.Type == HandshakeType.supplemental_data)
            {
                ProcessServerSupplementalData(state, serverMessage.Body);
                serverMessage = handshake.ReceiveMessage();
            }
            else
            {
                state.client.ProcessServerSupplementalData(null);
            }

            state.keyExchange = state.client.GetKeyExchange();
            state.keyExchange.Init(state.clientContext);

            Certificate serverCertificate = null;

            if (serverMessage.Type == HandshakeType.certificate)
            {
                serverCertificate = ProcessServerCertificate(state, serverMessage.Body);
                serverMessage = handshake.ReceiveMessage();
            }
            else
            {
                // Okay, Certificate is optional
                state.keyExchange.SkipServerCredentials();
            }

            // TODO[RFC 3546] Check whether empty certificates is possible, allowed, or excludes CertificateStatus
            if (serverCertificate == null || serverCertificate.IsEmpty)
            {
                state.allowCertificateStatus = false;
            }

            if (serverMessage.Type == HandshakeType.certificate_status)
            {
                ProcessCertificateStatus(state, serverMessage.Body);
                serverMessage = handshake.ReceiveMessage();
            }
            else
            {
                // Okay, CertificateStatus is optional
            }

            if (serverMessage.Type == HandshakeType.server_key_exchange)
            {
                ProcessServerKeyExchange(state, serverMessage.Body);
                serverMessage = handshake.ReceiveMessage();
            }
            else
            {
                // Okay, ServerKeyExchange is optional
                state.keyExchange.SkipServerKeyExchange();
            }

            if (serverMessage.Type == HandshakeType.certificate_request)
            {
                ProcessCertificateRequest(state, serverMessage.Body);

                /*
                 * TODO Give the client a chance to immediately select the CertificateVerify hash
                 * algorithm here to avoid tracking the other hash algorithms unnecessarily?
                 */
                TlsUtilities.TrackHashAlgorithms(handshake.HandshakeHash,
                    state.certificateRequest.SupportedSignatureAlgorithms);

                serverMessage = handshake.ReceiveMessage();
            }
            else
            {
                // Okay, CertificateRequest is optional
            }

            if (serverMessage.Type == HandshakeType.server_hello_done)
            {
                if (serverMessage.Body.Length != 0)
                {
                    throw new TlsFatalAlert(AlertDescription.decode_error);
                }
            }
            else
            {
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
            }

            handshake.HandshakeHash.SealHashAlgorithms();

            IList clientSupplementalData = state.client.GetClientSupplementalData();
            if (clientSupplementalData != null)
            {
                byte[] supplementalDataBody = GenerateSupplementalData(clientSupplementalData);
                handshake.SendMessage(HandshakeType.supplemental_data, supplementalDataBody);
            }

            if (state.certificateRequest != null)
            {
                state.clientCredentials = state.authentication.GetClientCredentials(state.certificateRequest);

                /*
                 * RFC 5246 If no suitable certificate is available, the client MUST send a certificate
                 * message containing no certificates.
                 * 
                 * NOTE: In previous RFCs, this was SHOULD instead of MUST.
                 */
                Certificate clientCertificate = null;
                if (state.clientCredentials != null)
                {
                    clientCertificate = state.clientCredentials.Certificate;
                }
                if (clientCertificate == null)
                {
                    clientCertificate = Certificate.EmptyChain;
                }

                byte[] certificateBody = GenerateCertificate(clientCertificate);
                handshake.SendMessage(HandshakeType.certificate, certificateBody);
            }

            if (state.clientCredentials != null)
            {
                state.keyExchange.ProcessClientCredentials(state.clientCredentials);
            }
            else
            {
                state.keyExchange.SkipClientCredentials();
            }

            byte[] clientKeyExchangeBody = GenerateClientKeyExchange(state);
            handshake.SendMessage(HandshakeType.client_key_exchange, clientKeyExchangeBody);

            TlsHandshakeHash prepareFinishHash = handshake.PrepareToFinish();
            securityParameters.sessionHash = TlsProtocol.GetCurrentPrfHash(state.clientContext, prepareFinishHash, null);

            TlsProtocol.EstablishMasterSecret(state.clientContext, state.keyExchange);
            recordLayer.InitPendingEpoch(state.client.GetCipher());

            if (state.clientCredentials != null && state.clientCredentials is TlsSignerCredentials)
            {
                TlsSignerCredentials signerCredentials = (TlsSignerCredentials)state.clientCredentials;

                /*
                 * RFC 5246 4.7. digitally-signed element needs SignatureAndHashAlgorithm from TLS 1.2
                 */
                SignatureAndHashAlgorithm signatureAndHashAlgorithm = TlsUtilities.GetSignatureAndHashAlgorithm(
                    state.clientContext, signerCredentials);

                byte[] hash;
                if (signatureAndHashAlgorithm == null)
                {
                    hash = securityParameters.SessionHash;
                }
                else
                {
                    hash = prepareFinishHash.GetFinalHash(signatureAndHashAlgorithm.Hash);
                }

                byte[] signature = signerCredentials.GenerateCertificateSignature(hash);
                DigitallySigned certificateVerify = new DigitallySigned(signatureAndHashAlgorithm, signature);
                byte[] certificateVerifyBody = GenerateCertificateVerify(state, certificateVerify);
                handshake.SendMessage(HandshakeType.certificate_verify, certificateVerifyBody);
            }

            // NOTE: Calculated exclusive of the Finished message itself
            byte[] clientVerifyData = TlsUtilities.CalculateVerifyData(state.clientContext, ExporterLabel.client_finished,
                TlsProtocol.GetCurrentPrfHash(state.clientContext, handshake.HandshakeHash, null));
            handshake.SendMessage(HandshakeType.finished, clientVerifyData);

            if (state.expectSessionTicket)
            {
                serverMessage = handshake.ReceiveMessage();
                if (serverMessage.Type == HandshakeType.session_ticket)
                {
                    ProcessNewSessionTicket(state, serverMessage.Body);
                }
                else
                {
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }
            }

            // NOTE: Calculated exclusive of the actual Finished message from the server
            byte[] expectedServerVerifyData = TlsUtilities.CalculateVerifyData(state.clientContext, ExporterLabel.server_finished,
                TlsProtocol.GetCurrentPrfHash(state.clientContext, handshake.HandshakeHash, null));
            ProcessFinished(handshake.ReceiveMessageBody(HandshakeType.finished), expectedServerVerifyData);

            handshake.Finish();

            if (state.tlsSession != null)
            {
                state.sessionParameters = new SessionParameters.Builder()
                    .SetCipherSuite(securityParameters.CipherSuite)
                    .SetCompressionAlgorithm(securityParameters.CompressionAlgorithm)
                    .SetMasterSecret(securityParameters.MasterSecret)
                    .SetPeerCertificate(serverCertificate)
                    .SetPskIdentity(securityParameters.PskIdentity)
                    .SetSrpIdentity(securityParameters.SrpIdentity)
                    // TODO Consider filtering extensions that aren't relevant to resumed sessions
                    .SetServerExtensions(state.serverExtensions)
                    .Build();

                state.tlsSession = TlsUtilities.ImportSession(state.tlsSession.SessionID, state.sessionParameters);

                state.clientContext.SetResumableSession(state.tlsSession);
            }

            state.client.NotifyHandshakeComplete();

            return new DtlsTransport(recordLayer);
        }

        protected virtual byte[] GenerateCertificateVerify(ClientHandshakeState state, DigitallySigned certificateVerify)
        {
            MemoryStream buf = new MemoryStream();
            certificateVerify.Encode(buf);
            return buf.ToArray();
        }

        protected virtual byte[] GenerateClientHello(ClientHandshakeState state, TlsClient client)
        {
            MemoryStream buf = new MemoryStream();

            ProtocolVersion client_version = client.ClientVersion;
            if (!client_version.IsDtls)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            TlsClientContextImpl context = state.clientContext;

            context.SetClientVersion(client_version);
            TlsUtilities.WriteVersion(client_version, buf);

            SecurityParameters securityParameters = context.SecurityParameters;
            buf.Write(securityParameters.ClientRandom, 0, securityParameters.ClientRandom.Length);

            // Session ID
            byte[] session_id = TlsUtilities.EmptyBytes;
            if (state.tlsSession != null)
            {
                session_id = state.tlsSession.SessionID;
                if (session_id == null || session_id.Length > 32)
                {
                    session_id = TlsUtilities.EmptyBytes;
                }
            }
            TlsUtilities.WriteOpaque8(session_id, buf);

            // Cookie
            TlsUtilities.WriteOpaque8(TlsUtilities.EmptyBytes, buf);

            bool fallback = client.IsFallback;

            /*
             * Cipher suites
             */
            state.offeredCipherSuites = client.GetCipherSuites();

            // Integer -> byte[]
            state.clientExtensions = client.GetClientExtensions();

            // Cipher Suites (and SCSV)
            {
                /*
                 * RFC 5746 3.4. The client MUST include either an empty "renegotiation_info" extension,
                 * or the TLS_EMPTY_RENEGOTIATION_INFO_SCSV signaling cipher suite value in the
                 * ClientHello. Including both is NOT RECOMMENDED.
                 */
                byte[] renegExtData = TlsUtilities.GetExtensionData(state.clientExtensions, ExtensionType.renegotiation_info);
                bool noRenegExt = (null == renegExtData);

                bool noRenegSCSV = !Arrays.Contains(state.offeredCipherSuites, CipherSuite.TLS_EMPTY_RENEGOTIATION_INFO_SCSV);

                if (noRenegExt && noRenegSCSV)
                {
                    // TODO Consider whether to default to a client extension instead
                    state.offeredCipherSuites = Arrays.Append(state.offeredCipherSuites, CipherSuite.TLS_EMPTY_RENEGOTIATION_INFO_SCSV);
                }

                /*
                 * RFC 7507 4. If a client sends a ClientHello.client_version containing a lower value
                 * than the latest (highest-valued) version supported by the client, it SHOULD include
                 * the TLS_FALLBACK_SCSV cipher suite value in ClientHello.cipher_suites [..]. (The
                 * client SHOULD put TLS_FALLBACK_SCSV after all cipher suites that it actually intends
                 * to negotiate.)
                 */
                if (fallback && !Arrays.Contains(state.offeredCipherSuites, CipherSuite.TLS_FALLBACK_SCSV))
                {
                    state.offeredCipherSuites = Arrays.Append(state.offeredCipherSuites, CipherSuite.TLS_FALLBACK_SCSV);
                }

                TlsUtilities.WriteUint16ArrayWithUint16Length(state.offeredCipherSuites, buf);
            }

            // TODO Add support for compression
            // Compression methods
            // state.offeredCompressionMethods = client.getCompressionMethods();
            state.offeredCompressionMethods = new byte[]{ CompressionMethod.cls_null };

            TlsUtilities.WriteUint8ArrayWithUint8Length(state.offeredCompressionMethods, buf);

            // Extensions
            if (state.clientExtensions != null)
            {
                TlsProtocol.WriteExtensions(buf, state.clientExtensions);
            }

            return buf.ToArray();
        }

        protected virtual byte[] GenerateClientKeyExchange(ClientHandshakeState state)
        {
            MemoryStream buf = new MemoryStream();
            state.keyExchange.GenerateClientKeyExchange(buf);
            return buf.ToArray();
        }

        protected virtual void InvalidateSession(ClientHandshakeState state)
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

        protected virtual void ProcessCertificateRequest(ClientHandshakeState state, byte[] body)
        {
            if (state.authentication == null)
            {
                /*
                 * RFC 2246 7.4.4. It is a fatal handshake_failure alert for an anonymous server to
                 * request client identification.
                 */
                throw new TlsFatalAlert(AlertDescription.handshake_failure);
            }

            MemoryStream buf = new MemoryStream(body, false);

            state.certificateRequest = CertificateRequest.Parse(state.clientContext, buf);

            TlsProtocol.AssertEmpty(buf);

            state.keyExchange.ValidateCertificateRequest(state.certificateRequest);
        }

        protected virtual void ProcessCertificateStatus(ClientHandshakeState state, byte[] body)
        {
            if (!state.allowCertificateStatus)
            {
                /*
                 * RFC 3546 3.6. If a server returns a "CertificateStatus" message, then the
                 * server MUST have included an extension of type "status_request" with empty
                 * "extension_data" in the extended server hello..
                 */
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
            }

            MemoryStream buf = new MemoryStream(body, false);

            state.certificateStatus = CertificateStatus.Parse(buf);

            TlsProtocol.AssertEmpty(buf);

            // TODO[RFC 3546] Figure out how to provide this to the client/authentication.
        }

        protected virtual byte[] ProcessHelloVerifyRequest(ClientHandshakeState state, byte[] body)
        {
            MemoryStream buf = new MemoryStream(body, false);

            ProtocolVersion server_version = TlsUtilities.ReadVersion(buf);
            byte[] cookie = TlsUtilities.ReadOpaque8(buf);

            TlsProtocol.AssertEmpty(buf);

            // TODO Seems this behaviour is not yet in line with OpenSSL for DTLS 1.2
    //        reportServerVersion(state, server_version);
            if (!server_version.IsEqualOrEarlierVersionOf(state.clientContext.ClientVersion))
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            /*
             * RFC 6347 This specification increases the cookie size limit to 255 bytes for greater
             * future flexibility. The limit remains 32 for previous versions of DTLS.
             */
            if (!ProtocolVersion.DTLSv12.IsEqualOrEarlierVersionOf(server_version) && cookie.Length > 32)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            return cookie;
        }

        protected virtual void ProcessNewSessionTicket(ClientHandshakeState state, byte[] body)
        {
            MemoryStream buf = new MemoryStream(body, false);

            NewSessionTicket newSessionTicket = NewSessionTicket.Parse(buf);

            TlsProtocol.AssertEmpty(buf);

            state.client.NotifyNewSessionTicket(newSessionTicket);
        }

        protected virtual Certificate ProcessServerCertificate(ClientHandshakeState state, byte[] body)
        {
            MemoryStream buf = new MemoryStream(body, false);

            Certificate serverCertificate = Certificate.Parse(buf);

            TlsProtocol.AssertEmpty(buf);

            state.keyExchange.ProcessServerCertificate(serverCertificate);
            state.authentication = state.client.GetAuthentication();
            state.authentication.NotifyServerCertificate(serverCertificate);

            return serverCertificate;
        }

        protected virtual void ProcessServerHello(ClientHandshakeState state, byte[] body)
        {
            SecurityParameters securityParameters = state.clientContext.SecurityParameters;

            MemoryStream buf = new MemoryStream(body, false);

            {
                ProtocolVersion server_version = TlsUtilities.ReadVersion(buf);
                ReportServerVersion(state, server_version);
            }

            securityParameters.serverRandom = TlsUtilities.ReadFully(32, buf);

            state.selectedSessionID = TlsUtilities.ReadOpaque8(buf);
            if (state.selectedSessionID.Length > 32)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            state.client.NotifySessionID(state.selectedSessionID);
            state.resumedSession = state.selectedSessionID.Length > 0 && state.tlsSession != null
                && Arrays.AreEqual(state.selectedSessionID, state.tlsSession.SessionID);

            int selectedCipherSuite = TlsUtilities.ReadUint16(buf);
            if (!Arrays.Contains(state.offeredCipherSuites, selectedCipherSuite)
                || selectedCipherSuite == CipherSuite.TLS_NULL_WITH_NULL_NULL
                || CipherSuite.IsScsv(selectedCipherSuite)
                || !TlsUtilities.IsValidCipherSuiteForVersion(selectedCipherSuite, state.clientContext.ServerVersion))
            {
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }
            ValidateSelectedCipherSuite(selectedCipherSuite, AlertDescription.illegal_parameter);
            state.client.NotifySelectedCipherSuite(selectedCipherSuite);

            byte selectedCompressionMethod = TlsUtilities.ReadUint8(buf);
            if (!Arrays.Contains(state.offeredCompressionMethods, selectedCompressionMethod))
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            state.client.NotifySelectedCompressionMethod(selectedCompressionMethod);

            /*
             * RFC3546 2.2 The extended server hello message format MAY be sent in place of the server
             * hello message when the client has requested extended functionality via the extended
             * client hello message specified in Section 2.1. ... Note that the extended server hello
             * message is only sent in response to an extended client hello message. This prevents the
             * possibility that the extended server hello message could "break" existing TLS 1.0
             * clients.
             */

            /*
             * TODO RFC 3546 2.3 If [...] the older session is resumed, then the server MUST ignore
             * extensions appearing in the client hello, and send a server hello containing no
             * extensions.
             */

            // Integer -> byte[]
            state.serverExtensions = TlsProtocol.ReadExtensions(buf);

            /*
             * RFC 3546 2.2 Note that the extended server hello message is only sent in response to an
             * extended client hello message. However, see RFC 5746 exception below. We always include
             * the SCSV, so an Extended Server Hello is always allowed.
             */
            if (state.serverExtensions != null)
            {
                foreach (int extType in state.serverExtensions.Keys)
                {
                    /*
                     * RFC 5746 3.6. Note that sending a "renegotiation_info" extension in response to a
                     * ClientHello containing only the SCSV is an explicit exception to the prohibition
                     * in RFC 5246, Section 7.4.1.4, on the server sending unsolicited extensions and is
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
                    if (null == TlsUtilities.GetExtensionData(state.clientExtensions, extType))
                        throw new TlsFatalAlert(AlertDescription.unsupported_extension);

                    /*
                     * RFC 3546 2.3. If [...] the older session is resumed, then the server MUST ignore
                     * extensions appearing in the client hello, and send a server hello containing no
                     * extensions[.]
                     */
                    if (state.resumedSession)
                    {
                        // TODO[compat-gnutls] GnuTLS test server sends server extensions e.g. ec_point_formats
                        // TODO[compat-openssl] OpenSSL test server sends server extensions e.g. ec_point_formats
                        // TODO[compat-polarssl] PolarSSL test server sends server extensions e.g. ec_point_formats
                        //throw new TlsFatalAlert(AlertDescription.illegal_parameter);
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
                byte[] renegExtData = TlsUtilities.GetExtensionData(state.serverExtensions, ExtensionType.renegotiation_info);
                if (renegExtData != null)
                {
                    /*
                        * If the extension is present, set the secure_renegotiation flag to TRUE. The
                        * client MUST then verify that the length of the "renegotiated_connection"
                        * field is zero, and if it is not, MUST abort the handshake (by sending a fatal
                        * handshake_failure alert).
                        */
                    state.secure_renegotiation = true;

                    if (!Arrays.ConstantTimeAreEqual(renegExtData, TlsProtocol.CreateRenegotiationInfo(TlsUtilities.EmptyBytes)))
                        throw new TlsFatalAlert(AlertDescription.handshake_failure);
                }
            }

            // TODO[compat-gnutls] GnuTLS test server fails to send renegotiation_info extension when resuming
            state.client.NotifySecureRenegotiation(state.secure_renegotiation);

            IDictionary sessionClientExtensions = state.clientExtensions, sessionServerExtensions = state.serverExtensions;
            if (state.resumedSession)
            {
                if (selectedCipherSuite != state.sessionParameters.CipherSuite
                    || selectedCompressionMethod != state.sessionParameters.CompressionAlgorithm)
                {
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                }

                sessionClientExtensions = null;
                sessionServerExtensions = state.sessionParameters.ReadServerExtensions();
            }

            securityParameters.cipherSuite = selectedCipherSuite;
            securityParameters.compressionAlgorithm = selectedCompressionMethod;

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
                    if (serverSentEncryptThenMAC && !TlsUtilities.IsBlockCipherSuite(securityParameters.CipherSuite))
                        throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                    securityParameters.encryptThenMac = serverSentEncryptThenMAC;
                }

                securityParameters.extendedMasterSecret = TlsExtensionsUtilities.HasExtendedMasterSecretExtension(sessionServerExtensions);

                securityParameters.maxFragmentLength = EvaluateMaxFragmentLengthExtension(state.resumedSession,
                    sessionClientExtensions, sessionServerExtensions, AlertDescription.illegal_parameter);

                securityParameters.truncatedHMac = TlsExtensionsUtilities.HasTruncatedHMacExtension(sessionServerExtensions);

                /*
                 * TODO It's surprising that there's no provision to allow a 'fresh' CertificateStatus to be
                 * sent in a session resumption handshake.
                 */
                state.allowCertificateStatus = !state.resumedSession
                    && TlsUtilities.HasExpectedEmptyExtensionData(sessionServerExtensions, ExtensionType.status_request,
                        AlertDescription.illegal_parameter);

                state.expectSessionTicket = !state.resumedSession
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
                state.client.ProcessServerExtensions(sessionServerExtensions);
            }

            securityParameters.prfAlgorithm = TlsProtocol.GetPrfAlgorithm(state.clientContext,
                securityParameters.CipherSuite);

            /*
             * RFC 5246 7.4.9. Any cipher suite which does not explicitly specify verify_data_length has
             * a verify_data_length equal to 12. This includes all existing cipher suites.
             */
            securityParameters.verifyDataLength = 12;
        }

        protected virtual void ProcessServerKeyExchange(ClientHandshakeState state, byte[] body)
        {
            MemoryStream buf = new MemoryStream(body, false);

            state.keyExchange.ProcessServerKeyExchange(buf);

            TlsProtocol.AssertEmpty(buf);
        }

        protected virtual void ProcessServerSupplementalData(ClientHandshakeState state, byte[] body)
        {
            MemoryStream buf = new MemoryStream(body, false);
            IList serverSupplementalData = TlsProtocol.ReadSupplementalDataMessage(buf);
            state.client.ProcessServerSupplementalData(serverSupplementalData);
        }

        protected virtual void ReportServerVersion(ClientHandshakeState state, ProtocolVersion server_version)
        {
            TlsClientContextImpl clientContext = state.clientContext;
            ProtocolVersion currentServerVersion = clientContext.ServerVersion;
            if (null == currentServerVersion)
            {
                clientContext.SetServerVersion(server_version);
                state.client.NotifyServerVersion(server_version);
            }
            else if (!currentServerVersion.Equals(server_version))
            {
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }
        }

        protected static byte[] PatchClientHelloWithCookie(byte[] clientHelloBody, byte[] cookie)
        {
            int sessionIDPos = 34;
            int sessionIDLength = TlsUtilities.ReadUint8(clientHelloBody, sessionIDPos);

            int cookieLengthPos = sessionIDPos + 1 + sessionIDLength;
            int cookiePos = cookieLengthPos + 1;

            byte[] patched = new byte[clientHelloBody.Length + cookie.Length];
            Array.Copy(clientHelloBody, 0, patched, 0, cookieLengthPos);
            TlsUtilities.CheckUint8(cookie.Length);
            TlsUtilities.WriteUint8((byte)cookie.Length, patched, cookieLengthPos);
            Array.Copy(cookie, 0, patched, cookiePos, cookie.Length);
            Array.Copy(clientHelloBody, cookiePos, patched, cookiePos + cookie.Length, clientHelloBody.Length - cookiePos);

            return patched;
        }

        protected internal class ClientHandshakeState
        {
            internal TlsClient client = null;
            internal TlsClientContextImpl clientContext = null;
            internal TlsSession tlsSession = null;
            internal SessionParameters sessionParameters = null;
            internal SessionParameters.Builder sessionParametersBuilder = null;
            internal int[] offeredCipherSuites = null;
            internal byte[] offeredCompressionMethods = null;
            internal IDictionary clientExtensions = null;
            internal IDictionary serverExtensions = null;
            internal byte[] selectedSessionID = null;
            internal bool resumedSession = false;
            internal bool secure_renegotiation = false;
            internal bool allowCertificateStatus = false;
            internal bool expectSessionTicket = false;
            internal TlsKeyExchange keyExchange = null;
            internal TlsAuthentication authentication = null;
            internal CertificateStatus certificateStatus = null;
            internal CertificateRequest certificateRequest = null;
            internal TlsCredentials clientCredentials = null;
        }
    }
}
