using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class LayerGroupAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<CreateResult>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialMessage,
                MenuUtils.SitePermissions.MaterialImage,
                MenuUtils.SitePermissions.MaterialVideo,
                MenuUtils.SitePermissions.MaterialAudio,
                MenuUtils.SitePermissions.MaterialFile))
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
