using System.Security.Cryptography;
using System.Text;

namespace SSCMS.Utils
{
    /// <summary>
    /// 常量时间字符串比较，避免通过响应时间逐字节猜测密钥/签名。
    /// </summary>
    public static class SecureCompareUtils
    {
        public static bool FixedTimeEquals(string a, string b)
        {
            if (a == null || b == null) return false;
            var bytesA = Encoding.UTF8.GetBytes(a);
            var bytesB = Encoding.UTF8.GetBytes(b);
            return CryptographicOperations.FixedTimeEquals(bytesA, bytesB);
        }
    }
}
