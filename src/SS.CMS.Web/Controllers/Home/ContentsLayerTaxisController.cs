using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Home
{
    [Route("home/contentsLayerTaxis")]
    public partial class ContentsLayerTaxisController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;

        public ContentsLayerTaxisController(IAuthManager authManager, ICreateManager createManager)
        {
            _authManager = authManager;
            _createManager = createManager;
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

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            if (channel.DefaultTaxisType == TaxisType.OrderByTaxis)
            {
                request.IsUp = !request.IsUp;
            }

            if (request.IsUp == false)
            {
                request.ContentIds.Reverse();
            }

            foreach (var contentId in request.ContentIds)
            {
                var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channel, contentId);
                if (contentInfo == null) continue;

                var isTop = contentInfo.Top;
                for (var i = 1; i <= request.Taxis; i++)
                {
                    if (request.IsUp)
                    {
                        if (await DataProvider.ContentRepository.SetTaxisToUpAsync(site, channel, contentId, isTop) == false)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (await DataProvider.ContentRepository.SetTaxisToDownAsync(site, channel, contentId, isTop) == false)
                        {
                            break;
                        }
                    }
                }
            }

            await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);

            await auth.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "对内容排序", string.Empty);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
