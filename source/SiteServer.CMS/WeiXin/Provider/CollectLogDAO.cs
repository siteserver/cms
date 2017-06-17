using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CollectLogDao : DataProviderBase
    {
        private const string TableName = "wx_CollectLog";

        public void Insert(CollectLogInfo logInfo)
        {
            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(logInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);



            ExecuteNonQuery(sqlInsert, parms);
        }

        public void DeleteAll(int collectId)
        {
            if (collectId > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE {CollectLogAttribute.CollectId} = {collectId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> logIdList)
        {
            if (logIdList != null && logIdList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(logIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public int GetCount(int collectId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {CollectLogAttribute.CollectId} = {collectId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public bool IsCollectd(int collectId, string cookieSn, string wxOpenId)
        {
            var isCollectd = false;

            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {CollectLogAttribute.CollectId} = {collectId} AND {CollectLogAttribute.CookieSn} = '{cookieSn}'";

            isCollectd = BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString) > 0;

            return isCollectd;
        }

        public List<int> GetVotedItemIdList(int collectId, string cookieSn)
        {
            string sqlString =
                $"SELECT ItemID FROM {TableName} WHERE {CollectLogAttribute.CollectId} = {collectId} AND {CollectLogAttribute.CookieSn} = '{cookieSn}'";
            return BaiRongDataProvider.DatabaseDao.GetIntList(sqlString);
        }

        public string GetSelectString(int collectId)
        {
            string whereString = $"WHERE {CollectLogAttribute.CollectId} = {collectId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<CollectLogInfo> GetCollectLogInfoList(int publishmentSystemId, int collectId, int collectItemId)
        {

            var list = new List<CollectLogInfo>();

            string sqlWhere =
                $"WHERE {CollectLogAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CollectLogAttribute.CollectId} = {collectId} AND {CollectLogAttribute.ItemId} = {collectItemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var logInfo = new CollectLogInfo(rdr);
                    list.Add(logInfo);
                }
                rdr.Close();
            }

            return list;
        }
    }
}
