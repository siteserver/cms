using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;

namespace SS.CMS.Web.Controllers.Home
{
    [Route("home/contentsLayerAttributes")]
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
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            if (!await _authManager.IsUserAuthenticatedAsync() ||
                !await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            if (request.PageType == "setAttributes")
            {
                if (request.IsRecommend || request.IsHot || request.IsColor || request.IsTop)
                {
                    foreach (var contentId in request.ContentIds)
                    {
                        var contentInfo = await _contentRepository.GetAsync(site, channelInfo, contentId);
                        if (contentInfo == null) continue;

                        if (request.IsRecommend)
                        {
                            contentInfo.Recommend = true;
                        }
                        if (request.IsHot)
                        {
                            contentInfo.Hot = true;
                        }
                        if (request.IsColor)
                        {
                            contentInfo.Color = true;
                        }
                        if (request.IsTop)
                        {
                            contentInfo.Top = true;
                        }
                        await _contentRepository.UpdateAsync(site, channelInfo, contentInfo);
                    }

                    await _authManager.AddSiteLogAsync(request.SiteId, "设置内容属性");
                }
            }
            else if (request.PageType == "cancelAttributes")
            {
                if (request.IsRecommend || request.IsHot || request.IsColor || request.IsTop)
                {
                    foreach (var contentId in request.ContentIds)
                    {
                        var contentInfo = await _contentRepository.GetAsync(site, channelInfo, contentId);
                        if (contentInfo == null) continue;

                        if (request.IsRecommend)
                        {
                            contentInfo.Recommend = false;
                        }
                        if (request.IsHot)
                        {
                            contentInfo.Hot = false;
                        }
                        if (request.IsColor)
                        {
                            contentInfo.Color = false;
                        }
                        if (request.IsTop)
                        {
                            contentInfo.Top = false;
                        }
                        await _contentRepository.UpdateAsync(site, channelInfo, contentInfo);
                    }

                    await _authManager.AddSiteLogAsync(request.SiteId, "取消内容属性");
                }
            }
            else if (request.PageType == "setHits")
            {
                foreach (var contentId in request.ContentIds)
                {
                    var contentInfo = await _contentRepository.GetAsync(site, channelInfo, contentId);
                    if (contentInfo == null) continue;

                    contentInfo.Hits = request.Hits;
                    await _contentRepository.UpdateAsync(site, channelInfo, contentInfo);
                }

                await _authManager.AddSiteLogAsync(request.SiteId, "设置内容点击量");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
