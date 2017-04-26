using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using System;
using System.Web.UI.WebControls;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundCollectItem : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnDelete;
        public Button btnReturn;
        private int collectID;
        private string returnUrl;
        private int collectItemID;

        public static string GetRedirectUrl(int publishmentSystemID, int collectID, string returnUrl)
        {
            return PageUtils.GetWXUrl(
                $"background_collectItem.aspx?publishmentSystemID={publishmentSystemID}&collectID={collectID}&returnUrl={StringUtils.ValueToUrl(returnUrl)}");
        }

        public static string GetRedirectUrl(int publishmentSystemID, int collectItemID, int collectID)
        {
            return PageUtils.GetWXUrl(
                $"background_collectItem.aspx?publishmentSystemID={publishmentSystemID}&collectItemID={collectItemID}&collectID={collectID}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            collectID = TranslateUtils.ToInt(Request.QueryString["collectID"]);
            returnUrl = StringUtils.ValueFromUrl(Request.QueryString["returnUrl"]);
            collectItemID = TranslateUtils.ToInt(Request.QueryString["collectItemID"]);
            if (IsForbidden) return;

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWX.CollectItemDAO.Delete(PublishmentSystemID, list);
                        SuccessMessage("征集参赛选项删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "征集参赛选项删除失败！");
                    }
                }
            }

            if (collectItemID > 0)
            {
                try
                {
                    DataProviderWX.CollectItemDAO.Audit(PublishmentSystemID, collectItemID);
                    SuccessMessage("征集参赛选项审核成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "征集参赛选项审核失败！");
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 30;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            spContents.SelectCommand = DataProviderWX.CollectItemDAO.GetSelectString(PublishmentSystemID, collectID);
            spContents.SortField = CollectItemAttribute.ID;
            spContents.SortMode = SortMode.ASC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Collect, "参赛记录", AppManager.WeiXin.Permission.WebSite.Collect);
                spContents.DataBind();

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID, collectID, returnUrl), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的征集活动参赛选项", "此操作将删除所选征集活动参赛选项，确认吗？"));
                btnReturn.Attributes.Add("onclick", $"location.href='{returnUrl}';return false");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var collectItemInfo = new CollectItemInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlItemTitle = e.Item.FindControl("ltlItemTitle") as Literal;
                var ltlDescription = e.Item.FindControl("ltlDescription") as Literal;
                var ltlMobile = e.Item.FindControl("ltlMobile") as Literal;
                var ltlIsChecked = e.Item.FindControl("ltlIsChecked") as Literal;
                var ltlVoteNum = e.Item.FindControl("ltlVoteNum") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlItemTitle.Text = collectItemInfo.Title;
                ltlDescription.Text = collectItemInfo.Description;
                ltlMobile.Text = collectItemInfo.Mobile;
                ltlVoteNum.Text = collectItemInfo.VoteNum.ToString(); ;
                ltlIsChecked.Text = StringUtils.GetTrueOrFalseImageHtml(collectItemInfo.IsChecked);
                var urlEdit = GetRedirectUrl(PublishmentSystemID, collectItemInfo.ID, collectItemInfo.CollectID);
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">审核</a>";

            }
        }
    }
}
