using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ContentsController
    {
        [OpenApiOperation("审核内容 API", "审核内容使用POST发起请求，请求地址为/api/v1/contents/actions/check")]
        [HttpPost, Route(RouteActionsCheck)]
        public async Task<ActionResult<CheckResult>> CheckContents([FromBody] CheckRequest request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeContents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var adminId = _authManager.AdminId;
            var contents = new List<Content>();
            foreach (var channelContentId in request.Contents)
            {
                var channel = await _channelRepository.GetAsync(channelContentId.ChannelId);
                var content = await _contentRepository.GetAsync(site, channel, channelContentId.Id);
                if (content == null) continue;

                content.Set(ColumnsManager.CheckAdminId, adminId);
                content.Set(ColumnsManager.CheckDate, DateTime.Now);
                content.Set(ColumnsManager.CheckReasons, request.Reasons);

                content.Checked = true;
                content.CheckedLevel = 0;

                await _contentRepository.UpdateAsync(site, channel, content);

                contents.Add(content);

                await _contentCheckRepository.InsertAsync(new ContentCheck
                {
                    SiteId = request.SiteId,
                    ChannelId = content.ChannelId,
                    ContentId = content.Id,
                    AdminId = adminId,
                    Checked = true,
                    CheckedLevel = 0,
                    CheckDate = DateTime.Now,
                    Reasons = request.Reasons
                });
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "批量审核内容");

            foreach (var content in request.Contents)
            {
                await _createManager.CreateContentAsync(request.SiteId, content.ChannelId, content.Id);
            }

            foreach (var distinctChannelId in request.Contents.Select(x => x.ChannelId).Distinct())
            {
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, distinctChannelId);
            }

            await _createManager.CreateChannelAsync(request.SiteId, request.SiteId);

            return new CheckResult
            {
                Contents = contents
            };
        }
    }
}
