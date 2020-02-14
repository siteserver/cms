using System.Threading.Tasks;
using System.Web.Http;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Editor
{
    [RoutePrefix("pages/cms/contents/editorLayerTranslate")]
    public partial class PagesEditorLayerTranslateController : ApiController
    {
        private const string Route = "";
        private const string RouteOptions = "actions/options";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] ChannelRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentTranslate))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var siteIdList = await auth.AdminPermissions.GetSiteIdListAsync();
            var transSites = await DataProvider.SiteRepository.GetSelectsAsync(siteIdList);

            return new GetResult
            {
                TransSites = transSites
            };
        }

        [HttpPost, Route(RouteOptions)]
        public async Task<GetOptionsResult> GetOptions([FromBody]GetOptionsRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentTranslate))
            {
                return Request.Unauthorized<GetOptionsResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetOptionsResult>();

            var channelIdList = await auth.AdminPermissions.GetChannelIdListAsync(request.TransSiteId, Constants.ChannelPermissions.ContentAdd);

            var transChannels = await DataProvider.ChannelRepository.GetAsync(request.TransSiteId);
            var transSite = await DataProvider.SiteRepository.GetAsync(request.TransSiteId);
            var cascade = await DataProvider.ChannelRepository.GetCascadeAsync(transSite, transChannels, async summary =>
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, summary);

                return new
                {
                    Disabled = !channelIdList.Contains(summary.Id),
                    summary.IndexName,
                    Count = count
                };
            });

            return new GetOptionsResult
            {
                TransChannels = cascade
            };
        }

        [HttpPost, Route(Route)]
        public async Task<StringResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentTranslate))
            {
                return Request.Unauthorized<StringResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<StringResult>();

            var transSite = await DataProvider.SiteRepository.GetAsync(request.TransSiteId);
            var siteName = transSite.SiteName;

            var name = await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(request.TransSiteId, request.TransChannelId);
            if (request.TransSiteId != request.SiteId)
            {
                name = siteName + " : " + name;
            }

            name += $" ({request.TransType.GetDisplayName()})";

            return new StringResult
            {
                Value = name
            };
        }
    }
}
