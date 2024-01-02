using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils.Wx;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Wx
{
    public partial class PushController
    {
        [HttpPost, Route(Route)]
        public async Task<string> Submit([FromRoute] int siteId, [FromQuery] string signature, [FromQuery] string timestamp, [FromQuery] string nonce)
        {
            var account = await _wxAccountRepository.GetBySiteIdAsync(siteId);

            var wxcpt = new WXBizMsgCrypt(account.MpToken, account.MpEncodingAESKey, account.MpAppId);
            var sReqData = string.Empty;
            using (Stream stream = Request.Body)
            {
                var length = Request.ContentLength.HasValue ? Request.ContentLength.Value : 0;
                byte[] buffer = new byte[length];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                sReqData = Encoding.UTF8.GetString(buffer);
            }

            var sMsg = "";  //解析之后的明文
            var ret = wxcpt.DecryptMsg(signature, timestamp, nonce, sReqData, ref sMsg);
            if (ret != 0 || string.IsNullOrEmpty(sMsg))
            {
                await _errorLogRepository.AddErrorLogAsync(new Exception(sReqData), "WXBizMsgCrypt.DecryptMsg");
                return string.Empty;
            }

            var fromUserName = string.Empty;
            var toUserName = string.Empty;
            WxReplyMessage replyMessage = null;

            var doc = new XmlDocument();
            doc.LoadXml(sMsg);
            var root = doc.FirstChild;

            if (root != null)
            {
                fromUserName = StringUtils.GetInnerText(root["FromUserName"]);
                toUserName = StringUtils.GetInnerText(root["ToUserName"]);
                var msgType = StringUtils.GetInnerText(root["MsgType"]);

                // https://developers.weixin.qq.com/doc/offiaccount/Message_Management/Receiving_event_pushes.html
                if (msgType == "event")
                {
                    var theEvent = StringUtils.GetInnerText(root["Event"]);
                    var eventKey = StringUtils.GetInnerText(root["EventKey"]);

                    // 关注事件
                    if (theEvent == "subscribe")
                    {
                        if (account.MpReplyBeAddedMessageId > 0)
                        {
                            replyMessage = await _wxManager.GetMessageAsync(siteId, account.MpReplyBeAddedMessageId);
                        }
                    }
                    // 点击菜单拉取消息时的事件推送
                    else if (theEvent == "CLICK")
                    {
                        if (account.MpReplyBeAddedMessageId > 0)
                        {
                            replyMessage = await _wxManager.GetMessageAsync(siteId, account.MpReplyBeAddedMessageId);
                        }
                    }
                    // 点击菜单跳转链接时的事件推送
                    else if (theEvent == "VIEW")
                    {
                        if (account.MpReplyBeAddedMessageId > 0)
                        {
                            replyMessage = await _wxManager.GetMessageAsync(siteId, account.MpReplyBeAddedMessageId);
                        }
                    }
                }
                // https://developers.weixin.qq.com/doc/offiaccount/Message_Management/Receiving_standard_messages.html
                else if (msgType == "text")
                {
                    var content = StringUtils.GetInnerText(root["Content"]);

                    var isSession = await _wxChatRepository.UserAdd(new WxChat
                    {
                        SiteId = siteId,
                        OpenId = fromUserName,
                        IsReply = false,
                        Text = content
                    });

                    var textMessages = await _wxManager.GetMessagesAsync(siteId, content, isSession ? 0 : account.MpReplyAutoMessageId);
                    foreach (var textMessage in textMessages)
                    {
                        await _wxManager.CustomSendAsync(account.MpAppId, fromUserName, textMessage);
                    }

                    return "success";
                }
            }

            var message = string.Empty;
            if (replyMessage != null)
            {
                message = await _wxManager.ReplyAsync(account, fromUserName, toUserName, replyMessage, timestamp, nonce);
            }
            return message;
        }
    }
}
