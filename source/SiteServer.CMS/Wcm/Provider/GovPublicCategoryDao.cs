using System;
using System.Collections;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.Provider
{
    public class GovPublicCategoryDao : DataProviderBase
	{
        private const string SqlSelect = "SELECT CategoryID, ClassCode, PublishmentSystemID, CategoryName, CategoryCode, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, AddDate, Summary, ContentNum FROM wcm_GovPublicCategory WHERE CategoryID = @CategoryID";

        private const string SqlSelectName = "SELECT CategoryName FROM wcm_GovPublicCategory WHERE CategoryID = @CategoryID";

        private const string SqlSelectId = "SELECT CategoryID FROM wcm_GovPublicCategory WHERE CategoryID = @CategoryID";

        private const string SqlSelectCount = "SELECT COUNT(*) FROM wcm_GovPublicCategory WHERE ParentID = @ParentID";

        private const string SqlUpdate = "UPDATE wcm_GovPublicCategory SET CategoryName = @CategoryName, CategoryCode = @CategoryCode, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildrenCount = @ChildrenCount, IsLastNode = @IsLastNode, Summary = @Summary, ContentNum = @ContentNum WHERE CategoryID = @CategoryID";

        private const string ParmCategoryId = "@CategoryID";
        private const string ParmClassCode = "@ClassCode";
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmCategoryName = "@CategoryName";
        private const string ParmCategoryCode = "@CategoryCode";
        private const string ParmParentId = "@ParentID";
        private const string ParmParentsPath = "@ParentsPath";
        private const string ParmParentsCount = "@ParentsCount";
        private const string ParmChildrenCount = "@ChildrenCount";
        private const string ParmIsLastNode = "@IsLastNode";
        private const string ParmTaxis = "@Taxis";
        private const string ParmAddDate = "@AddDate";
        private const string ParmSummary = "@Summary";
        private const string ParmContentNum = "@ContentNum";

        private void InsertWithTrans(GovPublicCategoryInfo parentInfo, GovPublicCategoryInfo categoryInfo, IDbTransaction trans)
        {
            if (parentInfo != null)
            {
                categoryInfo.ParentsPath = parentInfo.ParentsPath + "," + parentInfo.CategoryID;
                categoryInfo.ParentsCount = parentInfo.ParentsCount + 1;

                var maxTaxis = GetMaxTaxisByParentPath(categoryInfo.ClassCode, categoryInfo.PublishmentSystemID, categoryInfo.ParentsPath);
                if (maxTaxis == 0)
                {
                    maxTaxis = parentInfo.Taxis;
                }
                categoryInfo.Taxis = maxTaxis + 1;
            }
            else
            {
                categoryInfo.ParentsPath = "0";
                categoryInfo.ParentsCount = 0;
                var maxTaxis = GetMaxTaxisByParentPath(categoryInfo.ClassCode, categoryInfo.PublishmentSystemID, "0");
                categoryInfo.Taxis = maxTaxis + 1;
            }

            var sqlInsert = "INSERT INTO wcm_GovPublicCategory (ClassCode, PublishmentSystemID, CategoryName, CategoryCode, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, Taxis, AddDate, Summary, ContentNum) VALUES (@ClassCode, @PublishmentSystemID, @CategoryName, @CategoryCode, @ParentID, @ParentsPath, @ParentsCount, @ChildrenCount, @IsLastNode, @Taxis, @AddDate, @Summary, @ContentNum)";

            var insertParms = new IDataParameter[]
			{
                GetParameter(ParmClassCode, EDataType.NVarChar, 50, categoryInfo.ClassCode),
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, categoryInfo.PublishmentSystemID),
				GetParameter(ParmCategoryName, EDataType.NVarChar, 255, categoryInfo.CategoryName),
                GetParameter(ParmCategoryCode, EDataType.VarChar, 50, categoryInfo.CategoryCode),
				GetParameter(ParmParentId, EDataType.Integer, categoryInfo.ParentID),
				GetParameter(ParmParentsPath, EDataType.NVarChar, 255, categoryInfo.ParentsPath),
				GetParameter(ParmParentsCount, EDataType.Integer, categoryInfo.ParentsCount),
				GetParameter(ParmChildrenCount, EDataType.Integer, 0),
				GetParameter(ParmIsLastNode, EDataType.VarChar, 18, true.ToString()),
				GetParameter(ParmTaxis, EDataType.Integer, categoryInfo.Taxis),
				GetParameter(ParmAddDate, EDataType.DateTime, categoryInfo.AddDate),
				GetParameter(ParmSummary, EDataType.NVarChar, 255, categoryInfo.Summary),
				GetParameter(ParmContentNum, EDataType.Integer, categoryInfo.ContentNum)
			};

            string sqlString =
                $"UPDATE wcm_GovPublicCategory SET {SqlUtils.GetAddOne("Taxis")} WHERE (ClassCode = '{categoryInfo.ClassCode}' AND PublishmentSystemID = {categoryInfo.PublishmentSystemID} AND Taxis >= {categoryInfo.Taxis})";
            ExecuteNonQuery(trans, sqlString);

            categoryInfo.CategoryID = ExecuteNonQueryAndReturnId(trans, sqlInsert, insertParms);

            if (!string.IsNullOrEmpty(categoryInfo.ParentsPath))
            {
                sqlString =
                    $"UPDATE wcm_GovPublicCategory SET {SqlUtils.GetAddOne("ChildrenCount")} WHERE CategoryID IN ({categoryInfo.ParentsPath})";

                ExecuteNonQuery(trans, sqlString);
            }

            sqlString =
                $"UPDATE wcm_GovPublicCategory SET IsLastNode = '{false}' WHERE ParentID = {categoryInfo.ParentID} AND ClassCode = '{categoryInfo.ClassCode}' AND PublishmentSystemID = {categoryInfo.PublishmentSystemID}";

            ExecuteNonQuery(trans, sqlString);

            //sqlString =
            //    $"UPDATE wcm_GovPublicCategory SET IsLastNode = '{true}' WHERE (CategoryID IN (SELECT TOP 1 CategoryID FROM wcm_GovPublicCategory WHERE ParentID = {categoryInfo.ParentID} ORDER BY Taxis DESC))";
            sqlString =
                $"UPDATE wcm_GovPublicCategory SET IsLastNode = '{true}' WHERE (CategoryID IN ({SqlUtils.GetInTopSqlString("wcm_GovPublicCategory", "CategoryID", $"WHERE ParentID = {categoryInfo.ParentID} ORDER BY Taxis DESC", 1)}))";

            ExecuteNonQuery(trans, sqlString);
        }

        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
        {
            if (!string.IsNullOrEmpty(parentsPath))
            {
                string sqlString =
                    $"UPDATE wcm_GovPublicCategory SET ChildrenCount = ChildrenCount - {subtractNum} WHERE CategoryID IN ({parentsPath})";
                ExecuteNonQuery(sqlString);
            }
        }

        private void TaxisSubtract(string classCode, int publishmentSystemId, int selectedCategoryId)
        {
            var categoryInfo = GetCategoryInfo(selectedCategoryId);
            if (categoryInfo == null) return;
            //Get Lower Taxis and CategoryID
            int lowerCategoryId;
            int lowerChildrenCount;
            string lowerParentsPath;
            //string sqlString = $@"SELECT TOP 1 CategoryID, ChildrenCount, ParentsPath FROM wcm_GovPublicCategory WHERE (ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId}) AND (ParentID = {categoryInfo.ParentID}) AND (CategoryID <> {categoryInfo.CategoryID}) AND (Taxis < {categoryInfo.Taxis}) ORDER BY Taxis DESC";
            var sqlString = SqlUtils.GetTopSqlString("wcm_GovPublicCategory", "CategoryID, ChildrenCount, ParentsPath", $"WHERE (ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId}) AND (ParentID = {categoryInfo.ParentID}) AND (CategoryID <> {categoryInfo.CategoryID}) AND (Taxis < {categoryInfo.Taxis}) ORDER BY Taxis DESC", 1);

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerCategoryId = GetInt(rdr, 0);
                    lowerChildrenCount = GetInt(rdr, 1);
                    lowerParentsPath = GetString(rdr, 2);
                }
                else
                {
                    return;
                }
                rdr.Close();
            }


            var lowerNodePath = string.Concat(lowerParentsPath, ",", lowerCategoryId);
            var selectedNodePath = string.Concat(categoryInfo.ParentsPath, ",", categoryInfo.CategoryID);

            SetTaxisSubtract(classCode, publishmentSystemId, selectedCategoryId, selectedNodePath, lowerChildrenCount + 1);
            SetTaxisAdd(classCode, publishmentSystemId, lowerCategoryId, lowerNodePath, categoryInfo.ChildrenCount + 1);

            UpdateIsLastNode(classCode, publishmentSystemId, categoryInfo.ParentID);

        }

        private void TaxisAdd(string classCode, int publishmentSystemId, int selectedCategoryId)
        {
            var categoryInfo = GetCategoryInfo(selectedCategoryId);
            if (categoryInfo == null) return;
            //Get Higher Taxis and CategoryID
            int higherCategoryId;
            int higherChildrenCount;
            string higherParentsPath;
            //string sqlString = $@"SELECT TOP 1 CategoryID, ChildrenCount, ParentsPath FROM wcm_GovPublicCategory WHERE (ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId}) AND (ParentID = {categoryInfo.ParentID}) AND (CategoryID <> {categoryInfo.CategoryID}) AND (Taxis > {categoryInfo.Taxis}) ORDER BY Taxis";
            var sqlString = SqlUtils.GetTopSqlString("wcm_GovPublicCategory", "CategoryID, ChildrenCount, ParentsPath", $"SELECT TOP 1  FROM  WHERE (ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId}) AND (ParentID = {categoryInfo.ParentID}) AND (CategoryID <> {categoryInfo.CategoryID}) AND (Taxis > {categoryInfo.Taxis}) ORDER BY Taxis", 1);

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherCategoryId = GetInt(rdr, 0);
                    higherChildrenCount = GetInt(rdr, 1);
                    higherParentsPath = GetString(rdr, 2);
                }
                else
                {
                    return;
                }
                rdr.Close();
            }


            var higherNodePath = string.Concat(higherParentsPath, ",", higherCategoryId);
            var selectedNodePath = string.Concat(categoryInfo.ParentsPath, ",", categoryInfo.CategoryID);

            SetTaxisAdd(classCode, publishmentSystemId, selectedCategoryId, selectedNodePath, higherChildrenCount + 1);
            SetTaxisSubtract(classCode, publishmentSystemId, higherCategoryId, higherNodePath, categoryInfo.ChildrenCount + 1);

            UpdateIsLastNode(categoryInfo.ClassCode, categoryInfo.PublishmentSystemID, categoryInfo.ParentID);
        }

        private void SetTaxisAdd(string classCode, int publishmentSystemId, int categoryId, string parentsPath, int addNum)
        {
            string sqlString =
                $"UPDATE wcm_GovPublicCategory SET Taxis = Taxis + {addNum} WHERE (ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId}) AND (CategoryID = {categoryId} OR ParentsPath = '{parentsPath}' OR ParentsPath LIKE '{parentsPath},%')";

            ExecuteNonQuery(sqlString);
        }

        private void SetTaxisSubtract(string classCode, int publishmentSystemId, int categoryId, string parentsPath, int subtractNum)
        {
            string sqlString =
                $"UPDATE wcm_GovPublicCategory SET Taxis = Taxis - {subtractNum} WHERE (ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId}) AND (CategoryID = {categoryId} OR ParentsPath = '{parentsPath}' OR ParentsPath LIKE '{parentsPath},%')";

            ExecuteNonQuery(sqlString);
        }

        private void UpdateIsLastNode(string classCode, int publishmentSystemId, int parentId)
        {
            if (parentId > 0)
            {
                string sqlString =
                    $"UPDATE wcm_GovPublicCategory SET IsLastNode = '{false}' WHERE (ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId}) AND (ParentID = {parentId})";

                ExecuteNonQuery(sqlString);

                //sqlString =
                //    $"UPDATE wcm_GovPublicCategory SET IsLastNode = '{true}' WHERE (CategoryID IN (SELECT TOP 1 CategoryID FROM wcm_GovPublicCategory WHERE (ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId}) AND (ParentID = {parentId}) ORDER BY Taxis DESC))";
                sqlString = $"UPDATE wcm_GovPublicCategory SET IsLastNode = '{true}' WHERE (CategoryID IN ({SqlUtils.GetInTopSqlString("wcm_GovPublicCategory", "CategoryID", $"WHERE (ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId}) AND (ParentID = {parentId}) ORDER BY Taxis DESC", 1)}))";

                ExecuteNonQuery(sqlString);
            }
        }

        private int GetMaxTaxisByParentPath(string classCode, int publishmentSystemId, string parentPath)
        {
            string sqlString =
                $"SELECT MAX(Taxis) AS MaxTaxis FROM wcm_GovPublicCategory WHERE (ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId}) AND (ParentsPath = '{parentPath}' OR ParentsPath LIKE '{parentPath},%')";
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

        public int Insert(GovPublicCategoryInfo categoryInfo)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var parentDepartmentInfo = GetCategoryInfo(categoryInfo.ParentID);

                        InsertWithTrans(parentDepartmentInfo, categoryInfo, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return categoryInfo.CategoryID;
        }

        public void Update(GovPublicCategoryInfo categoryInfo)
        {
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmCategoryName, EDataType.NVarChar, 255, categoryInfo.CategoryName),
                GetParameter(ParmCategoryCode, EDataType.VarChar, 50, categoryInfo.CategoryCode),
				GetParameter(ParmParentsPath, EDataType.NVarChar, 255, categoryInfo.ParentsPath),
				GetParameter(ParmParentsCount, EDataType.Integer, categoryInfo.ParentsCount),
				GetParameter(ParmChildrenCount, EDataType.Integer, categoryInfo.ChildrenCount),
				GetParameter(ParmIsLastNode, EDataType.VarChar, 18, categoryInfo.IsLastNode.ToString()),
				GetParameter(ParmSummary, EDataType.NVarChar, 255, categoryInfo.Summary),
				GetParameter(ParmContentNum, EDataType.Integer, categoryInfo.ContentNum),
				GetParameter(ParmCategoryId, EDataType.Integer, categoryInfo.CategoryID)
			};

            ExecuteNonQuery(SqlUpdate, updateParms);
        }

        public void UpdateTaxis(string classCode, int publishmentSystemId, int selectedCategoryId, bool isSubtract)
        {
            if (isSubtract)
            {
                TaxisSubtract(classCode, publishmentSystemId, selectedCategoryId);
            }
            else
            {
                TaxisAdd(classCode, publishmentSystemId, selectedCategoryId);
            }
        }

        public virtual void UpdateContentNum(PublishmentSystemInfo publishmentSystemInfo, string contentAttributeName, int categoryId)
        {
            string sqlString =
                $"UPDATE wcm_GovPublicCategory SET ContentNum = (SELECT COUNT(*) AS ContentNum FROM {publishmentSystemInfo.AuxiliaryTableForGovPublic} WHERE ({contentAttributeName} = {categoryId})) WHERE (CategoryID = {categoryId})";

            ExecuteNonQuery(sqlString);
        }

        public void Delete(int categoryId)
        {
            var categoryInfo = GetCategoryInfo(categoryId);
            if (categoryInfo != null)
            {
                var categoryIdArrayList = new ArrayList();
                if (categoryInfo.ChildrenCount > 0)
                {
                    categoryIdArrayList = GetCategoryIdArrayListForDescendant(categoryInfo.ClassCode, categoryInfo.PublishmentSystemID, categoryId);
                }
                categoryIdArrayList.Add(categoryId);

                string sqlString =
                    $"DELETE FROM wcm_GovPublicCategory WHERE CategoryID IN ({TranslateUtils.ToSqlInStringWithoutQuote(categoryIdArrayList)})";

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
                                    $"UPDATE wcm_GovPublicCategory SET Taxis = Taxis - {deletedNum} WHERE ClassCode = '{categoryInfo.ClassCode}' AND  PublishmentSystemID = {categoryInfo.PublishmentSystemID} AND Taxis > {categoryInfo.Taxis}";
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
                UpdateIsLastNode(categoryInfo.ClassCode, categoryInfo.PublishmentSystemID, categoryInfo.ParentID);
                UpdateSubtractChildrenCount(categoryInfo.ParentsPath, deletedNum);
            }
        }

        public GovPublicCategoryInfo GetCategoryInfo(int categoryId)
		{
            GovPublicCategoryInfo categoryInfo = null;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmCategoryId, EDataType.Integer, categoryId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    categoryInfo = new GovPublicCategoryInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetInt(rdr, i));
				}
				rdr.Close();
			}
            return categoryInfo;
		}

        public string GetCategoryName(int categoryId)
        {
            var departmentName = string.Empty;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmCategoryId, EDataType.Integer, categoryId)
			};

            using (var rdr = ExecuteReader(SqlSelectName, parms))
            {
                if (rdr.Read())
                {
                    departmentName = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return departmentName;
        }

		public int GetNodeCount(int categoryId)
		{
			var nodeCount = 0;

			var nodeParms = new IDataParameter[]
			{
				GetParameter(ParmParentId, EDataType.Integer, categoryId)
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

		public bool IsExists(int categoryId)
		{
			var exists = false;

			var nodeParms = new IDataParameter[]
			{
				GetParameter(ParmCategoryId, EDataType.Integer, categoryId)
			};

			using (var rdr = ExecuteReader(SqlSelectId, nodeParms))
			{
				if (rdr.Read())
				{
					if (!rdr.IsDBNull(0))
					{
						exists = true;
					}
				}
				rdr.Close();
			}
			return exists;
		}

        public ArrayList GetCategoryIdArrayList(string classCode, int publishmentSystemId)
        {
            var arraylist = new ArrayList();
            string sqlString =
                $"SELECT CategoryID FROM wcm_GovPublicCategory WHERE ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId} ORDER BY Taxis";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var categoryId = GetInt(rdr, 0);
                    arraylist.Add(categoryId);
                }
                rdr.Close();
            }

            return arraylist;
        }

        public ArrayList GetCategoryIdArrayListByParentId(string classCode, int publishmentSystemId, int parentId)
        {
            string sqlString =
                $@"SELECT CategoryID FROM wcm_GovPublicCategory WHERE ClassCode = '{classCode}' AND  PublishmentSystemID = {publishmentSystemId} AND ParentID = {parentId} ORDER BY Taxis";
            var list = new ArrayList();

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var theCategoryId = GetInt(rdr, 0);
                    list.Add(theCategoryId);
                }
                rdr.Close();
            }

            return list;
        }

		public ArrayList GetCategoryIdArrayListForDescendant(string classCode, int publishmentSystemId, int categoryId)
		{
            string sqlString = $@"SELECT CategoryID
FROM wcm_GovPublicCategory
WHERE (ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId}) AND (
      (ParentsPath LIKE '{categoryId},%') OR
      (ParentsPath LIKE '%,{categoryId},%') OR
      (ParentsPath LIKE '%,{categoryId}') OR
      (ParentID = {categoryId}))
";
			var list = new ArrayList();

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var theCategoryId = GetInt(rdr, 0);
                    list.Add(theCategoryId);
                }
                rdr.Close();
            }

			return list;
		}
	}
}
