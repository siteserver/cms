using System.Collections.Generic;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Repositories
{
    public class DepartmentRepository : GenericRepository<DepartmentInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(DepartmentInfo.Id);
            public const string ParentId = nameof(DepartmentInfo.ParentId);
            public const string ParentsPath = nameof(DepartmentInfo.ParentsPath);
            public const string ChildrenCount = nameof(DepartmentInfo.ChildrenCount);
            public const string Taxis = nameof(DepartmentInfo.Taxis);
            public const string IsLastNode = "IsLastNode";
            public const string CountOfAdmin = nameof(DepartmentInfo.CountOfAdmin);
        }

        private void Insert(DepartmentInfo parentInfo, DepartmentInfo departmentInfo)
        {
            if (parentInfo != null)
            {
                departmentInfo.ParentsPath = parentInfo.ParentsPath + "," + parentInfo.Id;
                departmentInfo.ParentsCount = parentInfo.ParentsCount + 1;

                var maxTaxis = GetMaxTaxisByParentPath(departmentInfo.ParentsPath);
                if (maxTaxis == 0)
                {
                    maxTaxis = parentInfo.Taxis;
                }
                departmentInfo.Taxis = maxTaxis + 1;
            }
            else
            {
                departmentInfo.ParentsPath = "0";
                departmentInfo.ParentsCount = 0;
                var maxTaxis = GetMaxTaxisByParentPath("0");
                departmentInfo.Taxis = maxTaxis + 1;
            }

            departmentInfo.ChildrenCount = 0;
            departmentInfo.LastNode = true;

            //string sqlString =
            //    $"UPDATE siteserver_Department SET Taxis = {SqlDifferences.ColumnIncrement("Taxis")} WHERE (Taxis >= {departmentInfo.Taxis})";
            //DatabaseApi.Instance.ExecuteNonQuery(trans, sqlString);

            IncrementAll(Attr.Taxis, Q
                .Where(Attr.Taxis, ">=", departmentInfo.Taxis));

            //var sqlInsert = "INSERT INTO siteserver_Department (DepartmentName, Code, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, AddDate, Summary, CountOfAdmin) VALUES (@DepartmentName, @Code, @ParentID, @ParentsPath, @ParentsCount, @ChildrenCount, @IsLastNode, @Taxis, @AddDate, @Summary, @CountOfAdmin)";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamName, departmentInfo.DepartmentName),
            //    GetParameter(ParamCode, departmentInfo.Code),
            //    GetParameter(ParamParentId, departmentInfo.ParentId),
            //    GetParameter(ParamParentsPath, departmentInfo.ParentsPath),
            //    GetParameter(ParamParentsCount, departmentInfo.ParentsCount),
            //    GetParameter(ParamChildrenCount, 0),
            //    GetParameter(ParamIsLastNode, true.ToString()),
            //    GetParameter(ParamTaxis, departmentInfo.Taxis),
            //    GetParameter(ParamAddDate,departmentInfo.AddDate),
            //    GetParameter(ParamSummary, departmentInfo.Summary),
            //    GetParameter(ParamCountOfAdmin, departmentInfo.CountOfAdmin)
            //};

            //departmentInfo.Id = DatabaseApi.Instance.ExecuteNonQueryAndReturnId(TableName, nameof(DepartmentInfo.Id), trans, sqlInsert, parameters);

            departmentInfo.Id = InsertObject(departmentInfo);

            if (!string.IsNullOrEmpty(departmentInfo.ParentsPath))
            {
                //sqlString = $"UPDATE siteserver_Department SET ChildrenCount = {SqlDifferences.ColumnIncrement("ChildrenCount")} WHERE Id IN ({AttackUtils.FilterSql(departmentInfo.ParentsPath)})";

                //DatabaseApi.Instance.ExecuteNonQuery(trans, sqlString);

                IncrementAll(Attr.ChildrenCount, Q
                    .WhereIn(Attr.Id, TranslateUtils.StringCollectionToIntList(departmentInfo.ParentsPath)));
            }

            //sqlString = $"UPDATE siteserver_Department SET IsLastNode = '{false}' WHERE ParentID = {departmentInfo.ParentId}";

            //DatabaseApi.Instance.ExecuteNonQuery(trans, sqlString);
            
            UpdateAll(Q
                .Set(Attr.IsLastNode, false.ToString())
                .Where(Attr.ParentId, departmentInfo.ParentId)
            );

            //sqlString =
            //    $"UPDATE siteserver_Department SET IsLastNode = '{true}' WHERE (Id IN (SELECT TOP 1 Id FROM siteserver_Department WHERE ParentID = {departmentInfo.ParentId} ORDER BY Taxis DESC))";

            //sqlString =
            //    $"UPDATE siteserver_Department SET IsLastNode = '{true}' WHERE (Id IN ({SqlUtils.ToInTopSqlString("siteserver_Department", new List<string> { nameof(DepartmentInfo.Id) }, $"WHERE ParentID = {departmentInfo.ParentId}", "ORDER BY Taxis DESC", 1)}))";

            //DatabaseApi.Instance.ExecuteNonQuery(trans, sqlString);

            var topId = GetValue<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, departmentInfo.ParentId)
                .OrderByDesc(Attr.Taxis));

            if (topId > 0)
            {
                UpdateAll(Q
                    .Set(Attr.IsLastNode, true.ToString())
                    .Where(Attr.Id, topId)
                );
            }

            DepartmentManager.ClearCache();
        }

        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
        {
            if (!string.IsNullOrEmpty(parentsPath))
            {
                //var sqlString = string.Concat("UPDATE siteserver_Department SET ChildrenCount = ChildrenCount - ", subtractNum, " WHERE Id IN (", AttackUtils.FilterSql(parentsPath), ")");
                //DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, sqlString);

                DecrementAll(Attr.ChildrenCount, Q
                    .WhereIn(Attr.Id, TranslateUtils.StringCollectionToIntList(parentsPath)), subtractNum);

                DepartmentManager.ClearCache();
            }
        }

        private void TaxisSubtract(int selectedId)
        {
            var departmentInfo = GetDepartmentInfo(selectedId);
            if (departmentInfo == null) return;

            //Get Lower Taxis and Id
            //int lowerId;
            //int lowerChildrenCount;
            //string lowerParentsPath;

            //var sqlString = SqlDifferences.GetSqlString("siteserver_Department", new List<string>
            //    {
            //        nameof(DepartmentInfo.Id),
            //        nameof(DepartmentInfo.ChildrenCount),
            //        nameof(DepartmentInfo.ParentsPath)
            //    },
            //    "WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis < @Taxis)",
            //    "ORDER BY Taxis DESC", 1);

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamParentId, departmentInfo.ParentId),
            //    GetParameter(ParamId, departmentInfo.Id),
            //    GetParameter(ParamTaxis, departmentInfo.Taxis),
            //};

            //using (var rdr = DatabaseApi.Instance.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        lowerId = DatabaseApi.Instance.GetInt(rdr, 0);
            //        lowerChildrenCount = DatabaseApi.Instance.GetInt(rdr, 1);
            //        lowerParentsPath = DatabaseApi.Instance.GetString(rdr, 2);
            //    }
            //    else
            //    {
            //        return;
            //    }
            //    rdr.Close();
            //}

            var result = GetValue<(int Id, int ChildrenCount, string ParentsPath)?>(Q
                .Select(
                    Attr.Id,
                    Attr.ChildrenCount,
                    Attr.ParentsPath)
                .Where(Attr.ParentId, departmentInfo.ParentId)
                .WhereNot(Attr.Id, departmentInfo.Id)
                .Where(Attr.Taxis, "<", departmentInfo.Taxis)
                .OrderByDesc(Attr.Taxis));

            if (result == null) return;

            var lowerId = result.Value.Id;
            var lowerChildrenCount = result.Value.ChildrenCount;
            var lowerParentsPath = result.Value.ParentsPath;

            var lowerNodePath = string.Concat(lowerParentsPath, ",", lowerId);
            var selectedNodePath = string.Concat(departmentInfo.ParentsPath, ",", departmentInfo.Id);

            SetTaxisSubtract(selectedId, selectedNodePath, lowerChildrenCount + 1);
            SetTaxisAdd(lowerId, lowerNodePath, departmentInfo.ChildrenCount + 1);

            UpdateIsLastNode(departmentInfo.ParentId);
        }

        private void TaxisAdd(int selectedId)
        {
            var departmentInfo = GetDepartmentInfo(selectedId);
            if (departmentInfo == null) return;
            //Get Higher Taxis and Id
            //int higherId;
            //int higherChildrenCount;
            //string higherParentsPath;

            //var sqlString = SqlDifferences.GetSqlString("siteserver_Department", new List<string>
            //    {
            //        nameof(DepartmentInfo.Id),
            //        nameof(DepartmentInfo.ChildrenCount),
            //        nameof(DepartmentInfo.ParentsPath)
            //    },
            //    "WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis > @Taxis)",
            //    "ORDER BY Taxis", 1);

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamParentId, departmentInfo.ParentId),
            //    GetParameter(ParamId, departmentInfo.Id),
            //    GetParameter(ParamTaxis, departmentInfo.Taxis)
            //};

            //using (var rdr = DatabaseApi.Instance.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        higherId = DatabaseApi.Instance.GetInt(rdr, 0);
            //        higherChildrenCount = DatabaseApi.Instance.GetInt(rdr, 1);
            //        higherParentsPath = DatabaseApi.Instance.GetString(rdr, 2);
            //    }
            //    else
            //    {
            //        return;
            //    }
            //    rdr.Close();
            //}

            var dataInfo = GetObject(Q
                .Select(Attr.Id, Attr.ChildrenCount, Attr.ParentsPath)
                .Where(Attr.ParentId, departmentInfo.ParentId)
                .WhereNot(Attr.Id, departmentInfo.Id)
                .Where(Attr.Taxis, ">", departmentInfo.Taxis)
                .OrderBy(Attr.Taxis));

            if (dataInfo == null) return;

            var higherId = dataInfo.Id;
            var higherChildrenCount = dataInfo.ChildrenCount;
            var higherParentsPath = dataInfo.ParentsPath;

            var higherNodePath = string.Concat(higherParentsPath, ",", higherId);
            var selectedNodePath = string.Concat(departmentInfo.ParentsPath, ",", departmentInfo.Id);

            SetTaxisAdd(selectedId, selectedNodePath, higherChildrenCount + 1);
            SetTaxisSubtract(higherId, higherNodePath, departmentInfo.ChildrenCount + 1);

            UpdateIsLastNode(departmentInfo.ParentId);
        }

        private void SetTaxisAdd(int id, string parentsPath, int addNum)
        {
            //var path = AttackUtils.FilterSql(parentsPath);
            //var sqlString =
            //    $"UPDATE siteserver_Department SET Taxis = Taxis + {addNum} WHERE Id = {id} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

            //DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, sqlString);

            IncrementAll(Attr.Taxis, Q
                .Where(Attr.Id, id)
                .OrWhere(Attr.ParentsPath, parentsPath)
                .OrWhereStarts(Attr.ParentsPath, $"{parentsPath},"), addNum);

            DepartmentManager.ClearCache();
        }

        private void SetTaxisSubtract(int id, string parentsPath, int subtractNum)
        {
            //var path = AttackUtils.FilterSql(parentsPath);
            //var sqlString =
            //    $"UPDATE siteserver_Department SET Taxis = Taxis - {subtractNum} WHERE  Id = {id} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

            //DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, sqlString);

            DecrementAll(Attr.Taxis, Q
                .Where(Attr.Id, id)
                .OrWhere(Attr.ParentsPath, parentsPath)
                .OrWhereStarts(Attr.ParentsPath, $"{parentsPath},"), subtractNum);

            DepartmentManager.ClearCache();
        }

        private void UpdateIsLastNode(int parentId)
        {
            if (parentId <= 0) return;

            //var sqlString = "UPDATE siteserver_Department SET IsLastNode = @IsLastNode WHERE  ParentID = @ParentID";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamIsLastNode, false.ToString()),
            //    GetParameter(ParamParentId, parentId)
            //};

            //DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, sqlString, parameters);
            
            UpdateAll(Q
                .Set(Attr.IsLastNode, false.ToString())
                .Where(Attr.ParentId, parentId)
            );

            //sqlString =
            //    $"UPDATE siteserver_Department SET IsLastNode = '{true}' WHERE (Id IN ({SqlUtils.ToInTopSqlString("siteserver_Department", new List<string> { nameof(DepartmentInfo.Id) }, $"WHERE ParentID = {parentId}", "ORDER BY Taxis DESC", 1)}))";

            //DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, sqlString);

            var topId = GetValue<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderByDesc(Attr.Taxis));

            if (topId > 0)
            {
                UpdateAll(Q
                    .Set(Attr.IsLastNode, true.ToString())
                    .Where(Attr.Id, topId)
                );
            }
        }

        private int GetMaxTaxisByParentPath(string parentPath)
        {
            //parentPath = AttackUtils.FilterSql(parentPath);
            //var sqlString = string.Concat("SELECT MAX(Taxis) AS MaxTaxis FROM siteserver_Department WHERE (ParentsPath = '", parentPath, "') OR (ParentsPath LIKE '", parentPath, ",%')");
            //var maxTaxis = 0;

            //using (var rdr = DatabaseApi.Instance.ExecuteReader(ConnectionString, sqlString))
            //{
            //    if (rdr.Read())
            //    {
            //        maxTaxis = DatabaseApi.Instance.GetInt(rdr, 0);
            //    }
            //    rdr.Close();
            //}
            //return maxTaxis;

            return Max(Attr.Taxis, Q
                .Where(Attr.ParentsPath, parentPath)
                .OrWhereStarts(Attr.ParentsPath, $"{parentPath},"));
        }

        public void Insert(DepartmentInfo departmentInfo)
        {
            var parentDepartmentInfo = GetDepartmentInfo(departmentInfo.ParentId);

            Insert(parentDepartmentInfo, departmentInfo);

            DepartmentManager.ClearCache();
        }

        public void Update(DepartmentInfo departmentInfo)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamName, departmentInfo.DepartmentName),
            //    GetParameter(ParamCode, departmentInfo.Code),
            //    GetParameter(ParamParentsPath, departmentInfo.ParentsPath),
            //    GetParameter(ParamParentsCount, departmentInfo.ParentsCount),
            //    GetParameter(ParamChildrenCount, departmentInfo.ChildrenCount),
            //    GetParameter(ParamIsLastNode, departmentInfo.IsLastNode.ToString()),
            //    GetParameter(ParamSummary, departmentInfo.Summary),
            //    GetParameter(ParamCountOfAdmin, departmentInfo.CountOfAdmin),
            //    GetParameter(ParamId, departmentInfo.Id)
            //};

            //string SqlUpdate = "UPDATE siteserver_Department SET DepartmentName = @DepartmentName, Code = @Code, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildrenCount = @ChildrenCount, IsLastNode = @IsLastNode, Summary = @Summary, CountOfAdmin = @CountOfAdmin WHERE Id = @Id";
            //DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);

            UpdateObject(departmentInfo);

            DepartmentManager.ClearCache();
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
            var idList = DepartmentManager.GetDepartmentIdList();
            foreach (var departmentId in idList)
            {
                var count = DataProvider.Administrator.GetCountByDepartmentId(departmentId);
                //string sqlString = $"UPDATE {TableName} SET CountOfAdmin = {count} WHERE Id = {departmentId}";
                //DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, sqlString);
                
                UpdateAll(Q
                    .Set(Attr.CountOfAdmin, count)
                    .Where(Attr.Id, departmentId)
                );
            }
            DepartmentManager.ClearCache();
        }

        public void Delete(int id)
        {
            var departmentInfo = GetDepartmentInfo(id);
            if (departmentInfo != null)
            {
                IList<int> idList = new List<int>();
                if (departmentInfo.ChildrenCount > 0)
                {
                    idList = GetIdListForDescendant(id);
                }
                idList.Add(id);

                //string sqlString =
                //    $"DELETE FROM siteserver_Department WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

                //deletedNum = DatabaseApi.Instance.ExecuteNonQuery(trans, sqlString);

                var deletedNum = DeleteAll(Q
                    .WhereIn(Attr.Id, idList));

                if (deletedNum > 0)
                {
                    //string sqlStringTaxis =
                    //    $"UPDATE siteserver_Department SET Taxis = Taxis - {deletedNum} WHERE (Taxis > {departmentInfo.Taxis})";
                    //DatabaseApi.Instance.ExecuteNonQuery(trans, sqlStringTaxis);

                    DecrementAll(Attr.Taxis, Q
                        .Where(Attr.Taxis, ">", departmentInfo.Taxis), deletedNum);
                }

                UpdateIsLastNode(departmentInfo.ParentId);
                UpdateSubtractChildrenCount(departmentInfo.ParentsPath, deletedNum);
            }

            DepartmentManager.ClearCache();
        }

        private DepartmentInfo GetDepartmentInfo(int id)
        {
            //DepartmentInfo departmentInfo = null;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamId, id)
            //};

            //string SqlSelect = "SELECT Id, DepartmentName, Code, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, AddDate, Summary, CountOfAdmin FROM siteserver_Department WHERE Id = @Id";
            //using (var rdr = DatabaseApi.Instance.ExecuteReader(ConnectionString, SqlSelect, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        var i = 0;
            //        departmentInfo = new DepartmentInfo(DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i++), TranslateUtils.ToBool(DatabaseApi.Instance.GetString(rdr, i++)), DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetDateTime(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i));
            //    }
            //    rdr.Close();
            //}
            //return departmentInfo;

            return GetObjectById(id);
        }

        private IList<DepartmentInfo> GetDepartmentInfoList()
        {
            //var list = new List<DepartmentInfo>();

            //string SqlSelectAll = "SELECT Id, DepartmentName, Code, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, AddDate, Summary, CountOfAdmin FROM siteserver_Department ORDER BY TAXIS";
            //using (var rdr = DatabaseApi.Instance.ExecuteReader(ConnectionString, SqlSelectAll))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var departmentInfo = new DepartmentInfo(DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i++), TranslateUtils.ToBool(DatabaseApi.Instance.GetString(rdr, i++)), DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetDateTime(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i));
            //        list.Add(departmentInfo);
            //    }
            //    rdr.Close();
            //}
            //return list;

            return GetObjectList(Q
                .OrderBy(Attr.Taxis));
        }

        public IList<int> GetIdListByParentId(int parentId)
        {
            //var sqlString =
            //    $@"SELECT Id FROM siteserver_Department WHERE ParentID = '{parentId}' ORDER BY Taxis";
            //var list = new List<int>();

            //using (var rdr = DatabaseApi.Instance.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var theId = DatabaseApi.Instance.GetInt(rdr, 0);
            //        list.Add(theId);
            //    }
            //    rdr.Close();
            //}

            //return list;

            return GetValueList<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis));
        }

        private IList<int> GetIdListForDescendant(int id)
        {
//            string sqlString = $@"SELECT Id
//FROM siteserver_Department
//WHERE (ParentsPath LIKE '{id},%') OR
//      (ParentsPath LIKE '%,{id},%') OR
//      (ParentsPath LIKE '%,{id}') OR
//      (ParentID = {id})
//";
//            var list = new List<int>();

//            using (var rdr = DatabaseApi.Instance.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var theId = DatabaseApi.Instance.GetInt(rdr, 0);
//                    list.Add(theId);
//                }
//                rdr.Close();
//            }

//            return list;

            return GetValueList<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, id)
                .OrWhereStarts(Attr.ParentsPath, $"{id},")
                .OrWhereContains(Attr.ParentsPath, $",{id},")
                .OrWhereEnds(Attr.ParentsPath, $",{id}"));
        }

        public List<KeyValuePair<int, DepartmentInfo>> GetDepartmentInfoKeyValuePair()
        {
            var list = new List<KeyValuePair<int, DepartmentInfo>>();

            var departmentInfoList = GetDepartmentInfoList();
            foreach (var departmentInfo in departmentInfoList)
            {
                var pair = new KeyValuePair<int, DepartmentInfo>(departmentInfo.Id, departmentInfo);
                list.Add(pair);
            }

            return list;
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
//    public class Department : DataProviderBase
//	{
//        public override string TableName => "siteserver_Department";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(DepartmentInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(DepartmentInfo.DepartmentName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(DepartmentInfo.Code),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(DepartmentInfo.ParentId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(DepartmentInfo.ParentsPath),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(DepartmentInfo.ParentsCount),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(DepartmentInfo.ChildrenCount),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(DepartmentInfo.IsLastNode),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(DepartmentInfo.Taxis),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(DepartmentInfo.AddDate),
//                DataType = DataType.DateTime
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(DepartmentInfo.Summary),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(DepartmentInfo.CountOfAdmin),
//                DataType = DataType.Integer
//            }
//        };

//        private const string SqlSelect = "SELECT Id, DepartmentName, Code, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, AddDate, Summary, CountOfAdmin FROM siteserver_Department WHERE Id = @Id";

//        private const string SqlSelectAll = "SELECT Id, DepartmentName, Code, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, AddDate, Summary, CountOfAdmin FROM siteserver_Department ORDER BY TAXIS";

//        private const string SqlUpdate = "UPDATE siteserver_Department SET DepartmentName = @DepartmentName, Code = @Code, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildrenCount = @ChildrenCount, IsLastNode = @IsLastNode, Summary = @Summary, CountOfAdmin = @CountOfAdmin WHERE Id = @Id";

//        private const string ParamId = "@Id";
//        private const string ParamName = "@DepartmentName";
//        private const string ParamCode = "@Code";
//        private const string ParamParentId = "@ParentID";
//        private const string ParamParentsPath = "@ParentsPath";
//        private const string ParamParentsCount = "@ParentsCount";
//        private const string ParamChildrenCount = "@ChildrenCount";
//        private const string ParamIsLastNode = "@IsLastNode";
//        private const string ParamTaxis = "@Taxis";
//        private const string ParamAddDate = "@AddDate";
//        private const string ParamSummary = "@Summary";
//        private const string ParamCountOfAdmin = "@CountOfAdmin";

//        private void InsertWithTrans(DepartmentInfo parentInfo, DepartmentInfo departmentInfo, IDbTransaction trans)
//        {
//            if (parentInfo != null)
//            {
//                departmentInfo.ParentsPath = parentInfo.ParentsPath + "," + parentInfo.Id;
//                departmentInfo.ParentsCount = parentInfo.ParentsCount + 1;

//                var maxTaxis = GetMaxTaxisByParentPath(departmentInfo.ParentsPath);
//                if (maxTaxis == 0)
//                {
//                    maxTaxis = parentInfo.Taxis;
//                }
//                departmentInfo.Taxis = maxTaxis + 1;
//            }
//            else
//            {
//                departmentInfo.ParentsPath = "0";
//                departmentInfo.ParentsCount = 0;
//                var maxTaxis = GetMaxTaxisByParentPath("0");
//                departmentInfo.Taxis = maxTaxis + 1;
//            }

//            var sqlInsert = "INSERT INTO siteserver_Department (DepartmentName, Code, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, AddDate, Summary, CountOfAdmin) VALUES (@DepartmentName, @Code, @ParentID, @ParentsPath, @ParentsCount, @ChildrenCount, @IsLastNode, @Taxis, @AddDate, @Summary, @CountOfAdmin)";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamName, departmentInfo.DepartmentName),
//                GetParameter(ParamCode, departmentInfo.Code),
//				GetParameter(ParamParentId, departmentInfo.ParentId),
//				GetParameter(ParamParentsPath, departmentInfo.ParentsPath),
//				GetParameter(ParamParentsCount, departmentInfo.ParentsCount),
//				GetParameter(ParamChildrenCount, 0),
//				GetParameter(ParamIsLastNode, true.ToString()),
//				GetParameter(ParamTaxis, departmentInfo.Taxis),
//				GetParameter(ParamAddDate,departmentInfo.AddDate),
//				GetParameter(ParamSummary, departmentInfo.Summary),
//				GetParameter(ParamCountOfAdmin, departmentInfo.CountOfAdmin)
//			};

//            string sqlString =
//                $"UPDATE siteserver_Department SET Taxis = {SqlDifferences.ColumnIncrement("Taxis")} WHERE (Taxis >= {departmentInfo.Taxis})";
//            DatabaseApi.Instance.ExecuteNonQuery(trans, sqlString);

//            departmentInfo.Id = DatabaseApi.Instance.ExecuteNonQueryAndReturnId(TableName, nameof(DepartmentInfo.Id), trans, sqlInsert, parameters);

//            if (!string.IsNullOrEmpty(departmentInfo.ParentsPath))
//            {
//                sqlString = $"UPDATE siteserver_Department SET ChildrenCount = {SqlDifferences.ColumnIncrement("ChildrenCount")} WHERE Id IN ({AttackUtils.FilterSql(departmentInfo.ParentsPath)})";

//                DatabaseApi.Instance.ExecuteNonQuery(trans, sqlString);
//            }

//            sqlString = $"UPDATE siteserver_Department SET IsLastNode = '{false}' WHERE ParentID = {departmentInfo.ParentId}";

//            DatabaseApi.Instance.ExecuteNonQuery(trans, sqlString);

//            //sqlString =
//            //    $"UPDATE siteserver_Department SET IsLastNode = '{true}' WHERE (Id IN (SELECT TOP 1 Id FROM siteserver_Department WHERE ParentID = {departmentInfo.ParentId} ORDER BY Taxis DESC))";

//            sqlString =
//                $"UPDATE siteserver_Department SET IsLastNode = '{true}' WHERE (Id IN ({SqlUtils.ToInTopSqlString("siteserver_Department", new List<string> { nameof(DepartmentInfo.Id) }, $"WHERE ParentID = {departmentInfo.ParentId}", "ORDER BY Taxis DESC", 1)}))";

//            DatabaseApi.Instance.ExecuteNonQuery(trans, sqlString);

//            DepartmentManager.ClearCache();
//        }

//        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
//        {
//            if (!string.IsNullOrEmpty(parentsPath))
//            {
//                var sqlString = string.Concat("UPDATE siteserver_Department SET ChildrenCount = ChildrenCount - ", subtractNum, " WHERE Id IN (", AttackUtils.FilterSql(parentsPath) , ")");
//                DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, sqlString);

//                DepartmentManager.ClearCache();
//            }
//        }

//        private void TaxisSubtract(int selectedId)
//        {
//            var departmentInfo = GetDepartmentInfo(selectedId);
//            if (departmentInfo == null) return;
//            //Get Lower Taxis and Id
//            int lowerId;
//            int lowerChildrenCount;
//            string lowerParentsPath;

//            var sqlString = SqlDifferences.GetSqlString("siteserver_Department", new List<string>
//                {
//                    nameof(DepartmentInfo.Id),
//                    nameof(DepartmentInfo.ChildrenCount),
//                    nameof(DepartmentInfo.ParentsPath)
//                }, 
//                "WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis < @Taxis)",
//                "ORDER BY Taxis DESC", 1);

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamParentId, departmentInfo.ParentId),
//				GetParameter(ParamId, departmentInfo.Id),
//				GetParameter(ParamTaxis, departmentInfo.Taxis),
//			};

//            using (var rdr = DatabaseApi.Instance.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    lowerId = DatabaseApi.Instance.GetInt(rdr, 0);
//                    lowerChildrenCount = DatabaseApi.Instance.GetInt(rdr, 1);
//                    lowerParentsPath = DatabaseApi.Instance.GetString(rdr, 2);
//                }
//                else
//                {
//                    return;
//                }
//                rdr.Close();
//            }


//            var lowerNodePath = string.Concat(lowerParentsPath, ",", lowerId);
//            var selectedNodePath = string.Concat(departmentInfo.ParentsPath, ",", departmentInfo.Id);

//            SetTaxisSubtract(selectedId, selectedNodePath, lowerChildrenCount + 1);
//            SetTaxisAdd(lowerId, lowerNodePath, departmentInfo.ChildrenCount + 1);

//            UpdateIsLastNode(departmentInfo.ParentId);
//        }

//        private void TaxisAdd(int selectedId)
//        {
//            var departmentInfo = GetDepartmentInfo(selectedId);
//            if (departmentInfo == null) return;
//            //Get Higher Taxis and Id
//            int higherId;
//            int higherChildrenCount;
//            string higherParentsPath;

//            var sqlString = SqlDifferences.GetSqlString("siteserver_Department", new List<string>
//                {
//                    nameof(DepartmentInfo.Id),
//                    nameof(DepartmentInfo.ChildrenCount),
//                    nameof(DepartmentInfo.ParentsPath)
//                }, 
//                "WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis > @Taxis)",
//                "ORDER BY Taxis", 1);

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamParentId, departmentInfo.ParentId),
//				GetParameter(ParamId, departmentInfo.Id),
//				GetParameter(ParamTaxis, departmentInfo.Taxis)
//			};

//            using (var rdr = DatabaseApi.Instance.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    higherId = DatabaseApi.Instance.GetInt(rdr, 0);
//                    higherChildrenCount = DatabaseApi.Instance.GetInt(rdr, 1);
//                    higherParentsPath = DatabaseApi.Instance.GetString(rdr, 2);
//                }
//                else
//                {
//                    return;
//                }
//                rdr.Close();
//            }


//            var higherNodePath = string.Concat(higherParentsPath, ",", higherId);
//            var selectedNodePath = string.Concat(departmentInfo.ParentsPath, ",", departmentInfo.Id);

//            SetTaxisAdd(selectedId, selectedNodePath, higherChildrenCount + 1);
//            SetTaxisSubtract(higherId, higherNodePath, departmentInfo.ChildrenCount + 1);

//            UpdateIsLastNode(departmentInfo.ParentId);
//        }

//        private void SetTaxisAdd(int id, string parentsPath, int addNum)
//        {
//            var path = AttackUtils.FilterSql(parentsPath);
//            var sqlString =
//                $"UPDATE siteserver_Department SET Taxis = Taxis + {addNum} WHERE Id = {id} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

//            DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, sqlString);

//            DepartmentManager.ClearCache();
//        }

//        private void SetTaxisSubtract(int id, string parentsPath, int subtractNum)
//        {
//            var path = AttackUtils.FilterSql(parentsPath);
//            var sqlString =
//                $"UPDATE siteserver_Department SET Taxis = Taxis - {subtractNum} WHERE  Id = {id} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

//            DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, sqlString);

//            DepartmentManager.ClearCache();
//        }

//        private void UpdateIsLastNode(int parentId)
//        {
//            if (parentId <= 0) return;

//            var sqlString = "UPDATE siteserver_Department SET IsLastNode = @IsLastNode WHERE  ParentID = @ParentID";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamIsLastNode, false.ToString()),
//                GetParameter(ParamParentId, parentId)
//            };

//            DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            sqlString =
//                $"UPDATE siteserver_Department SET IsLastNode = '{true}' WHERE (Id IN ({SqlUtils.ToInTopSqlString("siteserver_Department", new List<string>{ nameof(DepartmentInfo.Id) }, $"WHERE ParentID = {parentId}", "ORDER BY Taxis DESC", 1)}))";

//            DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, sqlString);
//        }

//        private int GetMaxTaxisByParentPath(string parentPath)
//        {
//            parentPath = AttackUtils.FilterSql(parentPath);
//            var sqlString = string.Concat("SELECT MAX(Taxis) AS MaxTaxis FROM siteserver_Department WHERE (ParentsPath = '", parentPath, "') OR (ParentsPath LIKE '", parentPath, ",%')");
//            var maxTaxis = 0;

//            using (var rdr = DatabaseApi.Instance.ExecuteReader(ConnectionString, sqlString))
//            {
//                if (rdr.Read())
//                {
//                    maxTaxis = DatabaseApi.Instance.GetInt(rdr, 0);
//                }
//                rdr.Close();
//            }
//            return maxTaxis;
//        }

//	    public int InsertObject(DepartmentInfo departmentInfo)
//        {
//            using (var conn = GetConnection())
//            {
//                conn.Open();
//                using (var trans = conn.BeginTransaction())
//                {
//                    try
//                    {
//                        var parentDepartmentInfo = GetDepartmentInfo(departmentInfo.ParentId);

//                        InsertWithTrans(parentDepartmentInfo, departmentInfo, trans);

//                        trans.Commit();
//                    }
//                    catch
//                    {
//                        trans.Rollback();
//                        throw;
//                    }
//                }
//            }

//            DepartmentManager.ClearCache();

//            return departmentInfo.Id;
//        }

//        public void UpdateObject(DepartmentInfo departmentInfo)
//        {
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamName, departmentInfo.DepartmentName),
//                GetParameter(ParamCode, departmentInfo.Code),
//				GetParameter(ParamParentsPath, departmentInfo.ParentsPath),
//				GetParameter(ParamParentsCount, departmentInfo.ParentsCount),
//				GetParameter(ParamChildrenCount, departmentInfo.ChildrenCount),
//				GetParameter(ParamIsLastNode, departmentInfo.IsLastNode.ToString()),
//				GetParameter(ParamSummary, departmentInfo.Summary),
//				GetParameter(ParamCountOfAdmin, departmentInfo.CountOfAdmin),
//				GetParameter(ParamId, departmentInfo.Id)
//			};

//            DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);

//            DepartmentManager.ClearCache();
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
//            var idList = DepartmentManager.GetDepartmentIdList();
//            foreach (var departmentId in idList)
//            {
//                var count = DataProvider.Administrator.GetCountByDepartmentId(departmentId);
//                string sqlString = $"UPDATE {TableName} SET CountOfAdmin = {count} WHERE Id = {departmentId}";
//                DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, sqlString);
//            }
//            DepartmentManager.ClearCache();
//        }

//        public void DeleteById(int id)
//        {
//            var departmentInfo = GetDepartmentInfo(id);
//            if (departmentInfo != null)
//            {
//                var idList = new List<int>();
//                if (departmentInfo.ChildrenCount > 0)
//                {
//                    idList = GetIdListForDescendant(id);
//                }
//                idList.Add(id);

//                string sqlString =
//                    $"DELETE FROM siteserver_Department WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

//                int deletedNum;

//                using (var conn = GetConnection())
//                {
//                    conn.Open();
//                    using (var trans = conn.BeginTransaction())
//                    {
//                        try
//                        {
//                            deletedNum = DatabaseApi.Instance.ExecuteNonQuery(trans, sqlString);

//                            if (deletedNum > 0)
//                            {
//                                string sqlStringTaxis =
//                                    $"UPDATE siteserver_Department SET Taxis = Taxis - {deletedNum} WHERE (Taxis > {departmentInfo.Taxis})";
//                                DatabaseApi.Instance.ExecuteNonQuery(trans, sqlStringTaxis);
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
//                UpdateIsLastNode(departmentInfo.ParentId);
//                UpdateSubtractChildrenCount(departmentInfo.ParentsPath, deletedNum);
//            }

//            DepartmentManager.ClearCache();
//        }

//        private DepartmentInfo GetDepartmentInfo(int id)
//		{
//            DepartmentInfo departmentInfo = null;

//			IDataParameter[] parameters =
//			{
//				GetParameter(ParamId, id)
//			};

//            using (var rdr = DatabaseApi.Instance.ExecuteReader(ConnectionString, SqlSelect, parameters)) 
//			{
//				if (rdr.Read())
//				{
//				    var i = 0;
//                    departmentInfo = new DepartmentInfo(DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i++), TranslateUtils.ToBool(DatabaseApi.Instance.GetString(rdr, i++)), DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetDateTime(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i));
//				}
//				rdr.Close();
//			}
//            return departmentInfo;
//		}

//        private List<DepartmentInfo> GetDepartmentInfoList()
//        {
//            var list = new List<DepartmentInfo>();

//            using (var rdr = DatabaseApi.Instance.ExecuteReader(ConnectionString, SqlSelectAll))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var departmentInfo = new DepartmentInfo(DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i++), TranslateUtils.ToBool(DatabaseApi.Instance.GetString(rdr, i++)), DatabaseApi.Instance.GetInt(rdr, i++), DatabaseApi.Instance.GetDateTime(rdr, i++), DatabaseApi.Instance.GetString(rdr, i++), DatabaseApi.Instance.GetInt(rdr, i));
//                    list.Add(departmentInfo);
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        public List<int> GetIdListByParentId(int parentId)
//        {
//            var sqlString =
//                $@"SELECT Id FROM siteserver_Department WHERE ParentID = '{parentId}' ORDER BY Taxis";
//            var list = new List<int>();

//            using (var rdr = DatabaseApi.Instance.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var theId = DatabaseApi.Instance.GetInt(rdr, 0);
//                    list.Add(theId);
//                }
//                rdr.Close();
//            }

//            return list;
//        }

//		public List<int> GetIdListForDescendant(int id)
//		{
//            string sqlString = $@"SELECT Id
//FROM siteserver_Department
//WHERE (ParentsPath LIKE '{id},%') OR
//      (ParentsPath LIKE '%,{id},%') OR
//      (ParentsPath LIKE '%,{id}') OR
//      (ParentID = {id})
//";
//			var list = new List<int>();

//            using (var rdr = DatabaseApi.Instance.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var theId = DatabaseApi.Instance.GetInt(rdr, 0);
//                    list.Add(theId);
//                }
//                rdr.Close();
//            }

//			return list;
//		}

//        public List<KeyValuePair<int, DepartmentInfo>> GetDepartmentInfoKeyValuePair()
//        {
//            var list = new List<KeyValuePair<int, DepartmentInfo>>();

//            var departmentInfoList = GetDepartmentInfoList();
//            foreach (var departmentInfo in departmentInfoList)
//            {
//                var pair = new KeyValuePair<int, DepartmentInfo>(departmentInfo.Id, departmentInfo);
//                list.Add(pair);
//            }

//            return list;
//        }
//	}
//}
