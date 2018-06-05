using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Data;
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

        private const string SqlSelectAllStyleItem = "SELECT Id, TableStyleID, ItemTitle, ItemValue, IsSelected FROM siteserver_TableStyleItem WHERE (TableStyleID = @TableStyleID)";

        private const string SqlDeleteStyleItems = "DELETE FROM siteserver_TableStyleItem WHERE TableStyleID = @TableStyleID";

        private const string SqlInsertStyleItem = "INSERT INTO siteserver_TableStyleItem (TableStyleID, ItemTitle, ItemValue, IsSelected) VALUES (@TableStyleID, @ItemTitle, @ItemValue, @IsSelected)";

        private const string ParmTableStyleId = "@TableStyleID";
        private const string ParmItemTitle = "@ItemTitle";
        private const string ParmItemValue = "@ItemValue";
        private const string ParmIsSelected = "@IsSelected";

        public void Insert(IDbTransaction trans, int tableStyleId, List<TableStyleItemInfo> styleItems)
        {
            if (styleItems == null || styleItems.Count <= 0) return;

            foreach (var itemInfo in styleItems)
            {
                var insertItemParms = new IDataParameter[]
                {
                    GetParameter(ParmTableStyleId, DataType.Integer, tableStyleId),
                    GetParameter(ParmItemTitle, DataType.VarChar, 255, itemInfo.ItemTitle),
                    GetParameter(ParmItemValue, DataType.VarChar, 255, itemInfo.ItemValue),
                    GetParameter(ParmIsSelected, DataType.VarChar, 18, itemInfo.IsSelected.ToString())
                };

                ExecuteNonQuery(trans, SqlInsertStyleItem, insertItemParms);
            }
        }

        public void InsertStyleItems(List<TableStyleItemInfo> styleItems)
        {
            if (styleItems == null || styleItems.Count == 0) return;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var itemInfo in styleItems)
                        {
                            var insertItemParms = new IDataParameter[]
							{
								GetParameter(ParmTableStyleId, DataType.Integer, itemInfo.TableStyleId),
								GetParameter(ParmItemTitle, DataType.VarChar, 255, itemInfo.ItemTitle),
								GetParameter(ParmItemValue, DataType.VarChar, 255, itemInfo.ItemValue),
								GetParameter(ParmIsSelected, DataType.VarChar, 18, itemInfo.IsSelected.ToString())
							};

                            ExecuteNonQuery(trans, SqlInsertStyleItem, insertItemParms);

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
				GetParameter(ParmTableStyleId, DataType.Integer, tableStyleId)
			};

            ExecuteNonQuery(SqlDeleteStyleItems, parms);
        }

        public List<TableStyleItemInfo> GetStyleItemInfoList(int tableStyleId)
        {
            var styleItems = new List<TableStyleItemInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableStyleId, DataType.Integer, tableStyleId)
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
    }
}
