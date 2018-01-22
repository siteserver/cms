using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

#if PORTABLE
using System.Linq;
#endif

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Bcpg
{
    /**
    * Basic output stream.
    */
    public class ArmoredOutputStream
        : BaseOutputStream
    {
        public static readonly string HeaderVersion = "Version";

        private static readonly byte[] encodingTable =
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

        /**
         * encode the input data producing a base 64 encoded byte array.
         */
        private static void Encode(
            Stream    outStream,
            int[]    data,
            int        len)
        {
            Debug.Assert(len > 0);
            Debug.Assert(len < 4);

            byte[] bs = new byte[4];
            int d1 = data[0];
            bs[0] = encodingTable[(d1 >> 2) & 0x3f];

            switch (len)
            {
                case 1:
                {
                    bs[1] = encodingTable[(d1 << 4) & 0x3f];
                    bs[2] = (byte)'=';
                    bs[3] = (byte)'=';
                    break;
                }
                case 2:
                {
                    int d2 = data[1];
                    bs[1] = encodingTable[((d1 << 4) | (d2 >> 4)) & 0x3f];
                    bs[2] = encodingTable[(d2 << 2) & 0x3f];
                    bs[3] = (byte)'=';
                    break;
                }
                case 3:
                {
                    int d2 = data[1];
                    int d3 = data[2];
                    bs[1] = encodingTable[((d1 << 4) | (d2 >> 4)) & 0x3f];
                    bs[2] = encodingTable[((d2 << 2) | (d3 >> 6)) & 0x3f];
                    bs[3] = encodingTable[d3 & 0x3f];
                    break;
                }
            }

            outStream.Write(bs, 0, bs.Length);
        }

        private readonly Stream outStream;
        private int[]           buf = new int[3];
        private int             bufPtr = 0;
        private Crc24           crc = new Crc24();
        private int             chunkCount = 0;
        private int             lastb;

        private bool            start = true;
        private bool            clearText = false;
        private bool            newLine = false;

        private string          type;

        private static readonly string    nl = Platform.NewLine;
        private static readonly string    headerStart = "-----BEGIN PGP ";
        private static readonly string    headerTail = "-----";
        private static readonly string    footerStart = "-----END PGP ";
        private static readonly string    footerTail = "-----";

        private static readonly string Version = "BCPG C# v1.8.1.0";

        private readonly IDictionary headers;

        public ArmoredOutputStream(Stream outStream)
        {
            this.outStream = outStream;
            this.headers = Platform.CreateHashtable(1);
            this.headers.Add(HeaderVersion, Version);
        }

        public ArmoredOutputStream(Stream outStream, IDictionary headers)
        {
            this.outStream = outStream;
            this.headers = Platform.CreateHashtable(headers);
            if (!this.headers.Contains(HeaderVersion))
            {
                this.headers.Add(HeaderVersion, Version);
            }
        }

        /**
         * Set an additional header entry. A null value will clear the entry for name.
         *
         * @param name the name of the header entry.
         * @param v the value of the header entry.
         */
        public void SetHeader(string name, string v)
        {
            if (v == null)
            {
                headers.Remove(name);
            }
            else
            {
                headers[name] = v;
            }
        }

        /**
         * Reset the headers to only contain a Version string (if one is present).
         */
        public void ResetHeaders()
        {
            string existingVersion = (string)headers[HeaderVersion];

            headers.Clear();

            if (existingVersion != null)
            {
                headers.Add(HeaderVersion, existingVersion);
            }
        }

        /**
         * Start a clear text signed message.
         * @param hashAlgorithm
         */
        public void BeginClearText(
            HashAlgorithmTag    hashAlgorithm)
        {
            string    hash;

            switch (hashAlgorithm)
            {
            case HashAlgorithmTag.Sha1:
                hash = "SHA1";
                break;
            case HashAlgorithmTag.Sha256:
                hash = "SHA256";
                break;
            case HashAlgorithmTag.Sha384:
                hash = "SHA384";
                break;
            case HashAlgorithmTag.Sha512:
                hash = "SHA512";
                break;
            case HashAlgorithmTag.MD2:
                hash = "MD2";
                break;
            case HashAlgorithmTag.MD5:
                hash = "MD5";
                break;
            case HashAlgorithmTag.RipeMD160:
                hash = "RIPEMD160";
                break;
            default:
                throw new IOException("unknown hash algorithm tag in beginClearText: " + hashAlgorithm);
            }

            DoWrite("-----BEGIN PGP SIGNED MESSAGE-----" + nl);
            DoWrite("Hash: " + hash + nl + nl);

            clearText = true;
            newLine = true;
            lastb = 0;
        }

        public void EndClearText()
        {
            clearText = false;
        }

        public override void WriteByte(
            byte b)
        {
            if (clearText)
            {
                outStream.WriteByte(b);

                if (newLine)
                {
                    if (!(b == '\n' && lastb == '\r'))
                    {
                        newLine = false;
                    }
                    if (b == '-')
                    {
                        outStream.WriteByte((byte)' ');
                        outStream.WriteByte((byte)'-');      // dash escape
                    }
                }
                if (b == '\r' || (b == '\n' && lastb != '\r'))
                {
                    newLine = true;
                }
                lastb = b;
                return;
            }

            if (start)
            {
                bool newPacket = (b & 0x40) != 0;

                int tag;
                if (newPacket)
                {
                    tag = b & 0x3f;
                }
                else
                {
                    tag = (b & 0x3f) >> 2;
                }

                switch ((PacketTag)tag)
                {
                case PacketTag.PublicKey:
                    type = "PUBLIC KEY BLOCK";
                    break;
                case PacketTag.SecretKey:
                    type = "PRIVATE KEY BLOCK";
                    break;
                case PacketTag.Signature:
                    type = "SIGNATURE";
                    break;
                default:
                    type = "MESSAGE";
                    break;
                }

                DoWrite(headerStart + type + headerTail + nl);
                if (headers.Contains(HeaderVersion))
                {
                    WriteHeaderEntry(HeaderVersion, (string)headers[HeaderVersion]);
                }

                foreach (DictionaryEntry de in headers)
                {
                    string k = (string)de.Key;
                    if (k != HeaderVersion)
                    {
                        string v = (string)de.Value;
                        WriteHeaderEntry(k, v);
                    }
                }

                DoWrite(nl);

                start = false;
            }

            if (bufPtr == 3)
            {
                Encode(outStream, buf, bufPtr);
                bufPtr = 0;
                if ((++chunkCount & 0xf) == 0)
                {
                    DoWrite(nl);
                }
            }

            crc.Update(b);
            buf[bufPtr++] = b & 0xff;
        }

        /**
         * <b>Note</b>: Close() does not close the underlying stream. So it is possible to write
         * multiple objects using armoring to a single stream.
         */
#if PORTABLE
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (type == null)
                    return;

                DoClose();

                type = null;
                start = true;
            }
            base.Dispose(disposing);
        }
#else
        public override void Close()
        {
            if (type == null)
                return;

            DoClose();

            type = null;
            start = true;

            base.Close();
        }
#endif

        private void DoClose()
        {
            if (bufPtr > 0)
            {
                Encode(outStream, buf, bufPtr);
            }

            DoWrite(nl + '=');

            int crcV = crc.Value;

            buf[0] = ((crcV >> 16) & 0xff);
            buf[1] = ((crcV >> 8) & 0xff);
            buf[2] = (crcV & 0xff);

            Encode(outStream, buf, 3);

            DoWrite(nl);
            DoWrite(footerStart);
            DoWrite(type);
            DoWrite(footerTail);
            DoWrite(nl);

            outStream.Flush();
        }

        private void WriteHeaderEntry(
            string name,
            string v)
        {
            DoWrite(name + ": " + v + nl);
        }

        private void DoWrite(
            string s)
        {
            byte[] bs = Strings.ToAsciiByteArray(s);
            outStream.Write(bs, 0, bs.Length);
        }
    }
}
