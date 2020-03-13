using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    [Route("admin/settings/administratorsProfile")]
    public partial class AdministratorsProfileController : ControllerBase
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IAdministratorRepository _administratorRepository;

        public AdministratorsProfileController(IAuthManager authManager, IPathManager pathManager, IAdministratorRepository administratorRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _administratorRepository = administratorRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]int userId)
        {
            if (!await _authManager.IsAdminAuthenticatedAsync()) return Unauthorized();

            var adminId = await _authManager.GetAdminIdAsync();
            if (adminId != userId &&
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var administrator = await _administratorRepository.GetByUserIdAsync(userId);

            return new GetResult
            {
                UserName = administrator.UserName,
                DisplayName = administrator.DisplayName,
                AvatarUrl = administrator.AvatarUrl,
                Mobile = administrator.Mobile,
                Email = administrator.Email
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromQuery] int userId, [FromForm]IFormFile file)
        {
            if (!await _authManager.IsAdminAuthenticatedAsync()) return Unauthorized();

            var administrator = await _administratorRepository.GetByUserIdAsync(userId);
            if (administrator == null) return NotFound();

            var adminId = await _authManager.GetAdminIdAsync();
            if (adminId != userId &&
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            if (file == null) return this.Error("请选择有效的文件上传");
            var fileName = _pathManager.GetUploadFileName(file.FileName);
            var filePath = _pathManager.GetAdministratorUploadPath(userId, fileName);
            if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
            {
                return this.Error("文件只能是图片格式，请选择有效的文件上传!");
            }

            await _pathManager.UploadAsync(file, filePath);

            var avatarUrl = _pathManager.GetAdministratorUploadUrl(userId, fileName);

            return new StringResult
            {
                Value = avatarUrl
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var userId = request.UserId;
            if (!await _authManager.IsAdminAuthenticatedAsync()) return Unauthorized();

            var adminId = await _authManager.GetAdminIdAsync();
            if (adminId != userId &&
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            Administrator administrator;
            if (userId > 0)
            {
                administrator = await _administratorRepository.GetByUserIdAsync(userId);
                if (administrator == null) return NotFound();
            }
            else
            {
                administrator = new Administrator();
            }

            if (administrator.Id == 0)
            {
                administrator.UserName = request.UserName;
                administrator.CreatorUserName = await _authManager.GetAdminNameAsync();
                administrator.CreationDate = DateTime.Now;
            }
            else
            {
                if (administrator.Mobile != request.Mobile && !string.IsNullOrEmpty(request.Mobile) && await _administratorRepository.IsMobileExistsAsync(request.Mobile))
                {
                    return this.Error("资料修改失败，手机号码已存在");
                }

                if (administrator.Email != request.Email && !string.IsNullOrEmpty(request.Email) && await _administratorRepository.IsEmailExistsAsync(request.Email))
                {
                    return this.Error("资料修改失败，邮箱地址已存在");
                }
            }

            administrator.DisplayName = request.DisplayName;
            administrator.AvatarUrl = request.AvatarUrl;
            administrator.Mobile = request.Mobile;
            administrator.Email = request.Email;

            if (administrator.Id == 0)
            {
                var (isValid, errorMessage) = await _administratorRepository.InsertAsync(administrator, request.Password);
                if (!isValid)
                {
                    return this.Error($"管理员添加失败：{errorMessage}");
                }
                await _authManager.AddAdminLogAsync("添加管理员", $"管理员:{administrator.UserName}");
            }
            else
            {
                var (isValid, errorMessage) = await _administratorRepository.UpdateAsync(administrator);
                if (!isValid)
                {
                    return this.Error($"管理员修改失败：{errorMessage}");
                }
                await _authManager.AddAdminLogAsync("修改管理员属性", $"管理员:{administrator.UserName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
