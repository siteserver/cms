using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Core;
using System;
using System.Web.UI.WebControls;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundScenceAdd : BackgroundBasePage
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

        public Button btnSubmit;

        private int scenceID;
        public static string GetRedirectUrl(int publishmentSystemID, int scenceID)
        {
            return PageUtils.GetWXUrl(
                $"background_scenceAdd.aspx?publishmentSystemID={publishmentSystemID}&scenceID={scenceID}");
        }
        public string GetUploadUrl()
        {
            return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            scenceID = TranslateUtils.ToInt(GetQueryString("scenceID"));

            if (!IsPostBack)
            {
                var pageTitle = scenceID > 0 ? "编辑站点场景" : "添加站点场景";
                //base.BreadCrumbConsole(AppManager.CMS.LeftMenu.ID_Site, pageTitle, AppManager.Permission.Platform_Site);
                ltlPageTitle.Text = pageTitle;

                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{CouponManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";

                if (scenceID > 0)
                {
                    var scenceInfo = DataProviderWX.ScenceDAO.GetScenceInfo(scenceID);

                    tbKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(scenceInfo.KeywordID);
                    cbIsEnabled.Checked = !scenceInfo.IsDisabled;
                    dtbStartDate.DateTime = scenceInfo.StartDate;
                    dtbEndDate.DateTime = scenceInfo.EndDate;
                    tbTitle.Text = scenceInfo.Title;
                    if (!string.IsNullOrEmpty(scenceInfo.ImageUrl))
                    {
                        ltlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, scenceInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    tbSummary.Text = scenceInfo.Summary;

                }
                else
                {
                    dtbEndDate.DateTime = DateTime.Now.AddMonths(1);
                }
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
                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(tbKeywords.Text))
                    {
                        if (scenceID > 0)
                        {
                            var actInfo = DataProviderWX.CouponActDAO.GetActInfo(scenceID);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemID, actInfo.KeywordID, tbKeywords.Text, out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemID, tbKeywords.Text, out conflictKeywords);
                        }
                    }

                    if (isConflict)
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                        phStep1.Visible = true;
                    }
                }
            }
        }
    }
}
