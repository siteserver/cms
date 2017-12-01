using System;
using System.Web.UI.WebControls;

namespace SiteServer.BackgroundPages.WeiXin
{
	public class PageAccountSync : BasePageCms
	{
		public Button BtnSync;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
                //base.BreadCrumbConsole(AppManager.CMS.LeftMenu.Id_Site, "系统站点管理", AppManager.Permission.Platform_Site);
			}
		}

        public void btnSync_Click(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                //ArrayList PublishmentSystemIdArrayList = PublishmentSystemManager.GetPublishmentSystemIdArrayList(EPublishmentSystemType.Weixin);
                //foreach (int PublishmentSystemId in PublishmentSystemIdArrayList)
                //{
                //    PublishmentSystemInfo publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(PublishmentSystemId);

                //    AccountInfo accountInfo = new AccountInfo { PublishmentSystemId = PublishmentSystemId, Token = publishmentSystemInfo.Additional.WXToken, IsBinding = publishmentSystemInfo.Additional.WXIsBinding, AccountType = string.Empty, WeChatID = string.Empty, SourceID = string.Empty, ThumbUrl = string.Empty, AppID = publishmentSystemInfo.Additional.WXAppID, AppSecret = publishmentSystemInfo.Additional.WXAppSecret, IsWelcome = publishmentSystemInfo.Additional.WXIsWelcome, WelcomeKeyword = publishmentSystemInfo.Additional.WXWelcomeKeyword, IsDefaultReply = publishmentSystemInfo.Additional.WXIsDefaultReply, DefaultReplyKeyword = publishmentSystemInfo.Additional.WXDefaultReplyKeyword };

                //    DataProviderWx.AccountDao.Insert(accountInfo);

                //    base.SuccessMessage("同步微信公众帐号成功！");
                //}
            }
        }
	}
}
