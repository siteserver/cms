using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
	public class PagePublishmentSystemUrlApi : BasePage
    {
        public DropDownList DdlIsSeparatedApi;
        public PlaceHolder PhSeparatedApi;
        public TextBox TbSeparatedApiUrl;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.SiteManagement);

            EBooleanUtils.AddListItems(DdlIsSeparatedApi, "API独立部署", "API与CMS部署在一起");
            ControlUtils.SelectSingleItem(DdlIsSeparatedApi, ConfigManager.SystemConfigInfo.IsSeparatedApi.ToString());
            PhSeparatedApi.Visible = ConfigManager.SystemConfigInfo.IsSeparatedApi;
            TbSeparatedApiUrl.Text = ConfigManager.SystemConfigInfo.SeparatedApiUrl;
        }

        public void DdlIsSeparatedApi_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedApi.Visible = TranslateUtils.ToBool(DdlIsSeparatedApi.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            ConfigManager.SystemConfigInfo.IsSeparatedApi = TranslateUtils.ToBool(DdlIsSeparatedApi.SelectedValue);
            ConfigManager.SystemConfigInfo.SeparatedApiUrl = TbSeparatedApiUrl.Text;

            BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

            Body.AddAdminLog("修改API访问地址");
            SuccessUpdateMessage();
        }
    }
}
