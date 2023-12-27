using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils.Wx;
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

            var message = string.Empty;
            var fromUserName = string.Empty;
            var toUserName = string.Empty;

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

                    // 已关注："<xml><ToUserName><![CDATA[gh_77d340ab6f7c]]></ToUserName>\n<FromUserName><![CDATA[o4biGjinUO9MV9KVUD4LFkqOcXx0]]></FromUserName>\n<CreateTime>1661216039</CreateTime>\n<MsgType><![CDATA[event]]></MsgType>\n<Event><![CDATA[SCAN]]></Event>\n<EventKey><![CDATA[login]]></EventKey>\n<Ticket><![CDATA[gQGd7zwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyZWR5WEk2eFVhNmUxNnp2ZGh6YzgAAgQjJQRjAwSAOgkA]]></Ticket>\n</xml>"
                    if (theEvent == "SCAN")
                    {
                        message = "恭喜您，成功登录！";
                        var guid = eventKey;
                        _cacheManager.AddOrUpdateAbsolute(guid, fromUserName, 30);
                    }
                    //首次关注："<xml><ToUserName><![CDATA[gh_77d340ab6f7c]]></ToUserName>\n<FromUserName><![CDATA[o4biGjinUO9MV9KVUD4LFkqOcXx0]]></FromUserName>\n<CreateTime>1661216958</CreateTime>\n<MsgType><![CDATA[event]]></MsgType>\n<Event><![CDATA[subscribe]]></Event>\n<EventKey><![CDATA[qrscene_login]]></EventKey>\n<Ticket><![CDATA[gQF08DwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyd1lBMUlZeFVhNmUxNFJ6ZE56Y0EAAgS1KARjAwSAOgkA]]></Ticket>\n</xml>"
                    else if (theEvent == "subscribe" && StringUtils.StartsWith(eventKey, "qrscene_"))
                    {
                        if (account.MpReplyBeAddedMessageId > 0)
                        {
                            var mpMessage = await _wxManager.GetMessageAsync(siteId, account.MpReplyBeAddedMessageId);
                            if (mpMessage != null)
                            {
                                message = mpMessage.Text;
                                var guid = StringUtils.ReplaceStartsWith(eventKey, "qrscene_", string.Empty);
                                _cacheManager.AddOrUpdateAbsolute(guid, fromUserName, 30);
                            }
                        }
                        
                        // message = @"Hi，终于等到您，超过 50 万网站的选择，欢迎来到SSCMS大家庭！";
                        // var guid = StringUtils.ReplaceStartsWith(eventKey, "qrscene_", string.Empty);
                        // _cacheManager.AddOrUpdateAbsolute(guid, fromUserName, 30);
                    }
                    else if (theEvent == "subscribe")
                    {
                        message = @"Hi，终于等到您，超过 50 万网站的选择，欢迎来到SSCMS大家庭！
想要了解更多，马上登录 SSCMS 官网：https://sscms.com 进行体验吧!";
                    }
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                var start = new DateTime(1970, 1, 1);
                var createTime = (long)(DateTime.Now - start).TotalSeconds;
                var sRespData = $"<xml><ToUserName><![CDATA[{fromUserName}]]></ToUserName><FromUserName><![CDATA[{toUserName}]]></FromUserName><CreateTime>{createTime}</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[{message}]]></Content><MsgId>1234567890123456</MsgId></xml>";
                var sEncryptMsg = "";
                ret = wxcpt.EncryptMsg(sRespData, timestamp, nonce, ref sEncryptMsg);
                if (ret != 0 || string.IsNullOrEmpty(sEncryptMsg))
                {
                    await _errorLogRepository.AddErrorLogAsync(new Exception(sRespData), "WXBizMsgCrypt.EncryptMsg");
                    message = string.Empty;
                }
                else
                {
                    message = sEncryptMsg;
                }
            }

            return message;
        }
    }
}
