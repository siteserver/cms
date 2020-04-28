using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Extensions;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ConfigsHomeController : ControllerBase
    {
        private const string Route = "settings/configsHome";
        private const string RouteUpload = "settings/configsHome/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public ConfigsHomeController(IAuthManager authManager, IPathManager pathManager, IConfigRepository configRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsConfigsHome))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            return new GetResult
            {
                Config = config,
                Styles = await _tableStyleRepository.GetUserStyleListAsync()
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsConfigsHome))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            config.IsHomeClosed = request.IsHomeClosed;
            config.HomeTitle = request.HomeTitle;
            config.IsHomeLogo = request.IsHomeLogo;
            config.HomeLogoUrl = request.HomeLogoUrl;
            config.HomeDefaultAvatarUrl = request.HomeDefaultAvatarUrl;
            config.UserRegistrationAttributes = request.UserRegistrationAttributes;
            config.IsUserRegistrationGroup = request.IsUserRegistrationGroup;
            config.IsHomeAgreement = request.IsHomeAgreement;
            config.HomeAgreementHtml = request.HomeAgreementHtml;
            config.HomeWelcomeHtml = request.HomeWelcomeHtml;

            await _configRepository.UpdateAsync(config);

            await _authManager.AddAdminLogAsync("修改用户中心设置");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromForm]IFormFile file)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsConfigsHome))
            {
                return Unauthorized();
            }

            if (file == null) return this.Error("请选择有效的文件上传");
            var fileName = _pathManager.GetUploadFileName(file.FileName);
            if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
            {
                return this.Error("文件只能是图片格式，请选择有效的文件上传!");
            }
            var filePath = _pathManager.GetHomeUploadPath(fileName);
            await _pathManager.UploadAsync(file, filePath);

            var url = _pathManager.GetHomeUploadUrl(fileName);

            return new StringResult
            {
                Value = url
            };
        }
    }
}
