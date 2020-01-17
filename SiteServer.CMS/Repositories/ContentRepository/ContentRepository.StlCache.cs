using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Core;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentRepository
    {
        public async Task<string> GetStlSqlStringCheckedAsync(string tableName, int siteId, int channelId, int startNum, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(GetStlSqlStringCheckedAsync),
                    tableName, siteId.ToString(), channelId.ToString(), startNum.ToString(),
                    totalNum.ToString(), orderByString, whereString, EScopeTypeUtils.GetValue(scopeType), groupChannel,
                    groupChannelNot);
            var retVal = StlCacheManager.Get<string>(cacheKey);
            if (retVal != null) return retVal;

            retVal = StlCacheManager.Get<string>(cacheKey);
            if (retVal == null)
            {
                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                var channelIdList = await ChannelManager.GetChannelIdListAsync(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
                retVal = GetStlSqlStringChecked(channelIdList, tableName, siteId, channelId, startNum,
                    totalNum, orderByString, whereString, scopeType, groupChannel, groupChannelNot);
                StlCacheManager.Set(cacheKey, retVal);
            }

            return retVal;
        }

        public async Task<int> GetCountOfContentAddAsync(string tableName, int siteId, int channelId, EScopeType scope, DateTime begin, DateTime end, string userName, ETriState checkedState)
        {
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            var channelIdList = await ChannelManager.GetChannelIdListAsync(channelInfo, scope, string.Empty, string.Empty, string.Empty);
            return GetCountOfContentAdd(tableName, siteId, channelIdList, begin, end, userName, checkedState);
        }

        public List<int> GetContentIdListChecked(string tableName, int channelId, string orderByFormatString)
        {
            return GetContentIdListChecked(tableName, channelId, orderByFormatString, string.Empty);
        }

        public async Task<string> GetValueAsync(string tableName, int contentId, string name)
        {
            var repository = GetRepository(tableName);
            return await repository.GetAsync<string>(Q
                .Select(name)
                .Where(nameof(Content.Id), contentId)
            );
        }

        //public Tuple<int, string> GetChannelIdAndValue(string tableName, int contentId, string name)
        //{
        //    Tuple<int, string> tuple = null;

        //    try
        //    {
        //        var sqlString = $"SELECT {ContentAttribute.ChannelId}, {name} FROM {tableName} WHERE Id = {contentId}";

        //        using (var conn = GetConnection())
        //        {
        //            conn.Open();
        //            using (var rdr = ExecuteReader(conn, sqlString))
        //            {
        //                if (rdr.Read())
        //                {
        //                    var channelId = GetInt(rdr, 0);
        //                    var value = GetString(rdr, 1);

        //                    tuple = new Tuple<int, string>(channelId, value);
        //                }

        //                rdr.Close();
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        // ignored
        //    }

        //    return tuple;
        //}

        public string GetStlSqlStringCheckedBySearch(string tableName, int startNum, int totalNum, string orderByString, string whereString)
        {
            var sqlWhereString =
                    $"WHERE (ChannelId > 0 AND IsChecked = '{true}' {whereString})";

            if (!string.IsNullOrEmpty(tableName))
            {
                //return DataProvider.DatabaseRepository.GetSelectSqlString(tableName, startNum, totalNum, TranslateUtils.ObjectCollectionToString(ContentAttribute.AllAttributes.Value), sqlWhereString, orderByString);
                return DataProvider.DatabaseRepository.GetPageSqlString(tableName, TranslateUtils.ObjectCollectionToString(ContentAttribute.AllAttributes.Value), sqlWhereString, orderByString, startNum - 1, totalNum);
            }
            return string.Empty;
        }

        public async Task<string> GetStlWhereStringAsync(int siteId, string group, string groupNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var whereBuilder = new StringBuilder();
            whereBuilder.Append($" AND SiteId = {siteId} ");

            if (isImageExists)
            {
                whereBuilder.Append(isImage
                    ? $" AND {ContentAttribute.ImageUrl} <> '' "
                    : $" AND {ContentAttribute.ImageUrl} = '' ");
            }

            if (isVideoExists)
            {
                whereBuilder.Append(isVideo
                    ? $" AND {ContentAttribute.VideoUrl} <> '' "
                    : $" AND {ContentAttribute.VideoUrl} = '' ");
            }

            if (isFileExists)
            {
                whereBuilder.Append(isFile
                    ? $" AND {ContentAttribute.FileUrl} <> '' "
                    : $" AND {ContentAttribute.FileUrl} = '' ");
            }

            if (isTopExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsTop} = '{isTop}' ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsRecommend} = '{isRecommend}' ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsHot} = '{isHot}' ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsColor} = '{isColor}' ");
            }

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr != null && groupArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroup in groupArr)
                    {
                        var trimGroup = theGroup.Trim();

                        whereBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} = '{AttackUtils.FilterSql(trimGroup)}' OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup)}) OR ");
                    }
                    if (groupArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 3;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr != null && groupNotArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroupNot in groupNotArr)
                    {
                        var trimGroup = theGroupNot.Trim();
                        //whereBuilder.Append(
                        //    $" ({ContentAttribute.GroupNameCollection} <> '{trimGroup}' AND CHARINDEX('{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup}',{ContentAttribute.GroupNameCollection}) = 0) AND ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + trimGroup)}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 4;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(tags))
            {
                var tagCollection = ContentTagUtils.ParseTagsString(tags);
                var contentIdArrayList = await DataProvider.ContentTagRepository.GetContentIdListByTagCollectionAsync(tagCollection, siteId);
                if (contentIdArrayList.Count > 0)
                {
                    whereBuilder.Append(
                        $" AND (ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdArrayList)}))");
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                whereBuilder.Append($" AND ({where}) ");
            }

            return whereBuilder.ToString();
        }

        public string GetStlWhereStringBySearch(string group, string groupNot, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var whereBuilder = new StringBuilder();

            if (isImageExists)
            {
                whereBuilder.Append(isImage
                    ? $" AND {ContentAttribute.ImageUrl} <> '' "
                    : $" AND {ContentAttribute.ImageUrl} = '' ");
            }

            if (isVideoExists)
            {
                whereBuilder.Append(isVideo
                    ? $" AND {ContentAttribute.VideoUrl} <> '' "
                    : $" AND {ContentAttribute.VideoUrl} = '' ");
            }

            if (isFileExists)
            {
                whereBuilder.Append(isFile
                    ? $" AND {ContentAttribute.FileUrl} <> '' "
                    : $" AND {ContentAttribute.FileUrl} = '' ");
            }

            if (isTopExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsTop} = '{isTop}' ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsRecommend} = '{isRecommend}' ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsHot} = '{isHot}' ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsColor} = '{isColor}' ");
            }

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr != null && groupArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroup in groupArr)
                    {
                        var trimGroup = theGroup.Trim();

                        whereBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} = '{AttackUtils.FilterSql(trimGroup)}' OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup)}) OR ");
                    }
                    if (groupArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 3;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr != null && groupNotArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroupNot in groupNotArr)
                    {
                        var trimGroup = theGroupNot.Trim();
                        //whereBuilder.Append(
                        //    $" ({ContentAttribute.GroupNameCollection} <> '{trimGroup}' AND CHARINDEX('{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup}',{ContentAttribute.GroupNameCollection}) = 0) AND ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + trimGroup)}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 4;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                whereBuilder.Append($" AND ({where}) ");
            }

            return whereBuilder.ToString();
        }

        public async Task<string> GetStlWhereStringAsync(int siteId, string group, string groupNot, string tags, bool isTopExists, bool isTop, string where)
        {
            var whereStringBuilder = new StringBuilder();

            if (isTopExists)
            {
                whereStringBuilder.Append($" AND IsTop = '{isTop}' ");
            }

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr.Length > 0)
                {
                    whereStringBuilder.Append(" AND (");
                    foreach (var theGroup in groupArr)
                    {
                        var trimGroup = theGroup.Trim();

                        whereStringBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} = '{AttackUtils.FilterSql(trimGroup)}' OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup)}) OR ");
                    }
                    if (groupArr.Length > 0)
                    {
                        whereStringBuilder.Length = whereStringBuilder.Length - 3;
                    }
                    whereStringBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr.Length > 0)
                {
                    whereStringBuilder.Append(" AND (");
                    foreach (var theGroupNot in groupNotArr)
                    {
                        //whereStringBuilder.Append(
                        //    $" ({ContentAttribute.GroupNameCollection} <> '{theGroupNot.Trim()}' AND CHARINDEX('{theGroupNot.Trim()},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{theGroupNot.Trim()},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{theGroupNot.Trim()}',{ContentAttribute.GroupNameCollection}) = 0) AND ");

                        whereStringBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} <> '{theGroupNot.Trim()}' AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, theGroupNot.Trim() + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + theGroupNot.Trim() + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + theGroupNot.Trim())}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereStringBuilder.Length = whereStringBuilder.Length - 4;
                    }
                    whereStringBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(tags))
            {
                var tagList = ContentTagUtils.ParseTagsString(tags);
                var contentIdList = await DataProvider.ContentTagRepository.GetContentIdListByTagCollectionAsync(tagList, siteId);
                if (contentIdList.Count > 0)
                {
                    var inString = TranslateUtils.ToSqlInStringWithoutQuote(contentIdList);
                    whereStringBuilder.Append($" AND (Id IN ({inString}))");
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                whereStringBuilder.Append($" AND ({where}) ");
            }

            return whereStringBuilder.ToString();
        }

        public int GetContentId(string tableName, int channelId, int taxis, bool isNextContent)
        {
            var contentId = 0;
            var sqlString = SqlUtils.ToTopSqlString(tableName, "Id", $"WHERE (ChannelId = {channelId} AND Taxis > {taxis} AND IsChecked = 'True')", "ORDER BY Taxis", 1);
            if (isNextContent)
            {
                sqlString = SqlUtils.ToTopSqlString(tableName, "Id",
                $"WHERE (ChannelId = {channelId} AND Taxis < {taxis} AND IsChecked = 'True')", "ORDER BY Taxis DESC", 1);
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    contentId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return contentId;
        }

        public int GetContentId(string tableName, int channelId, bool isCheckedOnly, string orderByString)
        {
            var contentId = 0;
            var whereString = $"WHERE {ContentAttribute.ChannelId} = {channelId}";
            if (isCheckedOnly)
            {
                whereString += $" AND {ContentAttribute.IsChecked} = '{true.ToString()}'";
            }
            var sqlString = SqlUtils.ToTopSqlString(tableName, "Id", whereString, orderByString, 1);

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    contentId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return contentId;
        }

        public int GetChannelId(string tableName, int contentId)
        {
            var channelId = 0;
            var sqlString = $"SELECT {ContentAttribute.ChannelId} FROM {tableName} WHERE (Id = {contentId})";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    channelId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return channelId;
        }

        public int GetSequence(string tableName, int channelId, int contentId)
        {
            var sqlString =
                $"SELECT COUNT(*) FROM {tableName} WHERE {ContentAttribute.ChannelId} = {channelId} AND {ContentAttribute.IsChecked} = '{true}' AND Taxis < (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND {ContentAttribute.SourceId} != {SourceManager.Preview}";

            return DataProvider.DatabaseRepository.GetIntResult(sqlString) + 1;
        }

        public async Task<DataSet> GetStlDataSourceCheckedAsync(List<int> channelIdList, string tableName, int startNum, int totalNum, string orderByString, string whereString, NameValueCollection others)
        {
            return await GetStlDataSourceCheckedAsync(tableName, channelIdList, startNum, totalNum, orderByString, whereString, others);
        }

        public async Task<int> GetCountCheckedImageAsync(int siteId, int channelId)
        {
            var tableName = await DataProvider.SiteRepository.GetTableNameAsync(siteId);
            var sqlString =
                $"SELECT COUNT(*) FROM {tableName} WHERE {ContentAttribute.ChannelId} = {channelId} AND {ContentAttribute.ImageUrl} != '' AND {ContentAttribute.IsChecked} = '{true}' AND {ContentAttribute.SourceId} != {SourceManager.Preview}";

            return DataProvider.DatabaseRepository.GetIntResult(sqlString);
        }
    }
}
