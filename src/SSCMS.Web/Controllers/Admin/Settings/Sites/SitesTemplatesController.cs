using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesTemplatesController : ControllerBase
    {
        private const string Route = "settings/sitesTemplates";
        private const string RouteZip = "settings/sitesTemplates/actions/zip";
        private const string RouteUnZip = "settings/sitesTemplates/actions/unZip";
        private const string RouteUpload = "settings/sitesTemplates/actions/upload";

        private readonly ICacheManager _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;

        public SitesTemplatesController(ICacheManager cacheManager, IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
        }

        public class ListResult
        {
            public List<SiteTemplateInfo> SiteTemplateInfoList { get; set; }
            public List<string> FileNameList { get; set; }
            public string SiteTemplateUrl { get; set; }
            public bool SiteAddPermission { get; set; }
        }

        public class ZipRequest
        {
            public string DirectoryName { get; set; }
        }

        public class UnZipRequest
        {
            public string FileName { get; set; }
        }

        public class DeleteRequest
        {
            public string DirectoryName { get; set; }
            public string FileName { get; set; }
        }

        private async Task<ListResult> GetListResultAsync()
        {
            var caching = new CacheUtils(_cacheManager);
            var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching);
            var siteTemplates = manager.GetSiteTemplateInfoList();
            var siteTemplateInfoList = new List<SiteTemplateInfo>();
            foreach (var siteTemplate in siteTemplates)
            {
                var directoryPath = _pathManager.GetSiteTemplatesPath(siteTemplate.DirectoryName);
                var dirInfo = new DirectoryInfo(directoryPath);
                if (string.IsNullOrEmpty(siteTemplate.SiteTemplateName)) continue;

                var filePath = _pathManager.GetSiteTemplatesPath(dirInfo.Name + ".zip");
                siteTemplate.FileExists = FileUtils.IsFileExists(filePath);
                siteTemplateInfoList.Add(siteTemplate);
            }

            var fileNames = manager.GetZipSiteTemplateList();
            var fileNameList = new List<string>();
            foreach (var fileName in fileNames)
            {
                if (DirectoryUtils.IsDirectoryExists(
                    _pathManager.GetSiteTemplatesPath(PathUtils.GetFileNameWithoutExtension(fileName)))) continue;
                var filePath = _pathManager.GetSiteTemplatesPath(fileName);
                var fileInfo = new FileInfo(filePath);
                fileNameList.Add(fileInfo.Name);
            }

            var siteTemplateUrl = StringUtils.TrimSlash(_pathManager.GetSiteTemplatesUrl(string.Empty));
            var siteAddPermission =
                await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesAdd);

            return new ListResult
            {
                SiteTemplateInfoList = siteTemplateInfoList,
                FileNameList = fileNameList,
                SiteTemplateUrl = siteTemplateUrl,
                SiteAddPermission = siteAddPermission
            };
        }
    }
}
