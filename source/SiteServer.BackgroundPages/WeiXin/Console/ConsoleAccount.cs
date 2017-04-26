using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;

namespace SiteServer.WeiXin.BackgroundPages
{
	public class ConsoleAccount : BackgroundBasePage
	{
		public DataGrid dgContents;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
                //base.BreadCrumbConsole(AppManager.CMS.LeftMenu.ID_Site, "系统站点管理", AppManager.Permission.Platform_Site);

                dgContents.DataSource = PublishmentSystemManager.GetPublishmentSystemIDArrayList(EPublishmentSystemType.Weixin);
                dgContents.ItemDataBound += new DataGridItemEventHandler(dgContents_ItemDataBound);
                dgContents.DataBind();
			}
		}

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var publishmentSystemID = (int)e.Item.DataItem;
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemID);
                if (publishmentSystemInfo != null)
                {
                    var ltlPublishmentSystemName = e.Item.FindControl("ltlPublishmentSystemName") as Literal;
                    var ltlPublishmentSystemType = e.Item.FindControl("ltlPublishmentSystemType") as Literal;
                    var ltlPublishmentSystemDir = e.Item.FindControl("ltlPublishmentSystemDir") as Literal;
                    var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                    var ltlManage = e.Item.FindControl("ltlManage") as Literal;
                    var ltlBinding = e.Item.FindControl("ltlBinding") as Literal;
                    var ltlDelete = e.Item.FindControl("ltlDelete") as Literal;

                    ltlPublishmentSystemName.Text = publishmentSystemInfo.PublishmentSystemName;

                    ltlPublishmentSystemType.Text = EPublishmentSystemTypeUtils.GetHtml(publishmentSystemInfo.PublishmentSystemType);
                    ltlPublishmentSystemDir.Text = publishmentSystemInfo.PublishmentSystemDir;
                    ltlAddDate.Text = DateUtils.GetDateString(NodeManager.GetAddDate(publishmentSystemID, publishmentSystemID));

                    var manageUrl = PageUtils.GetLoadingUrl(PageUtils.GetAdminDirectoryUrl(
                        $"main.aspx?publishmentSystemID={publishmentSystemID}"));
                    ltlManage.Text = $@"<a href=""{manageUrl}"" target=""top"">管理</a>";

                    var bindingUrl = ConsoleAccountBinding.GetRedirectUrl(publishmentSystemID, PageUtils.GetWXUrl("console_account.aspx"));

                    var accountInfo = WeiXinManager.GetAccountInfo(publishmentSystemID);

                    var isBinding = WeiXinManager.IsBinding(accountInfo);
                    if (isBinding)
                    {
                        ltlBinding.Text = $@"<a href=""{bindingUrl}"" class=""btn btn-success"">已绑定微信</a>";
                    }
                    else
                    {
                        ltlBinding.Text = $@"<a href=""{bindingUrl}"" class=""btn btn-danger"">未绑定微信</a>";
                    }

                    var urlDelete = PageUtils.GetSTLUrl(
                        $"console_publishmentSystemDelete.aspx?NodeID={publishmentSystemID}");
                    ltlDelete.Text = $@"<a href=""{urlDelete}"">删除</a>";
                }
            }
        }
	}
}
