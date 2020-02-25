using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Framework;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Configs
{
    [Route("admin/settings/configsHome")]
    public partial class ConfigsHomeController : ControllerBase
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;

        public ConfigsHomeController(IAuthManager authManager, IPathManager pathManager)
        {
            _authManager = authManager;
            _pathManager = pathManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigsHome))
            {
                return Unauthorized();
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            return new GetResult
            {
                Config = config,
                HomeDirectory = WebConfigUtils.HomeDirectory,
                AdminToken = auth.AdminToken,
                Styles = await DataProvider.TableStyleRepository.GetUserStyleListAsync()
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigsHome))
            {
                return Unauthorized();
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            config.IsHomeClosed = request.IsHomeClosed;
            config.HomeTitle = request.HomeTitle;
            config.IsHomeLogo = request.IsHomeLogo;
            config.HomeLogoUrl = request.HomeLogoUrl;
            config.HomeDefaultAvatarUrl = request.HomeDefaultAvatarUrl;
            config.UserRegistrationAttributes = request.UserRegistrationAttributes;
            config.IsUserRegistrationGroup = request.IsUserRegistrationGroup;
            config.IsHomeAgreement = request.IsHomeAgreement;
            config.HomeAgreementHtml = request.HomeAgreementHtml;

            await DataProvider.ConfigRepository.UpdateAsync(config);

            await auth.AddAdminLogAsync("修改用户中心设置");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromForm]IFormFile file)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigsHome))
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
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = _pathManager.GetHomeUploadUrl(fileName);

            return new StringResult
            {
                Value = url
            };
        }
    }
}
