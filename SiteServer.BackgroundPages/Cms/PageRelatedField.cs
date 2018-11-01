using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

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

                var relatedFieldName = DataProvider.RelatedFieldDao.GetTitle(relatedFieldId);
                DataProvider.RelatedFieldDao.Delete(relatedFieldId);
                AuthRequest.AddSiteLog(SiteId, "删除联动字段", $"联动字段:{relatedFieldName}");
                SuccessDeleteMessage();
            }

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);

            RptContents.DataSource = DataProvider.RelatedFieldDao.GetRelatedFieldInfoList(SiteId);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            BtnAdd.Attributes.Add("onclick", ModalRelatedFieldAdd.GetOpenWindowString(SiteId));
            BtnImport.Attributes.Add("onclick", ModalImport.GetOpenWindowString(SiteId, ModalImport.TypeRelatedField));
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

            ltlRelatedFieldName.Text = relatedFieldInfo.Title;
            ltlTotalLevel.Text = relatedFieldInfo.TotalLevel.ToString();
            var urlItems = PageRelatedFieldMain.GetRedirectUrl(SiteId, relatedFieldInfo.Id, relatedFieldInfo.TotalLevel);
            ltlItemsUrl.Text = $@"<a href=""{urlItems}"">管理字段项</a>";

            ltlEditUrl.Text =
                $@"<a href=""javascript:;"" onclick=""{ModalRelatedFieldAdd.GetOpenWindowString(
                    SiteId, relatedFieldInfo.Id)}"">编辑</a>";
            ltlExportUrl.Text =
                $@"<a href=""javascript:;"" onclick=""{ModalExportMessage.GetOpenWindowStringToRelatedField(SiteId, relatedFieldInfo.Id)}"">导出</a>";
            ltlDeleteUrl.Text =
                $@"<a href=""javascript:;"" onclick=""{PageUtils.GetRedirectStringWithConfirm(
                    PageUtils.GetCmsUrl(SiteId, nameof(PageRelatedField), new NameValueCollection
                    {
                        {"RelatedFieldID", relatedFieldInfo.Id.ToString()},
                        {"Delete", true.ToString()}
                    }), "确认删除此联动字段吗？")}"">删除</a>";
        }
	}
}
