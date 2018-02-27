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
                PageUtils.Redirect(PageInstall.GetRedirectUrl(false, _pluginId));
            }
            else if (Body.IsQueryExists("update"))
            {
                PageUtils.Redirect(PageInstall.GetRedirectUrl(true, _pluginId));
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
