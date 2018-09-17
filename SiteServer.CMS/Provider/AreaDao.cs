using System.Collections.Generic;
using System.Data;
using System.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class AreaDao : DataProviderBase
    {
        public override string TableName => "siteserver_Area";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(AreaInfo.Id),
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = true
            },
            new TableColumn
            {
                AttributeName = nameof(AreaInfo.AreaName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(AreaInfo.ParentId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(AreaInfo.ParentsPath),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(AreaInfo.ParentsCount),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(AreaInfo.ChildrenCount),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(AreaInfo.IsLastNode),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(AreaInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(AreaInfo.CountOfAdmin),
                DataType = DataType.Integer
            }
        };

        private const string SqlSelect = "SELECT Id, AreaName, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, CountOfAdmin FROM siteserver_Area WHERE Id = @Id";
        private const string SqlSelectAll = "SELECT Id, AreaName, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, CountOfAdmin FROM siteserver_Area ORDER BY TAXIS";
        private const string SqlSelectCount = "SELECT COUNT(*) FROM siteserver_Area WHERE ParentID = @ParentID";
        private const string SqlUpdate = "UPDATE siteserver_Area SET AreaName = @AreaName, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildrenCount = @ChildrenCount, IsLastNode = @IsLastNode, CountOfAdmin = @CountOfAdmin WHERE Id = @Id";

        private const string ParmId = "@Id";
        private const string ParmName = "@AreaName";
        private const string ParmParentId = "@ParentID";
        private const string ParmParentsPath = "@ParentsPath";
        private const string ParmParentsCount = "@ParentsCount";
        private const string ParmChildrenCount = "@ChildrenCount";
        private const string ParmIsLastNode = "@IsLastNode";
        private const string ParmTaxis = "@Taxis";
        private const string ParmCountOfAdmin = "@CountOfAdmin";

        private void InsertWithTrans(AreaInfo parentInfo, AreaInfo areaInfo, IDbTransaction trans)
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

            var sqlInsert = "INSERT INTO siteserver_Area (AreaName, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, CountOfAdmin) VALUES (@AreaName, @ParentID, @ParentsPath, @ParentsCount, @ChildrenCount, @IsLastNode, @Taxis, @CountOfAdmin)";

            IDataParameter[] insertParms = {
				GetParameter(ParmName, DataType.VarChar, 255, areaInfo.AreaName),
				GetParameter(ParmParentId, DataType.Integer, areaInfo.ParentId),
				GetParameter(ParmParentsPath, DataType.VarChar, 255, areaInfo.ParentsPath),
				GetParameter(ParmParentsCount, DataType.Integer, areaInfo.ParentsCount),
				GetParameter(ParmChildrenCount, DataType.Integer, 0),
				GetParameter(ParmIsLastNode, DataType.VarChar, 18, true.ToString()),
				GetParameter(ParmTaxis, DataType.Integer, areaInfo.Taxis),
				GetParameter(ParmCountOfAdmin, DataType.Integer, areaInfo.CountOfAdmin)
			};

            string sqlString = $"UPDATE siteserver_Area SET {SqlUtils.ToPlusSqlString("Taxis")} WHERE (Taxis >= {areaInfo.Taxis})";
            ExecuteNonQuery(trans, sqlString);

            areaInfo.Id = ExecuteNonQueryAndReturnId(TableName, nameof(AreaInfo.Id), trans, sqlInsert, insertParms);

            if (!string.IsNullOrEmpty(areaInfo.ParentsPath) && areaInfo.ParentsPath != "0")
            {
                sqlString = $"UPDATE siteserver_Area SET {SqlUtils.ToPlusSqlString("ChildrenCount")} WHERE Id IN ({AttackUtils.FilterSql(areaInfo.ParentsPath)})";

                ExecuteNonQuery(trans, sqlString);
            }

            sqlString = $"UPDATE siteserver_Area SET IsLastNode = '{false}' WHERE ParentID = {areaInfo.ParentId}";

            ExecuteNonQuery(trans, sqlString);

            //sqlString =
            //    $"UPDATE siteserver_Area SET IsLastNode = 'True' WHERE (Id IN (SELECT TOP 1 Id FROM siteserver_Area WHERE ParentID = {areaInfo.ParentId} ORDER BY Taxis DESC))";            
            sqlString =
                $"UPDATE siteserver_Area SET IsLastNode = '{true}' WHERE Id IN ({SqlUtils.ToInTopSqlString(TableName, "Id", $"WHERE ParentID = {areaInfo.ParentId}", "ORDER BY Taxis DESC", 1)})";

            ExecuteNonQuery(trans, sqlString);

            AreaManager.ClearCache();
        }

        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
        {
            if (!string.IsNullOrEmpty(parentsPath))
            {
                var sqlString = string.Concat("UPDATE siteserver_Area SET ChildrenCount = ChildrenCount - ", subtractNum, " WHERE Id IN (", AttackUtils.FilterSql(parentsPath), ")");
                ExecuteNonQuery(sqlString);

                AreaManager.ClearCache();
            }
        }

        private void TaxisSubtract(int selectedId)
        {
            var areaInfo = GetAreaInfo(selectedId);
            if (areaInfo == null) return;
            //Get Lower Taxis and Id
            int lowerId;
            int lowerChildrenCount;
            string lowerParentsPath;
            //            const string sqlString = @"SELECT TOP 1 Id, ChildrenCount, ParentsPath
            //FROM siteserver_Area
            //WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis < @Taxis)
            //ORDER BY Taxis DESC";
            var sqlString = SqlUtils.ToTopSqlString(TableName, "Id, ChildrenCount, ParentsPath",
                "WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis < @Taxis)", "ORDER BY Taxis DESC", 1);

            IDataParameter[] parms = {
				GetParameter(ParmParentId, DataType.Integer, areaInfo.ParentId),
				GetParameter(ParmId, DataType.Integer, areaInfo.Id),
				GetParameter(ParmTaxis, DataType.Integer, areaInfo.Taxis),
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    lowerId = GetInt(rdr, 0);
                    lowerChildrenCount = GetInt(rdr, 1);
                    lowerParentsPath = GetString(rdr, 2);
                }
                else
                {
                    return;
                }
                rdr.Close();
            }


            var lowerNodePath = string.Concat(lowerParentsPath, ",", lowerId);
            var selectedNodePath = string.Concat(areaInfo.ParentsPath, ",", areaInfo.Id);

            SetTaxisSubtract(selectedId, selectedNodePath, lowerChildrenCount + 1);
            SetTaxisAdd(lowerId, lowerNodePath, areaInfo.ChildrenCount + 1);

            UpdateIsLastNode(areaInfo.ParentId);
        }

        private void TaxisAdd(int selectedId)
        {
            var areaInfo = GetAreaInfo(selectedId);
            if (areaInfo == null) return;
            //Get Higher Taxis and Id
            int higherId;
            int higherChildrenCount;
            string higherParentsPath;
            //            var sqlString = @"SELECT TOP 1 Id, ChildrenCount, ParentsPath
            //FROM siteserver_Area
            //WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis > @Taxis)
            //ORDER BY Taxis";
            var sqlString = SqlUtils.ToTopSqlString(TableName, "Id, ChildrenCount, ParentsPath",
                "WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis > @Taxis)", "ORDER BY Taxis", 1);

            IDataParameter[] parms = {
				GetParameter(ParmParentId, DataType.Integer, areaInfo.ParentId),
				GetParameter(ParmId, DataType.Integer, areaInfo.Id),
				GetParameter(ParmTaxis, DataType.Integer, areaInfo.Taxis)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    higherId = GetInt(rdr, 0);
                    higherChildrenCount = GetInt(rdr, 1);
                    higherParentsPath = GetString(rdr, 2);
                }
                else
                {
                    return;
                }
                rdr.Close();
            }


            var higherNodePath = string.Concat(higherParentsPath, ",", higherId);
            var selectedNodePath = string.Concat(areaInfo.ParentsPath, ",", areaInfo.Id);

            SetTaxisAdd(selectedId, selectedNodePath, higherChildrenCount + 1);
            SetTaxisSubtract(higherId, higherNodePath, areaInfo.ChildrenCount + 1);

            UpdateIsLastNode(areaInfo.ParentId);
        }

        private void SetTaxisAdd(int areaId, string parentsPath, int addNum)
        {
            var path = AttackUtils.FilterSql(parentsPath);
            string sqlString =
                $"UPDATE siteserver_Area SET Taxis = Taxis + {addNum} WHERE Id = {areaId} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

            ExecuteNonQuery(sqlString);

            AreaManager.ClearCache();
        }

        private void SetTaxisSubtract(int areaId, string parentsPath, int subtractNum)
        {
            var path = AttackUtils.FilterSql(parentsPath);
            string sqlString =
                $"UPDATE siteserver_Area SET Taxis = Taxis - {subtractNum} WHERE  Id = {areaId} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

            ExecuteNonQuery(sqlString);

            AreaManager.ClearCache();
        }

        private void UpdateIsLastNode(int parentId)
        {
            if (parentId > 0)
            {
                var sqlString = "UPDATE siteserver_Area SET IsLastNode = @IsLastNode WHERE ParentID = @ParentID";

                IDataParameter[] parms = {
				    GetParameter(ParmIsLastNode, DataType.VarChar, 18, false.ToString()),
				    GetParameter(ParmParentId, DataType.Integer, parentId)
			    };

                ExecuteNonQuery(sqlString, parms);

                //sqlString =
                //    $"UPDATE siteserver_Area SET IsLastNode = '{true}' WHERE (Id IN (SELECT TOP 1 Id FROM siteserver_Area WHERE ParentID = {parentId} ORDER BY Taxis DESC))";
                sqlString =
                    $"UPDATE siteserver_Area SET IsLastNode = '{true}' WHERE Id IN ({SqlUtils.ToInTopSqlString(TableName, "Id", $"WHERE ParentID = {parentId}", "ORDER BY Taxis DESC", 1)})";

                ExecuteNonQuery(sqlString);
            }
        }

        private int GetMaxTaxisByParentPath(string parentPath)
        {
            var sqlString = string.Concat("SELECT MAX(Taxis) AS MaxTaxis FROM siteserver_Area WHERE (ParentsPath = '", AttackUtils.FilterSql(parentPath), "') OR (ParentsPath LIKE '", AttackUtils.FilterSql(parentPath), ",%')");
            var maxTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    maxTaxis = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return maxTaxis;
        }

        public int Insert(AreaInfo areaInfo)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var parentAreaInfo = GetAreaInfo(areaInfo.ParentId);

                        InsertWithTrans(parentAreaInfo, areaInfo, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            AreaManager.ClearCache();

            return areaInfo.Id;
        }

        public void Update(AreaInfo areaInfo)
        {
            IDataParameter[] updateParms = {
				GetParameter(ParmName, DataType.VarChar, 255, areaInfo.AreaName),
				GetParameter(ParmParentsPath, DataType.VarChar, 255, areaInfo.ParentsPath),
				GetParameter(ParmParentsCount, DataType.Integer, areaInfo.ParentsCount),
				GetParameter(ParmChildrenCount, DataType.Integer, areaInfo.ChildrenCount),
				GetParameter(ParmIsLastNode, DataType.VarChar, 18, areaInfo.IsLastNode.ToString()),
				GetParameter(ParmCountOfAdmin, DataType.Integer, areaInfo.CountOfAdmin),
				GetParameter(ParmId, DataType.Integer, areaInfo.Id)
			};

            ExecuteNonQuery(SqlUpdate, updateParms);

            AreaManager.ClearCache();
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
                string sqlString = $"UPDATE {TableName} SET CountOfAdmin = {count} WHERE Id = {areaId}";
                ExecuteNonQuery(sqlString);
            }
            AreaManager.ClearCache();
        }

        public void Delete(int areaId)
        {
            var areaInfo = GetAreaInfo(areaId);
            if (areaInfo != null)
            {
                var areaIdList = new List<int>();
                if (areaInfo.ChildrenCount > 0)
                {
                    areaIdList = GetIdListForDescendant(areaId);
                }
                areaIdList.Add(areaId);

                string sqlString =
                    $"DELETE FROM siteserver_Area WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(areaIdList)})";

                int deletedNum;

                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        try
                        {
                            deletedNum = ExecuteNonQuery(trans, sqlString);

                            if (deletedNum > 0)
                            {
                                string sqlStringTaxis =
                                    $"UPDATE siteserver_Area SET Taxis = Taxis - {deletedNum} WHERE (Taxis > {areaInfo.Taxis})";
                                ExecuteNonQuery(trans, sqlStringTaxis);
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
                UpdateIsLastNode(areaInfo.ParentId);
                UpdateSubtractChildrenCount(areaInfo.ParentsPath, deletedNum);
            }

            AreaManager.ClearCache();
        }

        private AreaInfo GetAreaInfo(int areaId)
        {
            AreaInfo areaInfo = null;

            IDataParameter[] parms = {
				GetParameter(ParmId, DataType.Integer, areaId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    areaInfo = new AreaInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i));
                }
                rdr.Close();
            }
            return areaInfo;
        }

        private List<AreaInfo> GetAreaInfoList()
        {
            var list = new List<AreaInfo>();

            using (var rdr = ExecuteReader(SqlSelectAll))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var areaInfo = new AreaInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i));
                    list.Add(areaInfo);
                }
                rdr.Close();
            }
            return list;
        }

        public int GetNodeCount(int areaId)
        {
            var nodeCount = 0;

            IDataParameter[] nodeParms = {
				GetParameter(ParmParentId, DataType.Integer, areaId)
			};

            using (var rdr = ExecuteReader(SqlSelectCount, nodeParms))
            {
                if (rdr.Read())
                {
                    nodeCount = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return nodeCount;
        }

        public List<int> GetIdListByParentId(int parentId)
        {
            string sqlString = $@"SELECT Id FROM siteserver_Area WHERE ParentID = '{parentId}' ORDER BY Taxis";
            var list = new List<int>();

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetIdListForDescendant(int areaId)
        {
            string sqlString = $@"SELECT Id
FROM siteserver_Area
WHERE (ParentsPath LIKE '{areaId},%') OR
      (ParentsPath LIKE '%,{areaId},%') OR
      (ParentsPath LIKE '%,{areaId}') OR
      (ParentID = {areaId})
";
            var list = new List<int>();

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var theId = GetInt(rdr, 0);
                    list.Add(theId);
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetIdListByIdCollection(string areaIdCollection)
        {
            var list = new List<int>();

            if (string.IsNullOrEmpty(areaIdCollection)) return list;

            string sqlString = $@"SELECT Id FROM siteserver_Area WHERE Id IN ({areaIdCollection})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetIdListByFirstIdList(List<int> firstIdList)
        {
            var list = new List<int>();

            if (firstIdList.Count > 0)
            {
                var builder = new StringBuilder();
                foreach (var areaId in firstIdList)
                {
                    builder.Append($"Id = {areaId} OR ParentID = {areaId} OR ParentsPath LIKE '{areaId},%' OR ");
                }
                builder.Length -= 3;

                string sqlString = $"SELECT Id FROM siteserver_Area WHERE {builder} ORDER BY Taxis";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        list.Add(GetInt(rdr, 0));
                    }
                    rdr.Close();
                }
            }

            return list;
        }

        public List<KeyValuePair<int, AreaInfo>> GetAreaInfoPairList()
        {
            var pairList = new List<KeyValuePair<int, AreaInfo>>();

            var areaInfoList = GetAreaInfoList();
            foreach (var areaInfo in areaInfoList)
            {
                var pair = new KeyValuePair<int, AreaInfo>(areaInfo.Id, areaInfo);
                pairList.Add(pair);
            }

            return pairList;
        }
    }
}
