using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalCouponRelated : BasePageCms
    {
        public CheckBoxList CblCoupon;

        private int _actId;

        public static string GetOpenWindowString(int publishmentSystemId, int actId)
        {
            return PageUtils.GetOpenWindowString("关联优惠劵",
                PageUtils.GetWeiXinUrl(nameof(ModalCouponRelated), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"actId", actId.ToString()}
                }), 400, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _actId = Body.GetQueryInt("actID");

            var actIdList = DataProviderWx.CouponActDao.GetActIdList(PublishmentSystemId);

			if (!IsPostBack)
			{
                var allCouponInfoList = DataProviderWx.CouponDao.GetAllCouponInfoList(PublishmentSystemId);
                foreach (var couponInfo in allCouponInfoList)
                {
                    if (!actIdList.Contains(couponInfo.ActId) || couponInfo.ActId == 0 || couponInfo.ActId == _actId)
                    {
                        var listItem = new ListItem(couponInfo.Title, couponInfo.Id.ToString());
                        if (couponInfo.ActId == _actId)
                        {
                            listItem.Selected = true;
                        }
                        CblCoupon.Items.Add(listItem);
                    }
                }
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                foreach (ListItem listItem in CblCoupon.Items)
                {
                    var couponId = TranslateUtils.ToInt(listItem.Value);
                    var updateActId = listItem.Selected ? _actId : 0;
                    DataProviderWx.CouponDao.UpdateActId(couponId, updateActId);
                }

                //DataProviderWx.CouponSnDao.UpdateStatus(ECouponStatusUtils.GetEnumType(this.ddlStatus.SelectedValue), TranslateUtils.StringCollectionToIntList(base.Request.QueryString["IDCollection"]));

                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "ʧ�ܣ�" + ex.Message);
            }

            if (isChanged)
            {
                PageUtils.CloseModalPage(Page);
            }
		}
	}
}
