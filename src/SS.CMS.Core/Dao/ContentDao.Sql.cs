using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Stl;
using SS.CMS.Core.Common;
using SS.CMS.Core.Components;
using SS.CMS.Core.Models;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Data;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;
using Attr = SS.CMS.Core.Models.Attributes.ContentAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentDao
    {
        public string GetSelectCommandByHitsAnalysis(int siteId)
        {
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

            var whereString = new StringBuilder();
            whereString.Append($"AND IsChecked = '{true}' AND SiteId = {siteId} AND Hits > 0");
            whereString.Append(orderByString);

            return DatabaseUtils.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString.ToString());
        }

        public string GetSqlStringOfAdminExcludeRecycle(int siteId, DateTime begin, DateTime end)
        {
            var tableName = TableName;
            var sqlString = $@"select userName,SUM(addCount) as addCount, SUM(updateCount) as updateCount from( 
SELECT AddUserName as userName, Count(AddUserName) as addCount, 0 as updateCount FROM {tableName} 
INNER JOIN {DataProvider.AdministratorDao.TableName} ON AddUserName = {DataProvider.AdministratorDao.TableName}.UserName 
WHERE {tableName}.SiteId = {siteId} AND (({tableName}.ChannelId > 0)) 
AND LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}
GROUP BY AddUserName
Union
SELECT LastEditUserName as userName,0 as addCount, Count(LastEditUserName) as updateCount FROM {tableName} 
INNER JOIN {DataProvider.AdministratorDao.TableName} ON LastEditUserName = {DataProvider.AdministratorDao.TableName}.UserName 
WHERE {tableName}.SiteId = {siteId} AND (({tableName}.ChannelId > 0)) 
AND LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}
AND LastEditDate != AddDate
GROUP BY LastEditUserName
) as tmp
group by tmp.userName";

            return sqlString;
        }

        public string GetStlWhereString(int siteId, ChannelInfo channelInfo, string group, string groupNot, string tags, bool isTopExists, bool isTop, bool isRelatedContents, int contentId)
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
                                $" ({Attr.GroupNameCollection} = '{AttackUtils.FilterSql(trimGroup)}' OR {SqlUtils.GetInStr(Attr.GroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + trimGroup)}) OR ");
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
                                $" ({Attr.GroupNameCollection} <> '{theGroupNot.Trim()}' AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, theGroupNot.Trim() + ",")} AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, "," + theGroupNot.Trim() + ",")} AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, "," + theGroupNot.Trim())}) AND ");
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
                var tagList = TagUtils.ParseTagsString(tags);
                var contentIdList = DataProvider.TagDao.GetContentIdListByTagCollection(tagList, siteId);
                if (contentIdList.Count > 0)
                {
                    whereStringBuilder.Append($" AND (Id IN ({string.Join(",", contentIdList)}))");
                }
            }

            if (isRelatedContents && contentId > 0)
            {
                var tagCollection = StlContentCache.GetValue(channelInfo, contentId, Attr.Tags);
                if (!string.IsNullOrEmpty(tagCollection))
                {
                    var contentIdList = StlTagCache.GetContentIdListByTagCollection(TranslateUtils.StringCollectionToStringList(tagCollection), siteId);
                    if (contentIdList.Count > 0)
                    {
                        contentIdList.Remove(contentId);

                        whereStringBuilder.Append($" AND ID IN ({string.Join(",", contentIdList)})");
                    }
                }
                else
                {
                    whereStringBuilder.Append($" AND ID <> {contentId}");
                }
            }

            return whereStringBuilder.ToString();
        }

        public string GetWhereStringByStlSearch(bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int siteId, List<string> excludeAttributes, NameValueCollection form)
        {
            var whereBuilder = new StringBuilder();

            SiteInfo siteInfo = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                siteInfo = SiteManager.GetSiteInfoBySiteName(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                siteInfo = SiteManager.GetSiteInfoByDirectory(siteDir);
            }
            if (siteInfo == null)
            {
                siteInfo = SiteManager.GetSiteInfo(siteId);
            }

            var channelId = ChannelManager.GetChannelId(siteId, siteId, channelIndex, channelName);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

            if (isAllSites)
            {
                whereBuilder.Append("(SiteId > 0) ");
            }
            else if (!string.IsNullOrEmpty(siteIds))
            {
                whereBuilder.Append($"(SiteId IN ({TranslateUtils.ToSqlInStringWithoutQuote(TranslateUtils.StringCollectionToIntList(siteIds))})) ");
            }
            else
            {
                whereBuilder.Append($"(SiteId = {siteInfo.Id}) ");
            }

            if (!string.IsNullOrEmpty(channelIds))
            {
                whereBuilder.Append(" AND ");
                var channelIdList = new List<int>();
                foreach (var theChannelId in TranslateUtils.StringCollectionToIntList(channelIds))
                {
                    var theSiteId = DataProvider.ChannelDao.GetSiteId(theChannelId);
                    channelIdList.AddRange(
                        ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(theSiteId, theChannelId),
                            EScopeType.All, string.Empty, string.Empty, string.Empty));
                }
                whereBuilder.Append(channelIdList.Count == 1
                    ? $"(ChannelId = {channelIdList[0]}) "
                    : $"(ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)})) ");
            }
            else if (channelId != siteId)
            {
                whereBuilder.Append(" AND ");

                var theSiteId = DataProvider.ChannelDao.GetSiteId(channelId);
                var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(theSiteId, channelId),
                            EScopeType.All, string.Empty, string.Empty, string.Empty);

                whereBuilder.Append(channelIdList.Count == 1
                    ? $"(ChannelId = {channelIdList[0]}) "
                    : $"(ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)})) ");
            }

            var typeList = new List<string>();
            if (string.IsNullOrEmpty(type))
            {
                typeList.Add(Attr.Title);
            }
            else
            {
                typeList = TranslateUtils.StringCollectionToStringList(type);
            }

            if (!string.IsNullOrEmpty(word))
            {
                whereBuilder.Append(" AND (");
                foreach (var attributeName in typeList)
                {
                    whereBuilder.Append($"[{attributeName}] LIKE '%{AttackUtils.FilterSql(word)}%' OR ");
                }
                whereBuilder.Length = whereBuilder.Length - 3;
                whereBuilder.Append(")");
            }

            if (string.IsNullOrEmpty(dateAttribute))
            {
                dateAttribute = Attr.AddDate;
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                whereBuilder.Append(" AND ");
                whereBuilder.Append($" {dateAttribute} >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))} ");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                whereBuilder.Append(" AND ");
                whereBuilder.Append($" {dateAttribute} <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo))} ");
            }
            if (!string.IsNullOrEmpty(since))
            {
                var sinceDate = DateTime.Now.AddHours(-DateUtils.GetSinceHours(since));
                whereBuilder.Append($" AND {dateAttribute} BETWEEN {SqlUtils.GetComparableDateTime(sinceDate)} AND {SqlUtils.GetComparableNow()} ");
            }

            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            //var styleInfoList = RelatedIdentities.GetTableStyleInfoList(siteInfo, channelInfo.Id);

            foreach (string key in form.Keys)
            {
                if (excludeAttributes.Contains(key.ToLower())) continue;
                if (string.IsNullOrEmpty(form[key])) continue;

                var value = StringUtils.Trim(form[key]);
                if (string.IsNullOrEmpty(value)) continue;

                var columnInfo = TableColumnManager.GetTableColumnInfo(tableName, key);

                if (columnInfo != null && (columnInfo.DataType == DataType.VarChar || columnInfo.DataType == DataType.Text))
                {
                    whereBuilder.Append(" AND ");
                    whereBuilder.Append($"({key} LIKE '%{value}%')");
                }
                //else
                //{
                //    foreach (var tableStyleInfo in styleInfoList)
                //    {
                //        if (StringUtils.EqualsIgnoreCase(tableStyleInfo.AttributeName, key))
                //        {
                //            whereBuilder.Append(" AND ");
                //            whereBuilder.Append($"({ContentAttribute.SettingsXml} LIKE '%{key}={value}%')");
                //            break;
                //        }
                //    }
                //}
            }

            return whereBuilder.ToString();
        }

        public string GetSqlString(int siteId, int channelId, bool isSystemAdministrator, List<int> owningChannelIdList, string searchType, string keyword, string dateFrom, string dateTo, bool isSearchChildren, ETriState checkedState, bool isTrashContent)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var channelIdList = ChannelManager.GetChannelIdList(channelInfo,
                isSearchChildren ? EScopeType.All : EScopeType.Self, string.Empty, string.Empty, channelInfo.ContentModelPluginId);

            var list = new List<int>();
            if (isSystemAdministrator)
            {
                list = channelIdList;
            }
            else
            {
                foreach (var theChannelId in channelIdList)
                {
                    if (owningChannelIdList.Contains(theChannelId))
                    {
                        list.Add(theChannelId);
                    }
                }
            }

            return GetSqlStringByCondition(siteId, list, searchType, keyword, dateFrom, dateTo, checkedState, isTrashContent);
        }

        public string GetSqlStringByContentGroup(string contentGroupName, int siteId)
        {
            contentGroupName = AttackUtils.FilterSql(contentGroupName);
            var sqlString =
                $"SELECT * FROM {TableName} WHERE SiteId = {siteId} AND ChannelId > 0 AND (GroupNameCollection LIKE '{contentGroupName},%' OR GroupNameCollection LIKE '%,{contentGroupName}' OR GroupNameCollection  LIKE '%,{contentGroupName},%'  OR GroupNameCollection='{contentGroupName}')";
            return sqlString;
        }

        public string GetSqlStringByContentTag(string tag, int siteId)
        {
            tag = AttackUtils.FilterSql(tag);

            var sqlString =
                $"SELECT * FROM {TableName} WHERE SiteId = {siteId} AND ChannelId > 0 AND (Tags LIKE '{tag} %' OR Tags LIKE '% {tag}' OR Tags  LIKE '% {tag} %'  OR Tags='{tag}')";
            return sqlString;
        }

        public string GetStlSqlStringChecked(List<int> channelIdList, int siteId, int channelId, int startNum, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            string sqlWhereString;

            if (siteId == channelId && scopeType == EScopeType.All && string.IsNullOrEmpty(groupChannel) && string.IsNullOrEmpty(groupChannelNot))
            {
                sqlWhereString =
                    $"WHERE (SiteId = {siteId} AND ChannelId > 0 AND IsChecked = '{true}' {whereString})";
            }
            else
            {
                if (channelIdList == null || channelIdList.Count == 0)
                {
                    return string.Empty;
                }
                sqlWhereString = channelIdList.Count == 1 ? $"WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString})" : $"WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString})";
            }

            return DatabaseUtils.GetPageSqlString(TableName, Container.Content.SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);
        }

        public string GetStlSqlStringCheckedBySearch(int startNum, int totalNum, string orderByString, string whereString)
        {
            var sqlWhereString =
                    $"WHERE (ChannelId > 0 AND IsChecked = '{true}' {whereString})";

            return DatabaseUtils.GetPageSqlString(TableName, TranslateUtils.ObjectCollectionToString(Attr.AllAttributes.Value), sqlWhereString, orderByString, startNum - 1, totalNum);
        }

        public string GetStlWhereString(int siteId, ChannelInfo channelInfo, string group, string groupNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, bool isRelatedContents, int contentId)
        {
            var whereBuilder = new StringBuilder();
            whereBuilder.Append($" AND SiteId = {siteId} ");

            if (isImageExists)
            {
                whereBuilder.Append(isImage
                    ? $" AND {Attr.ImageUrl} <> '' "
                    : $" AND {Attr.ImageUrl} = '' ");
            }

            if (isVideoExists)
            {
                whereBuilder.Append(isVideo
                    ? $" AND {Attr.VideoUrl} <> '' "
                    : $" AND {Attr.VideoUrl} = '' ");
            }

            if (isFileExists)
            {
                whereBuilder.Append(isFile
                    ? $" AND {Attr.FileUrl} <> '' "
                    : $" AND {Attr.FileUrl} = '' ");
            }

            if (isTopExists)
            {
                whereBuilder.Append($" AND {Attr.IsTop} = '{isTop}' ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {Attr.IsRecommend} = '{isRecommend}' ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {Attr.IsHot} = '{isHot}' ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {Attr.IsColor} = '{isColor}' ");
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
                                $" ({Attr.GroupNameCollection} = '{AttackUtils.FilterSql(trimGroup)}' OR {SqlUtils.GetInStr(Attr.GroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + trimGroup)}) OR ");
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
                        whereBuilder.Append(
                                $" ({Attr.GroupNameCollection} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, trimGroup + ",")} AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, "," + trimGroup)}) AND ");
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
                var tagCollection = TagUtils.ParseTagsString(tags);
                var contentIdList = DataProvider.TagDao.GetContentIdListByTagCollection(tagCollection, siteId);
                if (contentIdList.Count > 0)
                {
                    whereBuilder.Append(
                        $" AND (ID IN ({string.Join(",", contentIdList)}))");
                }
            }

            if (isRelatedContents && contentId > 0)
            {
                var tagCollection = StlContentCache.GetValue(channelInfo, contentId, Attr.Tags);
                if (!string.IsNullOrEmpty(tagCollection))
                {
                    var contentIdList = StlTagCache.GetContentIdListByTagCollection(TranslateUtils.StringCollectionToStringList(tagCollection), siteId);
                    if (contentIdList.Count > 0)
                    {
                        contentIdList.Remove(contentId);

                        whereBuilder.Append($" AND ID IN ({string.Join(",", contentIdList)})");
                    }
                }
                else
                {
                    whereBuilder.Append($" AND ID <> {contentId}");
                }
            }

            return whereBuilder.ToString();
        }

        public string GetStlWhereStringBySearch(string group, string groupNot, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var whereBuilder = new StringBuilder();

            if (isImageExists)
            {
                whereBuilder.Append(isImage
                    ? $" AND {Attr.ImageUrl} <> '' "
                    : $" AND {Attr.ImageUrl} = '' ");
            }

            if (isVideoExists)
            {
                whereBuilder.Append(isVideo
                    ? $" AND {Attr.VideoUrl} <> '' "
                    : $" AND {Attr.VideoUrl} = '' ");
            }

            if (isFileExists)
            {
                whereBuilder.Append(isFile
                    ? $" AND {Attr.FileUrl} <> '' "
                    : $" AND {Attr.FileUrl} = '' ");
            }

            if (isTopExists)
            {
                whereBuilder.Append($" AND {Attr.IsTop} = '{isTop}' ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {Attr.IsRecommend} = '{isRecommend}' ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {Attr.IsHot} = '{isHot}' ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {Attr.IsColor} = '{isColor}' ");
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
                                $" ({Attr.GroupNameCollection} = '{AttackUtils.FilterSql(trimGroup)}' OR {SqlUtils.GetInStr(Attr.GroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + trimGroup)}) OR ");
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
                                $" ({Attr.GroupNameCollection} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, trimGroup + ",")} AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, "," + trimGroup)}) AND ");
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

        private string GetSqlStringByCondition(int siteId, List<int> channelIdList, string searchType, string keyword, string dateFrom, string dateTo, ETriState checkedState, bool isTrashContent)
        {
            return GetSqlStringByCondition(siteId, channelIdList, searchType, keyword, dateFrom, dateTo, checkedState, isTrashContent, false, string.Empty);
        }

        private string GetSqlStringByCondition(int siteId, List<int> channelIdList, string searchType, string keyword, string dateFrom, string dateTo, ETriState checkedState, bool isTrashContent, bool isWritingOnly, string userNameOnly)
        {
            if (channelIdList == null || channelIdList.Count == 0)
            {
                return null;
            }

            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

            var dateString = string.Empty;
            if (!string.IsNullOrEmpty(dateFrom))
            {
                dateString = $" AND AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))} ";
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                dateString += $" AND AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo).AddDays(1))} ";
            }
            var whereString = new StringBuilder($"WHERE {nameof(Attr.SourceId)} != {SourceManager.Preview} AND ");

            if (isTrashContent)
            {
                for (var i = 0; i < channelIdList.Count; i++)
                {
                    var theChannelId = channelIdList[i];
                    channelIdList[i] = -theChannelId;
                }
            }

            whereString.Append(channelIdList.Count == 1
                ? $"SiteId = {siteId} AND (ChannelId = {channelIdList[0]}) "
                : $"SiteId = {siteId} AND (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)})) ");

            if (StringUtils.EqualsIgnoreCase(searchType, Attr.IsTop) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsRecommend) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsColor) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsHot))
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    whereString.Append($"AND ({Attr.Title} LIKE '%{keyword}%') ");
                }
                whereString.Append($" AND {searchType} = '{true}'");
            }
            else if (!string.IsNullOrEmpty(keyword))
            {
                var columnNameList = TableColumnManager.GetTableColumnNameList(TableName);

                if (StringUtils.ContainsIgnoreCase(columnNameList, searchType))
                {
                    whereString.Append($"AND ({searchType} LIKE '%{keyword}%') ");
                }
            }

            whereString.Append(dateString);

            if (checkedState == ETriState.True)
            {
                whereString.Append("AND IsChecked='True' ");
            }
            else if (checkedState == ETriState.False)
            {
                whereString.Append("AND IsChecked='False' ");
            }

            if (!string.IsNullOrEmpty(userNameOnly))
            {
                whereString.Append($" AND {Attr.AddUserName} = '{userNameOnly}' ");
            }
            if (isWritingOnly)
            {
                whereString.Append($" AND {Attr.UserId} > 0 ");
            }

            whereString.Append(" ").Append(orderByString);

            return DatabaseUtils.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString.ToString());
        }

        public string GetPagerWhereSqlString(SiteInfo siteInfo, ChannelInfo channelInfo, string searchType, string keyword, string dateFrom, string dateTo, int checkLevel, bool isCheckOnly, bool isSelfOnly, bool isTrashOnly, bool isWritingOnly, int? onlyAdminId, PermissionsImpl adminPermissions, List<string> allAttributeNameList)
        {
            var isAllChannels = false;
            var searchChannelIdList = new List<int>();

            if (isSelfOnly)
            {
                searchChannelIdList = new List<int>
                {
                    channelInfo.Id
                };
            }
            else
            {
                var channelIdList = ChannelManager.GetChannelIdList(channelInfo, EScopeType.All, string.Empty, string.Empty, channelInfo.ContentModelPluginId);

                if (adminPermissions.IsSystemAdministrator)
                {
                    if (channelInfo.Id == siteInfo.Id)
                    {
                        isAllChannels = true;
                    }

                    searchChannelIdList = channelIdList;
                }
                else
                {
                    foreach (var theChannelId in channelIdList)
                    {
                        if (adminPermissions.ChannelIdList.Contains(theChannelId))
                        {
                            searchChannelIdList.Add(theChannelId);
                        }
                    }
                }
            }
            if (isTrashOnly)
            {
                searchChannelIdList = searchChannelIdList.Select(i => -i).ToList();
            }

            var whereList = new List<string>
            {
                $"{nameof(Attr.SiteId)} = {siteInfo.Id}",
                $"{nameof(Attr.SourceId)} != {SourceManager.Preview}"
            };

            if (!string.IsNullOrEmpty(dateFrom))
            {
                whereList.Add($"{nameof(Attr.AddDate)} >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))}");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                whereList.Add($"{nameof(Attr.AddDate)} <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo).AddDays(1))}");
            }

            if (isAllChannels)
            {
                whereList.Add(isTrashOnly
                    ? $"{nameof(Attr.ChannelId)} < 0"
                    : $"{nameof(Attr.ChannelId)} > 0");
            }
            else if (searchChannelIdList.Count == 0)
            {
                whereList.Add($"{nameof(Attr.ChannelId)} = 0");
            }
            else if (searchChannelIdList.Count == 1)
            {
                whereList.Add($"{nameof(Attr.ChannelId)} = {channelInfo.Id}");
            }
            else
            {
                whereList.Add($"{nameof(Attr.ChannelId)} IN ({TranslateUtils.ToSqlInStringWithoutQuote(searchChannelIdList)})");
            }

            if (StringUtils.EqualsIgnoreCase(searchType, Attr.IsTop) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsRecommend) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsColor) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsHot))
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    whereList.Add($"{Attr.Title} LIKE '%{keyword}%'");
                }
                whereList.Add($"{searchType} = '{true}'");
            }
            else if (!string.IsNullOrEmpty(keyword))
            {
                if (StringUtils.ContainsIgnoreCase(allAttributeNameList, searchType))
                {
                    whereList.Add($"{searchType} LIKE '%{keyword}%'");
                }
                //whereList.Add(allLowerAttributeNameList.Contains(searchType.ToLower())
                //    ? $"{searchType} LIKE '%{keyword}%'"
                //    : $"{nameof(ContentAttribute.SettingsXml)} LIKE '%{searchType}={keyword}%'");
            }

            if (isCheckOnly)
            {
                whereList.Add(checkLevel == CheckManager.LevelInt.All
                    ? $"{nameof(Attr.IsChecked)} = '{false}'"
                    : $"{nameof(Attr.IsChecked)} = '{false}' AND {nameof(Attr.CheckedLevel)} = {checkLevel}");
            }
            else
            {
                if (checkLevel != CheckManager.LevelInt.All)
                {
                    whereList.Add(checkLevel == siteInfo.CheckContentLevel
                        ? $"{nameof(Attr.IsChecked)} = '{true}'"
                        : $"{nameof(Attr.IsChecked)} = '{false}' AND {nameof(Attr.CheckedLevel)} = {checkLevel}");
                }
            }

            if (onlyAdminId.HasValue)
            {
                whereList.Add($"{nameof(Attr.AdminId)} = {onlyAdminId.Value}");
            }

            if (isWritingOnly)
            {
                whereList.Add($"{nameof(Attr.UserId)} > 0");
            }

            return $"WHERE {string.Join(" AND ", whereList)}";
        }

        public string GetCacheWhereString(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId)
        {
            var whereString = $"WHERE {Attr.SiteId} = {siteInfo.Id} AND {Attr.ChannelId} = {channelInfo.Id} AND {nameof(Attr.SourceId)} != {SourceManager.Preview}";
            if (onlyAdminId.HasValue)
            {
                whereString += $" AND {nameof(Attr.AdminId)} = {onlyAdminId.Value}";
            }

            return whereString;
        }

        public DataSet GetStlDataSourceChecked(List<int> channelIdList, int startNum, int totalNum, string orderByString, string whereString, NameValueCollection others)
        {
            if (channelIdList == null || channelIdList.Count == 0)
            {
                return null;
            }
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

            return startNum <= 1 ? GetStlDataSourceByContentNumAndWhereString(totalNum, sqlWhereString, orderByString) : GetStlDataSourceByStartNum(startNum, totalNum, sqlWhereString, orderByString);
        }

        private DataSet GetStlDataSourceByContentNumAndWhereString(int totalNum, string whereString, string orderByString)
        {
            var sqlSelect = DatabaseUtils.GetSelectSqlString(TableName, totalNum, Container.Content.SqlColumns, whereString, orderByString);
            return DatabaseUtils.GetDataSet(sqlSelect);
        }

        private DataSet GetStlDataSourceByStartNum(int startNum, int totalNum, string whereString, string orderByString)
        {
            var sqlSelect = DatabaseUtils.GetPageSqlString(TableName, Container.Content.SqlColumns, whereString, orderByString, startNum - 1, totalNum);
            return DatabaseUtils.GetDataSet(sqlSelect);
        }

        public DataSet GetDataSetOfAdminExcludeRecycle(int siteId, DateTime begin, DateTime end)
        {
            var sqlString = GetSqlStringOfAdminExcludeRecycle(siteId, begin, end);

            return DatabaseUtils.GetDataSet(sqlString);
        }
    }
}
