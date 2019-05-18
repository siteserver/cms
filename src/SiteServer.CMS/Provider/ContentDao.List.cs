using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Dapper;
using Datory;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;
using Attr = SiteServer.CMS.Model.Attributes.ContentAttribute;

namespace SiteServer.CMS.Provider
{
    public partial class ContentDao
    {
        public IList<int> GetContentIdList(int channelId)
        {
            return _repository.GetAll<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, channelId)
            );
        }

        public IList<int> GetContentIdList(int channelId, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState)
        {
            var query = Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, channelId)
                .OrderByDesc(Attr.Taxis, Attr.Id);

            if (isPeriods)
            {
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    query.Where(Attr.AddDate, ">=", TranslateUtils.ToDateTime(dateFrom));
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    query.Where(Attr.AddDate, "<=", TranslateUtils.ToDateTime(dateTo).AddDays(1));
                }
            }

            if (checkedState != ETriState.All)
            {
                query.Where(Attr.IsChecked, ETriStateUtils.GetValue(checkedState));
            }

            return _repository.GetAll<int>(query);
        }

        public IList<int> GetContentIdListCheckedByChannelId(int siteId, int channelId)
        {
            return _repository.GetAll<int>(Q
                .Select(Attr.Id)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.IsChecked, true.ToString())
            );
        }

        public IList<(int, int)> GetContentIdListByTrash(int siteId)
        {
            var list = _repository.GetAll<(int contentId, int channelId)>(Q
                .Select(Attr.Id, Attr.ChannelId)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, "<", 0));

            return list.Select(o => (Math.Abs(o.channelId), o.contentId)).ToList();
        }

        public List<int> GetContentIdListChecked(int channelId, string orderByFormatString)
        {
            return GetContentIdListChecked(channelId, orderByFormatString, string.Empty);
        }

        private List<int> GetContentIdListChecked(int channelId, string orderByFormatString, string whereString)
        {
            return GetContentIdListChecked(channelId, 0, orderByFormatString, whereString);
        }

        private List<int> GetContentIdListChecked(int channelId, int totalNum, string orderByFormatString, string whereString)
        {
            var channelIdList = new List<int>
            {
                channelId
            };
            return GetContentIdListChecked(channelIdList, totalNum, orderByFormatString, whereString);
        }

        private List<int> GetContentIdListChecked(List<int> channelIdList, int totalNum, string orderString, string whereString)
        {
            var list = new List<int>();

            if (channelIdList == null || channelIdList.Count == 0)
            {
                return list;
            }

            string sqlString;

            if (totalNum > 0)
            {
                sqlString = SqlUtils.ToTopSqlString(TableName, "Id",
                    channelIdList.Count == 1
                        ? $"WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString})"
                        : $"WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString})", orderString,
                    totalNum);
            }
            else
            {
                sqlString = channelIdList.Count == 1
                    ? $"SELECT Id FROM {TableName} WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString}) {orderString}"
                    : $"SELECT Id FROM {TableName} WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString}) {orderString}";
            }

            using (var connection = new Connection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                using (var rdr = connection.ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var contentId = DatabaseUtils.GetInt(rdr, 0);
                        list.Add(contentId);
                    }
                    rdr.Close();
                }
            }
            return list;
        }

        public List<Tuple<int, int>> ApiGetContentIdListBySiteId(int siteId, ApiContentsParameters parameters, out int totalCount)
        {
            totalCount = 0;
            var list = new List<ContentInfo>();

            var whereString = $"WHERE {Attr.SiteId} = {siteId} AND {Attr.ChannelId} > 0 AND {Attr.IsChecked} = '{true}'";
            if (parameters.ChannelIds.Count > 0)
            {
                whereString += $" AND {Attr.ChannelId} IN ({TranslateUtils.ObjectCollectionToString(parameters.ChannelIds)})";
            }
            if (!string.IsNullOrEmpty(parameters.ChannelGroup))
            {
                var channelIdList = ChannelManager.GetChannelIdList(siteId, parameters.ChannelGroup);
                if (channelIdList.Count > 0)
                {
                    whereString += $" AND {Attr.ChannelId} IN ({TranslateUtils.ObjectCollectionToString(channelIdList)})";
                }
            }
            if (!string.IsNullOrEmpty(parameters.ContentGroup))
            {
                var contentGroup = parameters.ContentGroup;
                whereString += $" AND ({Attr.GroupNameCollection} = '{AttackUtils.FilterSql(contentGroup)}' OR {SqlUtils.GetInStr(Attr.GroupNameCollection, contentGroup + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + contentGroup + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + contentGroup)})";
            }
            if (!string.IsNullOrEmpty(parameters.Tag))
            {
                var tag = parameters.Tag;
                whereString += $" AND ({Attr.Tags} = '{AttackUtils.FilterSql(tag)}' OR {SqlUtils.GetInStr(Attr.Tags, tag + ",")} OR {SqlUtils.GetInStr(Attr.Tags, "," + tag + ",")} OR {SqlUtils.GetInStr(Attr.Tags, "," + tag)})";
            }

            var channelInfo = ChannelManager.GetChannelInfo(siteId, siteId);
            var orderString = GetOrderString(channelInfo, AttackUtils.FilterSql(parameters.OrderBy));
            var dbArgs = new Dictionary<string, object>();

            if (parameters.QueryString != null && parameters.QueryString.Count > 0)
            {
                var columnNameList = TableColumnManager.GetTableColumnNameList(TableName);

                foreach (string attributeName in parameters.QueryString)
                {
                    if (!StringUtils.ContainsIgnoreCase(columnNameList, attributeName)) continue;

                    var value = parameters.QueryString[attributeName];

                    if (StringUtils.EqualsIgnoreCase(attributeName, Attr.IsChecked) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsColor) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsHot) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsRecommend) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsTop))
                    {
                        whereString += $" AND {attributeName} = '{TranslateUtils.ToBool(value)}'";
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeName, Attr.Id) || StringUtils.EqualsIgnoreCase(attributeName, Attr.ReferenceId) || StringUtils.EqualsIgnoreCase(attributeName, Attr.SourceId) || StringUtils.EqualsIgnoreCase(attributeName, Attr.CheckedLevel))
                    {
                        whereString += $" AND {attributeName} = {TranslateUtils.ToInt(value)}";
                    }
                    else if (parameters.Likes.Contains(attributeName))
                    {
                        whereString += $" AND {attributeName} LIKE '%{AttackUtils.FilterSql(value)}%'";
                    }
                    else
                    {
                        whereString += $" AND {attributeName} = @{attributeName}";
                        dbArgs.Add(attributeName, value);
                    }
                }
            }

            totalCount = DatabaseUtils.GetPageTotalCount(TableName, whereString, dbArgs);
            if (totalCount > 0 && parameters.Skip < totalCount)
            {
                var sqlString = DatabaseUtils.GetPageSqlString(TableName, Container.Content.SqlColumns, whereString, orderString, parameters.Skip, parameters.Top);

                using (var connection = new Connection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
                {
                    list = connection.Query<ContentInfo>(sqlString, dbArgs).ToList();
                }
            }

            var tupleList = new List<Tuple<int, int>>();
            foreach (var contentInfo in list)
            {
                tupleList.Add(new Tuple<int, int>(contentInfo.ChannelId, contentInfo.Id));
            }

            return tupleList;
        }

        public IList<(int, int)> ApiGetContentIdListByChannelId(int siteId, int channelId, int top, int skip, string like, string orderBy, NameValueCollection queryString, out int totalCount)
        {
            var retVal = new List<(int, int)>();

            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var channelIdList = ChannelManager.GetChannelIdList(channelInfo, EScopeType.All, string.Empty, string.Empty, string.Empty);
            var whereString = $"WHERE {Attr.SiteId} = {siteId} AND {Attr.ChannelId} IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND {Attr.IsChecked} = '{true}'";

            var likeList = TranslateUtils.StringCollectionToStringList(StringUtils.TrimAndToLower(like));
            var orderString = GetOrderString(channelInfo, AttackUtils.FilterSql(orderBy));
            var dbArgs = new Dictionary<string, object>();

            if (queryString != null && queryString.Count > 0)
            {
                var columnNameList = TableColumnManager.GetTableColumnNameList(TableName);

                foreach (string attributeName in queryString)
                {
                    if (!StringUtils.ContainsIgnoreCase(columnNameList, attributeName)) continue;

                    var value = queryString[attributeName];

                    if (StringUtils.EqualsIgnoreCase(attributeName, Attr.IsChecked) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsColor) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsHot) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsRecommend) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsTop))
                    {
                        whereString += $" AND {attributeName} = '{TranslateUtils.ToBool(value)}'";
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeName, Attr.Id) || StringUtils.EqualsIgnoreCase(attributeName, Attr.ReferenceId) || StringUtils.EqualsIgnoreCase(attributeName, Attr.SourceId) || StringUtils.EqualsIgnoreCase(attributeName, Attr.CheckedLevel))
                    {
                        whereString += $" AND {attributeName} = {TranslateUtils.ToInt(value)}";
                    }
                    else if (likeList.Contains(attributeName))
                    {
                        whereString += $" AND {attributeName} LIKE '%{AttackUtils.FilterSql(value)}%'";
                    }
                    else
                    {
                        whereString += $" AND {attributeName} = @{attributeName}";
                        dbArgs.Add(attributeName, value);
                    }
                }
            }

            totalCount = DatabaseUtils.GetPageTotalCount(TableName, whereString, dbArgs);
            if (totalCount > 0 && skip < totalCount)
            {
                var sqlString = DatabaseUtils.GetPageSqlString(TableName, Container.Content.SqlColumns, whereString, orderString, skip, top);

                using (var connection = new Connection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
                {
                    retVal = connection.Query<ContentInfo>(sqlString, dbArgs).Select(o => (o.ChannelId, o.Id)).ToList();
                }
            }

            return retVal;
        }

        private IList<int> GetReferenceIdList(IList<int> contentIdList)
        {
            return _repository.GetAll<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, ">", 0)
                .WhereIn(Attr.ReferenceId, contentIdList)
            );
        }

        public IList<int> GetIdListBySameTitle(int channelId, string title)
        {
            return _repository.GetAll<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.Title, title)
            );
        }

        public IList<int> GetChannelIdListCheckedByLastEditDateHour(int siteId, int hour)
        {
            return _repository.GetAll<int>(Q
                .Select(Attr.ChannelId)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.IsChecked, true.ToString())
                .WhereBetween(Attr.LastEditDate, DateTime.Now.AddHours(-hour), DateTime.Now)
                .Distinct()
            );
        }

        public List<int> GetCacheContentIdList(string whereString, string orderString, int offset, int limit)
        {
            var list = new List<int>();

            var sqlString = DatabaseUtils.GetPageSqlString(TableName, Container.Content.SqlColumns, whereString, orderString, offset, limit);

            using (var conneciton = new Connection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                using (var rdr = conneciton.ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var contentId = DatabaseUtils.GetInt(rdr, 0);

                        list.Add(contentId);
                    }
                    rdr.Close();
                }
            }

            return list;
        }

        public IList<string> GetValueListByStartString(int channelId, string name, string startString, int totalNum)
        {
            return _repository.GetAll<string>(Q
                .Select(name)
                .Where(Attr.ChannelId, channelId)
                .WhereInStr(name, startString)
                .Distinct()
                .Limit(totalNum)
            );
        }

        public List<ContentInfo> GetContentInfoList(string whereString, string orderString, int offset, int limit)
        {
            var list = new List<ContentInfo>();

            var sqlString = DatabaseUtils.GetPageSqlString(TableName, SqlUtils.Asterisk, whereString, orderString, offset, limit);

            using (var connection = new Connection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                using (var rdr = connection.ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var contentInfo = new ContentInfo(TranslateUtils.ToDictionary(rdr));

                        list.Add(contentInfo);
                    }
                    rdr.Close();
                }
            }

            return list;
        }

        public List<ContentCountInfo> GetContentCountInfoList()
        {
            List<ContentCountInfo> list;

            var sqlString =
                $@"SELECT {Attr.SiteId}, {Attr.ChannelId}, {Attr.IsChecked}, {Attr.CheckedLevel}, {Attr.AdminId}, COUNT(*) AS {nameof(ContentCountInfo.Count)} FROM {TableName} WHERE {Attr.ChannelId} > 0 AND {Attr.SourceId} != {SourceManager.Preview} GROUP BY {Attr.SiteId}, {Attr.ChannelId}, {Attr.IsChecked}, {Attr.CheckedLevel}, {Attr.AdminId}";

            using (var connection = new Connection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                list = connection.Query<ContentCountInfo>(sqlString).ToList();
            }

            return list;
        }
    }
}
