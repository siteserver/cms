using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SqlKata;
using SS.CMS.Core.Common;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;
using Attr = SS.CMS.Core.Models.Attributes.ContentAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public async Task<IEnumerable<int>> GetContentIdListAsync(int channelId)
        {
            return await _repository.GetAllAsync<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, channelId)
            );
        }

        public async Task<IEnumerable<int>> GetContentIdListAsync(int channelId, bool isPeriods, string dateFrom, string dateTo, bool? checkedState)
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

            if (checkedState.HasValue)
            {
                query.Where(Attr.IsChecked, checkedState);
            }

            return await _repository.GetAllAsync<int>(query);
        }

        public async Task<IEnumerable<int>> GetContentIdListCheckedByChannelIdAsync(int siteId, int channelId)
        {
            return await _repository.GetAllAsync<int>(Q
                .Select(Attr.Id)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.IsChecked, true.ToString())
            );
        }

        public async Task<IEnumerable<(int, int)>> GetContentIdListByTrashAsync(int siteId)
        {
            var list = await _repository.GetAllAsync<(int contentId, int channelId)>(Q
                .Select(Attr.Id, Attr.ChannelId)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, "<", 0));

            return list.Select(o => (Math.Abs(o.channelId), o.contentId));
        }

        public async Task<IEnumerable<int>> GetContentIdListCheckedAsync(int channelId, TaxisType taxisType)
        {
            return await GetContentIdListCheckedAsync(channelId, 0, taxisType);
        }

        private async Task<IEnumerable<int>> GetContentIdListCheckedAsync(int channelId, int totalNum, TaxisType taxisType)
        {
            var channelIdList = new List<int>
            {
                channelId
            };
            return await GetContentIdListCheckedAsync(channelIdList, totalNum, taxisType);
        }

        private async Task<IEnumerable<int>> GetContentIdListCheckedAsync(List<int> channelIdList, int totalNum, TaxisType taxisType)
        {
            var list = new List<int>();

            if (channelIdList == null || channelIdList.Count == 0)
            {
                return list;
            }

            var query = Q
                .Select(Attr.Id)
                .WhereIn(Attr.ChannelId, channelIdList)
                .Where(Attr.IsChecked, true);

            QueryOrder(query, taxisType);

            if (totalNum > 0)
            {
                query.Limit(totalNum);
            }

            return await _repository.GetAllAsync<int>(query);
        }

        // public List<Tuple<int, int>> ApiGetContentIdListBySiteId(int siteId, ApiContentsParameters parameters, out int totalCount)
        // {
        //     totalCount = 0;
        //     var list = new List<ContentInfo>();

        //     var query = MinColumnsQuery
        //         .Where(Attr.SiteId, siteId)
        //         .Where(Attr.ChannelId, ">", 0)
        //         .Where(Attr.IsChecked, true);

        //     if (parameters.ChannelIds.Count > 0)
        //     {
        //         query.WhereIn(Attr.ChannelId, parameters.ChannelIds);
        //     }
        //     if (!string.IsNullOrEmpty(parameters.ChannelGroup))
        //     {
        //         var channelIdList = _channelRepository.GetChannelIdList(siteId, parameters.ChannelGroup);
        //         if (channelIdList.Count > 0)
        //         {
        //             query.WhereIn(Attr.ChannelId, channelIdList);
        //         }
        //     }
        //     if (!string.IsNullOrEmpty(parameters.ContentGroup))
        //     {
        //         var contentGroup = parameters.ContentGroup;

        //         query.Where(q =>
        //         {
        //             return q
        //                 .Where(Attr.GroupNameCollection, contentGroup)
        //                 .OrWhereInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, contentGroup + ",")
        //                 .OrWhereInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, "," + contentGroup + ",")
        //                 .OrWhereInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, "," + contentGroup)
        //                 ;
        //         });
        //     }
        //     if (!string.IsNullOrEmpty(parameters.Tag))
        //     {
        //         var tag = parameters.Tag;

        //         query.Where(q =>
        //         {
        //             return q
        //                 .Where(Attr.Tags, tag)
        //                 .OrWhereInStr(_settingsManager.DatabaseType, Attr.Tags, tag + ",")
        //                 .OrWhereInStr(_settingsManager.DatabaseType, Attr.Tags, "," + tag + ",")
        //                 .OrWhereInStr(_settingsManager.DatabaseType, Attr.Tags, "," + tag)
        //                 ;
        //         });
        //     }

        //     var channelInfo = _channelRepository.GetChannelInfo(siteId, siteId);

        //     AddOrderByString(query, channelInfo, parameters.OrderBy);

        //     if (parameters.QueryString != null && parameters.QueryString.Count > 0)
        //     {
        //         var columnNameList = _tableManager.GetTableColumnNameList(TableName);

        //         foreach (string attributeName in parameters.QueryString)
        //         {
        //             if (!StringUtils.ContainsIgnoreCase(columnNameList, attributeName)) continue;

        //             var value = parameters.QueryString[attributeName];

        //             if (StringUtils.EqualsIgnoreCase(attributeName, Attr.IsChecked) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsColor) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsHot) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsRecommend) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsTop))
        //             {
        //                 query.Where(attributeName, TranslateUtils.ToBool(value));
        //             }
        //             else if (StringUtils.EqualsIgnoreCase(attributeName, Attr.Id) || StringUtils.EqualsIgnoreCase(attributeName, Attr.ReferenceId) || StringUtils.EqualsIgnoreCase(attributeName, Attr.SourceId) || StringUtils.EqualsIgnoreCase(attributeName, Attr.CheckedLevel))
        //             {
        //                 query.Where(attributeName, TranslateUtils.ToInt(value));
        //             }
        //             else if (parameters.Likes.Contains(attributeName))
        //             {
        //                 query.WhereLike(attributeName, value);
        //             }
        //             else
        //             {
        //                 query.Where(attributeName, attributeName);
        //             }
        //         }
        //     }

        //     totalCount = _repository.Count(query);

        //     if (totalCount > 0 && parameters.Skip < totalCount)
        //     {
        //         query.Skip(parameters.Skip).Limit(parameters.Top);

        //         list = _repository.GetAll<ContentInfo>(query).ToList();
        //     }

        //     var tupleList = new List<Tuple<int, int>>();
        //     foreach (var contentInfo in list)
        //     {
        //         tupleList.Add(new Tuple<int, int>(contentInfo.ChannelId, contentInfo.Id));
        //     }

        //     return tupleList;
        // }

        //public async Task<IList<(int, int)>> ApiGetContentIdListByChannelIdAsync(int siteId, int channelId, int top, int skip, string like, string orderBy, NameValueCollection queryString)
        //{
        //    var retVal = new List<(int, int)>();

        //    var channelInfo = await _channelRepository.GetChannelInfoAsync(siteId, channelId);
        //    var channelIdList = await _channelRepository.GetChannelIdListAsync(channelInfo, ScopeType.All, string.Empty, string.Empty, string.Empty);

        //    var query = MinColumnsQuery.Where(Attr.SiteId, siteId).WhereIn(Attr.ChannelId, channelIdList).WhereTrue(Attr.IsChecked);
        //    QueryOrder(query, channelInfo, orderBy);

        //    var likeList = TranslateUtils.StringCollectionToStringList(StringUtils.TrimAndToLower(like));
        //    if (queryString != null && queryString.Count > 0)
        //    {
        //        var columnNameList = _tableManager.GetTableColumnNameList(TableName);

        //        foreach (string attributeName in queryString)
        //        {
        //            if (!StringUtils.ContainsIgnoreCase(columnNameList, attributeName)) continue;

        //            var value = queryString[attributeName];

        //            if (StringUtils.EqualsIgnoreCase(attributeName, Attr.IsChecked) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsColor) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsHot) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsRecommend) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsTop))
        //            {
        //                query.Where(attributeName, TranslateUtils.ToBool(value));
        //            }
        //            else if (StringUtils.EqualsIgnoreCase(attributeName, Attr.Id) || StringUtils.EqualsIgnoreCase(attributeName, Attr.ReferenceId) || StringUtils.EqualsIgnoreCase(attributeName, Attr.SourceId) || StringUtils.EqualsIgnoreCase(attributeName, Attr.CheckedLevel))
        //            {
        //                query.Where(attributeName, TranslateUtils.ToInt(value));
        //            }
        //            else if (likeList.Contains(attributeName))
        //            {
        //                query.WhereLike(attributeName, value);
        //            }
        //            else
        //            {
        //                query.WhereLike(attributeName, value);
        //            }
        //        }
        //    }

        //    totalCount = _repository.Count(query);
        //    if (totalCount > 0 && skip < totalCount)
        //    {
        //        retVal = _repository.GetAll(query.Skip(skip).Limit(top)).Select(o => (o.ChannelId, o.Id)).ToList();
        //    }

        //    return retVal;
        //}

        private async Task<IEnumerable<int>> GetReferenceIdListAsync(IEnumerable<int> contentIdList)
        {
            return await _repository.GetAllAsync<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, ">", 0)
                .WhereIn(Attr.ReferenceId, contentIdList)
            );
        }

        public async Task<IEnumerable<int>> GetIdListBySameTitleAsync(int channelId, string title)
        {
            return await _repository.GetAllAsync<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.Title, title)
            );
        }

        public async Task<IEnumerable<int>> GetChannelIdListCheckedByLastEditDateHourAsync(int siteId, int hour)
        {
            return await _repository.GetAllAsync<int>(Q
                .Select(Attr.ChannelId)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.IsChecked, true.ToString())
                .WhereBetween(Attr.LastModifiedDate, DateTime.Now.AddHours(-hour), DateTime.Now)
                .Distinct()
            );
        }

        public async Task<IEnumerable<int>> GetCacheContentIdListAsync(Query query, int offset, int limit)
        {
            var list = new List<int>();

            QuerySelectMinColumns(query);
            query.Offset(offset).Limit(limit);

            return await _repository.GetAllAsync<int>(query);
        }

        public async Task<IEnumerable<string>> GetValueListByStartStringAsync(int channelId, string name, string startString, int totalNum)
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(name)
                .Where(Attr.ChannelId, channelId)
                .WhereInStr(_repository.Database.DatabaseType, name, startString)
                .Distinct()
                .Limit(totalNum)
            );
        }

        public async Task<IEnumerable<ContentInfo>> GetContentInfoListAsync(Query query, int offset, int limit)
        {
            return await _repository.GetAllAsync(query
                .Offset(offset)
                .Limit(limit)
            );
        }

        public async Task<IEnumerable<ContentCountInfo>> GetContentCountInfoListAsync()
        {
            var sqlString =
                $@"SELECT {Attr.SiteId}, {Attr.ChannelId}, {Attr.IsChecked}, {Attr.CheckedLevel}, {Attr.UserId}, COUNT(*) AS {nameof(ContentCountInfo.Count)} FROM {TableName} WHERE {Attr.ChannelId} > 0 AND {Attr.SourceId} != {SourceManager.Preview} GROUP BY {Attr.SiteId}, {Attr.ChannelId}, {Attr.IsChecked}, {Attr.CheckedLevel}, {Attr.UserId}";

            using (var connection = _repository.Database.GetConnection())
            {
                return await connection.QueryAsync<ContentCountInfo>(sqlString);
            }
        }
    }
}
