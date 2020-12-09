using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class LayerGroupAddController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<UpdateResult>> Update([FromBody] UpdateRequest request)
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

            var group = await _materialGroupRepository.GetAsync(request.GroupId);

            if (group.GroupName != request.GroupName)
            {
                if (await _materialGroupRepository.IsExistsAsync(group.MaterialType, request.GroupName))
                {
                    return this.Error("分组名称已存在，请使用其他名称");
                }

                group.GroupName = request.GroupName;
                await _materialGroupRepository.UpdateAsync(group);
            }

            var groups = await _materialGroupRepository.GetAllAsync(group.MaterialType);

            return new UpdateResult
            {
                Groups = groups
            };
        }
    }
}
