using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Collection;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
    public class TableStyleDao : DataProviderBase
    {
        // Static constants
        private const string SqlSelectTableStyle = "SELECT TableStyleID, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisible, IsVisibleInList, IsSingleLine, InputType, DefaultValue, IsHorizontal, ExtendValues FROM bairong_TableStyle WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND AttributeName = @AttributeName";

        private const string SqlSelectTableStyleByTableStyleId = "SELECT TableStyleID, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisible, IsVisibleInList, IsSingleLine, InputType, DefaultValue, IsHorizontal, ExtendValues FROM bairong_TableStyle WHERE TableStyleID = @TableStyleID";

        private const string SqlSelectTableStyles = "SELECT TableStyleID, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisible, IsVisibleInList, IsSingleLine, InputType, DefaultValue, IsHorizontal, ExtendValues FROM bairong_TableStyle WHERE TableName = @TableName AND AttributeName = @AttributeName ORDER BY RelatedIdentity";

        private const string SqlSelectAllTableStyle = "SELECT TableStyleID, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisible, IsVisibleInList, IsSingleLine, InputType, DefaultValue, IsHorizontal, ExtendValues FROM bairong_TableStyle WHERE TableName <> '' AND AttributeName <> '' ORDER BY Taxis DESC, TableStyleID DESC";

        private const string SqlUpdateTableStyle = "UPDATE bairong_TableStyle SET AttributeName = @AttributeName, Taxis = @Taxis, DisplayName = @DisplayName, HelpText = @HelpText, IsVisible = @IsVisible, IsVisibleInList = @IsVisibleInList, IsSingleLine = @IsSingleLine, InputType = @InputType, DefaultValue = @DefaultValue, IsHorizontal = @IsHorizontal, ExtendValues = @ExtendValues WHERE TableStyleID = @TableStyleID";

        private const string SqlDeleteTableStyle = "DELETE FROM bairong_TableStyle WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND AttributeName = @AttributeName";

        private const string SqlUpdateTableStyleTaxis = "UPDATE bairong_TableStyle SET Taxis = @Taxis WHERE TableStyleID = @TableStyleID";

        //AuxiliaryTableStyleItemInfo
        private const string SqlSelectAllStyleItem = "SELECT TableStyleItemID, TableStyleID, ItemTitle, ItemValue, IsSelected FROM bairong_TableStyleItem WHERE (TableStyleID = @TableStyleID)";

        private const string SqlDeleteStyleItems = "DELETE FROM bairong_TableStyleItem WHERE TableStyleID = @TableStyleID";

        private const string ParmTableStyleId = "@TableStyleID";
        private const string ParmRelatedIdentity = "@RelatedIdentity";
        private const string ParmTableName = "@TableName";
        private const string ParmAttributeName = "@AttributeName";
        private const string ParmTaxis = "@Taxis";
        private const string ParmDisplayName = "@DisplayName";
        private const string ParmHelpText = "@HelpText";
        private const string ParmIsVisible = "@IsVisible";
        private const string ParmIsVisibleInList = "@IsVisibleInList";
        private const string ParmIsSingleLine = "@IsSingleLine";
        private const string ParmInputType = "@InputType";
        private const string ParmDefaultValue = "@DefaultValue";
        private const string ParmIsHorizontal = "@IsHorizontal";
        private const string ParmExtendValues = "@ExtendValues";

        //AuxiliaryTableStyleItemInfo
        private const string ParmItemTitle = "@ItemTitle";
        private const string ParmItemValue = "@ItemValue";
        private const string ParmIsSelected = "@IsSelected";

        public int Insert(TableStyleInfo styleInfo, ETableStyle tableStyle)
        {
            return Insert(styleInfo, tableStyle, false);
        }

        public int InsertWithTaxis(TableStyleInfo styleInfo, ETableStyle tableStyle)
        {
            return Insert(styleInfo, tableStyle, true);
        }

        private string GetInsertTableStyleSqlString()
        {
            var sqlInsertTableStyle = "INSERT INTO bairong_TableStyle (RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisible, IsVisibleInList, IsSingleLine, InputType, DefaultValue, IsHorizontal, ExtendValues) VALUES (@RelatedIdentity, @TableName, @AttributeName, @Taxis, @DisplayName, @HelpText, @IsVisible, @IsVisibleInList, @IsSingleLine, @InputType, @DefaultValue, @IsHorizontal, @ExtendValues)";

            return sqlInsertTableStyle;
        }

        private string GetInsertTableStyleItemSqlString()
        {
            var sqlInsertStyleItem = "INSERT INTO bairong_TableStyleItem (TableStyleID, ItemTitle, ItemValue, IsSelected) VALUES (@TableStyleID, @ItemTitle, @ItemValue, @IsSelected)";

            return sqlInsertStyleItem;
        }

        private int Insert(TableStyleInfo styleInfo, ETableStyle tableStyle, bool isWithTaxis)
        {
            int tableStyleId;

            if (!isWithTaxis)
            {
                styleInfo.Taxis = GetNewStyleInfoTaxis(tableStyle, styleInfo.AttributeName, styleInfo.RelatedIdentity, styleInfo.TableName);
            }

            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmRelatedIdentity, EDataType.Integer, styleInfo.RelatedIdentity),
                GetParameter(ParmTableName, EDataType.VarChar, 50, styleInfo.TableName),
				GetParameter(ParmAttributeName, EDataType.VarChar, 50, styleInfo.AttributeName),
                GetParameter(ParmTaxis, EDataType.Integer, styleInfo.Taxis),
                GetParameter(ParmDisplayName, EDataType.NVarChar, 255, styleInfo.DisplayName),
                GetParameter(ParmHelpText, EDataType.VarChar, 255, styleInfo.HelpText),
                GetParameter(ParmIsVisible, EDataType.VarChar, 18, styleInfo.IsVisible.ToString()),
                GetParameter(ParmIsVisibleInList, EDataType.VarChar, 18, styleInfo.IsVisibleInList.ToString()),
                GetParameter(ParmIsSingleLine, EDataType.VarChar, 18, styleInfo.IsSingleLine.ToString()),
				GetParameter(ParmInputType, EDataType.VarChar, 50, styleInfo.InputType),
                GetParameter(ParmDefaultValue, EDataType.VarChar, 255, styleInfo.DefaultValue),
                GetParameter(ParmIsHorizontal, EDataType.VarChar, 18, styleInfo.IsHorizontal.ToString()),
                GetParameter(ParmExtendValues, EDataType.NText, styleInfo.Additional.ToString())
			};

            var sqlInsertTableStyle = GetInsertTableStyleSqlString();
            var sqlInsertStyleItem = GetInsertTableStyleItemSqlString();

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        tableStyleId = ExecuteNonQueryAndReturnId(trans, sqlInsertTableStyle, insertParms);

                        if (styleInfo.StyleItems != null && styleInfo.StyleItems.Count > 0)
                        {
                            foreach (var itemInfo in styleInfo.StyleItems)
                            {
                                var insertItemParms = new IDataParameter[]
							    {
								    GetParameter(ParmTableStyleId, EDataType.Integer, tableStyleId),
								    GetParameter(ParmItemTitle, EDataType.NVarChar, 255, itemInfo.ItemTitle),
								    GetParameter(ParmItemValue, EDataType.VarChar, 255, itemInfo.ItemValue),
								    GetParameter(ParmIsSelected, EDataType.VarChar, 18, itemInfo.IsSelected.ToString())
							    };

                                ExecuteNonQuery(trans, sqlInsertStyleItem, insertItemParms);

                            }
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return tableStyleId;
        }

        private int GetNewStyleInfoTaxis(ETableStyle tableStyle, string attributeName, int relatedIdentity, string tableName)
        {
            var taxis = 0;
            if (!TableStyleManager.IsMetadata(tableStyle, attributeName))
            {
                var maxTaxis = GetMaxTaxisByKeyStart(relatedIdentity, tableName);
                taxis = maxTaxis + 1;
            }
            return taxis;
        }

        public void InsertWithTransaction(TableStyleInfo styleInfo, ETableStyle tableStyle, IDbTransaction trans)
        {
            styleInfo.Taxis = GetNewStyleInfoTaxis(tableStyle, styleInfo.AttributeName, styleInfo.RelatedIdentity, styleInfo.TableName);

            var insertParms = new IDataParameter[]
		    {
                GetParameter(ParmRelatedIdentity, EDataType.Integer, styleInfo.RelatedIdentity),
                GetParameter(ParmTableName, EDataType.VarChar, 50, styleInfo.TableName),
			    GetParameter(ParmAttributeName, EDataType.VarChar, 50, styleInfo.AttributeName),
                GetParameter(ParmTaxis, EDataType.Integer, styleInfo.Taxis),
                GetParameter(ParmDisplayName, EDataType.NVarChar, 255, styleInfo.DisplayName),
                GetParameter(ParmHelpText, EDataType.VarChar, 255, styleInfo.HelpText),
                GetParameter(ParmIsVisible, EDataType.VarChar, 18, styleInfo.IsVisible.ToString()),
                GetParameter(ParmIsVisibleInList, EDataType.VarChar, 18, styleInfo.IsVisibleInList.ToString()),
                GetParameter(ParmIsSingleLine, EDataType.VarChar, 18, styleInfo.IsSingleLine.ToString()),
			    GetParameter(ParmInputType, EDataType.VarChar, 50, styleInfo.InputType),
                GetParameter(ParmDefaultValue, EDataType.VarChar, 255, styleInfo.DefaultValue),
                GetParameter(ParmIsHorizontal, EDataType.VarChar, 18, styleInfo.IsHorizontal.ToString()),
                GetParameter(ParmExtendValues, EDataType.NText, styleInfo.Additional.ToString())
		    };

            var sqlInsertTableStyle = GetInsertTableStyleSqlString();
            var sqlInsertStyleItem = GetInsertTableStyleItemSqlString();

            if (styleInfo.StyleItems == null || styleInfo.StyleItems.Count == 0)
            {
                ExecuteNonQuery(trans, sqlInsertTableStyle, insertParms);
            }
            else
            {
                var tableStyleId = ExecuteNonQueryAndReturnId(trans, sqlInsertTableStyle, insertParms);

                foreach (var itemInfo in styleInfo.StyleItems)
                {
                    var insertItemParms = new IDataParameter[]
				    {
					    GetParameter(ParmTableStyleId, EDataType.Integer, tableStyleId),
					    GetParameter(ParmItemTitle, EDataType.NVarChar, 255, itemInfo.ItemTitle),
					    GetParameter(ParmItemValue, EDataType.VarChar, 255, itemInfo.ItemValue),
					    GetParameter(ParmIsSelected, EDataType.VarChar, 18, itemInfo.IsSelected.ToString())
				    };

                    ExecuteNonQuery(trans, sqlInsertStyleItem, insertItemParms);

                }
            }
        }

        public void InsertStyleItems(ArrayList styleItems)
        {
            var sqlInsertStyleItem = GetInsertTableStyleItemSqlString();

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {

                        foreach (TableStyleItemInfo itemInfo in styleItems)
                        {
                            var insertItemParms = new IDataParameter[]
							{
								GetParameter(ParmTableStyleId, EDataType.Integer, itemInfo.TableStyleId),
								GetParameter(ParmItemTitle, EDataType.NVarChar, 255, itemInfo.ItemTitle),
								GetParameter(ParmItemValue, EDataType.VarChar, 255, itemInfo.ItemValue),
								GetParameter(ParmIsSelected, EDataType.VarChar, 18, itemInfo.IsSelected.ToString())
							};

                            ExecuteNonQuery(trans, sqlInsertStyleItem, insertItemParms);

                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public void DeleteStyleItems(int tableStyleId)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableStyleId, EDataType.Integer, tableStyleId)
			};

            ExecuteNonQuery(SqlDeleteStyleItems, parms);
        }

        public List<TableStyleItemInfo> GetStyleItemInfoList(int tableStyleId)
        {
            var styleItems = new List<TableStyleItemInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableStyleId, EDataType.Integer, tableStyleId)
			};

            using (var rdr = ExecuteReader(SqlSelectAllStyleItem, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TableStyleItemInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetBool(rdr, i));
                    styleItems.Add(info);
                }
                rdr.Close();
            }
            return styleItems;
        }

        public void Update(TableStyleInfo info)
        {
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmAttributeName, EDataType.VarChar, 50, info.AttributeName),
                GetParameter(ParmTaxis, EDataType.Integer, info.Taxis),
                GetParameter(ParmDisplayName, EDataType.NVarChar, 255, info.DisplayName),
                GetParameter(ParmHelpText, EDataType.VarChar, 255, info.HelpText),
                GetParameter(ParmIsVisible, EDataType.VarChar, 18, info.IsVisible.ToString()),
	            GetParameter(ParmIsVisibleInList, EDataType.VarChar, 18, info.IsVisibleInList.ToString()),
                GetParameter(ParmIsSingleLine, EDataType.VarChar, 18, info.IsSingleLine.ToString()),
				GetParameter(ParmInputType, EDataType.VarChar, 50, info.InputType),
                GetParameter(ParmDefaultValue, EDataType.VarChar, 255, info.DefaultValue),
                GetParameter(ParmIsHorizontal, EDataType.VarChar, 18, info.IsHorizontal.ToString()),
                GetParameter(ParmExtendValues, EDataType.NText, info.Additional.ToString()),
                GetParameter(ParmTableStyleId, EDataType.Integer, info.TableStyleId)
			};

            ExecuteNonQuery(SqlUpdateTableStyle, updateParms);
        }

        public void Delete(int relatedIdentity, string tableName, string attributeName)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmRelatedIdentity, EDataType.Integer, relatedIdentity),
                GetParameter(ParmTableName, EDataType.VarChar, 50, tableName),
                GetParameter(ParmAttributeName, EDataType.VarChar, 50, attributeName)
            };

            ExecuteNonQuery(SqlDeleteTableStyle, parms);
            TableStyleManager.IsChanged = true;
        }

        public void Delete(List<int> relatedIdentities, string tableName)
        {
            if (relatedIdentities != null && relatedIdentities.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM bairong_TableStyle WHERE RelatedIdentity IN ({TranslateUtils.ToSqlInStringWithoutQuote(relatedIdentities)}) AND TableName = '{PageUtils.FilterSql(tableName)}'";
                ExecuteNonQuery(sqlString);
                TableStyleManager.IsChanged = true;
            }
        }

        public ArrayList GetTableStyleInfoArrayList(ArrayList relatedIdentities, string tableName)
        {
            var arraylist = new ArrayList();

            string sqlString =
                $"SELECT TableStyleID, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisible, IsVisibleInList, IsSingleLine, InputType, DefaultValue, IsHorizontal, ExtendValues FROM bairong_TableStyle WHERE RelatedIdentity IN ({TranslateUtils.ToSqlInStringWithoutQuote(relatedIdentities)}) AND TableName = '{PageUtils.FilterSql(tableName)}' ORDER BY TableStyleID DESC";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var styleInfo = GetTableStyleInfoByReader(rdr);
                    arraylist.Add(styleInfo);
                }
                rdr.Close();
            }

            return arraylist;
        }

        public bool IsExists(int relatedIdentity, string tableName, string attributeName)
        {
            var exists = false;

            var parms = new IDataParameter[]
			{
                GetParameter(ParmRelatedIdentity, EDataType.Integer, relatedIdentity),
                GetParameter(ParmTableName, EDataType.VarChar, 50, tableName),
				GetParameter(ParmAttributeName, EDataType.VarChar, 50, attributeName)
			};

            using (var rdr = ExecuteReader(SqlSelectTableStyle, parms))
            {
                if (rdr.Read())
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public TableStyleInfo GetTableStyleInfo(int tableStyleId)
        {
            TableStyleInfo styleInfo = null;

            var parms = new IDataParameter[]
			{
                GetParameter(ParmTableStyleId, EDataType.Integer, tableStyleId)
			};

            using (var rdr = ExecuteReader(SqlSelectTableStyleByTableStyleId, parms))
            {
                if (rdr.Read())
                {
                    styleInfo = GetTableStyleInfoByReader(rdr);
                }
                rdr.Close();
            }

            return styleInfo;
        }

        public TableStyleInfo GetTableStyleInfo(int relatedIdentity, string tableName, string attributeName)
        {
            TableStyleInfo styleInfo = null;

            var parms = new IDataParameter[]
			{
                GetParameter(ParmRelatedIdentity, EDataType.Integer, relatedIdentity),
                GetParameter(ParmTableName, EDataType.VarChar, 50, tableName),
				GetParameter(ParmAttributeName, EDataType.VarChar, 50, attributeName)
			};

            using (var rdr = ExecuteReader(SqlSelectTableStyle, parms))
            {
                if (rdr.Read())
                {
                    styleInfo = GetTableStyleInfoByReader(rdr);
                }
                rdr.Close();
            }

            return styleInfo;
        }

        private TableStyleInfo GetTableStyleInfoByReader(IDataReader rdr)
        {
            var i = 0;
            var tableStyleId = GetInt(rdr, i++);
            var relatedIdentity = GetInt(rdr, i++);
            var tableName = GetString(rdr, i++);
            var attributeName = GetString(rdr, i++);
            var taxis = GetInt(rdr, i++);
            var displayName = GetString(rdr, i++);
            var helpText = GetString(rdr, i++);
            var isVisible = GetBool(rdr, i++);
            var isVisibleInList = GetBool(rdr, i++);
            var isSingleLine = GetBool(rdr, i++);
            var inputType = GetString(rdr, i++);
            var defaultValue = GetString(rdr, i++);
            var isHorizontal = GetBool(rdr, i++);
            var extendValues = GetString(rdr, i);

            var styleInfo = new TableStyleInfo(tableStyleId, relatedIdentity, tableName, attributeName, taxis, displayName, helpText, isVisible, isVisibleInList, isSingleLine, inputType, defaultValue, isHorizontal, extendValues);

            return styleInfo;
        }

        public PairList GetAllTableStyleInfoPairs()
        {
            var pairs = new PairList();

            using (var rdr = ExecuteReader(SqlSelectAllTableStyle))
            {
                while (rdr.Read())
                {
                    var styleInfo = GetTableStyleInfoByReader(rdr);
                    var key = TableStyleManager.GetCacheKey(styleInfo.RelatedIdentity, styleInfo.TableName, styleInfo.AttributeName);
                    if (!pairs.ContainsKey(key))
                    {
                        var pair = new Pair(key, styleInfo);
                        pairs.Add(pair);
                    }
                }
                rdr.Close();
            }

            return pairs;
        }

        public List<TableStyleInfo> GetTableStyleInfoWithItemsList(string tableName, string attributeName)
        {
            var arraylist = new List<TableStyleInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableName, EDataType.VarChar, 50, tableName),
                GetParameter(ParmAttributeName, EDataType.VarChar, 50, attributeName)
			};

            using (var rdr = ExecuteReader(SqlSelectTableStyles, parms))
            {
                while (rdr.Read())
                {
                    var styleInfo = GetTableStyleInfoByReader(rdr);
                    if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.CheckBox) || EInputTypeUtils.Equals(styleInfo.InputType, EInputType.Radio) || EInputTypeUtils.Equals(styleInfo.InputType, EInputType.SelectMultiple) || EInputTypeUtils.Equals(styleInfo.InputType, EInputType.SelectOne))
                    {
                        var styleItems = GetStyleItemInfoList(styleInfo.TableStyleId);
                        if (styleItems != null && styleItems.Count > 0)
                        {
                            styleInfo.StyleItems = styleItems;
                        }
                    }
                    arraylist.Add(styleInfo);
                }
                rdr.Close();
            }

            return arraylist;
        }

        private int GetMaxTaxisByKeyStart(int relatedIdentity, string tableName)
        {
            string sqlString =
                $"SELECT MAX(Taxis) AS MaxTaxis FROM bairong_TableStyle WHERE RelatedIdentity = {relatedIdentity} AND TableName = '{PageUtils.FilterSql(tableName)}'";
            var maxTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    maxTaxis = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return maxTaxis;
        }

        public void TaxisUp(int tableStyleId)
        {
            var styleInfo = GetTableStyleInfo(tableStyleId);
            if (styleInfo != null)
            {
                //var sqlString = "SELECT TOP 1 TableStyleID, Taxis FROM bairong_TableStyle WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND Taxis > (SELECT Taxis FROM bairong_TableStyle WHERE TableStyleID = @TableStyleID) ORDER BY Taxis";
                var sqlString = SqlUtils.GetTopSqlString("bairong_TableStyle", "TableStyleID, Taxis", "WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND Taxis > (SELECT Taxis FROM bairong_TableStyle WHERE TableStyleID = @TableStyleID) ORDER BY Taxis", 1);

                var higherId = 0;
                var higherTaxis = 0;

                var parms = new IDataParameter[]
			    {
                    GetParameter(ParmRelatedIdentity, EDataType.Integer, styleInfo.RelatedIdentity),
                    GetParameter(ParmTableName, EDataType.VarChar, 50, styleInfo.TableName),
				    GetParameter(ParmTableStyleId, EDataType.Integer, tableStyleId)
			    };

                using (var rdr = ExecuteReader(sqlString, parms))
                {
                    if (rdr.Read())
                    {
                        higherId = GetInt(rdr, 0);
                        higherTaxis = GetInt(rdr, 1);
                    }
                    rdr.Close();
                }

                if (higherId != 0)
                {
                    SetTaxis(tableStyleId, higherTaxis);
                    SetTaxis(higherId, styleInfo.Taxis);
                }
            }
        }

        public void TaxisDown(int tableStyleId)
        {
            var styleInfo = GetTableStyleInfo(tableStyleId);
            if (styleInfo != null)
            {
                //var sqlString = "SELECT TOP 1 TableStyleID, Taxis FROM bairong_TableStyle WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND Taxis < (SELECT Taxis FROM bairong_TableStyle WHERE TableStyleID = @TableStyleID) ORDER BY Taxis DESC";
                var sqlString = SqlUtils.GetTopSqlString("bairong_TableStyle", "TableStyleID, Taxis", "WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND Taxis < (SELECT Taxis FROM bairong_TableStyle WHERE TableStyleID = @TableStyleID) ORDER BY Taxis DESC", 1);
                var lowerId = 0;
                var lowerTaxis = 0;

                var parms = new IDataParameter[]
			    {
                    GetParameter(ParmRelatedIdentity, EDataType.Integer, styleInfo.RelatedIdentity),
                    GetParameter(ParmTableName, EDataType.VarChar, 50, styleInfo.TableName),
				    GetParameter(ParmTableStyleId, EDataType.Integer, tableStyleId)
			    };

                using (var rdr = ExecuteReader(sqlString, parms))
                {
                    if (rdr.Read())
                    {
                        lowerId = GetInt(rdr, 0);
                        lowerTaxis = GetInt(rdr, 1);
                    }
                    rdr.Close();
                }

                if (lowerId != 0)
                {
                    SetTaxis(tableStyleId, lowerTaxis);
                    SetTaxis(lowerId, styleInfo.Taxis);
                }
            }
        }

        private void SetTaxis(int tableStyleId, int taxis)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmTaxis, EDataType.Integer, taxis),
				GetParameter(ParmTableStyleId, EDataType.Integer, tableStyleId)
			};

            ExecuteNonQuery(SqlUpdateTableStyleTaxis, parms);
            TableStyleManager.IsChanged = true;
        }
    }
}
