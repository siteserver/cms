using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsContentTagController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.SettingsContentTag))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            await _contentTagRepository.DeleteAsync(request.SiteId, request.TagName);

            await _authManager.AddSiteLogAsync(request.SiteId, "删除内容标签", $"内容标签:{request.TagName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}