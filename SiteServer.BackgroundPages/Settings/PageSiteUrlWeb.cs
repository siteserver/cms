using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteUrlWeb : BasePageCms
    {
		public Repeater RptContents;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteUrlWeb), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            var siteList = SiteManager.GetSiteIdListOrderByLevel();
            RptContents.DataSource = siteList;
            RptContents.ItemDataBound += DgContents_ItemDataBound;
            RptContents.DataBind();
        }

        private static void DgContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var siteId = (int)e.Item.DataItem;
            var siteInfo = SiteManager.GetSiteInfo(siteId);

            var ltlName = (Literal)e.Item.FindControl("ltlName");
            var ltlDir = (Literal)e.Item.FindControl("ltlDir");
            var ltlWebUrl = (Literal)e.Item.FindControl("ltlWebUrl");
            var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");

            ltlName.Text = SiteManager.GetSiteName(siteInfo);
            ltlDir.Text = siteInfo.SiteDir;

            ltlWebUrl.Text = $@"<a href=""{siteInfo.Additional.WebUrl}"" target=""_blank"">{siteInfo.Additional.WebUrl}</a>";

            ltlEditUrl.Text = $@"<a href=""{PageSiteUrlWebConfig.GetRedirectUrl(siteId)}"">修改</a>";
        }
	}
}
