using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Sys
{
	public class PagePublishmentSystemUrl : BasePageCms
    {
		public DataGrid dgContents;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
                BreadCrumbSys(AppManager.Sys.LeftMenu.Site, "访问地址管理", AppManager.Sys.Permission.SysSite);

                var publishmentSystemArrayList = PublishmentSystemManager.GetPublishmentSystemIdListOrderByLevel();
                //去除用户中心
                //publishmentSystemArrayList = PublishmentSystemManager.RemoveUserCenter(publishmentSystemArrayList);
                dgContents.DataSource = publishmentSystemArrayList;
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();
			}
		}

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var publishmentSystemID = (int)e.Item.DataItem;
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemID);
                var publishmentSystemUrl = PageUtility.GetPublishmentSystemUrl(publishmentSystemInfo, string.Empty);

                var ltlName = e.Item.FindControl("ltlName") as Literal;
                var ltlDir = e.Item.FindControl("ltlDir") as Literal;
                var ltlPublishmentSystemUrl = e.Item.FindControl("ltlPublishmentSystemUrl") as Literal;
                var ltlIsMultiDeployment = e.Item.FindControl("ltlIsMultiDeployment") as Literal;
                var ltlOuterUrl = e.Item.FindControl("ltlOuterUrl") as Literal;
                var ltlInnerUrl = e.Item.FindControl("ltlInnerUrl") as Literal;
                var ltlAPIUrl = e.Item.FindControl("ltlAPIUrl") as Literal;
                var ltlHomeUrl = e.Item.FindControl("ltlHomeUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlName.Text = GetPublishmentSystemName(publishmentSystemInfo);
                ltlDir.Text = publishmentSystemInfo.PublishmentSystemDir;
                ltlPublishmentSystemUrl.Text =
                    $@"<a href=""{publishmentSystemUrl}"" target=""_blank"">{publishmentSystemUrl}</a>";

                ltlIsMultiDeployment.Text = publishmentSystemInfo.Additional.IsMultiDeployment ? "内外网分离部署" : "默认部署";
                if (publishmentSystemInfo.Additional.IsMultiDeployment)
                {
                    ltlOuterUrl.Text =
                        $@"<a href=""{publishmentSystemInfo.Additional.OuterUrl}"" target=""_blank"">{publishmentSystemInfo
                            .Additional.OuterUrl}</a>";
                    ltlInnerUrl.Text =
                        $@"<a href=""{publishmentSystemInfo.Additional.InnerUrl}"" target=""_blank"">{publishmentSystemInfo
                            .Additional.InnerUrl}</a>";
                }
                else
                {
                    ltlOuterUrl.Text = ltlPublishmentSystemUrl.Text;
                }

                ltlAPIUrl.Text = publishmentSystemInfo.Additional.ApiUrl;
                ltlHomeUrl.Text = publishmentSystemInfo.Additional.HomeUrl;

                ltlEditUrl.Text =
                    $"<a href=\"javascript:;\" onClick=\"{ModalChangePublishmentSystemUrl.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId)}\">修改</a>";
            }
        }

        private string GetPublishmentSystemName(PublishmentSystemInfo publishmentSystemInfo)
		{
            var retval = string.Empty;
            var padding = string.Empty;

            var level = PublishmentSystemManager.GetPublishmentSystemLevel(publishmentSystemInfo.PublishmentSystemId);
            var psLogo = string.Empty;
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

            retval =
                $"<img align='absbottom' border='0' src='{psLogo}'/>&nbsp;<a href='{publishmentSystemInfo.PublishmentSystemUrl}' target='_blank'>{publishmentSystemInfo.PublishmentSystemName}</a>";

            return
                $"{padding}{retval}&nbsp;{EPublishmentSystemTypeUtils.GetIconHtml(publishmentSystemInfo.PublishmentSystemType)}";
		}
	}
}
