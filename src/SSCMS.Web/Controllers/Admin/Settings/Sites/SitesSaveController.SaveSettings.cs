using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesSaveController
    {
        [HttpPost, Route(RouteSettings)]
        public async Task<ActionResult<SaveSettingsResult>> SaveSettings([FromBody] SaveRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var caching = new CacheUtils(_cacheManager);
            var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching);

            if (manager.IsSiteTemplateDirectoryExists(request.TemplateDir))
            {
                return this.Error("站点模板文件夹已存在，请更换站点模板文件夹！");
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var sitePath = await _pathManager.GetSitePathAsync(site);
            var directoryNames = DirectoryUtils.GetDirectoryNames(sitePath);

            var directories = new List<string>();
            var siteDirList = await _siteRepository.GetSiteDirsAsync(0);
            foreach (var directoryName in directoryNames)
            {
                var isSiteDirectory = false;
                if (site.Root)
                {
                    foreach (var siteDir in siteDirList)
                    {
                        if (StringUtils.EqualsIgnoreCase(siteDir, directoryName))
                        {
                            isSiteDirectory = true;
                        }
                    }
                }
                if (!isSiteDirectory && !_pathManager.IsSystemDirectory(directoryName))
                {
                    directories.Add(directoryName);
                }
            }

            var files = DirectoryUtils.GetFileNames(sitePath);

            return new SaveSettingsResult
            {
                Directories = directories,
                Files = files
            };
        }
    }
}
