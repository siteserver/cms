using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageNodeGroup : BasePageCms
    {
        public Repeater RptContents;
        public Button BtnAddGroup;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageNodeGroup), null);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (AuthRequest.IsQueryExists("Delete"))
			{
                var groupName = AuthRequest.GetQueryString("GroupName");
			
				try
				{
                    DataProvider.ChannelGroupDao.Delete(SiteId, groupName);

                    AuthRequest.AddSiteLog(SiteId, "删除栏目组", $"栏目组:{groupName}");
				}
				catch(Exception ex)
				{
                    FailDeleteMessage(ex);
				}
			}
            if (AuthRequest.IsQueryExists("SetTaxis"))
            {
                var groupName = AuthRequest.GetQueryString("GroupName");
                var direction = AuthRequest.GetQueryString("Direction");

                switch (direction.ToUpper())
                {
                    case "UP":
                        DataProvider.ChannelGroupDao.UpdateTaxisToUp(SiteId, groupName);
                        break;
                    case "DOWN":
                        DataProvider.ChannelGroupDao.UpdateTaxisToDown(SiteId, groupName);
                        break;
                }
                AddWaitAndRedirectScript(GetRedirectUrl(SiteId));
            }

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);    

            RptContents.DataSource = ChannelGroupManager.GetChannelGroupInfoList(SiteId);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            BtnAddGroup.Attributes.Add("onclick", ModalNodeGroupAdd.GetOpenWindowString(SiteId));
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var groupInfo = (ChannelGroupInfo)e.Item.DataItem;

            var ltlNodeGroupName = (Literal)e.Item.FindControl("ltlNodeGroupName");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
            var hlUp = (HyperLink)e.Item.FindControl("hlUp");
            var hlDown = (HyperLink)e.Item.FindControl("hlDown");
            var ltlChannels = (Literal)e.Item.FindControl("ltlChannels");
            var ltlEdit = (Literal)e.Item.FindControl("ltlEdit");
            var ltlDelete = (Literal)e.Item.FindControl("ltlDelete");

            ltlNodeGroupName.Text = groupInfo.GroupName;
            ltlDescription.Text = groupInfo.Description;

            hlUp.NavigateUrl = PageUtils.GetCmsUrl(SiteId, nameof(PageNodeGroup), new NameValueCollection
            {
                {"GroupName", groupInfo.GroupName},
                {"SetTaxis", true.ToString()},
                {"Direction", "UP"}
            });
            hlDown.NavigateUrl = PageUtils.GetCmsUrl(SiteId, nameof(PageNodeGroup), new NameValueCollection
            {
                {"GroupName", groupInfo.GroupName},
                {"SetTaxis", true.ToString()},
                {"Direction", "DOWN"}
            });

            ltlChannels.Text =
                $@"<a href=""{PageChannelsGroup.GetRedirectUrl(SiteId, groupInfo.GroupName)}"">查看栏目</a>";

            ltlEdit.Text =
                $@"<a href=""javascript:;"" onClick=""{ModalNodeGroupAdd.GetOpenWindowString(SiteId,
                    groupInfo.GroupName)}"">修改</a>";

            ltlDelete.Text = $@"<a href=""{PageUtils.GetCmsUrl(SiteId, nameof(PageNodeGroup), new NameValueCollection
            {
                {"GroupName", groupInfo.GroupName},
                {"Delete", true.ToString()}
            })}"" onClick=""javascript:return confirm('此操作将删除栏目组“{groupInfo.GroupName}”，确认吗？');"">删除</a>";
        }
	}
}
