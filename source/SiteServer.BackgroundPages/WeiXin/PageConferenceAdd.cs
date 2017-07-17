using System;
using System.Collections.Generic;
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
using System.Collections.Specialized;
using SiteServer.BackgroundPages.Cms;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageConferenceAdd : BasePageCms
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
        public Literal LtlBackgroundImageUrl;
        public TextBox TbConferenceName;
        public TextBox TbAddress;
        public TextBox TbDuration;
        public UEditor BreIntroduction;

        public PlaceHolder PhStep3;
        public CheckBox CbIsAgenda;
        public TextBox TbAgendaTitle;
        public Literal LtlAgendaScript;

        public PlaceHolder PhStep4;
        public CheckBox CbIsGuest;
        public TextBox TbGuestTitle;
        public Literal LtlGuestScript;

        public PlaceHolder PhStep5;
        public TextBox TbEndTitle;
        public TextBox TbEndSummary;
        public Literal LtlEndImageUrl;

        public HtmlInputHidden ImageUrl;
        public HtmlInputHidden BackgroundImageUrl;
        public HtmlInputHidden EndImageUrl;

        public Button BtnSubmit;
        public Button BtnReturn;

        private int _conferenceId;

        public static string GetRedirectUrl(int publishmentSystemId, int conferenceId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageConferenceAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"conferenceId", conferenceId.ToString()}
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
            _conferenceId = Body.GetQueryInt("conferenceID");

			if (!IsPostBack)
            {
                var pageTitle = _conferenceId > 0 ? "编辑会议（活动）" : "添加会议（活动）";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdConference, pageTitle, AppManager.WeiXin.Permission.WebSite.Conference);
                LtlPageTitle.Text = pageTitle;
                
                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{ConferenceManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                LtlBackgroundImageUrl.Text =
                    $@"{ComponentsManager.GetBackgroundImageSelectHtml(PublishmentSystemInfo, string.Empty)}<hr /><img id=""preview_backgroundImageUrl"" src=""{ComponentsManager
                        .GetBackgroundImageUrl(PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                LtlEndImageUrl.Text =
                    $@"<img id=""preview_endImageUrl"" src=""{ConferenceManager.GetEndImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";

                var selectImageClick = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, "itemPicUrl_");
                var uploadImageClick = ModalUploadImageSingle.GetOpenWindowStringToTextBox(PublishmentSystemId, "itemPicUrl_");
                var cuttingImageClick = ModalCuttingImage.GetOpenWindowStringWithTextBox(PublishmentSystemId, "itemPicUrl_");
                var previewImageClick = ModalMessage.GetOpenWindowStringToPreviewImage(PublishmentSystemId, "itemPicUrl_");
                LtlGuestScript.Text =
                    $@"guestController.selectImageClickString = ""{selectImageClick}"";guestController.uploadImageClickString = ""{uploadImageClick}"";guestController.cuttingImageClickString = ""{cuttingImageClick}"";guestController.previewImageClickString = ""{previewImageClick}"";";

                if (_conferenceId == 0)
                {
                    LtlAgendaScript.Text += "agendaController.agendaCount = 2;agendaController.items = [{}, {}];";
                    LtlGuestScript.Text += "guestController.guestCount = 2;guestController.items = [{}, {}];";
                    DtbEndDate.DateTime = DateTime.Now.AddMonths(1);
                }
                else
                {
                    var conferenceInfo = DataProviderWx.ConferenceDao.GetConferenceInfo(_conferenceId);

                    TbKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(conferenceInfo.KeywordId);
                    CbIsEnabled.Checked = !conferenceInfo.IsDisabled;
                    DtbStartDate.DateTime = conferenceInfo.StartDate;
                    DtbEndDate.DateTime = conferenceInfo.EndDate;
                    TbTitle.Text = conferenceInfo.Title;
                    if (!string.IsNullOrEmpty(conferenceInfo.ImageUrl))
                    {
                        LtlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, conferenceInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    TbSummary.Text = conferenceInfo.Summary;
                    if (!string.IsNullOrEmpty(conferenceInfo.BackgroundImageUrl))
                    {
                        LtlBackgroundImageUrl.Text =
                            $@"{ComponentsManager.GetBackgroundImageSelectHtml(PublishmentSystemInfo, conferenceInfo.BackgroundImageUrl)}<hr /><img id=""preview_backgroundImageUrl"" src=""{ComponentsManager
                                .GetBackgroundImageUrl(PublishmentSystemInfo, conferenceInfo.BackgroundImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    TbConferenceName.Text = conferenceInfo.ConferenceName;
                    TbAddress.Text = conferenceInfo.Address;
                    TbDuration.Text = conferenceInfo.Duration;
                    BreIntroduction.Text = conferenceInfo.Introduction;

                    CbIsAgenda.Checked = conferenceInfo.IsAgenda;
                    TbAgendaTitle.Text = conferenceInfo.AgendaTitle;
                    var agendaInfoList = new List<ConferenceAgendaInfo>();
                    agendaInfoList = TranslateUtils.JsonToObject(conferenceInfo.AgendaList, agendaInfoList) as List<ConferenceAgendaInfo>;
                    if (agendaInfoList != null)
                    {
                        var agendaBuilder = new StringBuilder();
                        foreach (var agendaInfo in agendaInfoList)
                        {
                            agendaBuilder.AppendFormat(@"{{dateTime: '{0}', title: '{1}', summary: '{2}'}},", agendaInfo.DateTime, agendaInfo.Title, agendaInfo.Summary);
                        }
                        if (agendaBuilder.Length > 0) agendaBuilder.Length--;

                        LtlAgendaScript.Text +=
                            $@"agendaController.agendaCount = {agendaInfoList.Count};agendaController.items = [{agendaBuilder}];";
                    }
                    else
                    {
                        LtlAgendaScript.Text += "agendaController.agendaCount = 0;agendaController.items = [{}];";
                    }

                    CbIsGuest.Checked = conferenceInfo.IsGuest;
                    TbGuestTitle.Text = conferenceInfo.GuestTitle;
                    var guestInfoList = new List<ConferenceGuestInfo>();
                    guestInfoList = TranslateUtils.JsonToObject(conferenceInfo.GuestList, guestInfoList) as List<ConferenceGuestInfo>;
                    if (guestInfoList != null)
                    {
                        var guestBuilder = new StringBuilder();
                        foreach (var guestInfo in guestInfoList)
                        {
                            guestBuilder.AppendFormat(@"{{displayName: '{0}', position: '{1}', picUrl: '{2}'}},", guestInfo.DisplayName, guestInfo.Position, guestInfo.PicUrl);
                        }
                        if (guestBuilder.Length > 0) guestBuilder.Length--;

                        LtlGuestScript.Text +=
                            $@"guestController.guestCount = {guestInfoList.Count};guestController.items = [{guestBuilder}];";
                    }
                    else
                    {
                        LtlGuestScript.Text += "guestController.guestCount = 0;guestController.items = [{}];";
                    }

                    TbEndTitle.Text = conferenceInfo.EndTitle;
                    TbEndSummary.Text = conferenceInfo.EndSummary;
                    if (!string.IsNullOrEmpty(conferenceInfo.EndImageUrl))
                    {
                        LtlEndImageUrl.Text =
                            $@"<img id=""preview_endImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, conferenceInfo.EndImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    ImageUrl.Value = conferenceInfo.ImageUrl;
                    BackgroundImageUrl.Value = conferenceInfo.BackgroundImageUrl;
                    EndImageUrl.Value = conferenceInfo.EndImageUrl;
                }

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageConference.GetRedirectUrl(PublishmentSystemId)}"";return false");
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
                        if (_conferenceId > 0)
                        {
                            var conferenceInfo = DataProviderWx.ConferenceDao.GetConferenceInfo(_conferenceId);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, conferenceInfo.KeywordId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
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
                    var agendaCount = TranslateUtils.ToInt(Request.Form["agendaCount"]);

                    if (CbIsAgenda.Checked && agendaCount < 2)
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

                            var agendaInfo = new ConferenceAgendaInfo { DateTime = dateTime, Title = title, Summary = summary };

                            agendaInfoList.Add(agendaInfo);
                        }

                        if (isItemReady)
                        {
                            Page.Session.Add("BackgroundConferenceAdd.AgendaList", TranslateUtils.ObjectToJson(agendaInfoList));
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
                    var isItemReady = true;
                    var guestCount = TranslateUtils.ToInt(Request.Form["guestCount"]);

                    if (CbIsGuest.Checked && guestCount < 2)
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

                            var guestInfo = new ConferenceGuestInfo { DisplayName = displayName, Position = position, PicUrl = picUrl };

                            guestInfoList.Add(guestInfo);
                        }

                        if (isItemReady)
                        {
                            Page.Session.Add("BackgroundConferenceAdd.GuestList", TranslateUtils.ObjectToJson(guestInfoList));
                        }
                    }

                    if (isItemReady)
                    {
                        PhStep5.Visible = true;
                        BtnSubmit.Text = "确 认";
                    }
                    else
                    {
                        PhStep4.Visible = true;
                    }
                }
                else if (selectedStep == 5)
                {
                    var conferenceInfo = new ConferenceInfo();
                    if (_conferenceId > 0)
                    {
                        conferenceInfo = DataProviderWx.ConferenceDao.GetConferenceInfo(_conferenceId);
                    }

                    conferenceInfo.PublishmentSystemId = PublishmentSystemId;

                    conferenceInfo.KeywordId = DataProviderWx.KeywordDao.GetKeywordId(PublishmentSystemId, _conferenceId > 0, TbKeywords.Text, EKeywordType.Conference, conferenceInfo.KeywordId);
                    conferenceInfo.IsDisabled = !CbIsEnabled.Checked;

                    conferenceInfo.StartDate = DtbStartDate.DateTime;
                    conferenceInfo.EndDate = DtbEndDate.DateTime;
                    conferenceInfo.Title = PageUtils.FilterXss(TbTitle.Text);
                    conferenceInfo.ImageUrl = ImageUrl.Value; ;
                    conferenceInfo.Summary = TbSummary.Text;

                    conferenceInfo.BackgroundImageUrl = BackgroundImageUrl.Value;
                    conferenceInfo.ConferenceName = TbConferenceName.Text;
                    conferenceInfo.Address = TbAddress.Text;
                    conferenceInfo.Duration = TbDuration.Text;
                    conferenceInfo.Introduction = BreIntroduction.Text;

                    conferenceInfo.IsAgenda = CbIsAgenda.Checked;
                    conferenceInfo.AgendaTitle = TbAgendaTitle.Text;
                    conferenceInfo.AgendaList = Page.Session["BackgroundConferenceAdd.AgendaList"] as string;
                    Page.Session.Remove("BackgroundConferenceAdd.AgendaList");

                    conferenceInfo.IsGuest = CbIsGuest.Checked;
                    conferenceInfo.GuestTitle = TbGuestTitle.Text;
                    conferenceInfo.GuestList = Page.Session["BackgroundConferenceAdd.GuestList"] as string;
                    Page.Session.Remove("BackgroundConferenceAdd.GuestList");

                    conferenceInfo.EndTitle = TbEndTitle.Text;
                    conferenceInfo.EndImageUrl = EndImageUrl.Value;
                    conferenceInfo.EndSummary = TbEndSummary.Text;

                    try
                    {
                        if (_conferenceId > 0)
                        {
                            DataProviderWx.ConferenceDao.Update(conferenceInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "修改会议（活动）", $"会议（活动）:{TbTitle.Text}");
                            SuccessMessage("修改会议（活动）成功！");
                        }
                        else
                        {
                            _conferenceId = DataProviderWx.ConferenceDao.Insert(conferenceInfo);

                            //DataProviderWx.ConferenceItemDao.UpdateConferenceID(base.PublishmentSystemId, this.conferenceID);

                            LogUtils.AddAdminLog(Body.AdministratorName, "添加会议（活动）", $"会议（活动）:{TbTitle.Text}");
                            SuccessMessage("添加会议（活动）成功！");
                        }

                        var redirectUrl = PageConference.GetRedirectUrl(PublishmentSystemId);
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "会议（活动）设置失败！");
                    }

                    BtnSubmit.Visible = false;
                    BtnReturn.Visible = false;
                }
			}
		}
	}
}
