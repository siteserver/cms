using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSmsTemplates : BasePage
    {
        public Repeater RptContents;
        private ESmsProviderType _providerType;

        public static string GetRedirectUrl(ESmsProviderType providerType)
        {
            return PageUtils.GetSettingsUrl(nameof(PageSmsTemplates), new NameValueCollection
            {
                {"providerType", ESmsProviderTypeUtils.GetValue(providerType)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _providerType = ESmsProviderTypeUtils.GetEnumType(Body.GetQueryString("providerType"));

            if (!IsPostBack)
            {
                BreadCrumbSettings(AppManager.Settings.LeftMenu.Config, "短信模板管理", AppManager.Settings.Permission.SettingsConfig);

                RptContents.DataSource = ESmsTemplateTypeUtils.GetList();
                RptContents.ItemDataBound += rptInstalled_ItemDataBound;
                RptContents.DataBind();
            }
        }

        private void rptInstalled_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var templateType = (ESmsTemplateType)e.Item.DataItem;
                var tplId = string.Empty;

                if (templateType == ESmsTemplateType.Code)
                {
                    if (_providerType == ESmsProviderType.AliDaYu)
                    {
                        tplId = ConfigManager.SystemConfigInfo.SmsAliDaYuCodeTplId;
                    }
                    else if (_providerType == ESmsProviderType.YunPian)
                    {
                        tplId = ConfigManager.SystemConfigInfo.SmsYunPianCodeTplId;
                    }
                }

                var ltlType = e.Item.FindControl("ltlType") as Literal;
                var ltlTplId = e.Item.FindControl("ltlTplId") as Literal;
                var ltlTest = e.Item.FindControl("ltlTest") as Literal;
                var ltlEdit = e.Item.FindControl("ltlEdit") as Literal;

                if (ltlType != null) ltlType.Text = ESmsTemplateTypeUtils.GetText(templateType);

                //if (ltlName != null) ltlName.Text = $@"{ESmsProviderTypeUtils.GetText(smsProviderType)}(<a href=""{ESmsProviderTypeUtils.GetUrl(smsProviderType)}"" target=""_blank"">{ESmsProviderTypeUtils.GetUrl(smsProviderType)}</a>)";
                //if (ltlIsEnabled != null) ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(isEnabled);

                //var urlIsEnabled = GetRedirectUrl() + $"?isEnabled=True&providerID={providerID}";
                //var action = isEnabled ? "禁用" : "启用";
                //if (ltlIsEnabledUrl != null) ltlIsEnabledUrl.Text = $@"<a href=""{urlIsEnabled}"">{action}</a>";

                if (ltlTplId != null) ltlTplId.Text = tplId;
                if (ltlTest != null)
                {
                    ltlTest.Text = $@"<a href=""javascript:;"" onclick=""{ModalSmsTemplateTest.GetOpenWindowStringToEdit(_providerType, templateType)}"">发送测试短信</a>";
                }
                if (ltlEdit != null)
                {
                    ltlEdit.Text = $@"<a href=""javascript:;"" onclick=""{ModalSmsTemplateAdd.GetOpenWindowStringToEdit(_providerType, templateType)}"">设置</a>";
                }
            }
        }
    }
}

