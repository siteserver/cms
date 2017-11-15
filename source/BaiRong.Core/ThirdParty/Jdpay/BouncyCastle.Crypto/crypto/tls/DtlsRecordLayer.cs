using System;
using System.IO;

using Org.BouncyCastle.Utilities.Date;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal class DtlsRecordLayer
        :   DatagramTransport
    {
        private const int RECORD_HEADER_LENGTH = 13;
        private const int MAX_FRAGMENT_LENGTH = 1 << 14;
        private const long TCP_MSL = 1000L * 60 * 2;
        private const long RETRANSMIT_TIMEOUT = TCP_MSL * 2;

        private readonly DatagramTransport mTransport;
        private readonly TlsContext mContext;
        private readonly TlsPeer mPeer;

        private readonly ByteQueue mRecordQueue = new ByteQueue();

        private volatile bool mClosed = false;
        private volatile bool mFailed = false;
        private volatile ProtocolVersion mReadVersion = null, mWriteVersion = null;
        private volatile bool mInHandshake;
        private volatile int mPlaintextLimit;
        private DtlsEpoch mCurrentEpoch, mPendingEpoch;
        private DtlsEpoch mReadEpoch, mWriteEpoch;

        private DtlsHandshakeRetransmit mRetransmit = null;
        private DtlsEpoch mRetransmitEpoch = null;
        private long mRetransmitExpiry = 0;

        internal DtlsRecordLayer(DatagramTransport transport, TlsContext context, TlsPeer peer, byte contentType)
        {
            this.mTransport = transport;
            this.mContext = context;
            this.mPeer = peer;

            this.mInHandshake = true;

            this.mCurrentEpoch = new DtlsEpoch(0, new TlsNullCipher(context));
            this.mPendingEpoch = null;
            this.mReadEpoch = mCurrentEpoch;
            this.mWriteEpoch = mCurrentEpoch;

            SetPlaintextLimit(MAX_FRAGMENT_LENGTH);
        }

        internal virtual void SetPlaintextLimit(int plaintextLimit)
        {
            this.mPlaintextLimit = plaintextLimit;
        }

        internal virtual int ReadEpoch
        {
            get { return mReadEpoch.Epoch; }
        }

        internal virtual ProtocolVersion ReadVersion
        {
            get { return mReadVersion; }
            set { this.mReadVersion = value; }
        }

        internal virtual void SetWriteVersion(ProtocolVersion writeVersion)
        {
            this.mWriteVersion = writeVersion;
        }

        internal virtual void InitPendingEpoch(TlsCipher pendingCipher)
        {
            if (mPendingEpoch != null)
                throw new InvalidOperationException();

            /*
             * TODO "In order to ensure that any given sequence/epoch pair is unique, implementations
             * MUST NOT allow the same epoch value to be reused within two times the TCP maximum segment
             * lifetime."
             */

            // TODO Check for overflow
            this.mPendingEpoch = new DtlsEpoch(mWriteEpoch.Epoch + 1, pendingCipher);
        }

        internal virtual void HandshakeSuccessful(DtlsHandshakeRetransmit retransmit)
        {
            if (mReadEpoch == mCurrentEpoch || mWriteEpoch == mCurrentEpoch)
            {
                // TODO
                throw new InvalidOperationException();
            }

            if (retransmit != null)
            {
                this.mRetransmit = retransmit;
                this.mRetransmitEpoch = mCurrentEpoch;
                this.mRetransmitExpiry = DateTimeUtilities.CurrentUnixMs() + RETRANSMIT_TIMEOUT;
            }

            this.mInHandshake = false;
            this.mCurrentEpoch = mPendingEpoch;
            this.mPendingEpoch = null;
        }

        internal virtual void ResetWriteEpoch()
        {
            if (mRetransmitEpoch != null)
            {
                this.mWriteEpoch = mRetransmitEpoch;
            }
            else
            {
                this.mWriteEpoch = mCurrentEpoch;
            }
        }

        public virtual int GetReceiveLimit()
        {
            return System.Math.Min(this.mPlaintextLimit,
                mReadEpoch.Cipher.GetPlaintextLimit(mTransport.GetReceiveLimit() - RECORD_HEADER_LENGTH));
        }

        public virtual int GetSendLimit()
        {
            return System.Math.Min(this.mPlaintextLimit,
                mWriteEpoch.Cipher.GetPlaintextLimit(mTransport.GetSendLimit() - RECORD_HEADER_LENGTH));
        }

        public virtual int Receive(byte[] buf, int off, int len, int waitMillis)
        {
            byte[] record = null;

            for (;;)
            {
                int receiveLimit = System.Math.Min(len, GetReceiveLimit()) + RECORD_HEADER_LENGTH;
                if (record == null || record.Length < receiveLimit)
                {
                    record = new byte[receiveLimit];
                }

                try
                {
                    if (mRetransmit != null && DateTimeUtilities.CurrentUnixMs() > mRetransmitExpiry)
                    {
                        mRetransmit = null;
                        mRetransmitEpoch = null;
                    }

                    int received = ReceiveRecord(record, 0, receiveLimit, waitMillis);
                    if (received < 0)
                    {
                        return received;
                    }
                    if (received < RECORD_HEADER_LENGTH)
                    {
                        continue;
                    }
                    int length = TlsUtilities.ReadUint16(record, 11);
                    if (received != (length + RECORD_HEADER_LENGTH))
                    {
                        continue;
                    }

                    byte type = TlsUtilities.ReadUint8(record, 0);

                    // TODO Support user-specified custom protocols?
                    switch (type)
                    {
                    case ContentType.alert:
                    case ContentType.application_data:
                    case ContentType.change_cipher_spec:
                    case ContentType.handshake:
                    case ContentType.heartbeat:
                        break;
                    default:
                        // TODO Exception?
                        continue;
                    }

                    int epoch = TlsUtilities.ReadUint16(record, 3);

                    DtlsEpoch recordEpoch = null;
                    if (epoch == mReadEpoch.Epoch)
                    {
                        recordEpoch = mReadEpoch;
                    }
                    else if (type == ContentType.handshake && mRetransmitEpoch != null
                        && epoch == mRetransmitEpoch.Epoch)
                    {
                        recordEpoch = mRetransmitEpoch;
                    }

                    if (recordEpoch == null)
                    {
                        continue;
                    }

                    long seq = TlsUtilities.ReadUint48(record, 5);
                    if (recordEpoch.ReplayWindow.ShouldDiscard(seq))
                    {
                        continue;
                    }

                    ProtocolVersion version = TlsUtilities.ReadVersion(record, 1);
                    if (!version.IsDtls)
                    {
                        continue;
                    }

                    if (mReadVersion != null && !mReadVersion.Equals(version))
                    {
                        continue;
                    }

                    byte[] plaintext = recordEpoch.Cipher.DecodeCiphertext(
                        GetMacSequenceNumber(recordEpoch.Epoch, seq), type, record, RECORD_HEADER_LENGTH,
                        received - RECORD_HEADER_LENGTH);

                    recordEpoch.ReplayWindow.ReportAuthenticated(seq);

                    if (plaintext.Length > this.mPlaintextLimit)
                    {
                        continue;
                    }

                    if (mReadVersion == null)
                    {
                        mReadVersion = version;
                    }

                    switch (type)
                    {
                    case ContentType.alert:
                    {
                        if (plaintext.Length == 2)
                        {
                            byte alertLevel = plaintext[0];
                            byte alertDescription = plaintext[1];

                            mPeer.NotifyAlertReceived(alertLevel, alertDescription);

                            if (alertLevel == AlertLevel.fatal)
                            {
                                Failed();
                                throw new TlsFatalAlert(alertDescription);
                            }

                            // TODO Can close_notify be a fatal alert?
                            if (alertDescription == AlertDescription.close_notify)
                            {
                                CloseTransport();
                            }
                        }

                        continue;
                    }
                    case ContentType.application_data:
                    {
                        if (mInHandshake)
                        {
                            // TODO Consider buffering application data for new epoch that arrives
                            // out-of-order with the Finished message
                            continue;
                        }
                        break;
                    }
                    case ContentType.change_cipher_spec:
                    {
                        // Implicitly receive change_cipher_spec and change to pending cipher state

                        for (int i = 0; i < plaintext.Length; ++i)
                        {
                            byte message = TlsUtilities.ReadUint8(plaintext, i);
                            if (message != ChangeCipherSpec.change_cipher_spec)
                            {
                                continue;
                            }

                            if (mPendingEpoch != null)
                            {
                                mReadEpoch = mPendingEpoch;
                            }
                        }

                        continue;
                    }
                    case ContentType.handshake:
                    {
                        if (!mInHandshake)
                        {
                            if (mRetransmit != null)
                            {
                                mRetransmit.ReceivedHandshakeRecord(epoch, plaintext, 0, plaintext.Length);
                            }

                            // TODO Consider support for HelloRequest
                            continue;
                        }
                        break;
                    }
                    case ContentType.heartbeat:
                    {
                        // TODO[RFC 6520]
                        continue;
                    }
                    }

                    /*
                     * NOTE: If we receive any non-handshake data in the new epoch implies the peer has
                     * received our final flight.
                     */
                    if (!mInHandshake && mRetransmit != null)
                    {
                        this.mRetransmit = null;
                        this.mRetransmitEpoch = null;
                    }

                    Array.Copy(plaintext, 0, buf, off, plaintext.Length);
                    return plaintext.Length;
                }
                catch (IOException e)
                {
                    // NOTE: Assume this is a timeout for the moment
                    throw e;
                }
            }
        }

        /// <exception cref="IOException"/>
        public virtual void Send(byte[] buf, int off, int len)
        {
            byte contentType = ContentType.application_data;

            if (this.mInHandshake || this.mWriteEpoch == this.mRetransmitEpoch)
            {
                contentType = ContentType.handshake;

                byte handshakeType = TlsUtilities.ReadUint8(buf, off);
                if (handshakeType == HandshakeType.finished)
                {
                    DtlsEpoch nextEpoch = null;
                    if (this.mInHandshake)
                    {
                        nextEpoch = mPendingEpoch;
                    }
                    else if (this.mWriteEpoch == this.mRetransmitEpoch)
                    {
                        nextEpoch = mCurrentEpoch;
                    }

                    if (nextEpoch == null)
                    {
                        // TODO
                        throw new InvalidOperationException();
                    }

                    // Implicitly send change_cipher_spec and change to pending cipher state

                    // TODO Send change_cipher_spec and finished records in single datagram?
                    byte[] data = new byte[]{ 1 };
                    SendRecord(ContentType.change_cipher_spec, data, 0, data.Length);

                    mWriteEpoch = nextEpoch;
                }
            }

            SendRecord(contentType, buf, off, len);
        }

        public virtual void Close()
        {
            if (!mClosed)
            {
                if (mInHandshake)
                {
                    Warn(AlertDescription.user_canceled, "User canceled handshake");
                }
                CloseTransport();
            }
        }

        internal virtual void Failed()
        {
            if (!mClosed)
            {
                mFailed = true;

                CloseTransport();
            }
        }

        internal virtual void Fail(byte alertDescription)
        {
            if (!mClosed)
            {
                try
                {
                    RaiseAlert(AlertLevel.fatal, alertDescription, null, null);
                }
                catch (Exception)
                {
                    // Ignore
                }

                mFailed = true;

                CloseTransport();
            }
        }

        internal virtual void Warn(byte alertDescription, string message)
        {
            RaiseAlert(AlertLevel.warning, alertDescription, message, null);
        }

        private void CloseTransport()
        {
            if (!mClosed)
            {
                /*
                 * RFC 5246 7.2.1. Unless some other fatal alert has been transmitted, each party is
                 * required to send a close_notify alert before closing the write side of the
                 * connection. The other party MUST respond with a close_notify alert of its own and
                 * close down the connection immediately, discarding any pending writes.
                 */

                try
                {
                    if (!mFailed)
                    {
                        Warn(AlertDescription.close_notify, null);
                    }
                    mTransport.Close();
                }
                catch (Exception)
                {
                    // Ignore
                }

                mClosed = true;
            }
        }

        private void RaiseAlert(byte alertLevel, byte alertDescription, string message, Exception cause)
        {
            mPeer.NotifyAlertRaised(alertLevel, alertDescription, message, cause);

            byte[] error = new byte[2];
            error[0] = (byte)alertLevel;
            error[1] = (byte)alertDescription;

            SendRecord(ContentType.alert, error, 0, 2);
        }

        private int ReceiveRecord(byte[] buf, int off, int len, int waitMillis)
        {
            if (mRecordQueue.Available > 0)
            {
                int length = 0;
                if (mRecordQueue.Available >= RECORD_HEADER_LENGTH)
                {
                    byte[] lengthBytes = new byte[2];
                    mRecordQueue.Read(lengthBytes, 0, 2, 11);
                    length = TlsUtilities.ReadUint16(lengthBytes, 0);
                }

                int received = System.Math.Min(mRecordQueue.Available, RECORD_HEADER_LENGTH + length);
                mRecordQueue.RemoveData(buf, off, received, 0);
                return received;
            }

            {
                int received = mTransport.Receive(buf, off, len, waitMillis);
                if (received >= RECORD_HEADER_LENGTH)
                {
                    int fragmentLength = TlsUtilities.ReadUint16(buf, off + 11);
                    int recordLength = RECORD_HEADER_LENGTH + fragmentLength;
                    if (received > recordLength)
                    {
                        mRecordQueue.AddData(buf, off + recordLength, received - recordLength);
                        received = recordLength;
                    }
                }
                return received;
            }
        }

        private void SendRecord(byte contentType, byte[] buf, int off, int len)
        {
            // Never send anything until a valid ClientHello has been received
            if (mWriteVersion == null)
                return;

            if (len > this.mPlaintextLimit)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            /*
             * RFC 5246 6.2.1 Implementations MUST NOT send zero-length fragments of Handshake, Alert,
             * or ChangeCipherSpec content types.
             */
            if (len < 1 && contentType != ContentType.application_data)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            int recordEpoch = mWriteEpoch.Epoch;
            long recordSequenceNumber = mWriteEpoch.AllocateSequenceNumber();

            byte[] ciphertext = mWriteEpoch.Cipher.EncodePlaintext(
                GetMacSequenceNumber(recordEpoch, recordSequenceNumber), contentType, buf, off, len);

            // TODO Check the ciphertext length?

            byte[] record = new byte[ciphertext.Length + RECORD_HEADER_LENGTH];
            TlsUtilities.WriteUint8(contentType, record, 0);
            ProtocolVersion version = mWriteVersion;
            TlsUtilities.WriteVersion(version, record, 1);
            TlsUtilities.WriteUint16(recordEpoch, record, 3);
            TlsUtilities.WriteUint48(recordSequenceNumber, record, 5);
            TlsUtilities.WriteUint16(ciphertext.Length, record, 11);
            Array.Copy(ciphertext, 0, record, RECORD_HEADER_LENGTH, ciphertext.Length);

            mTransport.Send(record, 0, record.Length);
        }

        private static long GetMacSequenceNumber(int epoch, long sequence_number)
        {
            return ((epoch & 0xFFFFFFFFL) << 48) | sequence_number;
        }
    }
}
