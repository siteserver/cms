using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
    public class DepartmentDao : DataProviderBase
	{
        private const string SqlSelect = "SELECT DepartmentID, DepartmentName, Code, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, AddDate, Summary, CountOfAdmin FROM bairong_Department WHERE DepartmentID = @DepartmentID";

        private const string SqlSelectAll = "SELECT DepartmentID, DepartmentName, Code, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, AddDate, Summary, CountOfAdmin FROM bairong_Department ORDER BY TAXIS";

        private const string SqlSelectCount = "SELECT COUNT(*) FROM bairong_Department WHERE ParentID = @ParentID";

        private const string SqlUpdate = "UPDATE bairong_Department SET DepartmentName = @DepartmentName, Code = @Code, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildrenCount = @ChildrenCount, IsLastNode = @IsLastNode, Summary = @Summary, CountOfAdmin = @CountOfAdmin WHERE DepartmentID = @DepartmentID";

        private const string ParmId = "@DepartmentID";
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
                departmentInfo.ParentsPath = parentInfo.ParentsPath + "," + parentInfo.DepartmentId;
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

            var sqlInsert = "INSERT INTO bairong_Department (DepartmentName, Code, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, AddDate, Summary, CountOfAdmin) VALUES (@DepartmentName, @Code, @ParentID, @ParentsPath, @ParentsCount, @ChildrenCount, @IsLastNode, @Taxis, @AddDate, @Summary, @CountOfAdmin)";

            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmName, EDataType.NVarChar, 255, departmentInfo.DepartmentName),
                GetParameter(ParmCode, EDataType.VarChar, 50, departmentInfo.Code),
				GetParameter(ParmParentId, EDataType.Integer, departmentInfo.ParentId),
				GetParameter(ParmParentsPath, EDataType.NVarChar, 255, departmentInfo.ParentsPath),
				GetParameter(ParmParentsCount, EDataType.Integer, departmentInfo.ParentsCount),
				GetParameter(ParmChildrenCount, EDataType.Integer, 0),
				GetParameter(ParmIsLastNode, EDataType.VarChar, 18, true.ToString()),
				GetParameter(ParmTaxis, EDataType.Integer, departmentInfo.Taxis),
				GetParameter(ParmAddDate, EDataType.DateTime, departmentInfo.AddDate),
				GetParameter(ParmSummary, EDataType.NVarChar, 255, departmentInfo.Summary),
				GetParameter(ParmCountOfAdmin, EDataType.Integer, departmentInfo.CountOfAdmin)
			};

            string sqlString =
                $"UPDATE bairong_Department SET {SqlUtils.GetAddOne("Taxis")} WHERE (Taxis >= {departmentInfo.Taxis})";
            ExecuteNonQuery(trans, sqlString);

            departmentInfo.DepartmentId = ExecuteNonQueryAndReturnId(trans, sqlInsert, insertParms);

            if (!string.IsNullOrEmpty(departmentInfo.ParentsPath))
            {
                sqlString = $"UPDATE bairong_Department SET {SqlUtils.GetAddOne("ChildrenCount")} WHERE DepartmentID IN ({PageUtils.FilterSql(departmentInfo.ParentsPath)})";

                ExecuteNonQuery(trans, sqlString);
            }

            sqlString = $"UPDATE bairong_Department SET IsLastNode = '{false}' WHERE ParentID = {departmentInfo.ParentId}";

            ExecuteNonQuery(trans, sqlString);

            //sqlString =
            //    $"UPDATE bairong_Department SET IsLastNode = '{true}' WHERE (DepartmentID IN (SELECT TOP 1 DepartmentID FROM bairong_Department WHERE ParentID = {departmentInfo.ParentId} ORDER BY Taxis DESC))";

            sqlString =
                $"UPDATE bairong_Department SET IsLastNode = '{true}' WHERE (DepartmentID IN ({SqlUtils.GetInTopSqlString("bairong_Department", "DepartmentID", $"WHERE ParentID = {departmentInfo.ParentId} ORDER BY Taxis DESC", 1)}))";

            ExecuteNonQuery(trans, sqlString);

            DepartmentManager.ClearCache();
        }

        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
        {
            if (!string.IsNullOrEmpty(parentsPath))
            {
                var sqlString = string.Concat("UPDATE bairong_Department SET ChildrenCount = ChildrenCount - ", subtractNum, " WHERE DepartmentID IN (", PageUtils.FilterSql(parentsPath) , ")");
                ExecuteNonQuery(sqlString);

                DepartmentManager.ClearCache();
            }
        }

        private void TaxisSubtract(int selectedDepartmentId)
        {
            var departmentInfo = GetDepartmentInfo(selectedDepartmentId);
            if (departmentInfo == null) return;
            //Get Lower Taxis and DepartmentID
            int lowerDepartmentId;
            int lowerChildrenCount;
            string lowerParentsPath;
            //            var sqlString = @"SELECT TOP 1 DepartmentID, ChildrenCount, ParentsPath
            //FROM bairong_Department
            //WHERE (ParentID = @ParentID) AND (DepartmentID <> @DepartmentID) AND (Taxis < @Taxis)
            //ORDER BY Taxis DESC";

            var sqlString = SqlUtils.GetTopSqlString("bairong_Department", "DepartmentID, ChildrenCount, ParentsPath", "WHERE (ParentID = @ParentID) AND (DepartmentID <> @DepartmentID) AND (Taxis < @Taxis) ORDER BY Taxis DESC", 1);

            var parms = new IDataParameter[]
			{
				GetParameter(ParmParentId, EDataType.Integer, departmentInfo.ParentId),
				GetParameter(ParmId, EDataType.Integer, departmentInfo.DepartmentId),
				GetParameter(ParmTaxis, EDataType.Integer, departmentInfo.Taxis),
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    lowerDepartmentId = GetInt(rdr, 0);
                    lowerChildrenCount = GetInt(rdr, 1);
                    lowerParentsPath = GetString(rdr, 2);
                }
                else
                {
                    return;
                }
                rdr.Close();
            }


            var lowerNodePath = string.Concat(lowerParentsPath, ",", lowerDepartmentId);
            var selectedNodePath = string.Concat(departmentInfo.ParentsPath, ",", departmentInfo.DepartmentId);

            SetTaxisSubtract(selectedDepartmentId, selectedNodePath, lowerChildrenCount + 1);
            SetTaxisAdd(lowerDepartmentId, lowerNodePath, departmentInfo.ChildrenCount + 1);

            UpdateIsLastNode(departmentInfo.ParentId);
        }

        private void TaxisAdd(int selectedDepartmentId)
        {
            var departmentInfo = GetDepartmentInfo(selectedDepartmentId);
            if (departmentInfo == null) return;
            //Get Higher Taxis and DepartmentID
            int higherDepartmentId;
            int higherChildrenCount;
            string higherParentsPath;
            //            var sqlString = @"SELECT TOP 1 DepartmentID, ChildrenCount, ParentsPath
            //FROM bairong_Department
            //WHERE (ParentID = @ParentID) AND (DepartmentID <> @DepartmentID) AND (Taxis > @Taxis)
            //ORDER BY Taxis";

            var sqlString = SqlUtils.GetTopSqlString("bairong_Department", "DepartmentID, ChildrenCount, ParentsPath", "WHERE (ParentID = @ParentID) AND (DepartmentID <> @DepartmentID) AND (Taxis > @Taxis) ORDER BY Taxis", 1);

            var parms = new IDataParameter[]
			{
				GetParameter(ParmParentId, EDataType.Integer, departmentInfo.ParentId),
				GetParameter(ParmId, EDataType.Integer, departmentInfo.DepartmentId),
				GetParameter(ParmTaxis, EDataType.Integer, departmentInfo.Taxis)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    higherDepartmentId = GetInt(rdr, 0);
                    higherChildrenCount = GetInt(rdr, 1);
                    higherParentsPath = GetString(rdr, 2);
                }
                else
                {
                    return;
                }
                rdr.Close();
            }


            var higherNodePath = string.Concat(higherParentsPath, ",", higherDepartmentId);
            var selectedNodePath = string.Concat(departmentInfo.ParentsPath, ",", departmentInfo.DepartmentId);

            SetTaxisAdd(selectedDepartmentId, selectedNodePath, higherChildrenCount + 1);
            SetTaxisSubtract(higherDepartmentId, higherNodePath, departmentInfo.ChildrenCount + 1);

            UpdateIsLastNode(departmentInfo.ParentId);
        }

        private void SetTaxisAdd(int departmentId, string parentsPath, int addNum)
        {
            var path = PageUtils.FilterSql(parentsPath);
            string sqlString =
                $"UPDATE bairong_Department SET Taxis = Taxis + {addNum} WHERE DepartmentID = {departmentId} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

            ExecuteNonQuery(sqlString);

            DepartmentManager.ClearCache();
        }

        private void SetTaxisSubtract(int departmentId, string parentsPath, int subtractNum)
        {
            var path = PageUtils.FilterSql(parentsPath);
            string sqlString =
                $"UPDATE bairong_Department SET Taxis = Taxis - {subtractNum} WHERE  DepartmentID = {departmentId} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

            ExecuteNonQuery(sqlString);

            DepartmentManager.ClearCache();
        }

        private void UpdateIsLastNode(int parentId)
        {
            if (parentId > 0)
            {
                var sqlString = "UPDATE bairong_Department SET IsLastNode = @IsLastNode WHERE  ParentID = @ParentID";

                var parms = new IDataParameter[]
			    {
				    GetParameter(ParmIsLastNode, EDataType.VarChar, 18, false.ToString()),
				    GetParameter(ParmParentId, EDataType.Integer, parentId)
			    };

                ExecuteNonQuery(sqlString, parms);

                //sqlString =
                //    $"UPDATE bairong_Department SET IsLastNode = '{true.ToString()}' WHERE (DepartmentID IN (SELECT TOP 1 DepartmentID FROM bairong_Department WHERE ParentID = {parentId} ORDER BY Taxis DESC))";

                sqlString =
                    $"UPDATE bairong_Department SET IsLastNode = '{true}' WHERE (DepartmentID IN ({SqlUtils.GetInTopSqlString("bairong_Department", "DepartmentID", $"WHERE ParentID = {parentId} ORDER BY Taxis DESC", 1)}))";

                ExecuteNonQuery(sqlString);
            }
        }

        private int GetMaxTaxisByParentPath(string parentPath)
        {
            var sqlString = string.Concat("SELECT MAX(Taxis) AS MaxTaxis FROM bairong_Department WHERE (ParentsPath = '", PageUtils.FilterSql(parentPath), "') OR (ParentsPath LIKE '", PageUtils.FilterSql(parentPath), ",%')");
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

            return departmentInfo.DepartmentId;
        }

        public void Update(DepartmentInfo departmentInfo)
        {
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmName, EDataType.NVarChar, 255, departmentInfo.DepartmentName),
                GetParameter(ParmCode, EDataType.VarChar, 50, departmentInfo.Code),
				GetParameter(ParmParentsPath, EDataType.NVarChar, 255, departmentInfo.ParentsPath),
				GetParameter(ParmParentsCount, EDataType.Integer, departmentInfo.ParentsCount),
				GetParameter(ParmChildrenCount, EDataType.Integer, departmentInfo.ChildrenCount),
				GetParameter(ParmIsLastNode, EDataType.VarChar, 18, departmentInfo.IsLastNode.ToString()),
				GetParameter(ParmSummary, EDataType.NVarChar, 255, departmentInfo.Summary),
				GetParameter(ParmCountOfAdmin, EDataType.Integer, departmentInfo.CountOfAdmin),
				GetParameter(ParmId, EDataType.Integer, departmentInfo.DepartmentId)
			};

            ExecuteNonQuery(SqlUpdate, updateParms);

            DepartmentManager.ClearCache();
        }

        public void UpdateTaxis(int selectedDepartmentId, bool isSubtract)
        {
            if (isSubtract)
            {
                TaxisSubtract(selectedDepartmentId);
            }
            else
            {
                TaxisAdd(selectedDepartmentId);
            }
        }

        public void UpdateCountOfAdmin()
        {
            var departmentIdList = DepartmentManager.GetDepartmentIdList();
            foreach (var departmentId in departmentIdList)
            {
                string sqlString =
                    $"UPDATE bairong_Department SET CountOfAdmin = (SELECT COUNT(*) AS CountOfAdmin FROM bairong_Administrator WHERE DepartmentID = {departmentId}) WHERE DepartmentID = {departmentId}";
                ExecuteNonQuery(sqlString);
            }
            DepartmentManager.ClearCache();
        }

        public void Delete(int departmentId)
        {
            var departmentInfo = GetDepartmentInfo(departmentId);
            if (departmentInfo != null)
            {
                var departmentIdList = new List<int>();
                if (departmentInfo.ChildrenCount > 0)
                {
                    departmentIdList = GetDepartmentIdListForDescendant(departmentId);
                }
                departmentIdList.Add(departmentId);

                string sqlString =
                    $"DELETE FROM bairong_Department WHERE DepartmentID IN ({TranslateUtils.ToSqlInStringWithoutQuote(departmentIdList)})";

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
                                    $"UPDATE bairong_Department SET Taxis = Taxis - {deletedNum} WHERE (Taxis > {departmentInfo.Taxis})";
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

        private DepartmentInfo GetDepartmentInfo(int departmentId)
		{
            DepartmentInfo departmentInfo = null;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmId, EDataType.Integer, departmentId)
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

		public int GetNodeCount(int departmentId)
		{
			var nodeCount = 0;

			var nodeParms = new IDataParameter[]
			{
				GetParameter(ParmParentId, EDataType.Integer, departmentId)
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

        public List<int> GetDepartmentIdListByParentId(int parentId)
        {
            string sqlString =
                $@"SELECT DepartmentID FROM bairong_Department WHERE ParentID = '{parentId}' ORDER BY Taxis";
            var list = new List<int>();

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var theDepartmentId = GetInt(rdr, 0);
                    list.Add(theDepartmentId);
                }
                rdr.Close();
            }

            return list;
        }

		public List<int> GetDepartmentIdListForDescendant(int departmentId)
		{
            string sqlString = $@"SELECT DepartmentID
FROM bairong_Department
WHERE (ParentsPath LIKE '{departmentId},%') OR
      (ParentsPath LIKE '%,{departmentId},%') OR
      (ParentsPath LIKE '%,{departmentId}') OR
      (ParentID = {departmentId})
";
			var list = new List<int>();

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var theDepartmentId = GetInt(rdr, 0);
                    list.Add(theDepartmentId);
                }
                rdr.Close();
            }

			return list;
		}

        public List<int> GetDepartmentIdListByDepartmentIdCollection(string departmentIdCollection)
        {
            var list = new List<int>();

            if (string.IsNullOrEmpty(departmentIdCollection)) return list;

            string sqlString =
                $@"SELECT DepartmentID FROM bairong_Department WHERE DepartmentID IN ({departmentIdCollection})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var theDepartmentId = GetInt(rdr, 0);
                    list.Add(theDepartmentId);
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetDepartmentIdListByFirstDepartmentIdList(List<int> firstIdList)
        {
            var list = new List<int>();

            if (firstIdList.Count <= 0) return list;

            var builder = new StringBuilder();
            foreach (var departmentId in firstIdList)
            {
                builder.Append($"DepartmentID = {departmentId} OR ParentID = {departmentId} OR ParentsPath LIKE '{departmentId},%' OR ");
            }
            builder.Length -= 3;

            string sqlString =
                $"SELECT DepartmentID FROM bairong_Department WHERE {builder} ORDER BY Taxis";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var departmentId = GetInt(rdr, 0);
                    list.Add(departmentId);
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
                var pair = new KeyValuePair<int, DepartmentInfo>(departmentInfo.DepartmentId, departmentInfo);
                list.Add(pair);
            }

            return list;
        }
	}
}
