using System;
using System.IO;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public sealed class ProtocolVersion
    {
        public static readonly ProtocolVersion SSLv3 = new ProtocolVersion(0x0300, "SSL 3.0");
        public static readonly ProtocolVersion TLSv10 = new ProtocolVersion(0x0301, "TLS 1.0");
        public static readonly ProtocolVersion TLSv11 = new ProtocolVersion(0x0302, "TLS 1.1");
        public static readonly ProtocolVersion TLSv12 = new ProtocolVersion(0x0303, "TLS 1.2");
        public static readonly ProtocolVersion DTLSv10 = new ProtocolVersion(0xFEFF, "DTLS 1.0");
        public static readonly ProtocolVersion DTLSv12 = new ProtocolVersion(0xFEFD, "DTLS 1.2");

        private readonly int version;
        private readonly String name;

        private ProtocolVersion(int v, String name)
        {
            this.version = v & 0xffff;
            this.name = name;
        }

        public int FullVersion
        {
            get { return version; }
        }

        public int MajorVersion
        {
            get { return version >> 8; }
        }

        public int MinorVersion
        {
            get { return version & 0xff; }
        }

        public bool IsDtls
        {
            get { return MajorVersion == 0xFE; }
        }

        public bool IsSsl
        {
            get { return this == SSLv3; }
        }

        public bool IsTls
        {
            get { return MajorVersion == 0x03; }
        }

        public ProtocolVersion GetEquivalentTLSVersion()
        {
            if (!IsDtls)
            {
                return this;
            }
            if (this == DTLSv10)
            {
                return TLSv11;
            }
            return TLSv12;
        }

        public bool IsEqualOrEarlierVersionOf(ProtocolVersion version)
        {
            if (MajorVersion != version.MajorVersion)
            {
                return false;
            }
            int diffMinorVersion = version.MinorVersion - MinorVersion;
            return IsDtls ? diffMinorVersion <= 0 : diffMinorVersion >= 0;
        }

        public bool IsLaterVersionOf(ProtocolVersion version)
        {
            if (MajorVersion != version.MajorVersion)
            {
                return false;
            }
            int diffMinorVersion = version.MinorVersion - MinorVersion;
            return IsDtls ? diffMinorVersion > 0 : diffMinorVersion < 0;
        }

        public override bool Equals(object other)
        {
            return this == other || (other is ProtocolVersion && Equals((ProtocolVersion)other));
        }

        public bool Equals(ProtocolVersion other)
        {
            return other != null && this.version == other.version;
        }

        public override int GetHashCode()
        {
            return version;
        }

        /// <exception cref="IOException"/>
        public static ProtocolVersion Get(int major, int minor)
        {
            switch (major)
            {
                case 0x03:
                {
                    switch (minor)
                    {
                        case 0x00:
                            return SSLv3;
                        case 0x01:
                            return TLSv10;
                        case 0x02:
                            return TLSv11;
                        case 0x03:
                            return TLSv12;
                    }
                    return GetUnknownVersion(major, minor, "TLS");
                }
                case 0xFE:
                {
                    switch (minor)
                    {
                        case 0xFF:
                            return DTLSv10;
                        case 0xFE:
                            throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                        case 0xFD:
                            return DTLSv12;
                    }
                    return GetUnknownVersion(major, minor, "DTLS");
                }
                default:
                {
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                }
            }
        }

        public override string ToString()
        {
            return name;
        }

        private static ProtocolVersion GetUnknownVersion(int major, int minor, string prefix)
        {
            TlsUtilities.CheckUint8(major);
            TlsUtilities.CheckUint8(minor);

            int v = (major << 8) | minor;
            String hex = Platform.ToUpperInvariant(Convert.ToString(0x10000 | v, 16).Substring(1));
            return new ProtocolVersion(v, prefix + " 0x" + hex);
        }
    }
}
