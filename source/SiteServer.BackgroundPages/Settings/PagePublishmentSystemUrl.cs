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

            var publishmentSystemArrayList = PublishmentSystemManager.GetPublishmentSystemIdListOrderByLevel();
            //去除用户中心
            //publishmentSystemArrayList = PublishmentSystemManager.RemoveUserCenter(publishmentSystemArrayList);
            DgContents.DataSource = publishmentSystemArrayList;
            DgContents.ItemDataBound += dgContents_ItemDataBound;
            DgContents.DataBind();
        }

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var publishmentSystemId = (int)e.Item.DataItem;
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var publishmentSystemUrl = PageUtility.GetPublishmentSystemUrl(publishmentSystemInfo, string.Empty);

                var ltlName = (Literal)e.Item.FindControl("ltlName");
                var ltlDir = (Literal)e.Item.FindControl("ltlDir");
                var ltlPublishmentSystemUrl = (Literal)e.Item.FindControl("ltlPublishmentSystemUrl");
                var ltlIsMultiDeployment = (Literal)e.Item.FindControl("ltlIsMultiDeployment");
                var ltlSiteUrl = (Literal)e.Item.FindControl("ltlSiteUrl");
                var ltlApiUrl = (Literal)e.Item.FindControl("ltlApiUrl");
                var ltlHomeUrl = (Literal)e.Item.FindControl("ltlHomeUrl");
                var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");

                ltlName.Text = GetPublishmentSystemName(publishmentSystemInfo);
                ltlDir.Text = publishmentSystemInfo.PublishmentSystemDir;
                ltlPublishmentSystemUrl.Text =
                    $@"<a href=""{publishmentSystemUrl}"" target=""_blank"">{publishmentSystemUrl}</a>";

                ltlIsMultiDeployment.Text = publishmentSystemInfo.Additional.IsMultiDeployment ? "内外网分离部署" : "默认部署";
                if (publishmentSystemInfo.Additional.IsMultiDeployment)
                {
                    ltlSiteUrl.Text =
                        $@"外部站点地址：<a href=""{publishmentSystemInfo.Additional.OuterSiteUrl}"" target=""_blank"">{publishmentSystemInfo
                            .Additional.OuterSiteUrl}</a><br />内部站点地址：<a href=""{publishmentSystemInfo.Additional.InnerSiteUrl}"" target=""_blank"">{publishmentSystemInfo
                            .Additional.InnerSiteUrl}</a>";
                    ltlApiUrl.Text = $@"外部API地址：{publishmentSystemInfo.Additional.OuterApiUrl}<br />内部API地址：{publishmentSystemInfo.Additional.InnerApiUrl}";
                }
                else
                {
                    ltlSiteUrl.Text = $@"<a href=""{publishmentSystemInfo.Additional.SiteUrl}"" target=""_blank"">{publishmentSystemInfo
                            .Additional.OuterSiteUrl}</a>";
                    ltlApiUrl.Text = publishmentSystemInfo.Additional.ApiUrl;
                }

                
                ltlHomeUrl.Text = publishmentSystemInfo.Additional.HomeUrl;

                ltlEditUrl.Text =
                    $"<a href=\"javascript:;\" onClick=\"{ModalChangePublishmentSystemUrl.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId)}\">修改</a>";
            }
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

            return $"{padding}<img align='absbottom' border='0' src='{psLogo}'/>&nbsp;<a href='{publishmentSystemInfo.PublishmentSystemUrl}' target='_blank'>{publishmentSystemInfo.PublishmentSystemName}</a>";
		}
	}
}
