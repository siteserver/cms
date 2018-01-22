using System;

using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
    internal abstract class GcmUtilities
    {
        private const uint E1 = 0xe1000000;
        private const ulong E1L = (ulong)E1 << 32;

        private static uint[] GenerateLookup()
        {
            uint[] lookup = new uint[256];

            for (int c = 0; c < 256; ++c)
            {
                uint v = 0;
                for (int i = 7; i >= 0; --i)
                {
                    if ((c & (1 << i)) != 0)
                    {
                        v ^= (E1 >> (7 - i));
                    }
                }
                lookup[c] = v;
            }

            return lookup;
        }

        private static readonly uint[] LOOKUP = GenerateLookup();

        internal static byte[] OneAsBytes()
        {
            byte[] tmp = new byte[16];
            tmp[0] = 0x80;
            return tmp;
        }

        internal static uint[] OneAsUints()
        {
            uint[] tmp = new uint[4];
            tmp[0] = 0x80000000;
            return tmp;
        }

        internal static ulong[] OneAsUlongs()
        {
            ulong[] tmp = new ulong[2];
            tmp[0] = 1UL << 63;
            return tmp;
        }

        internal static byte[] AsBytes(uint[] x)
        {
            return Pack.UInt32_To_BE(x);
        }

        internal static void AsBytes(uint[] x, byte[] z)
        {
            Pack.UInt32_To_BE(x, z, 0);
        }

        internal static byte[] AsBytes(ulong[] x)
        {
            byte[] z = new byte[16];
            Pack.UInt64_To_BE(x, z, 0);
            return z;
        }

        internal static void AsBytes(ulong[] x, byte[] z)
        {
            Pack.UInt64_To_BE(x, z, 0);
        }

        internal static uint[] AsUints(byte[] bs)
        {
            uint[] output = new uint[4];
            Pack.BE_To_UInt32(bs, 0, output);
            return output;
        }

        internal static void AsUints(byte[] bs, uint[] output)
        {
            Pack.BE_To_UInt32(bs, 0, output);
        }

        internal static ulong[] AsUlongs(byte[] x)
        {
            ulong[] z = new ulong[2];
            Pack.BE_To_UInt64(x, 0, z);
            return z;
        }

        public static void AsUlongs(byte[] x, ulong[] z)
        {
            Pack.BE_To_UInt64(x, 0, z);
        }

        internal static void Multiply(byte[] x, byte[] y)
        {
            uint[] t1 = GcmUtilities.AsUints(x);
            uint[] t2 = GcmUtilities.AsUints(y);
            GcmUtilities.Multiply(t1, t2);
            GcmUtilities.AsBytes(t1, x);
        }

        internal static void Multiply(uint[] x, uint[] y)
        {
            uint r00 = x[0], r01 = x[1], r02 = x[2], r03 = x[3];
            uint r10 = 0, r11 = 0, r12 = 0, r13 = 0;

            for (int i = 0; i < 4; ++i)
            {
                int bits = (int)y[i];
                for (int j = 0; j < 32; ++j)
                {
                    uint m1 = (uint)(bits >> 31); bits <<= 1;
                    r10 ^= (r00 & m1);
                    r11 ^= (r01 & m1);
                    r12 ^= (r02 & m1);
                    r13 ^= (r03 & m1);

                    uint m2 = (uint)((int)(r03 << 31) >> 8);
                    r03 = (r03 >> 1) | (r02 << 31);
                    r02 = (r02 >> 1) | (r01 << 31);
                    r01 = (r01 >> 1) | (r00 << 31);
                    r00 = (r00 >> 1) ^ (m2 & E1);
                }
            }

            x[0] = r10;
            x[1] = r11;
            x[2] = r12;
            x[3] = r13;
        }

        internal static void Multiply(ulong[] x, ulong[] y)
        {
            ulong r00 = x[0], r01 = x[1], r10 = 0, r11 = 0;

            for (int i = 0; i < 2; ++i)
            {
                long bits = (long)y[i];
                for (int j = 0; j < 64; ++j)
                {
                    ulong m1 = (ulong)(bits >> 63); bits <<= 1;
                    r10 ^= (r00 & m1);
                    r11 ^= (r01 & m1);

                    ulong m2 = (ulong)((long)(r01 << 63) >> 8);
                    r01 = (r01 >> 1) | (r00 << 63);
                    r00 = (r00 >> 1) ^ (m2 & E1L);
                }
            }

            x[0] = r10;
            x[1] = r11;
        }

        // P is the value with only bit i=1 set
        internal static void MultiplyP(uint[] x)
        {
            uint m = (uint)((int)ShiftRight(x) >> 8);
            x[0] ^= (m & E1);
        }

        internal static void MultiplyP(uint[] x, uint[] z)
        {
            uint m = (uint)((int)ShiftRight(x, z) >> 8);
            z[0] ^= (m & E1);
        }

        internal static void MultiplyP8(uint[] x)
        {
//			for (int i = 8; i != 0; --i)
//			{
//				MultiplyP(x);
//			}

            uint c = ShiftRightN(x, 8);
            x[0] ^= LOOKUP[c >> 24];
        }

        internal static void MultiplyP8(uint[] x, uint[] y)
        {
            uint c = ShiftRightN(x, 8, y);
            y[0] ^= LOOKUP[c >> 24];
        }

        internal static uint ShiftRight(uint[] x)
        {
            uint b = x[0];
            x[0] = b >> 1;
            uint c = b << 31;
            b = x[1];
            x[1] = (b >> 1) | c;
            c = b << 31;
            b = x[2];
            x[2] = (b >> 1) | c;
            c = b << 31;
            b = x[3];
            x[3] = (b >> 1) | c;
            return b << 31;
        }

        internal static uint ShiftRight(uint[] x, uint[] z)
        {
            uint b = x[0];
            z[0] = b >> 1;
            uint c = b << 31;
            b = x[1];
            z[1] = (b >> 1) | c;
            c = b << 31;
            b = x[2];
            z[2] = (b >> 1) | c;
            c = b << 31;
            b = x[3];
            z[3] = (b >> 1) | c;
            return b << 31;
        }

        internal static uint ShiftRightN(uint[] x, int n)
        {
            uint b = x[0]; int nInv = 32 - n;
            x[0] = b >> n;
            uint c = b << nInv;
            b = x[1];
            x[1] = (b >> n) | c;
            c = b << nInv;
            b = x[2];
            x[2] = (b >> n) | c;
            c = b << nInv;
            b = x[3];
            x[3] = (b >> n) | c;
            return b << nInv;
        }

        internal static uint ShiftRightN(uint[] x, int n, uint[] z)
        {
            uint b = x[0]; int nInv = 32 - n;
            z[0] = b >> n;
            uint c = b << nInv;
            b = x[1];
            z[1] = (b >> n) | c;
            c = b << nInv;
            b = x[2];
            z[2] = (b >> n) | c;
            c = b << nInv;
            b = x[3];
            z[3] = (b >> n) | c;
            return b << nInv;
        }

        internal static void Xor(byte[] x, byte[] y)
        {
            int i = 0;
            do
            {
                x[i] ^= y[i]; ++i;
                x[i] ^= y[i]; ++i;
                x[i] ^= y[i]; ++i;
                x[i] ^= y[i]; ++i;
            }
            while (i < 16);
        }

        internal static void Xor(byte[] x, byte[] y, int yOff)
        {
            int i = 0;
            do
            {
                x[i] ^= y[yOff + i]; ++i;
                x[i] ^= y[yOff + i]; ++i;
                x[i] ^= y[yOff + i]; ++i;
                x[i] ^= y[yOff + i]; ++i;
            }
            while (i < 16);
        }

        internal static void Xor(byte[] x, int xOff, byte[] y, int yOff, byte[] z, int zOff)
        {
            int i = 0;
            do
            {
                z[zOff + i] = (byte)(x[xOff + i] ^ y[yOff + i]); ++i;
                z[zOff + i] = (byte)(x[xOff + i] ^ y[yOff + i]); ++i;
                z[zOff + i] = (byte)(x[xOff + i] ^ y[yOff + i]); ++i;
                z[zOff + i] = (byte)(x[xOff + i] ^ y[yOff + i]); ++i;
            }
            while (i < 16);
        }

        internal static void Xor(byte[] x, byte[] y, int yOff, int yLen)
        {
            while (--yLen >= 0)
            {
                x[yLen] ^= y[yOff + yLen];
            }
        }

        internal static void Xor(byte[] x, int xOff, byte[] y, int yOff, int len)
        {
            while (--len >= 0)
            {
                x[xOff + len] ^= y[yOff + len];
            }
        }

        internal static void Xor(byte[] x, byte[] y, byte[] z)
        {
            int i = 0;
            do
            {
                z[i] = (byte)(x[i] ^ y[i]); ++i;
                z[i] = (byte)(x[i] ^ y[i]); ++i;
                z[i] = (byte)(x[i] ^ y[i]); ++i;
                z[i] = (byte)(x[i] ^ y[i]); ++i;
            }
            while (i < 16);
        }

        internal static void Xor(uint[] x, uint[] y)
        {
            x[0] ^= y[0];
            x[1] ^= y[1];
            x[2] ^= y[2];
            x[3] ^= y[3];
        }

        internal static void Xor(uint[] x, uint[] y, uint[] z)
        {
            z[0] = x[0] ^ y[0];
            z[1] = x[1] ^ y[1];
            z[2] = x[2] ^ y[2];
            z[3] = x[3] ^ y[3];
        }

        internal static void Xor(ulong[] x, ulong[] y)
        {
            x[0] ^= y[0];
            x[1] ^= y[1];
        }

        internal static void Xor(ulong[] x, ulong[] y, ulong[] z)
        {
            z[0] = x[0] ^ y[0];
            z[1] = x[1] ^ y[1];
        }
    }
}
