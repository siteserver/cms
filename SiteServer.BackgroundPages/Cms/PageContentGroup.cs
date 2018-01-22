using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageContentGroup : BasePageCms
    {
		public Repeater RptContents;
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

            if (IsPostBack) return;

            VerifySitePermissions(AppManager.Permissions.WebSite.Configration);

            RptContents.DataSource = DataProvider.ContentGroupDao.GetContentGroupInfoList(PublishmentSystemId);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            var showPopWinString = ModalContentGroupAdd.GetOpenWindowString(PublishmentSystemId);
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

            ltlContentGroupName.Text = groupInfo.ContentGroupName;
            ltlDescription.Text = groupInfo.Description;

            hlUp.NavigateUrl = PageUtils.GetCmsUrl(nameof(PageContentGroup), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"GroupName", groupInfo.ContentGroupName},
                {"SetTaxis", true.ToString()},
                {"Direction", "UP"}
            });
            hlDown.NavigateUrl = PageUtils.GetCmsUrl(nameof(PageContentGroup), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"GroupName", groupInfo.ContentGroupName},
                {"SetTaxis", true.ToString()},
                {"Direction", "DOWN"}
            });

            ltlContents.Text = $@"<a href=""{PageContentsGroup.GetRedirectUrl(PublishmentSystemId, groupInfo.ContentGroupName)}"">查看内容</a>";

            ltlEdit.Text =
                $@"<a href=""javascript:;"" onClick=""{ModalContentGroupAdd.GetOpenWindowString(PublishmentSystemId, groupInfo.ContentGroupName)}"">修改</a>";

            ltlDelete.Text = $@"<a href=""{PageUtils.GetCmsUrl(nameof(PageContentGroup), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"GroupName", groupInfo.ContentGroupName},
                {"Delete", true.ToString()}
            })}"" onClick=""javascript:return confirm('此操作将删除内容组“{groupInfo.ContentGroupName}”，确认吗？');"">删除</a>";
        }
	}
}
