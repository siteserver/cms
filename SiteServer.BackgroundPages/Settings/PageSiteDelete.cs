using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteDelete : BasePageCms
	{
	    public Literal LtlSiteName;
		public RadioButtonList RblRetainFiles;

	    public static string GetRedirectUrl(int siteId)
	    {
	        return PageUtils.GetSettingsUrl(nameof(PageSiteDelete), new NameValueCollection
	        {
	            {"siteId", siteId.ToString()}
	        });
	    }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            LtlSiteName.Text = SiteInfo.SiteName;

            InfoMessage($"此操作将会删除站点“{SiteInfo.SiteName}({SiteInfo.SiteDir})”，确认吗？");
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var isRetainFiles = TranslateUtils.ToBool(RblRetainFiles.SelectedValue);

            if (isRetainFiles == false)
            {
                DirectoryUtility.DeleteSiteFiles(SiteInfo);
                SuccessMessage("成功删除站点以及相关文件！");
            }
            else
            {
                SuccessMessage("成功删除站点，相关文件未被删除！");
            }

            if (AuthRequest.AdminInfo.SiteId != SiteId)
            {
                AddWaitAndRedirectScript(PageSite.GetRedirectUrl());
            }
            else
            {
                AddScript(
                    $@"setTimeout(""window.top.location.href='{PageUtils.GetMainUrl(0)}'"", 1500);");
            }

            AuthRequest.AddAdminLog("删除站点", $"站点:{SiteInfo.SiteName}");

            DataProvider.SiteDao.Delete(SiteId);
        }

	    public void Return_OnClick(object sender, EventArgs e)
	    {
	        PageUtils.Redirect(PageSite.GetRedirectUrl());
	    }
	}
}
