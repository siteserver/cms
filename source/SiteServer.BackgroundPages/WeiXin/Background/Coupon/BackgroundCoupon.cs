using System;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundCoupon : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnAdd;
        public Button btnDelete;
        public Button btnReturn;

        private string actTitle = null;

        public static string GetRedirectUrl(int publishmentSystemID)
        {
            return PageUtils.GetWXUrl($"background_coupon.aspx?PublishmentSystemID={publishmentSystemID}");
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
                        DataProviderWX.CouponDAO.Delete(list);
                        SuccessMessage("优惠劵删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "优惠劵删除失败！");
                    }
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 30;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            spContents.SelectCommand = DataProviderWX.CouponDAO.GetSelectString(PublishmentSystemID);
            spContents.SortField = CouponAttribute.ID;
            spContents.SortMode = SortMode.ASC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Coupon, "优惠劵管理", AppManager.WeiXin.Permission.WebSite.Coupon);

                spContents.DataBind();

                btnAdd.Attributes.Add("onclick", Modal.CouponAdd.GetOpenWindowStringToAdd(PublishmentSystemID));

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的优惠劵", "此操作将删除所选优惠劵，确认吗？"));

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundCouponAct.GetRedirectUrl(PublishmentSystemID)}"";return false");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                actTitle = null;

                var couponInfo = new CouponInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlActTitle = e.Item.FindControl("ltlActTitle") as Literal;
                var ltlTotalNum = e.Item.FindControl("ltlTotalNum") as Literal;
                var ltlHoldNum = e.Item.FindControl("ltlHoldNum") as Literal;
                var ltlCashNum = e.Item.FindControl("ltlCashNum") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlSN = e.Item.FindControl("ltlSN") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = couponInfo.Title;
                if (couponInfo.ActID > 0 && actTitle == null)
                {
                    actTitle = DataProviderWX.CouponActDAO.GetTitle(couponInfo.ActID);
                }
                if (actTitle != null)
                {
                    ltlActTitle.Text = actTitle;
                }
                ltlTotalNum.Text = couponInfo.TotalNum.ToString();
                ltlHoldNum.Text = DataProviderWX.CouponSNDAO.GetHoldNum(PublishmentSystemID, couponInfo.ID).ToString();
                ltlCashNum.Text = DataProviderWX.CouponSNDAO.GetCashNum(PublishmentSystemID, couponInfo.ID).ToString();
                ltlAddDate.Text = DateUtils.GetDateString(couponInfo.AddDate);

                ltlSN.Text =
                    $@"<a href=""{BackgroundCouponSN.GetRedirectUrl(PublishmentSystemID, couponInfo.ID,
                        GetRedirectUrl(PublishmentSystemID))}"">优惠劵明细</a>";

                var urlEdit = Modal.CouponAdd.GetOpenWindowStringToEdit(PublishmentSystemID, couponInfo.ID);
                ltlEditUrl.Text = $@"<a href=""javascript:;"" onclick=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
