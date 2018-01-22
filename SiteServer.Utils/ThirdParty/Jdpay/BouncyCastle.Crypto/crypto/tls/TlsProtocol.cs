using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public abstract class TlsProtocol
    {
        /*
         * Our Connection states
         */
        protected const short CS_START = 0;
        protected const short CS_CLIENT_HELLO = 1;
        protected const short CS_SERVER_HELLO = 2;
        protected const short CS_SERVER_SUPPLEMENTAL_DATA = 3;
        protected const short CS_SERVER_CERTIFICATE = 4;
        protected const short CS_CERTIFICATE_STATUS = 5;
        protected const short CS_SERVER_KEY_EXCHANGE = 6;
        protected const short CS_CERTIFICATE_REQUEST = 7;
        protected const short CS_SERVER_HELLO_DONE = 8;
        protected const short CS_CLIENT_SUPPLEMENTAL_DATA = 9;
        protected const short CS_CLIENT_CERTIFICATE = 10;
        protected const short CS_CLIENT_KEY_EXCHANGE = 11;
        protected const short CS_CERTIFICATE_VERIFY = 12;
        protected const short CS_CLIENT_FINISHED = 13;
        protected const short CS_SERVER_SESSION_TICKET = 14;
        protected const short CS_SERVER_FINISHED = 15;
        protected const short CS_END = 16;

        /*
         * Different modes to handle the known IV weakness
         */
        protected const short ADS_MODE_1_Nsub1 = 0; // 1/n-1 record splitting
        protected const short ADS_MODE_0_N = 1; // 0/n record splitting
        protected const short ADS_MODE_0_N_FIRSTONLY = 2; // 0/n record splitting on first data fragment only

        /*
         * Queues for data from some protocols.
         */
        private ByteQueue mApplicationDataQueue = new ByteQueue(0);
        private ByteQueue mAlertQueue = new ByteQueue(2);
        private ByteQueue mHandshakeQueue = new ByteQueue(0);
    //    private ByteQueue mHeartbeatQueue = new ByteQueue();

        /*
         * The Record Stream we use
         */
        internal RecordStream mRecordStream;
        protected SecureRandom mSecureRandom;

        private TlsStream mTlsStream = null;

        private volatile bool mClosed = false;
        private volatile bool mFailedWithError = false;
        private volatile bool mAppDataReady = false;
        private volatile bool mAppDataSplitEnabled = true;
        private volatile int mAppDataSplitMode = ADS_MODE_1_Nsub1;
        private byte[] mExpectedVerifyData = null;

        protected TlsSession mTlsSession = null;
        protected SessionParameters mSessionParameters = null;
        protected SecurityParameters mSecurityParameters = null;
        protected Certificate mPeerCertificate = null;

        protected int[] mOfferedCipherSuites = null;
        protected byte[] mOfferedCompressionMethods = null;
        protected IDictionary mClientExtensions = null;
        protected IDictionary mServerExtensions = null;

        protected short mConnectionState = CS_START;
        protected bool mResumedSession = false;
        protected bool mReceivedChangeCipherSpec = false;
        protected bool mSecureRenegotiation = false;
        protected bool mAllowCertificateStatus = false;
        protected bool mExpectSessionTicket = false;

        protected bool mBlocking = true;
        protected ByteQueueStream mInputBuffers = null;
        protected ByteQueueStream mOutputBuffer = null;

        public TlsProtocol(Stream stream, SecureRandom secureRandom)
            :   this(stream, stream, secureRandom)
        {
        }

        public TlsProtocol(Stream input, Stream output, SecureRandom secureRandom)
        {
            this.mRecordStream = new RecordStream(this, input, output);
            this.mSecureRandom = secureRandom;
        }

        public TlsProtocol(SecureRandom secureRandom)
        {
            this.mBlocking = false;
            this.mInputBuffers = new ByteQueueStream();
            this.mOutputBuffer = new ByteQueueStream();
            this.mRecordStream = new RecordStream(this, mInputBuffers, mOutputBuffer);
            this.mSecureRandom = secureRandom;
        }

        protected abstract TlsContext Context { get; }

        internal abstract AbstractTlsContext ContextAdmin { get; }

        protected abstract TlsPeer Peer { get; }

        protected virtual void HandleAlertMessage(byte alertLevel, byte alertDescription)
        {
            Peer.NotifyAlertReceived(alertLevel, alertDescription);

            if (alertLevel == AlertLevel.warning)
            {
                HandleAlertWarningMessage(alertDescription);
            }
            else
            {
                HandleFailure();

                throw new TlsFatalAlertReceived(alertDescription);
            }
        }

        protected virtual void HandleAlertWarningMessage(byte alertDescription)
        {
            /*
             * RFC 5246 7.2.1. The other party MUST respond with a close_notify alert of its own
             * and close down the connection immediately, discarding any pending writes.
             */
            if (alertDescription == AlertDescription.close_notify)
            {
                if (!mAppDataReady)
                    throw new TlsFatalAlert(AlertDescription.handshake_failure);

                HandleClose(false);
            }
        }

        protected virtual void HandleChangeCipherSpecMessage()
        {
        }

        protected virtual void HandleClose(bool user_canceled)
        {
            if (!mClosed)
            {
                this.mClosed = true;

                if (user_canceled && !mAppDataReady)
                {
                    RaiseAlertWarning(AlertDescription.user_canceled, "User canceled handshake");
                }

                RaiseAlertWarning(AlertDescription.close_notify, "Connection closed");

                mRecordStream.SafeClose();

                if (!mAppDataReady)
                {
                    CleanupHandshake();
                }
            }
        }

        protected virtual void HandleException(byte alertDescription, string message, Exception cause)
        {
            if (!mClosed)
            {
                RaiseAlertFatal(alertDescription, message, cause);

                HandleFailure();
            }
        }

        protected virtual void HandleFailure()
        {
            this.mClosed = true;
            this.mFailedWithError = true;

            /*
             * RFC 2246 7.2.1. The session becomes unresumable if any connection is terminated
             * without proper close_notify messages with level equal to warning.
             */
            // TODO This isn't quite in the right place. Also, as of TLS 1.1 the above is obsolete.
            InvalidateSession();

            mRecordStream.SafeClose();

            if (!mAppDataReady)
            {
                CleanupHandshake();
            }
        }

        protected abstract void HandleHandshakeMessage(byte type, MemoryStream buf);

        protected virtual void ApplyMaxFragmentLengthExtension()
        {
            if (mSecurityParameters.maxFragmentLength >= 0)
            {
                if (!MaxFragmentLength.IsValid((byte)mSecurityParameters.maxFragmentLength))
                    throw new TlsFatalAlert(AlertDescription.internal_error);

                int plainTextLimit = 1 << (8 + mSecurityParameters.maxFragmentLength);
                mRecordStream.SetPlaintextLimit(plainTextLimit);
            }
        }

        protected virtual void CheckReceivedChangeCipherSpec(bool expected)
        {
            if (expected != mReceivedChangeCipherSpec)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);
        }

        protected virtual void CleanupHandshake()
        {
            if (this.mExpectedVerifyData != null)
            {
                Arrays.Fill(this.mExpectedVerifyData, (byte)0);
                this.mExpectedVerifyData = null;
            }

            this.mSecurityParameters.Clear();
            this.mPeerCertificate = null;

            this.mOfferedCipherSuites = null;
            this.mOfferedCompressionMethods = null;
            this.mClientExtensions = null;
            this.mServerExtensions = null;

            this.mResumedSession = false;
            this.mReceivedChangeCipherSpec = false;
            this.mSecureRenegotiation = false;
            this.mAllowCertificateStatus = false;
            this.mExpectSessionTicket = false;
        }

        protected virtual void BlockForHandshake()
        {
            if (mBlocking)
            {
                while (this.mConnectionState != CS_END)
                {
                    if (this.mClosed)
                    {
                        // NOTE: Any close during the handshake should have raised an exception.
                        throw new TlsFatalAlert(AlertDescription.internal_error);
                    }

                    SafeReadRecord();
                }
            }
        }

        protected virtual void CompleteHandshake()
        {
            try
            {
                this.mConnectionState = CS_END;

                this.mAlertQueue.Shrink();
                this.mHandshakeQueue.Shrink();

                this.mRecordStream.FinaliseHandshake();

                this.mAppDataSplitEnabled = !TlsUtilities.IsTlsV11(Context);

                /*
                 * If this was an initial handshake, we are now ready to send and receive application data.
                 */
                if (!mAppDataReady)
                {
                    this.mAppDataReady = true;

                    if (mBlocking)
                    {
                        this.mTlsStream = new TlsStream(this);
                    }
                }

                if (this.mTlsSession != null)
                {
                    if (this.mSessionParameters == null)
                    {
                        this.mSessionParameters = new SessionParameters.Builder()
                            .SetCipherSuite(this.mSecurityParameters.CipherSuite)
                            .SetCompressionAlgorithm(this.mSecurityParameters.CompressionAlgorithm)
                            .SetMasterSecret(this.mSecurityParameters.MasterSecret)
                            .SetPeerCertificate(this.mPeerCertificate)
                            .SetPskIdentity(this.mSecurityParameters.PskIdentity)
                            .SetSrpIdentity(this.mSecurityParameters.SrpIdentity)
                            // TODO Consider filtering extensions that aren't relevant to resumed sessions
                            .SetServerExtensions(this.mServerExtensions)
                            .Build();

                        this.mTlsSession = new TlsSessionImpl(this.mTlsSession.SessionID, this.mSessionParameters);
                    }

                    ContextAdmin.SetResumableSession(this.mTlsSession);
                }

                Peer.NotifyHandshakeComplete();
            }
            finally
            {
                CleanupHandshake();
            }
        }

        protected internal void ProcessRecord(byte protocol, byte[] buf, int off, int len)
        {
            /*
             * Have a look at the protocol type, and add it to the correct queue.
             */
            switch (protocol)
            {
            case ContentType.alert:
            {
                mAlertQueue.AddData(buf, off, len);
                ProcessAlertQueue();
                break;
            }
            case ContentType.application_data:
            {
                if (!mAppDataReady)
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);

                mApplicationDataQueue.AddData(buf, off, len);
                ProcessApplicationDataQueue();
                break;
            }
            case ContentType.change_cipher_spec:
            {
                ProcessChangeCipherSpec(buf, off, len);
                break;
            }
            case ContentType.handshake:
            {
                if (mHandshakeQueue.Available > 0)
                {
                    mHandshakeQueue.AddData(buf, off, len);
                    ProcessHandshakeQueue(mHandshakeQueue);
                }
                else
                {
                    ByteQueue tmpQueue = new ByteQueue(buf, off, len);
                    ProcessHandshakeQueue(tmpQueue);
                    int remaining = tmpQueue.Available;
                    if (remaining > 0)
                    {
                        mHandshakeQueue.AddData(buf, off + len - remaining, remaining);
                    }
                }
                break;
            }
            //case ContentType.heartbeat:
            //{
            //    if (!mAppDataReady)
            //        throw new TlsFatalAlert(AlertDescription.unexpected_message);

            //    // TODO[RFC 6520]
            //    //mHeartbeatQueue.AddData(buf, offset, len);
            //    //ProcessHeartbeat();
            //    break;
            //}
            default:
                // Record type should already have been checked
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        private void ProcessHandshakeQueue(ByteQueue queue)
        {
            while (queue.Available >= 4)
            {
                /*
                 * We need the first 4 bytes, they contain type and length of the message.
                 */
                byte[] beginning = new byte[4];
                queue.Read(beginning, 0, 4, 0);
                byte type = TlsUtilities.ReadUint8(beginning, 0);
                int length = TlsUtilities.ReadUint24(beginning, 1);
                int totalLength = 4 + length;

                /*
                 * Check if we have enough bytes in the buffer to read the full message.
                 */
                if (queue.Available < totalLength)
                    break;

                CheckReceivedChangeCipherSpec(mConnectionState == CS_END || type == HandshakeType.finished);

                /*
                 * RFC 2246 7.4.9. The value handshake_messages includes all handshake messages
                 * starting at client hello up to, but not including, this finished message.
                 * [..] Note: [Also,] Hello Request messages are omitted from handshake hashes.
                 */
                switch (type)
                {
                case HandshakeType.hello_request:
                    break;
                case HandshakeType.finished:
                default:
                {
                    TlsContext ctx = Context;
                    if (type == HandshakeType.finished
                        && this.mExpectedVerifyData == null
                        && ctx.SecurityParameters.MasterSecret != null)
                    {
                        this.mExpectedVerifyData = CreateVerifyData(!ctx.IsServer);
                    }

                    queue.CopyTo(mRecordStream.HandshakeHashUpdater, totalLength);
                    break;
                }
                }

                queue.RemoveData(4);

                MemoryStream buf = queue.ReadFrom(length);

                /*
                 * Now, parse the message.
                 */
                HandleHandshakeMessage(type, buf);
            }
        }

        private void ProcessApplicationDataQueue()
        {
            /*
             * There is nothing we need to do here.
             * 
             * This function could be used for callbacks when application data arrives in the future.
             */
        }

        private void ProcessAlertQueue()
        {
            while (mAlertQueue.Available >= 2)
            {
                /*
                 * An alert is always 2 bytes. Read the alert.
                 */
                byte[] alert = mAlertQueue.RemoveData(2, 0);
                byte alertLevel = alert[0];
                byte alertDescription = alert[1];

                HandleAlertMessage(alertLevel, alertDescription);
            }
        }

        /**
         * This method is called, when a change cipher spec message is received.
         *
         * @throws IOException If the message has an invalid content or the handshake is not in the correct
         * state.
         */
        private void ProcessChangeCipherSpec(byte[] buf, int off, int len)
        {
            for (int i = 0; i < len; ++i)
            {
                byte message = TlsUtilities.ReadUint8(buf, off + i);

                if (message != ChangeCipherSpec.change_cipher_spec)
                    throw new TlsFatalAlert(AlertDescription.decode_error);

                if (this.mReceivedChangeCipherSpec
                    || mAlertQueue.Available > 0
                    || mHandshakeQueue.Available > 0)
                {
                    throw new TlsFatalAlert(AlertDescription.unexpected_message);
                }

                mRecordStream.ReceivedReadCipherSpec();

                this.mReceivedChangeCipherSpec = true;

                HandleChangeCipherSpecMessage();
            }
        }

        protected internal virtual int ApplicationDataAvailable()
        {
            return mApplicationDataQueue.Available;
        }

        /**
         * Read data from the network. The method will return immediately, if there is still some data
         * left in the buffer, or block until some application data has been read from the network.
         *
         * @param buf    The buffer where the data will be copied to.
         * @param offset The position where the data will be placed in the buffer.
         * @param len    The maximum number of bytes to read.
         * @return The number of bytes read.
         * @throws IOException If something goes wrong during reading data.
         */
        protected internal virtual int ReadApplicationData(byte[] buf, int offset, int len)
        {
            if (len < 1)
                return 0;

            while (mApplicationDataQueue.Available == 0)
            {
                if (this.mClosed)
                {
                    if (this.mFailedWithError)
                        throw new IOException("Cannot read application data on failed TLS connection");

                    if (!mAppDataReady)
                        throw new InvalidOperationException("Cannot read application data until initial handshake completed.");

                    return 0;
                }

                SafeReadRecord();
            }

            len = System.Math.Min(len, mApplicationDataQueue.Available);
            mApplicationDataQueue.RemoveData(buf, offset, len, 0);
            return len;
        }

        protected virtual void SafeCheckRecordHeader(byte[] recordHeader)
        {
            try
            {
                mRecordStream.CheckRecordHeader(recordHeader);
            }
            catch (TlsFatalAlert e)
            {
                HandleException(e.AlertDescription, "Failed to read record", e);
                throw e;
            }
            catch (IOException e)
            {
                HandleException(AlertDescription.internal_error, "Failed to read record", e);
                throw e;
            }
            catch (Exception e)
            {
                HandleException(AlertDescription.internal_error, "Failed to read record", e);
                throw new TlsFatalAlert(AlertDescription.internal_error, e);
            }
        }

        protected virtual void SafeReadRecord()
        {
            try
            {
                if (mRecordStream.ReadRecord())
                    return;

                if (!mAppDataReady)
                    throw new TlsFatalAlert(AlertDescription.handshake_failure);
            }
            catch (TlsFatalAlertReceived e)
            {
                // Connection failure already handled at source
                throw e;
            }
            catch (TlsFatalAlert e)
            {
                HandleException(e.AlertDescription, "Failed to read record", e);
                throw e;
            }
            catch (IOException e)
            {
                HandleException(AlertDescription.internal_error, "Failed to read record", e);
                throw e;
            }
            catch (Exception e)
            {
                HandleException(AlertDescription.internal_error, "Failed to read record", e);
                throw new TlsFatalAlert(AlertDescription.internal_error, e);
            }

            HandleFailure();

            throw new TlsNoCloseNotifyException();
        }

        protected virtual void SafeWriteRecord(byte type, byte[] buf, int offset, int len)
        {
            try
            {
                mRecordStream.WriteRecord(type, buf, offset, len);
            }
            catch (TlsFatalAlert e)
            {
                HandleException(e.AlertDescription, "Failed to write record", e);
                throw e;
            }
            catch (IOException e)
            {
                HandleException(AlertDescription.internal_error, "Failed to write record", e);
                throw e;
            }
            catch (Exception e)
            {
                HandleException(AlertDescription.internal_error, "Failed to write record", e);
                throw new TlsFatalAlert(AlertDescription.internal_error, e);
            }
        }

        /**
         * Send some application data to the remote system.
         * <p/>
         * The method will handle fragmentation internally.
         *
         * @param buf    The buffer with the data.
         * @param offset The position in the buffer where the data is placed.
         * @param len    The length of the data.
         * @throws IOException If something goes wrong during sending.
         */
        protected internal virtual void WriteData(byte[] buf, int offset, int len)
        {
            if (this.mClosed)
                throw new IOException("Cannot write application data on closed/failed TLS connection");

            while (len > 0)
            {
                /*
                 * RFC 5246 6.2.1. Zero-length fragments of Application data MAY be sent as they are
                 * potentially useful as a traffic analysis countermeasure.
                 * 
                 * NOTE: Actually, implementations appear to have settled on 1/n-1 record splitting.
                 */

                if (this.mAppDataSplitEnabled)
                {
                    /*
                     * Protect against known IV attack!
                     * 
                     * DO NOT REMOVE THIS CODE, EXCEPT YOU KNOW EXACTLY WHAT YOU ARE DOING HERE.
                     */
                    switch (mAppDataSplitMode)
                    {
                    case ADS_MODE_0_N:
                        SafeWriteRecord(ContentType.application_data, TlsUtilities.EmptyBytes, 0, 0);
                        break;
                    case ADS_MODE_0_N_FIRSTONLY:
                        this.mAppDataSplitEnabled = false;
                        SafeWriteRecord(ContentType.application_data, TlsUtilities.EmptyBytes, 0, 0);
                        break;
                    case ADS_MODE_1_Nsub1:
                    default:
                        SafeWriteRecord(ContentType.application_data, buf, offset, 1);
                        ++offset;
                        --len;
                        break;
                    }
                }

                if (len > 0)
                {
                    // Fragment data according to the current fragment limit.
                    int toWrite = System.Math.Min(len, mRecordStream.GetPlaintextLimit());
                    SafeWriteRecord(ContentType.application_data, buf, offset, toWrite);
                    offset += toWrite;
                    len -= toWrite;
                }
            }
        }

        protected virtual void SetAppDataSplitMode(int appDataSplitMode)
        {
            if (appDataSplitMode < ADS_MODE_1_Nsub1 || appDataSplitMode > ADS_MODE_0_N_FIRSTONLY)
                throw new ArgumentException("Illegal appDataSplitMode mode: " + appDataSplitMode, "appDataSplitMode");

            this.mAppDataSplitMode = appDataSplitMode;
        }

        protected virtual void WriteHandshakeMessage(byte[] buf, int off, int len)
        {
            if (len < 4)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            byte type = TlsUtilities.ReadUint8(buf, off);
            if (type != HandshakeType.hello_request)
            {
                mRecordStream.HandshakeHashUpdater.Write(buf, off, len);
            }

            int total = 0;
            do
            {
                // Fragment data according to the current fragment limit.
                int toWrite = System.Math.Min(len - total, mRecordStream.GetPlaintextLimit());
                SafeWriteRecord(ContentType.handshake, buf, off + total, toWrite);
                total += toWrite;
            }
            while (total < len);
        }

        /// <summary>The secure bidirectional stream for this connection</summary>
        /// <remarks>Only allowed in blocking mode.</remarks>
        public virtual Stream Stream
        {
            get
            {
                if (!mBlocking)
                    throw new InvalidOperationException("Cannot use Stream in non-blocking mode! Use OfferInput()/OfferOutput() instead.");
                return this.mTlsStream;
            }
        }

        /**
         * Should be called in non-blocking mode when the input data reaches EOF.
         */
        public virtual void CloseInput()
        {
            if (mBlocking)
                throw new InvalidOperationException("Cannot use CloseInput() in blocking mode!");

            if (mClosed)
                return;

            if (mInputBuffers.Available > 0)
                throw new EndOfStreamException();

            if (!mAppDataReady)
                throw new TlsFatalAlert(AlertDescription.handshake_failure);

            throw new TlsNoCloseNotifyException();
        }

        /**
         * Offer input from an arbitrary source. Only allowed in non-blocking mode.<br/>
         * <br/>
         * After this method returns, the input buffer is "owned" by this object. Other code
         * must not attempt to do anything with it.<br/>
         * <br/>
         * This method will decrypt and process all records that are fully available.
         * If only part of a record is available, the buffer will be retained until the
         * remainder of the record is offered.<br/>
         * <br/>
         * If any records containing application data were processed, the decrypted data
         * can be obtained using {@link #readInput(byte[], int, int)}. If any records
         * containing protocol data were processed, a response may have been generated.
         * You should always check to see if there is any available output after calling
         * this method by calling {@link #getAvailableOutputBytes()}.
         * @param input The input buffer to offer
         * @throws IOException If an error occurs while decrypting or processing a record
         */
        public virtual void OfferInput(byte[] input)
        {
            if (mBlocking)
                throw new InvalidOperationException("Cannot use OfferInput() in blocking mode! Use Stream instead.");
            if (mClosed)
                throw new IOException("Connection is closed, cannot accept any more input");

            mInputBuffers.Write(input);

            // loop while there are enough bytes to read the length of the next record
            while (mInputBuffers.Available >= RecordStream.TLS_HEADER_SIZE)
            {
                byte[] recordHeader = new byte[RecordStream.TLS_HEADER_SIZE];
                mInputBuffers.Peek(recordHeader);

                int totalLength = TlsUtilities.ReadUint16(recordHeader, RecordStream.TLS_HEADER_LENGTH_OFFSET) + RecordStream.TLS_HEADER_SIZE;
                if (mInputBuffers.Available < totalLength)
                {
                    // not enough bytes to read a whole record
                    SafeCheckRecordHeader(recordHeader);
                    break;
                }

                SafeReadRecord();

                if (mClosed)
                {
                    if (mConnectionState != CS_END)
                    {
                        // NOTE: Any close during the handshake should have raised an exception.
                        throw new TlsFatalAlert(AlertDescription.internal_error);
                    }
                    break;
                }
            }
        }

        /**
         * Gets the amount of received application data. A call to {@link #readInput(byte[], int, int)}
         * is guaranteed to be able to return at least this much data.<br/>
         * <br/>
         * Only allowed in non-blocking mode.
         * @return The number of bytes of available application data
         */
        public virtual int GetAvailableInputBytes()
        {
            if (mBlocking)
                throw new InvalidOperationException("Cannot use GetAvailableInputBytes() in blocking mode! Use ApplicationDataAvailable() instead.");

            return ApplicationDataAvailable();
        }

        /**
         * Retrieves received application data. Use {@link #getAvailableInputBytes()} to check
         * how much application data is currently available. This method functions similarly to
         * {@link InputStream#read(byte[], int, int)}, except that it never blocks. If no data
         * is available, nothing will be copied and zero will be returned.<br/>
         * <br/>
         * Only allowed in non-blocking mode.
         * @param buffer The buffer to hold the application data
         * @param offset The start offset in the buffer at which the data is written
         * @param length The maximum number of bytes to read
         * @return The total number of bytes copied to the buffer. May be less than the
         *          length specified if the length was greater than the amount of available data.
         */
        public virtual int ReadInput(byte[] buffer, int offset, int length)
        {
            if (mBlocking)
                throw new InvalidOperationException("Cannot use ReadInput() in blocking mode! Use Stream instead.");

            return ReadApplicationData(buffer, offset, System.Math.Min(length, ApplicationDataAvailable()));
        }

        /**
         * Offer output from an arbitrary source. Only allowed in non-blocking mode.<br/>
         * <br/>
         * After this method returns, the specified section of the buffer will have been
         * processed. Use {@link #readOutput(byte[], int, int)} to get the bytes to
         * transmit to the other peer.<br/>
         * <br/>
         * This method must not be called until after the handshake is complete! Attempting
         * to call it before the handshake is complete will result in an exception.
         * @param buffer The buffer containing application data to encrypt
         * @param offset The offset at which to begin reading data
         * @param length The number of bytes of data to read
         * @throws IOException If an error occurs encrypting the data, or the handshake is not complete
         */
        public virtual void OfferOutput(byte[] buffer, int offset, int length)
        {
            if (mBlocking)
                throw new InvalidOperationException("Cannot use OfferOutput() in blocking mode! Use Stream instead.");
            if (!mAppDataReady)
                throw new IOException("Application data cannot be sent until the handshake is complete!");

            WriteData(buffer, offset, length);
        }

        /**
         * Gets the amount of encrypted data available to be sent. A call to
         * {@link #readOutput(byte[], int, int)} is guaranteed to be able to return at
         * least this much data.<br/>
         * <br/>
         * Only allowed in non-blocking mode.
         * @return The number of bytes of available encrypted data
         */
        public virtual int GetAvailableOutputBytes()
        {
            if (mBlocking)
                throw new InvalidOperationException("Cannot use GetAvailableOutputBytes() in blocking mode! Use Stream instead.");

            return mOutputBuffer.Available;
        }

        /**
         * Retrieves encrypted data to be sent. Use {@link #getAvailableOutputBytes()} to check
         * how much encrypted data is currently available. This method functions similarly to
         * {@link InputStream#read(byte[], int, int)}, except that it never blocks. If no data
         * is available, nothing will be copied and zero will be returned.<br/>
         * <br/>
         * Only allowed in non-blocking mode.
         * @param buffer The buffer to hold the encrypted data
         * @param offset The start offset in the buffer at which the data is written
         * @param length The maximum number of bytes to read
         * @return The total number of bytes copied to the buffer. May be less than the
         *          length specified if the length was greater than the amount of available data.
         */
        public virtual int ReadOutput(byte[] buffer, int offset, int length)
        {
            if (mBlocking)
                throw new InvalidOperationException("Cannot use ReadOutput() in blocking mode! Use Stream instead.");

            return mOutputBuffer.Read(buffer, offset, length);
        }

        protected virtual void InvalidateSession()
        {
            if (this.mSessionParameters != null)
            {
                this.mSessionParameters.Clear();
                this.mSessionParameters = null;
            }

            if (this.mTlsSession != null)
            {
                this.mTlsSession.Invalidate();
                this.mTlsSession = null;
            }
        }

        protected virtual void ProcessFinishedMessage(MemoryStream buf)
        {
            if (mExpectedVerifyData == null)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            byte[] verify_data = TlsUtilities.ReadFully(mExpectedVerifyData.Length, buf);

            AssertEmpty(buf);

            /*
             * Compare both checksums.
             */
            if (!Arrays.ConstantTimeAreEqual(mExpectedVerifyData, verify_data))
            {
                /*
                 * Wrong checksum in the finished message.
                 */
                throw new TlsFatalAlert(AlertDescription.decrypt_error);
            }
        }

        protected virtual void RaiseAlertFatal(byte alertDescription, string message, Exception cause)
        {
            Peer.NotifyAlertRaised(AlertLevel.fatal, alertDescription, message, cause);

            byte[] alert = new byte[]{ AlertLevel.fatal, alertDescription };

            try
            {
                mRecordStream.WriteRecord(ContentType.alert, alert, 0, 2);
            }
            catch (Exception)
            {
                // We are already processing an exception, so just ignore this
            }
        }

        protected virtual void RaiseAlertWarning(byte alertDescription, string message)
        {
            Peer.NotifyAlertRaised(AlertLevel.warning, alertDescription, message, null);

            byte[] alert = new byte[]{ AlertLevel.warning, alertDescription };

            SafeWriteRecord(ContentType.alert, alert, 0, 2);
        }

        protected virtual void SendCertificateMessage(Certificate certificate)
        {
            if (certificate == null)
            {
                certificate = Certificate.EmptyChain;
            }

            if (certificate.IsEmpty)
            {
                TlsContext context = Context;
                if (!context.IsServer)
                {
                    ProtocolVersion serverVersion = Context.ServerVersion;
                    if (serverVersion.IsSsl)
                    {
                        string errorMessage = serverVersion.ToString() + " client didn't provide credentials";
                        RaiseAlertWarning(AlertDescription.no_certificate, errorMessage);
                        return;
                    }
                }
            }

            HandshakeMessage message = new HandshakeMessage(HandshakeType.certificate);

            certificate.Encode(message);

            message.WriteToRecordStream(this);
        }

        protected virtual void SendChangeCipherSpecMessage()
        {
            byte[] message = new byte[]{ 1 };
            SafeWriteRecord(ContentType.change_cipher_spec, message, 0, message.Length);
            mRecordStream.SentWriteCipherSpec();
        }

        protected virtual void SendFinishedMessage()
        {
            byte[] verify_data = CreateVerifyData(Context.IsServer);

            HandshakeMessage message = new HandshakeMessage(HandshakeType.finished, verify_data.Length);

            message.Write(verify_data, 0, verify_data.Length);

            message.WriteToRecordStream(this);
        }

        protected virtual void SendSupplementalDataMessage(IList supplementalData)
        {
            HandshakeMessage message = new HandshakeMessage(HandshakeType.supplemental_data);

            WriteSupplementalData(message, supplementalData);

            message.WriteToRecordStream(this);
        }

        protected virtual byte[] CreateVerifyData(bool isServer)
        {
            TlsContext context = Context;
            string asciiLabel = isServer ? ExporterLabel.server_finished : ExporterLabel.client_finished;
            byte[] sslSender = isServer ? TlsUtilities.SSL_SERVER : TlsUtilities.SSL_CLIENT;
            byte[] hash = GetCurrentPrfHash(context, mRecordStream.HandshakeHash, sslSender);
            return TlsUtilities.CalculateVerifyData(context, asciiLabel, hash);
        }

        /**
         * Closes this connection.
         *
         * @throws IOException If something goes wrong during closing.
         */
        public virtual void Close()
        {
            HandleClose(true);
        }

        protected internal virtual void Flush()
        {
            mRecordStream.Flush();
        }

        public virtual bool IsClosed
        {
            get { return mClosed; }
        }

        protected virtual short ProcessMaxFragmentLengthExtension(IDictionary clientExtensions, IDictionary serverExtensions,
            byte alertDescription)
        {
            short maxFragmentLength = TlsExtensionsUtilities.GetMaxFragmentLengthExtension(serverExtensions);
            if (maxFragmentLength >= 0)
            {
                if (!MaxFragmentLength.IsValid((byte)maxFragmentLength)
                    || (!this.mResumedSession && maxFragmentLength != TlsExtensionsUtilities
                        .GetMaxFragmentLengthExtension(clientExtensions)))
                {
                    throw new TlsFatalAlert(alertDescription);
                }
            }
            return maxFragmentLength;
        }

        protected virtual void RefuseRenegotiation()
        {
            /*
             * RFC 5746 4.5 SSLv3 clients that refuse renegotiation SHOULD use a fatal
             * handshake_failure alert.
             */
            if (TlsUtilities.IsSsl(Context))
                throw new TlsFatalAlert(AlertDescription.handshake_failure);

            RaiseAlertWarning(AlertDescription.no_renegotiation, "Renegotiation not supported");
        }

        /**
         * Make sure the InputStream 'buf' now empty. Fail otherwise.
         *
         * @param buf The InputStream to check.
         * @throws IOException If 'buf' is not empty.
         */
        protected internal static void AssertEmpty(MemoryStream buf)
        {
            if (buf.Position < buf.Length)
                throw new TlsFatalAlert(AlertDescription.decode_error);
        }

        protected internal static byte[] CreateRandomBlock(bool useGmtUnixTime, IRandomGenerator randomGenerator)
        {
            byte[] result = new byte[32];
            randomGenerator.NextBytes(result);

            if (useGmtUnixTime)
            {
                TlsUtilities.WriteGmtUnixTime(result, 0);
            }

            return result;
        }

        protected internal static byte[] CreateRenegotiationInfo(byte[] renegotiated_connection)
        {
            return TlsUtilities.EncodeOpaque8(renegotiated_connection);
        }

        protected internal static void EstablishMasterSecret(TlsContext context, TlsKeyExchange keyExchange)
        {
            byte[] pre_master_secret = keyExchange.GeneratePremasterSecret();

            try
            {
                context.SecurityParameters.masterSecret = TlsUtilities.CalculateMasterSecret(context, pre_master_secret);
            }
            finally
            {
                // TODO Is there a way to ensure the data is really overwritten?
                /*
                 * RFC 2246 8.1. The pre_master_secret should be deleted from memory once the
                 * master_secret has been computed.
                 */
                if (pre_master_secret != null)
                {
                    Arrays.Fill(pre_master_secret, (byte)0);
                }
            }
        }

        /**
         * 'sender' only relevant to SSLv3
         */
        protected internal static byte[] GetCurrentPrfHash(TlsContext context, TlsHandshakeHash handshakeHash, byte[] sslSender)
        {
            IDigest d = handshakeHash.ForkPrfHash();

            if (sslSender != null && TlsUtilities.IsSsl(context))
            {
                d.BlockUpdate(sslSender, 0, sslSender.Length);
            }

            return DigestUtilities.DoFinal(d);
        }

        protected internal static IDictionary ReadExtensions(MemoryStream input)
        {
            if (input.Position >= input.Length)
                return null;

            byte[] extBytes = TlsUtilities.ReadOpaque16(input);

            AssertEmpty(input);

            MemoryStream buf = new MemoryStream(extBytes, false);

            // Integer -> byte[]
            IDictionary extensions = Platform.CreateHashtable();

            while (buf.Position < buf.Length)
            {
                int extension_type = TlsUtilities.ReadUint16(buf);
                byte[] extension_data = TlsUtilities.ReadOpaque16(buf);

                /*
                 * RFC 3546 2.3 There MUST NOT be more than one extension of the same type.
                 */
                if (extensions.Contains(extension_type))
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);

                extensions.Add(extension_type, extension_data);
            }

            return extensions;
        }

        protected internal static IList ReadSupplementalDataMessage(MemoryStream input)
        {
            byte[] supp_data = TlsUtilities.ReadOpaque24(input);

            AssertEmpty(input);

            MemoryStream buf = new MemoryStream(supp_data, false);

            IList supplementalData = Platform.CreateArrayList();

            while (buf.Position < buf.Length)
            {
                int supp_data_type = TlsUtilities.ReadUint16(buf);
                byte[] data = TlsUtilities.ReadOpaque16(buf);

                supplementalData.Add(new SupplementalDataEntry(supp_data_type, data));
            }

            return supplementalData;
        }

        protected internal static void WriteExtensions(Stream output, IDictionary extensions)
        {
            MemoryStream buf = new MemoryStream();

            /*
             * NOTE: There are reports of servers that don't accept a zero-length extension as the last
             * one, so we write out any zero-length ones first as a best-effort workaround.
             */
            WriteSelectedExtensions(buf, extensions, true);
            WriteSelectedExtensions(buf, extensions, false);

            byte[] extBytes = buf.ToArray();

            TlsUtilities.WriteOpaque16(extBytes, output);
        }

        protected internal static void WriteSelectedExtensions(Stream output, IDictionary extensions, bool selectEmpty)
        {
            foreach (int extension_type in extensions.Keys)
            {
                byte[] extension_data = (byte[])extensions[extension_type];
                if (selectEmpty == (extension_data.Length == 0))
                {
                    TlsUtilities.CheckUint16(extension_type);
                    TlsUtilities.WriteUint16(extension_type, output);
                    TlsUtilities.WriteOpaque16(extension_data, output);
                }
            }
        }

        protected internal static void WriteSupplementalData(Stream output, IList supplementalData)
        {
            MemoryStream buf = new MemoryStream();

            foreach (SupplementalDataEntry entry in supplementalData)
            {
                int supp_data_type = entry.DataType;
                TlsUtilities.CheckUint16(supp_data_type);
                TlsUtilities.WriteUint16(supp_data_type, buf);
                TlsUtilities.WriteOpaque16(entry.Data, buf);
            }

            byte[] supp_data = buf.ToArray();

            TlsUtilities.WriteOpaque24(supp_data, output);
        }

        protected internal static int GetPrfAlgorithm(TlsContext context, int ciphersuite)
        {
            bool isTLSv12 = TlsUtilities.IsTlsV12(context);

            switch (ciphersuite)
            {
            case CipherSuite.TLS_DH_anon_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_128_OCB:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM:
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_256_OCB:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_128_OCB:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_256_OCB:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_128_OCB:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_256_OCB:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_128_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_256_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_128_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_256_OCB:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_PSK_DHE_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_PSK_DHE_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_PSK_WITH_AES_128_CCM:
            case CipherSuite.TLS_PSK_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_PSK_WITH_AES_128_OCB:
            case CipherSuite.TLS_PSK_WITH_AES_256_CCM:
            case CipherSuite.TLS_PSK_WITH_AES_256_CCM_8:
            case CipherSuite.DRAFT_TLS_PSK_WITH_AES_256_OCB:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_RSA_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_RSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_RSA_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_NULL_SHA256:
            {
                if (isTLSv12)
                {
                    return PrfAlgorithm.tls_prf_sha256;
                }
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }

            case CipherSuite.TLS_DH_anon_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DH_DSS_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DH_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            {
                if (isTLSv12)
                {
                    return PrfAlgorithm.tls_prf_sha384;
                }
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }

            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA384:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA384:
            case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_PSK_WITH_NULL_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA384:
            {
                if (isTLSv12)
                {
                    return PrfAlgorithm.tls_prf_sha384;
                }
                return PrfAlgorithm.tls_prf_legacy;
            }

            default:
            {
                if (isTLSv12)
                {
                    return PrfAlgorithm.tls_prf_sha256;
                }
                return PrfAlgorithm.tls_prf_legacy;
            }
            }
        }

        internal class HandshakeMessage
            :   MemoryStream
        {
            internal HandshakeMessage(byte handshakeType)
                :   this(handshakeType, 60)
            {
            }

            internal HandshakeMessage(byte handshakeType, int length)
                :   base(length + 4)
            {
                TlsUtilities.WriteUint8(handshakeType, this);
                // Reserve space for length
                TlsUtilities.WriteUint24(0, this);
            }

            internal void Write(byte[] data)
            {
                Write(data, 0, data.Length);
            }

            internal void WriteToRecordStream(TlsProtocol protocol)
            {
                // Patch actual length back in
                long length = Length - 4;
                TlsUtilities.CheckUint24(length);
                this.Position = 1;
                TlsUtilities.WriteUint24((int)length, this);

#if PORTABLE
                byte[] buf = ToArray();
                int bufLen = buf.Length;
#else
                byte[] buf = GetBuffer();
                int bufLen = (int)Length;
#endif

                protocol.WriteHandshakeMessage(buf, 0, bufLen);
                Platform.Dispose(this);
            }
        }
    }
}
