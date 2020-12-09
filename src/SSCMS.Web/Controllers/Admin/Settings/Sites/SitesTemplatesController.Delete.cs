using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesTemplatesController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var caching = new CacheUtils(_cacheManager);
            var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching);

            if (!string.IsNullOrEmpty(request.DirectoryName))
            {
                manager.DeleteSiteTemplate(request.DirectoryName);
                await _authManager.AddAdminLogAsync("删除站点模板", $"站点模板:{request.DirectoryName}");
            }
            if (!string.IsNullOrEmpty(request.FileName))
            {
                manager.DeleteZipSiteTemplate(request.FileName);
                await _authManager.AddAdminLogAsync("删除未解压站点模板", $"站点模板:{request.FileName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
