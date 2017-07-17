using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using MessageManager = SiteServer.CMS.WeiXin.Manager.MessageManager;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageMessageAdd : BasePageCms
    {
        public Literal LtlPageTitle;

        public PlaceHolder PhStep1;
        public TextBox TbKeywords;
        public TextBox TbTitle;
        public TextBox TbSummary;
        public CheckBox CbIsEnabled;
        public Literal LtlImageUrl;

        public PlaceHolder PhStep2;
        public TextBox TbContentDescription;
        public Literal LtlContentImageUrl;

        public HtmlInputHidden ImageUrl;
        public HtmlInputHidden ContentImageUrl;

        public Button BtnSubmit;
        public Button BtnReturn;

        private int _messageId;

        public static string GetRedirectUrl(int publishmentSystemId, int messageId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageMessageAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"messageId", messageId.ToString()}
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
            _messageId = Body.GetQueryInt("messageID");

			if (!IsPostBack)
            {
                var pageTitle = _messageId > 0 ? "编辑微留言" : "添加微留言";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdMessage, pageTitle, AppManager.WeiXin.Permission.WebSite.Message);
                LtlPageTitle.Text = pageTitle;

                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{MessageManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                LtlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{MessageManager.GetContentImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";

                if (_messageId > 0)
                {
                    var messageInfo = DataProviderWx.MessageDao.GetMessageInfo(_messageId);

                    TbKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(messageInfo.KeywordId);
                    CbIsEnabled.Checked = !messageInfo.IsDisabled;
                    TbTitle.Text = messageInfo.Title;
                    if (!string.IsNullOrEmpty(messageInfo.ImageUrl))
                    {
                        LtlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, messageInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    TbSummary.Text = messageInfo.Summary;
                    if (!string.IsNullOrEmpty(messageInfo.ContentImageUrl))
                    {
                        LtlContentImageUrl.Text =
                            $@"<img id=""preview_contentImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, messageInfo.ContentImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    TbContentDescription.Text = messageInfo.ContentDescription;

                    ImageUrl.Value = messageInfo.ImageUrl;
                    ContentImageUrl.Value = messageInfo.ContentImageUrl;
                }

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageMessage.GetRedirectUrl(PublishmentSystemId)}"";return false");
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

                PhStep1.Visible = PhStep2.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(TbKeywords.Text))
                    {
                        if (_messageId > 0)
                        {
                            var messageInfo = DataProviderWx.MessageDao.GetMessageInfo(_messageId);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, messageInfo.KeywordId,PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
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
                        BtnSubmit.Text = "确 认";
                    }
                }
                else if (selectedStep == 2)
                {
                    var messageInfo = new MessageInfo();
                    if (_messageId > 0)
                    {
                        messageInfo = DataProviderWx.MessageDao.GetMessageInfo(_messageId);
                    }
                    messageInfo.PublishmentSystemId = PublishmentSystemId;

                    messageInfo.KeywordId = DataProviderWx.KeywordDao.GetKeywordId(PublishmentSystemId, _messageId > 0,PageUtils.FilterXss(TbKeywords.Text), EKeywordType.Message, messageInfo.KeywordId);
                    messageInfo.IsDisabled = !CbIsEnabled.Checked;

                    messageInfo.Title =PageUtils.FilterXss(TbTitle.Text);
                    messageInfo.ImageUrl = ImageUrl.Value; ;
                    messageInfo.Summary = TbSummary.Text;

                    messageInfo.ContentImageUrl = ContentImageUrl.Value;
                    messageInfo.ContentDescription = TbContentDescription.Text;

                    try
                    {
                        if (_messageId > 0)
                        {
                            DataProviderWx.MessageDao.Update(messageInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "修改微留言", $"微留言:{TbTitle.Text}");
                            SuccessMessage("修改微留言成功！");
                        }
                        else
                        {
                            _messageId = DataProviderWx.MessageDao.Insert(messageInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "添加微留言", $"微留言:{TbTitle.Text}");
                            SuccessMessage("添加微留言成功！");
                        }

                        var redirectUrl = PageMessage.GetRedirectUrl(PublishmentSystemId);
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "微留言设置失败！");
                    }

                    BtnSubmit.Visible = false;
                    BtnReturn.Visible = false;
                }
			}
		}
	}
}
