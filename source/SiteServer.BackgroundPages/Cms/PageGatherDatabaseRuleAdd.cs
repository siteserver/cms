using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageGatherDatabaseRuleAdd : BasePageCms
    {
        public Literal ltlPageTitle;
		public PlaceHolder GatherRuleBase;
		public TextBox GatherRuleName;
		public DropDownList NodeIDDropDownList;
		public TextBox GatherNum;
		public RadioButtonList IsChecked;
        public RadioButtonList IsAutoCreate;

        public PlaceHolder GatherDatabaseLogin;
        public TextBox DatabaseServer;
        public Control DatabaseServerRow;
        public TextBox DatabaseFilePath;
        public Control DatabaseFilePathRow;
        public TextBox UserName;
        public TextBox Password;
        public HtmlInputHidden PasswordHidden;
        public HtmlInputHidden DatabaseNameHidden;
        public HtmlInputHidden RelatedTableNameHidden;
        public HtmlInputHidden RelatedIdentityHidden;
        public HtmlInputHidden RelatedOrderByHidden;

        public PlaceHolder GatherRelatedTable;
        public DropDownList DatabaseName;
        public Control DatabaseNameRow;
        public DropDownList RelatedTableName;
        public DropDownList RelatedIdentity;
        public DropDownList RelatedOrderBy;
        public RadioButtonList IsOrderByDesc;
        public TextBox WhereString;
		
        public PlaceHolder GatherTableMatch;
        public Literal TableName;
        public Literal TableNameToMatch;
        public ListBox Columns;
        public ListBox ColumnsToMatch;

		public PlaceHolder Done;

		public PlaceHolder OperatingError;
		public Label ErrorLabel;

		public Button Previous;
		public Button Next;

		private bool _isEdit;
		private string _theGatherRuleName;

        public static string GetRedirectUrl(int publishmentSystemId, string gatherRuleName)
        {
            return PageUtils.GetCmsUrl(nameof(PageGatherDatabaseRuleAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"GatherRuleName", gatherRuleName}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (Body.IsQueryExists("GatherRuleName"))
			{
				_isEdit = true;
                _theGatherRuleName = Body.GetQueryString("GatherRuleName");
			}

			if (!Page.IsPostBack)
			{
                var pageTitle = _isEdit ? "编辑数据库采集规则" : "添加数据库采集规则";
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdGather, pageTitle, AppManager.Cms.Permission.WebSite.Gather);

                ltlPageTitle.Text = pageTitle;

                NodeManager.AddListItemsForAddContent(NodeIDDropDownList.Items, PublishmentSystemInfo, true, Body.AdministratorName);

				SetActivePanel(WizardPanel.GatherRuleBase, GatherRuleBase);

				if (_isEdit)
				{
                    var gatherDatabaseRuleInfo = DataProvider.GatherDatabaseRuleDao.GetGatherDatabaseRuleInfo(_theGatherRuleName, PublishmentSystemId);
					GatherRuleName.Text = gatherDatabaseRuleInfo.GatherRuleName;
                    WhereString.Text = gatherDatabaseRuleInfo.WhereString;
                    foreach (ListItem item in NodeIDDropDownList.Items)
                    {
                        if (item.Value.Equals(gatherDatabaseRuleInfo.NodeId.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }
					GatherNum.Text = gatherDatabaseRuleInfo.GatherNum.ToString();
					foreach (ListItem item in IsChecked.Items)
					{
                        if (item.Value.Equals(gatherDatabaseRuleInfo.IsChecked.ToString()))
						{
							item.Selected = true;
						}
						else
						{
							item.Selected = false;
						}
					}
                    foreach (ListItem item in IsAutoCreate.Items)
                    {
                        if (item.Value.Equals(gatherDatabaseRuleInfo.IsAutoCreate.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }

                    DatabaseServer.Text = SqlUtils.GetValueFromConnectionString(gatherDatabaseRuleInfo.ConnectionString, "server");
                    UserName.Text = SqlUtils.GetValueFromConnectionString(gatherDatabaseRuleInfo.ConnectionString, "uid");
                    Password.Text = SqlUtils.GetValueFromConnectionString(gatherDatabaseRuleInfo.ConnectionString, "pwd");
                    DatabaseNameHidden.Value = SqlUtils.GetValueFromConnectionString(gatherDatabaseRuleInfo.ConnectionString, "database");

                    RelatedTableNameHidden.Value = gatherDatabaseRuleInfo.RelatedTableName;
                    RelatedIdentityHidden.Value = gatherDatabaseRuleInfo.RelatedIdentity;
                    RelatedOrderByHidden.Value = gatherDatabaseRuleInfo.RelatedOrderBy;
                    
					foreach (ListItem item in IsOrderByDesc.Items)
					{
                        if (item.Value.Equals(gatherDatabaseRuleInfo.IsOrderByDesc.ToString()))
						{
							item.Selected = true;
						}
						else
						{
							item.Selected = false;
						}
					}
				}

                DatabaseType_Changed(null, EventArgs.Empty);
			}			

			SuccessMessage(string.Empty);
		}

        public void DatabaseType_Changed(object sender, EventArgs e)
        {
            DatabaseServerRow.Visible = true;
            DatabaseFilePathRow.Visible = false;
            DatabaseNameRow.Visible = true;
        }

        public void DatabaseName_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DatabaseName.SelectedValue))
            {
                RelatedTableName.Items.Clear();
            }
            else
            {
                var isSuccess = false;
                try
                {
                    using (var connection = new SqlConnection(GetDatabaseConnectionString()))
                    {
                        connection.Open();
                        connection.Close();
                    }
                    isSuccess = true;
                }
                catch (SqlException se)
                {
                    string errorMessage;
                    switch (se.Number)
                    {
                        case 4060:	// login fails
                            errorMessage = "You can't login to that database. Please select another one<br />" + se.Message;
                            break;
                        default:
                            errorMessage = $"Number:{se.Number}:<br/>Message:{se.Message}";
                            break;
                    }
                    FailMessage(errorMessage);
                    SetActivePanel(WizardPanel.GatherRelatedTable, GatherRelatedTable);
                }
                catch (Exception ex)
                {
                    FailMessage(ex.Message);
                    SetActivePanel(WizardPanel.GatherRelatedTable, GatherRelatedTable);
                }
                if (isSuccess)
                {
                    var dictionary = BaiRongDataProvider.TableStructureDao.GetTablesAndViewsDictionary(GetDatabaseConnectionString(), DatabaseName.SelectedValue);
                    RelatedTableName.Items.Clear();
                    var item = new ListItem("请选择表或视图", string.Empty);
                    RelatedTableName.Items.Add(item);
                    foreach (string theTableName in dictionary.Keys)
                    {
                        item = new ListItem(theTableName, dictionary[theTableName].ToString());
                        if (StringUtils.EqualsIgnoreCase(theTableName, RelatedTableNameHidden.Value))
                        {
                            item.Selected = true;
                        }
                        RelatedTableName.Items.Add(item);
                    }
                    RelatedTable_Changed(null, EventArgs.Empty);
                }
            }
        }

        public void RelatedTable_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RelatedTableName.SelectedValue))
            {
                RelatedIdentity.Items.Clear();
                RelatedOrderBy.Items.Clear();
            }
            else
            {
                var tableColumnInfoList = BaiRongDataProvider.TableStructureDao.GetTableColumnInfoList(GetDatabaseConnectionString(), DatabaseName.SelectedValue, RelatedTableName.SelectedValue);
                RelatedIdentity.Items.Clear();
                RelatedOrderBy.Items.Clear();
                var item = new ListItem("请选择主键字段名称", string.Empty);
                RelatedIdentity.Items.Add(item);
                item = new ListItem("请选择排序字段名称", string.Empty);
                RelatedOrderBy.Items.Add(item);
                foreach (var tableColumnInfo in tableColumnInfoList)
                {
                    item = new ListItem(
                        $"{tableColumnInfo.ColumnName}({EDataTypeUtils.GetValue(tableColumnInfo.DataType)} {tableColumnInfo.Length})", tableColumnInfo.ColumnName);
                    if (StringUtils.EqualsIgnoreCase(tableColumnInfo.ColumnName, RelatedIdentityHidden.Value))
                    {
                        item.Selected = true;
                    }
                    RelatedIdentity.Items.Add(item);
                }

                foreach (var tableColumnInfo in tableColumnInfoList)
                {
                    item = new ListItem(
                        $"{tableColumnInfo.ColumnName}({EDataTypeUtils.GetValue(tableColumnInfo.DataType)} {tableColumnInfo.Length})", tableColumnInfo.ColumnName);
                    if (StringUtils.EqualsIgnoreCase(tableColumnInfo.ColumnName, RelatedOrderByHidden.Value))
                    {
                        item.Selected = true;
                    }
                    RelatedOrderBy.Items.Add(item);
                }
            }
        }

        private string GetConnectionString()
        {
            string retval = retval =
                $"server={DatabaseServer.Text};uid={UserName.Text};pwd={PasswordHidden.Value}";
            return retval;
        }

        private string GetDatabaseConnectionString()
        {
            string retval = retval = $"{GetConnectionString()};database={DatabaseName.SelectedValue}";
            return retval;
        }

        private WizardPanel CurrentWizardPanel
		{
			get
			{
				if (ViewState["WizardPanel"] != null)
					return (WizardPanel)ViewState["WizardPanel"];

				return WizardPanel.GatherRuleBase;
			}
			set
			{
				ViewState["WizardPanel"] = value;
			}
		}


		private enum WizardPanel
		{
			GatherRuleBase,
            GatherDatabaseLogin,
            GatherRelatedTable,
            GatherTableMatch,
			OperatingError,
			Done
		}

		void SetActivePanel(WizardPanel panel, Control controlToShow)
		{
			var currentPanel = FindControl(CurrentWizardPanel.ToString()) as PlaceHolder;
			if (currentPanel != null)
				currentPanel.Visible = false;

			switch (panel)
			{
				case WizardPanel.GatherRuleBase:
                    Previous.CssClass = "btn disabled";
                    Previous.Enabled = false;
					break;
				case WizardPanel.Done:
                    Previous.CssClass = "btn disabled";
                    Previous.Enabled = false;
                    Next.CssClass = "btn btn-primary disabled";
                    Next.Enabled = false;
                    AddWaitAndRedirectScript(PageGatherDatabaseRule.GetRedirectUrl(PublishmentSystemId));
					break;
				case WizardPanel.OperatingError:
                    Previous.CssClass = "btn disabled";
                    Previous.Enabled = false;
                    Next.CssClass = "btn btn-primary disabled";
                    Next.Enabled = false;
					break;
				default:
                    Previous.CssClass = "btn";
                    Previous.Enabled = true;
                    Next.CssClass = "btn btn-primary";
                    Next.Enabled = true;
					break;
			}

			controlToShow.Visible = true;
			CurrentWizardPanel = panel;
		}

        private bool Validate_GatherRuleBase(out string errorMessage)
		{
			if (string.IsNullOrEmpty(GatherRuleName.Text))
			{
				errorMessage = "必须填写采集规则名称！";
				return false;
			}

			if (_isEdit == false)
			{
				var gatherRuleNameList = DataProvider.GatherRuleDao.GetGatherRuleNameArrayList(PublishmentSystemId);
				if (gatherRuleNameList.IndexOf(GatherRuleName.Text) != -1)
				{
					errorMessage = "采集规则名称已存在！";
					return false;
				}
			}

			errorMessage = string.Empty;
			return true;
		}

        private bool Validate_GatherDatabaseLogin(out string errorMessage)
        {
            if (string.IsNullOrEmpty(DatabaseServer.Text))
            {
                errorMessage = "必须填写IP地址或者服务器名！";
                return false;
            }
            else if (string.IsNullOrEmpty(UserName.Text))
            {
                errorMessage = "必须填写登录账号！";
                return false;
            }

            try
            {
                var connection = new SqlConnection(GetConnectionString());
                connection.Open();
                connection.Close();
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }

            var databaseNameList = BaiRongDataProvider.TableStructureDao.GetDatabaseNameList(GetConnectionString());
            DatabaseName.Items.Clear();
            var item = new ListItem("请选择数据库", string.Empty);
            DatabaseName.Items.Add(item);
            foreach (var theDatabaseName in databaseNameList)
            {
                item = new ListItem(theDatabaseName, theDatabaseName);
                if (StringUtils.EqualsIgnoreCase(theDatabaseName, DatabaseNameHidden.Value))
                {
                    item.Selected = true;
                }
                DatabaseName.Items.Add(item);
            }
            DatabaseName_Changed(null, EventArgs.Empty);

            errorMessage = string.Empty;
            return true;
        }

        private bool Validate_GatherRelatedTable(out string errorMessage)
        {
            if (string.IsNullOrEmpty(RelatedTableName.SelectedValue))
            {
                errorMessage = "必须选择采集表名称！";
                return false;
            }
            else if (string.IsNullOrEmpty(RelatedIdentity.SelectedValue))
            {
                errorMessage = "必须选择主键字段名称！";
                return false;
            }
            else if (string.IsNullOrEmpty(RelatedOrderBy.SelectedValue))
            {
                errorMessage = "必须选择排序字段名称！";
                return false;
            }

            TableName.Text = RelatedTableName.SelectedItem.Text;
            var nodeID = TranslateUtils.ToInt(NodeIDDropDownList.SelectedValue);
            TableNameToMatch.Text = NodeManager.GetTableName(PublishmentSystemInfo, nodeID);
            var columnsMap = new NameValueCollection();
            if (_isEdit)
            {
                var tableMatchID = DataProvider.GatherDatabaseRuleDao.GetTableMatchId(GatherRuleName.Text, PublishmentSystemId);
                if (tableMatchID != 0)
                {
                    var tableMatchInfo = BaiRongDataProvider.TableMatchDao.GetTableMatchInfo(tableMatchID);
                    if (tableMatchInfo != null)
                    {
                        columnsMap = tableMatchInfo.ColumnsMap;
                    }
                }
            }
            SetColumns(columnsMap);

            errorMessage = string.Empty;
            return true;
        }

        private static bool Validate_GatherTableMatch(out string errorMessage)
        {
            var columnsMap = GetColumnsMap();
            if (columnsMap == null || columnsMap.Count == 0)
            {
                errorMessage = "必须匹配表字段！";
                return false;
            }
            else
            {
                var hasTitle = false;
                foreach (string key in columnsMap.Keys)
                {
                    var columnsToMatch = columnsMap[key];
                    if (StringUtils.EqualsIgnoreCase(ContentAttribute.Title, columnsToMatch))
                    {
                        hasTitle = true;
                        break;
                    }
                }
                if (hasTitle == false)
                {
                    errorMessage = "必须匹配标题字段！";
                    return false;
                }
            }

            errorMessage = string.Empty;
            return true;
        }

        private bool Validate_InsertGatherDatabaseRule(out string errorMessage)
		{
			try
			{
                var isNeedAdd = false;
				if (_isEdit)
				{
                    if (_theGatherRuleName != GatherRuleName.Text)
                    {
                        isNeedAdd = true;
                        DataProvider.GatherDatabaseRuleDao.Delete(_theGatherRuleName, PublishmentSystemId);
                    }
                    else
                    {
                        var gatherDatabaseRuleInfo = DataProvider.GatherDatabaseRuleDao.GetGatherDatabaseRuleInfo(_theGatherRuleName, PublishmentSystemId);
                        gatherDatabaseRuleInfo.ConnectionString = GetDatabaseConnectionString();
                        gatherDatabaseRuleInfo.RelatedTableName = TableName.Text;
                        gatherDatabaseRuleInfo.RelatedIdentity = RelatedIdentity.SelectedValue;
                        gatherDatabaseRuleInfo.RelatedOrderBy = RelatedOrderBy.SelectedValue;
                        gatherDatabaseRuleInfo.WhereString = WhereString.Text;
                        if (NodeIDDropDownList.SelectedValue != null)
                        {
                            gatherDatabaseRuleInfo.NodeId = int.Parse(NodeIDDropDownList.SelectedValue);
                        }
                        gatherDatabaseRuleInfo.GatherNum = int.Parse(GatherNum.Text);
                        gatherDatabaseRuleInfo.IsChecked = TranslateUtils.ToBool(IsChecked.SelectedValue);
                        gatherDatabaseRuleInfo.IsAutoCreate = TranslateUtils.ToBool(IsAutoCreate.SelectedValue);
                        gatherDatabaseRuleInfo.IsOrderByDesc = TranslateUtils.ToBool(IsOrderByDesc.SelectedValue);
                        gatherDatabaseRuleInfo.LastGatherDate = DateUtils.SqlMinValue;

                        var tableMatchInfo =
                            BaiRongDataProvider.TableMatchDao.GetTableMatchInfo(gatherDatabaseRuleInfo.TableMatchId);
                        if (tableMatchInfo == null)
                        {
                            tableMatchInfo =
                                new TableMatchInfo(0, gatherDatabaseRuleInfo.ConnectionString, TableName.Text,
                                                   WebConfigUtils.ConnectionString, TableNameToMatch.Text,
                                                   GetColumnsMap());
                            gatherDatabaseRuleInfo.TableMatchId =
                                BaiRongDataProvider.TableMatchDao.Insert(tableMatchInfo);
                        }
                        else
                        {
                            tableMatchInfo.ConnectionString = gatherDatabaseRuleInfo.ConnectionString;
                            tableMatchInfo.TableName = TableName.Text;
                            tableMatchInfo.ConnectionStringToMatch = WebConfigUtils.ConnectionString;
                            tableMatchInfo.TableNameToMatch = TableNameToMatch.Text;
                            tableMatchInfo.ColumnsMap = GetColumnsMap();
                            BaiRongDataProvider.TableMatchDao.Update(tableMatchInfo);
                        }

                        DataProvider.GatherDatabaseRuleDao.Update(gatherDatabaseRuleInfo);
                    }
				}
				else
				{
				    isNeedAdd = true;
				}

                if (isNeedAdd)
                {
                    var gatherDatabaseRuleInfo = new GatherDatabaseRuleInfo();
                    gatherDatabaseRuleInfo.GatherRuleName = GatherRuleName.Text;
                    gatherDatabaseRuleInfo.PublishmentSystemId = PublishmentSystemId;
                    gatherDatabaseRuleInfo.ConnectionString = GetDatabaseConnectionString();
                    gatherDatabaseRuleInfo.RelatedTableName = TableName.Text;
                    gatherDatabaseRuleInfo.RelatedIdentity = RelatedIdentity.SelectedValue;
                    gatherDatabaseRuleInfo.RelatedOrderBy = RelatedOrderBy.SelectedValue;
                    gatherDatabaseRuleInfo.WhereString = WhereString.Text;
                    if (NodeIDDropDownList.SelectedValue != null)
                    {
                        gatherDatabaseRuleInfo.NodeId = int.Parse(NodeIDDropDownList.SelectedValue);
                    }
                    gatherDatabaseRuleInfo.GatherNum = int.Parse(GatherNum.Text);
                    gatherDatabaseRuleInfo.IsChecked = TranslateUtils.ToBool(IsChecked.SelectedValue);
                    gatherDatabaseRuleInfo.IsAutoCreate = TranslateUtils.ToBool(IsAutoCreate.SelectedValue);
                    gatherDatabaseRuleInfo.IsOrderByDesc = TranslateUtils.ToBool(IsOrderByDesc.SelectedValue);
                    gatherDatabaseRuleInfo.LastGatherDate = DateUtils.SqlMinValue;

                    var tableMatchInfo = new TableMatchInfo(0, gatherDatabaseRuleInfo.ConnectionString, TableName.Text, WebConfigUtils.ConnectionString, TableNameToMatch.Text, GetColumnsMap());
                    gatherDatabaseRuleInfo.TableMatchId = BaiRongDataProvider.TableMatchDao.Insert(tableMatchInfo);

                    DataProvider.GatherDatabaseRuleDao.Insert(gatherDatabaseRuleInfo);
                }

                if (isNeedAdd)
                {
                    Body.AddSiteLog(PublishmentSystemId, "添加数据库采集规则", $"采集规则:{GatherRuleName.Text}");
                }
                else
                {
                    Body.AddSiteLog(PublishmentSystemId, "编辑数据库采集规则", $"采集规则:{GatherRuleName.Text}");
                }

				errorMessage = string.Empty;
				return true;
			}
			catch
			{
				errorMessage = "操作失败！";
				return false;
			}
		}

        private static void SaveColumnsMap(NameValueCollection columnsMap)
        {
            DbCacheManager.RemoveAndInsert("SiteServer.BackgroundPages.Cms.BackgroundGatherDatabaseRuleAdd.TableMatchColumnsMap", TranslateUtils.NameValueCollectionToString(columnsMap));
        }

        private static NameValueCollection GetColumnsMap()
        {
            var columnsMap = TranslateUtils.ToNameValueCollection(DbCacheManager.GetValue("SiteServer.BackgroundPages.Cms.BackgroundGatherDatabaseRuleAdd.TableMatchColumnsMap"));
            return columnsMap;
        }

        private void SetColumns(NameValueCollection columnsMap)
        {
            Columns.Items.Clear();
            ColumnsToMatch.Items.Clear();

            var tableColumnInfoList = BaiRongDataProvider.TableStructureDao.GetTableColumnInfoList(GetDatabaseConnectionString(), DatabaseName.SelectedValue, RelatedTableName.SelectedValue);
            var columnToMatchArrayList = new ArrayList();
            foreach (var tableColumnInfo in tableColumnInfoList)
            {
                string text =
                    $"{tableColumnInfo.ColumnName}({EDataTypeUtils.GetValue(tableColumnInfo.DataType)} {tableColumnInfo.Length})";
                var value = tableColumnInfo.ColumnName.ToLower();
                var columnToMatch = columnsMap[value];
                if (!string.IsNullOrEmpty(columnToMatch))
                {
                    var tableMetadataInfoToMatch = TableManager.GetTableMetadataInfo(TableNameToMatch.Text, columnToMatch);
                    if (tableMetadataInfoToMatch != null)
                    {
                        columnToMatchArrayList.Add(columnToMatch);
                        text += " -> " +
                                $"{tableMetadataInfoToMatch.AttributeName}({EDataTypeUtils.GetValue(tableMetadataInfoToMatch.DataType)} {tableMetadataInfoToMatch.DataLength})";
                        value += "&" + columnToMatch;
                    }
                }
                Columns.Items.Add(new ListItem(text, value));
            }

            var tableMetadataInfoList = TableManager.GetTableMetadataInfoList(TableNameToMatch.Text);
            foreach (var tableMetadataInfo in tableMetadataInfoList)
            {
                var value = tableMetadataInfo.AttributeName.ToLower();
                if (!columnToMatchArrayList.Contains(tableMetadataInfo.AttributeName))
                {
                    string text =
                        $"{tableMetadataInfo.AttributeName}({EDataTypeUtils.GetValue(tableMetadataInfo.DataType)} {tableMetadataInfo.DataLength})";
                    ColumnsToMatch.Items.Add(new ListItem(text, value));
                }
            }

            SaveColumnsMap(columnsMap);
        }

        protected void Add_OnClick(object sender, EventArgs e)
        {
            var columnsMap = GetColumnsMap();

            if (!string.IsNullOrEmpty(Columns.SelectedValue) && !string.IsNullOrEmpty(ColumnsToMatch.SelectedValue))
            {
                if (Columns.SelectedValue.IndexOf("&") != -1)
                {
                    columnsMap[Columns.SelectedValue.Split('&')[0].ToLower()] = ColumnsToMatch.SelectedValue.ToLower();
                }
                else
                {
                    columnsMap[Columns.SelectedValue.ToLower()] = ColumnsToMatch.SelectedValue.ToLower();
                }
                SetColumns(columnsMap);
            }
        }

        protected void Delete_OnClick(object sender, EventArgs e)
        {
            var columnsMap = GetColumnsMap();

            if (!string.IsNullOrEmpty(Columns.SelectedValue))
            {
                if (Columns.SelectedValue.IndexOf("&") != -1)
                {
                    columnsMap.Remove(Columns.SelectedValue.Split('&')[0]);
                    SetColumns(columnsMap);
                }
            }
        }

		public void NextPanel(object sender, EventArgs e)
		{
			string errorMessage;
			switch (CurrentWizardPanel)
			{
				case WizardPanel.GatherRuleBase:

					if (Validate_GatherRuleBase(out errorMessage))
					{
                        SetActivePanel(WizardPanel.GatherDatabaseLogin, GatherDatabaseLogin);
					}
					else
					{
                        FailMessage(errorMessage);
						SetActivePanel(WizardPanel.GatherRuleBase, GatherRuleBase);
					}

					break;

                case WizardPanel.GatherDatabaseLogin:

                    PasswordHidden.Value = Password.Text;
                    if (Validate_GatherDatabaseLogin(out errorMessage))
                    {
                        SetActivePanel(WizardPanel.GatherRelatedTable, GatherRelatedTable);
                    }
                    else
                    {
                        FailMessage(errorMessage);
                        SetActivePanel(WizardPanel.GatherDatabaseLogin, GatherDatabaseLogin);
                    }

					break;

                case WizardPanel.GatherRelatedTable:

                    if (Validate_GatherRelatedTable(out errorMessage))
                    {
                        SetActivePanel(WizardPanel.GatherTableMatch, GatherTableMatch);
                    }
                    else
                    {
                        FailMessage(errorMessage);
                        SetActivePanel(WizardPanel.GatherRelatedTable, GatherRelatedTable);
                    }

                    break;

                case WizardPanel.GatherTableMatch:

                    if (Validate_GatherTableMatch(out errorMessage))
                    {
                        if (Validate_InsertGatherDatabaseRule(out errorMessage))
                        {
                            SetActivePanel(WizardPanel.Done, Done);
                        }
                        else
                        {
                            ErrorLabel.Text = errorMessage;
                            SetActivePanel(WizardPanel.OperatingError, OperatingError);
                        }
                    }
                    else
                    {
                        FailMessage(errorMessage);
                        SetActivePanel(WizardPanel.GatherTableMatch, GatherTableMatch);
                    }

					break;

				case WizardPanel.Done:
					break;
			}
		}

		public void PreviousPanel(object sender, EventArgs e)
		{
			switch (CurrentWizardPanel)
			{
				case WizardPanel.GatherRuleBase:
					break;

                case WizardPanel.GatherDatabaseLogin:
					SetActivePanel(WizardPanel.GatherRuleBase, GatherRuleBase);
					break;

                case WizardPanel.GatherRelatedTable:
                    SetActivePanel(WizardPanel.GatherDatabaseLogin, GatherDatabaseLogin);
                    break;

				case WizardPanel.GatherTableMatch:
                    SetActivePanel(WizardPanel.GatherRelatedTable, GatherRelatedTable);
					break;
			}
		}
	}
}
