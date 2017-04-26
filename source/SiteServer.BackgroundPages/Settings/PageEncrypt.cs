using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Cryptography;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageEncrypt : BasePage
    {
		public TextBox RawString;
        public Literal ltlString;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                BreadCrumbSettings(AppManager.Settings.LeftMenu.Utility, "加密字符串", AppManager.Settings.Permission.SettingsUtility);
            }
        }

		public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                ltlString.Text = EncryptUtils.Instance.EncryptString(RawString.Text);
			}
		}

	}
}
