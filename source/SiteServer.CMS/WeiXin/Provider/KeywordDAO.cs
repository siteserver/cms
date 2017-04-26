using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class KeywordDAO : DataProviderBase
    {

        private const string SQL_UPDATE = "UPDATE wx_Keyword SET PublishmentSystemID = @PublishmentSystemID, Keywords = @Keywords, IsDisabled = @IsDisabled, KeywordType = @KeywordType, MatchType = @MatchType, Reply = @Reply, AddDate = @AddDate, Taxis = @Taxis WHERE KeywordID = @KeywordID";

        private const string SQL_UPDATE_KEYWRODS_AND_ISDISABLED = "UPDATE wx_Keyword SET Keywords = @Keywords, IsDisabled = @IsDisabled WHERE KeywordID = @KeywordID";

        private const string SQL_UPDATE_KEYWRODS = "UPDATE wx_Keyword SET Keywords = @Keywords, KeywordType = @KeywordType WHERE KeywordID = @KeywordID";

        private const string SQL_DELETE = "DELETE FROM wx_Keyword WHERE KeywordID = @KeywordID";

        private const string SQL_SELECT = "SELECT KeywordID, PublishmentSystemID, Keywords, IsDisabled, KeywordType, MatchType, Reply, AddDate, Taxis FROM wx_Keyword WHERE KeywordID = @KeywordID";

        private const string SQL_SELECT_AVALIABLE = "SELECT TOP 1 KeywordID, PublishmentSystemID, Keywords, IsDisabled, KeywordType, MatchType, Reply, AddDate, Taxis FROM wx_Keyword WHERE PublishmentSystemID = @PublishmentSystemID AND IsDisabled = @IsDisabled AND KeywordType = @KeywordType";

        private const string SQL_SELECT_KEYWRODS = "SELECT Keywords FROM wx_Keyword WHERE KeywordID = @KeywordID";

        private const string SQL_SELECT_ALL = "SELECT KeywordID, PublishmentSystemID, Keywords, IsDisabled, KeywordType, MatchType, Reply, AddDate, Taxis FROM wx_Keyword WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY Taxis DESC";

        private const string SQL_SELECT_ALL_BY_TYPE = "SELECT KeywordID, PublishmentSystemID, Keywords, IsDisabled, KeywordType, MatchType, Reply, AddDate, Taxis FROM wx_Keyword WHERE PublishmentSystemID = @PublishmentSystemID AND KeywordType = @KeywordType ORDER BY Taxis DESC";

        private const string PARM_KEYWORD_ID = "@KeywordID";
        private const string PARM_PUBLISHMENT_SYSTEM_ID = "@PublishmentSystemID";
        private const string PARM_KEYWORDS = "@Keywords";
        private const string PARM_IS_DISABLED = "@IsDisabled";
        private const string PARM_KEYWORD_TYPE = "@KeywordType";
        private const string PARM_MATCH_TYPE = "@MatchType";
        private const string PARM_REPLY = "@Reply";
        private const string PARM_ADD_DATE = "@AddDate";
        private const string PARM_TAXIS = "@Taxis";

        public int Insert(KeywordInfo keywordInfo)
        {
            var keywordID = 0;

            var sqlString = "INSERT INTO wx_Keyword (PublishmentSystemID, Keywords, IsDisabled, KeywordType, MatchType, Reply, AddDate, Taxis) VALUES (@PublishmentSystemID, @Keywords, @IsDisabled, @KeywordType, @MatchType, @Reply, @AddDate, @Taxis)";

            var taxis = GetMaxTaxis(keywordInfo.PublishmentSystemID, keywordInfo.KeywordType) + 1;
            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, keywordInfo.PublishmentSystemID),
                GetParameter(PARM_KEYWORDS, EDataType.NVarChar, 255, keywordInfo.Keywords),
                GetParameter(PARM_IS_DISABLED, EDataType.VarChar, 18, keywordInfo.IsDisabled.ToString()),
                GetParameter(PARM_KEYWORD_TYPE, EDataType.VarChar, 50, EKeywordTypeUtils.GetValue(keywordInfo.KeywordType)),
                GetParameter(PARM_MATCH_TYPE, EDataType.VarChar, 50, EMatchTypeUtils.GetValue(keywordInfo.MatchType)),
                GetParameter(PARM_REPLY, EDataType.NText, keywordInfo.Reply),
                GetParameter(PARM_ADD_DATE, EDataType.DateTime, keywordInfo.AddDate),
                GetParameter(PARM_TAXIS, EDataType.Integer, taxis)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, sqlString, parms);
                        keywordID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, "wx_Keyword");
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
                    DataProviderWX.KeywordMatchDAO.Insert(new KeywordMatchInfo(0, keywordInfo.PublishmentSystemID, keyword, keywordID, keywordInfo.IsDisabled, keywordInfo.KeywordType, keywordInfo.MatchType));
                }
            }

            return keywordID;
        }

        public void Update(KeywordInfo keywordInfo)
        {
            if (keywordInfo != null && keywordInfo.KeywordID > 0)
            {
                var parms = new IDataParameter[]
			    {
                    GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, keywordInfo.PublishmentSystemID),
                    GetParameter(PARM_KEYWORDS, EDataType.NVarChar, 255, keywordInfo.Keywords),
                    GetParameter(PARM_IS_DISABLED, EDataType.VarChar, 18, keywordInfo.IsDisabled.ToString()),
                    GetParameter(PARM_KEYWORD_TYPE, EDataType.VarChar, 50, EKeywordTypeUtils.GetValue(keywordInfo.KeywordType)),
                    GetParameter(PARM_MATCH_TYPE, EDataType.VarChar, 50, EMatchTypeUtils.GetValue(keywordInfo.MatchType)),
                    GetParameter(PARM_REPLY, EDataType.NText, keywordInfo.Reply),
                    GetParameter(PARM_ADD_DATE, EDataType.DateTime, keywordInfo.AddDate),
                    GetParameter(PARM_TAXIS, EDataType.Integer, keywordInfo.Taxis),
                    GetParameter(PARM_KEYWORD_ID, EDataType.Integer, keywordInfo.KeywordID)
			    };

                ExecuteNonQuery(SQL_UPDATE, parms);

                DataProviderWX.KeywordMatchDAO.DeleteByKeywordID(keywordInfo.KeywordID);

                foreach (var str in TranslateUtils.StringCollectionToStringList(keywordInfo.Keywords, ' '))
                {
                    var keyword = str.Trim();
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        DataProviderWX.KeywordMatchDAO.Insert(new KeywordMatchInfo(0, keywordInfo.PublishmentSystemID, keyword, keywordInfo.KeywordID, keywordInfo.IsDisabled, keywordInfo.KeywordType, keywordInfo.MatchType));
                    }
                }
            }
        }

        public void Update(int publishmentSystemID, int keywordID, EKeywordType keywordType, EMatchType matchType, string keywords, bool isDisabled)
        {
            var parms = new IDataParameter[]
			{
                GetParameter(PARM_KEYWORDS, EDataType.NVarChar, 255, keywords),
                GetParameter(PARM_IS_DISABLED, EDataType.VarChar, 18, isDisabled.ToString()),
                GetParameter(PARM_KEYWORD_ID, EDataType.Integer, keywordID)
			};

            ExecuteNonQuery(SQL_UPDATE_KEYWRODS_AND_ISDISABLED, parms);

            DataProviderWX.KeywordMatchDAO.DeleteByKeywordID(keywordID);

            foreach (var str in TranslateUtils.StringCollectionToStringList(keywords, ' '))
            {
                var keyword = str.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    DataProviderWX.KeywordMatchDAO.Insert(new KeywordMatchInfo(0, publishmentSystemID, keyword, keywordID, isDisabled, keywordType, matchType));
                }
            }
        }

        public void Update(int publishmentSystemID, int keywordID, EKeywordType keywordType, EMatchType matchType, string keywords)
        {
            var parms = new IDataParameter[]
			{
                GetParameter(PARM_KEYWORDS, EDataType.NVarChar, 255, keywords),
                GetParameter(PARM_KEYWORD_TYPE, EDataType.VarChar, 50, EKeywordTypeUtils.GetValue(keywordType)),
                GetParameter(PARM_KEYWORD_ID, EDataType.Integer, keywordID)
			};

            ExecuteNonQuery(SQL_UPDATE_KEYWRODS, parms);

            DataProviderWX.KeywordMatchDAO.DeleteByKeywordID(keywordID);

            foreach (var str in TranslateUtils.StringCollectionToStringList(keywords, ' '))
            {
                var keyword = str.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    DataProviderWX.KeywordMatchDAO.Insert(new KeywordMatchInfo(0, publishmentSystemID, keyword, keywordID, false, keywordType, matchType));
                }
            }
        }

        public void Delete(int keywordID)
        {
            if (keywordID > 0)
            {
                var parms = new IDataParameter[]
			    {
				    GetParameter(PARM_KEYWORD_ID, EDataType.Integer, keywordID)
			    };

                ExecuteNonQuery(SQL_DELETE, parms);
            }
        }

        public void Delete(List<int> keywordIDList)
        {
            if (keywordIDList != null && keywordIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM wx_Keyword WHERE KeywordID IN ({TranslateUtils.ToSqlInStringWithoutQuote(keywordIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public string GetKeywords(int keywordID)
        {
            var keywords = string.Empty;

            if (keywordID > 0)
            {
                var parms = new IDataParameter[]
			    {
				    GetParameter(PARM_KEYWORD_ID, EDataType.Integer, keywordID)
			    };

                using (var rdr = ExecuteReader(SQL_SELECT_KEYWRODS, parms))
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

        public KeywordInfo GetKeywordInfo(int keywordID)
        {
            KeywordInfo keywordInfo = null;

            if (keywordID > 0)
            {
                var parms = new IDataParameter[]
			    {
				    GetParameter(PARM_KEYWORD_ID, EDataType.Integer, keywordID)
			    };

                using (var rdr = ExecuteReader(SQL_SELECT, parms))
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
                keywordInfo.KeywordID = 0;
                keywordInfo.Keywords = "";
            }

            if (keywordInfo == null)
            {
                keywordInfo = new KeywordInfo();
                keywordInfo.KeywordID = 0;
                keywordInfo.Keywords = "";
            }
            return keywordInfo;
        }

        public int GetKeywordID(int publishmentSystemID, bool isExists, string keywords, EKeywordType keywordType, int existKeywordID)
        {
            var keywordID = existKeywordID;

            if (isExists)
            {
                if (!string.IsNullOrEmpty(keywords))
                {
                    if (existKeywordID > 0)
                    {
                        DataProviderWX.KeywordDAO.Update(publishmentSystemID, existKeywordID, keywordType, EMatchType.Exact, keywords);
                    }
                    else
                    {
                        var keywordInfo = new KeywordInfo(0, publishmentSystemID, keywords, false, keywordType, EMatchType.Exact, string.Empty, DateTime.Now, 0);
                        keywordID = DataProviderWX.KeywordDAO.Insert(keywordInfo);
                    }
                }
                else
                {
                    if (existKeywordID > 0)
                    {
                        DataProviderWX.KeywordDAO.Delete(existKeywordID);
                        keywordID = 0;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(keywords))
                {
                    var keywordInfo = new KeywordInfo(0, publishmentSystemID, keywords, false, keywordType, EMatchType.Exact, string.Empty, DateTime.Now, 0);
                    keywordID = DataProviderWX.KeywordDAO.Insert(keywordInfo);
                }
            }

            return keywordID;
        }

        public KeywordInfo GetAvaliableKeywordInfo(int publishmentSystemID, EKeywordType keywordType)
        {
            KeywordInfo keywordInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, publishmentSystemID),
                GetParameter(PARM_IS_DISABLED, EDataType.VarChar, 18, false.ToString()),
                GetParameter(PARM_KEYWORD_TYPE, EDataType.VarChar, 50, EKeywordTypeUtils.GetValue(keywordType))
			};

            using (var rdr = ExecuteReader(SQL_SELECT_AVALIABLE, parms))
            {
                if (rdr.Read())
                {
                    keywordInfo = new KeywordInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetValue(2).ToString(), TranslateUtils.ToBool(rdr.GetValue(3).ToString()), EKeywordTypeUtils.GetEnumType(rdr.GetValue(4).ToString()), EMatchTypeUtils.GetEnumType(rdr.GetValue(5).ToString()), rdr.GetValue(6).ToString(), rdr.GetDateTime(7), rdr.GetInt32(8));
                }
                rdr.Close();
            }

            return keywordInfo;
        }

        public IEnumerable GetDataSource(int publishmentSystemID, EKeywordType keywordType)
        {
            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, publishmentSystemID),
				GetParameter(PARM_KEYWORD_TYPE, EDataType.VarChar, 50, EKeywordTypeUtils.GetValue(keywordType))
			};

            var enumerable = (IEnumerable)ExecuteReader(SQL_SELECT_ALL_BY_TYPE, parms);
            return enumerable;
        }

        public int GetCount(int publishmentSystemID, EKeywordType keywordType)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM wx_Keyword WHERE PublishmentSystemID = {publishmentSystemID} AND KeywordType = '{EKeywordTypeUtils.GetValue(keywordType)}'";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<KeywordInfo> GetKeywordInfoList(int publishmentSystemID)
        {
            var list = new List<KeywordInfo>();

            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, publishmentSystemID)
			};

            using (var rdr = ExecuteReader(SQL_SELECT_ALL, parms))
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

        public List<KeywordInfo> GetKeywordInfoList(int publishmentSystemID, EKeywordType keywordType)
        {
            var list = new List<KeywordInfo>();

            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, publishmentSystemID),
				GetParameter(PARM_KEYWORD_TYPE, EDataType.VarChar, 50, EKeywordTypeUtils.GetValue(keywordType))
			};

            using (var rdr = ExecuteReader(SQL_SELECT_ALL_BY_TYPE, parms))
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

        public bool UpdateTaxisToUp(int publishmentSystemID, EKeywordType keywordType, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 KeywordID, Taxis FROM wx_Keyword WHERE (Taxis > (SELECT Taxis FROM wx_Keyword WHERE KeywordID = {keywordID})) AND PublishmentSystemID = {publishmentSystemID} AND KeywordType = '{EKeywordTypeUtils.GetValue(keywordType)}' ORDER BY Taxis";
            var higherID = 0;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherID = rdr.GetInt32(0);
                    higherTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(keywordID);

            if (higherID > 0)
            {
                SetTaxis(keywordID, higherTaxis);
                SetTaxis(higherID, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int publishmentSystemID, EKeywordType keywordType, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 KeywordID, Taxis FROM wx_Keyword WHERE (Taxis < (SELECT Taxis FROM wx_Keyword WHERE KeywordID = {keywordID})) AND PublishmentSystemID = {publishmentSystemID} AND KeywordType = '{EKeywordTypeUtils.GetValue(keywordType)}' ORDER BY Taxis DESC";
            var lowerID = 0;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerID = rdr.GetInt32(0);
                    lowerTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(keywordID);

            if (lowerID > 0)
            {
                SetTaxis(keywordID, lowerTaxis);
                SetTaxis(lowerID, selectedTaxis);
                return true;
            }
            return false;
        }

        private int GetMaxTaxis(int publishmentSystemID, EKeywordType keywordType)
        {
            string sqlString =
                $"SELECT MAX(Taxis) FROM wx_Keyword WHERE PublishmentSystemID = {publishmentSystemID} AND KeywordType = '{EKeywordTypeUtils.GetValue(keywordType)}'";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int keywordID)
        {
            string sqlString = $"SELECT Taxis FROM wx_Keyword WHERE KeywordID = {keywordID}";
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

        private void SetTaxis(int keywordID, int taxis)
        {
            string sqlString = $"UPDATE wx_Keyword SET Taxis = {taxis} WHERE KeywordID = {keywordID}";
            ExecuteNonQuery(sqlString);
        }


        public int GetKeywordsIDbyName(int publishmentSystemID, string keywords)
        {
            string sqlString =
                $"SELECT KeywordID FROM wx_Keyword WHERE Keywords = '{keywords}' AND PublishmentSystemID = {publishmentSystemID}";
            var keywordID = 0;
            if (!string.IsNullOrEmpty(keywords))
            {
                using (var rdr = ExecuteReader(sqlString))
                {
                    if (rdr.Read())
                    {
                        keywordID = Convert.ToInt32(rdr[0]);
                    }
                    rdr.Close();
                }
            }
            return keywordID;
        }
    }
}