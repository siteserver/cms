using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Dto;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsStyleRelatedFieldController : ControllerBase
    {
        private const string Route = "cms/settings/settingsStyleRelatedField";
        private const string RouteImport = "cms/settings/settingsStyleRelatedField/actions/import";
        private const string RouteExport = "cms/settings/settingsStyleRelatedField/actions/export";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IRelatedFieldRepository _relatedFieldRepository;
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;

        public SettingsStyleRelatedFieldController(IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IRelatedFieldRepository relatedFieldRepository, IRelatedFieldItemRepository relatedFieldItemRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _relatedFieldRepository = relatedFieldRepository;
            _relatedFieldItemRepository = relatedFieldItemRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<IEnumerable<RelatedField>>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            return await _relatedFieldRepository.GetRelatedFieldsAsync(request.SiteId);
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<IEnumerable<RelatedField>>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            await _relatedFieldRepository.DeleteAsync(request.RelatedFieldId);

            await _authManager.AddSiteLogAsync(request.SiteId, "删除联动字段");

            return await _relatedFieldRepository.GetRelatedFieldsAsync(request.SiteId);
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<IEnumerable<RelatedField>>> Add([FromBody]RelatedField request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            await _relatedFieldRepository.InsertAsync(request);

            await _authManager.AddSiteLogAsync(request.SiteId, "新增联动字段");

            return await _relatedFieldRepository.GetRelatedFieldsAsync(request.SiteId);
        }

        [HttpPut, Route(Route)]
        public async Task<ActionResult<IEnumerable<RelatedField>>> Edit([FromBody]RelatedField request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            await _relatedFieldRepository.UpdateAsync(request);

            await _authManager.AddSiteLogAsync(request.SiteId, "编辑联动字段");

            return await _relatedFieldRepository.GetRelatedFieldsAsync(request.SiteId);
        }

        [HttpPost, Route(RouteImport)]
        public async Task<ActionResult<BoolResult>> Import([FromQuery] SiteRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

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

            var directoryPath = await ImportObject.ImportRelatedFieldByZipFileAsync(_pathManager, _databaseManager, site, filePath);

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            await _authManager.AddSiteLogAsync(request.SiteId, "导入联动字段");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            var fileName = await ExportObject.ExportRelatedFieldListAsync(_pathManager, _databaseManager, request.SiteId);

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            var downloadUrl = _pathManager.GetRootUrlByPath(filePath);

            return new StringResult
            {
                Value = downloadUrl
            };
        }
    }
}
