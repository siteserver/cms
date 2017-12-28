using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageAuxiliaryTable : BasePageCms
    {
		public DataGrid DgContents;
        public Button BtnAdd;

        public static string GetRedirectUrl()
	    {
	        return PageUtils.GetSettingsUrl(nameof(PageAuxiliaryTable), null);
	    }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (Body.IsQueryExists("Delete"))
			{
                var enName = Body.GetQueryString("ENName");//辅助表
                var enNameArchive = enName + "_Archive";//辅助表归档
			
				try
				{
                    BaiRongDataProvider.TableCollectionDao.DeleteCollectionTableInfoAndDbTable(enName);//删除辅助表
                    BaiRongDataProvider.TableCollectionDao.DeleteCollectionTableInfoAndDbTable(enNameArchive);//删除辅助表归档

                    Body.AddAdminLog("删除辅助表", $"辅助表:{enName}");

					SuccessDeleteMessage();
				}
				catch(Exception ex)
				{
                    FailDeleteMessage(ex);
				}
			}

            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.SiteManagement);

            DgContents.DataSource = BaiRongDataProvider.TableCollectionDao.GetTableCollectionInfoList();
            DgContents.ItemDataBound += DgContents_ItemDataBound;
            DgContents.DataBind();

            BtnAdd.OnClientClick = ModalAuxiliaryTableAdd.GetOpenWindowString();
        }

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var collectionInfo = (TableCollectionInfo)e.Item.DataItem;
            var tableName = collectionInfo.TableEnName;
            var isHighlight = !collectionInfo.IsCreatedInDb || collectionInfo.IsChangedAfterCreatedInDb;
            var isTableUsed = DataProvider.PublishmentSystemDao.IsTableUsed(tableName);

            if (isHighlight) e.Item.Attributes.Add("style", "color: red");

            var ltlTableName = (Literal)e.Item.FindControl("ltlTableName");
            var ltlTableCnName = (Literal)e.Item.FindControl("ltlTableCnName");
            var ltlIsUsed = (Literal)e.Item.FindControl("ltlIsUsed");
            var ltlIsCreatedInDb = (Literal)e.Item.FindControl("ltlIsCreatedInDB");
            var ltlIsChangedAfterCreatedInDb = (Literal)e.Item.FindControl("ltlIsChangedAfterCreatedInDb");
            var ltlMetadataEdit = (Literal)e.Item.FindControl("ltlMetadataEdit");
            var ltlStyleEdit = (Literal)e.Item.FindControl("ltlStyleEdit");
            var ltlEdit = (Literal)e.Item.FindControl("ltlEdit");
            var ltlDelete = (Literal)e.Item.FindControl("ltlDelete");

            ltlTableName.Text = tableName;
            ltlTableCnName.Text = collectionInfo.TableCnName;
            ltlIsUsed.Text = StringUtils.GetBoolText(isTableUsed);
            ltlIsCreatedInDb.Text = StringUtils.GetBoolText(collectionInfo.IsCreatedInDb);
            ltlIsChangedAfterCreatedInDb.Text = collectionInfo.IsCreatedInDb == false
                ? "----"
                : StringUtils.GetBoolText(collectionInfo.IsChangedAfterCreatedInDb);

            ltlMetadataEdit.Text =
                $@"<a href=""{PageTableMetadata.GetRedirectUrl(tableName)}"">管理真实字段</a>";

            ltlStyleEdit.Text = $@"<a href=""{PageTableStyle.GetRedirectUrl(tableName)}"">管理虚拟字段</a>";

            ltlEdit.Text = $@"<a href=""javascript:;"" onclick=""{ModalAuxiliaryTableAdd.GetOpenWindowString(tableName)}"">编辑</a>";

            if (!isTableUsed)
            {
                var script = AlertUtils.Warning("删除辅助表", $"此操作将删除辅助表“{tableName}”，如果辅助表已在数据库中建立，将同时删除建立的辅助表，确认吗？", "取 消",
                    "确认删除", $"location.href = '{GetRedirectUrl()}?Delete=True&ENName={tableName}';");
                ltlDelete.Text =
                $@"<a href=""javascript:;"" onClick=""{script}"">删除</a>";
            }
        }
	}
}
