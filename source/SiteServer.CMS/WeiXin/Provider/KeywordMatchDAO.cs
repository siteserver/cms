using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class KeywordMatchDao : DataProviderBase
    {
        private const string TableName = "wx_KeywordMatch";

        private const string SqlDeleteByKeywordId = "DELETE FROM wx_KeywordMatch WHERE KeywordID = @KeywordID";

        private const string SqlSelectKeyowrdByType = "SELECT Keyword FROM wx_KeywordMatch WHERE PublishmentSystemID = @PublishmentSystemID AND KeywordType = @KeywordType ORDER BY Keyword";

        private const string SqlSelectKeyowrdEnabled = "SELECT Keyword FROM wx_KeywordMatch WHERE PublishmentSystemID = @PublishmentSystemID AND IsDisabled = @IsDisabled ORDER BY Keyword";

        private const string ParmMatchId = "@MatchID";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmKeyword = "@Keyword";
        private const string ParmKeywordId = "@KeywordID";
        private const string ParmIsDisabled = "@IsDisabled";
        private const string ParmKeywordType = "@KeywordType";
        private const string ParmMatchType = "@MatchType";

        public void Insert(KeywordMatchInfo matchInfo)
        {
            var sqlString = "INSERT INTO wx_KeywordMatch (PublishmentSystemID, Keyword, KeywordID, IsDisabled, KeywordType, MatchType) VALUES (@PublishmentSystemID, @Keyword, @KeywordID, @IsDisabled, @KeywordType, @MatchType)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, matchInfo.PublishmentSystemId),
                GetParameter(ParmKeyword, DataType.NVarChar, 255, matchInfo.Keyword),
                GetParameter(ParmKeywordId, DataType.Integer, matchInfo.KeywordId),
                GetParameter(ParmIsDisabled, DataType.VarChar, 18, matchInfo.IsDisabled.ToString()),
                GetParameter(ParmKeywordType, DataType.VarChar, 50, EKeywordTypeUtils.GetValue(matchInfo.KeywordType)),
                GetParameter(ParmMatchType, DataType.VarChar, 50, EMatchTypeUtils.GetValue(matchInfo.MatchType))
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public void DeleteByKeywordId(int keywordId)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmKeywordId, DataType.Integer, keywordId)
			};

            ExecuteNonQuery(SqlDeleteByKeywordId, parms);
        }

        public List<string> GetKeywordList(int publishmentSystemId, EKeywordType keywordType)
        {
            var list = new List<string>();

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
                GetParameter(ParmKeywordType, DataType.VarChar, 50, EKeywordTypeUtils.GetValue(keywordType))
			};

            using (var rdr = ExecuteReader(SqlSelectKeyowrdByType, parms))
            {
                while (rdr.Read())
                {
                    list.Add(rdr.GetValue(0).ToString());
                }
                rdr.Close();
            }

            return list;
        }

        public List<string> GetKeywordListEnabled(int publishmentSystemId)
        {
            var list = new List<string>();

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
                GetParameter(ParmIsDisabled, DataType.VarChar, 18, false.ToString())
			};

            using (var rdr = ExecuteReader(SqlSelectKeyowrdEnabled, parms))
            {
                while (rdr.Read())
                {
                    list.Add(rdr.GetValue(0).ToString());
                }
                rdr.Close();
            }

            return list;
        }

        public int GetKeywordIdbyMpController(int publishmentSystemId, string keyword)
        {
            var keywordId = 0;

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim();

                #region 张浩然 2014-8-25 修改关键词触发规则（全字匹配优先）
                string sqlString =
                    $@"SELECT TOP 1 KeywordID FROM ( SELECT wx_KeywordMatch.*,'Exact' AS TypeName FROM wx_KeywordMatch WHERE PublishmentSystemID ={publishmentSystemId} AND IsDisabled <> 'True' AND MatchType = '{EMatchTypeUtils
                        .GetValue(EMatchType.Exact)}' AND Keyword = '{keyword}' union SELECT wx_KeywordMatch.*,'Contains' AS TypeName FROM wx_KeywordMatch WHERE PublishmentSystemID ={publishmentSystemId} AND IsDisabled <> 'True' AND MatchType = '{EMatchTypeUtils
                        .GetValue(EMatchType.Contains)}' AND CHARINDEX(Keyword, '{keyword}') <> 0 ) AS wx_KM";
                #endregion

                using (var rdr = ExecuteReader(sqlString))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        keywordId = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }
            }

            return keywordId;
        }

        public string GetSelectString(int publishmentSystemId)
        {
            string whereString = $"WHERE PublishmentSystemID = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString("wx_KeywordMatch", SqlUtils.Asterisk, whereString);
        }

        public string GetSelectString(int publishmentSystemId, string keywordType, string keyword)
        {
            string whereString = $"WHERE PublishmentSystemID = {publishmentSystemId}";
            if (!string.IsNullOrEmpty(keywordType))
            {
                whereString += $" AND KeywordType = '{keywordType}'";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                whereString += $" AND Keyword LIKE '%{keyword}%'";
            }
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString("wx_KeywordMatch", SqlUtils.Asterisk, whereString);
        }

        public string GetSortField()
        {
            return "MatchID";
        }

        public bool IsExists(int publishmentSystemId, string keyword)
        {
            var isExists = false;

            string sqlString =
                $"SELECT MatchID FROM wx_KeywordMatch WHERE PublishmentSystemID = {publishmentSystemId} AND Keyword = '{keyword}'";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    isExists = true;
                }
                rdr.Close();
            }
            return isExists;
        }

        public List<KeywordMatchInfo> GetKeywordMatchInfoList(int publishmentSystemId, int keyWordId)
        {

            var keywordMatchInfoList = new List<KeywordMatchInfo>();

            string sqlWhere =
                $"WHERE {KeywordMatchAttribute.PublishmentSystemId} = {publishmentSystemId} AND {KeywordMatchAttribute.KeywordId} = {keyWordId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var keywordMatchInfo = new KeywordMatchInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetValue(2).ToString(), rdr.GetInt32(3), TranslateUtils.ToBool(rdr.GetValue(6).ToString()), EKeywordTypeUtils.GetEnumType(rdr.GetValue(4).ToString()), EMatchTypeUtils.GetEnumType(rdr.GetValue(5).ToString()));
                    keywordMatchInfoList.Add(keywordMatchInfo);
                }
                rdr.Close();
            }

            return keywordMatchInfoList;
        }
    }
}