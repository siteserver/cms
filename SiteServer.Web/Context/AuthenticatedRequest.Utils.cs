using System;
using System.Web;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;

namespace SiteServer.API.Context
{
    public partial class AuthenticatedRequest
    {
        public void Download(HttpResponse response, string filePath, string fileName)
        {
            response.Buffer = true;
            response.Clear();
            response.ContentType = "application/force-download";
            response.AddHeader("Body-Disposition", "attachment; filename=" + PageUtils.UrlEncode(fileName));
            response.WriteFile(filePath);
            response.Flush();
            response.End();
        }

        public void Download(HttpResponse response, string filePath)
        {
            var fileName = PathUtils.GetFileName(filePath);
            Download(response, filePath, fileName);
        }

        public string SessionId
        {
            get
            {
                var sessionId = CookieUtils.GetCookie("SiteServer.SessionID");
                if (!string.IsNullOrEmpty(sessionId)) return sessionId;
                long i = 1;
                foreach (var b in Guid.NewGuid().ToByteArray())
                {
                    i *= b + 1;
                }
                sessionId = $"{i - DateTime.Now.Ticks:x}";
                CookieUtils.SetCookie("SiteServer.SessionID", sessionId, DateTime.Now.AddDays(100));
                return sessionId;
            }
        }
    }
}