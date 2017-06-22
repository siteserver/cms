using System;
using System.Collections.Generic;
using System.Data;
using ImagePoll.Model;
using SiteServer.Plugin;

namespace ImagePoll.Provider
{
    public class VoteLogDao
    {
        private const string TableName = "imagePoll_VoteLog";

        private readonly string _connectionString;
        private readonly IDbHelper _helper;

        public VoteLogDao(string connectionString, IDbHelper helper)
        {
            _connectionString = connectionString;
            _helper = helper;
        }

        public void Insert(VoteLogInfo logInfo)
        {
            string sqlString = $@"INSERT INTO {TableName}
           (SiteId, VoteId, ItemIdCollection, IpAddress, CookieSn, WxOpenId, UserName, AddDate)
     VALUES
           (@SiteId, @VoteId, @ItemIdCollection, @IpAddress, @CookieSn, @WxOpenId, @UserName, @AddDate)";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter("SiteId", logInfo.SiteId),
                _helper.GetParameter("VoteId", logInfo.VoteId),
                _helper.GetParameter("ItemIdCollection", logInfo.ItemIdCollection),
                _helper.GetParameter("IpAddress", logInfo.IpAddress),
                _helper.GetParameter("CookieSn", logInfo.CookieSn),
                _helper.GetParameter("WxOpenId", logInfo.WxOpenId),
                _helper.GetParameter("UserName", logInfo.UserName),
                _helper.GetParameter("AddDate", logInfo.AddDate)
            };

            _helper.ExecuteNonQuery(_connectionString, CommandType.Text, sqlString, parameters.ToArray());
        }

        public void DeleteAll(int voteId)
        {
            if (voteId <= 0) return;

            string sqlString = $"DELETE FROM {TableName} WHERE VoteId = {voteId}";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public void Delete(List<int> logIdList)
        {
            if (logIdList == null || logIdList.Count <= 0) return;
            string sqlString =
                $"DELETE FROM {TableName} WHERE ID IN ({string.Join(",", logIdList)})";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public int GetCount(int voteId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE VoteId = {voteId}";

            var count = 0;

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    count = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return count;
        }

        public bool IsVoted(int voteId, string cookieSn, string wxOpenId)
        {
            string sqlString;
            if (string.IsNullOrEmpty(wxOpenId))
            {
                sqlString =
                    $"SELECT COUNT(*) FROM {TableName} WHERE VoteId = {voteId} AND CookieSn = '{cookieSn}' ";
            }
            else
            {
                sqlString =
                    $"SELECT COUNT(*) FROM {TableName} WHERE VoteId = {voteId} AND WxOpenId = '{wxOpenId}' ";
            }

            var count = 0;

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    count = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return count > 0;
        }

        public string GetSelectString(int voteId)
        {
            return $"SELECT Id, SiteId, VoteId, ItemIdCollection, IpAddress, CookieSn, WxOpenId, UserName, AddDate FROM {TableName} WHERE VoteId = {voteId}";
        }

        public List<VoteLogInfo> GetVoteLogInfoListByVoteId(int siteId, int voteId)
        {
            var voteLogInfoList = new List<VoteLogInfo>();

            string sqlString =
                $"SELECT Id, SiteId, VoteId, ItemIdCollection, IpAddress, CookieSn, WxOpenId, UserName, AddDate FROM {TableName} WHERE SiteId = {siteId} AND VoteId = '{voteId}'";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var voteLogInfo = GetVoteItemInfo(rdr);
                    voteLogInfoList.Add(voteLogInfo);
                }
                rdr.Close();
            }

            return voteLogInfoList;
        }

        public List<VoteLogInfo> GetVoteLogInfoList(int siteId)
        {
            var voteLogInfoList = new List<VoteLogInfo>();

            string sqlString = $"SELECT Id, SiteId, VoteId, ItemIdCollection, IpAddress, CookieSn, WxOpenId, UserName, AddDate FROM {TableName} WHERE SiteId = {siteId}";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var voteLogInfo = GetVoteItemInfo(rdr);
                    voteLogInfoList.Add(voteLogInfo);
                }
                rdr.Close();
            }

            return voteLogInfoList;
        }

        public int GetCount(int voteId, string iPAddress)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE VoteId = {voteId} AND IpAddress = '{iPAddress}'";

            var count = 0;

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    count = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return count;
        }

        private static VoteLogInfo GetVoteItemInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var i = 0;
            var logInfo = new VoteLogInfo
            {
                Id = rdr.IsDBNull(i++) ? 0 : rdr.GetInt32(i),
                SiteId = rdr.IsDBNull(i++) ? 0 : rdr.GetInt32(i),
                VoteId = rdr.IsDBNull(i++) ? 0 : rdr.GetInt32(i),
                ItemIdCollection = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                IpAddress = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                CookieSn = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                WxOpenId = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                UserName = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                AddDate = rdr.IsDBNull(i++) ? DateTime.Now : rdr.GetDateTime(i)
            };
            return logInfo;
        }
    }
}
