using System;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
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
                    Value = ConfigManager.Instance,
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

                ConfigManager.Instance.IsHomeClosed = rest.GetPostBool(nameof(ConfigManager.Instance.IsHomeClosed).ToCamelCase());
                ConfigManager.Instance.HomeTitle = rest.GetPostString(nameof(ConfigManager.Instance.HomeTitle).ToCamelCase());
                ConfigManager.Instance.IsHomeLogo = rest.GetPostBool(nameof(ConfigManager.Instance.IsHomeLogo).ToCamelCase());
                ConfigManager.Instance.HomeLogoUrl = rest.GetPostString(nameof(ConfigManager.Instance.HomeLogoUrl).ToCamelCase());
                ConfigManager.Instance.IsHomeBackground = rest.GetPostBool(nameof(ConfigManager.Instance.IsHomeBackground).ToCamelCase());
                ConfigManager.Instance.HomeBackgroundUrl = rest.GetPostString(nameof(ConfigManager.Instance.HomeBackgroundUrl).ToCamelCase());
                ConfigManager.Instance.HomeDefaultAvatarUrl = rest.GetPostString(nameof(ConfigManager.Instance.HomeDefaultAvatarUrl).ToCamelCase());
                ConfigManager.Instance.UserRegistrationAttributes = rest.GetPostString(nameof(ConfigManager.Instance.UserRegistrationAttributes).ToCamelCase());
                ConfigManager.Instance.IsUserRegistrationGroup = rest.GetPostBool(nameof(ConfigManager.Instance.IsUserRegistrationGroup).ToCamelCase());
                ConfigManager.Instance.IsHomeAgreement = rest.GetPostBool(nameof(ConfigManager.Instance.IsHomeAgreement).ToCamelCase());
                ConfigManager.Instance.HomeAgreementHtml = rest.GetPostString(nameof(ConfigManager.Instance.HomeAgreementHtml).ToCamelCase());

                DataProvider.Config.Update(ConfigManager.Instance);

                rest.AddAdminLog("修改用户中心设置");

                return Ok(new
                {
                    Value = ConfigManager.Instance
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
