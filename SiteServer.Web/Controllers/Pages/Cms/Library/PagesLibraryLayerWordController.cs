using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Library
{
    [RoutePrefix("pages/cms/library/libraryLayerWord")]
    public partial class PagesLibraryLayerWordController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpPost, Route(RouteUpload)]
        public async Task<UploadResult> Upload([FromUri]int siteId)
        {
            var req = await AuthenticatedRequest.GetAuthAsync();
            if (!req.IsAdminLoggin ||
                !await req.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                    Constants.SitePermissions.Library))
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
        public async Task<StringResult> Submit([FromBody] SubmitRequest request)
        {
            var req = await AuthenticatedRequest.GetAuthAsync();

            if (!req.IsAdminLoggin ||
                !await req.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Library))
            {
                return Request.Unauthorized<StringResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<StringResult>("无法确定内容对应的站点");

            var builder = new StringBuilder();
            foreach (var fileName in request.FileNames)
            {
                if (string.IsNullOrEmpty(fileName)) continue;

                var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                var (_, wordContent) = await WordManager.GetWordAsync(site, false, request.IsClearFormat, request.IsFirstLineIndent, request.IsClearFontSize, request.IsClearFontFamily, request.IsClearImages, filePath);
                wordContent = ContentUtility.TextEditorContentDecode(site, wordContent, true);
                builder.Append(wordContent);
                FileUtils.DeleteFileIfExists(filePath);
            }

            return new StringResult
            {
                Value = builder.ToString()
            };
        }
    }
}
