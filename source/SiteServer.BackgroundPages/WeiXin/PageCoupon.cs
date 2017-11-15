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
    public class PageCoupon : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnAdd;
        public Button BtnDelete;
        public Button BtnReturn;

        private string _actTitle = null;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageCoupon), new NameValueCollection
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
                        DataProviderWx.CouponDao.Delete(list);
                        SuccessMessage("优惠劵删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "优惠劵删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            SpContents.SelectCommand = DataProviderWx.CouponDao.GetSelectString(PublishmentSystemId);
            SpContents.SortField = CouponAttribute.Id;
            SpContents.SortMode = SortMode.ASC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdCoupon, "优惠劵管理", AppManager.WeiXin.Permission.WebSite.Coupon);

                SpContents.DataBind();

                BtnAdd.Attributes.Add("onclick", ModalCouponAdd.GetOpenWindowStringToAdd(PublishmentSystemId));

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的优惠劵", "此操作将删除所选优惠劵，确认吗？"));

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageCouponAct.GetRedirectUrl(PublishmentSystemId)}"";return false");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                _actTitle = null;

                var couponInfo = new CouponInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlActTitle = e.Item.FindControl("ltlActTitle") as Literal;
                var ltlTotalNum = e.Item.FindControl("ltlTotalNum") as Literal;
                var ltlHoldNum = e.Item.FindControl("ltlHoldNum") as Literal;
                var ltlCashNum = e.Item.FindControl("ltlCashNum") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlSn = e.Item.FindControl("ltlSN") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = couponInfo.Title;
                if (couponInfo.ActId > 0 && _actTitle == null)
                {
                    _actTitle = DataProviderWx.CouponActDao.GetTitle(couponInfo.ActId);
                }
                if (_actTitle != null)
                {
                    ltlActTitle.Text = _actTitle;
                }
                ltlTotalNum.Text = couponInfo.TotalNum.ToString();
                ltlHoldNum.Text = DataProviderWx.CouponSnDao.GetHoldNum(PublishmentSystemId, couponInfo.Id).ToString();
                ltlCashNum.Text = DataProviderWx.CouponSnDao.GetCashNum(PublishmentSystemId, couponInfo.Id).ToString();
                ltlAddDate.Text = DateUtils.GetDateString(couponInfo.AddDate);

                ltlSn.Text =
                    $@"<a href=""{PageCouponSn.GetRedirectUrl(PublishmentSystemId, couponInfo.Id,
                        GetRedirectUrl(PublishmentSystemId))}"">优惠劵明细</a>";

                var urlEdit = ModalCouponAdd.GetOpenWindowStringToEdit(PublishmentSystemId, couponInfo.Id);
                ltlEditUrl.Text = $@"<a href=""javascript:;"" onclick=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
