using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class ConferenceContentDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_ConferenceContent";


        public void Insert(ConferenceContentInfo contentInfo)
        {
            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(contentInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);


            ExecuteNonQuery(SQL_INSERT, parms);
        }

        public void DeleteAll(int conferenceID)
        {
            if (conferenceID > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE {ConferenceContentAttribute.ConferenceID} = {conferenceID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> contentIDList)
        {
            if (contentIDList != null && contentIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIDList)})";
                ExecuteNonQuery(sqlString);

                UpdateUserCount(publishmentSystemID);
            }
        }

        private void UpdateUserCount(int publishmentSystemID)
        {
            var conferenceIDWithCount = new Dictionary<int, int>();

            string sqlString =
                $"SELECT {ConferenceContentAttribute.ConferenceID}, COUNT(*) FROM {TABLE_NAME} WHERE {ConferenceContentAttribute.PublishmentSystemID} = {publishmentSystemID} GROUP BY {ConferenceContentAttribute.ConferenceID}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    conferenceIDWithCount.Add(rdr.GetInt32(0), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            DataProviderWX.ConferenceDAO.UpdateUserCount(publishmentSystemID, conferenceIDWithCount);
        }

        public int GetCount(int conferenceID)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {ConferenceContentAttribute.ConferenceID} = {conferenceID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public bool IsApplied(int conferenceID, string cookieSN, string wxOpenID)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {ConferenceContentAttribute.ConferenceID} = {conferenceID}";

            sqlString += $" AND ({ConferenceContentAttribute.CookieSN} = '{cookieSN}'";
            sqlString += ")";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString) > 0;
        }

        public string GetSelectString(int publishmentSystemID, int conferenceID)
        {
            string whereString = $"WHERE {ConferenceContentAttribute.PublishmentSystemID} = {publishmentSystemID}";
            if (conferenceID > 0)
            {
                whereString += $" AND {ConferenceContentAttribute.ConferenceID} = {conferenceID}";
            }
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<ConferenceContentInfo> GetConferenceContentInfoList(int publishmentSystemID, int conferenceID)
        {
            var conferenceContentInfoList = new List<ConferenceContentInfo>();

            string SQL_WHERE =
                $" AND {ConferenceContentAttribute.PublishmentSystemID} = {publishmentSystemID} AND {ConferenceContentAttribute.ConferenceID} = {conferenceID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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
