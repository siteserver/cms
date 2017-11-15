using System;
using System.IO;

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class HeartbeatMessage
    {
        protected readonly byte mType;
        protected readonly byte[] mPayload;
        protected readonly int mPaddingLength;

        public HeartbeatMessage(byte type, byte[] payload, int paddingLength)
        {
            if (!HeartbeatMessageType.IsValid(type))
                throw new ArgumentException("not a valid HeartbeatMessageType value", "type");
            if (payload == null || payload.Length >= (1 << 16))
                throw new ArgumentException("must have length < 2^16", "payload");
            if (paddingLength < 16)
                throw new ArgumentException("must be at least 16", "paddingLength");

            this.mType = type;
            this.mPayload = payload;
            this.mPaddingLength = paddingLength;
        }

        /**
         * Encode this {@link HeartbeatMessage} to a {@link Stream}.
         * 
         * @param output
         *            the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(TlsContext context, Stream output)
        {
            TlsUtilities.WriteUint8(mType, output);

            TlsUtilities.CheckUint16(mPayload.Length);
            TlsUtilities.WriteUint16(mPayload.Length, output);
            output.Write(mPayload, 0, mPayload.Length);

            byte[] padding = new byte[mPaddingLength];
            context.NonceRandomGenerator.NextBytes(padding);
            output.Write(padding, 0, padding.Length);
        }

        /**
         * Parse a {@link HeartbeatMessage} from a {@link Stream}.
         * 
         * @param input
         *            the {@link Stream} to parse from.
         * @return a {@link HeartbeatMessage} object.
         * @throws IOException
         */
        public static HeartbeatMessage Parse(Stream input)
        {
            byte type = TlsUtilities.ReadUint8(input);
            if (!HeartbeatMessageType.IsValid(type))
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            int payload_length = TlsUtilities.ReadUint16(input);

            PayloadBuffer buf = new PayloadBuffer();
            Streams.PipeAll(input, buf);

            byte[] payload = buf.ToTruncatedByteArray(payload_length);
            if (payload == null)
            {
                /*
                 * RFC 6520 4. If the payload_length of a received HeartbeatMessage is too large, the
                 * received HeartbeatMessage MUST be discarded silently.
                 */
                return null;
            }

            TlsUtilities.CheckUint16(buf.Length);
            int padding_length = (int)buf.Length - payload.Length;

            /*
             * RFC 6520 4. The padding of a received HeartbeatMessage message MUST be ignored
             */
            return new HeartbeatMessage(type, payload, padding_length);
        }

        internal class PayloadBuffer
            :   MemoryStream
        {
            internal byte[] ToTruncatedByteArray(int payloadLength)
            {
                /*
                 * RFC 6520 4. The padding_length MUST be at least 16.
                 */
                int minimumCount = payloadLength + 16;
                if (Length < minimumCount)
                    return null;

#if PORTABLE
                byte[] buf = ToArray();
#else
                byte[] buf = GetBuffer();
#endif

                return Arrays.CopyOf(buf, payloadLength);
            }
        }
    }
}
