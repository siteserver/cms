using System;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
    public class BasicGcmMultiplier
        : IGcmMultiplier
    {
        private uint[] H;

        public void Init(byte[] H)
        {
            this.H = GcmUtilities.AsUints(H);
        }

        public void MultiplyH(byte[] x)
        {
            uint[] t = GcmUtilities.AsUints(x);
            GcmUtilities.Multiply(t, H);
            GcmUtilities.AsBytes(t, x);
        }
    }
}
