using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class ReplyMessageController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.WxReplyAuto, MenuUtils.SitePermissions.WxReplyBeAdded))
            {
                return Unauthorized();
            }

            WxReplyMessage message = null;

            var account = await _wxAccountRepository.GetBySiteIdAsync(request.SiteId);
            var (success, _, errorMessage) = await _wxManager.GetAccessTokenAsync(account.MpAppId, account.MpAppSecret);
            if (success)
            {
                if (StringUtils.EqualsIgnoreCase(request.ActiveName, "replyAuto"))
                {
                    if (account.MpReplyAutoMessageId > 0)
                    {
                        message = await _wxManager.GetMessageAsync(request.SiteId, account.MpReplyAutoMessageId);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(request.ActiveName, "replyBeAdded"))
                {
                    if (account.MpReplyBeAddedMessageId > 0)
                    {
                        message = await _wxManager.GetMessageAsync(request.SiteId, account.MpReplyBeAddedMessageId);
                    }
                }
            }

            return new GetResult
            {
                Success = success,
                ErrorMessage = errorMessage,
                Message = message
            };
        }
    }
}