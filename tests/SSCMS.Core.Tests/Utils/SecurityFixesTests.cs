using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using SSCMS.Core.Utils;
using SSCMS.Utils;
using Xunit;

namespace SSCMS.Core.Tests.Utils
{
    /// <summary>
    /// GJ20260921 / GJ20260924 修复的单元测试。
    /// </summary>
    public class SecurityFixesTests
    {
        private const string Ip = "203.0.113.10";

        // ---------- SmsCodeGuard ----------

        [Fact]
        public void Sms_CorrectCode_Passes_And_IsOneTime()
        {
            var cache = new FakeCacheManager();
            SmsCodeGuard.SaveCode(cache, "k", 123456);

            Assert.True(SmsCodeGuard.Verify(cache, "k", "123456", Ip).Success);
            Assert.False(SmsCodeGuard.Verify(cache, "k", "123456", Ip).Success);
        }

        [Fact]
        public void Sms_FiveFailures_InvalidateCode()
        {
            var cache = new FakeCacheManager();
            SmsCodeGuard.SaveCode(cache, "k", 123456);

            for (var i = 0; i < SmsCodeGuard.MaxFailuresPerCode; i++)
            {
                Assert.False(SmsCodeGuard.Verify(cache, "k", "000000", Ip).Success);
            }

            // 第 6 次即使输入正确也失败，必须重新发码
            Assert.False(SmsCodeGuard.Verify(cache, "k", "123456", Ip).Success);
        }

        [Fact]
        public void Sms_ExpiredCode_Fails_WithSameMessage()
        {
            var cache = new FakeCacheManager();
            cache.Set("SSCMS.Core.Utils.SmsCodeGuard.code.k", $"123456|{DateTime.UtcNow.AddSeconds(-1).Ticks}");

            var expired = SmsCodeGuard.Verify(cache, "k", "123456", Ip);
            SmsCodeGuard.SaveCode(cache, "k2", 123456);
            var wrong = SmsCodeGuard.Verify(cache, "k2", "654321", Ip);

            Assert.False(expired.Success);
            Assert.Equal(wrong.ErrorMessage, expired.ErrorMessage);
        }

        [Fact]
        public void Sms_ResendWithin60Seconds_Rejected()
        {
            var cache = new FakeCacheManager();
            Assert.True(SmsCodeGuard.TryBeginSend(cache, "k", Ip).Success);
            Assert.False(SmsCodeGuard.TryBeginSend(cache, "k", Ip).Success);
        }

        [Fact]
        public void Sms_IpFailureLimit_BlocksAcrossMobiles()
        {
            var cache = new FakeCacheManager();
            for (var i = 0; i < SmsCodeGuard.MaxFailuresPerIpPerHour; i++)
            {
                SmsCodeGuard.Verify(cache, "m" + i, "000000", Ip);
            }
            SmsCodeGuard.SaveCode(cache, "last", 111111);
            Assert.False(SmsCodeGuard.Verify(cache, "last", "111111", Ip).Success);
            Assert.True(SmsCodeGuard.Verify(cache, "last", "111111", "198.51.100.1").Success);
        }

        [Fact]
        public void Sms_NewCode_IsSixDigits()
        {
            for (var i = 0; i < 100; i++)
            {
                var code = SmsCodeGuard.NewCode();
                Assert.InRange(code, 100000, 999999);
            }
        }

        // ---------- PasswordHashUtils ----------

        [Fact]
        public void Pbkdf2_Verify_RoundTrip()
        {
            var md5 = PasswordHashUtils.ToMd5("ADMIN@@14049");
            var hash = PasswordHashUtils.Hash(md5, out var salt);

            Assert.StartsWith("pbkdf2-sha256$", salt);
            Assert.True(PasswordHashUtils.Verify(md5, hash, salt));
            Assert.True(PasswordHashUtils.Verify(md5.ToUpperInvariant(), hash, salt));
            Assert.False(PasswordHashUtils.Verify(PasswordHashUtils.ToMd5("wrong"), hash, salt));
        }

        [Fact]
        public void Pbkdf2_SamePassword_DifferentSalt()
        {
            var md5 = PasswordHashUtils.ToMd5("same");
            var h1 = PasswordHashUtils.Hash(md5, out var s1);
            var h2 = PasswordHashUtils.Hash(md5, out var s2);
            Assert.NotEqual(s1, s2);
            Assert.NotEqual(h1, h2);
        }

        [Fact]
        public void Pbkdf2_MalformedSalt_Fails()
        {
            var md5 = PasswordHashUtils.ToMd5("x");
            var hash = PasswordHashUtils.Hash(md5, out _);
            Assert.False(PasswordHashUtils.Verify(md5, hash, "9KjWcLKz5AVJCynH0S9XfQ=="));
            Assert.False(PasswordHashUtils.Verify(md5, "not-base64!", "pbkdf2-sha256$1000$AAAA"));
        }

        [Fact]
        public void Legacy_Des_Password_IsReversible_ThatIsWhyWeMigrate()
        {
            // 报告中的复现：DES 口令仅凭盐值即可还原，证明必须迁移为 PBKDF2
            const string salt = "9KjWcLKz5AVJCynH0S9XfQ==";
            var stored = DesEncryptor.EncryptStringBySecretKey("ADMIN@@14049", salt);
            Assert.Equal("ADMIN@@14049", DesEncryptor.DecryptStringBySecretKey(stored, salt));
        }

        // ---------- CaptchaUtils ----------

        [Fact]
        public void Captcha_ServerSide_OneTime()
        {
            var cache = new FakeCacheManager();
            var token = CaptchaUtils.Issue(cache);
            var value = CaptchaUtils.GetValue(cache, token);

            Assert.Matches("^[0-9a-f]{32}$", token);
            Assert.NotNull(value);
            Assert.True(CaptchaUtils.Validate(cache, token, value.ToLowerInvariant()));
            Assert.False(CaptchaUtils.Validate(cache, token, value));
        }

        [Fact]
        public void Captcha_WrongValue_ConsumesToken()
        {
            var cache = new FakeCacheManager();
            var token = CaptchaUtils.Issue(cache);
            var value = CaptchaUtils.GetValue(cache, token);

            Assert.False(CaptchaUtils.Validate(cache, token, "ZZZZ"));
            Assert.False(CaptchaUtils.Validate(cache, token, value));
        }

        [Fact]
        public void Captcha_ForgedToken_Rejected()
        {
            var cache = new FakeCacheManager();
            Assert.Null(CaptchaUtils.GetValue(cache, "{\"Value\":\"AAAA\",\"ExpireAt\":\"2099-01-01\"}"));
            Assert.False(CaptchaUtils.Validate(cache, new string('a', 32), "AAAA"));
        }

        // ---------- IP ----------

        [Fact]
        public void GetIpAddress_IgnoresSpoofedHeaders()
        {
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse(Ip);
            context.Request.Headers["X-Forwarded-For"] = "10.0.0.1";
            context.Request.Headers["X-Real-IP"] = "10.0.0.2";
            context.Request.Headers["HTTP_X_FORWARDED_FOR"] = "10.0.0.3";

            Assert.Equal(Ip, PageUtils.GetIpAddress(context.Request));
        }

        [Fact]
        public void GetIpAddress_MapsIpv4MappedIpv6()
        {
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse(Ip).MapToIPv6();
            Assert.Equal(Ip, PageUtils.GetIpAddress(context.Request));
        }
    }
}
