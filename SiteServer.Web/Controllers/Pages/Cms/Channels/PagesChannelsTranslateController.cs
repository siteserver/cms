using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Channels
{
    [RoutePrefix("pages/cms/channels/channelsTranslate")]
    public partial class PagesChannelsTranslateController : ApiController
    {
        private const string Route = "";
        private const string RouteOptions = "actions/options";

        private readonly ICreateManager _createManager;

        public PagesChannelsTranslateController(ICreateManager createManager)
        {
            _createManager = createManager;
        }

        [HttpGet, Route(Route)]
        public async Task<GetResult> GetConfig([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ChannelsTranslate))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.SiteId);
            var cascade = await DataProvider.ChannelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, summary);
                return new
                {
                    Count = count,
                    summary.IndexName
                };
            });

            var transSites = await DataProvider.SiteRepository.GetSelectsAsync();
            var translateTypes = TranslateUtils.GetEnums<TranslateType>().Select(x => new Select<string>(x));

            return new GetResult
            {
                Channels = cascade,
                TransSites = transSites,
                TranslateTypes = translateTypes
            };
        }

        [HttpPost, Route(RouteOptions)]
        public async Task<GetOptionsResult> GetOptions([FromBody]GetOptionsRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigCrossSiteTrans))
            {
                return Request.Unauthorized<GetOptionsResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<GetOptionsResult>("无法确定内容对应的站点");

            var transChannels = await DataProvider.ChannelRepository.GetAsync(request.TransSiteId);
            var transSite = await DataProvider.SiteRepository.GetAsync(request.TransSiteId);
            var cascade = await DataProvider.ChannelRepository.GetCascadeAsync(transSite, transChannels, async summary =>
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, summary);

                return new
                {
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
        public async Task<BoolResult> Translate([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ChannelsTranslate))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            await TranslateAsync(site, request.TransSiteId, request.TransChannelId, request.TranslateType, request.ChannelIds, request.IsDeleteAfterTranslate, auth.AdminId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
