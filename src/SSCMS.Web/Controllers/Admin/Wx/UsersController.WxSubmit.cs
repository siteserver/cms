using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class UsersController
    {

        [HttpPost, Route(RouteWx)]
        public async Task<ActionResult<BoolResult>> WxSubmit([FromBody] WxSubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.WxAccount))
            {
                return Unauthorized();
            }

            var account = await _wxAccountRepository.GetBySiteIdAsync(request.SiteId);

            account.MpAppId = request.WxAppId;
            account.MpAppSecret = request.WxAppSecret;
            account.MpUrl = request.WxUrl;
            account.MpToken = request.WxToken;
            account.MpIsEncrypt = request.WxIsEncrypt;
            account.MpEncodingAESKey = request.WxEncodingAESKey;

            await _wxAccountRepository.SetAsync(account);

            await _authManager.AddAdminLogAsync("修改微信公众号设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
