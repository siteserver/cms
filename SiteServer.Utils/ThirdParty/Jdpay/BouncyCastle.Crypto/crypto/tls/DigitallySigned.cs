using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class DigitallySigned
    {
        protected readonly SignatureAndHashAlgorithm mAlgorithm;
        protected readonly byte[] mSignature;

        public DigitallySigned(SignatureAndHashAlgorithm algorithm, byte[] signature)
        {
            if (signature == null)
                throw new ArgumentNullException("signature");

            this.mAlgorithm = algorithm;
            this.mSignature = signature;
        }

        /**
         * @return a {@link SignatureAndHashAlgorithm} (or null before TLS 1.2).
         */
        public virtual SignatureAndHashAlgorithm Algorithm
        {
            get { return mAlgorithm; }
        }

        public virtual byte[] Signature
        {
            get { return mSignature; }
        }

        /**
         * Encode this {@link DigitallySigned} to a {@link Stream}.
         * 
         * @param output
         *            the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            if (mAlgorithm != null)
            {
                mAlgorithm.Encode(output);
            }
            TlsUtilities.WriteOpaque16(mSignature, output);
        }

        /**
         * Parse a {@link DigitallySigned} from a {@link Stream}.
         * 
         * @param context
         *            the {@link TlsContext} of the current connection.
         * @param input
         *            the {@link Stream} to parse from.
         * @return a {@link DigitallySigned} object.
         * @throws IOException
         */
        public static DigitallySigned Parse(TlsContext context, Stream input)
        {
            SignatureAndHashAlgorithm algorithm = null;
            if (TlsUtilities.IsTlsV12(context))
            {
                algorithm = SignatureAndHashAlgorithm.Parse(input);
            }
            byte[] signature = TlsUtilities.ReadOpaque16(input);
            return new DigitallySigned(algorithm, signature);
        }
    }
}
