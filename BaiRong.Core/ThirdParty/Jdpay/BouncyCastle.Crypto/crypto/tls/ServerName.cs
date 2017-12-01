using System;
using System.IO;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class ServerName
    {
        protected readonly byte mNameType;
        protected readonly object mName;

        public ServerName(byte nameType, object name)
        {
            if (!IsCorrectType(nameType, name))
                throw new ArgumentException("not an instance of the correct type", "name");

            this.mNameType = nameType;
            this.mName = name;
        }

        public virtual byte NameType
        {
            get { return mNameType; }
        }

        public virtual object Name
        {
            get { return mName; }
        }

        public virtual string GetHostName()
        {
            if (!IsCorrectType(Tls.NameType.host_name, mName))
                throw new InvalidOperationException("'name' is not a HostName string");

            return (string)mName;
        }

        /**
         * Encode this {@link ServerName} to a {@link Stream}.
         * 
         * @param output
         *            the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            TlsUtilities.WriteUint8(mNameType, output);

            switch (mNameType)
            {
            case Tls.NameType.host_name:
                byte[] asciiEncoding = Strings.ToAsciiByteArray((string)mName);
                if (asciiEncoding.Length < 1)
                    throw new TlsFatalAlert(AlertDescription.internal_error);
                TlsUtilities.WriteOpaque16(asciiEncoding, output);
                break;
            default:
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        /**
         * Parse a {@link ServerName} from a {@link Stream}.
         * 
         * @param input
         *            the {@link Stream} to parse from.
         * @return a {@link ServerName} object.
         * @throws IOException
         */
        public static ServerName Parse(Stream input)
        {
            byte name_type = TlsUtilities.ReadUint8(input);
            object name;

            switch (name_type)
            {
            case Tls.NameType.host_name:
            {
                byte[] asciiEncoding = TlsUtilities.ReadOpaque16(input);
                if (asciiEncoding.Length < 1)
                    throw new TlsFatalAlert(AlertDescription.decode_error);
                name = Strings.FromAsciiByteArray(asciiEncoding);
                break;
            }
            default:
                throw new TlsFatalAlert(AlertDescription.decode_error);
            }

            return new ServerName(name_type, name);
        }

        protected static bool IsCorrectType(byte nameType, object name)
        {
            switch (nameType)
            {
            case Tls.NameType.host_name:
                return name is string;
            default:
                throw new ArgumentException("unsupported NameType", "nameType");
            }
        }
    }
}
