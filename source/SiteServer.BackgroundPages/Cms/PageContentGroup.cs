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
		public DataGrid dgContents;
		public Button AddGroup;

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
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, AppManager.Cms.LeftMenu.Configuration.IdConfigurationGroupAndTags, "内容组管理", AppManager.Cms.Permission.WebSite.Configration);

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
                        default:
                            break;
                    }
                    SuccessMessage("排序成功！");
                    AddWaitAndRedirectScript(GetRedirectUrl(PublishmentSystemId));
                }

                dgContents.DataSource = DataProvider.ContentGroupDao.GetDataSource(PublishmentSystemId);
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();

                var showPopWinString = ModalContentGroupAdd.GetOpenWindowString(PublishmentSystemId);
				AddGroup.Attributes.Add("onclick", showPopWinString);
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

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var groupName = SqlUtils.EvalString(e.Item.DataItem, "ContentGroupName");

                var upLinkButton = e.Item.FindControl("UpLinkButton") as HyperLink;
                var downLinkButton = e.Item.FindControl("DownLinkButton") as HyperLink;

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
}
