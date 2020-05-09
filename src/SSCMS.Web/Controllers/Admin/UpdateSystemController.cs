using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Extensions;
using SSCMS.Core.Packaging;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UpdateSystemController : ControllerBase
    {
        private const string Route = "updateSystem";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;

        public UpdateSystemController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IConfigRepository configRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            if (await _configRepository.IsNeedInstallAsync())
            {
                return this.Error("系统未安装，向导被禁用");
            }

            return new GetResult
            {
                IsNightly = _settingsManager.IsNightlyUpdate,
                Version = _settingsManager.Version
            };
        }

        [HttpPost, Route(Route)]
        public ActionResult<BoolResult> UpdateSsCms([FromBody] UpdateRequest request)
        {
            if (!CloudUtils.IsCmsDownload(_pathManager, request.Version))
            {
                return this.Error($"升级包 {request.Version} 不存在");
            }

            var name = CloudUtils.GetCmsDownloadName(request.Version);
            var packagePath = _pathManager.GetPackagesPath(name);
            var packageWebConfigPath = PathUtils.Combine(packagePath, Constants.ConfigFileName);

            //WebConfigUtils.UpdateWebConfig(packageWebConfigPath, WebConfigUtils.IsProtectData,
            //    WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, WebConfigUtils.RedisConnectionString, WebConfigUtils.AdminDirectory, WebConfigUtils.HomeDirectory,
            //    WebConfigUtils.SecretKey, WebConfigUtils.IsNightlyUpdate);

            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.SiteFilesDirectoryName), _pathManager.GetSiteFilesPath(string.Empty), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.HomeDirectoryName), PathUtils.Combine(_settingsManager.WebRootPath, Constants.HomeDirectory), true);
            DirectoryUtils.Copy(PathUtils.Combine(packagePath, DirectoryUtils.BinDirectoryName), _pathManager.GetBinDirectoryPath(string.Empty), true);
            FileUtils.CopyFile(packageWebConfigPath, PathUtils.Combine(_settingsManager.ContentRootPath, Constants.ConfigFileName), true);

            //SystemManager.SyncDatabase();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}