using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CollectDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Collect";

        public int Insert(CollectInfo collectInfo)
        {
            var collectID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(collectInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        collectID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return collectID;
        }

        public void Update(CollectInfo collectInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(collectInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void AddUserCount(int collectID)
        {
            if (collectID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {CollectAttribute.UserCount} = {CollectAttribute.UserCount} + 1 WHERE ID = {collectID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void AddPVCount(int collectID)
        {
            if (collectID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {CollectAttribute.PVCount} = {CollectAttribute.PVCount} + 1 WHERE ID = {collectID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, int collectID)
        {
            if (collectID > 0)
            {
                var collectIDList = new List<int>();
                collectIDList.Add(collectID);
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(collectIDList));

                DataProviderWX.CollectLogDAO.DeleteAll(collectID);
                DataProviderWX.CollectItemDAO.DeleteAll(publishmentSystemID, collectID);

                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {collectID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> collectIDList)
        {
            if (collectIDList != null && collectIDList.Count > 0)
            {
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(collectIDList));

                foreach (var collectID in collectIDList)
                {
                    DataProviderWX.CollectLogDAO.DeleteAll(collectID);
                    DataProviderWX.CollectItemDAO.DeleteAll(publishmentSystemID, collectID);
                }

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(collectIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIDList(List<int> collectIDList)
        {
            var keywordIDList = new List<int>();

            string sqlString =
                $"SELECT {CollectAttribute.KeywordID} FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(collectIDList)})";

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

        public CollectInfo GetCollectInfo(int collectID)
        {
            CollectInfo collectInfo = null;

            string SQL_WHERE = $"WHERE ID = {collectID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    collectInfo = new CollectInfo(rdr);
                }
                rdr.Close();
            }

            return collectInfo;
        }

        public List<CollectInfo> GetCollectInfoListByKeywordID(int publishmentSystemID, int keywordID)
        {
            var collectInfoList = new List<CollectInfo>();

            string SQL_WHERE =
                $"WHERE {CollectAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CollectAttribute.IsDisabled} <> '{true}'";
            if (keywordID > 0)
            {
                SQL_WHERE += $" AND {CollectAttribute.KeywordID} = {keywordID}";
            }

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var collectInfo = new CollectInfo(rdr);
                    collectInfoList.Add(collectInfo);
                }
                rdr.Close();
            }

            return collectInfoList;
        }

        public int GetFirstIDByKeywordID(int publishmentSystemID, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TABLE_NAME} WHERE {CollectAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CollectAttribute.IsDisabled} <> '{true}' AND {CollectAttribute.KeywordID} = {keywordID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int collectID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {collectID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, CollectAttribute.Title, SQL_WHERE, null);

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
            string whereString = $"WHERE {CollectAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<CollectInfo> GetCollectInfoList(int publishmentSystemID)
        {
            var collectInfoList = new List<CollectInfo>();

            string SQL_WHERE = $"WHERE {CollectAttribute.PublishmentSystemID} = {publishmentSystemID}";            

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var collectInfo = new CollectInfo(rdr);
                    collectInfoList.Add(collectInfo);
                }
                rdr.Close();
            }

            return collectInfoList;
        }
    }
}
