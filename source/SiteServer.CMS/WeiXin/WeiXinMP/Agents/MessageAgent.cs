using System;
using System.IO;
using System.Xml.Linq;
using SiteServer.CMS.WeiXin.WeiXinMP.Entities.Request;
using SiteServer.CMS.WeiXin.WeiXinMP.Entities.Response;
using SiteServer.CMS.WeiXin.WeiXinMP.Helpers;
using SiteServer.CMS.WeiXin.WeiXinMP.MessageHandlers;

namespace SiteServer.CMS.WeiXin.WeiXinMP.Agents
{
    /// <summary>
    /// 代理请求
    /// 注意！使用代理必然导致网络访问节点增加，会加重响应延时，
    ///       因此建议准备至少2-3秒的延迟时间的准备，
    ///       如果增加2-3秒后远远超过5秒的微信服务器等待时间，
    ///       需要慎重使用，否则可能导致用户无法收到消息。
    /// 
    /// 此外这个类中的方法也可以用于模拟服务器发送消息到自己的服务器进行测试。
    /// </summary>
    public static class MessageAgent
    {
        /// <summary>
        /// 获取Xml结果。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="stream"></param>
        /// <param name="useSouideaKey">是否使用SouideaKey，如果使用，则token为SouideaKey</param>
        /// <returns></returns>
        public static string RequestXml(this IMessageHandler messageHandler, string url, string token, Stream stream, bool useSouideaKey = false)
        {
            if (messageHandler!=null)
            {
                messageHandler.UsedMessageAgent = true;
            }
            var timestamp = DateTime.Now.Ticks.ToString();
            var nonce = "GodBlessYou";
            var echostr = Guid.NewGuid().ToString("n");
            var signature = CheckSignature.GetSignature(timestamp, nonce, token);
            url +=
                $"{(url.Contains("?") ? "&" : "?")}signature={signature}&timestamp={timestamp}&nonce={nonce}&echostr={echostr}";
            var responseXml = HttpUtility.RequestUtility.HttpPost(url, null, stream);
            return responseXml;
        }

        /// <summary>
        /// 获取Xml结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string RequestXml(this IMessageHandler messageHandler, string url, string token, string xml)
        {
            if (messageHandler != null)
            {
                messageHandler.UsedMessageAgent = true;
            } 
            using (var ms = new MemoryStream())
            {
                //这里用ms模拟Request.InputStream
                using (var sw = new StreamWriter(ms))
                {
                    sw.Write(xml);
                    sw.Flush();
                    sw.BaseStream.Position = 0;
                    return messageHandler.RequestXml(url, token, sw.BaseStream);
                }
            }
        }

        /// <summary>
        /// 对接Souidea（P2P）平台，获取Xml结果，使用SouideaKey对接
        /// SouideaKey的获取方式请看：
        /// </summary>
        /// <param name="souideKey"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string RequestSouideaXml(this IMessageHandler messageHandler, string souideKey, string xml, string souideaDomainName = "www.souidea.com")
        {
            if (messageHandler != null)
            {
                messageHandler.UsedMessageAgent = true;
            } 
            var url = "http://" + souideaDomainName + "/App/Weixin?souideaKey=" + souideKey;//官方地址
            using (var ms = new MemoryStream())
            {
                //这里用ms模拟Request.InputStream
                using (var sw = new StreamWriter(ms))
                {
                    sw.Write(xml);
                    sw.Flush();
                    sw.BaseStream.Position = 0;
                    return messageHandler.RequestXml(url, souideKey, sw.BaseStream);
                }
            }
        }

        /// <summary>
        /// 获取ResponseMessge结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static IResponseMessageBase RequestResponseMessage(this IMessageHandler messageHandler, string url, string token, Stream stream)
        {
            return messageHandler.RequestXml(url, token, stream).CreateResponseMessage();
        }

        /// <summary>
        /// 获取ResponseMessge结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static IResponseMessageBase RequestResponseMessage(this IMessageHandler messageHandler, string url, string token, string xml)
        {
            return messageHandler.RequestXml(url, token, xml).CreateResponseMessage();
        }

        /// <summary>
        /// 获取Souidea开放平台的ResponseMessge结果
        /// </summary>
        /// <param name="souideaKey"></param>
        /// <param name="xml"></param>
        /// <param name="souideaDomainName"></param>
        /// <returns></returns>
        public static IResponseMessageBase RequestSouideaResponseMessage(this IMessageHandler messageHandler,string souideaKey, string xml, string souideaDomainName = "www.souidea.com")
        {
            return messageHandler.RequestSouideaXml(souideaKey, xml, souideaDomainName).CreateResponseMessage();
        }

        /// <summary>
        /// 获取Souidea开放平台的ResponseMessge结果
        /// </summary>
        /// <param name="souideaKey"></param>
        /// <param name="souideaDomainName"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public static IResponseMessageBase RequestSouideaResponseMessage(this IMessageHandler messageHandler, string souideaKey, XDocument document, string souideaDomainName = "www.souidea.com")
        {
            return messageHandler.RequestSouideaXml(souideaKey, document.ToString(), souideaDomainName).CreateResponseMessage();
        }

        /// <summary>
        /// 获取Souidea开放平台的ResponseMessge结果
        /// </summary>
        /// <param name="souideaKey"></param>
        /// <param name="requestMessage"></param>
        /// <param name="souideaDomainName"></param>
        /// <returns></returns>
        public static IResponseMessageBase RequestSouideaResponseMessage(this IMessageHandler messageHandler, string souideaKey, RequestMessageBase requestMessage, string souideaDomainName = "www.souidea.com")
        {
            return messageHandler.RequestSouideaXml(souideaKey, requestMessage.ConvertEntityToXmlString(), souideaDomainName).CreateResponseMessage();
        }
    }
}
