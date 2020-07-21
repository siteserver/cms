using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Library
{
    public partial class ImageController
    {
        [HttpPost, Route(RouteActionsPull)]
        public async Task<ActionResult<BoolResult>> Pull([FromBody] PullRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryImage))
            {
                return Unauthorized();
            }

            var account = await _openAccountRepository.GetBySiteIdAsync(request.SiteId);
            var (success, token, errorMessage) = _openManager.GetWxAccessToken(account.WxAppId, account.WxAppSecret);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            var count = await MediaApi.GetMediaCountAsync(token);
            var list = await MediaApi.GetOthersMediaListAsync(token, UploadMediaFileType.image, 0, count.image_count);

            foreach (var image in list.item)
            {
                if (await _libraryImageRepository.IsExistsAsync(image.media_id)) continue;

                await using var ms = new MemoryStream();
                await MediaApi.GetForeverMediaAsync(token, image.media_id, ms);
                ms.Seek(0, SeekOrigin.Begin);

                var extName = image.url.Substring(image.url.LastIndexOf("=", StringComparison.Ordinal) + 1);

                var libraryFileName = PathUtils.GetLibraryFileNameByExtName(extName);
                var virtualDirectoryPath = PathUtils.GetLibraryVirtualDirectoryPath(UploadType.Image);

                var directoryPath = PathUtils.Combine(_settingsManager.WebRootPath, virtualDirectoryPath);
                var filePath = PathUtils.Combine(directoryPath, libraryFileName);

                await FileUtils.WriteStreamAsync(filePath, ms);

                var library = new LibraryImage
                {
                    GroupId = request.GroupId,
                    Title = image.name,
                    Url = PageUtils.Combine(virtualDirectoryPath, libraryFileName),
                    MediaId = image.media_id
                };

                await _libraryImageRepository.InsertAsync(library);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
