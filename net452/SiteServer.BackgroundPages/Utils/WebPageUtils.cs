using System;
using System.Web;
using System.Web.UI;
using SiteServer.CMS.Core;
using SiteServer.CMS.Fx;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Utils
{
    public static class WebPageUtils
    {
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

        public static void Redirect(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;
            var response = HttpContext.Current.Response;
            response.Clear();//这里是关键，清除在返回前已经设置好的标头信息，这样后面的跳转才不会报错
            response.BufferOutput = true;//设置输出缓冲
            if (!response.IsRequestBeingRedirected) //在跳转之前做判断,防止重复
            {
                response.Redirect(url, true);
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
            return $"{AdminPagesUtils.ErrorUrl}?logId={logId}";
        }

        public static string GetErrorPageUrl(string message)
        {
            return $"{AdminPagesUtils.ErrorUrl}?message={PageUtils.UrlEncode(message)}";
        }

        public static void RedirectToLoginPage()
        {
            Redirect(AdminPagesUtils.LoginUrl);
        }

        public static void CheckRequestParameter(params string[] parameters)
        {
            foreach (var parameter in parameters)
            {
                if (!string.IsNullOrEmpty(parameter) && HttpContext.Current.Request.QueryString[parameter] == null)
                {
                    Redirect(GetErrorPageUrl(PageErrorParameterIsNotCorrect));
                    return;
                }
            }
        }

        public static void SaveMessage(Message.EMessageType messageType, string message)
        {
            CookieUtils.SetCookie(Message.GetCookieName(messageType), message, TimeSpan.FromDays(1));
        }

        private static string DecodeMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                //message = StringUtils.HtmlDecode(message);
                message = message.Replace("''", "\"");
            }
            return message;
        }

        public static string GetMessageHtml(Message.EMessageType messageType, string message, Control control)
        {
            var messageHtml = string.Empty;
            message = DecodeMessage(message);
            if (!string.IsNullOrEmpty(message))
            {
                if (messageType == Message.EMessageType.Success)
                {
                    messageHtml = $@"<div class=""msg_succeed"">{message}</div>";
                }
                else if (messageType == Message.EMessageType.Error)
                {
                    messageHtml = $@"<div class=""msg_failed"">{message}</div>";
                }
                else if (messageType == Message.EMessageType.Info)
                {
                    messageHtml = $@"<div class=""msg_info"">{message}</div>";
                }
            }
            return messageHtml;
        }

        public static string GetMessageHtml(Control control)
        {
            var messageType = Message.EMessageType.None;
            var message = string.Empty;
            if (CookieUtils.IsExists(Message.GetCookieName(Message.EMessageType.Success)))
            {
                messageType = Message.EMessageType.Success;
                message = CookieUtils.GetCookie(Message.GetCookieName(Message.EMessageType.Success));
                CookieUtils.Erase(Message.GetCookieName(Message.EMessageType.Success));
            }
            else if (CookieUtils.IsExists(Message.GetCookieName(Message.EMessageType.Error)))
            {
                messageType = Message.EMessageType.Error;
                message = CookieUtils.GetCookie(Message.GetCookieName(Message.EMessageType.Error));
                CookieUtils.Erase(Message.GetCookieName(Message.EMessageType.Error));
            }
            else if (CookieUtils.IsExists(Message.GetCookieName(Message.EMessageType.Info)))
            {
                messageType = Message.EMessageType.Info;
                message = CookieUtils.GetCookie(Message.GetCookieName(Message.EMessageType.Info));
                CookieUtils.Erase(Message.GetCookieName(Message.EMessageType.Info));
            }
            return GetMessageHtml(messageType, message, control);
        }

        public static string GetAlertHtml(Message.EMessageType messageType, string message, Control control)
        {
            var messageHtml = string.Empty;
            message = DecodeMessage(message);
            if (messageType == Message.EMessageType.Success)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    messageHtml = $@"
<div class=""alert alert-success"">
    <button type=""button"" class=""close"" data-dismiss=""alert"">&times;</button>
  <strong>成功!</strong>&nbsp;&nbsp; {message}</div>";
                }
            }
            else if (messageType == Message.EMessageType.Error)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    messageHtml = $@"
<div class=""alert alert-danger"">
    <button type=""button"" class=""close"" data-dismiss=""alert"">&times;</button>
  <strong>错误!</strong>&nbsp;&nbsp; {message}</div>";
                }
            }
            else if (messageType == Message.EMessageType.Info)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    messageHtml = $@"
<div class=""alert alert-warning"">
    <button type=""button"" class=""close"" data-dismiss=""alert"">&times;</button>
  <strong>提示!</strong>&nbsp;&nbsp; {message}</div>";
                }
            }
            return messageHtml;
        }

        public static string GetAlertHtml(Control control, string text)
        {
            var messageType = Message.EMessageType.None;
            var message = string.Empty;
            if (CookieUtils.IsExists(Message.GetCookieName(Message.EMessageType.Success)))
            {
                messageType = Message.EMessageType.Success;
                message = CookieUtils.GetCookie(Message.GetCookieName(Message.EMessageType.Success));
                CookieUtils.Erase(Message.GetCookieName(Message.EMessageType.Success));
            }
            else if (CookieUtils.IsExists(Message.GetCookieName(Message.EMessageType.Error)))
            {
                messageType = Message.EMessageType.Error;
                message = CookieUtils.GetCookie(Message.GetCookieName(Message.EMessageType.Error));
                CookieUtils.Erase(Message.GetCookieName(Message.EMessageType.Error));
            }
            else if (CookieUtils.IsExists(Message.GetCookieName(Message.EMessageType.Info)))
            {
                messageType = Message.EMessageType.Info;
                message = CookieUtils.GetCookie(Message.GetCookieName(Message.EMessageType.Info));
                CookieUtils.Erase(Message.GetCookieName(Message.EMessageType.Info));
            }
            else if (!string.IsNullOrEmpty(text))
            {
                messageType = Message.EMessageType.Info;
                message = text;
            }
            return GetAlertHtml(messageType, message, control);
        }

        #region Message
        public class Message
        {
            private const string CookieName = "BaiRong_Message";
            public static string GetCookieName(EMessageType messageType)
            {
                return $"{CookieName}_{EMessageTypeUtils.GetValue(messageType)}";
            }

            public enum EMessageType
            {
                Success,
                Error,
                Info,
                None
            }

            public class EMessageTypeUtils
            {
                public static string GetValue(EMessageType type)
                {
                    if (type == EMessageType.Success)
                    {
                        return "Success";
                    }
                    if (type == EMessageType.Error)
                    {
                        return "Error";
                    }
                    if (type == EMessageType.Info)
                    {
                        return "Info";
                    }
                    if (type == EMessageType.None)
                    {
                        return "None";
                    }
                    throw new Exception();
                }
            }
        }
        #endregion

        #region Constants

        public const string InsertSuccess = "添加成功！";
        public const string UpdateSuccess = "更新成功！";
        public const string DeleteSuccess = "删除成功！";
        public const string CheckSuccess = "审核成功！";
        public const string InsertFail = "添加失败！";
        public const string UpdateFail = "更新失败！";
        public const string DeleteFail = "删除失败！";
        public const string CheckFail = "审核失败！";

        public const string AccountLocked = "登录失败，您的帐户已经被锁定！";
        public const string AccountUnchecked = "登录失败，您的帐户还未被审核！";
        public const string AccountError = "登录失败，请重试！";

        //public const string CheckDenied = "审核不通过";
        //public const string Unchecked = "未审核";
        //public const string CheckedLevel1 = "一级审核通过";
        //public const string CheckedLevel2 = "二级审核通过";
        //public const string CheckedLevel3 = "三级审核通过";
        //public const string CheckedLevel4 = "四级审核通过";
        //public const string CheckedLevel5 = "五级审核通过";


        public const string PageErrorParameterIsNotCorrect = "此页需要正确的参数传输进入！";

        public const string PermissionNotVisible = "对不起，您没有权限浏览此页!";

        #endregion
    }
}
