using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Net;
using BaiRong.Core.Data.Provider;
using BaiRong.Model;
using System.Collections;


using BaiRong.Controls;
using SiteServer.B2C.Core;
using SiteServer.B2C.Model;

using SiteServer.CMS.BackgroundPages;
using SiteServer.B2C.Core.Union;

namespace SiteServer.B2C.BackgroundPages
{
    public class BackgroundPaymentConfiguration : BackgroundBasePage
    {
        public TextBox tbPaymentName;
        public Literal ltlPaymentType;

        public PlaceHolder phAlipay;
        public TextBox tbAlipaySellerEmail;
        public TextBox tbAlipayPartner;
        public TextBox tbAlipayKey;
        public DropDownList ddlAlipayType;

        public PlaceHolder phUnionpay;
        public TextBox tbMerID;
        public FileUpload fuSignCert;
        public FileUpload fuEncryptCert;
        public TextBox tbSignCertPwd;
        public DropDownList ddlIsTest;

        public BREditor breDescription;

        private int paymentID;

        public static string GetRedirectUrl(int publishmentSystemID, int paymentID)
        {
            return PageUtils.GetB2CUrl(string.Format("background_paymentConfiguration.aspx?publishmentSystemID={0}&paymentID={1}", publishmentSystemID, paymentID));
        }

        public void Page_Load(object sender, EventArgs E)
        {
            if (base.IsForbidden) return;

            this.paymentID = base.GetIntQueryString("paymentID");

            if (!IsPostBack)
            {
                base.BreadCrumbConsole(AppManager.B2C.LeftMenu.ID_PaymentShipment, "配置支付方式", string.Empty);

                EBooleanUtils.AddListItems(this.ddlIsTest, "测试", "正式");

                if (this.paymentID > 0)
                {
                    PaymentInfo paymentInfo = DataProviderB2C.PaymentDAO.GetPaymentInfo(this.paymentID);
                    if (paymentInfo != null)
                    {
                        this.tbPaymentName.Text = paymentInfo.PaymentName;
                        this.ltlPaymentType.Text = EPaymentTypeUtils.GetText(paymentInfo.PaymentType);
                        if (paymentInfo.PaymentType == EPaymentType.Alipay)
                        {
                            EPaymentAlipayUtils.AddListItems(this.ddlAlipayType);

                            PaymentAlipayInfo alipayInfo = new PaymentAlipayInfo(paymentInfo.SettingsXML);
                            this.phAlipay.Visible = true;
                            this.tbAlipaySellerEmail.Text = alipayInfo.SellerEmail;
                            this.tbAlipayPartner.Text = alipayInfo.Partner;
                            this.tbAlipayKey.Text = alipayInfo.Key;
                            ControlUtils.SelectListItems(this.ddlAlipayType, EPaymentAlipayUtils.GetValue(alipayInfo.AlipayType));
                        }
                        else if (paymentInfo.PaymentType == EPaymentType.Unionpay)
                        {
                            PaymentUnionInfo unionInfo = new PaymentUnionInfo(paymentInfo.SettingsXML);
                            this.phUnionpay.Visible = true;
                            this.tbMerID.Text = unionInfo.MerID;
                            this.tbSignCertPwd.Text = unionInfo.SignCertPwd;
                            this.ddlIsTest.SelectedValue = unionInfo.IsTest.ToString();
                        }
                        this.breDescription.Text = paymentInfo.Description;
                    }
                }

                if (TranslateUtils.ToBool(this.ddlIsTest.SelectedValue))
                {
                    //测试
                    this.tbMerID.Text = "700000000000001";
                    this.tbMerID.Enabled = false;
                    this.fuSignCert.Enabled = false;
                    this.tbSignCertPwd.Text = "000000";
                    this.tbSignCertPwd.Enabled = false;
                    this.fuEncryptCert.Enabled = false;
                }
                else
                {
                    //正式
                    this.tbMerID.Text = "";
                    this.tbMerID.Enabled = true;
                    this.fuSignCert.Enabled = true;
                    this.tbSignCertPwd.Text = "";
                    this.tbSignCertPwd.Enabled = true;
                    this.fuEncryptCert.Enabled = true;
                }
            }
        }

        public override void Submit_OnClick(object sender, System.EventArgs e)
        {
            try
            {
                PaymentInfo paymentInfo = new PaymentInfo();
                if (this.paymentID > 0)
                {
                    paymentInfo = DataProviderB2C.PaymentDAO.GetPaymentInfo(this.paymentID);
                }

                paymentInfo.PaymentName = this.tbPaymentName.Text;
                if (paymentInfo.PaymentType == EPaymentType.Alipay)
                {
                    PaymentAlipayInfo alipayInfo = new PaymentAlipayInfo(paymentInfo.SettingsXML);
                    alipayInfo.SellerEmail = this.tbAlipaySellerEmail.Text;
                    alipayInfo.Partner = this.tbAlipayPartner.Text;
                    alipayInfo.Key = this.tbAlipayKey.Text;
                    alipayInfo.AlipayType = EPaymentAlipayUtils.GetEnumType(this.ddlAlipayType.SelectedValue);

                    paymentInfo.SettingsXML = alipayInfo.ToString();
                }
                else if (paymentInfo.PaymentType == EPaymentType.Unionpay)
                {
                    PaymentUnionInfo unionInfo = new PaymentUnionInfo(paymentInfo.SettingsXML);
                    unionInfo.MerID = this.tbMerID.Text;
                    unionInfo.SignCertPwd = this.tbSignCertPwd.Text;
                    if (!TranslateUtils.ToBool(this.ddlIsTest.SelectedValue))
                    {
                        if (this.fuEncryptCert.HasFile)
                        {
                            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(SDKConfig.GetSDKRootVirvualPath(this.fuEncryptCert.FileName)));
                            this.fuEncryptCert.SaveAs(SDKConfig.GetSDKRootVirvualPath(this.fuEncryptCert.FileName));
                            unionInfo.EncryptCert = this.fuEncryptCert.FileName;
                        }
                        if (this.fuSignCert.HasFile)
                        {
                            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(SDKConfig.GetSDKRootVirvualPath(this.fuEncryptCert.FileName)));
                            this.fuSignCert.SaveAs(SDKConfig.GetSDKRootVirvualPath(this.fuSignCert.FileName));
                            unionInfo.SignCertPath = this.fuSignCert.FileName;
                        }
                    }
                    else
                    {
                        unionInfo.MerID = "700000000000001";
                        unionInfo.SignCertPwd = "000000";
                        unionInfo.EncryptCert = "verify_sign_acp.cer";
                        unionInfo.SignCertPath = "700000000000001_acp.pfx";
                        unionInfo.IsTest = true;
                    }
                    unionInfo.IsTest = TranslateUtils.ToBool(this.ddlIsTest.SelectedValue);
                    paymentInfo.SettingsXML = unionInfo.ToString();
                }
                paymentInfo.Description = this.breDescription.Text;

                if (this.paymentID > 0)
                {
                    DataProviderB2C.PaymentDAO.Update(paymentInfo);
                }
                else
                {
                    DataProviderB2C.PaymentDAO.Insert(paymentInfo);
                }

                base.SuccessMessage("配置支付方式成功！");

                base.AddWaitAndRedirectScript(BackgroundPayment.GetRedirectUrl(base.PublishmentSystemID));
            }
            catch (Exception ex)
            {
                base.FailMessage(ex, ex.Message);
            }
        }

        protected void ddlIsTest_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TranslateUtils.ToBool(this.ddlIsTest.SelectedValue))
            {
                //测试
                this.tbMerID.Text = "700000000000001";
                this.tbMerID.Enabled = false;
                this.fuSignCert.Enabled = false;
                this.tbSignCertPwd.Text = "000000";
                this.tbSignCertPwd.Enabled = false;
                this.fuEncryptCert.Enabled = false;
            }
            else
            {
                //正式
                this.tbMerID.Text = "";
                this.tbMerID.Enabled = true;
                this.fuSignCert.Enabled = true;
                this.tbSignCertPwd.Text = "";
                this.tbSignCertPwd.Enabled = true;
                this.fuEncryptCert.Enabled = true;
            }
        }
    }
}
