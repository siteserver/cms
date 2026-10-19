using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    /// <summary>
    /// 安装流程短期令牌：替代原先直接返回 SecurityKey 的做法。
    /// 令牌格式为 "过期时间.随机数.HMAC签名"，签名密钥由 SecurityKey 派生，不在服务端保存状态。
    /// Prepare 写入 sscms.json 时会改写 Web.config，IIS 会因此回收应用进程，
    /// 只存在内存里的令牌会随之丢失，导致 Install 返回 401，所以改为可跨进程校验的签名令牌。
    /// 一次性：同一进程内校验成功后作废；跨进程由 Install 接口的"已安装即拒绝"兜底。
    /// </summary>
    public static class InstallTokenUtils
    {
        private const char Separator = '.';
        private static readonly ConcurrentDictionary<string, long> Consumed = new ConcurrentDictionary<string, long>();

        public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(30);

        private static string Sign(string securityKey, string payload)
        {
            var key = SHA256.HashData(Encoding.UTF8.GetBytes("sscms-install-token:" + securityKey));
            using var hmac = new HMACSHA256(key);
            return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
        }

        public static string Issue(string securityKey)
        {
            return Issue(securityKey, DateTimeOffset.UtcNow);
        }

        public static string Issue(string securityKey, DateTimeOffset now)
        {
            if (string.IsNullOrEmpty(securityKey)) throw new ArgumentException("SecurityKey 不能为空", nameof(securityKey));
            var expires = now.Add(Lifetime).ToUnixTimeSeconds();
            var nonce = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
            var payload = $"{expires}{Separator}{nonce}";
            return payload + Separator + Sign(securityKey, payload);
        }

        public static bool Consume(string securityKey, string token)
        {
            return Consume(securityKey, token, DateTimeOffset.UtcNow);
        }

        public static bool Consume(string securityKey, string token, DateTimeOffset now)
        {
            if (string.IsNullOrEmpty(securityKey) || string.IsNullOrEmpty(token)) return false;
            var parts = token.Split(Separator);
            if (parts.Length != 3 || !long.TryParse(parts[0], out var expires)) return false;

            var payload = parts[0] + Separator + parts[1];
            if (!SecureCompareUtils.FixedTimeEquals(Sign(securityKey, payload), parts[2])) return false;

            var nowSeconds = now.ToUnixTimeSeconds();
            if (nowSeconds > expires) return false;

            // 清理已过期的作废记录，避免字典无限增长
            foreach (var item in Consumed)
            {
                if (item.Value < nowSeconds) Consumed.TryRemove(item.Key, out _);
            }
            return Consumed.TryAdd(parts[1], expires);
        }
    }
}
