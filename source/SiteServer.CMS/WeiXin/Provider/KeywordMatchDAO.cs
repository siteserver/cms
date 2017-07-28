using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class KeywordMatchDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_KeywordMatch";

        private const string SQL_DELETE_BY_KEYWORD_ID = "DELETE FROM wx_KeywordMatch WHERE KeywordID = @KeywordID";

        private const string SQL_SELECT_KEYOWRD_BY_TYPE = "SELECT Keyword FROM wx_KeywordMatch WHERE PublishmentSystemID = @PublishmentSystemID AND KeywordType = @KeywordType ORDER BY Keyword";

        private const string SQL_SELECT_KEYOWRD_ENABLED = "SELECT Keyword FROM wx_KeywordMatch WHERE PublishmentSystemID = @PublishmentSystemID AND IsDisabled = @IsDisabled ORDER BY Keyword";

        private const string PARM_MATCH_ID = "@MatchID";
        private const string PARM_PUBLISHMENT_SYSTEM_ID = "@PublishmentSystemID";
        private const string PARM_KEYWORD = "@Keyword";
        private const string PARM_KEYWORD_ID = "@KeywordID";
        private const string PARM_IS_DISABLED = "@IsDisabled";
        private const string PARM_KEYWORD_TYPE = "@KeywordType";
        private const string PARM_MATCH_TYPE = "@MatchType";

        public void Insert(KeywordMatchInfo matchInfo)
        {
            var sqlString = "INSERT INTO wx_KeywordMatch (PublishmentSystemID, Keyword, KeywordID, IsDisabled, KeywordType, MatchType) VALUES (@PublishmentSystemID, @Keyword, @KeywordID, @IsDisabled, @KeywordType, @MatchType)";

            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, matchInfo.PublishmentSystemID),
                GetParameter(PARM_KEYWORD, EDataType.NVarChar, 255, matchInfo.Keyword),
                GetParameter(PARM_KEYWORD_ID, EDataType.Integer, matchInfo.KeywordID),
                GetParameter(PARM_IS_DISABLED, EDataType.VarChar, 18, matchInfo.IsDisabled.ToString()),
                GetParameter(PARM_KEYWORD_TYPE, EDataType.VarChar, 50, EKeywordTypeUtils.GetValue(matchInfo.KeywordType)),
                GetParameter(PARM_MATCH_TYPE, EDataType.VarChar, 50, EMatchTypeUtils.GetValue(matchInfo.MatchType))
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public void DeleteByKeywordID(int keywordID)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(PARM_KEYWORD_ID, EDataType.Integer, keywordID)
			};

            ExecuteNonQuery(SQL_DELETE_BY_KEYWORD_ID, parms);
        }

        public List<string> GetKeywordList(int publishmentSystemID, EKeywordType keywordType)
        {
            var list = new List<string>();

            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, publishmentSystemID),
                GetParameter(PARM_KEYWORD_TYPE, EDataType.VarChar, 50, EKeywordTypeUtils.GetValue(keywordType))
			};

            using (var rdr = ExecuteReader(SQL_SELECT_KEYOWRD_BY_TYPE, parms))
            {
                while (rdr.Read())
                {
                    list.Add(rdr.GetValue(0).ToString());
                }
                rdr.Close();
            }

            return list;
        }

        public List<string> GetKeywordListEnabled(int publishmentSystemID)
        {
            var list = new List<string>();

            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, publishmentSystemID),
                GetParameter(PARM_IS_DISABLED, EDataType.VarChar, 18, false.ToString())
			};

            using (var rdr = ExecuteReader(SQL_SELECT_KEYOWRD_ENABLED, parms))
            {
                while (rdr.Read())
                {
                    list.Add(rdr.GetValue(0).ToString());
                }
                rdr.Close();
            }

            return list;
        }

        public int GetKeywordIDByMPController(int publishmentSystemID, string keyword)
        {
            var keywordID = 0;

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim();

                #region 张浩然 2014-8-25 修改关键词触发规则（全字匹配优先）
                string sqlString =
                    $@"SELECT TOP 1 KeywordID FROM ( SELECT wx_KeywordMatch.*,'Exact' AS TypeName FROM wx_KeywordMatch WHERE PublishmentSystemID ={publishmentSystemID} AND IsDisabled <> 'True' AND MatchType = '{EMatchTypeUtils
                        .GetValue(EMatchType.Exact)}' AND Keyword = '{keyword}' union SELECT wx_KeywordMatch.*,'Contains' AS TypeName FROM wx_KeywordMatch WHERE PublishmentSystemID ={publishmentSystemID} AND IsDisabled <> 'True' AND MatchType = '{EMatchTypeUtils
                        .GetValue(EMatchType.Contains)}' AND CHARINDEX(Keyword, '{keyword}') <> 0 ) AS wx_KM";
                #endregion

                using (var rdr = ExecuteReader(sqlString))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        keywordID = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }
            }

            return keywordID;
        }

        public string GetSelectString(int publishmentSystemID)
        {
            string whereString = $"WHERE PublishmentSystemID = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString("wx_KeywordMatch", SqlUtils.Asterisk, whereString);
        }

        public string GetSelectString(int publishmentSystemID, string keywordType, string keyword)
        {
            string whereString = $"WHERE PublishmentSystemID = {publishmentSystemID}";
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

        public bool IsExists(int publishmentSystemID, string keyword)
        {
            var isExists = false;

            string sqlString =
                $"SELECT MatchID FROM wx_KeywordMatch WHERE PublishmentSystemID = {publishmentSystemID} AND Keyword = '{keyword}'";

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

        public List<KeywordMatchInfo> GetKeywordMatchInfoList(int publishmentSystemID, int keyWordID)
        {

            var keywordMatchInfoList = new List<KeywordMatchInfo>();

            string SQL_WHERE =
                $"WHERE {KeywordMatchAttribute.PublishmentSystemID} = {publishmentSystemID} AND {KeywordMatchAttribute.KeywordID} = {keyWordID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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