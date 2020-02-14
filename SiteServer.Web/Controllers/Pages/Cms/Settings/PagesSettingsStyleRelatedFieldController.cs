using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    
    [RoutePrefix("pages/cms/settings/settingsStyleRelatedField")]
    public partial class PagesSettingsStyleRelatedFieldController : ApiController
    {
        private const string Route = "";
        private const string RouteImport = "actions/import";
        private const string RouteExport = "actions/export";

        [HttpGet, Route(Route)]
        public async Task<IEnumerable<RelatedField>> Get([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<IEnumerable<RelatedField>>();
            }

            return await DataProvider.RelatedFieldRepository.GetRelatedFieldListAsync(request.SiteId);
        }

        [HttpDelete, Route(Route)]
        public async Task<IEnumerable<RelatedField>> Delete([FromBody] DeleteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<IEnumerable<RelatedField>>();
            }

            await DataProvider.RelatedFieldRepository.DeleteAsync(request.RelatedFieldId);

            await auth.AddSiteLogAsync(request.SiteId, "删除联动字段");

            return await DataProvider.RelatedFieldRepository.GetRelatedFieldListAsync(request.SiteId);
        }

        [HttpPost, Route(Route)]
        public async Task<IEnumerable<RelatedField>> Add([FromBody]RelatedField request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<IEnumerable<RelatedField>>();
            }

            await DataProvider.RelatedFieldRepository.InsertAsync(request);

            await auth.AddSiteLogAsync(request.SiteId, "新增联动字段");

            return await DataProvider.RelatedFieldRepository.GetRelatedFieldListAsync(request.SiteId);
        }

        [HttpPut, Route(Route)]
        public async Task<IEnumerable<RelatedField>> Edit([FromBody]RelatedField request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigTableStyles))
            {
                return Request.Unauthorized<IEnumerable<RelatedField>>();
            }

            await DataProvider.RelatedFieldRepository.UpdateAsync(request);

            await auth.AddSiteLogAsync(request.SiteId, "编辑联动字段");

            return await DataProvider.RelatedFieldRepository.GetRelatedFieldListAsync(request.SiteId);
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

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

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

            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var directoryPath = await ImportObject.ImportRelatedFieldByZipFileAsync(site, filePath);

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            await auth.AddSiteLogAsync(request.SiteId, "导入联动字段");

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

            var fileName = await ExportObject.ExportRelatedFieldListAsync(request.SiteId);

            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            var downloadUrl = PageUtils.GetRootUrlByPhysicalPath(filePath);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
