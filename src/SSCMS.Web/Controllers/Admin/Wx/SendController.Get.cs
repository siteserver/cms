using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class SendController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.WxSend))
            {
                return Unauthorized();
            }

            MaterialMessage message = null;

            var site = await _siteRepository.GetAsync(request.SiteId);
            var isWxEnabled = await _wxManager.IsEnabledAsync(site);
            
            if (isWxEnabled)
            {
                var account = await _wxManager.GetAccountAsync(request.SiteId);
                var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(account);
                if (!success)
                {
                    return this.Error(errorMessage);
                }

                if (account.MpType == WxMpType.Subscription || account.MpType == WxMpType.Service)
                {
                    return this.Error(_wxManager.GetErrorUnAuthenticated(account));
                }

                if (request.MessageId > 0)
                {
                    message = await _materialMessageRepository.GetAsync(request.MessageId);
                }
            }

            return new GetResult
            {
                IsWxEnabled = isWxEnabled,
                Message = message,
            };
        }
    }
}
