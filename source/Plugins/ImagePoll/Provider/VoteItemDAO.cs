using System.Collections.Generic;
using System.Data;
using ImagePoll.Model;
using SiteServer.Plugin;

namespace ImagePoll.Provider
{
    public class VoteItemDao
    {
        private const string TableName = "imagePoll_VoteItem";

        private readonly string _connectionString;
        private readonly IDbHelper _helper;

        public VoteItemDao(string connectionString, IDbHelper helper)
        {
            _connectionString = connectionString;
            _helper = helper;
        }

        public int Insert(VoteItemInfo itemInfo)
        {
            var voteItemId = 0;

            string sqlString = $@"INSERT INTO {TableName}
           (VoteId, SiteId, Title, ImageUrl, NavigationUrl, VoteNum)
     VALUES
           (@VoteId, @SiteId, @Title, @ImageUrl, @NavigationUrl, @VoteNum)";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter("VoteId", itemInfo.VoteId),
                _helper.GetParameter("SiteId", itemInfo.SiteId),
                _helper.GetParameter("Title", itemInfo.Title),
                _helper.GetParameter("ImageUrl", itemInfo.ImageUrl),
                _helper.GetParameter("NavigationUrl", itemInfo.NavigationUrl),
                _helper.GetParameter("VoteNum", itemInfo.VoteNum)
            };

            using (var conn = _helper.GetConnection(_connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        _helper.ExecuteNonQuery(trans, CommandType.Text, sqlString, parameters.ToArray());

                        using (var rdr = _helper.ExecuteReader(trans, "SELECT @@IDENTITY AS 'ID'"))
                        {
                            if (rdr.Read())
                            {
                                voteItemId = rdr.GetInt32(0);
                            }
                            rdr.Close();
                        }

                        if (voteItemId == 0)
                        {
                            trans.Rollback();
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return voteItemId;
        }

        public void Update(VoteItemInfo itemInfo)
        {
            string sqlString = $@"UPDATE {TableName} SET
                VoteId = @VoteId,
                SiteId = @SiteId,
                Title = @Title,
                ImageUrl = @ImageUrl,
                NavigationUrl = @NavigationUrl,
                VoteNum = @VoteNum
            WHERE ID = @ID";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter("VoteId", itemInfo.VoteId),
                _helper.GetParameter("SiteId", itemInfo.SiteId),
                _helper.GetParameter("Title", itemInfo.Title),
                _helper.GetParameter("ImageUrl", itemInfo.ImageUrl),
                _helper.GetParameter("NavigationUrl", itemInfo.NavigationUrl),
                _helper.GetParameter("VoteNum", itemInfo.VoteNum),
                _helper.GetParameter("ID", itemInfo.Id)
            };

            _helper.ExecuteNonQuery(_connectionString, CommandType.Text, sqlString, parameters.ToArray());
        }

        public void UpdateVoteId(int siteId, int voteId)
        {
            if (voteId <= 0) return;

            var sqlString =
                $"UPDATE {TableName} SET VoteId = {voteId} WHERE VoteId = 0 AND SiteId = {siteId}";
            _helper.ExecuteReader(_connectionString, sqlString);
        }

        public void DeleteAll(int siteId, int voteId)
        {
            if (voteId <= 0) return;

            string sqlString =
                $"DELETE FROM {TableName} WHERE SiteId = {siteId} AND VoteId = {voteId}";
            _helper.ExecuteReader(_connectionString, sqlString);
        }

        public VoteItemInfo GetVoteItemInfo(int itemId)
        {
            VoteItemInfo voteItemInfo = null;

            string sqlString = $"SELECT Id, VoteId, SiteId, Title, ImageUrl, NavigationUrl, VoteNum FROM {TableName} WHERE ID = {itemId}";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read())
                {
                    voteItemInfo = GetVoteItemInfo(rdr);
                }
                rdr.Close();
            }

            return voteItemInfo;
        }


        public List<VoteItemInfo> GetVoteItemInfoList(int voteId)
        {
            var list = new List<VoteItemInfo>();

            string sqlString = $"SELECT Id, VoteId, SiteId, Title, ImageUrl, NavigationUrl, VoteNum FROM {TableName} WHERE VoteId = {voteId}";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var itemInfo = GetVoteItemInfo(rdr);
                    list.Add(itemInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public void AddVoteNum(int voteId, List<int> itemIdList)
        {
            if (voteId <= 0 || itemIdList == null || itemIdList.Count <= 0) return;

            string sqlString =
                $"UPDATE {TableName} SET VoteNum = VoteNum + 1 WHERE ID IN ({string.Join(",", itemIdList)}) AND VoteID = {voteId}";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public void UpdateVoteNumById(int vNum, int voteItemId)
        {
            if (voteItemId <= 0) return;

            string sqlString =
                $"UPDATE {TableName} SET VoteNum = {vNum} WHERE ID = {voteItemId} ";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public void UpdateAllVoteNumByVoteId(int vNum, int voteId)
        {
            if (voteId <= 0) return;

            string sqlString =
                $"UPDATE {TableName} SET VoteNum = {vNum} WHERE VoteID = {voteId}";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public void UpdateOtherVoteNumByIdList(List<int> logIdList, int vNum, int voteId)
        {
            if (logIdList == null || logIdList.Count <= 0) return;

            string sqlString =
                $"UPDATE {TableName} SET VoteNum = {vNum} WHERE VoteID = {voteId} AND ID NOT IN ({string.Join(",", logIdList)}) ";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public List<VoteItemInfo> GetVoteItemInfoList(int siteId, int voteId)
        {
            var list = new List<VoteItemInfo>();

            string sqlString =
                $"SELECT Id, VoteId, SiteId, Title, ImageUrl, NavigationUrl, VoteNum FROM {TableName} WHERE SiteId = {siteId} AND VoteId = {voteId}";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var itemInfo = GetVoteItemInfo(rdr);
                    list.Add(itemInfo);
                }
                rdr.Close();
            }

            return list;
        }

        private static VoteItemInfo GetVoteItemInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var i = 0;
            var itemInfo = new VoteItemInfo
            {
                Id = rdr.IsDBNull(i++) ? 0 : rdr.GetInt32(i),
                VoteId = rdr.IsDBNull(i++) ? 0 : rdr.GetInt32(i),
                SiteId = rdr.IsDBNull(i++) ? 0 : rdr.GetInt32(i),
                Title = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                ImageUrl = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                NavigationUrl = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                VoteNum = rdr.IsDBNull(i++) ? 0 : rdr.GetInt32(i)
            };
            return itemInfo;
        }
    }
}
