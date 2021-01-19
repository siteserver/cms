using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<BoolResult>> Update([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId,
                    MenuUtils.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            var source = await _contentRepository.GetAsync(site, channel,  request.ContentId);

            var adminId = _authManager.AdminId;
            var content = await _pathManager.EncodeContentAsync(site, channel, request.Content);
            content.SiteId = site.Id;
            content.ChannelId = channel.Id;
            content.LastEditAdminId = adminId;

            var isChecked = request.Content.CheckedLevel >= site.CheckContentLevel;
            if (isChecked != source.Checked)
            {
                content.Set(ColumnsManager.CheckAdminId, adminId);
                content.Set(ColumnsManager.CheckDate, DateTime.Now);
                content.Set(ColumnsManager.CheckReasons, string.Empty);
                content.Checked = isChecked;
                if (isChecked)
                {
                    content.CheckedLevel = 0;
                }

                await _contentCheckRepository.InsertAsync(new ContentCheck
                {
                    SiteId = request.SiteId,
                    ChannelId = content.ChannelId,
                    ContentId = content.Id,
                    AdminId = adminId,
                    Checked = content.Checked,
                    CheckedLevel = content.CheckedLevel,
                    CheckDate = DateTime.Now,
                    Reasons = string.Empty
                });
            }

            await _contentRepository.UpdateAsync(site, channel, content);

            await _contentTagRepository.UpdateTagsAsync(source.TagNames, content.TagNames, request.SiteId, content.Id);

            if (request.Translates != null && request.Translates.Count > 0)
            {
                foreach (var translate in request.Translates)
                {
                    await ContentUtility.TranslateAsync(_pathManager, _databaseManager, _pluginManager, site, content.ChannelId, content.Id, translate.TargetSiteId, translate.TargetChannelId, translate.TranslateType, _createManager, _authManager.AdminId);
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
