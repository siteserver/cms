using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Editor
{
    public partial class PagesEditorController
    {
        [HttpPut, Route(Route)]
        public async Task<BoolResult> Update([FromBody] UpdateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            var source = await DataProvider.ContentRepository.GetAsync(site, channel,  request.ContentId);

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

                await DataProvider.ContentCheckRepository.InsertAsync(new ContentCheck
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

            await DataProvider.ContentRepository.UpdateAsync(site, channel, content);

            if (request.Translations != null && request.Translations.Count > 0)
            {
                foreach (var translation in request.Translations)
                {
                    await ContentUtility.TranslateAsync(site, content.ChannelId, content.Id, translation.TransSiteId, translation.TransChannelId, translation.TransType, _createManager);
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
