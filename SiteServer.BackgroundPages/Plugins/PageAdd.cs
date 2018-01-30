using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Plugins
{
    public class PageAdd : BasePage
    {
        public Button BtnUpload;

        public string AllowNightlyBuild => WebConfigUtils.AllowNightlyBuild.ToString().ToLower();

        public string AllowPrereleaseVersions => WebConfigUtils.AllowPrereleaseVersions.ToString().ToLower();

        public static string GetRedirectUrl()
        {
            return PageUtils.GetPluginsUrl(nameof(PageAdd), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Page.IsPostBack) return;

            VerifyAdministratorPermissions(ConfigManager.Permissions.Plugins.Add);

            BtnUpload.OnClientClick = ModalManualInstall.GetOpenWindowString();
        }
    }
}
