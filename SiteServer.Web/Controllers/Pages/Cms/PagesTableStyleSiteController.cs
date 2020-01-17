using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
{
    
    [RoutePrefix("pages/cms/tableStyleSite")]
    public partial class PagesTableStyleSiteController : ApiController
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
        public async Task<GenericResult<List<Style>>> Delete([FromBody] DeleteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<GenericResult<List<Style>>>();
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

            return new GenericResult<List<Style>>
            {
                Value = styles
            };
        }

        [HttpPost, Route(RouteImport)]
        public async Task<DefaultResult> Import([FromUri]SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<DefaultResult>();
            }

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<DefaultResult>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];
            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
            {
                return Request.BadRequest<DefaultResult>("导入文件为 Zip 格式，请选择有效的文件上传");
            }

            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var directoryPath = await ImportObject.ImportTableStyleByZipFileAsync(DataProvider.SiteRepository.TableName, DataProvider.TableStyleRepository.GetRelatedIdentities(request.SiteId), filePath);

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            await auth.AddSiteLogAsync(request.SiteId, "导入站点字段");

            return new DefaultResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteExport)]
        public async Task<GenericResult<string>> Export([FromBody] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<GenericResult<string>>();
            }

            var fileName = await ExportObject.ExportRootSingleTableStyleAsync(DataProvider.SiteRepository.TableName, DataProvider.TableStyleRepository.GetRelatedIdentities(request.SiteId));

            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            var downloadUrl = PageUtils.GetRootUrlByPhysicalPath(filePath);

            return new GenericResult<string>
            {
                Value = downloadUrl
            };
        }
    }
}
