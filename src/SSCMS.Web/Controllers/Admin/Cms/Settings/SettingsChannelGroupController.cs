using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsChannelGroupController : ControllerBase
    {
        private const string Route = "cms/settings/settingsChannelGroup";
        private const string RouteOrder = "cms/settings/settingsChannelGroup/actions/order";

        private readonly IAuthManager _authManager;
        private readonly IChannelGroupRepository _channelGroupRepository;

        public SettingsChannelGroupController(IAuthManager authManager, IChannelGroupRepository channelGroupRepository)
        {
            _authManager = authManager;
            _channelGroupRepository = channelGroupRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsChannelGroup))
            {
                return Unauthorized();
            }

            var groups = await _channelGroupRepository.GetChannelGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<GetResult>> Delete([FromBody]DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsChannelGroup))
            {
                return Unauthorized();
            }

            await _channelGroupRepository.DeleteAsync(request.SiteId, request.GroupName);

            var groups = await _channelGroupRepository.GetChannelGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }

        [HttpPost, Route(RouteOrder)]
        public async Task<ActionResult<GetResult>> Order([FromBody] OrderRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsChannelGroup))
            {
                return Unauthorized();
            }

            if (request.IsUp)
            {
                await _channelGroupRepository.UpdateTaxisUpAsync(request.SiteId, request.GroupId, request.Taxis);
            }
            else
            {
                await _channelGroupRepository.UpdateTaxisDownAsync(request.SiteId, request.GroupId, request.Taxis);
            }

            var groups = await _channelGroupRepository.GetChannelGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }
    }
}
