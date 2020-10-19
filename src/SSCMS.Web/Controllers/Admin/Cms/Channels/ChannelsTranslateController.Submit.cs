using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsTranslateController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.ChannelsTranslate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var adminId = _authManager.AdminId;
            await TranslateAsync(site, request.TransSiteId, request.TransChannelId, request.TranslateType, request.ChannelIds, request.IsDeleteAfterTranslate, adminId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
