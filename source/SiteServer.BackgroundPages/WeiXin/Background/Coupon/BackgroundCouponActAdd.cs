using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using BaiRong.Controls;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundCouponActAdd : BackgroundBasePageWX
	{
        public Literal ltlPageTitle;

        public PlaceHolder phStep1;
        public TextBox tbKeywords;
        public TextBox tbTitle;
        public TextBox tbSummary;
        public DateTimeTextBox dtbStartDate;
        public DateTimeTextBox dtbEndDate;
        public CheckBox cbIsEnabled;
        public Literal ltlImageUrl;

        public PlaceHolder phStep2;
        public TextBox tbContentUsage;
        public TextBox tbContentDescription;
        public TextBox tbAwardCode;
        public Literal ltlContentImageUrl;

        public PlaceHolder phStep3;
        public CheckBox cbIsFormRealName;
        public TextBox tbFormRealNameTitle;
        public CheckBox cbIsFormMobile;
        public TextBox tbFormMobileTitle;
        public CheckBox cbIsFormEmail;
        public TextBox tbFormEmailTitle;
        public CheckBox cbIsFormAddress;
        public TextBox tbFormAddressTitle;

        public PlaceHolder phStep4;
        public TextBox tbEndTitle;
        public TextBox tbEndSummary;
        public Literal ltlEndImageUrl;

        public HtmlInputHidden imageUrl;
        public HtmlInputHidden contentImageUrl;
        public HtmlInputHidden endImageUrl;

        public Button btnSubmit;
        public Button btnReturn;

        private int actID;

        public static string GetRedirectUrl(int publishmentSystemID, int actID)
        {
            return PageUtils.GetWXUrl(
                $"background_couponActAdd.aspx?publishmentSystemID={publishmentSystemID}&actID={actID}");
        }

        public string GetUploadUrl()
        {
            return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            actID = TranslateUtils.ToInt(GetQueryString("actID"));

			if (!IsPostBack)
            {
                var pageTitle = actID > 0 ? "编辑优惠劵活动" : "添加优惠劵活动";
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Coupon, pageTitle, AppManager.WeiXin.Permission.WebSite.Coupon);
                ltlPageTitle.Text = pageTitle;

                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{CouponManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                ltlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{CouponManager.GetContentImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                ltlEndImageUrl.Text =
                    $@"<img id=""preview_endImageUrl"" src=""{CouponManager.GetEndImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";

                if (actID > 0)
                {
                    var actInfo = DataProviderWX.CouponActDAO.GetActInfo(actID);

                    tbKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(actInfo.KeywordID);
                    cbIsEnabled.Checked = !actInfo.IsDisabled;
                    dtbStartDate.DateTime = actInfo.StartDate;
                    dtbEndDate.DateTime = actInfo.EndDate;
                    tbTitle.Text = actInfo.Title;
                    if (!string.IsNullOrEmpty(actInfo.ImageUrl))
                    {
                        ltlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, actInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    tbSummary.Text = actInfo.Summary;
                    if (!string.IsNullOrEmpty(actInfo.ContentImageUrl))
                    {
                        ltlContentImageUrl.Text =
                            $@"<img id=""preview_contentImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, actInfo.ContentImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    tbContentUsage.Text = actInfo.ContentUsage;
                    tbContentDescription.Text = actInfo.ContentDescription;
                    tbAwardCode.Text = actInfo.AwardCode;

                    tbEndTitle.Text = actInfo.EndTitle;
                    tbEndSummary.Text = actInfo.EndSummary;
                    if (!string.IsNullOrEmpty(actInfo.EndImageUrl))
                    {
                        ltlEndImageUrl.Text =
                            $@"<img id=""preview_endImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, actInfo.EndImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    imageUrl.Value = actInfo.ImageUrl;
                    contentImageUrl.Value = actInfo.ContentImageUrl;
                    endImageUrl.Value = actInfo.EndImageUrl;

                    cbIsFormRealName.Checked = actInfo.IsFormRealName;
                    tbFormRealNameTitle.Text = actInfo.FormRealNameTitle;
                    cbIsFormMobile.Checked = actInfo.IsFormMobile;
                    tbFormMobileTitle.Text = actInfo.FormMobileTitle;
                    cbIsFormEmail.Checked = actInfo.IsFormEmail;
                    tbFormEmailTitle.Text = actInfo.FormEmailTitle;
                    cbIsFormAddress.Checked = actInfo.IsFormAddress;
                    tbFormAddressTitle.Text = actInfo.FormAddressTitle;
                }
                else
                {
                    dtbEndDate.DateTime = DateTime.Now.AddMonths(1);
                }

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundCouponAct.GetRedirectUrl(PublishmentSystemID)}"";return false");
			}
		}

		public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                var selectedStep = 0;
                if (phStep1.Visible)
                {
                    selectedStep = 1;
                }
                else if (phStep2.Visible)
                {
                    selectedStep = 2;
                }
                else if (phStep3.Visible)
                {
                    selectedStep = 3;
                }
                else if (phStep4.Visible)
                {
                    selectedStep = 4;
                }
                phStep1.Visible = phStep2.Visible = phStep3.Visible = phStep4.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(tbKeywords.Text))
                    {
                        if (actID > 0)
                        {
                            var actInfo = DataProviderWX.CouponActDAO.GetActInfo(actID);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemID, actInfo.KeywordID, tbKeywords.Text, out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemID, tbKeywords.Text, out conflictKeywords);
                        }
                    }

                    if (isConflict)
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                        phStep1.Visible = true;
                    }
                    else
                    {
                        phStep2.Visible = true;
                    }
                }
                else if (selectedStep == 2)
                {
                    phStep3.Visible = true;                     
                }
                else if (selectedStep == 3)
                {
                    phStep4.Visible = true;
                    btnSubmit.Text = "确 认";
                }
                else if (selectedStep == 4)
                {
                    var actInfo = new CouponActInfo();
                    if (actID > 0)
                    {
                        actInfo = DataProviderWX.CouponActDAO.GetActInfo(actID);
                    }
                    actInfo.PublishmentSystemID = PublishmentSystemID;

                    actInfo.KeywordID = DataProviderWX.KeywordDAO.GetKeywordID(PublishmentSystemID, actID > 0,PageUtils.FilterXSS(tbKeywords.Text), EKeywordType.Coupon, actInfo.KeywordID);
                    actInfo.IsDisabled = !cbIsEnabled.Checked;

                    actInfo.StartDate = dtbStartDate.DateTime;
                    actInfo.EndDate = dtbEndDate.DateTime;
                    actInfo.Title = tbTitle.Text;
                    actInfo.ImageUrl = imageUrl.Value; ;
                    actInfo.Summary = tbSummary.Text;
                    actInfo.ContentImageUrl = contentImageUrl.Value; ;
                    actInfo.ContentUsage = tbContentUsage.Text;
                    actInfo.ContentDescription = tbContentDescription.Text;
                    actInfo.AwardCode = tbAwardCode.Text;                    

                    actInfo.IsFormRealName = cbIsFormRealName.Checked;
                    actInfo.FormRealNameTitle = tbFormRealNameTitle.Text;
                    actInfo.IsFormMobile = cbIsFormMobile.Checked;
                    actInfo.FormMobileTitle = tbFormMobileTitle.Text;
                    actInfo.IsFormEmail = cbIsFormEmail.Checked;
                    actInfo.FormEmailTitle = tbFormEmailTitle.Text;
                    actInfo.IsFormAddress = cbIsFormAddress.Checked;
                    actInfo.FormAddressTitle = tbFormAddressTitle.Text;

                    actInfo.EndTitle = tbEndTitle.Text;
                    actInfo.EndImageUrl = endImageUrl.Value;
                    actInfo.EndSummary = tbEndSummary.Text;

                    try
                    {
                        if (actID > 0)
                        {
                            DataProviderWX.CouponActDAO.Update(actInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "修改优惠劵活动",
                                $"优惠劵活动:{tbTitle.Text}");
                            SuccessMessage("修改优惠劵活动成功！");
                        }
                        else
                        {
                            actID = DataProviderWX.CouponActDAO.Insert(actInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "添加优惠劵活动",
                                $"优惠劵活动:{tbTitle.Text}");
                            SuccessMessage("添加优惠劵活动成功！");
                        }

                        var redirectUrl = PageUtils.GetWXUrl(
                            $"background_couponAct.aspx?publishmentSystemID={PublishmentSystemID}");
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "优惠劵活动设置失败！");
                    }

                    btnSubmit.Visible = false;
                    btnReturn.Visible = false;
                }
			}
		}
	}
}
