using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentRepository
    {
        

        public async Task<string> GetValueAsync(string tableName, int contentId, string name)
        {
            var repository = GetRepository(tableName);
            return await repository.GetAsync<string>(Q
                .Select(name)
                .Where(nameof(Content.Id), contentId)
            );
        }

        public string GetStlSqlStringCheckedBySearch(string tableName, int startNum, int totalNum, string orderByString, string whereString)
        {
            var sqlWhereString =
                    $"WHERE (ChannelId > 0 AND IsChecked = '{true}' {whereString})";

            if (!string.IsNullOrEmpty(tableName))
            {
                return DataProvider.DatabaseRepository.GetPageSqlString(tableName, Utilities.ToString(ContentAttribute.AllAttributes.Value), sqlWhereString, orderByString, startNum - 1, totalNum);
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
                whereBuilder.Append($" AND {nameof(Content.Top)} = {isTop.ToString().ToLower()} ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {nameof(Content.Recommend)} = {isRecommend.ToString().ToLower()} ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {nameof(Content.Hot)} = {isHot.ToString().ToLower()} ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {nameof(Content.Color)} = {isColor.ToString().ToLower()} ");
            }

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groups = Utilities.GetStringList(group);
                if (groups.Count > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroup in groups)
                    {
                        var trimGroup = theGroup.Trim();

                        whereBuilder.Append(
                                $" ({nameof(Content.GroupNames)} = '{AttackUtils.FilterSql(trimGroup)}' OR {SqlUtils.GetInStr(nameof(Content.GroupNames), trimGroup + ",")} OR {SqlUtils.GetInStr(nameof(Content.GroupNames), "," + trimGroup + ",")} OR {SqlUtils.GetInStr(nameof(Content.GroupNames), "," + trimGroup)}) OR ");
                    }
                    whereBuilder.Length = whereBuilder.Length - 3;
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNots = Utilities.GetStringList(groupNot);
                if (groupNots.Count > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroupNot in groupNots)
                    {
                        var trimGroup = theGroupNot.Trim();
                        //whereBuilder.Append(
                        //    $" ({ContentAttribute.GroupNameCollection} <> '{trimGroup}' AND CHARINDEX('{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup}',{ContentAttribute.GroupNameCollection}) = 0) AND ");

                        whereBuilder.Append(
                                $" ({nameof(Content.GroupNames)} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(nameof(Content.GroupNames), trimGroup + ",")} AND {SqlUtils.GetNotInStr(nameof(Content.GroupNames), "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(nameof(Content.GroupNames), "," + trimGroup)}) AND ");
                    }
                    whereBuilder.Length = whereBuilder.Length - 4;
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(tags))
            {
                tags = tags.Trim().Trim(',');
                var tagNames = Utilities.GetStringList(tags);
                if (tagNames.Count > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var tagName in tagNames)
                    {
                        whereBuilder.Append(
                            $" ({nameof(Content.TagNames)} = '{AttackUtils.FilterSql(tagName)}' OR {SqlUtils.GetInStr(nameof(Content.TagNames), tagName + ",")} OR {SqlUtils.GetInStr(nameof(Content.TagNames), "," + tagName + ",")} OR {SqlUtils.GetInStr(nameof(Content.TagNames), "," + tagName)}) OR ");
                    }
                    whereBuilder.Length = whereBuilder.Length - 3;
                    whereBuilder.Append(") ");
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
                whereBuilder.Append($" AND {nameof(Content.Top)} = {isTop.ToString().ToLower()} ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {nameof(Content.Recommend)} = {isRecommend.ToString().ToLower()} ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {nameof(Content.Hot)} = {isHot.ToString().ToLower()} ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {nameof(Content.Color)} = {isColor.ToString().ToLower()} ");
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
                                $" ({nameof(Content.GroupNames)} = '{AttackUtils.FilterSql(trimGroup)}' OR {SqlUtils.GetInStr(nameof(Content.GroupNames), trimGroup + ",")} OR {SqlUtils.GetInStr(nameof(Content.GroupNames), "," + trimGroup + ",")} OR {SqlUtils.GetInStr(nameof(Content.GroupNames), "," + trimGroup)}) OR ");
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
                                $" ({nameof(Content.GroupNames)} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(nameof(Content.GroupNames), trimGroup + ",")} AND {SqlUtils.GetNotInStr(nameof(Content.GroupNames), "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(nameof(Content.GroupNames), "," + trimGroup)}) AND ");
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
                                $" ({nameof(Content.GroupNames)} = '{AttackUtils.FilterSql(trimGroup)}' OR {SqlUtils.GetInStr(nameof(Content.GroupNames), trimGroup + ",")} OR {SqlUtils.GetInStr(nameof(Content.GroupNames), "," + trimGroup + ",")} OR {SqlUtils.GetInStr(nameof(Content.GroupNames), "," + trimGroup)}) OR ");
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
                                $" ({nameof(Content.GroupNames)} <> '{theGroupNot.Trim()}' AND {SqlUtils.GetNotInStr(nameof(Content.GroupNames), theGroupNot.Trim() + ",")} AND {SqlUtils.GetNotInStr(nameof(Content.GroupNames), "," + theGroupNot.Trim() + ",")} AND {SqlUtils.GetNotInStr(nameof(Content.GroupNames), "," + theGroupNot.Trim())}) AND ");
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
                tags = tags.Trim().Trim(',');
                var tagNames = Utilities.GetStringList(tags);
                if (tagNames.Count > 0)
                {
                    whereStringBuilder.Append(" AND (");
                    foreach (var tagName in tagNames)
                    {
                        whereStringBuilder.Append(
                            $" ({nameof(Content.TagNames)} = '{AttackUtils.FilterSql(tagName)}' OR {SqlUtils.GetInStr(nameof(Content.TagNames), tagName + ",")} OR {SqlUtils.GetInStr(nameof(Content.TagNames), "," + tagName + ",")} OR {SqlUtils.GetInStr(nameof(Content.TagNames), "," + tagName)}) OR ");
                    }
                    whereStringBuilder.Length = whereStringBuilder.Length - 3;
                    whereStringBuilder.Append(") ");
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
                whereString += $" AND {nameof(Content.Checked)} = {true.ToString().ToLower()}";
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
                $"SELECT COUNT(*) FROM {tableName} WHERE {ContentAttribute.ChannelId} = {channelId} AND {nameof(Content.Checked)} = {true.ToString().ToLower()} AND Taxis < (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND {ContentAttribute.SourceId} != {SourceManager.Preview}";

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
                $"SELECT COUNT(*) FROM {tableName} WHERE {ContentAttribute.ChannelId} = {channelId} AND {ContentAttribute.ImageUrl} != '' AND {nameof(Content.Checked)} = {true.ToString().ToLower()} AND {ContentAttribute.SourceId} != {SourceManager.Preview}";

            return DataProvider.DatabaseRepository.GetIntResult(sqlString);
        }
    }
}
