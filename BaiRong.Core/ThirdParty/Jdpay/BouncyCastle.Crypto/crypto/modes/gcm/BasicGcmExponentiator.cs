using System;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
    public class BasicGcmExponentiator
        : IGcmExponentiator
    {
        private uint[] x;

        public void Init(byte[] x)
        {
            this.x = GcmUtilities.AsUints(x);
        }

        public void ExponentiateX(long pow, byte[] output)
        {
            // Initial value is little-endian 1
            uint[] y = GcmUtilities.OneAsUints();

            if (pow > 0)
            {
                uint[] powX = Arrays.Clone(x);
                do
                {
                    if ((pow & 1L) != 0)
                    {
                        GcmUtilities.Multiply(y, powX);
                    }
                    GcmUtilities.Multiply(powX, powX);
                    pow >>= 1;
                }
                while (pow > 0);
            }

            GcmUtilities.AsBytes(y, output);
        }
    }
}
