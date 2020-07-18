using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Form
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerImageUploadController : ControllerBase
    {
        private const string Route = "common/form/layerImageUpload";
        private const string RouteUpload = "common/form/layerImageUpload/actions/upload";

        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ILibraryImageRepository _libraryImageRepository;

        public LayerImageUploadController(IPathManager pathManager, ISiteRepository siteRepository, ILibraryImageRepository libraryImageRepository)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _libraryImageRepository = libraryImageRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<Options>> Get([FromQuery] SiteRequest request)
        {
            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var options = TranslateUtils.JsonDeserialize(site.Get<string>(nameof(LayerImageUploadController)), new Options
            {
                IsEditor = true,
                IsLibrary = true,
                IsThumb = false,
                ThumbWidth = 1024,
                ThumbHeight = 1024,
                IsLinkToOriginal = true
            });

            return options;
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery]int siteId,  [FromForm]IFormFile file)
        {
            var site = await _siteRepository.GetAsync(siteId);

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var extName = PathUtils.GetExtension(fileName);
            if (!_pathManager.IsImageExtensionAllowed(site, extName))
            {
                return this.Error("此图片格式已被禁止上传，请转换格式后上传!");
            }

            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, UploadType.Image);
            var filePath = PathUtils.Combine(localDirectoryPath, _pathManager.GetUploadFileName(site, fileName));

            await _pathManager.UploadAsync(file, filePath);

            return new UploadResult
            {
                Name = fileName,
                Path = filePath
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

                var virtualUrl = await _pathManager.GetVirtualUrlByPhysicalPathAsync(site, filePath);
                var imageUrl = await _pathManager.ParseSiteUrlAsync(site, virtualUrl, true);

                if (request.IsLibrary)
                {
                    var libraryFileName = PathUtils.GetLibraryFileName(fileName);
                    var virtualDirectoryPath = PathUtils.GetLibraryVirtualDirectoryPath(UploadType.Image);

                    var directoryPath = _pathManager.ParsePath(virtualDirectoryPath);
                    var libraryFilePath = PathUtils.Combine(directoryPath, libraryFileName);
                    DirectoryUtils.CreateDirectoryIfNotExists(libraryFilePath);

                    FileUtils.CopyFile(filePath, libraryFilePath, true);

                    var library = new LibraryImage
                    {
                        GroupId = -request.SiteId,
                        Title = fileName,
                        Url = PageUtils.Combine(virtualDirectoryPath, libraryFileName)
                    };

                    await _libraryImageRepository.InsertAsync(library);
                }
                
                if (request.IsThumb)
                {
                    var localSmallFileName = Constants.SmallImageAppendix + fileName;
                    var localSmallFilePath = PathUtils.Combine(localDirectoryPath, localSmallFileName);

                    var thumbnailVirtualUrl = await _pathManager.GetVirtualUrlByPhysicalPathAsync(site, localSmallFilePath);
                    var thumbnailUrl = await _pathManager.ParseSiteUrlAsync(site, thumbnailVirtualUrl, true);

                    ImageUtils.ResizeByMax(filePath, localSmallFilePath, request.ThumbWidth, request.ThumbHeight);

                    if (request.IsLinkToOriginal)
                    {
                        result.Add(new SubmitResult
                        {
                            ImageUrl = thumbnailUrl,
                            ImageVirtualUrl = thumbnailVirtualUrl,
                            PreviewUrl = imageUrl,
                            PreviewVirtualUrl = virtualUrl
                        });
                    }
                    else
                    {
                        FileUtils.DeleteFileIfExists(filePath);
                        result.Add(new SubmitResult
                        {
                            ImageUrl = thumbnailUrl,
                            ImageVirtualUrl = thumbnailVirtualUrl
                        });
                    }
                }
                else
                {
                    result.Add(new SubmitResult
                    {
                        ImageUrl = imageUrl,
                        ImageVirtualUrl = virtualUrl
                    });
                }
            }

            var options = TranslateUtils.JsonDeserialize(site.Get<string>(nameof(LayerImageUploadController)), new Options
            {
                IsEditor = true,
                IsLibrary = true,
                IsThumb = false,
                ThumbWidth = 1024,
                ThumbHeight = 1024,
                IsLinkToOriginal = true,
            });

            options.IsEditor = request.IsEditor;
            options.IsLibrary = request.IsLibrary;
            options.IsThumb = request.IsThumb;
            options.ThumbWidth = request.ThumbWidth;
            options.ThumbHeight = request.ThumbHeight;
            options.IsLinkToOriginal = request.IsLinkToOriginal;
            site.Set(nameof(LayerImageUploadController), TranslateUtils.JsonSerialize(options));

            await _siteRepository.UpdateAsync(site);

            return result;
        }
    }
}
