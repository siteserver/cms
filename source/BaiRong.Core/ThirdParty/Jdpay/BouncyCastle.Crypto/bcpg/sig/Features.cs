using System;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Bcpg.Sig
{
    /**
    * packet giving signature expiration time.
    */
    public class Features
        : SignatureSubpacket
    {
        /** Identifier for the modification detection feature */
        public static readonly byte FEATURE_MODIFICATION_DETECTION = 1;

        private static byte[] FeatureToByteArray(byte feature)
        {
            return new byte[]{ feature };
        }

        public Features(
            bool    critical,
            bool    isLongLength,
            byte[]  data)
            : base(SignatureSubpacketTag.Features, critical, isLongLength, data)
        {
        }

        public Features(bool critical, byte feature)
            : base(SignatureSubpacketTag.Features, critical, false, FeatureToByteArray(feature))
        {
        }

        /**
         * Returns if modification detection is supported.
         */
        public bool SupportsModificationDetection
        {
            get { return SupportsFeature(FEATURE_MODIFICATION_DETECTION); }
        }

        /**
         * Returns if a particular feature is supported.
         */
        public bool SupportsFeature(byte feature)
        {
            return Array.IndexOf(data, feature) >= 0;
        }

        /**
         * Sets support for a particular feature.
         */
        private void SetSupportsFeature(byte feature, bool support)
        {
            if (feature == 0)
                throw new ArgumentException("cannot be 0", "feature");

            int i = Array.IndexOf(data, feature);
            if ((i >= 0) == support)
                return;

            if (support)
            {
                data = Arrays.Append(data, feature);
            }
            else
            {
                byte[] temp = new byte[data.Length - 1];
                Array.Copy(data, 0, temp, 0, i);
                Array.Copy(data, i + 1, temp, i, temp.Length - i);
                data = temp;
            }
        }
    }
}
