using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/utilityEncrypt")]
    public class PagesUtilityEncryptController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public IHttpActionResult Post()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Utility))
                {
                    return Unauthorized();
                }

                var isEncrypt = request.GetPostBool("isEncrypt");
                var value = request.GetPostString("value");

                var encoded = isEncrypt
                    ? TranslateUtils.EncryptStringBySecretKey(value)
                    : TranslateUtils.DecryptStringBySecretKey(value);

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
