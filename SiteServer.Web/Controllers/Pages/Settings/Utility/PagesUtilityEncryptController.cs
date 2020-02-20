using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Controllers.Pages.Settings.Utility
{
    
    [RoutePrefix("pages/settings/utilityEncrypt")]
    public class PagesUtilityEncryptController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Post()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUtilityEncrypt))
                {
                    return Unauthorized();
                }

                var isEncrypt = request.GetPostBool("isEncrypt");
                var value = request.GetPostString("value");

                var encoded = isEncrypt
                    ? TranslateUtils.EncryptStringBySecretKey(value, WebConfigUtils.SecretKey)
                    : TranslateUtils.DecryptStringBySecretKey(value, WebConfigUtils.SecretKey);

                if (!isEncrypt && string.IsNullOrEmpty(encoded))
                {
                    return BadRequest("指定的字符串为非系统加密的字符串");
                }

                return Ok(new
                {
                    Value = encoded
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
