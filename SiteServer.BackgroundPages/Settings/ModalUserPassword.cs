using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalUserPassword : BasePage
    {
		public Literal LtlUserName;
		public TextBox TbPassword;

        private string _userName;

        public static string GetOpenWindowString(string userName)
        {
            return LayerUtils.GetOpenScript("重设密码", PageUtils.GetSettingsUrl(nameof(ModalUserPassword), new NameValueCollection
            {
                {"userName", userName}
            }), 450, 290);
        }
        
		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _userName = AuthRequest.GetQueryString("userName");

            if (IsPostBack) return;

            if (!string.IsNullOrEmpty(_userName))
            {
                LtlUserName.Text = _userName;
            }
            else
            {
                FailMessage("此帐户不存在！");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!IsPostBack || !IsValid) return;

            try
            {
                string errorMessage;
                if (DataProvider.UserDao.ChangePassword(_userName, TbPassword.Text, out errorMessage))
                {
                    SuccessMessage("重设密码成功！");
                }
                else
                {
                    FailMessage(errorMessage);
                    return;
                }

                LayerUtils.Close(Page);
            }
            catch(Exception ex)
            {
                FailMessage(ex, "重设密码失败！");
            }
        }

	}
}
