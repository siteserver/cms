using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class StoreItemDao : DataProviderBase
    {
        private const string TableName = "wx_StoreItem";

        public int Insert(StoreItemInfo storeItemInfo)
        {
            var storeItemId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(storeItemInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        storeItemId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return storeItemId;
        }

        public int Insert(int publishmentSystemId, StoreItemInfo storeItemInfo)
        {
            var storeItemId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(storeItemInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        storeItemId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            DataProviderWx.StoreCategoryDao.UpdateStoreItemCount(publishmentSystemId);

            return storeItemId;
        }

        public void Update(int publishmentSystemId, StoreItemInfo storeItemInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(storeItemInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);
            ExecuteNonQuery(sqlUpdate, parms);

            DataProviderWx.StoreCategoryDao.UpdateStoreItemCount(publishmentSystemId);
        }

        public void Delete(int publishmentSystemId, int storeItemId)
        {
            if (storeItemId > 0)
            {
                var categoryIdList = GetCategoryIdList(TranslateUtils.ToIntList(storeItemId));

                string sqlString = $"DELETE FROM {TableName} WHERE ID = {storeItemId}";
                ExecuteNonQuery(sqlString);

                DataProviderWx.StoreCategoryDao.UpdateStoreItemCount(publishmentSystemId);
            }
        }

        public void Delete(int publishmentSystemId, List<int> storeItemIdList)
        {
            if (storeItemIdList != null && storeItemIdList.Count > 0)
            {
                var categoryIdList = GetCategoryIdList(storeItemIdList);

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(storeItemIdList)})";
                ExecuteNonQuery(sqlString);

                DataProviderWx.StoreCategoryDao.UpdateStoreItemCount(publishmentSystemId);
            }
        }

        private List<int> GetCategoryIdList(List<int> storeItemIdList)
        {
            var categoryIdList = new List<int>();

            string sqlString =
                $"SELECT {StoreItemAttribute.CategoryId} FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(storeItemIdList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read() && !rdr.IsDBNull(0))
                {
                    categoryIdList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return categoryIdList;
        }

        public StoreItemInfo GetStoreItemInfo(int storeItemId)
        {
            StoreItemInfo storeItemInfo = null;

            string sqlWhere = $"WHERE ID = {storeItemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    storeItemInfo = new StoreItemInfo(rdr);
                }
                rdr.Close();
            }

            return storeItemInfo;
        }

        public StoreItemInfo GetStoreItemInfoByParentId(int publishmentSystemId, int parentId)
        {
            StoreItemInfo storeItemInfo = null;

            string sqlWhere = $"WHERE publishmentSystemID = {publishmentSystemId} AND ParentID = {parentId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    storeItemInfo = new StoreItemInfo(rdr);
                }
                rdr.Close();
            }

            return storeItemInfo;
        }

        public string GetSelectString(int storeId)
        {
            string whereString = $"WHERE {StoreItemAttribute.StoreId} = {storeId} ";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<StoreItemInfo> GetStoreItemInfoListByCategoryId(int publishmentSystemId, int categoryId)
        {
            var list = new List<StoreItemInfo>();
            StringBuilder builder;
            if (categoryId == 0)
            {
                builder = new StringBuilder($"WHERE {StoreItemAttribute.PublishmentSystemId} = {publishmentSystemId} ");
            }
            else
            {
                builder = new StringBuilder(
                    $"WHERE {StoreItemAttribute.PublishmentSystemId} = {publishmentSystemId} AND {StoreItemAttribute.CategoryId} = {categoryId} ");
            }
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var storeItemInfo = new StoreItemInfo(rdr);
                    list.Add(storeItemInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public List<StoreItemInfo> GetAllStoreItemInfoList(int publishmentSystemId)
        {
            var list = new List<StoreItemInfo>();

            var builder = new StringBuilder(
                $"WHERE {StoreItemAttribute.PublishmentSystemId} = {publishmentSystemId}");
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var storeItemInfo = new StoreItemInfo(rdr);
                    list.Add(storeItemInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public void DeleteAll(int publishmentSystemId, int storeId)
        {
            if (storeId > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE {StoreItemAttribute.PublishmentSystemId} = {publishmentSystemId} AND {StoreItemAttribute.StoreId} = {storeId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public List<StoreItemInfo> GetAllStoreItemInfoListByLocation(int publishmentSystemId, string locationX)
        {
            var list = new List<StoreItemInfo>();

            var builder = new StringBuilder(
                $"WHERE {StoreItemAttribute.PublishmentSystemId} = {publishmentSystemId} AND {StoreItemAttribute.Latitude} BETWEEN '{Convert.ToDouble(locationX) - 0.5}' AND '{Convert.ToDouble(locationX) + 0.5}'");
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var storeItemInfo = new StoreItemInfo(rdr);
                    list.Add(storeItemInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public int GetCount(int publishmentSystemId, int categoryId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {StoreItemAttribute.PublishmentSystemId} = {publishmentSystemId} AND {StoreItemAttribute.CategoryId} = {categoryId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetAllCount(int publishmentSystemId, int categoryId)
        {
            var categoryIdList = DataProviderWx.StoreCategoryDao.GetCategoryIdListForLastNode(publishmentSystemId, categoryId);

            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {StoreItemAttribute.PublishmentSystemId} = {publishmentSystemId} AND {StoreItemAttribute.CategoryId} IN ({TranslateUtils.ToSqlInStringWithoutQuote(categoryIdList)})";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<StoreItemInfo> GetStoreItemInfoList(int publishmentSystemId, int storeId)
        {
            var list = new List<StoreItemInfo>();
            StringBuilder builder;
            builder = new StringBuilder(
                $"WHERE {StoreItemAttribute.PublishmentSystemId} = {publishmentSystemId} AND {StoreItemAttribute.StoreId} = {storeId} ");

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var storeItemInfo = new StoreItemInfo(rdr);
                    list.Add(storeItemInfo);
                }
                rdr.Close();
            }

            return list;
        }
    }
}
