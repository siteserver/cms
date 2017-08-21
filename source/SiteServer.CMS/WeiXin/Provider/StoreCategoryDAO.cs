using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class StoreCategoryDao : DataProviderBase
    {
        private const string TableName = "wx_StoreCategory";

        private const string SqlSelect = "SELECT ID, PublishmentSystemID, CategoryName, ParentID, Taxis, ChildCount, ParentsCount, ParentsPath, StoreCount, IsLastNode FROM wx_StoreCategory WHERE ID = @ID";

        private const string SqlSelectAll = "SELECT ID, PublishmentSystemID, CategoryName, ParentID, Taxis, ChildCount, ParentsCount, ParentsPath, StoreCount, IsLastNode FROM wx_StoreCategory WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY TAXIS";

        private const string SqlSelectName = "SELECT CategoryName FROM wx_StoreCategory WHERE ID = @ID";

        private const string SqlSelectId = "SELECT ID FROM wx_StoreCategory WHERE ID = @ID";

        private const string SqlSelectParentId = "SELECT ParentID FROM wx_StoreCategory WHERE ID = @ID";

        private const string SqlSelectCount = "SELECT COUNT(*) FROM wx_StoreCategory WHERE PublishmentSystemID = @PublishmentSystemID AND ParentID = @ParentID";

        private const string SqlUpdate = "UPDATE wx_StoreCategory SET CategoryName = @CategoryName, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildCount = @ChildCount, IsLastNode = @IsLastNode WHERE ID = @ID";

        private const string ParmId = "@ID";
        private const string ParmPublishimentsystemid = "@PublishmentSystemID";
        private const string ParmName = "@CategoryName";
        private const string ParmParentId = "@ParentID";
        private const string ParmParentsPath = "@ParentsPath";
        private const string ParmParentsCount = "@ParentsCount";
        private const string ParmChildrenCount = "@ChildCount";
        private const string ParmIsLastNode = "@IsLastNode";
        private const string ParmTaxis = "@Taxis";


        public int Insert(StoreCategoryInfo storeCategoryInfo)
        {
            var storeCategoryId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(storeCategoryInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        storeCategoryId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return storeCategoryId;
        }

        private void InsertWithTrans(int publishmentSystemId, StoreCategoryInfo parentInfo, StoreCategoryInfo categoryInfo, IDbTransaction trans)
        {
            if (parentInfo != null)
            {
                categoryInfo.ParentsPath = parentInfo.ParentsPath + "," + parentInfo.Id;
                categoryInfo.ParentsCount = parentInfo.ParentsCount + 1;

                var maxTaxis = GetMaxTaxisByParentPath(publishmentSystemId, categoryInfo.ParentsPath);
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
                var maxTaxis = GetMaxTaxisByParentPath(publishmentSystemId, "0");
                categoryInfo.Taxis = maxTaxis + 1;
            }

            var sqlInsert = "INSERT INTO wx_StoreCategory (PublishmentSystemID, CategoryName, ParentID, ParentsPath, ParentsCount, ChildCount, IsLastNode, Taxis) VALUES (@PublishmentSystemID, @CategoryName, @ParentID, @ParentsPath, @ParentsCount, @ChildCount, @IsLastNode, @Taxis)";

            var insertParms = new IDataParameter[]
			{
                GetParameter(ParmPublishimentsystemid, DataType.Integer, categoryInfo.PublishmentSystemId),
				GetParameter(ParmName, DataType.NVarChar, 255, categoryInfo.CategoryName),
				GetParameter(ParmParentId, DataType.Integer, categoryInfo.ParentId),
				GetParameter(ParmParentsPath, DataType.NVarChar, 255, categoryInfo.ParentsPath),
				GetParameter(ParmParentsCount, DataType.Integer, categoryInfo.ParentsCount),
				GetParameter(ParmChildrenCount, DataType.Integer, 0),
				GetParameter(ParmIsLastNode, DataType.VarChar, 18, true.ToString()),
				GetParameter(ParmTaxis, DataType.Integer, categoryInfo.Taxis),
			};

            string sqlString = $"UPDATE wx_StoreCategory SET Taxis = Taxis + 1 WHERE (Taxis >= {categoryInfo.Taxis})";
            ExecuteNonQuery(trans, sqlString);

            categoryInfo.Id = ExecuteNonQueryAndReturnId(trans, sqlInsert, insertParms);

            if (!string.IsNullOrEmpty(categoryInfo.ParentsPath))
            {
                sqlString = string.Concat("UPDATE wx_StoreCategory SET ChildCount = ChildCount + 1 WHERE ID in (", categoryInfo.ParentsPath, ")");

                ExecuteNonQuery(trans, sqlString);
            }

            sqlString = $"UPDATE wx_StoreCategory SET IsLastNode = 'False' WHERE ParentID = {categoryInfo.ParentId}";

            ExecuteNonQuery(trans, sqlString);

            sqlString =
                $"UPDATE wx_StoreCategory SET IsLastNode = 'True' WHERE (ID IN (SELECT TOP 1 ID FROM wx_StoreCategory WHERE ParentID = {categoryInfo.ParentId} ORDER BY Taxis DESC))";
            
            ExecuteNonQuery(trans, sqlString);
        }

        private void UpdateSubtractChildCount(int publishmentSystemId, string parentsPath, int subtractNum)
        {
            if (!string.IsNullOrEmpty(parentsPath))
            {
                var sqlString = string.Concat("UPDATE wx_StoreCategory SET ChildCount = ChildCount - ", subtractNum, " WHERE ID IN (", parentsPath, ")");
                ExecuteNonQuery(sqlString);
            }
        }

        public StoreCategoryInfo GetStoreCategoryInfo(int storeId)
        {
            StoreCategoryInfo storeCategoryInfo = null;

            string sqlWhere = $"WHERE ID = {storeId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    storeCategoryInfo = new StoreCategoryInfo(rdr);
                }
                rdr.Close();
            }

            return storeCategoryInfo;
        }

        public StoreCategoryInfo GetStoreCategoryInfoByParentId(int publishmentSystemId, int parentId)
        {
            StoreCategoryInfo storeCategoryInfo = null;

            string sqlWhere = $"WHERE PublishmentSystemID = {publishmentSystemId} AND ParentID = {parentId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    storeCategoryInfo = new StoreCategoryInfo(rdr);
                }
                rdr.Close();
            }

            return storeCategoryInfo;
        }

        public string GetSelectString(int publishmentSystemId)
        {
            string whereString = $"WHERE {StoreCategoryAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public Dictionary<string, int> GetStoreCategoryDictionary(int publishmentSystemId)
        {
            var dictionary = new Dictionary<string, int>();

            string sqlWhere = $" WHERE {StoreCategoryAttribute.PublishmentSystemId} = {publishmentSystemId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, StoreCategoryAttribute.CategoryName + "," + StoreCategoryAttribute.Taxis, sqlWhere);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    dictionary.Add(rdr.GetValue(0).ToString(), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            return dictionary;
        }

        public List<StoreCategoryInfo> GetStoreCategoryInfoList(int publishmentSystemId, int parentId)
        {
            var list = new List<StoreCategoryInfo>();

            var builder = new StringBuilder(
                $"WHERE {StoreCategoryAttribute.PublishmentSystemId} = {publishmentSystemId} AND {StoreCategoryAttribute.ParentId} = {parentId} ");
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY Taxis");

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var categoryInfo = new StoreCategoryInfo(rdr);
                    list.Add(categoryInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public List<StoreCategoryInfo> GetAllStoreCategoryInfoList(int publishmentSystemId)
        {
            var list = new List<StoreCategoryInfo>();

            var builder = new StringBuilder(
                $"WHERE {StoreCategoryAttribute.PublishmentSystemId} = {publishmentSystemId}");
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var storeCategoryInfo = new StoreCategoryInfo(rdr);
                    list.Add(storeCategoryInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public string GetCategoryName(int storeId)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {storeId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, StoreCategoryAttribute.CategoryName, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    title = rdr.GetValue(0).ToString();
                }
                rdr.Close();
            }

            return title;
        }

        private void TaxisSubtract(int publishmentSystemId, int selectedId)
        {
            var categoryInfo = GetCategoryInfo(selectedId);
            if (categoryInfo == null) return;
            //Get Lower Taxis and ID
            var lowerId = 0;
            var lowerChildCount = 0;
            var lowerParentsPath = "";
            var sqlString = @"SELECT TOP 1 ID, ChildCount, ParentsPath
FROM wx_StoreCategory
WHERE PublishmentSystemID = @PublishmentSystemID AND ParentID = @ParentID AND ID <> @ID AND Taxis < @Taxis
ORDER BY Taxis DESC";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishimentsystemid, DataType.Integer, publishmentSystemId),
                GetParameter(ParmParentId, DataType.Integer, categoryInfo.ParentId),
				GetParameter(ParmId, DataType.Integer, categoryInfo.Id),
				GetParameter(ParmTaxis, DataType.Integer, categoryInfo.Taxis),
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    lowerId = Convert.ToInt32(rdr[0]);
                    lowerChildCount = Convert.ToInt32(rdr[1]);
                    lowerParentsPath = rdr.GetValue(2).ToString();
                }
                else
                {
                    return;
                }
                rdr.Close();
            }

            var lowerNodePath = String.Concat(lowerParentsPath, ",", lowerId);
            var selectedNodePath = String.Concat(categoryInfo.ParentsPath, ",", categoryInfo.Id);

            SetTaxisSubtract(publishmentSystemId, selectedId, selectedNodePath, lowerChildCount + 1);
            SetTaxisAdd(publishmentSystemId, lowerId, lowerNodePath, categoryInfo.ChildCount + 1);

            UpdateIsLastNode(publishmentSystemId, categoryInfo.ParentId);
        }

        private void TaxisAdd(int publishmentSystemId, int selectedId)
        {
            var categoryInfo = GetCategoryInfo(selectedId);
            if (categoryInfo == null) return;
            //Get Higher Taxis and ID
            var higherId = 0;
            var higherChildCount = 0;
            var higherParentsPath = "";
            var sqlString = @"SELECT TOP 1 ID, ChildCount, ParentsPath
FROM wx_StoreCategory
WHERE PublishmentSystemID = @PublishmentSystemID AND ParentID = @ParentID AND ID <> @ID AND Taxis > @Taxis
ORDER BY Taxis";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishimentsystemid, DataType.Integer, publishmentSystemId),
				GetParameter(ParmParentId, DataType.Integer, categoryInfo.ParentId),
				GetParameter(ParmId, DataType.Integer, categoryInfo.Id),
				GetParameter(ParmTaxis, DataType.Integer, categoryInfo.Taxis)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    higherId = Convert.ToInt32(rdr[0]);
                    higherChildCount = Convert.ToInt32(rdr[1]);
                    higherParentsPath = rdr.GetValue(2).ToString();
                }
                else
                {
                    return;
                }
                rdr.Close();
            }

            var higherNodePath = String.Concat(higherParentsPath, ",", higherId);
            var selectedNodePath = String.Concat(categoryInfo.ParentsPath, ",", categoryInfo.Id);

            SetTaxisAdd(publishmentSystemId, selectedId, selectedNodePath, higherChildCount + 1);
            SetTaxisSubtract(publishmentSystemId, higherId, higherNodePath, categoryInfo.ChildCount + 1);

            UpdateIsLastNode(publishmentSystemId, categoryInfo.ParentId);
        }

        private void SetTaxisAdd(int publishmentSystemId, int id, string parentsPath, int addNum)
        {
            string sqlString =
                $"UPDATE wx_StoreCategory SET Taxis = Taxis + {addNum} WHERE PublishmentSystemID = {publishmentSystemId} AND (ID = {id} OR ParentsPath = '{parentsPath}' OR ParentsPath like '{parentsPath},%')";

            ExecuteNonQuery(sqlString);
        }

        private void SetTaxisSubtract(int publishmentSystemId, int id, string parentsPath, int subtractNum)
        {
            string sqlString =
                $"UPDATE wx_StoreCategory SET Taxis = Taxis - {subtractNum} WHERE PublishmentSystemID = {publishmentSystemId} AND (ID = {id} OR ParentsPath = '{parentsPath}' OR ParentsPath like '{parentsPath},%')";

            ExecuteNonQuery(sqlString);
        }

        private void UpdateIsLastNode(int publishmentSystemId, int parentId)
        {
            if (parentId > 0)
            {
                var sqlString = "UPDATE wx_StoreCategory SET IsLastNode = @IsLastNode WHERE PublishmentSystemID = @PublishmentSystemID AND ParentID = @ParentID";

                var parms = new IDataParameter[]
			    {
				    GetParameter(ParmIsLastNode, DataType.VarChar, 18, false.ToString()),
                    GetParameter(ParmPublishimentsystemid, DataType.Integer, publishmentSystemId),
				    GetParameter(ParmParentId, DataType.Integer, parentId)
			    };

                ExecuteNonQuery(sqlString, parms);

                sqlString =
                    $"UPDATE wx_StoreCategory SET IsLastNode = '{true.ToString()}' WHERE (ID IN (SELECT TOP 1 ID FROM wx_StoreCategory WHERE ParentID = {parentId} ORDER BY Taxis DESC))";

                ExecuteNonQuery(sqlString);
            }
        }

        private int GetMaxTaxisByParentPath(int publishmentSystemId, string parentPath)
        {
            string sqlString =
                $"SELECT MAX(Taxis) AS MaxTaxis FROM wx_StoreCategory WHERE PublishmentSystemID = {publishmentSystemId} AND (ParentsPath = '{parentPath}' OR ParentsPath LIKE '{parentPath},%')";
            var maxTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                    {
                        maxTaxis = Convert.ToInt32(rdr[0]);
                    }
                }
                rdr.Close();
            }
            return maxTaxis;
        }

        private int GetParentId(int id)
        {
            var parentId = 0;

            var nodeParms = new IDataParameter[]
			{
				GetParameter(ParmId, DataType.Integer, id)
			};

            using (var rdr = ExecuteReader(SqlSelectParentId, nodeParms))
            {
                if (rdr.Read())
                {
                    parentId = Convert.ToInt32(rdr[0]);
                }
                rdr.Close();
            }
            return parentId;
        }

        private int GetIdByParentIdAndOrder(int publishmentSystemId, int parentId, int order)
        {
            var id = parentId;

            string sqlString =
                $"SELECT ID FROM wx_StoreCategory WHERE PublishmentSystemID = {publishmentSystemId} AND ParentID = {parentId} ORDER BY Taxis";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    id = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return id;
        }

        public int Insert(int publishmentSystemId, StoreCategoryInfo categoryInfo)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var parentCategoryInfo = GetCategoryInfo(categoryInfo.ParentId);

                        InsertWithTrans(publishmentSystemId, parentCategoryInfo, categoryInfo, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return categoryInfo.Id;
        }

        public void Update(int publishmentSystemId, StoreCategoryInfo categoryInfo)
        {
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmName, DataType.NVarChar, 255, categoryInfo.CategoryName),
				GetParameter(ParmParentsPath, DataType.NVarChar, 255, categoryInfo.ParentsPath),
				GetParameter(ParmParentsCount, DataType.Integer, categoryInfo.ParentsCount),
				GetParameter(ParmChildrenCount, DataType.Integer, categoryInfo.ChildCount),
				GetParameter(ParmIsLastNode, DataType.VarChar, 18, categoryInfo.IsLastNode.ToString()),

				GetParameter(ParmId, DataType.Integer, categoryInfo.Id)
			};

            ExecuteNonQuery(SqlUpdate, updateParms);
        }

        public void UpdateTaxis(int publishmentSystemId, int selectedId, bool isSubtract)
        {
            if (isSubtract)
            {
                TaxisSubtract(publishmentSystemId, selectedId);
            }
            else
            {
                TaxisAdd(publishmentSystemId, selectedId);
            }
        }

        public void Delete(int publishmentSystemId, int id)
        {
            var categoryInfo = GetCategoryInfo(id);
            if (categoryInfo != null)
            {
                var idList = new List<int>();
                if (categoryInfo.ChildCount > 0)
                {
                    idList = GetCategoryIdListForDescendant(publishmentSystemId, id);
                }
                idList.Add(id);

                string sqlString =
                    $"DELETE FROM wx_StoreCategory WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

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
                                    $"UPDATE wx_StoreCategory SET Taxis = Taxis - {deletedNum} WHERE (Taxis > {categoryInfo.Taxis})";
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
                UpdateIsLastNode(publishmentSystemId, categoryInfo.ParentId);
                UpdateSubtractChildCount(publishmentSystemId, categoryInfo.ParentsPath, deletedNum);
            }
        }

        public StoreCategoryInfo GetCategoryInfo(int categoryId)
        {
            StoreCategoryInfo categoryInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmId, DataType.Integer, categoryId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    categoryInfo = new StoreCategoryInfo(rdr);
                }
                rdr.Close();
            }
            return categoryInfo;
        }

        private ArrayList GetCategoryInfoArrayList(int publishmentSystemId)
        {
            var arraylist = new ArrayList();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishimentsystemid, DataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAll, parms))
            {
                while (rdr.Read())
                {
                    var categoryInfo = new StoreCategoryInfo(rdr);
                    arraylist.Add(categoryInfo);
                }
                rdr.Close();
            }

            return arraylist;
        }

        public int GetNodeCount(int publishmentSystemId, int id)
        {
            var nodeCount = 0;

            var nodeParms = new IDataParameter[]
			{
                GetParameter(ParmPublishimentsystemid, DataType.Integer, publishmentSystemId),
				GetParameter(ParmParentId, DataType.Integer, id)
			};

            using (var rdr = ExecuteReader(SqlSelectCount, nodeParms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    nodeCount = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return nodeCount;
        }

        public List<int> GetCategoryIdListByParentId(int publishmentSystemId, int parentId)
        {
            var list = new List<int>();

            string sqlString =
                $@"SELECT ID FROM wx_StoreCategory WHERE PublishmentSystemID = {publishmentSystemId} AND ParentID = {parentId} ORDER BY Taxis";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read() && !rdr.IsDBNull(0))
                {
                    list.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetCategoryIdListForDescendant(int publishmentSystemId, int categoryId)
        {
            var list = new List<int>();

            string sqlString = $@"SELECT ID
FROM wx_StoreCategory
WHERE PublishmentSystemID = {publishmentSystemId} AND (
    (ParentsPath LIKE '{categoryId},%') OR
    (ParentsPath LIKE '%,{categoryId},%') OR
    (ParentsPath LIKE '%,{categoryId}') OR
    (ParentID = {categoryId})
)
";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read() && !rdr.IsDBNull(0))
                {
                    list.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetAllCategoryIdList(int publishmentSystemId)
        {
            var list = new List<int>();

            string sqlString =
                $@"SELECT ID FROM wx_StoreCategory WHERE PublishmentSystemID = {publishmentSystemId} ORDER BY Taxis";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read() && !rdr.IsDBNull(0))
                {
                    list.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetCategoryIdListForLastNode(int publishmentSystemId, int categoryId)
        {
            var list = new List<int>();

            string sqlString = $@"SELECT ID
FROM wx_StoreCategory
WHERE PublishmentSystemID = {publishmentSystemId} AND ChildCount = 0 AND (
    (ParentsPath LIKE '{categoryId},%') OR
    (ParentsPath LIKE '%,{categoryId},%') OR
    (ParentsPath LIKE '%,{categoryId}') OR
    (ParentID = {categoryId}) OR
    (ID = {categoryId})
)
";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read() && !rdr.IsDBNull(0))
                {
                    list.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetCategoryIdListByCategoryIdCollection(int publishmentSystemId, string idCollection)
        {
            var list = new List<int>();

            if (!string.IsNullOrEmpty(idCollection))
            {
                string sqlString =
                    $@"SELECT ID FROM wx_StoreCategory WHERE PublishmentSystemID = {publishmentSystemId} AND ID IN ({idCollection})";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        list.Add(rdr.GetInt32(0));
                    }
                    rdr.Close();
                }
            }

            return list;
        }

        public List<int> GetCategoryIdListByFirstCategoryIdArrayList(int publishmentSystemId, ArrayList firstIdArrayList)
        {
            var list = new List<int>();

            if (firstIdArrayList.Count > 0)
            {
                var builder = new StringBuilder();
                foreach (int id in firstIdArrayList)
                {
                    builder.AppendFormat("ID = {0} OR ParentID = {0} OR ParentsPath LIKE '{0},%' OR ", id);
                }
                builder.Length -= 3;

                string sqlString =
                    $"SELECT ID FROM wx_StoreCategory WHERE PublishmentSystemID = {publishmentSystemId} AND ({builder.ToString()}) ORDER BY Taxis";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        list.Add(rdr.GetInt32(0));
                    }
                    rdr.Close();
                }
            }

            return list;
        }

        public void UpdateStoreItemCount(int publishmentSystemId)
        {
            var categoryIdList = GetAllCategoryIdList(publishmentSystemId);

            foreach (var categoryId in categoryIdList)
            {
                var count = DataProviderWx.StoreItemDao.GetCount(publishmentSystemId, categoryId);
                string sqlString =
                    $@"UPDATE wx_StoreCategory SET StoreCount = {count} WHERE PublishmentSystemID = {publishmentSystemId} AND ID = {categoryId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public List<StoreCategoryInfo> GetStoreCategoryInfoList(int publishmentSystemId)
        {
            var list = new List<StoreCategoryInfo>();

            var builder = new StringBuilder(
                $"WHERE {StoreCategoryAttribute.PublishmentSystemId} = {publishmentSystemId}");
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY Taxis");

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var categoryInfo = new StoreCategoryInfo(rdr);
                    list.Add(categoryInfo);
                }
                rdr.Close();
            }

            return list;
        }
    }
}
