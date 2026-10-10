using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesAssetsController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] FileRequest request)
        {
            if (_settingsManager.IsSafeMode)
            {
                return this.Error(Constants.ErrorSafeMode);
            }
            
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesAssets))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var directoryPath = request.DirectoryPath;
            var fileName = request.FileName;
            var assetsDir = GetAssetsDir(site, request.FileType);
            if (string.IsNullOrEmpty(assetsDir) ||
                !AssetPathUtils.IsSafeFileName(fileName) ||
                !AssetPathUtils.IsSafeRelativePath(directoryPath))
            {
                return this.Error("资源文件路径错误！");
            }

            // 只允许删除对应类型资源目录内的文件
            var rootPath = await _pathManager.GetSitePathAsync(site, assetsDir);
            var filePath = await _pathManager.GetSitePathAsync(site, directoryPath, fileName);
            if (!AssetPathUtils.IsUnder(rootPath, filePath))
            {
                return this.Error("资源文件路径错误！");
            }

            FileUtils.DeleteFileIfExists(filePath);
            await _authManager.AddSiteLogAsync(request.SiteId, "删除资源文件", $"{directoryPath}:{fileName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
