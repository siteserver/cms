using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class PathManager
    {
        public string GetTemporaryFilesUrl(params string[] paths)
        {
            return GetSiteFilesUrl(DirectoryUtils.SiteFiles.TemporaryFiles, PageUtils.Combine(paths));
        }

        public string GetSiteTemplatesUrl(string relatedUrl)
        {
            return GetRootUrl(DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.SiteTemplates.DirectoryName, relatedUrl);
        }

        public string GetSiteFilesUrl(params string[] paths)
        {
            return GetRootUrl(DirectoryUtils.SiteFiles.DirectoryName, PageUtils.Combine(paths));
        }

        public string GetSiteFilesUrl(Site site, params string[] paths)
        {
            return site == null
                ? GetSiteFilesUrl(paths)
                : GetApiHostUrl(site, DirectoryUtils.SiteFiles.DirectoryName, PageUtils.Combine(paths));
        }

        public string GetSiteFilesPath(params string[] paths)
        {
            var path = PathUtils.Combine(_settingsManager.WebRootPath, DirectoryUtils.SiteFiles.DirectoryName, PathUtils.Combine(paths));
            return path;
        }

        public async Task<string> GetMailTemplateHtmlAsync()
        {
            var htmlPath = GetSiteFilesPath("assets/mail/template.html");
            if (_cacheManager.Exists(htmlPath)) return _cacheManager.Get<string>(htmlPath);

            var html = await FileUtils.ReadTextAsync(htmlPath);

            _cacheManager.AddOrUpdate(htmlPath, html);
            return html;
        }

        public async Task<string> GetMailListHtmlAsync()
        {
            var htmlPath = GetSiteFilesPath("assets/mail/list.html");
            if (_cacheManager.Exists(htmlPath)) return _cacheManager.Get<string>(htmlPath);

            var html = await FileUtils.ReadTextAsync(htmlPath);

            _cacheManager.AddOrUpdate(htmlPath, html);
            return html;
        }
    }
}