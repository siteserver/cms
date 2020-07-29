using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class FileController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<MaterialFile>> Create([FromQuery] CreateRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.MaterialFile))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var fileType = PathUtils.GetExtension(fileName);
            if (!_pathManager.IsUploadExtensionAllowed(UploadType.File, site, fileType))
            {
                return this.Error("此格式不允许上传，请选择有效的文件上传!");
            }

            var materialFileName = PathUtils.GetMaterialFileName(fileName);
            var virtualDirectoryPath = PathUtils.GetMaterialVirtualDirectoryPath(UploadType.File);

            var directoryPath = PathUtils.Combine(_settingsManager.WebRootPath, virtualDirectoryPath);
            var filePath = PathUtils.Combine(directoryPath, materialFileName);

            await _pathManager.UploadAsync(file, filePath);

            var material = new MaterialFile
            {
                GroupId = request.GroupId,
                Title = PathUtils.RemoveExtension(fileName),
                FileType = fileType.ToUpper().Replace(".", string.Empty),
                Url = PageUtils.Combine(virtualDirectoryPath, materialFileName)
            };

            await _materialFileRepository.InsertAsync(material);

            return material;
        }
    }
}
