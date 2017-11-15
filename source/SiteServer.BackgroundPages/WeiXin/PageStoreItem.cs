using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageStoreItem : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnAdd;
        public Button BtnDelete;
        public Button BtnReturn;

        private int _storeId;
        private int _storeItemId;

        public static string GetRedirectUrl(int publishmentSystemId, int storeId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageStoreItem), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"storeId", storeId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _storeId = Body.GetQueryInt("storeID");

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWx.StoreItemDao.Delete(PublishmentSystemId, list);
                        SuccessMessage("门店信息删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "门店信息删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            
            SpContents.SelectCommand = DataProviderWx.StoreItemDao.GetSelectString(_storeId);
            SpContents.SortField = StoreItemAttribute.Id;
            SpContents.SortMode = SortMode.ASC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            { 
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdStore, "微门店信息管理", AppManager.WeiXin.Permission.WebSite.Store);
                SpContents.DataBind();


                var urlAdd = PageStoreItemAdd.GetRedirectUrl(PublishmentSystemId, _storeItemId,_storeId);
                BtnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");
                

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId, _storeId), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的微门店信息", "此操作将删除所选微门店信息，确认吗？"));

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageStore.GetRedirectUrl(PublishmentSystemId)}"";return false");
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

                if (storeItemInfo.CategoryId > 0)
                {
                    ltlStoreCategoryName.Text = DataProviderWx.StoreCategoryDao.GetStoreCategoryInfo(storeItemInfo.CategoryId).CategoryName;
                }
                else
                {
                    ltlStoreCategoryName.Text = "";
                }
                ltlTel.Text = storeItemInfo.Tel;
                ltlMobile.Text = storeItemInfo.Mobile;
                var urlEdit = PageStoreItemAdd.GetRedirectUrl(PublishmentSystemId, storeItemInfo.Id, _storeId);
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
