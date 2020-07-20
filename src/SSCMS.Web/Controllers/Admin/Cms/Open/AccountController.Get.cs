using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;
using SSCMS.Web.Controllers.Open;

namespace SSCMS.Web.Controllers.Admin.Cms.Open
{
    public partial class AccountController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] int siteId)
        {
            if (!await _authManager.HasSitePermissionsAsync(siteId, AuthTypes.SitePermissions.Account))
            {
                return Unauthorized();
            }

            var defaultWxUrl = Request.Scheme + "://" + PageUtils.Combine(Request.Host.Host, Constants.ApiOpenPrefix, WxController.Route.Replace("{siteId}", siteId.ToString()));
            var account = await _openAccountRepository.GetBySiteIdAsync(siteId);

            if (string.IsNullOrEmpty(account.WxUrl))
            {
                account.WxUrl = defaultWxUrl;
            }
            if (string.IsNullOrEmpty(account.WxToken))
            {
                account.WxToken = StringUtils.GetRandomString(32);
            }
            if (string.IsNullOrEmpty(account.WxEncodingAESKey))
            {
                account.WxEncodingAESKey = StringUtils.GetRandomString(43);
            }

            var defaultTenPayAuthorizeUrl = Request.Scheme + "://" + PageUtils.Combine(Request.Host.Host, Constants.ApiOpenPrefix, TenPayController.Route.Replace("{siteId}", siteId.ToString()));
            var defaultTenPayNotifyUrl = Request.Scheme + "://" + PageUtils.Combine(Request.Host.Host, Constants.ApiOpenPrefix, TenPayController.Route.Replace("{siteId}", siteId.ToString()));

            if (string.IsNullOrEmpty(account.TenPayAuthorizeUrl))
            {
                account.TenPayAuthorizeUrl = defaultTenPayAuthorizeUrl;
            }
            if (string.IsNullOrEmpty(account.TenPayNotifyUrl))
            {
                account.TenPayNotifyUrl = defaultTenPayNotifyUrl;
            }

            return new GetResult
            {
                DefaultWxUrl = defaultWxUrl,
                DefaultTenPayAuthorizeUrl = defaultTenPayAuthorizeUrl,
                DefaultTenPayNotifyUrl = defaultTenPayNotifyUrl,
                Account = account
            };
        }
    }
}
