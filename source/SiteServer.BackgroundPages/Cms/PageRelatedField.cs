using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageRelatedField : BasePageCms
    {
        public DataGrid dgContents;
		public Button AddButton;
        public Button ImportButton;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageRelatedField), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (Body.IsQueryExists("Delete"))
			{
                var relatedFieldId = Body.GetQueryInt("RelatedFieldID");
			
				try
				{
                    var relatedFieldName = DataProvider.RelatedFieldDao.GetRelatedFieldName(relatedFieldId);
                    DataProvider.RelatedFieldDao.Delete(relatedFieldId);
                    Body.AddSiteLog(PublishmentSystemId, "删除联动字段", $"联动字段:{relatedFieldName}");
					SuccessDeleteMessage();
				}
				catch(Exception ex)
				{
					FailDeleteMessage(ex);
				}
			}
			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, AppManager.Cms.LeftMenu.Configuration.IdConfigurationContentModel, "联动字段管理", AppManager.Cms.Permission.WebSite.Configration);

                dgContents.DataSource = DataProvider.RelatedFieldDao.GetDataSource(PublishmentSystemId);
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();

                var showPopWinString = ModalRelatedFieldAdd.GetOpenWindowString(PublishmentSystemId);
				AddButton.Attributes.Add("onclick", showPopWinString);
                ImportButton.Attributes.Add("onclick", ModalImport.GetOpenWindowString(PublishmentSystemId, ModalImport.TypeRelatedField));
			}
		}

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var relatedFieldID = SqlUtils.EvalInt(e.Item.DataItem, "RelatedFieldID");
                var relatedFieldName = SqlUtils.EvalString(e.Item.DataItem, "RelatedFieldName");
                var totalLevel = SqlUtils.EvalInt(e.Item.DataItem, "TotalLevel");

                var ltlRelatedFieldName = (Literal)e.Item.FindControl("ltlRelatedFieldName");
                var ltlTotalLevel = (Literal)e.Item.FindControl("ltlTotalLevel");
                var ltlItemsUrl = (Literal)e.Item.FindControl("ltlItemsUrl");
                var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");
                var ltlExportUrl = (Literal)e.Item.FindControl("ltlExportUrl");
                var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

                ltlRelatedFieldName.Text = relatedFieldName;
                ltlTotalLevel.Text = totalLevel.ToString();
                var urlItems = PageRelatedFieldMain.GetRedirectUrl(PublishmentSystemId, relatedFieldID, totalLevel);
                ltlItemsUrl.Text = $@"<a href=""{urlItems}"">管理字段项</a>";

                ltlEditUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalRelatedFieldAdd.GetOpenWindowString(
                        PublishmentSystemId, relatedFieldID)}"">编辑</a>";
                ltlExportUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalExportMessage.GetOpenWindowStringToRelatedField(PublishmentSystemId, relatedFieldID)}"">导出</a>";
                ltlDeleteUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{PageUtils.GetRedirectStringWithConfirm(
                        PageUtils.GetCmsUrl(nameof(PageRelatedField), new NameValueCollection
                        {
                            {"PublishmentSystemID", PublishmentSystemId.ToString()},
                            {"RelatedFieldID", relatedFieldID.ToString()},
                            {"Delete", true.ToString()}
                        }), "确认删除此联动字段吗？")}"">删除</a>";
            }
        }
	}
}
