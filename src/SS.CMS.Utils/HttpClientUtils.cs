using System;
using System.Net;
using System.Text;

namespace SS.CMS.Utils
{
    public static class HttpClientUtils
    {
        public static string GetRemoteFileSource(string url, Encoding encoding, string cookieString)
        {
            try
            {
                string retval;
                var uri = new Uri(url);
                var hwReq = (HttpWebRequest)WebRequest.Create(uri);
                if (!string.IsNullOrEmpty(cookieString))
                {
                    hwReq.Headers.Add("Cookie", cookieString);
                }
                var hwRes = (HttpWebResponse)hwReq.GetResponse();
                hwReq.Method = "Get";
                //hwReq.ContentType = "text/html";
                hwReq.KeepAlive = false;

                using (var reader = new System.IO.StreamReader(hwRes.GetResponseStream(), encoding))
                {
                    retval = reader.ReadToEnd();
                }

                return retval;
            }
            catch
            {
                throw new Exception($"页面地址“{url}”无法访问！");
            }
        }

        public static string GetRemoteFileSource(string url)
        {
            return GetRemoteFileSource(url, Encoding.UTF8, string.Empty);
        }

        public static bool SaveRemoteFileToLocal(string remoteUrl, string localFileName)
        {
            try
            {
                var myWebClient = new WebClient();
                myWebClient.DownloadFile(remoteUrl, localFileName);
            }
            catch
            {
                throw new Exception($"页面地址“{remoteUrl}”无法访问！");
            }
            return true;
        }
    }
}