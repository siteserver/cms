using System;
using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache.Content
{
    public static partial class ContentManager
    {
        public static void RemoveCacheBySiteId(string tableName, int siteId)
        {
            foreach (var channelId in ChannelManager.GetChannelIdList(siteId))
            {
                ListCache.Remove(channelId);
                ContentCache.Remove(channelId);
            }
            CountCache.Clear(tableName);
            StlContentCache.ClearCache();
        }

        public static void RemoveCache(string tableName, int channelId)
        {
            ListCache.Remove(channelId);
            ContentCache.Remove(channelId);
            CountCache.Clear(tableName);
            StlContentCache.ClearCache();
        }

        public static void RemoveCountCache(string tableName)
        {
            CountCache.Clear(tableName);
            StlContentCache.ClearCache();
        }

        public static void InsertCache(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            if (contentInfo.SourceId == SourceManager.Preview) return;

            ListCache.Add(channelInfo, contentInfo);

            var dict = ContentCache.GetContentDict(contentInfo.ChannelId);
            dict[contentInfo.Id] = contentInfo;

            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            CountCache.Add(tableName, contentInfo);

            StlContentCache.ClearCache();
        }

        public static void UpdateCache(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfoToUpdate)
        {
            var dict = ContentCache.GetContentDict(channelInfo.Id);

            var contentInfo = GetContentInfo(siteInfo, channelInfo, contentInfoToUpdate.Id);
            if (contentInfo != null)
            {
                if (ListCache.IsChanged(channelInfo, contentInfo, contentInfoToUpdate))
                {
                    ListCache.Remove(channelInfo.Id);
                }

                if (CountCache.IsChanged(contentInfo, contentInfoToUpdate))
                {
                    var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
                    CountCache.Remove(tableName, contentInfo);
                    CountCache.Add(tableName, contentInfoToUpdate);
                }
            }
            
            dict[contentInfoToUpdate.Id] = contentInfoToUpdate;

            StlContentCache.ClearCache();
        }

        public static List<ContentColumn> GetContentColumns(SiteInfo siteInfo, ChannelInfo channelInfo, bool includeAll)
        {
            var columns = new List<ContentColumn>();
            
            var attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(channelInfo.Additional.ContentAttributesOfDisplay);
            var pluginIds = PluginContentManager.GetContentPluginIds(channelInfo);
            var pluginColumns = PluginContentManager.GetContentColumns(pluginIds);

            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(TableStyleManager.GetContentStyleInfoList(siteInfo, channelInfo));

            styleInfoList.Insert(0, new TableStyleInfo
            {
                AttributeName = ContentAttribute.Sequence,
                DisplayName = "序号"
            });

            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.InputType == InputType.TextEditor) continue;

                var column = new ContentColumn
                {
                    AttributeName = styleInfo.AttributeName,
                    DisplayName = styleInfo.DisplayName,
                    InputType = styleInfo.InputType
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

        public static ContentInfo Calculate(int sequence, ContentInfo contentInfo, List<ContentColumn> columns, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns)
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
                        var adminInfo = AdminManager.GetAdminInfoByUserId(contentInfo.AdminId);
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
                        var userInfo = UserManager.GetUserInfoByUserId(contentInfo.UserId);
                        if (userInfo != null)
                        {
                            value = string.IsNullOrEmpty(userInfo.DisplayName) ? userInfo.UserName : userInfo.DisplayName;
                        }
                    }
                    retVal.Set(ContentAttribute.UserId, value);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.SourceId))
                {
                    retVal.Set(ContentAttribute.SourceId, SourceManager.GetSourceName(contentInfo.SourceId));
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.AddUserName))
                {
                    var value = string.Empty;
                    if (!string.IsNullOrEmpty(contentInfo.AddUserName))
                    {
                        var adminInfo = AdminManager.GetAdminInfoByUserName(contentInfo.AddUserName);
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
                        var adminInfo = AdminManager.GetAdminInfoByUserName(contentInfo.LastEditUserName);
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
                            var value = func(new ContentContextImpl
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

        public static bool IsCreatable(ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            if (channelInfo == null || contentInfo == null) return false;

            //引用链接，不需要生成内容页；引用内容，需要生成内容页；
            if (contentInfo.ReferenceId > 0 &&
                ETranslateContentTypeUtils.GetEnumType(contentInfo.GetString(ContentAttribute.TranslateContentType)) !=
                ETranslateContentType.ReferenceContent)
            {
                return false;
            }
            
            return channelInfo.Additional.IsContentCreatable && string.IsNullOrEmpty(contentInfo.LinkUrl) && contentInfo.IsChecked && contentInfo.SourceId != SourceManager.Preview && contentInfo.ChannelId > 0;
        }
    }
}