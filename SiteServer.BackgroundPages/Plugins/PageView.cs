using System;
using System.Collections.Specialized;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.CMS.Plugin;

namespace SiteServer.BackgroundPages.Plugins
{
    public class PageView : BasePage
    {
        private string _pluginId;
        private string _returnUrl;

        public string Installed => PluginManager.IsExists(_pluginId).ToString().ToLower();

        public string Package => TranslateUtils.JsonSerialize(PluginManager.GetMetadata(_pluginId));

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

            _pluginId = AuthRequest.GetQueryString("pluginId");
            _returnUrl = AuthRequest.GetQueryString("returnUrl");

            if (Page.IsPostBack) return;

            VerifySystemPermissions(ConfigManager.PluginsPermissions.Add, ConfigManager.PluginsPermissions.Management);
        }

        public void Return_Click(object sender, EventArgs e)
        {
            PageUtils.Redirect(string.IsNullOrEmpty(_returnUrl) ? PageAdd.GetRedirectUrl() : _returnUrl);
        }
    }
}
