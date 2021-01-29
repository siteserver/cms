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

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "列表项循环", Description = "通过 stl:each 标签在模板中遍历指定的列表项")]
    public static class StlEach
    {
        public const string ElementName = "stl:each";

        [StlAttribute(Title = "循环类型")]
        private const string Type = nameof(Type);

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

            var type = listInfo.Others.Get(Type);
            if (string.IsNullOrEmpty(type))
            {
                type = nameof(Content.ImageUrl);
            }

            var valueList = new List<string>();
            var content = await parseManager.GetContentAsync();

            if (content != null)
            {
                if (!string.IsNullOrEmpty(content.Get<string>(type)))
                {
                    valueList.Add(content.Get<string>(type));
                }

                var countName = ColumnsManager.GetCountName(type);
                var total = content.Get<int>(countName);
                for (var i = 1; i <= total; i++)
                {
                    var extendName = ColumnsManager.GetExtendName(type, i);
                    var extend = content.Get<string>(extendName);
                    valueList.Add(extend);
                }

                //var extendAttributeName = ColumnsManager.GetExtendAttributeName(type);
                //var extendValues = content.Get<string>(extendAttributeName);
                //if (!string.IsNullOrEmpty(extendValues))
                //{
                //    foreach (var extendValue in ListUtils.GetStringList(extendValues))
                //    {
                //        valueList.Add(extendValue);
                //    }
                //}

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
