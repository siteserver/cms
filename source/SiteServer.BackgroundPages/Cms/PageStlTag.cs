using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageStlTag : BasePageCms
    {
		public DataGrid dgContents;
		public Button AddStlTag;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (Body.IsQueryExists("Delete"))
			{
				var tagName = Body.GetQueryString("TagName");
			
				try
				{
                    DataProvider.StlTagDao.Delete(PublishmentSystemId, tagName);
                    Body.AddSiteLog(PublishmentSystemId, "删除自定义模板语言 ", $"模板标签名:{tagName}");
					SuccessDeleteMessage();
				}
				catch(Exception ex)
				{
                    FailDeleteMessage(ex);
				}
			}
			if (!IsPostBack)
            {                
                if (PublishmentSystemId != 0)
                {
                    BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, "自定义模板语言", AppManager.Cms.Permission.WebSite.Template);
                }

                dgContents.DataSource = DataProvider.StlTagDao.GetDataSource(PublishmentSystemId);
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();

                var showPopWinString = ModalStlTagAdd.GetOpenWindowString(PublishmentSystemId);
				AddStlTag.Attributes.Add("onclick", showPopWinString);
			}
		}

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var publishmentSystemID = SqlUtils.EvalInt(e.Item.DataItem, "PublishmentSystemID");
                var tagName = SqlUtils.EvalString(e.Item.DataItem, "TagName");

                var ltlEditHtml = (Literal)e.Item.FindControl("ltlEditHtml");
                var ltlDeleteHtml = (Literal)e.Item.FindControl("ltlDeleteHtml");

                if (publishmentSystemID == PublishmentSystemId)
                {
                    var showPopWinString = ModalStlTagAdd.GetOpenWindowString(PublishmentSystemId, tagName);
                    ltlEditHtml.Text = $"<a href=\"javascript:;\" onClick=\"{showPopWinString}\">修改</a>";
                }

                var canEdit = false;
                if (publishmentSystemID != 0)
                {
                    canEdit = true;
                }
                else
                {
                    if (publishmentSystemID == PublishmentSystemId)
                    {
                        canEdit = true;
                    }
                }
                if (canEdit)
                {
                    ltlDeleteHtml.Text =
                        $@"<a href=""{PageUtils.GetCmsUrl(nameof(PageStlTag), new NameValueCollection
                        {
                            {"PublishmentSystemID", PublishmentSystemId.ToString()},
                            {"TagName", tagName},
                            {"Delete", true.ToString()}
                        })}"" onClick=""javascript:return confirm('此操作将删除自定义标签“{tagName}”，确认吗？');"">删除</a>";
                }
            }
        }

	}
}
