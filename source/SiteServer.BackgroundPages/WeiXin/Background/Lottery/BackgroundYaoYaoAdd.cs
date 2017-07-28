using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using BaiRong.Controls;
using System.Collections.Generic;
using System.Text;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundYaoYaoAdd : BackgroundBasePageWX
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
        public Literal ltlContentImageUrl;

        public PlaceHolder phStep3;
        public TextBox tbAwardUsage;
        public CheckBox cbIsAwardTotalNum;
        public TextBox tbAwardMaxCount;
        public TextBox tbAwardMaxDailyCount;
        public TextBox tbAwardCode;
        public Literal ltlAwardImageUrl;
        public Literal ltlAwardItems;

        public PlaceHolder phStep4;
        public CheckBox cbIsFormRealName;
        public TextBox tbFormRealNameTitle;
        public CheckBox cbIsFormMobile;
        public TextBox tbFormMobileTitle;
        public CheckBox cbIsFormEmail;
        public TextBox tbFormEmailTitle;
        public CheckBox cbIsFormAddress;
        public TextBox tbFormAddressTitle;

        public PlaceHolder phStep5;
        public TextBox tbEndTitle;
        public TextBox tbEndSummary;
        public Literal ltlEndImageUrl;

        public HtmlInputHidden imageUrl;
        public HtmlInputHidden contentImageUrl;
        public HtmlInputHidden awardImageUrl;
        public HtmlInputHidden endImageUrl;

        public Button btnSubmit;
        public Button btnReturn;

        private int lotteryID;

        public static string GetRedirectUrl(int publishmentSystemID, int lotteryID)
        {
            return PageUtils.GetWXUrl(
                $"background_yaoYaoAdd.aspx?publishmentSystemID={publishmentSystemID}&lotteryID={lotteryID}");
        }

        public string GetUploadUrl()
        {
            return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            lotteryID = TranslateUtils.ToInt(GetQueryString("lotteryID"));

			if (!IsPostBack)
            {
                var pageTitle = lotteryID > 0 ? "编辑摇摇乐" : "添加摇摇乐";
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_YaoYao, pageTitle, AppManager.WeiXin.Permission.WebSite.YaoYao);
                ltlPageTitle.Text = pageTitle;

                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{LotteryManager.GetImageUrl(PublishmentSystemInfo,
                        ELotteryType.YaoYao, string.Empty)}"" width=""370"" align=""middle"" />";
                ltlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{LotteryManager.GetContentImageUrl(
                        PublishmentSystemInfo, ELotteryType.YaoYao, string.Empty)}"" width=""370"" align=""middle"" />";
                ltlAwardImageUrl.Text =
                    $@"<img id=""preview_awardImageUrl"" src=""{LotteryManager.GetAwardImageUrl(PublishmentSystemInfo,
                        ELotteryType.YaoYao, string.Empty)}"" width=""370"" align=""middle"" />";
                ltlEndImageUrl.Text =
                    $@"<img id=""preview_endImageUrl"" src=""{LotteryManager.GetEndImageUrl(PublishmentSystemInfo,
                        ELotteryType.YaoYao, string.Empty)}"" width=""370"" align=""middle"" />";

                if (lotteryID == 0)
                {
                    ltlAwardItems.Text = "itemController.itemCount = 2;itemController.items = [{}, {}];";
                    dtbEndDate.DateTime = DateTime.Now.AddMonths(1);
                }
                else
                {
                    var lotteryInfo = DataProviderWX.LotteryDAO.GetLotteryInfo(lotteryID);

                    tbKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(lotteryInfo.KeywordID);
                    cbIsEnabled.Checked = !lotteryInfo.IsDisabled;
                    dtbStartDate.DateTime = lotteryInfo.StartDate;
                    dtbEndDate.DateTime = lotteryInfo.EndDate;
                    tbTitle.Text = lotteryInfo.Title;
                    if (!string.IsNullOrEmpty(lotteryInfo.ImageUrl))
                    {
                        ltlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, lotteryInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    tbSummary.Text = lotteryInfo.Summary;
                    if (!string.IsNullOrEmpty(lotteryInfo.ContentImageUrl))
                    {
                        ltlContentImageUrl.Text =
                            $@"<img id=""preview_contentImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, lotteryInfo.ContentImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    tbContentUsage.Text = lotteryInfo.ContentUsage;

                    tbAwardUsage.Text = lotteryInfo.AwardUsage;
                    cbIsAwardTotalNum.Checked = lotteryInfo.IsAwardTotalNum;
                    tbAwardMaxCount.Text = lotteryInfo.AwardMaxCount.ToString();
                    tbAwardMaxDailyCount.Text = lotteryInfo.AwardMaxDailyCount.ToString();
                    tbAwardCode.Text = lotteryInfo.AwardCode;

                    cbIsFormRealName.Checked = lotteryInfo.IsFormRealName;
                    tbFormRealNameTitle.Text = lotteryInfo.FormRealNameTitle;
                    cbIsFormMobile.Checked = lotteryInfo.IsFormMobile;
                    tbFormMobileTitle.Text = lotteryInfo.FormMobileTitle;
                    cbIsFormEmail.Checked = lotteryInfo.IsFormEmail;
                    tbFormEmailTitle.Text = lotteryInfo.FormEmailTitle;
                    cbIsFormAddress.Checked = lotteryInfo.IsFormAddress;
                    tbFormAddressTitle.Text = lotteryInfo.FormAddressTitle;

                    if (!string.IsNullOrEmpty(lotteryInfo.AwardImageUrl))
                    {
                        ltlAwardImageUrl.Text =
                            $@"<img id=""preview_awardImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, lotteryInfo.AwardImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    var awardInfoList = DataProviderWX.LotteryAwardDAO.GetLotteryAwardInfoList(PublishmentSystemID, lotteryID);
                    var itemBuilder = new StringBuilder();
                    foreach (var awardInfo in awardInfoList)
                    {
                        itemBuilder.AppendFormat(@"{{id: '{0}', awardName: '{1}', title: '{2}', totalNum: '{3}', probability: '{4}'}},", awardInfo.ID, awardInfo.AwardName, awardInfo.Title, awardInfo.TotalNum, awardInfo.Probability);
                    }
                    if (itemBuilder.Length > 0) itemBuilder.Length--;

                    ltlAwardItems.Text = $@"
itemController.itemCount = {awardInfoList.Count};itemController.items = [{itemBuilder.ToString()}];";

                    tbEndTitle.Text = lotteryInfo.EndTitle;
                    tbEndSummary.Text = lotteryInfo.EndSummary;
                    if (!string.IsNullOrEmpty(lotteryInfo.EndImageUrl))
                    {
                        ltlEndImageUrl.Text =
                            $@"<img id=""preview_endImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, lotteryInfo.EndImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    imageUrl.Value = lotteryInfo.ImageUrl;
                    contentImageUrl.Value = lotteryInfo.ContentImageUrl;
                    awardImageUrl.Value = lotteryInfo.AwardImageUrl;
                    endImageUrl.Value = lotteryInfo.EndImageUrl;
                }

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundLottery.GetRedirectUrl(PublishmentSystemID, ELotteryType.YaoYao)}"";return false");
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
                else if (phStep5.Visible)
                {
                    selectedStep = 5;
                }

                phStep1.Visible = phStep2.Visible = phStep3.Visible = phStep4.Visible = phStep5.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(tbKeywords.Text))
                    {
                        if (lotteryID > 0)
                        {
                            var lotteryInfo = DataProviderWX.LotteryDAO.GetLotteryInfo(lotteryID);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemID, lotteryInfo.KeywordID, PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemID, PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
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
                    var isItemReady = true;

                    var awardMaxCount = TranslateUtils.ToInt(tbAwardMaxCount.Text);
                    var awardMaxDailyCount = TranslateUtils.ToInt(tbAwardMaxDailyCount.Text);
                    if (awardMaxDailyCount > awardMaxCount)
                    {
                        FailMessage("摇摇乐保存失败，每人每天最多允许抽奖次数必须小于每人最多抽奖次数");
                        isItemReady = false;
                    }

                    if (isItemReady)
                    {
                        var itemCount = TranslateUtils.ToInt(Request.Form["itemCount"]);

                        if (itemCount < 1)
                        {
                            FailMessage("摇摇乐保存失败，至少需要设置一个奖项");
                            isItemReady = false;
                        }
                        else
                        {
                            var itemIDList = TranslateUtils.StringCollectionToIntList(Request.Form["itemID"]);
                            var awardNameList = TranslateUtils.StringCollectionToStringList(Request.Form["itemAwardName"]);
                            var titleList = TranslateUtils.StringCollectionToStringList(Request.Form["itemTitle"]);
                            var totalNumList = TranslateUtils.StringCollectionToIntList(Request.Form["itemTotalNum"]);
                            var probabilityList = TranslateUtils.StringCollectionToDecimalList(Request.Form["itemProbability"]);

                            decimal probabilityAll = 0;

                            var awardInfoList = new List<LotteryAwardInfo>();
                            for (var i = 0; i < itemCount; i++)
                            {
                                var awardInfo = new LotteryAwardInfo { ID = itemIDList[i], PublishmentSystemID = PublishmentSystemID, LotteryID = lotteryID, AwardName = awardNameList[i], Title = titleList[i], TotalNum = totalNumList[i], Probability = probabilityList[i] };

                                if (string.IsNullOrEmpty(awardInfo.AwardName))
                                {
                                    FailMessage("保存失败，奖项名称为必填项");
                                    isItemReady = false;
                                }
                                if (string.IsNullOrEmpty(awardInfo.Title))
                                {
                                    FailMessage("保存失败，奖品名为必填项");
                                    isItemReady = false;
                                }
                                if (awardInfo.Probability < 0 || awardInfo.Probability > 100)
                                {
                                    FailMessage("保存失败，各项中奖概率总和不能超过100%");
                                    isItemReady = false;
                                }

                                probabilityAll += awardInfo.Probability;

                                awardInfoList.Add(awardInfo);
                            }

                            if (probabilityAll <= 0 || probabilityAll > 100)
                            {
                                FailMessage("摇摇乐保存失败，获奖概率之和必须在1%到100%之间");
                                isItemReady = false;
                            }

                            if (isItemReady)
                            {
                                DataProviderWX.LotteryAwardDAO.DeleteAllNotInIDList(PublishmentSystemID, lotteryID, itemIDList);

                                foreach (var awardInfo in awardInfoList)
                                {
                                    var newAwardInfo = DataProviderWX.LotteryAwardDAO.GetAwardInfo(awardInfo.ID);
                                    if (awardInfo.ID > 0)
                                    {
                                        var wonNum = DataProviderWX.LotteryWinnerDAO.GetTotalNum(awardInfo.ID);
                                        if (awardInfo.TotalNum < wonNum)
                                        {
                                            awardInfo.TotalNum = wonNum;
                                        }
                                        awardInfo.WonNum = newAwardInfo.WonNum;
                                        DataProviderWX.LotteryAwardDAO.Update(awardInfo);
                                    }
                                    else
                                    {
                                        DataProviderWX.LotteryAwardDAO.Insert(awardInfo);
                                    }
                                }
                            }
                        }
                    }

                    if (isItemReady)
                    {
                        phStep4.Visible = true;
                    }
                    else
                    {
                        phStep3.Visible = true;
                    }
                }
                else if (selectedStep == 4)
                {
                    phStep5.Visible = true;
                    btnSubmit.Text = "确 认";
                }
                else if (selectedStep == 5)
                {
                    var lotteryInfo = new LotteryInfo();
                    if (lotteryID > 0)
                    {
                        lotteryInfo = DataProviderWX.LotteryDAO.GetLotteryInfo(lotteryID);
                    }
                    lotteryInfo.PublishmentSystemID = PublishmentSystemID;
                    lotteryInfo.LotteryType = ELotteryTypeUtils.GetValue(ELotteryType.YaoYao);

                    lotteryInfo.KeywordID = DataProviderWX.KeywordDAO.GetKeywordID(PublishmentSystemID, lotteryID > 0, tbKeywords.Text, EKeywordType.YaoYao, lotteryInfo.KeywordID);
                    lotteryInfo.IsDisabled = !cbIsEnabled.Checked;

                    lotteryInfo.StartDate = dtbStartDate.DateTime;
                    lotteryInfo.EndDate = dtbEndDate.DateTime;
                    lotteryInfo.Title = PageUtils.FilterXSS(tbTitle.Text);
                    lotteryInfo.ImageUrl = imageUrl.Value; ;
                    lotteryInfo.Summary = tbSummary.Text;
                    lotteryInfo.ContentImageUrl = contentImageUrl.Value;
                    lotteryInfo.ContentUsage = tbContentUsage.Text;

                    lotteryInfo.AwardUsage = tbAwardUsage.Text;
                    lotteryInfo.IsAwardTotalNum = cbIsAwardTotalNum.Checked;
                    lotteryInfo.AwardMaxCount = TranslateUtils.ToInt(tbAwardMaxCount.Text);
                    lotteryInfo.AwardMaxDailyCount = TranslateUtils.ToInt(tbAwardMaxDailyCount.Text);
                    lotteryInfo.AwardCode = tbAwardCode.Text;
                    lotteryInfo.AwardImageUrl = awardImageUrl.Value;

                    lotteryInfo.IsFormRealName = cbIsFormRealName.Checked;
                    lotteryInfo.FormRealNameTitle = tbFormRealNameTitle.Text;
                    lotteryInfo.IsFormMobile = cbIsFormMobile.Checked;
                    lotteryInfo.FormMobileTitle = tbFormMobileTitle.Text;
                    lotteryInfo.IsFormEmail = cbIsFormEmail.Checked;
                    lotteryInfo.FormEmailTitle = tbFormEmailTitle.Text;
                    lotteryInfo.IsFormAddress = cbIsFormAddress.Checked;
                    lotteryInfo.FormAddressTitle = tbFormAddressTitle.Text;

                    lotteryInfo.EndTitle = tbEndTitle.Text;
                    lotteryInfo.EndImageUrl = endImageUrl.Value;
                    lotteryInfo.EndSummary = tbEndSummary.Text;

                    try
                    {
                        if (lotteryID > 0)
                        {
                            DataProviderWX.LotteryDAO.Update(lotteryInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "修改摇摇乐",
                                $"摇摇乐:{tbTitle.Text}");
                            SuccessMessage("修改摇摇乐成功！");
                        }
                        else
                        {
                            lotteryID = DataProviderWX.LotteryDAO.Insert(lotteryInfo);

                            DataProviderWX.LotteryAwardDAO.UpdateLotteryID(PublishmentSystemID, lotteryID);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "添加摇摇乐",
                                $"摇摇乐:{tbTitle.Text}");
                            SuccessMessage("添加摇摇乐成功！");
                        }

                        var redirectUrl = BackgroundLottery.GetRedirectUrl(PublishmentSystemID, ELotteryType.YaoYao);
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "摇摇乐设置失败！");
                    }
                }
			}
		}
	}
}
