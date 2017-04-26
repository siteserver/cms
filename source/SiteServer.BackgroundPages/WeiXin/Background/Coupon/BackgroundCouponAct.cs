using System;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundCouponAct : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnAdd;
        public Button btnCouponAdd;
        public Button btnCoupon;
        public Button btnDelete;

        public static string GetRedirectUrl(int publishmentSystemID)
        {
            return PageUtils.GetWXUrl($"background_couponAct.aspx?PublishmentSystemID={publishmentSystemID}");
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
                        DataProviderWX.CouponActDAO.Delete(list);
                        SuccessMessage("活动删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "活动删除失败！");
                    }
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 30;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            spContents.SelectCommand = DataProviderWX.CouponActDAO.GetSelectString(PublishmentSystemID);
            spContents.SortField = CouponActAttribute.ID;
            spContents.SortMode = SortMode.ASC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Coupon, "优惠劵活动", AppManager.WeiXin.Permission.WebSite.Coupon);
                spContents.DataBind();

                var urlAdd = BackgroundCouponActAdd.GetRedirectUrl(PublishmentSystemID, 0);
                btnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");

                btnCouponAdd.Attributes.Add("onclick", Modal.CouponAdd.GetOpenWindowStringToAdd(PublishmentSystemID));

                var urlCoupon = BackgroundCoupon.GetRedirectUrl(PublishmentSystemID);
                btnCoupon.Attributes.Add("onclick", $"location.href='{urlCoupon}';return false");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的优惠劵活动", "此操作将删除所选优惠劵活动，确认吗？"));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var actInfo = new CouponActInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlKeywords = e.Item.FindControl("ltlKeywords") as Literal;
                var ltlStartDate = e.Item.FindControl("ltlStartDate") as Literal;
                var ltlEndDate = e.Item.FindControl("ltlEndDate") as Literal;
                var ltlUserCount = e.Item.FindControl("ltlUserCount") as Literal;
                var ltlPVCount = e.Item.FindControl("ltlPVCount") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlCoupons = e.Item.FindControl("ltlCoupons") as Literal;
                var ltlRelate = e.Item.FindControl("ltlRelate") as Literal;
                var ltlPreviewUrl = e.Item.FindControl("ltlPreviewUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = actInfo.Title;
                ltlKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(actInfo.KeywordID);
                ltlStartDate.Text = DateUtils.GetDateAndTimeString(actInfo.StartDate);
                ltlEndDate.Text = DateUtils.GetDateAndTimeString(actInfo.EndDate);
                ltlUserCount.Text = actInfo.UserCount.ToString();
                ltlPVCount.Text = actInfo.PVCount.ToString();

                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!actInfo.IsDisabled);

                var couponInfoList = DataProviderWX.CouponDAO.GetCouponInfoList(PublishmentSystemID, actInfo.ID);
                foreach (var couponInfo in couponInfoList)
                {
                    ltlCoupons.Text +=
                        $@"<a href=""{BackgroundCouponSN.GetRedirectUrl(PublishmentSystemID, couponInfo.ID,
                            GetRedirectUrl(PublishmentSystemID))}"">{couponInfo.Title}</a>&nbsp;&nbsp;" +
                        ",";
                }
                if (ltlCoupons.Text.Length > 0)
                {
                    ltlCoupons.Text = ltlCoupons.Text.Substring(0, ltlCoupons.Text.Length - 1);
                }
                ltlRelate.Text =
                    $@"<a href=""javascript:;"" onclick=""{Modal.CouponRelated.GetOpenWindowString(
                        PublishmentSystemID, actInfo.ID)}"">关联优惠劵</a>";

                if (couponInfoList.Count > 0)
                {
                    var urlPreview = CouponManager.GetCouponHoldUrl(PublishmentSystemInfo, actInfo.ID);
                    urlPreview = BackgroundPreview.GetRedirectUrlToMobile(urlPreview);
                    ltlPreviewUrl.Text = $@"<a target=""_blank"" href=""{urlPreview}"">预览</a>";
                }

                var urlEdit = BackgroundCouponActAdd.GetRedirectUrl(PublishmentSystemID, actInfo.ID);
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
