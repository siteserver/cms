using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteServer.CMS.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.DataCache.Content
{
    public static partial class ContentManager
    {
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

        public static async Task InsertCacheAsync(Site site, Channel channel, Model.Content content)
        {
            if (content.SourceId == SourceManager.Preview) return;

            var dict = ContentCache.GetContentDict(content.ChannelId);
            dict[content.Id] = content;

            var tableName = await ChannelManager.GetTableNameAsync(site, channel);
            CountCache.Add(tableName, content);

            StlContentCache.ClearCache();
        }

        public static async Task UpdateCacheAsync(Site site, Channel channel, Model.Content content)
        {
            var dict = ContentCache.GetContentDict(channel.Id);

            ListCache.Remove(channel.Id);

            var tableName = await ChannelManager.GetTableNameAsync(site, channel);
            CountCache.Remove(tableName, content);
            CountCache.Add(tableName, content);

            dict[content.Id] = content;

            StlContentCache.ClearCache();
        }

        public static async Task<List<ContentColumn>> GetContentColumnsAsync(Site site, Channel channel, bool includeAll)
        {
            var columns = new List<ContentColumn>();

            var attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(channel.ContentAttributesOfDisplay);
            var pluginIds = PluginContentManager.GetContentPluginIds(channel);
            var pluginColumns = await PluginContentManager.GetContentColumnsAsync(pluginIds);

            var styleList = ContentUtility.GetAllTableStyleList(await TableStyleManager.GetContentStyleListAsync(site, channel));

            styleList.Insert(0, new TableStyle
            {
                AttributeName = ContentAttribute.Sequence,
                DisplayName = "序号"
            });

            foreach (var style in styleList)
            {
                if (!includeAll && style.Type == InputType.TextEditor) continue;

                var column = new ContentColumn
                {
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.Type
                };
                if (style.AttributeName == ContentAttribute.Title)
                {
                    column.IsList = true;
                }
                else
                {
                    if (attributesOfDisplay.Contains(style.AttributeName))
                    {
                        column.IsList = true;
                    }
                }

                if (StringUtils.ContainsIgnoreCase(ContentAttribute.CalculateAttributes.Value, style.AttributeName))
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

        public static async Task<Model.Content> CalculateAsync(int sequence, Model.Content content, List<ContentColumn> columns, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns)
        {
            if (content == null) return null;

            var retVal = new Model.Content(content.ToDictionary());

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
                    if (content.AdminId > 0)
                    {
                        var adminInfo = await AdminManager.GetByUserIdAsync(content.AdminId);
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
                    if (content.UserId > 0)
                    {
                        var userInfo = await UserManager.GetByUserIdAsync(content.UserId);
                        if (userInfo != null)
                        {
                            value = string.IsNullOrEmpty(userInfo.DisplayName) ? userInfo.UserName : userInfo.DisplayName;
                        }
                    }
                    retVal.Set(ContentAttribute.UserId, value);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.SourceId))
                {
                    retVal.Set(ContentAttribute.SourceId, SourceManager.GetSourceNameAsync(content.SourceId));
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.AddUserName))
                {
                    var value = string.Empty;
                    if (!string.IsNullOrEmpty(content.AddUserName))
                    {
                        var adminInfo = await AdminManager.GetByUserNameAsync(content.AddUserName);
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
                    if (!string.IsNullOrEmpty(content.LastEditUserName))
                    {
                        var adminInfo = await AdminManager.GetByUserNameAsync(content.LastEditUserName);
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
                                SiteId = content.SiteId,
                                ChannelId = content.ChannelId,
                                ContentId = content.Id
                            });

                            retVal.Set(attributeName, value);
                        }
                        catch (Exception ex)
                        {
                            await LogUtils.AddErrorLogAsync(pluginId, ex);
                        }
                    }
                }
            }

            return retVal;
        }

        public static bool IsCreatable(Channel channel, Model.Content content)
        {
            if (channel == null || content == null) return false;

            //引用链接，不需要生成内容页；引用内容，需要生成内容页；
            if (content.ReferenceId > 0 &&
                ETranslateContentTypeUtils.GetEnumType(content.Get<string>(ContentAttribute.TranslateContentType)) !=
                ETranslateContentType.ReferenceContent)
            {
                return false;
            }

            return channel.IsContentCreatable && string.IsNullOrEmpty(content.LinkUrl) && content.Checked && content.SourceId != SourceManager.Preview && content.ChannelId > 0;
        }
    }
}