using System;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Stl
{
    [OpenApiIgnore]
    [Route(Constants.ApiPrefix + Constants.ApiStlPrefix)]
    public partial class ActionsDynamicController : ControllerBase
    {
        private const int RateLimitWindowMinutes = 1;
        private const int RateLimitMaxRequests = 60;

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IParseManager _parseManager;
        private readonly ICacheManager _cacheManager;

        public ActionsDynamicController(ISettingsManager settingsManager, IAuthManager authManager, IParseManager parseManager, ICacheManager cacheManager)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _parseManager = parseManager;
            _cacheManager = cacheManager;
        }

        public class SubmitRequest
        {
            public string Value { get; set; }
            public int Page { get; set; }
        }

        public class SubmitResult
        {
            public bool Value { get; set; }
            public string Html { get; set; }
        }

        private class RateLimitState
        {
            public int Count { get; set; }
            public DateTime ExpireAt { get; set; }
        }

        private static string GetRateLimitCacheKey(string ipAddress)
        {
            return CacheUtils.GetClassKey(typeof(ActionsDynamicController), "Rate", ipAddress ?? "unknown");
        }

        private bool TryConsumeRequestQuota(string ipAddress, out int retryAfterSeconds)
        {
            retryAfterSeconds = 0;
            var cacheKey = GetRateLimitCacheKey(ipAddress);
            var state = _cacheManager.Get<RateLimitState>(cacheKey);
            if (state == null || state.ExpireAt <= DateTime.Now)
            {
                state = new RateLimitState
                {
                    Count = 0,
                    ExpireAt = DateTime.Now.AddMinutes(RateLimitWindowMinutes)
                };
            }

            state.Count++;
            _cacheManager.AddOrUpdateAbsolute(cacheKey, state, RateLimitWindowMinutes);
            if (state.Count <= RateLimitMaxRequests) return true;

            retryAfterSeconds = (int)Math.Max(1, Math.Ceiling((state.ExpireAt - DateTime.Now).TotalSeconds));
            return false;
        }
    }
}
