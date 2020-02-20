using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Config
{
    
    [RoutePrefix("pages/settings/configAdmin")]
    public class PagesConfigAdminController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "upload";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigAdmin))
            {
                return Unauthorized();
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            return Ok(new
            {
                Value = config,
                request.AdminToken
            });
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigAdmin))
            {
                return Unauthorized();
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            config.AdminTitle = request.GetPostString("adminTitle");
            config.AdminLogoUrl = request.GetPostString("adminLogoUrl");
            config.AdminWelcomeHtml = request.GetPostString("adminWelcomeHtml");

            await DataProvider.ConfigRepository.UpdateAsync(config);

            await request.AddAdminLogAsync("修改管理后台设置");

            return Ok(new
            {
                Value = config
            });
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<IHttpActionResult> Upload()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigAdmin))
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
                var filePath = PathUtility.GetAdminDirectoryPath(fileName);

                if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
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
    }
}
