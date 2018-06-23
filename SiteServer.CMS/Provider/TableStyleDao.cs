using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
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

        private const string SqlSelectTableStyle = "SELECT Id, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisibleInList, InputType, DefaultValue, IsHorizontal, ExtendValues FROM siteserver_TableStyle WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND AttributeName = @AttributeName";

        private const string SqlSelectId = "SELECT Id FROM siteserver_TableStyle WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND AttributeName = @AttributeName";

        private const string SqlSelectTableStyleById = "SELECT Id, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisibleInList, InputType, DefaultValue, IsHorizontal, ExtendValues FROM siteserver_TableStyle WHERE Id = @Id";

        //private const string SqlSelectTableStyles = "SELECT Id, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisibleInList, InputType, DefaultValue, IsHorizontal, ExtendValues FROM siteserver_TableStyle WHERE TableName = @TableName AND AttributeName = @AttributeName ORDER BY RelatedIdentity";

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

            return id;
        }

        public void InsertWithTransaction(TableStyleInfo styleInfo, IDbTransaction trans)
        {
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

            if (styleInfo.StyleItems == null || styleInfo.StyleItems.Count == 0)
            {
                ExecuteNonQuery(trans, SqlInsertTableStyle, insertParms);
            }
            else
            {
                var id = ExecuteNonQueryAndReturnId(TableName, nameof(TableStyleInfo.Id), trans, SqlInsertTableStyle, insertParms);

                DataProvider.TableStyleItemDao.Insert(trans, id, styleInfo.StyleItems);
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
	            GetParameter(ParmIsVisibleInList, DataType.VarChar, 18, info.IsVisibleInList.ToString()),
				GetParameter(ParmInputType, DataType.VarChar, 50, info.InputType.Value),
                GetParameter(ParmDefaultValue, DataType.VarChar, 255, info.DefaultValue),
                GetParameter(ParmIsHorizontal, DataType.VarChar, 18, info.IsHorizontal.ToString()),
                GetParameter(ParmExtendValues, DataType.Text, info.Additional.ToString()),
                GetParameter(ParmId, DataType.Integer, info.Id)
			};

            ExecuteNonQuery(SqlUpdateTableStyle, updateParms);
        }

        public void Delete(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            const string sqlString = "DELETE FROM siteserver_TableStyle WHERE TableName = @TableName";

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
            if (relatedIdentities == null || relatedIdentities.Count <= 0) return;

            string sqlString =
                $"DELETE FROM siteserver_TableStyle WHERE RelatedIdentity IN ({TranslateUtils.ToSqlInStringWithoutQuote(relatedIdentities)}) AND TableName = '{PageUtils.FilterSql(tableName)}'";
            ExecuteNonQuery(sqlString);
            TableStyleManager.IsChanged = true;
        }

        public List<TableStyleInfo> GetTableStyleInfoList(List<int> relatedIdentities, string tableName)
        {
            var list = new List<TableStyleInfo>();

            string sqlString =
                $"SELECT Id, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisibleInList, InputType, DefaultValue, IsHorizontal, ExtendValues FROM siteserver_TableStyle WHERE RelatedIdentity IN ({TranslateUtils.ToSqlInStringWithoutQuote(relatedIdentities)}) AND TableName = '{PageUtils.FilterSql(tableName)}' ORDER BY Id DESC";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetTableStyleInfoByReader(rdr));
                }
                rdr.Close();
            }

            return list;
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

            using (var rdr = ExecuteReader(SqlSelectId, parms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public TableStyleInfo GetTableStyleInfo(int id)
        {
            TableStyleInfo styleInfo = null;

            var parms = new IDataParameter[]
			{
                GetParameter(ParmId, DataType.Integer, id)
			};

            using (var rdr = ExecuteReader(SqlSelectTableStyleById, parms))
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

        public PairList GetAllTableStyleInfoPairs()
        {
            var pairs = new PairList();

            using (var rdr = ExecuteReader(SqlSelectAllTableStyle))
            {
                while (rdr.Read())
                {
                    var styleInfo = GetTableStyleInfoByReader(rdr);
                    var inputType = styleInfo.InputType;
                    if (InputTypeUtils.IsWithStyleItems(inputType))
                    {
                        styleInfo.StyleItems = DataProvider.TableStyleItemDao.GetStyleItemInfoList(styleInfo.Id);
                    }

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

   //     public List<TableStyleInfo> GetTableStyleInfoWithItemsList(string tableName, string attributeName)
   //     {
   //         var list = new List<TableStyleInfo>();

   //         var parms = new IDataParameter[]
			//{
			//	GetParameter(ParmTableName, DataType.VarChar, 50, tableName),
   //             GetParameter(ParmAttributeName, DataType.VarChar, 50, attributeName)
			//};

   //         using (var rdr = ExecuteReader(SqlSelectTableStyles, parms))
   //         {
   //             while (rdr.Read())
   //             {
   //                 var styleInfo = GetTableStyleInfoByReader(rdr);
   //                 if (InputTypeUtils.Equals(styleInfo.InputType, InputType.CheckBox) || InputTypeUtils.Equals(styleInfo.InputType, InputType.Radio) || InputTypeUtils.Equals(styleInfo.InputType, InputType.SelectMultiple) || InputTypeUtils.Equals(styleInfo.InputType, InputType.SelectOne))
   //                 {
   //                     var styleItems = DataProvider.TableStyleItemDao.GetStyleItemInfoList(styleInfo.Id);
   //                     if (styleItems != null && styleItems.Count > 0)
   //                     {
   //                         styleInfo.StyleItems = styleItems;
   //                     }
   //                 }
   //                 list.Add(styleInfo);
   //             }
   //             rdr.Close();
   //         }

   //         return list;
   //     }
    }
}
