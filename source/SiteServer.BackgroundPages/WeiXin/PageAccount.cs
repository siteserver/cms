using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Manager;

namespace SiteServer.BackgroundPages.WeiXin
{
	public class PageAccount : BasePageCms
    {
        public static string GetRedirectUrl()
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAccount), null);
        }

        public DataGrid DgContents;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
                //base.BreadCrumbConsole(AppManager.CMS.LeftMenu.ID_Site, "系统站点管理", AppManager.Permission.Platform_Site);

                DgContents.DataSource = PublishmentSystemManager.GetPublishmentSystemIdList();
                DgContents.ItemDataBound += dgContents_ItemDataBound;
                DgContents.DataBind();
			}
		}

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var publishmentSystemId = (int)e.Item.DataItem;
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                if (publishmentSystemInfo != null)
                {
                    var ltlPublishmentSystemName = e.Item.FindControl("ltlPublishmentSystemName") as Literal;
                    var ltlPublishmentSystemDir = e.Item.FindControl("ltlPublishmentSystemDir") as Literal;
                    var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                    var ltlManage = e.Item.FindControl("ltlManage") as Literal;
                    var ltlBinding = e.Item.FindControl("ltlBinding") as Literal;
                    var ltlDelete = e.Item.FindControl("ltlDelete") as Literal;

                    ltlPublishmentSystemName.Text = publishmentSystemInfo.PublishmentSystemName;

                    ltlPublishmentSystemDir.Text = publishmentSystemInfo.PublishmentSystemDir;
                    ltlAddDate.Text = DateUtils.GetDateString(NodeManager.GetAddDate(publishmentSystemId, publishmentSystemId));

                    var manageUrl = PageMain.GetRedirectUrl(publishmentSystemId);
                    ltlManage.Text = $@"<a href=""{manageUrl}"" target=""top"">管理</a>";

                    var bindingUrl = PageAccountBinding.GetRedirectUrl(publishmentSystemId, GetRedirectUrl());

                    var accountInfo = WeiXinManager.GetAccountInfo(publishmentSystemId);

                    var isBinding = WeiXinManager.IsBinding(accountInfo);
                    if (isBinding)
                    {
                        ltlBinding.Text = $@"<a href=""{bindingUrl}"" class=""btn btn-success"">已绑定微信</a>";
                    }
                    else
                    {
                        ltlBinding.Text = $@"<a href=""{bindingUrl}"" class=""btn btn-danger"">未绑定微信</a>";
                    }

                    var urlDelete = PagePublishmentSystemDelete.GetRedirectUrl(publishmentSystemId);
                    ltlDelete.Text = $@"<a href=""{urlDelete}"">删除</a>";
                }
            }
        }
	}
}
