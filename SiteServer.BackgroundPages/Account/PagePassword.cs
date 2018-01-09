using System;
using System.Web.UI.WebControls;
using BaiRong.Core;

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

        public void Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            if (BaiRongDataProvider.AdministratorDao.CheckPassword(TbCurrentPassword.Text, Body.AdministratorInfo.Password, Body.AdministratorInfo.PasswordFormat, Body.AdministratorInfo.PasswordSalt))
            {
                string errorMessage;
                if (BaiRongDataProvider.AdministratorDao.ChangePassword(Body.AdministratorInfo.UserName, TbNewPassword.Text, out errorMessage))
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
