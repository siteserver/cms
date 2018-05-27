using System.Data;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "数据库列表", Description = "通过 stl:sqlContents 标签在模板中显示数据库列表")]
    public class StlSqlContents
    {
        public const string ElementName = "stl:sqlContents";

        public static readonly Attr ConnectionStringName = new Attr("connectionStringName", "数据库链接字符串名称");
        public static readonly Attr ConnectionString = new Attr("connectionString", "数据库链接字符串");
        public static readonly Attr QueryString = new Attr("queryString", "数据库查询语句");
        public static readonly Attr TotalNum = new Attr("totalNum", "填充");
        public static readonly Attr StartNum = new Attr("startNum", "间距");
        public static readonly Attr Order = new Attr("order", "Css类");
        public static readonly Attr CellPadding = new Attr("cellPadding", "列数");
        public static readonly Attr CellSpacing = new Attr("cellSpacing", "方向");
        public static readonly Attr Class = new Attr("class", "指定列表布局方式");
        public static readonly Attr Columns = new Attr("columns", "整体高度");
        public static readonly Attr Direction = new Attr("direction", "整体宽度");
        public static readonly Attr Height = new Attr("height", "整体对齐");
        public static readonly Attr Width = new Attr("width", "项高度");
        public static readonly Attr Align = new Attr("align", "项宽度");
        public static readonly Attr ItemHeight = new Attr("itemHeight", "项水平对齐");
        public static readonly Attr ItemWidth = new Attr("itemWidth", "项垂直对齐");
        public static readonly Attr ItemAlign = new Attr("itemAlign", "项Css类");
        public static readonly Attr ItemVerticalAlign = new Attr("itemVerticalAlign", "显示内容数目");
        public static readonly Attr ItemClass = new Attr("itemClass", "从第几条信息开始显示");
        public static readonly Attr Layout = new Attr("layout", "排序");

        public static object Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var listInfo = ListInfo.GetListInfoByXmlNode(pageInfo, contextInfo, EContextType.SqlContent);
            var dataSource = StlDataUtility.GetSqlContentsDataSource(listInfo.ConnectionString, listInfo.QueryString, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString);

            if (contextInfo.IsStlEntity)
            {
                return ParseEntity(pageInfo, dataSource);
            }

            return ParseElement(pageInfo, contextInfo, listInfo, dataSource);
        }

        private static string ParseElement(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo, DataSet dataSource)
        {
            var parsedContent = string.Empty;

            if (listInfo.Layout == ELayout.None)
            {
                var rptContents = new Repeater
                {
                    ItemTemplate =
                        new RepeaterTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues,
                            listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo,
                            EContextType.SqlContent, contextInfo)
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
                    rptContents.AlternatingItemTemplate = new RepeaterTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.SqlContent, contextInfo);
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

                pdlContents.ItemTemplate = new DataListTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.SqlContent, contextInfo);
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
                    pdlContents.AlternatingItemTemplate = new DataListTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.SqlContent, contextInfo);
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

        private static object ParseEntity(PageInfo pageInfo, DataSet dataSource)
        {
            var table = dataSource.Tables[0];
            return TranslateUtils.DataTableToDictionaryList(table);
        }
    }
}
