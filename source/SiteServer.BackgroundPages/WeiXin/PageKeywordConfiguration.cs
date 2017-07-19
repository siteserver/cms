using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageKeywordConfiguration : BasePageCms
    {
        public CheckBox CbIsWelcome;
        public TextBox TbWelcomeKeyword;
        public Button BtnWelcomeKeywordSelect;

        public CheckBox CbIsDefaultReply;
        public TextBox TbDefaultReplyKeyword;
        public Button BtnDefaultReplyKeywordSelect;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageKeywordConfiguration), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.WeiXin.LeftMenu.IdAccounts, string.Empty, AppManager.WeiXin.Permission.WebSite.SetReply);

                var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemId);

                CbIsWelcome.Checked = accountInfo.IsWelcome;
                TbWelcomeKeyword.Text = accountInfo.WelcomeKeyword;
                BtnWelcomeKeywordSelect.Attributes.Add("onclick", ModalKeywordSelect.GetOpenWindowString(PublishmentSystemId, "selectWelcomeKeyword"));

                CbIsDefaultReply.Checked = accountInfo.IsDefaultReply;
                TbDefaultReplyKeyword.Text = accountInfo.DefaultReplyKeyword;
                BtnDefaultReplyKeywordSelect.Attributes.Add("onclick", ModalKeywordSelect.GetOpenWindowString(PublishmentSystemId, "selectDefaultReplyKeyword"));
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				try
				{
                    var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemId);

                    accountInfo.IsWelcome = CbIsWelcome.Checked;
                    accountInfo.WelcomeKeyword = TbWelcomeKeyword.Text;
                    if (string.IsNullOrEmpty(TbWelcomeKeyword.Text))
                    {
                        accountInfo.IsWelcome = false;
                    }
                    accountInfo.IsDefaultReply = CbIsDefaultReply.Checked;
                    accountInfo.DefaultReplyKeyword = TbDefaultReplyKeyword.Text;
                    if (string.IsNullOrEmpty(TbDefaultReplyKeyword.Text))
                    {
                        accountInfo.IsDefaultReply = false;
                    }

                    if (!string.IsNullOrEmpty(accountInfo.WelcomeKeyword) && !DataProviderWx.KeywordMatchDao.IsExists(PublishmentSystemId, accountInfo.WelcomeKeyword))
                    {
                        FailMessage($"保存失败，关键词“{accountInfo.WelcomeKeyword}”不存在，请先在关键词回复中添加");
                        return;
                    }
                    if (!string.IsNullOrEmpty(accountInfo.DefaultReplyKeyword) && !DataProviderWx.KeywordMatchDao.IsExists(PublishmentSystemId, accountInfo.DefaultReplyKeyword))
                    {
                        FailMessage($"保存失败，关键词“{accountInfo.DefaultReplyKeyword}”不存在，请先在关键词回复中添加");
                        return;
                    }

                    DataProviderWx.AccountDao.Update(accountInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改智能回复设置");
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
