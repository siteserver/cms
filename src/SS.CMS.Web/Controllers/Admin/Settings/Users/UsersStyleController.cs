using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Core.Serialization;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Users
{
    [Route("admin/settings/usersStyle")]
    public partial class UsersStyleController : ControllerBase
    {
        private const string Route = "";
        private const string RouteImport = "actions/import";
        private const string RouteExport = "actions/export";
        private const string RouteReset = "actions/reset";

        private readonly IAuthManager _authManager;

        public UsersStyleController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsersStyle))
            {
                return Unauthorized();
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
        public async Task<ActionResult<DeleteResult>> Delete([FromBody] DeleteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsersStyle))
            {
                return Unauthorized();
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

            return new DeleteResult
            {
                Styles = styles
            };
        }

        [HttpPost, Route(RouteImport)]
        public async Task<ActionResult<BoolResult>> Import([FromBody] UploadRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            if (request.File == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(request.File.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
            {
                return this.Error("导入文件为 Zip 格式，请选择有效的文件上传");
            }

            var filePath = PathUtility.GetTemporaryFilesPath(fileName);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            request.File.CopyTo(new FileStream(filePath, FileMode.Create));

            var directoryPath = await ImportObject.ImportTableStyleByZipFileAsync(DataProvider.UserRepository.TableName, DataProvider.TableStyleRepository.EmptyRelatedIdentities, filePath);

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            await auth.AddAdminLogAsync("导入用户字段");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpGet, Route(RouteExport)]
        public async Task<ActionResult> Export()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var fileName = await ExportObject.ExportRootSingleTableStyleAsync(0, DataProvider.UserRepository.TableName, DataProvider.TableStyleRepository.EmptyRelatedIdentities);

            var filePath = PathUtility.GetTemporaryFilesPath(fileName);
            return this.Download(filePath);
        }

        [HttpPost, Route(RouteReset)]
        public async Task<ActionResult<ResetResult>> Reset()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsersStyle))
            {
                return Unauthorized();
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

            return new ResetResult
            {
                Styles = styles
            };
        }
    }
}
