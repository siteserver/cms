using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSmsProvider : BasePage
    {
        public Literal LtlType;

        public DropDownList DdlProviderType;
        public PlaceHolder PhSettings;
        public TextBox TbAppKey;
        public PlaceHolder PhAdditional;
        public TextBox TbAppSecret;
        public TextBox TbSignName;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageSmsProvider), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbSettings(AppManager.Settings.LeftMenu.Config, "短信服务商设置", AppManager.Settings.Permission.SettingsConfig);

            ESmsProviderTypeUtils.AddListItems(DdlProviderType);
            ControlUtils.SelectListItemsIgnoreCase(DdlProviderType, ESmsProviderTypeUtils.GetValue(ConfigManager.SystemConfigInfo.SmsProviderType));

            TbAppKey.Text = ConfigManager.SystemConfigInfo.SmsAppKey;
            TbAppSecret.Text = ConfigManager.SystemConfigInfo.SmsAppSecret;
            TbSignName.Text = ConfigManager.SystemConfigInfo.SmsSignName;

            DdlProviderType_SelectedIndexChanged(null, EventArgs.Empty);
        }

        public void DdlProviderType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = ESmsProviderTypeUtils.GetEnumType(DdlProviderType.SelectedValue);
            PhSettings.Visible = type != ESmsProviderType.None;
            PhAdditional.Visible = type == ESmsProviderType.Aliyun;

            if (type != ESmsProviderType.None)
            {
                LtlType.Text =
                    $@"{ESmsProviderTypeUtils.GetText(type)}(<a href=""{ESmsProviderTypeUtils.GetUrl(type)}"" target=""_blank"">{ESmsProviderTypeUtils
                        .GetUrl(type)}</a>)";
            }
            else
            {
                LtlType.Text = "请选择短信服务商";
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                ConfigManager.SystemConfigInfo.SmsProviderType =
                    ESmsProviderTypeUtils.GetEnumType(DdlProviderType.SelectedValue);
                ConfigManager.SystemConfigInfo.SmsAppKey = TbAppKey.Text;
                ConfigManager.SystemConfigInfo.SmsAppSecret = TbAppSecret.Text;
                ConfigManager.SystemConfigInfo.SmsSignName = TbSignName.Text;
                BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

                SuccessMessage("短信服务商设置成功！");
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
    }
}
