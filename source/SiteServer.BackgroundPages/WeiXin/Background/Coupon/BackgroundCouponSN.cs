using System;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundCouponSN : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnAdd;
        public Button btnSetting;
        public Button btnReturn;
        public Button btnUpFile;

        private int couponID;
        private string returnUrl;


        public static string GetRedirectUrl(int publishmentSystemID, int couponID, string returnUrl)
        {
            return PageUtils.GetWXUrl(
                $"background_couponSN.aspx?publishmentSystemID={publishmentSystemID}&couponID={couponID}&returnUrl={StringUtils.ValueToUrl(returnUrl)}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            couponID = TranslateUtils.ToInt(Request.QueryString["couponID"]);
            returnUrl = StringUtils.ValueFromUrl(Request.QueryString["returnUrl"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWX.CouponSNDAO.Delete(list);
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
            spContents.SelectCommand = DataProviderWX.CouponSNDAO.GetSelectString(PublishmentSystemID, couponID);
            spContents.SortField = CouponAttribute.ID;
            spContents.SortMode = SortMode.ASC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Coupon, "优惠劵明细", AppManager.WeiXin.Permission.WebSite.Coupon);
                spContents.DataBind();

                btnAdd.Attributes.Add("onclick", Modal.CouponSNAdd.GetOpenWindowString(PublishmentSystemID, couponID, 0));

                btnSetting.Attributes.Add("onclick", Modal.CouponSNSetting.GetOpenWindowString(PublishmentSystemID, couponID));

                btnReturn.Attributes.Add("onclick", $"location.href='{returnUrl}';return false");

                btnUpFile.Attributes.Add("onclick", Modal.CouponSNAdd.GetOpenUploadWindowString(PublishmentSystemID, couponID, 1));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var snInfo = new CouponSNInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlSN = e.Item.FindControl("ltlSN") as Literal;
                var ltlStatus = e.Item.FindControl("ltlStatus") as Literal;
                var ltlHoldDate = e.Item.FindControl("ltlHoldDate") as Literal;
                var ltlHoldMobile = e.Item.FindControl("ltlHoldMobile") as Literal;
                var ltlHoldEmail = e.Item.FindControl("ltlHoldEmail") as Literal;
                var ltlHoldRealName = e.Item.FindControl("ltlHoldRealName") as Literal;
                var ltlCashDate = e.Item.FindControl("ltlCashDate") as Literal;
                var ltlCashUserName = e.Item.FindControl("ltlCashUserName") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlSN.Text = snInfo.SN;
                var status = ECouponStatusUtils.GetEnumType(snInfo.Status);
                ltlStatus.Text = ECouponStatusUtils.GetText(status);
                if (status == ECouponStatus.Cash || status == ECouponStatus.Hold)
                {
                    ltlHoldDate.Text = DateUtils.GetDateAndTimeString(snInfo.HoldDate);
                    ltlHoldMobile.Text = snInfo.HoldMobile;
                    ltlHoldEmail.Text = snInfo.HoldEmail;
                    ltlHoldRealName.Text = snInfo.HoldRealName;
                }
                if (status == ECouponStatus.Cash)
                {
                    ltlCashDate.Text = DateUtils.GetDateAndTimeString(snInfo.CashDate);
                    ltlCashUserName.Text = snInfo.HoldRealName;
                }
            }
        }

    }
}
