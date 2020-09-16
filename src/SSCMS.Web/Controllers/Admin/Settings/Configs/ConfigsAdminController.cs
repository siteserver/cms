using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ConfigsAdminController : ControllerBase
    {
        private const string Route = "settings/configsAdmin";
        private const string RouteUpload = "settings/configsAdmin/actions/upload";

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
            if (!await _authManager.IsSuperAdminAsync())
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
            if (!await _authManager.IsSuperAdminAsync())
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
            if (!await _authManager.IsSuperAdminAsync())
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
