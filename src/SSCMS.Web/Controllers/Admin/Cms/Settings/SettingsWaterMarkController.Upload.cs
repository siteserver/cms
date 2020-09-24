using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsWaterMarkController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasChannelPermissionsAsync(request.SiteId, request.SiteId, Types.SitePermissions.SettingsWaterMark))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var fileExtName = StringUtils.ToLower(PathUtils.GetExtension(fileName));
            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, fileExtName);
            var localFileName = _pathManager.GetUploadFileName(site, fileName);
            var filePath = PathUtils.Combine(localDirectoryPath, localFileName);

            if (!FileUtils.IsImage(fileExtName))
            {
                return this.Error("请选择有效的图片文件上传");
            }

            await _pathManager.UploadAsync(file, filePath);

            var imageUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, filePath, true);
            var virtualUrl = _pathManager.GetVirtualUrl(site, imageUrl);

            return new UploadResult
            {
                ImageUrl = imageUrl,
                VirtualUrl = virtualUrl
            };
        }
    }
}