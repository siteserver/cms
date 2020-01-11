using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/editorLayerWord")]
    public partial class PagesEditorLayerWordController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpPost, Route(RouteUpload)]
        public async Task<UploadResult> Upload([FromUri]int siteId, [FromUri]int channelId)
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

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".doc") && !StringUtils.EqualsIgnoreCase(sExt, ".docx") && !StringUtils.EqualsIgnoreCase(sExt, ".wps"))
            {
                return Request.BadRequest<UploadResult>("文件只能是 Word 格式，请选择有效的文件上传!");
            }

            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var url = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, filePath, true);

            return new UploadResult
            {
                Name = fileName,
                Url = url
            };
        }

        [HttpPost, Route(Route)]
        public async Task<GenericResult<string>> Submit([FromBody] SubmitRequest request)
        {
            var req = await AuthenticatedRequest.GetAuthAsync();

            if (!req.IsAdminLoggin ||
                !await req.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await req.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<GenericResult<string>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<GenericResult<string>>("无法确定内容对应的站点");

            var channelInfo = await ChannelManager.GetChannelAsync(request.SiteId, request.ChannelId);
            if (channelInfo == null) return Request.BadRequest<GenericResult<string>>("无法确定内容对应的栏目");

            var builder = new StringBuilder();
            foreach (var fileName in request.FileNames)
            {
                if (string.IsNullOrEmpty(fileName)) continue;

                var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                var (_, wordContent) = await WordManager.GetWordAsync(site, false, false, request.IsClearFormat, request.IsFirstLineIndent, request.IsClearFontSize, request.IsClearFontFamily, request.IsClearImages, filePath);
                wordContent = ContentUtility.TextEditorContentDecode(site, wordContent, true);
                builder.Append(wordContent);
                FileUtils.DeleteFileIfExists(filePath);
            }

            return new GenericResult<string>
            {
                Value = builder.ToString()
            };
        }
    }
}
