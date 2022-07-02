using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SSCMS.Utils
{
    public static class HttpClientUtils
    {
        public static async Task<string> GetStringAsync(string url)
        {
            try
            {
                string html;

                using (var client = new HttpClient())
                {
                    html = await client.GetStringAsync(url);
                }

                // using (var client = new WebClient())
                // {
                //     html = client.DownloadString(url);
                // }

                return html;
            }
            catch
            {
                throw new Exception($"页面地址“{url}”无法访问！");
            }
        }

        public static async Task<bool> DownloadAsync(string remoteUrl, string filePath)
        {
            try
            {
                FileUtils.DeleteFileIfExists(filePath);
                
                using (var client = new HttpClient())
                {
                    using (var stream = await client.GetStreamAsync(remoteUrl))
                    {
                        using (var fs = new FileStream(filePath, FileMode.CreateNew))
                        {
                            await stream.CopyToAsync(fs);
                        }
                    }
                }

                // using var client = new WebClient();
                // client.DownloadFile(remoteUrl, filePath);
            }
            catch
            {
                throw new Exception($"页面地址“{remoteUrl}”无法访问！");
            }
            return true;
        }
    }
}
