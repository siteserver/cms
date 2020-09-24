using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCrossSiteTransController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.SettingsCrossSiteTrans))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            site.IsCrossSiteTransChecked = request.IsCrossSiteTransChecked;

            await _siteRepository.UpdateAsync(site);

            await _authManager.AddSiteLogAsync(request.SiteId, "修改默认跨站转发设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}