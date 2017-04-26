using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
    public class AreaDao : DataProviderBase
    {
        public string TableName => "bairong_Area";

        private const string SqlSelect = "SELECT AreaID, AreaName, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, CountOfAdmin FROM bairong_Area WHERE AreaID = @AreaID";
        private const string SqlSelectAll = "SELECT AreaID, AreaName, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, CountOfAdmin FROM bairong_Area ORDER BY TAXIS";
        private const string SqlSelectCount = "SELECT COUNT(*) FROM bairong_Area WHERE ParentID = @ParentID";
        private const string SqlUpdate = "UPDATE bairong_Area SET AreaName = @AreaName, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildrenCount = @ChildrenCount, IsLastNode = @IsLastNode, CountOfAdmin = @CountOfAdmin WHERE AreaID = @AreaID";

        private const string ParmId = "@AreaID";
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
                areaInfo.ParentsPath = parentInfo.ParentsPath + "," + parentInfo.AreaId;
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

            var sqlInsert = "INSERT INTO bairong_Area (AreaName, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, CountOfAdmin) VALUES (@AreaName, @ParentID, @ParentsPath, @ParentsCount, @ChildrenCount, @IsLastNode, @Taxis, @CountOfAdmin)";

            IDataParameter[] insertParms = {
				GetParameter(ParmName, EDataType.NVarChar, 255, areaInfo.AreaName),
				GetParameter(ParmParentId, EDataType.Integer, areaInfo.ParentId),
				GetParameter(ParmParentsPath, EDataType.NVarChar, 255, areaInfo.ParentsPath),
				GetParameter(ParmParentsCount, EDataType.Integer, areaInfo.ParentsCount),
				GetParameter(ParmChildrenCount, EDataType.Integer, 0),
				GetParameter(ParmIsLastNode, EDataType.VarChar, 18, true.ToString()),
				GetParameter(ParmTaxis, EDataType.Integer, areaInfo.Taxis),
				GetParameter(ParmCountOfAdmin, EDataType.Integer, areaInfo.CountOfAdmin)
			};

            string sqlString = $"UPDATE bairong_Area SET {SqlUtils.GetAddOne("Taxis")} WHERE (Taxis >= {areaInfo.Taxis})";
            ExecuteNonQuery(trans, sqlString);

            areaInfo.AreaId = ExecuteNonQueryAndReturnId(trans, sqlInsert, insertParms);

            if (!string.IsNullOrEmpty(areaInfo.ParentsPath))
            {
                sqlString = $"UPDATE bairong_Area SET {SqlUtils.GetAddOne("ChildrenCount")} WHERE AreaID IN ({PageUtils.FilterSql(areaInfo.ParentsPath)})";

                ExecuteNonQuery(trans, sqlString);
            }

            sqlString = $"UPDATE bairong_Area SET IsLastNode = '{false}' WHERE ParentID = {areaInfo.ParentId}";

            ExecuteNonQuery(trans, sqlString);

            //sqlString =
            //    $"UPDATE bairong_Area SET IsLastNode = 'True' WHERE (AreaID IN (SELECT TOP 1 AreaID FROM bairong_Area WHERE ParentID = {areaInfo.ParentId} ORDER BY Taxis DESC))";            
            sqlString =
                $"UPDATE bairong_Area SET IsLastNode = '{true}' WHERE AreaID IN ({SqlUtils.GetInTopSqlString(TableName, "AreaID", $"WHERE ParentID = {areaInfo.ParentId} ORDER BY Taxis DESC", 1)})";

            ExecuteNonQuery(trans, sqlString);

            AreaManager.ClearCache();
        }

        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
        {
            if (!string.IsNullOrEmpty(parentsPath))
            {
                var sqlString = string.Concat("UPDATE bairong_Area SET ChildrenCount = ChildrenCount - ", subtractNum, " WHERE AreaID IN (", PageUtils.FilterSql(parentsPath), ")");
                ExecuteNonQuery(sqlString);

                AreaManager.ClearCache();
            }
        }

        private void TaxisSubtract(int selectedAreaId)
        {
            var areaInfo = GetAreaInfo(selectedAreaId);
            if (areaInfo == null) return;
            //Get Lower Taxis and AreaID
            int lowerAreaId;
            int lowerChildrenCount;
            string lowerParentsPath;
            //            const string sqlString = @"SELECT TOP 1 AreaID, ChildrenCount, ParentsPath
            //FROM bairong_Area
            //WHERE (ParentID = @ParentID) AND (AreaID <> @AreaID) AND (Taxis < @Taxis)
            //ORDER BY Taxis DESC";
            var sqlString = SqlUtils.GetTopSqlString(TableName, "AreaID, ChildrenCount, ParentsPath",
                "WHERE (ParentID = @ParentID) AND (AreaID <> @AreaID) AND (Taxis < @Taxis) ORDER BY Taxis DESC", 1);

            IDataParameter[] parms = {
				GetParameter(ParmParentId, EDataType.Integer, areaInfo.ParentId),
				GetParameter(ParmId, EDataType.Integer, areaInfo.AreaId),
				GetParameter(ParmTaxis, EDataType.Integer, areaInfo.Taxis),
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    lowerAreaId = GetInt(rdr, 0);
                    lowerChildrenCount = GetInt(rdr, 1);
                    lowerParentsPath = GetString(rdr, 2);
                }
                else
                {
                    return;
                }
                rdr.Close();
            }


            var lowerNodePath = string.Concat(lowerParentsPath, ",", lowerAreaId);
            var selectedNodePath = string.Concat(areaInfo.ParentsPath, ",", areaInfo.AreaId);

            SetTaxisSubtract(selectedAreaId, selectedNodePath, lowerChildrenCount + 1);
            SetTaxisAdd(lowerAreaId, lowerNodePath, areaInfo.ChildrenCount + 1);

            UpdateIsLastNode(areaInfo.ParentId);
        }

        private void TaxisAdd(int selectedAreaId)
        {
            var areaInfo = GetAreaInfo(selectedAreaId);
            if (areaInfo == null) return;
            //Get Higher Taxis and AreaID
            int higherAreaId;
            int higherChildrenCount;
            string higherParentsPath;
            //            var sqlString = @"SELECT TOP 1 AreaID, ChildrenCount, ParentsPath
            //FROM bairong_Area
            //WHERE (ParentID = @ParentID) AND (AreaID <> @AreaID) AND (Taxis > @Taxis)
            //ORDER BY Taxis";
            var sqlString = SqlUtils.GetTopSqlString(TableName, "AreaID, ChildrenCount, ParentsPath",
                "WHERE (ParentID = @ParentID) AND (AreaID <> @AreaID) AND (Taxis > @Taxis) ORDER BY Taxis", 1);

            IDataParameter[] parms = {
				GetParameter(ParmParentId, EDataType.Integer, areaInfo.ParentId),
				GetParameter(ParmId, EDataType.Integer, areaInfo.AreaId),
				GetParameter(ParmTaxis, EDataType.Integer, areaInfo.Taxis)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    higherAreaId = GetInt(rdr, 0);
                    higherChildrenCount = GetInt(rdr, 1);
                    higherParentsPath = GetString(rdr, 2);
                }
                else
                {
                    return;
                }
                rdr.Close();
            }


            var higherNodePath = string.Concat(higherParentsPath, ",", higherAreaId);
            var selectedNodePath = string.Concat(areaInfo.ParentsPath, ",", areaInfo.AreaId);

            SetTaxisAdd(selectedAreaId, selectedNodePath, higherChildrenCount + 1);
            SetTaxisSubtract(higherAreaId, higherNodePath, areaInfo.ChildrenCount + 1);

            UpdateIsLastNode(areaInfo.ParentId);
        }

        private void SetTaxisAdd(int areaId, string parentsPath, int addNum)
        {
            var path = PageUtils.FilterSql(parentsPath);
            string sqlString =
                $"UPDATE bairong_Area SET Taxis = Taxis + {addNum} WHERE AreaID = {areaId} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

            ExecuteNonQuery(sqlString);

            AreaManager.ClearCache();
        }

        private void SetTaxisSubtract(int areaId, string parentsPath, int subtractNum)
        {
            var path = PageUtils.FilterSql(parentsPath);
            string sqlString =
                $"UPDATE bairong_Area SET Taxis = Taxis - {subtractNum} WHERE  AreaID = {areaId} OR ParentsPath = '{path}' OR ParentsPath LIKE '{path},%'";

            ExecuteNonQuery(sqlString);

            AreaManager.ClearCache();
        }

        private void UpdateIsLastNode(int parentId)
        {
            if (parentId > 0)
            {
                var sqlString = "UPDATE bairong_Area SET IsLastNode = @IsLastNode WHERE ParentID = @ParentID";

                IDataParameter[] parms = {
				    GetParameter(ParmIsLastNode, EDataType.VarChar, 18, false.ToString()),
				    GetParameter(ParmParentId, EDataType.Integer, parentId)
			    };

                ExecuteNonQuery(sqlString, parms);

                //sqlString =
                //    $"UPDATE bairong_Area SET IsLastNode = '{true}' WHERE (AreaID IN (SELECT TOP 1 AreaID FROM bairong_Area WHERE ParentID = {parentId} ORDER BY Taxis DESC))";
                sqlString =
                    $"UPDATE bairong_Area SET IsLastNode = '{true}' WHERE AreaID IN ({SqlUtils.GetInTopSqlString(TableName, "AreaID", $"WHERE ParentID = {parentId} ORDER BY Taxis DESC", 1)})";

                ExecuteNonQuery(sqlString);
            }
        }

        private int GetMaxTaxisByParentPath(string parentPath)
        {
            var sqlString = string.Concat("SELECT MAX(Taxis) AS MaxTaxis FROM bairong_Area WHERE (ParentsPath = '", PageUtils.FilterSql(parentPath), "') OR (ParentsPath LIKE '", PageUtils.FilterSql(parentPath), ",%')");
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

            return areaInfo.AreaId;
        }

        public void Update(AreaInfo areaInfo)
        {
            IDataParameter[] updateParms = {
				GetParameter(ParmName, EDataType.NVarChar, 255, areaInfo.AreaName),
				GetParameter(ParmParentsPath, EDataType.NVarChar, 255, areaInfo.ParentsPath),
				GetParameter(ParmParentsCount, EDataType.Integer, areaInfo.ParentsCount),
				GetParameter(ParmChildrenCount, EDataType.Integer, areaInfo.ChildrenCount),
				GetParameter(ParmIsLastNode, EDataType.VarChar, 18, areaInfo.IsLastNode.ToString()),
				GetParameter(ParmCountOfAdmin, EDataType.Integer, areaInfo.CountOfAdmin),
				GetParameter(ParmId, EDataType.Integer, areaInfo.AreaId)
			};

            ExecuteNonQuery(SqlUpdate, updateParms);

            AreaManager.ClearCache();
        }

        public void UpdateTaxis(int selectedAreaId, bool isSubtract)
        {
            if (isSubtract)
            {
                TaxisSubtract(selectedAreaId);
            }
            else
            {
                TaxisAdd(selectedAreaId);
            }
        }

        public void UpdateCountOfAdmin()
        {
            var areaIdList = AreaManager.GetAreaIdList();
            foreach (var areaId in areaIdList)
            {
                string sqlString =
                    $"UPDATE bairong_Area SET CountOfAdmin = (SELECT COUNT(*) AS CountOfAdmin FROM bairong_Administrator WHERE AreaID = {areaId}) WHERE AreaID = {areaId}";
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
                    areaIdList = GetAreaIdListForDescendant(areaId);
                }
                areaIdList.Add(areaId);

                string sqlString =
                    $"DELETE FROM bairong_Area WHERE AreaID IN ({TranslateUtils.ToSqlInStringWithoutQuote(areaIdList)})";

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
                                    $"UPDATE bairong_Area SET Taxis = Taxis - {deletedNum} WHERE (Taxis > {areaInfo.Taxis})";
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
				GetParameter(ParmId, EDataType.Integer, areaId)
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
				GetParameter(ParmParentId, EDataType.Integer, areaId)
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

        public List<int> GetAreaIdListByParentId(int parentId)
        {
            string sqlString = $@"SELECT AreaID FROM bairong_Area WHERE ParentID = '{parentId}' ORDER BY Taxis";
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

        public List<int> GetAreaIdListForDescendant(int areaId)
        {
            string sqlString = $@"SELECT AreaID
FROM bairong_Area
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
                    var theAreaId = GetInt(rdr, 0);
                    list.Add(theAreaId);
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetAreaIdListByAreaIdCollection(string areaIdCollection)
        {
            var list = new List<int>();

            if (string.IsNullOrEmpty(areaIdCollection)) return list;

            string sqlString = $@"SELECT AreaID FROM bairong_Area WHERE AreaID IN ({areaIdCollection})";

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

        public List<int> GetAreaIdListByFirstAreaIdList(List<int> firstIdList)
        {
            var list = new List<int>();

            if (firstIdList.Count > 0)
            {
                var builder = new StringBuilder();
                foreach (var areaId in firstIdList)
                {
                    builder.Append($"AreaID = {areaId} OR ParentID = {areaId} OR ParentsPath LIKE '{areaId},%' OR ");
                }
                builder.Length -= 3;

                string sqlString = $"SELECT AreaID FROM bairong_Area WHERE {builder} ORDER BY Taxis";

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
                var pair = new KeyValuePair<int, AreaInfo>(areaInfo.AreaId, areaInfo);
                pairList.Add(pair);
            }

            return pairList;
        }
    }
}
