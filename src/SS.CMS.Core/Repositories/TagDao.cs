using System.Collections.Generic;
using System.Text;
using SqlKata;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class TagDao : IDatabaseDao
    {
        private readonly Repository<TagInfo> _repository;
        public TagDao()
        {
            _repository = new Repository<TagInfo>(AppSettings.DatabaseType, AppSettings.ConnectionString);
        }

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string SiteId = nameof(TagInfo.SiteId);
            public const string ContentIdCollection = nameof(TagInfo.ContentIdCollection);
            public const string Tag = nameof(TagInfo.Tag);
            public const string UseNum = nameof(TagInfo.UseNum);
        }

        public void Insert(TagInfo tagInfo)
        {
            _repository.Insert(tagInfo);
        }

        public void Update(TagInfo tagInfo)
        {
            _repository.Update(tagInfo);
        }

        public TagInfo GetTagInfo(int siteId, string tag)
        {
            return _repository.Get(Q.Where(Attr.SiteId, siteId).Where(Attr.Tag, tag));
        }

        public IList<TagInfo> GetTagInfoList(int siteId, int contentId)
        {
            var query = GetQuery(null, siteId, contentId);
            return _repository.GetAll(query);
        }

        public string GetSqlString(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var whereString = GetWhereString(null, siteId, contentId);
            var orderString = string.Empty;
            if (isOrderByCount)
            {
                orderString = "ORDER BY UseNum DESC";
            }

            return DatabaseUtils.GetPageSqlString("siteserver_Tag", string.Join(",", new string[]
            {
                nameof(TagInfo.Id),
                nameof(TagInfo.SiteId),
                nameof(TagInfo.ContentIdCollection),
                nameof(TagInfo.Tag),
                nameof(TagInfo.UseNum)
            }), whereString, orderString, 0, totalNum);
        }

        public IList<TagInfo> GetTagInfoList(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var query = GetQuery(null, siteId, contentId).Limit(totalNum);
            if (isOrderByCount)
            {
                query.OrderByDesc(Attr.UseNum);
            }

            return _repository.GetAll(query);
        }

        public IList<string> GetTagListByStartString(int siteId, string startString, int totalNum)
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.Tag)
                .Where(Attr.SiteId, siteId)
                .WhereContains(Attr.Tag, startString)
                .OrderByDesc(Attr.UseNum)
                .Distinct()
                .Limit(totalNum));
        }

        public IList<string> GetTagList(int siteId)
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.Tag)
                .Where(Attr.SiteId, siteId)
                .Distinct()
                .OrderByDesc(Attr.UseNum));
        }

        public void DeleteTags(int siteId)
        {
            var query = GetQuery(null, siteId, 0);
            _repository.Delete(query);
        }

        public void DeleteTag(string tag, int siteId)
        {
            var query = GetQuery(tag, siteId, 0);
            _repository.Delete(query);
        }

        public int GetTagCount(string tag, int siteId)
        {
            var contentIdList = GetContentIdListByTag(tag, siteId);
            return contentIdList.Count;
        }

        private string GetWhereString(string tag, int siteId, int contentId)
        {
            var builder = new StringBuilder();
            builder.Append($" WHERE SiteId = {siteId} ");
            if (!string.IsNullOrEmpty(tag))
            {
                builder.Append($"AND Tag = '{AttackUtils.FilterSql(tag)}' ");
            }
            if (contentId > 0)
            {
                builder.Append(
                    $"AND (ContentIdCollection = '{contentId}' OR ContentIdCollection LIKE '{contentId},%' OR ContentIdCollection LIKE '%,{contentId},%' OR ContentIdCollection LIKE '%,{contentId}')");
            }

            return builder.ToString();
        }

        private Query GetQuery(string tag, int siteId, int contentId)
        {
            var query = Q.Where(Attr.SiteId, siteId);
            if (!string.IsNullOrEmpty(tag))
            {
                query.Where(Attr.Tag, tag);
            }
            if (contentId > 0)
            {
                query.Where(q => q
                    .Where(Attr.ContentIdCollection, contentId.ToString())
                    .OrWhereStarts(Attr.ContentIdCollection, $"{contentId},")
                    .OrWhereContains(Attr.ContentIdCollection, $",{contentId},")
                    .OrWhereEnds(Attr.ContentIdCollection, $",{contentId}"));
            }

            return query;
        }

        public List<int> GetContentIdListByTag(string tag, int siteId)
        {
            var idList = new List<int>();
            if (string.IsNullOrEmpty(tag)) return idList;

            var query = GetQuery(tag, siteId, 0);
            var contentIdCollectionList = _repository.GetAll<string>(query.Select(Attr.ContentIdCollection));
            foreach (var contentIdCollection in contentIdCollectionList)
            {
                var contentIdList = TranslateUtils.StringCollectionToIntList(contentIdCollection);
                foreach (var contentId in contentIdList)
                {
                    if (contentId > 0 && !idList.Contains(contentId))
                    {
                        idList.Add(contentId);
                    }
                }
            }

            return idList;
        }

        public IList<int> GetContentIdListByTagCollection(List<string> tagCollection, int siteId)
        {
            var contentIdList = new List<int>();
            if (tagCollection.Count <= 0) return contentIdList;

            var contentIdCollectionList = _repository.GetAll<string>(Q
                .Select(Attr.ContentIdCollection)
                .Where(Attr.SiteId, siteId)
                .WhereIn(Attr.Tag, tagCollection));

            foreach (var contentIdCollection in contentIdCollectionList)
            {
                var list = TranslateUtils.StringCollectionToIntList(contentIdCollection);
                foreach (var contentId in list)
                {
                    if (contentId > 0 && !contentIdList.Contains(contentId))
                    {
                        contentIdList.Add(contentId);
                    }
                }
            }

            return contentIdList;
        }
    }
}


// using System.Collections.Generic;
// using System.Collections.Specialized;
// using System.Data;
// using System.Text;
// using Datory;
// using SiteServer.CMS.Core;
// using SiteServer.CMS.Model;
// using SiteServer.Utils;

// namespace SiteServer.CMS.Provider
// {
//     public class TagDao
// 	{
//         public override string TableName => "siteserver_Tag";

//         public override List<TableColumn> TableColumns => new List<TableColumn>
//         {
//             new TableColumn
//             {
//                 AttributeName = nameof(TagInfo.Id),
//                 DataType = DataType.Integer,
//                 IsIdentity = true,
//                 IsPrimaryKey = true
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(TagInfo.SiteId),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(TagInfo.ContentIdCollection),
//                 DataType = DataType.VarChar,
//                 DataLength = 2000
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(TagInfo.Tag),
//                 DataType = DataType.VarChar,
//                 DataLength = 255
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(TagInfo.UseNum),
//                 DataType = DataType.Integer
//             }
//         };

//         private const string ParmId = "@Id";
//         private const string ParmSiteId = "@SiteId";
//         private const string ParmContentIdCollection = "@ContentIdCollection";
//         private const string ParmTag = "@Tag";
//         private const string ParmUseNum = "@UseNum";

//         public int Insert(TagInfo tagInfo)
//         {
//             const string sqlString = "INSERT INTO siteserver_Tag (SiteId, ContentIdCollection, Tag, UseNum) VALUES (@SiteId, @ContentIdCollection, @Tag, @UseNum)";

//             var parms = new IDataParameter[]
// 			{
// 				GetParameter(ParmSiteId, DataType.Integer, tagInfo.SiteId),
// 				GetParameter(ParmContentIdCollection, DataType.VarChar, 2000, tagInfo.ContentIdCollection),
//                 GetParameter(ParmTag, DataType.VarChar, 255, tagInfo.Tag),
//                 GetParameter(ParmUseNum, DataType.Integer, tagInfo.UseNum)
// 			};

//             return ExecuteNonQueryAndReturnId(TableName, nameof(TagInfo.Id), sqlString, parms);
//         }

//         public void Update(TagInfo tagInfo)
//         {
//             var sqlString = "UPDATE siteserver_Tag SET ContentIdCollection = @ContentIdCollection, UseNum = @UseNum WHERE Id = @Id";

//             var parms = new IDataParameter[]
// 			{
// 				GetParameter(ParmContentIdCollection, DataType.VarChar, 2000, tagInfo.ContentIdCollection),
//                 GetParameter(ParmUseNum, DataType.Integer, tagInfo.UseNum),
//                 GetParameter(ParmId, DataType.Integer, tagInfo.Id)
// 			};

//             ExecuteNonQuery(sqlString, parms);
//         }

//         public TagInfo GetTagInfo(int siteId, string tag)
//         {
//             TagInfo tagInfo = null;

//             var sqlString = "SELECT Id, SiteId, ContentIdCollection, Tag, UseNum FROM siteserver_Tag WHERE SiteId = @SiteId AND Tag = @Tag";

//             var parms = new IDataParameter[]
// 			{
// 				GetParameter(ParmSiteId, DataType.Integer, siteId),
//                 GetParameter(ParmTag, DataType.VarChar, 255, tag)
// 			};

//             using (var rdr = ExecuteReader(sqlString, parms))
//             {
//                 if (rdr.Read())
//                 {
//                     var i = 0;
//                     tagInfo = new TagInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i));
//                 }
//                 rdr.Close();
//             }
//             return tagInfo;
//         }

//         public List<TagInfo> GetTagInfoList(int siteId, int contentId)
//         {
//             var list = new List<TagInfo>();

//             var whereString = GetWhereString(null, siteId, contentId);
//             string sqlString =
//                 $"SELECT Id, SiteId, ContentIdCollection, Tag, UseNum FROM siteserver_Tag {whereString}";

//             using (var rdr = ExecuteReader(sqlString))
//             {
//                 while (rdr.Read())
//                 {
//                     var i = 0;
//                     var tagInfo = new TagInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i));
//                     list.Add(tagInfo);
//                 }
//                 rdr.Close();
//             }
//             return list;
//         }

//         public string GetSqlString(int siteId, int contentId, bool isOrderByCount, int totalNum)
//         {
//             var whereString = GetWhereString(null, siteId, contentId);
//             var orderString = string.Empty;
//             if (isOrderByCount)
//             {
//                 orderString = "ORDER BY UseNum DESC";
//             }

//             return SqlUtils.ToTopSqlString("siteserver_Tag", "Id, SiteId, ContentIdCollection, Tag, UseNum", whereString, orderString, totalNum);
//         }

//         public List<TagInfo> GetTagInfoList(int siteId, int contentId, bool isOrderByCount, int totalNum)
//         {
//             var list = new List<TagInfo>();

//             var whereString = GetWhereString(null, siteId, contentId);
//             var orderString = string.Empty;
//             if (isOrderByCount)
//             {
//                 orderString = "ORDER BY UseNum DESC";
//             }

//             var sqlString = SqlUtils.ToTopSqlString("siteserver_Tag", "Id, SiteId, ContentIdCollection, Tag, UseNum", whereString, orderString, totalNum);

//             using (var rdr = ExecuteReader(sqlString))
//             {
//                 while (rdr.Read())
//                 {
//                     var i = 0;
//                     var tagInfo = new TagInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i));
//                     list.Add(tagInfo);
//                 }
//                 rdr.Close();
//             }
//             return list;
//         }

//         public List<string> GetTagListByStartString(int siteId, string startString, int totalNum)
//         {
//             var sqlWithParameter = SqlUtils.GetInStrWithParameter("Tag", AttackUtils.FilterSql(startString));

//             var sqlString = SqlUtils.GetDistinctTopSqlString("siteserver_Tag", "Tag, UseNum",
//                 $"WHERE SiteId = @SiteId AND {sqlWithParameter.Key}",
//                 "ORDER BY UseNum DESC", totalNum);

//             IDataParameter[] parameters =
//             {
//                 GetParameter("@SiteId", DataType.Integer, siteId),
//                 sqlWithParameter.Value
//             };

//             return DatabaseUtils.GetStringList(sqlString, parameters);
//         }

//         public List<string> GetTagList(int siteId)
//         {
//             string sqlString =
//                 $"SELECT Tag FROM siteserver_Tag WHERE SiteId = {siteId} ORDER BY UseNum DESC";
//             return DatabaseUtils.GetStringList(sqlString);
//         }

//         public void DeleteTags(int siteId)
//         {
//             var whereString = GetWhereString(null, siteId, 0);
//             string sqlString = $"DELETE FROM siteserver_Tag {whereString}";
//             ExecuteNonQuery(sqlString);
//         }

//         public void DeleteTag(string tag, int siteId)
//         {
//             var whereString = GetWhereString(tag, siteId, 0);
//             string sqlString = $"DELETE FROM siteserver_Tag {whereString}";
//             ExecuteNonQuery(sqlString);
//         }

//         public int GetTagCount(string tag, int siteId)
//         {
//             var contentIdList = GetContentIdListByTag(tag, siteId);
//             return contentIdList.Count;
//         }

//         private string GetWhereString(string tag, int siteId, int contentId)
//         {
//             var builder = new StringBuilder();
//             builder.Append($" WHERE SiteId = {siteId} ");
//             if (!string.IsNullOrEmpty(tag))
//             {
//                 builder.Append($"AND Tag = '{AttackUtils.FilterSql(tag)}' ");
//             }
//             if (contentId > 0)
//             {
//                 builder.Append(
//                     $"AND (ContentIdCollection = '{contentId}' OR ContentIdCollection LIKE '{contentId},%' OR ContentIdCollection LIKE '%,{contentId},%' OR ContentIdCollection LIKE '%,{contentId}')");
//             }

//             return builder.ToString();
//         }

//         public List<int> GetContentIdListByTag(string tag, int siteId)
//         {
//             var idList = new List<int>();
//             if (string.IsNullOrEmpty(tag)) return idList;

//             var whereString = GetWhereString(tag, siteId, 0);
//             var sqlString = "SELECT ContentIdCollection FROM siteserver_Tag" + whereString;

//             using (var rdr = ExecuteReader(sqlString))
//             {
//                 if (rdr.Read())
//                 {
//                     var contentIdCollection = GetString(rdr, 0);
//                     var contentIdList = TranslateUtils.StringCollectionToIntList(contentIdCollection);
//                     foreach (var contentId in contentIdList)
//                     {
//                         if (contentId > 0 && !idList.Contains(contentId))
//                         {
//                             idList.Add(contentId);
//                         }
//                     }
//                 }
//                 rdr.Close();
//             }
//             return idList;
//         }

//         public List<int> GetContentIdListByTagCollection(List<string> tagList, int siteId)
//         {
//             var contentIdList = new List<int>();
//             if (tagList.Count > 0)
//             {
//                 string parameterNameList;
//                 var parameterList = GetInParameterList(ParmTag, DataType.VarChar, 255, tagList, out parameterNameList);

//                 string sqlString =
//                     $"SELECT ContentIdCollection FROM siteserver_Tag WHERE Tag IN ({parameterNameList}) AND SiteId = @SiteId";

//                 var paramList = new List<IDataParameter>();
//                 paramList.AddRange(parameterList);
//                 paramList.Add(GetParameter(ParmSiteId, DataType.Integer, siteId));

//                 using (var rdr = ExecuteReader(sqlString, paramList.ToArray()))
//                 {
//                     while (rdr.Read())
//                     {
//                         var contentIdCollection = GetString(rdr, 0);
//                         var list = TranslateUtils.StringCollectionToIntList(contentIdCollection);
//                         foreach (var contentId in list)
//                         {
//                             if (contentId > 0 && !contentIdList.Contains(contentId))
//                             {
//                                 contentIdList.Add(contentId);
//                             }
//                         }
//                     }
//                     rdr.Close();
//                 }
//             }
//             return contentIdList;
//         }
// 	}
// }
