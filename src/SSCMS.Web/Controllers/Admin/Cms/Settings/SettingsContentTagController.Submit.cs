using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsContentTagController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsContentTag))
            {
                return Unauthorized();
            }

            var tagNames = await _contentTagRepository.GetTagNamesAsync(request.SiteId);
            foreach (var tagName in ListUtils.GetStringListByReturnAndNewline(request.TagNames))
            {
                if (ListUtils.Contains(tagNames, tagName)) continue;
                await _contentTagRepository.InsertAsync(request.SiteId, tagName);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "新增内容标签");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}