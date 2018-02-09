using System;
using System.Collections.Specialized;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.CMS.Plugin;

namespace SiteServer.BackgroundPages.Plugins
{
    public class PageView : BasePage
    {
        private string _pluginId;
        private string _returnUrl;

        public string Installed => PluginManager.IsExists(_pluginId).ToString().ToLower();

        public string InstalledVersion
        {
            get
            {
                var plugin = PluginManager.GetPlugin(_pluginId);
                return plugin != null ? plugin.Version : string.Empty;
            }
        }

        public static string GetRedirectUrl(string pluginId, string returnUrl)
        {
            return PageUtils.GetPluginsUrl(nameof(PageView), new NameValueCollection
            {
                {"pluginId", pluginId},
                {"returnUrl", returnUrl}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _pluginId = Body.GetQueryString("pluginId");
            _returnUrl = Body.GetQueryString("returnUrl");

            if (Body.IsQueryExists("install"))
            {
                PageUtils.Redirect(PageInstall.GetRedirectUrl(_pluginId));
                //var version = Body.GetQueryString("version");

                //string errorMessage;

                //PackageUtils.DownloadPackage(_pluginId, version);

                //var idWithVersion = $"{_pluginId}.{version}";
                //if (!PackageUtils.UpdatePackage(idWithVersion, false, out errorMessage))
                //{
                //    FailMessage(errorMessage);
                //    return;
                //}

                //PluginManager.ClearCache();
                //Body.AddAdminLog("安装插件", $"插件:{_pluginId}");

                //AddScript(AlertUtils.Success("插件安装成功", "插件安装成功，系统需要重载页面", "重新载入", "window.top.location.reload();"));
            }
            else if (Body.IsQueryExists("update"))
            {
                PageUtils.Redirect(PageUpdate.GetRedirectUrl());
            }

            if (Page.IsPostBack) return;

            VerifyAdministratorPermissions(ConfigManager.Permissions.Plugins.Add, ConfigManager.Permissions.Plugins.Management);
        }

        public void Return_Click(object sender, EventArgs e)
        {
            PageUtils.Redirect(string.IsNullOrEmpty(_returnUrl) ? PageAdd.GetRedirectUrl() : _returnUrl);
        }
    }
}
