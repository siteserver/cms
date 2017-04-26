using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Sys
{
    public class PageTableMetadata : BasePageCms
    {
        public DataGrid DgContents;
        public Control DivSyncTable;
        public Literal LtlSqlString;
        public Button BtnAddColumn;
        public Button BtnBatchAddColumn;

        private bool _showSqlTable = true;
        private string _tableName;
        private EAuxiliaryTableType _tableType;
        private bool _tableIsRealCreated;
        private int _usedNum;
        private string _redirectUrl;

        public static string GetRedirectUrl(string tableName, EAuxiliaryTableType tableType, int publishmentSystemId)
        {
            return PageUtils.GetSysUrl(nameof(PageTableMetadata), new NameValueCollection
            {
                {"ENName", tableName},
                {"TableType", EAuxiliaryTableTypeUtils.GetValue(tableType)},
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public string GetReturnUrl()
        {
            if (PublishmentSystemId == 0)
            {
                return PageAuxiliaryTable.GetRedirectUrl();
            }
            else
            {
                return PageContentModel.GetRedirectUrl(PublishmentSystemId);
            }
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("ENName", "TableType");
            _showSqlTable = Body.IsQueryExists("ShowCrateDBCommand");

            _tableName = Body.GetQueryString("ENName").Trim();
            _tableType = EAuxiliaryTableTypeUtils.GetEnumType(Body.GetQueryString("TableType"));
            _redirectUrl = GetRedirectUrl(_tableName, _tableType, PublishmentSystemId);

            var tableInfo = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableInfo(_tableName);

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
                    BaiRongDataProvider.TableMetadataDao.CreateAuxiliaryTable(_tableName);
                    tableInfo.IsChangedAfterCreatedInDb = false;

                    Body.AddAdminLog("创建辅助表", $"辅助表:{_tableName}");

                    SuccessMessage("辅助表创建成功！");
                    PageUtils.Redirect(_redirectUrl);
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "<br>辅助表创建失败，失败原因为：" + ex.Message + "<br>请检查创建表SQL命令");
                    var sqlString = BaiRongDataProvider.AuxiliaryTableDataDao.GetCreateAuxiliaryTableSqlString(_tableName);
                    LtlSqlString.Text = sqlString.Replace("\r\n", "<br>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                    _showSqlTable = true;
                }
            }
            else if (Body.IsQueryExists("DeleteDB"))
            {
                try
                {
                    BaiRongDataProvider.TableMetadataDao.DeleteAuxiliaryTable(_tableName);
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
                    BaiRongDataProvider.TableMetadataDao.ReCreateAuxiliaryTable(_tableName, tableInfo.AuxiliaryTableType);
                    DataProvider.NodeDao.UpdateContentNumToZero(_tableName, tableInfo.AuxiliaryTableType);
                    tableInfo.IsChangedAfterCreatedInDb = false;

                    Body.AddAdminLog("重建辅助表", $"辅助表:{_tableName}");

                    SuccessMessage("辅助表重建成功！");
                    PageUtils.Redirect(_redirectUrl);
                }
                catch (Exception ex)
                {

                    FailMessage(ex, "<br>辅助表重建失败，失败原因为：" + ex.Message + "<br>请检查创建表SQL命令");
                    var sqlString = BaiRongDataProvider.AuxiliaryTableDataDao.GetCreateAuxiliaryTableSqlString(_tableName);
                    LtlSqlString.Text = sqlString.Replace("\r\n", "<br>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                    _showSqlTable = true;
                }
            }
            else if (Body.IsQueryExists("ShowCrateDBCommand"))
            {
                var sqlString = BaiRongDataProvider.AuxiliaryTableDataDao.GetCreateAuxiliaryTableSqlString(_tableName);
                LtlSqlString.Text = sqlString.Replace("\r\n", "<br>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            }
            else if (Body.IsQueryExists("SetTaxis"))
            {
                SetTaxis();
            }

            _tableIsRealCreated = BaiRongDataProvider.TableStructureDao.IsTableExists(_tableName);

            _usedNum = BaiRongDataProvider.TableCollectionDao.GetTableUsedNum(_tableName, tableInfo.AuxiliaryTableType);

            DivSyncTable.Visible = IsNeedSync(_tableIsRealCreated, tableInfo.IsChangedAfterCreatedInDb);

            if (!IsPostBack)
            {
                BreadCrumbSys(AppManager.Sys.LeftMenu.Auxiliary, $"辅助表字段管理（{_tableName}）", AppManager.Sys.Permission.SysAuxiliary);

                DgContents.DataSource = BaiRongDataProvider.TableMetadataDao.GetDataSource(_tableName);
                DgContents.ItemDataBound += dgContents_ItemDataBound;
                DgContents.DataBind();

                BtnAddColumn.Attributes.Add("onclick", ModalTableMetadataAdd.GetOpenWindowStringToAdd(_tableName, _tableType));

                BtnBatchAddColumn.Attributes.Add("onclick", ModalTableMetadataAddBatch.GetOpenWindowStringToAdd(_tableName, _tableType));
            }
        }

        private void SetTaxis()
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

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var tableMetadataId = SqlUtils.EvalInt(e.Item.DataItem, "TableMetadataID");
                var attributeName = SqlUtils.EvalString(e.Item.DataItem, "AttributeName");
                var dataType = SqlUtils.EvalString(e.Item.DataItem, "DataType");
                var dataLength = SqlUtils.EvalInt(e.Item.DataItem, "DataLength");
                var isSystem = SqlUtils.EvalString(e.Item.DataItem, "IsSystem");

                var ltlAttributeName = e.Item.FindControl("ltlAttributeName") as Literal;
                var ltlDisplayName = e.Item.FindControl("ltlDisplayName") as Literal;
                var ltlIsVisible = e.Item.FindControl("ltlIsVisible") as Literal;
                var ltlValidate = e.Item.FindControl("ltlValidate") as Literal;
                var ltlDataType = e.Item.FindControl("ltlDataType") as Literal;
                var ltlInputType = e.Item.FindControl("ltlInputType") as Literal;
                var upLinkButton = e.Item.FindControl("UpLinkButton") as HyperLink;
                var downLinkButton = e.Item.FindControl("DownLinkButton") as HyperLink;
                var ltlStyle = e.Item.FindControl("ltlStyle") as Literal;
                var ltlEditValidate = e.Item.FindControl("ltlEditValidate") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;
                var ltlDeleteUrl = e.Item.FindControl("ltlDeleteUrl") as Literal;

                var showPopWinString = ModalTableMetadataView.GetOpenWindowString(_tableType, _tableName, attributeName);
                ltlAttributeName.Text =
                    $"<a href=\"javascript:void 0;\" onClick=\"{showPopWinString}\">{attributeName}</a>";

                var styleInfo = TableStyleManager.GetTableStyleInfo(EAuxiliaryTableTypeUtils.GetTableStyle(_tableType), _tableName, attributeName, null);
                ltlDisplayName.Text = styleInfo.DisplayName;

                ltlIsVisible.Text = StringUtils.GetTrueOrFalseImageHtml(styleInfo.IsVisible.ToString());
                ltlValidate.Text = EInputValidateTypeUtils.GetValidateInfo(styleInfo);

                ltlDataType.Text = EDataTypeUtils.GetTextByAuxiliaryTable(EDataTypeUtils.GetEnumType(dataType), dataLength);
                ltlInputType.Text = EInputTypeUtils.GetText(EInputTypeUtils.GetEnumType(styleInfo.InputType));

                if (IsSystem(isSystem))
                {
                    if (upLinkButton != null)
                    {
                        upLinkButton.NavigateUrl = PageUtils.GetSysUrl(nameof(PageTableMetadata),
                            new NameValueCollection
                            {
                                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                                {"SetTaxis", "True"},
                                {"TableStyleID", styleInfo.TableStyleId.ToString()},
                                {"Direction", "UP"},
                                {"TableMetadataId", tableMetadataId.ToString()},
                                {"ENName", _tableName},
                                {"TableType", EAuxiliaryTableTypeUtils.GetValue(_tableType)}
                            });
                    }
                    if (downLinkButton != null)
                    {
                        downLinkButton.NavigateUrl = PageUtils.GetSysUrl(nameof(PageTableMetadata),
                            new NameValueCollection
                            {
                                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                                {"SetTaxis", "True"},
                                {"TableStyleID", styleInfo.TableStyleId.ToString()},
                                {"Direction", "DOWN"},
                                {"TableMetadataId", tableMetadataId.ToString()},
                                {"ENName", _tableName},
                                {"TableType", EAuxiliaryTableTypeUtils.GetValue(_tableType)}
                            });
                    }
                }

                ltlStyle.Text = GetEditStyleHtml(tableMetadataId, attributeName);

                showPopWinString = ModalTableStyleValidateAdd.GetOpenWindowString(styleInfo.TableStyleId, _tableName, styleInfo.AttributeName, EAuxiliaryTableTypeUtils.GetTableStyle(_tableType), _redirectUrl);
                ltlEditValidate.Text = $"<a href=\"javascript:void 0;\" onClick=\"{showPopWinString}\">设置</a>";

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
        }

        public string GetShowCommandElementStyle()
        {
            return _tableName != null ? StringUtils.Constants.ShowElementStyle : StringUtils.Constants.HideElementStyle;
        }

        public string GetCreateDbCommandElementStyle()
        {
            if (_tableName == null) return StringUtils.Constants.HideElementStyle;
            return !_tableIsRealCreated ? StringUtils.Constants.ShowElementStyle : StringUtils.Constants.HideElementStyle;
        }

        public string GetDeleteDbCommandElementStyle()
        {
            if (_tableName == null) return StringUtils.Constants.HideElementStyle;
            return _tableIsRealCreated && _usedNum == 0
                ? StringUtils.Constants.ShowElementStyle
                : StringUtils.Constants.HideElementStyle;
        }

        public string GetReCreateDbCommandElementStyle()
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
                    $@"<a href=""javascript:;"" onclick=""{ModalTableMetadataAdd.GetOpenWindowStringToEdit(_tableName,
                        _tableType, tableMetadataId)}"">修改字段</a>";
            }
            return retval;
        }

        public string GetEditStyleHtml(int tableMetadataId, string attributeName)
        {
            var tableStyle = EAuxiliaryTableTypeUtils.GetTableStyle(_tableType);
            var styleInfo = TableStyleManager.GetTableStyleInfo(tableStyle, _tableName, attributeName, null);
            var showPopWinString = ModalTableStyleAdd.GetOpenWindowString(0, _tableName, attributeName, tableStyle, _redirectUrl);

            var editText = "设置";
            if (styleInfo.TableStyleId != 0)//数据库中有样式
            {
                editText = "修改";
            }
            string retval = $"<a href=\"javascript:void 0;\" onClick=\"{showPopWinString}\">{editText}</a>";

            if (styleInfo.TableStyleId == 0) return retval;

            var attributes = new NameValueCollection
            {
                {"DeleteStyle", true.ToString()},
                {"TableMetadataID", tableMetadataId.ToString()},
                {"AttributeName", attributeName}
            };
            var deleteUrl = PageUtils.AddQueryString(_redirectUrl, attributes);

            retval +=
                $@"&nbsp;&nbsp;<a href=""{deleteUrl}"" onClick=""javascript:return confirm('此操作将删除对应显示样式，确认吗？');"">删除</a>";
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
                BaiRongDataProvider.TableMetadataDao.SyncTable(_tableName);
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
