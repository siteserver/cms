// ***************************************************************
// <copyright file="HashCode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;

    internal struct HashCode
    {
        int hash1;
        int hash2;
        int offset;

        public HashCode(bool ignore)
        {
            offset = 0;
            hash1 = hash2 = 5381;
        }

        public static int CalculateEmptyHash()
        {
            return 5381 + unchecked(5381 * 1566083941);
        }

        public static int Calculate(string obj)
        {
            var hash1 = 5381;
            var hash2 = hash1;

            unsafe
            {
                fixed (char* src = obj)
                {
                    var s = src;
                    var len = obj.Length;

                    while (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ s[0];
                        if (len < 2)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ s[1];
                        s += 2;
                        len -= 2;
                    }
                }
            }

            return hash1 + (hash2 * 1566083941);
        }

#if !DATAGEN

        public static int Calculate(BufferString obj)
        {
            var hash1 = 5381;
            var hash2 = hash1;

            unsafe
            {
                fixed (char* src = obj.Buffer)
                {
                    var s = src + obj.Offset;
                    var len = obj.Length;

                    while (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ s[0];
                        if (len == 1)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ s[1];
                        s += 2;
                        len -= 2;
                    }
                }
            }

            return hash1 + (hash2 * 1566083941);
        }
#endif
        public static int CalculateLowerCase(string obj)
        {
            var hash1 = 5381;
            var hash2 = hash1;

            unsafe
            {
                fixed (char* src = obj)
                {
                    var s = src;
                    var len = obj.Length;

                    while (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ ParseSupport.ToLowerCase(s[0]);
                        if (len == 1)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ ParseSupport.ToLowerCase(s[1]);
                        s += 2;
                        len -= 2;
                    }
                }
            }

            return hash1 + (hash2 * 1566083941);
        }

#if !DATAGEN

        public static int CalculateLowerCase(BufferString obj)
        {
            var hash1 = 5381;
            var hash2 = hash1;

            unsafe
            {
                fixed (char* src = obj.Buffer)
                {
                    var s = src + obj.Offset;
                    var len = obj.Length;

                    while (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ ParseSupport.ToLowerCase(s[0]);
                        if (len == 1)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ ParseSupport.ToLowerCase(s[1]);
                        s += 2;
                        len -= 2;
                    }
                }
            }

            return hash1 + (hash2 * 1566083941);
        }
#endif

        public static int Calculate(char[] buffer, int offset, int length)
        {
            var hash1 = 5381;
            var hash2 = hash1;

            CheckArgs(buffer, offset, length);

            unsafe
            {
                fixed (char* src = buffer)
                {
                    var s = src + offset;

                    while (length > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ s[0];
                        if (length == 1)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ s[1];
                        s += 2;
                        length -= 2;
                    }
                }
            }

            return hash1 + (hash2 * 1566083941);
        }

        public static int CalculateLowerCase(char[] buffer, int offset, int length)
        {
            var hash1 = 5381;
            var hash2 = hash1;

            CheckArgs(buffer, offset, length);

            unsafe
            {
                fixed (char* src = buffer)
                {
                    var s = src + offset;

                    while (length > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ ParseSupport.ToLowerCase(s[0]);
                        if (length == 1)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ ParseSupport.ToLowerCase(s[1]);
                        s += 2;
                        length -= 2;
                    }
                }
            }

            return hash1 + (hash2 * 1566083941);
        }

        public void Initialize()
        {
            offset = 0;
            hash1 = hash2 = 5381;
        }

        public unsafe void Advance(char* s, int len)
        {
            if (0 != (offset & 1))
            {
                hash2 = ((hash2 << 5) + hash2) ^ s[0];
                s++;
                len--;
                offset++;
            }

            offset += len;

            while (len > 0)
            {
                hash1 = ((hash1 << 5) + hash1) ^ s[0];
                if (len == 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ s[1];
                s += 2;
                len -= 2;
            }
        }

        public unsafe void AdvanceLowerCase(char* s, int len)
        {
            if (0 != (offset & 1))
            {
                hash2 = ((hash2 << 5) + hash2) ^ ParseSupport.ToLowerCase(s[0]);
                s++;
                len--;
                offset++;
            }

            offset += len;

            while (len > 0)
            {
                hash1 = ((hash1 << 5) + hash1) ^ ParseSupport.ToLowerCase(s[0]);
                if (len == 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ ParseSupport.ToLowerCase(s[1]);
                s += 2;
                len -= 2;
            }
        }

        public void Advance(int ucs32)
        {
            // Unicode 32bit literal.

            if (ucs32 >= 0x10000)
            {
                var c1 = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
                var c2 = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
                if (0 == ((offset += 2) & 1))
                {
                    hash1 = ((hash1 << 5) + hash1) ^ c1;
                    hash2 = ((hash2 << 5) + hash2) ^ c2;
                }
                else
                {
                    hash2 = ((hash2 << 5) + hash2) ^ c1;
                    hash1 = ((hash1 << 5) + hash1) ^ c2;
                }
            }
            else
            {
                if (0 == (offset++ & 1))
                {
                    hash1 = ((hash1 << 5) + hash1) ^ (int)ucs32;
                }
                else
                {
                    hash2 = ((hash2 << 5) + hash2) ^ (int)ucs32;
                }
            }
        }

        public void AdvanceLowerCase(int ucs32)
        {
            if (ucs32 >= 0x10000)
            {
                var c1 = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
                var c2 = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
                if (0 == ((offset += 2) & 1))
                {
                    hash1 = ((hash1 << 5) + hash1) ^ c1;
                    hash2 = ((hash2 << 5) + hash2) ^ c2;
                }
                else
                {
                    hash2 = ((hash2 << 5) + hash2) ^ c1;
                    hash1 = ((hash1 << 5) + hash1) ^ c2;
                }
            }
            else
            {
                AdvanceLowerCase((char)ucs32);
            }
        }

        public void Advance(char c)
        {
            if (0 == (offset++ & 1))
            {
                hash1 = ((hash1 << 5) + hash1) ^ c;
            }
            else
            {
                hash2 = ((hash2 << 5) + hash2) ^ c;
            }
        }

        public int AdvanceAndFinalizeHash(char c)
        {
            if (0 == (offset++ & 1))
            {
                hash1 = ((hash1 << 5) + hash1) ^ c;
            }
            else
            {
                hash2 = ((hash2 << 5) + hash2) ^ c;
            }
            return hash1 + (hash2 * 1566083941);
        }

        public void AdvanceLowerCase(char c)
        {
            if (0 == (offset++ & 1))
            {
                hash1 = ((hash1 << 5) + hash1) ^ ParseSupport.ToLowerCase(c);
            }
            else
            {
                hash2 = ((hash2 << 5) + hash2) ^ ParseSupport.ToLowerCase(c);
            }
        }

#if !DATAGEN

        public void Advance(BufferString obj)
        {
            unsafe
            {
                fixed (char* src = obj.Buffer)
                {
                    Advance(src + obj.Offset, obj.Length);
                }
            }
        }

        public void AdvanceLowerCase(BufferString obj)
        {
            unsafe
            {
                fixed (char* src = obj.Buffer)
                {
                    AdvanceLowerCase(src + obj.Offset, obj.Length);
                }
            }
        }
#endif

        public void Advance(char[] buffer, int offset, int length)
        {
            CheckArgs(buffer, offset, length);

            unsafe
            {
                fixed (char* src = buffer)
                {
                    Advance(src + offset, length);
                }
            }
        }

        public void AdvanceLowerCase(char[] buffer, int offset, int length)
        {
            CheckArgs(buffer, offset, length);

            unsafe
            {
                fixed (char* src = buffer)
                {
                    AdvanceLowerCase(src + offset, length);
                }
            }
        }

        private static void CheckArgs(char[] buffer, int offset, int length)
        {
            var bufferLength = buffer.Length;
            if (offset < 0 || offset > bufferLength)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }
            if (offset + length < offset ||
                offset + length > bufferLength)
            {
                throw new ArgumentOutOfRangeException("offset + length");
            }
        }

        public int FinalizeHash()
        {
            return hash1 + (hash2 * 1566083941);
        }
    }
}

