using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    [RoutePrefix("pages/cms/contents/contentsLayerAttributes")]
    public partial class PagesContentsLayerAttributesController : ApiController
    {
        private const string Route = "";

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

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);

            foreach (var summary in summaries)
            {
                var channel = await DataProvider.ChannelRepository.GetAsync(summary.ChannelId);
                var content = await DataProvider.ContentRepository.GetAsync(site, channel, summary.Id);
                if (content == null) continue;

                if (request.IsTop)
                {
                    content.Top = !request.IsCancel;
                }
                if (request.IsRecommend)
                {
                    content.Recommend = !request.IsCancel;
                }
                if (request.IsHot)
                {
                    content.Hot = !request.IsCancel;
                }
                if (request.IsColor)
                {
                    content.Color = !request.IsCancel;
                }

                await DataProvider.ContentRepository.UpdateAsync(site, channel, content);
            }

            await auth.AddSiteLogAsync(request.SiteId, request.IsCancel ? "取消内容属性" : "设置内容属性");

            //else if (pageType == "setHits")
            //{
            //    foreach (var channelContentId in channelContentIds)
            //    {
            //        var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelContentId.ChannelId);
            //        var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, channelContentId.Id);
            //        if (contentInfo == null) continue;

            //        contentInfo.Hits = hits;
            //        await DataProvider.ContentRepository.UpdateAsync(site, channelInfo, contentInfo);
            //    }

            //    await request.AddSiteLogAsync(siteId, "设置内容点击量");
            //}

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
