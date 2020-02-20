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
    
    [RoutePrefix("pages/settings/configHome")]
    public class PagesConfigHomeController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "upload";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigHome))
            {
                return Unauthorized();
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            return Ok(new
            {
                Value = config,
                WebConfigUtils.HomeDirectory,
                request.AdminToken,
                Styles = await DataProvider.TableStyleRepository.GetUserStyleListAsync()
            });
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            var request = await AuthenticatedRequest.GetAuthAsync();
            if (!request.IsAdminLoggin ||
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigHome))
            {
                return Unauthorized();
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            config.IsHomeClosed = request.GetPostBool("isHomeClosed");
            config.HomeTitle = request.GetPostString("homeTitle");
            config.IsHomeLogo = request.GetPostBool("isHomeLogo");
            config.HomeLogoUrl = request.GetPostString("homeLogoUrl");
            config.HomeDefaultAvatarUrl = request.GetPostString("homeDefaultAvatarUrl");
            config.UserRegistrationAttributes = request.GetPostString("userRegistrationAttributes");
            config.IsUserRegistrationGroup = request.GetPostBool("isUserRegistrationGroup");
            config.IsHomeAgreement = request.GetPostBool("isHomeAgreement");
            config.HomeAgreementHtml = request.GetPostString("homeAgreementHtml");

            await DataProvider.ConfigRepository.UpdateAsync(config);

            //                var config = $@"var $apiConfig = {{
            //    isSeparatedApi: {ApiManager.IsSeparatedApi.ToString().ToLower()},
            //    apiUrl: '{ApiManager.ApiUrl}',
            //    innerApiUrl: '{ApiManager.InnerApiUrl}'
            //}};
            //";

            await request.AddAdminLogAsync("修改用户中心设置");

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
                !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigHome))
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
                var filePath = DataProvider.UserRepository.GetHomeUploadPath(fileName);

                if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
                {
                    return BadRequest("image file extension is not correct");
                }

                postFile.SaveAs(filePath);

                homeLogoUrl = PageUtils.AddProtocolToUrl(DataProvider.UserRepository.GetHomeUploadUrl(fileName));
            }

            return Ok(new
            {
                Value = homeLogoUrl
            });
        }
    }
}
