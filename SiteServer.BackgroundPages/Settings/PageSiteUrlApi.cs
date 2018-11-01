using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteUrlApi : BasePage
    {
        public RadioButtonList RblIsSeparatedApi;
        public PlaceHolder PhSeparatedApi;
        public TextBox TbSeparatedApiUrl;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            EBooleanUtils.AddListItems(RblIsSeparatedApi, "API独立部署", "API与CMS部署在一起");
            ControlUtils.SelectSingleItem(RblIsSeparatedApi, ConfigManager.SystemConfigInfo.IsSeparatedApi.ToString());
            PhSeparatedApi.Visible = ConfigManager.SystemConfigInfo.IsSeparatedApi;
            TbSeparatedApiUrl.Text = ConfigManager.SystemConfigInfo.SeparatedApiUrl;
        }

        public void RblIsSeparatedApi_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedApi.Visible = TranslateUtils.ToBool(RblIsSeparatedApi.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            ConfigManager.SystemConfigInfo.IsSeparatedApi = TranslateUtils.ToBool(RblIsSeparatedApi.SelectedValue);
            ConfigManager.SystemConfigInfo.SeparatedApiUrl = TbSeparatedApiUrl.Text;

            DataProvider.ConfigDao.Update(ConfigManager.Instance);

            AuthRequest.AddAdminLog("修改API访问地址");
            SuccessUpdateMessage();
        }
    }
}
