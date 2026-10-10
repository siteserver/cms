using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesAssetsEditorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
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
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var path = string.Empty;
            var content = string.Empty;

            var assetsDir = GetAssetsDir(site, request.FileType);
            if (string.IsNullOrEmpty(assetsDir))
            {
                return this.Error("文件获取失败，必须为Html/Css/Js文件！");
            }

            var directoryPath = request.DirectoryPath;
            var fileName = request.FileName;

            if (!string.IsNullOrEmpty(fileName))
            {
                // 路径必须位于对应类型的资源目录内且扩展名匹配，否则拒绝（防止任意文件读取）
                var filePath = await ResolveAssetPathAsync(site, request.FileType, directoryPath, fileName);
                if (filePath == null)
                {
                    return this.Error("文件获取失败，文件路径不正确！");
                }

                if (FileUtils.IsFileExists(filePath))
                {
                    content = await FileUtils.ReadTextAsync(filePath);
                }

                path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(directoryPath ?? string.Empty, assetsDir,
                    string.Empty), fileName);
                path = StringUtils.TrimSlash(PathUtils.RemoveExtension(path));
            }

            return new GetResult
            {
                TemplatesAssetsIncludeDir = site.TemplatesAssetsIncludeDir,
                TemplatesAssetsCssDir = site.TemplatesAssetsCssDir,
                TemplatesAssetsJsDir = site.TemplatesAssetsJsDir,
                Path = path,
                Content = content
            };
        }
    }
}
