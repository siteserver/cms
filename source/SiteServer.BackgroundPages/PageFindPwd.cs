using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages
{
    public class PageFindPwd : BasePage
    {
        protected Literal LtlPageTitle;
        protected Literal LtlMessage;

        protected PlaceHolder PhStepAccount;
        protected TextBox TbAccount;
        protected PlaceHolder PhValidateCode;
        protected TextBox TbValidateCode;
        protected Literal LtlValidateCodeImage;

        protected PlaceHolder PhStepSmsCode;
        protected TextBox TbSmsCode;

        protected PlaceHolder PhStepChangePassword;
        protected TextBox TbPassword;
        protected TextBox TbConfirmPassword;

        private VcManager _vcManager;

        protected override bool IsAccessable => true;

        private string GetMessageHtml(string message, bool isError) => $@"<div class=""alert {(isError ? "alert-error" : "alert-info")}"">{message}</div>";

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _vcManager = VcManager.GetInstance();
            if (Page.IsPostBack) return;

            if (!FileConfigManager.Instance.IsFindPassword)
            {
                PageUtils.RedirectToErrorPage("基于安全考虑，找回密码功能已关闭，如需使用请与管理员联系！");
            }
            else if (!SmsManager.IsSmsReady())
            {
                PageUtils.RedirectToErrorPage("短信发送未开启或配置不正确，找回密码功能无法使用，如需使用请与管理员联系！");
            }

            if (FileConfigManager.Instance.IsValidateCode)
            {
                LtlValidateCodeImage.Text =
                    $@"<img id=""imgVerify"" name=""imgVerify"" src=""{PageValidateCode.GetRedirectUrl(_vcManager.GetCookieName())}"" align=""absmiddle"" />";
            }
            else
            {
                PhValidateCode.Visible = false;
            }

            this.LtlPageTitle.Text = "找回密码";
        }

        public void Account_OnClick(object sender, EventArgs e)
        {
            var account = TbAccount.Text;

            if (FileConfigManager.Instance.IsValidateCode)
            {
                if (!_vcManager.IsCodeValid(TbValidateCode.Text))
                {
                    LtlMessage.Text = GetMessageHtml("验证码不正确，请重新输入！", true);
                    return;
                }
            }

            string userName = null;
            if (StringUtils.IsMobile(account))
            {
                userName = BaiRongDataProvider.AdministratorDao.GetUserNameByMobile(account);
            }
            else if (StringUtils.IsEmail(account))
            {
                userName = BaiRongDataProvider.AdministratorDao.GetUserNameByEmail(account);
            }
            else
            {
                if (BaiRongDataProvider.AdministratorDao.IsUserNameExists(account))
                {
                    userName = account;
                }
            }

            if (string.IsNullOrEmpty(userName))
            {
                LtlMessage.Text = GetMessageHtml("找回密码错误，输入的账号不存在", true);
                return;
            }

            var mobile = BaiRongDataProvider.AdministratorDao.GetMobileByUserName(account);
            if (string.IsNullOrEmpty(mobile) || !StringUtils.IsMobile(mobile))
            {
                LtlMessage.Text = GetMessageHtml("找回密码错误，账号对应的管理员未设置手机号码", true);
                return;
            }

            string errorMessage;
            var code = StringUtils.GetRandomInt(1111, 9999);
            DbCacheManager.RemoveAndInsert($"BaiRong.BackgroundPages.FrameworkFindPwd.{userName}.Code", code.ToString());
            var isSend = SmsManager.SendCode(mobile, code, out errorMessage);
            if (!isSend)
            {
                LtlMessage.Text = GetMessageHtml($"找回密码错误：{errorMessage}", true);
                return;
            }

            base.ViewState["UserName"] = userName;
            this.LtlPageTitle.Text = "验证手机";
            LtlMessage.Text = GetMessageHtml($"短信验证码已发送至：{mobile.Substring(0, 3) + "*****" + mobile.Substring(8)}", true);
            PhStepAccount.Visible = false;
            PhStepSmsCode.Visible = true;
            PhStepChangePassword.Visible = false;
        }

        public void SmsCode_OnClick(object sender, EventArgs e)
        {
            var smsCode = TbSmsCode.Text;
            var userName = base.ViewState["UserName"];
            var code = DbCacheManager.GetValueAndRemove($"BaiRong.BackgroundPages.FrameworkFindPwd.{userName}.Code");

            if (smsCode != code)
            {
                LtlMessage.Text = GetMessageHtml("找回密码错误：短信验证码不正确", true);
                return;
            }

            this.LtlPageTitle.Text = "重设密码";
            LtlMessage.Text = string.Empty;
            PhStepAccount.Visible = false;
            PhStepSmsCode.Visible = false;
            PhStepChangePassword.Visible = true;
        }

        public void ChangePassword_OnClick(object sender, EventArgs e)
        {
            var userName = base.ViewState["UserName"] as string;

            var password = TbPassword.Text;
            var confirmPassword = TbConfirmPassword.Text;

            if (password != confirmPassword)
            {
                LtlMessage.Text = GetMessageHtml("重设密码错误：新密码与确认密码不一致", true);
                return;
            }

            string errorMessage;
            if (!BaiRongDataProvider.AdministratorDao.ChangePassword(userName, EPasswordFormat.Encrypted, password, out errorMessage))
            {
                LtlMessage.Text = GetMessageHtml($"重设密码错误：{errorMessage}", true);
                return;
            }

            this.LtlPageTitle.Text = "密码设置成功";
            LtlMessage.Text = GetMessageHtml("密码设置成功，系统将跳转至登录页面", false);
            base.AddWaitAndRedirectScript("login.aspx");
            PhStepAccount.Visible = false;
            PhStepSmsCode.Visible = false;
            PhStepChangePassword.Visible = false;
        }
    }
}
