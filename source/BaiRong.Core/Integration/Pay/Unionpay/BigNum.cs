using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace SiteServer.B2C.Core.Union
{
    public class BigNum
    {
        const UInt64 base32 = 65536ul * 65536;
        const UInt64 base32_10 = 10000 * 100000;
        const int wLen = 8;
        static readonly UInt64[] baseMod = { 1, 294967296, 709551616, 543950336, 768211456, 932542976, 34512896, 610249216,
                                         129639936, 375533056, 86936576, 746218496, 990306816, 725889536, 628614656, 306290176 };
        static readonly char[] trimZero = { '0' };

        public static UInt64[] ConvertFromHex(string s)
        {
            int str_len = s.Length;
            int cut_pos = str_len - wLen;

            int part_count = ((str_len - 1) / wLen) + 1;
            UInt64[] ret = new UInt64[part_count];
            int idx = part_count - 1;

            while (cut_pos >= 0)
            {

                string part = s.Substring(cut_pos, wLen);
                ret[idx] = Convert.ToUInt64(part, 16);
                --idx;
                cut_pos -= wLen;
            }
            if (cut_pos > -wLen)
            {
                ret[0] = Convert.ToUInt64(s.Substring(0, cut_pos + wLen), 16);
            }
            return ret;
        }

        public static string ToDecimalStr(UInt64[] ll)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            List<string> li_str = new List<string>();

            while (ll.Length > 0 && (ll[0] != 0))
            {
                UInt64 m = Mod(ll);
                ll = Div(ll);
                li_str.Add(m.ToString("D9"));
            }
            for (int i = li_str.Count - 1; i >= 0; --i)
            {
                sb.Append(li_str[i]);
            }
            if (sb.Length == 0) { return "0"; }

            return sb.ToString().TrimStart(trimZero);
        }

        private static UInt64[] Div(UInt64[] ll)
        {
            List<UInt64> b10 = new List<UInt64>();
            int ubound = ll.GetUpperBound(0);
            int idx = 0;
            UInt64 l = 0, m = 0, d = 0;
            bool first = true;

            while (idx <= ubound)
            {
                l = ll[idx] + m;
                m = l % base32_10;
                d = l / base32_10;
                if (!(first && d == 0))
                {
                    b10.Add(d);
                    first = false;
                }
                m *= base32;
                ++idx;
            }
            return b10.ToArray();
        }

        private static UInt64 Mod(UInt64[] ll)
        {
            int ubound = ll.GetUpperBound(0);
            int idx = 0;
            UInt64 l = 0;
            UInt64 ret = ll[ubound] % base32_10;

            while (idx < ubound)
            {

                l = ll[idx] % base32_10;
                l *= GetBaseMod(ubound - idx);
                l %= base32_10;
                ret += l;
                ++idx;
            }
            return ret % base32_10;
        }

        private static UInt64 GetBaseMod(int pow)
        {
            return baseMod[pow]; //pre calc
        }

    }
}