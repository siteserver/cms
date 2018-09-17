using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteUrlAssets : BasePageCms
    {
		public Repeater RptContents;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteUrlAssets), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            var siteList = SiteManager.GetSiteIdListOrderByLevel();
            RptContents.DataSource = siteList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private static void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var siteId = (int)e.Item.DataItem;
            var siteInfo = SiteManager.GetSiteInfo(siteId);

            var ltlName = (Literal)e.Item.FindControl("ltlName");
            var ltlDir = (Literal)e.Item.FindControl("ltlDir");
            var ltlAssetsDir = (Literal)e.Item.FindControl("ltlAssetsDir");
            var ltlAssetsUrl = (Literal)e.Item.FindControl("ltlAssetsUrl");
            var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");

            ltlName.Text = SiteManager.GetSiteName(siteInfo);
            ltlDir.Text = siteInfo.SiteDir;

            ltlAssetsDir.Text = siteInfo.Additional.AssetsDir;
            ltlAssetsUrl.Text = $@"<a href=""{siteInfo.Additional.AssetsUrl}"" target=""_blank"">{siteInfo.Additional.AssetsUrl}</a>";

            ltlEditUrl.Text = $@"<a href=""{PageSiteUrlAssetsConfig.GetRedirectUrl(siteId)}"">修改</a>";
        }
	}
}
