using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Datory.Utils;
using SS.CMS.Abstractions;
using SS.CMS.Core.Plugins;

namespace SS.CMS.Core
{
    public class ColumnsManager
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;

        public ColumnsManager(IDatabaseManager databaseManager, IPluginManager pluginManager)
        {
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
        }

        private const string Sequence = nameof(Sequence);                            //序号
        private const string ChannelName = nameof(ChannelName);
        private const string AdminName = nameof(AdminName);
        private const string LastEditAdminName = nameof(LastEditAdminName);
        private const string UserName = nameof(UserName);
        private const string CheckAdminName = nameof(CheckAdminName);
        private const string SourceName = nameof(SourceName);
        private const string State = nameof(State);

        private static readonly List<string> CalculatedAttributes = new List<string>
        {
            Sequence,
            nameof(Content.ChannelId),
            nameof(Content.SourceId),
            nameof(Content.AdminId),
            nameof(Content.LastEditAdminId),
            nameof(Content.UserId),
            nameof(Content.CheckAdminId)
        };

        private static readonly List<string> UnSearchableAttributes = new List<string>
        {
            Sequence,
            nameof(Content.ChannelId),
            nameof(Content.AddDate),
            nameof(Content.LastEditDate),
            nameof(Content.LastHitsDate),
            nameof(Content.GroupNames),
            nameof(Content.TagNames),
            nameof(Content.SourceId),
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
                    AttributeName = nameof(Content.ChannelId),
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

        public async Task<Content> CalculateContentListAsync(int sequence, Site site, int currentChannelId, Content source, List<ContentColumn> columns, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns)
        {
            if (source == null) return null;

            var content = new Content(source.ToDictionary(new List<string> {ContentAttribute.Content}));

            content.Set(State, CheckManager.GetCheckState(site, content));

            foreach (var column in columns)
            {
                if (!StringUtils.ContainsIgnoreCase(CalculatedAttributes, column.AttributeName)) continue;

                if (StringUtils.EqualsIgnoreCase(column.AttributeName, Sequence))
                {
                    content.Set(Sequence, sequence);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(Content.ChannelId)))
                {
                    var channelName = await _databaseManager.ChannelRepository.GetChannelNameNavigationAsync(source.SiteId, currentChannelId, source.ChannelId);
                    content.Set(ChannelName, channelName);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(Content.AdminId)))
                {
                    var adminName = await _databaseManager.AdministratorRepository.GetDisplayAsync(source.AdminId);
                    content.Set(AdminName, adminName);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(Content.LastEditAdminId)))
                {
                    var lastEditAdminName =
                        await _databaseManager.AdministratorRepository.GetDisplayAsync(source.LastEditAdminId);
                    content.Set(LastEditAdminName, lastEditAdminName);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(Content.UserId)))
                {
                    var userName = await _databaseManager.UserRepository.GetDisplayAsync(source.UserId);
                    content.Set(UserName, userName);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(Content.CheckAdminId)))
                {
                    var checkAdminName =
                        await _databaseManager.AdministratorRepository.GetDisplayAsync(source.CheckAdminId);
                    content.Set(CheckAdminName, checkAdminName);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, nameof(Content.SourceId)))
                {
                    var sourceName = await SourceManager.GetSourceNameAsync(_databaseManager, source.SiteId, source.SourceId);
                    content.Set(SourceName, sourceName);
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
                                SiteId = source.SiteId,
                                ChannelId = source.ChannelId,
                                ContentId = source.Id
                            });

                            content.Set(attributeName, value);
                        }
                        catch (Exception ex)
                        {
                            await _databaseManager.ErrorLogRepository.AddErrorLogAsync(pluginId, ex);
                        }
                    }
                }
            }

            return content;
        }

        public enum PageType
        {
            Contents,
            SearchContents,
            CheckContents,
            RecycleContents
        }

        public async Task<List<ContentColumn>> GetContentListColumnsAsync(Site site, Channel channel, PageType pageType)
        {
            var columns = new List<ContentColumn>();
            var listColumns = new List<string>();

            if (pageType == PageType.Contents)
            {
                listColumns = Utilities.GetStringList(channel.ListColumns);
                if (listColumns.Count == 0)
                {
                    listColumns.Add(nameof(Content.Title));
                    listColumns.Add(nameof(Content.AddDate));
                }
            }
            else if (pageType == PageType.SearchContents)
            {
                listColumns = Utilities.GetStringList(site.SearchListColumns);
                if (listColumns.Count == 0)
                {
                    listColumns.Add(nameof(Content.Title));
                    listColumns.Add(nameof(Content.AddDate));
                }
            }
            else if (pageType == PageType.CheckContents)
            {
                listColumns = Utilities.GetStringList(site.CheckListColumns);
                if (listColumns.Count == 0)
                {
                    listColumns.Add(nameof(Content.Title));
                    listColumns.Add(nameof(Content.ChannelId));
                    listColumns.Add(nameof(Content.AdminId));
                    listColumns.Add(nameof(Content.AddDate));
                }
            }
            else if (pageType == PageType.RecycleContents)
            {
                listColumns = Utilities.GetStringList(site.RecycleListColumns);
                if (listColumns.Count == 0)
                {
                    listColumns.Add(nameof(Content.Title));
                }
            }

            var pluginIds = _pluginManager.GetContentPluginIds(channel);
            var pluginColumns = await _pluginManager.GetContentColumnsAsync(pluginIds);

            var tableName = _databaseManager.ChannelRepository.GetTableName(site, channel);
            var styleList = GetContentListStyles(await _databaseManager.TableStyleRepository.GetContentStyleListAsync(channel, tableName));

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
                    if (StringUtils.ContainsIgnoreCase(listColumns, style.AttributeName))
                    {
                        column.IsList = true;
                    }
                }

                if (!StringUtils.ContainsIgnoreCase(UnSearchableAttributes, style.AttributeName))
                {
                    column.IsSearchable = true;
                }

                columns.Add(column);
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

                        if (StringUtils.ContainsIgnoreCase(listColumns, attributeName))
                        {
                            column.IsList = true;
                        }

                        columns.Add(column);
                    }
                }
            }

            return columns;
        }

        public static async Task<Dictionary<string, object>> SaveAttributesAsync(IPathManager pathManager, Site site, List<TableStyle> styleList, NameValueCollection formCollection, List<string> dontAddAttributes)
        {
            var dict = new Dictionary<string, object>();

            if (dontAddAttributes == null)
            {
                dontAddAttributes = new List<string>();
            }

            foreach (var style in styleList)
            {
                if (StringUtils.ContainsIgnoreCase(dontAddAttributes, style.AttributeName)) continue;
                //var theValue = GetValueByForm(style, site, formCollection);

                var theValue = formCollection[style.AttributeName] ?? string.Empty;
                var inputType = style.InputType;
                if (inputType == InputType.TextEditor)
                {
                    theValue = await ContentUtility.TextEditorContentEncodeAsync(pathManager, site, theValue);
                    theValue = UEditorUtils.TranslateToStlElement(theValue);
                }

                if (inputType != InputType.TextEditor && inputType != InputType.Image && inputType != InputType.File && inputType != InputType.Video && style.AttributeName != ContentAttribute.LinkUrl)
                {
                    theValue = AttackUtils.FilterXss(theValue);
                }

                dict[style.AttributeName] = theValue;

                if (style.IsFormatString)
                {
                    var formatString = TranslateUtils.ToBool(formCollection[style.AttributeName + "_formatStrong"]);
                    var formatEm = TranslateUtils.ToBool(formCollection[style.AttributeName + "_formatEM"]);
                    var formatU = TranslateUtils.ToBool(formCollection[style.AttributeName + "_formatU"]);
                    var formatColor = formCollection[style.AttributeName + "_formatColor"];
                    var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);

                    dict[ContentAttribute.GetFormatStringAttributeName(style.AttributeName)] = theFormatString;
                }

                if (inputType == InputType.Image || inputType == InputType.File || inputType == InputType.Video)
                {
                    var attributeName = ContentAttribute.GetExtendAttributeName(style.AttributeName);
                    dict[attributeName] = formCollection[attributeName];
                }
            }

            return dict;
        }
    }
}
