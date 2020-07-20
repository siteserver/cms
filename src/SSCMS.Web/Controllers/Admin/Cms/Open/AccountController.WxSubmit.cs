using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Open
{
    public partial class AccountController
    {

        [HttpPost, Route(RouteWx)]
        public async Task<ActionResult<BoolResult>> WxSubmit([FromBody] WxSubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.Account))
            {
                return Unauthorized();
            }

            var account = await _openAccountRepository.GetBySiteIdAsync(request.SiteId);

            account.WxAppId = request.WxAppId;
            account.WxAppSecret = request.WxAppSecret;
            account.WxUrl = request.WxUrl;
            account.WxToken = request.WxToken;
            account.WxIsEncrypt = request.WxIsEncrypt;
            account.WxEncodingAESKey = request.WxEncodingAESKey;

            await _openAccountRepository.SetAsync(account);

            await _authManager.AddAdminLogAsync("修改微信公众号设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
