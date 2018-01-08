using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

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

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Site);

            var publishmentSystemList = PublishmentSystemManager.GetPublishmentSystemIdListOrderByLevel();
            RptContents.DataSource = publishmentSystemList;
            RptContents.ItemDataBound += DgContents_ItemDataBound;
            RptContents.DataBind();
        }

        private static void DgContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var publishmentSystemId = (int)e.Item.DataItem;
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            var ltlName = (Literal)e.Item.FindControl("ltlName");
            var ltlDir = (Literal)e.Item.FindControl("ltlDir");
            var ltlWebUrl = (Literal)e.Item.FindControl("ltlWebUrl");
            var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");

            ltlName.Text = PublishmentSystemManager.GetPublishmentSystemName(publishmentSystemInfo);
            ltlDir.Text = publishmentSystemInfo.PublishmentSystemDir;

            ltlWebUrl.Text = $@"<a href=""{publishmentSystemInfo.Additional.WebUrl}"" target=""_blank"">{publishmentSystemInfo.Additional.WebUrl}</a>";

            ltlEditUrl.Text = $@"<a href=""{PageSiteUrlWebConfig.GetRedirectUrl(publishmentSystemId)}"">修改</a>";
        }
	}
}
