using System;
using SSCMS.Core.Utils;
using SSCMS.Services;

namespace SSCMS.Web.Controllers
{
    internal static class SmsRateLimitUtils
    {
        private const int DefaultSendSmsMaxCount = 5;
        private const int DefaultSendSmsWindowMinutes = 10;

        private class SendSmsRateLimitState
        {
            public int Count { get; set; }
            public DateTime ExpireAt { get; set; }
        }

        public static bool TryConsume(
            ICacheManager cacheManager,
            Type ownerType,
            string mobile,
            string ipAddress,
            out int retryAfterSeconds)
        {
            retryAfterSeconds = 0;
            var cacheKey = CacheUtils.GetClassKey(ownerType, "SendSmsRate",
                (mobile ?? string.Empty).Trim(), ipAddress ?? "unknown");
            var state = cacheManager.Get<SendSmsRateLimitState>(cacheKey);
            if (state == null || state.ExpireAt <= DateTime.Now)
            {
                state = new SendSmsRateLimitState
                {
                    Count = 0,
                    ExpireAt = DateTime.Now.AddMinutes(DefaultSendSmsWindowMinutes)
                };
            }

            state.Count++;
            cacheManager.AddOrUpdateAbsolute(cacheKey, state, DefaultSendSmsWindowMinutes);
            if (state.Count <= DefaultSendSmsMaxCount) return true;

            retryAfterSeconds = (int)Math.Max(1, Math.Ceiling((state.ExpireAt - DateTime.Now).TotalSeconds));
            return false;
        }
    }
}
