using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class EditorController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Insert([FromBody] SaveRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Types.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            
            var content = await _pathManager.EncodeContentAsync(site, channel, request.Content);

            content.SiteId = site.Id;
            content.ChannelId = channel.Id;
            content.AdminId = _authManager.AdminId;
            content.LastEditAdminId = _authManager.AdminId;

            content.Checked = request.Content.CheckedLevel >= site.CheckContentLevel;
            if (content.Checked)
            {
                content.CheckedLevel = 0;
            }

            await _contentRepository.InsertAsync(site, channel, content);

            if (request.Translations != null && request.Translations.Count > 0)
            {
                foreach (var translation in request.Translations)
                {
                    await ContentUtility.TranslateAsync(_pathManager, _databaseManager, _pluginManager, site, content.ChannelId, content.Id, translation.TransSiteId, translation.TransChannelId, translation.TransType, _createManager, _authManager.AdminId);
                }
            }

            await _createManager.CreateContentAsync(request.SiteId, channel.Id, content.Id);
            await _createManager.TriggerContentChangedEventAsync(request.SiteId, channel.Id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
