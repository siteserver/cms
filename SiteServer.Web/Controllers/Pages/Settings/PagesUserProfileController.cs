using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/userProfile")]
    public class PagesUserProfileController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "upload";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var userId = request.GetQueryInt("userId");

                User user;
                if (userId > 0)
                {
                    user = await UserManager.GetUserByUserIdAsync(userId);
                    if (user == null) return NotFound();
                }
                else
                {
                    user = new User();
                }

                return Ok(new
                {
                    Value = user,
                    request.AdminToken
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<IHttpActionResult> Upload()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var userId = request.GetQueryInt("userId");
                var user = await UserManager.GetUserByUserIdAsync(userId);
                if (user == null) return NotFound();

                var avatarUrl = string.Empty;

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read image from body");
                    }

                    var fileName = UserManager.GetUserUploadFileName(postFile.FileName);
                    var filePath = UserManager.GetUserUploadPath(userId, fileName);

                    if (!EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
                    {
                        return BadRequest("image file extension is not correct");
                    }

                    postFile.SaveAs(filePath);

                    avatarUrl = UserManager.GetUserUploadUrl(userId, fileName);
                }

                return Ok(new
                {
                    Value = avatarUrl
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var userId = request.GetQueryInt("userId");
                User user;
                if (userId > 0)
                {
                    user = await UserManager.GetUserByUserIdAsync(userId);
                    if (user == null) return NotFound();
                }
                else
                {
                    user = new User();
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
                    if (user.Mobile != mobile && !string.IsNullOrEmpty(mobile) && await DataProvider.UserDao.IsMobileExistsAsync(mobile))
                    {
                        return BadRequest("资料修改失败，手机号码已存在");
                    }

                    if (user.Email != email && !string.IsNullOrEmpty(email) && await DataProvider.UserDao.IsEmailExistsAsync(email))
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
                    var valid = await DataProvider.UserDao.InsertAsync(user, password, PageUtils.GetIpAddress());
                    if (valid.UserId == 0)
                    {
                        return BadRequest($"用户添加失败：{valid.ErrorMessage}");
                    }
                    await request.AddAdminLogAsync("添加用户", $"用户:{user.UserName}");
                }
                else
                {
                    await DataProvider.UserDao.UpdateAsync(user);
                    await request.AddAdminLogAsync("修改用户属性", $"用户:{user.UserName}");
                }

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
