using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class VoteLogDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_VoteLog";

        public void Insert(VoteLogInfo logInfo)
        {
            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(logInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_INSERT, parms);
        }

        public void DeleteAll(int voteID)
        {
            if (voteID > 0)
            {
                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE {VoteLogAttribute.VoteID} = {voteID}";
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

        public int GetCount(int voteID)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {VoteLogAttribute.VoteID} = {voteID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public bool IsVoted(int voteID, string cookieSN, string wxOpenID)
        {
            var isVoted = false;
            string sqlString;
            if (string.IsNullOrEmpty(wxOpenID))
            {
                sqlString =
                    $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {VoteLogAttribute.VoteID} = {voteID} AND {VoteLogAttribute.CookieSN} = '{cookieSN}' ";
            }
            else
            {
                sqlString =
                    $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {VoteLogAttribute.VoteID} = {voteID} AND {VoteLogAttribute.WXOpenID} = '{wxOpenID}' ";
            } 

            isVoted = BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString) > 0;

            return isVoted;
        }

        public string GetSelectString(int voteID)
        {
            string whereString = $"WHERE {VoteLogAttribute.VoteID} = {voteID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<VoteLogInfo> GetVoteLogInfoListByVoteID(int publishmentSystemID, int voteID)
        {
            var voteLogInfoList = new List<VoteLogInfo>();

            string SQL_WHERE =
                $"WHERE {VoteLogAttribute.PublishmentSystemID} = {publishmentSystemID} AND {VoteLogAttribute.VoteID} = '{voteID}'";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var voteLogInfo = new VoteLogInfo(rdr);
                    voteLogInfoList.Add(voteLogInfo);
                }
                rdr.Close();
            }

            return voteLogInfoList;
        }

        public List<VoteLogInfo> GetVoteLogInfoList(int publishmentSystemID)
        {
            var voteLogInfoList = new List<VoteLogInfo>();

            string SQL_WHERE = $"WHERE {VoteLogAttribute.PublishmentSystemID} = {publishmentSystemID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var voteLogInfo = new VoteLogInfo(rdr);
                    voteLogInfoList.Add(voteLogInfo);
                }
                rdr.Close();
            }

            return voteLogInfoList;
        }

        public int GetCount(int voteID, string iPAddress)
        {

            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {VoteLogAttribute.VoteID} = {voteID} AND {VoteLogAttribute.IPAddress} = '{iPAddress}'";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }
    }
}
