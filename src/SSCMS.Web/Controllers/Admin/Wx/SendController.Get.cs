using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Core.Services;

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

            var results = new GetResult
            {
                Success = false,
                ErrorMessage = string.Empty,
                Tags = new List<WxUserTag>(),
                Message = null,
            };

            var account = await _wxManager.GetAccountAsync(request.SiteId);
            string token;
            (results.Success, token, results.ErrorMessage) = await _wxManager.GetAccessTokenAsync(account);
            if (!results.Success)
            {
                return results;
            }
            if (account.MpType == WxMpType.Subscription || account.MpType == WxMpType.Service)
            {
                results.Success = false;
                results.ErrorMessage = _wxManager.GetErrorUnAuthenticated(account);
                return results;
            }

            results.Tags = await _wxManager.GetUserTagsAsync(token);
            if (request.MessageId > 0)
            {
                results.Message = await _materialMessageRepository.GetAsync(request.MessageId);
            }

            return results;
        }
    }
}
