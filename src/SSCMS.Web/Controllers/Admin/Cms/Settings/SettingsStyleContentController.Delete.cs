using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleContentController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<DeleteResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsStyleContent))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            var tableName = _channelRepository.GetTableName(site, channel);

            await _tableStyleRepository.DeleteAsync(tableName, request.ChannelId, request.AttributeName);

            var relatedIdentities = _tableStyleRepository.GetRelatedIdentities(channel);
            var styles = await _tableStyleRepository.GetTableStylesAsync(tableName, relatedIdentities);
            foreach (var style in styles)
            {
                style.IsSystem = style.RelatedIdentity != request.ChannelId;
            }

            return new DeleteResult
            {
                Styles = styles
            };
        }
    }
}
