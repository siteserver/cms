using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;
using SiteServer.Utils.Images;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/editorLayerImage")]
    public partial class PagesEditorLayerImageController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpPost, Route(RouteUpload)]
        public UploadResult Upload([FromUri]int siteId, [FromUri]int channelId)
        {
            var req = new AuthenticatedRequest();
            if (!req.IsAdminLoggin ||
                !req.AdminPermissionsImpl.HasSitePermissions(siteId,
                    ConfigManager.SitePermissions.Contents) ||
                !req.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                    ConfigManager.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<UploadResult>();
            }

            var site = SiteManager.GetSiteInfo(siteId);

            var fileName = req.HttpRequest["fileName"];
            var fileCount = req.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<UploadResult>("请选择有效的文件上传");
            }

            var file = req.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".jpg", ".jpeg", ".bmp", ".gif", ".png", ".webp"))
            {
                return Request.BadRequest<UploadResult>("文件只能是 Image 格式，请选择有效的文件上传!");
            }

            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(site, EUploadType.Image);
            var filePath = PathUtils.Combine(localDirectoryPath, PathUtility.GetUploadFileName(site, fileName));

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var imageUrl = PageUtility.GetSiteUrlByPhysicalPath(site, filePath, true);

            return new UploadResult
            {
                Name = fileName,
                Path = filePath,
                Url = imageUrl
            };
        }

        [HttpPost, Route(Route)]
        public List<SubmitResult> Submit([FromBody] SubmitRequest request)
        {
            var req = new AuthenticatedRequest();

            if (!req.IsAdminLoggin ||
                !req.AdminPermissionsImpl.HasSitePermissions(request.SiteId,
                    ConfigManager.SitePermissions.Contents) ||
                !req.AdminPermissionsImpl.HasChannelPermissions(request.SiteId, request.ChannelId,
                    ConfigManager.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<List<SubmitResult>>();
            }

            var site = SiteManager.GetSiteInfo(request.SiteId);
            if (site == null) return Request.BadRequest<List<SubmitResult>>("无法确定内容对应的站点");

            var channelInfo = ChannelManager.GetChannelInfo(request.SiteId, request.ChannelId);
            if (channelInfo == null) return Request.BadRequest<List<SubmitResult>>("无法确定内容对应的栏目");

            var result = new List<SubmitResult>();
            foreach (var filePath in request.FilePaths)
            {
                if (string.IsNullOrEmpty(filePath)) continue;

                var fileName = PathUtils.GetFileName(filePath);

                var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(site, fileExtName);

                var imageUrl = PageUtility.GetSiteUrlByPhysicalPath(site, filePath, true);

                if (request.IsThumb)
                {
                    var localSmallFileName = Constants.SmallImageAppendix + fileName;
                    var localSmallFilePath = PathUtils.Combine(localDirectoryPath, localSmallFileName);

                    var thumbnailUrl = PageUtility.GetSiteUrlByPhysicalPath(site, localSmallFilePath, true);

                    var width = request.ThumbWidth;
                    var height = request.ThumbHeight;
                    ImageUtils.MakeThumbnail(filePath, localSmallFilePath, width, height, true);

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
