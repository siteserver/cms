using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Insert([FromBody] SaveRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            
            var content = request.Content;
            content.SiteId = site.Id;
            content.ChannelId = channel.Id;
            content.LastEditAdminId = auth.AdminId;
            content.LastEditDate = DateTime.Now;

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
                    await ContentUtility.TranslateAsync(_pathManager, _databaseManager, _pluginManager, site, content.ChannelId, content.Id, translation.TransSiteId, translation.TransChannelId, translation.TransType, _createManager);
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
