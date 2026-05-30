using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AgentController : ControllerBase
    {
        private const int SecurityKeyRateLimitWindowMinutes = 10;
        private const int SecurityKeyRateLimitMaxFailures = 10;

        public const string Route = "agent";
        private const string RouteSites = "agent/sites";
        private const string RoutePlugins = "agent/plugins";
        private const string RouteInstall = "agent/actions/install";
        private const string RouteSetDomain = "agent/actions/setDomain";
        private const string RouteAddSite = "agent/actions/addSite";
        private const string RouteProcess = "agent/actions/process";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ICacheManager _cacheManager;
        private readonly ICreateManager _createManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IDbCacheRepository _dbCacheRepository;
        private readonly IContentRepository _contentRepository;

        public AgentController(ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager, IPluginManager pluginManager, ICacheManager cacheManager, ICreateManager createManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository, ISiteRepository siteRepository, IDbCacheRepository dbCacheRepository, IContentRepository contentRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _cacheManager = cacheManager;
            _createManager = createManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
            _dbCacheRepository = dbCacheRepository;
            _contentRepository = contentRepository;
        }

        public class AgentRequest
        {
            public string SecurityKey { get; set; }
        }

        public class InstallRequest : AgentRequest
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public class SitesResult
        {
            public List<Site> Sites { get; set; }
            public int RootSiteId { get; set; }
        }

        public class SetDomainRequest : AgentRequest
        {
            public string HostDomain { get; set; }
            public int SiteId { get; set; }
            public string SiteDomain { get; set; }
        }

        public class AddSiteRequest : AgentRequest
        {
            public string SiteName { get; set; }
            public bool Root { get; set; }
            public int ParentId { get; set; }
            public string SiteDir { get; set; }
            public string ThemeDownloadUrl { get; set; }
            public string Guid { get; set; }
        }

        public class AddSiteResult
        {
            public Site Site { get; set; }
        }

        public class ProcessRequest : AgentRequest
        {
            public string Guid { get; set; }
        }

        public class AgentPlugin
        {
            public string Publisher { get; set; }
            public string Name { get; set; }
            public string Version { get; set; }
        }

        public class PluginsResult
        {
            public List<AgentPlugin> Plugins { get; set; }
        }

        private class SecurityKeyRateLimitState
        {
            public int Count { get; set; }
            public DateTime ExpireAt { get; set; }
        }

        private static string GetSecurityKeyRateLimitCacheKey(string ipAddress)
        {
            return CacheUtils.GetClassKey(typeof(AgentController), "SecurityKey", ipAddress ?? "unknown");
        }

        private static bool FixedTimeEquals(string left, string right)
        {
            if (left == null || right == null) return false;

            var leftHash = SHA256.HashData(Encoding.UTF8.GetBytes(left));
            var rightHash = SHA256.HashData(Encoding.UTF8.GetBytes(right));
            return CryptographicOperations.FixedTimeEquals(leftHash, rightHash);
        }

        private bool TryValidateAgentSecurityKey(string securityKey, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(securityKey))
            {
                errorMessage = "系统参数不足";
                return false;
            }

            var cacheKey = GetSecurityKeyRateLimitCacheKey(PageUtils.GetIpAddress(Request));
            var state = _cacheManager.Get<SecurityKeyRateLimitState>(cacheKey);
            if (state != null && state.ExpireAt <= DateTime.Now)
            {
                state = null;
            }

            if (state != null && state.Count >= SecurityKeyRateLimitMaxFailures)
            {
                var retryAfterSeconds = (int)Math.Max(1, Math.Ceiling((state.ExpireAt - DateTime.Now).TotalSeconds));
                errorMessage = $"请求过于频繁，请在{retryAfterSeconds}秒后重试";
                return false;
            }

            if (FixedTimeEquals(_settingsManager.SecurityKey, securityKey))
            {
                if (state != null)
                {
                    _cacheManager.Remove(cacheKey);
                }
                return true;
            }

            if (state == null)
            {
                state = new SecurityKeyRateLimitState
                {
                    Count = 0,
                    ExpireAt = DateTime.Now.AddMinutes(SecurityKeyRateLimitWindowMinutes)
                };
            }

            state.Count++;
            _cacheManager.AddOrUpdateAbsolute(cacheKey, state, SecurityKeyRateLimitWindowMinutes);
            errorMessage = "SecurityKey不正确";
            return false;
        }
    }
}
