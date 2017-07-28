using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalSmsTemplateAdd : BasePage
    {
        public TextBox TbTplId;
	    private ESmsProviderType _providerType;
	    private ESmsTemplateType _templateType;

        public static string GetOpenWindowStringToEdit(ESmsProviderType providerType, ESmsTemplateType templateType)
        {
            return PageUtils.GetOpenWindowString("设置短信模板", PageUtils.GetSettingsUrl(nameof(ModalSmsTemplateAdd), new NameValueCollection
            {
                {"providerType", ESmsProviderTypeUtils.GetValue(providerType)},
                {"templateType", ESmsTemplateTypeUtils.GetValue(templateType)}
            }), 460, 360);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _providerType = ESmsProviderTypeUtils.GetEnumType(Body.GetQueryString("providerType"));
            _templateType = ESmsTemplateTypeUtils.GetEnumType(Body.GetQueryString("templateType"));

			if (!IsPostBack)
			{
                var tplId = string.Empty;

                if (_templateType == ESmsTemplateType.Code)
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

			    TbTplId.Text = tplId;
                InfoMessage(GetSample());
			}
		}

        private string GetSample()
        {
            var str = "";
            if (_providerType == ESmsProviderType.AliDaYu)
            {
                str = "请在短信服务商管理后台添加短信模板，如： <br /><code>您的验证码是${code}。如非本人操作，请忽略本短信</code><br />添加完成后获取模板ID并填写到下方";
            }
            else if (_providerType == ESmsProviderType.YunPian)
            {
                str = "请在短信服务商管理后台添加短信模板，如： <br /><code>您的验证码是#code#。如非本人操作，请忽略本短信</code><br />添加完成后获取模板ID并填写到下方";
            }
            return str;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                var tplId = this.TbTplId.Text;
                if (_templateType == ESmsTemplateType.Code)
                {
                    if (_providerType == ESmsProviderType.AliDaYu)
                    {
                        ConfigManager.SystemConfigInfo.SmsAliDaYuCodeTplId = tplId;
                    }
                    else if (_providerType == ESmsProviderType.YunPian)
                    {
                        ConfigManager.SystemConfigInfo.SmsYunPianCodeTplId = tplId;
                    }
                }
                BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

                Body.AddAdminLog("设置短信模板");

                SuccessMessage("短信模板设置成功！");
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "短信模板设置失败！");
            }

            if (!isChanged) return;

            PageUtils.CloseModalPageAndRedirect(Page, PageSmsTemplates.GetRedirectUrl(_providerType));
        }
	}
}
