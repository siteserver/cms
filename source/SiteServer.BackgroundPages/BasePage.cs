using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Permissions;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Share;

namespace SiteServer.BackgroundPages
{
    public class BasePage : Page
    {
        public Literal ltlBreadCrumb;
        public Message messageCtrl;

        private MessageUtils.Message.EMessageType _messageType;
        private string _message = string.Empty;
        private string _scripts = string.Empty;

        protected virtual bool IsAccessable => false;

        protected virtual bool IsSinglePage => false;

        protected bool IsForbidden { get; private set; }

        public RequestBody Body { get; private set; }

        private void SetMessage(MessageUtils.Message.EMessageType messageType, Exception ex, string message)
        {
            _messageType = messageType;
            _message = ex != null ? $"{message}<!-- {ex} -->" : message;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Body = new RequestBody();

            if (!IsAccessable && !Body.IsAdministratorLoggin)
            {
                IsForbidden = true;
                PageUtils.RedirectToLoginPage();
            }

            //防止csrf攻击
            Response.AddHeader("X-Frame-Options", "SAMEORIGIN");
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(_message))
            {
                if (messageCtrl != null)
                {
                    messageCtrl.IsShowImmidiatary = true;
                    messageCtrl.MessageType = _messageType;
                    messageCtrl.Content = _message;
                }
                else
                {
                    MessageUtils.SaveMessage(_messageType, _message);
                }
            }

            base.Render(writer);

            if (!IsAccessable && !IsSinglePage)
            {
                writer.Write($@"<script type=""text/javascript"">
if (window.top.location.href.toLowerCase().indexOf(""main.aspx"") == -1){{
	window.top.location.href = ""{PageInitialization.GetRedirectUrl()}"";
}}
</script>");
            }

            if (!string.IsNullOrEmpty(_scripts))
            {
                writer.Write(@"<script type=""text/javascript"">{0}</script>", _scripts);
            }
        }

        public void AddScript(string script)
        {
            _scripts += script;
        }

        public void AddWaitAndRedirectScript(string redirectUrl)
        {
            _scripts += $@"
setTimeout(function() {{
    location.href = '{redirectUrl}';
}}, 1500);
$('.operation-area').hide();
";
        }

        public void AddWaitAndScript(string scripts)
        {
            _scripts += $@"
setTimeout(function() {{
    {scripts}
}}, 1500);
$('.operation-area').hide();
";
        }

        public void FailMessage(Exception ex, string message)
        {
            SetMessage(MessageUtils.Message.EMessageType.Error, ex, message);
        }

        public void FailMessage(string message)
        {
            SetMessage(MessageUtils.Message.EMessageType.Error, null, message);
        }

        public void SuccessMessage(string message)
        {
            SetMessage(MessageUtils.Message.EMessageType.Success, null, message);
        }

        public void SuccessMessage()
        {
            SuccessMessage("操作成功！");
        }

        public void InfoMessage(string message)
        {
            SetMessage(MessageUtils.Message.EMessageType.Info, null, message);
        }

        public void SuccessDeleteMessage()
        {
            SuccessMessage(MessageUtils.DeleteSuccess);
        }

        public void SuccessUpdateMessage()
        {
            SuccessMessage(MessageUtils.UpdateSuccess);
        }

        public void SuccessCheckMessage()
        {
            SuccessMessage(MessageUtils.CheckSuccess);
        }

        public void SuccessInsertMessage()
        {
            SuccessMessage(MessageUtils.InsertSuccess);
        }

        public void FailInsertMessage(Exception ex)
        {
            FailMessage(ex, MessageUtils.InsertFail);
        }

        public void FailUpdateMessage(Exception ex)
        {
            FailMessage(ex, MessageUtils.UpdateFail);
        }

        public void FailDeleteMessage(Exception ex)
        {
            FailMessage(ex, MessageUtils.DeleteFail);
        }

        public void FailCheckMessage(Exception ex)
        {
            FailMessage(ex, MessageUtils.CheckFail);
        }

        public string MaxLengthText(string str, int length)
        {
            return StringUtils.MaxLengthText(str, length);
        }

        public Control FindControlBySelfAndChildren(string controlId)
        {
            return ControlUtils.FindControlBySelfAndChildren(controlId, this);
        }

        public virtual void BreadCrumb(string leftMenuId, string pageTitle, string permission)
        {

        }

        public void BreadCrumbSys(string leftMenuId, string pageTitle, string permission)
        {
            if (ltlBreadCrumb != null)
            {
                var pageUrl = PathUtils.GetFileName(Request.FilePath);
                var topTitle = AppManager.GetTopMenuName(AppManager.IdSys);
                var leftTitle = AppManager.GetLeftMenuName(leftMenuId);
                ltlBreadCrumb.Text = StringUtils.GetBreadCrumbHtml(AppManager.IdSys, topTitle, leftMenuId, leftTitle, String.Empty, string.Empty, pageUrl, pageTitle, string.Empty);
            }

            if (!string.IsNullOrEmpty(permission))
            {
                AdminManager.VerifyAdministratorPermissions(Body.AdministratorName, permission);
            }
        }

        public void BreadCrumbAdmin(string leftMenuId, string pageTitle, string permission)
        {
            if (ltlBreadCrumb != null)
            {
                var pageUrl = PathUtils.GetFileName(Request.FilePath);
                var topTitle = AppManager.GetTopMenuName(AppManager.IdAdmin);
                var leftTitle = AppManager.GetLeftMenuName(leftMenuId);
                ltlBreadCrumb.Text = StringUtils.GetBreadCrumbHtml(AppManager.IdAdmin, topTitle, leftMenuId, leftTitle, string.Empty, string.Empty, pageUrl, pageTitle, string.Empty);
            }

            if (!string.IsNullOrEmpty(permission))
            {
                AdminManager.VerifyAdministratorPermissions(Body.AdministratorName, permission);
            }
        }

        public void BreadCrumbUser(string leftMenuId, string pageTitle, string permission)
        {
            if (ltlBreadCrumb != null)
            {
                var pageUrl = PathUtils.GetFileName(Request.FilePath);
                var topTitle = AppManager.GetTopMenuName(AppManager.IdUser);
                var leftTitle = AppManager.GetLeftMenuName(leftMenuId);
                ltlBreadCrumb.Text = StringUtils.GetBreadCrumbHtml(AppManager.IdUser, topTitle, leftMenuId, leftTitle, string.Empty, string.Empty, pageUrl, pageTitle, string.Empty);
            }

            if (!string.IsNullOrEmpty(permission))
            {
                AdminManager.VerifyAdministratorPermissions(Body.AdministratorName, permission);
            }
        }

        public void BreadCrumbAnalysis(string leftMenuId, string pageTitle, string permission)
        {
            if (ltlBreadCrumb != null)
            {
                var pageUrl = PathUtils.GetFileName(Request.FilePath);
                var topTitle = AppManager.GetTopMenuName(AppManager.IdAnalysis);
                var leftTitle = AppManager.GetLeftMenuName(leftMenuId);
                ltlBreadCrumb.Text = StringUtils.GetBreadCrumbHtml(AppManager.IdAnalysis, topTitle, leftMenuId, leftTitle, string.Empty, string.Empty, pageUrl, pageTitle, string.Empty);
            }

            if (!string.IsNullOrEmpty(permission))
            {
                AdminManager.VerifyAdministratorPermissions(Body.AdministratorName, permission);
            }
        }

        public void BreadCrumbSettings(string leftMenuId, string pageTitle, string permission)
        {
            if (ltlBreadCrumb != null)
            {
                var pageUrl = PathUtils.GetFileName(Request.FilePath);
                var topTitle = AppManager.GetTopMenuName(AppManager.IdSettings);
                var leftTitle = AppManager.GetLeftMenuName(leftMenuId);
                ltlBreadCrumb.Text = StringUtils.GetBreadCrumbHtml(AppManager.IdSettings, topTitle, leftMenuId, leftTitle, string.Empty, string.Empty, pageUrl, pageTitle, string.Empty);
            }

            if (!string.IsNullOrEmpty(permission))
            {
                AdminManager.VerifyAdministratorPermissions(Body.AdministratorName, permission);
            }
        }

        public void BreadCrumbService(string leftMenuId, string pageTitle, string permission)
        {
            if (ltlBreadCrumb != null)
            {
                var pageUrl = PathUtils.GetFileName(Request.FilePath);
                var topTitle = AppManager.GetTopMenuName(AppManager.IdService);
                var leftTitle = AppManager.GetLeftMenuName(leftMenuId);
                ltlBreadCrumb.Text = StringUtils.GetBreadCrumbHtml(AppManager.IdService, topTitle, leftMenuId, leftTitle, string.Empty, string.Empty, pageUrl, pageTitle, string.Empty);
            }

            if (!string.IsNullOrEmpty(permission))
            {
                AdminManager.VerifyAdministratorPermissions(Body.AdministratorName, permission);
            }
        }

        public virtual void Submit_OnClick(object sender, EventArgs e)
        {
            PageUtils.CloseModalPage(Page);
        }

        public static string GetShowHintScript()
        {
            return GetShowHintScript("操作进行中");
        }

        public static string GetShowHintScript(string message)
        {
            return GetShowHintScript(message, 120);
        }

        public static string GetShowHintScript(string message, int top)
        {
            return $@"hideBoxAndShowHint(this, '{message}, 请稍候...', {top});";
        }

        public void ClientScriptRegisterClientScriptBlock(string key, string script)
        {
            if (!ClientScript.IsStartupScriptRegistered(key))
            {
                ClientScript.RegisterClientScriptBlock(GetType(), key, script);
            }
        }

        public void ClientScriptRegisterStartupScript(string key, string script)
        {
            if (!ClientScript.IsStartupScriptRegistered(key))
            {
                ClientScript.RegisterStartupScript(GetType(), key, script);
            }
        }

        public bool ClientScriptIsStartupScriptRegistered(string key)
        {
            return ClientScript.IsStartupScriptRegistered(key);
        }

        public static string GetShowImageScript(string imageClientId, string publishmentSystemUrl)
        {
            return GetShowImageScript("this", imageClientId, publishmentSystemUrl);
        }

        public static string GetShowImageScript(string objString, string imageClientId, string publishmentSystemUrl)
        {
            return
                $"showImage({objString}, '{imageClientId}', '{WebConfigUtils.ApplicationPath}', '{publishmentSystemUrl}')";
        }

        public static List<Analytics> ParseJsonStringToName(string json)
        {
            var analytics = new List<Analytics>();
            if (json.IndexOf("count", StringComparison.Ordinal) != -1)
            {
                json = json.Substring(json.IndexOf("count", StringComparison.Ordinal));
                json = json.TrimEnd('}');
                json = json.TrimEnd(']');
                json = json.TrimEnd('}');
                json = json.Replace("}", "");
                json = json.Replace("{", "");
                json = json.Replace("\"", "");
                var arr = json.Split(',');
                var a = new Analytics[arr.Length / 2];
                for (var i = 0; i < arr.Length / 2; i++)
                {
                    a[i] = new Analytics();
                }
                for (var i = 0; i < arr.Length; i++)
                {
                    var arr1 = arr[i].Split(':');
                    var j = i / 2;

                    if (arr1[0] == "count")
                    {
                        a[j].Count = Convert.ToInt32((arr1[1]));

                    }
                    if (arr1[0] == "metric")
                    {
                        a[j].Metric = arr1[1];
                    }
                    if (i % 2 == 1)
                    {
                        analytics.Add(a[i / 2]);
                    }
                }
            }
            return analytics;
        }
    }
}
