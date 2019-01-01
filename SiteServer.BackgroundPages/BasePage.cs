using System;
using System.Web.UI;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.BackgroundPages
{
    public class BasePage : Page
    {
        private MessageUtils.Message.EMessageType _messageType;
        private string _message = string.Empty;
        private string _scripts = string.Empty;

        protected virtual bool IsAccessable => false; // 页面默认情况下是不能直接访问

        protected virtual bool IsSinglePage => false; // 是否为单页（即是否需要放在框架页内运行,false表示需要）

        protected virtual bool IsInstallerPage => false; // 是否为系统安装页面

        public string IsNightly => WebConfigUtils.IsNightlyUpdate.ToString().ToLower(); // 系统是否允许升级到最新的开发版本

        public string Version => SystemManager.PluginVersion; // 系统采用的插件API版本号

        protected bool IsForbidden { get; private set; }

        public RequestImpl AuthRequest { get; private set; }

        private void SetMessage(MessageUtils.Message.EMessageType messageType, Exception ex, string message)
        {
            _messageType = messageType; 
            _message = ex != null ? $"{message}<!-- {ex} -->" : message;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AuthRequest = new RequestImpl(Request);

            if (!IsInstallerPage)
            {
                if (string.IsNullOrEmpty(WebConfigUtils.ConnectionString))
                {
                    PageUtils.Redirect(PageUtils.GetAdminUrl("Installer"));
                    return;
                }

                #if !DEBUG
                if (ConfigManager.Instance.IsInitialized && ConfigManager.Instance.DatabaseVersion != SystemManager.Version)
                {
                    PageUtils.Redirect(PageSyncDatabase.GetRedirectUrl());
                    return;
                }
                #endif
            }

            if (!IsAccessable) // 如果页面不能直接访问且又没有登录则直接跳登录页
            {
                if (!AuthRequest.IsAdminLoggin || AuthRequest.AdminInfo == null || AuthRequest.AdminInfo.IsLockedOut) // 检测管理员是否登录，检测管理员帐号是否被锁定
                {
                    IsForbidden = true;
                    PageUtils.RedirectToLoginPage();
                    return;
                }
            }

            //防止csrf攻击
            Response.AddHeader("X-Frame-Options", "SAMEORIGIN");
            //tell Chrome to disable its XSS protection
            Response.AddHeader("X-XSS-Protection", "0");
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(_message))
            {
                MessageUtils.SaveMessage(_messageType, _message);
            }

            base.Render(writer);

            if (!IsAccessable && !IsSinglePage) // 页面不能直接访问且不是单页，需要加一段框架检测代码，检测页面是否运行在框架内
            {
                writer.Write($@"<script type=""text/javascript"">
if (window.top.location.href.toLowerCase().indexOf(""main.cshtml"") == -1){{
	window.top.location.href = ""{PageUtils.GetMainUrl(0)}"";
}}
</script>");
            }

            if (!string.IsNullOrEmpty(_scripts))
            {
                writer.Write($@"<script type=""text/javascript"">{_scripts}</script>");
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
";
        }

        public void AddWaitAndReloadMainPage()
        {
            _scripts += @"
setTimeout(function() {{
    window.top.location.reload(true);
}}, 1500);
";
        }

        public void AddWaitAndScript(string scripts)
        {
            _scripts += $@"
setTimeout(function() {{
    {scripts}
}}, 1500);
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

        public void VerifySystemPermissions(params string[] permissionArray)
        {
            if (AuthRequest.AdminPermissionsImpl.HasSystemPermissions(permissionArray))
            {
                return;
            }
            AuthRequest.AdminLogout();
            PageUtils.Redirect(PageUtils.GetAdminUrl(string.Empty));
        }

        public virtual void Submit_OnClick(object sender, EventArgs e)
        {
            LayerUtils.Close(Page);
        }

        public static string PageLoading()
        {
            return "pageUtils.loading(true);";
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

        public static string GetShowImageScript(string imageClientId, string siteUrl)
        {
            return GetShowImageScript("this", imageClientId, siteUrl);
        }

        public static string GetShowImageScript(string objString, string imageClientId, string siteUrl)
        {
            return
                $"showImage({objString}, '{imageClientId}', '{PageUtils.ApplicationPath}', '{siteUrl}')";
        }
    }
}
