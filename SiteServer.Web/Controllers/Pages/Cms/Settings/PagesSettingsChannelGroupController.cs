using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    
    [RoutePrefix("pages/cms/settings/settingsChannelGroup")]
    public partial class PagesSettingsChannelGroupController : ApiController
    {
        private const string Route = "";
        private const string RouteOrder = "actions/order";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Request.Unauthorized<GetResult>();
            }

            var groups = await DataProvider.ChannelGroupRepository.GetChannelGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<GetResult> Delete([FromBody]DeleteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Request.Unauthorized<GetResult>();
            }

            await DataProvider.ChannelGroupRepository.DeleteAsync(request.SiteId, request.GroupName);

            var groups = await DataProvider.ChannelGroupRepository.GetChannelGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }

        [HttpPost, Route(RouteOrder)]
        public async Task<GetResult> Order([FromBody] OrderRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigGroups))
            {
                return Request.Unauthorized<GetResult>();
            }

            if (request.IsUp)
            {
                await DataProvider.ChannelGroupRepository.UpdateTaxisUpAsync(request.SiteId, request.GroupId, request.Taxis);
            }
            else
            {
                await DataProvider.ChannelGroupRepository.UpdateTaxisDownAsync(request.SiteId, request.GroupId, request.Taxis);
            }

            var groups = await DataProvider.ChannelGroupRepository.GetChannelGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }
    }
}
