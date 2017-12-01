using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using SiteServer.CMS.WeiXin.WeiXinMP.Entities.JsonResult;
using SiteServer.CMS.WeiXin.WeiXinMP.Exceptions;

namespace SiteServer.CMS.WeiXin.WeiXinMP.HttpUtility
{
    public static class Get
    {
        public static T GetJson<T>(string url, Encoding encoding = null)
        {
            var returnText = RequestUtility.HttpGet(url, encoding);

            var js = new JavaScriptSerializer();

            if (returnText.Contains("errcode"))
            {
                //可能发生错误
                var errorResult = js.Deserialize<WxJsonResult>(returnText);
                if (errorResult.errcode != ReturnCode.请求成功)
                {
                    //发生错误
                    throw new ErrorJsonResultException(
                        $"微信请求发生错误！错误代码：{(int) errorResult.errcode}，说明：{errorResult.errmsg}",
                                      null, errorResult);
                }
            }

            var result = js.Deserialize<T>(returnText);

            return result;
        }

        public static void Download(string url, Stream stream)
        {
            var wc = new WebClient();
            var data = wc.DownloadData(url);
            foreach (var b in data)
            {
                stream.WriteByte(b);
            }
        }
    }
}
