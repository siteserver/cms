using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Route(Constants.ApiAdminPrefix)]
    public partial class IndexController : ControllerBase
    {
        private const string Route = "index";
        private const string RouteActionsSetLanguage = "index/actions/setLanguage";
        private const string RouteActionsCache = "index/actions/cache";

        //private readonly IStringLocalizer<IndexController> _local;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ICloudManager _cloudManager;
        private readonly IPathManager _pathManager;
        private readonly IPluginManager _pluginManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IDbCacheRepository _dbCacheRepository;
        private readonly IFormRepository _formRepository;

        public IndexController(ISettingsManager settingsManager, IAuthManager authManager, ICloudManager cloudManager, IPathManager pathManager, IPluginManager pluginManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IDbCacheRepository dbCacheRepository, IFormRepository formRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _cloudManager = cloudManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _dbCacheRepository = dbCacheRepository;
            _formRepository = formRepository;
        }

        public class Local
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string AvatarUrl { get; set; }
            public string Level { get; set; }
        }

        public class GetRequest : SiteRequest
        {
            public string SessionId { get; set; }
        }

        public class GetPlugin
        {
            public string PluginId { get; set; }
            public string DisplayName { get; set; }
            public string Version { get; set; }
        }

        public class GetResult
        {
            public bool Value { get; set; }
            public string RedirectUrl { get; set; }
            public string CmsVersion { get; set; }
            public string OSArchitecture { get; set; }
            public bool IsCloudAdmin { get; set; }
            public string AdminFaviconUrl { get; set; }
            public string AdminLogoUrl { get; set; }
            public string AdminLogoLinkUrl { get; set; }
            public string AdminTitle { get; set; }
            public bool IsSuperAdmin { get; set; }
            public string Culture { get; set; }
            public List<GetPlugin> Plugins { get; set; }
            public IList<Menu> Menus { get; set; }
            public SiteType SiteType { get; set; }
            public string SiteUrl { get; set; }
            public string PreviewUrl { get; set; }
            public Local Local { get; set; }
            public bool IsSafeMode { get; set; }
            public CloudType CloudType { get; set; }
            public string CloudUserName { get; set; }
            public string CloudToken { get; set; }
            public List<string> CssUrls { get; set; }
            public List<string> JsUrls { get; set; }
        }

        public class SetLanguageRequest
        {
            public string Culture { get; set; }
        }

        private IList<Menu> GetChildren(Menu menu, IList<string> permissions, Func<Menu, Menu> op = null)
        {
            if (menu.Children == null || menu.Children.Count == 0) return null;

            foreach (var child in menu.Children)
            {
                child.Children = GetChildren(child, permissions, op);
            }

            var children = new List<Menu>(menu.Children);

            if (op != null)
            {
                children = children.Select(op).ToList();
            }

            return children.Where(x => _authManager.IsMenuValid(x, permissions)).ToList();
        }

        private async Task<(bool redirect, string redirectUrl)> AdminRedirectCheckAsync()
        {
            var redirect = false;
            var redirectUrl = string.Empty;

            var config = await _configRepository.GetAsync();

            if (string.IsNullOrEmpty(_settingsManager.Database.ConnectionString) || await _configRepository.IsNeedInstallAsync())
            {
                redirect = true;
                redirectUrl = _pathManager.GetAdminUrl(InstallController.Route);
            }
            else if (config.Initialized &&
                     config.DatabaseVersion != _settingsManager.Version)
            {
                redirect = true;
                redirectUrl = _pathManager.GetAdminUrl(SyncDatabaseController.Route);
            }

            return (redirect, redirectUrl);
        }




    }
}