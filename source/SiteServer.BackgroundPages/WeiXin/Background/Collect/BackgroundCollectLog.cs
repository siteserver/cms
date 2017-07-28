using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundCollectLog : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnDelete;
        public Button btnReturn;

        private Dictionary<int, string> idTitleMap;
        private int collectID;
        private string returnUrl;

        public static string GetRedirectUrl(int publishmentSystemID, int collectID, string returnUrl)
        {
            return PageUtils.GetWXUrl(
                $"background_collectLog.aspx?publishmentSystemID={publishmentSystemID}&collectID={collectID}&returnUrl={StringUtils.ValueToUrl(returnUrl)}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            collectID = TranslateUtils.ToInt(Request.QueryString["collectID"]);
            returnUrl = StringUtils.ValueFromUrl(Request.QueryString["returnUrl"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWX.CollectLogDAO.Delete(list);
                        SuccessMessage("删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "删除失败！");
                    }
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 30;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            spContents.SelectCommand = DataProviderWX.CollectLogDAO.GetSelectString(collectID);
            spContents.SortField = CollectLogAttribute.ID;
            spContents.SortMode = SortMode.ASC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Collect, "投票记录", AppManager.WeiXin.Permission.WebSite.Collect);
                var itemInfoList = DataProviderWX.CollectItemDAO.GetCollectItemInfoList(collectID);
                idTitleMap = new Dictionary<int, string>();
                foreach (var itemInfo in itemInfoList)
                {
                    idTitleMap[itemInfo.ID] = itemInfo.Title;
                }

                spContents.DataBind();

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID, collectID, returnUrl), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的投票项", "此操作将删除所选投票项，确认吗？"));
                btnReturn.Attributes.Add("onclick", $"location.href='{returnUrl}';return false");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var logInfo = new CollectLogInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlItemID = e.Item.FindControl("ltlItemID") as Literal;
                var ltlIPAddress = e.Item.FindControl("ltlIPAddress") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();

                if (idTitleMap.ContainsKey(logInfo.ItemID))
                {
                    ltlItemID.Text = idTitleMap[logInfo.ItemID];
                }

                ltlIPAddress.Text = logInfo.IPAddress;
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(logInfo.AddDate);
            }
        }
    }
}
