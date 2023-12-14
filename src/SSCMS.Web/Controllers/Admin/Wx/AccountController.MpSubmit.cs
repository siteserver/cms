using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class AccountController
    {
        [HttpPost, Route(RouteMp)]
        public async Task<ActionResult<MpSubmitResult>> MpSubmit([FromBody] MpSubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxAccount))
            {
                return Unauthorized();
            }

            var account = await _wxAccountRepository.GetBySiteIdAsync(request.SiteId);
            var success = true;
            var errorMessage = string.Empty;

            if (request.IsEnabled)
            {
                (success, _, errorMessage) = await _wxManager.GetAccessTokenAsync(request.MpAppId, request.MpAppSecret);
                if (success)
                {
                    account.IsEnabled = true;
                    account.MpName = request.MpName;
                    account.MpType = request.MpType;
                    account.MpAppId = request.MpAppId;
                    account.MpAppSecret = request.MpAppSecret;

                    if (string.IsNullOrEmpty(account.MpToken))
                    {
                        account.MpToken = StringUtils.GetRandomString(32);
                    }
                    if (string.IsNullOrEmpty(account.MpEncodingAESKey))
                    {
                        account.MpIsEncrypt = true;
                        account.MpEncodingAESKey = StringUtils.GetRandomString(43);
                    }

                    await _wxAccountRepository.SetAsync(account);
                    await _authManager.AddSiteLogAsync(request.SiteId, "修改微信公众号设置");
                }
            }
            else
            {
                account.IsEnabled = false;
                await _wxAccountRepository.SetAsync(account);
            }

            return new MpSubmitResult
            {
                Success = success,
                ErrorMessage = errorMessage,
                Account = account
            };
        }
    }
}
