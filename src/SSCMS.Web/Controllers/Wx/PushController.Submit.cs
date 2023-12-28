using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils.Wx;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

// https://developers.weixin.qq.com/doc/offiaccount/Message_Management/Receiving_event_pushes.html

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
            }

            var message = string.Empty;
            if (replyMessage != null)
            {
                if (replyMessage.MaterialType == MaterialType.Text)
                {
                    message = await _wxManager.ReplyTextAsync(account, fromUserName, toUserName, message, timestamp, nonce);
                }
            }
            return message;

            // if (!string.IsNullOrEmpty(message))
            // {
            //     var start = new DateTime(1970, 1, 1);
            //     var createTime = (long)(DateTime.Now - start).TotalSeconds;
            //     var sRespData = $"<xml><ToUserName><![CDATA[{fromUserName}]]></ToUserName><FromUserName><![CDATA[{toUserName}]]></FromUserName><CreateTime>{createTime}</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[{message}]]></Content><MsgId>1234567890123456</MsgId></xml>";
            //     var sEncryptMsg = "";
            //     ret = wxcpt.EncryptMsg(sRespData, timestamp, nonce, ref sEncryptMsg);
            //     if (ret != 0 || string.IsNullOrEmpty(sEncryptMsg))
            //     {
            //         await _errorLogRepository.AddErrorLogAsync(new Exception(sRespData), "WXBizMsgCrypt.EncryptMsg");
            //         message = string.Empty;
            //     }
            //     else
            //     {
            //         message = sEncryptMsg;
            //     }
            // }

            // return message;
        }
    }
}
