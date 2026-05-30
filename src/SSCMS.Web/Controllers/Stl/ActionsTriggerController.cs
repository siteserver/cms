using System;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Stl
{
    [OpenApiIgnore]
    [Route(Constants.ApiPrefix + Constants.ApiStlPrefix)]
    public partial class ActionsTriggerController : ControllerBase
    {
        private const int RateLimitWindowMinutes = 1;
        private const int RateLimitMaxRequests = 60;

        private readonly ICreateManager _createManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ISettingsManager _settingsManager;
        private readonly ICacheManager _cacheManager;

        public ActionsTriggerController(ICreateManager createManager, IPathManager pathManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ISettingsManager settingsManager, ICacheManager cacheManager)
        {
            _createManager = createManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _settingsManager = settingsManager;
            _cacheManager = cacheManager;
        }

        public class GetRequest : ChannelRequest
        {
            public int ContentId { get; set; }
            public int FileTemplateId { get; set; }
            public int SpecialId { get; set; }
            public bool IsRedirect { get; set; }
            public string ReturnUrl { get; set; }
            public string Token { get; set; }
        }

        private static string GetTriggerTokenPayload(int siteId, int channelId, int contentId, int fileTemplateId, int specialId, bool isRedirect)
        {
            return $"{siteId}:{channelId}:{contentId}:{fileTemplateId}:{specialId}:{isRedirect}";
        }

        public static string GetTriggerTokenPayload(GetRequest request)
        {
            return request == null
                ? string.Empty
                : GetTriggerTokenPayload(request.SiteId, request.ChannelId, request.ContentId, request.FileTemplateId, request.SpecialId, request.IsRedirect);
        }

        private bool IsValidTriggerToken(GetRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Token)) return false;

            try
            {
                return _settingsManager.Decrypt(request.Token) == GetTriggerTokenPayload(request);
            }
            catch
            {
                return false;
            }
        }

        private class RateLimitState
        {
            public int Count { get; set; }
            public DateTime ExpireAt { get; set; }
        }

        private static string GetRateLimitCacheKey(string token, string ipAddress)
        {
            return CacheUtils.GetClassKey(typeof(ActionsTriggerController), "Rate", token ?? string.Empty, ipAddress ?? "unknown");
        }

        private bool TryConsumeRequestQuota(string token, string ipAddress, out int retryAfterSeconds)
        {
            retryAfterSeconds = 0;
            var cacheKey = GetRateLimitCacheKey(token, ipAddress);
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
