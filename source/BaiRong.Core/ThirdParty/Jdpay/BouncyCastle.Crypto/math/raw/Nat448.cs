using System;
using System.Diagnostics;

using Org.BouncyCastle.Crypto.Utilities;

namespace Org.BouncyCastle.Math.Raw
{
    internal abstract class Nat448
    {
        public static void Copy64(ulong[] x, ulong[] z)
        {
            z[0] = x[0];
            z[1] = x[1];
            z[2] = x[2];
            z[3] = x[3];
            z[4] = x[4];
            z[5] = x[5];
            z[6] = x[6];
        }

        public static ulong[] Create64()
        {
            return new ulong[7];
        }

        public static ulong[] CreateExt64()
        {
            return new ulong[14];
        }

        public static bool Eq64(ulong[] x, ulong[] y)
        {
            for (int i = 6; i >= 0; --i)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static ulong[] FromBigInteger64(BigInteger x)
        {
            if (x.SignValue < 0 || x.BitLength > 448)
                throw new ArgumentException();

            ulong[] z = Create64();
            int i = 0;
            while (x.SignValue != 0)
            {
                z[i++] = (ulong)x.LongValue;
                x = x.ShiftRight(64);
            }
            return z;
        }

        public static bool IsOne64(ulong[] x)
        {
            if (x[0] != 1UL)
            {
                return false;
            }
            for (int i = 1; i < 7; ++i)
            {
                if (x[i] != 0UL)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsZero64(ulong[] x)
        {
            for (int i = 0; i < 7; ++i)
            {
                if (x[i] != 0UL)
                {
                    return false;
                }
            }
            return true;
        }

        public static BigInteger ToBigInteger64(ulong[] x)
        {
            byte[] bs = new byte[56];
            for (int i = 0; i < 7; ++i)
            {
                ulong x_i = x[i];
                if (x_i != 0L)
                {
                    Pack.UInt64_To_BE(x_i, bs, (6 - i) << 3);
                }
            }
            return new BigInteger(1, bs);
        }
    }
}
