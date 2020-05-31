using System.IO;
using System.Text;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.API.Results;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/libraryLayerWord")]
    public partial class PagesLibraryLayerWordController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpPost, Route(RouteUpload)]
        public UploadResult Upload([FromUri]int siteId)
        {
            var req = new AuthenticatedRequest();
            if (!req.IsAdminLoggin ||
                !req.AdminPermissionsImpl.HasSitePermissions(siteId,
                    ConfigManager.SitePermissions.Library))
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

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".doc") && !StringUtils.EqualsIgnoreCase(sExt, ".docx") && !StringUtils.EqualsIgnoreCase(sExt, ".wps"))
            {
                return Request.BadRequest<UploadResult>("文件只能是 Word 格式，请选择有效的文件上传!");
            }

            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var url = PageUtility.GetSiteUrlByPhysicalPath(site, filePath, true);

            return new UploadResult
            {
                Name = fileName,
                Url = url
            };
        }

        [HttpPost, Route(Route)]
        public GenericResult<string> Submit([FromBody] SubmitRequest request)
        {
            var req = new AuthenticatedRequest();

            if (!req.IsAdminLoggin ||
                !req.AdminPermissionsImpl.HasSitePermissions(request.SiteId,
                    ConfigManager.SitePermissions.Library))
            {
                return Request.Unauthorized<GenericResult<string>>();
            }

            var site = SiteManager.GetSiteInfo(request.SiteId);
            if (site == null) return Request.BadRequest<GenericResult<string>>("无法确定内容对应的站点");

            var builder = new StringBuilder();
            foreach (var fileName in request.FileNames)
            {
                if (string.IsNullOrEmpty(fileName)) continue;

                var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                var wordContent = WordUtils.Parse(request.SiteId, filePath, request.IsClearFormat, request.IsFirstLineIndent, request.IsClearFontSize, request.IsClearFontFamily, request.IsClearImages);
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
