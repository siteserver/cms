using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    
    [RoutePrefix("pages/settings/adminProfile")]
    public class PagesAdminProfileController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "upload";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var userId = request.GetQueryInt("userId");
                if (!request.IsAdminLoggin) return Unauthorized();
                if (request.AdminId != userId &&
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                Administrator adminInfo;
                if (userId > 0)
                {
                    adminInfo = await AdminManager.GetByUserIdAsync(userId);
                    if (adminInfo == null) return NotFound();
                }
                else
                {
                    adminInfo = new Administrator();
                }

                return Ok(new
                {
                    Value = adminInfo,
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
                var request = await AuthenticatedRequest.GetAuthAsync();
                var userId = request.GetQueryInt("userId");
                if (!request.IsAdminLoggin) return Unauthorized();
                var adminInfo = await AdminManager.GetByUserIdAsync(userId);
                if (adminInfo == null) return NotFound();
                if (request.AdminId != userId &&
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var avatarUrl = string.Empty;

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read image from body");
                    }

                    var fileName = AdminManager.GetUserUploadFileName(postFile.FileName);
                    var filePath = AdminManager.GetUserUploadPath(userId, fileName);

                    if (!EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
                    {
                        return BadRequest("image file extension is not correct");
                    }

                    postFile.SaveAs(filePath);

                    avatarUrl = AdminManager.GetUserUploadUrl(userId, fileName);
                }

                return Ok(new
                {
                    Value = avatarUrl
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var userId = request.GetQueryInt("userId");
                if (!request.IsAdminLoggin) return Unauthorized();
                if (request.AdminId != userId &&
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                Administrator adminInfo;
                if (userId > 0)
                {
                    adminInfo = await AdminManager.GetByUserIdAsync(userId);
                    if (adminInfo == null) return NotFound();
                }
                else
                {
                    adminInfo = new Administrator();
                }

                var userName = request.GetPostString("userName");
                var password = request.GetPostString("password");
                var displayName = request.GetPostString("displayName");
                var avatarUrl = request.GetPostString("avatarUrl");
                var mobile = request.GetPostString("mobile");
                var email = request.GetPostString("email");

                if (adminInfo.Id == 0)
                {
                    adminInfo.UserName = userName;
                    adminInfo.CreatorUserName = request.AdminName;
                    adminInfo.CreationDate = DateTime.Now;
                }
                else
                {
                    if (adminInfo.Mobile != mobile && !string.IsNullOrEmpty(mobile) && await DataProvider.AdministratorDao.IsMobileExistsAsync(mobile))
                    {
                        return BadRequest("资料修改失败，手机号码已存在");
                    }

                    if (adminInfo.Email != email && !string.IsNullOrEmpty(email) && await DataProvider.AdministratorDao.IsEmailExistsAsync(email))
                    {
                        return BadRequest("资料修改失败，邮箱地址已存在");
                    }
                }

                adminInfo.DisplayName = displayName;
                adminInfo.AvatarUrl = avatarUrl;
                adminInfo.Mobile = mobile;
                adminInfo.Email = email;

                if (adminInfo.Id == 0)
                {
                    var valid = await DataProvider.AdministratorDao.InsertAsync(adminInfo, password);
                    if (!valid.IsValid)
                    {
                        return BadRequest($"管理员添加失败：{valid.ErrorMessage}");
                    }
                    await request.AddAdminLogAsync("添加管理员", $"管理员:{adminInfo.UserName}");
                }
                else
                {
                    var valid = await DataProvider.AdministratorDao.UpdateAsync(adminInfo);
                    if (!valid.IsValid)
                    {
                        return BadRequest($"管理员修改失败：{valid.ErrorMessage}");
                    }
                    await request.AddAdminLogAsync("修改管理员属性", $"管理员:{adminInfo.UserName}");
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
