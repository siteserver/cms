using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Extensions;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesTemplatesController : ControllerBase
    {
        private const string Route = "settings/sitesTemplates";
        private const string RouteZip = "settings/sitesTemplates/actions/zip";
        private const string RouteUnZip = "settings/sitesTemplates/actions/unZip";
        private const string RouteUpload = "settings/sitesTemplates/actions/upload";

        private readonly ICacheManager<CacheUtils.Process> _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IOldPluginManager _pluginManager;

        public SitesTemplatesController(ICacheManager<CacheUtils.Process> cacheManager, IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, IOldPluginManager pluginManager)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
        }

        private async Task<ListResult> GetListResultAsync()
        {
            var caching = new CacheUtils(_cacheManager);
            var manager = new SiteTemplateManager(_pathManager, _pluginManager, _databaseManager, caching);
            var siteTemplates = manager.GetSiteTemplateInfoList();
            var siteTemplateInfoList = new List<SiteTemplateInfo>();
            foreach (var siteTemplate in siteTemplates)
            {
                var directoryPath = _pathManager.GetSiteTemplatesPath(siteTemplate.DirectoryName);
                var dirInfo = new DirectoryInfo(directoryPath);
                if (string.IsNullOrEmpty(siteTemplate.SiteTemplateName)) continue;

                var filePath = _pathManager.GetSiteTemplatesPath(dirInfo.Name + ".zip");
                siteTemplate.FileExists = FileUtils.IsFileExists(filePath);
                siteTemplateInfoList.Add(siteTemplate);
            }

            var fileNames = manager.GetZipSiteTemplateList();
            var fileNameList = new List<string>();
            foreach (var fileName in fileNames)
            {
                if (DirectoryUtils.IsDirectoryExists(
                    _pathManager.GetSiteTemplatesPath(PathUtils.GetFileNameWithoutExtension(fileName)))) continue;
                var filePath = _pathManager.GetSiteTemplatesPath(fileName);
                var fileInfo = new FileInfo(filePath);
                fileNameList.Add(fileInfo.Name);
            }

            var siteTemplateUrl = StringUtils.TrimSlash(_pathManager.GetSiteTemplatesUrl(string.Empty));
            var siteAddPermission =
                await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesAdd);

            return new ListResult
            {
                SiteTemplateInfoList = siteTemplateInfoList,
                FileNameList = fileNameList,
                SiteTemplateUrl = siteTemplateUrl,
                SiteAddPermission = siteAddPermission
            };
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> GetList()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            return await GetListResultAsync();
        }

        [HttpPost, Route(RouteZip)]
        public async Task<ActionResult<StringResult>> Zip([FromBody]ZipRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var directoryName = PathUtils.RemoveParentPath(request.DirectoryName);
            var fileName = directoryName + ".zip";
            var filePath = _pathManager.GetSiteTemplatesPath(fileName);
            var directoryPath = _pathManager.GetSiteTemplatesPath(directoryName);

            FileUtils.DeleteFileIfExists(filePath);

            ZipUtils.CreateZip(filePath, directoryPath);

            return new StringResult
            {
                Value = _pathManager.GetSiteTemplatesUrl(fileName)
            };
        }

        [HttpPost, Route(RouteUnZip)]
        public async Task<ActionResult<ListResult>> UnZip([FromBody]UnZipRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var fileNameToUnZip = request.FileName;

            var directoryPathToUnZip = _pathManager.GetSiteTemplatesPath(PathUtils.GetFileNameWithoutExtension(fileNameToUnZip));
            var zipFilePath = _pathManager.GetSiteTemplatesPath(fileNameToUnZip);

            ZipUtils.ExtractZip(zipFilePath, directoryPathToUnZip);

            return await GetListResultAsync();
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody]DeleteRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            var caching = new CacheUtils(_cacheManager);
            var manager = new SiteTemplateManager(_pathManager, _pluginManager, _databaseManager, caching);

            if (!string.IsNullOrEmpty(request.DirectoryName))
            {
                manager.DeleteSiteTemplate(request.DirectoryName);
                await _authManager.AddAdminLogAsync("删除站点模板", $"站点模板:{request.DirectoryName}");
            }
            if (!string.IsNullOrEmpty(request.FileName))
            {
                manager.DeleteZipSiteTemplate(request.FileName);
                await _authManager.AddAdminLogAsync("删除未解压站点模板", $"站点模板:{request.FileName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<ListResult>> Upload([FromForm]IFormFile file)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            if (file == null) return this.Error("请选择有效的文件上传");
            var extension = PathUtils.GetExtension(file.FileName);
            if (!FileUtils.IsZip(extension))
            {
                return this.Error("站点模板压缩包为zip格式，请选择有效的文件上传!");
            }
            var directoryName = PathUtils.GetFileNameWithoutExtension(file.FileName);
            var directoryPath = _pathManager.GetSiteFilesPath(PathUtils.Combine(DirectoryUtils.SiteTemplates.DirectoryName, directoryName));
            if (DirectoryUtils.IsDirectoryExists(directoryPath))
            {
                return this.Error($"站点模板导入失败，文件夹{directoryName}已存在");
            }
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);
            var filePath = _pathManager.GetSiteFilesPath(PathUtils.Combine(DirectoryUtils.SiteTemplates.DirectoryName, file.FileName));
            
            FileUtils.DeleteFileIfExists(filePath);

            await _pathManager.UploadAsync(file, filePath);

            ZipUtils.ExtractZip(filePath, directoryPath);

            return await GetListResultAsync();
        }
    }
}
