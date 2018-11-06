using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Provider
{
    public class TableStyleItemDao : DataProviderBase
    {
        public override string TableName => "siteserver_TableStyleItem";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(TableStyleItemInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleItemInfo.TableStyleId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleItemInfo.ItemTitle),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleItemInfo.ItemValue),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(TableStyleItemInfo.IsSelected),
                DataType = DataType.VarChar,
                DataLength = 18
            }
        };

        private const string ParmTableStyleId = "@TableStyleID";
        private const string ParmItemTitle = "@ItemTitle";
        private const string ParmItemValue = "@ItemValue";
        private const string ParmIsSelected = "@IsSelected";

        public void Insert(IDbTransaction trans, int tableStyleId, List<TableStyleItemInfo> styleItems)
        {
            if (styleItems == null || styleItems.Count <= 0) return;

            var sqlString =
                $"INSERT INTO {TableName} ({nameof(TableStyleItemInfo.TableStyleId)}, {nameof(TableStyleItemInfo.ItemTitle)}, {nameof(TableStyleItemInfo.ItemValue)}, {nameof(TableStyleItemInfo.IsSelected)}) VALUES (@{nameof(TableStyleItemInfo.TableStyleId)}, @{nameof(TableStyleItemInfo.ItemTitle)}, @{nameof(TableStyleItemInfo.ItemValue)}, @{nameof(TableStyleItemInfo.IsSelected)})";

            foreach (var itemInfo in styleItems)
            {
                var insertItemParms = new IDataParameter[]
                {
                    GetParameter(ParmTableStyleId, DataType.Integer, tableStyleId),
                    GetParameter(ParmItemTitle, DataType.VarChar, 255, itemInfo.ItemTitle),
                    GetParameter(ParmItemValue, DataType.VarChar, 255, itemInfo.ItemValue),
                    GetParameter(ParmIsSelected, DataType.VarChar, 18, itemInfo.IsSelected.ToString())
                };

                ExecuteNonQuery(trans, sqlString, insertItemParms);
            }

            TableStyleManager.ClearCache();
        }

        public void DeleteAndInsertStyleItems(IDbTransaction trans, int tableStyleId, List<TableStyleItemInfo> styleItems)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmTableStyleId, DataType.Integer, tableStyleId)
            };
            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(TableStyleItemInfo.TableStyleId)} = @{nameof(TableStyleItemInfo.TableStyleId)}";

            ExecuteNonQuery(trans, sqlString, parms);

            if (styleItems == null || styleItems.Count == 0) return;

            sqlString =
                $"INSERT INTO {TableName} ({nameof(TableStyleItemInfo.TableStyleId)}, {nameof(TableStyleItemInfo.ItemTitle)}, {nameof(TableStyleItemInfo.ItemValue)}, {nameof(TableStyleItemInfo.IsSelected)}) VALUES (@{nameof(TableStyleItemInfo.TableStyleId)}, @{nameof(TableStyleItemInfo.ItemTitle)}, @{nameof(TableStyleItemInfo.ItemValue)}, @{nameof(TableStyleItemInfo.IsSelected)})";

            foreach (var itemInfo in styleItems)
            {
                var insertItemParms = new IDataParameter[]
                {
                    GetParameter(ParmTableStyleId, DataType.Integer, itemInfo.TableStyleId),
                    GetParameter(ParmItemTitle, DataType.VarChar, 255, itemInfo.ItemTitle),
                    GetParameter(ParmItemValue, DataType.VarChar, 255, itemInfo.ItemValue),
                    GetParameter(ParmIsSelected, DataType.VarChar, 18, itemInfo.IsSelected.ToString())
                };

                ExecuteNonQuery(trans, sqlString, insertItemParms);
            }

            TableStyleManager.ClearCache();
        }

        public Dictionary<int, List<TableStyleItemInfo>> GetAllTableStyleItems()
        {
            var allDict = new Dictionary<int, List<TableStyleItemInfo>>();

            using (var rdr = ExecuteReader("SELECT Id, TableStyleID, ItemTitle, ItemValue, IsSelected FROM siteserver_TableStyleItem"))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var item = new TableStyleItemInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetBool(rdr, i));

                    allDict.TryGetValue(item.TableStyleId, out var list);

                    if (list == null)
                    {
                        list = new List<TableStyleItemInfo>();
                    }

                    list.Add(item);

                    allDict[item.TableStyleId] = list;
                }
                rdr.Close();
            }

            return allDict;
        }
    }
}
