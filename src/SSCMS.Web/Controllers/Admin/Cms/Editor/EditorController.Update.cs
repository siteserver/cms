using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Core.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [HttpPost, Route(RouteUpdate)]
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
            if (site == null) return this.Error(Constants.ErrorNotFound);

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

            if (request.IsScheduled)
            {
                content.Checked = false;
                content.CheckedLevel = CheckManager.LevelInt.ScheduledPublish;
            }

            if (content.LinkType == Enums.LinkType.None)
            {
                content.LinkUrl = request.Content.LinkUrl;
            }
            else if (content.LinkType == Enums.LinkType.LinkToChannel)
            {
                content.LinkUrl = ListUtils.ToString(request.LinkTo.ChannelIds);
            }
            else if (content.LinkType == Enums.LinkType.LinkToContent)
            {
                content.LinkUrl = ListUtils.ToString(request.LinkTo.ChannelIds) + "_" + request.LinkTo.ContentId;
            }
            else
            {
                content.LinkUrl = string.Empty;
            }

            await _contentRepository.UpdateAsync(site, channel, content);

            if (request.IsScheduled)
            {
                await _scheduledTaskRepository.InsertPublishAsync(content, request.ScheduledDate);
            }

            var channelNames = await _channelRepository.GetChannelNameNavigationAsync(content.SiteId, content.ChannelId);
            await _authManager.AddSiteLogAsync(content.SiteId, content.ChannelId, content.Id, "修改内容",
                $"栏目：{channelNames}，内容标题：{content.Title}");
            await CloudManager.SendContentChangedMail(_pathManager, _mailManager, _errorLogRepository, site, content, channelNames, _authManager.AdminName, true);
            
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
