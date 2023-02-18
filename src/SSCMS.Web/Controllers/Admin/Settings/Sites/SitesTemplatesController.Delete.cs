using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesTemplatesController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var caching = new CacheUtils(_cacheManager);
            var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching);

            var directoryName = PathUtils.RemoveParentPath(request.DirectoryName);
            var fileName = PathUtils.RemoveParentPath(request.FileName);
            if (!string.IsNullOrEmpty(directoryName))
            {
                manager.DeleteSiteTemplate(directoryName);
                await _authManager.AddAdminLogAsync("删除站点模板", $"站点模板:{directoryName}");
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                manager.DeleteZipSiteTemplate(fileName);
                await _authManager.AddAdminLogAsync("删除未解压站点模板", $"站点模板:{fileName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
