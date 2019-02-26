using System.Collections.Generic;
using System.Text;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;
using SqlKata;

namespace SiteServer.CMS.Database.Repositories
{
    public class TagRepository : GenericRepository<TagInfo>
    {
        private static class Attr
        {
            public const string SiteId = nameof(TagInfo.SiteId);
            public const string ContentIdCollection = nameof(TagInfo.ContentIdCollection);
            public const string Tag = nameof(TagInfo.Tag);
            public const string UseNum = nameof(TagInfo.UseNum);
        }

        public void Insert(TagInfo tagInfo)
        {
            //const string sqlString = "INSERT INTO siteserver_Tag (SiteId, ContentIdCollection, Tag, UseNum) VALUES (@SiteId, @ContentIdCollection, @Tag, @UseNum)";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteId, tagInfo.SiteId),
            //    GetParameter(ParamContentIdCollection, tagInfo.ContentIdCollection),
            //    GetParameter(ParamTag, tagInfo.Tag),
            //    GetParameter(ParamUseNum, tagInfo.UseNum)
            //};

            //return DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, nameof(TagInfo.Id), sqlString, parameters);

            InsertObject(tagInfo);
        }

        public void Update(TagInfo tagInfo)
        {
            //const string sqlString = "UPDATE siteserver_Tag SET ContentIdCollection = @ContentIdCollection, UseNum = @UseNum WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamContentIdCollection, tagInfo.ContentIdCollection),
            //    GetParameter(ParamUseNum, tagInfo.UseNum),
            //    GetParameter(ParamId, tagInfo.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateObject(tagInfo);
        }

        public TagInfo GetTagInfo(int siteId, string tag)
        {
            //TagInfo tagInfo = null;

            //const string sqlString = "SELECT Id, SiteId, ContentIdCollection, Tag, UseNum FROM siteserver_Tag WHERE SiteId = @SiteId AND Tag = @Tag";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteId, siteId),
            //    GetParameter(ParamTag, tag)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        var i = 0;
            //        tagInfo = new TagInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i));
            //    }
            //    rdr.Close();
            //}
            //return tagInfo;

            return GetObject(Q.Where(Attr.SiteId, siteId).Where(Attr.Tag, tag));
        }

        public IList<TagInfo> GetTagInfoList(int siteId, int contentId)
        {
            //var list = new List<TagInfo>();

            //var whereString = GetWhereString(null, siteId, contentId);
            //var sqlString =
            //    $"SELECT Id, SiteId, ContentIdCollection, Tag, UseNum FROM siteserver_Tag {whereString}";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var tagInfo = new TagInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i));
            //        list.Add(tagInfo);
            //    }
            //    rdr.Close();
            //}
            //return list;

            var query = GetQuery(null, siteId, contentId);
            return GetObjectList(query);
        }

        public string GetSqlString(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var whereString = GetWhereString(null, siteId, contentId);
            var orderString = string.Empty;
            if (isOrderByCount)
            {
                orderString = "ORDER BY UseNum DESC";
            }

            return SqlDifferences.GetSqlString("siteserver_Tag", new List<string>
            {
                nameof(TagInfo.Id),
                nameof(TagInfo.SiteId),
                nameof(TagInfo.ContentIdCollection),
                nameof(TagInfo.Tag),
                nameof(TagInfo.UseNum)
            }, whereString, orderString, totalNum);
        }

        public IList<TagInfo> GetTagInfoList(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            //var list = new List<TagInfo>();

            //var whereString = GetWhereString(null, siteId, contentId);
            //var orderString = string.Empty;
            //if (isOrderByCount)
            //{
            //    orderString = "ORDER BY UseNum DESC";
            //}

            //var sqlString = SqlDifferences.GetSqlString("siteserver_Tag", new List<string>
            //{
            //    nameof(TagInfo.Id),
            //    nameof(TagInfo.SiteId),
            //    nameof(TagInfo.ContentIdCollection),
            //    nameof(TagInfo.Tag),
            //    nameof(TagInfo.UseNum)
            //}, whereString, orderString, totalNum);

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var tagInfo = new TagInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i));
            //        list.Add(tagInfo);
            //    }
            //    rdr.Close();
            //}
            //return list;

            var query = GetQuery(null, siteId, contentId).Limit(totalNum);
            if (isOrderByCount)
            {
                query.OrderByDesc(Attr.UseNum);
            }

            return GetObjectList(query);
        }

        public IList<string> GetTagListByStartString(int siteId, string startString, int totalNum)
        {
            //var sqlString = SqlDifferences.GetSqlString("siteserver_Tag", new List<string>
            //    {
            //        nameof(TagInfo.Tag),
            //        nameof(TagInfo.UseNum)
            //    },
            //    $"WHERE SiteId = {siteId} AND {SqlUtils.GetInStr("Tag", AttackUtils.FilterSql(startString))}",
            //    "ORDER BY UseNum DESC", 0, totalNum, true);
            //return DatabaseApi.GetStringList(sqlString);

            return GetValueList<string>(Q
                .Select(Attr.Tag)
                .Where(Attr.SiteId, siteId)
                .WhereContains(Attr.Tag, startString)
                .OrderByDesc(Attr.UseNum)
                .Distinct()
                .Limit(totalNum));
        }

        public IList<string> GetTagList(int siteId)
        {
            //var sqlString =
            //    $"SELECT Tag FROM siteserver_Tag WHERE SiteId = {siteId} ORDER BY UseNum DESC";
            //return DatabaseApi.GetStringList(sqlString);

            return GetValueList<string>(Q
                .Select(Attr.Tag)
                .Where(Attr.SiteId, siteId)
                .Distinct()
                .OrderByDesc(Attr.UseNum));
        }

        public void DeleteTags(int siteId)
        {
            //var whereString = GetWhereString(null, siteId, 0);
            //var sqlString = $"DELETE FROM siteserver_Tag {whereString}";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            var query = GetQuery(null, siteId, 0);
            DeleteAll(query);
        }

        public void DeleteTag(string tag, int siteId)
        {
            //var whereString = GetWhereString(tag, siteId, 0);
            //var sqlString = $"DELETE FROM siteserver_Tag {whereString}";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            var query = GetQuery(tag, siteId, 0);
            DeleteAll(query);
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
            //var idList = new List<int>();
            //if (string.IsNullOrEmpty(tag)) return idList;

            //var whereString = GetWhereString(tag, siteId, 0);
            //var sqlString = "SELECT ContentIdCollection FROM siteserver_Tag" + whereString;

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    if (rdr.Read())
            //    {
            //        var contentIdCollection = DatabaseApi.GetString(rdr, 0);
            //        var contentIdList = TranslateUtils.StringCollectionToIntList(contentIdCollection);
            //        foreach (var contentId in contentIdList)
            //        {
            //            if (contentId > 0 && !idList.Contains(contentId))
            //            {
            //                idList.Add(contentId);
            //            }
            //        }
            //    }
            //    rdr.Close();
            //}
            //return idList;

            var idList = new List<int>();
            if (string.IsNullOrEmpty(tag)) return idList;

            var query = GetQuery(tag, siteId, 0);
            var contentIdCollectionList = GetValueList<string>(query.Select(Attr.ContentIdCollection));
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

        //private List<IDataParameter> GetInParameterList(string parameterName, ICollection valueCollection, out string parameterNameList)
        //{
        //    parameterNameList = string.Empty;
        //    if (valueCollection == null || valueCollection.Count <= 0) return new List<IDataParameter>();

        //    var parameterList = new List<IDataParameter>();

        //    var sbCondition = new StringBuilder();
        //    var i = 0;
        //    foreach (var obj in valueCollection)
        //    {
        //        i++;

        //        var value = obj.ToString();
        //        var name = parameterName + "_" + i;

        //        sbCondition.Append(name + ",");

        //        parameterList.Add(GetParameter(name, value));
        //    }

        //    parameterNameList = sbCondition.ToString().TrimEnd(',');

        //    return parameterList;
        //}

        public IList<int> GetContentIdListByTagCollection(List<string> tagCollection, int siteId)
        {
            var contentIdList = new List<int>();
            if (tagCollection.Count <= 0) return contentIdList;

            var contentIdCollectionList = GetValueList<string>(Q
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

            //var parameterList = GetInParameterList(ParamTag, tagCollection, out var parameterNameList);

            //var sqlString =
            //    $"SELECT ContentIdCollection FROM siteserver_Tag WHERE Tag IN ({parameterNameList}) AND SiteId = @SiteId";

            //var paramList = new List<IDataParameter>();
            //paramList.AddRange(parameterList);
            //paramList.Add(GetParameter(ParamSiteId, siteId));

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, paramList.ToArray()))
            //{
            //    while (rdr.Read())
            //    {
            //        var contentIdCollection = DatabaseApi.GetString(rdr, 0);
            //        var list = TranslateUtils.StringCollectionToIntList(contentIdCollection);
            //        foreach (var contentId in list)
            //        {
            //            if (contentId > 0 && !contentIdList.Contains(contentId))
            //            {
            //                contentIdList.Add(contentId);
            //            }
            //        }
            //    }
            //    rdr.Close();
            //}
            //return contentIdList;
        }
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Data;
//using System.Text;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class TagDao : DataProviderBase
//	{
//        public override string TableName => "siteserver_Tag";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(TagInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TagInfo.SiteId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TagInfo.ContentIdCollection),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TagInfo.Tag),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TagInfo.UseNum),
//                DataType = DataType.Integer
//            }
//        };

//        private const string ParamId = "@Id";
//        private const string ParamSiteId = "@SiteId";
//        private const string ParamContentIdCollection = "@ContentIdCollection";
//        private const string ParamTag = "@Tag";
//        private const string ParamUseNum = "@UseNum";

//        public int InsertObject(TagInfo tagInfo)
//        {
//            const string sqlString = "INSERT INTO siteserver_Tag (SiteId, ContentIdCollection, Tag, UseNum) VALUES (@SiteId, @ContentIdCollection, @Tag, @UseNum)";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamSiteId, tagInfo.SiteId),
//				GetParameter(ParamContentIdCollection, tagInfo.ContentIdCollection),
//                GetParameter(ParamTag, tagInfo.Tag),
//                GetParameter(ParamUseNum, tagInfo.UseNum)
//			};

//            return DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, nameof(TagInfo.Id), sqlString, parameters);
//        }

//        public void UpdateObject(TagInfo tagInfo)
//        {
//            const string sqlString = "UPDATE siteserver_Tag SET ContentIdCollection = @ContentIdCollection, UseNum = @UseNum WHERE Id = @Id";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamContentIdCollection, tagInfo.ContentIdCollection),
//                GetParameter(ParamUseNum, tagInfo.UseNum),
//                GetParameter(ParamId, tagInfo.Id)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//        }

//        public TagInfo GetTagInfo(int siteId, string tag)
//        {
//            TagInfo tagInfo = null;

//            const string sqlString = "SELECT Id, SiteId, ContentIdCollection, Tag, UseNum FROM siteserver_Tag WHERE SiteId = @SiteId AND Tag = @Tag";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamSiteId, siteId),
//                GetParameter(ParamTag, tag)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    var i = 0;
//                    tagInfo = new TagInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i));
//                }
//                rdr.Close();
//            }
//            return tagInfo;
//        }

//        public List<TagInfo> GetTagInfoList(int siteId, int contentId)
//        {
//            var list = new List<TagInfo>();

//            var whereString = GetWhereString(null, siteId, contentId);
//            var sqlString =
//                $"SELECT Id, SiteId, ContentIdCollection, Tag, UseNum FROM siteserver_Tag {whereString}";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var tagInfo = new TagInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i));
//                    list.Add(tagInfo);
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        public string GetSqlString(int siteId, int contentId, bool isOrderByCount, int totalNum)
//        {
//            var whereString = GetWhereString(null, siteId, contentId);
//            var orderString = string.Empty;
//            if (isOrderByCount)
//            {
//                orderString = "ORDER BY UseNum DESC";
//            }

//            return SqlDifferences.GetSqlString("siteserver_Tag", new List<string>
//            {
//                nameof(TagInfo.Id),
//                nameof(TagInfo.SiteId),
//                nameof(TagInfo.ContentIdCollection),
//                nameof(TagInfo.Tag),
//                nameof(TagInfo.UseNum)
//            }, whereString, orderString, totalNum);
//        }

//        public List<TagInfo> GetTagInfoList(int siteId, int contentId, bool isOrderByCount, int totalNum)
//        {
//            var list = new List<TagInfo>();

//            var whereString = GetWhereString(null, siteId, contentId);
//            var orderString = string.Empty;
//            if (isOrderByCount)
//            {
//                orderString = "ORDER BY UseNum DESC";
//            }

//            var sqlString = SqlDifferences.GetSqlString("siteserver_Tag", new List<string>
//            {
//                nameof(TagInfo.Id),
//                nameof(TagInfo.SiteId),
//                nameof(TagInfo.ContentIdCollection),
//                nameof(TagInfo.Tag),
//                nameof(TagInfo.UseNum)
//            }, whereString, orderString, totalNum);

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var tagInfo = new TagInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i));
//                    list.Add(tagInfo);
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        public List<string> GetTagListByStartString(int siteId, string startString, int totalNum)
//        {
//            var sqlString = SqlDifferences.GetSqlString("siteserver_Tag", new List<string>
//                {
//                    nameof(TagInfo.Tag),
//                    nameof(TagInfo.UseNum)
//                },
//                $"WHERE SiteId = {siteId} AND {SqlUtils.GetInStr("Tag", AttackUtils.FilterSql(startString))}",
//                "ORDER BY UseNum DESC", 0, totalNum, true);
//            return DatabaseApi.GetStringList(sqlString);
//        }

//        public List<string> GetTagList(int siteId)
//        {
//            var sqlString =
//                $"SELECT Tag FROM siteserver_Tag WHERE SiteId = {siteId} ORDER BY UseNum DESC";
//            return DatabaseApi.GetStringList(sqlString);
//        }

//        public void DeleteTags(int siteId)
//        {
//            var whereString = GetWhereString(null, siteId, 0);
//            var sqlString = $"DELETE FROM siteserver_Tag {whereString}";
//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//        }

//        public void DeleteTag(string tag, int siteId)
//        {
//            var whereString = GetWhereString(tag, siteId, 0);
//            var sqlString = $"DELETE FROM siteserver_Tag {whereString}";
//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//        }

//        public int GetTagCount(string tag, int siteId)
//        {
//            var contentIdList = GetContentIdListByTag(tag, siteId);
//            return contentIdList.Count;
//        }

//        private string GetWhereString(string tag, int siteId, int contentId)
//        {
//            var builder = new StringBuilder();
//            builder.Append($" WHERE SiteId = {siteId} ");
//            if (!string.IsNullOrEmpty(tag))
//            {
//                builder.Append($"AND Tag = '{AttackUtils.FilterSql(tag)}' ");
//            }
//            if (contentId > 0)
//            {
//                builder.Append(
//                    $"AND (ContentIdCollection = '{contentId}' OR ContentIdCollection LIKE '{contentId},%' OR ContentIdCollection LIKE '%,{contentId},%' OR ContentIdCollection LIKE '%,{contentId}')");
//            }

//            return builder.ToString();
//        }

//        public List<int> GetContentIdListByTag(string tag, int siteId)
//        {
//            var idList = new List<int>();
//            if (string.IsNullOrEmpty(tag)) return idList;

//            var whereString = GetWhereString(tag, siteId, 0);
//            var sqlString = "SELECT ContentIdCollection FROM siteserver_Tag" + whereString;

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                if (rdr.Read())
//                {
//                    var contentIdCollection = DatabaseApi.GetString(rdr, 0);
//                    var contentIdList = TranslateUtils.StringCollectionToIntList(contentIdCollection);
//                    foreach (var contentId in contentIdList)
//                    {
//                        if (contentId > 0 && !idList.Contains(contentId))
//                        {
//                            idList.Add(contentId);
//                        }
//                    }
//                }
//                rdr.Close();
//            }
//            return idList;
//        }

//	    private List<IDataParameter> GetInParameterList(string parameterName, ICollection valueCollection, out string parameterNameList)
//	    {
//	        parameterNameList = string.Empty;
//	        if (valueCollection == null || valueCollection.Count <= 0) return new List<IDataParameter>();

//	        var parameterList = new List<IDataParameter>();

//	        var sbCondition = new StringBuilder();
//	        var i = 0;
//	        foreach (var obj in valueCollection)
//	        {
//	            i++;

//	            var value = obj.ToString();
//	            var name = parameterName + "_" + i;

//	            sbCondition.Append(name + ",");

//	            parameterList.Add(GetParameter(name, value));
//	        }

//	        parameterNameList = sbCondition.ToString().TrimEnd(',');

//	        return parameterList;
//	    }

//        public List<int> GetContentIdListByTagCollection(StringCollection tagCollection, int siteId)
//        {
//            var contentIdList = new List<int>();
//            if (tagCollection.Count <= 0) return contentIdList;

//            var parameterList = GetInParameterList(ParamTag, tagCollection, out var parameterNameList);

//            var sqlString =
//                $"SELECT ContentIdCollection FROM siteserver_Tag WHERE Tag IN ({parameterNameList}) AND SiteId = @SiteId";

//            var paramList = new List<IDataParameter>();
//            paramList.AddRange(parameterList);
//            paramList.Add(GetParameter(ParamSiteId, siteId));

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, paramList.ToArray()))
//            {
//                while (rdr.Read())
//                {
//                    var contentIdCollection = DatabaseApi.GetString(rdr, 0);
//                    var list = TranslateUtils.StringCollectionToIntList(contentIdCollection);
//                    foreach (var contentId in list)
//                    {
//                        if (contentId > 0 && !contentIdList.Contains(contentId))
//                        {
//                            contentIdList.Add(contentId);
//                        }
//                    }
//                }
//                rdr.Close();
//            }
//            return contentIdList;
//        }
//	}
//}
