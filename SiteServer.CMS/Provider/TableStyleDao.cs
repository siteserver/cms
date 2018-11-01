using System.Collections.Generic;
using System.Data;
using System.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class TableStyleDao : DataProviderBase
    {
        public override string TableName => "siteserver_TableStyle";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(TableStyleInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleInfo.RelatedIdentity),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleInfo.TableName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleInfo.AttributeName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleInfo.DisplayName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleInfo.HelpText),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleInfo.IsVisibleInList),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleInfo.InputType),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleInfo.DefaultValue),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleInfo.IsHorizontal),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleInfo.ExtendValues),
                DataType = DataType.Text
            }
        };

        private const string SqlSelectAllTableStyle = "SELECT Id, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisibleInList, InputType, DefaultValue, IsHorizontal, ExtendValues FROM siteserver_TableStyle ORDER BY Taxis DESC, Id DESC";

        private const string SqlUpdateTableStyle = "UPDATE siteserver_TableStyle SET AttributeName = @AttributeName, Taxis = @Taxis, DisplayName = @DisplayName, HelpText = @HelpText, IsVisibleInList = @IsVisibleInList, InputType = @InputType, DefaultValue = @DefaultValue, IsHorizontal = @IsHorizontal, ExtendValues = @ExtendValues WHERE Id = @Id";

        private const string SqlDeleteTableStyle = "DELETE FROM siteserver_TableStyle WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND AttributeName = @AttributeName";

        private const string SqlInsertTableStyle = "INSERT INTO siteserver_TableStyle (RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisibleInList, InputType, DefaultValue, IsHorizontal, ExtendValues) VALUES (@RelatedIdentity, @TableName, @AttributeName, @Taxis, @DisplayName, @HelpText, @IsVisibleInList, @InputType, @DefaultValue, @IsHorizontal, @ExtendValues)";

        private const string ParmId = "@Id";
        private const string ParmRelatedIdentity = "@RelatedIdentity";
        private const string ParmTableName = "@TableName";
        private const string ParmAttributeName = "@AttributeName";
        private const string ParmTaxis = "@Taxis";
        private const string ParmDisplayName = "@DisplayName";
        private const string ParmHelpText = "@HelpText";
        private const string ParmIsVisibleInList = "@IsVisibleInList";
        private const string ParmInputType = "@InputType";
        private const string ParmDefaultValue = "@DefaultValue";
        private const string ParmIsHorizontal = "@IsHorizontal";
        private const string ParmExtendValues = "@ExtendValues";

        public int Insert(TableStyleInfo styleInfo)
        {
            int id;

            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmRelatedIdentity, DataType.Integer, styleInfo.RelatedIdentity),
                GetParameter(ParmTableName, DataType.VarChar, 50, styleInfo.TableName),
				GetParameter(ParmAttributeName, DataType.VarChar, 50, styleInfo.AttributeName),
                GetParameter(ParmTaxis, DataType.Integer, styleInfo.Taxis),
                GetParameter(ParmDisplayName, DataType.VarChar, 255, styleInfo.DisplayName),
                GetParameter(ParmHelpText, DataType.VarChar, 255, styleInfo.HelpText),
                GetParameter(ParmIsVisibleInList, DataType.VarChar, 18, styleInfo.IsVisibleInList.ToString()),
				GetParameter(ParmInputType, DataType.VarChar, 50, styleInfo.InputType.Value),
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
                        id = ExecuteNonQueryAndReturnId(TableName, nameof(TableStyleInfo.Id), trans, SqlInsertTableStyle, insertParms);

                        DataProvider.TableStyleItemDao.Insert(trans, id, styleInfo.StyleItems);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            TableStyleManager.ClearCache();

            return id;
        }

        public void Update(TableStyleInfo info, bool deleteAndInsertStyleItems = true)
        {
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmAttributeName, DataType.VarChar, 50, info.AttributeName),
                GetParameter(ParmTaxis, DataType.Integer, info.Taxis),
                GetParameter(ParmDisplayName, DataType.VarChar, 255, info.DisplayName),
                GetParameter(ParmHelpText, DataType.VarChar, 255, info.HelpText),
	            GetParameter(ParmIsVisibleInList, DataType.VarChar, 18, info.IsVisibleInList.ToString()),
				GetParameter(ParmInputType, DataType.VarChar, 50, info.InputType.Value),
                GetParameter(ParmDefaultValue, DataType.VarChar, 255, info.DefaultValue),
                GetParameter(ParmIsHorizontal, DataType.VarChar, 18, info.IsHorizontal.ToString()),
                GetParameter(ParmExtendValues, DataType.Text, info.Additional.ToString()),
                GetParameter(ParmId, DataType.Integer, info.Id)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(SqlUpdateTableStyle, updateParms);

                        if (deleteAndInsertStyleItems)
                        {
                            DataProvider.TableStyleItemDao.DeleteAndInsertStyleItems(trans, info.Id, info.StyleItems);
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

            TableStyleManager.ClearCache();
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

            TableStyleManager.ClearCache();
        }

        public void Delete(List<int> relatedIdentities, string tableName)
        {
            if (relatedIdentities == null || relatedIdentities.Count <= 0) return;

            var sqlString =
                $"DELETE FROM siteserver_TableStyle WHERE RelatedIdentity IN ({TranslateUtils.ToSqlInStringWithoutQuote(relatedIdentities)}) AND TableName = '{AttackUtils.FilterSql(tableName)}'";
            ExecuteNonQuery(sqlString);

            TableStyleManager.ClearCache();
        }

        private TableStyleInfo GetTableStyleInfoByReader(IDataReader rdr)
        {
            var i = 0;
            var id = GetInt(rdr, i++);
            var relatedIdentity = GetInt(rdr, i++);
            var tableName = GetString(rdr, i++);
            var attributeName = GetString(rdr, i++);
            var taxis = GetInt(rdr, i++);
            var displayName = GetString(rdr, i++);
            var helpText = GetString(rdr, i++);
            var isVisibleInList = GetBool(rdr, i++);
            var inputType = GetString(rdr, i++);
            var defaultValue = GetString(rdr, i++);
            var isHorizontal = GetBool(rdr, i++);
            var extendValues = GetString(rdr, i);

            var styleInfo = new TableStyleInfo(id, relatedIdentity, tableName, attributeName, taxis, displayName, helpText, isVisibleInList, InputTypeUtils.GetEnumType(inputType), defaultValue, isHorizontal, extendValues);

            return styleInfo;
        }

        public List<KeyValuePair<string, TableStyleInfo>> GetAllTableStyles()
        {
            var pairs = new List<KeyValuePair<string, TableStyleInfo>>();

            var allItemsDict = DataProvider.TableStyleItemDao.GetAllTableStyleItems();

            using (var rdr = ExecuteReader(SqlSelectAllTableStyle))
            {
                while (rdr.Read())
                {
                    var styleInfo = GetTableStyleInfoByReader(rdr);

                    allItemsDict.TryGetValue(styleInfo.Id, out var items);
                    styleInfo.StyleItems = items;

                    var key = TableStyleManager.GetKey(styleInfo.RelatedIdentity, styleInfo.TableName, styleInfo.AttributeName);

                    if (pairs.All(pair => pair.Key != key))
                    {
                        var pair = new KeyValuePair<string, TableStyleInfo>(key, styleInfo);
                        pairs.Add(pair);
                    }
                }
                rdr.Close();
            }

            return pairs;
        }
    }
}
