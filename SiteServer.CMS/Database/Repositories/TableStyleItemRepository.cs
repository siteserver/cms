using System.Collections.Generic;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Repositories
{
    public class TableStyleItemRepository : GenericRepository<TableStyleItemInfo>
    {
        private static class Attr
        {
            public const string TableStyleId = nameof(TableStyleItemInfo.TableStyleId);
        }

        public void Insert(int tableStyleId, List<TableStyleItemInfo> styleItems)
        {
            if (styleItems == null || styleItems.Count <= 0) return;

            //var sqlString =
            //    $"INSERT INTO {TableName} ({nameof(TableStyleItemInfo.TableStyleId)}, {nameof(TableStyleItemInfo.ItemTitle)}, {nameof(TableStyleItemInfo.ItemValue)}, {nameof(TableStyleItemInfo.IsSelected)}) VALUES (@{nameof(TableStyleItemInfo.TableStyleId)}, @{nameof(TableStyleItemInfo.ItemTitle)}, @{nameof(TableStyleItemInfo.ItemValue)}, @{nameof(TableStyleItemInfo.IsSelected)})";

            foreach (var itemInfo in styleItems)
            {
                //IDataParameter[] parameters =
                //{
                //    GetParameter(ParamTableStyleId, tableStyleId),
                //    GetParameter(ParamItemTitle, itemInfo.ItemTitle),
                //    GetParameter(ParamItemValue, itemInfo.ItemValue),
                //    GetParameter(ParamIsSelected, itemInfo.IsSelected.ToString())
                //};

                //DatabaseApi.ExecuteNonQuery(WebConfigUtils.ConnectionString, sqlString, parameters);
                itemInfo.TableStyleId = tableStyleId;
                base.InsertObject(itemInfo);
            }

            TableStyleManager.ClearCache();
        }

        public void DeleteAndInsertStyleItems(int tableStyleId, List<TableStyleItemInfo> styleItems)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamTableStyleId, tableStyleId)
            //};
            //var sqlString = $"DELETE FROM {TableName} WHERE {nameof(TableStyleItemInfo.TableStyleId)} = @{nameof(TableStyleItemInfo.TableStyleId)}";

            //DatabaseApi.ExecuteNonQuery(WebConfigUtils.ConnectionString, sqlString, parameters);

            DeleteAll(Q.Where(Attr.TableStyleId, tableStyleId));

            if (styleItems == null || styleItems.Count == 0) return;

            //sqlString =
            //    $"INSERT INTO {TableName} ({nameof(TableStyleItemInfo.TableStyleId)}, {nameof(TableStyleItemInfo.ItemTitle)}, {nameof(TableStyleItemInfo.ItemValue)}, {nameof(TableStyleItemInfo.IsSelected)}) VALUES (@{nameof(TableStyleItemInfo.TableStyleId)}, @{nameof(TableStyleItemInfo.ItemTitle)}, @{nameof(TableStyleItemInfo.ItemValue)}, @{nameof(TableStyleItemInfo.IsSelected)})";

            foreach (var itemInfo in styleItems)
            {
               // parameters = new[]
               //{
               //     GetParameter(ParamTableStyleId, itemInfo.TableStyleId),
               //     GetParameter(ParamItemTitle, itemInfo.ItemTitle),
               //     GetParameter(ParamItemValue, itemInfo.ItemValue),
               //     GetParameter(ParamIsSelected, itemInfo.IsSelected.ToString())
               // };

               // DatabaseApi.ExecuteNonQuery(WebConfigUtils.ConnectionString, sqlString, parameters);

                base.InsertObject(itemInfo);
            }

            TableStyleManager.ClearCache();
        }

        public Dictionary<int, List<TableStyleItemInfo>> GetAllTableStyleItems()
        {
            var allDict = new Dictionary<int, List<TableStyleItemInfo>>();

            var itemInfoList = GetObjectList();
            foreach (var itemInfo in itemInfoList)
            {
                allDict.TryGetValue(itemInfo.TableStyleId, out var list);

                if (list == null)
                {
                    list = new List<TableStyleItemInfo>();
                }

                list.Add(itemInfo);

                allDict[itemInfo.TableStyleId] = list;
            }

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, "SELECT Id, TableStyleID, ItemTitle, ItemValue, IsSelected FROM siteserver_TableStyleItem"))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var item = new TableStyleItemInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i)));

            //        allDict.TryGetValue(item.TableStyleId, out var list);

            //        if (list == null)
            //        {
            //            list = new List<TableStyleItemInfo>();
            //        }

            //        list.Add(item);

            //        allDict[item.TableStyleId] = list;
            //    }
            //    rdr.Close();
            //}

            return allDict;
        }
    }
}


//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Database.Caches;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class TableStyleItem : DataProviderBase
//    {
//        public override string TableName => "siteserver_TableStyleItem";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleItemInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleItemInfo.TableStyleId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleItemInfo.ItemTitle),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleItemInfo.ItemValue),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TableStyleItemInfo.IsSelected),
//                DataType = DataType.VarChar
//            }
//        };

//        private const string ParamTableStyleId = "@TableStyleID";
//        private const string ParamItemTitle = "@ItemTitle";
//        private const string ParamItemValue = "@ItemValue";
//        private const string ParamIsSelected = "@IsSelected";

//        public void InsertObject(IDbTransaction trans, int tableStyleId, List<TableStyleItemInfo> styleItems)
//        {
//            if (styleItems == null || styleItems.Count <= 0) return;

//            var sqlString =
//                $"INSERT INTO {TableName} ({nameof(TableStyleItemInfo.TableStyleId)}, {nameof(TableStyleItemInfo.ItemTitle)}, {nameof(TableStyleItemInfo.ItemValue)}, {nameof(TableStyleItemInfo.IsSelected)}) VALUES (@{nameof(TableStyleItemInfo.TableStyleId)}, @{nameof(TableStyleItemInfo.ItemTitle)}, @{nameof(TableStyleItemInfo.ItemValue)}, @{nameof(TableStyleItemInfo.IsSelected)})";

//            foreach (var itemInfo in styleItems)
//            {
//                IDataParameter[] parameters =
//                {
//                    GetParameter(ParamTableStyleId, tableStyleId),
//                    GetParameter(ParamItemTitle, itemInfo.ItemTitle),
//                    GetParameter(ParamItemValue, itemInfo.ItemValue),
//                    GetParameter(ParamIsSelected, itemInfo.IsSelected.ToString())
//                };

//                DatabaseApi.ExecuteNonQuery(trans, sqlString, parameters);
//            }

//            TableStyleManager.ClearCache();
//        }

//        public void DeleteAndInsertStyleItems(IDbTransaction trans, int tableStyleId, List<TableStyleItemInfo> styleItems)
//        {
//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamTableStyleId, tableStyleId)
//            };
//            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(TableStyleItemInfo.TableStyleId)} = @{nameof(TableStyleItemInfo.TableStyleId)}";

//            DatabaseApi.ExecuteNonQuery(trans, sqlString, parameters);

//            if (styleItems == null || styleItems.Count == 0) return;

//            sqlString =
//                $"INSERT INTO {TableName} ({nameof(TableStyleItemInfo.TableStyleId)}, {nameof(TableStyleItemInfo.ItemTitle)}, {nameof(TableStyleItemInfo.ItemValue)}, {nameof(TableStyleItemInfo.IsSelected)}) VALUES (@{nameof(TableStyleItemInfo.TableStyleId)}, @{nameof(TableStyleItemInfo.ItemTitle)}, @{nameof(TableStyleItemInfo.ItemValue)}, @{nameof(TableStyleItemInfo.IsSelected)})";

//            foreach (var itemInfo in styleItems)
//            {
//                 parameters = new []
//                {
//                    GetParameter(ParamTableStyleId, itemInfo.TableStyleId),
//                    GetParameter(ParamItemTitle, itemInfo.ItemTitle),
//                    GetParameter(ParamItemValue, itemInfo.ItemValue),
//                    GetParameter(ParamIsSelected, itemInfo.IsSelected.ToString())
//                };

//                DatabaseApi.ExecuteNonQuery(trans, sqlString, parameters);
//            }

//            TableStyleManager.ClearCache();
//        }

//        public Dictionary<int, List<TableStyleItemInfo>> GetAllTableStyleItems()
//        {
//            var allDict = new Dictionary<int, List<TableStyleItemInfo>>();

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, "SELECT Id, TableStyleID, ItemTitle, ItemValue, IsSelected FROM siteserver_TableStyleItem"))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var item = new TableStyleItemInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i)));

//                    allDict.TryGetValue(item.TableStyleId, out var list);

//                    if (list == null)
//                    {
//                        list = new List<TableStyleItemInfo>();
//                    }

//                    list.Add(item);

//                    allDict[item.TableStyleId] = list;
//                }
//                rdr.Close();
//            }

//            return allDict;
//        }
//    }
//}
