using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSmsProvider : BasePage
    {
        public Repeater RptContents;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageSmsProvider), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("isEnabled") && Body.IsQueryExists("providerType"))
            {
                var providerType = ESmsProviderTypeUtils.GetEnumType(Body.GetQueryString("providerType"));

                if (providerType == ESmsProviderType.AliDaYu)
                {
                    ConfigManager.SystemConfigInfo.IsSmsAliDaYu = !ConfigManager.SystemConfigInfo.IsSmsAliDaYu;
                }
                else if (providerType == ESmsProviderType.YunPian)
                {
                    ConfigManager.SystemConfigInfo.IsSmsYunPian = !ConfigManager.SystemConfigInfo.IsSmsYunPian;
                }

                BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);
            }

            if (!IsPostBack)
            {
                BreadCrumbSettings(AppManager.Settings.LeftMenu.Config, "短信服务商管理", AppManager.Settings.Permission.SettingsConfig);

                RptContents.DataSource = ESmsProviderTypeUtils.GetList();
                RptContents.ItemDataBound += rptInstalled_ItemDataBound;
                RptContents.DataBind();
            }
        }

        private void rptInstalled_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var providerType = (ESmsProviderType)e.Item.DataItem;

                var isEnabled = false;
                if (providerType == ESmsProviderType.AliDaYu)
                {
                    isEnabled = ConfigManager.SystemConfigInfo.IsSmsAliDaYu;
                }
                else if (providerType == ESmsProviderType.YunPian)
                {
                    isEnabled = ConfigManager.SystemConfigInfo.IsSmsYunPian;
                }

                var ltlName = e.Item.FindControl("ltlName") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlTemplates = e.Item.FindControl("ltlTemplates") as Literal;
                var ltlConfigUrl = e.Item.FindControl("ltlConfigUrl") as Literal;
                var ltlIsEnabledUrl = e.Item.FindControl("ltlIsEnabledUrl") as Literal;

                if (ltlName != null) ltlName.Text = $@"{ESmsProviderTypeUtils.GetText(providerType)}(<a href=""{ESmsProviderTypeUtils.GetUrl(providerType)}"" target=""_blank"">{ESmsProviderTypeUtils.GetUrl(providerType)}</a>)";
                if (ltlIsEnabled != null) ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(isEnabled);

                if (isEnabled)
                {
                    var urlConfig = string.Empty;
                    var isConfig = false;
                    var isTemplate = false;
                    if (providerType == ESmsProviderType.AliDaYu)
                    {
                        urlConfig = PageSmsProviderAliDaYu.GetRedirectUrl();
                        isConfig = !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsAliDaYuAppKey) &&
                                   !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsAliDaYuAppSecret) &&
                                   !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsAliDaYuSignName);
                        isTemplate = !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsAliDaYuCodeTplId);
                    }
                    else if (providerType == ESmsProviderType.YunPian)
                    {
                        urlConfig = PageSmsProviderYunPian.GetRedirectUrl();
                        isConfig = !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsYunPianApiKey);
                        isTemplate = !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsYunPianCodeTplId);
                    }
                    if (ltlConfigUrl != null)
                    {
                        ltlConfigUrl.Text = isConfig ? $@"<a href=""{urlConfig}"">设置</a>" : $@"<a href=""{urlConfig}"" style=""color:red"">设置</a>";
                    }
                    if (ltlTemplates != null)
                    {
                        ltlTemplates.Text = isTemplate ? $@"<a href=""{PageSmsTemplates.GetRedirectUrl(providerType)}"">短信模板管理</a>" : $@"<a href=""{PageSmsTemplates.GetRedirectUrl(providerType)}"" style=""color:red"">短信模板管理</a>";
                    }
                }

                var urlIsEnabled = GetRedirectUrl() + $"?isEnabled=True&providerType={ESmsProviderTypeUtils.GetValue(providerType)}";
                var action = isEnabled ? "禁用" : "启用";
                if (ltlIsEnabledUrl != null) ltlIsEnabledUrl.Text = $@"<a href=""{urlIsEnabled}"">{action}</a>";
            }
        }
    }
}



