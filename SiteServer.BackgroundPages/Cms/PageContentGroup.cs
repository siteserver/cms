using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageContentGroup : BasePageCms
    {
		public DataGrid DgContents;
		public Button BtnAddGroup;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageContentGroup), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (Body.IsQueryExists("Delete"))
			{
                var groupName = Body.GetQueryString("GroupName");
			
				try
				{
					DataProvider.ContentGroupDao.Delete(groupName, PublishmentSystemId);
                    Body.AddSiteLog(PublishmentSystemId, "删除内容组", $"内容组:{groupName}");
					SuccessDeleteMessage();
				}
				catch(Exception ex)
				{
                    FailDeleteMessage(ex);
				}
			}
			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, "内容组管理", AppManager.Permissions.WebSite.Configration);

                if (Body.IsQueryExists("SetTaxis"))
                {
                    var groupName = Body.GetQueryString("GroupName");
                    var direction = Body.GetQueryString("Direction");

                    switch (direction.ToUpper())
                    {
                        case "UP":
                            DataProvider.ContentGroupDao.UpdateTaxisToUp(PublishmentSystemId, groupName);
                            break;
                        case "DOWN":
                            DataProvider.ContentGroupDao.UpdateTaxisToDown(PublishmentSystemId, groupName);
                            break;
                    }
                    SuccessMessage("排序成功！");
                    AddWaitAndRedirectScript(GetRedirectUrl(PublishmentSystemId));
                }

                DgContents.DataSource = DataProvider.ContentGroupDao.GetDataSource(PublishmentSystemId);
                DgContents.ItemDataBound += DgContents_ItemDataBound;
                DgContents.DataBind();

                var showPopWinString = ModalContentGroupAdd.GetOpenWindowString(PublishmentSystemId);
                BtnAddGroup.Attributes.Add("onclick", showPopWinString);
			}
		}

        public string GetContentsHtml(string groupName)
        {
            var urlGroup = PageUtils.GetCmsUrl(nameof(PageContentGroup), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"contentGroupName", groupName}
            });
            return $"<a href=\"{urlGroup}\">查看内容</a>";
        }

		public string GetEditHtml(string groupName)
		{
            var showPopWinString = ModalContentGroupAdd.GetOpenWindowString(PublishmentSystemId, groupName);
            return $"<a href=\"javascript:;\" onClick=\"{showPopWinString}\">修改</a>";
		}

		public string GetDeleteHtml(string groupName)
		{
		    var urlGroup = PageUtils.GetCmsUrl(nameof(PageContentGroup), new NameValueCollection
		    {
		        {"PublishmentSystemID", PublishmentSystemId.ToString()},
		        {"GroupName", groupName},
                {"Delete", true.ToString()}
            });
            return
                $"<a href=\"{urlGroup}\" onClick=\"javascript:return confirm('此操作将删除内容组“{groupName}”，确认吗？');\">删除</a>";
		}

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var groupName = SqlUtils.EvalString(e.Item.DataItem, "ContentGroupName");

            var upLinkButton = (HyperLink)e.Item.FindControl("UpLinkButton");
            var downLinkButton = (HyperLink)e.Item.FindControl("DownLinkButton");

            upLinkButton.NavigateUrl = PageUtils.GetCmsUrl(nameof(PageContentGroup), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"GroupName", groupName},
                {"SetTaxis", true.ToString()},
                {"Direction", "UP"}
            });
            downLinkButton.NavigateUrl = PageUtils.GetCmsUrl(nameof(PageContentGroup), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"GroupName", groupName},
                {"SetTaxis", true.ToString()},
                {"Direction", "DOWN"}
            });
        }
	}
}
