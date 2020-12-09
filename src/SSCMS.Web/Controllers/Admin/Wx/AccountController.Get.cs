using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Web.Controllers.Wx;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class AccountController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] int siteId)
        {
            if (!await _authManager.HasSitePermissionsAsync(siteId, MenuUtils.SitePermissions.WxAccount))
            {
                return Unauthorized();
            }

            var account = await _wxAccountRepository.GetBySiteIdAsync(siteId);

            var defaultTenPayAuthorizeUrl = Request.Scheme + "://" + PageUtils.Combine(Request.Host.Host, Constants.ApiWxPrefix, TenPayController.Route.Replace("{siteId}", siteId.ToString()));
            var defaultTenPayNotifyUrl = Request.Scheme + "://" + PageUtils.Combine(Request.Host.Host, Constants.ApiWxPrefix, TenPayController.Route.Replace("{siteId}", siteId.ToString()));

            if (string.IsNullOrEmpty(account.TenPayAuthorizeUrl))
            {
                account.TenPayAuthorizeUrl = defaultTenPayAuthorizeUrl;
            }
            if (string.IsNullOrEmpty(account.TenPayNotifyUrl))
            {
                account.TenPayNotifyUrl = defaultTenPayNotifyUrl;
            }

            var mpUrl = Request.Scheme + "://" + PageUtils.Combine(Request.Host.Host, Constants.ApiWxPrefix, Controllers.Wx.IndexController.Route.Replace("{siteId}", siteId.ToString()));

            return new GetResult
            {
                MpUrl = mpUrl,
                DefaultTenPayAuthorizeUrl = defaultTenPayAuthorizeUrl,
                DefaultTenPayNotifyUrl = defaultTenPayNotifyUrl,
                Account = account
            };
        }
    }
}
