using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Extensions;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class LayerGroupAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<CreateResult>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.MaterialMessage,
                AuthTypes.SitePermissions.MaterialImage,
                AuthTypes.SitePermissions.MaterialVideo,
                AuthTypes.SitePermissions.MaterialAudio,
                AuthTypes.SitePermissions.MaterialFile))
            {
                return Unauthorized();
            }

            if (await _materialGroupRepository.IsExistsAsync(request.LibraryType, request.GroupName))
            {
                return this.Error("分组名称已存在，请使用其他名称");
            }

            await _materialGroupRepository.InsertAsync(new MaterialGroup
            {
                LibraryType = request.LibraryType,
                GroupName = request.GroupName
            });

            var groups = await _materialGroupRepository.GetAllAsync(request.LibraryType);

            return new CreateResult
            {
                Groups = groups
            };
        }
    }
}
