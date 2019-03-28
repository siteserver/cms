using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Repositories
{
    public class TableStyleRepository : GenericRepository<TableStyleInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(TableStyleInfo.Id);
            public const string RelatedIdentity = nameof(TableStyleInfo.RelatedIdentity);
            public const string TableName = nameof(TableStyleInfo.TableName);
            public const string AttributeName = nameof(TableStyleInfo.AttributeName);
            public const string Taxis = nameof(TableStyleInfo.Taxis);
        }

        public int Insert(TableStyleInfo styleInfo)
        {
            //int id;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamRelatedIdentity, styleInfo.RelatedIdentity),
            //    GetParameter(ParamTableName, styleInfo.TableName),
            //    GetParameter(ParamAttributeName, styleInfo.AttributeName),
            //    GetParameter(ParamTaxis, styleInfo.Taxis),
            //    GetParameter(ParamDisplayName, styleInfo.DisplayName),
            //    GetParameter(ParamHelpText, styleInfo.HelpText),
            //    GetParameter(ParamIsVisibleInList, styleInfo.IsVisibleInList.ToString()),
            //    GetParameter(ParamInputType, styleInfo.InputType.Value),
            //    GetParameter(ParamDefaultValue, styleInfo.DefaultValue),
            //    GetParameter(ParamIsHorizontal, styleInfo.IsHorizontal.ToString()),
            //    GetParameter(ParamExtendValues,styleInfo.ToString())
            //};

            //using (var conn = GetConnection())
            //{
            //    conn.Open();
            //    using (var trans = conn.BeginTransaction())
            //    {
            //        try
            //        {
            //            string SqlInsertTableStyle = "INSERT INTO siteserver_TableStyle (RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisibleInList, InputType, DefaultValue, IsHorizontal, ExtendValues) VALUES (@RelatedIdentity, @TableName, @AttributeName, @Taxis, @DisplayName, @HelpText, @IsVisibleInList, @InputType, @DefaultValue, @IsHorizontal, @ExtendValues)";
            //            id = DatabaseApi.ExecuteNonQueryAndReturnId(TableName, nameof(TableStyleInfo.Id), trans, SqlInsertTableStyle, parameters);

            //            DataProvider.TableStyleItem.InsertObject(trans, id, styleInfo.StyleItems);

            //            trans.Commit();
            //        }
            //        catch
            //        {
            //            trans.Rollback();
            //            throw;
            //        }
            //    }
            //}

            var id = InsertObject(styleInfo);
            DataProvider.TableStyleItem.Insert(id, styleInfo.StyleItems);

            TableStyleManager.ClearCache();

            return id;
        }

        public void Update(TableStyleInfo info, bool deleteAndInsertStyleItems = true)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamAttributeName, info.AttributeName),
            //    GetParameter(ParamTaxis, info.Taxis),
            //    GetParameter(ParamDisplayName, info.DisplayName),
            //    GetParameter(ParamHelpText, info.HelpText),
            //    GetParameter(ParamIsVisibleInList, info.IsVisibleInList.ToString()),
            //    GetParameter(ParamInputType, info.InputType.Value),
            //    GetParameter(ParamDefaultValue, info.DefaultValue),
            //    GetParameter(ParamIsHorizontal, info.IsHorizontal.ToString()),
            //    GetParameter(ParamExtendValues,info.ToString()),
            //    GetParameter(ParamId, info.Id)
            //};

            //using (var conn = GetConnection())
            //{
            //    conn.Open();
            //    using (var trans = conn.BeginTransaction())
            //    {
            //        try
            //        {
            //            string SqlUpdateTableStyle = "UPDATE siteserver_TableStyle SET AttributeName = @AttributeName, Taxis = @Taxis, DisplayName = @DisplayName, HelpText = @HelpText, IsVisibleInList = @IsVisibleInList, InputType = @InputType, DefaultValue = @DefaultValue, IsHorizontal = @IsHorizontal, ExtendValues = @ExtendValues WHERE Id = @Id";
            //            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdateTableStyle, parameters);

            //            if (deleteAndInsertStyleItems)
            //            {
            //                DataProvider.TableStyleItem.DeleteAndInsertStyleItems(trans, info.Id, info.StyleItems);
            //            }

            //            trans.Commit();
            //        }
            //        catch
            //        {
            //            trans.Rollback();
            //            throw;
            //        }
            //    }
            //}

            UpdateObject(info);
            if (deleteAndInsertStyleItems)
            {
                DataProvider.TableStyleItem.DeleteAndInsertStyleItems(info.Id, info.StyleItems);
            }

            TableStyleManager.ClearCache();
        }

        public void Delete(int relatedIdentity, string tableName, string attributeName)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamRelatedIdentity, relatedIdentity),
            //    GetParameter(ParamTableName, tableName),
            //    GetParameter(ParamAttributeName, attributeName)
            //};
            //string SqlDeleteTableStyle = "DELETE FROM siteserver_TableStyle WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND AttributeName = @AttributeName";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDeleteTableStyle, parameters);

            DeleteAll(Q
                .Where(Attr.RelatedIdentity, relatedIdentity)
                .Where(Attr.TableName, tableName)
                .Where(Attr.AttributeName, attributeName));

            TableStyleManager.ClearCache();
        }

        public void Delete(List<int> relatedIdentities, string tableName)
        {
            if (relatedIdentities == null || relatedIdentities.Count <= 0) return;

            //var sqlString =
            //    $"DELETE FROM siteserver_TableStyle WHERE RelatedIdentity IN ({TranslateUtils.ToSqlInStringWithoutQuote(relatedIdentities)}) AND TableName = '{AttackUtils.FilterSql(tableName)}'";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            DeleteAll(Q
                .WhereIn(Attr.RelatedIdentity, relatedIdentities)
                .Where(Attr.TableName, tableName));

            TableStyleManager.ClearCache();
        }

        //private TableStyleInfo GetTableStyleInfoByReader(IDataReader rdr)
        //{
        //    var i = 0;
        //    var id = DatabaseApi.GetInt(rdr, i++);
        //    var relatedIdentity = DatabaseApi.GetInt(rdr, i++);
        //    var tableName = DatabaseApi.GetString(rdr, i++);
        //    var attributeName = DatabaseApi.GetString(rdr, i++);
        //    var taxis = DatabaseApi.GetInt(rdr, i++);
        //    var displayName = DatabaseApi.GetString(rdr, i++);
        //    var helpText = DatabaseApi.GetString(rdr, i++);
        //    var isVisibleInList = TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++));
        //    var inputType = DatabaseApi.GetString(rdr, i++);
        //    var defaultValue = DatabaseApi.GetString(rdr, i++);
        //    var isHorizontal = TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++));
        //    var extendValues = DatabaseApi.GetString(rdr, i);

        //    var styleInfo = new TableStyleInfo(id, relatedIdentity, tableName, attributeName, taxis, displayName, helpText, isVisibleInList, InputTypeUtils.GetEnumType(inputType), defaultValue, isHorizontal, extendValues);

        //    return styleInfo;
        //}

        public List<KeyValuePair<string, TableStyleInfo>> GetAllTableStyles()
        {
            var pairs = new List<KeyValuePair<string, TableStyleInfo>>();

            var allItemsDict = DataProvider.TableStyleItem.GetAllTableStyleItems();

            var styleInfoList = GetObjectList(Q.OrderByDesc(Attr.Taxis, Attr.Id));
            foreach (var styleInfo in styleInfoList)
            {
                allItemsDict.TryGetValue(styleInfo.Id, out var items);
                styleInfo.StyleItems = items;

                var key = TableStyleManager.GetKey(styleInfo.RelatedIdentity, styleInfo.TableName, styleInfo.AttributeName);

                if (pairs.All(pair => pair.Key != key))
                {
                    var pair = new KeyValuePair<string, TableStyleInfo>(key, styleInfo);
                    pairs.Add(pair);
                }
            }

            //string SqlSelectAllTableStyle = "SELECT Id, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisibleInList, InputType, DefaultValue, IsHorizontal, ExtendValues FROM siteserver_TableStyle ORDER BY Taxis DESC, Id DESC";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAllTableStyle))
            //{
            //    while (rdr.Read())
            //    {
            //        var styleInfo = GetTableStyleInfoByReader(rdr);

            //        allItemsDict.TryGetValue(styleInfo.Id, out var items);
            //        styleInfo.StyleItems = items;

            //        var key = TableStyleManager.GetKey(styleInfo.RelatedIdentity, styleInfo.TableName, styleInfo.AttributeName);

            //        if (pairs.All(pair => pair.Key != key))
            //        {
            //            var pair = new KeyValuePair<string, TableStyleInfo>(key, styleInfo);
            //            pairs.Add(pair);
            //        }
            //    }
            //    rdr.Close();
            //}

            return pairs;
        }
    }
}


//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using SiteServer.CMS.Core;
//using SiteServer.CMS.Database.Caches;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class TableStyle : DataProviderBase
//    {
//        public override string TableName => "siteserver_TableStyle";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleInfo.RelatedIdentity),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleInfo.TableName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleInfo.AttributeName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleInfo.Taxis),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleInfo.DisplayName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleInfo.HelpText),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleInfo.IsVisibleInList),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleInfo.InputType),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleInfo.DefaultValue),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleInfo.IsHorizontal),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleInfo.ExtendValues),
//                DataType = DataType.Text
//            }
//        };

//        private const string SqlSelectAllTableStyle = "SELECT Id, RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisibleInList, InputType, DefaultValue, IsHorizontal, ExtendValues FROM siteserver_TableStyle ORDER BY Taxis DESC, Id DESC";

//        private const string SqlUpdateTableStyle = "UPDATE siteserver_TableStyle SET AttributeName = @AttributeName, Taxis = @Taxis, DisplayName = @DisplayName, HelpText = @HelpText, IsVisibleInList = @IsVisibleInList, InputType = @InputType, DefaultValue = @DefaultValue, IsHorizontal = @IsHorizontal, ExtendValues = @ExtendValues WHERE Id = @Id";

//        private const string SqlDeleteTableStyle = "DELETE FROM siteserver_TableStyle WHERE RelatedIdentity = @RelatedIdentity AND TableName = @TableName AND AttributeName = @AttributeName";

//        private const string SqlInsertTableStyle = "INSERT INTO siteserver_TableStyle (RelatedIdentity, TableName, AttributeName, Taxis, DisplayName, HelpText, IsVisibleInList, InputType, DefaultValue, IsHorizontal, ExtendValues) VALUES (@RelatedIdentity, @TableName, @AttributeName, @Taxis, @DisplayName, @HelpText, @IsVisibleInList, @InputType, @DefaultValue, @IsHorizontal, @ExtendValues)";

//        private const string ParamId = "@Id";
//        private const string ParamRelatedIdentity = "@RelatedIdentity";
//        private const string ParamTableName = "@TableName";
//        private const string ParamAttributeName = "@AttributeName";
//        private const string ParamTaxis = "@Taxis";
//        private const string ParamDisplayName = "@DisplayName";
//        private const string ParamHelpText = "@HelpText";
//        private const string ParamIsVisibleInList = "@IsVisibleInList";
//        private const string ParamInputType = "@InputType";
//        private const string ParamDefaultValue = "@DefaultValue";
//        private const string ParamIsHorizontal = "@IsHorizontal";
//        private const string ParamExtendValues = "@ExtendValues";

//        public int InsertObject(TableStyleInfo styleInfo)
//        {
//            int id;

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamRelatedIdentity, styleInfo.RelatedIdentity),
//                GetParameter(ParamTableName, styleInfo.TableName),
//				GetParameter(ParamAttributeName, styleInfo.AttributeName),
//                GetParameter(ParamTaxis, styleInfo.Taxis),
//                GetParameter(ParamDisplayName, styleInfo.DisplayName),
//                GetParameter(ParamHelpText, styleInfo.HelpText),
//                GetParameter(ParamIsVisibleInList, styleInfo.IsVisibleInList.ToString()),
//				GetParameter(ParamInputType, styleInfo.InputType.Value),
//                GetParameter(ParamDefaultValue, styleInfo.DefaultValue),
//                GetParameter(ParamIsHorizontal, styleInfo.IsHorizontal.ToString()),
//                GetParameter(ParamExtendValues,styleInfo.ToString())
//			};

//            using (var conn = GetConnection())
//            {
//                conn.Open();
//                using (var trans = conn.BeginTransaction())
//                {
//                    try
//                    {
//                        id = DatabaseApi.ExecuteNonQueryAndReturnId(TableName, nameof(TableStyleInfo.Id), trans, SqlInsertTableStyle, parameters);

//                        DataProvider.TableStyleItem.InsertObject(trans, id, styleInfo.StyleItems);

//                        trans.Commit();
//                    }
//                    catch
//                    {
//                        trans.Rollback();
//                        throw;
//                    }
//                }
//            }

//            TableStyleManager.ClearCache();

//            return id;
//        }

//        public void UpdateObject(TableStyleInfo info, bool deleteAndInsertStyleItems = true)
//        {
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamAttributeName, info.AttributeName),
//                GetParameter(ParamTaxis, info.Taxis),
//                GetParameter(ParamDisplayName, info.DisplayName),
//                GetParameter(ParamHelpText, info.HelpText),
//	            GetParameter(ParamIsVisibleInList, info.IsVisibleInList.ToString()),
//				GetParameter(ParamInputType, info.InputType.Value),
//                GetParameter(ParamDefaultValue, info.DefaultValue),
//                GetParameter(ParamIsHorizontal, info.IsHorizontal.ToString()),
//                GetParameter(ParamExtendValues,info.ToString()),
//                GetParameter(ParamId, info.Id)
//			};

//            using (var conn = GetConnection())
//            {
//                conn.Open();
//                using (var trans = conn.BeginTransaction())
//                {
//                    try
//                    {
//                        DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdateTableStyle, parameters);

//                        if (deleteAndInsertStyleItems)
//                        {
//                            DataProvider.TableStyleItem.DeleteAndInsertStyleItems(trans, info.Id, info.StyleItems);
//                        }

//                        trans.Commit();
//                    }
//                    catch
//                    {
//                        trans.Rollback();
//                        throw;
//                    }
//                }
//            }

//            TableStyleManager.ClearCache();
//        }

//        public void DeleteById(int relatedIdentity, string tableName, string attributeName)
//        {
//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamRelatedIdentity, relatedIdentity),
//                GetParameter(ParamTableName, tableName),
//                GetParameter(ParamAttributeName, attributeName)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDeleteTableStyle, parameters);

//            TableStyleManager.ClearCache();
//        }

//        public void DeleteById(List<int> relatedIdentities, string tableName)
//        {
//            if (relatedIdentities == null || relatedIdentities.Count <= 0) return;

//            var sqlString =
//                $"DELETE FROM siteserver_TableStyle WHERE RelatedIdentity IN ({TranslateUtils.ToSqlInStringWithoutQuote(relatedIdentities)}) AND TableName = '{AttackUtils.FilterSql(tableName)}'";
//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

//            TableStyleManager.ClearCache();
//        }

//        private TableStyleInfo GetTableStyleInfoByReader(IDataReader rdr)
//        {
//            var i = 0;
//            var id = DatabaseApi.GetInt(rdr, i++);
//            var relatedIdentity = DatabaseApi.GetInt(rdr, i++);
//            var tableName = DatabaseApi.GetString(rdr, i++);
//            var attributeName = DatabaseApi.GetString(rdr, i++);
//            var taxis = DatabaseApi.GetInt(rdr, i++);
//            var displayName = DatabaseApi.GetString(rdr, i++);
//            var helpText = DatabaseApi.GetString(rdr, i++);
//            var isVisibleInList = TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++));
//            var inputType = DatabaseApi.GetString(rdr, i++);
//            var defaultValue = DatabaseApi.GetString(rdr, i++);
//            var isHorizontal = TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++));
//            var extendValues = DatabaseApi.GetString(rdr, i);

//            var styleInfo = new TableStyleInfo(id, relatedIdentity, tableName, attributeName, taxis, displayName, helpText, isVisibleInList, InputTypeUtils.GetEnumType(inputType), defaultValue, isHorizontal, extendValues);

//            return styleInfo;
//        }

//        public List<KeyValuePair<string, TableStyleInfo>> GetAllTableStyles()
//        {
//            var pairs = new List<KeyValuePair<string, TableStyleInfo>>();

//            var allItemsDict = DataProvider.TableStyleItem.GetAllTableStyleItems();

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAllTableStyle))
//            {
//                while (rdr.Read())
//                {
//                    var styleInfo = GetTableStyleInfoByReader(rdr);

//                    allItemsDict.TryGetValue(styleInfo.Id, out var items);
//                    styleInfo.StyleItems = items;

//                    var key = TableStyleManager.GetKey(styleInfo.RelatedIdentity, styleInfo.TableName, styleInfo.AttributeName);

//                    if (pairs.All(pair => pair.Key != key))
//                    {
//                        var pair = new KeyValuePair<string, TableStyleInfo>(key, styleInfo);
//                        pairs.Add(pair);
//                    }
//                }
//                rdr.Close();
//            }

//            return pairs;
//        }
//    }
//}
