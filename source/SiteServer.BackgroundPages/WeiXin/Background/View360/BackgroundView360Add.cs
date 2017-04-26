using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using View360Manager = SiteServer.WeiXin.Core.View360Manager;

namespace SiteServer.WeiXin.BackgroundPages
{
	public class BackgroundView360Add : BackgroundBasePageWX
	{
        public Literal ltlPageTitle;

        public PlaceHolder phStep1;
        public TextBox tbKeywords;
        public TextBox tbTitle;
        public TextBox tbSummary;
        public CheckBox cbIsEnabled;
        public Literal ltlImageUrl;

        public PlaceHolder phStep2;
        public TextBox tbContentImageUrl1;
        public TextBox tbContentImageUrl2;
        public TextBox tbContentImageUrl3;
        public TextBox tbContentImageUrl4;
        public TextBox tbContentImageUrl5;
        public TextBox tbContentImageUrl6;
        public Literal ltlContentImageUrl1;
        public Literal ltlContentImageUrl2;
        public Literal ltlContentImageUrl3;
        public Literal ltlContentImageUrl4;
        public Literal ltlContentImageUrl5;
        public Literal ltlContentImageUrl6;

        public HtmlInputHidden imageUrl;

        public Button btnSubmit;
        public Button btnReturn;

        private int view360ID;

        public static string GetRedirectUrl(int publishmentSystemID, int view360ID)
        {
            return PageUtils.GetWXUrl(
                $"background_view360Add.aspx?publishmentSystemID={publishmentSystemID}&view360ID={view360ID}");
        }

        public string GetUploadUrl()
        {
            return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            view360ID = TranslateUtils.ToInt(GetQueryString("view360ID"));

			if (!IsPostBack)
            {
                var pageTitle = view360ID > 0 ? "编辑360全景" : "添加360全景";

                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_View360, pageTitle, AppManager.WeiXin.Permission.WebSite.View360);
                ltlPageTitle.Text = pageTitle;

                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{View360Manager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";

                var selectImageClick = CMS.BackgroundPages.Modal.SelectImage.GetOpenWindowString(PublishmentSystemInfo, tbContentImageUrl1.ClientID);
                var uploadImageClick = CMS.BackgroundPages.Modal.UploadImageSingle.GetOpenWindowStringToTextBox(PublishmentSystemID, tbContentImageUrl1.ClientID);
                var cuttingImageClick = CMS.BackgroundPages.Modal.CuttingImage.GetOpenWindowStringWithTextBox(PublishmentSystemID, tbContentImageUrl1.ClientID);
                var previewImageClick = CMS.BackgroundPages.Modal.Message.GetOpenWindowStringToPreviewImage(PublishmentSystemID, tbContentImageUrl1.ClientID);
                ltlContentImageUrl1.Text = $@"
                      <a class=""btn"" href=""javascript:;"" onclick=""{selectImageClick};return false;"" title=""选择""><i class=""icon-th""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{uploadImageClick};return false;"" title=""上传""><i class=""icon-arrow-up""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{cuttingImageClick};return false;"" title=""裁切""><i class=""icon-crop""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{previewImageClick};return false;"" title=""预览""><i class=""icon-eye-open""></i></a>";

                ltlContentImageUrl2.Text = ltlContentImageUrl1.Text.Replace(tbContentImageUrl1.ClientID, tbContentImageUrl2.ClientID);
                ltlContentImageUrl3.Text = ltlContentImageUrl1.Text.Replace(tbContentImageUrl1.ClientID, tbContentImageUrl3.ClientID);
                ltlContentImageUrl4.Text = ltlContentImageUrl1.Text.Replace(tbContentImageUrl1.ClientID, tbContentImageUrl4.ClientID);
                ltlContentImageUrl5.Text = ltlContentImageUrl1.Text.Replace(tbContentImageUrl1.ClientID, tbContentImageUrl5.ClientID);
                ltlContentImageUrl6.Text = ltlContentImageUrl1.Text.Replace(tbContentImageUrl1.ClientID, tbContentImageUrl6.ClientID);

                if (view360ID == 0)
                {
                    tbContentImageUrl1.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, string.Empty, 1);
                    tbContentImageUrl2.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, string.Empty, 2);
                    tbContentImageUrl3.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, string.Empty, 3);
                    tbContentImageUrl4.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, string.Empty, 4);
                    tbContentImageUrl5.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, string.Empty, 5);
                    tbContentImageUrl6.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, string.Empty, 6);
                }
                else
                {
                    var view360Info = DataProviderWX.View360DAO.GetView360Info(view360ID);

                    tbKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(view360Info.KeywordID);
                    cbIsEnabled.Checked = !view360Info.IsDisabled;
                    tbTitle.Text = view360Info.Title;
                    if (!string.IsNullOrEmpty(view360Info.ImageUrl))
                    {
                        ltlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, view360Info.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    tbSummary.Text = view360Info.Summary;

                    tbContentImageUrl1.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, view360Info.ContentImageUrl1, 1);
                    tbContentImageUrl2.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, view360Info.ContentImageUrl2, 2);
                    tbContentImageUrl3.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, view360Info.ContentImageUrl3, 3);
                    tbContentImageUrl4.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, view360Info.ContentImageUrl4, 4);
                    tbContentImageUrl5.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, view360Info.ContentImageUrl5, 5);
                    tbContentImageUrl6.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, view360Info.ContentImageUrl6, 6);

                    imageUrl.Value = view360Info.ImageUrl;
                }

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundView360.GetRedirectUrl(PublishmentSystemID)}"";return false");
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
                        if (view360ID > 0)
                        {
                            var view360Info = DataProviderWX.View360DAO.GetView360Info(view360ID);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemID, view360Info.KeywordID, PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
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
                    var view360Info = new View360Info();
                    if (view360ID > 0)
                    {
                        view360Info = DataProviderWX.View360DAO.GetView360Info(view360ID);
                    }
                    view360Info.PublishmentSystemID = PublishmentSystemID;

                    view360Info.KeywordID = DataProviderWX.KeywordDAO.GetKeywordID(PublishmentSystemID, view360ID > 0, tbKeywords.Text, EKeywordType.View360, view360Info.KeywordID);
                    view360Info.IsDisabled = !cbIsEnabled.Checked;

                    view360Info.Title = PageUtils.FilterXSS(tbTitle.Text);
                    view360Info.ImageUrl = imageUrl.Value; ;
                    view360Info.Summary = tbSummary.Text;

                    view360Info.ContentImageUrl1 = tbContentImageUrl1.Text;
                    view360Info.ContentImageUrl2 = tbContentImageUrl2.Text;
                    view360Info.ContentImageUrl3 = tbContentImageUrl3.Text;
                    view360Info.ContentImageUrl4 = tbContentImageUrl4.Text;
                    view360Info.ContentImageUrl5 = tbContentImageUrl5.Text;
                    view360Info.ContentImageUrl6 = tbContentImageUrl6.Text;

                    try
                    {
                        if (view360ID > 0)
                        {
                            DataProviderWX.View360DAO.Update(view360Info);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "修改360全景",
                                $"360全景:{tbTitle.Text}");
                            SuccessMessage("修改360全景成功！");
                        }
                        else
                        {
                            view360ID = DataProviderWX.View360DAO.Insert(view360Info);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "添加360全景",
                                $"360全景:{tbTitle.Text}");
                            SuccessMessage("添加360全景成功！");
                        }

                        var redirectUrl = PageUtils.GetWXUrl(
                            $"background_view360.aspx?publishmentSystemID={PublishmentSystemID}");
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "360全景设置失败！");
                    }

                    btnSubmit.Visible = false;
                    btnReturn.Visible = false;
                }
			}
		}
	}
}
