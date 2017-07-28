using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CollectLogDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_CollectLog";

        public void Insert(CollectLogInfo logInfo)
        {
            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(logInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);



            ExecuteNonQuery(SQL_INSERT, parms);
        }

        public void DeleteAll(int collectID)
        {
            if (collectID > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE {CollectLogAttribute.CollectID} = {collectID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> logIDList)
        {
            if (logIDList != null && logIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(logIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public int GetCount(int collectID)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {CollectLogAttribute.CollectID} = {collectID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public bool IsCollectd(int collectID, string cookieSN, string wxOpenID)
        {
            var isCollectd = false;

            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {CollectLogAttribute.CollectID} = {collectID} AND {CollectLogAttribute.CookieSN} = '{cookieSN}'";

            isCollectd = BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString) > 0;

            return isCollectd;
        }

        public List<int> GetVotedItemIDList(int collectID, string cookieSN)
        {
            string sqlString =
                $"SELECT ItemID FROM {TABLE_NAME} WHERE {CollectLogAttribute.CollectID} = {collectID} AND {CollectLogAttribute.CookieSN} = '{cookieSN}'";
            return BaiRongDataProvider.DatabaseDao.GetIntList(sqlString);
        }

        public string GetSelectString(int collectID)
        {
            string whereString = $"WHERE {CollectLogAttribute.CollectID} = {collectID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<CollectLogInfo> GetCollectLogInfoList(int publishmentSystemID, int collectID, int collectItemID)
        {

            var list = new List<CollectLogInfo>();

            string SQL_WHERE =
                $"WHERE {CollectLogAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CollectLogAttribute.CollectID} = {collectID} AND {CollectLogAttribute.ItemID} = {collectItemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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
