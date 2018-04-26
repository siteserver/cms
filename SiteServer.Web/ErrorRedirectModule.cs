using System;
using System.Web;
using SiteServer.CMS.Core;

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

                LogUtils.AddErrorLogAndRedirect(ex, "ÏµÍ³´íÎó");
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
