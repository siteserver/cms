using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
	public class PageView360Add : BasePageCms
    {
        public Literal LtlPageTitle;

        public PlaceHolder PhStep1;
        public TextBox TbKeywords;
        public TextBox TbTitle;
        public TextBox TbSummary;
        public CheckBox CbIsEnabled;
        public Literal LtlImageUrl;

        public PlaceHolder PhStep2;
        public TextBox TbContentImageUrl1;
        public TextBox TbContentImageUrl2;
        public TextBox TbContentImageUrl3;
        public TextBox TbContentImageUrl4;
        public TextBox TbContentImageUrl5;
        public TextBox TbContentImageUrl6;
        public Literal LtlContentImageUrl1;
        public Literal LtlContentImageUrl2;
        public Literal LtlContentImageUrl3;
        public Literal LtlContentImageUrl4;
        public Literal LtlContentImageUrl5;
        public Literal LtlContentImageUrl6;

        public HtmlInputHidden ImageUrl;

        public Button BtnSubmit;
        public Button BtnReturn;

        private int _view360Id;

        public static string GetRedirectUrl(int publishmentSystemId, int view360Id)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageView360Add), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"view360Id", view360Id.ToString()}
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
            _view360Id = Body.GetQueryInt("view360ID");

			if (!IsPostBack)
            {
                var pageTitle = _view360Id > 0 ? "编辑360全景" : "添加360全景";

                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdView360, pageTitle, AppManager.WeiXin.Permission.WebSite.View360);
                LtlPageTitle.Text = pageTitle;

                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{View360Manager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";

                var selectImageClick = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, TbContentImageUrl1.ClientID);
                var uploadImageClick = ModalUploadImageSingle.GetOpenWindowStringToTextBox(PublishmentSystemId, TbContentImageUrl1.ClientID);
                var cuttingImageClick = ModalCuttingImage.GetOpenWindowStringWithTextBox(PublishmentSystemId, TbContentImageUrl1.ClientID);
                var previewImageClick = ModalMessage.GetOpenWindowStringToPreviewImage(PublishmentSystemId, TbContentImageUrl1.ClientID);
                LtlContentImageUrl1.Text = $@"
                      <a class=""btn"" href=""javascript:;"" onclick=""{selectImageClick};return false;"" title=""选择""><i class=""icon-th""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{uploadImageClick};return false;"" title=""上传""><i class=""icon-arrow-up""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{cuttingImageClick};return false;"" title=""裁切""><i class=""icon-crop""></i></a>
                      <a class=""btn"" href=""javascript:;"" onclick=""{previewImageClick};return false;"" title=""预览""><i class=""icon-eye-open""></i></a>";

                LtlContentImageUrl2.Text = LtlContentImageUrl1.Text.Replace(TbContentImageUrl1.ClientID, TbContentImageUrl2.ClientID);
                LtlContentImageUrl3.Text = LtlContentImageUrl1.Text.Replace(TbContentImageUrl1.ClientID, TbContentImageUrl3.ClientID);
                LtlContentImageUrl4.Text = LtlContentImageUrl1.Text.Replace(TbContentImageUrl1.ClientID, TbContentImageUrl4.ClientID);
                LtlContentImageUrl5.Text = LtlContentImageUrl1.Text.Replace(TbContentImageUrl1.ClientID, TbContentImageUrl5.ClientID);
                LtlContentImageUrl6.Text = LtlContentImageUrl1.Text.Replace(TbContentImageUrl1.ClientID, TbContentImageUrl6.ClientID);

                if (_view360Id == 0)
                {
                    TbContentImageUrl1.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, string.Empty, 1);
                    TbContentImageUrl2.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, string.Empty, 2);
                    TbContentImageUrl3.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, string.Empty, 3);
                    TbContentImageUrl4.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, string.Empty, 4);
                    TbContentImageUrl5.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, string.Empty, 5);
                    TbContentImageUrl6.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, string.Empty, 6);
                }
                else
                {
                    var view360Info = DataProviderWx.View360Dao.GetView360Info(_view360Id);

                    TbKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(view360Info.KeywordId);
                    CbIsEnabled.Checked = !view360Info.IsDisabled;
                    TbTitle.Text = view360Info.Title;
                    if (!string.IsNullOrEmpty(view360Info.ImageUrl))
                    {
                        LtlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, view360Info.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    TbSummary.Text = view360Info.Summary;

                    TbContentImageUrl1.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, view360Info.ContentImageUrl1, 1);
                    TbContentImageUrl2.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, view360Info.ContentImageUrl2, 2);
                    TbContentImageUrl3.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, view360Info.ContentImageUrl3, 3);
                    TbContentImageUrl4.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, view360Info.ContentImageUrl4, 4);
                    TbContentImageUrl5.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, view360Info.ContentImageUrl5, 5);
                    TbContentImageUrl6.Text = View360Manager.GetContentImageUrl(PublishmentSystemInfo, view360Info.ContentImageUrl6, 6);

                    ImageUrl.Value = view360Info.ImageUrl;
                }

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageView360.GetRedirectUrl(PublishmentSystemId)}"";return false");
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
                        if (_view360Id > 0)
                        {
                            var view360Info = DataProviderWx.View360Dao.GetView360Info(_view360Id);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, view360Info.KeywordId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
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
                    var view360Info = new View360Info();
                    if (_view360Id > 0)
                    {
                        view360Info = DataProviderWx.View360Dao.GetView360Info(_view360Id);
                    }
                    view360Info.PublishmentSystemId = PublishmentSystemId;

                    view360Info.KeywordId = DataProviderWx.KeywordDao.GetKeywordId(PublishmentSystemId, _view360Id > 0, TbKeywords.Text, EKeywordType.View360, view360Info.KeywordId);
                    view360Info.IsDisabled = !CbIsEnabled.Checked;

                    view360Info.Title = PageUtils.FilterXss(TbTitle.Text);
                    view360Info.ImageUrl = ImageUrl.Value; ;
                    view360Info.Summary = TbSummary.Text;

                    view360Info.ContentImageUrl1 = TbContentImageUrl1.Text;
                    view360Info.ContentImageUrl2 = TbContentImageUrl2.Text;
                    view360Info.ContentImageUrl3 = TbContentImageUrl3.Text;
                    view360Info.ContentImageUrl4 = TbContentImageUrl4.Text;
                    view360Info.ContentImageUrl5 = TbContentImageUrl5.Text;
                    view360Info.ContentImageUrl6 = TbContentImageUrl6.Text;

                    try
                    {
                        if (_view360Id > 0)
                        {
                            DataProviderWx.View360Dao.Update(view360Info);

                            LogUtils.AddAdminLog(Body.AdministratorName, "修改360全景", $"360全景:{TbTitle.Text}");
                            SuccessMessage("修改360全景成功！");
                        }
                        else
                        {
                            _view360Id = DataProviderWx.View360Dao.Insert(view360Info);

                            LogUtils.AddAdminLog(Body.AdministratorName, "添加360全景", $"360全景:{TbTitle.Text}");
                            SuccessMessage("添加360全景成功！");
                        }

                        var redirectUrl = PageView360.GetRedirectUrl(PublishmentSystemId);
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "360全景设置失败！");
                    }

                    BtnSubmit.Visible = false;
                    BtnReturn.Visible = false;
                }
			}
		}
	}
}
