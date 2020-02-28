using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<BoolResult>> Update([FromBody] UpdateRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd) ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            var source = await _contentRepository.GetAsync(site, channel,  request.ContentId);

            var content = request.Content;
            content.SiteId = site.Id;
            content.ChannelId = channel.Id;
            content.LastEditAdminId = auth.AdminId;
            content.LastEditDate = DateTime.Now;

            var isChecked = request.Content.CheckedLevel >= site.CheckContentLevel;
            if (isChecked != source.Checked)
            {
                content.CheckAdminId = auth.AdminId;
                content.CheckDate = DateTime.Now;
                content.CheckReasons = string.Empty;
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
                    AdminId = auth.AdminId,
                    Checked = content.Checked,
                    CheckedLevel = content.CheckedLevel,
                    CheckDate = DateTime.Now,
                    Reasons = string.Empty
                });
            }

            await _contentRepository.UpdateAsync(site, channel, content);

            if (request.Translations != null && request.Translations.Count > 0)
            {
                foreach (var translation in request.Translations)
                {
                    await ContentUtility.TranslateAsync(_pathManager, _databaseManager, site, content.ChannelId, content.Id, translation.TransSiteId, translation.TransChannelId, translation.TransType, _createManager);
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }

        public class Translation
        {
            public int TransSiteId { get; set; }
            public int TransChannelId { get; set; }
            public TranslateContentType TransType { get; set; }
        }

        public class UpdateRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
            public Content Content { get; set; }
            public List<Translation> Translations { get; set; }
        }
    }
}
