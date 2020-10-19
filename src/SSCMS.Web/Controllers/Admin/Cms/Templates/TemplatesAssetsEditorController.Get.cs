using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesAssetsEditorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (request.FileType == "html")
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.TemplatesIncludes))
                {
                    return Unauthorized();
                }
            }
            else
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.TemplatesAssets))
                {
                    return Unauthorized();
                }
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var filePath = await _pathManager.GetSitePathAsync(site, request.DirectoryPath, request.FileName);
            var content = string.Empty;
            if (FileUtils.IsFileExists(filePath))
            {
                content = await FileUtils.ReadTextAsync(filePath);
            }

            var path = string.Empty;

            if (StringUtils.EqualsIgnoreCase(request.FileType, "html"))
            {
                path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(request.DirectoryPath, site.TemplatesAssetsIncludeDir,
                    string.Empty), request.FileName);
            }
            else if (StringUtils.EqualsIgnoreCase(request.FileType, "css"))
            {
                path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(request.DirectoryPath, site.TemplatesAssetsCssDir,
                    string.Empty), request.FileName);
            }
            else if (StringUtils.EqualsIgnoreCase(request.FileType, "js"))
            {
                path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(request.DirectoryPath, site.TemplatesAssetsJsDir,
                    string.Empty), request.FileName);
            }

            return new GetResult
            {
                Path = StringUtils.TrimSlash(PathUtils.RemoveExtension(path)),
                Content = content
            };
        }
    }
}
