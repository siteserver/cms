using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using MessageManager = SiteServer.WeiXin.Core.MessageManager;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundMessageAdd : BackgroundBasePageWX
	{
        public Literal ltlPageTitle;

        public PlaceHolder phStep1;
        public TextBox tbKeywords;
        public TextBox tbTitle;
        public TextBox tbSummary;
        public CheckBox cbIsEnabled;
        public Literal ltlImageUrl;

        public PlaceHolder phStep2;
        public TextBox tbContentDescription;
        public Literal ltlContentImageUrl;

        public HtmlInputHidden imageUrl;
        public HtmlInputHidden contentImageUrl;

        public Button btnSubmit;
        public Button btnReturn;

        private int messageID;

        public static string GetRedirectUrl(int publishmentSystemID, int messageID)
        {
            return PageUtils.GetWXUrl(
                $"background_messageAdd.aspx?publishmentSystemID={publishmentSystemID}&messageID={messageID}");
        }

        public string GetUploadUrl()
        {
            return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            messageID = TranslateUtils.ToInt(GetQueryString("messageID"));

			if (!IsPostBack)
            {
                var pageTitle = messageID > 0 ? "编辑微留言" : "添加微留言";
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Message, pageTitle, AppManager.WeiXin.Permission.WebSite.Message);
                ltlPageTitle.Text = pageTitle;

                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{MessageManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                ltlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{MessageManager.GetContentImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";

                if (messageID > 0)
                {
                    var messageInfo = DataProviderWX.MessageDAO.GetMessageInfo(messageID);

                    tbKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(messageInfo.KeywordID);
                    cbIsEnabled.Checked = !messageInfo.IsDisabled;
                    tbTitle.Text = messageInfo.Title;
                    if (!string.IsNullOrEmpty(messageInfo.ImageUrl))
                    {
                        ltlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, messageInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    tbSummary.Text = messageInfo.Summary;
                    if (!string.IsNullOrEmpty(messageInfo.ContentImageUrl))
                    {
                        ltlContentImageUrl.Text =
                            $@"<img id=""preview_contentImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, messageInfo.ContentImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    tbContentDescription.Text = messageInfo.ContentDescription;

                    imageUrl.Value = messageInfo.ImageUrl;
                    contentImageUrl.Value = messageInfo.ContentImageUrl;
                }

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundMessage.GetRedirectUrl(PublishmentSystemID)}"";return false");
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

                phStep1.Visible = phStep2.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(tbKeywords.Text))
                    {
                        if (messageID > 0)
                        {
                            var messageInfo = DataProviderWX.MessageDAO.GetMessageInfo(messageID);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemID, messageInfo.KeywordID,PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
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
                        btnSubmit.Text = "确 认";
                    }
                }
                else if (selectedStep == 2)
                {
                    var messageInfo = new MessageInfo();
                    if (messageID > 0)
                    {
                        messageInfo = DataProviderWX.MessageDAO.GetMessageInfo(messageID);
                    }
                    messageInfo.PublishmentSystemID = PublishmentSystemID;

                    messageInfo.KeywordID = DataProviderWX.KeywordDAO.GetKeywordID(PublishmentSystemID, messageID > 0,PageUtils.FilterXSS(tbKeywords.Text), EKeywordType.Message, messageInfo.KeywordID);
                    messageInfo.IsDisabled = !cbIsEnabled.Checked;

                    messageInfo.Title =PageUtils.FilterXSS(tbTitle.Text);
                    messageInfo.ImageUrl = imageUrl.Value; ;
                    messageInfo.Summary = tbSummary.Text;

                    messageInfo.ContentImageUrl = contentImageUrl.Value;
                    messageInfo.ContentDescription = tbContentDescription.Text;

                    try
                    {
                        if (messageID > 0)
                        {
                            DataProviderWX.MessageDAO.Update(messageInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "修改微留言",
                                $"微留言:{tbTitle.Text}");
                            SuccessMessage("修改微留言成功！");
                        }
                        else
                        {
                            messageID = DataProviderWX.MessageDAO.Insert(messageInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "添加微留言",
                                $"微留言:{tbTitle.Text}");
                            SuccessMessage("添加微留言成功！");
                        }

                        var redirectUrl = PageUtils.GetWXUrl(
                            $"background_message.aspx?publishmentSystemID={PublishmentSystemID}");
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "微留言设置失败！");
                    }

                    btnSubmit.Visible = false;
                    btnReturn.Visible = false;
                }
			}
		}
	}
}
