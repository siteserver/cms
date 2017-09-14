using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageIntegrationPayment : BasePage
    {
        public Literal LtlType;

        public DropDownList DdlProviderType;
        public PlaceHolder PhPingxx;
        public TextBox TbPingxxAppId;
        public TextBox TbPingxxSecretKey;
        public CheckBoxList CblPingxxChannels;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageIntegrationPayment), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbSettings("支付设置", AppManager.Permissions.Settings.Integration);

            EPaymentProviderTypeUtils.AddListItems(DdlProviderType);
            ControlUtils.SelectListItemsIgnoreCase(DdlProviderType, EPaymentProviderTypeUtils.GetValue(ConfigManager.SystemConfigInfo.PaymentProviderType));

            TbPingxxAppId.Text = ConfigManager.SystemConfigInfo.PaymentPingxxAppId;
            TbPingxxSecretKey.Text = ConfigManager.SystemConfigInfo.PaymentPingxxSecretKey;
            EPaymentChannelUtils.AddListItems(CblPingxxChannels);
            ControlUtils.SelectListItems(CblPingxxChannels, ConfigManager.SystemConfigInfo.PaymentChannels.Split(','));

            DdlProviderType_SelectedIndexChanged(null, EventArgs.Empty);
        }

        public void DdlProviderType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = EPaymentProviderTypeUtils.GetEnumType(DdlProviderType.SelectedValue);
            PhPingxx.Visible = type == EPaymentProviderType.Pingxx;

            if (type != EPaymentProviderType.None)
            {
                LtlType.Text =
                    $@"{EPaymentProviderTypeUtils.GetText(type)}(<a href=""{EPaymentProviderTypeUtils.GetUrl(type)}"" target=""_blank"">{EPaymentProviderTypeUtils
                        .GetUrl(type)}</a>)";
            }
            else
            {
                LtlType.Text = "请选择聚合支付服务商";
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                ConfigManager.SystemConfigInfo.PaymentProviderType =
                    EPaymentProviderTypeUtils.GetEnumType(DdlProviderType.SelectedValue);
                ConfigManager.SystemConfigInfo.PaymentPingxxAppId = TbPingxxAppId.Text;
                ConfigManager.SystemConfigInfo.PaymentPingxxSecretKey = TbPingxxSecretKey.Text;
                ConfigManager.SystemConfigInfo.PaymentChannels =
                    ControlUtils.GetSelectedListControlValueCollection(CblPingxxChannels);

                BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

                SuccessMessage("聚合支付服务商设置成功！");
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
    }
}
