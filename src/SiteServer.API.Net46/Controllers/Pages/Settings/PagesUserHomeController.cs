using System;
using System.Web;
using System.Web.Http;
using SiteServer.BackgroundPages.Core;
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
                var request = new Request(HttpContext.Current.Request);
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = ConfigManager.Instance,
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
                var request = new Request(HttpContext.Current.Request);
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                ConfigManager.Instance.IsHomeClosed = request.GetPostBool("isHomeClosed");
                ConfigManager.Instance.HomeTitle = request.GetPostString("homeTitle");
                ConfigManager.Instance.IsHomeLogo = request.GetPostBool("isHomeLogo");
                ConfigManager.Instance.HomeLogoUrl = request.GetPostString("homeLogoUrl");
                ConfigManager.Instance.HomeDefaultAvatarUrl = request.GetPostString("homeDefaultAvatarUrl");
                ConfigManager.Instance.UserRegistrationAttributes = request.GetPostString("userRegistrationAttributes");
                ConfigManager.Instance.IsUserRegistrationGroup = request.GetPostBool("isUserRegistrationGroup");
                ConfigManager.Instance.IsHomeAgreement = request.GetPostBool("isHomeAgreement");
                ConfigManager.Instance.HomeAgreementHtml = request.GetPostString("homeAgreementHtml");

                DataProvider.ConfigDao.Update(ConfigManager.Instance);

                //                var config = $@"var $apiConfig = {{
                //    isSeparatedApi: {ApiManager.IsSeparatedApi.ToString().ToLower()},
                //    apiUrl: '{ApiManager.ApiUrl}',
                //    innerApiUrl: '{ApiManager.InnerApiUrl}'
                //}};
                //";

                request.AddAdminLog("修改用户中心设置");

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
                var request = new Request(HttpContext.Current.Request);
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

                    homeLogoUrl = PageUtilsEx.AddProtocolToUrl(UserManager.GetHomeUploadUrl(fileName));
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
