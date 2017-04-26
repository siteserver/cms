using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class ConferenceDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Conference";

        public int Insert(ConferenceInfo conferenceInfo)
        {
            var conferenceID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(conferenceInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);


            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        conferenceID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return conferenceID;
        }

        public void Update(ConferenceInfo conferenceInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(conferenceInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void AddUserCount(int conferenceID)
        {
            if (conferenceID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {ConferenceAttribute.UserCount} = {ConferenceAttribute.UserCount} + 1 WHERE ID = {conferenceID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void AddPVCount(int conferenceID)
        {
            if (conferenceID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {ConferenceAttribute.PVCount} = {ConferenceAttribute.PVCount} + 1 WHERE ID = {conferenceID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, int conferenceID)
        {
            if (conferenceID > 0)
            {
                var conferenceIDList = new List<int>();
                conferenceIDList.Add(conferenceID);
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(conferenceIDList));

                DataProviderWX.ConferenceContentDAO.DeleteAll(conferenceID);

                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {conferenceID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> conferenceIDList)
        {
            if (conferenceIDList != null && conferenceIDList.Count > 0)
            {
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(conferenceIDList));

                foreach (var conferenceID in conferenceIDList)
                {
                    DataProviderWX.ConferenceContentDAO.DeleteAll(conferenceID);
                }

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(conferenceIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetConferenceIDList(int publishmentSystemID)
        {
            var conferenceIDList = new List<int>();

            string SQL_WHERE = $"WHERE {ConferenceAttribute.PublishmentSystemID} = {publishmentSystemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, ConferenceAttribute.ID, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    conferenceIDList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return conferenceIDList;
        }

        public void UpdateUserCount(int publishmentSystemID, Dictionary<int, int> conferenceIDWithCount)
        {
            if (conferenceIDWithCount.Count == 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {ConferenceAttribute.UserCount} = 0 WHERE {ConferenceAttribute.PublishmentSystemID} = {publishmentSystemID}";
                ExecuteNonQuery(sqlString);
            }
            else
            {
                var conferenceIDList = GetConferenceIDList(publishmentSystemID);
                foreach (var conferenceID in conferenceIDList)
                {
                    if (conferenceIDWithCount.ContainsKey(conferenceID))
                    {
                        string sqlString =
                            $"UPDATE {TABLE_NAME} SET {ConferenceAttribute.UserCount} = {conferenceIDWithCount[conferenceID]} WHERE ID = {conferenceID}";
                        ExecuteNonQuery(sqlString);
                    }
                    else
                    {
                        string sqlString =
                            $"UPDATE {TABLE_NAME} SET {ConferenceAttribute.UserCount} = 0 WHERE ID = {conferenceID}";
                        ExecuteNonQuery(sqlString);
                    }
                }
            }
        }

        private List<int> GetKeywordIDList(List<int> conferenceIDList)
        {
            var keywordIDList = new List<int>();

            string sqlString =
                $"SELECT {ConferenceAttribute.KeywordID} FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(conferenceIDList)})";

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

        public ConferenceInfo GetConferenceInfo(int conferenceID)
        {
            ConferenceInfo conferenceInfo = null;

            string SQL_WHERE = $"WHERE ID = {conferenceID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    conferenceInfo = new ConferenceInfo(rdr);
                }
                rdr.Close();
            }

            return conferenceInfo;
        }

        public List<ConferenceInfo> GetConferenceInfoListByKeywordID(int publishmentSystemID, int keywordID)
        {
            var conferenceInfoList = new List<ConferenceInfo>();

            string SQL_WHERE =
                $"WHERE {ConferenceAttribute.PublishmentSystemID} = {publishmentSystemID} AND {ConferenceAttribute.IsDisabled} <> '{true}'";
            if (keywordID > 0)
            {
                SQL_WHERE += $" AND {ConferenceAttribute.KeywordID} = {keywordID}";
            }

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var conferenceInfo = new ConferenceInfo(rdr);
                    conferenceInfoList.Add(conferenceInfo);
                }
                rdr.Close();
            }

            return conferenceInfoList;
        }

        public int GetFirstIDByKeywordID(int publishmentSystemID, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TABLE_NAME} WHERE {ConferenceAttribute.PublishmentSystemID} = {publishmentSystemID} AND {ConferenceAttribute.IsDisabled} <> '{true}' AND {ConferenceAttribute.KeywordID} = {keywordID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int conferenceID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {conferenceID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, ConferenceAttribute.Title, SQL_WHERE, null);

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
            string whereString = $"WHERE {ConferenceAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<ConferenceInfo> GetConferenceInfoList(int publishmentSystemID)
        {
            var conferenceInfoList = new List<ConferenceInfo>();

            string SQL_WHERE = $" AND {ConferenceAttribute.PublishmentSystemID} = {publishmentSystemID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var conferenceInfo = new ConferenceInfo(rdr);
                    conferenceInfoList.Add(conferenceInfo);
                }
                rdr.Close();
            }

            return conferenceInfoList;
        }

    }
}
