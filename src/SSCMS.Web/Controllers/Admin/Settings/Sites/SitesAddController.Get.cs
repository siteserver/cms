using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesAddController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesAdd))
            {
                return Unauthorized();
            }

            if (_settingsManager.MaxSites > 0)
            {
                var siteIds = await _siteRepository.GetSiteIdsAsync();
                if (siteIds.Count >= _settingsManager.MaxSites)
                {
                    return this.Error("站点数量已超过限制，无法创建新站点!");
                }
            }

            var caching = new CacheUtils(_cacheManager);
            var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching);
            var siteTemplates = manager.GetSiteTemplates();

            var tableNameList = await _siteRepository.GetSiteTableNamesAsync();

            var rootExists = await _siteRepository.GetSiteByIsRootAsync() != null;

            var sites = await _siteRepository.GetCascadeChildrenAsync(0);
            sites.Insert(0, new Cascade<int>
            {
                Value = 0,
                Label = "<无上级站点>"
            });

            return new GetResult
            {
                SiteTemplates = siteTemplates,
                RootExists = rootExists,
                Sites = sites,
                TableNameList = tableNameList,
                Guid = StringUtils.GetShortGuid(false)
            };
        }
    }
}