using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Account
{
    public class PagePassword : BasePage
    {
        public Literal LtlUserName;
        public TextBox TbCurrentPassword;
        public TextBox TbNewPassword;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!Page.IsPostBack)
            {
                LtlUserName.Text = Body.AdminName;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            if (DataProvider.AdministratorDao.CheckPassword(TbCurrentPassword.Text, Body.AdministratorInfo.Password, Body.AdministratorInfo.PasswordFormat, Body.AdministratorInfo.PasswordSalt))
            {
                string errorMessage;
                if (DataProvider.AdministratorDao.ChangePassword(Body.AdministratorInfo.UserName, TbNewPassword.Text, out errorMessage))
                {
                    SuccessMessage("密码更改成功");
                }
                else
                {
                    FailMessage(errorMessage);
                }
            }
            else
            {
                FailMessage("当前帐号密码错误");
            }
        }
    }
}
