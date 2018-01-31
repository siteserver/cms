using System;
using System.Web;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.API
{
    public class ErrorRedirectModule : IHttpModule
    {
        public string ModuleName => "ErrorRedirectModule";

        public void Init(HttpApplication app)
        {
            app.Error += Application_Error;
        }

        private static void Application_Error(object sender, EventArgs e)
        {
            try
            {
                var ex = HttpContext.Current.Server.GetLastError();
                if (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                HttpContext.Current.Server.ClearError();

                if (ex.HResult == -2147467259) // 文件名不存在
                {
                    PageUtils.RedirectToErrorPage(ex.Message);
                }
                else
                {
                    var logId = LogUtils.AddSystemErrorLog(ex, "Application Error");
                    if (logId > 0)
                    {
                        PageUtils.RedirectToErrorPage(logId);
                    }
                    else
                    {
                        PageUtils.RedirectToErrorPage(ex.Message);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        public void Dispose()
        {
        }
    }

}
