using System;
using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;
using Attr = SiteServer.CMS.Database.Attributes.ContentAttribute;

namespace SiteServer.CMS.Database.Repositories.Contents
{
    public partial class ContentTableRepository
    {
        public IList<(int, int)> GetContentIdListByTrash(int siteId)
        {

            var list = GetAll<ContentInfo>(Q
                .Select(Attr.Id, Attr.ChannelId)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, "<", 0));

            return list.Select(o => (Math.Abs(o.ChannelId), o.Id)).ToList();
        }

        private IList<int> GetReferenceIdList(IList<int> contentIdList)
        {
            //var list = new List<int>();
            //var sqlString =
            //    $"SELECT Id FROM {tableName} WHERE ChannelId > 0 AND ReferenceId IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(DatabaseApi.GetInt(rdr, 0));
            //    }
            //    rdr.Close();
            //}

            //return list;

            return GetAll<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, ">", 0)
                .WhereIn(Attr.ReferenceId, contentIdList)
            );
        }

        public IList<int> GetContentIdList(int channelId)
        {
            //var list = new List<int>();

            //var sqlString = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId}";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var contentId = DatabaseApi.GetInt(rdr, 0);
            //        list.Add(contentId);
            //    }
            //    rdr.Close();
            //}
            //return list;

            return GetAll<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, channelId)
            );
        }

        private IList<int> GetContentIdListChecked(ICollection<int> channelIdList, int totalNum = 0)
        {
            var list = new List<int>();

            if (channelIdList == null || channelIdList.Count == 0)
            {
                return list;
            }

            var query = Q
                .Select(Attr.Id)
                .Where(Attr.IsChecked, true.ToString())
                .WhereIn(Attr.ChannelId, channelIdList);

            if (totalNum > 0)
            {
                query.Limit(totalNum);
            }

            return GetAll<int>(query);

            //string sqlString;

            //if (totalNum > 0)
            //{
            //    sqlString = SqlDifferences.GetSqlString(tableName, new List<string>
            //        {
            //            Attr.Id
            //        },
            //        channelIdList.Count == 1
            //            ? $"WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString})"
            //            : $"WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString})", orderString,
            //        totalNum);
            //}
            //else
            //{
            //    sqlString = channelIdList.Count == 1
            //        ? $"SELECT Id FROM {tableName} WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString}) {orderString}"
            //        : $"SELECT Id FROM {tableName} WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString}) {orderString}";
            //}

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var contentId = DatabaseApi.GetInt(rdr, 0);
            //        list.Add(contentId);
            //    }
            //    rdr.Close();
            //}
            //return list;
        }

        private IList<int> GetContentIdListChecked(int channelId, int totalNum = 0)
        {
            var query = Q
                .Select(Attr.Id)
                .Where(Attr.IsChecked, true.ToString())
                .Where(Attr.ChannelId, channelId);

            if (totalNum > 0)
            {
                query.Limit(totalNum);
            }
            return GetAll<int>(query);

            //string sqlString;

            //if (totalNum > 0)
            //{
            //    sqlString = SqlDifferences.GetSqlString(tableName, new List<string>
            //        {
            //            Attr.Id
            //        },
            //        channelIdList.Count == 1
            //            ? $"WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString})"
            //            : $"WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString})", orderString,
            //        totalNum);
            //}
            //else
            //{
            //    sqlString = channelIdList.Count == 1
            //        ? $"SELECT Id FROM {tableName} WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString}) {orderString}"
            //        : $"SELECT Id FROM {tableName} WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString}) {orderString}";
            //}

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var contentId = DatabaseApi.GetInt(rdr, 0);
            //        list.Add(contentId);
            //    }
            //    rdr.Close();
            //}
            //return list;
        }

        public int GetTotalHits(int siteId)
        {
            //return DatabaseApi.GetIntResult($"SELECT SUM(Hits) FROM {tableName} WHERE IsChecked='{true}' AND SiteId = {siteId} AND Hits > 0");

            return Sum(Q
                .Select(Attr.Hits)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.IsChecked, true.ToString())
                .Where(Attr.Hits, ">", 0)
            );
        }

        public int GetFirstContentId(int siteId, int channelId)
        {
            //var sqlString = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId} ORDER BY Taxis DESC, Id DESC";
            //return DatabaseApi.GetIntResult(sqlString);

            return Get<int>(Q
                .Select(Attr.Id)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
                .OrderByDesc(Attr.Taxis, Attr.Id)
            );
        }

        private int GetTaxis(int contentId)
        {
            return Get<int>(Q
                .Select(Attr.Taxis)
                .Where(Attr.Id, contentId)
            );

            //var sqlString = $"SELECT Taxis FROM {tableName} WHERE (Id = {selectedId})";

            //return DatabaseApi.GetIntResult(sqlString);
        }

        public int GetTaxisToInsert(int channelId, bool isTop)
        {
            int taxis;

            if (isTop)
            {
                taxis = GetMaxTaxis(channelId, true) + 1;
            }
            else
            {
                taxis = GetMaxTaxis(channelId, false) + 1;
            }

            return taxis;
        }

        public int GetMaxTaxis(int channelId, bool isTop)
        {
            var maxTaxis = 0;

            if (isTop)
            {
                maxTaxis = TaxisIsTopStartValue;

                var max = Max(Q
                    .Select(Attr.Taxis)
                    .Where(Attr.ChannelId, channelId)
                    .Where(Attr.Taxis, ">=", TaxisIsTopStartValue)
                );

                if (max != null)
                {
                    maxTaxis = max.Value;
                }

                if (maxTaxis < TaxisIsTopStartValue)
                {
                    maxTaxis = TaxisIsTopStartValue;
                }
            }
            else
            {
                var max = Max(Q
                    .Select(Attr.Taxis)
                    .Where(Attr.ChannelId, channelId)
                    .Where(Attr.Taxis, "<", TaxisIsTopStartValue)
                );

                if (max != null)
                {
                    maxTaxis = max.Value;
                }
            }

            return maxTaxis;
        }

        public bool GetChanelIdAndValue<T>(int contentId, string name, out int channelId, out T value)
        {
            channelId = 0;
            value = default(T);

            var tuple = Get<(int ChannelId, T Result)?>(Q
                .Select(Attr.ChannelId, name)
                .Where(Attr.Id, contentId)
            );
            if (tuple == null) return false;

            channelId = tuple.Value.ChannelId;
            value = tuple.Value.Result;

            return true;
            //Tuple<int, string> tuple = null;

            //try
            //{
            //    var sqlString = $"SELECT {Attr.ChannelId}, {name} FROM {tableName} WHERE Id = {contentId}";

            //    using (var conn = GetConnection())
            //    {
            //        conn.Open();
            //        using (var rdr = DatabaseApi.ExecuteReader(conn, sqlString))
            //        {
            //            if (rdr.Read())
            //            {
            //                var channelId = DatabaseApi.GetInt(rdr, 0);
            //                var value = DatabaseApi.GetString(rdr, 1);

            //                tuple = new Tuple<int, string>(channelId, value);
            //            }

            //            rdr.Close();
            //        }
            //    }
            //}
            //catch
            //{
            //    // ignored
            //}

            //return tuple;
        }

        public T GetValue<T>(int contentId, string name)
        {
            return Get<T>(Q
                .Select(name)
                .Where(Attr.Id, contentId)
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
                    query.Where(Attr.AddDate, ">=", SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom)));
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    query.Where(Attr.AddDate, "<=", SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo).AddDays(1)));
                }
            }

            if (checkedState != ETriState.All)
            {
                query.Where(Attr.IsChecked, ETriStateUtils.GetValue(checkedState));
            }

            return GetAll<int>(query);
        }

        public IList<int> GetContentIdListCheckedByChannelId(int siteId, int channelId)
        {
            return GetAll<int>(Q
                .Select(Attr.Id)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.IsChecked, true.ToString())
            );
        }

        public int GetContentId(int channelId, int taxis, bool isNextContent)
        {
            if (isNextContent)
            {
                return Get<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.ChannelId, channelId)
                    .Where(Attr.Taxis, "<", taxis)
                    .Where(Attr.IsChecked, true.ToString())
                    .OrderByDesc(Attr.Taxis));
            }

            return Get<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.Taxis, ">", taxis)
                .Where(Attr.IsChecked, true.ToString())
                .OrderBy(Attr.Taxis));
        }

        public int GetSequence(int channelId, int contentId)
        {
            var taxis = GetTaxis(contentId);

            return Count(Q
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.IsChecked, true.ToString())
                .Where(Attr.Taxis, "<", taxis)
                .Where(Attr.SourceId, "!=", SourceManager.Preview)
            ) + 1;
        }

        public IList<int> GetIdListBySameTitle(int channelId, string title)
        {
            return GetAll<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.Title, title)
            );
        }

        public int GetCountCheckedImage(int siteId, int channelId)
        {
            return Count(Q
                .Where(Attr.ChannelId, channelId)
                .WhereNotNull(Attr.ImageUrl)
                .Where(Attr.ImageUrl, "!=", string.Empty)
                .Where(Attr.IsChecked, true.ToString())
                .Where(Attr.SourceId, "!=", SourceManager.Preview)
            );
        }
    }
}
