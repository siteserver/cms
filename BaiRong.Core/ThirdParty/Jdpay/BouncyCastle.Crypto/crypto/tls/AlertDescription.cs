namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>
    /// RFC 5246 7.2
    /// </summary>
    public abstract class AlertDescription
    {
        /**
         * This message notifies the recipient that the sender will not send any more messages on this
         * connection. Note that as of TLS 1.1, failure to properly close a connection no longer
         * requires that a session not be resumed. This is a change from TLS 1.0 ("The session becomes
         * unresumable if any connection is terminated without proper close_notify messages with level
         * equal to warning.") to conform with widespread implementation practice.
         */
        public const byte close_notify = 0;

        /**
         * An inappropriate message was received. This alert is always fatal and should never be
         * observed in communication between proper implementations.
         */
        public const byte unexpected_message = 10;

        /**
         * This alert is returned if a record is received with an incorrect MAC. This alert also MUST be
         * returned if an alert is sent because a TLSCiphertext decrypted in an invalid way: either it
         * wasn't an even multiple of the block length, or its padding values, when checked, weren't
         * correct. This message is always fatal and should never be observed in communication between
         * proper implementations (except when messages were corrupted in the network).
         */
        public const byte bad_record_mac = 20;

        /**
         * This alert was used in some earlier versions of TLS, and may have permitted certain attacks
         * against the CBC mode [CBCATT]. It MUST NOT be sent by compliant implementations.
         */
        public const byte decryption_failed = 21;

        /**
         * A TLSCiphertext record was received that had a length more than 2^14+2048 bytes, or a record
         * decrypted to a TLSCompressed record with more than 2^14+1024 bytes. This message is always
         * fatal and should never be observed in communication between proper implementations (except
         * when messages were corrupted in the network).
         */
        public const byte record_overflow = 22;

        /**
         * The decompression function received improper input (e.g., data that would expand to excessive
         * length). This message is always fatal and should never be observed in communication between
         * proper implementations.
         */
        public const byte decompression_failure = 30;

        /**
         * Reception of a handshake_failure alert message indicates that the sender was unable to
         * negotiate an acceptable set of security parameters given the options available. This is a
         * fatal error.
         */
        public const byte handshake_failure = 40;

        /**
         * This alert was used in SSLv3 but not any version of TLS. It MUST NOT be sent by compliant
         * implementations.
         */
        public const byte no_certificate = 41;

        /**
         * A certificate was corrupt, contained signatures that did not verify correctly, etc.
         */
        public const byte bad_certificate = 42;

        /**
         * A certificate was of an unsupported type.
         */
        public const byte unsupported_certificate = 43;

        /**
         * A certificate was revoked by its signer.
         */
        public const byte certificate_revoked = 44;

        /**
         * A certificate has expired or is not currently valid.
         */
        public const byte certificate_expired = 45;

        /**
         * Some other (unspecified) issue arose in processing the certificate, rendering it
         * unacceptable.
         */
        public const byte certificate_unknown = 46;

        /**
         * A field in the handshake was out of range or inconsistent with other fields. This message is
         * always fatal.
         */
        public const byte illegal_parameter = 47;

        /**
         * A valid certificate chain or partial chain was received, but the certificate was not accepted
         * because the CA certificate could not be located or couldn't be matched with a known, trusted
         * CA. This message is always fatal.
         */
        public const byte unknown_ca = 48;

        /**
         * A valid certificate was received, but when access control was applied, the sender decided not
         * to proceed with negotiation. This message is always fatal.
         */
        public const byte access_denied = 49;

        /**
         * A message could not be decoded because some field was out of the specified range or the
         * length of the message was incorrect. This message is always fatal and should never be
         * observed in communication between proper implementations (except when messages were corrupted
         * in the network).
         */
        public const byte decode_error = 50;

        /**
         * A handshake cryptographic operation failed, including being unable to correctly verify a
         * signature or validate a Finished message. This message is always fatal.
         */
        public const byte decrypt_error = 51;

        /**
         * This alert was used in some earlier versions of TLS. It MUST NOT be sent by compliant
         * implementations.
         */
        public const byte export_restriction = 60;

        /**
         * The protocol version the client has attempted to negotiate is recognized but not supported.
         * (For example, old protocol versions might be avoided for security reasons.) This message is
         * always fatal.
         */
        public const byte protocol_version = 70;

        /**
         * Returned instead of handshake_failure when a negotiation has failed specifically because the
         * server requires ciphers more secure than those supported by the client. This message is
         * always fatal.
         */
        public const byte insufficient_security = 71;

        /**
         * An internal error unrelated to the peer or the correctness of the protocol (such as a memory
         * allocation failure) makes it impossible to continue. This message is always fatal.
         */
        public const byte internal_error = 80;

        /**
         * This handshake is being canceled for some reason unrelated to a protocol failure. If the user
         * cancels an operation after the handshake is complete, just closing the connection by sending
         * a close_notify is more appropriate. This alert should be followed by a close_notify. This
         * message is generally a warning.
         */
        public const byte user_canceled = 90;

        /**
         * Sent by the client in response to a hello request or by the server in response to a client
         * hello after initial handshaking. Either of these would normally lead to renegotiation; when
         * that is not appropriate, the recipient should respond with this alert. At that point, the
         * original requester can decide whether to proceed with the connection. One case where this
         * would be appropriate is where a server has spawned a process to satisfy a request; the
         * process might receive security parameters (key length, authentication, etc.) at startup, and
         * it might be difficult to communicate changes to these parameters after that point. This
         * message is always a warning.
         */
        public const byte no_renegotiation = 100;

        /**
         * Sent by clients that receive an extended server hello containing an extension that they did
         * not put in the corresponding client hello. This message is always fatal.
         */
        public const byte unsupported_extension = 110;

        /*
         * RFC 3546
         */

        /**
         * This alert is sent by servers who are unable to retrieve a certificate chain from the URL
         * supplied by the client (see Section 3.3). This message MAY be fatal - for example if client
         * authentication is required by the server for the handshake to continue and the server is
         * unable to retrieve the certificate chain, it may send a fatal alert.
         */
        public const byte certificate_unobtainable = 111;

        /**
         * This alert is sent by servers that receive a server_name extension request, but do not
         * recognize the server name. This message MAY be fatal.
         */
        public const byte unrecognized_name = 112;

        /**
         * This alert is sent by clients that receive an invalid certificate status response (see
         * Section 3.6). This message is always fatal.
         */
        public const byte bad_certificate_status_response = 113;

        /**
         * This alert is sent by servers when a certificate hash does not match a client provided
         * certificate_hash. This message is always fatal.
         */
        public const byte bad_certificate_hash_value = 114;

        /*
         * RFC 4279
         */

        /**
         * If the server does not recognize the PSK identity, it MAY respond with an
         * "unknown_psk_identity" alert message.
         */
        public const byte unknown_psk_identity = 115;

        /*
         * RFC 7507
         */

        /**
         * If TLS_FALLBACK_SCSV appears in ClientHello.cipher_suites and the highest protocol version
         * supported by the server is higher than the version indicated in ClientHello.client_version,
         * the server MUST respond with a fatal inappropriate_fallback alert [..].
         */
        public const byte inappropriate_fallback = 86;

        public static string GetName(byte alertDescription)
        {
            switch (alertDescription)
            {
            case close_notify:
                return "close_notify";
            case unexpected_message:
                return "unexpected_message";
            case bad_record_mac:
                return "bad_record_mac";
            case decryption_failed:
                return "decryption_failed";
            case record_overflow:
                return "record_overflow";
            case decompression_failure:
                return "decompression_failure";
            case handshake_failure:
                return "handshake_failure";
            case no_certificate:
                return "no_certificate";
            case bad_certificate:
                return "bad_certificate";
            case unsupported_certificate:
                return "unsupported_certificate";
            case certificate_revoked:
                return "certificate_revoked";
            case certificate_expired:
                return "certificate_expired";
            case certificate_unknown:
                return "certificate_unknown";
            case illegal_parameter:
                return "illegal_parameter";
            case unknown_ca:
                return "unknown_ca";
            case access_denied:
                return "access_denied";
            case decode_error:
                return "decode_error";
            case decrypt_error:
                return "decrypt_error";
            case export_restriction:
                return "export_restriction";
            case protocol_version:
                return "protocol_version";
            case insufficient_security:
                return "insufficient_security";
            case internal_error:
                return "internal_error";
            case user_canceled:
                return "user_canceled";
            case no_renegotiation:
                return "no_renegotiation";
            case unsupported_extension:
                return "unsupported_extension";
            case certificate_unobtainable:
                return "certificate_unobtainable";
            case unrecognized_name:
                return "unrecognized_name";
            case bad_certificate_status_response:
                return "bad_certificate_status_response";
            case bad_certificate_hash_value:
                return "bad_certificate_hash_value";
            case unknown_psk_identity:
                return "unknown_psk_identity";
            case inappropriate_fallback:
                return "inappropriate_fallback";
            default:
                return "UNKNOWN";
            }
        }

        public static string GetText(byte alertDescription)
        {
            return GetName(alertDescription) + "(" + alertDescription + ")";
        }
    }
}
