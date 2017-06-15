using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalWinnerSetting : BasePageCms
    {
        public DropDownList DdlStatus;

        public static string GetOpenWindowString(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("设置中奖状态",
                PageUtils.GetWeiXinUrl(nameof(ModalWinnerSetting), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()}
                }), "IDCollection", "请选择需要设置的中奖名单", 400, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
                EWinStatusUtils.AddListItems(DdlStatus);
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {

                DataProviderWx.LotteryWinnerDao.UpdateStatus(EWinStatusUtils.GetEnumType(DdlStatus.SelectedValue), TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]));

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
