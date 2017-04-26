using System;
using System.IO;
using System.Text;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.WeiXinMP.Entities.JsonResult;
using SiteServer.CMS.WeiXin.WeiXinMP.Helpers;
using SiteServer.CMS.WeiXin.WeiXinMP.HttpUtility;

namespace SiteServer.CMS.WeiXin.WeiXinMP.CommonAPIs
{
    public enum CommonJsonSendType
    {
        GET,
        POST
    }

    public static class CommonJsonSend
    {
        /// <summary>
        /// 向需要AccessToken的API发送消息的公共方法
        /// </summary>
        /// <param name="accessToken">这里的AccessToken是通用接口的AccessToken，非OAuth的。如果不需要，可以为null，此时urlFormat不要提供{0}参数</param>
        /// <param name="urlFormat"></param>
        /// <param name="data">如果是Get方式，可以为null</param>
        /// <returns></returns>
        public static WxJsonResult Send(string accessToken, string urlFormat, object data, CommonJsonSendType sendType = CommonJsonSendType.POST)
        {
            return Send<WxJsonResult>(accessToken, urlFormat, data, sendType);
        }

        /// <summary>
        /// 向需要AccessToken的API发送消息的公共方法
        /// </summary>
        /// <param name="accessToken">这里的AccessToken是通用接口的AccessToken，非OAuth的。如果不需要，可以为null，此时urlFormat不要提供{0}参数</param>
        /// <param name="urlFormat"></param>
        /// <param name="data">如果是Get方式，可以为null</param>
        /// <returns></returns>
        public static T Send<T>(string accessToken, string urlFormat, object data, CommonJsonSendType sendType = CommonJsonSendType.POST)
        {
            var jsonString = String.Empty;
            try
            {
                var url = String.IsNullOrEmpty(accessToken) ? urlFormat : String.Format(urlFormat, accessToken);
                switch (sendType)
                {
                    case CommonJsonSendType.GET:
                        return Get.GetJson<T>(url);
                    case CommonJsonSendType.POST:
                        var serializerHelper = new SerializerHelper();
                        jsonString = serializerHelper.GetJsonString(data);
                        using (var ms = new MemoryStream())
                        {
                            var bytes = Encoding.UTF8.GetBytes(jsonString);
                            ms.Write(bytes, 0, bytes.Length);
                            ms.Seek(0, SeekOrigin.Begin);

                            return Post.PostGetJson<T>(url, null, ms);
                        }
                    default:
                        throw new ArgumentOutOfRangeException("sendType");
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex, jsonString);
                throw ex;
            }
        }
    }
}