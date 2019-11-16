using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings.Utility
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/utilityEncrypt")]
    public class PagesUtilityEncryptController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Post()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.Utility))
                {
                    return Unauthorized();
                }

                var isEncrypt = request.GetPostBool("isEncrypt");
                var value = request.GetPostString("value");

                var encoded = isEncrypt
                    ? WebConfigUtils.EncryptStringBySecretKey(value)
                    : WebConfigUtils.DecryptStringBySecretKey(value);

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
