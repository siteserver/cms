using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageNodeGroup : BasePageCms
    {
		public DataGrid DgContents;
		public Button BtnAddGroup;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageNodeGroup), new NameValueCollection
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
                    DataProvider.NodeGroupDao.Delete(PublishmentSystemId, groupName);

                    Body.AddSiteLog(PublishmentSystemId, "删除栏目组", $"栏目组:{groupName}");
				}
				catch(Exception ex)
				{
                    FailDeleteMessage(ex);
				}
			}
			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, AppManager.Cms.LeftMenu.Configuration.IdConfigurationGroupAndTags, "栏目组管理", AppManager.Cms.Permission.WebSite.Configration);

                if (Body.IsQueryExists("SetTaxis"))
                {
                    var groupName = Body.GetQueryString("GroupName");
                    var direction = Body.GetQueryString("Direction");

                    switch (direction.ToUpper())
                    {
                        case "UP":
                            DataProvider.NodeGroupDao.UpdateTaxisToUp(PublishmentSystemId, groupName);
                            break;
                        case "DOWN":
                            DataProvider.NodeGroupDao.UpdateTaxisToDown(PublishmentSystemId, groupName);
                            break;
                    }
                    AddWaitAndRedirectScript(GetRedirectUrl(PublishmentSystemId));
                }

                DgContents.DataSource = DataProvider.NodeGroupDao.GetDataSource(PublishmentSystemId);
                DgContents.ItemDataBound += DgContents_ItemDataBound;
                DgContents.DataBind();

                var showPopWinString = ModalNodeGroupAdd.GetOpenWindowString(PublishmentSystemId);
                BtnAddGroup.Attributes.Add("onclick", showPopWinString);
			}
		}

        public string GetChannelHtml(string groupName)
        {
            var publishmentSystemId = PublishmentSystemId;
            return $"<a href=\"{PageChannelsGroup.GetRedirectUrl(publishmentSystemId, groupName)}\">查看栏目</a>";
        }

		public string GetEditHtml(string groupName)
		{
            var showPopWinString = ModalNodeGroupAdd.GetOpenWindowString(PublishmentSystemId, groupName);            
            return $"<a href=\"javascript:;\" onClick=\"{showPopWinString}\">修改</a>";
		}

		public string GetDeleteHtml(string groupName)
		{
            var urlDelete = PageUtils.GetCmsUrl(nameof(PageNodeGroup), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"GroupName", groupName},
                {"Delete", true.ToString()}
            });
			return $"<a href=\"{urlDelete}\" onClick=\"javascript:return confirm('此操作将删除栏目组“{groupName}”，确认吗？');\">删除</a>";
		}

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var groupName = SqlUtils.EvalString(e.Item.DataItem, "NodeGroupName");

                var upLinkButton = (HyperLink)e.Item.FindControl("UpLinkButton");
                var downLinkButton = (HyperLink)e.Item.FindControl("DownLinkButton");

                upLinkButton.NavigateUrl = PageUtils.GetCmsUrl(nameof(PageNodeGroup), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"GroupName", groupName},
                    {"SetTaxis", true.ToString()},
                    {"Direction", "UP"}
                });
                downLinkButton.NavigateUrl = PageUtils.GetCmsUrl(nameof(PageNodeGroup), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"GroupName", groupName},
                    {"SetTaxis", true.ToString()},
                    {"Direction", "DOWN"}
                });
            }
        }
	}
}
