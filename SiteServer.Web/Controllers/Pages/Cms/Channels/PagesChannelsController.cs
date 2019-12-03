using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Channels
{
    
    [RoutePrefix("pages/cms/channels")]
    public partial class PagesChannelsController : ApiController
    {
        private const string Route = "";
        private const string RouteAppend = "actions/append";
        private const string RouteUp = "actions/up";
        private const string RouteDown = "actions/down";
        private const string RouteCreate = "actions/create";

        [HttpGet, Route(Route)]
        public async Task<Channel> Get([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.WebSitePermissions.Content);

            var channel = await ChannelManager.GetChannelAsync(request.SiteId, request.SiteId);
            if (channel == null)
            {
                auth.NotFound(Request);
            }
            else
            {
                channel.Children = await ChannelManager.GetChildrenAsync(request.SiteId, channel.Id);
            }

            return channel;
        }

        [HttpPost, Route(RouteAppend)]
        public async Task<DefaultResult> Append([FromBody] AppendRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.ChannelPermissions.ChannelAdd);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) auth.NotFound(Request);

            //foreach (var channelId in request.ChannelIds)
            //{
            //    await CreateManager.CreateChannelAsync(request.SiteId, channelId);
            //}

            return new DefaultResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteUp)]
        public async Task<DefaultResult> Taxis([FromBody] ChannelIdsRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.WebSitePermissions.Content);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null)
            {
                auth.NotFound(Request);
            }

            foreach (var channelId in request.ChannelIds)
            {
                await CreateManager.CreateChannelAsync(request.SiteId, channelId);
            }

            return new DefaultResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteCreate)]
        public async Task<DefaultResult> Create([FromBody] ChannelIdsRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.WebSitePermissions.Content);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null)
            {
                auth.NotFound(Request);
            }

            foreach (var channelId in request.ChannelIds)
            {
                await CreateManager.CreateChannelAsync(request.SiteId, channelId);
            }

            return new DefaultResult
            {
                Value = true
            };
        }
    }
}
