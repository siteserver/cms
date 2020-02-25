using System;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions;
using SS.CMS;
using SS.CMS.Core;

namespace SS.CMS.Services
{
    public partial class PathManager : IPathManager
    {
        private readonly HttpContext _httpContext;
        private readonly ISettingsManager _settingsManager;

        public PathManager(IHttpContextAccessor httpContextAccessor, ISettingsManager settingsManager)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _settingsManager = settingsManager;
        }

        //public string ApplicationPath => StringUtils.TrimEnd(_httpContext.Request.PathBase.Value, Constants.ApiPrefix);

        public string ApplicationPath => "/";

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

        public string GetUploadFileName(string fileName)
        {
            var dt = DateTime.Now;
            return $"{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}{PathUtils.GetExtension(fileName)}";
        }
    }
}