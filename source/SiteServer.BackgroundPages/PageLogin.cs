using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages
{
    public class PageLogin : BasePage
	{
        public Literal LtlMessage;
        public TextBox TbAccount;
        public TextBox TbPassword;
        public TextBox TbValidateCode;
        public Literal LtlValidateCodeImage;
        public CheckBox CbRememberMe;
	    public PlaceHolder PhFindPassword;

        private VcManager _vcManager; // 验证码类

        protected override bool IsAccessable => true; // 设置本页面是否能直接访问 如果为false，则必须管理员登录后才能访问

        public void Page_Load(object sender, EventArgs e)
		{
            if (IsForbidden) return; // 如果无权访问页面，则返回空白页

            try
            {
                _vcManager = VcManager.GetInstance(); // 构建验证码实例
                if (Page.IsPostBack) return;

                PhFindPassword.Visible = ConfigManager.SystemConfigInfo.IsFindPassword;

                if (Body.IsQueryExists("error")) // 如果url参数error不为空，则把错误信息显示到页面上
                {
                    LtlMessage.Text = GetMessageHtml(Body.GetQueryString("error"));
                }

                PageUtils.DetermineRedirectToInstaller(); // 判断是否需要安装，如果需要则转到安装页面。

                LtlValidateCodeImage.Text =
                        $@"<a href=""javascript:;"" onclick=""$('#imgVerify').attr('src', $('#imgVerify').attr('src') + '&' + new Date().getTime())""><img id=""imgVerify"" name=""imgVerify"" src=""{PageValidateCode.GetRedirectUrl(_vcManager.GetCookieName())}"" align=""absmiddle"" /></a>";
            }
            catch
            {
                // 再次探测是否需要安装或升级
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

            if (!_vcManager.IsCodeValid(TbValidateCode.Text)) // 检测验证码是否正确
            {
                LtlMessage.Text = GetMessageHtml("验证码不正确，请重新输入！");
                return;
            }

            string userName;
            string errorMessage;
            if (!BaiRongDataProvider.AdministratorDao.ValidateAccount(account, password, out userName, out errorMessage)) // 检测密码是否正确
            {
                LogUtils.AddAdminLog(userName, "后台管理员登录失败");
                BaiRongDataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfFailedLogin(userName); // 记录最后登录时间、失败次数+1
                LtlMessage.Text = GetMessageHtml(errorMessage); // 把错误信息显示在页面上
                return;
            }

            BaiRongDataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfLogin(userName); // 记录最后登录时间、失败次数清零
            Body.AdministratorLogin(userName); // 写Cookie并记录管理员操作日志
            PageUtils.Redirect(PageUtils.GetAdminDirectoryUrl(string.Empty)); // 跳转到登录成功的后台页
        }

        private string GetMessageHtml(string message) => $@"<div class=""alert alert-error"">{message}</div>";
	}
}
