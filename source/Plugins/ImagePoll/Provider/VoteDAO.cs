using System;
using System.Collections.Generic;
using System.Data;
using ImagePoll.Model;
using SiteServer.Plugin;

namespace ImagePoll.Provider
{
    public class VoteDao
    {
        private const string TableName = "imagePoll_Vote";

        private readonly string _connectionString;
        private readonly IDbHelper _helper;

        public VoteDao(string connectionString, IDbHelper helper)
        {
            _connectionString = connectionString;
            _helper = helper;
        }

        public int Insert(VoteInfo voteInfo)
        {
            var voteId = 0;

            string sqlString = $@"INSERT INTO {TableName}
           (SiteId, KeywordId, IsDisabled, UserCount, PvCount, StartDate, EndDate, Title, ImageUrl, Summary, ContentImageUrl, ContentDescription, ContentIsImageOption, ContentIsCheckBox, ContentResultVisible, EndTitle, EndImageUrl, EndSummary)
     VALUES
           (@SiteId, @KeywordId, @IsDisabled, @UserCount, @PvCount, @StartDate, @EndDate, @Title, @ImageUrl, @Summary, @ContentImageUrl, @ContentDescription, @ContentIsImageOption, @ContentIsCheckBox, @ContentResultVisible, @EndTitle, @EndImageUrl, @EndSummary)";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter("SiteId", voteInfo.SiteId),
                _helper.GetParameter("KeywordId", voteInfo.KeywordId),
                _helper.GetParameter("IsDisabled", voteInfo.IsDisabled),
                _helper.GetParameter("UserCount", voteInfo.UserCount),
                _helper.GetParameter("PvCount", voteInfo.PvCount),
                _helper.GetParameter("StartDate", voteInfo.StartDate),
                _helper.GetParameter("EndDate", voteInfo.EndDate),
                _helper.GetParameter("Title", voteInfo.Title),
                _helper.GetParameter("ImageUrl", voteInfo.ImageUrl),
                _helper.GetParameter("Summary", voteInfo.Summary),
                _helper.GetParameter("ContentImageUrl", voteInfo.ContentImageUrl),
                _helper.GetParameter("ContentDescription", voteInfo.ContentDescription),
                _helper.GetParameter("ContentIsImageOption", voteInfo.ContentIsImageOption),
                _helper.GetParameter("ContentIsCheckBox", voteInfo.ContentIsCheckBox),
                _helper.GetParameter("ContentResultVisible", voteInfo.ContentResultVisible),
                _helper.GetParameter("EndTitle", voteInfo.EndTitle),
                _helper.GetParameter("EndImageUrl", voteInfo.EndImageUrl),
                _helper.GetParameter("EndSummary", voteInfo.EndSummary)
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
                                voteId = rdr.GetInt32(0);
                            }
                            rdr.Close();
                        }

                        if (voteId == 0)
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

            return voteId;
        }

        public void Update(VoteInfo voteInfo)
        {
            string sqlString = $@"UPDATE {TableName} SET
                SiteId = @SiteId,
                KeywordId = @KeywordId,
                IsDisabled = @IsDisabled,
                UserCount = @UserCount,
                PvCount = @PvCount,
                StartDate = @StartDate,
                EndDate = @EndDate,
                Title = @Title,
                ImageUrl = @ImageUrl,
                Summary = @Summary,
                ContentImageUrl = @ContentImageUrl,
                ContentDescription = @ContentDescription,
                ContentIsImageOption = @ContentIsImageOption,
                ContentIsCheckBox = @ContentIsCheckBox,
                ContentResultVisible = @ContentResultVisible,
                EndTitle = @EndTitle,
                EndImageUrl = @EndImageUrl,
                EndSummary = @EndSummary
            WHERE ID = @ID";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter("SiteId", voteInfo.SiteId),
                _helper.GetParameter("KeywordId", voteInfo.KeywordId),
                _helper.GetParameter("IsDisabled", voteInfo.IsDisabled),
                _helper.GetParameter("UserCount", voteInfo.UserCount),
                _helper.GetParameter("PvCount", voteInfo.PvCount),
                _helper.GetParameter("StartDate", voteInfo.StartDate),
                _helper.GetParameter("EndDate", voteInfo.EndDate),
                _helper.GetParameter("Title", voteInfo.Title),
                _helper.GetParameter("ImageUrl", voteInfo.ImageUrl),
                _helper.GetParameter("Summary", voteInfo.Summary),
                _helper.GetParameter("ContentImageUrl", voteInfo.ContentImageUrl),
                _helper.GetParameter("ContentDescription", voteInfo.ContentDescription),
                _helper.GetParameter("ContentIsImageOption", voteInfo.ContentIsImageOption),
                _helper.GetParameter("ContentIsCheckBox", voteInfo.ContentIsCheckBox),
                _helper.GetParameter("ContentResultVisible", voteInfo.ContentResultVisible),
                _helper.GetParameter("EndTitle", voteInfo.EndTitle),
                _helper.GetParameter("EndImageUrl", voteInfo.EndImageUrl),
                _helper.GetParameter("EndSummary", voteInfo.EndSummary),
                _helper.GetParameter("ID", voteInfo.Id)
            };

            _helper.ExecuteNonQuery(_connectionString, CommandType.Text, sqlString, parameters.ToArray());
        }

        public void AddUserCount(int voteId)
        {
            if (voteId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET UserCount = UserCount + 1 WHERE ID = {voteId}";
                _helper.ExecuteNonQuery(_connectionString, sqlString);
            }
        }

        public void AddPvCount(int voteId)
        {
            if (voteId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET PvCount = PvCount + 1 WHERE ID = {voteId}";
                _helper.ExecuteNonQuery(_connectionString, sqlString);
            }
        }

        public void Delete(int siteId, int voteId)
        {
            if (voteId <= 0) return;

            //DataProviderWx.VoteLogDao.DeleteAll(voteId);
            //DataProviderWx.VoteItemDao.DeleteAll(siteId, voteId);

            string sqlString = $"DELETE FROM {TableName} WHERE ID = {voteId}";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public void Delete(int siteId, List<int> voteIdList)
        {
            if (voteIdList == null || voteIdList.Count <= 0) return;

            //foreach (var voteId in voteIdList)
            //{
            //    //DataProviderWx.VoteLogDao.DeleteAll(voteId);
            //    //DataProviderWx.VoteItemDao.DeleteAll(siteId, voteId);
            //}

            string sqlString =
                $"DELETE FROM {TableName} WHERE ID IN ({string.Join(",", voteIdList)})";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public List<int> GetKeywordIdList(List<int> voteIdList)
        {
            var keywordIdList = new List<int>();

            string sqlString =
                $"SELECT KeywordId FROM {TableName} WHERE ID IN ({string.Join(",", voteIdList.ToArray())})";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    keywordIdList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return keywordIdList;
        }

        public VoteInfo GetVoteInfo(int voteId)
        {
            VoteInfo voteInfo = null;

            string sqlString = $"SELECT Id, SiteId, KeywordId, IsDisabled, UserCount, PvCount, StartDate, EndDate, Title, ImageUrl, Summary, ContentImageUrl, ContentDescription, ContentIsImageOption, ContentIsCheckBox, ContentResultVisible, EndTitle, EndImageUrl, EndSummary FROM {TableName} WHERE ID = {voteId}";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read())
                {
                    voteInfo = GetVoteInfo(rdr);
                }
                rdr.Close();
            }

            return voteInfo;
        }

        public List<VoteInfo> GetVoteInfoListByKeywordId(int siteId, int keywordId)
        {
            var voteInfoList = new List<VoteInfo>();

            string sqlString =
                $"SELECT Id, SiteId, KeywordId, IsDisabled, UserCount, PvCount, StartDate, EndDate, Title, ImageUrl, Summary, ContentImageUrl, ContentDescription, ContentIsImageOption, ContentIsCheckBox, ContentResultVisible, EndTitle, EndImageUrl, EndSummary FROM {TableName} WHERE SiteId = {siteId} AND IsDisabled <> '{true}'";
            if (keywordId > 0)
            {
                sqlString += $" AND KeywordId = {keywordId}";
            }

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var voteInfo = GetVoteInfo(rdr);
                    voteInfoList.Add(voteInfo);
                }
                rdr.Close();
            }

            return voteInfoList;
        }

        public int GetFirstIdByKeywordId(int siteId, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TableName} WHERE SiteId = {siteId} AND IsDisabled <> '{true}' AND KeywordId = {keywordId}";

            var id = 0;

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    id = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return id;
        }

        public string GetTitle(int voteId)
        {
            var title = string.Empty;

            string sqlString = $"SELECT Title WHERE ID = {voteId}";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                if (rdr.Read())
                {
                    title = rdr.GetValue(0).ToString();
                }
                rdr.Close();
            }

            return title;
        }

        public string GetSelectString(int siteId)
        {
            return  $"SELECT Id, SiteId, KeywordId, IsDisabled, UserCount, PvCount, StartDate, EndDate, Title, ImageUrl, Summary, ContentImageUrl, ContentDescription, ContentIsImageOption, ContentIsCheckBox, ContentResultVisible, EndTitle, EndImageUrl, EndSummary FROM {TableName} WHERE SiteId = {siteId}";
        }

        public void UpdateUserCountById(int cNum, int voteId)
        {
            if (voteId <= 0) return;

            string sqlString =
                $"UPDATE {TableName} SET UserCount = {cNum} WHERE ID = {voteId}";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }

        public List<VoteInfo> GetVoteInfoList(int siteId)
        {
            var voteInfoList = new List<VoteInfo>();

            string sqlString = $"SELECT Id, SiteId, KeywordId, IsDisabled, UserCount, PvCount, StartDate, EndDate, Title, ImageUrl, Summary, ContentImageUrl, ContentDescription, ContentIsImageOption, ContentIsCheckBox, ContentResultVisible, EndTitle, EndImageUrl, EndSummary FROM {TableName} WHERE SiteId = {siteId}";

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var voteInfo = GetVoteInfo(rdr);
                    voteInfoList.Add(voteInfo);
                }
                rdr.Close();
            }

            return voteInfoList;
        }

        private static VoteInfo GetVoteInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;
            
            var i = 0;
            var voteInfo = new VoteInfo
            {
                Id = rdr.IsDBNull(i++) ? 0 : rdr.GetInt32(i),
                SiteId = rdr.IsDBNull(i++) ? 0 : rdr.GetInt32(i),
                KeywordId = rdr.IsDBNull(i++) ? 0 : rdr.GetInt32(i),
                IsDisabled = !rdr.IsDBNull(i++) && rdr.GetString(i) == true.ToString(),
                UserCount = rdr.IsDBNull(i++) ? 0 : rdr.GetInt32(i),
                PvCount = rdr.IsDBNull(i++) ? 0 : rdr.GetInt32(i),
                StartDate = rdr.IsDBNull(i++) ? DateTime.Now : rdr.GetDateTime(i),
                EndDate = rdr.IsDBNull(i++) ? DateTime.Now : rdr.GetDateTime(i),
                Title = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                ImageUrl = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                Summary = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                ContentImageUrl = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                ContentDescription = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                ContentIsImageOption = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                ContentIsCheckBox = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                ContentResultVisible = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                EndTitle = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                EndImageUrl = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i),
                EndSummary = rdr.IsDBNull(i++) ? string.Empty : rdr.GetString(i)
            };
            return voteInfo;
        }

    }
}
