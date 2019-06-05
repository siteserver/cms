using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Dapper;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentDao
    {
        public List<Container.Content> GetContainerContentListChecked(List<int> channelIdList, int startNum, int totalNum, string orderByString, string whereString, NameValueCollection others)
        {
            if (channelIdList == null || channelIdList.Count == 0) return null;

            var sqlWhereString = channelIdList.Count == 1 ? $"WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString})" : $"WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString})";

            if (others != null && others.Count > 0)
            {
                var columnNameList = TableColumnManager.GetTableColumnNameList(TableName);

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
                                    sqlWhereString += $" AND ({attributeName} <> '{value}')";
                                }
                                else
                                {
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    foreach (var val in collection)
                                    {
                                        sqlWhereString += $" AND ({attributeName} <> '{val}')";
                                    }
                                }
                            }
                            else if (StringUtils.StartsWithIgnoreCase(value, "contains:"))
                            {
                                value = value.Substring("contains:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    sqlWhereString += $" AND ({attributeName} LIKE '%{value}%')";
                                }
                                else
                                {
                                    var builder = new StringBuilder(" AND (");
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    foreach (var val in collection)
                                    {
                                        builder.Append($" {attributeName} LIKE '%{val}%' OR ");
                                    }
                                    builder.Length -= 3;

                                    builder.Append(")");

                                    sqlWhereString += builder.ToString();
                                }
                            }
                            else if (StringUtils.StartsWithIgnoreCase(value, "start:"))
                            {
                                value = value.Substring("start:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    sqlWhereString += $" AND ({attributeName} LIKE '{value}%')";
                                }
                                else
                                {
                                    var builder = new StringBuilder(" AND (");
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    foreach (var val in collection)
                                    {
                                        builder.Append($" {attributeName} LIKE '{val}%' OR ");
                                    }
                                    builder.Length -= 3;

                                    builder.Append(")");

                                    sqlWhereString += builder.ToString();
                                }
                            }
                            else if (StringUtils.StartsWithIgnoreCase(value, "end:"))
                            {
                                value = value.Substring("end:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    sqlWhereString += $" AND ({attributeName} LIKE '%{value}')";
                                }
                                else
                                {
                                    var builder = new StringBuilder(" AND (");
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    foreach (var val in collection)
                                    {
                                        builder.Append($" {attributeName} LIKE '%{val}' OR ");
                                    }
                                    builder.Length -= 3;

                                    builder.Append(")");

                                    sqlWhereString += builder.ToString();
                                }
                            }
                            else
                            {
                                if (value.IndexOf(',') == -1)
                                {
                                    sqlWhereString += $" AND ({attributeName} = '{value}')";
                                }
                                else
                                {
                                    var builder = new StringBuilder(" AND (");
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    foreach (var val in collection)
                                    {
                                        builder.Append($" {attributeName} = '{val}' OR ");
                                    }
                                    builder.Length -= 3;

                                    builder.Append(")");

                                    sqlWhereString += builder.ToString();
                                }
                            }
                        }
                    }
                }
            }

            return startNum <= 1 ? GetContainerContentListByContentNumAndWhereString(totalNum, sqlWhereString, orderByString) : GetContainerContentListByStartNum(startNum, totalNum, sqlWhereString, orderByString);
        }

        private List<Container.Content> GetContainerContentListByContentNumAndWhereString(int totalNum, string whereString, string orderByString)
        {
            var list = new List<Container.Content>();
            var itemIndex = 0;

            var sqlString = DatabaseUtils.GetSelectSqlString(TableName, totalNum, Container.Content.SqlColumns, whereString, orderByString);

            //{ContentAttribute.Id}, {ContentAttribute.ChannelId}, {ContentAttribute.IsTop}, {ContentAttribute.AddDate}, {ContentAttribute.LastEditDate}, {ContentAttribute.Taxis}, {ContentAttribute.Hits}, {ContentAttribute.HitsByDay}, {ContentAttribute.HitsByWeek}, {ContentAttribute.HitsByMonth}

            using (var connection = new Connection(AppSettings.DatabaseType, AppSettings.ConnectionString))
            {
                using (var rdr = connection.ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var i = 0;
                        list.Add(new Container.Content
                        {
                            ItemIndex = itemIndex++,
                            Id = DatabaseUtils.GetInt(rdr, i++),
                            SiteId = DatabaseUtils.GetInt(rdr, i++),
                            ChannelId = DatabaseUtils.GetInt(rdr, i++),
                            Top = TranslateUtils.ToBool(DatabaseUtils.GetString(rdr, i++)),
                            AddDate = DatabaseUtils.GetDateTime(rdr, i++),
                            LastEditDate = DatabaseUtils.GetDateTime(rdr, i++),
                            Taxis = DatabaseUtils.GetInt(rdr, i++),
                            Hits = DatabaseUtils.GetInt(rdr, i++),
                            HitsByDay = DatabaseUtils.GetInt(rdr, i++),
                            HitsByWeek = DatabaseUtils.GetInt(rdr, i++),
                            HitsByMonth = DatabaseUtils.GetInt(rdr, i++),
                        });
                    }
                    rdr.Close();
                }
            }

            return list;
        }

        private List<Container.Content> GetContainerContentListByStartNum(int startNum, int totalNum, string whereString, string orderByString)
        {
            var list = new List<Container.Content>();
            var itemIndex = 0;

            //var sqlSelect = DatabaseUtils.GetSelectSqlString(tableName, startNum, totalNum, MinListColumns, whereString, orderByString);
            var sqlString = DatabaseUtils.GetPageSqlString(TableName, Container.Content.SqlColumns, whereString, orderByString, startNum - 1, totalNum);

            using (var connection = new Connection(AppSettings.DatabaseType, AppSettings.ConnectionString))
            {
                using (var rdr = connection.ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var i = 0;
                        list.Add(new Container.Content
                        {
                            ItemIndex = itemIndex++,
                            Id = DatabaseUtils.GetInt(rdr, i++),
                            SiteId = DatabaseUtils.GetInt(rdr, i++),
                            ChannelId = DatabaseUtils.GetInt(rdr, i++),
                            Top = TranslateUtils.ToBool(DatabaseUtils.GetString(rdr, i++)),
                            AddDate = DatabaseUtils.GetDateTime(rdr, i++),
                            LastEditDate = DatabaseUtils.GetDateTime(rdr, i++),
                            Taxis = DatabaseUtils.GetInt(rdr, i++),
                            Hits = DatabaseUtils.GetInt(rdr, i++),
                            HitsByDay = DatabaseUtils.GetInt(rdr, i++),
                            HitsByWeek = DatabaseUtils.GetInt(rdr, i++),
                            HitsByMonth = DatabaseUtils.GetInt(rdr, i++),
                        });
                    }
                    rdr.Close();
                }
            }

            return list;
        }

        public List<Container.Content> GetContainerContentListBySqlString(string sqlString, string orderString, int totalCount, int itemsPerPage, int currentPageIndex)
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

            if (AppSettings.DatabaseType == DatabaseType.MySql)
            {
                pageSqlString = $@"
SELECT {Container.Content.SqlColumns} FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.SqlServer)
            {
                pageSqlString = $@"
SELECT {Container.Content.SqlColumns} FROM (
    SELECT TOP {recsToRetrieve} * FROM (
        SELECT TOP {itemsPerPage * (currentPageIndex + 1)} * FROM ({sqlString}) AS t0 {orderString}
    ) AS t1 {orderStringReverse}
) AS t2 {orderString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.PostgreSql)
            {
                pageSqlString = $@"
SELECT {Container.Content.SqlColumns} FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) AS t0 {orderString} LIMIT {itemsPerPage * (currentPageIndex + 1)}
    ) AS t1 {orderStringReverse} LIMIT {recsToRetrieve}
) AS t2 {orderString}";
            }
            else if (AppSettings.DatabaseType == DatabaseType.Oracle)
            {
                pageSqlString = $@"
SELECT {Container.Content.SqlColumns} FROM (
    SELECT * FROM (
        SELECT * FROM ({sqlString}) WHERE ROWNUM <= {itemsPerPage * (currentPageIndex + 1)} {orderString}
    ) WHERE ROWNUM <= {recsToRetrieve} {orderStringReverse}
) {orderString}";
            }

            var list = new List<Container.Content>();
            var itemIndex = 0;

            using (var connection = new Connection(AppSettings.DatabaseType, AppSettings.ConnectionString))
            {
                using (var rdr = connection.ExecuteReader(pageSqlString))
                {
                    while (rdr.Read())
                    {
                        var i = 0;
                        list.Add(new Container.Content
                        {
                            ItemIndex = itemIndex++,
                            Id = DatabaseUtils.GetInt(rdr, i++),
                            SiteId = DatabaseUtils.GetInt(rdr, i++),
                            ChannelId = DatabaseUtils.GetInt(rdr, i++),
                            Top = TranslateUtils.ToBool(DatabaseUtils.GetString(rdr, i++)),
                            AddDate = DatabaseUtils.GetDateTime(rdr, i++),
                            LastEditDate = DatabaseUtils.GetDateTime(rdr, i++),
                            Taxis = DatabaseUtils.GetInt(rdr, i++),
                            Hits = DatabaseUtils.GetInt(rdr, i++),
                            HitsByDay = DatabaseUtils.GetInt(rdr, i++),
                            HitsByWeek = DatabaseUtils.GetInt(rdr, i++),
                            HitsByMonth = DatabaseUtils.GetInt(rdr, i++),
                        });
                    }
                    rdr.Close();
                }
            }

            return list;
        }
    }
}
