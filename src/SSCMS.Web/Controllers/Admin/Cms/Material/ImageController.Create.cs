using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class ImageController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(Route)]
        public async Task<ActionResult<MaterialImage>> Create([FromQuery] CreateRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.MaterialImage))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);
            var extName = PathUtils.GetExtension(fileName);
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (!_pathManager.IsImageExtensionAllowed(site, extName))
            {
                return this.Error("此图片格式已被禁止上传，请转换格式后上传!");
            }

            var materialFileName = PathUtils.GetMaterialFileName(fileName);
            var virtualDirectoryPath = PathUtils.GetMaterialVirtualDirectoryPath(UploadType.Image);

            var directoryPath = PathUtils.Combine(_settingsManager.WebRootPath, virtualDirectoryPath);
            var filePath = PathUtils.Combine(directoryPath, materialFileName);

            await _pathManager.UploadAsync(file, filePath);
            await _pathManager.AddWaterMarkAsync(site, filePath);

            var image = new MaterialImage
            {
                GroupId = request.GroupId,
                Title = fileName,
                Url = PageUtils.Combine(virtualDirectoryPath, materialFileName)
            };

            await _materialImageRepository.InsertAsync(image);

            return image;
        }
    }
}
