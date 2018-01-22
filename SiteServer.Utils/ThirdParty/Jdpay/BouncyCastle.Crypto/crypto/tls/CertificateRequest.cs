using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    /**
     * Parsing and encoding of a <i>CertificateRequest</i> struct from RFC 4346.
     * <p/>
     * <pre>
     * struct {
     *     ClientCertificateType certificate_types&lt;1..2^8-1&gt;;
     *     DistinguishedName certificate_authorities&lt;3..2^16-1&gt;
     * } CertificateRequest;
     * </pre>
     *
     * @see ClientCertificateType
     * @see X509Name
     */
    public class CertificateRequest
    {
        protected readonly byte[] mCertificateTypes;
        protected readonly IList mSupportedSignatureAlgorithms;
        protected readonly IList mCertificateAuthorities;

        /**
         * @param certificateTypes       see {@link ClientCertificateType} for valid constants.
         * @param certificateAuthorities an {@link IList} of {@link X509Name}.
         */
        public CertificateRequest(byte[] certificateTypes, IList supportedSignatureAlgorithms,
            IList certificateAuthorities)
        {
            this.mCertificateTypes = certificateTypes;
            this.mSupportedSignatureAlgorithms = supportedSignatureAlgorithms;
            this.mCertificateAuthorities = certificateAuthorities;
        }

        /**
         * @return an array of certificate types
         * @see {@link ClientCertificateType}
         */
        public virtual byte[] CertificateTypes
        {
            get { return mCertificateTypes; }
        }

        /**
         * @return an {@link IList} of {@link SignatureAndHashAlgorithm} (or null before TLS 1.2).
         */
        public virtual IList SupportedSignatureAlgorithms
        {
            get { return mSupportedSignatureAlgorithms; }
        }

        /**
         * @return an {@link IList} of {@link X509Name}
         */
        public virtual IList CertificateAuthorities
        {
            get { return mCertificateAuthorities; }
        }

        /**
         * Encode this {@link CertificateRequest} to a {@link Stream}.
         *
         * @param output the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            if (mCertificateTypes == null || mCertificateTypes.Length == 0)
            {
                TlsUtilities.WriteUint8(0, output);
            }
            else
            {
                TlsUtilities.WriteUint8ArrayWithUint8Length(mCertificateTypes, output);
            }

            if (mSupportedSignatureAlgorithms != null)
            {
                // TODO Check whether SignatureAlgorithm.anonymous is allowed here
                TlsUtilities.EncodeSupportedSignatureAlgorithms(mSupportedSignatureAlgorithms, false, output);
            }

            if (mCertificateAuthorities == null || mCertificateAuthorities.Count < 1)
            {
                TlsUtilities.WriteUint16(0, output);
            }
            else
            {
                IList derEncodings = Platform.CreateArrayList(mCertificateAuthorities.Count);

                int totalLength = 0;
                foreach (Asn1Encodable certificateAuthority in mCertificateAuthorities)
                {
                    byte[] derEncoding = certificateAuthority.GetEncoded(Asn1Encodable.Der);
                    derEncodings.Add(derEncoding);
                    totalLength += derEncoding.Length + 2;
                }

                TlsUtilities.CheckUint16(totalLength);
                TlsUtilities.WriteUint16(totalLength, output);

                foreach (byte[] derEncoding in derEncodings)
                {
                    TlsUtilities.WriteOpaque16(derEncoding, output);
                }
            }
        }

        /**
         * Parse a {@link CertificateRequest} from a {@link Stream}.
         * 
         * @param context
         *            the {@link TlsContext} of the current connection.
         * @param input
         *            the {@link Stream} to parse from.
         * @return a {@link CertificateRequest} object.
         * @throws IOException
         */
        public static CertificateRequest Parse(TlsContext context, Stream input)
        {
            int numTypes = TlsUtilities.ReadUint8(input);
            byte[] certificateTypes = new byte[numTypes];
            for (int i = 0; i < numTypes; ++i)
            {
                certificateTypes[i] = TlsUtilities.ReadUint8(input);
            }

            IList supportedSignatureAlgorithms = null;
            if (TlsUtilities.IsTlsV12(context))
            {
                // TODO Check whether SignatureAlgorithm.anonymous is allowed here
                supportedSignatureAlgorithms = TlsUtilities.ParseSupportedSignatureAlgorithms(false, input);
            }

            IList certificateAuthorities = Platform.CreateArrayList();
            byte[] certAuthData = TlsUtilities.ReadOpaque16(input);
            MemoryStream bis = new MemoryStream(certAuthData, false);
            while (bis.Position < bis.Length)
            {
                byte[] derEncoding = TlsUtilities.ReadOpaque16(bis);
                Asn1Object asn1 = TlsUtilities.ReadDerObject(derEncoding);
                // TODO Switch to X500Name when available
                certificateAuthorities.Add(X509Name.GetInstance(asn1));
            }

            return new CertificateRequest(certificateTypes, supportedSignatureAlgorithms, certificateAuthorities);
        }
    }
}
