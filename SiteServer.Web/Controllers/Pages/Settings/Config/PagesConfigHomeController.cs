using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

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
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Config))
                {
                    return Unauthorized();
                }

                var config = await DataProvider.ConfigDao.GetAsync();

                return Ok(new
                {
                    Value = config,
                    WebConfigUtils.HomeDirectory,
                    request.AdminToken,
                    Styles = await TableStyleManager.GetUserStyleListAsync()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Config))
                {
                    return Unauthorized();
                }

                var config = await DataProvider.ConfigDao.GetAsync();

                config.IsHomeClosed = request.GetPostBool("isHomeClosed");
                config.HomeTitle = request.GetPostString("homeTitle");
                config.IsHomeLogo = request.GetPostBool("isHomeLogo");
                config.HomeLogoUrl = request.GetPostString("homeLogoUrl");
                config.HomeDefaultAvatarUrl = request.GetPostString("homeDefaultAvatarUrl");
                config.UserRegistrationAttributes = request.GetPostString("userRegistrationAttributes");
                config.IsUserRegistrationGroup = request.GetPostBool("isUserRegistrationGroup");
                config.IsHomeAgreement = request.GetPostBool("isHomeAgreement");
                config.HomeAgreementHtml = request.GetPostString("homeAgreementHtml");

                await DataProvider.ConfigDao.UpdateAsync(config);

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
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Config))
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
                    var filePath = DataProvider.UserDao.GetHomeUploadPath(fileName);

                    if (!EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
                    {
                        return BadRequest("image file extension is not correct");
                    }

                    postFile.SaveAs(filePath);

                    homeLogoUrl = PageUtils.AddProtocolToUrl(DataProvider.UserDao.GetHomeUploadUrl(fileName));
                }

                return Ok(new
                {
                    Value = homeLogoUrl
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }
    }
}
