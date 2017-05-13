using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "站点列表", Description = "通过 stl:sites 标签在模板中显示站点列表")]
    public class StlSites
    {
        public const string ElementName = "stl:sites";

        public const string AttributeSiteName = "siteName";
        public const string AttributeSiteDir = "siteDir";
        public const string AttributeTotalNum = "totalNum";
        public const string AttributeStartNum = "startNum";
        public const string AttributeWhere = "where";
        public const string AttributeScope = "scope";
        public const string AttributeOrder = "order";
        public const string AttributeSince = "since";
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
            {AttributeSiteName, "站点名称"},
            {AttributeSiteDir, "站点文件夹"},
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
            {AttributeItemClass, "项Css类"},
            {AttributeTotalNum, "显示内容数目"},
            {AttributeStartNum, "从第几条信息开始显示"},
            {AttributeWhere, "获取站点列表的条件判断"},
            {AttributeScope, "范围"},
            {AttributeOrder, "排序"},
            {AttributeSince, "时间段"},
            {AttributeIsDynamic, "是否动态显示"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var listInfo = ListInfo.GetListInfoByXmlNode(node, pageInfo, contextInfo, EContextType.Site);

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

            contextInfo.TitleWordNum = 0;

            var siteName = listInfo.Others.Get(AttributeSiteName);
            var siteDir = listInfo.Others.Get(AttributeSiteDir);
            var since = listInfo.Others.Get(AttributeSince);

            if (listInfo.Layout == ELayout.None)
            {
                var rptContents = new Repeater
                {
                    ItemTemplate =
                        new RepeaterTemplate(listInfo.ItemTemplate, listInfo.SelectedItems,
                            listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat,
                            pageInfo, EContextType.Site, contextInfo)
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
                    rptContents.AlternatingItemTemplate = new RepeaterTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Site, contextInfo);
                }

                rptContents.DataSource = StlDataUtility.GetSitesDataSource(siteName, siteDir, listInfo.StartNum, listInfo.TotalNum, listInfo.Where, listInfo.Scope, listInfo.OrderByString, since);
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

                pdlContents.ItemTemplate = new DataListTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Site, contextInfo);
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
                    pdlContents.AlternatingItemTemplate = new DataListTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Site, contextInfo);
                }

                pdlContents.DataSource = StlDataUtility.GetSitesDataSource(siteName, siteDir, listInfo.StartNum, listInfo.TotalNum, listInfo.Where, listInfo.Scope, listInfo.OrderByString, since);
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
