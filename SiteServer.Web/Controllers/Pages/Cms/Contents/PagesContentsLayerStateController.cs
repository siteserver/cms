using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    [RoutePrefix("pages/cms/contents/contentsLayerState")]
    public partial class PagesContentsLayerStateController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentView))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return Request.BadRequest<GetResult>("无法确定内容对应的栏目");

            var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channel, request.ContentId);
            if (contentInfo == null) return Request.BadRequest<GetResult>("无法确定对应的内容");

            //var title = WebUtils.GetContentTitle(site, contentInfo, string.Empty);
            //var checkState =
            //    CheckManager.GetCheckState(site, contentInfo);

            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);
            var contentChecks = await DataProvider.ContentCheckRepository.GetCheckListAsync(tableName, request.ContentId);

            return new GetResult
            {
                ContentChecks = contentChecks,
                Content = contentInfo
            };
        }
    }
}
