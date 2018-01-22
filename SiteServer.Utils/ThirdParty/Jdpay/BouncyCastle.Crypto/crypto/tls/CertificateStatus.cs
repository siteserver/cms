using System;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class CertificateStatus
    {
        protected readonly byte mStatusType;
        protected readonly object mResponse;

        public CertificateStatus(byte statusType, object response)
        {
            if (!IsCorrectType(statusType, response))
                throw new ArgumentException("not an instance of the correct type", "response");

            this.mStatusType = statusType;
            this.mResponse = response;
        }

        public virtual byte StatusType
        {
            get { return mStatusType; }
        }

        public virtual object Response
        {
            get { return mResponse; }
        }

        public virtual OcspResponse GetOcspResponse()
        {
            if (!IsCorrectType(CertificateStatusType.ocsp, mResponse))
                throw new InvalidOperationException("'response' is not an OcspResponse");

            return (OcspResponse)mResponse;
        }

        /**
         * Encode this {@link CertificateStatus} to a {@link Stream}.
         * 
         * @param output
         *            the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            TlsUtilities.WriteUint8(mStatusType, output);

            switch (mStatusType)
            {
            case CertificateStatusType.ocsp:
                byte[] derEncoding = ((OcspResponse)mResponse).GetEncoded(Asn1Encodable.Der);
                TlsUtilities.WriteOpaque24(derEncoding, output);
                break;
            default:
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        /**
         * Parse a {@link CertificateStatus} from a {@link Stream}.
         * 
         * @param input
         *            the {@link Stream} to parse from.
         * @return a {@link CertificateStatus} object.
         * @throws IOException
         */
        public static CertificateStatus Parse(Stream input)
        {
            byte status_type = TlsUtilities.ReadUint8(input);
            object response;

            switch (status_type)
            {
            case CertificateStatusType.ocsp:
            {
                byte[] derEncoding = TlsUtilities.ReadOpaque24(input);
                response = OcspResponse.GetInstance(TlsUtilities.ReadDerObject(derEncoding));
                break;
            }
            default:
                throw new TlsFatalAlert(AlertDescription.decode_error);
            }

            return new CertificateStatus(status_type, response);
        }

        protected static bool IsCorrectType(byte statusType, object response)
        {
            switch (statusType)
            {
            case CertificateStatusType.ocsp:
                return response is OcspResponse;
            default:
                throw new ArgumentException("unsupported CertificateStatusType", "statusType");
            }
        }
    }
}
