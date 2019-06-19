using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Dapper;
using SqlKata;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Utils;
using Attr = SS.CMS.Core.Models.Attributes.ContentAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        private List<KeyValuePair<int, ContentInfo>> GetContainerContentListChecked(List<int> channelIdList, int startNum, int totalNum, string order, Query query, NameValueCollection others)
        {
            if (channelIdList == null || channelIdList.Count == 0) return null;

            query.WhereIn(Attr.ChannelId, channelIdList).WhereTrue(Attr.IsChecked);

            if (others != null && others.Count > 0)
            {
                var columnNameList = _tableManager.GetTableColumnNameList(TableName);

                foreach (var attributeName in others.AllKeys)
                {
                    if (StringUtils.ContainsIgnoreCase(columnNameList, attributeName))
                    {
                        var value = others.Get(attributeName);
                        if (!string.IsNullOrEmpty(value))
                        {
                            value = value.Trim();
                            if (StringUtils.StartsWithIgnoreCase(value, "not:"))
                            {
                                value = value.Substring("not:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    query.WhereNot(attributeName, value);
                                }
                                else
                                {
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    foreach (var val in collection)
                                    {
                                        query.WhereNot(attributeName, val);
                                    }
                                }
                            }
                            else if (StringUtils.StartsWithIgnoreCase(value, "contains:"))
                            {
                                value = value.Substring("contains:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    query.WhereContains(attributeName, value);
                                }
                                else
                                {
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    query.Where(q =>
                                    {
                                        foreach (var val in collection)
                                        {
                                            q.OrWhereContains(attributeName, val);
                                        }
                                        return q;
                                    });
                                }
                            }
                            else if (StringUtils.StartsWithIgnoreCase(value, "start:"))
                            {
                                value = value.Substring("start:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    query.WhereStarts(attributeName, value);
                                }
                                else
                                {
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    query.Where(q =>
                                    {
                                        foreach (var val in collection)
                                        {
                                            q.OrWhereStarts(attributeName, val);
                                        }
                                        return q;
                                    });
                                }
                            }
                            else if (StringUtils.StartsWithIgnoreCase(value, "end:"))
                            {
                                value = value.Substring("end:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    query.WhereEnds(attributeName, value);
                                }
                                else
                                {
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    query.Where(q =>
                                    {
                                        foreach (var val in collection)
                                        {
                                            q.OrWhereEnds(attributeName, val);
                                        }
                                        return q;
                                    });
                                }
                            }
                            else
                            {
                                if (value.IndexOf(',') == -1)
                                {
                                    query.Where(attributeName, value);
                                }
                                else
                                {
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    query.Where(q =>
                                    {
                                        foreach (var val in collection)
                                        {
                                            q.OrWhere(attributeName, val);
                                        }
                                        return q;
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return startNum <= 1 ? GetContainerContentListByContentNumAndWhereString(totalNum, query, order) : GetContainerContentListByStartNum(startNum, totalNum, query, order);
        }

        private List<KeyValuePair<int, ContentInfo>> GetContainerContentListByContentNumAndWhereString(int totalNum, Query query, string order)
        {
            var list = new List<KeyValuePair<int, ContentInfo>>();
            var itemIndex = 0;

            QuerySelectMinColumns(query);
            var contentInfoList = _repository.GetAll(query
                .Limit(totalNum)
                .OrderByRaw(order)
                ).ToList();

            foreach (var contentInfo in contentInfoList)
            {
                list.Add(new KeyValuePair<int, ContentInfo>(itemIndex++, contentInfo));
            }

            return list;
        }

        private List<KeyValuePair<int, ContentInfo>> GetContainerContentListByStartNum(int startNum, int totalNum, Query query, string order)
        {
            var list = new List<KeyValuePair<int, ContentInfo>>();
            var itemIndex = 0;

            QuerySelectMinColumns(query);
            var contentInfoList = _repository.GetAll(query
                .Offset(startNum - 1)
                .Limit(totalNum)
                .OrderByRaw(order)
            ).ToList();

            foreach (var contentInfo in contentInfoList)
            {
                list.Add(new KeyValuePair<int, ContentInfo>(itemIndex++, contentInfo));
            }

            return list;
        }

        public List<KeyValuePair<int, ContentInfo>> GetContainerContentListBySqlString(string sqlString, string orderString, int totalCount, int itemsPerPage, int currentPageIndex)
        {
            var pageSqlString = string.Empty;

            var temp = sqlString.ToLower();
            var pos = temp.LastIndexOf("order by", StringComparison.Ordinal);
            if (pos > -1)
                sqlString = sqlString.Substring(0, pos);

            var recordsInLastPage = itemsPerPage;

            // Calculate the correspondent number of pages
            var lastPage = totalCount / itemsPerPage;
            var remainder = totalCount % itemsPerPage;
            if (remainder > 0)
                lastPage++;
            var pageCount = lastPage;

            if (remainder > 0)
                recordsInLastPage = remainder;

            var recsToRetrieve = itemsPerPage;
            if (currentPageIndex == pageCount - 1)
                recsToRetrieve = recordsInLastPage;

            orderString = orderString.ToUpper();
            var orderStringReverse = orderString.Replace(" DESC", " DESC2");
            orderStringReverse = orderStringReverse.Replace(" ASC", " DESC");
            orderStringReverse = orderStringReverse.Replace(" DESC2", " ASC");

            if (_repository.Db.DatabaseType == DatabaseType.MySql)
            {
                pageSqlString = $@"
SELECT {Container.Content.SqlColumns} FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }
            else if (_repository.Db.DatabaseType == DatabaseType.SqlServer)
            {
                pageSqlString = $@"
SELECT {Container.Content.SqlColumns} FROM (
    SELECT TOP {recsToRetrieve} * FROM (
        SELECT TOP {itemsPerPage * (currentPageIndex + 1)} * FROM ({sqlString}) AS t0 {orderString}
    ) AS t1 {orderStringReverse}
) AS t2 {orderString}";
            }
            else if (_repository.Db.DatabaseType == DatabaseType.PostgreSql)
            {
                pageSqlString = $@"
SELECT {Container.Content.SqlColumns} FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }
            else if (_repository.Db.DatabaseType == DatabaseType.Oracle)
            {
                pageSqlString = $@"
SELECT {Container.Content.SqlColumns} FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) WHERE ROWNUM <= {itemsPerPage * (currentPageIndex + 1)} {orderString}
    ) WHERE ROWNUM <= {recsToRetrieve} {orderStringReverse}
) {orderString}";
            }

            var list = new List<KeyValuePair<int, ContentInfo>>();
            var itemIndex = 0;
            var contentInfoList = new List<ContentInfo>();

            using (var connection = _repository.Db.GetConnection())
            {
                contentInfoList = connection.Query<ContentInfo>(pageSqlString).ToList();
            }

            foreach (var contentInfo in contentInfoList)
            {
                list.Add(new KeyValuePair<int, ContentInfo>(itemIndex++, contentInfo));
            }

            return list;
        }
    }
}
