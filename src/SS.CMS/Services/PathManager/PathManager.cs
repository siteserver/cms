using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Services
{
    public partial class PathManager : IPathManager
    {
        private readonly HttpContext _httpContext;
        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISpecialRepository _specialRepository;
        private readonly ITemplateLogRepository _templateLogRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public PathManager(IHttpContextAccessor httpContextAccessor, ISettingsManager settingsManager, IDatabaseManager databaseManager, ISpecialRepository specialRepository, ITemplateLogRepository templateLogRepository, ITemplateRepository templateRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _specialRepository = specialRepository;
            _templateLogRepository = templateLogRepository;
            _templateRepository = templateRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        //public string ApplicationPath => StringUtils.TrimEnd(_httpContext.Request.PathBase.Value, Constants.ApiPrefix);

        public string ContentRootPath => _settingsManager.ContentRootPath;
        public string WebRootPath => _settingsManager.WebRootPath;

        public string WebUrl => "/";

        public string GetWebPath(params string[] paths)
        {
            return PathUtils.Combine(_settingsManager.WebRootPath, PageUtils.Combine(paths));
        }

        public string GetWebUrl(params string[] paths)
        {
            return PageUtils.Combine(WebUrl, PageUtils.Combine(paths));
        }

        public string GetAdminUrl(params string[] paths)
        {
            return PageUtils.Combine(WebUrl, _settingsManager.AdminDirectory, PageUtils.Combine(paths), "/");
        }

        public string GetUploadFileName(string fileName)
        {
            var dt = DateTime.Now;
            return $"{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}{PathUtils.GetExtension(fileName)}";
        }

        public async Task<string> GetWebUrlAsync(Site site)
        {
            return site.IsSeparatedWeb ? site.SeparatedWebUrl : await GetLocalSiteUrlAsync(site);
        }

        public async Task<string> GetAssetsUrlAsync(Site site)
        {
            return site.IsSeparatedAssets
                ? site.SeparatedAssetsUrl
                : PageUtils.Combine(await GetWebUrlAsync(site), site.AssetsDir);
        }

        public string MapPath(string virtualPath)
        {
            virtualPath = PathUtils.RemovePathInvalidChar(virtualPath);
            if (!string.IsNullOrEmpty(virtualPath))
            {
                if (virtualPath.StartsWith("~"))
                {
                    virtualPath = virtualPath.Substring(1);
                }
                virtualPath = PageUtils.Combine("~", virtualPath);
            }
            else
            {
                virtualPath = "~/";
            }
            var rootPath = WebRootPath;

            virtualPath = !string.IsNullOrEmpty(virtualPath) ? virtualPath.Substring(2) : string.Empty;
            var retVal = PathUtils.Combine(rootPath, virtualPath) ?? string.Empty;

            return retVal.Replace("/", "\\");
        }

        public async Task UploadAsync(IFormFile file, string filePath)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }
    }
}