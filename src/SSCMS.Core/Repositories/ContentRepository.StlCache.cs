using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public string GetStlWhereString(int siteId, string group, string groupNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var whereBuilder = new StringBuilder();
            whereBuilder.Append($" AND SiteId = {siteId} ");

            if (isImageExists)
            {
                whereBuilder.Append(isImage
                    ? $" AND {nameof(Content.ImageUrl)} <> '' "
                    : $" AND {nameof(Content.ImageUrl)} = '' ");
            }

            if (isVideoExists)
            {
                whereBuilder.Append(isVideo
                    ? $" AND {nameof(Content.VideoUrl)} <> '' "
                    : $" AND {nameof(Content.VideoUrl)} = '' ");
            }

            if (isFileExists)
            {
                whereBuilder.Append(isFile
                    ? $" AND {nameof(Content.FileUrl)} <> '' "
                    : $" AND {nameof(Content.FileUrl)} = '' ");
            }

            if (isTopExists)
            {
                whereBuilder.Append($" AND {nameof(Content.Top)} = {StringUtils.ToLower(isTop.ToString())} ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {nameof(Content.Recommend)} = {StringUtils.ToLower(isRecommend.ToString())} ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {nameof(Content.Hot)} = {StringUtils.ToLower(isHot.ToString())} ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {nameof(Content.Color)} = {StringUtils.ToLower(isColor.ToString())} ");
            }

            var database = _settingsManager.Database;

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groups = ListUtils.GetStringList(group);
                if (groups.Count > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroup in groups)
                    {
                        var trimGroup = theGroup.Trim();

                        whereBuilder.Append(
                                $" ({nameof(Content.GroupNames)} = '{AttackUtils.FilterSql(trimGroup)}' OR {DatabaseUtils.GetInStr(database, nameof(Content.GroupNames), trimGroup + ",")} OR {DatabaseUtils.GetInStr(database, nameof(Content.GroupNames), "," + trimGroup + ",")} OR {DatabaseUtils.GetInStr(database, nameof(Content.GroupNames), "," + trimGroup)}) OR ");
                    }
                    whereBuilder.Length -= 3;
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNots = ListUtils.GetStringList(groupNot);
                if (groupNots.Count > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroupNot in groupNots)
                    {
                        var trimGroup = theGroupNot.Trim();
                        //whereBuilder.Append(
                        //    $" ({ContentAttribute.GroupNameCollection} <> '{trimGroup}' AND CHARINDEX('{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup}',{ContentAttribute.GroupNameCollection}) = 0) AND ");

                        whereBuilder.Append(
                                $" ({nameof(Content.GroupNames)} <> '{trimGroup}' AND {DatabaseUtils.GetNotInStr(database, nameof(Content.GroupNames), trimGroup + ",")} AND {DatabaseUtils.GetNotInStr(database, nameof(Content.GroupNames), "," + trimGroup + ",")} AND {DatabaseUtils.GetNotInStr(database, nameof(Content.GroupNames), "," + trimGroup)}) AND ");
                    }
                    whereBuilder.Length -= 4;
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(tags))
            {
                tags = tags.Trim().Trim(',');
                var tagNames = ListUtils.GetStringList(tags);
                if (tagNames.Count > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var tagName in tagNames)
                    {
                        whereBuilder.Append(
                            $" ({nameof(Content.TagNames)} = '{AttackUtils.FilterSql(tagName)}' OR {DatabaseUtils.GetInStr(database, nameof(Content.TagNames), tagName + ",")} OR {DatabaseUtils.GetInStr(database, nameof(Content.TagNames), "," + tagName + ",")} OR {DatabaseUtils.GetInStr(database, nameof(Content.TagNames), "," + tagName)}) OR ");
                    }
                    whereBuilder.Length -= 3;
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
                    ? $" AND {nameof(Content.ImageUrl)} <> '' "
                    : $" AND {nameof(Content.ImageUrl)} = '' ");
            }

            if (isVideoExists)
            {
                whereBuilder.Append(isVideo
                    ? $" AND {nameof(Content.VideoUrl)} <> '' "
                    : $" AND {nameof(Content.VideoUrl)} = '' ");
            }

            if (isFileExists)
            {
                whereBuilder.Append(isFile
                    ? $" AND {nameof(Content.FileUrl)} <> '' "
                    : $" AND {nameof(Content.FileUrl)} = '' ");
            }

            if (isTopExists)
            {
                whereBuilder.Append($" AND {nameof(Content.Top)} = {StringUtils.ToLower(isTop.ToString())} ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {nameof(Content.Recommend)} = {StringUtils.ToLower(isRecommend.ToString())} ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {nameof(Content.Hot)} = {StringUtils.ToLower(isHot.ToString())} ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {nameof(Content.Color)} = {StringUtils.ToLower(isColor.ToString())} ");
            }

            var database = _settingsManager.Database;

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
                                $" ({nameof(Content.GroupNames)} = '{AttackUtils.FilterSql(trimGroup)}' OR {DatabaseUtils.GetInStr(database, nameof(Content.GroupNames), trimGroup + ",")} OR {DatabaseUtils.GetInStr(database, nameof(Content.GroupNames), "," + trimGroup + ",")} OR {DatabaseUtils.GetInStr(database, nameof(Content.GroupNames), "," + trimGroup)}) OR ");
                    }
                    if (groupArr.Length > 0)
                    {
                        whereBuilder.Length -= 3;
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
                                $" ({nameof(Content.GroupNames)} <> '{trimGroup}' AND {DatabaseUtils.GetNotInStr(database, nameof(Content.GroupNames), trimGroup + ",")} AND {DatabaseUtils.GetNotInStr(database, nameof(Content.GroupNames), "," + trimGroup + ",")} AND {DatabaseUtils.GetNotInStr(database, nameof(Content.GroupNames), "," + trimGroup)}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereBuilder.Length -= 4;
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

        public string GetStlWhereString(int siteId, string group, string groupNot, string tags, bool isTopExists, bool isTop, string where)
        {
            var whereStringBuilder = new StringBuilder();

            if (isTopExists)
            {
                whereStringBuilder.Append($" AND IsTop = '{isTop}' ");
            }

            var database = Database;

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
                                $" ({nameof(Content.GroupNames)} = '{AttackUtils.FilterSql(trimGroup)}' OR {DatabaseUtils.GetInStr(database, nameof(Content.GroupNames), trimGroup + ",")} OR {DatabaseUtils.GetInStr(database, nameof(Content.GroupNames), "," + trimGroup + ",")} OR {DatabaseUtils.GetInStr(database, nameof(Content.GroupNames), "," + trimGroup)}) OR ");
                    }
                    if (groupArr.Length > 0)
                    {
                        whereStringBuilder.Length -= 3;
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
                                $" ({nameof(Content.GroupNames)} <> '{theGroupNot.Trim()}' AND {DatabaseUtils.GetNotInStr(database, nameof(Content.GroupNames), theGroupNot.Trim() + ",")} AND {DatabaseUtils.GetNotInStr(database, nameof(Content.GroupNames), "," + theGroupNot.Trim() + ",")} AND {DatabaseUtils.GetNotInStr(database, nameof(Content.GroupNames), "," + theGroupNot.Trim())}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereStringBuilder.Length -= 4;
                    }
                    whereStringBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(tags))
            {
                tags = tags.Trim().Trim(',');
                var tagNames = ListUtils.GetStringList(tags);
                if (tagNames.Count > 0)
                {
                    whereStringBuilder.Append(" AND (");
                    foreach (var tagName in tagNames)
                    {
                        whereStringBuilder.Append(
                            $" ({nameof(Content.TagNames)} = '{AttackUtils.FilterSql(tagName)}' OR {DatabaseUtils.GetInStr(database, nameof(Content.TagNames), tagName + ",")} OR {DatabaseUtils.GetInStr(database, nameof(Content.TagNames), "," + tagName + ",")} OR {DatabaseUtils.GetInStr(database, nameof(Content.TagNames), "," + tagName)}) OR ");
                    }
                    whereStringBuilder.Length -= 3;
                    whereStringBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                whereStringBuilder.Append($" AND ({where}) ");
            }

            return whereStringBuilder.ToString();
        }

        public async Task<int> GetContentIdAsync(string tableName, int channelId, int taxis, bool isNextContent)
        {
            var repository = GetRepository(tableName);
            var query = Q
                .Select(nameof(Content.Id))
                .Where(nameof(Content.ChannelId), channelId)
                .WhereTrue(nameof(Content.Checked))
                ;
            if (isNextContent)
            {
                query
                    .Where(nameof(Content.Taxis), "<", taxis)
                    .OrderByDesc(nameof(Content.Taxis));
            }
            else
            {
                query
                    .Where(nameof(Content.Taxis), ">", taxis)
                    .OrderBy(nameof(Content.Taxis));
            }

            return await repository.GetAsync<int>(query);
        }

        public int GetContentId(string tableName, int channelId, bool isCheckedOnly, string orderByString)
        {
            var contentId = 0;
            var whereString = $"WHERE {Quote(nameof(Content.ChannelId))} = {channelId}";
            if (isCheckedOnly)
            {
                whereString += $" AND {Quote(nameof(Content.Checked))} = {DatabaseUtils.ToSqlBool(Database.DatabaseType, true)}";
            }
            var sqlString = DatabaseUtils.ToTopSqlString(Database, tableName, "Id", whereString, orderByString, 1);

            var repository = GetRepository(tableName);
            using (var connection = repository.Database.GetConnection())
            {
                using (var rdr = connection.ExecuteReader(sqlString))
                {
                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0))
                        {
                            contentId = rdr.GetInt32(0);
                        }
                    }
                    rdr.Close();
                }
            }

            return contentId;
        }

        public async Task<int> GetSequenceAsync(string tableName, int siteId, int channelId, int contentId)
        {
            var repository = GetRepository(tableName);

            var taxis = await repository.GetAsync<int>(GetQuery(siteId, channelId)
                .Select(nameof(Content.Taxis))
                .Where(nameof(Content.Id), contentId)
            );

            return await repository.CountAsync(GetQuery(siteId, channelId)
                       .WhereTrue(nameof(Content.Checked))
                       .Where(nameof(Content.Taxis), "<", taxis)
                   ) + 1;
        }

        private List<ContentSummary> GetStlDataSourceByContentNumAndWhereString(string tableName, int totalNum, string whereString, string orderByString)
        {
            List<ContentSummary> dataset = null;
            if (!string.IsNullOrEmpty(tableName))
            {
                //var sqlSelect = _databaseRepository.GetSelectSqlString(tableName, totalNum, MinListColumns, whereString, orderByString);

                //dataset = _databaseApi.ExecuteDataset(WebConfigUtils.ConnectionString, sqlSelect);
            }
            return dataset;
        }

        private List<ContentSummary> GetStlDataSourceByStartNum(string tableName, int startNum, int totalNum, string whereString, string orderByString)
        {
            List<ContentSummary> dataset = null;
            if (!string.IsNullOrEmpty(tableName))
            {
                //var sqlSelect = _databaseRepository.GetSelectSqlString(tableName, startNum, totalNum, MinListColumns, whereString, orderByString);
                //var sqlSelect = _databaseRepository.GetPageSqlString(tableName, MinListColumns, whereString, orderByString, startNum - 1, totalNum);
                //dataset = _databaseApi.ExecuteDataset(WebConfigUtils.ConnectionString, sqlSelect);
            }
            return dataset;
        }

        public async Task<int> GetCountCheckedImageAsync(Site site, Channel channel)
        {
            var repository = GetRepository(site, channel);

            return await repository.CountAsync(GetQuery(site.Id, channel.Id)
                       .WhereTrue(nameof(Content.Checked))
                       .WhereNotNullOrEmpty(nameof(Content.ImageUrl))
                   ) + 1;
        }
    }
}
