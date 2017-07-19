using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager.Store;
using SiteServer.CMS.WeiXin.Model;
using System.Collections.Specialized;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageStoreItemAdd : BasePageCms
    {
        public Literal LtlPageTitle;

        public TextBox TbStoreName;
        public TextBox TbStoreTel;
        public TextBox TbStoreMobile;
        public TextBox TbStoreAddress;
        public Literal LtlImageUrl;
        public TextBox TbSummary;
        public DropDownList DdlStoreCategoryName;

        public Button BtnSubmit;
        public Button BtnReturn;

        public HtmlInputHidden ImageUrl;
        public HtmlInputHidden TxtLongitude;
        public HtmlInputHidden TxtLatitude;

        private int _storeItemId;
        private int _storeId;

        private bool[] _isLastNodeArray;

        public static string GetRedirectUrl(int publishmentSystemId, int storeItemId, int storeId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageStoreItemAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"storeItemId", storeItemId.ToString()},
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
            _storeItemId = Body.GetQueryInt("storeItemID");

            if (!IsPostBack)
            {
                var pageTitle = _storeItemId > 0 ? "编辑微门店信息" : "添加微门店信息";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdStore, pageTitle, AppManager.WeiXin.Permission.WebSite.Store);
                LtlPageTitle.Text = pageTitle;
                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{StoreManager.GetImageUrl(PublishmentSystemInfo, string.Empty)}"" width=""200"" height=""200"" align=""middle"" />";


                DdlStoreCategoryName.Items.Add(new ListItem("<请选择>", "0"));

                var categoryIdList = DataProviderWx.StoreCategoryDao.GetAllCategoryIdList(PublishmentSystemId);
                var count = categoryIdList.Count;
                if (count > 0)
                {
                    _isLastNodeArray = new bool[count];
                    foreach (var theCategoryId in categoryIdList)
                    {
                        var categoryInfo = DataProviderWx.StoreCategoryDao.GetCategoryInfo(theCategoryId);
                        var listitem = new ListItem(GetTitle(categoryInfo.Id, categoryInfo.CategoryName, categoryInfo.ParentsCount, categoryInfo.IsLastNode), theCategoryId.ToString());
                        DdlStoreCategoryName.Items.Add(listitem);
                    }
                }

                if (_storeItemId > 0)
                {
                    var storeItemInfo = DataProviderWx.StoreItemDao.GetStoreItemInfo(_storeItemId);

                    TbStoreName.Text = storeItemInfo.StoreName;
                    TbStoreTel.Text = storeItemInfo.Tel;
                    TbStoreMobile.Text = storeItemInfo.Mobile;
                    TbStoreAddress.Text = storeItemInfo.Address;
                    TxtLatitude.Value = storeItemInfo.Latitude;
                    TxtLongitude.Value = storeItemInfo.Longitude;
                    TbSummary.Text = storeItemInfo.Summary;

                    if (storeItemInfo.CategoryId > 0)
                    {
                        DdlStoreCategoryName.Items.FindByValue("" + storeItemInfo.CategoryId + "").Selected = true;
                    }

                    if (!string.IsNullOrEmpty(storeItemInfo.ImageUrl))
                    {
                        LtlImageUrl.Text =
                            $@"<img id=""preview_storeImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, storeItemInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    ImageUrl.Value = storeItemInfo.ImageUrl;

                    _storeId = storeItemInfo.StoreId;
                }

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageStoreItem.GetRedirectUrl(PublishmentSystemId, _storeId)}"";return false");
            }
        }
        public string GetTitle(int categoryId, string categoryName, int parentsCount, bool isLastNode)
        {
            var str = "";
            if (isLastNode == false)
            {
                _isLastNodeArray[parentsCount] = false;
            }
            else
            {
                _isLastNodeArray[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                if (_isLastNodeArray[i])
                {
                    str = String.Concat(str, "　");
                }
                else
                {
                    str = String.Concat(str, "│");
                }
            }
            if (isLastNode)
            {
                str = String.Concat(str, "└");
            }
            else
            {
                str = String.Concat(str, "├");
            }
            str = String.Concat(str, categoryName);
            return str;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var conflictKeywords = string.Empty;

                var storeItemInfo = new StoreItemInfo();
                if (_storeItemId > 0)
                {
                    storeItemInfo = DataProviderWx.StoreItemDao.GetStoreItemInfo(_storeItemId);
                }
                storeItemInfo.PublishmentSystemId = PublishmentSystemId;
                storeItemInfo.StoreId = _storeId;
                storeItemInfo.CategoryId = Convert.ToInt32(DdlStoreCategoryName.SelectedValue);
                storeItemInfo.StoreName = TbStoreName.Text;
                storeItemInfo.Tel = TbStoreTel.Text;
                storeItemInfo.Mobile = TbStoreMobile.Text;
                storeItemInfo.Address = TbStoreAddress.Text;
                storeItemInfo.ImageUrl = ImageUrl.Value;
                storeItemInfo.Latitude = TxtLatitude.Value;
                storeItemInfo.Longitude = TxtLongitude.Value;
                storeItemInfo.Summary = TbSummary.Text;

                try
                {
                    if (_storeItemId > 0)
                    {
                        DataProviderWx.StoreItemDao.Update(PublishmentSystemId, storeItemInfo);

                        LogUtils.AddAdminLog(Body.AdministratorName, "修改微门店信息", $"微门店:{TbStoreName.Text}");
                        SuccessMessage("修改微门店成功！");
                    }
                    else
                    {
                        _storeItemId = DataProviderWx.StoreItemDao.Insert(PublishmentSystemId, storeItemInfo);
                        LogUtils.AddAdminLog(Body.AdministratorName, "添加微门店信息", $"微门店:{TbStoreName.Text}");
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
