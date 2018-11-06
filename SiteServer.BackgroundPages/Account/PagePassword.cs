using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils.Enumerations;

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
                LtlUserName.Text = AuthRequest.AdminName;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var adminInfo = AdminManager.GetAdminInfoByUserId(AuthRequest.AdminId);

            if (DataProvider.AdministratorDao.CheckPassword(TbCurrentPassword.Text, false, adminInfo.Password, EPasswordFormatUtils.GetEnumType(adminInfo.PasswordFormat), adminInfo.PasswordSalt))
            {
                string errorMessage;
                if (DataProvider.AdministratorDao.ChangePassword(adminInfo, TbNewPassword.Text, out errorMessage))
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
