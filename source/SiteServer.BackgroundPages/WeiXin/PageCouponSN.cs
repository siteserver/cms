using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageCouponSn : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnAdd;
        public Button BtnSetting;
        public Button BtnReturn;
        public Button BtnUpFile;

        private int _couponId;
        private string _returnUrl;


        public static string GetRedirectUrl(int publishmentSystemId, int couponId, string returnUrl)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageCouponSn), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"couponId", couponId.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            _couponId = TranslateUtils.ToInt(Request.QueryString["couponID"]);
            _returnUrl = StringUtils.ValueFromUrl(Request.QueryString["returnUrl"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWx.CouponSnDao.Delete(list);
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
            
            SpContents.SelectCommand = DataProviderWx.CouponSnDao.GetSelectString(PublishmentSystemId, _couponId);
            SpContents.SortField = CouponAttribute.Id;
            SpContents.SortMode = SortMode.ASC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdCoupon, "优惠劵明细", AppManager.WeiXin.Permission.WebSite.Coupon);
                SpContents.DataBind();

                BtnAdd.Attributes.Add("onclick", ModalCouponSnAdd.GetOpenWindowString(PublishmentSystemId, _couponId, 0));

                BtnSetting.Attributes.Add("onclick", ModalCouponSnSetting.GetOpenWindowString(PublishmentSystemId, _couponId));

                BtnReturn.Attributes.Add("onclick", $"location.href='{_returnUrl}';return false");

                BtnUpFile.Attributes.Add("onclick", ModalCouponSnAdd.GetOpenUploadWindowString(PublishmentSystemId, _couponId, 1));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var snInfo = new CouponSnInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlSn = e.Item.FindControl("ltlSN") as Literal;
                var ltlStatus = e.Item.FindControl("ltlStatus") as Literal;
                var ltlHoldDate = e.Item.FindControl("ltlHoldDate") as Literal;
                var ltlHoldMobile = e.Item.FindControl("ltlHoldMobile") as Literal;
                var ltlHoldEmail = e.Item.FindControl("ltlHoldEmail") as Literal;
                var ltlHoldRealName = e.Item.FindControl("ltlHoldRealName") as Literal;
                var ltlCashDate = e.Item.FindControl("ltlCashDate") as Literal;
                var ltlCashUserName = e.Item.FindControl("ltlCashUserName") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlSn.Text = snInfo.Sn;
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
