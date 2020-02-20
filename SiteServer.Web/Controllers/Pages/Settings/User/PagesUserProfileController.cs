using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.User
{
    
    [RoutePrefix("pages/settings/userProfile")]
    public class PagesUserProfileController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "upload";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser))
            {
                return Unauthorized();
            }

            var userId = request.GetQueryInt("userId");

            Abstractions.User user;
            if (userId > 0)
            {
                user = await DataProvider.UserRepository.GetByUserIdAsync(userId);
                if (user == null) return NotFound();
            }
            else
            {
                user = new Abstractions.User();
            }

            return Ok(new
            {
                Value = user,
                request.AdminToken
            });
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<IHttpActionResult> Upload()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser))
            {
                return Unauthorized();
            }

            var userId = request.GetQueryInt("userId");
            var user = await DataProvider.UserRepository.GetByUserIdAsync(userId);
            if (user == null) return NotFound();

            var avatarUrl = string.Empty;

            foreach (string name in HttpContext.Current.Request.Files)
            {
                var postFile = HttpContext.Current.Request.Files[name];

                if (postFile == null)
                {
                    return BadRequest("Could not read image from body");
                }

                var fileName = DataProvider.UserRepository.GetUserUploadFileName(postFile.FileName);
                var filePath = DataProvider.UserRepository.GetUserUploadPath(userId, fileName);

                if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
                {
                    return BadRequest("image file extension is not correct");
                }

                postFile.SaveAs(filePath);

                avatarUrl = DataProvider.UserRepository.GetUserUploadUrl(userId, fileName);
            }

            return Ok(new
            {
                Value = avatarUrl
            });
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUser))
            {
                return Unauthorized();
            }

            var userId = request.GetQueryInt("userId");
            Abstractions.User user;
            if (userId > 0)
            {
                user = await DataProvider.UserRepository.GetByUserIdAsync(userId);
                if (user == null) return NotFound();
            }
            else
            {
                user = new Abstractions.User();
            }

            var userName = request.GetPostString("userName");
            var password = request.GetPostString("password");
            var displayName = request.GetPostString("displayName");
            var avatarUrl = request.GetPostString("avatarUrl");
            var mobile = request.GetPostString("mobile");
            var email = request.GetPostString("email");

            if (user.Id == 0)
            {
                user.UserName = userName;
                user.CreateDate = DateTime.Now;
            }
            else
            {
                if (user.Mobile != mobile && !string.IsNullOrEmpty(mobile) && await DataProvider.UserRepository.IsMobileExistsAsync(mobile))
                {
                    return BadRequest("资料修改失败，手机号码已存在");
                }

                if (user.Email != email && !string.IsNullOrEmpty(email) && await DataProvider.UserRepository.IsEmailExistsAsync(email))
                {
                    return BadRequest("资料修改失败，邮箱地址已存在");
                }
            }

            user.DisplayName = displayName;
            user.AvatarUrl = avatarUrl;
            user.Mobile = mobile;
            user.Email = email;

            if (user.Id == 0)
            {
                var valid = await DataProvider.UserRepository.InsertAsync(user, password, string.Empty);
                if (valid.UserId == 0)
                {
                    return BadRequest($"用户添加失败：{valid.ErrorMessage}");
                }
                await request.AddAdminLogAsync("添加用户", $"用户:{user.UserName}");
            }
            else
            {
                await DataProvider.UserRepository.UpdateAsync(user);
                await request.AddAdminLogAsync("修改用户属性", $"用户:{user.UserName}");
            }

            return Ok(new
            {
                Value = true
            });
        }
    }
}
