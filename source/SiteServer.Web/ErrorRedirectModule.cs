using System;
using System.Web;
using BaiRong.Core;
using BaiRong.Core.Text;

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
                LogUtils.AddErrorLog(ex, "Application Error");
                HttpContext.Current.Server.ClearError();

                PageUtils.RedirectToErrorPage(ex.Message);
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
