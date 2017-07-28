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
    public class BackgroundConferenceAdd : BackgroundBasePageWX
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
        public Literal ltlBackgroundImageUrl;
        public TextBox tbConferenceName;
        public TextBox tbAddress;
        public TextBox tbDuration;
        public BREditor breIntroduction;

        public PlaceHolder phStep3;
        public CheckBox cbIsAgenda;
        public TextBox tbAgendaTitle;
        public Literal ltlAgendaScript;

        public PlaceHolder phStep4;
        public CheckBox cbIsGuest;
        public TextBox tbGuestTitle;
        public Literal ltlGuestScript;

        public PlaceHolder phStep5;
        public TextBox tbEndTitle;
        public TextBox tbEndSummary;
        public Literal ltlEndImageUrl;

        public HtmlInputHidden imageUrl;
        public HtmlInputHidden backgroundImageUrl;
        public HtmlInputHidden endImageUrl;

        public Button btnSubmit;
        public Button btnReturn;

        private int conferenceID;

        public static string GetRedirectUrl(int publishmentSystemID, int conferenceID)
        {
            return PageUtils.GetWXUrl(
                $"background_conferenceAdd.aspx?publishmentSystemID={publishmentSystemID}&conferenceID={conferenceID}");
        }

        public string GetUploadUrl()
        {
            return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            conferenceID = TranslateUtils.ToInt(GetQueryString("conferenceID"));

			if (!IsPostBack)
            {
                var pageTitle = conferenceID > 0 ? "编辑会议（活动）" : "添加会议（活动）";
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Conference, pageTitle, AppManager.WeiXin.Permission.WebSite.Conference);
                ltlPageTitle.Text = pageTitle;
                
                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{ConferenceManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                ltlBackgroundImageUrl.Text =
                    $@"{ComponentsManager.GetBackgroundImageSelectHtml(string.Empty)}<hr /><img id=""preview_backgroundImageUrl"" src=""{ComponentsManager
                        .GetBackgroundImageUrl(PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                ltlEndImageUrl.Text =
                    $@"<img id=""preview_endImageUrl"" src=""{ConferenceManager.GetEndImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";

                var selectImageClick = CMS.BackgroundPages.Modal.SelectImage.GetOpenWindowString(PublishmentSystemInfo, "itemPicUrl_");
                var uploadImageClick = CMS.BackgroundPages.Modal.UploadImageSingle.GetOpenWindowStringToTextBox(PublishmentSystemID, "itemPicUrl_");
                var cuttingImageClick = CMS.BackgroundPages.Modal.CuttingImage.GetOpenWindowStringWithTextBox(PublishmentSystemID, "itemPicUrl_");
                var previewImageClick = CMS.BackgroundPages.Modal.Message.GetOpenWindowStringToPreviewImage(PublishmentSystemID, "itemPicUrl_");
                ltlGuestScript.Text =
                    $@"guestController.selectImageClickString = ""{selectImageClick}"";guestController.uploadImageClickString = ""{uploadImageClick}"";guestController.cuttingImageClickString = ""{cuttingImageClick}"";guestController.previewImageClickString = ""{previewImageClick}"";";

                if (conferenceID == 0)
                {
                    ltlAgendaScript.Text += "agendaController.agendaCount = 2;agendaController.items = [{}, {}];";
                    ltlGuestScript.Text += "guestController.guestCount = 2;guestController.items = [{}, {}];";
                    dtbEndDate.DateTime = DateTime.Now.AddMonths(1);
                }
                else
                {
                    var conferenceInfo = DataProviderWX.ConferenceDAO.GetConferenceInfo(conferenceID);

                    tbKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(conferenceInfo.KeywordID);
                    cbIsEnabled.Checked = !conferenceInfo.IsDisabled;
                    dtbStartDate.DateTime = conferenceInfo.StartDate;
                    dtbEndDate.DateTime = conferenceInfo.EndDate;
                    tbTitle.Text = conferenceInfo.Title;
                    if (!string.IsNullOrEmpty(conferenceInfo.ImageUrl))
                    {
                        ltlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, conferenceInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    tbSummary.Text = conferenceInfo.Summary;
                    if (!string.IsNullOrEmpty(conferenceInfo.BackgroundImageUrl))
                    {
                        ltlBackgroundImageUrl.Text =
                            $@"{ComponentsManager.GetBackgroundImageSelectHtml(conferenceInfo.BackgroundImageUrl)}<hr /><img id=""preview_backgroundImageUrl"" src=""{ComponentsManager
                                .GetBackgroundImageUrl(PublishmentSystemInfo, conferenceInfo.BackgroundImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    tbConferenceName.Text = conferenceInfo.ConferenceName;
                    tbAddress.Text = conferenceInfo.Address;
                    tbDuration.Text = conferenceInfo.Duration;
                    breIntroduction.Text = conferenceInfo.Introduction;

                    cbIsAgenda.Checked = conferenceInfo.IsAgenda;
                    tbAgendaTitle.Text = conferenceInfo.AgendaTitle;
                    var agendaInfoList = new List<ConferenceAgendaInfo>();
                    agendaInfoList = TranslateUtils.JsonToObject(conferenceInfo.AgendaList, agendaInfoList) as List<ConferenceAgendaInfo>;
                    if (agendaInfoList != null)
                    {
                        var agendaBuilder = new StringBuilder();
                        foreach (var agendaInfo in agendaInfoList)
                        {
                            agendaBuilder.AppendFormat(@"{{dateTime: '{0}', title: '{1}', summary: '{2}'}},", agendaInfo.dateTime, agendaInfo.title, agendaInfo.summary);
                        }
                        if (agendaBuilder.Length > 0) agendaBuilder.Length--;

                        ltlAgendaScript.Text +=
                            $@"agendaController.agendaCount = {agendaInfoList.Count};agendaController.items = [{agendaBuilder
                                .ToString()}];";
                    }
                    else
                    {
                        ltlAgendaScript.Text += "agendaController.agendaCount = 0;agendaController.items = [{}];";
                    }

                    cbIsGuest.Checked = conferenceInfo.IsGuest;
                    tbGuestTitle.Text = conferenceInfo.GuestTitle;
                    var guestInfoList = new List<ConferenceGuestInfo>();
                    guestInfoList = TranslateUtils.JsonToObject(conferenceInfo.GuestList, guestInfoList) as List<ConferenceGuestInfo>;
                    if (guestInfoList != null)
                    {
                        var guestBuilder = new StringBuilder();
                        foreach (var guestInfo in guestInfoList)
                        {
                            guestBuilder.AppendFormat(@"{{displayName: '{0}', position: '{1}', picUrl: '{2}'}},", guestInfo.displayName, guestInfo.position, guestInfo.picUrl);
                        }
                        if (guestBuilder.Length > 0) guestBuilder.Length--;

                        ltlGuestScript.Text +=
                            $@"guestController.guestCount = {guestInfoList.Count};guestController.items = [{guestBuilder
                                .ToString()}];";
                    }
                    else
                    {
                        ltlGuestScript.Text += "guestController.guestCount = 0;guestController.items = [{}];";
                    }

                    tbEndTitle.Text = conferenceInfo.EndTitle;
                    tbEndSummary.Text = conferenceInfo.EndSummary;
                    if (!string.IsNullOrEmpty(conferenceInfo.EndImageUrl))
                    {
                        ltlEndImageUrl.Text =
                            $@"<img id=""preview_endImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, conferenceInfo.EndImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    imageUrl.Value = conferenceInfo.ImageUrl;
                    backgroundImageUrl.Value = conferenceInfo.BackgroundImageUrl;
                    endImageUrl.Value = conferenceInfo.EndImageUrl;
                }

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundConference.GetRedirectUrl(PublishmentSystemID)}"";return false");
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
                        if (conferenceID > 0)
                        {
                            var conferenceInfo = DataProviderWX.ConferenceDAO.GetConferenceInfo(conferenceID);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemID, conferenceInfo.KeywordID, PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
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
                    var agendaCount = TranslateUtils.ToInt(Request.Form["agendaCount"]);

                    if (cbIsAgenda.Checked && agendaCount < 2)
                    {
                        FailMessage("微会议保存失败，至少需要设置两个日程项");
                        isItemReady = false;
                    }
                    else
                    {
                        var dateTimeList = TranslateUtils.StringCollectionToStringList(Request.Form["itemDateTime"]);
                        var titleList = TranslateUtils.StringCollectionToStringList(Request.Form["itemTitle"]);
                        var summaryList = TranslateUtils.StringCollectionToStringList(Request.Form["itemSummary"]);
                        var agendaInfoList = new List<ConferenceAgendaInfo>();

                        for (var i = 0; i < agendaCount; i++)
                        {
                            var dateTime = dateTimeList[i];
                            var title = titleList[i];
                            var summary = summaryList[i];

                            if (string.IsNullOrEmpty(dateTime) || string.IsNullOrEmpty(title))
                            {
                                FailMessage("微会议保存失败，日程项不能为空");
                                isItemReady = false;
                            }

                            var agendaInfo = new ConferenceAgendaInfo { dateTime = dateTime, title = title, summary = summary };

                            agendaInfoList.Add(agendaInfo);
                        }

                        if (isItemReady)
                        {
                            Page.Session.Add("BackgroundConferenceAdd.AgendaList", TranslateUtils.ObjectToJson(agendaInfoList));
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
                    var isItemReady = true;
                    var guestCount = TranslateUtils.ToInt(Request.Form["guestCount"]);

                    if (cbIsGuest.Checked && guestCount < 2)
                    {
                        FailMessage("微会议保存失败，至少需要设置两个嘉宾项");
                        isItemReady = false;
                    }
                    else
                    {
                        var displayNameList = TranslateUtils.StringCollectionToStringList(Request.Form["itemDisplayName"]);
                        var positionList = TranslateUtils.StringCollectionToStringList(Request.Form["itemPosition"]);
                        var picUrlList = TranslateUtils.StringCollectionToStringList(Request.Form["itemPicUrl"]);
                        var guestInfoList = new List<ConferenceGuestInfo>();

                        for (var i = 0; i < guestCount; i++)
                        {
                            var displayName = displayNameList[i];
                            var position = positionList[i];
                            var picUrl = picUrlList[i];

                            if (string.IsNullOrEmpty(displayName) || string.IsNullOrEmpty(position))
                            {
                                FailMessage("微会议保存失败，嘉宾项不能为空");
                                isItemReady = false;
                            }

                            var guestInfo = new ConferenceGuestInfo { displayName = displayName, position = position, picUrl = picUrl };

                            guestInfoList.Add(guestInfo);
                        }

                        if (isItemReady)
                        {
                            Page.Session.Add("BackgroundConferenceAdd.GuestList", TranslateUtils.ObjectToJson(guestInfoList));
                        }
                    }

                    if (isItemReady)
                    {
                        phStep5.Visible = true;
                        btnSubmit.Text = "确 认";
                    }
                    else
                    {
                        phStep4.Visible = true;
                    }
                }
                else if (selectedStep == 5)
                {
                    var conferenceInfo = new ConferenceInfo();
                    if (conferenceID > 0)
                    {
                        conferenceInfo = DataProviderWX.ConferenceDAO.GetConferenceInfo(conferenceID);
                    }

                    conferenceInfo.PublishmentSystemID = PublishmentSystemID;

                    conferenceInfo.KeywordID = DataProviderWX.KeywordDAO.GetKeywordID(PublishmentSystemID, conferenceID > 0, tbKeywords.Text, EKeywordType.Conference, conferenceInfo.KeywordID);
                    conferenceInfo.IsDisabled = !cbIsEnabled.Checked;

                    conferenceInfo.StartDate = dtbStartDate.DateTime;
                    conferenceInfo.EndDate = dtbEndDate.DateTime;
                    conferenceInfo.Title = PageUtils.FilterXSS(tbTitle.Text);
                    conferenceInfo.ImageUrl = imageUrl.Value; ;
                    conferenceInfo.Summary = tbSummary.Text;

                    conferenceInfo.BackgroundImageUrl = backgroundImageUrl.Value;
                    conferenceInfo.ConferenceName = tbConferenceName.Text;
                    conferenceInfo.Address = tbAddress.Text;
                    conferenceInfo.Duration = tbDuration.Text;
                    conferenceInfo.Introduction = breIntroduction.Text;

                    conferenceInfo.IsAgenda = cbIsAgenda.Checked;
                    conferenceInfo.AgendaTitle = tbAgendaTitle.Text;
                    conferenceInfo.AgendaList = Page.Session["BackgroundConferenceAdd.AgendaList"] as string;
                    Page.Session.Remove("BackgroundConferenceAdd.AgendaList");

                    conferenceInfo.IsGuest = cbIsGuest.Checked;
                    conferenceInfo.GuestTitle = tbGuestTitle.Text;
                    conferenceInfo.GuestList = Page.Session["BackgroundConferenceAdd.GuestList"] as string;
                    Page.Session.Remove("BackgroundConferenceAdd.GuestList");

                    conferenceInfo.EndTitle = tbEndTitle.Text;
                    conferenceInfo.EndImageUrl = endImageUrl.Value;
                    conferenceInfo.EndSummary = tbEndSummary.Text;

                    try
                    {
                        if (conferenceID > 0)
                        {
                            DataProviderWX.ConferenceDAO.Update(conferenceInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "修改会议（活动）",
                                $"会议（活动）:{tbTitle.Text}");
                            SuccessMessage("修改会议（活动）成功！");
                        }
                        else
                        {
                            conferenceID = DataProviderWX.ConferenceDAO.Insert(conferenceInfo);

                            //DataProviderWX.ConferenceItemDAO.UpdateConferenceID(base.PublishmentSystemID, this.conferenceID);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "添加会议（活动）",
                                $"会议（活动）:{tbTitle.Text}");
                            SuccessMessage("添加会议（活动）成功！");
                        }

                        var redirectUrl = PageUtils.GetWXUrl(
                            $"background_conference.aspx?publishmentSystemID={PublishmentSystemID}");
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "会议（活动）设置失败！");
                    }

                    btnSubmit.Visible = false;
                    btnReturn.Visible = false;
                }
			}
		}
	}
}
