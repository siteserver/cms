using System;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/userHome")]
    public class PagesUserHomeController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "upload";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = ConfigManager.Instance.SystemConfigInfo,
                    WebConfigUtils.HomeDirectory,
                    request.AdminToken,
                    Styles = TableStyleManager.GetUserStyleInfoList()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                ConfigManager.SystemConfigInfo.IsHomeClosed = request.GetPostBool(nameof(ConfigManager.SystemConfigInfo.IsHomeClosed).ToCamelCase());
                ConfigManager.SystemConfigInfo.HomeTitle = request.GetPostString(nameof(ConfigManager.SystemConfigInfo.HomeTitle).ToCamelCase());
                ConfigManager.SystemConfigInfo.IsHomeLogo = request.GetPostBool(nameof(ConfigManager.SystemConfigInfo.IsHomeLogo).ToCamelCase());
                ConfigManager.SystemConfigInfo.HomeLogoUrl = request.GetPostString(nameof(ConfigManager.SystemConfigInfo.HomeLogoUrl).ToCamelCase());
                ConfigManager.SystemConfigInfo.IsHomeBackground = request.GetPostBool(nameof(ConfigManager.SystemConfigInfo.IsHomeBackground).ToCamelCase());
                ConfigManager.SystemConfigInfo.HomeBackgroundUrl = request.GetPostString(nameof(ConfigManager.SystemConfigInfo.HomeBackgroundUrl).ToCamelCase());
                ConfigManager.SystemConfigInfo.HomeDefaultAvatarUrl = request.GetPostString(nameof(ConfigManager.SystemConfigInfo.HomeDefaultAvatarUrl).ToCamelCase());
                ConfigManager.SystemConfigInfo.UserRegistrationAttributes = request.GetPostString(nameof(ConfigManager.SystemConfigInfo.UserRegistrationAttributes).ToCamelCase());
                ConfigManager.SystemConfigInfo.IsUserRegistrationGroup = request.GetPostBool(nameof(ConfigManager.SystemConfigInfo.IsUserRegistrationGroup).ToCamelCase());
                ConfigManager.SystemConfigInfo.IsHomeAgreement = request.GetPostBool(nameof(ConfigManager.SystemConfigInfo.IsHomeAgreement).ToCamelCase());
                ConfigManager.SystemConfigInfo.HomeAgreementHtml = request.GetPostString(nameof(ConfigManager.SystemConfigInfo.HomeAgreementHtml).ToCamelCase());

                DataProvider.ConfigDao.Update(ConfigManager.Instance);

                request.AddAdminLog("修改用户中心设置");

                return Ok(new
                {
                    Value = ConfigManager.SystemConfigInfo
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUpload)]
        public IHttpActionResult Upload()
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var homeLogoUrl = string.Empty;

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read image from body");
                    }

                    var fileName = postFile.FileName;
                    var filePath = UserManager.GetHomeUploadPath(fileName);

                    if (!EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
                    {
                        return BadRequest("image file extension is not correct");
                    }

                    postFile.SaveAs(filePath);

                    homeLogoUrl = PageUtils.AddProtocolToUrl(UserManager.GetHomeUploadUrl(fileName));
                }

                return Ok(new
                {
                    Value = homeLogoUrl
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
