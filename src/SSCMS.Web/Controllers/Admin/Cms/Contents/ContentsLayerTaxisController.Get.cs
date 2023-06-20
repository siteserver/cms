using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerTaxisController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var totalCount = 0;
            var channelIds = new List<int>();
            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);

            var contents = new List<Content>();
            foreach (var summary in summaries)
            {
                var channel = await _channelRepository.GetAsync(summary.ChannelId);
                var content = await _contentRepository.GetAsync(site, channel, summary.Id);
                if (content == null) continue;

                if (!channelIds.Contains(channel.Id))
                {
                  channelIds.Add(channel.Id);
                }

                if (channelIds.Count == 1 && totalCount == 0)
                {
                  var current = await _contentRepository.GetSummariesAsync(site, channel);
                  totalCount = current.Count;
                }

                var pageContent = content.Clone<Content>();
                pageContent.Set(ColumnsManager.CheckState, CheckManager.GetCheckState(site, content));
                contents.Add(pageContent);
            }

            if (channelIds.Count > 1)
            {
              totalCount = 0;
            }

            return new GetResult
            {
                Contents = contents,
                TotalCount = totalCount,
            };
        }
    }
}