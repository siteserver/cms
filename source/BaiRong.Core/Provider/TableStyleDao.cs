using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Collection;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Provider
{
    public class TableStyleDao : DataProviderBase
    {
        public override string TableName => "bairong_TableStyle";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(TableStyleInfo.TableStyleId),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableStyleInfo.RelatedIdentity),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableStyleInfo.TableName),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableStyleInfo.AttributeName),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableStyleInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableStyleInfo.DisplayName),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableStyleInfo.HelpText),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableStyleInfo.IsVisible),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableStyleInfo.IsVisibleInList),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableStyleInfo.IsSingleLine),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableStyleInfo.InputType),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableStyleInfo.DefaultValue),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableStyleInfo.IsHorizontal),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableStyleInfo.ExtendValues),
                DataType = DataType.Text
            }
        };

        private const string SqlSelectTableStyle = "SELECT TableStyleID, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisible, IsVisibleInList, IsSingleLine, InputType, DefaultValue, IsHorizontal, ExtendValues FROM bairong_TableStyle WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND AttributeName = @AttributeName";

        private const string SqlSelectTableStyleId = "SELECT TableStyleID FROM bairong_TableStyle WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND AttributeName = @AttributeName";

        private const string SqlSelectTableStyleByTableStyleId = "SELECT TableStyleID, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisible, IsVisibleInList, IsSingleLine, InputType, DefaultValue, IsHorizontal, ExtendValues FROM bairong_TableStyle WHERE TableStyleID = @TableStyleID";

        private const string SqlSelectTableStyles = "SELECT TableStyleID, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisible, IsVisibleInList, IsSingleLine, InputType, DefaultValue, IsHorizontal, ExtendValues FROM bairong_TableStyle WHERE TableName = @TableName AND AttributeName = @AttributeName ORDER BY RelatedIdentity";

        private const string SqlSelectAllTableStyle = "SELECT TableStyleID, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisible, IsVisibleInList, IsSingleLine, InputType, DefaultValue, IsHorizontal, ExtendValues FROM bairong_TableStyle WHERE TableName <> '' AND AttributeName <> '' ORDER BY Taxis DESC, TableStyleID DESC";

        private const string SqlUpdateTableStyle = "UPDATE bairong_TableStyle SET AttributeName = @AttributeName, Taxis = @Taxis, DisplayName = @DisplayName, HelpText = @HelpText, IsVisible = @IsVisible, IsVisibleInList = @IsVisibleInList, IsSingleLine = @IsSingleLine, InputType = @InputType, DefaultValue = @DefaultValue, IsHorizontal = @IsHorizontal, ExtendValues = @ExtendValues WHERE TableStyleID = @TableStyleID";

        private const string SqlDeleteTableStyle = "DELETE FROM bairong_TableStyle WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND AttributeName = @AttributeName";

        private const string SqlUpdateTableStyleTaxis = "UPDATE bairong_TableStyle SET Taxis = @Taxis WHERE TableStyleID = @TableStyleID";

        private const string SqlInsertTableStyle = "INSERT INTO bairong_TableStyle (RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisible, IsVisibleInList, IsSingleLine, InputType, DefaultValue, IsHorizontal, ExtendValues) VALUES (@RelatedIdentity, @TableName, @AttributeName, @Taxis, @DisplayName, @HelpText, @IsVisible, @IsVisibleInList, @IsSingleLine, @InputType, @DefaultValue, @IsHorizontal, @ExtendValues)";

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

        public int Insert(TableStyleInfo styleInfo, ETableStyle tableStyle)
        {
            return Insert(styleInfo, tableStyle, false);
        }

        public int InsertWithTaxis(TableStyleInfo styleInfo, ETableStyle tableStyle)
        {
            return Insert(styleInfo, tableStyle, true);
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
				GetParameter(ParmRelatedIdentity, DataType.Integer, styleInfo.RelatedIdentity),
                GetParameter(ParmTableName, DataType.VarChar, 50, styleInfo.TableName),
				GetParameter(ParmAttributeName, DataType.VarChar, 50, styleInfo.AttributeName),
                GetParameter(ParmTaxis, DataType.Integer, styleInfo.Taxis),
                GetParameter(ParmDisplayName, DataType.VarChar, 255, styleInfo.DisplayName),
                GetParameter(ParmHelpText, DataType.VarChar, 255, styleInfo.HelpText),
                GetParameter(ParmIsVisible, DataType.VarChar, 18, styleInfo.IsVisible.ToString()),
                GetParameter(ParmIsVisibleInList, DataType.VarChar, 18, styleInfo.IsVisibleInList.ToString()),
                GetParameter(ParmIsSingleLine, DataType.VarChar, 18, styleInfo.IsSingleLine.ToString()),
				GetParameter(ParmInputType, DataType.VarChar, 50, styleInfo.InputType),
                GetParameter(ParmDefaultValue, DataType.VarChar, 255, styleInfo.DefaultValue),
                GetParameter(ParmIsHorizontal, DataType.VarChar, 18, styleInfo.IsHorizontal.ToString()),
                GetParameter(ParmExtendValues, DataType.Text, styleInfo.Additional.ToString())
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        tableStyleId = ExecuteNonQueryAndReturnId(TableName, nameof(TableStyleInfo.TableStyleId), trans, SqlInsertTableStyle, insertParms);

                        BaiRongDataProvider.TableStyleItemDao.Insert(trans, tableStyleId, styleInfo.StyleItems);

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
                GetParameter(ParmRelatedIdentity, DataType.Integer, styleInfo.RelatedIdentity),
                GetParameter(ParmTableName, DataType.VarChar, 50, styleInfo.TableName),
			    GetParameter(ParmAttributeName, DataType.VarChar, 50, styleInfo.AttributeName),
                GetParameter(ParmTaxis, DataType.Integer, styleInfo.Taxis),
                GetParameter(ParmDisplayName, DataType.VarChar, 255, styleInfo.DisplayName),
                GetParameter(ParmHelpText, DataType.VarChar, 255, styleInfo.HelpText),
                GetParameter(ParmIsVisible, DataType.VarChar, 18, styleInfo.IsVisible.ToString()),
                GetParameter(ParmIsVisibleInList, DataType.VarChar, 18, styleInfo.IsVisibleInList.ToString()),
                GetParameter(ParmIsSingleLine, DataType.VarChar, 18, styleInfo.IsSingleLine.ToString()),
			    GetParameter(ParmInputType, DataType.VarChar, 50, styleInfo.InputType),
                GetParameter(ParmDefaultValue, DataType.VarChar, 255, styleInfo.DefaultValue),
                GetParameter(ParmIsHorizontal, DataType.VarChar, 18, styleInfo.IsHorizontal.ToString()),
                GetParameter(ParmExtendValues, DataType.Text, styleInfo.Additional.ToString())
		    };

            if (styleInfo.StyleItems == null || styleInfo.StyleItems.Count == 0)
            {
                ExecuteNonQuery(trans, SqlInsertTableStyle, insertParms);
            }
            else
            {
                var tableStyleId = ExecuteNonQueryAndReturnId(TableName, nameof(TableStyleInfo.TableStyleId), trans, SqlInsertTableStyle, insertParms);

                BaiRongDataProvider.TableStyleItemDao.Insert(trans, tableStyleId, styleInfo.StyleItems);
            }
        }

        public void Update(TableStyleInfo info)
        {
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmAttributeName, DataType.VarChar, 50, info.AttributeName),
                GetParameter(ParmTaxis, DataType.Integer, info.Taxis),
                GetParameter(ParmDisplayName, DataType.VarChar, 255, info.DisplayName),
                GetParameter(ParmHelpText, DataType.VarChar, 255, info.HelpText),
                GetParameter(ParmIsVisible, DataType.VarChar, 18, info.IsVisible.ToString()),
	            GetParameter(ParmIsVisibleInList, DataType.VarChar, 18, info.IsVisibleInList.ToString()),
                GetParameter(ParmIsSingleLine, DataType.VarChar, 18, info.IsSingleLine.ToString()),
				GetParameter(ParmInputType, DataType.VarChar, 50, info.InputType),
                GetParameter(ParmDefaultValue, DataType.VarChar, 255, info.DefaultValue),
                GetParameter(ParmIsHorizontal, DataType.VarChar, 18, info.IsHorizontal.ToString()),
                GetParameter(ParmExtendValues, DataType.Text, info.Additional.ToString()),
                GetParameter(ParmTableStyleId, DataType.Integer, info.TableStyleId)
			};

            ExecuteNonQuery(SqlUpdateTableStyle, updateParms);
        }

        public void Delete(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            const string sqlString = "DELETE FROM bairong_TableStyle WHERE TableName = @TableName";

            var parameters = new IDataParameter[]
            {
                GetParameter(ParmTableName, DataType.VarChar, 50, tableName)
            };

            ExecuteNonQuery(sqlString, parameters);

            TableStyleManager.IsChanged = true;
        }

        public void Delete(int relatedIdentity, string tableName, string attributeName)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmRelatedIdentity, DataType.Integer, relatedIdentity),
                GetParameter(ParmTableName, DataType.VarChar, 50, tableName),
                GetParameter(ParmAttributeName, DataType.VarChar, 50, attributeName)
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
                GetParameter(ParmRelatedIdentity, DataType.Integer, relatedIdentity),
                GetParameter(ParmTableName, DataType.VarChar, 50, tableName),
				GetParameter(ParmAttributeName, DataType.VarChar, 50, attributeName)
			};

            using (var rdr = ExecuteReader(SqlSelectTableStyleId, parms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
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
                GetParameter(ParmTableStyleId, DataType.Integer, tableStyleId)
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
                GetParameter(ParmRelatedIdentity, DataType.Integer, relatedIdentity),
                GetParameter(ParmTableName, DataType.VarChar, 50, tableName),
				GetParameter(ParmAttributeName, DataType.VarChar, 50, attributeName)
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
				GetParameter(ParmTableName, DataType.VarChar, 50, tableName),
                GetParameter(ParmAttributeName, DataType.VarChar, 50, attributeName)
			};

            using (var rdr = ExecuteReader(SqlSelectTableStyles, parms))
            {
                while (rdr.Read())
                {
                    var styleInfo = GetTableStyleInfoByReader(rdr);
                    if (InputTypeUtils.Equals(styleInfo.InputType, InputType.CheckBox) || InputTypeUtils.Equals(styleInfo.InputType, InputType.Radio) || InputTypeUtils.Equals(styleInfo.InputType, InputType.SelectMultiple) || InputTypeUtils.Equals(styleInfo.InputType, InputType.SelectOne))
                    {
                        var styleItems = BaiRongDataProvider.TableStyleItemDao.GetStyleItemInfoList(styleInfo.TableStyleId);
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
                var sqlString = SqlUtils.GetTopSqlString("bairong_TableStyle", "TableStyleID, Taxis",
                    "WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND Taxis > (SELECT Taxis FROM bairong_TableStyle WHERE TableStyleID = @TableStyleID)",
                    "ORDER BY Taxis", 1);

                var higherId = 0;
                var higherTaxis = 0;

                var parms = new IDataParameter[]
			    {
                    GetParameter(ParmRelatedIdentity, DataType.Integer, styleInfo.RelatedIdentity),
                    GetParameter(ParmTableName, DataType.VarChar, 50, styleInfo.TableName),
				    GetParameter(ParmTableStyleId, DataType.Integer, tableStyleId)
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
                var sqlString = SqlUtils.GetTopSqlString("bairong_TableStyle", "TableStyleID, Taxis",
                    "WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND Taxis < (SELECT Taxis FROM bairong_TableStyle WHERE TableStyleID = @TableStyleID)",
                    "ORDER BY Taxis DESC", 1);
                var lowerId = 0;
                var lowerTaxis = 0;

                var parms = new IDataParameter[]
			    {
                    GetParameter(ParmRelatedIdentity, DataType.Integer, styleInfo.RelatedIdentity),
                    GetParameter(ParmTableName, DataType.VarChar, 50, styleInfo.TableName),
				    GetParameter(ParmTableStyleId, DataType.Integer, tableStyleId)
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
				GetParameter(ParmTaxis, DataType.Integer, taxis),
				GetParameter(ParmTableStyleId, DataType.Integer, tableStyleId)
			};

            ExecuteNonQuery(SqlUpdateTableStyleTaxis, parms);
            TableStyleManager.IsChanged = true;
        }
    }
}
