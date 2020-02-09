using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Images;
using SiteServer.CMS.Core;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Shared
{
    [RoutePrefix("pages/shared/imageLayerUpload")]
    public partial class PagesImageLayerUploadController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpPost, Route(RouteUpload)]
        public async Task<UploadResult> Upload([FromUri]int siteId)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<UploadResult>();

            var site = await DataProvider.SiteRepository.GetAsync(siteId);

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

            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(site, UploadType.Image);
            var filePath = PathUtils.Combine(localDirectoryPath, PathUtility.GetUploadFileName(site, fileName));

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var imageUrl = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, filePath, true);

            return new UploadResult
            {
                Name = fileName,
                Path = filePath,
                Url = imageUrl
            };
        }

        [HttpPost, Route(Route)]
        public async Task<List<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<List<SubmitResult>>();

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<List<SubmitResult>>("无法确定内容对应的站点");

            var result = new List<SubmitResult>();
            foreach (var filePath in request.FilePaths)
            {
                if (string.IsNullOrEmpty(filePath)) continue;

                var fileName = PathUtils.GetFileName(filePath);

                var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(site, fileExtName);

                var imageUrl = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, filePath, true);

                if (request.IsThumb)
                {
                    var localSmallFileName = Constants.SmallImageAppendix + fileName;
                    var localSmallFilePath = PathUtils.Combine(localDirectoryPath, localSmallFileName);

                    var thumbnailUrl = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, localSmallFilePath, true);

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
