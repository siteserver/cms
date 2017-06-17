using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CollectDao : DataProviderBase
    {
        private const string TableName = "wx_Collect";

        public int Insert(CollectInfo collectInfo)
        {
            var collectId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(collectInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        collectId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return collectId;
        }

        public void Update(CollectInfo collectInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(collectInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void AddUserCount(int collectId)
        {
            if (collectId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {CollectAttribute.UserCount} = {CollectAttribute.UserCount} + 1 WHERE ID = {collectId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void AddPvCount(int collectId)
        {
            if (collectId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {CollectAttribute.PvCount} = {CollectAttribute.PvCount} + 1 WHERE ID = {collectId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, int collectId)
        {
            if (collectId > 0)
            {
                var collectIdList = new List<int>();
                collectIdList.Add(collectId);
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(collectIdList));

                DataProviderWx.CollectLogDao.DeleteAll(collectId);
                DataProviderWx.CollectItemDao.DeleteAll(publishmentSystemId, collectId);

                string sqlString = $"DELETE FROM {TableName} WHERE ID = {collectId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> collectIdList)
        {
            if (collectIdList != null && collectIdList.Count > 0)
            {
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(collectIdList));

                foreach (var collectId in collectIdList)
                {
                    DataProviderWx.CollectLogDao.DeleteAll(collectId);
                    DataProviderWx.CollectItemDao.DeleteAll(publishmentSystemId, collectId);
                }

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(collectIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIdList(List<int> collectIdList)
        {
            var keywordIdList = new List<int>();

            string sqlString =
                $"SELECT {CollectAttribute.KeywordId} FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(collectIdList)})";

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

        public CollectInfo GetCollectInfo(int collectId)
        {
            CollectInfo collectInfo = null;

            string sqlWhere = $"WHERE ID = {collectId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    collectInfo = new CollectInfo(rdr);
                }
                rdr.Close();
            }

            return collectInfo;
        }

        public List<CollectInfo> GetCollectInfoListByKeywordId(int publishmentSystemId, int keywordId)
        {
            var collectInfoList = new List<CollectInfo>();

            string sqlWhere =
                $"WHERE {CollectAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CollectAttribute.IsDisabled} <> '{true}'";
            if (keywordId > 0)
            {
                sqlWhere += $" AND {CollectAttribute.KeywordId} = {keywordId}";
            }

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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

        public int GetFirstIdByKeywordId(int publishmentSystemId, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TableName} WHERE {CollectAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CollectAttribute.IsDisabled} <> '{true}' AND {CollectAttribute.KeywordId} = {keywordId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int collectId)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {collectId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, CollectAttribute.Title, sqlWhere, null);

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
            string whereString = $"WHERE {CollectAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<CollectInfo> GetCollectInfoList(int publishmentSystemId)
        {
            var collectInfoList = new List<CollectInfo>();

            string sqlWhere = $"WHERE {CollectAttribute.PublishmentSystemId} = {publishmentSystemId}";            

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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
