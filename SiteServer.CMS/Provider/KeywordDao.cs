using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin;

namespace SiteServer.CMS.Provider
{
    public class KeywordDao : DataProviderBase
    {
        public override string TableName => "siteserver_Keyword";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(KeywordInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(KeywordInfo.Keyword),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(KeywordInfo.Alternative),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(KeywordInfo.Grade),
                DataType = DataType.VarChar,
                DataLength = 50
            }
        };

        private const string ParmId = "@Id";
        private const string ParmKeyword = "@Keyword";
        private const string ParmAlternative = "@Alternative";
        private const string ParmGrade = "@Grade";

        private const string SqlUpdate = "UPDATE siteserver_Keyword SET Keyword=@Keyword, Alternative=@Alternative, Grade=@Grade WHERE Id=@Id";

        private const string SqlDelete = "DELETE FROM siteserver_Keyword WHERE Id=@Id";

        private const string SqlSelect = "SELECT Id, Keyword, Alternative, Grade FROM siteserver_Keyword WHERE Id=@Id";

        private const string SqlSelectAll = "SELECT Id, Keyword, Alternative, Grade FROM siteserver_Keyword";

        private const string SqlSelectKeyword = "SELECT Id, Keyword, Alternative, Grade FROM siteserver_Keyword WHERE Keyword = @Keyword";

        public void Insert(KeywordInfo keywordInfo)
        {
            var sqlString = "INSERT INTO siteserver_Keyword(Keyword, Alternative, Grade) VALUES(@Keyword, @Alternative, @Grade)";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmKeyword, DataType.VarChar,50, keywordInfo.Keyword),
                GetParameter(ParmAlternative, DataType.VarChar,50, keywordInfo.Alternative),
                GetParameter(ParmGrade, DataType.VarChar, 50, EKeywordGradeUtils.GetValue(keywordInfo.Grade))
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public int GetCount()
        {
            var sqlString = "SELECT COUNT(*) AS TotalNum FROM siteserver_Keyword";
            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public void Update(KeywordInfo keywordInfo)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmKeyword, DataType.VarChar,50, keywordInfo.Keyword),
                GetParameter(ParmAlternative, DataType.VarChar,50, keywordInfo.Alternative),
                GetParameter(ParmGrade, DataType.VarChar, 50, EKeywordGradeUtils.GetValue(keywordInfo.Grade)),
                GetParameter(ParmId, DataType.Integer, keywordInfo.Id)
            };
            ExecuteNonQuery(SqlUpdate, parms);
        }

        public KeywordInfo GetKeywordInfo(int id)
        {
            var keywordInfo = new KeywordInfo();

            var parms = new IDataParameter[]
            {
                GetParameter(ParmId, DataType.Integer, id)
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

        public void Delete(int id)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmId, DataType.Integer, id)
            };
            ExecuteNonQuery(SqlDelete, parms);
        }

        public void Delete(List<int> idList)
        {
            string sqlString =
                $@"DELETE FROM siteserver_Keyword WHERE Id IN ({TranslateUtils.ObjectCollectionToString(idList)})";
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
                GetParameter(ParmKeyword, DataType.VarChar, 50, keyword)
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
                $"SELECT Id, Keyword, Alternative, Grade FROM siteserver_Keyword WHERE Keyword IN ({TranslateUtils.ToSqlInStringWithQuote(keywords)})";
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
            var sqlString = $"SELECT Keyword FROM siteserver_Keyword WHERE {SqlUtils.GetInStrReverse(AttackUtils.FilterSql(content), nameof(KeywordInfo.Keyword))}";
            return DataProvider.DatabaseDao.GetStringList(sqlString);
        }
    }
}
