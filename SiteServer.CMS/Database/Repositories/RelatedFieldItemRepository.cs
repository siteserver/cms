using System.Collections.Generic;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Repositories
{
    public class RelatedFieldItemRepository : GenericRepository<RelatedFieldItemInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(RelatedFieldItemInfo.Id);
            public const string RelatedFieldId = nameof(RelatedFieldItemInfo.RelatedFieldId);
            public const string ParentId = nameof(RelatedFieldItemInfo.ParentId);
            public const string Taxis = nameof(RelatedFieldItemInfo.Taxis);
        }

        public int Insert(RelatedFieldItemInfo info)
        {
            info.Taxis = GetMaxTaxis(info.ParentId) + 1;

            //const string sqlString = "INSERT INTO siteserver_RelatedFieldItem (RelatedFieldID, ItemName, ItemValue, ParentID, Taxis) VALUES (@RelatedFieldID, @ItemName, @ItemValue, @ParentID, @Taxis)";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamRelatedFieldId, info.RelatedFieldId),
            //    GetParameter(ParamItemName, info.ItemName),
            //    GetParameter(ParamItemValue, info.ItemValue),
            //    GetParameter(ParamParentId, info.ParentId),
            //    GetParameter(ParamTaxis, info.Taxis)
            //};

            //return DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, nameof(RelatedFieldItemInfo.Id), sqlString, parameters);

            return InsertObject(info);

            //RelatedFieldManager.ClearCache();
        }

        public void Update(RelatedFieldItemInfo info)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamItemName, info.ItemName),
            //    GetParameter(ParamItemValue, info.ItemValue),
            //    GetParameter(ParamId, info.Id)
            //};
            //string SqlUpdate = "UPDATE siteserver_RelatedFieldItem SET ItemName = @ItemName, ItemValue = @ItemValue WHERE ID = @ID";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);

            UpdateObject(info);

            //RelatedFieldManager.ClearCache();
        }

        public void Delete(int id)
        {
            //if (id > 0)
            //{
            //    string sqlString = $"DELETE FROM siteserver_RelatedFieldItem WHERE ID = {id} OR ParentID = {id}";
            //    DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
            //}

            DeleteById(id);

            //RelatedFieldManager.ClearCache();
        }

        public IList<RelatedFieldItemInfo> GetRelatedFieldItemInfoList(int relatedFieldId, int parentId)
        {
            //var list = new List<RelatedFieldItemInfo>();

            //string sqlString =
            //    $"SELECT ID, RelatedFieldID, ItemName, ItemValue, ParentID, Taxis FROM siteserver_RelatedFieldItem WHERE RelatedFieldID = {relatedFieldId} AND ParentID = {parentId} ORDER BY Taxis";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var info = new RelatedFieldItemInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i));
            //        list.Add(info);
            //    }
            //    rdr.Close();
            //}

            //return list;

            return GetObjectList(Q
                .Where(Attr.RelatedFieldId, relatedFieldId)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis));
        }

        public void UpdateTaxisToUp(int id, int parentId)
        {
            //Get Higher Taxis and ClassID
            //string sqlString =
            //    $"SELECT TOP 1 ID, Taxis FROM siteserver_RelatedFieldItem WHERE ((Taxis > (SELECT Taxis FROM siteserver_RelatedFieldItem WHERE ID = {id})) AND ParentID = {parentId}) ORDER BY Taxis";
            //var sqlString = SqlDifferences.GetSqlString("siteserver_RelatedFieldItem", new List<string>
            //{
            //    nameof(RelatedFieldItemInfo.Id),
            //    nameof(RelatedFieldItemInfo.Taxis)
            //}, $"WHERE ((Taxis > (SELECT Taxis FROM siteserver_RelatedFieldItem WHERE ID = {id})) AND ParentID = {parentId})", "ORDER BY Taxis", 1);

            //var higherId = 0;
            //var higherTaxis = 0;

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    if (rdr.Read())
            //    {
            //        higherId = DatabaseApi.GetInt(rdr, 0);
            //        higherTaxis = DatabaseApi.GetInt(rdr, 1);
            //    }
            //    rdr.Close();
            //}

            var selectedTaxis = GetTaxis(id);
            var result = GetValue<(int Id, int Taxis)?>(Q
                .Select(Attr.Id, Attr.Taxis)
                .Where(Attr.Taxis, ">", selectedTaxis)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis));

            if (result == null) return;
            
            var higherId = result.Value.Id;
            var higherTaxis = result.Value.Taxis;

            ////Get Taxis Of Selected Class
            //var selectedTaxis = GetTaxis(id);

            if (higherId != 0)
            {
                //Set The Selected Class Taxis To Higher Level
                SetTaxis(id, higherTaxis);
                //Set The Higher Class Taxis To Lower Level
                SetTaxis(higherId, selectedTaxis);
            }

            //RelatedFieldManager.ClearCache();
        }

        public void UpdateTaxisToDown(int id, int parentId)
        {
            //Get Lower Taxis and ClassID
            //string sqlString =
            //    $"SELECT TOP 1 ID, Taxis FROM siteserver_RelatedFieldItem WHERE ((Taxis < (SELECT Taxis FROM siteserver_RelatedFieldItem WHERE (ID = {id}))) AND ParentID = {parentId}) ORDER BY Taxis DESC";
            //var sqlString = SqlDifferences.GetSqlString("siteserver_RelatedFieldItem", new List<string>
            //{
            //    nameof(RelatedFieldItemInfo.Id),
            //    nameof(RelatedFieldItemInfo.Taxis)
            //}, $"WHERE ((Taxis < (SELECT Taxis FROM siteserver_RelatedFieldItem WHERE (ID = {id}))) AND ParentID = {parentId})", "ORDER BY Taxis DESC", 1);

            //var lowerId = 0;
            //var lowerTaxis = 0;

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    if (rdr.Read())
            //    {
            //        lowerId = DatabaseApi.GetInt(rdr, 0);
            //        lowerTaxis = DatabaseApi.GetInt(rdr, 1);
            //    }
            //    rdr.Close();
            //}

            var selectedTaxis = GetTaxis(id);
            var result = GetValue<(int Id, int Taxis)?> (Q
                .Select(Attr.Id, Attr.Taxis)
                .Where(Attr.Taxis, "<", selectedTaxis)
                .Where(Attr.ParentId, parentId)
                .OrderByDesc(Attr.Taxis));

            if (result == null) return;

            var lowerId = result.Value.Id;
            var lowerTaxis = result.Value.Taxis;

            if (lowerId != 0)
            {
                //Set The Selected Class Taxis To Lower Level
                SetTaxis(id, lowerTaxis);
                //Set The Lower Class Taxis To Higher Level
                SetTaxis(lowerId, selectedTaxis);
            }

            //RelatedFieldManager.ClearCache();
        }

        private int GetTaxis(int id)
        {
            //string cmd = $"SELECT Taxis FROM siteserver_RelatedFieldItem WHERE (ID = {id})";
            //var taxis = 0;

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, cmd))
            //{
            //    if (rdr.Read())
            //    {
            //        taxis = DatabaseApi.GetInt(rdr, 0);
            //    }
            //    rdr.Close();
            //}
            //return taxis;

            return GetValue<int>(Q
                .Select(Attr.Taxis)
                .Where(Attr.Id, id));
        }

        private void SetTaxis(int id, int taxis)
        {
            //string cmd = $"UPDATE siteserver_RelatedFieldItem SET Taxis = {taxis} WHERE ID = {id}";

            //DatabaseApi.ExecuteNonQuery(ConnectionString, cmd);

            UpdateAll(Q
                .Set(Attr.Taxis, taxis)
                .Where(Attr.Id, id)
            );
        }

        private int GetMaxTaxis(int parentId)
        {
            //int maxTaxis;
            //var cmd =
            //    $"SELECT MAX(Taxis) FROM siteserver_RelatedFieldItem WHERE ParentID = {parentId} AND Taxis <> {int.MaxValue}";
            //using (var conn = GetConnection())
            //{
            //    conn.Open();
            //    var o = DatabaseApi.ExecuteScalar(conn, cmd);
            //    maxTaxis = o is System.DBNull ? 0 : int.Parse(o.ToString());
            //}
            //return maxTaxis;

            return Max(Attr.Taxis, Q.Where(Attr.ParentId, parentId));
        }

        public RelatedFieldItemInfo GetRelatedFieldItemInfo(int id)
        {
            //RelatedFieldItemInfo info = null;

            //string sqlString =
            //    $"SELECT ID, RelatedFieldID, ItemName, ItemValue, ParentID, Taxis FROM siteserver_RelatedFieldItem WHERE ID = {id}";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    if (rdr.Read())
            //    {
            //        var i = 0;
            //        info = new RelatedFieldItemInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i));
            //    }
            //    rdr.Close();
            //}

            //return info;
            return GetObjectById(id);
        }
    }
}

//using System.Collections.Generic;
 //using System.Data;
 //using SiteServer.CMS.Database.Core;
 //using SiteServer.CMS.Database.Models;
 //using SiteServer.Plugin;
 //using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class RelatedFieldItem : DataProviderBase
//    {
//        public override string TableName => "siteserver_RelatedFieldItem";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(RelatedFieldItemInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(RelatedFieldItemInfo.RelatedFieldId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(RelatedFieldItemInfo.ItemName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(RelatedFieldItemInfo.ItemValue),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(RelatedFieldItemInfo.ParentId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(RelatedFieldItemInfo.Taxis),
//                DataType = DataType.Integer
//            }
//        };

//        private const string SqlUpdate = "UPDATE siteserver_RelatedFieldItem SET ItemName = @ItemName, ItemValue = @ItemValue WHERE ID = @ID";

//        private const string ParamId = "@ID";
//        private const string ParamRelatedFieldId = "@RelatedFieldID";
//        private const string ParamItemName = "@ItemName";
//        private const string ParamItemValue = "@ItemValue";
//        private const string ParamParentId = "@ParentID";
//        private const string ParamTaxis = "@Taxis";

//        public int InsertObject(RelatedFieldItemInfo info)
//        {
//            info.Taxis = GetMaxTaxis(info.ParentId) + 1;

//            const string sqlString = "INSERT INTO siteserver_RelatedFieldItem (RelatedFieldID, ItemName, ItemValue, ParentID, Taxis) VALUES (@RelatedFieldID, @ItemName, @ItemValue, @ParentID, @Taxis)";

//            IDataParameter[] parameters =
//			{
//                GetParameter(ParamRelatedFieldId, info.RelatedFieldId),
//                GetParameter(ParamItemName, info.ItemName),
//                GetParameter(ParamItemValue, info.ItemValue),
//				GetParameter(ParamParentId, info.ParentId),
//                GetParameter(ParamTaxis, info.Taxis)
//			};

//            return DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, nameof(RelatedFieldItemInfo.Id), sqlString, parameters);

//            //RelatedFieldManager.ClearCache();
//        }

//        public void UpdateObject(RelatedFieldItemInfo info)
//        {
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamItemName, info.ItemName),
//                GetParameter(ParamItemValue, info.ItemValue),
//				GetParameter(ParamId, info.Id)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);

//            //RelatedFieldManager.ClearCache();
//        }

//        public void DeleteById(int id)
//        {
//            if (id > 0)
//            {
//                string sqlString = $"DELETE FROM siteserver_RelatedFieldItem WHERE ID = {id} OR ParentID = {id}";
//                DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//            }
//            //RelatedFieldManager.ClearCache();
//        }

//        public List<RelatedFieldItemInfo> GetRelatedFieldItemInfoList(int relatedFieldId, int parentId)
//        {
//            var list = new List<RelatedFieldItemInfo>();

//            string sqlString =
//                $"SELECT ID, RelatedFieldID, ItemName, ItemValue, ParentID, Taxis FROM siteserver_RelatedFieldItem WHERE RelatedFieldID = {relatedFieldId} AND ParentID = {parentId} ORDER BY Taxis";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var info = new RelatedFieldItemInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i));
//                    list.Add(info);
//                }
//                rdr.Close();
//            }

//            return list;
//        }

//        public void UpdateTaxisToUp(int id, int parentId)
//        {
//            //Get Higher Taxis and ClassID
//            //string sqlString =
//            //    $"SELECT TOP 1 ID, Taxis FROM siteserver_RelatedFieldItem WHERE ((Taxis > (SELECT Taxis FROM siteserver_RelatedFieldItem WHERE ID = {id})) AND ParentID = {parentId}) ORDER BY Taxis";
//            var sqlString = SqlDifferences.GetSqlString("siteserver_RelatedFieldItem", new List<string>
//            {
//                nameof(RelatedFieldItemInfo.Id),
//                nameof(RelatedFieldItemInfo.Taxis)
//            }, $"WHERE ((Taxis > (SELECT Taxis FROM siteserver_RelatedFieldItem WHERE ID = {id})) AND ParentID = {parentId})", "ORDER BY Taxis", 1);

//            var higherId = 0;
//            var higherTaxis = 0;

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                if (rdr.Read())
//                {
//                    higherId = DatabaseApi.GetInt(rdr, 0);
//                    higherTaxis = DatabaseApi.GetInt(rdr, 1);
//                }
//                rdr.Close();
//            }

//            //Get Taxis Of Selected Class
//            var selectedTaxis = GetTaxis(id);

//            if (higherId != 0)
//            {
//                //Set The Selected Class Taxis To Higher Level
//                SetTaxis(id, higherTaxis);
//                //Set The Higher Class Taxis To Lower Level
//                SetTaxis(higherId, selectedTaxis);
//            }

//            //RelatedFieldManager.ClearCache();
//        }

//        public void UpdateTaxisToDown(int id, int parentId)
//        {
//            //Get Lower Taxis and ClassID
//            //string sqlString =
//            //    $"SELECT TOP 1 ID, Taxis FROM siteserver_RelatedFieldItem WHERE ((Taxis < (SELECT Taxis FROM siteserver_RelatedFieldItem WHERE (ID = {id}))) AND ParentID = {parentId}) ORDER BY Taxis DESC";
//            var sqlString = SqlDifferences.GetSqlString("siteserver_RelatedFieldItem", new List<string>
//            {
//                nameof(RelatedFieldItemInfo.Id),
//                nameof(RelatedFieldItemInfo.Taxis)
//            }, $"WHERE ((Taxis < (SELECT Taxis FROM siteserver_RelatedFieldItem WHERE (ID = {id}))) AND ParentID = {parentId})", "ORDER BY Taxis DESC", 1);

//            var lowerId = 0;
//            var lowerTaxis = 0;

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                if (rdr.Read())
//                {
//                    lowerId = DatabaseApi.GetInt(rdr, 0);
//                    lowerTaxis = DatabaseApi.GetInt(rdr, 1);
//                }
//                rdr.Close();
//            }

//            //Get Taxis Of Selected Class
//            var selectedTaxis = GetTaxis(id);

//            if (lowerId != 0)
//            {
//                //Set The Selected Class Taxis To Lower Level
//                SetTaxis(id, lowerTaxis);
//                //Set The Lower Class Taxis To Higher Level
//                SetTaxis(lowerId, selectedTaxis);
//            }

//            //RelatedFieldManager.ClearCache();
//        }

//        private int GetTaxis(int id)
//        {
//            string cmd = $"SELECT Taxis FROM siteserver_RelatedFieldItem WHERE (ID = {id})";
//            var taxis = 0;

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, cmd))
//            {
//                if (rdr.Read())
//                {
//                    taxis = DatabaseApi.GetInt(rdr, 0);
//                }
//                rdr.Close();
//            }
//            return taxis;
//        }

//        private void SetTaxis(int id, int taxis)
//        {
//            string cmd = $"UPDATE siteserver_RelatedFieldItem SET Taxis = {taxis} WHERE ID = {id}";

//            DatabaseApi.ExecuteNonQuery(ConnectionString, cmd);
//        }

//        public int GetMaxTaxis(int parentId)
//        {
//            int maxTaxis;
//            var cmd =
//                $"SELECT MAX(Taxis) FROM siteserver_RelatedFieldItem WHERE ParentID = {parentId} AND Taxis <> {int.MaxValue}";
//            using (var conn = GetConnection())
//            {
//                conn.Open();
//                var o = DatabaseApi.ExecuteScalar(conn, cmd);
//                maxTaxis = o is System.DBNull ? 0 : int.Parse(o.ToString());
//            }
//            return maxTaxis;
//        }

//        public RelatedFieldItemInfo GetRelatedFieldItemInfo(int id)
//        {
//            RelatedFieldItemInfo info = null;

//            string sqlString =
//                $"SELECT ID, RelatedFieldID, ItemName, ItemValue, ParentID, Taxis FROM siteserver_RelatedFieldItem WHERE ID = {id}";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                if (rdr.Read())
//                {
//                    var i = 0;
//                    info = new RelatedFieldItemInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i));
//                }
//                rdr.Close();
//            }

//            return info;
//        }


//    }
//}