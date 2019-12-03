using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Repositories;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageRelatedField : BasePageCms
    {
        public Repeater RptContents;
		public Button BtnAdd;
        public Button BtnImport;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageRelatedField), null);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (AuthRequest.IsQueryExists("Delete"))
			{
                var relatedFieldId = AuthRequest.GetQueryInt("RelatedFieldID");

                var relatedFieldName = DataProvider.RelatedFieldRepository.GetTitleAsync(relatedFieldId).GetAwaiter().GetResult();
                DataProvider.RelatedFieldRepository.DeleteAsync(relatedFieldId).GetAwaiter().GetResult();
                AuthRequest.AddSiteLogAsync(SiteId, "删除联动字段", $"联动字段:{relatedFieldName}").GetAwaiter().GetResult();
                SuccessDeleteMessage();
            }

            if (IsPostBack) return;

            VerifySitePermissions(Constants.WebSitePermissions.Configuration);

            RptContents.DataSource = DataProvider.RelatedFieldRepository.GetRelatedFieldListAsync(SiteId).GetAwaiter().GetResult();
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            BtnAdd.Attributes.Add("onClick", ModalRelatedFieldAdd.GetOpenWindowString(SiteId));
            BtnImport.Attributes.Add("onClick", ModalImport.GetOpenWindowString(SiteId, ModalImport.TypeRelatedField));
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var relatedFieldInfo = (RelatedField) e.Item.DataItem;

            var ltlRelatedFieldName = (Literal)e.Item.FindControl("ltlRelatedFieldName");
            var ltlTotalLevel = (Literal)e.Item.FindControl("ltlTotalLevel");
            var ltlItemsUrl = (Literal)e.Item.FindControl("ltlItemsUrl");
            var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");
            var ltlExportUrl = (Literal)e.Item.FindControl("ltlExportUrl");
            var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

            ltlRelatedFieldName.Text = relatedFieldInfo.Title;
            ltlTotalLevel.Text = relatedFieldInfo.TotalLevel.ToString();
            var urlItems = PageRelatedFieldMain.GetRedirectUrl(SiteId, relatedFieldInfo.Id, relatedFieldInfo.TotalLevel);
            ltlItemsUrl.Text = $@"<a href=""{urlItems}"">管理字段项</a>";

            ltlEditUrl.Text =
                $@"<a href=""javascript:;"" onClick=""{ModalRelatedFieldAdd.GetOpenWindowString(
                    SiteId, relatedFieldInfo.Id)}"">编辑</a>";
            ltlExportUrl.Text =
                $@"<a href=""javascript:;"" onClick=""{ModalExportMessage.GetOpenWindowStringToRelatedField(SiteId, relatedFieldInfo.Id)}"">导出</a>";
            ltlDeleteUrl.Text =
                $@"<a href=""javascript:;"" onClick=""{PageUtils.GetRedirectStringWithConfirm(
                    PageUtils.GetCmsUrl(SiteId, nameof(PageRelatedField), new NameValueCollection
                    {
                        {"RelatedFieldID", relatedFieldInfo.Id.ToString()},
                        {"Delete", true.ToString()}
                    }), "确认删除此联动字段吗？")}"">删除</a>";
        }
	}
}
