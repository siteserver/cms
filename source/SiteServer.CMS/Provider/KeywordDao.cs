using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class KeywordDao : DataProviderBase
    {
        private const string ParmKeywordId = "@KeywordID";
        private const string ParmKeyword = "@Keyword";
        private const string ParmAlternative = "@Alternative";
        private const string ParmGrade = "@Grade";

        private const string SqlUpdate = "UPDATE siteserver_Keyword SET Keyword=@Keyword,Alternative=@Alternative,Grade=@Grade WHERE KeywordID=@KeywordID";

        private const string SqlDelete = "DELETE FROM siteserver_Keyword WHERE KeywordID=@KeywordID";

        private const string SqlSelect = "SELECT KeywordID,Keyword,Alternative,Grade FROM siteserver_Keyword WHERE KeywordID=@KeywordID";

        private const string SqlSelectAll = "SELECT KeywordID,Keyword,Alternative,Grade FROM siteserver_Keyword";

        private const string SqlSelectKeyword = "SELECT KeywordID,Keyword,Alternative,Grade FROM siteserver_Keyword WHERE Keyword = @Keyword";

        public void Insert(KeywordInfo keywordInfo)
        {
            var sqlString = "INSERT INTO siteserver_Keyword(Keyword,Alternative,Grade) VALUES(@Keyword,@Alternative,@Grade)";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmKeyword, EDataType.NVarChar,50, keywordInfo.Keyword),
                GetParameter(ParmAlternative, EDataType.NVarChar,50, keywordInfo.Alternative),
                GetParameter(ParmGrade, EDataType.NVarChar, 50, EKeywordGradeUtils.GetValue(keywordInfo.Grade))
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public int GetCount()
        {
            var sqlString = "SELECT COUNT(*) AS TotalNum FROM siteserver_Keyword";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public void Update(KeywordInfo keywordInfo)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmKeyword, EDataType.NVarChar,50, keywordInfo.Keyword),
                GetParameter(ParmAlternative, EDataType.NVarChar,50, keywordInfo.Alternative),
                GetParameter(ParmGrade, EDataType.NVarChar, 50, EKeywordGradeUtils.GetValue(keywordInfo.Grade)),
                GetParameter(ParmKeywordId, EDataType.Integer, keywordInfo.KeywordID)
            };
            ExecuteNonQuery(SqlUpdate, parms);
        }

        public KeywordInfo GetKeywordInfo(int keywordId)
        {
            var keywordInfo = new KeywordInfo();

            var parms = new IDataParameter[]
            {
                GetParameter(ParmKeywordId, EDataType.Integer, keywordId)
            };

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    keywordInfo = new KeywordInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), EKeywordGradeUtils.GetEnumType(GetString(rdr, i)));
                }
                rdr.Close();
            }
            return keywordInfo;
        }

        public void Delete(int keywordId)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmKeywordId, EDataType.Integer, keywordId)
            };
            ExecuteNonQuery(SqlDelete, parms);
        }

        public void Delete(ArrayList idArrayList)
        {
            string sqlString =
                $@"DELETE FROM siteserver_Keyword WHERE KeywordID IN ({TranslateUtils.ObjectCollectionToString(
                    idArrayList)})";
            ExecuteNonQuery(sqlString);
        }

        public string GetSelectCommand()
        {
            return SqlSelectAll;
        }

        public bool IsExists(string keyword)
        {
            var isExists = false;

            var parms = new IDataParameter[]
            {
                GetParameter(ParmKeyword, EDataType.NVarChar, 50, keyword)
            };
            using (var rdr = ExecuteReader(SqlSelectKeyword, parms))
            {
                if (rdr.Read())
                {
                    isExists = true;
                }
                rdr.Close();
            }
            return isExists;
        }

        public List<KeywordInfo> GetKeywordInfoList()
        {
            var list = new List<KeywordInfo>();
            using (var rdr = ExecuteReader(SqlSelectAll))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var keywordInfo = new KeywordInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), EKeywordGradeUtils.GetEnumType(GetString(rdr, i)));
                    list.Add(keywordInfo);
                }
                rdr.Close();
            }
            return list;
        }

        public List<KeywordInfo> GetKeywordInfoList(List<string> keywords)
        {
            if (keywords == null || keywords.Count == 0) return new List<KeywordInfo>();

            var list = new List<KeywordInfo>();
            string sqlSelectKeywords =
                $"SELECT KeywordID,Keyword,Alternative,Grade FROM siteserver_Keyword WHERE Keyword in ({TranslateUtils.ToSqlInStringWithQuote(keywords)})";
            using (var rdr = ExecuteReader(sqlSelectKeywords))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var keywordInfo = new KeywordInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), EKeywordGradeUtils.GetEnumType(GetString(rdr, i)));
                    list.Add(keywordInfo);
                }
                rdr.Close();
            }
            return list;
        }

        public List<string> GetKeywordListByContent(string content)
        {
            //string sqlString =
            //    $"SELECT Keyword FROM siteserver_Keyword WHERE CHARINDEX(Keyword, '{PageUtils.FilterSql(content)}') > 0";

            var inStr = WebConfigUtils.IsMySql ? $"INSTR('{PageUtils.FilterSql(content)}', Keyword) > 0" : $"CHARINDEX(Keyword, '{PageUtils.FilterSql(content)}') > 0";
            var sqlString = $"SELECT Keyword FROM siteserver_Keyword WHERE {inStr}";
            return BaiRongDataProvider.DatabaseDao.GetStringList(sqlString);
        }
    }
}
