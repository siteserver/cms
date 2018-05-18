using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "站点列表", Description = "通过 stl:sites 标签在模板中显示站点列表")]
    public class StlSites
    {
        public const string ElementName = "stl:sites";

        private static readonly Attr SiteName = new Attr("siteName", "站点名称");
        private static readonly Attr SiteDir = new Attr("siteDir", "站点文件夹");
        private static readonly Attr TotalNum = new Attr("totalNum", "填充");
        private static readonly Attr StartNum = new Attr("startNum", "间距");
        private static readonly Attr Where = new Attr("where", "Css类");
        private static readonly Attr Scope = new Attr("scope", "列数");
        private static readonly Attr Order = new Attr("order", "方向");
        private static readonly Attr Since = new Attr("since", "指定列表布局方式");
        private static readonly Attr CellPadding = new Attr("cellPadding", "整体高度");
        private static readonly Attr CellSpacing = new Attr("cellSpacing", "整体宽度");
        private static readonly Attr Class = new Attr("class", "整体对齐");
        private static readonly Attr Columns = new Attr("columns", "项高度");
        private static readonly Attr Direction = new Attr("direction", "项宽度");
        private static readonly Attr Height = new Attr("height", "项水平对齐");
        private static readonly Attr Width = new Attr("width", "项垂直对齐");
        private static readonly Attr Align = new Attr("align", "项Css类");
        private static readonly Attr ItemHeight = new Attr("itemHeight", "显示内容数目");
        private static readonly Attr ItemWidth = new Attr("itemWidth", "从第几条信息开始显示");
        private static readonly Attr ItemAlign = new Attr("itemAlign", "获取站点列表的条件判断");
        private static readonly Attr ItemVerticalAlign = new Attr("itemVerticalAlign", "范围");
        private static readonly Attr ItemClass = new Attr("itemClass", "排序");
        private static readonly Attr Layout = new Attr("layout", "时间段");

        public static object Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var listInfo = ListInfo.GetListInfoByXmlNode(pageInfo, contextInfo, EContextType.Site);
            var siteName = listInfo.Others.Get(SiteName.Name);
            var siteDir = listInfo.Others.Get(SiteDir.Name);
            var since = listInfo.Others.Get(Since.Name);

            var dataSource = StlDataUtility.GetSitesDataSource(siteName, siteDir, listInfo.StartNum, listInfo.TotalNum, listInfo.Where, listInfo.Scope, listInfo.OrderByString, since);

            if (contextInfo.IsStlEntity)
            {
                return ParseEntity(dataSource);
            }

            return ParseElement(pageInfo, contextInfo, listInfo, dataSource);
        }

        private static string ParseElement(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo, IDataReader dataSource)
        {
            var parsedContent = string.Empty;

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

                pdlContents.DataSource = dataSource;
                pdlContents.DataBind();

                if (pdlContents.Items.Count > 0)
                {
                    parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
                }
            }

            return parsedContent;
        }

        private static List<SiteInfo> ParseEntity(IDataReader dataSource)
        {
            var siteInfoList = new List<SiteInfo>();

            while (dataSource.Read())
            {
                var siteId = dataSource.GetInt32(0);
                var siteInfo = SiteManager.GetSiteInfo(siteId);

                if (siteInfo != null)
                {
                    siteInfoList.Add(siteInfo);
                }
            }

            return siteInfoList;
        }
    }
}
