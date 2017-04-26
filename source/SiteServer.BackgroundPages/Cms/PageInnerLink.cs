using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageInnerLink : BasePageCms
    {
		public DataGrid dgContents;
		public Button AddInnerLink;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (Body.IsQueryExists("Delete"))
			{
                var innerLinkName = Body.GetQueryString("InnerLinkName");
			
				try
				{
                    DataProvider.InnerLinkDao.Delete(innerLinkName, PublishmentSystemId);
                    Body.AddSiteLog(PublishmentSystemId, "删除站内链接", $"站内链接:{innerLinkName}");
					SuccessDeleteMessage();
				}
				catch(Exception ex)
				{
					FailDeleteMessage(ex);
				}
			}
			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdInnerLink, "站内链接管理", AppManager.Cms.Permission.WebSite.InnerLink);

                dgContents.DataSource = DataProvider.InnerLinkDao.GetDataSource(PublishmentSystemId);
                dgContents.DataBind();

				AddInnerLink.Attributes.Add("onclick", ModalInnerLinkAdd.GetOpenWindowStringToAdd(PublishmentSystemId));
			}
		}

		public string GetEditHtml(string innerLinkName)
		{
			var retval = string.Empty;
			var canEdit = false;
            if (PublishmentSystemId != 0)
			{
                if (DataProvider.InnerLinkDao.IsExactExists(innerLinkName, PublishmentSystemId))
				{
					canEdit = true;
				}
			}
			else
			{
				canEdit = true;
			}
			if (canEdit)
			{
				retval =
				    $@"<a href=""javascript:;"" onclick=""{ModalInnerLinkAdd.GetOpenWindowStringToEdit(PublishmentSystemId,
				        innerLinkName)}"">修改</a>";
			}
			return retval;
		}

        public string GetLinkUrl(string linkUrl)
        {
            return PageUtils.AddProtocolToUrl(PageUtils.ParseNavigationUrl(linkUrl));
        }

        public string GetDeleteHtml(string innerLinkName)
		{
			var retval = string.Empty;
			var canEdit = false;
            if (PublishmentSystemId != 0)
			{
                if (DataProvider.InnerLinkDao.IsExactExists(innerLinkName, PublishmentSystemId))
				{
					canEdit = true;
				}
			}
			else
			{
				canEdit = true;
			}
			if (canEdit)
			{
			    var urlInnerLink = PageUtils.GetCmsUrl(nameof(PageInnerLink), new NameValueCollection
			    {
			        {"PublishmentSystemID", PublishmentSystemId.ToString()},
			        {"InnerLinkName", innerLinkName},
			        {"Delete", true.ToString()}
			    });
                retval =
                    $"<a href=\"{urlInnerLink}\" onClick=\"javascript:return confirm('此操作将删除内部链接“{innerLinkName}”，确认吗？');\">删除</a>";
			}
			return retval;
		}
	}
}
