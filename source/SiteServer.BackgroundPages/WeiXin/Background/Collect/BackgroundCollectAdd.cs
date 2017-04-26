using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using BaiRong.Controls;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.WeiXin.BackgroundPages
{
	public class BackgroundCollectAdd : BackgroundBasePageWX
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
        public TextBox tbContentDescription;
        public TextBox tbContentMaxVote;
        public DropDownList ddlContentIsCheck;
        public Literal ltlContentImageUrl;

        public PlaceHolder phStep3;
        public TextBox tbEndTitle;
        public TextBox tbEndSummary;
        public Literal ltlEndImageUrl;

        public HtmlInputHidden imageUrl;
        public HtmlInputHidden contentImageUrl;
        public HtmlInputHidden endImageUrl;

        public Button btnSubmit;
        public Button btnReturn;

        private int collectID;

        public static string GetRedirectUrl(int publishmentSystemID, int collectID)
        {
            return PageUtils.GetWXUrl(
                $"background_collectAdd.aspx?publishmentSystemID={publishmentSystemID}&collectID={collectID}");
        }

        public string GetUploadUrl()
        {
            return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            collectID = TranslateUtils.ToInt(GetQueryString("collectID"));

			if (!IsPostBack)
            {
                var pageTitle = collectID > 0 ? "编辑征集活动" : "添加征集活动";
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Collect, pageTitle, AppManager.WeiXin.Permission.WebSite.Collect);
                ltlPageTitle.Text = pageTitle;

                EBooleanUtils.AddListItems(ddlContentIsCheck, "需要审核", "无需审核");
                ControlUtils.SelectListItems(ddlContentIsCheck, false.ToString());

                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{CollectManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";
                ltlContentImageUrl.Text =
                    $@"<img id=""preview_contentImageUrl"" src=""{CollectManager.GetContentImageUrl(
                        PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";
                ltlEndImageUrl.Text =
                    $@"<img id=""preview_endImageUrl"" src=""{CollectManager.GetEndImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";

                if (collectID == 0)
                {
                    dtbEndDate.DateTime = DateTime.Now.AddMonths(1);
                }
                else
                {
                    var collectInfo = DataProviderWX.CollectDAO.GetCollectInfo(collectID);

                    tbKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(collectInfo.KeywordID);
                    cbIsEnabled.Checked = !collectInfo.IsDisabled;
                    dtbStartDate.DateTime = collectInfo.StartDate;
                    dtbEndDate.DateTime = collectInfo.EndDate;
                    tbTitle.Text = collectInfo.Title;
                    if (!string.IsNullOrEmpty(collectInfo.ImageUrl))
                    {
                        ltlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, collectInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    tbSummary.Text = collectInfo.Summary;
                    if (!string.IsNullOrEmpty(collectInfo.ContentImageUrl))
                    {
                        ltlContentImageUrl.Text =
                            $@"<img id=""preview_contentImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, collectInfo.ContentImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    tbContentDescription.Text = collectInfo.ContentDescription;

                    tbContentMaxVote.Text = collectInfo.ContentMaxVote.ToString();
                    ControlUtils.SelectListItems(ddlContentIsCheck, collectInfo.ContentIsCheck.ToString());

                    tbEndTitle.Text = collectInfo.EndTitle;
                    tbEndSummary.Text = collectInfo.EndSummary;
                    if (!string.IsNullOrEmpty(collectInfo.EndImageUrl))
                    {
                        ltlEndImageUrl.Text =
                            $@"<img id=""preview_endImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, collectInfo.EndImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    imageUrl.Value = collectInfo.ImageUrl;
                    contentImageUrl.Value = collectInfo.ContentImageUrl;
                    endImageUrl.Value = collectInfo.EndImageUrl;
                }

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundCollect.GetRedirectUrl(PublishmentSystemID)}"";return false");
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

                phStep1.Visible = phStep2.Visible = phStep3.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(tbKeywords.Text))
                    {
                        if (collectID > 0)
                        {
                            var collectInfo = DataProviderWX.CollectDAO.GetCollectInfo(collectID);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemID, collectInfo.KeywordID, PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
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
                    btnSubmit.Text = "确 认";
                }
                else if (selectedStep == 3)
                {
                    var collectInfo = new CollectInfo();
                    if (collectID > 0)
                    {
                        collectInfo = DataProviderWX.CollectDAO.GetCollectInfo(collectID);
                    }
                    collectInfo.PublishmentSystemID = PublishmentSystemID;

                    collectInfo.KeywordID = DataProviderWX.KeywordDAO.GetKeywordID(PublishmentSystemID, collectID > 0, tbKeywords.Text, EKeywordType.Collect, collectInfo.KeywordID);
                    collectInfo.IsDisabled = !cbIsEnabled.Checked;

                    collectInfo.StartDate = dtbStartDate.DateTime;
                    collectInfo.EndDate = dtbEndDate.DateTime;
                    collectInfo.Title = PageUtils.FilterXSS(tbTitle.Text);
                    collectInfo.ImageUrl = imageUrl.Value; ;
                    collectInfo.Summary = tbSummary.Text;

                    collectInfo.ContentImageUrl = contentImageUrl.Value;
                    collectInfo.ContentDescription = tbContentDescription.Text;
                    collectInfo.ContentMaxVote = TranslateUtils.ToInt(tbContentMaxVote.Text);
                    collectInfo.ContentIsCheck = TranslateUtils.ToBool(ddlContentIsCheck.SelectedValue);

                    collectInfo.EndTitle = tbEndTitle.Text;
                    collectInfo.EndImageUrl = endImageUrl.Value;
                    collectInfo.EndSummary = tbEndSummary.Text;

                    try
                    {
                        if (collectID > 0)
                        {
                            DataProviderWX.CollectDAO.Update(collectInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "修改征集活动",
                                $"征集活动:{tbTitle.Text}");
                            SuccessMessage("修改征集活动成功！");
                        }
                        else
                        {
                            collectID = DataProviderWX.CollectDAO.Insert(collectInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "添加征集活动",
                                $"征集活动:{tbTitle.Text}");
                            SuccessMessage("添加征集活动成功！");
                        }

                        var redirectUrl = PageUtils.GetWXUrl(
                            $"background_collect.aspx?publishmentSystemID={PublishmentSystemID}");
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "征集活动设置失败！");
                    }

                    btnSubmit.Visible = false;
                    btnReturn.Visible = false;
                }
			}
		}
	}
}
