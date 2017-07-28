using BaiRong.Core;
using System;
using System.Web.UI.WebControls;
using SiteServer.WeiXin.Model;
using SiteServer.WeiXin.Core;

using System.Web.UI.HtmlControls;
using StoreManager = SiteServer.WeiXin.Core.StoreManager;
using SiteServer.CMS.Core;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundStoreAdd : BackgroundBasePageWX
    {
        public Literal ltlPageTitle;

        public PlaceHolder phStep1;
        public TextBox tbKeywords;
        public TextBox tbTitle;
        public TextBox tbSummary;
        public CheckBox cbIsEnabled;
        public Literal ltlImageUrl;

        public Button btnSubmit;
        public Button btnReturn;

        public HtmlInputHidden imageUrl;


        private int storeID;

        public static string GetRedirectUrl(int publishmentSystemID, int storeID)
        {
            return PageUtils.GetWXUrl(
                $"background_StoreAdd.aspx?publishmentSystemID={publishmentSystemID}&storeID={storeID}");
        }

        public string GetUploadUrl()
        {
            return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }


        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            storeID = TranslateUtils.ToInt(GetQueryString("storeID"));

            if (!IsPostBack)
            {
                var pageTitle = storeID > 0 ? "编辑微门店" : "添加微门店";
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Store, pageTitle, AppManager.WeiXin.Permission.WebSite.Store);
                ltlPageTitle.Text = pageTitle;

                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{StoreManager.GetImageUrl(PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";

                if (storeID > 0)
                {
                    var storeInfo = DataProviderWX.StoreDAO.GetStoreInfo(storeID);

                    tbKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(storeInfo.KeywordID);
                    cbIsEnabled.Checked = !storeInfo.IsDisabled;
                    tbTitle.Text = storeInfo.Title;
                    if (!string.IsNullOrEmpty(storeInfo.ImageUrl))
                    {
                        ltlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, storeInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    tbSummary.Text = storeInfo.Summary;

                    imageUrl.Value = storeInfo.ImageUrl;
                }

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundStore.GetRedirectUrl(PublishmentSystemID)}"";return false");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var isConflict = false;
                var conflictKeywords = string.Empty;
                if (!string.IsNullOrEmpty(tbKeywords.Text))
                {
                    if (storeID > 0)
                    {
                        var storeInfo = DataProviderWX.StoreDAO.GetStoreInfo(storeID);
                        isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemID, storeInfo.KeywordID, PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
                    }
                    else
                    {
                        isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemID, PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
                    }
                }

                if (isConflict)
                {
                    FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                }
                else
                {
                    var storeInfo = new StoreInfo();
                    if (storeID > 0)
                    {
                        storeInfo = DataProviderWX.StoreDAO.GetStoreInfo(storeID);
                    }
                    storeInfo.PublishmentSystemID = PublishmentSystemID;

                    storeInfo.KeywordID = DataProviderWX.KeywordDAO.GetKeywordID(PublishmentSystemID, storeID > 0, tbKeywords.Text, EKeywordType.Store, storeInfo.KeywordID);
                    storeInfo.IsDisabled = !cbIsEnabled.Checked;

                    storeInfo.Title = PageUtils.FilterXSS(tbTitle.Text);
                    storeInfo.ImageUrl = imageUrl.Value; ;
                    storeInfo.Summary = tbSummary.Text;

                    try
                    {
                        if (storeID > 0)
                        {
                            DataProviderWX.StoreDAO.Update(storeInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "修改微门店",
                                $"微门店:{tbTitle.Text}");
                            SuccessMessage("修改微门店成功！");
                        }
                        else
                        {
                            storeID = DataProviderWX.StoreDAO.Insert(storeInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "添加微门店",
                                $"微门店:{tbTitle.Text}");
                            SuccessMessage("添加微门店成功！");
                        }

                        var redirectUrl = PageUtils.GetWXUrl(
                            $"background_storeItem.aspx?publishmentSystemID={PublishmentSystemID}&storeID={storeID}");
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "微门店设置失败！");
                    }
                }
            }
        }
    }
}
