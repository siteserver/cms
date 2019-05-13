using System.Collections.Generic;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Template;
using System.Text;

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
            {BackgroundContentAttribute.ImageUrl, "遍历内容的图片字段"},
            {BackgroundContentAttribute.VideoUrl, "遍历内容的视频字段"},
            {BackgroundContentAttribute.FileUrl, "遍历内容的附件字段"}
        };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var listInfo = ListInfo.GetListInfo(pageInfo, contextInfo, EContextType.Content);

            return ParseImpl(pageInfo, contextInfo, listInfo);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var parsedContent = string.Empty;

            var type = listInfo.Others.Get(Type);
            if (string.IsNullOrEmpty(type))
            {
                type = BackgroundContentAttribute.ImageUrl;
            }

            var contentInfo = contextInfo.ContentInfo;
            var valueList = new List<string>();

            if (contentInfo != null)
            {
                if (!string.IsNullOrEmpty(contentInfo.GetString(type)))
                {
                    valueList.Add(contentInfo.GetString(type));
                }

                var extendAttributeName = ContentAttribute.GetExtendAttributeName(type);
                var extendValues = contentInfo.GetString(extendAttributeName);
                if (!string.IsNullOrEmpty(extendValues))
                {
                    foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
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

            if (valueList == null || valueList.Count == 0) return string.Empty;

            var eachList = new List<Container.Each>();
            var index = 0;
            foreach (string value in valueList)
            {
                eachList.Add(new Container.Each
                {
                    ItemIndex = index++,
                    Value = value
                });
            }

            var builder = new StringBuilder();

            if (listInfo.Layout == ELayout.None)
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
                    builder.Append(TemplateUtility.GetEachsTemplateString(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, pageInfo, EContextType.Each, contextInfo));
                }

                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
                {
                    builder.Append(listInfo.FooterTemplate);
                }
            }
            else
            {
                var isAlternative = false;
                if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
                {
                    isAlternative = true;
                }

                var tableAttributes = listInfo.GetTableAttributes();
                var cellAttributes = listInfo.GetCellAttributes();

                using (Html.Table table = new Html.Table(builder, tableAttributes))
                {
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
                        using (var tr = table.AddRow(null))
                        {
                            for (var cell = 1; cell <= columns; cell++)
                            {
                                var cellHtml = string.Empty;
                                if (itemIndex < eachList.Count)
                                {
                                    var each = eachList[itemIndex];

                                    pageInfo.EachItems.Push(each);
                                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                                    cellHtml = TemplateUtility.GetEachsTemplateString(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, pageInfo, EContextType.Each, contextInfo);
                                }
                                tr.AddCell(cellHtml, cellAttributes);
                                itemIndex++;
                            }
                            if (itemIndex >= eachList.Count) break;
                        }
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
            }

            return builder.ToString();

            // if (listInfo.Layout == ELayout.None)
            // {
            //     var rptContents = new Repeater
            //     {
            //         ItemTemplate =
            //             new RepeaterTemplate(listInfo.ItemTemplate, listInfo.SelectedItems,
            //                 listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat,
            //                 pageInfo, contextType, contextInfo)
            //     };

            //     if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
            //     {
            //         rptContents.HeaderTemplate = new SeparatorTemplate(listInfo.HeaderTemplate);
            //     }
            //     if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
            //     {
            //         rptContents.FooterTemplate = new SeparatorTemplate(listInfo.FooterTemplate);
            //     }
            //     if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
            //     {
            //         rptContents.SeparatorTemplate = new SeparatorTemplate(listInfo.SeparatorTemplate);
            //     }
            //     if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
            //     {
            //         rptContents.AlternatingItemTemplate = new RepeaterTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, contextType, contextInfo);
            //     }

            //     rptContents.DataSource = dataSource;

            //     rptContents.DataBind();

            //     if (rptContents.Items.Count > 0)
            //     {
            //         parsedContent = ControlUtils.GetControlRenderHtml(rptContents);
            //     }
            // }
            // else
            // {
            //     var pdlContents = new ParsedDataList();

            //     TemplateUtility.PutListInfoToMyDataList(pdlContents, listInfo);

            //     pdlContents.ItemTemplate = new DataListTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, contextType, contextInfo);
            //     if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
            //     {
            //         pdlContents.HeaderTemplate = new SeparatorTemplate(listInfo.HeaderTemplate);
            //     }
            //     if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
            //     {
            //         pdlContents.FooterTemplate = new SeparatorTemplate(listInfo.FooterTemplate);
            //     }
            //     if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
            //     {
            //         pdlContents.SeparatorTemplate = new SeparatorTemplate(listInfo.SeparatorTemplate);
            //     }
            //     if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
            //     {
            //         pdlContents.AlternatingItemTemplate = new DataListTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, contextType, contextInfo);
            //     }

            //     pdlContents.DataSource = dataSource;

            //     pdlContents.DataBind();

            //     if (pdlContents.Items.Count > 0)
            //     {
            //         parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
            //     }
            // }

            // return parsedContent;
        }
    }
}
