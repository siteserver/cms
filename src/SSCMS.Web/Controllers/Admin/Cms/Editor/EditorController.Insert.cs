using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Core.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [HttpPost, Route(RouteInsert)]
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
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.ChannelId);

            var content = await _pathManager.EncodeContentAsync(site, channel, request.Content);

            content.SiteId = site.Id;
            content.ChannelId = channel.Id;
            content.AdminId = _authManager.AdminId;
            content.LastEditAdminId = _authManager.AdminId;

            if (request.IsScheduled)
            {
                content.Checked = false;
                content.CheckedLevel = CheckManager.LevelInt.ScheduledPublish;
            }
            else
            {
                content.Checked = request.Content.CheckedLevel >= site.CheckContentLevel;
                if (content.Checked)
                {
                    content.CheckedLevel = 0;
                }
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

            content.Id = await _contentRepository.InsertAsync(site, channel, content);

            if (request.IsScheduled)
            {
                await _scheduledTaskRepository.InsertPublishAsync(content, request.ScheduledDate);
            }

            var channelNames = await _channelRepository.GetChannelNameNavigationAsync(content.SiteId, content.ChannelId);
            await _authManager.AddSiteLogAsync(content.SiteId, content.ChannelId, content.Id, "添加内容",
                $"栏目：{channelNames}，内容标题：{content.Title}");
            await CloudManager.SendContentChangedMail(_pathManager, _mailManager, _errorLogRepository, site, content, channelNames, _authManager.AdminName, false);

            await _contentTagRepository.UpdateTagsAsync(null, content.TagNames, request.SiteId, content.Id);

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
