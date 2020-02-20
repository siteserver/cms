using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    [RoutePrefix("pages/cms/contents/contentsLayerTaxis")]
    public partial class PagesContentsLayerTaxisController : ApiController
    {
        private const string Route = "";

        private readonly ICreateManager _createManager;

        public PagesContentsLayerTaxisController(ICreateManager createManager)
        {
            _createManager = createManager;
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

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
            var isUp = request.IsUp;

            if (isUp == false)
            {
                summaries.Reverse();
            }

            foreach (var summary in summaries)
            {
                var channel = await DataProvider.ChannelRepository.GetAsync(summary.ChannelId);
                var content = await DataProvider.ContentRepository.GetAsync(site, channel, summary.Id);
                if (content == null) continue;

                var isTop = content.Top;
                for (var i = 1; i <= request.Taxis; i++)
                {
                    if (isUp)
                    {
                        if (await DataProvider.ContentRepository.SetTaxisToUpAsync(site, channel, summary.Id, isTop) == false)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (await DataProvider.ContentRepository.SetTaxisToDownAsync(site, channel, summary.Id, isTop) == false)
                        {
                            break;
                        }
                    }
                }
            }

            foreach (var distinctChannelId in summaries.Select(x => x.ChannelId).Distinct())
            {
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, distinctChannelId);
            }

            await auth.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "对内容排序", string.Empty);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
