using System;
using System.Net;

namespace SSCMS.Core.Utils
{
	public static class WebClientUtils
	{
        public static string GetHtml(string url)
		{
			try
			{
			    string html;

                using (var client = new WebClient())
                {
                    html = client.DownloadString(url);
                }

                return html;
			}
			catch
			{
				throw new Exception($"页面地址“{url}”无法访问！");
			}
		}

        public static bool Download(string remoteUrl, string localFileName)
		{
			try
            {
                using var client = new WebClient();
                client.DownloadFile(remoteUrl, localFileName);
            }
			catch
			{
				throw new Exception($"页面地址“{remoteUrl}”无法访问！");
			}
			return true;
		}
    }
}
