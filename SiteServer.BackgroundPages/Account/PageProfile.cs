using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;

namespace SiteServer.BackgroundPages.Account
{
    public class PageProfile : BasePage
    {
        public Literal LtlUserName;
        public TextBox TbDisplayName;
        public TextBox TbEmail;
        public TextBox TbMobile;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Page.IsPostBack) return;

            LtlUserName.Text = AuthRequest.AdminInfo.UserName;
            TbDisplayName.Text = AuthRequest.AdminInfo.DisplayName;
            TbEmail.Text = AuthRequest.AdminInfo.Email;
            TbMobile.Text = AuthRequest.AdminInfo.Mobile;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var adminInfo = AdminManager.GetAdminInfoByUserId(AuthRequest.AdminId);

            adminInfo.DisplayName = TbDisplayName.Text;
            adminInfo.Email = TbEmail.Text;
            adminInfo.Mobile = TbMobile.Text;

            var updated = DataProvider.Administrator.Update(adminInfo, out var errorMessage);
            if (updated)
            {
                SuccessMessage("资料更改成功");
            }
            else
            {
                FailMessage(errorMessage);
            }
        }
    }
}
