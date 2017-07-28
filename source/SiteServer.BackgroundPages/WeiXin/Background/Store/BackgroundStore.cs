using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using System;
using System.Web.UI.WebControls;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundStore : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnAdd;
        public Button btnDelete;

        public static string GetRedirectUrl(int publishmentSystemID)
        {
            return PageUtils.GetWXUrl($"background_store.aspx?publishmentSystemID={publishmentSystemID}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWX.StoreDAO.Delete(PublishmentSystemID, list);
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
            spContents.SelectCommand = DataProviderWX.StoreDAO.GetSelectString(PublishmentSystemID);
            spContents.SortField = StoreAttribute.ID;
            spContents.SortMode = SortMode.ASC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            { 
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Store, "门店管理", AppManager.WeiXin.Permission.WebSite.Store);
                spContents.DataBind();

                var urlAdd = BackgroundStoreAdd.GetRedirectUrl(PublishmentSystemID, 0);
                btnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的门店信息", "此操作将删除所选门店信息，确认吗？"));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var storeInfo = new StoreInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlKeywords = e.Item.FindControl("ltlKeywords") as Literal;
                var ltlPVCount = e.Item.FindControl("ltlPVCount") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlStoreContentUrl = e.Item.FindControl("ltlStoreContentUrl") as Literal;
                var ltlPreviewUrl = e.Item.FindControl("ltlPreviewUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = storeInfo.Title;
                ltlKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(storeInfo.KeywordID);
                ltlPVCount.Text = storeInfo.PVCount.ToString();

                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!storeInfo.IsDisabled);

                var urlStoreContent = BackgroundStoreItem.GetRedirectUrl(PublishmentSystemID, storeInfo.ID);
                 
                ltlStoreContentUrl.Text = $@"<a href=""{urlStoreContent}"">微门店</a>";

                var urlPreview = StoreManager.GetStoreUrl(storeInfo, string.Empty);
                urlPreview = BackgroundPreview.GetRedirectUrlToMobile(urlPreview);
                ltlPreviewUrl.Text = $@"<a href=""{urlPreview}"" target=""_blank"">预览</a>";

                var urlEdit = BackgroundStoreAdd.GetRedirectUrl(PublishmentSystemID, storeInfo.ID);
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
         
    }
}
