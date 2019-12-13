using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Repositories;

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
					DataProvider.ContentGroupRepository.DeleteAsync(SiteId, groupName).GetAwaiter().GetResult();
                    AuthRequest.AddSiteLogAsync(SiteId, "删除内容组", $"内容组:{groupName}").GetAwaiter().GetResult();
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
                        DataProvider.ContentGroupRepository.UpdateTaxisToUpAsync(SiteId, groupName).GetAwaiter().GetResult();
                        break;
                    case "DOWN":
                        DataProvider.ContentGroupRepository.UpdateTaxisToDownAsync(SiteId, groupName).GetAwaiter().GetResult();
                        break;
                }
                SuccessMessage("排序成功！");
                AddWaitAndRedirectScript(GetRedirectUrl(SiteId));
            }

            if (IsPostBack) return;

            VerifySitePermissions(Constants.SitePermissions.ConfigGroups);

            RptContents.DataSource = DataProvider.ContentGroupRepository.GetContentGroupsAsync(SiteId).GetAwaiter().GetResult();
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            var showPopWinString = ModalContentGroupAdd.GetOpenWindowString(SiteId);
            BtnAddGroup.Attributes.Add("onClick", showPopWinString);
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var groupInfo = (ContentGroup) e.Item.DataItem;

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
