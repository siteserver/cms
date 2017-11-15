using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageCouponActAdd : BasePageCms
    {
        public Literal LtlPageTitle;

        public PlaceHolder PhStep1;
        public TextBox TbKeywords;
        public TextBox TbTitle;
        public TextBox TbSummary;
        public DateTimeTextBox DtbStartDate;
        public DateTimeTextBox DtbEndDate;
        public CheckBox CbIsEnabled;
        public Literal LtlImageUrl;

        public PlaceHolder PhStep2;
        public TextBox TbContentUsage;
        public TextBox TbContentDescription;
        public TextBox TbAwardCode;
        public Literal LtlContentImageUrl;

        public PlaceHolder PhStep3;
        public CheckBox CbIsFormRealName;
        public TextBox TbFormRealNameTitle;
        public CheckBox CbIsFormMobile;
        public TextBox TbFormMobileTitle;
        public CheckBox CbIsFormEmail;
        public TextBox TbFormEmailTitle;
        public CheckBox CbIsFormAddress;
        public TextBox TbFormAddressTitle;

        public PlaceHolder PhStep4;
        public TextBox TbEndTitle;
        public TextBox TbEndSummary;
        public Literal LtlEndImageUrl;

        public HtmlInputHidden ImageUrl;
        public HtmlInputHidden ContentImageUrl;
        public HtmlInputHidden EndImageUrl;

        public Button BtnSubmit;
        public Button BtnReturn;

        private int _actId;

        public static string GetRedirectUrl(int publishmentSystemId, int actId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageCouponActAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"actId", actId.ToString()}
            });
        }

        public string GetUploadUrl()
        {
            return AjaxUpload.GetImageUrlUploadUrl(PublishmentSystemId);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");
            _actId = Body.GetQueryInt("actID");

			if (!IsPostBack)
            {
                var pageTitle = _actId > 0 ? "编辑优惠劵活动" : "添加优惠劵活动";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdCoupon, pageTitle, AppManager.WeiXin.Permission.WebSite.Coupon);
                LtlPageTitle.Text = pageTitle;

                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{CouponManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                LtlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{CouponManager.GetContentImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                LtlEndImageUrl.Text =
                    $@"<img id=""preview_endImageUrl"" src=""{CouponManager.GetEndImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";

                if (_actId > 0)
                {
                    var actInfo = DataProviderWx.CouponActDao.GetActInfo(_actId);

                    TbKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(actInfo.KeywordId);
                    CbIsEnabled.Checked = !actInfo.IsDisabled;
                    DtbStartDate.DateTime = actInfo.StartDate;
                    DtbEndDate.DateTime = actInfo.EndDate;
                    TbTitle.Text = actInfo.Title;
                    if (!string.IsNullOrEmpty(actInfo.ImageUrl))
                    {
                        LtlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, actInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    TbSummary.Text = actInfo.Summary;
                    if (!string.IsNullOrEmpty(actInfo.ContentImageUrl))
                    {
                        LtlContentImageUrl.Text =
                            $@"<img id=""preview_contentImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, actInfo.ContentImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    TbContentUsage.Text = actInfo.ContentUsage;
                    TbContentDescription.Text = actInfo.ContentDescription;
                    TbAwardCode.Text = actInfo.AwardCode;

                    TbEndTitle.Text = actInfo.EndTitle;
                    TbEndSummary.Text = actInfo.EndSummary;
                    if (!string.IsNullOrEmpty(actInfo.EndImageUrl))
                    {
                        LtlEndImageUrl.Text =
                            $@"<img id=""preview_endImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, actInfo.EndImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    ImageUrl.Value = actInfo.ImageUrl;
                    ContentImageUrl.Value = actInfo.ContentImageUrl;
                    EndImageUrl.Value = actInfo.EndImageUrl;

                    CbIsFormRealName.Checked = actInfo.IsFormRealName;
                    TbFormRealNameTitle.Text = actInfo.FormRealNameTitle;
                    CbIsFormMobile.Checked = actInfo.IsFormMobile;
                    TbFormMobileTitle.Text = actInfo.FormMobileTitle;
                    CbIsFormEmail.Checked = actInfo.IsFormEmail;
                    TbFormEmailTitle.Text = actInfo.FormEmailTitle;
                    CbIsFormAddress.Checked = actInfo.IsFormAddress;
                    TbFormAddressTitle.Text = actInfo.FormAddressTitle;
                }
                else
                {
                    DtbEndDate.DateTime = DateTime.Now.AddMonths(1);
                }

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageCouponAct.GetRedirectUrl(PublishmentSystemId)}"";return false");
			}
		}

		public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                var selectedStep = 0;
                if (PhStep1.Visible)
                {
                    selectedStep = 1;
                }
                else if (PhStep2.Visible)
                {
                    selectedStep = 2;
                }
                else if (PhStep3.Visible)
                {
                    selectedStep = 3;
                }
                else if (PhStep4.Visible)
                {
                    selectedStep = 4;
                }
                PhStep1.Visible = PhStep2.Visible = PhStep3.Visible = PhStep4.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(TbKeywords.Text))
                    {
                        if (_actId > 0)
                        {
                            var actInfo = DataProviderWx.CouponActDao.GetActInfo(_actId);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, actInfo.KeywordId, TbKeywords.Text, out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, TbKeywords.Text, out conflictKeywords);
                        }
                    }

                    if (isConflict)
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                        PhStep1.Visible = true;
                    }
                    else
                    {
                        PhStep2.Visible = true;
                    }
                }
                else if (selectedStep == 2)
                {
                    PhStep3.Visible = true;                     
                }
                else if (selectedStep == 3)
                {
                    PhStep4.Visible = true;
                    BtnSubmit.Text = "确 认";
                }
                else if (selectedStep == 4)
                {
                    var actInfo = new CouponActInfo();
                    if (_actId > 0)
                    {
                        actInfo = DataProviderWx.CouponActDao.GetActInfo(_actId);
                    }
                    actInfo.PublishmentSystemId = PublishmentSystemId;

                    actInfo.KeywordId = DataProviderWx.KeywordDao.GetKeywordId(PublishmentSystemId, _actId > 0,PageUtils.FilterXss(TbKeywords.Text), EKeywordType.Coupon, actInfo.KeywordId);
                    actInfo.IsDisabled = !CbIsEnabled.Checked;

                    actInfo.StartDate = DtbStartDate.DateTime;
                    actInfo.EndDate = DtbEndDate.DateTime;
                    actInfo.Title = TbTitle.Text;
                    actInfo.ImageUrl = ImageUrl.Value; ;
                    actInfo.Summary = TbSummary.Text;
                    actInfo.ContentImageUrl = ContentImageUrl.Value; ;
                    actInfo.ContentUsage = TbContentUsage.Text;
                    actInfo.ContentDescription = TbContentDescription.Text;
                    actInfo.AwardCode = TbAwardCode.Text;                    

                    actInfo.IsFormRealName = CbIsFormRealName.Checked;
                    actInfo.FormRealNameTitle = TbFormRealNameTitle.Text;
                    actInfo.IsFormMobile = CbIsFormMobile.Checked;
                    actInfo.FormMobileTitle = TbFormMobileTitle.Text;
                    actInfo.IsFormEmail = CbIsFormEmail.Checked;
                    actInfo.FormEmailTitle = TbFormEmailTitle.Text;
                    actInfo.IsFormAddress = CbIsFormAddress.Checked;
                    actInfo.FormAddressTitle = TbFormAddressTitle.Text;

                    actInfo.EndTitle = TbEndTitle.Text;
                    actInfo.EndImageUrl = EndImageUrl.Value;
                    actInfo.EndSummary = TbEndSummary.Text;

                    try
                    {
                        if (_actId > 0)
                        {
                            DataProviderWx.CouponActDao.Update(actInfo);

                            LogUtils.AddAdminLog("修改优惠劵活动",
                                $"优惠劵活动:{TbTitle.Text}");
                            SuccessMessage("修改优惠劵活动成功！");
                        }
                        else
                        {
                            _actId = DataProviderWx.CouponActDao.Insert(actInfo);

                            LogUtils.AddAdminLog("添加优惠劵活动",
                                $"优惠劵活动:{TbTitle.Text}");
                            SuccessMessage("添加优惠劵活动成功！");
                        }

                        var redirectUrl = PageCouponAct.GetRedirectUrl(PublishmentSystemId);
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "优惠劵活动设置失败！");
                    }

                    BtnSubmit.Visible = false;
                    BtnReturn.Visible = false;
                }
			}
		}
	}
}
