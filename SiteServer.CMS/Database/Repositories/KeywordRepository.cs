using System.Collections.Generic;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Repositories
{
    public class KeywordRepository : GenericRepository<KeywordInfo>
    {
        private static class Attr
        {
            public const string Keyword = nameof(KeywordInfo.Keyword);
        }

        public void Insert(KeywordInfo keywordInfo)
        {
            //const string sqlString = "INSERT INTO siteserver_Keyword(Keyword, Alternative, Grade) VALUES(@Keyword, @Alternative, @Grade)";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamKeyword, keywordInfo.Keyword),
            //    GetParameter(ParamAlternative, keywordInfo.Alternative),
            //    GetParameter(ParamGrade, EKeywordGradeUtils.GetValueById(keywordInfo.Grade))
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            InsertObject(keywordInfo);
        }

        public int GetCount()
        {
            //var sqlString = "SELECT COUNT(*) AS TotalNum FROM siteserver_Keyword";
            //return DatabaseApi.Instance.GetIntResult(sqlString);

            return Count();
        }

        public void Update(KeywordInfo keywordInfo)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamKeyword, keywordInfo.Keyword),
            //    GetParameter(ParamAlternative, keywordInfo.Alternative),
            //    GetParameter(ParamGrade, EKeywordGradeUtils.GetValueById(keywordInfo.Grade)),
            //    GetParameter(ParamId, keywordInfo.Id)
            //};
            //string SqlUpdate = "UPDATE siteserver_Keyword SET Keyword = @Keyword, Alternative = @Alternative, Grade = @Grade WHERE Id=@Id";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);

            UpdateObject(keywordInfo);
        }

        public KeywordInfo Get(int id)
        {
            //var keywordInfo = new KeywordInfo();

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamId, id)
            //};
            //string SqlSelect = "SELECT Id, Keyword, Alternative, Grade FROM siteserver_Keyword WHERE Id=@Id";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelect, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        var i = 0;
            //        keywordInfo = new KeywordInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), EKeywordGradeUtils.GetEnumType(DatabaseApi.GetString(rdr, i)));
            //    }
            //    rdr.Close();
            //}
            //return keywordInfo;

            return GetObjectById(id);
        }

        public void Delete(int id)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamId, id)
            //};
            //string SqlDelete = "DELETE FROM siteserver_Keyword WHERE Id = @Id";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDelete, parameters);

            DeleteById(id);
        }

        public string GetSelectCommand()
        {
            string SqlSelectAll = "SELECT Id, Keyword, Alternative, Grade FROM siteserver_Keyword";
            return SqlSelectAll;
        }

        public bool IsExists(string keyword)
        {
            //var isExists = false;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamKeyword, keyword)
            //};
            //string SqlSelectKeyword = "SELECT Id, Keyword, Alternative, Grade FROM siteserver_Keyword WHERE Keyword = @Keyword";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectKeyword, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        isExists = true;
            //    }
            //    rdr.Close();
            //}
            //return isExists;

            return Exists(Q.Where(Attr.Keyword, keyword));
        }

        public IList<KeywordInfo> GetKeywordInfoList()
        {
            //var list = new List<KeywordInfo>();
            //string SqlSelectAll = "SELECT Id, Keyword, Alternative, Grade FROM siteserver_Keyword";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAll))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var keywordInfo = new KeywordInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), EKeywordGradeUtils.GetEnumType(DatabaseApi.GetString(rdr, i)));
            //        list.Add(keywordInfo);
            //    }
            //    rdr.Close();
            //}
            //return list;

            return GetObjectList();
        }

        public IList<KeywordInfo> GetKeywordInfoList(IList<string> keywords)
        {
            if (keywords == null || keywords.Count == 0) return new List<KeywordInfo>();

            //var list = new List<KeywordInfo>();
            //string sqlSelectKeywords =
            //    $"SELECT Id, Keyword, Alternative, Grade FROM siteserver_Keyword WHERE Keyword IN ({TranslateUtils.ToSqlInStringWithQuote(keywords)})";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlSelectKeywords))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var keywordInfo = new KeywordInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), EKeywordGradeUtils.GetEnumType(DatabaseApi.GetString(rdr, i)));
            //        list.Add(keywordInfo);
            //    }
            //    rdr.Close();
            //}
            //return list;

            return GetObjectList(Q
                .WhereIn(Attr.Keyword, keywords));
        }

        //todo: 实现INSTR函数
        public IList<string> GetKeywordListByContent(string content)
        {
            //var sqlString = $"SELECT Keyword FROM siteserver_Keyword WHERE {SqlUtils.GetInStrReverse(AttackUtils.FilterSql(content), nameof(KeywordInfo.Keyword))}";
            //return DatabaseApi.Instance.GetStringList(sqlString);

            return GetValueList<string>(Q
                .Select(Attr.Keyword)
                .WhereContains(Attr.Keyword, content));
        }

        //public GenericQuery InStr(string name, string value, bool reverse = false)
        //{
        //    var paramKey = GetParameterKey(name);

        //    var retVal = string.Empty;
        //    if (reverse)
        //    {
        //        if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
        //        {
        //            retVal = $"INSTR(@{paramKey}, {name}) > 0";
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
        //        {
        //            retVal = $"CHARINDEX({name}, @{paramKey}) > 0";
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
        //        {
        //            retVal = $"POSITION({name} IN @{paramKey}) > 0";
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
        //        {
        //            retVal = $"INSTR(@{paramKey}, {name}) > 0";
        //        }
        //    }
        //    else
        //    {
        //        if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
        //        {
        //            retVal = $"INSTR({name}, @{paramKey}) > 0";
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
        //        {
        //            retVal = $"CHARINDEX(@{paramKey}, {name}) > 0";
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
        //        {
        //            retVal = $"POSITION(@{paramKey} IN {name}) > 0";
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
        //        {
        //            retVal = $"INSTR({name} @{paramKey}) > 0";
        //        }
        //    }

        //    _whereList.Add(retVal);
        //    SqlParameters.Add(paramKey, value);

        //    return this;
        //}

        //public GenericQuery NotInStr(string name, string value, bool reverse = false)
        //{
        //    var paramKey = GetParameterKey(name);

        //    var retVal = string.Empty;
        //    if (reverse)
        //    {
        //        if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
        //        {
        //            retVal = $"INSTR(@{paramKey}, {name}) = 0";
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
        //        {
        //            retVal = $"CHARINDEX({name}, @{paramKey}) = 0";
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
        //        {
        //            retVal = $"POSITION({name} IN @{paramKey}) = 0";
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
        //        {
        //            retVal = $"INSTR(@{paramKey}, {name}) = 0";
        //        }
        //    }
        //    else
        //    {
        //        if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
        //        {
        //            retVal = $"INSTR({name}, @{paramKey}) = 0";
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
        //        {
        //            retVal = $"CHARINDEX(@{paramKey}, {name}) = 0";
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
        //        {
        //            retVal = $"POSITION(@{paramKey} IN {name}) = 0";
        //        }
        //        else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
        //        {
        //            retVal = $"INSTR({name}, @{paramKey}) = 0";
        //        }
        //    }

        //    _whereList.Add(retVal);
        //    SqlParameters.Add(paramKey, value);

        //    return this;
        //}
    }
}


//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Core.Enumerations;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class Keyword : DataProviderBase
//    {
//        public override string TableName => "siteserver_Keyword";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(KeywordInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(KeywordInfo.Keyword),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(KeywordInfo.Alternative),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(KeywordInfo.Grade),
//                DataType = DataType.VarChar
//            }
//        };

//        private const string ParamId = "@Id";
//        private const string ParamKeyword = "@Keyword";
//        private const string ParamAlternative = "@Alternative";
//        private const string ParamGrade = "@Grade";

//        private const string SqlUpdate = "UPDATE siteserver_Keyword SET Keyword = @Keyword, Alternative = @Alternative, Grade = @Grade WHERE Id=@Id";

//        private const string SqlDelete = "DELETE FROM siteserver_Keyword WHERE Id = @Id";

//        private const string SqlSelect = "SELECT Id, Keyword, Alternative, Grade FROM siteserver_Keyword WHERE Id=@Id";

//        private const string SqlSelectAll = "SELECT Id, Keyword, Alternative, Grade FROM siteserver_Keyword";

//        private const string SqlSelectKeyword = "SELECT Id, Keyword, Alternative, Grade FROM siteserver_Keyword WHERE Keyword = @Keyword";

//        public void InsertObject(KeywordInfo keywordInfo)
//        {
//            const string sqlString = "INSERT INTO siteserver_Keyword(Keyword, Alternative, Grade) VALUES(@Keyword, @Alternative, @Grade)";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamKeyword, keywordInfo.Keyword),
//                GetParameter(ParamAlternative, keywordInfo.Alternative),
//                GetParameter(ParamGrade, EKeywordGradeUtils.GetValueById(keywordInfo.Grade))
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//        }

//        public int GetCount()
//        {
//            var sqlString = "SELECT COUNT(*) AS TotalNum FROM siteserver_Keyword";
//            return DatabaseApi.Instance.GetIntResult(sqlString);
//        }

//        public void UpdateObject(KeywordInfo keywordInfo)
//        {
//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamKeyword, keywordInfo.Keyword),
//                GetParameter(ParamAlternative, keywordInfo.Alternative),
//                GetParameter(ParamGrade, EKeywordGradeUtils.GetValueById(keywordInfo.Grade)),
//                GetParameter(ParamId, keywordInfo.Id)
//            };
//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);
//        }

//        public KeywordInfo GetKeywordInfo(int id)
//        {
//            var keywordInfo = new KeywordInfo();

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamId, id)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelect, parameters))
//            {
//                if (rdr.Read())
//                {
//                    var i = 0;
//                    keywordInfo = new KeywordInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), EKeywordGradeUtils.GetEnumType(DatabaseApi.GetString(rdr, i)));
//                }
//                rdr.Close();
//            }
//            return keywordInfo;
//        }

//        public void DeleteById(int id)
//        {
//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamId, id)
//            };
//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDelete, parameters);
//        }

//        public void DeleteById(List<int> idList)
//        {
//            string sqlString =
//                $@"DELETE FROM siteserver_Keyword WHERE Id IN ({TranslateUtils.ObjectCollectionToString(idList)})";
//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//        }

//        public string GetSelectCommand()
//        {
//            return SqlSelectAll;
//        }

//        public bool IsExists(string keyword)
//        {
//            var isExists = false;

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamKeyword, keyword)
//            };
//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectKeyword, parameters))
//            {
//                if (rdr.Read())
//                {
//                    isExists = true;
//                }
//                rdr.Close();
//            }
//            return isExists;
//        }

//        public List<KeywordInfo> GetKeywordInfoList()
//        {
//            var list = new List<KeywordInfo>();
//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAll))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var keywordInfo = new KeywordInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), EKeywordGradeUtils.GetEnumType(DatabaseApi.GetString(rdr, i)));
//                    list.Add(keywordInfo);
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        public List<KeywordInfo> GetKeywordInfoList(List<string> keywords)
//        {
//            if (keywords == null || keywords.Count == 0) return new List<KeywordInfo>();

//            var list = new List<KeywordInfo>();
//            string sqlSelectKeywords =
//                $"SELECT Id, Keyword, Alternative, Grade FROM siteserver_Keyword WHERE Keyword IN ({TranslateUtils.ToSqlInStringWithQuote(keywords)})";
//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlSelectKeywords))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var keywordInfo = new KeywordInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), EKeywordGradeUtils.GetEnumType(DatabaseApi.GetString(rdr, i)));
//                    list.Add(keywordInfo);
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        public List<string> GetKeywordListByContent(string content)
//        {
//            var sqlString = $"SELECT Keyword FROM siteserver_Keyword WHERE {SqlUtils.GetInStrReverse(AttackUtils.FilterSql(content), nameof(KeywordInfo.Keyword))}";
//            return DatabaseApi.Instance.GetStringList(sqlString);
//        }
//    }
//}
