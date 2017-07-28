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
    public class StoreItemDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_StoreItem";

        public int Insert(StoreItemInfo storeItemInfo)
        {
            var storeItemID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(storeItemInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        storeItemID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return storeItemID;
        }

        public int Insert(int publishmentSystemID, StoreItemInfo storeItemInfo)
        {
            var storeItemID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(storeItemInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        storeItemID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            DataProviderWX.StoreCategoryDAO.UpdateStoreItemCount(publishmentSystemID);

            return storeItemID;
        }

        public void Update(int publishmentSystemID, StoreItemInfo storeItemInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(storeItemInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);
            ExecuteNonQuery(SQL_UPDATE, parms);

            DataProviderWX.StoreCategoryDAO.UpdateStoreItemCount(publishmentSystemID);
        }

        public void Delete(int publishmentSystemID, int storeItemID)
        {
            if (storeItemID > 0)
            {
                var categoryIDList = GetCategoryIDList(TranslateUtils.ToIntList(storeItemID));

                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {storeItemID}";
                ExecuteNonQuery(sqlString);

                DataProviderWX.StoreCategoryDAO.UpdateStoreItemCount(publishmentSystemID);
            }
        }

        public void Delete(int publishmentSystemID, List<int> storeItemIDList)
        {
            if (storeItemIDList != null && storeItemIDList.Count > 0)
            {
                var categoryIDList = GetCategoryIDList(storeItemIDList);

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(storeItemIDList)})";
                ExecuteNonQuery(sqlString);

                DataProviderWX.StoreCategoryDAO.UpdateStoreItemCount(publishmentSystemID);
            }
        }

        private List<int> GetCategoryIDList(List<int> storeItemIDList)
        {
            var categoryIDList = new List<int>();

            string sqlString =
                $"SELECT {StoreItemAttribute.CategoryID} FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(storeItemIDList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read() && !rdr.IsDBNull(0))
                {
                    categoryIDList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return categoryIDList;
        }

        public StoreItemInfo GetStoreItemInfo(int storeItemID)
        {
            StoreItemInfo storeItemInfo = null;

            string SQL_WHERE = $"WHERE ID = {storeItemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    storeItemInfo = new StoreItemInfo(rdr);
                }
                rdr.Close();
            }

            return storeItemInfo;
        }

        public StoreItemInfo GetStoreItemInfoByParentID(int publishmentSystemID, int parentID)
        {
            StoreItemInfo storeItemInfo = null;

            string SQL_WHERE = $"WHERE publishmentSystemID = {publishmentSystemID} AND ParentID = {parentID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    storeItemInfo = new StoreItemInfo(rdr);
                }
                rdr.Close();
            }

            return storeItemInfo;
        }

        public string GetSelectString(int storeID)
        {
            string whereString = $"WHERE {StoreItemAttribute.StoreID} = {storeID} ";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<StoreItemInfo> GetStoreItemInfoListByCategoryID(int publishmentSystemID, int categoryID)
        {
            var list = new List<StoreItemInfo>();
            StringBuilder builder;
            if (categoryID == 0)
            {
                builder = new StringBuilder($"WHERE {StoreItemAttribute.PublishmentSystemID} = {publishmentSystemID} ");
            }
            else
            {
                builder = new StringBuilder(
                    $"WHERE {StoreItemAttribute.PublishmentSystemID} = {publishmentSystemID} AND {StoreItemAttribute.CategoryID} = {categoryID} ");
            }
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public List<StoreItemInfo> GetAllStoreItemInfoList(int publishmentSystemID)
        {
            var list = new List<StoreItemInfo>();

            var builder = new StringBuilder(
                $"WHERE {StoreItemAttribute.PublishmentSystemID} = {publishmentSystemID}");
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public void DeleteAll(int publishmentSystemID, int storeID)
        {
            if (storeID > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE {StoreItemAttribute.PublishmentSystemID} = {publishmentSystemID} AND {StoreItemAttribute.StoreID} = {storeID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public List<StoreItemInfo> GetAllStoreItemInfoListByLocation(int publishmentSystemID, string location_X)
        {
            var list = new List<StoreItemInfo>();

            var builder = new StringBuilder(
                $"WHERE {StoreItemAttribute.PublishmentSystemID} = {publishmentSystemID} AND {StoreItemAttribute.Latitude} BETWEEN '{Convert.ToDouble(location_X) - 0.5}' AND '{Convert.ToDouble(location_X) + 0.5}'");
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public int GetCount(int publishmentSystemID, int categoryID)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {StoreItemAttribute.PublishmentSystemID} = {publishmentSystemID} AND {StoreItemAttribute.CategoryID} = {categoryID}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetAllCount(int publishmentSystemID, int categoryID)
        {
            var categoryIDList = DataProviderWX.StoreCategoryDAO.GetCategoryIDListForLastNode(publishmentSystemID, categoryID);

            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {StoreItemAttribute.PublishmentSystemID} = {publishmentSystemID} AND {StoreItemAttribute.CategoryID} IN ({TranslateUtils.ToSqlInStringWithoutQuote(categoryIDList)})";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<StoreItemInfo> GetStoreItemInfoList(int publishmentSystemID, int storeID)
        {
            var list = new List<StoreItemInfo>();
            StringBuilder builder;
            builder = new StringBuilder(
                $"WHERE {StoreItemAttribute.PublishmentSystemID} = {publishmentSystemID} AND {StoreItemAttribute.StoreID} = {storeID} ");

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(SQL_SELECT))
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
