using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.CMS.Plugin;
using SiteServer.Utils.Packaging;

namespace SiteServer.BackgroundPages.Plugins
{
    public class PageView : BasePage
    {
        public Button BtnInstall;
        public PlaceHolder PhSuccess;
        public PlaceHolder PhFailure;
        public Literal LtlErrorMessage;

        private string _pluginId;
        private string _version;

        public static string GetRedirectUrl(string pluginId, string version)
        {
            return PageUtils.GetPluginsUrl(nameof(PageView), new NameValueCollection
            {
                {"pluginId", pluginId},
                {"version", version}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _pluginId = Body.GetQueryString("pluginId");
            _version = Body.GetQueryString("version");

            if (Page.IsPostBack) return;

            VerifyAdministratorPermissions(ConfigManager.Permissions.Plugins.Add, ConfigManager.Permissions.Plugins.Management);

            if (PluginManager.IsExists(_pluginId))
            {
                BtnInstall.Text = "插件已安装";
                BtnInstall.Enabled = false;
            }
        }

        public void BtnInstall_Click(object sender, EventArgs e)
        {
            string errorMessage;

            if (!PackageUtils.InstallPackage(_pluginId, _version, true, out errorMessage))
            {
                PhFailure.Visible = true;
                LtlErrorMessage.Text = errorMessage;
                return;
            }

            if (!PluginManager.Install($"{_pluginId}.{_version}", out errorMessage))
            {
                PhFailure.Visible = true;
                LtlErrorMessage.Text = errorMessage;
                return;
            }

            PhSuccess.Visible = true;
        }

        public void Return_Click(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageAdd.GetRedirectUrl());
        }
    }
}
