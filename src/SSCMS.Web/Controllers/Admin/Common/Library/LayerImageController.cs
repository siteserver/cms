using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Extensions;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Library
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerImageController : ControllerBase
    {
        private const string Route = "common/library/layerImage";
        private const string RouteUpload = "common/library/layerImage/actions/upload";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public LayerImageController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery]SiteRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.LibraryImage))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".jpg", ".jpeg", ".bmp", ".gif", ".png", ".webp"))
            {
                return this.Error("文件只能是 Image 格式，请选择有效的文件上传!");
            }

            var virtualUrl = PathUtils.GetLibraryVirtualFilePath(UploadType.Image, _pathManager.GetUploadFileName(site, fileName));
            var filePath = PathUtils.Combine(_settingsManager.WebRootPath, virtualUrl);

            await _pathManager.UploadAsync(file, filePath);

            return new UploadResult
            {
                Name = fileName,
                Path = filePath,
                Url = virtualUrl
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<SubmitResult>>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.LibraryImage))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var result = new List<SubmitResult>();
            foreach (var filePath in request.FilePaths)
            {
                if (string.IsNullOrEmpty(filePath)) continue;

                var fileName = PathUtils.GetFileName(filePath);

                var fileExtName = StringUtils.ToLower(PathUtils.GetExtension(filePath));
                var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, fileExtName);

                var imageUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, filePath, true);

                if (request.IsThumb)
                {
                    var localSmallFileName = Constants.SmallImageAppendix + fileName;
                    var localSmallFilePath = PathUtils.Combine(localDirectoryPath, localSmallFileName);

                    var thumbnailUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, localSmallFilePath, true);

                    var width = request.ThumbWidth;
                    var height = request.ThumbHeight;
                    OldImageUtils.MakeThumbnail(filePath, localSmallFilePath, width, height, true);

                    if (request.IsLinkToOriginal)
                    {
                        result.Add(new SubmitResult
                        {
                            Url = imageUrl,
                            ThumbUrl = thumbnailUrl
                        });
                    }
                    else
                    {
                        FileUtils.DeleteFileIfExists(filePath);
                        result.Add(new SubmitResult
                        {
                            Url = thumbnailUrl
                        });
                    }
                }
                else
                {
                    result.Add(new SubmitResult
                    {
                        Url = imageUrl
                    });
                }
            }

            return result;
        }
    }
}
