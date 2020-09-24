using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesAssetsEditorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] FileRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.TemplatesAssets))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var filePath = await _pathManager.GetSitePathAsync(site, request.DirectoryPath, request.FileName);
            var content = string.Empty;
            if (FileUtils.IsFileExists(filePath))
            {
                content = await FileUtils.ReadTextAsync(filePath);
            }
            var extName = StringUtils.ToLower(PathUtils.GetExtension(filePath));
            extName = StringUtils.ReplaceStartsWith(extName, ".", string.Empty);
            var path = string.Empty;

            if (StringUtils.EqualsIgnoreCase(extName, "html"))
            {
                path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(request.DirectoryPath, site.TemplatesAssetsIncludeDir,
                    string.Empty), request.FileName);
            }
            else if (StringUtils.EqualsIgnoreCase(extName, "css"))
            {
                path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(request.DirectoryPath, site.TemplatesAssetsCssDir,
                    string.Empty), request.FileName);
            }
            else if (StringUtils.EqualsIgnoreCase(extName, "js"))
            {
                path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(request.DirectoryPath, site.TemplatesAssetsJsDir,
                    string.Empty), request.FileName);
            }

            return new GetResult
            {
                Path = StringUtils.TrimSlash(PathUtils.RemoveExtension(path)),
                ExtName = extName,
                Content = content
            };
        }
    }
}
