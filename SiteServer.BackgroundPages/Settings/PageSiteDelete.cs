using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteDelete : BasePageCms
	{
	    public Literal LtlPublishmentSystemName;
		public RadioButtonList RblRetainFiles;

	    public static string GetRedirectUrl(int publishmentSystemId)
	    {
	        return PageUtils.GetSettingsUrl(nameof(PageSiteDelete), new NameValueCollection
	        {
	            {"publishmentSystemId", publishmentSystemId.ToString()}
	        });
	    }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Site);

            LtlPublishmentSystemName.Text = PublishmentSystemInfo.PublishmentSystemName;

            InfoMessage($"此操作将会删除站点“{PublishmentSystemInfo.PublishmentSystemName}({PublishmentSystemInfo.PublishmentSystemDir})”，确认吗？");
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var isRetainFiles = TranslateUtils.ToBool(RblRetainFiles.SelectedValue);

            if (isRetainFiles == false)
            {
                DirectoryUtility.DeletePublishmentSystemFiles(PublishmentSystemInfo);
                SuccessMessage("成功删除站点以及相关文件！");
            }
            else
            {
                SuccessMessage("成功删除站点，相关文件未被删除！");
            }

            if (Body.AdministratorInfo.PublishmentSystemId != PublishmentSystemId)
            {
                AddWaitAndRedirectScript(PageSite.GetRedirectUrl());
            }
            else
            {
                AddScript(
                    $@"setTimeout(""window.top.location.href='{PageMain.GetRedirectUrl()}'"", 1500);");
            }

            Body.AddAdminLog("删除站点", $"站点:{PublishmentSystemInfo.PublishmentSystemName}");

            DataProvider.PublishmentSystemDao.Delete(PublishmentSystemId);
        }

	    public void Return_OnClick(object sender, EventArgs e)
	    {
	        PageUtils.Redirect(PageSite.GetRedirectUrl());
	    }
	}
}
