using System;
using System.Security.Cryptography;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    /// <summary>
    /// 安装流程一次性令牌：替代原先直接返回 SecurityKey 的做法。
    /// 令牌仅保存在当前进程内存中，30 分钟过期，校验成功即作废。
    /// </summary>
    public static class InstallTokenUtils
    {
        private static readonly object Lock = new object();
        private static string _token;
        private static DateTime _expiresAt = DateTime.MinValue;

        public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(30);

        public static string Issue()
        {
            var bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            var token = Convert.ToHexString(bytes).ToLowerInvariant();
            lock (Lock)
            {
                _token = token;
                _expiresAt = DateTime.UtcNow.Add(Lifetime);
            }
            return token;
        }

        public static bool Consume(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;
            lock (Lock)
            {
                if (_token == null || DateTime.UtcNow > _expiresAt) return false;
                if (!SecureCompareUtils.FixedTimeEquals(_token, token)) return false;
                _token = null;
                _expiresAt = DateTime.MinValue;
                return true;
            }
        }
    }
}
