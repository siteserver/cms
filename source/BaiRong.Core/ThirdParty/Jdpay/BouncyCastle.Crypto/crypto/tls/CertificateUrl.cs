using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    /*
     * RFC 3546 3.3
     */
    public class CertificateUrl
    {
        protected readonly byte mType;
        protected readonly IList mUrlAndHashList;

        /**
         * @param type
         *            see {@link CertChainType} for valid constants.
         * @param urlAndHashList
         *            a {@link IList} of {@link UrlAndHash}.
         */
        public CertificateUrl(byte type, IList urlAndHashList)
        {
            if (!CertChainType.IsValid(type))
                throw new ArgumentException("not a valid CertChainType value", "type");
            if (urlAndHashList == null || urlAndHashList.Count < 1)
                throw new ArgumentException("must have length > 0", "urlAndHashList");

            this.mType = type;
            this.mUrlAndHashList = urlAndHashList;
        }

        /**
         * @return {@link CertChainType}
         */
        public virtual byte Type
        {
            get { return mType; }
        }

        /**
         * @return an {@link IList} of {@link UrlAndHash} 
         */
        public virtual IList UrlAndHashList
        {
            get { return mUrlAndHashList; }
        }

        /**
         * Encode this {@link CertificateUrl} to a {@link Stream}.
         *
         * @param output the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            TlsUtilities.WriteUint8(this.mType, output);

            ListBuffer16 buf = new ListBuffer16();
            foreach (UrlAndHash urlAndHash in this.mUrlAndHashList)
            {
                urlAndHash.Encode(buf);
            }
            buf.EncodeTo(output);
        }

        /**
         * Parse a {@link CertificateUrl} from a {@link Stream}.
         * 
         * @param context
         *            the {@link TlsContext} of the current connection.
         * @param input
         *            the {@link Stream} to parse from.
         * @return a {@link CertificateUrl} object.
         * @throws IOException
         */
        public static CertificateUrl parse(TlsContext context, Stream input)
        {
            byte type = TlsUtilities.ReadUint8(input);
            if (!CertChainType.IsValid(type))
                throw new TlsFatalAlert(AlertDescription.decode_error);

            int totalLength = TlsUtilities.ReadUint16(input);
            if (totalLength < 1)
                throw new TlsFatalAlert(AlertDescription.decode_error);

            byte[] urlAndHashListData = TlsUtilities.ReadFully(totalLength, input);

            MemoryStream buf = new MemoryStream(urlAndHashListData, false);

            IList url_and_hash_list = Platform.CreateArrayList();
            while (buf.Position < buf.Length)
            {
                UrlAndHash url_and_hash = UrlAndHash.Parse(context, buf);
                url_and_hash_list.Add(url_and_hash);
            }

            return new CertificateUrl(type, url_and_hash_list);
        }

        // TODO Could be more generally useful
        internal class ListBuffer16
            : MemoryStream
        {
            internal ListBuffer16()
            {
                // Reserve space for length
                TlsUtilities.WriteUint16(0,  this);
            }

            internal void EncodeTo(Stream output)
            {
                // Patch actual length back in
                long length = Length - 2;
                TlsUtilities.CheckUint16(length);
                this.Position = 0;
                TlsUtilities.WriteUint16((int)length, this);
                Streams.WriteBufTo(this, output);
                Platform.Dispose(this);
            }
        }
    }
}
