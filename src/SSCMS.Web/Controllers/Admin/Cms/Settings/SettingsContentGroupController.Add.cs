using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsContentGroupController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Add([FromBody] ChannelGroup request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsContentGroup))
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
    }
}