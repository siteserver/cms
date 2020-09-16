using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorLayerTranslateController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Translate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var transSite = await _siteRepository.GetAsync(request.TransSiteId);
            var siteName = transSite.SiteName;

            var channels = new List<TransChannel>();
            foreach (var transChannelId in request.TransChannelIds)
            {
                var name = await _channelRepository.GetChannelNameNavigationAsync(request.TransSiteId, transChannelId);
                if (request.TransSiteId != request.SiteId)
                {
                    name = siteName + " : " + name;
                }

                name += $" ({request.TransType.GetDisplayName()})";

                channels.Add(new TransChannel
                {
                    Id = transChannelId,
                    Name = name
                });
            }

            return new SubmitResult
            {
                Channels = channels
            };
        }
    }
}