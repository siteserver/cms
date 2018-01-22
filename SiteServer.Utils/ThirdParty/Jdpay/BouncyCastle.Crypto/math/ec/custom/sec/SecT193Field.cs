using System;
using System.Diagnostics;

using Org.BouncyCastle.Math.Raw;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecT193Field
    {
        private const ulong M01 = 1UL;
        private const ulong M49 = ulong.MaxValue >> 15;

        public static void Add(ulong[] x, ulong[] y, ulong[] z)
        {
            z[0] = x[0] ^ y[0];
            z[1] = x[1] ^ y[1];
            z[2] = x[2] ^ y[2];
            z[3] = x[3] ^ y[3];
        }

        public static void AddExt(ulong[] xx, ulong[] yy, ulong[] zz)
        {
            zz[0] = xx[0] ^ yy[0];
            zz[1] = xx[1] ^ yy[1];
            zz[2] = xx[2] ^ yy[2];
            zz[3] = xx[3] ^ yy[3];
            zz[4] = xx[4] ^ yy[4];
            zz[5] = xx[5] ^ yy[5];
            zz[6] = xx[6] ^ yy[6];
        }

        public static void AddOne(ulong[] x, ulong[] z)
        {
            z[0] = x[0] ^ 1UL;
            z[1] = x[1];
            z[2] = x[2];
            z[3] = x[3];
        }

        public static ulong[] FromBigInteger(BigInteger x)
        {
            ulong[] z = Nat256.FromBigInteger64(x);
            Reduce63(z, 0);
            return z;
        }

        public static void Invert(ulong[] x, ulong[] z)
        {
            if (Nat256.IsZero64(x))
                throw new InvalidOperationException();

            // Itoh-Tsujii inversion with bases { 2, 3 }

            ulong[] t0 = Nat256.Create64();
            ulong[] t1 = Nat256.Create64();

            Square(x, t0);

            // 3 | 192
            SquareN(t0, 1, t1);
            Multiply(t0, t1, t0);
            SquareN(t1, 1, t1);
            Multiply(t0, t1, t0);

            // 2 | 64
            SquareN(t0, 3, t1);
            Multiply(t0, t1, t0);

            // 2 | 32
            SquareN(t0, 6, t1);
            Multiply(t0, t1, t0);

            // 2 | 16
            SquareN(t0, 12, t1);
            Multiply(t0, t1, t0);

            // 2 | 8
            SquareN(t0, 24, t1);
            Multiply(t0, t1, t0);

            // 2 | 4
            SquareN(t0, 48, t1);
            Multiply(t0, t1, t0);

            // 2 | 2
            SquareN(t0, 96, t1);
            Multiply(t0, t1, z);
        }

        public static void Multiply(ulong[] x, ulong[] y, ulong[] z)
        {
            ulong[] tt = Nat256.CreateExt64();
            ImplMultiply(x, y, tt);
            Reduce(tt, z);
        }

        public static void MultiplyAddToExt(ulong[] x, ulong[] y, ulong[] zz)
        {
            ulong[] tt = Nat256.CreateExt64();
            ImplMultiply(x, y, tt);
            AddExt(zz, tt, zz);
        }

        public static void Reduce(ulong[] xx, ulong[] z)
        {
            ulong x0 = xx[0], x1 = xx[1], x2 = xx[2], x3 = xx[3], x4 = xx[4], x5 = xx[5], x6 = xx[6];

            x2 ^= (x6 << 63);
            x3 ^= (x6 >>  1) ^ (x6 << 14);
            x4 ^= (x6 >> 50);

            x1 ^= (x5 << 63);
            x2 ^= (x5 >>  1) ^ (x5 << 14);
            x3 ^= (x5 >> 50);

            x0 ^= (x4 << 63);
            x1 ^= (x4 >>  1) ^ (x4 << 14);
            x2 ^= (x4 >> 50);

            ulong t = x3 >> 1;
            z[0]    = x0 ^ t ^ (t << 15);
            z[1]    = x1     ^ (t >> 49);
            z[2]    = x2;
            z[3]    = x3 & M01;
        }

        public static void Reduce63(ulong[] z, int zOff)
        {
            ulong z3     = z[zOff + 3], t = z3 >> 1;
            z[zOff    ] ^= t ^ (t << 15);
            z[zOff + 1] ^=     (t >> 49);
            z[zOff + 3]  = z3 & M01;
        }

        public static void Sqrt(ulong[] x, ulong[] z)
        {
            ulong u0, u1;
            u0 = Interleave.Unshuffle(x[0]); u1 = Interleave.Unshuffle(x[1]);
            ulong e0 = (u0 & 0x00000000FFFFFFFFUL) | (u1 << 32);
            ulong c0 = (u0 >> 32) | (u1 & 0xFFFFFFFF00000000UL);

            u0 = Interleave.Unshuffle(x[2]);
            ulong e1 = (u0 & 0x00000000FFFFFFFFUL) ^ (x[3] << 32);
            ulong c1 = (u0 >> 32);

            z[0] = e0 ^ (c0 << 8);
            z[1] = e1 ^ (c1 << 8) ^ (c0 >> 56) ^ (c0 << 33);
            z[2] =                  (c1 >> 56) ^ (c1 << 33) ^ (c0 >> 31);
            z[3] =                                            (c1 >> 31);
        }

        public static void Square(ulong[] x, ulong[] z)
        {
            ulong[] tt = Nat256.CreateExt64();
            ImplSquare(x, tt);
            Reduce(tt, z);
        }

        public static void SquareAddToExt(ulong[] x, ulong[] zz)
        {
            ulong[] tt = Nat256.CreateExt64();
            ImplSquare(x, tt);
            AddExt(zz, tt, zz);
        }

        public static void SquareN(ulong[] x, int n, ulong[] z)
        {
            Debug.Assert(n > 0);

            ulong[] tt = Nat256.CreateExt64();
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
            ulong z0 = zz[0], z1 = zz[1], z2 = zz[2], z3 = zz[3], z4 = zz[4], z5 = zz[5], z6 = zz[6], z7 = zz[7];
            zz[0] =  z0        ^ (z1 << 49);
            zz[1] = (z1 >> 15) ^ (z2 << 34);
            zz[2] = (z2 >> 30) ^ (z3 << 19);
            zz[3] = (z3 >> 45) ^ (z4 <<  4)
                               ^ (z5 << 53);
            zz[4] = (z4 >> 60) ^ (z6 << 38)
                  ^ (z5 >> 11);
            zz[5] = (z6 >> 26) ^ (z7 << 23);
            zz[6] = (z7 >> 41);
            zz[7] = 0;
        }

        protected static void ImplExpand(ulong[] x, ulong[] z)
        {
            ulong x0 = x[0], x1 = x[1], x2 = x[2], x3 = x[3];
            z[0] = x0 & M49;
            z[1] = ((x0 >> 49) ^ (x1 << 15)) & M49;
            z[2] = ((x1 >> 34) ^ (x2 << 30)) & M49;
            z[3] = ((x2 >> 19) ^ (x3 << 45));
        }

        protected static void ImplMultiply(ulong[] x, ulong[] y, ulong[] zz)
        {
            /*
             * "Two-level seven-way recursion" as described in "Batch binary Edwards", Daniel J. Bernstein.
             */

            ulong[] f = new ulong[4], g = new ulong[4];
            ImplExpand(x, f);
            ImplExpand(y, g);

            ImplMulwAcc(f[0], g[0], zz, 0);
            ImplMulwAcc(f[1], g[1], zz, 1);
            ImplMulwAcc(f[2], g[2], zz, 2);
            ImplMulwAcc(f[3], g[3], zz, 3);

            // U *= (1 - t^n)
            for (int i = 5; i > 0; --i)
            {
                zz[i] ^= zz[i - 1];
            }

            ImplMulwAcc(f[0] ^ f[1], g[0] ^ g[1], zz, 1);
            ImplMulwAcc(f[2] ^ f[3], g[2] ^ g[3], zz, 3);

            // V *= (1 - t^2n)
            for (int i = 7; i > 1; --i)
            {
                zz[i] ^= zz[i - 2];
            }

            // Double-length recursion
            {
                ulong c0 = f[0] ^ f[2], c1 = f[1] ^ f[3];
                ulong d0 = g[0] ^ g[2], d1 = g[1] ^ g[3];
                ImplMulwAcc(c0 ^ c1, d0 ^ d1, zz, 3);
                ulong[] t = new ulong[3];
                ImplMulwAcc(c0, d0, t, 0);
                ImplMulwAcc(c1, d1, t, 1);
                ulong t0 = t[0], t1 = t[1], t2 = t[2];
                zz[2] ^= t0;
                zz[3] ^= t0 ^ t1;
                zz[4] ^= t2 ^ t1;
                zz[5] ^= t2;
            }

            ImplCompactExt(zz);
        }

        protected static void ImplMulwAcc(ulong x, ulong y, ulong[] z, int zOff)
        {
            Debug.Assert(x >> 49 == 0);
            Debug.Assert(y >> 49 == 0);

            ulong[] u = new ulong[8];
            //u[0] = 0;
            u[1] = y;
            u[2] = u[1] << 1;
            u[3] = u[2] ^  y;
            u[4] = u[2] << 1;
            u[5] = u[4] ^  y;
            u[6] = u[3] << 1;
            u[7] = u[6] ^ y;

            uint j = (uint)x;
            ulong g, h = 0, l = u[j & 7]
                              ^ (u[(j >> 3) & 7] << 3);
            int k = 36;
            do
            {
                j  = (uint)(x >> k);
                g  = u[j & 7]
                   ^ u[(j >> 3) & 7] << 3
                   ^ u[(j >> 6) & 7] << 6
                   ^ u[(j >> 9) & 7] << 9
                   ^ u[(j >> 12) & 7] << 12;
                l ^= (g <<  k);
                h ^= (g >> -k);
            }
            while ((k -= 15) > 0);

            Debug.Assert(h >> 33 == 0);

            z[zOff    ] ^= l & M49;
            z[zOff + 1] ^= (l >> 49) ^ (h << 15);
        }

        protected static void ImplSquare(ulong[] x, ulong[] zz)
        {
            Interleave.Expand64To128(x[0], zz, 0);
            Interleave.Expand64To128(x[1], zz, 2);
            Interleave.Expand64To128(x[2], zz, 4);
            zz[6] = (x[3] & M01);
        }
    }
}
