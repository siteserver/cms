using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Datory;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.Serialization;

namespace SiteServer.API.Controllers.Pages.Settings.User
{
    
    [RoutePrefix("pages/settings/userStyle")]
    public partial class PagesUserStyleController : ApiController
    {
        private const string Route = "";
        private const string RouteImport = "actions/import";
        private const string RouteExport = "actions/export";
        private const string RouteReset = "actions/reset";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUserStyle))
            {
                return Request.Unauthorized<GetResult>();
            }

            var allAttributes = DataProvider.UserRepository.TableColumns.Select(x => x.AttributeName).ToList();

            var styles = new List<Style>();
            foreach (var style in await DataProvider.TableStyleRepository.GetUserStyleListAsync())
            {
                styles.Add(new Style
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType.GetDisplayName(),
                    Rules = TranslateUtils.JsonDeserialize<IEnumerable<TableStyleRule>>(style.RuleValues),
                    Taxis = style.Taxis,
                    IsSystem = StringUtils.ContainsIgnoreCase(allAttributes, style.AttributeName)
                });
            }

            return new GetResult
            {
                Styles = styles,
                TableName = DataProvider.UserRepository.TableName,
                RelatedIdentities = DataProvider.TableStyleRepository.EmptyRelatedIdentities
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ObjectResult<List<Style>>> Delete([FromBody] DeleteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUserStyle))
            {
                return Request.Unauthorized<ObjectResult<List<Style>>>();
            }

            await DataProvider.TableStyleRepository.DeleteAsync(0, DataProvider.UserRepository.TableName, request.AttributeName);

            var allAttributes = DataProvider.UserRepository.TableColumns.Select(x => x.AttributeName).ToList();

            var styles = new List<Style>();
            foreach (var style in await DataProvider.TableStyleRepository.GetUserStyleListAsync())
            {
                styles.Add(new Style
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType.GetDisplayName(),
                    Rules = TranslateUtils.JsonDeserialize<IEnumerable<TableStyleRule>>(style.RuleValues),
                    Taxis = style.Taxis,
                    IsSystem = StringUtils.ContainsIgnoreCase(allAttributes, style.AttributeName)
                });
            }

            return new ObjectResult<List<Style>>
            {
                Value = styles
            };
        }

        [HttpPost, Route(RouteImport)]
        public async Task<BoolResult> Import()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser))
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

            var directoryPath = await ImportObject.ImportTableStyleByZipFileAsync(DataProvider.UserRepository.TableName, DataProvider.TableStyleRepository.EmptyRelatedIdentities, filePath);

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            await auth.AddAdminLogAsync("导入用户字段");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteExport)]
        public async Task<StringResult> Export()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser))
            {
                return Request.Unauthorized<StringResult>();
            }

            var fileName = await ExportObject.ExportRootSingleTableStyleAsync(0, DataProvider.UserRepository.TableName, DataProvider.TableStyleRepository.EmptyRelatedIdentities);

            var filePath = PathUtility.GetTemporaryFilesPath(fileName);
            var downloadUrl = PageUtils.GetRootUrlByPhysicalPath(filePath);

            return new StringResult
            {
                Value = downloadUrl
            };
        }

        [HttpPost, Route(RouteReset)]
        public async Task<ObjectResult<List<Style>>> Reset()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUserStyle))
            {
                return Request.Unauthorized<ObjectResult<List<Style>>>();
            }

            await DataProvider.TableStyleRepository.DeleteAllAsync(DataProvider.UserRepository.TableName);

            var allAttributes = DataProvider.UserRepository.TableColumns.Select(x => x.AttributeName).ToList();

            var styles = new List<Style>();
            foreach (var style in await DataProvider.TableStyleRepository.GetUserStyleListAsync())
            {
                styles.Add(new Style
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType.GetDisplayName(),
                    Rules = TranslateUtils.JsonDeserialize<IEnumerable<TableStyleRule>>(style.RuleValues),
                    Taxis = style.Taxis,
                    IsSystem = StringUtils.ContainsIgnoreCase(allAttributes, style.AttributeName)
                });
            }

            return new ObjectResult<List<Style>>
            {
                Value = styles
            };
        }
    }
}
