using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Manager.Store;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageStoreAdd : BasePageCms
    {
        public Literal LtlPageTitle;

        public PlaceHolder PhStep1;
        public TextBox TbKeywords;
        public TextBox TbTitle;
        public TextBox TbSummary;
        public CheckBox CbIsEnabled;
        public Literal LtlImageUrl;

        public Button BtnSubmit;
        public Button BtnReturn;

        public HtmlInputHidden ImageUrl;


        private int _storeId;

        public static string GetRedirectUrl(int publishmentSystemId, int storeId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageStoreAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"storeId", storeId.ToString()}
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
            _storeId = Body.GetQueryInt("storeID");

            if (!IsPostBack)
            {
                var pageTitle = _storeId > 0 ? "编辑微门店" : "添加微门店";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdStore, pageTitle, AppManager.WeiXin.Permission.WebSite.Store);
                LtlPageTitle.Text = pageTitle;

                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{StoreManager.GetImageUrl(PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";

                if (_storeId > 0)
                {
                    var storeInfo = DataProviderWx.StoreDao.GetStoreInfo(_storeId);

                    TbKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(storeInfo.KeywordId);
                    CbIsEnabled.Checked = !storeInfo.IsDisabled;
                    TbTitle.Text = storeInfo.Title;
                    if (!string.IsNullOrEmpty(storeInfo.ImageUrl))
                    {
                        LtlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, storeInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    TbSummary.Text = storeInfo.Summary;

                    ImageUrl.Value = storeInfo.ImageUrl;
                }

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageStore.GetRedirectUrl(PublishmentSystemId)}"";return false");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var isConflict = false;
                var conflictKeywords = string.Empty;
                if (!string.IsNullOrEmpty(TbKeywords.Text))
                {
                    if (_storeId > 0)
                    {
                        var storeInfo = DataProviderWx.StoreDao.GetStoreInfo(_storeId);
                        isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, storeInfo.KeywordId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
                    }
                    else
                    {
                        isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
                    }
                }

                if (isConflict)
                {
                    FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                }
                else
                {
                    var storeInfo = new StoreInfo();
                    if (_storeId > 0)
                    {
                        storeInfo = DataProviderWx.StoreDao.GetStoreInfo(_storeId);
                    }
                    storeInfo.PublishmentSystemId = PublishmentSystemId;

                    storeInfo.KeywordId = DataProviderWx.KeywordDao.GetKeywordId(PublishmentSystemId, _storeId > 0, TbKeywords.Text, EKeywordType.Store, storeInfo.KeywordId);
                    storeInfo.IsDisabled = !CbIsEnabled.Checked;

                    storeInfo.Title = PageUtils.FilterXss(TbTitle.Text);
                    storeInfo.ImageUrl = ImageUrl.Value; ;
                    storeInfo.Summary = TbSummary.Text;

                    try
                    {
                        if (_storeId > 0)
                        {
                            DataProviderWx.StoreDao.Update(storeInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "修改微门店", $"微门店:{TbTitle.Text}");
                            SuccessMessage("修改微门店成功！");
                        }
                        else
                        {
                            _storeId = DataProviderWx.StoreDao.Insert(storeInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "添加微门店", $"微门店:{TbTitle.Text}");
                            SuccessMessage("添加微门店成功！");
                        }

                        var redirectUrl = PageStoreItem.GetRedirectUrl(PublishmentSystemId, _storeId);
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
