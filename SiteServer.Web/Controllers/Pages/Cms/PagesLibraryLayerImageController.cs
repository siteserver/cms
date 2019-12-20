using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;
using SiteServer.Utils.Images;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/libraryLayerImage")]
    public partial class PagesLibraryLayerImageController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpPost, Route(RouteUpload)]
        public UploadResult Upload([FromUri]int siteId)
        {
            var auth = new AuthenticatedRequest();

            if (!auth.IsAdminLoggin ||
                !auth.AdminPermissionsImpl.HasSitePermissions(siteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<UploadResult>();
            }

            var site = SiteManager.GetSiteInfo(siteId);

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<UploadResult>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".jpg", ".jpeg", ".bmp", ".gif", ".png", ".webp"))
            {
                return Request.BadRequest<UploadResult>("文件只能是 Image 格式，请选择有效的文件上传!");
            }

            var virtualUrl = PathUtils.GetLibraryVirtualPath(EUploadType.Image, PathUtility.GetUploadFileName(site, fileName));
            var filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, virtualUrl);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            return new UploadResult
            {
                Name = fileName,
                Path = filePath,
                Url = virtualUrl
            };
        }

        [HttpPost, Route(Route)]
        public List<SubmitResult> Submit([FromBody] SubmitRequest request)
        {
            var req = new AuthenticatedRequest();

            if (!req.IsAdminLoggin ||
                !req.AdminPermissionsImpl.HasSitePermissions(request.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<List<SubmitResult>>();
            }

            var site = SiteManager.GetSiteInfo(request.SiteId);
            if (site == null) return Request.BadRequest<List<SubmitResult>>("无法确定内容对应的站点");

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
