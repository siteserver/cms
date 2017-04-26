using System;
using System.Web.UI;

namespace BaiRong.Core
{
    public class MessageUtils
    {
        private MessageUtils()
        {
        }

        public static void SaveMessage(Message.EMessageType messageType, string message)
        {
            CookieUtils.SetCookie(Message.GetCookieName(messageType), message, DateTime.MaxValue);
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
            if (messageType == Message.EMessageType.Success)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    messageHtml = $@"<DIV class=""msg_succeed"">{message}</DIV>";
                }
            }
            else if (messageType == Message.EMessageType.Error)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    messageHtml = $@"<DIV class=""msg_failed"">{message}</DIV>";
                }
            }
            else if (messageType == Message.EMessageType.Info)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    messageHtml = $@"<DIV class=""msg_info"">{message}</DIV>";
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
<div class=""alert alert-error"">
    <button type=""button"" class=""close"" data-dismiss=""alert"">&times;</button>
  <strong>错误!</strong>&nbsp;&nbsp; {message}</div>";
                }
            }
            else if (messageType == Message.EMessageType.Info)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    messageHtml = $@"
<div class=""alert alert-info"">
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
            private const string cookieName = "BaiRong_Message";
            public static string GetCookieName(EMessageType messageType)
            {
                return $"{cookieName}_{EMessageTypeUtils.GetValue(messageType)}";
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
                    else if (type == EMessageType.Error)
                    {
                        return "Error";
                    }
                    else if (type == EMessageType.Info)
                    {
                        return "Info";
                    }
                    else if (type == EMessageType.None)
                    {
                        return "None";
                    }
                    else
                    {
                        throw new Exception();
                    }
                }

                public static EMessageType GetEnumType(string typeStr)
                {
                    var retval = EMessageType.None;

                    if (Equals(EMessageType.Success, typeStr))
                    {
                        retval = EMessageType.Success;
                    }
                    else if (Equals(EMessageType.Error, typeStr))
                    {
                        retval = EMessageType.Error;
                    }
                    else if (Equals(EMessageType.Info, typeStr))
                    {
                        retval = EMessageType.Info;
                    }

                    return retval;
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