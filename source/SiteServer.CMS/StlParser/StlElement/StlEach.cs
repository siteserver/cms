using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "列表项循环", Description = "通过 stl:each 标签在模板中遍历指定的列表项")]
    public class StlEach
    {
        public const string ElementName = "stl:each";

        public const string AttributeType = "type";
        public const string AttributeTotalNum = "totalNum";
        public const string AttributeStartNum = "startNum";
        public const string AttributeOrder = "order";
        public const string AttributeIsDynamic = "isDynamic";
        public const string AttributeCellPadding = "cellPadding";
        public const string AttributeCellSpacing = "cellSpacing";
        public const string AttributeClass = "class";
        public const string AttributeColumns = "columns";
        public const string AttributeDirection = "direction";
        public const string AttributeHeight = "height";
        public const string AttributeWidth = "width";
        public const string AttributeAlign = "align";
        public const string AttributeItemHeight = "itemHeight";
        public const string AttributeItemWidth = "itemWidth";
        public const string AttributeItemAlign = "itemAlign";
        public const string AttributeItemVerticalAlign = "itemVerticalAlign";
        public const string AttributeItemClass = "itemClass";
        public const string AttributeLayout = "layout";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeType, StringUtils.SortedListToAttributeValueString("循环类型", TypeList)},
            {AttributeTotalNum, "显示信息数目"},
            {AttributeStartNum, "从第几条信息开始显示"},
            {AttributeOrder, "排序"},
            {AttributeIsDynamic, "是否动态显示"},
            {AttributeCellPadding, "填充"},
            {AttributeCellSpacing, "间距"},
            {AttributeClass, "Css类"},
            {AttributeColumns, "列数"},
            {AttributeDirection, "方向"},
            {AttributeLayout, "指定列表布局方式"},
            {AttributeHeight, "整体高度"},
            {AttributeWidth, "整体宽度"},
            {AttributeAlign, "整体对齐"},
            {AttributeItemHeight, "项高度"},
            {AttributeItemWidth, "项宽度"},
            {AttributeItemAlign, "项水平对齐"},
            {AttributeItemVerticalAlign, "项垂直对齐"},
            {AttributeItemClass, "项Css类"}
        };

        public const string TypePhoto = "Photo";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypePhoto, "遍历内容模型为图片的内容的图片列表"},
            {BackgroundContentAttribute.ImageUrl, "遍历内容的图片字段"},
            {BackgroundContentAttribute.VideoUrl, "遍历内容的视频字段"},
            {BackgroundContentAttribute.FileUrl, "遍历内容的附件字段"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var listInfo = ListInfo.GetListInfoByXmlNode(node, pageInfo, contextInfo, EContextType.Content);

                parsedContent = listInfo.IsDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, listInfo);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var parsedContent = string.Empty;

            var type = listInfo.Others.Get(AttributeType);
            if (string.IsNullOrEmpty(type))
            {
                type = BackgroundContentAttribute.ImageUrl;
            }

            var contextType = EContextType.Each;
            IEnumerable dataSource = null;
            if (StringUtils.EqualsIgnoreCase(type, TypePhoto))
            {
                contextType = EContextType.Photo;
                dataSource = StlDataUtility.GetPhotosDataSource(pageInfo.PublishmentSystemInfo, contextInfo.ContentId, listInfo.StartNum, listInfo.TotalNum);
            }
            else
            {
                var contentInfo = contextInfo.ContentInfo;
                if (contentInfo != null)
                {
                    var eachList = new List<string>();

                    if (!string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(type)))
                    {
                        eachList.Add(contentInfo.GetExtendedAttribute(type));
                    }

                    var extendAttributeName = ContentAttribute.GetExtendAttributeName(type);
                    var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                    if (!string.IsNullOrEmpty(extendValues))
                    {
                        foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                        {
                            eachList.Add(extendValue);
                        }
                    }

                    if (listInfo.StartNum > 1 || listInfo.TotalNum > 0)
                    {
                        if (listInfo.StartNum > 1)
                        {
                            var count = listInfo.StartNum - 1;
                            if (count > eachList.Count)
                            {
                                count = eachList.Count;
                            }
                            eachList.RemoveRange(0, count);
                        }

                        if (listInfo.TotalNum > 0)
                        {
                            if (listInfo.TotalNum < eachList.Count)
                            {
                                eachList.RemoveRange(listInfo.TotalNum, eachList.Count - listInfo.TotalNum);
                            }
                        }
                    }

                    dataSource = eachList;
                }
            }

            if (listInfo.Layout == ELayout.None)
            {
                var rptContents = new Repeater
                {
                    ItemTemplate =
                        new RepeaterTemplate(listInfo.ItemTemplate, listInfo.SelectedItems,
                            listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat,
                            pageInfo, contextType, contextInfo)
                };

                if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
                {
                    rptContents.HeaderTemplate = new SeparatorTemplate(listInfo.HeaderTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
                {
                    rptContents.FooterTemplate = new SeparatorTemplate(listInfo.FooterTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
                {
                    rptContents.SeparatorTemplate = new SeparatorTemplate(listInfo.SeparatorTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
                {
                    rptContents.AlternatingItemTemplate = new RepeaterTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, contextType, contextInfo);
                }

                rptContents.DataSource = dataSource;

                rptContents.DataBind();

                if (rptContents.Items.Count > 0)
                {
                    parsedContent = ControlUtils.GetControlRenderHtml(rptContents);
                }
            }
            else
            {
                var pdlContents = new ParsedDataList();

                TemplateUtility.PutListInfoToMyDataList(pdlContents, listInfo);

                pdlContents.ItemTemplate = new DataListTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, contextType, contextInfo);
                if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
                {
                    pdlContents.HeaderTemplate = new SeparatorTemplate(listInfo.HeaderTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
                {
                    pdlContents.FooterTemplate = new SeparatorTemplate(listInfo.FooterTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
                {
                    pdlContents.SeparatorTemplate = new SeparatorTemplate(listInfo.SeparatorTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
                {
                    pdlContents.AlternatingItemTemplate = new DataListTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, contextType, contextInfo);
                }

                pdlContents.DataSource = dataSource;

                pdlContents.DataBind();

                if (pdlContents.Items.Count > 0)
                {
                    parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
                }
            }

            return parsedContent;
        }
    }
}
