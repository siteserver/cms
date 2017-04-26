using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class StoreCategoryDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_StoreCategory";

        private const string SQL_SELECT = "SELECT ID, PublishmentSystemID, CategoryName, ParentID, Taxis, ChildCount, ParentsCount, ParentsPath, StoreCount, IsLastNode FROM wx_StoreCategory WHERE ID = @ID";

        private const string SQL_SELECT_ALL = "SELECT ID, PublishmentSystemID, CategoryName, ParentID, Taxis, ChildCount, ParentsCount, ParentsPath, StoreCount, IsLastNode FROM wx_StoreCategory WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY TAXIS";

        private const string SQL_SELECT_NAME = "SELECT CategoryName FROM wx_StoreCategory WHERE ID = @ID";

        private const string SQL_SELECT_ID = "SELECT ID FROM wx_StoreCategory WHERE ID = @ID";

        private const string SQL_SELECT_PARENT_ID = "SELECT ParentID FROM wx_StoreCategory WHERE ID = @ID";

        private const string SQL_SELECT_COUNT = "SELECT COUNT(*) FROM wx_StoreCategory WHERE PublishmentSystemID = @PublishmentSystemID AND ParentID = @ParentID";

        private const string SQL_UPDATE = "UPDATE wx_StoreCategory SET CategoryName = @CategoryName, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildCount = @ChildCount, IsLastNode = @IsLastNode WHERE ID = @ID";

        private const string PARM_ID = "@ID";
        private const string PARM_PUBLISHIMENTSYSTEMID = "@PublishmentSystemID";
        private const string PARM_NAME = "@CategoryName";
        private const string PARM_PARENT_ID = "@ParentID";
        private const string PARM_PARENTS_PATH = "@ParentsPath";
        private const string PARM_PARENTS_COUNT = "@ParentsCount";
        private const string PARM_CHILDREN_COUNT = "@ChildCount";
        private const string PARM_IS_LAST_NODE = "@IsLastNode";
        private const string PARM_TAXIS = "@Taxis";


        public int Insert(StoreCategoryInfo storeCategoryInfo)
        {
            var storeCategoryID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(storeCategoryInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        storeCategoryID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return storeCategoryID;
        }

        private void InsertWithTrans(int publishmentSystemID, StoreCategoryInfo parentInfo, StoreCategoryInfo categoryInfo, IDbTransaction trans)
        {
            if (parentInfo != null)
            {
                categoryInfo.ParentsPath = parentInfo.ParentsPath + "," + parentInfo.ID;
                categoryInfo.ParentsCount = parentInfo.ParentsCount + 1;

                var maxTaxis = GetMaxTaxisByParentPath(publishmentSystemID, categoryInfo.ParentsPath);
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
                var maxTaxis = GetMaxTaxisByParentPath(publishmentSystemID, "0");
                categoryInfo.Taxis = maxTaxis + 1;
            }

            var SQL_INSERT = "INSERT INTO wx_StoreCategory (PublishmentSystemID, CategoryName, ParentID, ParentsPath, ParentsCount, ChildCount, IsLastNode, Taxis) VALUES (@PublishmentSystemID, @CategoryName, @ParentID, @ParentsPath, @ParentsCount, @ChildCount, @IsLastNode, @Taxis)";

            var insertParms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHIMENTSYSTEMID, EDataType.Integer, categoryInfo.PublishmentSystemID),
				GetParameter(PARM_NAME, EDataType.NVarChar, 255, categoryInfo.CategoryName),
				GetParameter(PARM_PARENT_ID, EDataType.Integer, categoryInfo.ParentID),
				GetParameter(PARM_PARENTS_PATH, EDataType.NVarChar, 255, categoryInfo.ParentsPath),
				GetParameter(PARM_PARENTS_COUNT, EDataType.Integer, categoryInfo.ParentsCount),
				GetParameter(PARM_CHILDREN_COUNT, EDataType.Integer, 0),
				GetParameter(PARM_IS_LAST_NODE, EDataType.VarChar, 18, true.ToString()),
				GetParameter(PARM_TAXIS, EDataType.Integer, categoryInfo.Taxis),
			};

            string sqlString = $"UPDATE wx_StoreCategory SET Taxis = Taxis + 1 WHERE (Taxis >= {categoryInfo.Taxis})";
            ExecuteNonQuery(trans, sqlString);

            ExecuteNonQuery(trans, SQL_INSERT, insertParms);

            categoryInfo.ID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, "wx_StoreCategory");

            if (!string.IsNullOrEmpty(categoryInfo.ParentsPath))
            {
                sqlString = string.Concat("UPDATE wx_StoreCategory SET ChildCount = ChildCount + 1 WHERE ID in (", categoryInfo.ParentsPath, ")");

                ExecuteNonQuery(trans, sqlString);
            }

            sqlString = $"UPDATE wx_StoreCategory SET IsLastNode = 'False' WHERE ParentID = {categoryInfo.ParentID}";

            ExecuteNonQuery(trans, sqlString);

            sqlString =
                $"UPDATE wx_StoreCategory SET IsLastNode = 'True' WHERE (ID IN (SELECT TOP 1 ID FROM wx_StoreCategory WHERE ParentID = {categoryInfo.ParentID} ORDER BY Taxis DESC))";
            
            ExecuteNonQuery(trans, sqlString);
        }

        private void UpdateSubtractChildCount(int publishmentSystemID, string parentsPath, int subtractNum)
        {
            if (!string.IsNullOrEmpty(parentsPath))
            {
                var sqlString = string.Concat("UPDATE wx_StoreCategory SET ChildCount = ChildCount - ", subtractNum, " WHERE ID IN (", parentsPath, ")");
                ExecuteNonQuery(sqlString);
            }
        }

        public StoreCategoryInfo GetStoreCategoryInfo(int storeID)
        {
            StoreCategoryInfo storeCategoryInfo = null;

            string SQL_WHERE = $"WHERE ID = {storeID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    storeCategoryInfo = new StoreCategoryInfo(rdr);
                }
                rdr.Close();
            }

            return storeCategoryInfo;
        }

        public StoreCategoryInfo GetStoreCategoryInfoByParentID(int publishmentSystemID, int parentID)
        {
            StoreCategoryInfo storeCategoryInfo = null;

            string SQL_WHERE = $"WHERE PublishmentSystemID = {publishmentSystemID} AND ParentID = {parentID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    storeCategoryInfo = new StoreCategoryInfo(rdr);
                }
                rdr.Close();
            }

            return storeCategoryInfo;
        }

        public string GetSelectString(int publishmentSystemID)
        {
            string whereString = $"WHERE {StoreCategoryAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public Dictionary<string, int> GetStoreCategoryDictionary(int publishmentSystemID)
        {
            var dictionary = new Dictionary<string, int>();

            string SQL_WHERE = $" WHERE {StoreCategoryAttribute.PublishmentSystemID} = {publishmentSystemID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, StoreCategoryAttribute.CategoryName + "," + StoreCategoryAttribute.Taxis, SQL_WHERE);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    dictionary.Add(rdr.GetValue(0).ToString(), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            return dictionary;
        }

        public List<StoreCategoryInfo> GetStoreCategoryInfoList(int publishmentSystemID, int parentID)
        {
            var list = new List<StoreCategoryInfo>();

            var builder = new StringBuilder(
                $"WHERE {StoreCategoryAttribute.PublishmentSystemID} = {publishmentSystemID} AND {StoreCategoryAttribute.ParentID} = {parentID} ");
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY Taxis");

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public List<StoreCategoryInfo> GetAllStoreCategoryInfoList(int publishmentSystemID)
        {
            var list = new List<StoreCategoryInfo>();

            var builder = new StringBuilder(
                $"WHERE {StoreCategoryAttribute.PublishmentSystemID} = {publishmentSystemID}");
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var StoreCategoryInfo = new StoreCategoryInfo(rdr);
                    list.Add(StoreCategoryInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public string GetCategoryName(int storeID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {storeID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, StoreCategoryAttribute.CategoryName, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    title = rdr.GetValue(0).ToString();
                }
                rdr.Close();
            }

            return title;
        }

        private void TaxisSubtract(int publishmentSystemID, int selectedID)
        {
            var categoryInfo = GetCategoryInfo(selectedID);
            if (categoryInfo == null) return;
            //Get Lower Taxis and ID
            var lowerID = 0;
            var lowerChildCount = 0;
            var lowerParentsPath = "";
            var sqlString = @"SELECT TOP 1 ID, ChildCount, ParentsPath
FROM wx_StoreCategory
WHERE PublishmentSystemID = @PublishmentSystemID AND ParentID = @ParentID AND ID <> @ID AND Taxis < @Taxis
ORDER BY Taxis DESC";

            var parms = new IDataParameter[]
			{
				GetParameter(PARM_PUBLISHIMENTSYSTEMID, EDataType.Integer, publishmentSystemID),
                GetParameter(PARM_PARENT_ID, EDataType.Integer, categoryInfo.ParentID),
				GetParameter(PARM_ID, EDataType.Integer, categoryInfo.ID),
				GetParameter(PARM_TAXIS, EDataType.Integer, categoryInfo.Taxis),
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    lowerID = Convert.ToInt32(rdr[0]);
                    lowerChildCount = Convert.ToInt32(rdr[1]);
                    lowerParentsPath = rdr.GetValue(2).ToString();
                }
                else
                {
                    return;
                }
                rdr.Close();
            }

            var lowerNodePath = String.Concat(lowerParentsPath, ",", lowerID);
            var selectedNodePath = String.Concat(categoryInfo.ParentsPath, ",", categoryInfo.ID);

            SetTaxisSubtract(publishmentSystemID, selectedID, selectedNodePath, lowerChildCount + 1);
            SetTaxisAdd(publishmentSystemID, lowerID, lowerNodePath, categoryInfo.ChildCount + 1);

            UpdateIsLastNode(publishmentSystemID, categoryInfo.ParentID);
        }

        private void TaxisAdd(int publishmentSystemID, int selectedID)
        {
            var categoryInfo = GetCategoryInfo(selectedID);
            if (categoryInfo == null) return;
            //Get Higher Taxis and ID
            var higherID = 0;
            var higherChildCount = 0;
            var higherParentsPath = "";
            var sqlString = @"SELECT TOP 1 ID, ChildCount, ParentsPath
FROM wx_StoreCategory
WHERE PublishmentSystemID = @PublishmentSystemID AND ParentID = @ParentID AND ID <> @ID AND Taxis > @Taxis
ORDER BY Taxis";

            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHIMENTSYSTEMID, EDataType.Integer, publishmentSystemID),
				GetParameter(PARM_PARENT_ID, EDataType.Integer, categoryInfo.ParentID),
				GetParameter(PARM_ID, EDataType.Integer, categoryInfo.ID),
				GetParameter(PARM_TAXIS, EDataType.Integer, categoryInfo.Taxis)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    higherID = Convert.ToInt32(rdr[0]);
                    higherChildCount = Convert.ToInt32(rdr[1]);
                    higherParentsPath = rdr.GetValue(2).ToString();
                }
                else
                {
                    return;
                }
                rdr.Close();
            }

            var higherNodePath = String.Concat(higherParentsPath, ",", higherID);
            var selectedNodePath = String.Concat(categoryInfo.ParentsPath, ",", categoryInfo.ID);

            SetTaxisAdd(publishmentSystemID, selectedID, selectedNodePath, higherChildCount + 1);
            SetTaxisSubtract(publishmentSystemID, higherID, higherNodePath, categoryInfo.ChildCount + 1);

            UpdateIsLastNode(publishmentSystemID, categoryInfo.ParentID);
        }

        private void SetTaxisAdd(int publishmentSystemID, int id, string parentsPath, int addNum)
        {
            string sqlString =
                $"UPDATE wx_StoreCategory SET Taxis = Taxis + {addNum} WHERE PublishmentSystemID = {publishmentSystemID} AND (ID = {id} OR ParentsPath = '{parentsPath}' OR ParentsPath like '{parentsPath},%')";

            ExecuteNonQuery(sqlString);
        }

        private void SetTaxisSubtract(int publishmentSystemID, int id, string parentsPath, int subtractNum)
        {
            string sqlString =
                $"UPDATE wx_StoreCategory SET Taxis = Taxis - {subtractNum} WHERE PublishmentSystemID = {publishmentSystemID} AND (ID = {id} OR ParentsPath = '{parentsPath}' OR ParentsPath like '{parentsPath},%')";

            ExecuteNonQuery(sqlString);
        }

        private void UpdateIsLastNode(int publishmentSystemID, int parentID)
        {
            if (parentID > 0)
            {
                var sqlString = "UPDATE wx_StoreCategory SET IsLastNode = @IsLastNode WHERE PublishmentSystemID = @PublishmentSystemID AND ParentID = @ParentID";

                var parms = new IDataParameter[]
			    {
				    GetParameter(PARM_IS_LAST_NODE, EDataType.VarChar, 18, false.ToString()),
                    GetParameter(PARM_PUBLISHIMENTSYSTEMID, EDataType.Integer, publishmentSystemID),
				    GetParameter(PARM_PARENT_ID, EDataType.Integer, parentID)
			    };

                ExecuteNonQuery(sqlString, parms);

                sqlString =
                    $"UPDATE wx_StoreCategory SET IsLastNode = '{true.ToString()}' WHERE (ID IN (SELECT TOP 1 ID FROM wx_StoreCategory WHERE ParentID = {parentID} ORDER BY Taxis DESC))";

                ExecuteNonQuery(sqlString);
            }
        }

        private int GetMaxTaxisByParentPath(int publishmentSystemID, string parentPath)
        {
            string sqlString =
                $"SELECT MAX(Taxis) AS MaxTaxis FROM wx_StoreCategory WHERE PublishmentSystemID = {publishmentSystemID} AND (ParentsPath = '{parentPath}' OR ParentsPath LIKE '{parentPath},%')";
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

        private int GetParentID(int id)
        {
            var parentID = 0;

            var nodeParms = new IDataParameter[]
			{
				GetParameter(PARM_ID, EDataType.Integer, id)
			};

            using (var rdr = ExecuteReader(SQL_SELECT_PARENT_ID, nodeParms))
            {
                if (rdr.Read())
                {
                    parentID = Convert.ToInt32(rdr[0]);
                }
                rdr.Close();
            }
            return parentID;
        }

        private int GetIDByParentIDAndOrder(int publishmentSystemID, int parentID, int order)
        {
            var id = parentID;

            string sqlString =
                $"SELECT ID FROM wx_StoreCategory WHERE PublishmentSystemID = {publishmentSystemID} AND ParentID = {parentID} ORDER BY Taxis";

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

        public int Insert(int publishmentSystemID, StoreCategoryInfo categoryInfo)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var parentCategoryInfo = GetCategoryInfo(categoryInfo.ParentID);

                        InsertWithTrans(publishmentSystemID, parentCategoryInfo, categoryInfo, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return categoryInfo.ID;
        }

        public void Update(int publishmentSystemID, StoreCategoryInfo categoryInfo)
        {
            var updateParms = new IDataParameter[]
			{
				GetParameter(PARM_NAME, EDataType.NVarChar, 255, categoryInfo.CategoryName),
				GetParameter(PARM_PARENTS_PATH, EDataType.NVarChar, 255, categoryInfo.ParentsPath),
				GetParameter(PARM_PARENTS_COUNT, EDataType.Integer, categoryInfo.ParentsCount),
				GetParameter(PARM_CHILDREN_COUNT, EDataType.Integer, categoryInfo.ChildCount),
				GetParameter(PARM_IS_LAST_NODE, EDataType.VarChar, 18, categoryInfo.IsLastNode.ToString()),

				GetParameter(PARM_ID, EDataType.Integer, categoryInfo.ID)
			};

            ExecuteNonQuery(SQL_UPDATE, updateParms);
        }

        public void UpdateTaxis(int publishmentSystemID, int selectedID, bool isSubtract)
        {
            if (isSubtract)
            {
                TaxisSubtract(publishmentSystemID, selectedID);
            }
            else
            {
                TaxisAdd(publishmentSystemID, selectedID);
            }
        }

        public void Delete(int publishmentSystemID, int id)
        {
            var categoryInfo = GetCategoryInfo(id);
            if (categoryInfo != null)
            {
                var idList = new List<int>();
                if (categoryInfo.ChildCount > 0)
                {
                    idList = GetCategoryIDListForDescendant(publishmentSystemID, id);
                }
                idList.Add(id);

                string sqlString =
                    $"DELETE FROM wx_StoreCategory WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

                var deletedNum = 0;

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
                UpdateIsLastNode(publishmentSystemID, categoryInfo.ParentID);
                UpdateSubtractChildCount(publishmentSystemID, categoryInfo.ParentsPath, deletedNum);
            }
        }

        public StoreCategoryInfo GetCategoryInfo(int categoryID)
        {
            StoreCategoryInfo categoryInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(PARM_ID, EDataType.Integer, categoryID)
			};

            using (var rdr = ExecuteReader(SQL_SELECT, parms))
            {
                if (rdr.Read())
                {
                    categoryInfo = new StoreCategoryInfo(rdr);
                }
                rdr.Close();
            }
            return categoryInfo;
        }

        private ArrayList GetCategoryInfoArrayList(int publishmentSystemID)
        {
            var arraylist = new ArrayList();

            var parms = new IDataParameter[]
			{
				GetParameter(PARM_PUBLISHIMENTSYSTEMID, EDataType.Integer, publishmentSystemID)
			};

            using (var rdr = ExecuteReader(SQL_SELECT_ALL, parms))
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

        public int GetNodeCount(int publishmentSystemID, int id)
        {
            var nodeCount = 0;

            var nodeParms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHIMENTSYSTEMID, EDataType.Integer, publishmentSystemID),
				GetParameter(PARM_PARENT_ID, EDataType.Integer, id)
			};

            using (var rdr = ExecuteReader(SQL_SELECT_COUNT, nodeParms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    nodeCount = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return nodeCount;
        }

        public List<int> GetCategoryIDListByParentID(int publishmentSystemID, int parentID)
        {
            var list = new List<int>();

            string sqlString =
                $@"SELECT ID FROM wx_StoreCategory WHERE PublishmentSystemID = {publishmentSystemID} AND ParentID = {parentID} ORDER BY Taxis";

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

        public List<int> GetCategoryIDListForDescendant(int publishmentSystemID, int categoryID)
        {
            var list = new List<int>();

            string sqlString = $@"SELECT ID
FROM wx_StoreCategory
WHERE PublishmentSystemID = {publishmentSystemID} AND (
    (ParentsPath LIKE '{categoryID},%') OR
    (ParentsPath LIKE '%,{categoryID},%') OR
    (ParentsPath LIKE '%,{categoryID}') OR
    (ParentID = {categoryID})
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

        public List<int> GetAllCategoryIDList(int publishmentSystemID)
        {
            var list = new List<int>();

            string sqlString =
                $@"SELECT ID FROM wx_StoreCategory WHERE PublishmentSystemID = {publishmentSystemID} ORDER BY Taxis";

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

        public List<int> GetCategoryIDListForLastNode(int publishmentSystemID, int categoryID)
        {
            var list = new List<int>();

            string sqlString = $@"SELECT ID
FROM wx_StoreCategory
WHERE PublishmentSystemID = {publishmentSystemID} AND ChildCount = 0 AND (
    (ParentsPath LIKE '{categoryID},%') OR
    (ParentsPath LIKE '%,{categoryID},%') OR
    (ParentsPath LIKE '%,{categoryID}') OR
    (ParentID = {categoryID}) OR
    (ID = {categoryID})
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

        public List<int> GetCategoryIDListByCategoryIDCollection(int publishmentSystemID, string idCollection)
        {
            var list = new List<int>();

            if (!string.IsNullOrEmpty(idCollection))
            {
                string sqlString =
                    $@"SELECT ID FROM wx_StoreCategory WHERE PublishmentSystemID = {publishmentSystemID} AND ID IN ({idCollection})";

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

        public List<int> GetCategoryIDListByFirstCategoryIDArrayList(int publishmentSystemID, ArrayList firstIDArrayList)
        {
            var list = new List<int>();

            if (firstIDArrayList.Count > 0)
            {
                var builder = new StringBuilder();
                foreach (int id in firstIDArrayList)
                {
                    builder.AppendFormat("ID = {0} OR ParentID = {0} OR ParentsPath LIKE '{0},%' OR ", id);
                }
                builder.Length -= 3;

                string sqlString =
                    $"SELECT ID FROM wx_StoreCategory WHERE PublishmentSystemID = {publishmentSystemID} AND ({builder.ToString()}) ORDER BY Taxis";

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

        public void UpdateStoreItemCount(int publishmentSystemID)
        {
            var categoryIDList = GetAllCategoryIDList(publishmentSystemID);

            foreach (var categoryID in categoryIDList)
            {
                var count = DataProviderWX.StoreItemDAO.GetCount(publishmentSystemID, categoryID);
                string sqlString =
                    $@"UPDATE wx_StoreCategory SET StoreCount = {count} WHERE PublishmentSystemID = {publishmentSystemID} AND ID = {categoryID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public List<StoreCategoryInfo> GetStoreCategoryInfoList(int publishmentSystemID)
        {
            var list = new List<StoreCategoryInfo>();

            var builder = new StringBuilder(
                $"WHERE {StoreCategoryAttribute.PublishmentSystemID} = {publishmentSystemID}");
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY Taxis");

            using (var rdr = ExecuteReader(SQL_SELECT))
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
