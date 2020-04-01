using System;
using System.Web;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Settings.Config
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/configAdmin")]
    public class PagesConfigAdminController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "upload";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsConfigAdmin))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = ConfigManager.Instance.SystemConfigInfo,
                    request.AdminToken
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
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsConfigAdmin))
                {
                    return Unauthorized();
                }

                ConfigManager.SystemConfigInfo.AdminTitle = request.GetPostString("adminTitle");
                ConfigManager.SystemConfigInfo.AdminLogoUrl = request.GetPostString("adminLogoUrl");
                ConfigManager.SystemConfigInfo.AdminWelcomeHtml = request.GetPostString("adminWelcomeHtml");

                DataProvider.ConfigDao.Update(ConfigManager.Instance);

                request.AddAdminLog("修改管理后台设置");

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
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsConfigAdmin))
                {
                    return Unauthorized();
                }

                var adminLogoUrl = string.Empty;

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read image from body");
                    }

                    var fileName = postFile.FileName;
                    var filePath = PathUtils.GetAdminDirectoryPath(fileName);

                    if (!EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
                    {
                        return BadRequest("image file extension is not correct");
                    }

                    postFile.SaveAs(filePath);

                    adminLogoUrl = fileName;
                }

                return Ok(new
                {
                    Value = adminLogoUrl
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
