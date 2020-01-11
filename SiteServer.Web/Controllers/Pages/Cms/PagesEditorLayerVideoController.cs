using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/editorLayerVideo")]
    public partial class PagesEditorLayerVideoController : ApiController
    {
        private const string RouteUploadVideo = "actions/uploadVideo";
        private const string RouteUploadImage = "actions/uploadImage";

        [HttpPost, Route(RouteUploadVideo)]
        public async Task<UploadResult> UploadVideo([FromUri]int siteId, [FromUri]int channelId)
        {
            var req = await AuthenticatedRequest.GetAuthAsync();
            if (!req.IsAdminLoggin ||
                !await req.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                    Constants.SitePermissions.Contents) ||
                !await req.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<UploadResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(siteId);

            var fileName = req.HttpRequest["fileName"];
            var fileCount = req.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<UploadResult>("请选择有效的文件上传");
            }

            var file = req.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".mp4", ".flv", ".f4v", ".webm", ".m4v", ".mov", ".3gp", ".3g2"))
            {
                return Request.BadRequest<UploadResult>("文件只能是主流视频格式，请选择有效的文件上传!");
            }

            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(site, UploadType.Video);
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

        [HttpPost, Route(RouteUploadImage)]
        public async Task<UploadResult> UploadImage([FromUri]int siteId, [FromUri]int channelId)
        {
            var req = await AuthenticatedRequest.GetAuthAsync();
            if (!req.IsAdminLoggin ||
                !await req.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                    Constants.SitePermissions.Contents) ||
                !await req.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<UploadResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(siteId);

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
    }
}
