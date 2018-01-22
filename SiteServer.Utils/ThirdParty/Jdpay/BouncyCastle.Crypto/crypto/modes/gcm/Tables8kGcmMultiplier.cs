using System;

using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
    public class Tables8kGcmMultiplier
        : IGcmMultiplier
    {
        private byte[] H;
        private uint[][][] M;

        public void Init(byte[] H)
        {
            if (M == null)
            {
                M = new uint[32][][];
            }
            else if (Arrays.AreEqual(this.H, H))
            {
                return;
            }

            this.H = Arrays.Clone(H);

            M[0] = new uint[16][];
            M[1] = new uint[16][];
            M[0][0] = new uint[4];
            M[1][0] = new uint[4];
            M[1][8] = GcmUtilities.AsUints(H);

            for (int j = 4; j >= 1; j >>= 1)
            {
                uint[] tmp = (uint[])M[1][j + j].Clone();
                GcmUtilities.MultiplyP(tmp);
                M[1][j] = tmp;
            }

            {
                uint[] tmp = (uint[])M[1][1].Clone();
                GcmUtilities.MultiplyP(tmp);
                M[0][8] = tmp;
            }

            for (int j = 4; j >= 1; j >>= 1)
            {
                uint[] tmp = (uint[])M[0][j + j].Clone();
                GcmUtilities.MultiplyP(tmp);
                M[0][j] = tmp;
            }

            for (int i = 0; ; )
            {
                for (int j = 2; j < 16; j += j)
                {
                    for (int k = 1; k < j; ++k)
                    {
                        uint[] tmp = (uint[])M[i][j].Clone();
                        GcmUtilities.Xor(tmp, M[i][k]);
                        M[i][j + k] = tmp;
                    }
                }

                if (++i == 32) return;

                if (i > 1)
                {
                    M[i] = new uint[16][];
                    M[i][0] = new uint[4];
                    for (int j = 8; j > 0; j >>= 1)
                    {
                        uint[] tmp = (uint[])M[i - 2][j].Clone();
                        GcmUtilities.MultiplyP8(tmp);
                        M[i][j] = tmp;
                    }
                }
            }
        }

        public void MultiplyH(byte[] x)
        {
            uint[] z = new uint[4];
            for (int i = 15; i >= 0; --i)
            {
                //GcmUtilities.Xor(z, M[i + i][x[i] & 0x0f]);
                uint[] m = M[i + i][x[i] & 0x0f];
                z[0] ^= m[0];
                z[1] ^= m[1];
                z[2] ^= m[2];
                z[3] ^= m[3];
                //GcmUtilities.Xor(z, M[i + i + 1][(x[i] & 0xf0) >> 4]);
                m = M[i + i + 1][(x[i] & 0xf0) >> 4];
                z[0] ^= m[0];
                z[1] ^= m[1];
                z[2] ^= m[2];
                z[3] ^= m[3];
            }

            Pack.UInt32_To_BE(z, x, 0);
        }
    }
}
