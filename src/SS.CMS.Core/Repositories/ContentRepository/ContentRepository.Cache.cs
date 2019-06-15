using System;
using System.Collections.Generic;
using System.Linq;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Stl;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.Plugin;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public void RemoveCacheBySiteId(string tableName, int siteId)
        {
            foreach (var channelId in ChannelManager.GetChannelIdList(siteId))
            {
                ListRemove(channelId);
                ContentRemove(channelId);
            }
            CountClear(tableName);
            StlClearCache();
        }

        public void RemoveCache(string tableName, int channelId)
        {
            ListRemove(channelId);
            ContentRemove(channelId);
            CountClear(tableName);
            StlClearCache();
        }

        public void RemoveCountCache(string tableName)
        {
            CountClear(tableName);
            StlClearCache();
        }

        public void InsertCache(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            if (contentInfo.SourceId == SourceManager.Preview) return;

            ListAdd(channelInfo, contentInfo);

            var dict = ContentGetContentDict(contentInfo.ChannelId);
            dict[contentInfo.Id] = contentInfo;

            var tableName = ChannelManager.GetTableName(_pluginManager, siteInfo, channelInfo);
            CountAdd(tableName, contentInfo);

            StlClearCache();
        }

        public void UpdateCache(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfoToUpdate)
        {
            var dict = ContentGetContentDict(channelInfo.Id);

            var contentInfo = GetContentInfo(siteInfo, channelInfo, contentInfoToUpdate.Id);
            if (contentInfo != null)
            {
                if (ListIsChanged(channelInfo, contentInfo, contentInfoToUpdate))
                {
                    ListRemove(channelInfo.Id);
                }

                if (CountIsChanged(contentInfo, contentInfoToUpdate))
                {
                    var tableName = ChannelManager.GetTableName(_pluginManager, siteInfo, channelInfo);
                    CountRemove(tableName, contentInfo);
                    CountAdd(tableName, contentInfoToUpdate);
                }
            }

            dict[contentInfoToUpdate.Id] = contentInfoToUpdate;

            StlClearCache();
        }

        public List<ContentColumn> GetContentColumns(SiteInfo siteInfo, ChannelInfo channelInfo, bool includeAll)
        {
            var columns = new List<ContentColumn>();

            var attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(channelInfo.ContentAttributesOfDisplay);
            var pluginIds = _pluginManager.GetContentPluginIds(channelInfo);
            var pluginColumns = _pluginManager.GetContentColumns(pluginIds);

            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(_tableStyleRepository.GetContentStyleInfoList(_pluginManager, siteInfo, channelInfo));

            styleInfoList.Insert(0, new TableStyleInfo
            {
                AttributeName = ContentAttribute.Sequence,
                DisplayName = "序号"
            });

            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.Type == InputType.TextEditor) continue;

                var column = new ContentColumn
                {
                    AttributeName = styleInfo.AttributeName,
                    DisplayName = styleInfo.DisplayName,
                    InputType = styleInfo.Type
                };
                if (styleInfo.AttributeName == ContentAttribute.Title)
                {
                    column.IsList = true;
                }
                else
                {
                    if (attributesOfDisplay.Contains(styleInfo.AttributeName))
                    {
                        column.IsList = true;
                    }
                }

                if (StringUtils.ContainsIgnoreCase(ContentAttribute.CalculateAttributes.Value, styleInfo.AttributeName))
                {
                    column.IsCalculate = true;
                }

                if (includeAll || column.IsList)
                {
                    columns.Add(column);
                }
            }

            if (pluginColumns != null)
            {
                foreach (var pluginId in pluginColumns.Keys)
                {
                    var contentColumns = pluginColumns[pluginId];
                    if (contentColumns == null || contentColumns.Count == 0) continue;

                    foreach (var columnName in contentColumns.Keys)
                    {
                        var attributeName = $"{pluginId}:{columnName}";
                        var column = new ContentColumn
                        {
                            AttributeName = attributeName,
                            DisplayName = $"{columnName}({pluginId})",
                            InputType = InputType.Text,
                            IsCalculate = true
                        };

                        if (attributesOfDisplay.Contains(attributeName))
                        {
                            column.IsList = true;
                        }

                        if (includeAll || column.IsList)
                        {
                            columns.Add(column);
                        }
                    }
                }
            }

            return columns;
        }

        public ContentInfo Calculate(int sequence, ContentInfo contentInfo, List<ContentColumn> columns, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns)
        {
            if (contentInfo == null) return null;

            var retVal = new ContentInfo(contentInfo.ToDictionary());

            foreach (var column in columns)
            {
                if (!column.IsCalculate) continue;

                if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.Sequence))
                {
                    retVal.Set(ContentAttribute.Sequence, sequence);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.AdminId))
                {
                    var value = string.Empty;
                    if (contentInfo.AdminId > 0)
                    {
                        var adminInfo = _administratorRepository.GetAdminInfoByUserId(contentInfo.AdminId);
                        if (adminInfo != null)
                        {
                            value = string.IsNullOrEmpty(adminInfo.DisplayName) ? adminInfo.UserName : adminInfo.DisplayName;
                        }
                    }
                    retVal.Set(ContentAttribute.AdminId, value);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.UserId))
                {
                    var value = string.Empty;
                    if (contentInfo.UserId > 0)
                    {
                        var userInfo = _userRepository.GetUserInfoByUserId(contentInfo.UserId);
                        if (userInfo != null)
                        {
                            value = string.IsNullOrEmpty(userInfo.DisplayName) ? userInfo.UserName : userInfo.DisplayName;
                        }
                    }
                    retVal.Set(ContentAttribute.UserId, value);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.SourceId))
                {
                    retVal.Set(ContentAttribute.SourceId, _siteRepository.GetSourceName(contentInfo.SourceId));
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.AddUserName))
                {
                    var value = string.Empty;
                    if (!string.IsNullOrEmpty(contentInfo.AddUserName))
                    {
                        var adminInfo = _administratorRepository.GetAdminInfoByUserName(contentInfo.AddUserName);
                        if (adminInfo != null)
                        {
                            value = string.IsNullOrEmpty(adminInfo.DisplayName) ? adminInfo.UserName : adminInfo.DisplayName;
                        }
                    }
                    retVal.Set(ContentAttribute.AddUserName, value);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.LastEditUserName))
                {
                    var value = string.Empty;
                    if (!string.IsNullOrEmpty(contentInfo.LastEditUserName))
                    {
                        var adminInfo = _administratorRepository.GetAdminInfoByUserName(contentInfo.LastEditUserName);
                        if (adminInfo != null)
                        {
                            value = string.IsNullOrEmpty(adminInfo.DisplayName) ? adminInfo.UserName : adminInfo.DisplayName;
                        }
                    }
                    retVal.Set(ContentAttribute.LastEditUserName, value);
                }
            }

            if (pluginColumns != null)
            {
                foreach (var pluginId in pluginColumns.Keys)
                {
                    var contentColumns = pluginColumns[pluginId];
                    if (contentColumns == null || contentColumns.Count == 0) continue;

                    foreach (var columnName in contentColumns.Keys)
                    {
                        var attributeName = $"{pluginId}:{columnName}";
                        if (columns.All(x => x.AttributeName != attributeName)) continue;

                        try
                        {
                            var func = contentColumns[columnName];
                            var value = func(new ContentContext
                            {
                                SiteId = contentInfo.SiteId,
                                ChannelId = contentInfo.ChannelId,
                                ContentId = contentInfo.Id
                            });

                            retVal.Set(attributeName, value);
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddErrorLog(pluginId, ex);
                        }
                    }
                }
            }

            return retVal;
        }

        public bool IsCreatable(ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            if (channelInfo == null || contentInfo == null) return false;

            //引用链接，不需要生成内容页；引用内容，需要生成内容页；
            if (contentInfo.ReferenceId > 0 &&
                TranslateContentType.Parse(contentInfo.TranslateContentType) !=
                TranslateContentType.ReferenceContent)
            {
                return false;
            }

            return channelInfo.IsContentCreatable && string.IsNullOrEmpty(contentInfo.LinkUrl) && contentInfo.Checked && contentInfo.SourceId != SourceManager.Preview && contentInfo.ChannelId > 0;
        }
    }
}