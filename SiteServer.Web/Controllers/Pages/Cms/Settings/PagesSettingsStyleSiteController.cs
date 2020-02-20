using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Datory;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.Serialization;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    [RoutePrefix("pages/cms/settings/settingsStyleSite")]
    public partial class PagesSettingsStyleSiteController : ApiController
    {
        private const string Route = "";
        private const string RouteImport = "actions/import";
        private const string RouteExport = "actions/export";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<GetResult>();
            }

            var styles = new List<Style>();
            foreach (var style in await DataProvider.TableStyleRepository.GetSiteStyleListAsync(request.SiteId))
            {
                styles.Add(new Style
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType.GetDisplayName(),
                    Rules = TranslateUtils.JsonDeserialize<IEnumerable<TableStyleRule>>(style.RuleValues),
                    Taxis = style.Taxis,
                    IsSystem = false
                });
            }

            return new GetResult
            {
                Styles = styles,
                TableName = DataProvider.SiteRepository.TableName,
                RelatedIdentities = DataProvider.TableStyleRepository.GetRelatedIdentities(request.SiteId)
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ObjectResult<List<Style>>> Delete([FromBody] DeleteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<ObjectResult<List<Style>>>();
            }

            await DataProvider.TableStyleRepository.DeleteAsync(request.SiteId, DataProvider.SiteRepository.TableName, request.AttributeName);

            var styles = new List<Style>();
            foreach (var style in await DataProvider.TableStyleRepository.GetSiteStyleListAsync(request.SiteId))
            {
                styles.Add(new Style
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType.GetDisplayName(),
                    Rules = TranslateUtils.JsonDeserialize<IEnumerable<TableStyleRule>>(style.RuleValues),
                    Taxis = style.Taxis,
                    IsSystem = false
                });
            }

            return new ObjectResult<List<Style>>
            {
                Value = styles
            };
        }

        [HttpPost, Route(RouteImport)]
        public async Task<BoolResult> Import([FromUri]SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<BoolResult>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
            {
                return Request.BadRequest<BoolResult>("导入文件为 Zip 格式，请选择有效的文件上传");
            }

            var filePath = PathUtility.GetTemporaryFilesPath(fileName);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var directoryPath = await ImportObject.ImportTableStyleByZipFileAsync(DataProvider.SiteRepository.TableName, DataProvider.TableStyleRepository.GetRelatedIdentities(request.SiteId), filePath);

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            await auth.AddSiteLogAsync(request.SiteId, "导入站点字段");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteExport)]
        public async Task<StringResult> Export([FromBody] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<StringResult>();
            }

            var fileName = await ExportObject.ExportRootSingleTableStyleAsync(request.SiteId, DataProvider.SiteRepository.TableName, DataProvider.TableStyleRepository.GetRelatedIdentities(request.SiteId));

            var filePath = PathUtility.GetTemporaryFilesPath(fileName);
            var downloadUrl = PageUtils.GetRootUrlByPhysicalPath(filePath);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
