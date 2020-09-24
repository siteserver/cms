using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsContentTagController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Add([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.SettingsContentTag))
            {
                return Unauthorized();
            }

            foreach (var tagName in request.TagNames)
            {
                await _contentTagRepository.InsertAsync(request.SiteId, tagName);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "新增内容标签", $"内容标签:{ListUtils.ToString(request.TagNames)}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}