using System;
using System.IO;

namespace Org.BouncyCastle.Utilities.Encoders
{
    public class Base64Encoder
        : IEncoder
    {
        protected readonly byte[] encodingTable =
        {
            (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G',
            (byte)'H', (byte)'I', (byte)'J', (byte)'K', (byte)'L', (byte)'M', (byte)'N',
            (byte)'O', (byte)'P', (byte)'Q', (byte)'R', (byte)'S', (byte)'T', (byte)'U',
            (byte)'V', (byte)'W', (byte)'X', (byte)'Y', (byte)'Z',
            (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f', (byte)'g',
            (byte)'h', (byte)'i', (byte)'j', (byte)'k', (byte)'l', (byte)'m', (byte)'n',
            (byte)'o', (byte)'p', (byte)'q', (byte)'r', (byte)'s', (byte)'t', (byte)'u',
            (byte)'v',
            (byte)'w', (byte)'x', (byte)'y', (byte)'z',
            (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6',
            (byte)'7', (byte)'8', (byte)'9',
            (byte)'+', (byte)'/'
        };

        protected byte padding = (byte)'=';

        /*
        * set up the decoding table.
        */
        protected readonly byte[] decodingTable = new byte[128];

        protected void InitialiseDecodingTable()
        {
            Arrays.Fill(decodingTable, (byte)0xff);

            for (int i = 0; i < encodingTable.Length; i++)
            {
                decodingTable[encodingTable[i]] = (byte)i;
            }
        }

        public Base64Encoder()
        {
            InitialiseDecodingTable();
        }

        /**
        * encode the input data producing a base 64 output stream.
        *
        * @return the number of bytes produced.
        */
        public int Encode(
            byte[]	data,
            int		off,
            int		length,
            Stream	outStream)
        {
            int modulus = length % 3;
            int dataLength = (length - modulus);
            int a1, a2, a3;

            for (int i = off; i < off + dataLength; i += 3)
            {
                a1 = data[i] & 0xff;
                a2 = data[i + 1] & 0xff;
                a3 = data[i + 2] & 0xff;

                outStream.WriteByte(encodingTable[(int) ((uint) a1 >> 2) & 0x3f]);
                outStream.WriteByte(encodingTable[((a1 << 4) | (int) ((uint) a2 >> 4)) & 0x3f]);
                outStream.WriteByte(encodingTable[((a2 << 2) | (int) ((uint) a3 >> 6)) & 0x3f]);
                outStream.WriteByte(encodingTable[a3 & 0x3f]);
            }

            /*
            * process the tail end.
            */
            int b1, b2, b3;
            int d1, d2;

            switch (modulus)
            {
                case 0:        /* nothing left to do */
                    break;
                case 1:
                    d1 = data[off + dataLength] & 0xff;
                    b1 = (d1 >> 2) & 0x3f;
                    b2 = (d1 << 4) & 0x3f;

                    outStream.WriteByte(encodingTable[b1]);
                    outStream.WriteByte(encodingTable[b2]);
                    outStream.WriteByte(padding);
                    outStream.WriteByte(padding);
                    break;
                case 2:
                    d1 = data[off + dataLength] & 0xff;
                    d2 = data[off + dataLength + 1] & 0xff;

                    b1 = (d1 >> 2) & 0x3f;
                    b2 = ((d1 << 4) | (d2 >> 4)) & 0x3f;
                    b3 = (d2 << 2) & 0x3f;

                    outStream.WriteByte(encodingTable[b1]);
                    outStream.WriteByte(encodingTable[b2]);
                    outStream.WriteByte(encodingTable[b3]);
                    outStream.WriteByte(padding);
                    break;
            }

            return (dataLength / 3) * 4 + ((modulus == 0) ? 0 : 4);
        }

        private bool ignore(
            char c)
        {
            return (c == '\n' || c =='\r' || c == '\t' || c == ' ');
        }

        /**
        * decode the base 64 encoded byte data writing it to the given output stream,
        * whitespace characters will be ignored.
        *
        * @return the number of bytes produced.
        */
        public int Decode(
            byte[]	data,
            int		off,
            int		length,
            Stream	outStream)
        {
            byte b1, b2, b3, b4;
            int outLen = 0;

            int end = off + length;

            while (end > off)
            {
                if (!ignore((char)data[end - 1]))
                {
                    break;
                }

                end--;
            }

            int  i = off;
            int  finish = end - 4;

            i = nextI(data, i, finish);

            while (i < finish)
            {
                b1 = decodingTable[data[i++]];

                i = nextI(data, i, finish);

                b2 = decodingTable[data[i++]];

                i = nextI(data, i, finish);

                b3 = decodingTable[data[i++]];

                i = nextI(data, i, finish);

                b4 = decodingTable[data[i++]];

                if ((b1 | b2 | b3 | b4) >= 0x80)
                    throw new IOException("invalid characters encountered in base64 data");

                outStream.WriteByte((byte)((b1 << 2) | (b2 >> 4)));
                outStream.WriteByte((byte)((b2 << 4) | (b3 >> 2)));
                outStream.WriteByte((byte)((b3 << 6) | b4));

                outLen += 3;

                i = nextI(data, i, finish);
            }

            outLen += decodeLastBlock(outStream, (char)data[end - 4], (char)data[end - 3], (char)data[end - 2], (char)data[end - 1]);

            return outLen;
        }

        private int nextI(
            byte[]	data,
            int		i,
            int		finish)
        {
            while ((i < finish) && ignore((char)data[i]))
            {
                i++;
            }
            return i;
        }

        /**
        * decode the base 64 encoded string data writing it to the given output stream,
        * whitespace characters will be ignored.
        *
        * @return the number of bytes produced.
        */
        public int DecodeString(
            string	data,
            Stream	outStream)
        {
            // Platform Implementation
//			byte[] bytes = Convert.FromBase64String(data);
//			outStream.Write(bytes, 0, bytes.Length);
//			return bytes.Length;

            byte b1, b2, b3, b4;
            int length = 0;

            int end = data.Length;

            while (end > 0)
            {
                if (!ignore(data[end - 1]))
                {
                    break;
                }

                end--;
            }

            int  i = 0;
            int  finish = end - 4;

            i = nextI(data, i, finish);

            while (i < finish)
            {
                b1 = decodingTable[data[i++]];

                i = nextI(data, i, finish);

                b2 = decodingTable[data[i++]];

                i = nextI(data, i, finish);

                b3 = decodingTable[data[i++]];

                i = nextI(data, i, finish);

                b4 = decodingTable[data[i++]];

                if ((b1 | b2 | b3 | b4) >= 0x80)
                    throw new IOException("invalid characters encountered in base64 data");

                outStream.WriteByte((byte)((b1 << 2) | (b2 >> 4)));
                outStream.WriteByte((byte)((b2 << 4) | (b3 >> 2)));
                outStream.WriteByte((byte)((b3 << 6) | b4));

                length += 3;

                i = nextI(data, i, finish);
            }

            length += decodeLastBlock(outStream, data[end - 4], data[end - 3], data[end - 2], data[end - 1]);

            return length;
        }

        private int decodeLastBlock(
            Stream	outStream,
            char	c1,
            char	c2,
            char	c3,
            char	c4)
        {
            if (c3 == padding)
            {
                if (c4 != padding)
                    throw new IOException("invalid characters encountered at end of base64 data");

                byte b1 = decodingTable[c1];
                byte b2 = decodingTable[c2];

                if ((b1 | b2) >= 0x80)
                    throw new IOException("invalid characters encountered at end of base64 data");

                outStream.WriteByte((byte)((b1 << 2) | (b2 >> 4)));

                return 1;
            }

            if (c4 == padding)
            {
                byte b1 = decodingTable[c1];
                byte b2 = decodingTable[c2];
                byte b3 = decodingTable[c3];

                if ((b1 | b2 | b3) >= 0x80)
                    throw new IOException("invalid characters encountered at end of base64 data");

                outStream.WriteByte((byte)((b1 << 2) | (b2 >> 4)));
                outStream.WriteByte((byte)((b2 << 4) | (b3 >> 2)));

                return 2;
            }

            {
                byte b1 = decodingTable[c1];
                byte b2 = decodingTable[c2];
                byte b3 = decodingTable[c3];
                byte b4 = decodingTable[c4];

                if ((b1 | b2 | b3 | b4) >= 0x80)
                    throw new IOException("invalid characters encountered at end of base64 data");

                outStream.WriteByte((byte)((b1 << 2) | (b2 >> 4)));
                outStream.WriteByte((byte)((b2 << 4) | (b3 >> 2)));
                outStream.WriteByte((byte)((b3 << 6) | b4));

                return 3;
            }
        }

        private int nextI(string data, int i, int finish)
        {
            while ((i < finish) && ignore(data[i]))
            {
                i++;
            }
            return i;
        }
    }
}
