using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Sys;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageContentModel : BasePageCms
    {
        public DataGrid dgContents;
        public Button AddContentModel;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageContentModel), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Body.IsQueryExists("Delete"))
            {
                var modelId = Body.GetQueryString("ModelID");
                try
                {
                    BaiRongDataProvider.ContentModelDao.Delete(modelId, PublishmentSystemId);
                    Body.AddSiteLog(PublishmentSystemId, "删除内容模型", $"内容模型:{modelId}");
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

			if(!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, AppManager.Cms.LeftMenu.Configuration.IdConfigurationContentModel, "内容模型管理", AppManager.Cms.Permission.WebSite.Configration);

                dgContents.DataSource = ContentModelManager.GetContentModelInfoList(PublishmentSystemInfo);
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();

                AddContentModel.Attributes.Add("onclick", ModalContentModelAdd.GetOpenWindowStringToAdd(PublishmentSystemId));
			}
		}

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var modelInfo = e.Item.DataItem as ContentModelInfo;

                var ltlItemIcon = (Literal)e.Item.FindControl("ltlItemIcon");
                var ltlModelID = (Literal)e.Item.FindControl("ltlModelID");
                var ltlModelName = (Literal)e.Item.FindControl("ltlModelName");
                var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
                var ltlTableName = (Literal)e.Item.FindControl("ltlTableName");
                var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");
                var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

                if (!string.IsNullOrEmpty(modelInfo.IconUrl))
                {
                    ltlItemIcon.Text = $"<img src='{SiteServerAssets.GetIconUrl($"tree/{modelInfo.IconUrl}")}' />";
                }
                ltlModelID.Text = modelInfo.ModelId;
                ltlModelName.Text = modelInfo.ModelName;
                ltlTableName.Text = modelInfo.TableName;
                ltlDescription.Text = modelInfo.Description;

                if (!modelInfo.IsSystem)
                {
                    ltlEditUrl.Text =
                        $@"<a href=""javascript:;"" onclick=""{ModalContentModelAdd.GetOpenWindowStringToEdit(
                            PublishmentSystemId, modelInfo.ModelId)}"">编辑</a>";

                    var urlDelete = PageUtils.GetCmsUrl(nameof(PageContentModel), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"ModelID", modelInfo.ModelId},
                        {"Delete", true.ToString()}
                    });
                    ltlDeleteUrl.Text =
                        $@"<a href=""{urlDelete}"" onClick=""javascript:return confirm('此操作将删除内容模型“{modelInfo.ModelName}”，确认吗？');"">删除</a>";
                }
            }
        }
	}
}
