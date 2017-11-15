using System;
using System.Collections;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <remarks>Some helper functions for MicroTLS.</remarks>
    public abstract class TlsUtilities
    {
        public static readonly byte[] EmptyBytes = new byte[0];
        public static readonly short[] EmptyShorts = new short[0];
        public static readonly int[] EmptyInts = new int[0];
        public static readonly long[] EmptyLongs = new long[0];

        public static void CheckUint8(int i)
        {
            if (!IsValidUint8(i))
                throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        public static void CheckUint8(long i)
        {
            if (!IsValidUint8(i))
                throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        public static void CheckUint16(int i)
        {
            if (!IsValidUint16(i))
                throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        public static void CheckUint16(long i)
        {
            if (!IsValidUint16(i))
                throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        public static void CheckUint24(int i)
        {
            if (!IsValidUint24(i))
                throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        public static void CheckUint24(long i)
        {
            if (!IsValidUint24(i))
                throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        public static void CheckUint32(long i)
        {
            if (!IsValidUint32(i))
                throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        public static void CheckUint48(long i)
        {
            if (!IsValidUint48(i))
                throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        public static void CheckUint64(long i)
        {
            if (!IsValidUint64(i))
                throw new TlsFatalAlert(AlertDescription.internal_error);
        }

        public static bool IsValidUint8(int i)
        {
            return (i & 0xFF) == i;
        }

        public static bool IsValidUint8(long i)
        {
            return (i & 0xFFL) == i;
        }

        public static bool IsValidUint16(int i)
        {
            return (i & 0xFFFF) == i;
        }

        public static bool IsValidUint16(long i)
        {
            return (i & 0xFFFFL) == i;
        }

        public static bool IsValidUint24(int i)
        {
            return (i & 0xFFFFFF) == i;
        }

        public static bool IsValidUint24(long i)
        {
            return (i & 0xFFFFFFL) == i;
        }

        public static bool IsValidUint32(long i)
        {
            return (i & 0xFFFFFFFFL) == i;
        }

        public static bool IsValidUint48(long i)
        {
            return (i & 0xFFFFFFFFFFFFL) == i;
        }

        public static bool IsValidUint64(long i)
        {
            return true;
        }

        public static bool IsSsl(TlsContext context)
        {
            return context.ServerVersion.IsSsl;
        }

        public static bool IsTlsV11(ProtocolVersion version)
        {
            return ProtocolVersion.TLSv11.IsEqualOrEarlierVersionOf(version.GetEquivalentTLSVersion());
        }

        public static bool IsTlsV11(TlsContext context)
        {
            return IsTlsV11(context.ServerVersion);
        }

        public static bool IsTlsV12(ProtocolVersion version)
        {
            return ProtocolVersion.TLSv12.IsEqualOrEarlierVersionOf(version.GetEquivalentTLSVersion());
        }

        public static bool IsTlsV12(TlsContext context)
        {
            return IsTlsV12(context.ServerVersion);
        }

        public static void WriteUint8(byte i, Stream output)
        {
            output.WriteByte(i);
        }

        public static void WriteUint8(byte i, byte[] buf, int offset)
        {
            buf[offset] = i;
        }

        public static void WriteUint16(int i, Stream output)
        {
            output.WriteByte((byte)(i >> 8));
            output.WriteByte((byte)i);
        }

        public static void WriteUint16(int i, byte[] buf, int offset)
        {
            buf[offset] = (byte)(i >> 8);
            buf[offset + 1] = (byte)i;
        }

        public static void WriteUint24(int i, Stream output)
        {
            output.WriteByte((byte)(i >> 16));
            output.WriteByte((byte)(i >> 8));
            output.WriteByte((byte)i);
        }

        public static void WriteUint24(int i, byte[] buf, int offset)
        {
            buf[offset] = (byte)(i >> 16);
            buf[offset + 1] = (byte)(i >> 8);
            buf[offset + 2] = (byte)i;
        }

        public static void WriteUint32(long i, Stream output)
        {
            output.WriteByte((byte)(i >> 24));
            output.WriteByte((byte)(i >> 16));
            output.WriteByte((byte)(i >> 8));
            output.WriteByte((byte)i);
        }

        public static void WriteUint32(long i, byte[] buf, int offset)
        {
            buf[offset] = (byte)(i >> 24);
            buf[offset + 1] = (byte)(i >> 16);
            buf[offset + 2] = (byte)(i >> 8);
            buf[offset + 3] = (byte)i;
        }

        public static void WriteUint48(long i, Stream output)
        {
            output.WriteByte((byte)(i >> 40));
            output.WriteByte((byte)(i >> 32));
            output.WriteByte((byte)(i >> 24));
            output.WriteByte((byte)(i >> 16));
            output.WriteByte((byte)(i >> 8));
            output.WriteByte((byte)i);
        }

        public static void WriteUint48(long i, byte[] buf, int offset)
        {
            buf[offset] = (byte)(i >> 40);
            buf[offset + 1] = (byte)(i >> 32);
            buf[offset + 2] = (byte)(i >> 24);
            buf[offset + 3] = (byte)(i >> 16);
            buf[offset + 4] = (byte)(i >> 8);
            buf[offset + 5] = (byte)i;
        }

        public static void WriteUint64(long i, Stream output)
        {
            output.WriteByte((byte)(i >> 56));
            output.WriteByte((byte)(i >> 48));
            output.WriteByte((byte)(i >> 40));
            output.WriteByte((byte)(i >> 32));
            output.WriteByte((byte)(i >> 24));
            output.WriteByte((byte)(i >> 16));
            output.WriteByte((byte)(i >> 8));
            output.WriteByte((byte)i);
        }

        public static void WriteUint64(long i, byte[] buf, int offset)
        {
            buf[offset] = (byte)(i >> 56);
            buf[offset + 1] = (byte)(i >> 48);
            buf[offset + 2] = (byte)(i >> 40);
            buf[offset + 3] = (byte)(i >> 32);
            buf[offset + 4] = (byte)(i >> 24);
            buf[offset + 5] = (byte)(i >> 16);
            buf[offset + 6] = (byte)(i >> 8);
            buf[offset + 7] = (byte)i;
        }

        public static void WriteOpaque8(byte[] buf, Stream output)
        {
            WriteUint8((byte)buf.Length, output);
            output.Write(buf, 0, buf.Length);
        }

        public static void WriteOpaque16(byte[] buf, Stream output)
        {
            WriteUint16(buf.Length, output);
            output.Write(buf, 0, buf.Length);
        }

        public static void WriteOpaque24(byte[] buf, Stream output)
        {
            WriteUint24(buf.Length, output);
            output.Write(buf, 0, buf.Length);
        }

        public static void WriteUint8Array(byte[] uints, Stream output)
        {
            output.Write(uints, 0, uints.Length);
        }

        public static void WriteUint8Array(byte[] uints, byte[] buf, int offset)
        {
            for (int i = 0; i < uints.Length; ++i)
            {
                WriteUint8(uints[i], buf, offset);
                ++offset;
            }
        }

        public static void WriteUint8ArrayWithUint8Length(byte[] uints, Stream output)
        {
            CheckUint8(uints.Length);
            WriteUint8((byte)uints.Length, output);
            WriteUint8Array(uints, output);
        }

        public static void WriteUint8ArrayWithUint8Length(byte[] uints, byte[] buf, int offset)
        {
            CheckUint8(uints.Length);
            WriteUint8((byte)uints.Length, buf, offset);
            WriteUint8Array(uints, buf, offset + 1);
        }

        public static void WriteUint16Array(int[] uints, Stream output)
        {
            for (int i = 0; i < uints.Length; ++i)
            {
                WriteUint16(uints[i], output);
            }
        }

        public static void WriteUint16Array(int[] uints, byte[] buf, int offset)
        {
            for (int i = 0; i < uints.Length; ++i)
            {
                WriteUint16(uints[i], buf, offset);
                offset += 2;
            }
        }

        public static void WriteUint16ArrayWithUint16Length(int[] uints, Stream output)
        {
            int length = 2 * uints.Length;
            CheckUint16(length);
            WriteUint16(length, output);
            WriteUint16Array(uints, output);
        }

        public static void WriteUint16ArrayWithUint16Length(int[] uints, byte[] buf, int offset)
        {
            int length = 2 * uints.Length;
            CheckUint16(length);
            WriteUint16(length, buf, offset);
            WriteUint16Array(uints, buf, offset + 2);
        }

        public static byte DecodeUint8(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");
            if (buf.Length != 1)
                throw new TlsFatalAlert(AlertDescription.decode_error);
            return ReadUint8(buf, 0);
        }

        public static byte[] DecodeUint8ArrayWithUint8Length(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");

            int count = ReadUint8(buf, 0);
            if (buf.Length != (count + 1))
                throw new TlsFatalAlert(AlertDescription.decode_error);

            byte[] uints = new byte[count];
            for (int i = 0; i < count; ++i)
            {
                uints[i] = ReadUint8(buf, i + 1);
            }
            return uints;
        }

        public static byte[] EncodeOpaque8(byte[] buf)
        {
            CheckUint8(buf.Length);
            return Arrays.Prepend(buf, (byte)buf.Length);
        }

        public static byte[] EncodeUint8(byte val)
        {
            CheckUint8(val);

            byte[] extensionData = new byte[1];
            WriteUint8(val, extensionData, 0);
            return extensionData;
        }

        public static byte[] EncodeUint8ArrayWithUint8Length(byte[] uints)
        {
            byte[] result = new byte[1 + uints.Length];
            WriteUint8ArrayWithUint8Length(uints, result, 0);
            return result;
        }

        public static byte[] EncodeUint16ArrayWithUint16Length(int[] uints)
        {
            int length = 2 * uints.Length;
            byte[] result = new byte[2 + length];
            WriteUint16ArrayWithUint16Length(uints, result, 0);
            return result;
        }

        public static byte ReadUint8(Stream input)
        {
            int i = input.ReadByte();
            if (i < 0)
                throw new EndOfStreamException();
            return (byte)i;
        }

        public static byte ReadUint8(byte[] buf, int offset)
        {
            return buf[offset];
        }

        public static int ReadUint16(Stream input)
        {
            int i1 = input.ReadByte();
            int i2 = input.ReadByte();
            if (i2 < 0)
                throw new EndOfStreamException();
            return (i1 << 8) | i2;
        }

        public static int ReadUint16(byte[] buf, int offset)
        {
            uint n = (uint)buf[offset] << 8;
            n |= (uint)buf[++offset];
            return (int)n;
        }

        public static int ReadUint24(Stream input)
        {
            int i1 = input.ReadByte();
            int i2 = input.ReadByte();
            int i3 = input.ReadByte();
            if (i3 < 0)
                throw new EndOfStreamException();
            return (i1 << 16) | (i2 << 8) | i3;
        }

        public static int ReadUint24(byte[] buf, int offset)
        {
            uint n = (uint)buf[offset] << 16;
            n |= (uint)buf[++offset] << 8;
            n |= (uint)buf[++offset];
            return (int)n;
        }

        public static long ReadUint32(Stream input)
        {
            int i1 = input.ReadByte();
            int i2 = input.ReadByte();
            int i3 = input.ReadByte();
            int i4 = input.ReadByte();
            if (i4 < 0)
                throw new EndOfStreamException();
            return (long)(uint)((i1 << 24) | (i2 << 16) | (i3 << 8) | i4);
        }

        public static long ReadUint32(byte[] buf, int offset)
        {
            uint n = (uint)buf[offset] << 24;
            n |= (uint)buf[++offset] << 16;
            n |= (uint)buf[++offset] << 8;
            n |= (uint)buf[++offset];
            return (long)n;
        }

        public static long ReadUint48(Stream input)
        {
            int hi = ReadUint24(input);
            int lo = ReadUint24(input);
            return ((long)(hi & 0xffffffffL) << 24) | (long)(lo & 0xffffffffL);
        }

        public static long ReadUint48(byte[] buf, int offset)
        {
            int hi = ReadUint24(buf, offset);
            int lo = ReadUint24(buf, offset + 3);
            return ((long)(hi & 0xffffffffL) << 24) | (long)(lo & 0xffffffffL);
        }

        public static byte[] ReadAllOrNothing(int length, Stream input)
        {
            if (length < 1)
                return EmptyBytes;
            byte[] buf = new byte[length];
            int read = Streams.ReadFully(input, buf);
            if (read == 0)
                return null;
            if (read != length)
                throw new EndOfStreamException();
            return buf;
        }

        public static byte[] ReadFully(int length, Stream input)
        {
            if (length < 1)
                return EmptyBytes;
            byte[] buf = new byte[length];
            if (length != Streams.ReadFully(input, buf))
                throw new EndOfStreamException();
            return buf;
        }

        public static void ReadFully(byte[] buf, Stream input)
        {
            if (Streams.ReadFully(input, buf, 0, buf.Length) < buf.Length)
                throw new EndOfStreamException();
        }

        public static byte[] ReadOpaque8(Stream input)
        {
            byte length = ReadUint8(input);
            byte[] bytes = new byte[length];
            ReadFully(bytes, input);
            return bytes;
        }

        public static byte[] ReadOpaque16(Stream input)
        {
            int length = ReadUint16(input);
            byte[] bytes = new byte[length];
            ReadFully(bytes, input);
            return bytes;
        }

        public static byte[] ReadOpaque24(Stream input)
        {
            int length = ReadUint24(input);
            return ReadFully(length, input);
        }

        public static byte[] ReadUint8Array(int count, Stream input)
        {
            byte[] uints = new byte[count];
            for (int i = 0; i < count; ++i)
            {
                uints[i] = ReadUint8(input);
            }
            return uints;
        }

        public static int[] ReadUint16Array(int count, Stream input)
        {
            int[] uints = new int[count];
            for (int i = 0; i < count; ++i)
            {
                uints[i] = ReadUint16(input);
            }
            return uints;
        }

        public static ProtocolVersion ReadVersion(byte[] buf, int offset)
        {
            return ProtocolVersion.Get(buf[offset], buf[offset + 1]);
        }

        public static ProtocolVersion ReadVersion(Stream input)
        {
            int i1 = input.ReadByte();
            int i2 = input.ReadByte();
            if (i2 < 0)
                throw new EndOfStreamException();
            return ProtocolVersion.Get(i1, i2);
        }

        public static int ReadVersionRaw(byte[] buf, int offset)
        {
            return (buf[offset] << 8) | buf[offset + 1];
        }

        public static int ReadVersionRaw(Stream input)
        {
            int i1 = input.ReadByte();
            int i2 = input.ReadByte();
            if (i2 < 0)
                throw new EndOfStreamException();
            return (i1 << 8) | i2;
        }

        public static Asn1Object ReadAsn1Object(byte[] encoding)
        {
            MemoryStream input = new MemoryStream(encoding, false);
            Asn1InputStream asn1 = new Asn1InputStream(input, encoding.Length);
            Asn1Object result = asn1.ReadObject();
            if (null == result)
                throw new TlsFatalAlert(AlertDescription.decode_error);
            if (input.Position != input.Length)
                throw new TlsFatalAlert(AlertDescription.decode_error);
            return result;
        }

        public static Asn1Object ReadDerObject(byte[] encoding)
        {
            /*
             * NOTE: The current ASN.1 parsing code can't enforce DER-only parsing, but since DER is
             * canonical, we can check it by re-encoding the result and comparing to the original.
             */
            Asn1Object result = ReadAsn1Object(encoding);
            byte[] check = result.GetEncoded(Asn1Encodable.Der);
            if (!Arrays.AreEqual(check, encoding))
                throw new TlsFatalAlert(AlertDescription.decode_error);
            return result;
        }

        public static void WriteGmtUnixTime(byte[] buf, int offset)
        {
            int t = (int)(DateTimeUtilities.CurrentUnixMs() / 1000L);
            buf[offset] = (byte)(t >> 24);
            buf[offset + 1] = (byte)(t >> 16);
            buf[offset + 2] = (byte)(t >> 8);
            buf[offset + 3] = (byte)t;
        }

        public static void WriteVersion(ProtocolVersion version, Stream output)
        {
            output.WriteByte((byte)version.MajorVersion);
            output.WriteByte((byte)version.MinorVersion);
        }

        public static void WriteVersion(ProtocolVersion version, byte[] buf, int offset)
        {
            buf[offset] = (byte)version.MajorVersion;
            buf[offset + 1] = (byte)version.MinorVersion;
        }

        public static IList GetAllSignatureAlgorithms()
        {
            IList v = Platform.CreateArrayList(4);
            v.Add(SignatureAlgorithm.anonymous);
            v.Add(SignatureAlgorithm.rsa);
            v.Add(SignatureAlgorithm.dsa);
            v.Add(SignatureAlgorithm.ecdsa);
            return v;
        }

        public static IList GetDefaultDssSignatureAlgorithms()
        {
            return VectorOfOne(new SignatureAndHashAlgorithm(HashAlgorithm.sha1, SignatureAlgorithm.dsa));
        }

        public static IList GetDefaultECDsaSignatureAlgorithms()
        {
            return VectorOfOne(new SignatureAndHashAlgorithm(HashAlgorithm.sha1, SignatureAlgorithm.ecdsa));
        }

        public static IList GetDefaultRsaSignatureAlgorithms()
        {
            return VectorOfOne(new SignatureAndHashAlgorithm(HashAlgorithm.sha1, SignatureAlgorithm.rsa));
        }

        public static byte[] GetExtensionData(IDictionary extensions, int extensionType)
        {
            return extensions == null ? null : (byte[])extensions[extensionType];
        }

        public static IList GetDefaultSupportedSignatureAlgorithms()
        {
            byte[] hashAlgorithms = new byte[]{ HashAlgorithm.sha1, HashAlgorithm.sha224, HashAlgorithm.sha256,
                HashAlgorithm.sha384, HashAlgorithm.sha512 };
            byte[] signatureAlgorithms = new byte[]{ SignatureAlgorithm.rsa, SignatureAlgorithm.dsa,
                SignatureAlgorithm.ecdsa };

            IList result = Platform.CreateArrayList();
            for (int i = 0; i < signatureAlgorithms.Length; ++i)
            {
                for (int j = 0; j < hashAlgorithms.Length; ++j)
                {
                    result.Add(new SignatureAndHashAlgorithm(hashAlgorithms[j], signatureAlgorithms[i]));
                }
            }
            return result;
        }

        public static SignatureAndHashAlgorithm GetSignatureAndHashAlgorithm(TlsContext context,
            TlsSignerCredentials signerCredentials)
        {
            SignatureAndHashAlgorithm signatureAndHashAlgorithm = null;
            if (IsTlsV12(context))
            {
                signatureAndHashAlgorithm = signerCredentials.SignatureAndHashAlgorithm;
                if (signatureAndHashAlgorithm == null)
                    throw new TlsFatalAlert(AlertDescription.internal_error);
            }
            return signatureAndHashAlgorithm;
        }

        public static bool HasExpectedEmptyExtensionData(IDictionary extensions, int extensionType,
            byte alertDescription)
        {
            byte[] extension_data = GetExtensionData(extensions, extensionType);
            if (extension_data == null)
                return false;
            if (extension_data.Length != 0)
                throw new TlsFatalAlert(alertDescription);
            return true;
        }

        public static TlsSession ImportSession(byte[] sessionID, SessionParameters sessionParameters)
        {
            return new TlsSessionImpl(sessionID, sessionParameters);
        }

        public static bool IsSignatureAlgorithmsExtensionAllowed(ProtocolVersion clientVersion)
        {
            return ProtocolVersion.TLSv12.IsEqualOrEarlierVersionOf(clientVersion.GetEquivalentTLSVersion());
        }

        /**
         * Add a 'signature_algorithms' extension to existing extensions.
         *
         * @param extensions                   A {@link Hashtable} to add the extension to.
         * @param supportedSignatureAlgorithms {@link Vector} containing at least 1 {@link SignatureAndHashAlgorithm}.
         * @throws IOException
         */
        public static void AddSignatureAlgorithmsExtension(IDictionary extensions, IList supportedSignatureAlgorithms)
        {
            extensions[ExtensionType.signature_algorithms] = CreateSignatureAlgorithmsExtension(supportedSignatureAlgorithms);
        }

        /**
         * Get a 'signature_algorithms' extension from extensions.
         *
         * @param extensions A {@link Hashtable} to get the extension from, if it is present.
         * @return A {@link Vector} containing at least 1 {@link SignatureAndHashAlgorithm}, or null.
         * @throws IOException
         */
        public static IList GetSignatureAlgorithmsExtension(IDictionary extensions)
        {
            byte[] extensionData = GetExtensionData(extensions, ExtensionType.signature_algorithms);
            return extensionData == null ? null : ReadSignatureAlgorithmsExtension(extensionData);
        }

        /**
         * Create a 'signature_algorithms' extension value.
         *
         * @param supportedSignatureAlgorithms A {@link Vector} containing at least 1 {@link SignatureAndHashAlgorithm}.
         * @return A byte array suitable for use as an extension value.
         * @throws IOException
         */
        public static byte[] CreateSignatureAlgorithmsExtension(IList supportedSignatureAlgorithms)
        {
            MemoryStream buf = new MemoryStream();

            // supported_signature_algorithms
            EncodeSupportedSignatureAlgorithms(supportedSignatureAlgorithms, false, buf);

            return buf.ToArray();
        }

        /**
         * Read 'signature_algorithms' extension data.
         *
         * @param extensionData The extension data.
         * @return A {@link Vector} containing at least 1 {@link SignatureAndHashAlgorithm}.
         * @throws IOException
         */
        public static IList ReadSignatureAlgorithmsExtension(byte[] extensionData)
        {
            if (extensionData == null)
                throw new ArgumentNullException("extensionData");

            MemoryStream buf = new MemoryStream(extensionData, false);

            // supported_signature_algorithms
            IList supported_signature_algorithms = ParseSupportedSignatureAlgorithms(false, buf);

            TlsProtocol.AssertEmpty(buf);

            return supported_signature_algorithms;
        }

        public static void EncodeSupportedSignatureAlgorithms(IList supportedSignatureAlgorithms, bool allowAnonymous,
            Stream output)
        {
            if (supportedSignatureAlgorithms == null)
                throw new ArgumentNullException("supportedSignatureAlgorithms");
            if (supportedSignatureAlgorithms.Count < 1 || supportedSignatureAlgorithms.Count >= (1 << 15))
                throw new ArgumentException("must have length from 1 to (2^15 - 1)", "supportedSignatureAlgorithms");

            // supported_signature_algorithms
            int length = 2 * supportedSignatureAlgorithms.Count;
            CheckUint16(length);
            WriteUint16(length, output);

            foreach (SignatureAndHashAlgorithm entry in supportedSignatureAlgorithms)
            {
                if (!allowAnonymous && entry.Signature == SignatureAlgorithm.anonymous)
                {
                    /*
                     * RFC 5246 7.4.1.4.1 The "anonymous" value is meaningless in this context but used
                     * in Section 7.4.3. It MUST NOT appear in this extension.
                     */
                    throw new ArgumentException(
                        "SignatureAlgorithm.anonymous MUST NOT appear in the signature_algorithms extension");
                }
                entry.Encode(output);
            }
        }

        public static IList ParseSupportedSignatureAlgorithms(bool allowAnonymous, Stream input)
        {
            // supported_signature_algorithms
            int length = ReadUint16(input);
            if (length < 2 || (length & 1) != 0)
                throw new TlsFatalAlert(AlertDescription.decode_error);
            int count = length / 2;
            IList supportedSignatureAlgorithms = Platform.CreateArrayList(count);
            for (int i = 0; i < count; ++i)
            {
                SignatureAndHashAlgorithm entry = SignatureAndHashAlgorithm.Parse(input);
                if (!allowAnonymous && entry.Signature == SignatureAlgorithm.anonymous)
                {
                    /*
                     * RFC 5246 7.4.1.4.1 The "anonymous" value is meaningless in this context but used
                     * in Section 7.4.3. It MUST NOT appear in this extension.
                     */
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                }
                supportedSignatureAlgorithms.Add(entry);
            }
            return supportedSignatureAlgorithms;
        }

        public static void VerifySupportedSignatureAlgorithm(IList supportedSignatureAlgorithms, SignatureAndHashAlgorithm signatureAlgorithm)
        {
            if (supportedSignatureAlgorithms == null)
                throw new ArgumentNullException("supportedSignatureAlgorithms");
            if (supportedSignatureAlgorithms.Count < 1 || supportedSignatureAlgorithms.Count >= (1 << 15))
                throw new ArgumentException("must have length from 1 to (2^15 - 1)", "supportedSignatureAlgorithms");
            if (signatureAlgorithm == null)
                throw new ArgumentNullException("signatureAlgorithm");

            if (signatureAlgorithm.Signature != SignatureAlgorithm.anonymous)
            {
                foreach (SignatureAndHashAlgorithm entry in supportedSignatureAlgorithms)
                {
                    if (entry.Hash == signatureAlgorithm.Hash && entry.Signature == signatureAlgorithm.Signature)
                        return;
                }
            }

            throw new TlsFatalAlert(AlertDescription.illegal_parameter);
        }

        public static byte[] PRF(TlsContext context, byte[] secret, string asciiLabel, byte[] seed, int size)
        {
            ProtocolVersion version = context.ServerVersion;

            if (version.IsSsl)
                throw new InvalidOperationException("No PRF available for SSLv3 session");

            byte[] label = Strings.ToByteArray(asciiLabel);
            byte[] labelSeed = Concat(label, seed);

            int prfAlgorithm = context.SecurityParameters.PrfAlgorithm;

            if (prfAlgorithm == PrfAlgorithm.tls_prf_legacy)
                return PRF_legacy(secret, label, labelSeed, size);

            IDigest prfDigest = CreatePrfHash(prfAlgorithm);
            byte[] buf = new byte[size];
            HMacHash(prfDigest, secret, labelSeed, buf);
            return buf;
        }

        public static byte[] PRF_legacy(byte[] secret, string asciiLabel, byte[] seed, int size)
        {
            byte[] label = Strings.ToByteArray(asciiLabel);
            byte[] labelSeed = Concat(label, seed);

            return PRF_legacy(secret, label, labelSeed, size);
        }

        internal static byte[] PRF_legacy(byte[] secret, byte[] label, byte[] labelSeed, int size)
        {
            int s_half = (secret.Length + 1) / 2;
            byte[] s1 = new byte[s_half];
            byte[] s2 = new byte[s_half];
            Array.Copy(secret, 0, s1, 0, s_half);
            Array.Copy(secret, secret.Length - s_half, s2, 0, s_half);

            byte[] b1 = new byte[size];
            byte[] b2 = new byte[size];
            HMacHash(CreateHash(HashAlgorithm.md5), s1, labelSeed, b1);
            HMacHash(CreateHash(HashAlgorithm.sha1), s2, labelSeed, b2);
            for (int i = 0; i < size; i++)
            {
                b1[i] ^= b2[i];
            }
            return b1;
        }

        internal static byte[] Concat(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            Array.Copy(a, 0, c, 0, a.Length);
            Array.Copy(b, 0, c, a.Length, b.Length);
            return c;
        }

        internal static void HMacHash(IDigest digest, byte[] secret, byte[] seed, byte[] output)
        {
            HMac mac = new HMac(digest);
            mac.Init(new KeyParameter(secret));
            byte[] a = seed;
            int size = digest.GetDigestSize();
            int iterations = (output.Length + size - 1) / size;
            byte[] buf = new byte[mac.GetMacSize()];
            byte[] buf2 = new byte[mac.GetMacSize()];
            for (int i = 0; i < iterations; i++)
            {
                mac.BlockUpdate(a, 0, a.Length);
                mac.DoFinal(buf, 0);
                a = buf;
                mac.BlockUpdate(a, 0, a.Length);
                mac.BlockUpdate(seed, 0, seed.Length);
                mac.DoFinal(buf2, 0);
                Array.Copy(buf2, 0, output, (size * i), System.Math.Min(size, output.Length - (size * i)));
            }
        }

        internal static void ValidateKeyUsage(X509CertificateStructure c, int keyUsageBits)
        {
            X509Extensions exts = c.TbsCertificate.Extensions;
            if (exts != null)
            {
                X509Extension ext = exts.GetExtension(X509Extensions.KeyUsage);
                if (ext != null)
                {
                    DerBitString ku = KeyUsage.GetInstance(ext);
                    int bits = ku.GetBytes()[0];
                    if ((bits & keyUsageBits) != keyUsageBits)
                        throw new TlsFatalAlert(AlertDescription.certificate_unknown);
                }
            }
        }

        internal static byte[] CalculateKeyBlock(TlsContext context, int size)
        {
            SecurityParameters securityParameters = context.SecurityParameters;
            byte[] master_secret = securityParameters.MasterSecret;
            byte[] seed = Concat(securityParameters.ServerRandom, securityParameters.ClientRandom);

            if (IsSsl(context))
                return CalculateKeyBlock_Ssl(master_secret, seed, size);

            return PRF(context, master_secret, ExporterLabel.key_expansion, seed, size);
        }

        internal static byte[] CalculateKeyBlock_Ssl(byte[] master_secret, byte[] random, int size)
        {
            IDigest md5 = CreateHash(HashAlgorithm.md5);
            IDigest sha1 = CreateHash(HashAlgorithm.sha1);
            int md5Size = md5.GetDigestSize();
            byte[] shatmp = new byte[sha1.GetDigestSize()];
            byte[] tmp = new byte[size + md5Size];

            int i = 0, pos = 0;
            while (pos < size)
            {
                byte[] ssl3Const = SSL3_CONST[i];

                sha1.BlockUpdate(ssl3Const, 0, ssl3Const.Length);
                sha1.BlockUpdate(master_secret, 0, master_secret.Length);
                sha1.BlockUpdate(random, 0, random.Length);
                sha1.DoFinal(shatmp, 0);

                md5.BlockUpdate(master_secret, 0, master_secret.Length);
                md5.BlockUpdate(shatmp, 0, shatmp.Length);
                md5.DoFinal(tmp, pos);

                pos += md5Size;
                ++i;
            }

            return Arrays.CopyOfRange(tmp, 0, size);
        }

        internal static byte[] CalculateMasterSecret(TlsContext context, byte[] pre_master_secret)
        {
            SecurityParameters securityParameters = context.SecurityParameters;

            byte[] seed = securityParameters.extendedMasterSecret
                ?   securityParameters.SessionHash
                :   Concat(securityParameters.ClientRandom, securityParameters.ServerRandom);

            if (IsSsl(context))
                return CalculateMasterSecret_Ssl(pre_master_secret, seed);

            string asciiLabel = securityParameters.extendedMasterSecret
                ?   ExporterLabel.extended_master_secret
                :   ExporterLabel.master_secret;

            return PRF(context, pre_master_secret, asciiLabel, seed, 48);
        }

        internal static byte[] CalculateMasterSecret_Ssl(byte[] pre_master_secret, byte[] random)
        {
            IDigest md5 = CreateHash(HashAlgorithm.md5);
            IDigest sha1 = CreateHash(HashAlgorithm.sha1);
            int md5Size = md5.GetDigestSize();
            byte[] shatmp = new byte[sha1.GetDigestSize()];

            byte[] rval = new byte[md5Size * 3];
            int pos = 0;

            for (int i = 0; i < 3; ++i)
            {
                byte[] ssl3Const = SSL3_CONST[i];

                sha1.BlockUpdate(ssl3Const, 0, ssl3Const.Length);
                sha1.BlockUpdate(pre_master_secret, 0, pre_master_secret.Length);
                sha1.BlockUpdate(random, 0, random.Length);
                sha1.DoFinal(shatmp, 0);

                md5.BlockUpdate(pre_master_secret, 0, pre_master_secret.Length);
                md5.BlockUpdate(shatmp, 0, shatmp.Length);
                md5.DoFinal(rval, pos);

                pos += md5Size;
            }

            return rval;
        }

        internal static byte[] CalculateVerifyData(TlsContext context, string asciiLabel, byte[] handshakeHash)
        {
            if (IsSsl(context))
                return handshakeHash;

            SecurityParameters securityParameters = context.SecurityParameters;
            byte[] master_secret = securityParameters.MasterSecret;
            int verify_data_length = securityParameters.VerifyDataLength;

            return PRF(context, master_secret, asciiLabel, handshakeHash, verify_data_length);
        }

        public static IDigest CreateHash(byte hashAlgorithm)
        {
            switch (hashAlgorithm)
            {
            case HashAlgorithm.md5:
                return new MD5Digest();
            case HashAlgorithm.sha1:
                return new Sha1Digest();
            case HashAlgorithm.sha224:
                return new Sha224Digest();
            case HashAlgorithm.sha256:
                return new Sha256Digest();
            case HashAlgorithm.sha384:
                return new Sha384Digest();
            case HashAlgorithm.sha512:
                return new Sha512Digest();
            default:
                throw new ArgumentException("unknown HashAlgorithm", "hashAlgorithm");
            }
        }

        public static IDigest CreateHash(SignatureAndHashAlgorithm signatureAndHashAlgorithm)
        {
            return signatureAndHashAlgorithm == null
                ?   new CombinedHash()
                :   CreateHash(signatureAndHashAlgorithm.Hash);
        }

        public static IDigest CloneHash(byte hashAlgorithm, IDigest hash)
        {
            switch (hashAlgorithm)
            {
            case HashAlgorithm.md5:
                return new MD5Digest((MD5Digest)hash);
            case HashAlgorithm.sha1:
                return new Sha1Digest((Sha1Digest)hash);
            case HashAlgorithm.sha224:
                return new Sha224Digest((Sha224Digest)hash);
            case HashAlgorithm.sha256:
                return new Sha256Digest((Sha256Digest)hash);
            case HashAlgorithm.sha384:
                return new Sha384Digest((Sha384Digest)hash);
            case HashAlgorithm.sha512:
                return new Sha512Digest((Sha512Digest)hash);
            default:
                throw new ArgumentException("unknown HashAlgorithm", "hashAlgorithm");
            }
        }

        public static IDigest CreatePrfHash(int prfAlgorithm)
        {
            switch (prfAlgorithm)
            {
                case PrfAlgorithm.tls_prf_legacy:
                    return new CombinedHash();
                default:
                    return CreateHash(GetHashAlgorithmForPrfAlgorithm(prfAlgorithm));
            }
        }

        public static IDigest ClonePrfHash(int prfAlgorithm, IDigest hash)
        {
            switch (prfAlgorithm)
            {
                case PrfAlgorithm.tls_prf_legacy:
                    return new CombinedHash((CombinedHash)hash);
                default:
                    return CloneHash(GetHashAlgorithmForPrfAlgorithm(prfAlgorithm), hash);
            }
        }

        public static byte GetHashAlgorithmForPrfAlgorithm(int prfAlgorithm)
        {
            switch (prfAlgorithm)
            {
                case PrfAlgorithm.tls_prf_legacy:
                    throw new ArgumentException("legacy PRF not a valid algorithm", "prfAlgorithm");
                case PrfAlgorithm.tls_prf_sha256:
                    return HashAlgorithm.sha256;
                case PrfAlgorithm.tls_prf_sha384:
                    return HashAlgorithm.sha384;
                default:
                    throw new ArgumentException("unknown PrfAlgorithm", "prfAlgorithm");
            }
        }

        public static DerObjectIdentifier GetOidForHashAlgorithm(byte hashAlgorithm)
        {
            switch (hashAlgorithm)
            {
                case HashAlgorithm.md5:
                    return PkcsObjectIdentifiers.MD5;
                case HashAlgorithm.sha1:
                    return X509ObjectIdentifiers.IdSha1;
                case HashAlgorithm.sha224:
                    return NistObjectIdentifiers.IdSha224;
                case HashAlgorithm.sha256:
                    return NistObjectIdentifiers.IdSha256;
                case HashAlgorithm.sha384:
                    return NistObjectIdentifiers.IdSha384;
                case HashAlgorithm.sha512:
                    return NistObjectIdentifiers.IdSha512;
                default:
                    throw new ArgumentException("unknown HashAlgorithm", "hashAlgorithm");
            }
        }

        internal static short GetClientCertificateType(Certificate clientCertificate, Certificate serverCertificate)
        {
            if (clientCertificate.IsEmpty)
                return -1;

            X509CertificateStructure x509Cert = clientCertificate.GetCertificateAt(0);
            SubjectPublicKeyInfo keyInfo = x509Cert.SubjectPublicKeyInfo;
            try
            {
                AsymmetricKeyParameter publicKey = PublicKeyFactory.CreateKey(keyInfo);
                if (publicKey.IsPrivate)
                    throw new TlsFatalAlert(AlertDescription.internal_error);

                /*
                 * TODO RFC 5246 7.4.6. The certificates MUST be signed using an acceptable hash/
                 * signature algorithm pair, as described in Section 7.4.4. Note that this relaxes the
                 * constraints on certificate-signing algorithms found in prior versions of TLS.
                 */

                /*
                 * RFC 5246 7.4.6. Client Certificate
                 */

                /*
                 * RSA public key; the certificate MUST allow the key to be used for signing with the
                 * signature scheme and hash algorithm that will be employed in the certificate verify
                 * message.
                 */
                if (publicKey is RsaKeyParameters)
                {
                    ValidateKeyUsage(x509Cert, KeyUsage.DigitalSignature);
                    return ClientCertificateType.rsa_sign;
                }

                /*
                 * DSA public key; the certificate MUST allow the key to be used for signing with the
                 * hash algorithm that will be employed in the certificate verify message.
                 */
                if (publicKey is DsaPublicKeyParameters)
                {
                    ValidateKeyUsage(x509Cert, KeyUsage.DigitalSignature);
                    return ClientCertificateType.dss_sign;
                }

                /*
                 * ECDSA-capable public key; the certificate MUST allow the key to be used for signing
                 * with the hash algorithm that will be employed in the certificate verify message; the
                 * public key MUST use a curve and point format supported by the server.
                 */
                if (publicKey is ECPublicKeyParameters)
                {
                    ValidateKeyUsage(x509Cert, KeyUsage.DigitalSignature);
                    // TODO Check the curve and point format
                    return ClientCertificateType.ecdsa_sign;
                }

                // TODO Add support for ClientCertificateType.*_fixed_*

                throw new TlsFatalAlert(AlertDescription.unsupported_certificate);
            }
            catch (Exception e)
            {
                throw new TlsFatalAlert(AlertDescription.unsupported_certificate, e);
            }
        }

        internal static void TrackHashAlgorithms(TlsHandshakeHash handshakeHash, IList supportedSignatureAlgorithms)
        {
            if (supportedSignatureAlgorithms != null)
            {
                foreach (SignatureAndHashAlgorithm signatureAndHashAlgorithm in supportedSignatureAlgorithms)
                {
                    byte hashAlgorithm = signatureAndHashAlgorithm.Hash;

                    // TODO Support values in the "Reserved for Private Use" range
                    if (!HashAlgorithm.IsPrivate(hashAlgorithm))
                    {
                        handshakeHash.TrackHashAlgorithm(hashAlgorithm);
                    }
                }
            }
        }

        public static bool HasSigningCapability(byte clientCertificateType)
        {
            switch (clientCertificateType)
            {
            case ClientCertificateType.dss_sign:
            case ClientCertificateType.ecdsa_sign:
            case ClientCertificateType.rsa_sign:
                return true;
            default:
                return false;
            }
        }

        public static TlsSigner CreateTlsSigner(byte clientCertificateType)
        {
            switch (clientCertificateType)
            {
            case ClientCertificateType.dss_sign:
                return new TlsDssSigner();
            case ClientCertificateType.ecdsa_sign:
                return new TlsECDsaSigner();
            case ClientCertificateType.rsa_sign:
                return new TlsRsaSigner();
            default:
                throw new ArgumentException("not a type with signing capability", "clientCertificateType");
            }
        }

        internal static readonly byte[] SSL_CLIENT = {0x43, 0x4C, 0x4E, 0x54};
        internal static readonly byte[] SSL_SERVER = {0x53, 0x52, 0x56, 0x52};

        // SSL3 magic mix constants ("A", "BB", "CCC", ...)
        internal static readonly byte[][] SSL3_CONST = GenSsl3Const();

        private static byte[][] GenSsl3Const()
        {
            int n = 10;
            byte[][] arr = new byte[n][];
            for (int i = 0; i < n; i++)
            {
                byte[] b = new byte[i + 1];
                Arrays.Fill(b, (byte)('A' + i));
                arr[i] = b;
            }
            return arr;
        }

        private static IList VectorOfOne(object obj)
        {
            IList v = Platform.CreateArrayList(1);
            v.Add(obj);
            return v;
        }

        public static int GetCipherType(int ciphersuite)
        {
            switch (GetEncryptionAlgorithm(ciphersuite))
            {
            case EncryptionAlgorithm.AES_128_CCM:
            case EncryptionAlgorithm.AES_128_CCM_8:
            case EncryptionAlgorithm.AES_128_GCM:
            case EncryptionAlgorithm.AES_128_OCB_TAGLEN96:
            case EncryptionAlgorithm.AES_256_CCM:
            case EncryptionAlgorithm.AES_256_CCM_8:
            case EncryptionAlgorithm.AES_256_GCM:
            case EncryptionAlgorithm.AES_256_OCB_TAGLEN96:
            case EncryptionAlgorithm.CAMELLIA_128_GCM:
            case EncryptionAlgorithm.CAMELLIA_256_GCM:
            case EncryptionAlgorithm.CHACHA20_POLY1305:
                return CipherType.aead;

            case EncryptionAlgorithm.RC2_CBC_40:
            case EncryptionAlgorithm.IDEA_CBC:
            case EncryptionAlgorithm.DES40_CBC:
            case EncryptionAlgorithm.DES_CBC:
            case EncryptionAlgorithm.cls_3DES_EDE_CBC:
            case EncryptionAlgorithm.AES_128_CBC:
            case EncryptionAlgorithm.AES_256_CBC:
            case EncryptionAlgorithm.CAMELLIA_128_CBC:
            case EncryptionAlgorithm.CAMELLIA_256_CBC:
            case EncryptionAlgorithm.SEED_CBC:
                return CipherType.block;

            case EncryptionAlgorithm.NULL:
            case EncryptionAlgorithm.RC4_40:
            case EncryptionAlgorithm.RC4_128:
                return CipherType.stream;

            default:
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        public static int GetEncryptionAlgorithm(int ciphersuite)
        {
            switch (ciphersuite)
            {
            case CipherSuite.TLS_DH_anon_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_PSK_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_RSA_PSK_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_DSS_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_WITH_3DES_EDE_CBC_SHA:
                return EncryptionAlgorithm.cls_3DES_EDE_CBC;

            case CipherSuite.TLS_DH_anon_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DH_anon_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_anon_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_WITH_AES_128_CBC_SHA:
                return EncryptionAlgorithm.AES_128_CBC;

            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_PSK_WITH_AES_128_CCM:
            case CipherSuite.TLS_RSA_WITH_AES_128_CCM:
                return EncryptionAlgorithm.AES_128_CCM;

            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_PSK_DHE_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_PSK_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_RSA_WITH_AES_128_CCM_8:
                return EncryptionAlgorithm.AES_128_CCM_8;

            case CipherSuite.TLS_DH_anon_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256:
                return EncryptionAlgorithm.AES_128_GCM;

            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_128_OCB:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_128_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_128_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_128_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_128_OCB:
            case CipherSuite.DRAFT_TLS_PSK_WITH_AES_128_OCB:
                return EncryptionAlgorithm.AES_128_OCB_TAGLEN96;

            case CipherSuite.TLS_DH_anon_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DH_anon_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_ECDH_anon_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_WITH_AES_256_CBC_SHA:
                return EncryptionAlgorithm.AES_256_CBC;

            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_PSK_WITH_AES_256_CCM:
            case CipherSuite.TLS_RSA_WITH_AES_256_CCM:
                return EncryptionAlgorithm.AES_256_CCM;

            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_PSK_DHE_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_PSK_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_RSA_WITH_AES_256_CCM_8:
                return EncryptionAlgorithm.AES_256_CCM_8;

            case CipherSuite.TLS_DH_anon_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DH_DSS_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DH_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384:
                return EncryptionAlgorithm.AES_256_GCM;

            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_256_OCB:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_256_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_256_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_256_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_256_OCB:
            case CipherSuite.DRAFT_TLS_PSK_WITH_AES_256_OCB:
                return EncryptionAlgorithm.AES_256_OCB_TAGLEN96;

            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_CBC_SHA256:
                return EncryptionAlgorithm.CAMELLIA_128_CBC;

            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_GCM_SHA256:
                return EncryptionAlgorithm.CAMELLIA_128_GCM;

            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_CBC_SHA384:
                return EncryptionAlgorithm.CAMELLIA_256_CBC;

            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_GCM_SHA384:
                return EncryptionAlgorithm.CAMELLIA_256_GCM;

            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.DRAFT_TLS_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.DRAFT_TLS_RSA_PSK_WITH_CHACHA20_POLY1305_SHA256:
                return EncryptionAlgorithm.CHACHA20_POLY1305;

            case CipherSuite.TLS_RSA_WITH_NULL_MD5:
                return EncryptionAlgorithm.NULL;

            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_NULL_SHA:
            case CipherSuite.TLS_PSK_WITH_NULL_SHA:
            case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA:
            case CipherSuite.TLS_RSA_WITH_NULL_SHA:
                return EncryptionAlgorithm.NULL;

            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA256:
            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA256:
            case CipherSuite.TLS_PSK_WITH_NULL_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA256:
            case CipherSuite.TLS_RSA_WITH_NULL_SHA256:
                return EncryptionAlgorithm.NULL;

            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA384:
            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA384:
            case CipherSuite.TLS_PSK_WITH_NULL_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA384:
                return EncryptionAlgorithm.NULL;

            case CipherSuite.TLS_DH_anon_WITH_RC4_128_MD5:
            case CipherSuite.TLS_RSA_WITH_RC4_128_MD5:
                return EncryptionAlgorithm.RC4_128;

            case CipherSuite.TLS_DHE_PSK_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_PSK_WITH_RC4_128_SHA:
            case CipherSuite.TLS_RSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_RSA_PSK_WITH_RC4_128_SHA:
                return EncryptionAlgorithm.RC4_128;

            case CipherSuite.TLS_DH_anon_WITH_SEED_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_SEED_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_SEED_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_SEED_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_SEED_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_SEED_CBC_SHA:
                return EncryptionAlgorithm.SEED_CBC;

            default:
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        public static int GetKeyExchangeAlgorithm(int ciphersuite)
        {
            switch (ciphersuite)
            {
            case CipherSuite.TLS_DH_anon_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DH_anon_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DH_anon_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DH_anon_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DH_anon_WITH_RC4_128_MD5:
            case CipherSuite.TLS_DH_anon_WITH_SEED_CBC_SHA:
                return KeyExchangeAlgorithm.DH_anon;

            case CipherSuite.TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DH_DSS_WITH_SEED_CBC_SHA:
                return KeyExchangeAlgorithm.DH_DSS;

            case CipherSuite.TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DH_RSA_WITH_SEED_CBC_SHA:
                return KeyExchangeAlgorithm.DH_RSA;

            case CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_DSS_WITH_SEED_CBC_SHA:
                return KeyExchangeAlgorithm.DHE_DSS;

            case CipherSuite.TLS_DHE_PSK_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_128_OCB:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_256_OCB:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_RC4_128_SHA:
            case CipherSuite.TLS_PSK_DHE_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_PSK_DHE_WITH_AES_256_CCM_8:
                return KeyExchangeAlgorithm.DHE_PSK;

            case CipherSuite.TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_128_OCB:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_256_OCB:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_SEED_CBC_SHA:
                return KeyExchangeAlgorithm.DHE_RSA;

            case CipherSuite.TLS_ECDH_anon_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_RC4_128_SHA:
                return KeyExchangeAlgorithm.ECDH_anon;

            case CipherSuite.TLS_ECDH_ECDSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_128_OCB:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_256_OCB:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_RC4_128_SHA:
                return KeyExchangeAlgorithm.ECDH_ECDSA;

            case CipherSuite.TLS_ECDH_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_RC4_128_SHA:
                return KeyExchangeAlgorithm.ECDH_RSA;

            case CipherSuite.TLS_ECDHE_ECDSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_RC4_128_SHA:
                return KeyExchangeAlgorithm.ECDHE_ECDSA;

            case CipherSuite.TLS_ECDHE_PSK_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_128_OCB:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_256_OCB:
            case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA256:
            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA384:
            case CipherSuite.TLS_ECDHE_PSK_WITH_RC4_128_SHA:
                return KeyExchangeAlgorithm.ECDHE_PSK;

            case CipherSuite.TLS_ECDHE_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_128_OCB:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_256_OCB:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_RC4_128_SHA:
                return KeyExchangeAlgorithm.ECDHE_RSA;

            case CipherSuite.TLS_PSK_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_PSK_WITH_AES_128_CCM:
            case CipherSuite.TLS_PSK_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_PSK_WITH_AES_128_OCB:
            case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_PSK_WITH_AES_256_CCM:
            case CipherSuite.TLS_PSK_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_PSK_WITH_AES_256_OCB:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_PSK_WITH_NULL_SHA:
            case CipherSuite.TLS_PSK_WITH_NULL_SHA256:
            case CipherSuite.TLS_PSK_WITH_NULL_SHA384:
            case CipherSuite.TLS_PSK_WITH_RC4_128_SHA:
                return KeyExchangeAlgorithm.PSK;

            case CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_RSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_RSA_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_RSA_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_RSA_WITH_NULL_MD5:
            case CipherSuite.TLS_RSA_WITH_NULL_SHA:
            case CipherSuite.TLS_RSA_WITH_NULL_SHA256:
            case CipherSuite.TLS_RSA_WITH_RC4_128_MD5:
            case CipherSuite.TLS_RSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_RSA_WITH_SEED_CBC_SHA:
                return KeyExchangeAlgorithm.RSA;

            case CipherSuite.TLS_RSA_PSK_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA:
            case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_RC4_128_SHA:
                return KeyExchangeAlgorithm.RSA_PSK;

            case CipherSuite.TLS_SRP_SHA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_WITH_AES_256_CBC_SHA:
                return KeyExchangeAlgorithm.SRP;

            case CipherSuite.TLS_SRP_SHA_DSS_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_256_CBC_SHA:
                return KeyExchangeAlgorithm.SRP_DSS;

            case CipherSuite.TLS_SRP_SHA_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_256_CBC_SHA:
                return KeyExchangeAlgorithm.SRP_RSA;

            default:
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        public static int GetMacAlgorithm(int ciphersuite)
        {
            switch (ciphersuite)
            {
            case CipherSuite.TLS_DH_anon_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DH_DSS_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DH_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_128_OCB:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_256_OCB:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_128_OCB:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_256_OCB:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_128_OCB:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_256_OCB:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_128_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_256_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_128_OCB:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_256_OCB:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_PSK_DHE_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_PSK_DHE_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_PSK_WITH_AES_128_CCM:
            case CipherSuite.TLS_PSK_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_PSK_WITH_AES_128_OCB:
            case CipherSuite.TLS_PSK_WITH_AES_256_CCM:
            case CipherSuite.TLS_PSK_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_PSK_WITH_AES_256_OCB:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_RSA_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_RSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_RSA_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_GCM_SHA384:
                return MacAlgorithm.cls_null;

            case CipherSuite.TLS_DH_anon_WITH_RC4_128_MD5:
            case CipherSuite.TLS_RSA_WITH_NULL_MD5:
            case CipherSuite.TLS_RSA_WITH_RC4_128_MD5:
                return MacAlgorithm.hmac_md5;

            case CipherSuite.TLS_DH_anon_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DH_anon_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DH_anon_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DH_anon_WITH_SEED_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DH_DSS_WITH_SEED_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DH_RSA_WITH_SEED_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DHE_DSS_WITH_SEED_CBC_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA:
            case CipherSuite.TLS_DHE_PSK_WITH_RC4_128_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_DHE_RSA_WITH_SEED_CBC_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDH_anon_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDH_RSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDHE_PSK_WITH_RC4_128_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_NULL_SHA:
            case CipherSuite.TLS_ECDHE_RSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_RSA_PSK_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_PSK_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_PSK_WITH_NULL_SHA:
            case CipherSuite.TLS_PSK_WITH_RC4_128_SHA:
            case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA:
            case CipherSuite.TLS_RSA_PSK_WITH_RC4_128_SHA:
            case CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA:
            case CipherSuite.TLS_RSA_WITH_NULL_SHA:
            case CipherSuite.TLS_RSA_WITH_RC4_128_SHA:
            case CipherSuite.TLS_RSA_WITH_SEED_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_DSS_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_RSA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_256_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_WITH_3DES_EDE_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_WITH_AES_128_CBC_SHA:
            case CipherSuite.TLS_SRP_SHA_WITH_AES_256_CBC_SHA:
                return MacAlgorithm.hmac_sha1;

            case CipherSuite.TLS_DH_anon_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_PSK_WITH_NULL_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_NULL_SHA256:
                return MacAlgorithm.hmac_sha256;

            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_PSK_WITH_NULL_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA384:
                return MacAlgorithm.hmac_sha384;

            default:
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        public static ProtocolVersion GetMinimumVersion(int ciphersuite)
        {
            switch (ciphersuite)
            {
            case CipherSuite.TLS_DH_anon_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_128_OCB:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM:
            case CipherSuite.TLS_DHE_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_AES_256_OCB:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_128_OCB:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_AES_256_OCB:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_128_OCB:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_AES_256_OCB:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_128_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_AES_256_OCB:
            case CipherSuite.DRAFT_TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_128_OCB:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_AES_256_OCB:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_CBC_SHA384:
            case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_PSK_DHE_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_PSK_DHE_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_PSK_WITH_AES_128_CCM:
            case CipherSuite.TLS_PSK_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.DRAFT_TLS_PSK_WITH_AES_128_OCB:
            case CipherSuite.TLS_PSK_WITH_AES_256_CCM:
            case CipherSuite.TLS_PSK_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_PSK_WITH_AES_256_OCB:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.DRAFT_TLS_RSA_PSK_WITH_CHACHA20_POLY1305_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_128_CCM:
            case CipherSuite.TLS_RSA_WITH_AES_128_CCM_8:
            case CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_AES_256_CCM:
            case CipherSuite.TLS_RSA_WITH_AES_256_CCM_8:
            case CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_GCM_SHA256:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA256:
            case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_GCM_SHA384:
            case CipherSuite.TLS_RSA_WITH_NULL_SHA256:
                return ProtocolVersion.TLSv12;

            default:
                return ProtocolVersion.SSLv3;
            }
        }

        public static bool IsAeadCipherSuite(int ciphersuite)
        {
            return CipherType.aead == GetCipherType(ciphersuite);
        }

        public static bool IsBlockCipherSuite(int ciphersuite)
        {
            return CipherType.block == GetCipherType(ciphersuite);
        }

        public static bool IsStreamCipherSuite(int ciphersuite)
        {
            return CipherType.stream == GetCipherType(ciphersuite);
        }

        public static bool IsValidCipherSuiteForSignatureAlgorithms(int cipherSuite, IList sigAlgs)
        {
            int keyExchangeAlgorithm;
            try
            {
                keyExchangeAlgorithm = GetKeyExchangeAlgorithm(cipherSuite);
            }
            catch (IOException e)
            {
                return true;
            }

            switch (keyExchangeAlgorithm)
            {
            case KeyExchangeAlgorithm.DH_anon:
            case KeyExchangeAlgorithm.DH_anon_EXPORT:
            case KeyExchangeAlgorithm.ECDH_anon:
                return sigAlgs.Contains(SignatureAlgorithm.anonymous);

            case KeyExchangeAlgorithm.DHE_RSA:
            case KeyExchangeAlgorithm.DHE_RSA_EXPORT:
            case KeyExchangeAlgorithm.ECDHE_RSA:
            case KeyExchangeAlgorithm.SRP_RSA:
                return sigAlgs.Contains(SignatureAlgorithm.rsa);

            case KeyExchangeAlgorithm.DHE_DSS:
            case KeyExchangeAlgorithm.DHE_DSS_EXPORT:
            case KeyExchangeAlgorithm.SRP_DSS:
                return sigAlgs.Contains(SignatureAlgorithm.dsa);

            case KeyExchangeAlgorithm.ECDHE_ECDSA:
                return sigAlgs.Contains(SignatureAlgorithm.ecdsa);

            default:
                return true;
            }
        }

        public static bool IsValidCipherSuiteForVersion(int cipherSuite, ProtocolVersion serverVersion)
        {
            return GetMinimumVersion(cipherSuite).IsEqualOrEarlierVersionOf(serverVersion.GetEquivalentTLSVersion());
        }

        public static IList GetUsableSignatureAlgorithms(IList sigHashAlgs)
        {
            if (sigHashAlgs == null)
                return GetAllSignatureAlgorithms();

            IList v = Platform.CreateArrayList(4);
            v.Add(SignatureAlgorithm.anonymous);
            foreach (SignatureAndHashAlgorithm sigHashAlg in sigHashAlgs)
            {
                //if (sigHashAlg.Hash >= MINIMUM_HASH_STRICT)
                {
                    byte sigAlg = sigHashAlg.Signature;
                    if (!v.Contains(sigAlg))
                    {
                        v.Add(sigAlg);
                    }
                }
            }
            return v;
        }
    }
}
