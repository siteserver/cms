using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Core.StlParser.Enums;
using SSCMS.Core.StlParser.Mocks;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Models;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Services;
using Datory;
using SSCMS.Utils;
using SSCMS.Enums;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "列表项循环", Description = "通过 stl:each 标签在模板中遍历指定的列表项")]
    public static class StlEach
    {
        public const string ElementName = "stl:each";

        [StlAttribute(Title = "循环类型")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "分割字符串")]
        private const string Separator = nameof(Separator);

        [StlAttribute(Title = "所处上下文")]
        private const string Context = nameof(Context);

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {nameof(Content.ImageUrl), "遍历内容的图片字段"},
            {nameof(Content.VideoUrl), "遍历内容的视频字段"},
            {nameof(Content.FileUrl), "遍历内容的附件字段"}
        };

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var listInfo = await ListInfo.GetListInfoAsync(parseManager, ParseType.Each);
            return await ParseAsync(parseManager, listInfo);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, ListInfo listInfo)
        {
            var pageInfo = parseManager.PageInfo;
            var databaseManager = parseManager.DatabaseManager;

            var type = listInfo.Others.Get(Type);
            if (string.IsNullOrEmpty(type))
            {
                type = nameof(Content.ImageUrl);
            }

            var separator = listInfo.Others.Get(Separator);

            var context = parseManager.ContextInfo.ContextType;
            var contextStr = listInfo.Others.Get(Context);
            if (!string.IsNullOrEmpty(contextStr))
            {
                context = TranslateUtils.ToEnum(contextStr, context);
            }

            var valueList = new List<string>();
            Entity entity = null;
            var inputType = InputType.Text;
            if (context == ParseType.Content)
            {
                entity = await parseManager.GetContentAsync();
                var channel = await parseManager.GetChannelAsync();
                var style = await databaseManager.TableStyleRepository.GetContentStyleAsync(pageInfo.Site, channel, type);
                if (style != null)
                {
                    inputType = style.InputType;
                }
            }
            else if (context == ParseType.Site)
            {
                entity = pageInfo.Site;
                var style = await databaseManager.TableStyleRepository.GetSiteStyleAsync(pageInfo.SiteId, type);
                if (style != null)
                {
                    inputType = style.InputType;
                }
            }
            else
            {
                var channel = await parseManager.GetChannelAsync();
                entity = channel;
                var style = await databaseManager.TableStyleRepository.GetChannelStyleAsync(channel, type);
                if (style != null)
                {
                    inputType = style.InputType;
                }
            }

            if (inputType == InputType.Image || inputType == InputType.Video || inputType == InputType.File)
            {
                if (!string.IsNullOrEmpty(entity.Get<string>(type)))
                {
                    valueList.Add(entity.Get<string>(type));
                }

                var countName = ColumnsManager.GetCountName(type);
                var total = entity.Get<int>(countName);
                for (var i = 1; i <= total; i++)
                {
                    var extendName = ColumnsManager.GetExtendName(type, i);
                    var extend = entity.Get<string>(extendName);
                    valueList.Add(extend);
                }
            }
            else if (inputType == InputType.TextArea)
            {
                var values = entity.Get<string>(type);
                if (string.IsNullOrEmpty(separator))
                {
                    valueList = ListUtils.GetStringListByReturnAndNewline(values);
                }
                else
                {
                    valueList = ListUtils.GetStringList(values, separator);
                }
            }
            else
            {
                var values = entity.Get<string>(type);
                if (string.IsNullOrEmpty(separator))
                {
                    valueList = ListUtils.GetStringList(values);
                }
                else
                {
                    valueList = ListUtils.GetStringList(values, separator);
                }
            }

            if (listInfo.StartNum > 1 || listInfo.TotalNum > 0)
            {
                if (listInfo.StartNum > 1)
                {
                    var count = listInfo.StartNum - 1;
                    if (count > valueList.Count)
                    {
                        count = valueList.Count;
                    }
                    valueList.RemoveRange(0, count);
                }

                if (listInfo.TotalNum > 0)
                {
                    if (listInfo.TotalNum < valueList.Count)
                    {
                        valueList.RemoveRange(listInfo.TotalNum, valueList.Count - listInfo.TotalNum);
                    }
                }
            }

            if (valueList.Count == 0) return string.Empty;

            var dataSource = new List<KeyValuePair<int, object>>();
            var index = 0;
            foreach (var value in valueList)
            {
                dataSource.Add(new KeyValuePair<int, object>(index++, value));
            }

            var builder = new StringBuilder();
            if (listInfo.Layout == ListLayout.None)
            {
                if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
                {
                    builder.Append(listInfo.HeaderTemplate);
                }

                var isAlternative = false;
                var isSeparator = false;
                var isSeparatorRepeat = false;
                if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
                {
                    isAlternative = true;
                }
                if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
                {
                    isSeparator = true;
                }
                if (!string.IsNullOrEmpty(listInfo.SeparatorRepeatTemplate))
                {
                    isSeparatorRepeat = true;
                }

                for (var i = 0; i < dataSource.Count; i++)
                {
                    var each = dataSource[i];

                    pageInfo.EachItems.Push(each);
                    var templateString = isAlternative && i % 2 == 1 ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                    builder.Append(await TemplateUtility.GetEachsTemplateStringAsync(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, parseManager, ParseType.Each));

                    if (isSeparator && i != dataSource.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorTemplate);
                    }

                    if (isSeparatorRepeat && (i + 1) % listInfo.SeparatorRepeat == 0 && i != dataSource.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorRepeatTemplate);
                    }
                }

                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
                {
                    builder.Append(listInfo.FooterTemplate);
                }
            }
            else
            {
                var isAlternative = !string.IsNullOrEmpty(listInfo.AlternatingItemTemplate);

                var tableAttributes = listInfo.GetTableAttributes();
                var cellAttributes = listInfo.GetCellAttributes();

                using var table = new HtmlTable(builder, tableAttributes);
                if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
                {
                    table.StartHead();
                    using (var tHead = table.AddRow())
                    {
                        tHead.AddCell(listInfo.HeaderTemplate, cellAttributes);
                    }
                    table.EndHead();
                }

                table.StartBody();

                var columns = listInfo.Columns <= 1 ? 1 : listInfo.Columns;
                var itemIndex = 0;

                while (true)
                {
                    using var tr = table.AddRow();
                    for (var cell = 1; cell <= columns; cell++)
                    {
                        var cellHtml = string.Empty;
                        if (itemIndex < dataSource.Count)
                        {
                            var each = dataSource[itemIndex];

                            pageInfo.EachItems.Push(each);
                            var templateString = isAlternative && itemIndex % 2 == 1 ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                            cellHtml = await TemplateUtility.GetEachsTemplateStringAsync(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, parseManager, ParseType.Each);
                        }
                        tr.AddCell(cellHtml, cellAttributes);
                        itemIndex++;
                    }
                    if (itemIndex >= dataSource.Count) break;
                }

                table.EndBody();

                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
                {
                    table.StartFoot();
                    using (var tFoot = table.AddRow())
                    {
                        tFoot.AddCell(listInfo.FooterTemplate, cellAttributes);
                    }
                    table.EndFoot();
                }
            }

            return builder.ToString();
        }
    }
}
