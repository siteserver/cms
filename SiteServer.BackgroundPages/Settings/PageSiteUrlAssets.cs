using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

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

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Site);

            var publishmentSystemList = PublishmentSystemManager.GetPublishmentSystemIdListOrderByLevel();
            RptContents.DataSource = publishmentSystemList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private static void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var publishmentSystemId = (int)e.Item.DataItem;
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            var ltlName = (Literal)e.Item.FindControl("ltlName");
            var ltlDir = (Literal)e.Item.FindControl("ltlDir");
            var ltlAssetsDir = (Literal)e.Item.FindControl("ltlAssetsDir");
            var ltlAssetsUrl = (Literal)e.Item.FindControl("ltlAssetsUrl");
            var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");

            ltlName.Text = PublishmentSystemManager.GetPublishmentSystemName(publishmentSystemInfo);
            ltlDir.Text = publishmentSystemInfo.PublishmentSystemDir;

            ltlAssetsDir.Text = publishmentSystemInfo.Additional.AssetsDir;
            ltlAssetsUrl.Text = $@"<a href=""{publishmentSystemInfo.Additional.AssetsUrl}"" target=""_blank"">{publishmentSystemInfo.Additional.AssetsUrl}</a>";

            ltlEditUrl.Text = $@"<a href=""{PageSiteUrlAssetsConfig.GetRedirectUrl(publishmentSystemId)}"">修改</a>";
        }
	}
}
