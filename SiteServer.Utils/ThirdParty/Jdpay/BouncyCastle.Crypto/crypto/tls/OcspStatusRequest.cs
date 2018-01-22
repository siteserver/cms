using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    /**
     * RFC 3546 3.6
     */
    public class OcspStatusRequest
    {
        protected readonly IList mResponderIDList;
        protected readonly X509Extensions mRequestExtensions;

        /**
         * @param responderIDList
         *            an {@link IList} of {@link ResponderID}, specifying the list of trusted OCSP
         *            responders. An empty list has the special meaning that the responders are
         *            implicitly known to the server - e.g., by prior arrangement.
         * @param requestExtensions
         *            OCSP request extensions. A null value means that there are no extensions.
         */
        public OcspStatusRequest(IList responderIDList, X509Extensions requestExtensions)
        {
            this.mResponderIDList = responderIDList;
            this.mRequestExtensions = requestExtensions;
        }

        /**
         * @return an {@link IList} of {@link ResponderID}
         */
        public virtual IList ResponderIDList
        {
            get { return mResponderIDList; }
        }

        /**
         * @return OCSP request extensions
         */
        public virtual X509Extensions RequestExtensions
        {
            get { return mRequestExtensions; }
        }

        /**
         * Encode this {@link OcspStatusRequest} to a {@link Stream}.
         * 
         * @param output
         *            the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            if (mResponderIDList == null || mResponderIDList.Count < 1)
            {
                TlsUtilities.WriteUint16(0, output);
            }
            else
            {
                MemoryStream buf = new MemoryStream();
                for (int i = 0; i < mResponderIDList.Count; ++i)
                {
                    ResponderID responderID = (ResponderID)mResponderIDList[i];
                    byte[] derEncoding = responderID.GetEncoded(Asn1Encodable.Der);
                    TlsUtilities.WriteOpaque16(derEncoding, buf);
                }
                TlsUtilities.CheckUint16(buf.Length);
                TlsUtilities.WriteUint16((int)buf.Length, output);
                Streams.WriteBufTo(buf, output);
            }

            if (mRequestExtensions == null)
            {
                TlsUtilities.WriteUint16(0, output);
            }
            else
            {
                byte[] derEncoding = mRequestExtensions.GetEncoded(Asn1Encodable.Der);
                TlsUtilities.CheckUint16(derEncoding.Length);
                TlsUtilities.WriteUint16(derEncoding.Length, output);
                output.Write(derEncoding, 0, derEncoding.Length);
            }
        }

        /**
         * Parse a {@link OcspStatusRequest} from a {@link Stream}.
         * 
         * @param input
         *            the {@link Stream} to parse from.
         * @return an {@link OcspStatusRequest} object.
         * @throws IOException
         */
        public static OcspStatusRequest Parse(Stream input)
        {
            IList responderIDList = Platform.CreateArrayList();
            {
                int length = TlsUtilities.ReadUint16(input);
                if (length > 0)
                {
                    byte[] data = TlsUtilities.ReadFully(length, input);
                    MemoryStream buf = new MemoryStream(data, false);
                    do
                    {
                        byte[] derEncoding = TlsUtilities.ReadOpaque16(buf);
                        ResponderID responderID = ResponderID.GetInstance(TlsUtilities.ReadDerObject(derEncoding));
                        responderIDList.Add(responderID);
                    }
                    while (buf.Position < buf.Length);
                }
            }

            X509Extensions requestExtensions = null;
            {
                int length = TlsUtilities.ReadUint16(input);
                if (length > 0)
                {
                    byte[] derEncoding = TlsUtilities.ReadFully(length, input);
                    requestExtensions = X509Extensions.GetInstance(TlsUtilities.ReadDerObject(derEncoding));
                }
            }

            return new OcspStatusRequest(responderIDList, requestExtensions);
        }
    }
}
