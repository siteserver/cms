using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    [RoutePrefix("pages/cms/contents/contentsLayerTag")]
    public partial class PagesContentsLayerTagController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<ObjectResult<IEnumerable<string>>> Get([FromUri] ChannelRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Request.Unauthorized<ObjectResult<IEnumerable<string>>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<ObjectResult<IEnumerable<string>>>();

            var tagNames = await DataProvider.ContentTagRepository.GetTagNamesAsync(request.SiteId);

            return new ObjectResult<IEnumerable<string>>
            {
                Value = tagNames
            };
        }

        [HttpPost, Route(Route)]
        public async Task<BoolResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            var allTagNames = await DataProvider.ContentTagRepository.GetTagNamesAsync(request.SiteId);

            foreach (var tagName in request.TagNames)
            {
                if (!allTagNames.Contains(tagName))
                {
                    await DataProvider.ContentTagRepository.InsertAsync(request.SiteId, tagName);
                }
            }

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
            foreach (var summary in summaries)
            {
                var channel = await DataProvider.ChannelRepository.GetAsync(summary.ChannelId);
                var content = await DataProvider.ContentRepository.GetAsync(site, channel, summary.Id);
                if (content == null) continue;

                var list = new List<string>();
                foreach (var tagName in Utilities.GetStringList(content.TagNames))
                {
                    if (allTagNames.Contains(tagName))
                    {
                        list.Add(tagName);
                    }
                }

                foreach (var name in request.TagNames)
                {
                    if (request.IsCancel)
                    {
                        if (list.Contains(name)) list.Remove(name);
                    }
                    else
                    {
                        if (!list.Contains(name)) list.Add(name);
                    }
                }
                content.TagNames = list;

                await DataProvider.ContentRepository.UpdateAsync(site, channel, content);
            }

            await auth.AddSiteLogAsync(request.SiteId, request.IsCancel ? "批量取消内容标签" : "批量设置内容标签");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
