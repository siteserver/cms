using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Home
{
    [Route("home/contentsLayerAttributes")]
    public partial class ContentsLayerAttributesController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public ContentsLayerAttributesController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetUserAsync();
            if (!auth.IsUserLoggin ||
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            if (request.PageType == "setAttributes")
            {
                if (request.IsRecommend || request.IsHot || request.IsColor || request.IsTop)
                {
                    foreach (var contentId in request.ContentIds)
                    {
                        var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);
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
                        await DataProvider.ContentRepository.UpdateAsync(site, channelInfo, contentInfo);
                    }

                    await auth.AddSiteLogAsync(request.SiteId, "设置内容属性");
                }
            }
            else if (request.PageType == "cancelAttributes")
            {
                if (request.IsRecommend || request.IsHot || request.IsColor || request.IsTop)
                {
                    foreach (var contentId in request.ContentIds)
                    {
                        var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);
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
                        await DataProvider.ContentRepository.UpdateAsync(site, channelInfo, contentInfo);
                    }

                    await auth.AddSiteLogAsync(request.SiteId, "取消内容属性");
                }
            }
            else if (request.PageType == "setHits")
            {
                foreach (var contentId in request.ContentIds)
                {
                    var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);
                    if (contentInfo == null) continue;

                    contentInfo.Hits = request.Hits;
                    await DataProvider.ContentRepository.UpdateAsync(site, channelInfo, contentInfo);
                }

                await auth.AddSiteLogAsync(request.SiteId, "设置内容点击量");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
