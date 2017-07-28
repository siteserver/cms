using System;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;

using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class WinnerSetting : BackgroundBasePage
	{
        public DropDownList ddlStatus;

        public static string GetOpenWindowString(int publishmentSystemID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            return PageUtilityWX.GetOpenWindowStringWithCheckBoxValue("设置中奖状态", "modal_winnerSetting.aspx", arguments, "IDCollection", "请选择需要设置的中奖名单", 400, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
                EWinStatusUtils.AddListItems(ddlStatus);
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {

                DataProviderWX.LotteryWinnerDAO.UpdateStatus(EWinStatusUtils.GetEnumType(ddlStatus.SelectedValue), TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]));

                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }

            if (isChanged)
            {
                JsUtils.OpenWindow.CloseModalPage(Page);
            }
		}
	}
}
