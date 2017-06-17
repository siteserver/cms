using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalCouponSnSetting : BasePageCms
    {
        public DropDownList DdlStatus;

        private int _couponId;

        public static string GetOpenWindowString(int publishmentSystemId, int couponId)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("新增优惠劵数量",
                PageUtils.GetWeiXinUrl(nameof(ModalCouponSnSetting), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"couponId", couponId.ToString()}
                }), "IDCollection", "请选择需要设置的SN码", 400, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _couponId = Body.GetQueryInt("couponID");

			if (!IsPostBack)
			{
                ECouponStatusUtils.AddListItems(DdlStatus);
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                DataProviderWx.CouponSnDao.UpdateStatus(ECouponStatusUtils.GetEnumType(DdlStatus.SelectedValue), TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]));

                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }

            if (isChanged)
            {
                PageUtils.CloseModalPage(Page);
            }
		}
	}
}
