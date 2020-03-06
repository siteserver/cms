using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Cms.Contents
{
    [Route("admin/cms/contents/contentsLayerAttributes")]
    public partial class ContentsLayerAttributesController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ContentsLayerAttributesController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);

            foreach (var summary in summaries)
            {
                var channel = await _channelRepository.GetAsync(summary.ChannelId);
                var content = await _contentRepository.GetAsync(site, channel, summary.Id);
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

                await _contentRepository.UpdateAsync(site, channel, content);
            }

            await auth.AddSiteLogAsync(request.SiteId, request.IsCancel ? "取消内容属性" : "设置内容属性");

            //else if (pageType == "setHits")
            //{
            //    foreach (var channelContentId in channelContentIds)
            //    {
            //        var channelInfo = await _channelRepository.GetAsync(channelContentId.ChannelId);
            //        var contentInfo = await _contentRepository.GetAsync(site, channelInfo, channelContentId.Id);
            //        if (contentInfo == null) continue;

            //        contentInfo.Hits = hits;
            //        await _contentRepository.UpdateAsync(site, channelInfo, contentInfo);
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
