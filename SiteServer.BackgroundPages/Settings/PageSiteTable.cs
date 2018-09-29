using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteTable : BasePageCms
    {
		public Repeater RptContents;
        public Button BtnAdd;

        public static string GetRedirectUrl()
	    {
	        return PageUtils.GetSettingsUrl(nameof(PageSiteTable), null);
	    }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			//if (AuthRequest.IsQueryExists("Delete"))
			//{
   //             var enName = AuthRequest.GetQueryString("ENName");//内容表
   //             var enNameArchive = enName + "_Archive";//内容表归档
			
			//	try
			//	{
   //                 DataProvider.TableDao.DeleteCollectionTableInfoAndDbTable(enName);//删除内容表
   //                 DataProvider.TableDao.DeleteCollectionTableInfoAndDbTable(enNameArchive);//删除内容表归档

   //                 AuthRequest.AddAdminLog("删除内容表", $"内容表:{enName}");

			//		SuccessDeleteMessage();
			//	}
			//	catch(Exception ex)
			//	{
   //                 FailDeleteMessage(ex);
			//	}
			//}

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            RptContents.DataSource = SiteManager.GetSiteTableNames();
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            //BtnAdd.OnClientClick = ModalAuxiliaryTableAdd.GetOpenWindowString();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var tableName = (string)e.Item.DataItem;
            //var isHighlight = !collectionInfo.IsCreatedInDb || collectionInfo.IsChangedAfterCreatedInDb;
            var isTableUsed = DataProvider.SiteDao.IsTableUsed(tableName);

            //if (isHighlight) e.Item.Attributes.Add("style", "color: red");

            var ltlTableName = (Literal)e.Item.FindControl("ltlTableName");
            var ltlMetadataEdit = (Literal)e.Item.FindControl("ltlMetadataEdit");
            var ltlStyleEdit = (Literal)e.Item.FindControl("ltlStyleEdit");
            var ltlEdit = (Literal)e.Item.FindControl("ltlEdit");
            var ltlDelete = (Literal)e.Item.FindControl("ltlDelete");

            ltlTableName.Text = tableName;

            //ltlMetadataEdit.Text =
            //    $@"<a href=""{PageSiteTableMetadata.GetRedirectUrl(tableName)}"">管理真实字段</a>";

            ltlStyleEdit.Text = $@"<a href=""{PageSiteTableStyle.GetRedirectUrl(tableName)}"">管理虚拟字段</a>";

            //ltlEdit.Text = $@"<a href=""javascript:;"" onclick=""{ModalAuxiliaryTableAdd.GetOpenWindowString(tableName)}"">编辑</a>";

            if (!isTableUsed)
            {
                var script = AlertUtils.Warning("删除内容表", $"此操作将删除内容表“{tableName}”，如果内容表已在数据库中建立，将同时删除建立的内容表，确认吗？", "取 消",
                    "确认删除", $"location.href = '{GetRedirectUrl()}?Delete=True&ENName={tableName}';");
                ltlDelete.Text =
                $@"<a href=""javascript:;"" onClick=""{script}"">删除</a>";
            }
        }
	}
}
