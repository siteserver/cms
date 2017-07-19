using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
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
    public class PageGoldEggAdd : BasePageCms
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
        public Literal LtlContentImageUrl;

        public PlaceHolder PhStep3;
        public TextBox TbAwardUsage;
        public CheckBox CbIsAwardTotalNum;
        public TextBox TbAwardMaxCount;
        public TextBox TbAwardMaxDailyCount;
        public TextBox TbAwardCode;
        public Literal LtlAwardImageUrl;
        public Literal LtlAwardItems;

        public PlaceHolder PhStep4;
        public CheckBox CbIsFormRealName;
        public TextBox TbFormRealNameTitle;
        public CheckBox CbIsFormMobile;
        public TextBox TbFormMobileTitle;
        public CheckBox CbIsFormEmail;
        public TextBox TbFormEmailTitle;
        public CheckBox CbIsFormAddress;
        public TextBox TbFormAddressTitle;

        public PlaceHolder PhStep5;
        public TextBox TbEndTitle;
        public TextBox TbEndSummary;
        public Literal LtlEndImageUrl;

        public HtmlInputHidden ImageUrl;
        public HtmlInputHidden ContentImageUrl;
        public HtmlInputHidden AwardImageUrl;
        public HtmlInputHidden EndImageUrl;

        public Button BtnSubmit;
        public Button BtnReturn;

        private int _lotteryId;

        public static string GetRedirectUrl(int publishmentSystemId, int lotteryId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageGoldEggAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"lotteryId", lotteryId.ToString()}
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
            _lotteryId = Body.GetQueryInt("lotteryID");

			if (!IsPostBack)
            {
                var pageTitle = _lotteryId > 0 ? "编辑砸金蛋" : "添加砸金蛋";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdGoldEgg, pageTitle, AppManager.WeiXin.Permission.WebSite.GoldEgg);
                LtlPageTitle.Text = pageTitle;

                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{LotteryManager.GetImageUrl(PublishmentSystemInfo,
                        ELotteryType.GoldEgg, string.Empty)}"" width=""370"" align=""middle"" />";
                LtlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{LotteryManager.GetContentImageUrl(
                        PublishmentSystemInfo, ELotteryType.GoldEgg, string.Empty)}"" width=""370"" align=""middle"" />";
                LtlAwardImageUrl.Text =
                    $@"<img id=""preview_awardImageUrl"" src=""{LotteryManager.GetAwardImageUrl(PublishmentSystemInfo,
                        ELotteryType.GoldEgg, string.Empty)}"" width=""370"" align=""middle"" />";
                LtlEndImageUrl.Text =
                    $@"<img id=""preview_endImageUrl"" src=""{LotteryManager.GetEndImageUrl(PublishmentSystemInfo,
                        ELotteryType.GoldEgg, string.Empty)}"" width=""370"" align=""middle"" />";

                if (_lotteryId == 0)
                {
                    LtlAwardItems.Text = "itemController.itemCount = 2;itemController.items = [{}, {}];";
                    DtbEndDate.DateTime = DateTime.Now.AddMonths(1);
                }
                else
                {
                    var lotteryInfo = DataProviderWx.LotteryDao.GetLotteryInfo(_lotteryId);

                    TbKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(lotteryInfo.KeywordId);
                    CbIsEnabled.Checked = !lotteryInfo.IsDisabled;
                    DtbStartDate.DateTime = lotteryInfo.StartDate;
                    DtbEndDate.DateTime = lotteryInfo.EndDate;
                    TbTitle.Text = lotteryInfo.Title;
                    if (!string.IsNullOrEmpty(lotteryInfo.ImageUrl))
                    {
                        LtlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, lotteryInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    TbSummary.Text = lotteryInfo.Summary;
                    if (!string.IsNullOrEmpty(lotteryInfo.ContentImageUrl))
                    {
                        LtlContentImageUrl.Text =
                            $@"<img id=""preview_contentImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, lotteryInfo.ContentImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    TbContentUsage.Text = lotteryInfo.ContentUsage;

                    TbAwardUsage.Text = lotteryInfo.AwardUsage;
                    CbIsAwardTotalNum.Checked = lotteryInfo.IsAwardTotalNum;
                    TbAwardMaxCount.Text = lotteryInfo.AwardMaxCount.ToString();
                    TbAwardMaxDailyCount.Text = lotteryInfo.AwardMaxDailyCount.ToString();
                    TbAwardCode.Text = lotteryInfo.AwardCode;

                    CbIsFormRealName.Checked = lotteryInfo.IsFormRealName;
                    TbFormRealNameTitle.Text = lotteryInfo.FormRealNameTitle;
                    CbIsFormMobile.Checked = lotteryInfo.IsFormMobile;
                    TbFormMobileTitle.Text = lotteryInfo.FormMobileTitle;
                    CbIsFormEmail.Checked = lotteryInfo.IsFormEmail;
                    TbFormEmailTitle.Text = lotteryInfo.FormEmailTitle;
                    CbIsFormAddress.Checked = lotteryInfo.IsFormAddress;
                    TbFormAddressTitle.Text = lotteryInfo.FormAddressTitle;

                    if (!string.IsNullOrEmpty(lotteryInfo.AwardImageUrl))
                    {
                        LtlAwardImageUrl.Text =
                            $@"<img id=""preview_awardImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, lotteryInfo.AwardImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    var awardInfoList = DataProviderWx.LotteryAwardDao.GetLotteryAwardInfoList(PublishmentSystemId, _lotteryId);
                    var itemBuilder = new StringBuilder();
                    foreach (var awardInfo in awardInfoList)
                    {
                        itemBuilder.AppendFormat(@"{{id: '{0}', awardName: '{1}', title: '{2}', totalNum: '{3}', probability: '{4}'}},", awardInfo.Id, awardInfo.AwardName, awardInfo.Title, awardInfo.TotalNum, awardInfo.Probability);
                    }
                    if (itemBuilder.Length > 0) itemBuilder.Length--;

                    LtlAwardItems.Text = $@"
itemController.itemCount = {awardInfoList.Count};itemController.items = [{itemBuilder}];";

                    TbEndTitle.Text = lotteryInfo.EndTitle;
                    TbEndSummary.Text = lotteryInfo.EndSummary;
                    if (!string.IsNullOrEmpty(lotteryInfo.EndImageUrl))
                    {
                        LtlEndImageUrl.Text =
                            $@"<img id=""preview_endImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, lotteryInfo.EndImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    ImageUrl.Value = lotteryInfo.ImageUrl;
                    ContentImageUrl.Value = lotteryInfo.ContentImageUrl;
                    AwardImageUrl.Value = lotteryInfo.AwardImageUrl;
                    EndImageUrl.Value = lotteryInfo.EndImageUrl;
                }

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageLottery.GetRedirectUrl(PublishmentSystemId, ELotteryType.GoldEgg)}"";return false");
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
                else if (PhStep5.Visible)
                {
                    selectedStep = 5;
                }

                PhStep1.Visible = PhStep2.Visible = PhStep3.Visible = PhStep4.Visible = PhStep5.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(TbKeywords.Text))
                    {
                        if (_lotteryId > 0)
                        {
                            var lotteryInfo = DataProviderWx.LotteryDao.GetLotteryInfo(_lotteryId);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, lotteryInfo.KeywordId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
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
                    var isItemReady = true;

                    var awardMaxCount = TranslateUtils.ToInt(TbAwardMaxCount.Text);
                    var awardMaxDailyCount = TranslateUtils.ToInt(TbAwardMaxDailyCount.Text);
                    if (awardMaxDailyCount > awardMaxCount)
                    {
                        FailMessage("砸金蛋保存失败，每人每天最多允许抽奖次数必须小于每人最多抽奖次数");
                        isItemReady = false;
                    }

                    if (isItemReady)
                    {
                        var itemCount = TranslateUtils.ToInt(Request.Form["itemCount"]);

                        if (itemCount < 1)
                        {
                            FailMessage("砸金蛋保存失败，至少需要设置一个奖项");
                            isItemReady = false;
                        }
                        else
                        {
                            var itemIdList = TranslateUtils.StringCollectionToIntList(Request.Form["itemID"]);
                            var awardNameList = TranslateUtils.StringCollectionToStringList(Request.Form["itemAwardName"]);
                            var titleList = TranslateUtils.StringCollectionToStringList(Request.Form["itemTitle"]);
                            var totalNumList = TranslateUtils.StringCollectionToIntList(Request.Form["itemTotalNum"]);
                            var probabilityList = TranslateUtils.StringCollectionToDecimalList(Request.Form["itemProbability"]);

                            decimal probabilityAll = 0;

                            var awardInfoList = new List<LotteryAwardInfo>();
                            for (var i = 0; i < itemCount; i++)
                            {
                                var awardInfo = new LotteryAwardInfo { Id = itemIdList[i], PublishmentSystemId = PublishmentSystemId, LotteryId = _lotteryId, AwardName = awardNameList[i], Title = titleList[i], TotalNum = totalNumList[i], Probability = probabilityList[i] };

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
                                FailMessage("砸金蛋保存失败，获奖概率之和必须在1%到100%之间");
                                isItemReady = false;
                            }

                            if (isItemReady)
                            {
                                DataProviderWx.LotteryAwardDao.DeleteAllNotInIdList(PublishmentSystemId, _lotteryId, itemIdList);

                                foreach (var awardInfo in awardInfoList)
                                {
                                    var newAwardInfo = DataProviderWx.LotteryAwardDao.GetAwardInfo(awardInfo.Id);
                                    if (awardInfo.Id > 0)
                                    {
                                        var wonNum = DataProviderWx.LotteryWinnerDao.GetTotalNum(awardInfo.Id);
                                        if (awardInfo.TotalNum < wonNum)
                                        {
                                            awardInfo.TotalNum = wonNum;
                                        }
                                        awardInfo.WonNum = newAwardInfo.WonNum;
                                        DataProviderWx.LotteryAwardDao.Update(awardInfo);
                                    }
                                    else
                                    {
                                        DataProviderWx.LotteryAwardDao.Insert(awardInfo);
                                    }
                                }
                            }
                        }
                    }

                    if (isItemReady)
                    {
                        PhStep4.Visible = true;
                    }
                    else
                    {
                        PhStep3.Visible = true;
                    }
                }
                else if (selectedStep == 4)
                {
                    PhStep5.Visible = true;
                    BtnSubmit.Text = "确 认";
                }
                else if (selectedStep == 5)
                {
                    var lotteryInfo = new LotteryInfo();
                    if (_lotteryId > 0)
                    {
                        lotteryInfo = DataProviderWx.LotteryDao.GetLotteryInfo(_lotteryId);
                    }
                    lotteryInfo.PublishmentSystemId = PublishmentSystemId;
                    lotteryInfo.LotteryType = ELotteryTypeUtils.GetValue(ELotteryType.GoldEgg);

                    lotteryInfo.KeywordId = DataProviderWx.KeywordDao.GetKeywordId(PublishmentSystemId, _lotteryId > 0, TbKeywords.Text, EKeywordType.GoldEgg, lotteryInfo.KeywordId);
                    lotteryInfo.IsDisabled = !CbIsEnabled.Checked;

                    lotteryInfo.StartDate = DtbStartDate.DateTime;
                    lotteryInfo.EndDate = DtbEndDate.DateTime;
                    lotteryInfo.Title = PageUtils.FilterXss(TbTitle.Text);
                    lotteryInfo.ImageUrl = ImageUrl.Value; ;
                    lotteryInfo.Summary = TbSummary.Text;
                    lotteryInfo.ContentImageUrl = ContentImageUrl.Value;
                    lotteryInfo.ContentUsage = TbContentUsage.Text;

                    lotteryInfo.AwardUsage = TbAwardUsage.Text;
                    lotteryInfo.IsAwardTotalNum = CbIsAwardTotalNum.Checked;
                    lotteryInfo.AwardMaxCount = TranslateUtils.ToInt(TbAwardMaxCount.Text);
                    lotteryInfo.AwardMaxDailyCount = TranslateUtils.ToInt(TbAwardMaxDailyCount.Text);
                    lotteryInfo.AwardCode = TbAwardCode.Text;
                    lotteryInfo.AwardImageUrl = AwardImageUrl.Value;

                    lotteryInfo.IsFormRealName = CbIsFormRealName.Checked;
                    lotteryInfo.FormRealNameTitle = TbFormRealNameTitle.Text;
                    lotteryInfo.IsFormMobile = CbIsFormMobile.Checked;
                    lotteryInfo.FormMobileTitle = TbFormMobileTitle.Text;
                    lotteryInfo.IsFormEmail = CbIsFormEmail.Checked;
                    lotteryInfo.FormEmailTitle = TbFormEmailTitle.Text;
                    lotteryInfo.IsFormAddress = CbIsFormAddress.Checked;
                    lotteryInfo.FormAddressTitle = TbFormAddressTitle.Text;

                    lotteryInfo.EndTitle = TbEndTitle.Text;
                    lotteryInfo.EndImageUrl = EndImageUrl.Value;
                    lotteryInfo.EndSummary = TbEndSummary.Text;

                    try
                    {
                        if (_lotteryId > 0)
                        {
                            DataProviderWx.LotteryDao.Update(lotteryInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "修改砸金蛋", $"砸金蛋:{TbTitle.Text}");
                            SuccessMessage("修改砸金蛋成功！");
                        }
                        else
                        {
                            _lotteryId = DataProviderWx.LotteryDao.Insert(lotteryInfo);

                            DataProviderWx.LotteryAwardDao.UpdateLotteryId(PublishmentSystemId, _lotteryId);

                            LogUtils.AddAdminLog(Body.AdministratorName, "添加砸金蛋", $"砸金蛋:{TbTitle.Text}");
                            SuccessMessage("添加砸金蛋成功！");
                        }

                        var redirectUrl = PageLottery.GetRedirectUrl(PublishmentSystemId, ELotteryType.GoldEgg);
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "砸金蛋设置失败！");
                    }
                }
			}
		}
	}
}
