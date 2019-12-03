using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentRepository
    {
        public async Task<List<ContentColumn>> GetContentColumnsAsync(Site site, Channel channel, bool includeAll)
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

        public async Task<Content> CalculateAsync(int sequence, Content content, List<ContentColumn> columns, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns)
        {
            if (content == null) return null;

            var retVal = new Content(content.ToDictionary());

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
                        var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(content.AdminId);
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
                        var userInfo = await DataProvider.UserRepository.GetByUserIdAsync(content.UserId);
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
                        var adminInfo = await DataProvider.AdministratorRepository.GetByUserNameAsync(content.AddUserName);
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
                        var adminInfo = await DataProvider.AdministratorRepository.GetByUserNameAsync(content.LastEditUserName);
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

        
    }
}