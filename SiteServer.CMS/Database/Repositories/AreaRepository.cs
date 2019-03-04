using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Repositories
{
    public class AreaRepository : GenericRepository<AreaInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(AreaInfo.Id);
            public const string ParentId = nameof(AreaInfo.ParentId);
            public const string ParentsPath = nameof(AreaInfo.ParentsPath);
            public const string ChildrenCount = nameof(AreaInfo.ChildrenCount);
            public const string Taxis = nameof(AreaInfo.Taxis);
            public const string IsLastNode = "IsLastNode";
            public const string CountOfAdmin = nameof(AreaInfo.CountOfAdmin);
        }

        private void Insert(AreaInfo parentInfo, AreaInfo areaInfo)
        {
            if (parentInfo != null)
            {
                areaInfo.ParentsPath = parentInfo.ParentsPath + "," + parentInfo.Id;
                areaInfo.ParentsCount = parentInfo.ParentsCount + 1;

                var maxTaxis = GetMaxTaxisByParentPath(areaInfo.ParentsPath);
                if (maxTaxis == 0)
                {
                    maxTaxis = parentInfo.Taxis;
                }
                areaInfo.Taxis = maxTaxis + 1;
            }
            else
            {
                areaInfo.ParentsPath = "0";
                areaInfo.ParentsCount = 0;
                var maxTaxis = GetMaxTaxisByParentPath("0");
                areaInfo.Taxis = maxTaxis + 1;
            }
            areaInfo.ChildrenCount = 0;
            areaInfo.LastNode = true;

            //DatabaseApi.Instance.ExecuteNonQuery(trans, $"UPDATE siteserver_Area SET {SqlUtils.ToPlusSqlString("Taxis")} WHERE (Taxis >= {areaInfo.Taxis})");
            IncrementAll(Attr.Taxis, Q
                    .Where(Attr.Taxis, ">=", areaInfo.Taxis));

            //IDataParameter[] parameters = {
            //    _db.GetParameter(ParamName, areaInfo.AreaName),
            //    _db.GetParameter(ParamParentId, areaInfo.ParentId),
            //    _db.GetParameter(ParamParentsPath, areaInfo.ParentsPath),
            //    _db.GetParameter(ParamParentsCount, areaInfo.ParentsCount),
            //    _db.GetParameter(ParamChildrenCount, 0),
            //    _db.GetParameter(ParamIsLastNode, true.ToString()),
            //    _db.GetParameter(ParamTaxis, areaInfo.Taxis),
            //    _db.GetParameter(ParamCountOfAdmin, areaInfo.CountOfAdmin)
            //};

            //areaInfo.Id = _db.ExecuteNonQueryAndReturnId(TableName, nameof(AreaInfo.Id), trans, "INSERT INTO siteserver_Area (AreaName, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, CountOfAdmin) VALUES (@AreaName, @ParentID, @ParentsPath, @ParentsCount, @ChildrenCount, @IsLastNode, @Taxis, @CountOfAdmin)", parameters);
            InsertObject(areaInfo);

            if (!string.IsNullOrEmpty(areaInfo.ParentsPath) && areaInfo.ParentsPath != "0")
            {
                //_db.ExecuteNonQuery(trans, $"UPDATE siteserver_Area SET {SqlUtils.ToPlusSqlString("ChildrenCount")} WHERE Id IN ({AttackUtils.FilterSql(areaInfo.ParentsPath)})");
                IncrementAll(Attr.ChildrenCount, Q
                    .WhereIn(Attr.Id, TranslateUtils.StringCollectionToIntList(areaInfo.ParentsPath)));
            }

            //_db.ExecuteNonQuery(trans, $"UPDATE siteserver_Area SET IsLastNode = '{false}' WHERE ParentID = {areaInfo.ParentId}");
            //UpdateValue(new Dictionary<string, object>
            //{
            //    {Attr.IsLastNode, false.ToString()}
            //}, Q.Where(Attr.ParentId, areaInfo.ParentId));
            UpdateAll(Q
                .Set(Attr.IsLastNode, false.ToString())
                .Where(Attr.ParentId, areaInfo.ParentId)
            );

            //sqlString =
            //    $"UPDATE siteserver_Area SET IsLastNode = 'True' WHERE (Id IN (SELECT TOP 1 Id FROM siteserver_Area WHERE ParentID = {areaInfo.ParentId} ORDER BY Taxis DESC))";

            var topId = GetValue<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, areaInfo.ParentId)
                .OrderByDesc(Attr.Taxis));

            if (topId > 0)
            {
                UpdateAll(Q
                    .Set(Attr.IsLastNode, true.ToString())
                    .Where(nameof(Attr.Id), topId)
                );
            }

            //_db.ExecuteNonQuery(trans, $"UPDATE siteserver_Area SET IsLastNode = '{true}' WHERE Id IN ({SqlUtils.ToInTopSqlString(TableName, "Id", $"WHERE ParentID = {areaInfo.ParentId}", "ORDER BY Taxis DESC", 1)})");

            AreaManager.ClearCache();
        }

        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
        {
            if (string.IsNullOrEmpty(parentsPath)) return;

            //var sqlString = string.Concat("UPDATE siteserver_Area SET ChildrenCount = ChildrenCount - ", subtractNum, " WHERE Id IN (", AttackUtils.FilterSql(parentsPath), ")");
            //_db.ExecuteNonQuery(_connectionString, sqlString);
            DecrementAll(Attr.ChildrenCount, Q
                .WhereIn(Attr.Id, TranslateUtils.StringCollectionToIntList(parentsPath)), subtractNum);

            AreaManager.ClearCache();
        }

        private void TaxisSubtract(int selectedId)
        {
            var areaInfo = GetObjectById(selectedId);
            if (areaInfo == null) return;
            //Get Lower Taxis and Id
   //         int lowerId;
   //         int lowerChildrenCount;
   //         string lowerParentsPath;

   //         var sqlString = SqlUtils.ToTopSqlString(TableName, "Id, ChildrenCount, ParentsPath",
   //             "WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis < @Taxis)", "ORDER BY Taxis DESC", 1);

   //         IDataParameter[] parameters = {
   //             _db.GetParameter(ParamParentId, areaInfo.ParentId),
   //             _db.GetParameter(ParamId, areaInfo.Id),
   //             _db.GetParameter(ParamTaxis, areaInfo.Taxis),
			//};

   //         using (var rdr = DatabaseApi.Instance.ExecuteReader(_connectionString, sqlString, parameters))
   //         {
   //             if (rdr.Read())
   //             {
   //                 lowerId = _db.GetInt(rdr, 0);
   //                 lowerChildrenCount = _db.GetInt(rdr, 1);
   //                 lowerParentsPath = _db.GetString(rdr, 2);
   //             }
   //             else
   //             {
   //                 return;
   //             }
   //             rdr.Close();
   //         }

            //var dataInfo = Get(new GenericQuery()
            //    .Equal(Attr.ParentId, areaInfo.ParentId)
            //    .NotEqual(Attr.Id, areaInfo.Id)
            //    .Less(Attr.Taxis, areaInfo.Taxis)
            //    .OrderByDescending(Attr.Taxis));

            var dataInfo = GetObject(Q
                .Where(Attr.ParentId, areaInfo.ParentId)
                .WhereNot(Attr.Id, areaInfo.Id)
                .Where(Attr.Taxis, "<", areaInfo.Taxis)
                .OrderByDesc(Attr.Taxis));

            if (dataInfo == null) return;

            var lowerId = dataInfo.Id;
            var lowerChildrenCount = dataInfo.ChildrenCount;
            var lowerParentsPath = dataInfo.ParentsPath;

            var lowerNodePath = string.Concat(lowerParentsPath, ",", lowerId);
            var selectedNodePath = string.Concat(areaInfo.ParentsPath, ",", areaInfo.Id);

            SetTaxisSubtract(selectedId, selectedNodePath, lowerChildrenCount + 1);
            SetTaxisAdd(lowerId, lowerNodePath, areaInfo.ChildrenCount + 1);

            UpdateIsLastNode(areaInfo.ParentId);
        }

        private void TaxisAdd(int selectedId)
        {
            var areaInfo = GetObjectById(selectedId);
            if (areaInfo == null) return;
            //Get Higher Taxis and Id
            //         int higherId;
            //         int higherChildrenCount;
            //         string higherParentsPath;

            //         var sqlString = SqlUtils.ToTopSqlString(TableName, "Id, ChildrenCount, ParentsPath",
            //             "WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis > @Taxis)", "ORDER BY Taxis", 1);

            //         IDataParameter[] parameters = {
            //             _db.GetParameter(ParamParentId, areaInfo.ParentId),
            //             _db.GetParameter(ParamId, areaInfo.Id),
            //             _db.GetParameter(ParamTaxis, areaInfo.Taxis)
            //};

            //         using (var rdr = _db.ExecuteReader(_connectionString, sqlString, parameters))
            //         {
            //             if (rdr.Read())
            //             {
            //                 higherId = _db.GetInt(rdr, 0);
            //                 higherChildrenCount = _db.GetInt(rdr, 1);
            //                 higherParentsPath = _db.GetString(rdr, 2);
            //             }
            //             else
            //             {
            //                 return;
            //             }
            //             rdr.Close();
            //         }

            //var dataInfo = Get(new GenericQuery()
            //    .Select(new[]
            //    {
            //        Attr.Id,
            //        Attr.ChildrenCount,
            //        Attr.ParentsPath
            //    })
            //    .Equal(Attr.ParentId, areaInfo.ParentId)
            //    .NotEqual(Attr.Id, areaInfo.Id)
            //    .Greater(Attr.Taxis, areaInfo.Taxis)
            //    .OrderBy(Attr.Taxis));

            var dataInfo = GetObject(Q
                .Where(Attr.ParentId, areaInfo.ParentId)
                .WhereNot(Attr.Id, areaInfo.Id)
                .Where(Attr.Taxis, ">", areaInfo.Taxis)
                .OrderBy(Attr.Taxis));

            if (dataInfo == null) return;

            var higherId = dataInfo.Id;
            var higherChildrenCount = dataInfo.ChildrenCount;
            var higherParentsPath = dataInfo.ParentsPath;

            var higherNodePath = string.Concat(higherParentsPath, ",", higherId);
            var selectedNodePath = string.Concat(areaInfo.ParentsPath, ",", areaInfo.Id);

            SetTaxisAdd(selectedId, selectedNodePath, higherChildrenCount + 1);
            SetTaxisSubtract(higherId, higherNodePath, areaInfo.ChildrenCount + 1);

            UpdateIsLastNode(areaInfo.ParentId);
        }

        private void SetTaxisAdd(int areaId, string parentsPath, int addNum)
        {
            //var path = AttackUtils.FilterSql(parentsPath);
            //var sqlString =
            //    $"UPDATE siteserver_Area SET Taxis = Taxis + {addNum} WHERE Id = {areaId} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

            //_db.ExecuteNonQuery(_connectionString, sqlString);
            IncrementAll(Attr.Taxis, Q
                .Where(Attr.Id, areaId)
                .OrWhere(Attr.ParentsPath, parentsPath)
                .OrWhereStarts(Attr.ParentsPath, parentsPath + ","), addNum);

            AreaManager.ClearCache();
        }

        private void SetTaxisSubtract(int areaId, string parentsPath, int subtractNum)
        {
            //var path = AttackUtils.FilterSql(parentsPath);
            //var sqlString =
            //    $"UPDATE siteserver_Area SET Taxis = Taxis - {subtractNum} WHERE  Id = {areaId} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

            //_db.ExecuteNonQuery(_connectionString, sqlString);
            DecrementAll(Attr.Taxis, Q
                .Where(Attr.Id, areaId)
                .OrWhere(Attr.ParentsPath, parentsPath)
                .OrWhereStarts(Attr.ParentsPath, parentsPath + ","), subtractNum);

            AreaManager.ClearCache();
        }

        private void UpdateIsLastNode(int parentId)
        {
            if (parentId <= 0) return;

            //var sqlString = "UPDATE siteserver_Area SET IsLastNode = @IsLastNode WHERE ParentID = @ParentID";

            //IDataParameter[] parameters = {
            //    _db.GetParameter(ParamIsLastNode, false.ToString()),
            //    _db.GetParameter(ParamParentId, parentId)
            //};

            //_db.ExecuteNonQuery(_connectionString, sqlString, parameters);

            UpdateAll(Q
                .Set(Attr.IsLastNode, false.ToString())
                .Where(Attr.ParentId, parentId)
            );

            //sqlString =
            //    $"UPDATE siteserver_Area SET IsLastNode = '{true}' WHERE Id IN ({SqlUtils.ToInTopSqlString(TableName, "Id", $"WHERE ParentID = {parentId}", "ORDER BY Taxis DESC", 1)})";

            //_db.ExecuteNonQuery(_connectionString, sqlString);

            var topId = GetValue<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderByDesc(Attr.Taxis));

            if (topId > 0)
            {
                UpdateAll(Q
                    .Set(Attr.IsLastNode, true.ToString())
                    .Where(nameof(Attr.Id), topId)
                );
            }
        }

        private int GetMaxTaxisByParentPath(string parentPath)
        {
            return Max(Attr.Taxis, Q
                .Where(Attr.ParentsPath, parentPath)
                .OrWhereStarts(Attr.ParentsPath, parentPath + ","));

            //var sqlString = string.Concat("SELECT MAX(Taxis) AS MaxTaxis FROM siteserver_Area WHERE (ParentsPath = '", AttackUtils.FilterSql(parentPath), "') OR (ParentsPath LIKE '", AttackUtils.FilterSql(parentPath), ",%')");
            //var maxTaxis = 0;

            //using (var rdr = _db.ExecuteReader(_connectionString, sqlString))
            //{
            //    if (rdr.Read())
            //    {
            //        maxTaxis = _db.GetInt(rdr, 0);
            //    }
            //    rdr.Close();
            //}
            //return maxTaxis;
        }

        public void Insert(AreaInfo areaInfo)
        {
            //using (var conn = GetConnection())
            //{
            //    conn.Open();
            //    using (var trans = conn.BeginTransaction())
            //    {
            //        try
            //        {
            //            var parentAreaInfo = GetAreaInfo(areaInfo.ParentId);

            //            InsertWithTrans(parentAreaInfo, areaInfo, trans);

            //            trans.Commit();
            //        }
            //        catch
            //        {
            //            trans.Rollback();
            //            throw;
            //        }
            //    }
            //}
            
            var parentAreaInfo = GetObjectById(areaInfo.ParentId);

            Insert(parentAreaInfo, areaInfo);

            //var parentAreaInfo = GetAreaInfo(areaInfo.ParentId);
            //InsertObject(parentAreaInfo, areaInfo);

            AreaManager.ClearCache();
        }

        public void Update(AreaInfo areaInfo)
        {
   //         IDataParameter[] updateParams = {
   //             _db.GetParameter(ParamName, areaInfo.AreaName),
   //             _db.GetParameter(ParamParentsPath, areaInfo.ParentsPath),
   //             _db.GetParameter(ParamParentsCount, areaInfo.ParentsCount),
   //             _db.GetParameter(ParamChildrenCount, areaInfo.ChildrenCount),
   //             _db.GetParameter(ParamIsLastNode, areaInfo.LastNode.ToString()),
   //             _db.GetParameter(ParamCountOfAdmin, areaInfo.CountOfAdmin),
   //             _db.GetParameter(ParamId, areaInfo.Id)
			//};

   //         var i = _db.ExecuteNonQuery(_connectionString, SqlUpdate, updateParams);
   
            if (UpdateObject(areaInfo))
            {
                AreaManager.ClearCache();
            }
        }

        public void UpdateTaxis(int selectedId, bool isSubtract)
        {
            if (isSubtract)
            {
                TaxisSubtract(selectedId);
            }
            else
            {
                TaxisAdd(selectedId);
            }
        }

        public void UpdateCountOfAdmin()
        {
            var areaIdList = AreaManager.GetAreaIdList();
            foreach (var areaId in areaIdList)
            {
                var count = DataProvider.Administrator.GetCountByAreaId(areaId);
                //var sqlString = $"UPDATE {TableName} SET CountOfAdmin = {count} WHERE Id = {areaId}";
                //_db.ExecuteNonQuery(_connectionString, sqlString);

                UpdateAll(Q
                    .Set(Attr.CountOfAdmin, count)
                    .Where(nameof(Attr.Id), areaId)
                );
            }
            AreaManager.ClearCache();
        }

        public void Delete(int areaId)
        {
            var areaInfo = GetObjectById(areaId);
            if (areaInfo != null)
            {
                IList<int> areaIdList = new List<int>();
                if (areaInfo.ChildrenCount > 0)
                {
                    areaIdList = GetIdListForDescendant(areaId);
                }
                areaIdList.Add(areaId);

                //var sqlString =
                //    $"DELETE FROM siteserver_Area WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(areaIdList)})";

                //int deletedNum;

                //using (var conn = GetConnection())
                //{
                //    conn.Open();
                //    using (var trans = conn.BeginTransaction())
                //    {
                //        try
                //        {
                //            deletedNum = _db.ExecuteNonQuery(trans, sqlString);

                //            if (deletedNum > 0)
                //            {
                //                string sqlStringTaxis =
                //                    $"UPDATE siteserver_Area SET Taxis = Taxis - {deletedNum} WHERE (Taxis > {areaInfo.Taxis})";
                //                _db.ExecuteNonQuery(trans, sqlStringTaxis);
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
                var deletedNum = DeleteAll(Q
                    .WhereIn(Attr.Id, areaIdList));

                if (deletedNum > 0)
                {
                    DecrementAll(Attr.Taxis, Q
                        .Where(Attr.Taxis, ">", areaInfo.Taxis), deletedNum);
                }

                UpdateIsLastNode(areaInfo.ParentId);
                UpdateSubtractChildrenCount(areaInfo.ParentsPath, deletedNum);
            }

            AreaManager.ClearCache();
        }

        //private AreaInfo GetAreaInfo(int areaId)
        //{
        //    //         AreaInfo areaInfo = null;

        //    //         IDataParameter[] parameters = {
        //    //             _db.GetParameter(ParamId, areaId)
        //    //};

        //    //         using (var rdr = _db.ExecuteReader(_connectionString, SqlSelect, parameters))
        //    //         {
        //    //             if (rdr.Read())
        //    //             {
        //    //                 areaInfo = GetAreaInfo(rdr);
        //    //             }
        //    //             rdr.Close();
        //    //         }
        //    //         return areaInfo;
        //    return base.Get(areaId);
        //}

        private IList<AreaInfo> GetAreaInfoList()
        {
            //var list = new List<AreaInfo>();

            //using (var rdr = _db.ExecuteReader(_connectionString, SqlSelectAll))
            //{
            //    while (rdr.Read())
            //    {
            //        var areaInfo = GetAreaInfo(rdr);
            //        list.Add(areaInfo);
            //    }
            //    rdr.Close();
            //}
            //return list;
            return GetObjectList(Q
                .OrderBy(Attr.Taxis));
        }

        public IList<int> GetIdListByParentId(int parentId)
        {
            //var sqlString = $@"SELECT Id FROM siteserver_Area WHERE ParentID = '{parentId}' ORDER BY Taxis";
            //var list = new List<int>();

            //using (var rdr = _db.ExecuteReader(_connectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(_db.GetInt(rdr, 0));
            //    }
            //    rdr.Close();
            //}

            //return list;
            return GetValueList<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis));
        }

        private IList<int> GetIdListForDescendant(int areaId)
        {
//            string sqlString = $@"SELECT Id
//FROM siteserver_Area
//WHERE (ParentsPath LIKE '{areaId},%') OR
//      (ParentsPath LIKE '%,{areaId},%') OR
//      (ParentsPath LIKE '%,{areaId}') OR
//      (ParentID = {areaId})
//";
//            var list = new List<int>();

//            using (var rdr = _db.ExecuteReader(_connectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var theId = _db.GetInt(rdr, 0);
//                    list.Add(theId);
//                }
//                rdr.Close();
//            }

//            return list;
            return GetValueList<int>(Q
                .Select(Attr.Id)
                .WhereStarts(Attr.ParentsPath, $"{areaId},")
                .OrWhereContains(Attr.ParentsPath, $",{areaId},")
                .OrWhereEnds(Attr.ParentsPath, $",{areaId}"));
        }

        public List<KeyValuePair<int, AreaInfo>> GetAreaInfoPairList()
        {
            var areaInfoList = GetAreaInfoList();

            return areaInfoList.Select(areaInfo => new KeyValuePair<int, AreaInfo>(areaInfo.Id, areaInfo)).ToList();
        }

        //private AreaInfo GetAreaInfo(IDataReader rdr)
        //{
        //    var i = 0;
        //    return new AreaInfo
        //    {
        //        Id = _db.GetInt(rdr, i++),
        //        Guid = _db.GetString(rdr, i++),
        //        LastModifiedDate = _db.GetDateTime(rdr, i++),
        //        AreaName = _db.GetString(rdr, i++),
        //        ParentId = _db.GetInt(rdr, i++),
        //        ParentsPath = _db.GetString(rdr, i++),
        //        ParentsCount = _db.GetInt(rdr, i++),
        //        ChildrenCount = _db.GetInt(rdr, i++),
        //        LastNode = TranslateUtils.ToBool(_db.GetString(rdr, i++)),
        //        Taxis = _db.GetInt(rdr, i++),
        //        CountOfAdmin = _db.GetInt(rdr, i)
        //    };
        //}
    }
}

//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Core;
//using SiteServer.CMS.Core.Database;
//using SiteServer.CMS.DataCache;
//using SiteServer.CMS.Model;
//using SiteServer.Plugin;
//using SiteServer.Utils;

//namespace SiteServer.CMS.Repositories
//{
//    public class AreaDao : DataProviderBase
//    {
//        public override string TableName => "siteserver_Area";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(AreaInfo.Id),
//                DataType = DataType.Integer,
//                IsPrimaryKey = true,
//                IsIdentity = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(AreaInfo.AreaName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(AreaInfo.ParentId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(AreaInfo.ParentsPath),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(AreaInfo.ParentsCount),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(AreaInfo.ChildrenCount),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(AreaInfo.IsLastNode),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(AreaInfo.Taxis),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(AreaInfo.CountOfAdmin),
//                DataType = DataType.Integer
//            }
//        };

//        private const string SqlSelect = "SELECT Id, AreaName, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, CountOfAdmin FROM siteserver_Area WHERE Id = @Id";
//        private const string SqlSelectAll = "SELECT Id, AreaName, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, CountOfAdmin FROM siteserver_Area ORDER BY TAXIS";
//        private const string SqlUpdate = "UPDATE siteserver_Area SET AreaName = @AreaName, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildrenCount = @ChildrenCount, IsLastNode = @IsLastNode, CountOfAdmin = @CountOfAdmin WHERE Id = @Id";

//        private const string ParamId = "@Id";
//        private const string ParamName = "@AreaName";
//        private const string ParamParentId = "@ParentID";
//        private const string ParamParentsPath = "@ParentsPath";
//        private const string ParamParentsCount = "@ParentsCount";
//        private const string ParamChildrenCount = "@ChildrenCount";
//        private const string ParamIsLastNode = "@IsLastNode";
//        private const string ParamTaxis = "@Taxis";
//        private const string ParamCountOfAdmin = "@CountOfAdmin";

//        private void InsertWithTrans(AreaInfo parentInfo, AreaInfo areaInfo, IDbTransaction trans)
//        {
//            if (parentInfo != null)
//            {
//                areaInfo.ParentsPath = parentInfo.ParentsPath + "," + parentInfo.Id;
//                areaInfo.ParentsCount = parentInfo.ParentsCount + 1;

//                var maxTaxis = GetMaxTaxisByParentPath(areaInfo.ParentsPath);
//                if (maxTaxis == 0)
//                {
//                    maxTaxis = parentInfo.Taxis;
//                }
//                areaInfo.Taxis = maxTaxis + 1;
//            }
//            else
//            {
//                areaInfo.ParentsPath = "0";
//                areaInfo.ParentsCount = 0;
//                var maxTaxis = GetMaxTaxisByParentPath("0");
//                areaInfo.Taxis = maxTaxis + 1;
//            }

//            var sqlInsert = "INSERT INTO siteserver_Area (AreaName, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, CountOfAdmin) VALUES (@AreaName, @ParentID, @ParentsPath, @ParentsCount, @ChildrenCount, @IsLastNode, @Taxis, @CountOfAdmin)";

//            IDataParameter[] insertParams = {
//                GetParameter(ParamName, areaInfo.AreaName),
//                GetParameter(ParamParentId, areaInfo.ParentId),
//                GetParameter(ParamParentsPath, areaInfo.ParentsPath),
//                GetParameter(ParamParentsCount, areaInfo.ParentsCount),
//                GetParameter(ParamChildrenCount, 0),
//                GetParameter(ParamIsLastNode, true.ToString()),
//                GetParameter(ParamTaxis, areaInfo.Taxis),
//                GetParameter(ParamCountOfAdmin, areaInfo.CountOfAdmin)
//            };

//            string sqlString = $"UPDATE siteserver_Area SET {SqlUtils.ToPlusSqlString("Taxis")} WHERE (Taxis >= {areaInfo.Taxis})";
//            DatabaseApi.ExecuteNonQuery(trans, sqlString);

//            areaInfo.Id = DatabaseApi.ExecuteNonQueryAndReturnId(TableName, nameof(AreaInfo.Id), trans, sqlInsert, insertParams);

//            if (!string.IsNullOrEmpty(areaInfo.ParentsPath) && areaInfo.ParentsPath != "0")
//            {
//                sqlString = $"UPDATE siteserver_Area SET {SqlUtils.ToPlusSqlString("ChildrenCount")} WHERE Id IN ({AttackUtils.FilterSql(areaInfo.ParentsPath)})";

//                DatabaseApi.ExecuteNonQuery(trans, sqlString);
//            }

//            sqlString = $"UPDATE siteserver_Area SET IsLastNode = '{false}' WHERE ParentID = {areaInfo.ParentId}";

//            DatabaseApi.ExecuteNonQuery(trans, sqlString);

//            //sqlString =
//            //    $"UPDATE siteserver_Area SET IsLastNode = 'True' WHERE (Id IN (SELECT TOP 1 Id FROM siteserver_Area WHERE ParentID = {areaInfo.ParentId} ORDER BY Taxis DESC))";            
//            sqlString =
//                $"UPDATE siteserver_Area SET IsLastNode = '{true}' WHERE Id IN ({SqlUtils.ToInTopSqlString(TableName, "Id", $"WHERE ParentID = {areaInfo.ParentId}", "ORDER BY Taxis DESC", 1)})";

//            DatabaseApi.ExecuteNonQuery(trans, sqlString);

//            AreaManager.ClearCache();
//        }

//        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
//        {
//            if (!string.IsNullOrEmpty(parentsPath))
//            {
//                var sqlString = string.Concat("UPDATE siteserver_Area SET ChildrenCount = ChildrenCount - ", subtractNum, " WHERE Id IN (", AttackUtils.FilterSql(parentsPath), ")");
//                DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

//                AreaManager.ClearCache();
//            }
//        }

//        private void TaxisSubtract(int selectedId)
//        {
//            var areaInfo = GetAreaInfo(selectedId);
//            if (areaInfo == null) return;
//            //Get Lower Taxis and Id
//            int lowerId;
//            int lowerChildrenCount;
//            string lowerParentsPath;

//            var sqlString = SqlUtils.ToTopSqlString(TableName, "Id, ChildrenCount, ParentsPath",
//                "WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis < @Taxis)", "ORDER BY Taxis DESC", 1);

//            IDataParameter[] parameters = {
//                GetParameter(ParamParentId, areaInfo.ParentId),
//                GetParameter(ParamId, areaInfo.Id),
//                GetParameter(ParamTaxis, areaInfo.Taxis),
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    lowerId = DatabaseApi.GetInt(rdr, 0);
//                    lowerChildrenCount = DatabaseApi.GetInt(rdr, 1);
//                    lowerParentsPath = DatabaseApi.GetString(rdr, 2);
//                }
//                else
//                {
//                    return;
//                }
//                rdr.Close();
//            }


//            var lowerNodePath = string.Concat(lowerParentsPath, ",", lowerId);
//            var selectedNodePath = string.Concat(areaInfo.ParentsPath, ",", areaInfo.Id);

//            SetTaxisSubtract(selectedId, selectedNodePath, lowerChildrenCount + 1);
//            SetTaxisAdd(lowerId, lowerNodePath, areaInfo.ChildrenCount + 1);

//            UpdateIsLastNode(areaInfo.ParentId);
//        }

//        private void TaxisAdd(int selectedId)
//        {
//            var areaInfo = GetAreaInfo(selectedId);
//            if (areaInfo == null) return;
//            //Get Higher Taxis and Id
//            int higherId;
//            int higherChildrenCount;
//            string higherParentsPath;

//            var sqlString = SqlUtils.ToTopSqlString(TableName, "Id, ChildrenCount, ParentsPath",
//                "WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis > @Taxis)", "ORDER BY Taxis", 1);

//            IDataParameter[] parameters = {
//                GetParameter(ParamParentId, areaInfo.ParentId),
//                GetParameter(ParamId, areaInfo.Id),
//                GetParameter(ParamTaxis, areaInfo.Taxis)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    higherId = DatabaseApi.GetInt(rdr, 0);
//                    higherChildrenCount = DatabaseApi.GetInt(rdr, 1);
//                    higherParentsPath = DatabaseApi.GetString(rdr, 2);
//                }
//                else
//                {
//                    return;
//                }
//                rdr.Close();
//            }


//            var higherNodePath = string.Concat(higherParentsPath, ",", higherId);
//            var selectedNodePath = string.Concat(areaInfo.ParentsPath, ",", areaInfo.Id);

//            SetTaxisAdd(selectedId, selectedNodePath, higherChildrenCount + 1);
//            SetTaxisSubtract(higherId, higherNodePath, areaInfo.ChildrenCount + 1);

//            UpdateIsLastNode(areaInfo.ParentId);
//        }

//        private void SetTaxisAdd(int areaId, string parentsPath, int addNum)
//        {
//            var path = AttackUtils.FilterSql(parentsPath);
//            string sqlString =
//                $"UPDATE siteserver_Area SET Taxis = Taxis + {addNum} WHERE Id = {areaId} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

//            AreaManager.ClearCache();
//        }

//        private void SetTaxisSubtract(int areaId, string parentsPath, int subtractNum)
//        {
//            var path = AttackUtils.FilterSql(parentsPath);
//            string sqlString =
//                $"UPDATE siteserver_Area SET Taxis = Taxis - {subtractNum} WHERE  Id = {areaId} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

//            AreaManager.ClearCache();
//        }

//        private void UpdateIsLastNode(int parentId)
//        {
//            if (parentId <= 0) return;

//            var sqlString = "UPDATE siteserver_Area SET IsLastNode = @IsLastNode WHERE ParentID = @ParentID";

//            IDataParameter[] parameters = {
//                GetParameter(ParamIsLastNode, false.ToString()),
//                GetParameter(ParamParentId, parentId)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            sqlString =
//                $"UPDATE siteserver_Area SET IsLastNode = '{true}' WHERE Id IN ({SqlUtils.ToInTopSqlString(TableName, "Id", $"WHERE ParentID = {parentId}", "ORDER BY Taxis DESC", 1)})";

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//        }

//        private int GetMaxTaxisByParentPath(string parentPath)
//        {
//            var sqlString = string.Concat("SELECT MAX(Taxis) AS MaxTaxis FROM siteserver_Area WHERE (ParentsPath = '", AttackUtils.FilterSql(parentPath), "') OR (ParentsPath LIKE '", AttackUtils.FilterSql(parentPath), ",%')");
//            var maxTaxis = 0;

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                if (rdr.Read())
//                {
//                    maxTaxis = DatabaseApi.GetInt(rdr, 0);
//                }
//                rdr.Close();
//            }
//            return maxTaxis;
//        }

//        public int InsertObject(AreaInfo areaInfo)
//        {
//            using (var conn = GetConnection())
//            {
//                conn.Open();
//                using (var trans = conn.BeginTransaction())
//                {
//                    try
//                    {
//                        var parentAreaInfo = GetAreaInfo(areaInfo.ParentId);

//                        InsertWithTrans(parentAreaInfo, areaInfo, trans);

//                        trans.Commit();
//                    }
//                    catch
//                    {
//                        trans.Rollback();
//                        throw;
//                    }
//                }
//            }

//            AreaManager.ClearCache();

//            return areaInfo.Id;
//        }

//        public void UpdateObject(AreaInfo areaInfo)
//        {
//            IDataParameter[] updateParams = {
//                GetParameter(ParamName, areaInfo.AreaName),
//                GetParameter(ParamParentsPath, areaInfo.ParentsPath),
//                GetParameter(ParamParentsCount, areaInfo.ParentsCount),
//                GetParameter(ParamChildrenCount, areaInfo.ChildrenCount),
//                GetParameter(ParamIsLastNode, areaInfo.IsLastNode.ToString()),
//                GetParameter(ParamCountOfAdmin, areaInfo.CountOfAdmin),
//                GetParameter(ParamId, areaInfo.Id)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, updateParams);

//            AreaManager.ClearCache();
//        }

//        public void UpdateTaxis(int selectedId, bool isSubtract)
//        {
//            if (isSubtract)
//            {
//                TaxisSubtract(selectedId);
//            }
//            else
//            {
//                TaxisAdd(selectedId);
//            }
//        }

//        public void UpdateCountOfAdmin()
//        {
//            var areaIdList = AreaManager.GetAreaIdList();
//            foreach (var areaId in areaIdList)
//            {
//                var count = DataProvider.Administrator.GetCountByAreaId(areaId);
//                string sqlString = $"UPDATE {TableName} SET CountOfAdmin = {count} WHERE Id = {areaId}";
//                DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//            }
//            AreaManager.ClearCache();
//        }

//        public void DeleteById(int areaId)
//        {
//            var areaInfo = GetAreaInfo(areaId);
//            if (areaInfo != null)
//            {
//                var areaIdList = new List<int>();
//                if (areaInfo.ChildrenCount > 0)
//                {
//                    areaIdList = GetIdListForDescendant(areaId);
//                }
//                areaIdList.Add(areaId);

//                string sqlString =
//                    $"DELETE FROM siteserver_Area WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(areaIdList)})";

//                int deletedNum;

//                using (var conn = GetConnection())
//                {
//                    conn.Open();
//                    using (var trans = conn.BeginTransaction())
//                    {
//                        try
//                        {
//                            deletedNum = DatabaseApi.ExecuteNonQuery(trans, sqlString);

//                            if (deletedNum > 0)
//                            {
//                                string sqlStringTaxis =
//                                    $"UPDATE siteserver_Area SET Taxis = Taxis - {deletedNum} WHERE (Taxis > {areaInfo.Taxis})";
//                                DatabaseApi.ExecuteNonQuery(trans, sqlStringTaxis);
//                            }

//                            trans.Commit();
//                        }
//                        catch
//                        {
//                            trans.Rollback();
//                            throw;
//                        }
//                    }
//                }
//                UpdateIsLastNode(areaInfo.ParentId);
//                UpdateSubtractChildrenCount(areaInfo.ParentsPath, deletedNum);
//            }

//            AreaManager.ClearCache();
//        }

//        private AreaInfo GetAreaInfo(int areaId)
//        {
//            AreaInfo areaInfo = null;

//            IDataParameter[] parameters = {
//                GetParameter(ParamId, areaId)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelect, parameters))
//            {
//                if (rdr.Read())
//                {
//                    var i = 0;
//                    areaInfo = new AreaInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i));
//                }
//                rdr.Close();
//            }
//            return areaInfo;
//        }

//        private List<AreaInfo> GetAreaInfoList()
//        {
//            var list = new List<AreaInfo>();

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAll))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var areaInfo = new AreaInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i));
//                    list.Add(areaInfo);
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        public List<int> GetIdListByParentId(int parentId)
//        {
//            var sqlString = $@"SELECT Id FROM siteserver_Area WHERE ParentID = '{parentId}' ORDER BY Taxis";
//            var list = new List<int>();

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    list.Add(DatabaseApi.GetInt(rdr, 0));
//                }
//                rdr.Close();
//            }

//            return list;
//        }

//        public List<int> GetIdListForDescendant(int areaId)
//        {
//            string sqlString = $@"SELECT Id
//FROM siteserver_Area
//WHERE (ParentsPath LIKE '{areaId},%') OR
//      (ParentsPath LIKE '%,{areaId},%') OR
//      (ParentsPath LIKE '%,{areaId}') OR
//      (ParentID = {areaId})
//";
//            var list = new List<int>();

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var theId = DatabaseApi.GetInt(rdr, 0);
//                    list.Add(theId);
//                }
//                rdr.Close();
//            }

//            return list;
//        }

//        public List<KeyValuePair<int, AreaInfo>> GetAreaInfoPairList()
//        {
//            var pairList = new List<KeyValuePair<int, AreaInfo>>();

//            var areaInfoList = GetAreaInfoList();
//            foreach (var areaInfo in areaInfoList)
//            {
//                var pair = new KeyValuePair<int, AreaInfo>(areaInfo.Id, areaInfo);
//                pairList.Add(pair);
//            }

//            return pairList;
//        }
//    }
//}
