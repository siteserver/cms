using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.BackgroundPages;

namespace SiteServer.WeiXin.BackgroundPages
{
	public class ConsoleAccountSync : BackgroundBasePage
	{
		public Button btnSync;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
                //base.BreadCrumbConsole(AppManager.CMS.LeftMenu.ID_Site, "系统站点管理", AppManager.Permission.Platform_Site);
			}
		}

        public void btnSync_Click(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                //ArrayList publishmentSystemIDArrayList = PublishmentSystemManager.GetPublishmentSystemIDArrayList(EPublishmentSystemType.Weixin);
                //foreach (int publishmentSystemID in publishmentSystemIDArrayList)
                //{
                //    PublishmentSystemInfo publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemID);

                //    AccountInfo accountInfo = new AccountInfo { PublishmentSystemID = publishmentSystemID, Token = publishmentSystemInfo.Additional.WXToken, IsBinding = publishmentSystemInfo.Additional.WXIsBinding, AccountType = string.Empty, WeChatID = string.Empty, SourceID = string.Empty, ThumbUrl = string.Empty, AppID = publishmentSystemInfo.Additional.WXAppID, AppSecret = publishmentSystemInfo.Additional.WXAppSecret, IsWelcome = publishmentSystemInfo.Additional.WXIsWelcome, WelcomeKeyword = publishmentSystemInfo.Additional.WXWelcomeKeyword, IsDefaultReply = publishmentSystemInfo.Additional.WXIsDefaultReply, DefaultReplyKeyword = publishmentSystemInfo.Additional.WXDefaultReplyKeyword };

                //    DataProviderWX.AccountDAO.Insert(accountInfo);

                //    base.SuccessMessage("同步微信公众帐号成功！");
                //}
            }
        }
	}
}
