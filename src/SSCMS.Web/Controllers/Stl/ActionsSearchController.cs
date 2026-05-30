using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Models;
using SSCMS.Core.Utils;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers;

namespace SSCMS.Web.Controllers.Stl
{
    [OpenApiIgnore]
    [Route(Constants.ApiPrefix + Constants.ApiStlPrefix)]
    public partial class ActionsSearchController : ControllerBase
    {
        private const int RateLimitWindowMinutes = 1;
        private const int RateLimitMaxRequests = 60;

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IParseManager _parseManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ICacheManager _cacheManager;

        public ActionsSearchController(ISettingsManager settingsManager, IAuthManager authManager, IParseManager parseManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IContentRepository contentRepository, ICacheManager cacheManager)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _parseManager = parseManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
            _cacheManager = cacheManager;
        }

        private class RateLimitState
        {
            public int Count { get; set; }
            public DateTime ExpireAt { get; set; }
        }

        private static string GetRateLimitCacheKey(string ipAddress)
        {
            return CacheUtils.GetClassKey(typeof(ActionsSearchController), "Rate", ipAddress ?? "unknown");
        }

        public static string GetHighlightRegexPattern(string word)
        {
            var escapedWord = Regex.Escape(word).Replace("\\ ", "\\s");
            return $"({escapedWord})(?!</a>)(?![^><]*>)";
        }

        private bool TryConsumeRequestQuota(string ipAddress, out int retryAfterSeconds)
        {
            retryAfterSeconds = 0;
            var cacheKey = GetRateLimitCacheKey(ipAddress);
            lock (RateLimitLockManager.GetLock(cacheKey))
            {
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

        private static NameValueCollection GetPostCollection(StlSearchRequest request)
        {
            var formCollection = new NameValueCollection();
            if (request != null)
            {
                foreach (var key in request.GetKeys())
                {
                    var value = request.Get(key);
                    if (value != null)
                    {
                        formCollection[key] = request.Get(key).ToString();
                    }
                }
            }

            return formCollection;
        }
    }
}
