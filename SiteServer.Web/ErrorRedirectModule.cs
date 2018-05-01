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
                var lastError = HttpContext.Current.Server.GetLastError();
                if (lastError != null)
                {
                    var httpError = lastError as HttpException;
                    if (httpError != null)
                    {
                        //ASP.NET的400与404错误不记录日志，并都以自定义404页面响应
                        var httpCode = httpError.GetHttpCode();
                        if (httpCode == 400 || httpCode == 404)
                        {
                            HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                            HttpContext.Current.Response.StatusCode = 404;//在IIS中配置自定义404页面
                            HttpContext.Current.Server.ClearError();
                            return;
                        }
                    }

                    //对于路径错误不记录日志，并都以自定义404页面响应
                    if (lastError.TargetSite.ReflectedType == typeof(System.IO.Path))
                    {
                        HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                        HttpContext.Current.Response.StatusCode = 404;
                        HttpContext.Current.Server.ClearError();
                        return;
                    }

                    if (lastError.InnerException != null)
                    {
                        lastError = lastError.InnerException;
                    }
                    HttpContext.Current.Server.ClearError();

                    LogUtils.AddErrorLogAndRedirect(lastError, "Server Error in Application");
                }


                //var ex = HttpContext.Current.Server.GetLastError();

                //if (ex != null)
                //{
                //    var httpError = ex as HttpException;
                //    if (httpError != null && httpError.ErrorCode == 404)
                //    {
                //        var response = HttpContext.Current.Response;
                //        response.TrySkipIisCustomErrors = true; // For IIS 7 Integrated Pipeline - see previous post
                //        response.Status = "404 Not Found";
                //        response.StatusCode = 404;

                //        HttpContext.Current.Server.ClearError();
                //        return;
                //    }

                //    if (ex.InnerException != null)
                //    {
                //        ex = ex.InnerException;
                //    }
                //    HttpContext.Current.Server.ClearError();

                //    LogUtils.AddErrorLogAndRedirect(ex, "Server Error in Application");
                //}
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
