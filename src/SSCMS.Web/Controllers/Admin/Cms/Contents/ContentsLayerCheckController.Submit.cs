using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerCheckController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.CheckLevel1))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var isChecked = request.CheckedLevel >= site.CheckContentLevel;
            if (isChecked)
            {
                request.CheckedLevel = 0;
            }

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
            summaries.Reverse();

            var adminId = _authManager.AdminId;
            foreach (var summary in summaries)
            {
                var contentChannelInfo = await _channelRepository.GetAsync(summary.ChannelId);
                var contentInfo = await _contentRepository.GetAsync(site, contentChannelInfo, summary.Id);
                if (contentInfo == null) continue;

                contentInfo.Set(ColumnsManager.CheckAdminId, adminId);
                contentInfo.Set(ColumnsManager.CheckDate, DateTime.Now);
                contentInfo.Set(ColumnsManager.CheckReasons, request.Reasons);

                contentInfo.Checked = isChecked;
                contentInfo.CheckedLevel = request.CheckedLevel;

                await _contentRepository.UpdateAsync(site, contentChannelInfo, contentInfo);

                await _contentCheckRepository.InsertAsync(new ContentCheck
                {
                    SiteId = request.SiteId,
                    ChannelId = contentInfo.ChannelId,
                    ContentId = contentInfo.Id,
                    AdminId = adminId,
                    Checked = isChecked,
                    CheckedLevel = request.CheckedLevel,
                    CheckDate = DateTime.Now,
                    Reasons = request.Reasons
                });

                if (request.IsTranslate)
                {
                    await ContentUtility.TranslateAsync(_pathManager, _databaseManager, _pluginManager, site, summary.ChannelId, summary.Id, request.TransSiteId, request.TransChannelId, TranslateType.Cut, _createManager, _authManager.AdminId);
                }
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "批量审核内容");

            if (request.IsTranslate)
            {
                await _createManager.TriggerContentChangedEventAsync(request.TransSiteId, request.TransChannelId);
            }
            else
            {
                foreach (var summary in summaries)
                {
                    await _createManager.CreateContentAsync(request.SiteId, summary.ChannelId, summary.Id);
                }
            }

            foreach (var distinctChannelId in summaries.Select(x => x.ChannelId).Distinct())
            {
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, distinctChannelId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}