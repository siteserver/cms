using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using System;
using System.Web.UI.WebControls;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundStoreItem : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnAdd;
        public Button btnDelete;
        public Button btnReturn;

        private int storeID;
        private int storeItemID;

        public static string GetRedirectUrl(int publishmentSystemID, int storeID)
        {
            return PageUtils.GetWXUrl(
                $"background_StoreItem.aspx?publishmentSystemID={publishmentSystemID}&storeID={storeID}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            storeID = TranslateUtils.ToInt(GetQueryString("storeID"));

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWX.StoreItemDAO.Delete(PublishmentSystemID, list);
                        SuccessMessage("门店信息删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "门店信息删除失败！");
                    }
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 30;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            spContents.SelectCommand = DataProviderWX.StoreItemDAO.GetSelectString(storeID);
            spContents.SortField = StoreItemAttribute.ID;
            spContents.SortMode = SortMode.ASC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            { 
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Store, "微门店信息管理", AppManager.WeiXin.Permission.WebSite.Store);
                spContents.DataBind();


                var urlAdd = BackgroundStoreItemAdd.GetRedirectUrl(PublishmentSystemID, storeItemID,storeID);
                btnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");
                

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID, storeID), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的微门店信息", "此操作将删除所选微门店信息，确认吗？"));

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundStore.GetRedirectUrl(PublishmentSystemID)}"";return false");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var storeItemInfo = new StoreItemInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlStoreName = e.Item.FindControl("ltlStoreName") as Literal;
                var ltlStoreCategoryName = e.Item.FindControl("ltlStoreCategoryName") as Literal;
                var ltlTel = e.Item.FindControl("ltlTel") as Literal;
                var ltlMobile = e.Item.FindControl("ltlMobile") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;
                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlStoreName.Text = storeItemInfo.StoreName;

                if (storeItemInfo.CategoryID > 0)
                {
                    ltlStoreCategoryName.Text = DataProviderWX.StoreCategoryDAO.GetStoreCategoryInfo(storeItemInfo.CategoryID).CategoryName.ToString();
                }
                else
                {
                    ltlStoreCategoryName.Text = "";
                }
                ltlTel.Text = storeItemInfo.Tel;
                ltlMobile.Text = storeItemInfo.Mobile;
                var urlEdit = BackgroundStoreItemAdd.GetRedirectUrl(PublishmentSystemID, storeItemInfo.ID, storeID);
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
