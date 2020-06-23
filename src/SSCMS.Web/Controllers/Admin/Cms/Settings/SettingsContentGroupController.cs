using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsContentGroupController : ControllerBase
    {
        private const string Route = "cms/settings/settingsContentGroup";
        private const string RouteOrder = "cms/settings/settingsContentGroup/actions/order";

        private readonly IAuthManager _authManager;
        private readonly IContentGroupRepository _contentGroupRepository;

        public SettingsContentGroupController(IAuthManager authManager, IContentGroupRepository contentGroupRepository)
        {
            _authManager = authManager;
            _contentGroupRepository = contentGroupRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsContentGroup))
            {
                return Unauthorized();
            }

            var groups = await _contentGroupRepository.GetContentGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<GetResult>> Delete([FromBody]DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsContentGroup))
            {
                return Unauthorized();
            }

            await _contentGroupRepository.DeleteAsync(request.SiteId, request.GroupName);

            var groups = await _contentGroupRepository.GetContentGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Add([FromBody] ChannelGroup request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsContentGroup))
            {
                return Unauthorized();
            }

            if (await _contentGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return this.Error("保存失败，已存在相同名称的内容组！");
            }

            var groupInfo = new ContentGroup
            {
                SiteId = request.SiteId,
                GroupName = request.GroupName,
                Description = request.Description
            };

            await _contentGroupRepository.InsertAsync(groupInfo);

            await _authManager.AddSiteLogAsync(request.SiteId, "新增内容组", $"内容组:{groupInfo.GroupName}");

            var groups = await _contentGroupRepository.GetContentGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }

        [HttpPut, Route(Route)]
        public async Task<ActionResult<GetResult>> Edit([FromBody] ChannelGroup request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsContentGroup))
            {
                return Unauthorized();
            }

            var groupInfo = await _contentGroupRepository.GetAsync(request.SiteId, request.Id);

            if (groupInfo.GroupName != request.GroupName && await _contentGroupRepository.IsExistsAsync(request.SiteId, request.GroupName))
            {
                return this.Error("保存失败，已存在相同名称的内容组！");
            }

            groupInfo.GroupName = request.GroupName;
            groupInfo.Description = request.Description;

            await _contentGroupRepository.UpdateAsync(groupInfo);

            await _authManager.AddSiteLogAsync(request.SiteId, "修改内容组", $"内容组:{groupInfo.GroupName}");

            var groups = await _contentGroupRepository.GetContentGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }

        [HttpPost, Route(RouteOrder)]
        public async Task<ActionResult<GetResult>> Order([FromBody] OrderRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsContentGroup))
            {
                return Unauthorized();
            }

            if (request.IsUp)
            {
                await _contentGroupRepository.UpdateTaxisUpAsync(request.SiteId, request.GroupId, request.Taxis);
            }
            else
            {
                await _contentGroupRepository.UpdateTaxisDownAsync(request.SiteId, request.GroupId, request.Taxis);
            }

            var groups = await _contentGroupRepository.GetContentGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }
    }
}
