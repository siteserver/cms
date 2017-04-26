using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Top.Api.Util
{
    public abstract class StringUtil
    {
        private const string pattern = @"^\d*$";
        private static readonly Regex REG_CIDR = new Regex("^(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})/(\\d{1,2})$");

        public static bool IsDigits(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, pattern);
        }

        public static string[] Split(string data, char separatorChar)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            return data.Split(new char[] { separatorChar }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string ToCamelStyle(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            char[] chars = name.ToCharArray();
            StringBuilder buf = new StringBuilder(chars.Length);
            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 0)
                {
                    buf.Append(char.ToLower(chars[i]));
                }
                else
                {
                    buf.Append(chars[i]);
                }
            }
            return buf.ToString();
        }

        public static string ToUnderlineStyle(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (char.IsUpper(c))
                {
                    if (i > 0)
                    {
                        buf.Append("_");
                    }
                    buf.Append(char.ToLower(c));
                }
                else
                {
                    buf.Append(c);
                }
            }
            return buf.ToString();
        }

        public static string EscapeXml(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            StringBuilder buf = new StringBuilder();
            char[] chars = value.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                switch (c)
                {
                    case '<':
                        buf.Append("&lt;");
                        break;
                    case '>':
                        buf.Append("&gt;");
                        break;
                    case '\'':
                        buf.Append("&apos;");
                        break;
                    case '&':
                        buf.Append("&amp;");
                        break;
                    case '"':
                        buf.Append("&quot;");
                        break;
                    default:
                        if ((c == 0x9) || (c == 0xA) || (c == 0xD) || ((c >= 0x20) && (c <= 0xD7FF))
                        || ((c >= 0xE000) && (c <= 0xFFFD)) || ((c >= 0x10000) && (c <= 0x10FFFF)))
                        {
                            buf.Append(c);
                        }
                        break;
                }
            }

            return buf.ToString();
        }

        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString(Constants.DATE_TIME_FORMAT);
        }

        public static bool IsIpInRange(string ipAddr, string cidrAddr)
        {
            Match match = REG_CIDR.Match(cidrAddr);
            if (!match.Success)
            {
                throw new TopException("Invalid CIDR address: " + cidrAddr);
            }

            int[] minIpParts = new int[4];
            int[] maxIpParts = new int[4];
            string[] ipParts = match.Groups[1].Value.Split(new string[1] { "." }, StringSplitOptions.None);
            int intMask = int.Parse(match.Groups[2].Value);

            for (int i = 0; i < ipParts.Length; i++)
            {
                int ipPart = int.Parse(ipParts[i]);
                if (intMask > 8)
                {
                    minIpParts[i] = ipPart;
                    maxIpParts[i] = ipPart;
                    intMask -= 8;
                }
                else if (intMask > 0)
                {
                    minIpParts[i] = ipPart >> intMask;
                    maxIpParts[i] = ipPart | (0xFF >> intMask);
                    intMask = 0;
                }
                else
                {
                    minIpParts[i] = 1;
                    maxIpParts[i] = 0xFF - 1;
                }
            }

            string[] realIpParts = ipAddr.Split(new string[1] { "." }, StringSplitOptions.None);
            for (int i = 0; i < realIpParts.Length; i++)
            {
                int realIp = int.Parse(realIpParts[i]);
                if (realIp < minIpParts[i] || realIp > maxIpParts[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
