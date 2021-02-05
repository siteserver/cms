using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils.Office;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerWordController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<ObjectResult<List<int>>>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error("无法确定内容对应的栏目");

            var isChecked = request.CheckedLevel >= site.CheckContentLevel;

            var adminId = _authManager.AdminId;
            var contentIdList = new List<int>();
            foreach (var file in request.Files)
            {
                if (string.IsNullOrEmpty(file.FileName) || string.IsNullOrEmpty(file.Title)) continue;

                try
                {
                    var filePath = _pathManager.GetTemporaryFilesPath(file.FileName);
                    var (title, imageUrl, body) = await WordManager.GetWordAsync(_pathManager, site, request.IsFirstLineTitle, request.IsClearFormat, request.IsFirstLineIndent, request.IsClearFontSize, request.IsClearFontFamily, request.IsClearImages, filePath, file.Title);

                    if (string.IsNullOrEmpty(title)) continue;

                    var contentInfo = new Content
                    {
                        ChannelId = channel.Id,
                        SiteId = request.SiteId,
                        AdminId = adminId,
                        LastEditAdminId = adminId,
                        AddDate = DateTime.Now,
                        Checked = isChecked,
                        CheckedLevel = request.CheckedLevel,
                        Title = title,
                        ImageUrl = imageUrl,
                        Body = body
                    };

                    await _contentRepository.InsertAsync(site, channel, contentInfo);
                    contentIdList.Add(contentInfo.Id);
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(ex);
                }
            }

            if (isChecked)
            {
                foreach (var contentId in contentIdList)
                {
                    await _createManager.CreateContentAsync(request.SiteId, channel.Id, contentId);
                }
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, channel.Id);
            }

            return new ObjectResult<List<int>>
            {
                Value = contentIdList
            };
        }
    }
}