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
    public class PageCouponAct : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnAdd;
        public Button BtnCouponAdd;
        public Button BtnCoupon;
        public Button BtnDelete;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageCouponAct), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
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
                        DataProviderWx.CouponActDao.Delete(list);
                        SuccessMessage("活动删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "活动删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            SpContents.SelectCommand = DataProviderWx.CouponActDao.GetSelectString(PublishmentSystemId);
            SpContents.SortField = CouponActAttribute.Id;
            SpContents.SortMode = SortMode.ASC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdCoupon, "优惠劵活动", AppManager.WeiXin.Permission.WebSite.Coupon);
                SpContents.DataBind();

                var urlAdd = PageCouponActAdd.GetRedirectUrl(PublishmentSystemId, 0);
                BtnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");

                BtnCouponAdd.Attributes.Add("onclick", ModalCouponAdd.GetOpenWindowStringToAdd(PublishmentSystemId));

                var urlCoupon = PageCoupon.GetRedirectUrl(PublishmentSystemId);
                BtnCoupon.Attributes.Add("onclick", $"location.href='{urlCoupon}';return false");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的优惠劵活动", "此操作将删除所选优惠劵活动，确认吗？"));
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
                var ltlPvCount = e.Item.FindControl("ltlPVCount") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlCoupons = e.Item.FindControl("ltlCoupons") as Literal;
                var ltlRelate = e.Item.FindControl("ltlRelate") as Literal;
                var ltlPreviewUrl = e.Item.FindControl("ltlPreviewUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = actInfo.Title;
                ltlKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(actInfo.KeywordId);
                ltlStartDate.Text = DateUtils.GetDateAndTimeString(actInfo.StartDate);
                ltlEndDate.Text = DateUtils.GetDateAndTimeString(actInfo.EndDate);
                ltlUserCount.Text = actInfo.UserCount.ToString();
                ltlPvCount.Text = actInfo.PvCount.ToString();

                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!actInfo.IsDisabled);

                var couponInfoList = DataProviderWx.CouponDao.GetCouponInfoList(PublishmentSystemId, actInfo.Id);
                foreach (var couponInfo in couponInfoList)
                {
                    ltlCoupons.Text +=
                        $@"<a href=""{PageCouponSn.GetRedirectUrl(PublishmentSystemId, couponInfo.Id,
                            GetRedirectUrl(PublishmentSystemId))}"">{couponInfo.Title}</a>&nbsp;&nbsp;" +
                        ",";
                }
                if (ltlCoupons.Text.Length > 0)
                {
                    ltlCoupons.Text = ltlCoupons.Text.Substring(0, ltlCoupons.Text.Length - 1);
                }
                ltlRelate.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalCouponRelated.GetOpenWindowString(
                        PublishmentSystemId, actInfo.Id)}"">关联优惠劵</a>";

                if (couponInfoList.Count > 0)
                {
                    //var urlPreview = CouponManager.GetCouponHoldUrl(PublishmentSystemInfo, actInfo.Id);
                    //urlPreview = BackgroundPreview.GetRedirectUrlToMobile(urlPreview);
                    //ltlPreviewUrl.Text = $@"<a target=""_blank"" href=""{urlPreview}"">预览</a>";
                }

                var urlEdit = PageCouponActAdd.GetRedirectUrl(PublishmentSystemId, actInfo.Id);
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
