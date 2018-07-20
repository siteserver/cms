using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSiteTableMetadata : BasePageCms
    {
        public Repeater RptContents;
        public PlaceHolder PhSyncTable;
        public PlaceHolder PhSqlString;
        public Literal LtlSqlString;
        public Button BtnAdd;
        public Button BtnCreateDb;
        public Button BtnDelete;
        public Button BtnReCreateDb;
        public Button BtnSqlString;

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

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("ENName");
            _showSqlTable = AuthRequest.IsQueryExists("ShowCrateDBCommand");

            _tableName = AuthRequest.GetQueryString("ENName").Trim();
            _redirectUrl = GetRedirectUrl(_tableName);

            var tableInfo = DataProvider.TableDao.GetTableCollectionInfo(_tableName);

            if (AuthRequest.IsQueryExists("Delete"))
            {
                var tableMetadataId = AuthRequest.GetQueryInt("TableMetadataID");

                try
                {
                    var tableMetadataInfo = DataProvider.TableMetadataDao.GetTableMetadataInfo(tableMetadataId);
                    DataProvider.TableMetadataDao.Delete(tableMetadataId);

                    AuthRequest.AddAdminLog("删除内容表字段", $"内容表:{_tableName},字段名:{tableMetadataInfo.AttributeName}");

                    SuccessDeleteMessage();
                    PageUtils.Redirect(_redirectUrl);
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (AuthRequest.IsQueryExists("DeleteStyle"))//删除样式
            {
                var attributeName = AuthRequest.GetQueryString("AttributeName");
                if (TableStyleManager.IsExists(0, _tableName, attributeName))
                {
                    try
                    {
                        TableStyleManager.Delete(0, _tableName, attributeName);

                        AuthRequest.AddAdminLog("删除内容表字段样式", $"内容表:{_tableName},字段名:{attributeName}");

                        SuccessDeleteMessage();
                        PageUtils.Redirect(_redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailDeleteMessage(ex);
                    }
                }
            }
            else if (AuthRequest.IsQueryExists("CreateDB"))
            {
                try
                {
                    DataProvider.TableDao.CreateDbTable(_tableName);
                    tableInfo.IsChangedAfterCreatedInDb = false;

                    AuthRequest.AddAdminLog("创建内容表", $"内容表:{_tableName}");

                    SuccessMessage("内容表创建成功！");
                    PageUtils.Redirect(_redirectUrl);
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "<br>内容表创建失败，失败原因为：" + ex.Message + "<br>请检查创建表SQL命令");
                    var sqlString = DataProvider.ContentDao.GetCreateTableCollectionInfoSqlString(_tableName);
                    LtlSqlString.Text = sqlString.Replace("\r\n", "<br>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                    _showSqlTable = true;
                }
            }
            else if (AuthRequest.IsQueryExists("DeleteDB"))
            {
                try
                {
                    DataProvider.TableDao.DeleteDbTable(_tableName);
                    tableInfo.IsChangedAfterCreatedInDb = false;

                    AuthRequest.AddAdminLog("删除内容表", $"内容表:{_tableName}");

                    SuccessMessage("内容表删除成功！");
                    PageUtils.Redirect(_redirectUrl);
                }
                catch (Exception ex)
                {

                    FailMessage(ex, "<br>内容表删除失败，失败原因为：" + ex.Message + "<br>");
                }
            }
            else if (AuthRequest.IsQueryExists("ReCreateDB"))
            {
                try
                {
                    DataProvider.TableDao.ReCreateDbTable(_tableName);
                    DataProvider.ChannelDao.UpdateContentNumToZero(_tableName);
                    tableInfo.IsChangedAfterCreatedInDb = false;

                    AuthRequest.AddAdminLog("重建内容表", $"内容表:{_tableName}");

                    SuccessMessage("内容表重建成功！");
                    PageUtils.Redirect(_redirectUrl);
                }
                catch (Exception ex)
                {

                    FailMessage(ex, "<br>内容表重建失败，失败原因为：" + ex.Message + "<br>请检查创建表SQL命令");
                    var sqlString = DataProvider.ContentDao.GetCreateTableCollectionInfoSqlString(_tableName);
                    LtlSqlString.Text = sqlString.Replace("\r\n", "<br>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                    _showSqlTable = true;
                }
            }
            else if (AuthRequest.IsQueryExists("ShowCrateDBCommand"))
            {
                var sqlString = DataProvider.ContentDao.GetCreateTableCollectionInfoSqlString(_tableName);
                LtlSqlString.Text = sqlString.Replace("\r\n", "<br>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            }
            else if (AuthRequest.IsQueryExists("SetTaxis"))
            {
                var direction = AuthRequest.GetQueryString("Direction");
                var tableMetadataId = AuthRequest.GetQueryInt("TableMetadataId");
                switch (direction.ToUpper())
                {
                    case "UP":
                        DataProvider.TableMetadataDao.TaxisDown(tableMetadataId, _tableName);
                        break;
                    case "DOWN":
                        DataProvider.TableMetadataDao.TaxisUp(tableMetadataId, _tableName);
                        break;
                }
                SuccessMessage("排序成功！");
            }

            _tableIsRealCreated = DataProvider.DatabaseDao.IsTableExists(_tableName);

            _isTableUsed = DataProvider.SiteDao.IsTableUsed(_tableName);

            PhSyncTable.Visible = _tableIsRealCreated && tableInfo.IsChangedAfterCreatedInDb;
            PhSqlString.Visible = _showSqlTable;

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            RptContents.DataSource = DataProvider.TableMetadataDao.GetDataSource(_tableName);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            BtnAdd.Attributes.Add("onclick", ModalTableMetadataAdd.GetOpenWindowStringToAdd(_tableName));

            var redirectUrl = GetRedirectUrl(_tableName);

            bool isBtnCreateDb;
            if (string.IsNullOrEmpty(_tableName))
            {
                isBtnCreateDb = false;
            }
            else
            {
                isBtnCreateDb = !_tableIsRealCreated;
            }
            if (isBtnCreateDb)
            {
                BtnCreateDb.Attributes.Add("onclick", $"location.href='{redirectUrl}&CreateDB={true}';return false;");
            }
            else
            {
                BtnCreateDb.Visible = false;
            }

            bool isBtnDelete;
            if (_tableName == null)
            {
                isBtnDelete = false;
            }
            else
            {
                isBtnDelete = !_isTableUsed;
            }
            if (isBtnDelete)
            {
                BtnDelete.Attributes.Add("onclick", $"if (confirm('此操作将删除内容表“{_tableName}”，确认吗？'))location.href='{redirectUrl}&DeleteDB={true}';return false;");
            }
            else
            {
                BtnDelete.Visible = false;
            }

            if (isBtnCreateDb)
            {
                BtnReCreateDb.Attributes.Add("onclick", $"if (confirm('此操作将覆盖已建立的内容表，表中已存数据将丢失，确认吗？'))location.href='{redirectUrl}&ReCreateDB={true}';return false;");
            }
            else
            {
                BtnReCreateDb.Visible = false;
            }

            var isSqlString = !string.IsNullOrEmpty(_tableName);
            if (isSqlString)
            {
                BtnSqlString.Attributes.Add("onclick", $"location.href='{redirectUrl}&ShowCrateDBCommand={true}';return false;");
            }
            else
            {
                BtnSqlString.Visible = false;
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var tableMetadataId = SqlUtils.EvalInt(e.Item.DataItem, nameof(TableMetadataInfo.Id));
            var attributeName = SqlUtils.EvalString(e.Item.DataItem, nameof(TableMetadataInfo.AttributeName));
            var dataType = SqlUtils.EvalString(e.Item.DataItem, nameof(TableMetadataInfo.DataType));
            var isSystem = SqlUtils.EvalString(e.Item.DataItem, nameof(TableMetadataInfo.IsSystem));

            var ltlAttributeName = (Literal)e.Item.FindControl("ltlAttributeName");
            var ltlDataType = (Literal)e.Item.FindControl("ltlDataType");
            var hlUp = (HyperLink)e.Item.FindControl("hlUp");
            var hlDown = (HyperLink)e.Item.FindControl("hlDown");
            var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");
            var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

            ltlAttributeName.Text = attributeName;

            ltlDataType.Text = DataTypeUtils.GetText(DataTypeUtils.GetEnumType(dataType));

            if (TranslateUtils.ToBool(isSystem))
            {
                hlUp.Visible = hlDown.Visible = false;
                return;
            }

            hlUp.NavigateUrl = PageUtils.GetSettingsUrl(nameof(PageSiteTableMetadata),
                new NameValueCollection
                {
                    {"SiteId", SiteId.ToString()},
                    {"SetTaxis", "True"},
                    {"Direction", "UP"},
                    {"TableMetadataId", tableMetadataId.ToString()},
                    {"ENName", _tableName}
                });

            hlDown.NavigateUrl = PageUtils.GetSettingsUrl(nameof(PageSiteTableMetadata),
                new NameValueCollection
                {
                    {"SiteId", SiteId.ToString()},
                    {"SetTaxis", "True"},
                    {"Direction", "DOWN"},
                    {"TableMetadataId", tableMetadataId.ToString()},
                    {"ENName", _tableName}
                });

            ltlEditUrl.Text =
                $@"<a href=""javascript:;"" onclick=""{ModalTableMetadataAdd.GetOpenWindowStringToEdit(_tableName, tableMetadataId)}"">修改字段</a>";

            ltlDeleteUrl.Text =
                $@"<a href=""{PageUtils.AddQueryString(_redirectUrl, new NameValueCollection
                {
                    {"Delete", true.ToString()},
                    {"TableMetadataID", tableMetadataId.ToString()}
                })}"" onClick=""javascript:return confirm('此操作将删除辅助字段“{attributeName}”，确认吗？');"">删除字段</a>";
        }

        public void SyncTableButton_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            DataProvider.TableDao.SyncDbTable(_tableName);
            PhSyncTable.Visible = false;
            SuccessMessage("同步内容表成功！");
            PageUtils.Redirect(_redirectUrl);
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageSiteAuxiliaryTable.GetRedirectUrl());
        }
    }
}
