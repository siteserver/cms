using System;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils.Wx;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

// https://developers.weixin.qq.com/doc/offiaccount/Message_Management/Passive_user_reply_message.html#0

namespace SSCMS.Core.Services
{
    public partial class WxManager : IWxManager
    {
        public async Task<string> ReplyTextAsync(WxAccount account, string fromUserName, string toUserName, string message, string timestamp, string nonce)
        {
            var replyMessage = string.Empty;

            var start = new DateTime(1970, 1, 1);
            var createTime = (long)(DateTime.Now - start).TotalSeconds;
            var sRespData = $@"
<xml>
    <ToUserName><![CDATA[{fromUserName}]]></ToUserName>
    <FromUserName><![CDATA[{toUserName}]]></FromUserName>
    <CreateTime>{createTime}</CreateTime>
    <MsgType><![CDATA[text]]></MsgType>
    <Content><![CDATA[{message}]]></Content>
</xml>";
            var sEncryptMsg = "";
            var wxcpt = new WXBizMsgCrypt(account.MpToken, account.MpEncodingAESKey, account.MpAppId);
            var ret = wxcpt.EncryptMsg(sRespData, timestamp, nonce, ref sEncryptMsg);
            if (ret != 0 || string.IsNullOrEmpty(sEncryptMsg))
            {
                await _errorLogRepository.AddErrorLogAsync(new Exception(sRespData), "WXBizMsgCrypt.EncryptMsg");
            }
            else
            {
                replyMessage = sEncryptMsg;
            }

            return replyMessage;
        }
    }
}
