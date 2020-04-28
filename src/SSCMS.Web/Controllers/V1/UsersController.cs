using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Extensions;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    [Authorize(Roles = AuthTypes.Roles.Api)]
    [Route(Constants.ApiV1Prefix)]
    public partial class UsersController : ControllerBase
    {
        private const string Route = "users";
        private const string RouteActionsLogin = "users/actions/login";
        private const string RouteUser = "users/{id:int}";
        private const string RouteUserAvatar = "users/{id:int}/avatar";
        private const string RouteUserLogs = "users/{id:int}/logs";
        private const string RouteUserResetPassword = "users/{id:int}/actions/resetPassword";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;

        public UsersController(IAuthManager authManager, IPathManager pathManager, IConfigRepository configRepository, IAccessTokenRepository accessTokenRepository, IUserRepository userRepository, ILogRepository logRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
            _accessTokenRepository = accessTokenRepository;
            _userRepository = userRepository;
            _logRepository = logRepository;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<User>> Create([FromBody]User request)
        {
            var config = await _configRepository.GetAsync();

            if (!config.IsUserRegistrationGroup)
            {
                request.GroupId = 0;
            }
            var password = request.Password;

            var (user, errorMessage) = await _userRepository.InsertAsync(request, password, string.Empty);
            if (user == null)
            {
                return this.Error(errorMessage);
            }

            return user;
        }

        [HttpPut, Route(RouteUser)]
        public async Task<ActionResult<User>> Update([FromRoute]int id, [FromBody]User request)
        {
            var isAuth = await
                             _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsUser &&
                         _authManager.UserId == id ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            var (success, errorMessage) = await _userRepository.UpdateAsync(request);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            return request;
        }

        [HttpDelete, Route(RouteUser)]
        public async Task<ActionResult<User>> Delete(int id)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsUser &&
                         _authManager.UserId == id ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            var user = await _userRepository.DeleteAsync(id);

            return user;
        }

        [HttpGet, Route(RouteUser)]
        public async Task<ActionResult<User>> Get(int id)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsUser &&
                         _authManager.UserId == id ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            if (!await _userRepository.IsExistsAsync(id)) return NotFound();

            var user = await _userRepository.GetByUserIdAsync(id);

            return user;
        }

        [HttpGet, Route(RouteUserAvatar)]
        public async Task<StringResult> GetAvatar(int id)
        {
            var user = await _userRepository.GetByUserIdAsync(id);

            var avatarUrl = !string.IsNullOrEmpty(user?.AvatarUrl) ? user.AvatarUrl : _pathManager.DefaultAvatarUrl;
            avatarUrl = PageUtils.AddProtocolToUrl(avatarUrl);

            return new StringResult
            {
                Value = avatarUrl
            };
        }

        [HttpPost, Route(RouteUserAvatar)]
        public async Task<ActionResult<User>> UploadAvatar([FromQuery] int id, [FromForm] IFormFile file)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsUser &&
                         _authManager.UserId == id ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            var user = await _userRepository.GetByUserIdAsync(id);
            if (user == null) return NotFound();

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            fileName = _pathManager.GetUserUploadFileName(fileName);
            var filePath = _pathManager.GetUserUploadPath(user.Id, fileName);

            if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
            {
                return this.Error("文件只能是 Image 格式，请选择有效的文件上传");
            }

            await _pathManager.UploadAsync(file, filePath);

            user.AvatarUrl = _pathManager.GetUserUploadUrl(user.Id, fileName);

            await _userRepository.UpdateAsync(user);

            return user;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> List([FromQuery]ListRequest request)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            var top = request.Top;
            if (top <= 0)
            {
                top = 20;
            }

            var skip = request.Skip;

            var users = await _userRepository.GetUsersAsync(null, 0, 0, null, null, skip, top);
            var count = await _userRepository.GetCountAsync();

            return new ListResult
            {
                Count = count,
                Users = users
            };
        }

        [HttpPost, Route(RouteActionsLogin)]
        public async Task<ActionResult<LoginResult>> Login([FromBody]LoginRequest request)
        {
            var (user, _, errorMessage) = await _userRepository.ValidateAsync(request.Account, request.Password, true);
            if (user == null)
            {
                return this.Error(errorMessage);
            }

            var accessToken = _authManager.AuthenticateUser(user, request.IsPersistent);

            await _userRepository.UpdateLastActivityDateAndCountOfLoginAsync(user);
            await _logRepository.AddUserLogAsync(user, "用户登录", string.Empty);

            return new LoginResult
            {
                User = user,
                AccessToken = accessToken
            };
        }

        [HttpPost, Route(RouteUserLogs)]
        public async Task<ActionResult<Log>> CreateLog(int id, [FromBody] Log log)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsUser &&
                         _authManager.UserId == id ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            var user = await _userRepository.GetByUserIdAsync(id);
            if (user == null) return NotFound();

            log.UserId = user.Id;
            await _logRepository.AddUserLogAsync(user, log.Action, log.Summary);

            return log;
        }

        [HttpGet, Route(RouteUserLogs)]
        public async Task<ActionResult<GetLogsResult>> GetLogs(int id, [FromQuery]GetLogsRequest request)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsUser &&
                         _authManager.UserId == id ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            var user = await _userRepository.GetByUserIdAsync(id);
            if (user == null) return NotFound();

            var top = request.Top;
            if (top <= 0)
            {
                top = 20;
            }
            var skip = request.Skip;

            var logs = await _logRepository.GetUserLogsAsync(user.Id, skip, top);

            return new GetLogsResult
            {
                Count = await _userRepository.GetCountAsync(),
                Logs = logs
            };
        }

        [HttpPost, Route(RouteUserResetPassword)]
        public async Task<ActionResult<User>> ResetPassword(int id, [FromBody]ResetPasswordRequest request)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsUser &&
                         _authManager.UserId == id ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            var user = await _userRepository.GetByUserIdAsync(id);
            if (user == null) return NotFound();

            if (!_userRepository.CheckPassword(request.Password, false, user.Password, user.PasswordFormat, user.PasswordSalt))
            {
                return this.Error("原密码不正确，请重新输入");
            }

            var (success, errorMessage) = await _userRepository.ChangePasswordAsync(user.Id, request.NewPassword);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            await _logRepository.AddUserLogAsync(user, "修改密码", string.Empty);

            return user;
        }
    }
}
