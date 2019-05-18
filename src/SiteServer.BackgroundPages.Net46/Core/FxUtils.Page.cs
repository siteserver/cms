using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Core
{
    public static partial class FxUtils
    {
        public static class Page
        {
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

            public static void RedirectToErrorPage(int logId)
            {
                Redirect(GetErrorPageUrl(logId));
            }

            public static void RedirectToErrorPage(string message)
            {
                Redirect(GetErrorPageUrl(message));
            }

            public static string GetErrorPageUrl(int logId)
            {
                return PageUtilsEx.GetAdminUrl($"pageError.html?logId={logId}");
            }

            public static string GetErrorPageUrl(string message)
            {
                return PageUtilsEx.GetAdminUrl($"pageError.html?message={HttpUtility.UrlPathEncode(message)}");
            }

            public static string GetLoginUrl()
            {
                return PageUtilsEx.GetAdminUrl("pageLogin.cshtml");
            }

            public static void RedirectToLoginPage()
            {
                Redirect(GetLoginUrl());
            }

            public static void AddErrorLogAndRedirect(Exception ex, string summary = "")
            {
                if (ex == null || ex.StackTrace.Contains("System.Web.HttpResponse.set_StatusCode(Int32 value)")) return;

                var logId = LogUtils.AddErrorLog(ex, summary);
                if (logId > 0)
                {
                    RedirectToErrorPage(logId);
                }
                else
                {
                    RedirectToErrorPage(ex.Message);
                }
            }

            public static void Download(HttpResponse response, string filePath, string fileName)
            {
                var fileType = PathUtils.GetExtension(filePath);
                var fileSystemType = EFileSystemTypeUtils.GetEnumType(fileType);
                response.Buffer = true;
                response.Clear();
                response.ContentType = EFileSystemTypeUtils.GetResponseContentType(fileSystemType);
                response.AddHeader("Content-Disposition", "attachment; filename=" + PageUtils.UrlEncode(fileName));
                response.WriteFile(filePath);
                response.Flush();
                response.End();
            }

            public static void Download(HttpResponse response, string filePath)
            {
                var fileName = PathUtils.GetFileName(filePath);
                Download(response, filePath, fileName);
            }
        }
    }
}