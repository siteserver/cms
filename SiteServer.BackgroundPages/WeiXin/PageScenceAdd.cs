using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageScenceAdd : BasePageCms
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

        public Button BtnSubmit;

        private int _scenceId;
        public static string GetRedirectUrl(int publishmentSystemId, int scenceId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageScenceAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"scenceId", scenceId.ToString()}
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
            _scenceId = Body.GetQueryInt("scenceID");

            if (!IsPostBack)
            {
                var pageTitle = _scenceId > 0 ? "编辑站点场景" : "添加站点场景";
                //base.BreadCrumbConsole(AppManager.CMS.LeftMenu.Id_Site, pageTitle, AppManager.Permission.Platform_Site);
                LtlPageTitle.Text = pageTitle;

                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{CouponManager.GetImageUrl(PublishmentSystemInfo,
                        string.Empty)}"" width=""370"" align=""middle"" />";

                if (_scenceId > 0)
                {
                    var scenceInfo = DataProviderWx.ScenceDao.GetScenceInfo(_scenceId);

                    TbKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(scenceInfo.KeywordId);
                    CbIsEnabled.Checked = !scenceInfo.IsDisabled;
                    DtbStartDate.DateTime = scenceInfo.StartDate;
                    DtbEndDate.DateTime = scenceInfo.EndDate;
                    TbTitle.Text = scenceInfo.Title;
                    if (!string.IsNullOrEmpty(scenceInfo.ImageUrl))
                    {
                        LtlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, scenceInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    TbSummary.Text = scenceInfo.Summary;

                }
                else
                {
                    DtbEndDate.DateTime = DateTime.Now.AddMonths(1);
                }
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
                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(TbKeywords.Text))
                    {
                        if (_scenceId > 0)
                        {
                            var actInfo = DataProviderWx.CouponActDao.GetActInfo(_scenceId);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, actInfo.KeywordId, TbKeywords.Text, out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, TbKeywords.Text, out conflictKeywords);
                        }
                    }

                    if (isConflict)
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                        PhStep1.Visible = true;
                    }
                }
            }
        }
    }
}
