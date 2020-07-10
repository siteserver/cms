using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class ProfileController : ControllerBase
    {
        private const string Route = "profile";
        private const string RouteUpload = "profile/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public ProfileController(IAuthManager authManager, IPathManager pathManager, IConfigRepository configRepository, IUserRepository userRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
            _userRepository = userRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        [HttpGet, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var config = await _configRepository.GetAsync();
            if (config.IsHomeClosed) return this.Error("对不起，用户中心已被禁用！");

            var user = await _authManager.GetUserAsync();
            var userStyles = await _tableStyleRepository.GetUserStylesAsync();
            var styles = userStyles.Select(x => new InputStyle(x));

            return new GetResult
            {
                User = user,
                Styles = styles
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromForm]IFormFile file)
        {
            if (file == null) return this.Error("请选择有效的文件上传");
            var fileName = _pathManager.GetUploadFileName(file.FileName);
            var filePath = _pathManager.GetUserUploadPath(_authManager.UserId, fileName);
            if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
            {
                return this.Error("文件只能是图片格式，请选择有效的文件上传!");
            }

            await _pathManager.UploadAsync(file, filePath);

            var avatarUrl = _pathManager.GetUserUploadUrl(_authManager.UserId, fileName);

            return new StringResult
            {
                Value = avatarUrl
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]User request)
        {
            if (request.Id != _authManager.UserId) return Unauthorized();

            var (success, errorMessage) = await _userRepository.UpdateAsync(request);
            if (!success)
            {
                return this.Error($"修改资料失败：{errorMessage}");
            }

            await _authManager.AddUserLogAsync("修改资料");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
