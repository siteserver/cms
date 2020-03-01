using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Users
{
    [Route("admin/settings/usersProfile")]
    public partial class UsersProfileController : ControllerBase
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IUserRepository _userRepository;

        public UsersProfileController(IAuthManager authManager, IPathManager pathManager, IUserRepository userRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _userRepository = userRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]int userId)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserIdAsync(userId);

            return new GetResult
            {
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                AvatarUrl = user.AvatarUrl,
                Mobile = user.Mobile,
                Email = user.Email
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromQuery] int userId, [FromForm]IFormFile file)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsers))
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
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            User user;
            if (request.UserId > 0)
            {
                user = await _userRepository.GetByUserIdAsync(request.UserId);
                if (user == null) return NotFound();
            }
            else
            {
                user = new User();
            }

            if (user.Id == 0)
            {
                user.UserName = request.UserName;
                user.CreateDate = DateTime.Now;
            }
            else
            {
                if (user.Mobile != request.Mobile && !string.IsNullOrEmpty(request.Mobile) && await _userRepository.IsMobileExistsAsync(request.Mobile))
                {
                    return this.Error("资料修改失败，手机号码已存在");
                }

                if (user.Email != request.Email && !string.IsNullOrEmpty(request.Email) && await _userRepository.IsEmailExistsAsync(request.Email))
                {
                    return this.Error("资料修改失败，邮箱地址已存在");
                }
            }

            user.DisplayName = request.DisplayName;
            user.AvatarUrl = request.AvatarUrl;
            user.Mobile = request.Mobile;
            user.Email = request.Email;

            if (user.Id == 0)
            {
                var valid = await _userRepository.InsertAsync(user, request.Password, string.Empty);
                if (valid.UserId == 0)
                {
                    return this.Error($"用户添加失败：{valid.ErrorMessage}");
                }
                await auth.AddAdminLogAsync("添加用户", $"用户:{user.UserName}");
            }
            else
            {
                await _userRepository.UpdateAsync(user);
                await auth.AddAdminLogAsync("修改用户属性", $"用户:{user.UserName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
