using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.Office;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsLayerWordController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var styles = await _tableStyleRepository.GetContentStylesAsync(site, channel);
            var isChecked = request.CheckedLevel >= site.CheckContentLevel;
            var adminId = _authManager.AdminId;
            var userId = _authManager.UserId;

            var contentIdList = new List<int>();
            foreach (var file in request.Files)
            {
                if (string.IsNullOrEmpty(file.FileName) || string.IsNullOrEmpty(file.Title)) continue;

                var filePath = _pathManager.GetTemporaryFilesPath(file.FileName);
                var (title, imageUrl, content) = await WordManager.GetWordAsync(_pathManager, site, request.IsFirstLineTitle, request.IsClearFormat, request.IsFirstLineIndent, request.IsClearFontSize, request.IsClearFontFamily, request.IsClearImages, filePath, file.Title);

                if (string.IsNullOrEmpty(title)) continue;

                var dict = await ColumnsManager.SaveAttributesAsync(_pathManager, site, styles, new NameValueCollection(), ColumnsManager.MetadataAttributes.Value);

                var contentInfo = new Content
                {
                    ChannelId = channel.Id,
                    SiteId = request.SiteId,
                    AddDate = DateTime.Now,
                    SourceId = SourceManager.User,
                    AdminId = adminId,
                    UserId = userId,
                    LastEditAdminId = adminId,
                    Checked = isChecked,
                    CheckedLevel = request.CheckedLevel
                };
                contentInfo.LoadDict(dict);

                contentInfo.Title = title;
                contentInfo.ImageUrl = imageUrl;
                contentInfo.Body = content;

                contentInfo.Id = await _contentRepository.InsertAsync(site, channel, contentInfo);

                contentIdList.Add(contentInfo.Id);
            }

            if (isChecked)
            {
                foreach (var contentId in contentIdList)
                {
                    await _createManager.CreateContentAsync(request.SiteId, channel.Id, contentId);
                }
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, channel.Id);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
