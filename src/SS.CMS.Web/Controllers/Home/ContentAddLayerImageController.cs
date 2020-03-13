using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Core;
using SS.CMS.Core.Images;
using SS.CMS.Extensions;

namespace SS.CMS.Web.Controllers.Home
{
    [Route("home/contentAddLayerImage")]
    public partial class ContentAddLayerImageController : ControllerBase
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public ContentAddLayerImageController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]ChannelRequest request)
        {
            if (!await _authManager.IsUserAuthenticatedAsync() ||
                !await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            return new GetResult
            {
                Site = site
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] ChannelRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.IsUserAuthenticatedAsync() ||
                !await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".jpg", ".jpeg", ".bmp", ".gif", ".png", ".webp"))
            {
                return this.Error("文件只能是 Image 格式，请选择有效的文件上传!");
            }

            var filePath = file.FileName;
            var fileExtName = PathUtils.GetExtension(filePath).ToLower();
            var localDirectoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, fileExtName);
            var localFileName = _pathManager.GetUploadFileName(site, filePath);
            var path = PathUtils.Combine(localDirectoryPath, localFileName);
            var contentLength = file.Length;

            if (!_pathManager.IsImageExtensionAllowed(site, fileExtName))
            {
                return this.Error("上传失败，上传图片格式不正确！");
            }
            if (!_pathManager.IsImageSizeAllowed(site, contentLength))
            {
                return this.Error("上传失败，上传图片超出规定文件大小！");
            }

            await _pathManager.UploadAsync(file, filePath);

            await FileUtility.AddWaterMarkAsync(_pathManager, site, path);

            var url = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, path, true);

            return new UploadResult
            {
                Path = path,
                Url = url,
                ContentLength = contentLength
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody]SubmitRequest request)
        {
            if (!await _authManager.IsUserAuthenticatedAsync() ||
                !await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return this.Error("无法确定内容对应的栏目");

            var retVal = new List<string>();
            var editors = new List<object>();

            foreach (var filePath in request.FilePaths)
            {
                if (string.IsNullOrEmpty(filePath)) continue;

                var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                var fileName = _pathManager.GetUploadFileName(site, filePath);

                var directoryPath = await _pathManager.GetUploadDirectoryPathAsync(site, fileExtName);
                var fixFilePath = PathUtils.Combine(directoryPath, Constants.TitleImageAppendix + fileName);
                var editorFixFilePath = PathUtils.Combine(directoryPath, Constants.SmallImageAppendix + fileName);

                var isImage = FileUtils.IsImage(fileExtName);

                if (isImage)
                {
                    if (request.IsFix)
                    {
                        var width = TranslateUtils.ToInt(request.FixWidth);
                        var height = TranslateUtils.ToInt(request.FixHeight);
                        ImageUtils.MakeThumbnail(filePath, fixFilePath, width, height, true);
                    }

                    if (request.IsEditor)
                    {
                        if (request.EditorIsFix)
                        {
                            var width = TranslateUtils.ToInt(request.EditorFixWidth);
                            var height = TranslateUtils.ToInt(request.EditorFixHeight);
                            ImageUtils.MakeThumbnail(filePath, editorFixFilePath, width, height, true);
                        }
                    }
                }

                var imageUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, filePath, true);
                var fixImageUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, fixFilePath, true);
                var editorFixImageUrl = await _pathManager.GetSiteUrlByPhysicalPathAsync(site, editorFixFilePath, true);

                retVal.Add(request.IsFix ? fixImageUrl : imageUrl);

                editors.Add(new
                {
                    ImageUrl = request.IsFix ? editorFixImageUrl : imageUrl,
                    OriginalUrl = imageUrl
                });
            }

            var changed = false;
            if (site.ConfigImageIsFix != request.IsFix)
            {
                changed = true;
                site.ConfigImageIsFix = request.IsFix;
            }
            if (site.ConfigImageFixWidth != request.FixWidth)
            {
                changed = true;
                site.ConfigImageFixWidth = request.FixWidth;
            }
            if (site.ConfigImageFixHeight != request.FixHeight)
            {
                changed = true;
                site.ConfigImageFixHeight = request.FixHeight;
            }
            if (site.ConfigImageIsEditor != request.IsEditor)
            {
                changed = true;
                site.ConfigImageIsEditor = request.IsEditor;
            }
            if (site.ConfigImageEditorIsFix != request.EditorIsFix)
            {
                changed = true;
                site.ConfigImageEditorIsFix = request.EditorIsFix;
            }
            if (site.ConfigImageEditorFixWidth != request.EditorFixWidth)
            {
                changed = true;
                site.ConfigImageEditorFixWidth = request.EditorFixWidth;
            }
            if (site.ConfigImageEditorFixHeight != request.EditorFixHeight)
            {
                changed = true;
                site.ConfigImageEditorFixHeight = request.EditorFixHeight;
            }
            if (site.ConfigImageEditorIsLinkToOriginal != request.EditorIsLinkToOriginal)
            {
                changed = true;
                site.ConfigImageEditorIsLinkToOriginal = request.EditorIsLinkToOriginal;
            }

            if (changed)
            {
                await _siteRepository.UpdateAsync(site);
            }

            return new SubmitResult
            {
                Value = retVal,
                Editors = editors
            };
        }
    }
}
