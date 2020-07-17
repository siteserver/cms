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

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerImageController : ControllerBase
    {
        private const string Route = "common/editor/layerImage";
        private const string RouteUpload = "common/editor/layerImage/actions/upload";

        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public LayerImageController(IPathManager pathManager, ISiteRepository siteRepository)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
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

            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, UploadType.Image);
            var filePath = PathUtils.Combine(localDirectoryPath, _pathManager.GetUploadFileName(site, fileName));

            await _pathManager.UploadAsync(file, filePath);

            var imageUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, filePath, true);

            return new UploadResult
            {
                Name = fileName,
                Path = filePath,
                Url = imageUrl
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<SubmitResult>>> Submit([FromBody] SubmitRequest request)
        {
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
                            ImageUrl = thumbnailUrl,
                            PreviewUrl = imageUrl
                        });
                    }
                    else
                    {
                        FileUtils.DeleteFileIfExists(filePath);
                        result.Add(new SubmitResult
                        {
                            ImageUrl = thumbnailUrl
                        });
                    }
                }
                else
                {
                    result.Add(new SubmitResult
                    {
                        ImageUrl = imageUrl
                    });
                }
            }

            return result;
        }
    }
}
