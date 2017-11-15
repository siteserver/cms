using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
	public class PagePublishmentSystemUrl : BasePageCms
    {
		public DataGrid DgContents;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbSettings("访问地址管理", AppManager.Permissions.Settings.SiteManagement);

            var publishmentSystemList = PublishmentSystemManager.GetPublishmentSystemIdListOrderByLevel();
            DgContents.DataSource = publishmentSystemList;
            DgContents.ItemDataBound += DgContents_ItemDataBound;
            DgContents.DataBind();

            if (ConfigManager.SystemConfigInfo.IsUrlGlobalSetting)
            {
                DgContents.Columns[DgContents.Columns.Count - 1].Visible = false;
            }
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

            ltlName.Text = GetPublishmentSystemName(publishmentSystemInfo);
            ltlDir.Text = publishmentSystemInfo.PublishmentSystemDir;

            ltlWebUrl.Text = $@"<a href=""{publishmentSystemInfo.Additional.WebUrl}"" target=""_blank"">{publishmentSystemInfo.Additional.WebUrl}</a>";

            ltlEditUrl.Text =
                $"<a href=\"javascript:;\" onClick=\"{ModalChangePublishmentSystemUrl.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId)}\">修改</a>";
        }

        private static string GetPublishmentSystemName(PublishmentSystemInfo publishmentSystemInfo)
		{
		    var padding = string.Empty;

            var level = PublishmentSystemManager.GetPublishmentSystemLevel(publishmentSystemInfo.PublishmentSystemId);
            string psLogo;
            if (publishmentSystemInfo.IsHeadquarters)
            {
                psLogo = "siteHQ.gif";
            }
            else
            {
                psLogo = "site.gif";
                if (level > 0 && level < 10)
                {
                    psLogo = $"subsite{level + 1}.gif";
                }
            }
            psLogo = SiteServerAssets.GetIconUrl("tree/" + psLogo);

            for (var i = 0; i < level; i++)
            {
                padding += "　";
            }
            if (level > 0)
            {
                padding += "└ ";
            }

            return $"{padding}<img align='absbottom' border='0' src='{psLogo}'/>&nbsp;<a href='{publishmentSystemInfo.Additional.WebUrl}' target='_blank'>{publishmentSystemInfo.PublishmentSystemName}</a>";
		}
	}
}
