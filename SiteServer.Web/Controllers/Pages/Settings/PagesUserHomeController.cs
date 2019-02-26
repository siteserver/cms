using System;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Caches;
using SiteServer.CMS.Database.Core;
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
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = ConfigManager.Instance.SystemExtend,
                    WebConfigUtils.HomeDirectory,
                    rest.AdminToken,
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
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                ConfigManager.Instance.SystemExtend.IsHomeClosed = rest.GetPostBool(nameof(ConfigManager.Instance.SystemExtend.IsHomeClosed).ToCamelCase());
                ConfigManager.Instance.SystemExtend.HomeTitle = rest.GetPostString(nameof(ConfigManager.Instance.SystemExtend.HomeTitle).ToCamelCase());
                ConfigManager.Instance.SystemExtend.IsHomeLogo = rest.GetPostBool(nameof(ConfigManager.Instance.SystemExtend.IsHomeLogo).ToCamelCase());
                ConfigManager.Instance.SystemExtend.HomeLogoUrl = rest.GetPostString(nameof(ConfigManager.Instance.SystemExtend.HomeLogoUrl).ToCamelCase());
                ConfigManager.Instance.SystemExtend.IsHomeBackground = rest.GetPostBool(nameof(ConfigManager.Instance.SystemExtend.IsHomeBackground).ToCamelCase());
                ConfigManager.Instance.SystemExtend.HomeBackgroundUrl = rest.GetPostString(nameof(ConfigManager.Instance.SystemExtend.HomeBackgroundUrl).ToCamelCase());
                ConfigManager.Instance.SystemExtend.HomeDefaultAvatarUrl = rest.GetPostString(nameof(ConfigManager.Instance.SystemExtend.HomeDefaultAvatarUrl).ToCamelCase());
                ConfigManager.Instance.SystemExtend.UserRegistrationAttributes = rest.GetPostString(nameof(ConfigManager.Instance.SystemExtend.UserRegistrationAttributes).ToCamelCase());
                ConfigManager.Instance.SystemExtend.IsUserRegistrationGroup = rest.GetPostBool(nameof(ConfigManager.Instance.SystemExtend.IsUserRegistrationGroup).ToCamelCase());
                ConfigManager.Instance.SystemExtend.IsHomeAgreement = rest.GetPostBool(nameof(ConfigManager.Instance.SystemExtend.IsHomeAgreement).ToCamelCase());
                ConfigManager.Instance.SystemExtend.HomeAgreementHtml = rest.GetPostString(nameof(ConfigManager.Instance.SystemExtend.HomeAgreementHtml).ToCamelCase());

                DataProvider.Config.Update(ConfigManager.Instance);

                rest.AddAdminLog("修改用户中心设置");

                return Ok(new
                {
                    Value = ConfigManager.Instance.SystemExtend
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
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
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
