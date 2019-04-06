using System;
using System.Linq;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Plugins
{
    public class PageAdd : BasePage
    {
        public string PackageIds
        {
            get
            {
                var dict = PluginManager.GetPluginIdAndVersionDict();

                var list = dict.Keys.ToList();

                return TranslateUtils.ObjectCollectionToString(list);
            }
        }

        public static string GetRedirectUrl()
        {
            return PageUtils.GetPluginsUrl(nameof(PageAdd), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Page.IsPostBack) return;

            VerifySystemPermissions(ConfigManager.PluginsPermissions.Add);
        }
    }
}
