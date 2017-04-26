using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.User
{
	public class ModalUserPassword : BasePage
    {
		public Label ltlUserName;
		public TextBox tbPassword;

        private string _userName;

        public static string GetOpenWindowString(string userName)
        {
            return PageUtils.GetOpenWindowString("重设密码", PageUtils.GetUserUrl(nameof(ModalUserPassword), new NameValueCollection
            {
                {"userName", userName}
            }), 400, 300);
        }
        
		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _userName = Body.GetQueryString("userName");

			if (!IsPostBack)
			{
                if (!string.IsNullOrEmpty(_userName))
                {
                    ltlUserName.Text = _userName;
                }
                else
                {
                    FailMessage("此帐户不存在！");
                }
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			if (IsPostBack && IsValid)
			{
				try
				{
				    string errorMessage;
				    if (BaiRongDataProvider.UserDao.ChangePassword(_userName, tbPassword.Text, out errorMessage))
				    {
				        SuccessMessage("重设密码成功！");
				    }
				    else
				    {
                        FailMessage(errorMessage);
				        return;
				    }

                    PageUtils.CloseModalPage(Page);
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "重设密码失败！");
				}
			}
		}

	}
}
