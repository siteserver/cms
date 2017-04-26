using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalSmsTemplateTest : BasePage
    {
        public TextBox TbMobile;
        private ESmsProviderType _providerType;
        private ESmsTemplateType _templateType;

        public static string GetOpenWindowStringToEdit(ESmsProviderType providerType, ESmsTemplateType templateType)
        {
            return PageUtils.GetOpenWindowString("发送测试短信", PageUtils.GetSettingsUrl(nameof(ModalSmsTemplateTest), new NameValueCollection
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
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            var mobile = TbMobile.Text;
            if (StringUtils.IsMobile(mobile))
            {
                var errorMessage = string.Empty;
                var isSuccess = false;
                if (_providerType == ESmsProviderType.AliDaYu)
                {
                    if (_templateType == ESmsTemplateType.Code)
                    {
                        isSuccess = SmsManager.SendCodeByAliDaYu(mobile, 6856, out errorMessage);
                    }
                }
                else if (_providerType == ESmsProviderType.YunPian)
                {
                    if (_templateType == ESmsTemplateType.Code)
                    {
                        isSuccess = SmsManager.SendCodeByYunPian(mobile, 6856, out errorMessage);
                    }
                }

                if (isSuccess)
                {
                    SuccessMessage("短信发送成功！");
                    isChanged = true;
                }
                else
                {
                    FailMessage("短信发送失败：" + errorMessage + "!");
                }
            }

            if (!isChanged) return;

            PageUtils.CloseModalPageAndRedirect(Page, PageSmsTemplates.GetRedirectUrl(_providerType));
        }
	}
}
