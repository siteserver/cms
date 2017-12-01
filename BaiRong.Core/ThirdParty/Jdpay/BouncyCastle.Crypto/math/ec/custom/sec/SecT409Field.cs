using System;
using System.Diagnostics;

using Org.BouncyCastle.Math.Raw;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecT409Field
    {
        private const ulong M25 = ulong.MaxValue >> 39;
        private const ulong M59 = ulong.MaxValue >> 5;

        public static void Add(ulong[] x, ulong[] y, ulong[] z)
        {
            z[0] = x[0] ^ y[0];
            z[1] = x[1] ^ y[1];
            z[2] = x[2] ^ y[2];
            z[3] = x[3] ^ y[3];
            z[4] = x[4] ^ y[4];
            z[5] = x[5] ^ y[5];
            z[6] = x[6] ^ y[6];
        }

        public static void AddExt(ulong[] xx, ulong[] yy, ulong[] zz)
        {
            for (int i = 0; i < 13; ++i)
            {
                zz[i] = xx[i] ^ yy[i]; 
            }
        }

        public static void AddOne(ulong[] x, ulong[] z)
        {
            z[0] = x[0] ^ 1UL;
            z[1] = x[1];
            z[2] = x[2];
            z[3] = x[3];
            z[4] = x[4];
            z[5] = x[5];
            z[6] = x[6];
        }

        public static ulong[] FromBigInteger(BigInteger x)
        {
            ulong[] z = Nat448.FromBigInteger64(x);
            Reduce39(z, 0);
            return z;
        }

        public static void Invert(ulong[] x, ulong[] z)
        {
            if (Nat448.IsZero64(x))
                throw new InvalidOperationException();

            // Itoh-Tsujii inversion with bases { 2, 3 }

            ulong[] t0 = Nat448.Create64();
            ulong[] t1 = Nat448.Create64();
            ulong[] t2 = Nat448.Create64();

            Square(x, t0);

            // 3 | 408
            SquareN(t0, 1, t1);
            Multiply(t0, t1, t0);
            SquareN(t1, 1, t1);
            Multiply(t0, t1, t0);

            // 2 | 136
            SquareN(t0, 3, t1);
            Multiply(t0, t1, t0);

            // 2 | 68
            SquareN(t0, 6, t1);
            Multiply(t0, t1, t0);

            // 2 | 34
            SquareN(t0, 12, t1);
            Multiply(t0, t1, t2);

            // ! {2,3} | 17
            SquareN(t2, 24, t0);
            SquareN(t0, 24, t1);
            Multiply(t0, t1, t0);

            // 2 | 8
            SquareN(t0, 48, t1);
            Multiply(t0, t1, t0);

            // 2 | 4
            SquareN(t0, 96, t1);
            Multiply(t0, t1, t0);

            // 2 | 2
            SquareN(t0, 192, t1);
            Multiply(t0, t1, t0);

            Multiply(t0, t2, z);
        }

        public static void Multiply(ulong[] x, ulong[] y, ulong[] z)
        {
            ulong[] tt = Nat448.CreateExt64();
            ImplMultiply(x, y, tt);
            Reduce(tt, z);
        }

        public static void MultiplyAddToExt(ulong[] x, ulong[] y, ulong[] zz)
        {
            ulong[] tt = Nat448.CreateExt64();
            ImplMultiply(x, y, tt);
            AddExt(zz, tt, zz);
        }

        public static void Reduce(ulong[] xx, ulong[] z)
        {
            ulong x00 = xx[0], x01 = xx[1], x02 = xx[2], x03 = xx[3];
            ulong x04 = xx[4], x05 = xx[5], x06 = xx[6], x07 = xx[7];

            ulong u = xx[12];
            x05 ^= (u << 39);
            x06 ^= (u >> 25) ^ (u << 62);
            x07 ^= (u >>  2);

            u = xx[11];
            x04 ^= (u << 39);
            x05 ^= (u >> 25) ^ (u << 62);
            x06 ^= (u >>  2);

            u = xx[10];
            x03 ^= (u << 39);
            x04 ^= (u >> 25) ^ (u << 62);
            x05 ^= (u >>  2);

            u = xx[9];
            x02 ^= (u << 39);
            x03 ^= (u >> 25) ^ (u << 62);
            x04 ^= (u >>  2);

            u = xx[8];
            x01 ^= (u << 39);
            x02 ^= (u >> 25) ^ (u << 62);
            x03 ^= (u >>  2);

            u = x07;
            x00 ^= (u << 39);
            x01 ^= (u >> 25) ^ (u << 62);
            x02 ^= (u >>  2);

            ulong t = x06 >> 25;
            z[0]    = x00 ^ t;
            z[1]    = x01 ^ (t << 23);
            z[2]    = x02;
            z[3]    = x03;
            z[4]    = x04;
            z[5]    = x05;
            z[6]    = x06 & M25;
        }

        public static void Reduce39(ulong[] z, int zOff)
        {
            ulong z6 = z[zOff + 6], t = z6 >> 25;
            z[zOff    ] ^= t; 
            z[zOff + 1] ^= (t << 23);
            z[zOff + 6]  = z6 & M25;
        }

        public static void Sqrt(ulong[] x, ulong[] z)
        {
            ulong u0, u1;
            u0 = Interleave.Unshuffle(x[0]); u1 = Interleave.Unshuffle(x[1]);
            ulong e0 = (u0 & 0x00000000FFFFFFFFUL) | (u1 << 32);
            ulong c0 = (u0 >> 32) | (u1 & 0xFFFFFFFF00000000UL);

            u0 = Interleave.Unshuffle(x[2]); u1 = Interleave.Unshuffle(x[3]);
            ulong e1 = (u0 & 0x00000000FFFFFFFFUL) | (u1 << 32);
            ulong c1 = (u0 >> 32) | (u1 & 0xFFFFFFFF00000000UL);

            u0 = Interleave.Unshuffle(x[4]); u1 = Interleave.Unshuffle(x[5]);
            ulong e2 = (u0 & 0x00000000FFFFFFFFUL) | (u1 << 32);
            ulong c2 = (u0 >> 32) | (u1 & 0xFFFFFFFF00000000UL);

            u0 = Interleave.Unshuffle(x[6]);
            ulong e3 = (u0 & 0x00000000FFFFFFFFUL);
            ulong c3 = (u0 >> 32);

            z[0] = e0 ^ (c0 << 44);
            z[1] = e1 ^ (c1 << 44) ^ (c0 >> 20);
            z[2] = e2 ^ (c2 << 44) ^ (c1 >> 20);
            z[3] = e3 ^ (c3 << 44) ^ (c2 >> 20) ^ (c0 << 13);
            z[4] =                   (c3 >> 20) ^ (c1 << 13) ^ (c0 >> 51);
            z[5] =                                (c2 << 13) ^ (c1 >> 51);
            z[6] =                                (c3 << 13) ^ (c2 >> 51);

            Debug.Assert((c3 >> 51) == 0);
        }

        public static void Square(ulong[] x, ulong[] z)
        {
            ulong[] tt = Nat.Create64(13);
            ImplSquare(x, tt);
            Reduce(tt, z);
        }

        public static void SquareAddToExt(ulong[] x, ulong[] zz)
        {
            ulong[] tt = Nat.Create64(13);
            ImplSquare(x, tt);
            AddExt(zz, tt, zz);
        }

        public static void SquareN(ulong[] x, int n, ulong[] z)
        {
            Debug.Assert(n > 0);

            ulong[] tt = Nat.Create64(13);
            ImplSquare(x, tt);
            Reduce(tt, z);

            while (--n > 0)
            {
                ImplSquare(z, tt);
                Reduce(tt, z);
            }
        }

        public static uint Trace(ulong[] x)
        {
            // Non-zero-trace bits: 0
            return (uint)(x[0]) & 1U;
        }

        protected static void ImplCompactExt(ulong[] zz)
        {
            ulong z00 = zz[ 0], z01 = zz[ 1], z02 = zz[ 2], z03 = zz[ 3], z04 = zz[ 4], z05 = zz[ 5], z06 = zz[ 6];
            ulong z07 = zz[ 7], z08 = zz[ 8], z09 = zz[ 9], z10 = zz[10], z11 = zz[11], z12 = zz[12], z13 = zz[13];
            zz[ 0] =  z00        ^ (z01 << 59);
            zz[ 1] = (z01 >>  5) ^ (z02 << 54);
            zz[ 2] = (z02 >> 10) ^ (z03 << 49);
            zz[ 3] = (z03 >> 15) ^ (z04 << 44);
            zz[ 4] = (z04 >> 20) ^ (z05 << 39);
            zz[ 5] = (z05 >> 25) ^ (z06 << 34);
            zz[ 6] = (z06 >> 30) ^ (z07 << 29);
            zz[ 7] = (z07 >> 35) ^ (z08 << 24);
            zz[ 8] = (z08 >> 40) ^ (z09 << 19);
            zz[ 9] = (z09 >> 45) ^ (z10 << 14);
            zz[10] = (z10 >> 50) ^ (z11 <<  9);
            zz[11] = (z11 >> 55) ^ (z12 <<  4)
                                 ^ (z13 << 63);
            zz[12] = (z12 >> 60)
                   ^ (z13 >> 1);
            zz[13] = 0;
        }

        protected static void ImplExpand(ulong[] x, ulong[] z)
        {
            ulong x0 = x[0], x1 = x[1], x2 = x[2], x3 = x[3], x4 = x[4], x5 = x[5], x6 = x[6];
            z[0] = x0 & M59;
            z[1] = ((x0 >> 59) ^ (x1 <<  5)) & M59;
            z[2] = ((x1 >> 54) ^ (x2 << 10)) & M59;
            z[3] = ((x2 >> 49) ^ (x3 << 15)) & M59;
            z[4] = ((x3 >> 44) ^ (x4 << 20)) & M59;
            z[5] = ((x4 >> 39) ^ (x5 << 25)) & M59;
            z[6] = ((x5 >> 34) ^ (x6 << 30));
        }

        protected static void ImplMultiply(ulong[] x, ulong[] y, ulong[] zz)
        {
            ulong[] a = new ulong[7], b = new ulong[7];
            ImplExpand(x, a);
            ImplExpand(y, b);

            for (int i = 0; i < 7; ++i)
            {
                ImplMulwAcc(a, b[i], zz, i);
            }

            ImplCompactExt(zz);
        }

        protected static void ImplMulwAcc(ulong[] xs, ulong y, ulong[] z, int zOff)
        {
            Debug.Assert(y >> 59 == 0);

            ulong[] u = new ulong[8];
            //u[0] = 0;
            u[1] = y;
            u[2] = u[1] << 1;
            u[3] = u[2] ^  y;
            u[4] = u[2] << 1;
            u[5] = u[4] ^  y;
            u[6] = u[3] << 1;
            u[7] = u[6] ^  y;

            for (int i = 0; i < 7; ++i)
            {
                ulong x = xs[i];

                Debug.Assert(x >> 59 == 0);

                uint j = (uint)x;
                ulong g, h = 0, l = u[j & 7]
                                  ^ (u[(j >> 3) & 7] << 3);
                int k = 54;
                do
                {
                    j  = (uint)(x >> k);
                    g  = u[j & 7]
                       ^ u[(j >> 3) & 7] << 3;
                    l ^= (g <<  k);
                    h ^= (g >> -k);
                }
                while ((k -= 6) > 0);

                Debug.Assert(h >> 53 == 0);

                z[zOff + i    ] ^= l & M59;
                z[zOff + i + 1] ^= (l >> 59) ^ (h << 5);
            }
        }

        protected static void ImplSquare(ulong[] x, ulong[] zz)
        {
            for (int i = 0; i < 6; ++i)
            {
                Interleave.Expand64To128(x[i], zz, i << 1);
            }
            zz[12] = Interleave.Expand32to64((uint)x[6]);
        }
    }
}
