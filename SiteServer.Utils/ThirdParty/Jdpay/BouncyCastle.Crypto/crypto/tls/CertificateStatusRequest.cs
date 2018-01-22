using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class CertificateStatusRequest
    {
        protected readonly byte mStatusType;
        protected readonly object mRequest;

        public CertificateStatusRequest(byte statusType, Object request)
        {
            if (!IsCorrectType(statusType, request))
                throw new ArgumentException("not an instance of the correct type", "request");

            this.mStatusType = statusType;
            this.mRequest = request;
        }

        public virtual byte StatusType
        {
            get { return mStatusType; }
        }

        public virtual object Request
        {
            get { return mRequest; }
        }

        public virtual OcspStatusRequest GetOcspStatusRequest()
        {
            if (!IsCorrectType(CertificateStatusType.ocsp, mRequest))
                throw new InvalidOperationException("'request' is not an OCSPStatusRequest");

            return (OcspStatusRequest)mRequest;
        }

        /**
         * Encode this {@link CertificateStatusRequest} to a {@link Stream}.
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
                ((OcspStatusRequest)mRequest).Encode(output);
                break;
            default:
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        /**
         * Parse a {@link CertificateStatusRequest} from a {@link Stream}.
         * 
         * @param input
         *            the {@link Stream} to parse from.
         * @return a {@link CertificateStatusRequest} object.
         * @throws IOException
         */
        public static CertificateStatusRequest Parse(Stream input)
        {
            byte status_type = TlsUtilities.ReadUint8(input);
            object result;

            switch (status_type)
            {
            case CertificateStatusType.ocsp:
                result = OcspStatusRequest.Parse(input);
                break;
            default:
                throw new TlsFatalAlert(AlertDescription.decode_error);
            }

            return new CertificateStatusRequest(status_type, result);
        }

        protected static bool IsCorrectType(byte statusType, object request)
        {
            switch (statusType)
            {
            case CertificateStatusType.ocsp:
                return request is OcspStatusRequest;
            default:
                throw new ArgumentException("unsupported CertificateStatusType", "statusType");
            }
        }
    }
}
