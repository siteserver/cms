using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class StoreDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Store";

        public int Insert(StoreInfo storeInfo)
        {
            var voteID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(storeInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        voteID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return voteID;
        }


        public void Update(StoreInfo storeInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(storeInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void AddPVCount(int storeID)
        {
            if (storeID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {StoreAttribute.PVCount} = {StoreAttribute.PVCount} + 1 WHERE ID = {storeID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, int storeID)
        {
            if (storeID > 0)
            {
                var StoreIDList = new List<int>();
                StoreIDList.Add(storeID);
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(StoreIDList));                 
                DataProviderWX.StoreItemDAO.DeleteAll(publishmentSystemID, storeID);

                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {storeID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> storeIDList)
        {
            if (storeIDList != null && storeIDList.Count > 0)
            {
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(storeIDList));

                foreach (var storeID in storeIDList)
                {                     
                    DataProviderWX.StoreItemDAO.DeleteAll(publishmentSystemID, storeID);
                }

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(storeIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIDList(List<int> storeIDList)
        {
            var keywordIDList = new List<int>();

            string sqlString =
                $"SELECT {StoreAttribute.KeywordID} FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(storeIDList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    keywordIDList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return keywordIDList;
        }

        public StoreInfo GetStoreInfo(int storeID)
        {
            StoreInfo storeInfo = null;

            string SQL_WHERE = $"WHERE ID = {storeID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    storeInfo = new StoreInfo(rdr);
                }
                rdr.Close();
            }

            return storeInfo;
        }

        public List<StoreInfo> GetStoreInfoListByKeywordID(int publishmentSystemID, int keywordID)
        {
            var storeInfoList = new List<StoreInfo>();

            string SQL_WHERE =
                $"WHERE {StoreAttribute.PublishmentSystemID} = {publishmentSystemID} AND {StoreAttribute.IsDisabled} <> '{true}'";
            if (keywordID > 0)
            {
                SQL_WHERE += $" AND {StoreAttribute.KeywordID} = {keywordID}";
            }

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var StoreInfo = new StoreInfo(rdr);
                    storeInfoList.Add(StoreInfo);
                }
                rdr.Close();
            }

            return storeInfoList;
        }

        public string GetTitle(int storeID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {storeID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, StoreAttribute.Title, SQL_WHERE, null);

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

        public string GetSelectString(int publishmentSystemID)
        {
            string whereString = $"WHERE {StoreAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public int GetFirstIDByKeywordID(int publishmentSystemID, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TABLE_NAME} WHERE {StoreAttribute.PublishmentSystemID} = {publishmentSystemID} AND {StoreAttribute.IsDisabled} <> '{true}' AND {StoreAttribute.KeywordID} = {keywordID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<StoreInfo> GetStoreInfoList(int publishmentSystemID)
        {
            var storeInfoList = new List<StoreInfo>();

            string SQL_WHERE = $"WHERE {StoreAttribute.PublishmentSystemID} = {publishmentSystemID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var StoreInfo = new StoreInfo(rdr);
                    storeInfoList.Add(StoreInfo);
                }
                rdr.Close();
            }

            return storeInfoList;
        }
        

    }
}
