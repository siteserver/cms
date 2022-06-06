using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesAddController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromForm] IFormFile file)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesAdd))
            {
                return Unauthorized();
            }

            if (file == null) return this.Error(Constants.ErrorUpload);
            var extension = PathUtils.GetExtension(file.FileName);
            if (!FileUtils.IsZip(extension))
            {
                return this.Error("站点模板压缩包为zip格式，请选择有效的文件上传!");
            }
            var directoryName = PathUtils.GetFileNameWithoutExtension(file.FileName);
            var directoryPath = _pathManager.GetSiteFilesPath(PathUtils.Combine(DirectoryUtils.SiteFiles.SiteTemplates.DirectoryName, directoryName));
            if (DirectoryUtils.IsDirectoryExists(directoryPath))
            {
                return this.Error($"站点模板导入失败，文件夹{directoryName}已存在");
            }
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);
            var filePath = _pathManager.GetSiteFilesPath(PathUtils.Combine(DirectoryUtils.SiteFiles.SiteTemplates.DirectoryName, file.FileName));

            FileUtils.DeleteFileIfExists(filePath);

            await _pathManager.UploadAsync(file, filePath);

            _pathManager.ExtractZip(filePath, directoryPath);

            // var caching = new CacheUtils(_cacheManager);
            // var manager = new SiteTemplateManager(_pathManager, _databaseManager, caching);
            // var siteTemplates = manager.GetSiteTemplates();
            // SiteTemplate siteTemplate = null;
            // foreach (var value in siteTemplates)
            // {
            //     if (siteTemplate.DirectoryName == directoryName)
            //     {
            //       siteTemplate = value;
            //       siteTemplate.FileExists = true;
            //       break;
            //     }
            // }

            return new UploadResult
            {
                DirectoryName = directoryName
            };
        }
    }
}
