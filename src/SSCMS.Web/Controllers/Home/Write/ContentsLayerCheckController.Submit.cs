using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsLayerCheckController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.CheckLevel1))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            var isChecked = request.CheckedLevel >= site.CheckContentLevel;
            if (isChecked)
            {
                request.CheckedLevel = 0;
            }

            var adminId = _authManager.AdminId;
            var contentInfoList = new List<Content>();
            foreach (var contentId in request.ContentIds)
            {
                var contentInfo = await _contentRepository.GetAsync(site, channelInfo, contentId);
                if (contentInfo == null) continue;

                contentInfo.Set(ColumnsManager.CheckAdminId, adminId);
                contentInfo.Set(ColumnsManager.CheckDate, DateTime.Now);
                contentInfo.Set(ColumnsManager.CheckReasons, request.Reasons);

                contentInfo.Checked = isChecked;
                contentInfo.CheckedLevel = request.CheckedLevel;

                if (request.IsTranslate && request.TranslateChannelId > 0)
                {
                    var translateChannelInfo = await _channelRepository.GetAsync(request.TranslateChannelId);
                    contentInfo.ChannelId = translateChannelInfo.Id;
                    await _contentRepository.UpdateAsync(site, translateChannelInfo, contentInfo);
                }
                else
                {
                    await _contentRepository.UpdateAsync(site, channelInfo, contentInfo);
                }

                contentInfoList.Add(contentInfo);

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
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "批量审核内容");

            foreach (var contentInfo in contentInfoList)
            {
                await _createManager.CreateContentAsync(request.SiteId, contentInfo.ChannelId, contentInfo.Id);
            }
            await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);
            if (request.IsTranslate && request.TranslateChannelId > 0)
            {
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.TranslateChannelId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
