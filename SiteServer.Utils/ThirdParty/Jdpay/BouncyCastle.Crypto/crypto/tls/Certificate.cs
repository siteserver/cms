using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    /**
     * Parsing and encoding of a <i>Certificate</i> struct from RFC 4346.
     * <p/>
     * <pre>
     * opaque ASN.1Cert&lt;2^24-1&gt;;
     *
     * struct {
     *     ASN.1Cert certificate_list&lt;0..2^24-1&gt;;
     * } Certificate;
     * </pre>
     *
     * @see Org.BouncyCastle.Asn1.X509.X509CertificateStructure
     */
    public class Certificate
    {
        public static readonly Certificate EmptyChain = new Certificate(new X509CertificateStructure[0]);

        /**
        * The certificates.
        */
        protected readonly X509CertificateStructure[] mCertificateList;

        public Certificate(X509CertificateStructure[] certificateList)
        {
            if (certificateList == null)
                throw new ArgumentNullException("certificateList");

            this.mCertificateList = certificateList;
        }

        /**
         * @return an array of {@link org.bouncycastle.asn1.x509.Certificate} representing a certificate
         *         chain.
         */
        public virtual X509CertificateStructure[] GetCertificateList()
        {
            return CloneCertificateList();
        }

        public virtual X509CertificateStructure GetCertificateAt(int index)
        {
            return mCertificateList[index];
        }

        public virtual int Length
        {
            get { return mCertificateList.Length; }
        }

        /**
         * @return <code>true</code> if this certificate chain contains no certificates, or
         *         <code>false</code> otherwise.
         */
        public virtual bool IsEmpty
        {
            get { return mCertificateList.Length == 0; }
        }

        /**
         * Encode this {@link Certificate} to a {@link Stream}.
         *
         * @param output the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            IList derEncodings = Platform.CreateArrayList(mCertificateList.Length);

            int totalLength = 0;
            foreach (Asn1Encodable asn1Cert in mCertificateList)
            {
                byte[] derEncoding = asn1Cert.GetEncoded(Asn1Encodable.Der);
                derEncodings.Add(derEncoding);
                totalLength += derEncoding.Length + 3;
            }

            TlsUtilities.CheckUint24(totalLength);
            TlsUtilities.WriteUint24(totalLength, output);

            foreach (byte[] derEncoding in derEncodings)
            {
                TlsUtilities.WriteOpaque24(derEncoding, output);
            }
        }

        /**
         * Parse a {@link Certificate} from a {@link Stream}.
         *
         * @param input the {@link Stream} to parse from.
         * @return a {@link Certificate} object.
         * @throws IOException
         */
        public static Certificate Parse(Stream input)
        {
            int totalLength = TlsUtilities.ReadUint24(input);
            if (totalLength == 0)
            {
                return EmptyChain;
            }

            byte[] certListData = TlsUtilities.ReadFully(totalLength, input);

            MemoryStream buf = new MemoryStream(certListData, false);

            IList certificate_list = Platform.CreateArrayList();
            while (buf.Position < buf.Length)
            {
                byte[] berEncoding = TlsUtilities.ReadOpaque24(buf);
                Asn1Object asn1Cert = TlsUtilities.ReadAsn1Object(berEncoding);
                certificate_list.Add(X509CertificateStructure.GetInstance(asn1Cert));
            }

            X509CertificateStructure[] certificateList = new X509CertificateStructure[certificate_list.Count];
            for (int i = 0; i < certificate_list.Count; ++i)
            {
                certificateList[i] = (X509CertificateStructure)certificate_list[i];
            }
            return new Certificate(certificateList);
        }

        protected virtual X509CertificateStructure[] CloneCertificateList()
        {
            return (X509CertificateStructure[])mCertificateList.Clone();
        }
    }
}
