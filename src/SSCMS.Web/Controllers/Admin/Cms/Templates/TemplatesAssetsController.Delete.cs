using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesAssetsController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] FileRequest request)
        {
            if (request.FileType == "html")
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesIncludes))
                {
                    return Unauthorized();
                }
            }
            else
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesAssets))
                {
                    return Unauthorized();
                }
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            FileUtils.DeleteFileIfExists(await _pathManager.GetSitePathAsync(site, request.DirectoryPath, request.FileName));
            await _authManager.AddSiteLogAsync(request.SiteId, "删除资源文件", $"{request.DirectoryPath}:{request.FileName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
