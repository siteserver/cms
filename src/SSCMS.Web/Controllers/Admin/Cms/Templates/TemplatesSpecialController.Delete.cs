using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesSpecialController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<DeleteResult>> Delete([FromBody] SpecialIdRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var specialInfo = await _pathManager.DeleteSpecialAsync(site, request.SpecialId);

            await _authManager.AddSiteLogAsync(request.SiteId,
                "删除专题",
                $"专题名称:{specialInfo.Title}");

            var specialInfoList = await _specialRepository.GetSpecialsAsync(request.SiteId);

            return new DeleteResult
            {
                Specials = specialInfoList
            };
        }
    }
}