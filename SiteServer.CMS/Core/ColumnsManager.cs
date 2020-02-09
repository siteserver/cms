using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.Core
{
    public static class ColumnsManager
    {
        public const string Sequence = nameof(Sequence);                            //序号
        public const string ChannelName = nameof(ChannelName);
        public const string AdminName = nameof(AdminName);
        public const string LastEditAdminName = nameof(LastEditAdminName);
        public const string UserName = nameof(UserName);
        public const string CheckAdminName = nameof(CheckAdminName);
        public const string SourceName = nameof(SourceName);

        public static readonly List<string> CalculatedAttributes = new List<string>
        {
            Sequence,
            nameof(Content.ChannelId),
            nameof(Content.SourceId),
            nameof(Content.AdminId),
            nameof(Content.LastEditAdminId),
            nameof(Content.UserId),
            nameof(Content.CheckAdminId)
        };

        public static readonly List<string> UnSearchableAttributes = new List<string>
        {
            Sequence,
            nameof(Content.SourceId),
            UserName,
            nameof(Content.CheckAdminId),
            nameof(Content.CheckDate),
            nameof(Content.CheckReasons),
        };

        public static List<TableStyle> GetContentListStyles(List<TableStyle> tableStyleList)
        {
            var taxis = 1;
            var list = new List<TableStyle>
            {
                new TableStyle
                {
                    AttributeName = nameof(Content.Id),
                    DisplayName = "内容Id",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.Guid),
                    DisplayName = "识别码",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.Title),
                    DisplayName = "标题",
                    Taxis = taxis++
                }
            };

            if (tableStyleList != null)
            {
                foreach (var tableStyle in tableStyleList)
                {
                    if (!list.Exists(t => t.AttributeName == tableStyle.AttributeName))
                    {
                        list.Add(new TableStyle
                        {
                            AttributeName = tableStyle.AttributeName,
                            DisplayName = tableStyle.DisplayName,
                            InputType = tableStyle.InputType,
                            Taxis = taxis++
                        });
                    }
                }
            }

            list.AddRange(new List<TableStyle>
            {
                new TableStyle
                {
                    AttributeName = nameof(Content.LinkUrl),
                    DisplayName = "外部链接",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(ChannelName),
                    DisplayName = "所属栏目",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.AddDate),
                    DisplayName = "添加时间",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.LastEditDate),
                    DisplayName = "最后修改时间",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.GroupNames),
                    DisplayName = "内容组",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.TagNames),
                    DisplayName = "标签",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.AdminId),
                    DisplayName = "添加人",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.LastEditAdminId),
                    DisplayName = "最后修改人",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.UserId),
                    DisplayName = "投稿用户",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.SourceId),
                    DisplayName = "来源标识",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.Hits),
                    DisplayName = "点击量",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.HitsByDay),
                    DisplayName = "日点击",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.HitsByWeek),
                    DisplayName = "周点击",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.HitsByMonth),
                    DisplayName = "月点击",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.LastHitsDate),
                    DisplayName = "最后点击时间",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.Downloads),
                    DisplayName = "下载量",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.CheckAdminId),
                    DisplayName = "审核人",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.CheckDate),
                    DisplayName = "审核时间",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.CheckReasons),
                    DisplayName = "审核原因",
                    Taxis = taxis
                },
            });

            return list.OrderBy(styleInfo => styleInfo.Taxis == 0 ? int.MaxValue : styleInfo.Taxis).ToList();
        }

        public static async Task<Content> CalculateContentListAsync(int sequence, int channelId, Content content, List<ContentColumn> columns, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns)
        {
            if (content == null) return null;

            var retVal = new Content(content.ToDictionary(new List<string> {ContentAttribute.Content}));

            foreach (var column in columns)
            {
                if (!StringUtils.ContainsIgnoreCase(CalculatedAttributes, column.AttributeName)) continue;

                if (StringUtils.EqualsIgnoreCase(column.AttributeName, Sequence))
                {
                    retVal.Set(Sequence, sequence);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ChannelName))
                {
                    var channelName = await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(content.SiteId, channelId, content.ChannelId);
                    retVal.Set(ChannelName, channelName);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(Content.AdminId)))
                {
                    var value = string.Empty;
                    if (content.AdminId > 0)
                    {
                        var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(content.AdminId);
                        if (adminInfo != null)
                        {
                            value = string.IsNullOrEmpty(adminInfo.DisplayName) || adminInfo.UserName == adminInfo.DisplayName ? adminInfo.UserName : $"{adminInfo.DisplayName}({adminInfo.UserName})";
                        }
                    }
                    retVal.Set(AdminName, value);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(Content.LastEditAdminId)))
                {
                    var value = string.Empty;
                    if (content.LastEditAdminId > 0)
                    {
                        var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(content.LastEditAdminId);
                        if (adminInfo != null)
                        {
                            value = string.IsNullOrEmpty(adminInfo.DisplayName) || adminInfo.UserName == adminInfo.DisplayName ? adminInfo.UserName : $"{adminInfo.DisplayName}({adminInfo.UserName})";
                        }
                    }
                    retVal.Set(LastEditAdminName, value);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(Content.UserId)))
                {
                    var value = string.Empty;
                    if (content.UserId > 0)
                    {
                        var userInfo = await DataProvider.UserRepository.GetByUserIdAsync(content.UserId);
                        if (userInfo != null)
                        {
                            value = string.IsNullOrEmpty(userInfo.DisplayName) || userInfo.UserName == userInfo.DisplayName ? userInfo.UserName : $"{userInfo.DisplayName}({userInfo.UserName})";
                        }
                    }
                    retVal.Set(UserName, value);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(Content.CheckAdminId)))
                {
                    var value = string.Empty;
                    if (content.CheckAdminId > 0)
                    {
                        var adminInfo = await DataProvider.AdministratorRepository.GetByUserIdAsync(content.CheckAdminId);
                        if (adminInfo != null)
                        {
                            value = string.IsNullOrEmpty(adminInfo.DisplayName) || adminInfo.UserName == adminInfo.DisplayName ? adminInfo.UserName : $"{adminInfo.DisplayName}({adminInfo.UserName})";
                        }
                    }
                    retVal.Set(CheckAdminName, value);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(Content.SourceId)))
                {
                    retVal.Set(SourceName, SourceManager.GetSourceNameAsync(content.SourceId));
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

        public static async Task<List<ContentColumn>> GetContentListColumnsAsync(Site site, Channel channel, bool includeAll)
        {
            var columns = new List<ContentColumn>();

            var attributesOfDisplay = Utilities.GetStringList(channel.ListColumns);
            var pluginIds = PluginContentManager.GetContentPluginIds(channel);
            var pluginColumns = await PluginContentManager.GetContentColumnsAsync(pluginIds);

            var styleList = GetContentListStyles(await DataProvider.TableStyleRepository.GetContentStyleListAsync(site, channel));

            styleList.Insert(0, new TableStyle
            {
                AttributeName = Sequence,
                DisplayName = "序号"
            });

            foreach (var style in styleList)
            {
                if (string.IsNullOrEmpty(style.DisplayName) || style.InputType == InputType.TextEditor) continue;

                var column = new ContentColumn
                {
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType
                };
                if (style.AttributeName == nameof(Content.Title))
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

                if (!StringUtils.ContainsIgnoreCase(UnSearchableAttributes, style.AttributeName))
                {
                    column.IsSearchable = true;
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
                            InputType = InputType.Text
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

        //public async Task<List<InputListItem>> GetContentsColumnsAsync(Site site, Channel channel, bool includeAll)
        //{
        //    var items = new List<InputListItem>();

        //    var attributesOfDisplay = Utilities.GetStringList(channel.ListColumns);
        //    var pluginIds = PluginContentManager.GetContentPluginIds(channel);
        //    var pluginColumns = await PluginContentManager.GetContentColumnsAsync(pluginIds);

        //    var styleList = ColumnsManager.GetContentListStyles(await DataProvider.TableStyleRepository.GetContentStyleListAsync(site, channel));

        //    styleList.Insert(0, new TableStyle
        //    {
        //        AttributeName = ContentAttribute.Sequence,
        //        DisplayName = "序号"
        //    });

        //    foreach (var style in styleList)
        //    {
        //        if (style.InputType == InputType.TextEditor) continue;

        //        var listitem = new InputListItem
        //        {
        //            Text = style.DisplayName,
        //            Value = style.AttributeName
        //        };
        //        if (style.AttributeName == ContentAttribute.Title)
        //        {
        //            listitem.Selected = true;
        //        }
        //        else
        //        {
        //            if (attributesOfDisplay.Contains(style.AttributeName))
        //            {
        //                listitem.Selected = true;
        //            }
        //        }

        //        if (includeAll || listitem.Selected)
        //        {
        //            items.Add(listitem);
        //        }
        //    }

        //    if (pluginColumns != null)
        //    {
        //        foreach (var pluginId in pluginColumns.Keys)
        //        {
        //            var contentColumns = pluginColumns[pluginId];
        //            if (contentColumns == null || contentColumns.Count == 0) continue;

        //            foreach (var columnName in contentColumns.Keys)
        //            {
        //                var attributeName = $"{pluginId}:{columnName}";
        //                var listitem = new InputListItem
        //                {
        //                    Text = $"{columnName}({pluginId})",
        //                    Value = attributeName
        //                };

        //                if (attributesOfDisplay.Contains(attributeName))
        //                {
        //                    listitem.Selected = true;
        //                }

        //                if (includeAll || listitem.Selected)
        //                {
        //                    items.Add(listitem);
        //                }
        //            }
        //        }
        //    }

        //    return items;
        //}
    }
}
