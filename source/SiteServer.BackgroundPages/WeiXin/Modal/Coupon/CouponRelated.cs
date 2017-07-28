using System;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;

using SiteServer.WeiXin.Core;

namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class CouponRelated : BackgroundBasePage
	{
        public CheckBoxList cblCoupon;

        private int actID;

        public static string GetOpenWindowString(int publishmentSystemID, int actID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("actID", actID.ToString());
            return PageUtilityWX.GetOpenWindowString("�����Ż݄�", "modal_couponRelated.aspx", arguments, 400, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            actID = TranslateUtils.ToInt(GetQueryString("actID"));

            var actIDList = DataProviderWX.CouponActDAO.GetActIDList(PublishmentSystemID);

			if (!IsPostBack)
			{
                var allCouponInfoList = DataProviderWX.CouponDAO.GetAllCouponInfoList(PublishmentSystemID);
                foreach (var couponInfo in allCouponInfoList)
                {
                    if (!actIDList.Contains(couponInfo.ActID) || couponInfo.ActID == 0 || couponInfo.ActID == actID)
                    {
                        var listItem = new ListItem(couponInfo.Title, couponInfo.ID.ToString());
                        if (couponInfo.ActID == actID)
                        {
                            listItem.Selected = true;
                        }
                        cblCoupon.Items.Add(listItem);
                    }
                }
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                foreach (ListItem listItem in cblCoupon.Items)
                {
                    var couponID = TranslateUtils.ToInt(listItem.Value);
                    var updateActID = listItem.Selected ? actID : 0;
                    DataProviderWX.CouponDAO.UpdateActID(couponID, updateActID);
                }

                //DataProviderWX.CouponSNDAO.UpdateStatus(ECouponStatusUtils.GetEnumType(this.ddlStatus.SelectedValue), TranslateUtils.StringCollectionToIntList(base.Request.QueryString["IDCollection"]));

                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "ʧ�ܣ�" + ex.Message);
            }

            if (isChanged)
            {
                JsUtils.OpenWindow.CloseModalPage(Page);
            }
		}
	}
}
