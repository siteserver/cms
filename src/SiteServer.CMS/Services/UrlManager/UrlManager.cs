using Microsoft.AspNetCore.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Services
{
    public partial class UrlManager : IUrlManager
    {
        private readonly HttpContext _httpContext;
        private readonly ISettingsManager _settingsManager;

        public UrlManager(IHttpContextAccessor httpContextAccessor, ISettingsManager settingsManager)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _settingsManager = settingsManager;
        }

        public string ApplicationPath => StringUtils.TrimEnd(_httpContext.Request.PathBase.Value, Constants.ApiPrefix);

        public string GetRootUrl(string relatedUrl)
        {
            
            return PageUtils.Combine(ApplicationPath, relatedUrl);
        }

        public string GetApiUrl(string route)
        {
            return PageUtils.Combine(ApplicationPath, Constants.ApiPrefix, route);
        }

        public string GetAdminUrl(string relatedUrl)
        {
            return PageUtils.Combine("/", ApplicationPath, _settingsManager.AdminDirectory, relatedUrl, "/");
        }
    }
}