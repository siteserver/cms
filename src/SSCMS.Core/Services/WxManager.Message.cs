using System;
using System.Threading.Tasks;
using SSCMS.Core.Utils.Wx;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;

// https://developers.weixin.qq.com/doc/offiaccount/Message_Management/Passive_user_reply_message.html#0

namespace SSCMS.Core.Services
{
    public partial class WxManager : IWxManager
    {
        public async Task<string> ReplyAsync(WxAccount account, string fromUserName, string toUserName, WxReplyMessage replyMessage, string timestamp, string nonce)
        {
            var start = new DateTime(1970, 1, 1);
            var createTime = (long)(DateTime.Now - start).TotalSeconds;
            var material = string.Empty;

            if (replyMessage.MaterialType == MaterialType.Message || replyMessage.MaterialType == MaterialType.Article)
            {
                material = $@"
  <MsgType><![CDATA[news]]></MsgType>
  <ArticleCount>1</ArticleCount>
  <Articles>
    <item>
      <Title><![CDATA[title1]]></Title>
      <Description><![CDATA[description1]]></Description>
      <PicUrl><![CDATA[picurl]]></PicUrl>
      <Url><![CDATA[url]]></Url>
    </item>
  </Articles>
";
            }
            else if (replyMessage.MaterialType == MaterialType.Text)
            {
                material = $@"
    <MsgType><![CDATA[text]]></MsgType>
    <Content><![CDATA[{replyMessage.Text}]]></Content>
";
            }

            var sRespData = $@"
<xml>
    <ToUserName><![CDATA[{fromUserName}]]></ToUserName>
    <FromUserName><![CDATA[{toUserName}]]></FromUserName>
    <CreateTime>{createTime}</CreateTime>
    {material}
</xml>";

            var retVal = string.Empty;
            var sEncryptMsg = "";
            var wxcpt = new WXBizMsgCrypt(account.MpToken, account.MpEncodingAESKey, account.MpAppId);
            var ret = wxcpt.EncryptMsg(sRespData, timestamp, nonce, ref sEncryptMsg);
            if (ret != 0 || string.IsNullOrEmpty(sEncryptMsg))
            {
                await _errorLogRepository.AddErrorLogAsync(new Exception(sRespData), "WXBizMsgCrypt.EncryptMsg");
            }
            else
            {
                retVal = sEncryptMsg;
            }

            return retVal;
        }
    }
}
