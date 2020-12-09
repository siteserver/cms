using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class ReplyMessageController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.WxReplyAuto, MenuUtils.SitePermissions.WxReplyBeAdded))
            {
                return Unauthorized();
            }

            var account = await _wxAccountRepository.GetBySiteIdAsync(request.SiteId);
            if (StringUtils.EqualsIgnoreCase(request.ActiveName, "replyAuto"))
            {
                account.MpReplyAutoMessageId = 0;
            }
            else if (StringUtils.EqualsIgnoreCase(request.ActiveName, "replyBeAdded"))
            {
                account.MpReplyBeAddedMessageId = 0;
            }
            await _wxAccountRepository.SetAsync(account);
            await _authManager.AddSiteLogAsync(request.SiteId, "修改微信收到消息回复");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
