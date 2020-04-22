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

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = Constants.RoleTypeUser)]
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
            var user = await _authManager.GetUserAsync();

            return new GetResult
            {
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                AvatarUrl = user.AvatarUrl,
                Mobile = user.Mobile,
                Email = user.Email,
                Config = config,
                Styles = await _tableStyleRepository.GetUserStyleListAsync()
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
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var user = await _authManager.GetUserAsync();

            if (user.Mobile != request.Mobile && !string.IsNullOrEmpty(request.Mobile) && await _userRepository.IsMobileExistsAsync(request.Mobile))
            {
                return this.Error("资料修改失败，手机号码已存在");
            }

            if (user.Email != request.Email && !string.IsNullOrEmpty(request.Email) && await _userRepository.IsEmailExistsAsync(request.Email))
            {
                return this.Error("资料修改失败，邮箱地址已存在");
            }

            user.DisplayName = request.DisplayName;
            user.AvatarUrl = request.AvatarUrl;
            user.Mobile = request.Mobile;
            user.Email = request.Email;

            await _userRepository.UpdateAsync(user);
            await _authManager.AddUserLogAsync("修改资料");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
