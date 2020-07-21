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

namespace SSCMS.Web.Controllers.Admin.Cms.Library
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerGroupAddController : ControllerBase
    {
        private const string Route = "cms/library/layerGroupAdd";

        private readonly IAuthManager _authManager;
        private readonly ILibraryGroupRepository _libraryGroupRepository;

        public LayerGroupAddController(IAuthManager authManager, ILibraryGroupRepository libraryGroupRepository)
        {
            _authManager = authManager;
            _libraryGroupRepository = libraryGroupRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<StringResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryCard,
                AuthTypes.SitePermissions.LibraryImage,
                AuthTypes.SitePermissions.LibraryVideo,
                AuthTypes.SitePermissions.LibraryAudio,
                AuthTypes.SitePermissions.LibraryFile))
            {
                return Unauthorized();
            }

            var group = await _libraryGroupRepository.GetAsync(request.GroupId);

            return new StringResult
            {
                Value = group.GroupName
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<CreateResult>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryCard,
                AuthTypes.SitePermissions.LibraryImage,
                AuthTypes.SitePermissions.LibraryVideo,
                AuthTypes.SitePermissions.LibraryAudio,
                AuthTypes.SitePermissions.LibraryFile))
            {
                return Unauthorized();
            }

            if (await _libraryGroupRepository.IsExistsAsync(request.LibraryType, request.GroupName))
            {
                return this.Error("分组名称已存在，请使用其他名称");
            }

            await _libraryGroupRepository.InsertAsync(new LibraryGroup
            {
                LibraryType = request.LibraryType,
                GroupName = request.GroupName
            });

            var groups = await _libraryGroupRepository.GetAllAsync(request.LibraryType);

            return new CreateResult
            {
                Groups = groups
            };
        }

        [HttpPut, Route(Route)]
        public async Task<ActionResult<UpdateResult>> Update([FromBody] UpdateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryCard,
                AuthTypes.SitePermissions.LibraryImage,
                AuthTypes.SitePermissions.LibraryVideo,
                AuthTypes.SitePermissions.LibraryAudio,
                AuthTypes.SitePermissions.LibraryFile))
            {
                return Unauthorized();
            }

            var group = await _libraryGroupRepository.GetAsync(request.GroupId);

            if (group.GroupName != request.GroupName)
            {
                if (await _libraryGroupRepository.IsExistsAsync(group.LibraryType, request.GroupName))
                {
                    return this.Error("分组名称已存在，请使用其他名称");
                }

                group.GroupName = request.GroupName;
                await _libraryGroupRepository.UpdateAsync(group);
            }

            var groups = await _libraryGroupRepository.GetAllAsync(group.LibraryType);

            return new UpdateResult
            {
                Groups = groups
            };
        }
    }
}
