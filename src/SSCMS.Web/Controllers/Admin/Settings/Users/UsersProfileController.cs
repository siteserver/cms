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

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersProfileController : ControllerBase
    {
        private const string Route = "settings/usersProfile";
        private const string RouteUpload = "settings/usersProfile/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public UsersProfileController(IAuthManager authManager, IPathManager pathManager, IUserRepository userRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _userRepository = userRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]int userId)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserIdAsync(userId);
            var userStyles = await _tableStyleRepository.GetUserStylesAsync();
            var styles = userStyles.Select(x => new InputStyle(x));

            return new GetResult
            {
                User = user,
                Styles = styles
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromQuery] int userId, [FromForm]IFormFile file)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            if (file == null) return this.Error("请选择有效的文件上传");
            var fileName = _pathManager.GetUploadFileName(file.FileName);
            var filePath = _pathManager.GetUserUploadPath(userId, fileName);
            if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
            {
                return this.Error("文件只能是图片格式，请选择有效的文件上传!");
            }

            await _pathManager.UploadAsync(file, filePath);

            var avatarUrl = _pathManager.GetUserUploadUrl(userId, fileName);

            return new StringResult
            {
                Value = avatarUrl
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]User request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            if (request.Id == 0)
            {
                var (user, errorMessage) = await _userRepository.InsertAsync(request, request.Password, string.Empty);
                if (user == null)
                {
                    return this.Error($"用户添加失败：{errorMessage}");
                }

                await _authManager.AddAdminLogAsync("添加用户", $"用户:{request.UserName}");
            }
            else
            {
                var(success, errorMessage) = await _userRepository.UpdateAsync(request);
                if (!success)
                {
                    return this.Error($"用户修改失败：{errorMessage}");
                }

                await _authManager.AddAdminLogAsync("修改用户", $"用户:{request.UserName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
