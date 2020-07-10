using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Datory;
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

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsStyleSiteController : ControllerBase
    {
        private const string Route = "cms/settings/settingsStyleSite";
        private const string RouteImport = "cms/settings/settingsStyleSite/actions/import";
        private const string RouteExport = "cms/settings/settingsStyleSite/actions/export";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public SettingsStyleSiteController(IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsStyleSite))
            {
                return Unauthorized();
            }

            var styles = new List<Style>();
            foreach (var style in await _tableStyleRepository.GetSiteStylesAsync(request.SiteId))
            {
                styles.Add(new Style
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType.GetDisplayName(),
                    Rules = TranslateUtils.JsonDeserialize<IEnumerable<InputStyleRule>>(style.RuleValues),
                    Taxis = style.Taxis,
                    IsSystem = false
                });
            }

            return new GetResult
            {
                Styles = styles,
                TableName = _siteRepository.TableName,
                RelatedIdentities = _tableStyleRepository.GetRelatedIdentities(request.SiteId)
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<ObjectResult<List<Style>>>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsStyleSite))
            {
                return Unauthorized();
            }

            await _tableStyleRepository.DeleteAsync(request.SiteId, _siteRepository.TableName, request.AttributeName);

            var styles = new List<Style>();
            foreach (var style in await _tableStyleRepository.GetSiteStylesAsync(request.SiteId))
            {
                styles.Add(new Style
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType.GetDisplayName(),
                    Rules = TranslateUtils.JsonDeserialize<IEnumerable<InputStyleRule>>(style.RuleValues),
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
        public async Task<ActionResult<BoolResult>> Import([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsStyleSite))
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

            var directoryPath = await ImportObject.ImportTableStyleByZipFileAsync(_pathManager, _databaseManager, _siteRepository.TableName, _tableStyleRepository.GetRelatedIdentities(request.SiteId), filePath);

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            await _authManager.AddSiteLogAsync(request.SiteId, "导入站点字段");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsStyleSite))
            {
                return Unauthorized();
            }

            var fileName = await ExportObject.ExportRootSingleTableStyleAsync(_pathManager, _databaseManager, request.SiteId, _siteRepository.TableName, _tableStyleRepository.GetRelatedIdentities(request.SiteId));

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            var downloadUrl = _pathManager.GetRootUrlByPath(filePath);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
