using System;
using System.Web.UI.WebControls;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Platform
{
    public class PageUserPassword : BasePage
    {
        public Literal UserName;
        public TextBox CurrentPassword;
        public TextBox NewPassword;
        public TextBox ConfirmNewPassword;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!Page.IsPostBack)
            {
                UserName.Text = Body.AdministratorName;
            }
        }

        public void Submit_Click(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (BaiRongDataProvider.AdministratorDao.CheckPassword(CurrentPassword.Text, Body.AdministratorInfo.Password, Body.AdministratorInfo.PasswordFormat, Body.AdministratorInfo.PasswordSalt))
                {
                    var errorMessage = string.Empty;
                    if (BaiRongDataProvider.AdministratorDao.ChangePassword(Body.AdministratorInfo.UserName, Body.AdministratorInfo.PasswordFormat, NewPassword.Text, out errorMessage))
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
}
