using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.UEditor
{
    /// <summary>
    /// Crawler 的摘要说明
    /// </summary>
    public class CrawlerHandler : Handler
    {
        private string[] Sources;
        private Crawler[] Crawlers;
        public int PublishmentSystemID { get; private set; }
        public CrawlerHandler(HttpContext context, int publishmentSystemID) : base(context) {

            PublishmentSystemID = publishmentSystemID; 
        }

        public override void Process()
        {
            Sources = Request.Form.GetValues("source[]");
            if (Sources == null || Sources.Length == 0)
            {
                WriteJson(new
                {
                    state = "参数错误：没有指定抓取源"
                });
                return;
            }
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(PublishmentSystemID);

            Crawlers = Sources.Select(x => new Crawler(x, publishmentSystemInfo,Server).Fetch()).ToArray();
            WriteJson(new
            {
                state = "SUCCESS",
                list = Crawlers.Select(x => new
                {
                    state = x.State,
                    source = x.SourceUrl,
                    url = x.ServerUrl
                })
            });
        }
    }

    public class Crawler
    {
        public string SourceUrl { get; set; }
        public string ServerUrl { get; set; }
        public string State { get; set; }
        public PublishmentSystemInfo PubSystemInfo { get; private set; }

        private HttpServerUtility Server { get; set; }


        public Crawler(string sourceUrl, PublishmentSystemInfo pubSystemInfo, HttpServerUtility server)
        {
            SourceUrl = sourceUrl;
            PubSystemInfo = pubSystemInfo;
            Server = server;
        }

        public Crawler Fetch()
        {
            if (!IsExternalIPAddress(SourceUrl))
            {
                State = "INVALID_URL";
                return this;
            }

            //格式验证
            string uploadFileName = Path.GetFileName(SourceUrl);
            var currentType = PathUtils.GetExtension(uploadFileName);
            if (!PathUtility.IsUploadExtenstionAllowed(EUploadType.Image, PubSystemInfo, currentType))
            {
                State = "不允许的文件类型";
                return this;
            }

            var request = WebRequest.Create(SourceUrl) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    State = "Url returns " + response.StatusCode + ", " + response.StatusDescription;
                    return this;
                }
                if (response.ContentType.IndexOf("image") == -1)
                {
                    State = "Url is not an image";
                    return this;
                }
                //ServerUrl = PathFormatter.Format(Path.GetFileName(SourceUrl), Config.GetString("catcherPathFormat"));
                //var savePath = Server.MapPath(ServerUrl);
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(PubSystemInfo, EUploadType.Image);
                var localFileName = PathUtility.GetUploadFileName(PubSystemInfo, uploadFileName);
                var savePath = PathUtils.Combine(localDirectoryPath, localFileName); 
                
                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                }
                try
                {
                    var stream = response.GetResponseStream();
                    var reader = new BinaryReader(stream);
                    byte[] bytes;
                    using (var ms = new MemoryStream())
                    {
                        var buffer = new byte[4096];
                        int count;
                        while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            ms.Write(buffer, 0, count);
                        }
                        bytes = ms.ToArray();
                    }
                    File.WriteAllBytes(savePath, bytes);
                    State = "SUCCESS";

                    ServerUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PubSystemInfo, savePath);

                }
                catch (Exception e)
                {
                    State = "抓取错误：" + e.Message;
                }
                return this;
            }
        }

        private bool IsExternalIPAddress(string url)
        {
            var uri = new Uri(url);
            switch (uri.HostNameType)
            {
                case UriHostNameType.Dns:
                    var ipHostEntry = Dns.GetHostEntry(uri.DnsSafeHost);
                    foreach (var ipAddress in ipHostEntry.AddressList)
                    {
                        var ipBytes = ipAddress.GetAddressBytes();
                        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            if (!IsPrivateIP(ipAddress))
                            {
                                return true;
                            }
                        }
                    }
                    break;

                case UriHostNameType.IPv4:
                    return !IsPrivateIP(IPAddress.Parse(uri.DnsSafeHost));
            }
            return false;
        }

        private bool IsPrivateIP(IPAddress myIPAddress)
        {
            if (IPAddress.IsLoopback(myIPAddress)) return true;
            if (myIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                var ipBytes = myIPAddress.GetAddressBytes();
                // 10.0.0.0/24 
                if (ipBytes[0] == 10)
                {
                    return true;
                }
                // 172.16.0.0/16
                else if (ipBytes[0] == 172 && ipBytes[1] == 16)
                {
                    return true;
                }
                // 192.168.0.0/16
                else if (ipBytes[0] == 192 && ipBytes[1] == 168)
                {
                    return true;
                }
                // 169.254.0.0/16
                else if (ipBytes[0] == 169 && ipBytes[1] == 254)
                {
                    return true;
                }
            }
            return false;
        }
    }
}