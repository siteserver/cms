using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCrossSiteTransChannelsController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.SettingsCrossSiteTransChannels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            await _translateRepository.DeleteAsync(request.SiteId, request.ChannelId);

            if (request.Translates != null && request.Translates.Count > 0)
            {
                foreach (var translate in request.Translates)
                {
                    await _translateRepository.InsertAsync(translate);
                }
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "修改跨站转发设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}