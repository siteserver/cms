using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Permissions;

namespace SiteServer.BackgroundPages.Admin
{
	public class ModalAdminView : BasePage
    {
        protected Literal ltlUserName;
        protected Literal ltlDisplayName;
        protected Literal ltlCreationDate;
        protected Literal ltlLastActivityDate;
        protected Literal ltlEmail;
        protected Literal ltlMobile;
        protected Literal ltlRoles;

        public static string GetOpenWindowString(string userName)
        {
            return PageUtils.GetOpenWindowString("查看管理员资料", PageUtils.GetAdminUrl(nameof(ModalAdminView), new NameValueCollection
            {
                {"UserName", userName}
            }), 400, 450, true);
        }
	
		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var userName = Body.GetQueryString("UserName");
            var adminInfo = BaiRongDataProvider.AdministratorDao.GetByUserName(userName);
            ltlUserName.Text = adminInfo.UserName;
            ltlDisplayName.Text = adminInfo.DisplayName;
            ltlCreationDate.Text = DateUtils.GetDateAndTimeString(adminInfo.CreationDate);
            ltlLastActivityDate.Text = DateUtils.GetDateAndTimeString(adminInfo.LastActivityDate);
            ltlEmail.Text = adminInfo.Email;
            ltlMobile.Text = adminInfo.Mobile;
            ltlRoles.Text = AdminManager.GetRolesHtml(userName);
		}
	}
}
