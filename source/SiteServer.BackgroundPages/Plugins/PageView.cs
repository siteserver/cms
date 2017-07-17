using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Plugin;

namespace SiteServer.BackgroundPages.Plugins
{
    public class PageView : BasePage
    {
        public Button BtnInstall;
        public PlaceHolder PhSuccess;
        public PlaceHolder PhFailure;
        public Literal LtlErrorMessage;

        private string _pluginId;

        public static string GetRedirectUrl(int type)
        {
            return PageUtils.GetPluginsUrl(nameof(PageManagement), new NameValueCollection
            {
                {"type", type.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _pluginId = Body.GetQueryString("pluginId");

            if (Page.IsPostBack) return;

            BreadCrumbPlugins("插件查看", AppManager.Permissions.Plugins.Add);

            if (PluginManager.IsExists(_pluginId))
            {
                BtnInstall.Text = "插件已安装";
                BtnInstall.Enabled = false;
            }
        }

        public void BtnInstall_Click(object sender, EventArgs e)
        {
            string errorMessage;
            var isSuccess = PluginManager.Install(_pluginId, out errorMessage);
            if (isSuccess)
            {
                PhSuccess.Visible = true;
            }
            else
            {
                PhFailure.Visible = true;
                LtlErrorMessage.Text = errorMessage;
            }
        }
    }
}
