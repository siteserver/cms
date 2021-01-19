using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Insert([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId,
                    MenuUtils.ContentPermissions.Add))
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

            var contentId = await _contentRepository.InsertAsync(site, channel, content);

            await _contentTagRepository.UpdateTagsAsync(null, content.TagNames, request.SiteId, contentId);

            var translates = await _translateRepository.GetTranslatesAsync(request.SiteId, request.ChannelId);
            if (request.Translates != null && request.Translates.Count > 0)
            {
                translates.AddRange(request.Translates);
            }
            foreach (var translate in translates)
            {
                await ContentUtility.TranslateAsync(_pathManager, _databaseManager, _pluginManager, site, content.ChannelId, content.Id, translate.TargetSiteId, translate.TargetChannelId, translate.TranslateType, _createManager, _authManager.AdminId);
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
