using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using StoreManager = SiteServer.WeiXin.Core.StoreManager;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundStoreItemAdd : BackgroundBasePageWX
    {
        public Literal ltlPageTitle;

        public TextBox tbStoreName;
        public TextBox tbStoreTel;
        public TextBox tbStoreMobile;
        public TextBox tbStoreAddress;
        public Literal ltlImageUrl;
        public TextBox tbSummary;
        public DropDownList ddlStoreCategoryName;

        public Button btnSubmit;
        public Button btnReturn;

        public HtmlInputHidden imageUrl;
        public HtmlInputHidden txtLongitude;
        public HtmlInputHidden txtLatitude;

        private int storeItemID;
        private int storeID;

        private bool[] isLastNodeArray;

        public static string GetRedirectUrl(int publishmentSystemID, int storeItemID, int storeID)
        {
            return PageUtils.GetWXUrl(
                $"background_storeItemAdd.aspx?publishmentSystemID={publishmentSystemID}&storeItemID={storeItemID}&storeID={storeID}");
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
            storeItemID = TranslateUtils.ToInt(GetQueryString("storeItemID"));

            if (!IsPostBack)
            {
                var pageTitle = storeItemID > 0 ? "编辑微门店信息" : "添加微门店信息";
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Store, pageTitle, AppManager.WeiXin.Permission.WebSite.Store);
                ltlPageTitle.Text = pageTitle;
                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{StoreManager.GetImageUrl(PublishmentSystemInfo, string.Empty)}"" width=""200"" height=""200"" align=""middle"" />";


                ddlStoreCategoryName.Items.Add(new ListItem("<请选择>", "0"));

                var categoryIDList = DataProviderWX.StoreCategoryDAO.GetAllCategoryIDList(PublishmentSystemID);
                var count = categoryIDList.Count;
                if (count > 0)
                {
                    isLastNodeArray = new bool[count];
                    foreach (var theCategoryID in categoryIDList)
                    {
                        var categoryInfo = DataProviderWX.StoreCategoryDAO.GetCategoryInfo(theCategoryID);
                        var listitem = new ListItem(GetTitle(categoryInfo.ID, categoryInfo.CategoryName, categoryInfo.ParentsCount, categoryInfo.IsLastNode), theCategoryID.ToString());
                        ddlStoreCategoryName.Items.Add(listitem);
                    }
                }

                if (storeItemID > 0)
                {
                    var storeItemInfo = DataProviderWX.StoreItemDAO.GetStoreItemInfo(storeItemID);

                    tbStoreName.Text = storeItemInfo.StoreName;
                    tbStoreTel.Text = storeItemInfo.Tel;
                    tbStoreMobile.Text = storeItemInfo.Mobile;
                    tbStoreAddress.Text = storeItemInfo.Address;
                    txtLatitude.Value = storeItemInfo.Latitude;
                    txtLongitude.Value = storeItemInfo.Longitude;
                    tbSummary.Text = storeItemInfo.Summary;

                    if (storeItemInfo.CategoryID > 0)
                    {
                        ddlStoreCategoryName.Items.FindByValue("" + storeItemInfo.CategoryID + "").Selected = true;
                    }

                    if (!string.IsNullOrEmpty(storeItemInfo.ImageUrl))
                    {
                        ltlImageUrl.Text =
                            $@"<img id=""preview_storeImageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, storeItemInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }

                    imageUrl.Value = storeItemInfo.ImageUrl;

                    storeID = storeItemInfo.StoreID;
                }

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundStoreItem.GetRedirectUrl(PublishmentSystemID, storeID)}"";return false");
            }
        }
        public string GetTitle(int categoryID, string categoryName, int parentsCount, bool isLastNode)
        {
            var str = "";
            if (isLastNode == false)
            {
                isLastNodeArray[parentsCount] = false;
            }
            else
            {
                isLastNodeArray[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                if (isLastNodeArray[i])
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
                if (storeItemID > 0)
                {
                    storeItemInfo = DataProviderWX.StoreItemDAO.GetStoreItemInfo(storeItemID);
                }
                storeItemInfo.PublishmentSystemID = PublishmentSystemID;
                storeItemInfo.StoreID = storeID;
                storeItemInfo.CategoryID = Convert.ToInt32(ddlStoreCategoryName.SelectedValue);
                storeItemInfo.StoreName = tbStoreName.Text;
                storeItemInfo.Tel = tbStoreTel.Text;
                storeItemInfo.Mobile = tbStoreMobile.Text;
                storeItemInfo.Address = tbStoreAddress.Text;
                storeItemInfo.ImageUrl = imageUrl.Value;
                storeItemInfo.Latitude = txtLatitude.Value;
                storeItemInfo.Longitude = txtLongitude.Value;
                storeItemInfo.Summary = tbSummary.Text;

                try
                {
                    if (storeItemID > 0)
                    {
                        DataProviderWX.StoreItemDAO.Update(PublishmentSystemID, storeItemInfo);

                        LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "修改微门店信息",
                            $"微门店:{tbStoreName.Text}");
                        SuccessMessage("修改微门店成功！");
                    }
                    else
                    {
                        storeItemID = DataProviderWX.StoreItemDAO.Insert(PublishmentSystemID, storeItemInfo);
                        LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "添加微门店信息",
                            $"微门店:{tbStoreName.Text}");
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
