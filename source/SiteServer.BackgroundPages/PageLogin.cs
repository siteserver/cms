using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Text;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages
{
    public class PageLogin : BasePage
	{
        protected Literal LtlMessage;
		protected TextBox TbAccount;
		protected TextBox TbPassword;
        protected PlaceHolder PhValidateCode;
        protected TextBox TbValidateCode;
        protected Literal LtlValidateCodeImage;
        protected CheckBox CbRememberMe;

        private VcManager _vcManager;

        protected override bool IsAccessable => true;

	    public void Page_Load(object sender, EventArgs e)
		{
            if (IsForbidden) return;

            try
            {
                _vcManager = VcManager.GetInstance();
                if (!Page.IsPostBack)
                {
                    if (Body.IsQueryExists("error"))
                    {
                        LtlMessage.Text = GetMessageHtml(Body.GetQueryString("error"));
                    }
                    if (RestrictionManager.IsVisitAllowed(ConfigManager.SystemConfigInfo.RestrictionType, ConfigManager.Instance.RestrictionBlackList, ConfigManager.Instance.RestrictionWhiteList))
                    {
                        PageUtils.DetermineRedirectToInstaller();

                        if (FileConfigManager.Instance.IsValidateCode)
                        {
                            LtlValidateCodeImage.Text =
                                $@"<a href=""javascript:;"" onclick=""$('#imgVerify').attr('src', $('#imgVerify').attr('src') + '&' + new Date().getTime())""><img id=""imgVerify"" name=""imgVerify"" src=""{PageValidateCode.GetRedirectUrl(_vcManager.GetCookieName())}"" align=""absmiddle"" /></a>";
                        }
                        else
                        {
                            PhValidateCode.Visible = false;
                        }
                    }
                    else
                    {
                        Page.Response.Write("<h1>此页面禁止访问.</h1>");
                        Page.Response.Write($"<p>IP地址：{PageUtils.GetIpAddress()}<br />需要访问此页面请与网站管理员联系开通相关权限.</p>");
                        Page.Response.End();
                    }
                }
            }
            catch
            {
                if (AppManager.IsNeedInstall())
                {
                    PageUtils.Redirect("installer/default.aspx");
                }
                else if (AppManager.IsNeedUpgrade())
                {
                    PageUtils.Redirect("upgrade/default.aspx");
                }
                else
                {
                    throw;
                }
            }
		}
		
		public override void Submit_OnClick(object sender, EventArgs e)
		{
            var account = TbAccount.Text;
            var password = TbPassword.Text;

            if (FileConfigManager.Instance.IsValidateCode)
            {
                if (!_vcManager.IsCodeValid(TbValidateCode.Text))
                {
                    LtlMessage.Text = GetMessageHtml("验证码不正确，请重新输入！");
                    return;
                }
            }

		    string userName;
            string errorMessage;
            if (!BaiRongDataProvider.AdministratorDao.ValidateAccount(account, password, out userName, out errorMessage))
            {
                LogUtils.AddAdminLog(userName, "后台管理员登录失败");
                BaiRongDataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfFailedLogin(userName);
                LtlMessage.Text = GetMessageHtml(errorMessage);
                return;
            }

            BaiRongDataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfLogin(userName);
            Body.AdministratorLogin(userName);
            PageUtils.Redirect(PageUtils.GetAdminDirectoryUrl(string.Empty));
        }

        private string GetMessageHtml(string message) => $@"<div class=""alert alert-error"">{message}</div>";
	}
}
