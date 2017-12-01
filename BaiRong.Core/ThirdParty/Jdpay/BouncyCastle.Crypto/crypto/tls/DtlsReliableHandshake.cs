using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    internal class DtlsReliableHandshake
    {
        private const int MaxReceiveAhead = 16;
        private const int MessageHeaderLength = 12;

        private readonly DtlsRecordLayer mRecordLayer;

        private TlsHandshakeHash mHandshakeHash;

        private IDictionary mCurrentInboundFlight = Platform.CreateHashtable();
        private IDictionary mPreviousInboundFlight = null;
        private IList mOutboundFlight = Platform.CreateArrayList();
        private bool mSending = true;

        private int mMessageSeq = 0, mNextReceiveSeq = 0;

        internal DtlsReliableHandshake(TlsContext context, DtlsRecordLayer transport)
        {
            this.mRecordLayer = transport;
            this.mHandshakeHash = new DeferredHash();
            this.mHandshakeHash.Init(context);
        }

        internal void NotifyHelloComplete()
        {
            this.mHandshakeHash = mHandshakeHash.NotifyPrfDetermined();
        }

        internal TlsHandshakeHash HandshakeHash
        {
            get { return mHandshakeHash; }
        }

        internal TlsHandshakeHash PrepareToFinish()
        {
            TlsHandshakeHash result = mHandshakeHash;
            this.mHandshakeHash = mHandshakeHash.StopTracking();
            return result;
        }

        internal void SendMessage(byte msg_type, byte[] body)
        {
            TlsUtilities.CheckUint24(body.Length);

            if (!mSending)
            {
                CheckInboundFlight();
                mSending = true;
                mOutboundFlight.Clear();
            }

            Message message = new Message(mMessageSeq++, msg_type, body);

            mOutboundFlight.Add(message);

            WriteMessage(message);
            UpdateHandshakeMessagesDigest(message);
        }

        internal byte[] ReceiveMessageBody(byte msg_type)
        {
            Message message = ReceiveMessage();
            if (message.Type != msg_type)
                throw new TlsFatalAlert(AlertDescription.unexpected_message);

            return message.Body;
        }

        internal Message ReceiveMessage()
        {
            if (mSending)
            {
                mSending = false;
                PrepareInboundFlight(Platform.CreateHashtable());
            }

            byte[] buf = null;

            // TODO Check the conditions under which we should reset this
            int readTimeoutMillis = 1000;

            for (;;)
            {
                try
                {
                    for (;;)
                    {
                        Message pending = GetPendingMessage();
                        if (pending != null)
                            return pending;

                        int receiveLimit = mRecordLayer.GetReceiveLimit();
                        if (buf == null || buf.Length < receiveLimit)
                        {
                            buf = new byte[receiveLimit];
                        }

                        int received = mRecordLayer.Receive(buf, 0, receiveLimit, readTimeoutMillis);
                        if (received < 0)
                            break;

                        bool resentOutbound = ProcessRecord(MaxReceiveAhead, mRecordLayer.ReadEpoch, buf, 0, received);
                        if (resentOutbound)
                        {
                            readTimeoutMillis = BackOff(readTimeoutMillis);
                        }
                    }
                }
                catch (IOException e)
                {
                    // NOTE: Assume this is a timeout for the moment
                }

                ResendOutboundFlight();
                readTimeoutMillis = BackOff(readTimeoutMillis);
            }
        }

        internal void Finish()
        {
            DtlsHandshakeRetransmit retransmit = null;
            if (!mSending)
            {
                CheckInboundFlight();
            }
            else
            {
                PrepareInboundFlight(null);

                if (mPreviousInboundFlight != null)
                {
                    /*
                     * RFC 6347 4.2.4. In addition, for at least twice the default MSL defined for [TCP],
                     * when in the FINISHED state, the node that transmits the last flight (the server in an
                     * ordinary handshake or the client in a resumed handshake) MUST respond to a retransmit
                     * of the peer's last flight with a retransmit of the last flight.
                     */
                    retransmit = new Retransmit(this);
                }
            }

            mRecordLayer.HandshakeSuccessful(retransmit);
        }

        internal void ResetHandshakeMessagesDigest()
        {
            mHandshakeHash.Reset();
        }

        private int BackOff(int timeoutMillis)
        {
            /*
             * TODO[DTLS] implementations SHOULD back off handshake packet size during the
             * retransmit backoff.
             */
            return System.Math.Min(timeoutMillis * 2, 60000);
        }

        /**
         * Check that there are no "extra" messages left in the current inbound flight
         */
        private void CheckInboundFlight()
        {
            foreach (int key in mCurrentInboundFlight.Keys)
            {
                if (key >= mNextReceiveSeq)
                {
                    // TODO Should this be considered an error?
                }
            }
        }

        private Message GetPendingMessage()
        {
            DtlsReassembler next = (DtlsReassembler)mCurrentInboundFlight[mNextReceiveSeq];
            if (next != null)
            {
                byte[] body = next.GetBodyIfComplete();
                if (body != null)
                {
                    mPreviousInboundFlight = null;
                    return UpdateHandshakeMessagesDigest(new Message(mNextReceiveSeq++, next.MsgType, body));
                }
            }
            return null;
        }

        private void PrepareInboundFlight(IDictionary nextFlight)
        {
            ResetAll(mCurrentInboundFlight);
            mPreviousInboundFlight = mCurrentInboundFlight;
            mCurrentInboundFlight = nextFlight;
        }

        private bool ProcessRecord(int windowSize, int epoch, byte[] buf, int off, int len)
        {
            bool checkPreviousFlight = false;

            while (len >= MessageHeaderLength)
            {
                int fragment_length = TlsUtilities.ReadUint24(buf, off + 9);
                int message_length = fragment_length + MessageHeaderLength;
                if (len < message_length)
                {
                    // NOTE: Truncated message - ignore it
                    break;
                }

                int length = TlsUtilities.ReadUint24(buf, off + 1);
                int fragment_offset = TlsUtilities.ReadUint24(buf, off + 6);
                if (fragment_offset + fragment_length > length)
                {
                    // NOTE: Malformed fragment - ignore it and the rest of the record
                    break;
                }

                /*
                 * NOTE: This very simple epoch check will only work until we want to support
                 * renegotiation (and we're not likely to do that anyway).
                 */
                byte msg_type = TlsUtilities.ReadUint8(buf, off + 0);
                int expectedEpoch = msg_type == HandshakeType.finished ? 1 : 0;
                if (epoch != expectedEpoch)
                {
                    break;
                }

                int message_seq = TlsUtilities.ReadUint16(buf, off + 4);
                if (message_seq >= (mNextReceiveSeq + windowSize))
                {
                    // NOTE: Too far ahead - ignore
                }
                else if (message_seq >= mNextReceiveSeq)
                {
                    DtlsReassembler reassembler = (DtlsReassembler)mCurrentInboundFlight[message_seq];
                    if (reassembler == null)
                    {
                        reassembler = new DtlsReassembler(msg_type, length);
                        mCurrentInboundFlight[message_seq] = reassembler;
                    }

                    reassembler.ContributeFragment(msg_type, length, buf, off + MessageHeaderLength, fragment_offset,
                        fragment_length);
                }
                else if (mPreviousInboundFlight != null)
                {
                    /*
                     * NOTE: If we receive the previous flight of incoming messages in full again,
                     * retransmit our last flight
                     */

                    DtlsReassembler reassembler = (DtlsReassembler)mPreviousInboundFlight[message_seq];
                    if (reassembler != null)
                    {
                        reassembler.ContributeFragment(msg_type, length, buf, off + MessageHeaderLength, fragment_offset,
                            fragment_length);
                        checkPreviousFlight = true;
                    }
                }

                off += message_length;
                len -= message_length;
            }

            bool result = checkPreviousFlight && CheckAll(mPreviousInboundFlight);
            if (result)
            {
                ResendOutboundFlight();
                ResetAll(mPreviousInboundFlight);
            }
            return result;
        }

        private void ResendOutboundFlight()
        {
            mRecordLayer.ResetWriteEpoch();
            for (int i = 0; i < mOutboundFlight.Count; ++i)
            {
                WriteMessage((Message)mOutboundFlight[i]);
            }
        }

        private Message UpdateHandshakeMessagesDigest(Message message)
        {
            if (message.Type != HandshakeType.hello_request)
            {
                byte[] body = message.Body;
                byte[] buf = new byte[MessageHeaderLength];
                TlsUtilities.WriteUint8(message.Type, buf, 0);
                TlsUtilities.WriteUint24(body.Length, buf, 1);
                TlsUtilities.WriteUint16(message.Seq, buf, 4);
                TlsUtilities.WriteUint24(0, buf, 6);
                TlsUtilities.WriteUint24(body.Length, buf, 9);
                mHandshakeHash.BlockUpdate(buf, 0, buf.Length);
                mHandshakeHash.BlockUpdate(body, 0, body.Length);
            }
            return message;
        }

        private void WriteMessage(Message message)
        {
            int sendLimit = mRecordLayer.GetSendLimit();
            int fragmentLimit = sendLimit - MessageHeaderLength;

            // TODO Support a higher minimum fragment size?
            if (fragmentLimit < 1)
            {
                // TODO Should we be throwing an exception here?
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }

            int length = message.Body.Length;

            // NOTE: Must still send a fragment if body is empty
            int fragment_offset = 0;
            do
            {
                int fragment_length = System.Math.Min(length - fragment_offset, fragmentLimit);
                WriteHandshakeFragment(message, fragment_offset, fragment_length);
                fragment_offset += fragment_length;
            }
            while (fragment_offset < length);
        }

        private void WriteHandshakeFragment(Message message, int fragment_offset, int fragment_length)
        {
            RecordLayerBuffer fragment = new RecordLayerBuffer(MessageHeaderLength + fragment_length);
            TlsUtilities.WriteUint8(message.Type, fragment);
            TlsUtilities.WriteUint24(message.Body.Length, fragment);
            TlsUtilities.WriteUint16(message.Seq, fragment);
            TlsUtilities.WriteUint24(fragment_offset, fragment);
            TlsUtilities.WriteUint24(fragment_length, fragment);
            fragment.Write(message.Body, fragment_offset, fragment_length);

            fragment.SendToRecordLayer(mRecordLayer);
        }

        private static bool CheckAll(IDictionary inboundFlight)
        {
            foreach (DtlsReassembler r in inboundFlight.Values)
            {
                if (r.GetBodyIfComplete() == null)
                {
                    return false;
                }
            }
            return true;
        }

        private static void ResetAll(IDictionary inboundFlight)
        {
            foreach (DtlsReassembler r in inboundFlight.Values)
            {
                r.Reset();
            }
        }

        internal class Message
        {
            private readonly int mMessageSeq;
            private readonly byte mMsgType;
            private readonly byte[] mBody;

            internal Message(int message_seq, byte msg_type, byte[] body)
            {
                this.mMessageSeq = message_seq;
                this.mMsgType = msg_type;
                this.mBody = body;
            }

            public int Seq
            {
                get { return mMessageSeq; }
            }

            public byte Type
            {
                get { return mMsgType; }
            }

            public byte[] Body
            {
                get { return mBody; }
            }
        }

        internal class RecordLayerBuffer
            :   MemoryStream
        {
            internal RecordLayerBuffer(int size)
                :   base(size)
            {
            }

            internal void SendToRecordLayer(DtlsRecordLayer recordLayer)
            {
#if PORTABLE
                byte[] buf = ToArray();
                int bufLen = buf.Length;
#else
                byte[] buf = GetBuffer();
                int bufLen = (int)Length;
#endif

                recordLayer.Send(buf, 0, bufLen);
                Platform.Dispose(this);
            }
        }

        internal class Retransmit
            :   DtlsHandshakeRetransmit
        {
            private readonly DtlsReliableHandshake mOuter;

            internal Retransmit(DtlsReliableHandshake outer)
            {
                this.mOuter = outer;
            }

            public void ReceivedHandshakeRecord(int epoch, byte[] buf, int off, int len)
            {
                mOuter.ProcessRecord(0, epoch, buf, off, len);
            }
        }
    }
}
