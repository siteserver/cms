using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Plugins
{
	public class PageBackup : BasePageCms
    {
		public DropDownList BackupType;
		public Button BackupButton;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Body.IsQueryExists("isRedirectToFiles"))
            {
                var redirectUrl = PageUtils.GetPluginsUrl("PageFileMain", new NameValueCollection
                {
                    {"RootPath", $"~/SiteFiles/BackupFiles/{PublishmentSystemInfo.PublishmentSystemDir}"}
                });
                PageUtils.Redirect(redirectUrl);
                return;
            }

			if (!IsPostBack)
            {
                EBackupTypeUtils.AddListItems(BackupType);
			}
		}


		public void BackupButton_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                var url = PageProgressBar.GetBackupUrl(PublishmentSystemId, BackupType.SelectedValue, StringUtils.Guid());
                PageUtils.Redirect(url);
			}
		}
	}
}
