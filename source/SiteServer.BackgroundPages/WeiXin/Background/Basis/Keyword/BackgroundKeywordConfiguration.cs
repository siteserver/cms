using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Core;


namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundKeywordConfiguration : BackgroundBasePageWX
	{
        public CheckBox cbIsWelcome;
        public TextBox tbWelcomeKeyword;
        public Button btnWelcomeKeywordSelect;

        public CheckBox cbIsDefaultReply;
        public TextBox tbDefaultReplyKeyword;
        public Button btnDefaultReplyKeywordSelect;

        public static string GetRedirectUrl(int publishmentSystemID)
        {
            return PageUtils.GetWXUrl($"background_keywordConfiguration.aspx?publishmentSystemID={publishmentSystemID}");
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Accounts, AppManager.WeiXin.LeftMenu.Function.ID_SetReply, string.Empty, AppManager.WeiXin.Permission.WebSite.SetReply);

                var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemID);

                cbIsWelcome.Checked = accountInfo.IsWelcome;
                tbWelcomeKeyword.Text = accountInfo.WelcomeKeyword;
                btnWelcomeKeywordSelect.Attributes.Add("onclick", Modal.KeywordSelect.GetOpenWindowString(PublishmentSystemID, "selectWelcomeKeyword"));

                cbIsDefaultReply.Checked = accountInfo.IsDefaultReply;
                tbDefaultReplyKeyword.Text = accountInfo.DefaultReplyKeyword;
                btnDefaultReplyKeywordSelect.Attributes.Add("onclick", Modal.KeywordSelect.GetOpenWindowString(PublishmentSystemID, "selectDefaultReplyKeyword"));
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				try
				{
                    var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemID);

                    accountInfo.IsWelcome = cbIsWelcome.Checked;
                    accountInfo.WelcomeKeyword = tbWelcomeKeyword.Text;
                    if (string.IsNullOrEmpty(tbWelcomeKeyword.Text))
                    {
                        accountInfo.IsWelcome = false;
                    }
                    accountInfo.IsDefaultReply = cbIsDefaultReply.Checked;
                    accountInfo.DefaultReplyKeyword = tbDefaultReplyKeyword.Text;
                    if (string.IsNullOrEmpty(tbDefaultReplyKeyword.Text))
                    {
                        accountInfo.IsDefaultReply = false;
                    }

                    if (!string.IsNullOrEmpty(accountInfo.WelcomeKeyword) && !DataProviderWX.KeywordMatchDAO.IsExists(PublishmentSystemID, accountInfo.WelcomeKeyword))
                    {
                        FailMessage($"保存失败，关键词“{accountInfo.WelcomeKeyword}”不存在，请先在关键词回复中添加");
                        return;
                    }
                    if (!string.IsNullOrEmpty(accountInfo.DefaultReplyKeyword) && !DataProviderWX.KeywordMatchDAO.IsExists(PublishmentSystemID, accountInfo.DefaultReplyKeyword))
                    {
                        FailMessage($"保存失败，关键词“{accountInfo.DefaultReplyKeyword}”不存在，请先在关键词回复中添加");
                        return;
                    }

                    DataProviderWX.AccountDAO.Update(accountInfo);

                    StringUtility.AddLog(PublishmentSystemID, "修改智能回复设置");
                    SuccessMessage("智能回复设置配置成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "智能回复设置配置失败！");
				}
			}
		}
	}
}
