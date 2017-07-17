using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
	public class PageCollectAdd : BasePageCms
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
        public TextBox TbContentDescription;
        public TextBox TbContentMaxVote;
        public DropDownList DdlContentIsCheck;
        public Literal LtlContentImageUrl;

        public PlaceHolder PhStep3;
        public TextBox TbEndTitle;
        public TextBox TbEndSummary;
        public Literal LtlEndImageUrl;

        public HtmlInputHidden ImageUrl;
        public HtmlInputHidden ContentImageUrl;
        public HtmlInputHidden EndImageUrl;

        public Button BtnSubmit;
        public Button BtnReturn;

        private int _collectId;

        public static string GetRedirectUrl(int publishmentSystemId, int collectId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageCollectAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"collectId", collectId.ToString()}
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
            _collectId = Body.GetQueryInt("collectID");

			if (!IsPostBack)
            {
                var pageTitle = _collectId > 0 ? "编辑征集活动" : "添加征集活动";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdCollect, pageTitle, AppManager.WeiXin.Permission.WebSite.Collect);
                LtlPageTitle.Text = pageTitle;

                EBooleanUtils.AddListItems(DdlContentIsCheck, "需要审核", "无需审核");
                ControlUtils.SelectListItems(DdlContentIsCheck, false.ToString());

                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{CollectManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                LtlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{CollectManager.GetContentImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                LtlEndImageUrl.Text =
                    $@"<img id=""preview_endImageUrl"" src=""{CollectManager.GetEndImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";

                if (_collectId == 0)
                {
                    DtbEndDate.DateTime = DateTime.Now.AddMonths(1);
                }
                else
                {
                    var collectInfo = DataProviderWx.CollectDao.GetCollectInfo(_collectId);

                    TbKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(collectInfo.KeywordId);
                    CbIsEnabled.Checked = !collectInfo.IsDisabled;
                    DtbStartDate.DateTime = collectInfo.StartDate;
                    DtbEndDate.DateTime = collectInfo.EndDate;
                    TbTitle.Text = collectInfo.Title;
                    if (!string.IsNullOrEmpty(collectInfo.ImageUrl))
                    {
                        LtlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, collectInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    TbSummary.Text = collectInfo.Summary;
                    if (!string.IsNullOrEmpty(collectInfo.ContentImageUrl))
                    {
                        LtlContentImageUrl.Text =
                            $@"<img id=""preview_contentImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, collectInfo.ContentImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    TbContentDescription.Text = collectInfo.ContentDescription;

                    TbContentMaxVote.Text = collectInfo.ContentMaxVote.ToString();
                    ControlUtils.SelectListItems(DdlContentIsCheck, collectInfo.ContentIsCheck.ToString());

                    TbEndTitle.Text = collectInfo.EndTitle;
                    TbEndSummary.Text = collectInfo.EndSummary;
                    if (!string.IsNullOrEmpty(collectInfo.EndImageUrl))
                    {
                        LtlEndImageUrl.Text =
                            $@"<img id=""preview_endImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, collectInfo.EndImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    ImageUrl.Value = collectInfo.ImageUrl;
                    ContentImageUrl.Value = collectInfo.ContentImageUrl;
                    EndImageUrl.Value = collectInfo.EndImageUrl;
                }

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageCollect.GetRedirectUrl(PublishmentSystemId)}"";return false");
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

                PhStep1.Visible = PhStep2.Visible = PhStep3.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(TbKeywords.Text))
                    {
                        if (_collectId > 0)
                        {
                            var collectInfo = DataProviderWx.CollectDao.GetCollectInfo(_collectId);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, collectInfo.KeywordId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
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
                    BtnSubmit.Text = "确 认";
                }
                else if (selectedStep == 3)
                {
                    var collectInfo = new CollectInfo();
                    if (_collectId > 0)
                    {
                        collectInfo = DataProviderWx.CollectDao.GetCollectInfo(_collectId);
                    }
                    collectInfo.PublishmentSystemId = PublishmentSystemId;

                    collectInfo.KeywordId = DataProviderWx.KeywordDao.GetKeywordId(PublishmentSystemId, _collectId > 0, TbKeywords.Text, EKeywordType.Collect, collectInfo.KeywordId);
                    collectInfo.IsDisabled = !CbIsEnabled.Checked;

                    collectInfo.StartDate = DtbStartDate.DateTime;
                    collectInfo.EndDate = DtbEndDate.DateTime;
                    collectInfo.Title = PageUtils.FilterXss(TbTitle.Text);
                    collectInfo.ImageUrl = ImageUrl.Value; ;
                    collectInfo.Summary = TbSummary.Text;

                    collectInfo.ContentImageUrl = ContentImageUrl.Value;
                    collectInfo.ContentDescription = TbContentDescription.Text;
                    collectInfo.ContentMaxVote = TranslateUtils.ToInt(TbContentMaxVote.Text);
                    collectInfo.ContentIsCheck = TranslateUtils.ToBool(DdlContentIsCheck.SelectedValue);

                    collectInfo.EndTitle = TbEndTitle.Text;
                    collectInfo.EndImageUrl = EndImageUrl.Value;
                    collectInfo.EndSummary = TbEndSummary.Text;

                    try
                    {
                        if (_collectId > 0)
                        {
                            DataProviderWx.CollectDao.Update(collectInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "修改征集活动", $"征集活动:{TbTitle.Text}");
                            SuccessMessage("修改征集活动成功！");
                        }
                        else
                        {
                            _collectId = DataProviderWx.CollectDao.Insert(collectInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "添加征集活动", $"征集活动:{TbTitle.Text}");
                            SuccessMessage("添加征集活动成功！");
                        }

                        var redirectUrl = PageCollect.GetRedirectUrl(PublishmentSystemId);
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "征集活动设置失败！");
                    }

                    BtnSubmit.Visible = false;
                    BtnReturn.Visible = false;
                }
			}
		}
	}
}
