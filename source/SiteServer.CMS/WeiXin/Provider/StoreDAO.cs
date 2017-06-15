using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class StoreDao : DataProviderBase
    {
        private const string TableName = "wx_Store";

        public int Insert(StoreInfo storeInfo)
        {
            var voteId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(storeInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        voteId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return voteId;
        }


        public void Update(StoreInfo storeInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(storeInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void AddPvCount(int storeId)
        {
            if (storeId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {StoreAttribute.PvCount} = {StoreAttribute.PvCount} + 1 WHERE ID = {storeId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, int storeId)
        {
            if (storeId > 0)
            {
                var storeIdList = new List<int>();
                storeIdList.Add(storeId);
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(storeIdList));                 
                DataProviderWx.StoreItemDao.DeleteAll(publishmentSystemId, storeId);

                string sqlString = $"DELETE FROM {TableName} WHERE ID = {storeId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> storeIdList)
        {
            if (storeIdList != null && storeIdList.Count > 0)
            {
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(storeIdList));

                foreach (var storeId in storeIdList)
                {                     
                    DataProviderWx.StoreItemDao.DeleteAll(publishmentSystemId, storeId);
                }

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(storeIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIdList(List<int> storeIdList)
        {
            var keywordIdList = new List<int>();

            string sqlString =
                $"SELECT {StoreAttribute.KeywordId} FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(storeIdList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    keywordIdList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return keywordIdList;
        }

        public StoreInfo GetStoreInfo(int storeId)
        {
            StoreInfo storeInfo = null;

            string sqlWhere = $"WHERE ID = {storeId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    storeInfo = new StoreInfo(rdr);
                }
                rdr.Close();
            }

            return storeInfo;
        }

        public List<StoreInfo> GetStoreInfoListByKeywordId(int publishmentSystemId, int keywordId)
        {
            var storeInfoList = new List<StoreInfo>();

            string sqlWhere =
                $"WHERE {StoreAttribute.PublishmentSystemId} = {publishmentSystemId} AND {StoreAttribute.IsDisabled} <> '{true}'";
            if (keywordId > 0)
            {
                sqlWhere += $" AND {StoreAttribute.KeywordId} = {keywordId}";
            }

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var storeInfo = new StoreInfo(rdr);
                    storeInfoList.Add(storeInfo);
                }
                rdr.Close();
            }

            return storeInfoList;
        }

        public string GetTitle(int storeId)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {storeId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, StoreAttribute.Title, sqlWhere, null);

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

        public string GetSelectString(int publishmentSystemId)
        {
            string whereString = $"WHERE {StoreAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public int GetFirstIdByKeywordId(int publishmentSystemId, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TableName} WHERE {StoreAttribute.PublishmentSystemId} = {publishmentSystemId} AND {StoreAttribute.IsDisabled} <> '{true}' AND {StoreAttribute.KeywordId} = {keywordId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<StoreInfo> GetStoreInfoList(int publishmentSystemId)
        {
            var storeInfoList = new List<StoreInfo>();

            string sqlWhere = $"WHERE {StoreAttribute.PublishmentSystemId} = {publishmentSystemId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var storeInfo = new StoreInfo(rdr);
                    storeInfoList.Add(storeInfo);
                }
                rdr.Close();
            }

            return storeInfoList;
        }
        

    }
}
