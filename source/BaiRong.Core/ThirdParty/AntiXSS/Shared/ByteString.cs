// ***************************************************************
// <copyright file="ByteString.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Internal
{
    using System;
    using Strings = CtsResources.SharedStrings;

    internal static class ByteString
    {
        internal static readonly byte[] LowerC = new byte[256]
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
            0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17,
            0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
            (byte) ' ', (byte) '!', (byte) '"', (byte) '#', (byte) '$', (byte) '%', (byte) '&', (byte) '\'',
            (byte) '(', (byte) ')', (byte) '*', (byte) '+', (byte) ',', (byte) '-', (byte) '.', (byte) '/',
            (byte) '0', (byte) '1', (byte) '2', (byte) '3', (byte) '4', (byte) '5', (byte) '6', (byte) '7',
            (byte) '8', (byte) '9', (byte) ':', (byte) ';', (byte) '<', (byte) '=', (byte) '>', (byte) '?',
            (byte) '@', (byte) 'a', (byte) 'b', (byte) 'c', (byte) 'd', (byte) 'e', (byte) 'f', (byte) 'g',
            (byte) 'h', (byte) 'i', (byte) 'j', (byte) 'k', (byte) 'l', (byte) 'm', (byte) 'n', (byte) 'o',
            (byte) 'p', (byte) 'q', (byte) 'r', (byte) 's', (byte) 't', (byte) 'u', (byte) 'v', (byte) 'w',
            (byte) 'x', (byte) 'y', (byte) 'z', (byte) '[', (byte) '\\', (byte) ']', (byte) '^', (byte) '_',
            (byte) '`', (byte) 'a', (byte) 'b', (byte) 'c', (byte) 'd', (byte) 'e', (byte) 'f', (byte) 'g',
            (byte) 'h', (byte) 'i', (byte) 'j', (byte) 'k', (byte) 'l', (byte) 'm', (byte) 'n', (byte) 'o',
            (byte) 'p', (byte) 'q', (byte) 'r', (byte) 's', (byte) 't', (byte) 'u', (byte) 'v', (byte) 'w',
            (byte) 'x', (byte) 'y', (byte) 'z', (byte) '{', (byte) '|', (byte) '}', (byte) '~', 0x7F,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        };

        private static readonly uint[] CrcTable = new uint[256]
        {
            0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419, 0x706AF48F,
            0xE963A535, 0x9E6495A3, 0x0EDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988,
            0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91, 0x1DB71064, 0x6AB020F2,
            0xF3B97148, 0x84BE41DE, 0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7,
            0x136C9856, 0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9,
            0xFA0F3D63, 0x8D080DF5, 0x3B6E20C8, 0x4C69105E, 0xD56041E4, 0xA2677172,
            0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B, 0x35B5A8FA, 0x42B2986C,
            0xDBBBC9D6, 0xACBCF940, 0x32D86CE3, 0x45DF5C75, 0xDCD60DCF, 0xABD13D59,
            0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423,
            0xCFBA9599, 0xB8BDA50F, 0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924,
            0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D, 0x76DC4190, 0x01DB7106,
            0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F, 0x9FBFE4A5, 0xE8B8D433,
            0x7807C9A2, 0x0F00F934, 0x9609A88E, 0xE10E9818, 0x7F6A0DBB, 0x086D3D2D,
            0x91646C97, 0xE6635C01, 0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E,
            0x6C0695ED, 0x1B01A57B, 0x8208F4C1, 0xF50FC457, 0x65B0D9C6, 0x12B7E950,
            0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65,
            0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2, 0x4ADFA541, 0x3DD895D7,
            0xA4D1C46D, 0xD3D6F4FB, 0x4369E96A, 0x346ED9FC, 0xAD678846, 0xDA60B8D0,
            0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9, 0x5005713C, 0x270241AA,
            0xBE0B1010, 0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
            0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17, 0x2EB40D81,
            0xB7BD5C3B, 0xC0BA6CAD, 0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A,
            0xEAD54739, 0x9DD277AF, 0x04DB2615, 0x73DC1683, 0xE3630B12, 0x94643B84,
            0x0D6D6A3E, 0x7A6A5AA8, 0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1,
            0xF00F9344, 0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB,
            0x196C3671, 0x6E6B06E7, 0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC,
            0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5, 0xD6D6A3E8, 0xA1D1937E,
            0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1, 0xA6BC5767, 0x3FB506DD, 0x48B2364B,
            0xD80D2BDA, 0xAF0A1B4C, 0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55,
            0x316E8EEF, 0x4669BE79, 0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236,
            0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 0xC5BA3BBE, 0xB2BD0B28,
            0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D,
            0x9B64C2B0, 0xEC63F226, 0x756AA39C, 0x026D930A, 0x9C0906A9, 0xEB0E363F,
            0x72076785, 0x05005713, 0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38,
            0x92D28E9B, 0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21, 0x86D3D2D4, 0xF1D4E242,
            0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1, 0x18B74777,
            0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C, 0x8F659EFF, 0xF862AE69,
            0x616BFFD3, 0x166CCF45, 0xA00AE278, 0xD70DD2EE, 0x4E048354, 0x3903B3C2,
            0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB, 0xAED16A4A, 0xD9D65ADC,
            0x40DF0B66, 0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
            0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605, 0xCDD70693,
            0x54DE5729, 0x23D967BF, 0xB3667A2E, 0xC4614AB8, 0x5D681B02, 0x2A6F2B94,
            0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B, 0x2D02EF8D
        };


        
        
        public static int IndexOf(byte[] buffer, byte val, int offset, int count)
        {
            
            

#if false 

            
            
            

            return Array.IndexOf<byte>(buffer, val, offset, count);

#else 

            unsafe 
            {
                

                
                

                
                
                
                
                
                
                
                
                

                

                
                
                
                
                

                fixed (byte * pB = buffer)
                {
                    var p = pB + offset;

                    
                    while (((int)p & 3) != 0)
                    {
                        if (count == 0)
                            return -1;

                        if (*p == val)
                            return (int) (p - pB);

                        count--;
                        p++;
                    }

                    var mask = (uint)val + ((uint)val << 8);
                    mask += (mask << 16);

                    while (count >= 32)
                    {
                        offset = 0;
                        var t1 = *(uint*)p ^ mask;
                        if ((((t1 ^ 0xffffffff) ^ (0x7efefeff + t1)) & 0x81010100) == 0)
                        {
                            
                            
                            
                            offset += 4;
                            t1 = *(uint*)(p + 4) ^ mask;
                            if ((((t1 ^ 0xffffffff) ^ (0x7efefeff + t1)) & 0x81010100) == 0)
                            {
                                offset += 4;
                                t1 = *(uint*)(p + 8) ^ mask;
                                if ((((t1 ^ 0xffffffff) ^ (0x7efefeff + t1)) & 0x81010100) == 0)
                                {
                                    offset += 4;
                                    t1 = *(uint*)(p + 12) ^ mask;
                                    if ((((t1 ^ 0xffffffff) ^ (0x7efefeff + t1)) & 0x81010100) == 0)
                                    {
                                        offset += 4;
                                        t1 = *(uint*)(p + 16) ^ mask;
                                        if ((((t1 ^ 0xffffffff) ^ (0x7efefeff + t1)) & 0x81010100) == 0)
                                        {
                                            offset += 4;
                                            t1 = *(uint*)(p + 20) ^ mask;
                                            if ((((t1 ^ 0xffffffff) ^ (0x7efefeff + t1)) & 0x81010100) == 0)
                                            {
                                                offset += 4;
                                                t1 = *(uint*)(p + 24) ^ mask;
                                                if ((((t1 ^ 0xffffffff) ^ (0x7efefeff + t1)) & 0x81010100) == 0)
                                                {
                                                    offset += 4;
                                                    t1 = *(uint*)(p + 28) ^ mask;
                                                    if ((((t1 ^ 0xffffffff) ^ (0x7efefeff + t1)) & 0x81010100) == 0)
                                                    {
                                                        p += 32;
                                                        count -= 32;
                                                        continue;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        

                        
                        

                        
                        
                        

                        p += offset;

                        var foundIndex1 = (int) (p - pB);
                        if (p[0] == val)
                            return foundIndex1;
                        else if (p[1] == val)
                            return foundIndex1 + 1;
                        else if (p[2] == val)
                            return foundIndex1 + 2;
                        else if (p[3] == val)
                            return foundIndex1 + 3;

                        
                        p += 4;
                        count -= offset + 4;
                    }

                    
                    while (count != 0)
                    {
                        if (*p == val)
                            return (int) (p - pB);

                        count--;
                        p++;
                    }

                    return -1;
                }
            }
#endif 
        }

        
        
        
        
        
        
        
        public static void ValidateStringArgumentIsAscii(string value)
        {
            if (!IsStringArgumentAscii(value))
            {
                throw new ArgumentException(Strings.StringArgumentMustBeAscii);
            }
        }

        public static bool IsStringArgumentAscii(string value)
        {
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] >= 0x0080)
                {
                    return false;
                }
            }

            return true;
        }

        
        
        
        
        
        public static byte[] AsciiStringToBytes(string value)
        {
            var bytes = new byte[value.Length];
            for (var i = 0; i < value.Length; i++)
            {
                bytes[i] = value[i] < 0x0080 ? (byte) value[i] : (byte) '?';
            }
            return bytes;
        }

        
        
        
        
        
        
        public static void AsciiStringToBytes(string value, byte[] bytes, int offset)
        {
            InternalDebug.Assert(value.Length <= bytes.Length - offset);

            for (var i = 0; i < value.Length; i++)
            {
                bytes[offset + i] = value[i] < 0x0080 ? (byte) value[i] : (byte) '?';
            }
        }

        
        
        
        
        
        
        
        
        internal static int CompareI(byte[] str1, byte[] str2)
        {
            var index = -1;
            do
            {
                index++;
                if (index == str1.Length)
                {
                    return (index < str2.Length) ? -1 : 0;
                }
                else if (index == str2.Length)
                {
                    return 1;
                }
            }
            while (LowerC[str1[index]] == LowerC[str2[index]]);
            
            return (LowerC[str1[index]] < LowerC[str2[index]]) ? -1 : 1;
        }

        
        
        
        
        
        
        
        internal static bool SubstringEquals(byte[] str1, int str1Offset, byte[] str2)
        {
            if (str1.Length - str1Offset < str2.Length)
            {
                return false;
            }

            var str2Length = str2.Length;

            var i = 0;
            while (--str2Length >= 0)
            {
                if (str1[str1Offset++] != str2[i++])
                {
                    return false;
                }
            }

            return true;
        }

        
        
        
        
        
        
        
        
        internal static bool EqualsOrdinalIgnoreCase(string str1, byte[] str2, int str2Offset, int str2Length)
        {
            

            if (str1.Length != str2Length)
            {
                return false;
            }

            var i = 0;
            while (--str2Length >= 0)
            {
                if (LowerC[str1[i++]] != LowerC[str2[str2Offset++]])
                {
                    return false;
                }
            }

            return true;
        }

        
        
        
        
        
        
        
        
        
        internal static int CompareNI(byte[] str1, byte[] str2, int count)
        {
            var index = -1;
            do
            {
                index++;
                if (index == count)
                {
                    return 0;
                }
                else if (index == str1.Length)
                {
                    return (index < str2.Length) ? -1 : 0;
                }
                else if (index == str2.Length)
                {
                    return 1;
                }
            }
            while (LowerC[str1[index]] == LowerC[str2[index]]);
            
            return (LowerC[str1[index]] < LowerC[str2[index]]) ? -1 : 1;
        }

        
        
        
        
        
        
        
        internal static uint ComputeCrcI(byte[] bytes, int offset, int length)
        {
            uint checksum = 0;

            length ++;
            while (--length != 0)
            {
                checksum = ComputeCrc(checksum, LowerC[bytes[offset++]]);
            }

            return checksum;
        }

        
        
        
        
        
        internal static uint ComputeCrcI(string value)
        {
            uint checksum = 0;

            var offset = 0;
            var length = value.Length + 1;
            while (--length != 0)
            {
                var ch = value[offset++];
                checksum = ComputeCrc(checksum, LowerC[ch < 0x0080 ? ch : '?']);
            }

            return checksum;
        }

        
        
        
        
        
        
        
        internal static uint ComputeCrc(byte[] bytes, int offset, int length)
        {
            uint checksum = 0;

            length ++;
            while (--length != 0)
            {
                checksum = ComputeCrc(checksum, bytes[offset++]);
            }

            return checksum;
        }

        
        
        
        
        
        
        private static uint ComputeCrc(uint seed, byte ch)
        {
            return CrcTable[(seed ^ ch) & 0xff] ^ (seed >> 8);
        }
    }
}

