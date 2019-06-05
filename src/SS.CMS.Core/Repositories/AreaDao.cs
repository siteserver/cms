using System.Collections.Generic;
using System.Linq;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class AreaDao : IDatabaseDao
    {
        private readonly Repository<AreaInfo> _repository;
        public AreaDao()
        {
            _repository = new Repository<AreaInfo>(AppSettings.DatabaseType, AppSettings.ConnectionString);
        }

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

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

        private int Insert(AreaInfo parentInfo, AreaInfo areaInfo)
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

            _repository.Increment(Attr.Taxis, Q
                    .Where(Attr.Taxis, ">=", areaInfo.Taxis)
                    );

            _repository.Insert(areaInfo);

            if (!string.IsNullOrEmpty(areaInfo.ParentsPath) && areaInfo.ParentsPath != "0")
            {
                _repository.Increment(Attr.ChildrenCount, Q
                    .WhereIn(Attr.Id, TranslateUtils.StringCollectionToIntList(areaInfo.ParentsPath)));
            }

            _repository.Update(Q
                .Set(Attr.IsLastNode, false.ToString())
                .Where(Attr.ParentId, areaInfo.ParentId)
            );

            var topId = _repository.Get<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, areaInfo.ParentId)
                .OrderByDesc(Attr.Taxis));

            if (topId > 0)
            {
                _repository.Update(Q
                    .Set(Attr.IsLastNode, true.ToString())
                    .Where(nameof(Attr.Id), topId)
                );
            }

            AreaManager.ClearCache();

            return areaInfo.Id;
        }

        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
        {
            if (string.IsNullOrEmpty(parentsPath)) return;

            _repository.Decrement(Attr.ChildrenCount, Q
                .WhereIn(Attr.Id, TranslateUtils.StringCollectionToIntList(parentsPath)), subtractNum);

            AreaManager.ClearCache();
        }

        private void TaxisSubtract(int selectedId)
        {
            var areaInfo = _repository.Get(selectedId);
            if (areaInfo == null) return;

            var dataInfo = _repository.Get(Q
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
            var areaInfo = _repository.Get(selectedId);
            if (areaInfo == null) return;

            var dataInfo = _repository.Get(Q
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
            _repository.Increment(Attr.Taxis, Q
                .Where(Attr.Id, areaId)
                .OrWhere(Attr.ParentsPath, parentsPath)
                .OrWhereStarts(Attr.ParentsPath, parentsPath + ","), addNum);

            AreaManager.ClearCache();
        }

        private void SetTaxisSubtract(int areaId, string parentsPath, int subtractNum)
        {
            _repository.Decrement(Attr.Taxis, Q
                .Where(Attr.Id, areaId)
                .OrWhere(Attr.ParentsPath, parentsPath)
                .OrWhereStarts(Attr.ParentsPath, parentsPath + ","), subtractNum);

            AreaManager.ClearCache();
        }

        private void UpdateIsLastNode(int parentId)
        {
            if (parentId <= 0) return;

            _repository.Update(Q
                .Set(Attr.IsLastNode, false.ToString())
                .Where(Attr.ParentId, parentId)
            );

            var topId = _repository.Get<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderByDesc(Attr.Taxis));

            if (topId > 0)
            {
                _repository.Update(Q
                    .Set(Attr.IsLastNode, true.ToString())
                    .Where(nameof(Attr.Id), topId)
                );
            }
        }

        private int GetMaxTaxisByParentPath(string parentPath)
        {
            return _repository.Max(Attr.Taxis, Q
                .Where(Attr.ParentsPath, parentPath)
                .OrWhereStarts(Attr.ParentsPath, parentPath + ",")) ?? 0;
        }

        public int Insert(AreaInfo areaInfo)
        {
            var parentAreaInfo = _repository.Get(areaInfo.ParentId);

            areaInfo.Id = Insert(parentAreaInfo, areaInfo);
            AreaManager.ClearCache();

            return areaInfo.Id;
        }

        public bool Update(AreaInfo areaInfo)
        {
            var updated = _repository.Update(areaInfo);
            if (updated)
            {
                AreaManager.ClearCache();
            }

            return updated;
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
                var count = DataProvider.AdministratorDao.GetCountByAreaId(areaId);

                _repository.Update(Q
                    .Set(Attr.CountOfAdmin, count)
                    .Where(nameof(Attr.Id), areaId)
                );
            }
            AreaManager.ClearCache();
        }

        public bool Delete(int areaId)
        {
            var areaInfo = _repository.Get(areaId);
            if (areaInfo != null)
            {
                IList<int> areaIdList = new List<int>();
                if (areaInfo.ChildrenCount > 0)
                {
                    areaIdList = GetIdListForDescendant(areaId);
                }
                areaIdList.Add(areaId);

                var deletedNum = _repository.Delete(Q
                    .WhereIn(Attr.Id, areaIdList));

                if (deletedNum > 0)
                {
                    _repository.Decrement(Attr.Taxis, Q
                        .Where(Attr.Taxis, ">", areaInfo.Taxis), deletedNum);
                }

                UpdateIsLastNode(areaInfo.ParentId);
                UpdateSubtractChildrenCount(areaInfo.ParentsPath, deletedNum);
            }

            AreaManager.ClearCache();

            return true;
        }

        private IList<AreaInfo> GetAreaInfoList()
        {
            return _repository.GetAll(Q
                .OrderBy(Attr.Taxis));
        }

        public IList<int> GetIdListByParentId(int parentId)
        {
            return _repository.GetAll<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis));
        }

        private IList<int> GetIdListForDescendant(int areaId)
        {
            return _repository.GetAll<int>(Q
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
    }
}

//    public class AreaDao
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
//                DataType = DataType.VarChar,
//                DataLength = 255
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(AreaInfo.ParentId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(AreaInfo.ParentsPath),
//                DataType = DataType.VarChar,
//                DataLength = 255
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
//                DataType = DataType.VarChar,
//                DataLength = 18
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
//        private const string SqlSelectCount = "SELECT COUNT(*) FROM siteserver_Area WHERE ParentID = @ParentID";
//        private const string SqlUpdate = "UPDATE siteserver_Area SET AreaName = @AreaName, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildrenCount = @ChildrenCount, IsLastNode = @IsLastNode, CountOfAdmin = @CountOfAdmin WHERE Id = @Id";

//        private const string ParmId = "@Id";
//        private const string ParmName = "@AreaName";
//        private const string ParmParentId = "@ParentID";
//        private const string ParmParentsPath = "@ParentsPath";
//        private const string ParmParentsCount = "@ParentsCount";
//        private const string ParmChildrenCount = "@ChildrenCount";
//        private const string ParmIsLastNode = "@IsLastNode";
//        private const string ParmTaxis = "@Taxis";
//        private const string ParmCountOfAdmin = "@CountOfAdmin";

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

//            IDataParameter[] insertParms = {
//				GetParameter(ParmName, DataType.VarChar, 255, areaInfo.AreaName),
//				GetParameter(ParmParentId, DataType.Integer, areaInfo.ParentId),
//				GetParameter(ParmParentsPath, DataType.VarChar, 255, areaInfo.ParentsPath),
//				GetParameter(ParmParentsCount, DataType.Integer, areaInfo.ParentsCount),
//				GetParameter(ParmChildrenCount, DataType.Integer, 0),
//				GetParameter(ParmIsLastNode, DataType.VarChar, 18, true.ToString()),
//				GetParameter(ParmTaxis, DataType.Integer, areaInfo.Taxis),
//				GetParameter(ParmCountOfAdmin, DataType.Integer, areaInfo.CountOfAdmin)
//			};

//            string sqlString = $"UPDATE siteserver_Area SET {SqlUtils.ToPlusSqlString("Taxis")} WHERE (Taxis >= {areaInfo.Taxis})";
//            ExecuteNonQuery(trans, sqlString);

//            areaInfo.Id = ExecuteNonQueryAndReturnId(TableName, nameof(AreaInfo.Id), trans, sqlInsert, insertParms);

//            if (!string.IsNullOrEmpty(areaInfo.ParentsPath) && areaInfo.ParentsPath != "0")
//            {
//                sqlString = $"UPDATE siteserver_Area SET {SqlUtils.ToPlusSqlString("ChildrenCount")} WHERE Id IN ({AttackUtils.FilterSql(areaInfo.ParentsPath)})";

//                ExecuteNonQuery(trans, sqlString);
//            }

//            sqlString = $"UPDATE siteserver_Area SET IsLastNode = '{false}' WHERE ParentID = {areaInfo.ParentId}";

//            ExecuteNonQuery(trans, sqlString);

//            //sqlString =
//            //    $"UPDATE siteserver_Area SET IsLastNode = 'True' WHERE (Id IN (SELECT TOP 1 Id FROM siteserver_Area WHERE ParentID = {areaInfo.ParentId} ORDER BY Taxis DESC))";            
//            sqlString =
//                $"UPDATE siteserver_Area SET IsLastNode = '{true}' WHERE Id IN ({SqlUtils.ToInTopSqlString(TableName, "Id", $"WHERE ParentID = {areaInfo.ParentId}", "ORDER BY Taxis DESC", 1)})";

//            ExecuteNonQuery(trans, sqlString);

//            AreaManager.ClearCache();
//        }

//        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
//        {
//            if (!string.IsNullOrEmpty(parentsPath))
//            {
//                var sqlString = string.Concat("UPDATE siteserver_Area SET ChildrenCount = ChildrenCount - ", subtractNum, " WHERE Id IN (", AttackUtils.FilterSql(parentsPath), ")");
//                ExecuteNonQuery(sqlString);

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
//            //            const string sqlString = @"SELECT TOP 1 Id, ChildrenCount, ParentsPath
//            //FROM siteserver_Area
//            //WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis < @Taxis)
//            //ORDER BY Taxis DESC";
//            var sqlString = SqlUtils.ToTopSqlString(TableName, "Id, ChildrenCount, ParentsPath",
//                "WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis < @Taxis)", "ORDER BY Taxis DESC", 1);

//            IDataParameter[] parms = {
//				GetParameter(ParmParentId, DataType.Integer, areaInfo.ParentId),
//				GetParameter(ParmId, DataType.Integer, areaInfo.Id),
//				GetParameter(ParmTaxis, DataType.Integer, areaInfo.Taxis),
//			};

//            using (var rdr = ExecuteReader(sqlString, parms))
//            {
//                if (rdr.Read())
//                {
//                    lowerId = GetInt(rdr, 0);
//                    lowerChildrenCount = GetInt(rdr, 1);
//                    lowerParentsPath = GetString(rdr, 2);
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
//            //            var sqlString = @"SELECT TOP 1 Id, ChildrenCount, ParentsPath
//            //FROM siteserver_Area
//            //WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis > @Taxis)
//            //ORDER BY Taxis";
//            var sqlString = SqlUtils.ToTopSqlString(TableName, "Id, ChildrenCount, ParentsPath",
//                "WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis > @Taxis)", "ORDER BY Taxis", 1);

//            IDataParameter[] parms = {
//				GetParameter(ParmParentId, DataType.Integer, areaInfo.ParentId),
//				GetParameter(ParmId, DataType.Integer, areaInfo.Id),
//				GetParameter(ParmTaxis, DataType.Integer, areaInfo.Taxis)
//			};

//            using (var rdr = ExecuteReader(sqlString, parms))
//            {
//                if (rdr.Read())
//                {
//                    higherId = GetInt(rdr, 0);
//                    higherChildrenCount = GetInt(rdr, 1);
//                    higherParentsPath = GetString(rdr, 2);
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

//            ExecuteNonQuery(sqlString);

//            AreaManager.ClearCache();
//        }

//        private void SetTaxisSubtract(int areaId, string parentsPath, int subtractNum)
//        {
//            var path = AttackUtils.FilterSql(parentsPath);
//            string sqlString =
//                $"UPDATE siteserver_Area SET Taxis = Taxis - {subtractNum} WHERE  Id = {areaId} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

//            ExecuteNonQuery(sqlString);

//            AreaManager.ClearCache();
//        }

//        private void UpdateIsLastNode(int parentId)
//        {
//            if (parentId > 0)
//            {
//                var sqlString = "UPDATE siteserver_Area SET IsLastNode = @IsLastNode WHERE ParentID = @ParentID";

//                IDataParameter[] parms = {
//				    GetParameter(ParmIsLastNode, DataType.VarChar, 18, false.ToString()),
//				    GetParameter(ParmParentId, DataType.Integer, parentId)
//			    };

//                ExecuteNonQuery(sqlString, parms);

//                //sqlString =
//                //    $"UPDATE siteserver_Area SET IsLastNode = '{true}' WHERE (Id IN (SELECT TOP 1 Id FROM siteserver_Area WHERE ParentID = {parentId} ORDER BY Taxis DESC))";
//                sqlString =
//                    $"UPDATE siteserver_Area SET IsLastNode = '{true}' WHERE Id IN ({SqlUtils.ToInTopSqlString(TableName, "Id", $"WHERE ParentID = {parentId}", "ORDER BY Taxis DESC", 1)})";

//                ExecuteNonQuery(sqlString);
//            }
//        }

//        private int GetMaxTaxisByParentPath(string parentPath)
//        {
//            var sqlString = string.Concat("SELECT MAX(Taxis) AS MaxTaxis FROM siteserver_Area WHERE (ParentsPath = '", AttackUtils.FilterSql(parentPath), "') OR (ParentsPath LIKE '", AttackUtils.FilterSql(parentPath), ",%')");
//            var maxTaxis = 0;

//            using (var rdr = ExecuteReader(sqlString))
//            {
//                if (rdr.Read())
//                {
//                    maxTaxis = GetInt(rdr, 0);
//                }
//                rdr.Close();
//            }
//            return maxTaxis;
//        }

//        public int Insert(AreaInfo areaInfo)
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

//        public void Update(AreaInfo areaInfo)
//        {
//            IDataParameter[] updateParms = {
//				GetParameter(ParmName, DataType.VarChar, 255, areaInfo.AreaName),
//				GetParameter(ParmParentsPath, DataType.VarChar, 255, areaInfo.ParentsPath),
//				GetParameter(ParmParentsCount, DataType.Integer, areaInfo.ParentsCount),
//				GetParameter(ParmChildrenCount, DataType.Integer, areaInfo.ChildrenCount),
//				GetParameter(ParmIsLastNode, DataType.VarChar, 18, areaInfo.IsLastNode.ToString()),
//				GetParameter(ParmCountOfAdmin, DataType.Integer, areaInfo.CountOfAdmin),
//				GetParameter(ParmId, DataType.Integer, areaInfo.Id)
//			};

//            ExecuteNonQuery(SqlUpdate, updateParms);

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
//                var count = DataProvider.AdministratorDao.GetCountByAreaId(areaId);
//                string sqlString = $"UPDATE {TableName} SET CountOfAdmin = {count} WHERE Id = {areaId}";
//                ExecuteNonQuery(sqlString);
//            }
//            AreaManager.ClearCache();
//        }

//        public void Delete(int areaId)
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
//                            deletedNum = ExecuteNonQuery(trans, sqlString);

//                            if (deletedNum > 0)
//                            {
//                                string sqlStringTaxis =
//                                    $"UPDATE siteserver_Area SET Taxis = Taxis - {deletedNum} WHERE (Taxis > {areaInfo.Taxis})";
//                                ExecuteNonQuery(trans, sqlStringTaxis);
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

//            IDataParameter[] parms = {
//				GetParameter(ParmId, DataType.Integer, areaId)
//			};

//            using (var rdr = ExecuteReader(SqlSelect, parms))
//            {
//                if (rdr.Read())
//                {
//                    var i = 0;
//                    areaInfo = new AreaInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i));
//                }
//                rdr.Close();
//            }
//            return areaInfo;
//        }

//        private List<AreaInfo> GetAreaInfoList()
//        {
//            var list = new List<AreaInfo>();

//            using (var rdr = ExecuteReader(SqlSelectAll))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var areaInfo = new AreaInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i));
//                    list.Add(areaInfo);
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        public int GetNodeCount(int areaId)
//        {
//            var nodeCount = 0;

//            IDataParameter[] nodeParms = {
//				GetParameter(ParmParentId, DataType.Integer, areaId)
//			};

//            using (var rdr = ExecuteReader(SqlSelectCount, nodeParms))
//            {
//                if (rdr.Read())
//                {
//                    nodeCount = GetInt(rdr, 0);
//                }
//                rdr.Close();
//            }
//            return nodeCount;
//        }

//        public List<int> GetIdListByParentId(int parentId)
//        {
//            string sqlString = $@"SELECT Id FROM siteserver_Area WHERE ParentID = '{parentId}' ORDER BY Taxis";
//            var list = new List<int>();

//            using (var rdr = ExecuteReader(sqlString))
//            {
//                while (rdr.Read())
//                {
//                    list.Add(GetInt(rdr, 0));
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

//            using (var rdr = ExecuteReader(sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var theId = GetInt(rdr, 0);
//                    list.Add(theId);
//                }
//                rdr.Close();
//            }

//            return list;
//        }

//        public List<int> GetIdListByIdCollection(string areaIdCollection)
//        {
//            var list = new List<int>();

//            if (string.IsNullOrEmpty(areaIdCollection)) return list;

//            string sqlString = $@"SELECT Id FROM siteserver_Area WHERE Id IN ({areaIdCollection})";

//            using (var rdr = ExecuteReader(sqlString))
//            {
//                while (rdr.Read())
//                {
//                    list.Add(GetInt(rdr, 0));
//                }
//                rdr.Close();
//            }

//            return list;
//        }

//        public List<int> GetIdListByFirstIdList(List<int> firstIdList)
//        {
//            var list = new List<int>();

//            if (firstIdList.Count > 0)
//            {
//                var builder = new StringBuilder();
//                foreach (var areaId in firstIdList)
//                {
//                    builder.Append($"Id = {areaId} OR ParentID = {areaId} OR ParentsPath LIKE '{areaId},%' OR ");
//                }
//                builder.Length -= 3;

//                string sqlString = $"SELECT Id FROM siteserver_Area WHERE {builder} ORDER BY Taxis";

//                using (var rdr = ExecuteReader(sqlString))
//                {
//                    while (rdr.Read())
//                    {
//                        list.Add(GetInt(rdr, 0));
//                    }
//                    rdr.Close();
//                }
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
