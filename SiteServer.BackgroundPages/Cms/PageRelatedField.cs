using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageRelatedField : BasePageCms
    {
        public Repeater RptContents;
		public Button BtnAdd;
        public Button BtnImport;

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

                var relatedFieldName = DataProvider.RelatedFieldDao.GetRelatedFieldName(relatedFieldId);
                DataProvider.RelatedFieldDao.Delete(relatedFieldId);
                Body.AddSiteLog(PublishmentSystemId, "删除联动字段", $"联动字段:{relatedFieldName}");
                SuccessDeleteMessage();
            }

            if (IsPostBack) return;

            VerifySitePermissions(AppManager.Permissions.WebSite.Configration);

            RptContents.DataSource = DataProvider.RelatedFieldDao.GetRelatedFieldInfoList(PublishmentSystemId);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            BtnAdd.Attributes.Add("onclick", ModalRelatedFieldAdd.GetOpenWindowString(PublishmentSystemId));
            BtnImport.Attributes.Add("onclick", ModalImport.GetOpenWindowString(PublishmentSystemId, ModalImport.TypeRelatedField));
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var relatedFieldInfo = (RelatedFieldInfo) e.Item.DataItem;

            var ltlRelatedFieldName = (Literal)e.Item.FindControl("ltlRelatedFieldName");
            var ltlTotalLevel = (Literal)e.Item.FindControl("ltlTotalLevel");
            var ltlItemsUrl = (Literal)e.Item.FindControl("ltlItemsUrl");
            var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");
            var ltlExportUrl = (Literal)e.Item.FindControl("ltlExportUrl");
            var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

            ltlRelatedFieldName.Text = relatedFieldInfo.RelatedFieldName;
            ltlTotalLevel.Text = relatedFieldInfo.TotalLevel.ToString();
            var urlItems = PageRelatedFieldMain.GetRedirectUrl(PublishmentSystemId, relatedFieldInfo.RelatedFieldId, relatedFieldInfo.TotalLevel);
            ltlItemsUrl.Text = $@"<a href=""{urlItems}"">管理字段项</a>";

            ltlEditUrl.Text =
                $@"<a href=""javascript:;"" onclick=""{ModalRelatedFieldAdd.GetOpenWindowString(
                    PublishmentSystemId, relatedFieldInfo.RelatedFieldId)}"">编辑</a>";
            ltlExportUrl.Text =
                $@"<a href=""javascript:;"" onclick=""{ModalExportMessage.GetOpenWindowStringToRelatedField(PublishmentSystemId, relatedFieldInfo.RelatedFieldId)}"">导出</a>";
            ltlDeleteUrl.Text =
                $@"<a href=""javascript:;"" onclick=""{PageUtils.GetRedirectStringWithConfirm(
                    PageUtils.GetCmsUrl(nameof(PageRelatedField), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"RelatedFieldID", relatedFieldInfo.RelatedFieldId.ToString()},
                        {"Delete", true.ToString()}
                    }), "确认删除此联动字段吗？")}"">删除</a>";
        }
	}
}
