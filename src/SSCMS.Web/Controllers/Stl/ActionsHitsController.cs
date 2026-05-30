using System;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Stl
{
    [OpenApiIgnore]
    [Route(Constants.ApiPrefix + Constants.ApiStlPrefix)]
    public partial class ActionsHitsController : ControllerBase
    {
        private const int RateLimitWindowMinutes = 1;
        private const int RateLimitMaxRequests = 60;

        private readonly IContentRepository _contentRepository;
        private readonly ICacheManager _cacheManager;

        public ActionsHitsController(IContentRepository contentRepository, ICacheManager cacheManager)
        {
            _contentRepository = contentRepository;
            _cacheManager = cacheManager;
        }

        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
            public bool AutoIncrease { get; set; }
        }

        private class RateLimitState
        {
            public int Count { get; set; }
            public DateTime ExpireAt { get; set; }
        }

        private static string GetRateLimitCacheKey(string ipAddress)
        {
            return CacheUtils.GetClassKey(typeof(ActionsHitsController), "Rate", ipAddress ?? "unknown");
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
