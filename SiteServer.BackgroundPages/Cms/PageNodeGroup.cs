using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageNodeGroup : BasePageCms
    {
        public Repeater RptContents;
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

            if (IsPostBack) return;

            VerifySitePermissions(AppManager.Permissions.WebSite.Configration);    

            RptContents.DataSource = DataProvider.NodeGroupDao.GetNodeGroupInfoList(PublishmentSystemId);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            BtnAddGroup.Attributes.Add("onclick", ModalNodeGroupAdd.GetOpenWindowString(PublishmentSystemId));
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var groupInfo = (NodeGroupInfo)e.Item.DataItem;

            var ltlNodeGroupName = (Literal)e.Item.FindControl("ltlNodeGroupName");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
            var hlUp = (HyperLink)e.Item.FindControl("hlUp");
            var hlDown = (HyperLink)e.Item.FindControl("hlDown");
            var ltlChannels = (Literal)e.Item.FindControl("ltlChannels");
            var ltlEdit = (Literal)e.Item.FindControl("ltlEdit");
            var ltlDelete = (Literal)e.Item.FindControl("ltlDelete");

            ltlNodeGroupName.Text = groupInfo.NodeGroupName;
            ltlDescription.Text = groupInfo.Description;

            hlUp.NavigateUrl = PageUtils.GetCmsUrl(nameof(PageNodeGroup), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"GroupName", groupInfo.NodeGroupName},
                {"SetTaxis", true.ToString()},
                {"Direction", "UP"}
            });
            hlDown.NavigateUrl = PageUtils.GetCmsUrl(nameof(PageNodeGroup), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"GroupName", groupInfo.NodeGroupName},
                {"SetTaxis", true.ToString()},
                {"Direction", "DOWN"}
            });

            ltlChannels.Text =
                $@"<a href=""{PageChannelsGroup.GetRedirectUrl(PublishmentSystemId, groupInfo.NodeGroupName)}"">查看栏目</a>";

            ltlEdit.Text =
                $@"<a href=""javascript:;"" onClick=""{ModalNodeGroupAdd.GetOpenWindowString(PublishmentSystemId,
                    groupInfo.NodeGroupName)}"">修改</a>";

            ltlDelete.Text = $@"<a href=""{PageUtils.GetCmsUrl(nameof(PageNodeGroup), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"GroupName", groupInfo.NodeGroupName},
                {"Delete", true.ToString()}
            })}"" onClick=""javascript:return confirm('此操作将删除栏目组“{groupInfo.NodeGroupName}”，确认吗？');"">删除</a>";
        }
	}
}
