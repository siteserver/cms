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
    public class DepartmentDao : DataProviderBase
	{
        public override string TableName => "siteserver_Department";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(DepartmentInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(DepartmentInfo.DepartmentName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(DepartmentInfo.Code),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(DepartmentInfo.ParentId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(DepartmentInfo.ParentsPath),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(DepartmentInfo.ParentsCount),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(DepartmentInfo.ChildrenCount),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(DepartmentInfo.IsLastNode),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(DepartmentInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(DepartmentInfo.AddDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(DepartmentInfo.Summary),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(DepartmentInfo.CountOfAdmin),
                DataType = DataType.Integer
            }
        };

        private const string SqlSelect = "SELECT Id, DepartmentName, Code, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, AddDate, Summary, CountOfAdmin FROM siteserver_Department WHERE Id = @Id";

        private const string SqlSelectAll = "SELECT Id, DepartmentName, Code, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, AddDate, Summary, CountOfAdmin FROM siteserver_Department ORDER BY TAXIS";

        private const string SqlSelectCount = "SELECT COUNT(*) FROM siteserver_Department WHERE ParentID = @ParentID";

        private const string SqlUpdate = "UPDATE siteserver_Department SET DepartmentName = @DepartmentName, Code = @Code, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildrenCount = @ChildrenCount, IsLastNode = @IsLastNode, Summary = @Summary, CountOfAdmin = @CountOfAdmin WHERE Id = @Id";

        private const string ParmId = "@Id";
        private const string ParmName = "@DepartmentName";
        private const string ParmCode = "@Code";
        private const string ParmParentId = "@ParentID";
        private const string ParmParentsPath = "@ParentsPath";
        private const string ParmParentsCount = "@ParentsCount";
        private const string ParmChildrenCount = "@ChildrenCount";
        private const string ParmIsLastNode = "@IsLastNode";
        private const string ParmTaxis = "@Taxis";
        private const string ParmAddDate = "@AddDate";
        private const string ParmSummary = "@Summary";
        private const string ParmCountOfAdmin = "@CountOfAdmin";

        private void InsertWithTrans(DepartmentInfo parentInfo, DepartmentInfo departmentInfo, IDbTransaction trans)
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

            var sqlInsert = "INSERT INTO siteserver_Department (DepartmentName, Code, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, AddDate, Summary, CountOfAdmin) VALUES (@DepartmentName, @Code, @ParentID, @ParentsPath, @ParentsCount, @ChildrenCount, @IsLastNode, @Taxis, @AddDate, @Summary, @CountOfAdmin)";

            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmName, DataType.VarChar, 255, departmentInfo.DepartmentName),
                GetParameter(ParmCode, DataType.VarChar, 50, departmentInfo.Code),
				GetParameter(ParmParentId, DataType.Integer, departmentInfo.ParentId),
				GetParameter(ParmParentsPath, DataType.VarChar, 255, departmentInfo.ParentsPath),
				GetParameter(ParmParentsCount, DataType.Integer, departmentInfo.ParentsCount),
				GetParameter(ParmChildrenCount, DataType.Integer, 0),
				GetParameter(ParmIsLastNode, DataType.VarChar, 18, true.ToString()),
				GetParameter(ParmTaxis, DataType.Integer, departmentInfo.Taxis),
				GetParameter(ParmAddDate, DataType.DateTime, departmentInfo.AddDate),
				GetParameter(ParmSummary, DataType.VarChar, 255, departmentInfo.Summary),
				GetParameter(ParmCountOfAdmin, DataType.Integer, departmentInfo.CountOfAdmin)
			};

            string sqlString =
                $"UPDATE siteserver_Department SET {SqlUtils.ToPlusSqlString("Taxis")} WHERE (Taxis >= {departmentInfo.Taxis})";
            ExecuteNonQuery(trans, sqlString);

            departmentInfo.Id = ExecuteNonQueryAndReturnId(TableName, nameof(DepartmentInfo.Id), trans, sqlInsert, insertParms);

            if (!string.IsNullOrEmpty(departmentInfo.ParentsPath))
            {
                sqlString = $"UPDATE siteserver_Department SET {SqlUtils.ToPlusSqlString("ChildrenCount")} WHERE Id IN ({AttackUtils.FilterSql(departmentInfo.ParentsPath)})";

                ExecuteNonQuery(trans, sqlString);
            }

            sqlString = $"UPDATE siteserver_Department SET IsLastNode = '{false}' WHERE ParentID = {departmentInfo.ParentId}";

            ExecuteNonQuery(trans, sqlString);

            //sqlString =
            //    $"UPDATE siteserver_Department SET IsLastNode = '{true}' WHERE (Id IN (SELECT TOP 1 Id FROM siteserver_Department WHERE ParentID = {departmentInfo.ParentId} ORDER BY Taxis DESC))";

            sqlString =
                $"UPDATE siteserver_Department SET IsLastNode = '{true}' WHERE (Id IN ({SqlUtils.ToInTopSqlString("siteserver_Department", "Id", $"WHERE ParentID = {departmentInfo.ParentId}", "ORDER BY Taxis DESC", 1)}))";

            ExecuteNonQuery(trans, sqlString);

            DepartmentManager.ClearCache();
        }

        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
        {
            if (!string.IsNullOrEmpty(parentsPath))
            {
                var sqlString = string.Concat("UPDATE siteserver_Department SET ChildrenCount = ChildrenCount - ", subtractNum, " WHERE Id IN (", AttackUtils.FilterSql(parentsPath) , ")");
                ExecuteNonQuery(sqlString);

                DepartmentManager.ClearCache();
            }
        }

        private void TaxisSubtract(int selectedId)
        {
            var departmentInfo = GetDepartmentInfo(selectedId);
            if (departmentInfo == null) return;
            //Get Lower Taxis and Id
            int lowerId;
            int lowerChildrenCount;
            string lowerParentsPath;
            //            var sqlString = @"SELECT TOP 1 Id, ChildrenCount, ParentsPath
            //FROM siteserver_Department
            //WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis < @Taxis)
            //ORDER BY Taxis DESC";

            var sqlString = SqlUtils.ToTopSqlString("siteserver_Department", "Id, ChildrenCount, ParentsPath",
                "WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis < @Taxis)",
                "ORDER BY Taxis DESC", 1);

            var parms = new IDataParameter[]
			{
				GetParameter(ParmParentId, DataType.Integer, departmentInfo.ParentId),
				GetParameter(ParmId, DataType.Integer, departmentInfo.Id),
				GetParameter(ParmTaxis, DataType.Integer, departmentInfo.Taxis),
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
            int higherId;
            int higherChildrenCount;
            string higherParentsPath;
            //            var sqlString = @"SELECT TOP 1 Id, ChildrenCount, ParentsPath
            //FROM siteserver_Department
            //WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis > @Taxis)
            //ORDER BY Taxis";

            var sqlString = SqlUtils.ToTopSqlString("siteserver_Department", "Id, ChildrenCount, ParentsPath",
                "WHERE (ParentID = @ParentID) AND (Id <> @Id) AND (Taxis > @Taxis)",
                "ORDER BY Taxis", 1);

            var parms = new IDataParameter[]
			{
				GetParameter(ParmParentId, DataType.Integer, departmentInfo.ParentId),
				GetParameter(ParmId, DataType.Integer, departmentInfo.Id),
				GetParameter(ParmTaxis, DataType.Integer, departmentInfo.Taxis)
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
            var selectedNodePath = string.Concat(departmentInfo.ParentsPath, ",", departmentInfo.Id);

            SetTaxisAdd(selectedId, selectedNodePath, higherChildrenCount + 1);
            SetTaxisSubtract(higherId, higherNodePath, departmentInfo.ChildrenCount + 1);

            UpdateIsLastNode(departmentInfo.ParentId);
        }

        private void SetTaxisAdd(int id, string parentsPath, int addNum)
        {
            var path = AttackUtils.FilterSql(parentsPath);
            var sqlString =
                $"UPDATE siteserver_Department SET Taxis = Taxis + {addNum} WHERE Id = {id} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

            ExecuteNonQuery(sqlString);

            DepartmentManager.ClearCache();
        }

        private void SetTaxisSubtract(int id, string parentsPath, int subtractNum)
        {
            var path = AttackUtils.FilterSql(parentsPath);
            var sqlString =
                $"UPDATE siteserver_Department SET Taxis = Taxis - {subtractNum} WHERE  Id = {id} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

            ExecuteNonQuery(sqlString);

            DepartmentManager.ClearCache();
        }

        private void UpdateIsLastNode(int parentId)
        {
            if (parentId > 0)
            {
                var sqlString = "UPDATE siteserver_Department SET IsLastNode = @IsLastNode WHERE  ParentID = @ParentID";

                var parms = new IDataParameter[]
			    {
				    GetParameter(ParmIsLastNode, DataType.VarChar, 18, false.ToString()),
				    GetParameter(ParmParentId, DataType.Integer, parentId)
			    };

                ExecuteNonQuery(sqlString, parms);

                //sqlString =
                //    $"UPDATE siteserver_Department SET IsLastNode = '{true.ToString()}' WHERE (Id IN (SELECT TOP 1 Id FROM siteserver_Department WHERE ParentID = {parentId} ORDER BY Taxis DESC))";

                sqlString =
                    $"UPDATE siteserver_Department SET IsLastNode = '{true}' WHERE (Id IN ({SqlUtils.ToInTopSqlString("siteserver_Department", "Id", $"WHERE ParentID = {parentId}", "ORDER BY Taxis DESC", 1)}))";

                ExecuteNonQuery(sqlString);
            }
        }

        private int GetMaxTaxisByParentPath(string parentPath)
        {
            parentPath = AttackUtils.FilterSql(parentPath);
            var sqlString = string.Concat("SELECT MAX(Taxis) AS MaxTaxis FROM siteserver_Department WHERE (ParentsPath = '", parentPath, "') OR (ParentsPath LIKE '", parentPath, ",%')");
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

	    public int Insert(DepartmentInfo departmentInfo)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var parentDepartmentInfo = GetDepartmentInfo(departmentInfo.ParentId);

                        InsertWithTrans(parentDepartmentInfo, departmentInfo, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            DepartmentManager.ClearCache();

            return departmentInfo.Id;
        }

        public void Update(DepartmentInfo departmentInfo)
        {
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmName, DataType.VarChar, 255, departmentInfo.DepartmentName),
                GetParameter(ParmCode, DataType.VarChar, 50, departmentInfo.Code),
				GetParameter(ParmParentsPath, DataType.VarChar, 255, departmentInfo.ParentsPath),
				GetParameter(ParmParentsCount, DataType.Integer, departmentInfo.ParentsCount),
				GetParameter(ParmChildrenCount, DataType.Integer, departmentInfo.ChildrenCount),
				GetParameter(ParmIsLastNode, DataType.VarChar, 18, departmentInfo.IsLastNode.ToString()),
				GetParameter(ParmSummary, DataType.VarChar, 255, departmentInfo.Summary),
				GetParameter(ParmCountOfAdmin, DataType.Integer, departmentInfo.CountOfAdmin),
				GetParameter(ParmId, DataType.Integer, departmentInfo.Id)
			};

            ExecuteNonQuery(SqlUpdate, updateParms);

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
                var count = DataProvider.AdministratorDao.GetCountByDepartmentId(departmentId);
                string sqlString = $"UPDATE {TableName} SET CountOfAdmin = {count} WHERE Id = {departmentId}";
                ExecuteNonQuery(sqlString);
            }
            DepartmentManager.ClearCache();
        }

        public void Delete(int id)
        {
            var departmentInfo = GetDepartmentInfo(id);
            if (departmentInfo != null)
            {
                var idList = new List<int>();
                if (departmentInfo.ChildrenCount > 0)
                {
                    idList = GetIdListForDescendant(id);
                }
                idList.Add(id);

                string sqlString =
                    $"DELETE FROM siteserver_Department WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

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
                                    $"UPDATE siteserver_Department SET Taxis = Taxis - {deletedNum} WHERE (Taxis > {departmentInfo.Taxis})";
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
                UpdateIsLastNode(departmentInfo.ParentId);
                UpdateSubtractChildrenCount(departmentInfo.ParentsPath, deletedNum);
            }

            DepartmentManager.ClearCache();
        }

        private DepartmentInfo GetDepartmentInfo(int id)
		{
            DepartmentInfo departmentInfo = null;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmId, DataType.Integer, id)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    departmentInfo = new DepartmentInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetInt(rdr, i));
				}
				rdr.Close();
			}
            return departmentInfo;
		}

        private List<DepartmentInfo> GetDepartmentInfoList()
        {
            var list = new List<DepartmentInfo>();

            using (var rdr = ExecuteReader(SqlSelectAll))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var departmentInfo = new DepartmentInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetInt(rdr, i));
                    list.Add(departmentInfo);
                }
                rdr.Close();
            }
            return list;
        }

		public int GetNodeCount(int id)
		{
			var nodeCount = 0;

			var nodeParms = new IDataParameter[]
			{
				GetParameter(ParmParentId, DataType.Integer, id)
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
            string sqlString =
                $@"SELECT Id FROM siteserver_Department WHERE ParentID = '{parentId}' ORDER BY Taxis";
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

		public List<int> GetIdListForDescendant(int id)
		{
            string sqlString = $@"SELECT Id
FROM siteserver_Department
WHERE (ParentsPath LIKE '{id},%') OR
      (ParentsPath LIKE '%,{id},%') OR
      (ParentsPath LIKE '%,{id}') OR
      (ParentID = {id})
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

        public List<int> GetIdListByIdCollection(string idCollection)
        {
            var list = new List<int>();

            if (string.IsNullOrEmpty(idCollection)) return list;

            string sqlString =
                $@"SELECT Id FROM siteserver_Department WHERE Id IN ({idCollection})";

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

        public List<int> GetIdListByFirstIdList(List<int> firstIdList)
        {
            var list = new List<int>();

            if (firstIdList.Count <= 0) return list;

            var builder = new StringBuilder();
            foreach (var id in firstIdList)
            {
                builder.Append($"Id = {id} OR ParentID = {id} OR ParentsPath LIKE '{id},%' OR ");
            }
            builder.Length -= 3;

            string sqlString =
                $"SELECT Id FROM siteserver_Department WHERE {builder} ORDER BY Taxis";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var id = GetInt(rdr, 0);
                    list.Add(id);
                }
                rdr.Close();
            }

            return list;
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
