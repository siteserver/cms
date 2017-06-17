using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class VoteLogDao : DataProviderBase
    {
        private const string TableName = "wx_VoteLog";

        public void Insert(VoteLogInfo logInfo)
        {
            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(logInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlInsert, parms);
        }

        public void DeleteAll(int voteId)
        {
            if (voteId > 0)
            {
                string sqlString = $"DELETE FROM {TableName} WHERE {VoteLogAttribute.VoteId} = {voteId}";
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

        public int GetCount(int voteId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {VoteLogAttribute.VoteId} = {voteId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public bool IsVoted(int voteId, string cookieSn, string wxOpenId)
        {
            var isVoted = false;
            string sqlString;
            if (string.IsNullOrEmpty(wxOpenId))
            {
                sqlString =
                    $"SELECT COUNT(*) FROM {TableName} WHERE {VoteLogAttribute.VoteId} = {voteId} AND {VoteLogAttribute.CookieSn} = '{cookieSn}' ";
            }
            else
            {
                sqlString =
                    $"SELECT COUNT(*) FROM {TableName} WHERE {VoteLogAttribute.VoteId} = {voteId} AND {VoteLogAttribute.WxOpenId} = '{wxOpenId}' ";
            } 

            isVoted = BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString) > 0;

            return isVoted;
        }

        public string GetSelectString(int voteId)
        {
            string whereString = $"WHERE {VoteLogAttribute.VoteId} = {voteId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<VoteLogInfo> GetVoteLogInfoListByVoteId(int publishmentSystemId, int voteId)
        {
            var voteLogInfoList = new List<VoteLogInfo>();

            string sqlWhere =
                $"WHERE {VoteLogAttribute.PublishmentSystemId} = {publishmentSystemId} AND {VoteLogAttribute.VoteId} = '{voteId}'";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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

        public List<VoteLogInfo> GetVoteLogInfoList(int publishmentSystemId)
        {
            var voteLogInfoList = new List<VoteLogInfo>();

            string sqlWhere = $"WHERE {VoteLogAttribute.PublishmentSystemId} = {publishmentSystemId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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

        public int GetCount(int voteId, string iPAddress)
        {

            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {VoteLogAttribute.VoteId} = {voteId} AND {VoteLogAttribute.IpAddress} = '{iPAddress}'";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }
    }
}
