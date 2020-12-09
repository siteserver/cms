using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class ReplyMessageController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<IntResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.WxReplyAuto, MenuUtils.SitePermissions.WxReplyBeAdded))
            {
                return Unauthorized();
            }

            WxReplyMessage message = null;
            var account = await _wxAccountRepository.GetBySiteIdAsync(request.SiteId);
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

            int messageId;
            if (message == null)
            {
                message = new WxReplyMessage
                {
                    SiteId = request.SiteId,
                    RuleId = 0,
                    MaterialType = request.MaterialType,
                    MaterialId = request.MaterialId,
                    Text = request.Text
                };
                messageId = await _wxReplyMessageRepository.InsertAsync(message);

                if (StringUtils.EqualsIgnoreCase(request.ActiveName, "replyAuto"))
                {
                    account.MpReplyAutoMessageId = messageId;
                }
                else if (StringUtils.EqualsIgnoreCase(request.ActiveName, "replyBeAdded"))
                {
                    account.MpReplyBeAddedMessageId = messageId;
                }

                await _wxAccountRepository.SetAsync(account);
            }
            else
            {
                messageId = message.Id;
                message.MaterialType = request.MaterialType;
                message.MaterialId = request.MaterialId;
                message.Text = request.Text;
                await _wxReplyMessageRepository.UpdateAsync(message);
            }

            return new IntResult
            {
                Value = messageId
            };
        }
    }
}
