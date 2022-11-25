using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Material
{
    public partial class LayerImageSelectController
    {
        [HttpPost, Route(RouteSelect)]
        public async Task<ActionResult<StringResult>> Select([FromBody] SelectRequest request)
        {
            var image = await _materialImageRepository.GetAsync(request.MaterialId);
            var materialFilePath = PathUtils.Combine(_settingsManager.WebRootPath, image.Url);
            if (!FileUtils.IsFileExists(materialFilePath))
            {
                return this.Error("图片文件不存在，请重新选择");
            }

            var imageUrl = string.Empty;
            if (request.SiteId > 0)
            {
                var site = await _siteRepository.GetAsync(request.SiteId);
                var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, UploadType.Image);
                var filePath = PathUtils.Combine(localDirectoryPath, _pathManager.GetUploadFileName(site, materialFilePath));

                DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                FileUtils.CopyFile(materialFilePath, filePath);

                imageUrl = await _pathManager.GetVirtualUrlByPhysicalPathAsync(site, filePath);
            }
            else
            {
                imageUrl = image.Url;
            }

            return new StringResult
            {
                Value = imageUrl
            };
        }
    }
}
