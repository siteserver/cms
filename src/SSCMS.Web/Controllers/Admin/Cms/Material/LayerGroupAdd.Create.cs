using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class LayerGroupAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<CreateResult>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.MaterialMessage,
                Types.SitePermissions.MaterialImage,
                Types.SitePermissions.MaterialVideo,
                Types.SitePermissions.MaterialAudio,
                Types.SitePermissions.MaterialFile))
            {
                return Unauthorized();
            }

            if (await _materialGroupRepository.IsExistsAsync(request.MaterialType, request.GroupName))
            {
                return this.Error("分组名称已存在，请使用其他名称");
            }

            await _materialGroupRepository.InsertAsync(new MaterialGroup
            {
                MaterialType = request.MaterialType,
                GroupName = request.GroupName
            });

            var groups = await _materialGroupRepository.GetAllAsync(request.MaterialType);

            return new CreateResult
            {
                Groups = groups
            };
        }
    }
}
