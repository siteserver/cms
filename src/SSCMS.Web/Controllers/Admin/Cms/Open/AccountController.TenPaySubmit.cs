using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Open
{
    public partial class AccountController
    {

        [HttpPost, Route(RouteTenPay)]
        public async Task<ActionResult<BoolResult>> TenPaySubmit([FromBody] TenPaySubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.Account))
            {
                return Unauthorized();
            }

            var account = await _openAccountRepository.GetBySiteIdAsync(request.SiteId);

            account.TenPayAppId = request.TenPayAppId;
            account.TenPayAppSecret = request.TenPayAppSecret;
            account.TenPayMchId = request.TenPayMchId;
            account.TenPayKey = request.TenPayKey;
            account.TenPayAuthorizeUrl = request.TenPayAuthorizeUrl;
            account.TenPayNotifyUrl = request.TenPayNotifyUrl;

            await _openAccountRepository.SetAsync(account);

            await _authManager.AddAdminLogAsync("修改微信支付设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
