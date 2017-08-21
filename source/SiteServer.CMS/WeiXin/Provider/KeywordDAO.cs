using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class KeywordDao : DataProviderBase
    {

        private const string SqlUpdate = "UPDATE wx_Keyword SET PublishmentSystemID = @PublishmentSystemID, Keywords = @Keywords, IsDisabled = @IsDisabled, KeywordType = @KeywordType, MatchType = @MatchType, Reply = @Reply, AddDate = @AddDate, Taxis = @Taxis WHERE KeywordID = @KeywordID";

        private const string SqlUpdateKeywrodsAndIsdisabled = "UPDATE wx_Keyword SET Keywords = @Keywords, IsDisabled = @IsDisabled WHERE KeywordID = @KeywordID";

        private const string SqlUpdateKeywrods = "UPDATE wx_Keyword SET Keywords = @Keywords, KeywordType = @KeywordType WHERE KeywordID = @KeywordID";

        private const string SqlDelete = "DELETE FROM wx_Keyword WHERE KeywordID = @KeywordID";

        private const string SqlSelect = "SELECT KeywordID, PublishmentSystemID, Keywords, IsDisabled, KeywordType, MatchType, Reply, AddDate, Taxis FROM wx_Keyword WHERE KeywordID = @KeywordID";

        private const string SqlSelectAvaliable = "SELECT TOP 1 KeywordID, PublishmentSystemID, Keywords, IsDisabled, KeywordType, MatchType, Reply, AddDate, Taxis FROM wx_Keyword WHERE PublishmentSystemID = @PublishmentSystemID AND IsDisabled = @IsDisabled AND KeywordType = @KeywordType";

        private const string SqlSelectKeywrods = "SELECT Keywords FROM wx_Keyword WHERE KeywordID = @KeywordID";

        private const string SqlSelectAll = "SELECT KeywordID, PublishmentSystemID, Keywords, IsDisabled, KeywordType, MatchType, Reply, AddDate, Taxis FROM wx_Keyword WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY Taxis DESC";

        private const string SqlSelectAllByType = "SELECT KeywordID, PublishmentSystemID, Keywords, IsDisabled, KeywordType, MatchType, Reply, AddDate, Taxis FROM wx_Keyword WHERE PublishmentSystemID = @PublishmentSystemID AND KeywordType = @KeywordType ORDER BY Taxis DESC";

        private const string ParmKeywordId = "@KeywordID";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmKeywords = "@Keywords";
        private const string ParmIsDisabled = "@IsDisabled";
        private const string ParmKeywordType = "@KeywordType";
        private const string ParmMatchType = "@MatchType";
        private const string ParmReply = "@Reply";
        private const string ParmAddDate = "@AddDate";
        private const string ParmTaxis = "@Taxis";

        public int Insert(KeywordInfo keywordInfo)
        {
            var keywordId = 0;

            var sqlString = "INSERT INTO wx_Keyword (PublishmentSystemID, Keywords, IsDisabled, KeywordType, MatchType, Reply, AddDate, Taxis) VALUES (@PublishmentSystemID, @Keywords, @IsDisabled, @KeywordType, @MatchType, @Reply, @AddDate, @Taxis)";

            var taxis = GetMaxTaxis(keywordInfo.PublishmentSystemId, keywordInfo.KeywordType) + 1;
            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, keywordInfo.PublishmentSystemId),
                GetParameter(ParmKeywords, DataType.NVarChar, 255, keywordInfo.Keywords),
                GetParameter(ParmIsDisabled, DataType.VarChar, 18, keywordInfo.IsDisabled.ToString()),
                GetParameter(ParmKeywordType, DataType.VarChar, 50, EKeywordTypeUtils.GetValue(keywordInfo.KeywordType)),
                GetParameter(ParmMatchType, DataType.VarChar, 50, EMatchTypeUtils.GetValue(keywordInfo.MatchType)),
                GetParameter(ParmReply, DataType.NText, keywordInfo.Reply),
                GetParameter(ParmAddDate, DataType.DateTime, keywordInfo.AddDate),
                GetParameter(ParmTaxis, DataType.Integer, taxis)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        keywordId = ExecuteNonQueryAndReturnId(trans, sqlString, parms);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            foreach (var str in TranslateUtils.StringCollectionToStringList(keywordInfo.Keywords, ' '))
            {
                var keyword = str.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    DataProviderWx.KeywordMatchDao.Insert(new KeywordMatchInfo(0, keywordInfo.PublishmentSystemId, keyword, keywordId, keywordInfo.IsDisabled, keywordInfo.KeywordType, keywordInfo.MatchType));
                }
            }

            return keywordId;
        }

        public void Update(KeywordInfo keywordInfo)
        {
            if (keywordInfo != null && keywordInfo.KeywordId > 0)
            {
                var parms = new IDataParameter[]
			    {
                    GetParameter(ParmPublishmentSystemId, DataType.Integer, keywordInfo.PublishmentSystemId),
                    GetParameter(ParmKeywords, DataType.NVarChar, 255, keywordInfo.Keywords),
                    GetParameter(ParmIsDisabled, DataType.VarChar, 18, keywordInfo.IsDisabled.ToString()),
                    GetParameter(ParmKeywordType, DataType.VarChar, 50, EKeywordTypeUtils.GetValue(keywordInfo.KeywordType)),
                    GetParameter(ParmMatchType, DataType.VarChar, 50, EMatchTypeUtils.GetValue(keywordInfo.MatchType)),
                    GetParameter(ParmReply, DataType.NText, keywordInfo.Reply),
                    GetParameter(ParmAddDate, DataType.DateTime, keywordInfo.AddDate),
                    GetParameter(ParmTaxis, DataType.Integer, keywordInfo.Taxis),
                    GetParameter(ParmKeywordId, DataType.Integer, keywordInfo.KeywordId)
			    };

                ExecuteNonQuery(SqlUpdate, parms);

                DataProviderWx.KeywordMatchDao.DeleteByKeywordId(keywordInfo.KeywordId);

                foreach (var str in TranslateUtils.StringCollectionToStringList(keywordInfo.Keywords, ' '))
                {
                    var keyword = str.Trim();
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        DataProviderWx.KeywordMatchDao.Insert(new KeywordMatchInfo(0, keywordInfo.PublishmentSystemId, keyword, keywordInfo.KeywordId, keywordInfo.IsDisabled, keywordInfo.KeywordType, keywordInfo.MatchType));
                    }
                }
            }
        }

        public void Update(int publishmentSystemId, int keywordId, EKeywordType keywordType, EMatchType matchType, string keywords, bool isDisabled)
        {
            var parms = new IDataParameter[]
			{
                GetParameter(ParmKeywords, DataType.NVarChar, 255, keywords),
                GetParameter(ParmIsDisabled, DataType.VarChar, 18, isDisabled.ToString()),
                GetParameter(ParmKeywordId, DataType.Integer, keywordId)
			};

            ExecuteNonQuery(SqlUpdateKeywrodsAndIsdisabled, parms);

            DataProviderWx.KeywordMatchDao.DeleteByKeywordId(keywordId);

            foreach (var str in TranslateUtils.StringCollectionToStringList(keywords, ' '))
            {
                var keyword = str.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    DataProviderWx.KeywordMatchDao.Insert(new KeywordMatchInfo(0, publishmentSystemId, keyword, keywordId, isDisabled, keywordType, matchType));
                }
            }
        }

        public void Update(int publishmentSystemId, int keywordId, EKeywordType keywordType, EMatchType matchType, string keywords)
        {
            var parms = new IDataParameter[]
			{
                GetParameter(ParmKeywords, DataType.NVarChar, 255, keywords),
                GetParameter(ParmKeywordType, DataType.VarChar, 50, EKeywordTypeUtils.GetValue(keywordType)),
                GetParameter(ParmKeywordId, DataType.Integer, keywordId)
			};

            ExecuteNonQuery(SqlUpdateKeywrods, parms);

            DataProviderWx.KeywordMatchDao.DeleteByKeywordId(keywordId);

            foreach (var str in TranslateUtils.StringCollectionToStringList(keywords, ' '))
            {
                var keyword = str.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    DataProviderWx.KeywordMatchDao.Insert(new KeywordMatchInfo(0, publishmentSystemId, keyword, keywordId, false, keywordType, matchType));
                }
            }
        }

        public void Delete(int keywordId)
        {
            if (keywordId > 0)
            {
                var parms = new IDataParameter[]
			    {
				    GetParameter(ParmKeywordId, DataType.Integer, keywordId)
			    };

                ExecuteNonQuery(SqlDelete, parms);
            }
        }

        public void Delete(List<int> keywordIdList)
        {
            if (keywordIdList != null && keywordIdList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM wx_Keyword WHERE KeywordID IN ({TranslateUtils.ToSqlInStringWithoutQuote(keywordIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public string GetKeywords(int keywordId)
        {
            var keywords = string.Empty;

            if (keywordId > 0)
            {
                var parms = new IDataParameter[]
			    {
				    GetParameter(ParmKeywordId, DataType.Integer, keywordId)
			    };

                using (var rdr = ExecuteReader(SqlSelectKeywrods, parms))
                {
                    if (rdr.Read())
                    {
                        keywords = rdr.GetValue(0).ToString();
                    }
                    rdr.Close();
                }
            }

            return keywords;
        }

        public KeywordInfo GetKeywordInfo(int keywordId)
        {
            KeywordInfo keywordInfo = null;

            if (keywordId > 0)
            {
                var parms = new IDataParameter[]
			    {
				    GetParameter(ParmKeywordId, DataType.Integer, keywordId)
			    };

                using (var rdr = ExecuteReader(SqlSelect, parms))
                {
                    if (rdr.Read())
                    {
                        keywordInfo = new KeywordInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetValue(2).ToString(), TranslateUtils.ToBool(rdr.GetValue(3).ToString()), EKeywordTypeUtils.GetEnumType(rdr.GetValue(4).ToString()), EMatchTypeUtils.GetEnumType(rdr.GetValue(5).ToString()), rdr.GetValue(6).ToString(), rdr.GetDateTime(7), rdr.GetInt32(8));
                    }
                    rdr.Close();
                }
            }
            else
            {
                keywordInfo = new KeywordInfo();
                keywordInfo.KeywordId = 0;
                keywordInfo.Keywords = "";
            }

            if (keywordInfo == null)
            {
                keywordInfo = new KeywordInfo();
                keywordInfo.KeywordId = 0;
                keywordInfo.Keywords = "";
            }
            return keywordInfo;
        }

        public int GetKeywordId(int publishmentSystemId, bool isExists, string keywords, EKeywordType keywordType, int existKeywordId)
        {
            var keywordId = existKeywordId;

            if (isExists)
            {
                if (!string.IsNullOrEmpty(keywords))
                {
                    if (existKeywordId > 0)
                    {
                        DataProviderWx.KeywordDao.Update(publishmentSystemId, existKeywordId, keywordType, EMatchType.Exact, keywords);
                    }
                    else
                    {
                        var keywordInfo = new KeywordInfo(0, publishmentSystemId, keywords, false, keywordType, EMatchType.Exact, string.Empty, DateTime.Now, 0);
                        keywordId = DataProviderWx.KeywordDao.Insert(keywordInfo);
                    }
                }
                else
                {
                    if (existKeywordId > 0)
                    {
                        DataProviderWx.KeywordDao.Delete(existKeywordId);
                        keywordId = 0;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(keywords))
                {
                    var keywordInfo = new KeywordInfo(0, publishmentSystemId, keywords, false, keywordType, EMatchType.Exact, string.Empty, DateTime.Now, 0);
                    keywordId = DataProviderWx.KeywordDao.Insert(keywordInfo);
                }
            }

            return keywordId;
        }

        public KeywordInfo GetAvaliableKeywordInfo(int publishmentSystemId, EKeywordType keywordType)
        {
            KeywordInfo keywordInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
                GetParameter(ParmIsDisabled, DataType.VarChar, 18, false.ToString()),
                GetParameter(ParmKeywordType, DataType.VarChar, 50, EKeywordTypeUtils.GetValue(keywordType))
			};

            using (var rdr = ExecuteReader(SqlSelectAvaliable, parms))
            {
                if (rdr.Read())
                {
                    keywordInfo = new KeywordInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetValue(2).ToString(), TranslateUtils.ToBool(rdr.GetValue(3).ToString()), EKeywordTypeUtils.GetEnumType(rdr.GetValue(4).ToString()), EMatchTypeUtils.GetEnumType(rdr.GetValue(5).ToString()), rdr.GetValue(6).ToString(), rdr.GetDateTime(7), rdr.GetInt32(8));
                }
                rdr.Close();
            }

            return keywordInfo;
        }

        public IEnumerable GetDataSource(int publishmentSystemId, EKeywordType keywordType)
        {
            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmKeywordType, DataType.VarChar, 50, EKeywordTypeUtils.GetValue(keywordType))
			};

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllByType, parms);
            return enumerable;
        }

        public int GetCount(int publishmentSystemId, EKeywordType keywordType)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM wx_Keyword WHERE PublishmentSystemID = {publishmentSystemId} AND KeywordType = '{EKeywordTypeUtils.GetValue(keywordType)}'";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<KeywordInfo> GetKeywordInfoList(int publishmentSystemId)
        {
            var list = new List<KeywordInfo>();

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAll, parms))
            {
                while (rdr.Read())
                {
                    var keywordInfo = new KeywordInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetValue(2).ToString(), TranslateUtils.ToBool(rdr.GetValue(3).ToString()), EKeywordTypeUtils.GetEnumType(rdr.GetValue(4).ToString()), EMatchTypeUtils.GetEnumType(rdr.GetValue(5).ToString()), rdr.GetValue(6).ToString(), rdr.GetDateTime(7), rdr.GetInt32(8));
                    list.Add(keywordInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public List<KeywordInfo> GetKeywordInfoList(int publishmentSystemId, EKeywordType keywordType)
        {
            var list = new List<KeywordInfo>();

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmKeywordType, DataType.VarChar, 50, EKeywordTypeUtils.GetValue(keywordType))
			};

            using (var rdr = ExecuteReader(SqlSelectAllByType, parms))
            {
                while (rdr.Read())
                {
                    var keywordInfo = new KeywordInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetValue(2).ToString(), TranslateUtils.ToBool(rdr.GetValue(3).ToString()), EKeywordTypeUtils.GetEnumType(rdr.GetValue(4).ToString()), EMatchTypeUtils.GetEnumType(rdr.GetValue(5).ToString()), rdr.GetValue(6).ToString(), rdr.GetDateTime(7), rdr.GetInt32(8));
                    list.Add(keywordInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public bool UpdateTaxisToUp(int publishmentSystemId, EKeywordType keywordType, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 KeywordID, Taxis FROM wx_Keyword WHERE (Taxis > (SELECT Taxis FROM wx_Keyword WHERE KeywordID = {keywordId})) AND PublishmentSystemID = {publishmentSystemId} AND KeywordType = '{EKeywordTypeUtils.GetValue(keywordType)}' ORDER BY Taxis";
            var higherId = 0;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherId = rdr.GetInt32(0);
                    higherTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(keywordId);

            if (higherId > 0)
            {
                SetTaxis(keywordId, higherTaxis);
                SetTaxis(higherId, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int publishmentSystemId, EKeywordType keywordType, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 KeywordID, Taxis FROM wx_Keyword WHERE (Taxis < (SELECT Taxis FROM wx_Keyword WHERE KeywordID = {keywordId})) AND PublishmentSystemID = {publishmentSystemId} AND KeywordType = '{EKeywordTypeUtils.GetValue(keywordType)}' ORDER BY Taxis DESC";
            var lowerId = 0;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerId = rdr.GetInt32(0);
                    lowerTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(keywordId);

            if (lowerId > 0)
            {
                SetTaxis(keywordId, lowerTaxis);
                SetTaxis(lowerId, selectedTaxis);
                return true;
            }
            return false;
        }

        private int GetMaxTaxis(int publishmentSystemId, EKeywordType keywordType)
        {
            string sqlString =
                $"SELECT MAX(Taxis) FROM wx_Keyword WHERE PublishmentSystemID = {publishmentSystemId} AND KeywordType = '{EKeywordTypeUtils.GetValue(keywordType)}'";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int keywordId)
        {
            string sqlString = $"SELECT Taxis FROM wx_Keyword WHERE KeywordID = {keywordId}";
            var taxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    taxis = Convert.ToInt32(rdr[0]);
                }
                rdr.Close();
            }

            return taxis;
        }

        private void SetTaxis(int keywordId, int taxis)
        {
            string sqlString = $"UPDATE wx_Keyword SET Taxis = {taxis} WHERE KeywordID = {keywordId}";
            ExecuteNonQuery(sqlString);
        }


        public int GetKeywordsIDbyName(int publishmentSystemId, string keywords)
        {
            string sqlString =
                $"SELECT KeywordID FROM wx_Keyword WHERE Keywords = '{keywords}' AND PublishmentSystemID = {publishmentSystemId}";
            var keywordId = 0;
            if (!string.IsNullOrEmpty(keywords))
            {
                using (var rdr = ExecuteReader(sqlString))
                {
                    if (rdr.Read())
                    {
                        keywordId = Convert.ToInt32(rdr[0]);
                    }
                    rdr.Close();
                }
            }
            return keywordId;
        }
    }
}