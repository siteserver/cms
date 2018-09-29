using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageUtilityEncrypt : BasePage
    {
		public TextBox TbString;
        public PlaceHolder PhEncrypted;
        public TextBox TbEncrypted;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                VerifySystemPermissions(ConfigManager.SettingsPermissions.Utility);
            }
        }

		public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

		    PhEncrypted.Visible = true;
		    TbEncrypted.Text = TranslateUtils.EncryptStringBySecretKey(TbString.Text);
		}

	}
}
