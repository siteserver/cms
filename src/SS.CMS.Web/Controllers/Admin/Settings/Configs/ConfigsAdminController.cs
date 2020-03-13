using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Configs
{
    [Route("admin/settings/configsAdmin")]
    public partial class ConfigsAdminController : ControllerBase
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;

        public ConfigsAdminController(IAuthManager authManager, IPathManager pathManager, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigsAdmin))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            return new GetResult
            {
                AdminTitle = config.AdminTitle,
                AdminLogoUrl = config.AdminLogoUrl,
                AdminWelcomeHtml = config.AdminWelcomeHtml
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigsAdmin))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            config.AdminTitle = request.AdminTitle;
            config.AdminLogoUrl = request.AdminLogoUrl;
            config.AdminWelcomeHtml = request.AdminWelcomeHtml;

            await _configRepository.UpdateAsync(config);

            await _authManager.AddAdminLogAsync("修改管理后台设置");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromForm]IFormFile file)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigsAdmin))
            {
                return Unauthorized();
            }

            if (file == null) return this.Error("请选择有效的文件上传");
            var extension = PathUtils.GetExtension(file.FileName);
            if (!FileUtils.IsImage(extension))
            {
                return this.Error("文件只能是图片格式，请选择有效的文件上传!");
            }
            var fileName = $"logo{extension}";
            var filePath = _pathManager.GetSiteFilesPath(fileName);
            await _pathManager.UploadAsync(file, filePath);

            var adminLogoUrl = _pathManager.GetSiteFilesUrl(fileName);

            return new StringResult
            {
                Value = adminLogoUrl
            };
        }
    }
}
