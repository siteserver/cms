using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Dto;
using SSCMS.Extensions;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersStyleController : ControllerBase
    {
        private const string Route = "settings/usersStyle";
        private const string RouteImport = "settings/usersStyle/actions/import";
        private const string RouteExport = "settings/usersStyle/actions/export";
        private const string RouteReset = "settings/usersStyle/actions/reset";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public UsersStyleController(IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, IUserRepository userRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _userRepository = userRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsersStyle))
            {
                return Unauthorized();
            }

            var allAttributes = _userRepository.TableColumns.Select(x => x.AttributeName).ToList();

            var styles = new List<InputStyle>();
            foreach (var style in await _tableStyleRepository.GetUserStylesAsync())
            {
                styles.Add(new InputStyle
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType,
                    Rules = TranslateUtils.JsonDeserialize<List<InputStyleRule>>(style.RuleValues),
                    Taxis = style.Taxis,
                    IsSystem = ListUtils.ContainsIgnoreCase(allAttributes, style.AttributeName)
                });
            }

            return new GetResult
            {
                Styles = styles,
                TableName = _userRepository.TableName,
                RelatedIdentities = _tableStyleRepository.EmptyRelatedIdentities
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<DeleteResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsersStyle))
            {
                return Unauthorized();
            }

            await _tableStyleRepository.DeleteAsync(0, _userRepository.TableName, request.AttributeName);

            var allAttributes = _userRepository.TableColumns.Select(x => x.AttributeName).ToList();

            var styles = new List<InputStyle>();
            foreach (var style in await _tableStyleRepository.GetUserStylesAsync())
            {
                styles.Add(new InputStyle
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType,
                    Rules = TranslateUtils.JsonDeserialize<List<InputStyleRule>>(style.RuleValues),
                    Taxis = style.Taxis,
                    IsSystem = ListUtils.ContainsIgnoreCase(allAttributes, style.AttributeName)
                });
            }

            return new DeleteResult
            {
                Styles = styles
            };
        }

        [HttpPost, Route(RouteImport)]
        public async Task<ActionResult<BoolResult>> Import([FromForm] IFormFile file)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
            {
                return this.Error("导入文件为 Zip 格式，请选择有效的文件上传");
            }

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            await _pathManager.UploadAsync(file, filePath);

            var directoryPath = await ImportObject.ImportTableStyleByZipFileAsync(_pathManager, _databaseManager, _userRepository.TableName, _tableStyleRepository.EmptyRelatedIdentities, filePath);

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            await _authManager.AddAdminLogAsync("导入用户字段");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpGet, Route(RouteExport)]
        public async Task<ActionResult> Export()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var fileName = await ExportObject.ExportRootSingleTableStyleAsync(_pathManager, _databaseManager, 0, _userRepository.TableName, _tableStyleRepository.EmptyRelatedIdentities);

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            return this.Download(filePath);
        }

        [HttpPost, Route(RouteReset)]
        public async Task<ActionResult<ResetResult>> Reset()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsersStyle))
            {
                return Unauthorized();
            }

            await _tableStyleRepository.DeleteAllAsync(_userRepository.TableName);

            var allAttributes = _userRepository.TableColumns.Select(x => x.AttributeName).ToList();

            var styles = new List<InputStyle>();
            foreach (var style in await _tableStyleRepository.GetUserStylesAsync())
            {
                styles.Add(new InputStyle
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType,
                    Rules = TranslateUtils.JsonDeserialize<List<InputStyleRule>>(style.RuleValues),
                    Taxis = style.Taxis,
                    IsSystem = ListUtils.ContainsIgnoreCase(allAttributes, style.AttributeName)
                });
            }

            return new ResetResult
            {
                Styles = styles
            };
        }
    }
}
