using System;
using System.Web.UI.WebControls;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Platform
{
    public class PageUserProfile : BasePage
    {
        public Literal UserName;
        public TextBox DisplayName;
        public TextBox Email;
        public TextBox Mobile;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!Page.IsPostBack)
            {
                UserName.Text = Body.AdministratorInfo.UserName;
                DisplayName.Text = Body.AdministratorInfo.DisplayName;
                Email.Text = Body.AdministratorInfo.Email;
                Mobile.Text = Body.AdministratorInfo.Mobile;
            }
        }

        public void Submit_Click(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                Body.AdministratorInfo.DisplayName = DisplayName.Text;
                Body.AdministratorInfo.Email = Email.Text;
                Body.AdministratorInfo.Mobile = Mobile.Text;

                BaiRongDataProvider.AdministratorDao.Update(Body.AdministratorInfo);

                SuccessMessage("资料更改成功");
            }
        }
    }
}
