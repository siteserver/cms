using System;
using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;
using Attr = SiteServer.CMS.Database.Attributes.ContentAttribute;

namespace SiteServer.CMS.Database.Repositories.Contents
{
    public partial class ContentTableRepository
    {
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

            return GetValueList<int>(Q
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

            return GetValueList<int>(Q
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

            return GetValueList<int>(query);

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
            return GetValueList<int>(query);

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

            return base.Sum(Attr.Hits, Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.IsChecked, true.ToString())
                .Where(Attr.Hits, ">", 0)
            );
        }

        public int GetFirstContentId(int siteId, int channelId)
        {
            //var sqlString = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId} ORDER BY Taxis DESC, Id DESC";
            //return DatabaseApi.GetIntResult(sqlString);

            return base.GetValue<int>(Q
                .Select(Attr.Id)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
                .OrderByDesc(Attr.Taxis, Attr.Id)
            );
        }
    }
}
