using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Settings
{
	public class PagePublishmentSystemUrlWeb : BasePageCms
    {
		public DataGrid DgContents;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PagePublishmentSystemUrlWeb), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbSettings("访问地址管理", AppManager.Permissions.Settings.SiteManagement);

            var publishmentSystemList = PublishmentSystemManager.GetPublishmentSystemIdListOrderByLevel();
            DgContents.DataSource = publishmentSystemList;
            DgContents.ItemDataBound += DgContents_ItemDataBound;
            DgContents.DataBind();
        }

        private static void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
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

            ltlEditUrl.Text = $@"<a href=""{PagePublishmentSystemUrlWebConfig.GetRedirectUrl(publishmentSystemId)}"">修改</a>";
        }
	}
}
