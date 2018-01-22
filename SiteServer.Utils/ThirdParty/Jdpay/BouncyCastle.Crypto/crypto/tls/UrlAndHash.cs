using System;
using System.IO;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    /**
     * RFC 6066 5.
     */
    public class UrlAndHash
    {
        protected readonly string mUrl;
        protected readonly byte[] mSha1Hash;

        public UrlAndHash(string url, byte[] sha1Hash)
        {
            if (url == null || url.Length < 1 || url.Length >= (1 << 16))
                throw new ArgumentException("must have length from 1 to (2^16 - 1)", "url");
            if (sha1Hash != null && sha1Hash.Length != 20)
                throw new ArgumentException("must have length == 20, if present", "sha1Hash");

            this.mUrl = url;
            this.mSha1Hash = sha1Hash;
        }

        public virtual string Url
        {
            get { return mUrl; }
        }

        public virtual byte[] Sha1Hash
        {
            get { return mSha1Hash; }
        }

        /**
         * Encode this {@link UrlAndHash} to a {@link Stream}.
         *
         * @param output the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            byte[] urlEncoding = Strings.ToByteArray(this.mUrl);
            TlsUtilities.WriteOpaque16(urlEncoding, output);

            if (this.mSha1Hash == null)
            {
                TlsUtilities.WriteUint8(0, output);
            }
            else
            {
                TlsUtilities.WriteUint8(1, output);
                output.Write(this.mSha1Hash, 0, this.mSha1Hash.Length);
            }
        }

        /**
         * Parse a {@link UrlAndHash} from a {@link Stream}.
         * 
         * @param context
         *            the {@link TlsContext} of the current connection.
         * @param input
         *            the {@link Stream} to parse from.
         * @return a {@link UrlAndHash} object.
         * @throws IOException
         */
        public static UrlAndHash Parse(TlsContext context, Stream input)
        {
            byte[] urlEncoding = TlsUtilities.ReadOpaque16(input);
            if (urlEncoding.Length < 1)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            string url = Strings.FromByteArray(urlEncoding);

            byte[] sha1Hash = null;
            byte padding = TlsUtilities.ReadUint8(input);
            switch (padding)
            {
            case 0:
                if (TlsUtilities.IsTlsV12(context))
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
                break;
            case 1:
                sha1Hash = TlsUtilities.ReadFully(20, input);
                break;
            default:
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }

            return new UrlAndHash(url, sha1Hash);
        }
    }
}
