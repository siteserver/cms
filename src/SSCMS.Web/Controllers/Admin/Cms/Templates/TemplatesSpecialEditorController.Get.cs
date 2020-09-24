using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesSpecialEditorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var specialInfo = await _specialRepository.GetSpecialAsync(request.SiteId, request.SpecialId);

            if (specialInfo == null)
            {
                return this.Error("专题不存在！");
            }

            var specialUrl = await _pathManager.ParseSiteUrlAsync(site, $"@/{StringUtils.TrimSlash(specialInfo.Url)}/", true);
            var filePath = PathUtils.Combine(await _pathManager.GetSpecialDirectoryPathAsync(site, specialInfo.Url), "index.html");
            var html = FileUtils.ReadText(filePath, Encoding.UTF8);

            return new GetResult
            {
                Special = specialInfo,
                SpecialUrl = specialUrl,
                Html = html
            };
        }
    }
}