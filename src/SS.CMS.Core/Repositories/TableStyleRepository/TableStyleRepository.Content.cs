using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Core.Services;
using SS.CMS.Utils;
using SS.CMS.Enums;
using System.Text;
using SS.CMS.Core.Models.Attributes;
using System;
using SS.CMS.Services;
using SS.CMS.Core.Common;

namespace SS.CMS.Core.Repositories
{
    public partial class TableStyleRepository
    {
        public async Task<List<TableStyle>> GetContentStyleInfoListAsync(Site site, Channel channel)
        {
            var relatedIdentities = GetRelatedIdentities(channel);
            var tableName = _channelRepository.GetTableName(site, channel);
            return await GetStyleInfoListAsync(tableName, relatedIdentities);
        }

        public async Task<List<InputListItem>> GetContentsColumnsAsync(Site siteInfo, Channel channelInfo, bool includeAll, IPluginManager pluginManager)
        {
            var items = new List<InputListItem>();

            var attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(channelInfo.ContentAttributesOfDisplay);
            var pluginIds = pluginManager.GetContentPluginIds(channelInfo);
            var pluginColumns = await pluginManager.GetContentColumnsAsync(pluginIds);

            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(await GetContentStyleInfoListAsync(siteInfo, channelInfo));

            styleInfoList.Insert(0, new TableStyle
            {
                AttributeName = ContentAttribute.Sequence,
                DisplayName = "序号"
            });

            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.Type == InputType.TextEditor) continue;

                var listitem = new InputListItem
                {
                    Text = styleInfo.DisplayName,
                    Value = styleInfo.AttributeName
                };
                if (styleInfo.AttributeName == ContentAttribute.Title)
                {
                    listitem.Selected = true;
                }
                else
                {
                    if (attributesOfDisplay.Contains(styleInfo.AttributeName))
                    {
                        listitem.Selected = true;
                    }
                }

                if (includeAll || listitem.Selected)
                {
                    items.Add(listitem);
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
                        var listitem = new InputListItem
                        {
                            Text = $"{columnName}({pluginId})",
                            Value = attributeName
                        };

                        if (attributesOfDisplay.Contains(attributeName))
                        {
                            listitem.Selected = true;
                        }

                        if (includeAll || listitem.Selected)
                        {
                            items.Add(listitem);
                        }
                    }
                }
            }

            return items;
        }

        public async Task<List<ContentColumn>> GetContentColumnsAsync(Site siteInfo, Channel channelInfo, bool includeAll, IPluginManager pluginManager)
        {
            var columns = new List<ContentColumn>();

            var attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(channelInfo.ContentAttributesOfDisplay);
            var pluginIds = pluginManager.GetContentPluginIds(channelInfo);
            var pluginColumns = await pluginManager.GetContentColumnsAsync(pluginIds);

            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(await GetContentStyleInfoListAsync(siteInfo, channelInfo));

            styleInfoList.Insert(0, new TableStyle
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
    }
}