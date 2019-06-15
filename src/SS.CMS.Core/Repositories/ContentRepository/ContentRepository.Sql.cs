using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using SqlKata;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Stl;
using SS.CMS.Core.Common;
using SS.CMS.Core.Security;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Data;
using SS.CMS.Utils;
using Attr = SS.CMS.Core.Models.Attributes.ContentAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public List<ContentInfo> GetSelectCommandByHitsAnalysis(int siteId)
        {
            var query = Q.Where(Attr.SiteId, siteId).Where(Attr.Hits, ">", 0).WhereTrue(Attr.IsChecked);
            QueryOrder(query, TaxisType.OrderByTaxisDesc);

            return _repository.GetAll(query).ToList();
        }

        //         public string GetSqlStringOfAdminExcludeRecycle(int siteId, DateTime begin, DateTime end)
        //         {
        //             var tableName = TableName;
        //             var sqlString = $@"select userName,SUM(addCount) as addCount, SUM(updateCount) as updateCount from( 
        // SELECT AddUserName as userName, Count(AddUserName) as addCount, 0 as updateCount FROM {tableName} 
        // INNER JOIN {_administratorRepository.TableName} ON AddUserName = {_administratorRepository.TableName}.UserName 
        // WHERE {tableName}.SiteId = {siteId} AND (({tableName}.ChannelId > 0)) 
        // AND LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}
        // GROUP BY AddUserName
        // Union
        // SELECT LastEditUserName as userName,0 as addCount, Count(LastEditUserName) as updateCount FROM {tableName} 
        // INNER JOIN {_administratorRepository.TableName} ON LastEditUserName = {_administratorRepository.TableName}.UserName 
        // WHERE {tableName}.SiteId = {siteId} AND (({tableName}.ChannelId > 0)) 
        // AND LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}
        // AND LastEditDate != AddDate
        // GROUP BY LastEditUserName
        // ) as tmp
        // group by tmp.userName";

        //             return sqlString;
        //         }

        public Query GetStlWhereString(int siteId, ChannelInfo channelInfo, string group, string groupNot, string tags, bool? isTop, bool isRelatedContents, int contentId)
        {
            var query = Q.NewQuery();

            QueryWhereIsTop(query, isTop);

            QueryWhereGroup(query, group);

            QueryWhereGroupNot(query, groupNot);

            QueryWhereTags(query, siteId, tags);

            QueryWhereIsRelatedContents(query, isRelatedContents, siteId, channelInfo, contentId);

            return query;
        }

        public Query GetWhereStringByStlSearch(bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int siteId, List<string> excludeAttributes, NameValueCollection form)
        {
            var query = Q.NewQuery();

            SiteInfo siteInfo = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                siteInfo = _siteRepository.GetSiteInfoBySiteName(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                siteInfo = _siteRepository.GetSiteInfoByDirectory(siteDir);
            }
            if (siteInfo == null)
            {
                siteInfo = _siteRepository.GetSiteInfo(siteId);
            }

            var channelId = ChannelManager.GetChannelId(siteId, siteId, channelIndex, channelName);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

            if (!string.IsNullOrEmpty(siteIds))
            {
                query.WhereIn(Attr.SiteId, TranslateUtils.StringCollectionToIntList(siteIds));
            }
            else if (!isAllSites)
            {
                query.Where(Attr.SiteId, siteInfo.Id);
            }

            if (!string.IsNullOrEmpty(channelIds))
            {
                var channelIdList = new List<int>();
                foreach (var theChannelId in TranslateUtils.StringCollectionToIntList(channelIds))
                {
                    var theSiteId = DataProvider.ChannelRepository.GetSiteId(theChannelId);
                    channelIdList.AddRange(
                        ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(theSiteId, theChannelId),
                            ScopeType.All, string.Empty, string.Empty, string.Empty));
                }

                query.WhereIn(Attr.ChannelId, channelIdList);
            }
            else if (channelId != siteId)
            {
                var theSiteId = DataProvider.ChannelRepository.GetSiteId(channelId);
                var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(theSiteId, channelId), ScopeType.All, string.Empty, string.Empty, string.Empty);

                query.WhereIn(Attr.ChannelId, channelIdList);
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
                query.Where(q =>
                {
                    foreach (var attributeName in typeList)
                    {
                        q.OrWhereLike(attributeName, word);
                    }
                    return q;
                });
            }

            if (string.IsNullOrEmpty(dateAttribute))
            {
                dateAttribute = Attr.AddDate;
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                query.WhereDate(dateAttribute, ">=", dateFrom);
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                query.WhereDate(dateAttribute, "<=", dateTo);
            }
            if (!string.IsNullOrEmpty(since))
            {
                var sinceDate = DateTime.Now.AddHours(-DateUtils.GetSinceHours(since));
                query.WhereBetween(dateAttribute, sinceDate, DateTime.Now);
            }

            var tableName = ChannelManager.GetTableName(_pluginManager, siteInfo, channelInfo);
            //var styleInfoList = RelatedIdentities.GetTableStyleInfoList(siteInfo, channelInfo.Id);

            foreach (string key in form.Keys)
            {
                if (excludeAttributes.Contains(key.ToLower())) continue;
                if (string.IsNullOrEmpty(form[key])) continue;

                var value = StringUtils.Trim(form[key]);
                if (string.IsNullOrEmpty(value)) continue;

                var columnInfo = _tableManager.GetTableColumnInfo(tableName, key);

                if (columnInfo != null && (columnInfo.DataType == DataType.VarChar || columnInfo.DataType == DataType.Text))
                {
                    query.WhereLike(key, value);
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

            return query;
        }

        // public string GetSqlString(int siteId, int channelId, bool isSystemAdministrator, List<int> owningChannelIdList, string searchType, string keyword, string dateFrom, string dateTo, bool isSearchChildren, bool? checkedState, bool isTrashContent)
        // {
        //     var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
        //     var channelIdList = ChannelManager.GetChannelIdList(channelInfo,
        //         isSearchChildren ? ScopeType.All : ScopeType.Self, string.Empty, string.Empty, channelInfo.ContentModelPluginId);

        //     var list = new List<int>();
        //     if (isSystemAdministrator)
        //     {
        //         list = channelIdList;
        //     }
        //     else
        //     {
        //         foreach (var theChannelId in channelIdList)
        //         {
        //             if (owningChannelIdList.Contains(theChannelId))
        //             {
        //                 list.Add(theChannelId);
        //             }
        //         }
        //     }

        //     return GetSqlStringByCondition(siteId, list, searchType, keyword, dateFrom, dateTo, checkedState, isTrashContent);
        // }

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

        public List<ContentInfo> GetStlSqlStringChecked(List<int> channelIdList, int siteId, int channelId, int startNum, int totalNum, string order, Query query, ScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            if (siteId == channelId && scopeType == ScopeType.All && string.IsNullOrEmpty(groupChannel) && string.IsNullOrEmpty(groupChannelNot))
            {
                query.Where(Attr.SiteId, siteId).Where(Attr.ChannelId, ">", 0).WhereTrue(Attr.IsChecked);
            }
            else
            {
                if (channelIdList == null || channelIdList.Count == 0)
                {
                    return new List<ContentInfo>();
                }

                query.WhereIn(Attr.ChannelId, channelIdList).WhereTrue(Attr.IsChecked);
            }

            QuerySelectMinColumns(query);
            query.Offset(startNum - 1).Limit(totalNum);

            return _repository.GetAll(query).ToList();
        }

        public List<ContentInfo> GetStlSqlStringCheckedBySearch(int startNum, int totalNum, string order, Query query)
        {
            query.Where(Attr.ChannelId, ">", 0).WhereTrue(Attr.IsChecked);
            query.Offset(startNum - 1).Limit(totalNum);

            return _repository.GetAll(query).ToList();
        }

        public Query GetStlWhereString(int siteId, ChannelInfo channelInfo, string group, string groupNot, string tags, bool? isImage, bool? isVideo, bool? isFile, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor, bool isRelatedContents, int contentId)
        {
            var query = Q.Where(Attr.SiteId, siteId);

            if (isImage.HasValue)
            {
                if (isImage.Value)
                {
                    query.WhereNotNull(Attr.ImageUrl).WhereNot(Attr.ImageUrl, string.Empty);
                }
                else
                {
                    query.Where(q =>
                    {
                        return q.WhereNull(Attr.ImageUrl).OrWhere(Attr.ImageUrl, string.Empty);
                    });
                }
            }

            if (isVideo.HasValue)
            {
                if (isVideo.Value)
                {
                    query.WhereNotNull(Attr.VideoUrl).WhereNot(Attr.VideoUrl, string.Empty);
                }
                else
                {
                    query.Where(q =>
                    {
                        return q.WhereNull(Attr.VideoUrl).OrWhere(Attr.VideoUrl, string.Empty);
                    });
                }
            }

            if (isFile.HasValue)
            {
                if (isFile.Value)
                {
                    query.WhereNotNull(Attr.FileUrl).WhereNot(Attr.FileUrl, string.Empty);
                }
                else
                {
                    query.Where(q =>
                    {
                        return q.WhereNull(Attr.FileUrl).OrWhere(Attr.FileUrl, string.Empty);
                    });
                }
            }

            if (isTop.HasValue)
            {
                query.Where(Attr.IsTop, isTop.Value);
            }

            if (isRecommend.HasValue)
            {
                query.Where(Attr.IsRecommend, isRecommend.Value);
            }

            if (isHot.HasValue)
            {
                query.Where(Attr.IsHot, isHot.Value);
            }

            if (isColor.HasValue)
            {
                query.Where(Attr.IsColor, isColor.Value);
            }

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr != null && groupArr.Length > 0)
                {
                    foreach (var theGroup in groupArr)
                    {
                        var trimGroup = theGroup.Trim();

                        query.Where(q =>
                        {
                            return q
                                .Where(Attr.GroupNameCollection, trimGroup)
                                .OrWhereInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, trimGroup + ",")
                                .OrWhereInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, "," + trimGroup + ",")
                                .OrWhereInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, "," + trimGroup)
                                ;
                        });
                    }
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr != null && groupNotArr.Length > 0)
                {
                    foreach (var theGroupNot in groupNotArr)
                    {
                        var trimGroup = theGroupNot.Trim();

                        query
                            .WhereNot(Attr.GroupNameCollection, trimGroup)
                            .WhereNotInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, trimGroup + ",")
                            .WhereNotInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, "," + trimGroup + ",")
                            .WhereNotInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, "," + trimGroup)
                            ;
                    }
                }
            }

            if (!string.IsNullOrEmpty(tags))
            {
                var tagCollection = TagUtils.ParseTagsString(tags);
                var contentIdList = DataProvider.TagRepository.GetContentIdListByTagCollection(tagCollection, siteId);
                if (contentIdList.Count > 0)
                {
                    query.WhereIn(Attr.Id, contentIdList);
                }
            }

            if (isRelatedContents && contentId > 0)
            {
                var tagCollection = StlGetValue(channelInfo, contentId, Attr.Tags);
                if (!string.IsNullOrEmpty(tagCollection))
                {
                    var contentIdList = StlTagCache.GetContentIdListByTagCollection(TranslateUtils.StringCollectionToStringList(tagCollection), siteId);
                    if (contentIdList.Count > 0)
                    {
                        contentIdList.Remove(contentId);
                        query.WhereIn(Attr.Id, contentIdList);
                    }
                }
                else
                {
                    query.WhereNot(Attr.Id, contentId);
                }
            }

            return query;
        }

        public Query GetStlWhereStringBySearch(string group, string groupNot, bool? isImage, bool? isVideo, bool? isFile, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor)
        {
            var query = Q.NewQuery();

            if (isImage.HasValue)
            {
                if (isImage.Value)
                {
                    query.WhereNotNull(Attr.ImageUrl).WhereNot(Attr.ImageUrl, string.Empty);
                }
                else
                {
                    query.Where(q =>
                    {
                        return q.WhereNull(Attr.ImageUrl).OrWhere(Attr.ImageUrl, string.Empty);
                    });
                }
            }

            if (isVideo.HasValue)
            {
                if (isVideo.Value)
                {
                    query.WhereNotNull(Attr.VideoUrl).WhereNot(Attr.VideoUrl, string.Empty);
                }
                else
                {
                    query.Where(q =>
                    {
                        return q.WhereNull(Attr.VideoUrl).OrWhere(Attr.VideoUrl, string.Empty);
                    });
                }
            }

            if (isFile.HasValue)
            {
                if (isFile.Value)
                {
                    query.WhereNotNull(Attr.FileUrl).WhereNot(Attr.FileUrl, string.Empty);
                }
                else
                {
                    query.Where(q =>
                    {
                        return q.WhereNull(Attr.FileUrl).OrWhere(Attr.FileUrl, string.Empty);
                    });
                }
            }

            if (isTop.HasValue)
            {
                query.Where(Attr.IsTop, isTop.Value);
            }

            if (isRecommend.HasValue)
            {
                query.Where(Attr.IsRecommend, isRecommend.Value);
            }

            if (isHot.HasValue)
            {
                query.Where(Attr.IsHot, isHot.Value);
            }

            if (isColor.HasValue)
            {
                query.Where(Attr.IsColor, isColor.Value);
            }

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr != null && groupArr.Length > 0)
                {
                    foreach (var theGroup in groupArr)
                    {
                        var trimGroup = theGroup.Trim();

                        query.Where(q =>
                        {
                            return q
                                .Where(Attr.GroupNameCollection, trimGroup)
                                .OrWhereInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, trimGroup + ",")
                                .OrWhereInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, "," + trimGroup + ",")
                                .OrWhereInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, "," + trimGroup)
                                ;
                        });
                    }
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr != null && groupNotArr.Length > 0)
                {
                    foreach (var theGroupNot in groupNotArr)
                    {
                        var trimGroup = theGroupNot.Trim();

                        query
                            .WhereNot(Attr.GroupNameCollection, trimGroup)
                            .WhereNotInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, trimGroup + ",")
                            .WhereNotInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, "," + trimGroup + ",")
                            .WhereNotInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, "," + trimGroup)
                            ;
                    }
                }
            }

            return query;
        }

        // private string GetSqlStringByCondition(int siteId, List<int> channelIdList, string searchType, string keyword, string dateFrom, string dateTo, bool? checkedState, bool isTrashContent)
        // {
        //     return GetSqlStringByCondition(siteId, channelIdList, searchType, keyword, dateFrom, dateTo, checkedState, isTrashContent, false, string.Empty);
        // }

        // private Query GetSqlStringByCondition(int siteId, List<int> channelIdList, string searchType, string keyword, string dateFrom, string dateTo, bool? checkedState, bool isTrashContent, bool isWritingOnly, string userNameOnly)
        // {
        //     if (channelIdList == null || channelIdList.Count == 0)
        //     {
        //         return null;
        //     }

        //     var orderByString = TaxisTypeUtils.GetContentOrderByString(TaxisType.OrderByTaxisDesc);

        //     var dateString = string.Empty;
        //     if (!string.IsNullOrEmpty(dateFrom))
        //     {
        //         dateString = $" AND AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))} ";
        //     }
        //     if (!string.IsNullOrEmpty(dateTo))
        //     {
        //         dateString += $" AND AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo).AddDays(1))} ";
        //     }
        //     var whereString = new StringBuilder($"WHERE {nameof(Attr.SourceId)} != {SourceManager.Preview} AND ");

        //     if (isTrashContent)
        //     {
        //         for (var i = 0; i < channelIdList.Count; i++)
        //         {
        //             var theChannelId = channelIdList[i];
        //             channelIdList[i] = -theChannelId;
        //         }
        //     }

        //     whereString.Append(channelIdList.Count == 1
        //         ? $"SiteId = {siteId} AND (ChannelId = {channelIdList[0]}) "
        //         : $"SiteId = {siteId} AND (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)})) ");

        //     if (StringUtils.EqualsIgnoreCase(searchType, Attr.IsTop) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsRecommend) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsColor) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsHot))
        //     {
        //         if (!string.IsNullOrEmpty(keyword))
        //         {
        //             whereString.Append($"AND ({Attr.Title} LIKE '%{keyword}%') ");
        //         }
        //         whereString.Append($" AND {searchType} = '{true}'");
        //     }
        //     else if (!string.IsNullOrEmpty(keyword))
        //     {
        //         var columnNameList = TableColumnManager.GetTableColumnNameList(TableName);

        //         if (StringUtils.ContainsIgnoreCase(columnNameList, searchType))
        //         {
        //             whereString.Append($"AND ({searchType} LIKE '%{keyword}%') ");
        //         }
        //     }

        //     whereString.Append(dateString);

        //     if (checkedState == true)
        //     {
        //         whereString.Append("AND IsChecked='True' ");
        //     }
        //     else if (checkedState == false)
        //     {
        //         whereString.Append("AND IsChecked='False' ");
        //     }

        //     if (!string.IsNullOrEmpty(userNameOnly))
        //     {
        //         whereString.Append($" AND {Attr.AddUserName} = '{userNameOnly}' ");
        //     }
        //     if (isWritingOnly)
        //     {
        //         whereString.Append($" AND {Attr.UserId} > 0 ");
        //     }

        //     whereString.Append(" ").Append(orderByString);

        //     return DatabaseUtils.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString.ToString());
        // }

        // public string GetPagerWhereSqlString(SiteInfo siteInfo, ChannelInfo channelInfo, string searchType, string keyword, string dateFrom, string dateTo, int checkLevel, bool isCheckOnly, bool isSelfOnly, bool isTrashOnly, bool isWritingOnly, int? onlyAdminId, Permissions adminPermissions, List<string> allAttributeNameList)
        // {
        //     var isAllChannels = false;
        //     var searchChannelIdList = new List<int>();

        //     if (isSelfOnly)
        //     {
        //         searchChannelIdList = new List<int>
        //         {
        //             channelInfo.Id
        //         };
        //     }
        //     else
        //     {
        //         var channelIdList = ChannelManager.GetChannelIdList(channelInfo, ScopeType.All, string.Empty, string.Empty, channelInfo.ContentModelPluginId);

        //         if (adminPermissions.IsSystemAdministrator)
        //         {
        //             if (channelInfo.Id == siteInfo.Id)
        //             {
        //                 isAllChannels = true;
        //             }

        //             searchChannelIdList = channelIdList;
        //         }
        //         else
        //         {
        //             foreach (var theChannelId in channelIdList)
        //             {
        //                 if (adminPermissions.ChannelIdList.Contains(theChannelId))
        //                 {
        //                     searchChannelIdList.Add(theChannelId);
        //                 }
        //             }
        //         }
        //     }
        //     if (isTrashOnly)
        //     {
        //         searchChannelIdList = searchChannelIdList.Select(i => -i).ToList();
        //     }

        //     var whereList = new List<string>
        //     {
        //         $"{nameof(Attr.SiteId)} = {siteInfo.Id}",
        //         $"{nameof(Attr.SourceId)} != {SourceManager.Preview}"
        //     };

        //     if (!string.IsNullOrEmpty(dateFrom))
        //     {
        //         whereList.Add($"{nameof(Attr.AddDate)} >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))}");
        //     }
        //     if (!string.IsNullOrEmpty(dateTo))
        //     {
        //         whereList.Add($"{nameof(Attr.AddDate)} <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo).AddDays(1))}");
        //     }

        //     if (isAllChannels)
        //     {
        //         whereList.Add(isTrashOnly
        //             ? $"{nameof(Attr.ChannelId)} < 0"
        //             : $"{nameof(Attr.ChannelId)} > 0");
        //     }
        //     else if (searchChannelIdList.Count == 0)
        //     {
        //         whereList.Add($"{nameof(Attr.ChannelId)} = 0");
        //     }
        //     else if (searchChannelIdList.Count == 1)
        //     {
        //         whereList.Add($"{nameof(Attr.ChannelId)} = {channelInfo.Id}");
        //     }
        //     else
        //     {
        //         whereList.Add($"{nameof(Attr.ChannelId)} IN ({TranslateUtils.ToSqlInStringWithoutQuote(searchChannelIdList)})");
        //     }

        //     if (StringUtils.EqualsIgnoreCase(searchType, Attr.IsTop) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsRecommend) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsColor) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsHot))
        //     {
        //         if (!string.IsNullOrEmpty(keyword))
        //         {
        //             whereList.Add($"{Attr.Title} LIKE '%{keyword}%'");
        //         }
        //         whereList.Add($"{searchType} = '{true}'");
        //     }
        //     else if (!string.IsNullOrEmpty(keyword))
        //     {
        //         if (StringUtils.ContainsIgnoreCase(allAttributeNameList, searchType))
        //         {
        //             whereList.Add($"{searchType} LIKE '%{keyword}%'");
        //         }
        //         //whereList.Add(allLowerAttributeNameList.Contains(searchType.ToLower())
        //         //    ? $"{searchType} LIKE '%{keyword}%'"
        //         //    : $"{nameof(ContentAttribute.SettingsXml)} LIKE '%{searchType}={keyword}%'");
        //     }

        //     if (isCheckOnly)
        //     {
        //         whereList.Add(checkLevel == CheckManager.LevelInt.All
        //             ? $"{nameof(Attr.IsChecked)} = '{false}'"
        //             : $"{nameof(Attr.IsChecked)} = '{false}' AND {nameof(Attr.CheckedLevel)} = {checkLevel}");
        //     }
        //     else
        //     {
        //         if (checkLevel != CheckManager.LevelInt.All)
        //         {
        //             whereList.Add(checkLevel == siteInfo.CheckContentLevel
        //                 ? $"{nameof(Attr.IsChecked)} = '{true}'"
        //                 : $"{nameof(Attr.IsChecked)} = '{false}' AND {nameof(Attr.CheckedLevel)} = {checkLevel}");
        //         }
        //     }

        //     if (onlyAdminId.HasValue)
        //     {
        //         whereList.Add($"{nameof(Attr.AdminId)} = {onlyAdminId.Value}");
        //     }

        //     if (isWritingOnly)
        //     {
        //         whereList.Add($"{nameof(Attr.UserId)} > 0");
        //     }

        //     return $"WHERE {string.Join(" AND ", whereList)}";
        // }

        public Query GetCacheWhereString(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId)
        {
            var query = Q.Where(Attr.SiteId, siteInfo.Id).Where(Attr.ChannelId, channelInfo.Id).WhereNot(Attr.SourceId, SourceManager.Preview);
            if (onlyAdminId.HasValue)
            {
                query.Where(Attr.AdminId, onlyAdminId.Value);
            }

            return query;
        }

        public List<ContentInfo> GetStlDataSourceChecked(List<int> channelIdList, int startNum, int totalNum, TaxisType taxisType, Query query, NameValueCollection others)
        {
            if (channelIdList == null || channelIdList.Count == 0)
            {
                return null;
            }

            QueryOrder(query, taxisType);
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
                                        query.WhereNot(attributeName, value);
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

            return startNum <= 1 ? GetStlDataSourceByContentNumAndWhereString(totalNum, query) : GetStlDataSourceByStartNum(startNum, totalNum, query);
        }

        private List<ContentInfo> GetStlDataSourceByContentNumAndWhereString(int totalNum, Query query)
        {
            QuerySelectMinColumns(query);
            query.Limit(totalNum);

            return _repository.GetAll(query).ToList();
        }

        private List<ContentInfo> GetStlDataSourceByStartNum(int startNum, int totalNum, Query query)
        {
            QuerySelectMinColumns(query);
            query.Offset(startNum - 1).Limit(totalNum);

            return _repository.GetAll(query).ToList();
        }

        // public DataSet GetDataSetOfAdminExcludeRecycle(int siteId, DateTime begin, DateTime end)
        // {
        //     var sqlString = GetSqlStringOfAdminExcludeRecycle(siteId, begin, end);

        //     return DatabaseUtils.GetDataSet(sqlString);
        // }
    }
}
