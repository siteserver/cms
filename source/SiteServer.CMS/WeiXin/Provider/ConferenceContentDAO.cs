using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class ConferenceContentDao : DataProviderBase
    {
        private const string TableName = "wx_ConferenceContent";


        public void Insert(ConferenceContentInfo contentInfo)
        {
            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(contentInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);


            ExecuteNonQuery(sqlInsert, parms);
        }

        public void DeleteAll(int conferenceId)
        {
            if (conferenceId > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE {ConferenceContentAttribute.ConferenceId} = {conferenceId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> contentIdList)
        {
            if (contentIdList != null && contentIdList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                ExecuteNonQuery(sqlString);

                UpdateUserCount(publishmentSystemId);
            }
        }

        private void UpdateUserCount(int publishmentSystemId)
        {
            var conferenceIdWithCount = new Dictionary<int, int>();

            string sqlString =
                $"SELECT {ConferenceContentAttribute.ConferenceId}, COUNT(*) FROM {TableName} WHERE {ConferenceContentAttribute.PublishmentSystemId} = {publishmentSystemId} GROUP BY {ConferenceContentAttribute.ConferenceId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    conferenceIdWithCount.Add(rdr.GetInt32(0), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            DataProviderWx.ConferenceDao.UpdateUserCount(publishmentSystemId, conferenceIdWithCount);
        }

        public int GetCount(int conferenceId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {ConferenceContentAttribute.ConferenceId} = {conferenceId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public bool IsApplied(int conferenceId, string cookieSn, string wxOpenId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {ConferenceContentAttribute.ConferenceId} = {conferenceId}";

            sqlString += $" AND ({ConferenceContentAttribute.CookieSn} = '{cookieSn}'";
            sqlString += ")";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString) > 0;
        }

        public string GetSelectString(int publishmentSystemId, int conferenceId)
        {
            string whereString = $"WHERE {ConferenceContentAttribute.PublishmentSystemId} = {publishmentSystemId}";
            if (conferenceId > 0)
            {
                whereString += $" AND {ConferenceContentAttribute.ConferenceId} = {conferenceId}";
            }
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<ConferenceContentInfo> GetConferenceContentInfoList(int publishmentSystemId, int conferenceId)
        {
            var conferenceContentInfoList = new List<ConferenceContentInfo>();

            string sqlWhere =
                $" AND {ConferenceContentAttribute.PublishmentSystemId} = {publishmentSystemId} AND {ConferenceContentAttribute.ConferenceId} = {conferenceId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var conferenceContentInfo = new ConferenceContentInfo(rdr);
                    conferenceContentInfoList.Add(conferenceContentInfo);
                }
                rdr.Close();
            }

            return conferenceContentInfoList;
        }


    }
}
