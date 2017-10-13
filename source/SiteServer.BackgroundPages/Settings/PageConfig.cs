using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageConfig : BasePage
    {
        public DropDownList DdlIsSeparatedApi;
        public PlaceHolder PhSeparatedApi;
        public TextBox TbSeparatedApiUrl;

        public DropDownList DdlIsUrlGlobalSetting;
        public PlaceHolder PhSettings;

        public DropDownList DdlIsSeparatedWeb;
        public PlaceHolder PhSeparatedWeb;
        public TextBox TbSeparatedWebUrl;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbSettings("系统设置", AppManager.Permissions.Settings.Config);

            EBooleanUtils.AddListItems(DdlIsUrlGlobalSetting, "设置", "不设置，针对站点单独设置");
            ControlUtils.SelectListItems(DdlIsUrlGlobalSetting, ConfigManager.SystemConfigInfo.IsUrlGlobalSetting.ToString());
            PhSettings.Visible = ConfigManager.SystemConfigInfo.IsUrlGlobalSetting;

            EBooleanUtils.AddListItems(DdlIsSeparatedWeb, "Web独立部署", "Web与CMS部署在一起");
            ControlUtils.SelectListItems(DdlIsSeparatedWeb, ConfigManager.SystemConfigInfo.IsSeparatedWeb.ToString());
            PhSeparatedWeb.Visible = ConfigManager.SystemConfigInfo.IsSeparatedWeb;
            TbSeparatedWebUrl.Text = ConfigManager.SystemConfigInfo.SeparatedWebUrl;

            EBooleanUtils.AddListItems(DdlIsSeparatedApi, "API独立部署", "API与CMS部署在一起");
            ControlUtils.SelectListItems(DdlIsSeparatedApi, ConfigManager.SystemConfigInfo.IsSeparatedApi.ToString());
            PhSeparatedApi.Visible = ConfigManager.SystemConfigInfo.IsSeparatedApi;
            TbSeparatedApiUrl.Text = ConfigManager.SystemConfigInfo.SeparatedApiUrl;
        }

        public void DdlIsUrlGlobalSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSettings.Visible = TranslateUtils.ToBool(DdlIsUrlGlobalSetting.SelectedValue);
        }

        public void DdlIsSeparatedWeb_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedWeb.Visible = TranslateUtils.ToBool(DdlIsSeparatedWeb.SelectedValue);
        }

        public void DdlIsSeparatedApi_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedApi.Visible = TranslateUtils.ToBool(DdlIsSeparatedApi.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                ConfigManager.SystemConfigInfo.IsUrlGlobalSetting = TranslateUtils.ToBool(DdlIsUrlGlobalSetting.SelectedValue);
                ConfigManager.SystemConfigInfo.IsSeparatedWeb = TranslateUtils.ToBool(DdlIsSeparatedWeb.SelectedValue);
                ConfigManager.SystemConfigInfo.SeparatedWebUrl = TbSeparatedWebUrl.Text;
                ConfigManager.SystemConfigInfo.IsSeparatedApi = TranslateUtils.ToBool(DdlIsSeparatedApi.SelectedValue);
                ConfigManager.SystemConfigInfo.SeparatedApiUrl = TbSeparatedApiUrl.Text;

                BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

                Body.AddAdminLog("修改系统设置");
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage($"修改失败：{ex.Message}");
            }
        }
    }
}
