using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.StlParser.Mock;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "列表项循环", Description = "通过 stl:each 标签在模板中遍历指定的列表项")]
    public class StlEach
    {
        public const string ElementName = "stl:each";

        [StlAttribute(Title = "循环类型")]
        private const string Type = nameof(Type);

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {ContentAttribute.ImageUrl, "遍历内容的图片字段"},
            {ContentAttribute.VideoUrl, "遍历内容的视频字段"},
            {ContentAttribute.FileUrl, "遍历内容的附件字段"}
        };

        public static async Task<object> ParseAsync(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var listInfo = await ListInfo.GetListInfoAsync(pageInfo, contextInfo, ContextType.Content);

            return await ParseImplAsync(pageInfo, contextInfo, listInfo);
        }

        private static async Task<string> ParseImplAsync(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var type = listInfo.Others.Get(Type);
            if (string.IsNullOrEmpty(type))
            {
                type = ContentAttribute.ImageUrl;
            }

            var valueList = new List<string>();
            var content = await contextInfo.GetContentAsync();

            if (content != null)
            {
                if (!string.IsNullOrEmpty(content.Get<string>(type)))
                {
                    valueList.Add(content.Get<string>(type));
                }

                var extendAttributeName = ContentAttribute.GetExtendAttributeName(type);
                var extendValues = content.Get<string>(extendAttributeName);
                if (!string.IsNullOrEmpty(extendValues))
                {
                    foreach (var extendValue in Utilities.GetStringList(extendValues))
                    {
                        valueList.Add(extendValue);
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
            }

            if (valueList.Count == 0) return string.Empty;

            var eachList = new List<KeyValuePair<int, object>>();
            var index = 0;
            foreach (string value in valueList)
            {
                eachList.Add(new KeyValuePair<int, object>(index++, value));
            }

            var builder = new StringBuilder();
            if (listInfo.Layout == Layout.None)
            {
                if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
                {
                    builder.Append(listInfo.HeaderTemplate);
                }

                var isAlternative = false;
                var isSeparator = false;
                if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
                {
                    isAlternative = true;
                }
                if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
                {
                    isSeparator = true;
                }

                for (var i = 0; i < eachList.Count; i++)
                {
                    if (isSeparator && i % 2 != 0 && i != eachList.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorTemplate);
                    }

                    var each = eachList[i];

                    pageInfo.EachItems.Push(each);
                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                    builder.Append(await TemplateUtility.GetEachsTemplateStringAsync(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, pageInfo, ContextType.Each, contextInfo));
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
                        if (itemIndex < eachList.Count)
                        {
                            var each = eachList[itemIndex];

                            pageInfo.EachItems.Push(each);
                            var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                            cellHtml = await TemplateUtility.GetEachsTemplateStringAsync(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, pageInfo, ContextType.Each, contextInfo);
                        }
                        tr.AddCell(cellHtml, cellAttributes);
                        itemIndex++;
                    }
                    if (itemIndex >= eachList.Count) break;
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
