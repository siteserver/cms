using System;
using System.Security.Cryptography;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    /// <summary>
    /// 短信验证码防爆破、防滥发（GJ20260921）。
    /// - 验证码由密码学安全随机数生成，有效期 5 分钟；
    /// - 同一手机号 60 秒内只能发一次，单 IP 每小时发送次数有上限；
    /// - 同一手机号连续校验失败 5 次即作废当前验证码，单 IP 每小时校验失败次数有上限；
    /// - 码错误与码过期返回同一提示，比较使用常量时间。
    /// </summary>
    public static class SmsCodeGuard
    {
        public const int CodeMinutes = 5;
        public const int ResendSeconds = 60;
        public const int MaxFailuresPerCode = 5;
        public const int MaxSendsPerIpPerHour = 20;
        public const int MaxFailuresPerIpPerHour = 30;

        public const string ErrorInvalidCode = "输入的验证码有误或验证码已超时，请重新获取验证码";
        public const string ErrorTooFrequent = "验证码发送过于频繁，请稍后再试";
        public const string ErrorTooManyFailures = "验证码错误次数过多，请稍后再试";
        public const string ErrorSendFailed = "验证码发送失败，请稍后重试";

        private const string Prefix = "SSCMS.Core.Utils.SmsCodeGuard.";

        public static int NewCode()
        {
            return RandomNumberGenerator.GetInt32(100000, 1000000);
        }

        /// <summary>
        /// 发送前调用：检查手机号重发间隔与 IP 发送上限，并占用本次发送配额。
        /// 无论手机号是否绑定账号都应调用，避免通过限频差异枚举。
        /// </summary>
        public static (bool Success, string ErrorMessage) TryBeginSend(ICacheManager cache, string codeKey, string ipAddress)
        {
            var resendKey = Prefix + "resend." + codeKey;
            if (GetCounter(cache, resendKey) > 0)
            {
                return (false, ErrorTooFrequent);
            }

            var ipKey = Prefix + "ip.send." + ipAddress;
            if (GetCounter(cache, ipKey) >= MaxSendsPerIpPerHour)
            {
                return (false, ErrorTooFrequent);
            }

            Increment(cache, resendKey, TimeSpan.FromSeconds(ResendSeconds));
            Increment(cache, ipKey, TimeSpan.FromHours(1));
            return (true, string.Empty);
        }

        /// <summary>
        /// 短信发送成功后保存验证码，同时清零该验证码的失败计数。
        /// </summary>
        public static void SaveCode(ICacheManager cache, string codeKey, int code)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(CodeMinutes);
            cache.AddOrUpdateAbsolute(Prefix + "code." + codeKey, $"{code}|{expiresAt.Ticks}", CodeMinutes + 1);
            cache.Remove(Prefix + "fail." + codeKey);
        }

        /// <summary>
        /// 校验验证码。成功时默认作废验证码（一次性）；失败计数达到上限时作废验证码。
        /// </summary>
        public static (bool Success, string ErrorMessage) Verify(ICacheManager cache, string codeKey, string inputCode, string ipAddress, bool consume = true)
        {
            var ipKey = Prefix + "ip.fail." + ipAddress;
            if (GetCounter(cache, ipKey) >= MaxFailuresPerIpPerHour)
            {
                return (false, ErrorTooManyFailures);
            }

            var codeCacheKey = Prefix + "code." + codeKey;
            var stored = cache.Get<string>(codeCacheKey);
            var storedCode = ParseStoredCode(stored);

            if (storedCode == null || string.IsNullOrEmpty(inputCode) || !SecureCompareUtils.FixedTimeEquals(storedCode, inputCode.Trim()))
            {
                Increment(cache, ipKey, TimeSpan.FromHours(1));
                if (storedCode != null)
                {
                    var failKey = Prefix + "fail." + codeKey;
                    var failures = Increment(cache, failKey, TimeSpan.FromMinutes(CodeMinutes));
                    if (failures >= MaxFailuresPerCode)
                    {
                        cache.Remove(codeCacheKey);
                        cache.Remove(failKey);
                    }
                }
                return (false, ErrorInvalidCode);
            }

            if (consume)
            {
                cache.Remove(codeCacheKey);
                cache.Remove(Prefix + "fail." + codeKey);
            }
            return (true, string.Empty);
        }

        /// <summary>
        /// 作废验证码（配合 Verify(consume: false) 使用，业务成功后再作废）。
        /// </summary>
        public static void Invalidate(ICacheManager cache, string codeKey)
        {
            cache.Remove(Prefix + "code." + codeKey);
            cache.Remove(Prefix + "fail." + codeKey);
        }

        /// <summary>
        /// 记录手机号已通过验证（用于注册：先校验验证码，再提交注册）。
        /// </summary>
        public static void MarkVerified(ICacheManager cache, string codeKey)
        {
            Increment(cache, Prefix + "verified." + codeKey, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 消费“手机号已验证”标记，存在则返回 true 并删除。
        /// </summary>
        public static bool ConsumeVerified(ICacheManager cache, string codeKey)
        {
            var key = Prefix + "verified." + codeKey;
            if (GetCounter(cache, key) <= 0) return false;
            cache.Remove(key);
            return true;
        }

        private static string ParseStoredCode(string stored)
        {
            if (string.IsNullOrEmpty(stored)) return null;
            var parts = stored.Split('|');
            if (parts.Length != 2 || !long.TryParse(parts[1], out var ticks)) return null;
            if (DateTime.UtcNow.Ticks > ticks) return null;
            return parts[0];
        }

        // 固定窗口计数器：值为 "count|expiresUtcTicks"，窗口内递增不重置过期时间
        internal static int GetCounter(ICacheManager cache, string key)
        {
            var (count, _) = ReadCounter(cache.Get<string>(key));
            return count;
        }

        internal static int Increment(ICacheManager cache, string key, TimeSpan window)
        {
            var (count, expiresTicks) = ReadCounter(cache.Get<string>(key));
            var now = DateTime.UtcNow.Ticks;
            if (count == 0)
            {
                expiresTicks = now + window.Ticks;
            }
            count++;
            var minutes = (int)Math.Ceiling(TimeSpan.FromTicks(expiresTicks - now).TotalMinutes);
            if (minutes < 1) minutes = 1;
            cache.AddOrUpdateAbsolute(key, $"{count}|{expiresTicks}", minutes);
            return count;
        }

        private static (int Count, long ExpiresTicks) ReadCounter(string value)
        {
            if (string.IsNullOrEmpty(value)) return (0, 0);
            var parts = value.Split('|');
            if (parts.Length != 2 || !int.TryParse(parts[0], out var count) || !long.TryParse(parts[1], out var ticks)) return (0, 0);
            if (DateTime.UtcNow.Ticks > ticks) return (0, 0);
            return (count, ticks);
        }
    }
}
