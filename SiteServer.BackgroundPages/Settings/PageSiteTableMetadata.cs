using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Table;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSiteTableMetadata : BasePageCms
    {
        public DataGrid DgContents;
        public Control DivSyncTable;
        public Literal LtlSqlString;
        public Button BtnAddColumn;
        public Literal LtlCommands;

        private bool _showSqlTable = true;
        private string _tableName;
        private bool _tableIsRealCreated;
        private bool _isTableUsed;
        private string _redirectUrl;

        public static string GetRedirectUrl(string tableName)
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteTableMetadata), new NameValueCollection
            {
                {"ENName", tableName}
            });
        }

        private string GetReturnUrl()
        {
            return PageSiteAuxiliaryTable.GetRedirectUrl();
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("ENName");
            _showSqlTable = Body.IsQueryExists("ShowCrateDBCommand");

            _tableName = Body.GetQueryString("ENName").Trim();
            _redirectUrl = GetRedirectUrl(_tableName);

            var tableInfo = BaiRongDataProvider.TableCollectionDao.GetTableCollectionInfo(_tableName);

            if (Body.IsQueryExists("Delete"))
            {
                var tableMetadataId = Body.GetQueryInt("TableMetadataID");

                try
                {
                    var tableMetadataInfo = BaiRongDataProvider.TableMetadataDao.GetTableMetadataInfo(tableMetadataId);
                    BaiRongDataProvider.TableMetadataDao.Delete(tableMetadataId);

                    Body.AddAdminLog("删除辅助表字段", $"辅助表:{_tableName},字段名:{tableMetadataInfo.AttributeName}");

                    SuccessDeleteMessage();
                    PageUtils.Redirect(_redirectUrl);
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (Body.IsQueryExists("DeleteStyle"))//删除样式
            {
                var attributeName = Body.GetQueryString("AttributeName");
                if (TableStyleManager.IsExists(0, _tableName, attributeName))
                {
                    try
                    {
                        TableStyleManager.Delete(0, _tableName, attributeName);

                        Body.AddAdminLog("删除辅助表字段样式", $"辅助表:{_tableName},字段名:{attributeName}");

                        SuccessDeleteMessage();
                        PageUtils.Redirect(_redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailDeleteMessage(ex);
                    }
                }
            }
            else if (Body.IsQueryExists("CreateDB"))
            {
                try
                {
                    BaiRongDataProvider.TableCollectionDao.CreateDbTable(_tableName);
                    tableInfo.IsChangedAfterCreatedInDb = false;

                    Body.AddAdminLog("创建辅助表", $"辅助表:{_tableName}");

                    SuccessMessage("辅助表创建成功！");
                    PageUtils.Redirect(_redirectUrl);
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "<br>辅助表创建失败，失败原因为：" + ex.Message + "<br>请检查创建表SQL命令");
                    var sqlString = SqlUtils.GetCreateTableCollectionInfoSqlString(_tableName);
                    LtlSqlString.Text = sqlString.Replace("\r\n", "<br>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                    _showSqlTable = true;
                }
            }
            else if (Body.IsQueryExists("DeleteDB"))
            {
                try
                {
                    BaiRongDataProvider.TableCollectionDao.DeleteDbTable(_tableName);
                    tableInfo.IsChangedAfterCreatedInDb = false;

                    Body.AddAdminLog("删除辅助表", $"辅助表:{_tableName}");

                    SuccessMessage("辅助表删除成功！");
                    PageUtils.Redirect(_redirectUrl);
                }
                catch (Exception ex)
                {

                    FailMessage(ex, "<br>辅助表删除失败，失败原因为：" + ex.Message + "<br>");
                }
            }
            else if (Body.IsQueryExists("ReCreateDB"))
            {
                try
                {
                    BaiRongDataProvider.TableCollectionDao.ReCreateDbTable(_tableName);
                    DataProvider.NodeDao.UpdateContentNumToZero(_tableName);
                    tableInfo.IsChangedAfterCreatedInDb = false;

                    Body.AddAdminLog("重建辅助表", $"辅助表:{_tableName}");

                    SuccessMessage("辅助表重建成功！");
                    PageUtils.Redirect(_redirectUrl);
                }
                catch (Exception ex)
                {

                    FailMessage(ex, "<br>辅助表重建失败，失败原因为：" + ex.Message + "<br>请检查创建表SQL命令");
                    var sqlString = SqlUtils.GetCreateTableCollectionInfoSqlString(_tableName);
                    LtlSqlString.Text = sqlString.Replace("\r\n", "<br>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                    _showSqlTable = true;
                }
            }
            else if (Body.IsQueryExists("ShowCrateDBCommand"))
            {
                var sqlString = SqlUtils.GetCreateTableCollectionInfoSqlString(_tableName);
                LtlSqlString.Text = sqlString.Replace("\r\n", "<br>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            }
            else if (Body.IsQueryExists("SetTaxis"))
            {
                var direction = Body.GetQueryString("Direction");
                var tableMetadataId = Body.GetQueryInt("TableMetadataId");
                switch (direction.ToUpper())
                {
                    case "UP":
                        BaiRongDataProvider.TableMetadataDao.TaxisDown(tableMetadataId, _tableName);
                        break;
                    case "DOWN":
                        BaiRongDataProvider.TableMetadataDao.TaxisUp(tableMetadataId, _tableName);
                        break;
                }
                SuccessMessage("排序成功！");
            }

            _tableIsRealCreated = BaiRongDataProvider.DatabaseDao.IsTableExists(_tableName);

            _isTableUsed = DataProvider.PublishmentSystemDao.IsTableUsed(_tableName);

            DivSyncTable.Visible = IsNeedSync(_tableIsRealCreated, tableInfo.IsChangedAfterCreatedInDb);

            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Site);

            DgContents.DataSource = BaiRongDataProvider.TableMetadataDao.GetDataSource(_tableName);
            DgContents.ItemDataBound += DgContents_ItemDataBound;
            DgContents.DataBind();

            BtnAddColumn.Attributes.Add("onclick", ModalTableMetadataAdd.GetOpenWindowStringToAdd(_tableName));

            var redirectUrl = GetRedirectUrl(_tableName);

            LtlCommands.Text = $@"
<span style=""{GetCreateDbCommandElementStyle()}"">
    <input type=""button"" class=""btn"" onclick=""location.href='{redirectUrl}&CreateDB={true}';"" value=""创建辅助表"" />
</span>
<span style=""{GetDeleteDbCommandElementStyle()}"">
    <input type=""button"" class=""btn"" onclick=""if (confirm('此操作将删除辅助表“{_tableName}”，确认吗？'))location.href='{redirectUrl}&DeleteDB={true}';"" value=""删除辅助表"" />
</span>
<span style=""{GetReCreateDbCommandElementStyle()}"">
    <input type=""button"" class=""btn"" onclick=""if (confirm('此操作将覆盖已建立的辅助表，表中已存数据将丢失，确认吗？'))location.href='{redirectUrl}&ReCreateDB={true}';"" value=""重新创建辅助表"" />
</span>
<span style=""{GetShowCommandElementStyle()}"">
    <input type=""button"" class=""btn"" onclick=""location.href='{redirectUrl}&ShowCrateDBCommand={true}';"" value=""显示创建表SQL命令"" />
</span>
<input type=""button"" class=""btn"" onclick=""location.href='{GetReturnUrl()}';"" value=""返 回"" />
";
        }

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            var tableMetadataId = SqlUtils.EvalInt(e.Item.DataItem, "TableMetadataID");
            var attributeName = SqlUtils.EvalString(e.Item.DataItem, "AttributeName");
            var dataType = SqlUtils.EvalString(e.Item.DataItem, "DataType");
            var dataLength = SqlUtils.EvalInt(e.Item.DataItem, "DataLength");
            var isSystem = SqlUtils.EvalString(e.Item.DataItem, "IsSystem");

            var ltlAttributeName = (Literal)e.Item.FindControl("ltlAttributeName");
            var ltlDataType = (Literal)e.Item.FindControl("ltlDataType");
            var upLinkButton = (HyperLink)e.Item.FindControl("UpLinkButton");
            var downLinkButton = (HyperLink)e.Item.FindControl("DownLinkButton");
            var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");
            var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

            ltlAttributeName.Text = attributeName;

            ltlDataType.Text = DataTypeUtils.GetTextByAuxiliaryTable(DataTypeUtils.GetEnumType(dataType), dataLength);

            if (IsSystem(isSystem))
            {
                if (upLinkButton != null)
                {
                    upLinkButton.NavigateUrl = PageUtils.GetSettingsUrl(nameof(PageSiteTableMetadata),
                        new NameValueCollection
                        {
                            {"PublishmentSystemID", PublishmentSystemId.ToString()},
                            {"SetTaxis", "True"},
                            {"Direction", "UP"},
                            {"TableMetadataId", tableMetadataId.ToString()},
                            {"ENName", _tableName}
                        });
                }
                if (downLinkButton != null)
                {
                    downLinkButton.NavigateUrl = PageUtils.GetSettingsUrl(nameof(PageSiteTableMetadata),
                        new NameValueCollection
                        {
                            {"PublishmentSystemID", PublishmentSystemId.ToString()},
                            {"SetTaxis", "True"},
                            {"Direction", "DOWN"},
                            {"TableMetadataId", tableMetadataId.ToString()},
                            {"ENName", _tableName}
                        });
                }
            }

            ltlEditUrl.Text = GetEditHtml(isSystem, tableMetadataId);

            if (!IsSystem(isSystem))
            {
                var attributes = new NameValueCollection
                {
                    {"Delete", true.ToString()},
                    {"TableMetadataID", tableMetadataId.ToString()}
                };
                var deleteUrl = PageUtils.AddQueryString(_redirectUrl, attributes);
                ltlDeleteUrl.Text =
                    $@"<a href=""{deleteUrl}"" onClick=""javascript:return confirm('此操作将删除辅助字段“{attributeName}”，确认吗？');"">删除字段</a>";
            }
        }

        private string GetShowCommandElementStyle()
        {
            return _tableName != null ? StringUtils.Constants.ShowElementStyle : StringUtils.Constants.HideElementStyle;
        }

        private string GetCreateDbCommandElementStyle()
        {
            if (_tableName == null) return StringUtils.Constants.HideElementStyle;
            return !_tableIsRealCreated ? StringUtils.Constants.ShowElementStyle : StringUtils.Constants.HideElementStyle;
        }

        private string GetDeleteDbCommandElementStyle()
        {
            if (_tableName == null) return StringUtils.Constants.HideElementStyle;
            return !_isTableUsed
                ? StringUtils.Constants.ShowElementStyle
                : StringUtils.Constants.HideElementStyle;
        }

        private string GetReCreateDbCommandElementStyle()
        {
            if (_tableName == null) return StringUtils.Constants.HideElementStyle;
            return _tableIsRealCreated ? StringUtils.Constants.ShowElementStyle : StringUtils.Constants.HideElementStyle;
        }

        public string GetSqlTableStyle()
        {
            return _showSqlTable ? StringUtils.Constants.ShowElementStyle : StringUtils.Constants.HideElementStyle;
        }

        public bool IsSystem(string isSystem)
        {
            return TranslateUtils.ToBool(isSystem);
        }

        public string GetEditHtml(string isSystem, int tableMetadataId)
        {
            var retval = string.Empty;

            if (!IsSystem(isSystem))
            {
                retval =
                    $@"<a href=""javascript:;"" onclick=""{ModalTableMetadataAdd.GetOpenWindowStringToEdit(_tableName, tableMetadataId)}"">修改字段</a>";
            }
            return retval;
        }

        public bool IsNeedSync(bool isCreatedInDb, bool isChangedAfterCreatedInDb)
        {
            return isCreatedInDb && isChangedAfterCreatedInDb;
        }

        public void MyDataGrid_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            var tableMetadataId = (int)DgContents.DataKeys[e.Item.ItemIndex];
            var direction = e.CommandName;

            switch (direction.ToUpper())
            {
                case "UP":
                    BaiRongDataProvider.TableMetadataDao.TaxisDown(tableMetadataId, _tableName);
                    break;
                case "DOWN":
                    BaiRongDataProvider.TableMetadataDao.TaxisUp(tableMetadataId, _tableName);
                    break;
            }
            SuccessMessage("排序成功！");
            PageUtils.Redirect(_redirectUrl);
        }

        public void SyncTableButton_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            try
            {
                BaiRongDataProvider.TableCollectionDao.SyncDbTable(_tableName);
                DivSyncTable.Visible = false;
                SuccessMessage("同步辅助表成功！");
                PageUtils.Redirect(_redirectUrl);
            }
            catch (Exception ex)
            {
                FailMessage(ex, "<br>同步辅助表失败，失败原因为：" + ex.Message);
            }
        }
    }
}
