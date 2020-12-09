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

            var caching = new CacheUtils(_cacheManager);
            var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching);
            var siteTemplates = manager.GetSiteTemplateInfoList();

            var tableNameList = await _siteRepository.GetSiteTableNamesAsync();

            var rootExists = await _siteRepository.GetSiteByIsRootAsync() != null;

            var sites = await _siteRepository.GetCascadeChildrenAsync(0);
            sites.Insert(0, new Cascade<int>
            {
                Value = 0,
                Label = "<无上级站点>"
            });

            var siteTypes = _settingsManager.GetSiteTypes();

            return new GetResult
            {
                SiteTypes = siteTypes,
                SiteTemplates = siteTemplates,
                RootExists = rootExists,
                Sites = sites,
                TableNameList = tableNameList,
                Guid = StringUtils.GetShortGuid(false)
            };
        }
    }
}