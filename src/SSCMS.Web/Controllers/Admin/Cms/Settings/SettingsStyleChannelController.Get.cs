using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleChannelController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] ChannelRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsStyleChannel))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            var tableName = _channelRepository.TableName;
            var relatedIdentities = _tableStyleRepository.GetRelatedIdentities(channel);
            var styles = await _tableStyleRepository.GetTableStylesAsync(tableName, relatedIdentities);
            foreach (var style in styles)
            {
                style.IsSystem = style.RelatedIdentity != request.ChannelId;
            }

            Cascade<int> cascade = null;
            if (request.ChannelId == request.SiteId)
            {
                cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
                {
                    var count = await _contentRepository.GetCountAsync(site, summary);
                    return new
                    {
                        Count = count
                    };
                });
            }

            var inputTypes = ListUtils.GetSelects<InputType>();

            return new GetResult
            {
                InputTypes = inputTypes,
                TableName = tableName,
                RelatedIdentities = ListUtils.ToString(relatedIdentities),
                Styles = styles,
                Channels = cascade
            };
        }
    }
}
