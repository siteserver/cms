using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "列表项循环", Description = "通过 stl:each 标签在模板中遍历指定的列表项")]
    public class StlEach
    {
        public const string ElementName = "stl:each";

        private static readonly Attr Type = new Attr("type", "循环类型");
        private static readonly Attr TotalNum = new Attr("totalNum", "显示信息数目");
        private static readonly Attr StartNum = new Attr("startNum", "从第几条信息开始显示");
        private static readonly Attr Order = new Attr("order", "排序");
        private static readonly Attr CellPadding = new Attr("cellPadding", "填充");
        private static readonly Attr CellSpacing = new Attr("cellSpacing", "间距");
        private static readonly Attr Class = new Attr("class", "Css类");
        private static readonly Attr Columns = new Attr("columns", "列数");
        private static readonly Attr Direction = new Attr("direction", "方向");
        private static readonly Attr Height = new Attr("height", "指定列表布局方式");
        private static readonly Attr Width = new Attr("width", "整体高度");
        private static readonly Attr Align = new Attr("align", "整体宽度");
        private static readonly Attr ItemHeight = new Attr("itemHeight", "整体对齐");
        private static readonly Attr ItemWidth = new Attr("itemWidth", "项高度");
        private static readonly Attr ItemAlign = new Attr("itemAlign", "项宽度");
        private static readonly Attr ItemVerticalAlign = new Attr("itemVerticalAlign", "项水平对齐");
        private static readonly Attr ItemClass = new Attr("itemClass", "项垂直对齐");
        private static readonly Attr Layout = new Attr("layout", "项Css类");

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {BackgroundContentAttribute.ImageUrl, "遍历内容的图片字段"},
            {BackgroundContentAttribute.VideoUrl, "遍历内容的视频字段"},
            {BackgroundContentAttribute.FileUrl, "遍历内容的附件字段"}
        };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var listInfo = ListInfo.GetListInfoByXmlNode(pageInfo, contextInfo, EContextType.Content);

            return ParseImpl(pageInfo, contextInfo, listInfo);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var parsedContent = string.Empty;

            var type = listInfo.Others.Get(Type.Name);
            if (string.IsNullOrEmpty(type))
            {
                type = BackgroundContentAttribute.ImageUrl;
            }

            var contextType = EContextType.Each;
            IEnumerable dataSource = null;
            var contentInfo = contextInfo.ContentInfo;
            if (contentInfo != null)
            {
                var eachList = new List<string>();

                if (!string.IsNullOrEmpty(contentInfo.GetString(type)))
                {
                    eachList.Add(contentInfo.GetString(type));
                }

                var extendAttributeName = ContentAttribute.GetExtendAttributeName(type);
                var extendValues = contentInfo.GetString(extendAttributeName);
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
