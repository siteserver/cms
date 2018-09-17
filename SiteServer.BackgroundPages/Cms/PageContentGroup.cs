using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageContentGroup : BasePageCms
    {
		public Repeater RptContents;
		public Button BtnAddGroup;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageContentGroup), null);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (AuthRequest.IsQueryExists("Delete"))
			{
                var groupName = AuthRequest.GetQueryString("GroupName");
			
				try
				{
					DataProvider.ContentGroupDao.Delete(groupName, SiteId);
                    AuthRequest.AddSiteLog(SiteId, "删除内容组", $"内容组:{groupName}");
					SuccessDeleteMessage();
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
                        DataProvider.ContentGroupDao.UpdateTaxisToUp(SiteId, groupName);
                        break;
                    case "DOWN":
                        DataProvider.ContentGroupDao.UpdateTaxisToDown(SiteId, groupName);
                        break;
                }
                SuccessMessage("排序成功！");
                AddWaitAndRedirectScript(GetRedirectUrl(SiteId));
            }

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);

            RptContents.DataSource = ContentGroupManager.GetContentGroupInfoList(SiteId);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            var showPopWinString = ModalContentGroupAdd.GetOpenWindowString(SiteId);
            BtnAddGroup.Attributes.Add("onclick", showPopWinString);
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var groupInfo = (ContentGroupInfo) e.Item.DataItem;

            var ltlContentGroupName = (Literal)e.Item.FindControl("ltlContentGroupName");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
            var hlUp = (HyperLink)e.Item.FindControl("hlUp");
            var hlDown = (HyperLink)e.Item.FindControl("hlDown");
            var ltlContents = (Literal)e.Item.FindControl("ltlContents");
            var ltlEdit = (Literal)e.Item.FindControl("ltlEdit");
            var ltlDelete = (Literal)e.Item.FindControl("ltlDelete");

            ltlContentGroupName.Text = groupInfo.GroupName;
            ltlDescription.Text = groupInfo.Description;

            hlUp.NavigateUrl = PageUtils.GetCmsUrl(SiteId, nameof(PageContentGroup), new NameValueCollection
            {
                {"GroupName", groupInfo.GroupName},
                {"SetTaxis", true.ToString()},
                {"Direction", "UP"}
            });
            hlDown.NavigateUrl = PageUtils.GetCmsUrl(SiteId, nameof(PageContentGroup), new NameValueCollection
            {
                {"GroupName", groupInfo.GroupName},
                {"SetTaxis", true.ToString()},
                {"Direction", "DOWN"}
            });

            ltlContents.Text = $@"<a href=""{PageContentsGroup.GetRedirectUrl(SiteId, groupInfo.GroupName)}"">查看内容</a>";

            ltlEdit.Text =
                $@"<a href=""javascript:;"" onClick=""{ModalContentGroupAdd.GetOpenWindowString(SiteId, groupInfo.GroupName)}"">修改</a>";

            ltlDelete.Text = $@"<a href=""{PageUtils.GetCmsUrl(SiteId, nameof(PageContentGroup), new NameValueCollection
            {
                {"GroupName", groupInfo.GroupName},
                {"Delete", true.ToString()}
            })}"" onClick=""javascript:return confirm('此操作将删除内容组“{groupInfo.GroupName}”，确认吗？');"">删除</a>";
        }
	}
}
