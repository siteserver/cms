using System;
using System.Text;

namespace BaiRong.Core.Auth
{
    public class Base64Utils
    {
        public static string Base64Encode(string unencodedText)
        {
            try
            {
                var encodedBytes = Encoding.UTF8.GetBytes(unencodedText);
                return Convert.ToBase64String(encodedBytes);
            }
            catch { }
            return string.Empty;
        }

        public static string Base64Decode(string str)
        {
            try
            {
                var decbuff = Convert.FromBase64String(str);
                return Encoding.UTF8.GetString(decbuff);
            }
            catch { }
            return string.Empty;
        }

        public static string Base64ForUrlEncode(string unencodedText)
        {
            var s = Base64Encode(unencodedText);

            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
        }

        public static string Base64ForUrlDecode(string str)
        {
            var s = str;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
            }

            return Base64Decode(s);
        }
    }
}