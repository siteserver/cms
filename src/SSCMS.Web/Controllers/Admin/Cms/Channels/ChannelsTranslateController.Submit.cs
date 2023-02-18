using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsTranslateController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.ChannelsTranslate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var adminId = _authManager.AdminId;
            foreach (var transChannelId in request.TransChannelIds)
            {
                await TranslateAsync(site, request.TransSiteId, transChannelId, request.TranslateType, request.ChannelIds, request.IsDeleteAfterTranslate, adminId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
