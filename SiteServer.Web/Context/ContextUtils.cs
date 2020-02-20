using System;
using System.Threading.Tasks;
using System.Web;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Context
{
    public static class ContextUtils
    {
        public static void RedirectToErrorPage(int logId)
        {
            Redirect(GetErrorPageUrl(logId));
        }

        public static void RedirectToErrorPage(string message)
        {
            Redirect(GetErrorPageUrl(message));
        }

        public static string GetAdminUrl(string relatedUrl)
        {
            return PageUtils.Combine(PageUtils.ApplicationPath, WebConfigUtils.AdminDirectory, relatedUrl);
        }

        public static string GetErrorPageUrl(int logId)
        {
            return GetAdminUrl($"error.html?logId={logId}");
        }

        public static string GetErrorPageUrl(string message)
        {
            return GetAdminUrl($"error.html?message={HttpUtility.UrlPathEncode(message)}");
        }

        public static void Redirect(string url)
        {
            var response = HttpContext.Current.Response;
            response.Clear();//这里是关键，清除在返回前已经设置好的标头信息，这样后面的跳转才不会报错
            response.BufferOutput = true;//设置输出缓冲
            if (!response.IsRequestBeingRedirected) //在跳转之前做判断,防止重复
            {
                response.Redirect(url, true);
            }
        }

        public static async Task AddErrorLogAndRedirectAsync(Exception ex, string summary = "")
        {
            if (ex == null || ex.StackTrace.Contains("System.Web.HttpResponse.set_StatusCode(Int32 value)")) return;

            var logId = await DataProvider.ErrorLogRepository.AddErrorLogAsync(ex, summary);
            if (logId > 0)
            {
                RedirectToErrorPage(logId);
            }
            else
            {
                RedirectToErrorPage(ex.Message);
            }
        }
    }
}