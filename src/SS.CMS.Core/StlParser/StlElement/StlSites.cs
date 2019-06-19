using System.Collections.Generic;
using System.Text;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Models;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "站点列表", Description = "通过 stl:sites 标签在模板中显示站点列表")]
    public class StlSites
    {
        public const string ElementName = "stl:sites";

        [StlAttribute(Title = "站点名称")]
        private const string SiteName = nameof(SiteName);

        [StlAttribute(Title = "站点文件夹")]
        private const string SiteDir = nameof(SiteDir);

        public static object Parse(ParseContext parseContext)
        {
            var context = parseContext.Clone(EContextType.Site);
            var listInfo = ListInfo.GetListInfo(context);
            var siteName = listInfo.Others.Get(SiteName);
            var siteDir = listInfo.Others.Get(SiteDir);

            // var dataSource = StlDataUtility.GetSitesDataSource(siteName, siteDir, listInfo.StartNum, listInfo.TotalNum, listInfo.Where, listInfo.Scope, listInfo.OrderByString);

            var siteList = parseContext.GetContainerSiteList(siteName, siteDir, listInfo.StartNum, listInfo.TotalNum, listInfo.Scope, listInfo.Order);

            if (context.IsStlEntity)
            {
                return ParseEntity(context, siteList);
            }

            return ParseElement(context, listInfo, siteList);
        }

        private static string ParseElement(ParseContext context, ListInfo listInfo, List<KeyValuePair<int, SiteInfo>> siteList)
        {
            if (siteList == null || siteList.Count == 0) return string.Empty;

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

                for (var i = 0; i < siteList.Count; i++)
                {
                    if (isSeparator && i % 2 != 0 && i != siteList.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorTemplate);
                    }

                    var site = siteList[i];

                    context.PageInfo.SiteItems.Push(site);
                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                    builder.Append(TemplateUtility.GetSitesTemplateString(templateString, string.Empty, context));
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
                                if (itemIndex < siteList.Count)
                                {
                                    var site = siteList[itemIndex];

                                    context.PageInfo.SiteItems.Push(site);
                                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                                    cellHtml = TemplateUtility.GetSitesTemplateString(templateString, string.Empty, context);
                                }
                                tr.AddCell(cellHtml, cellAttributes);
                                itemIndex++;
                            }
                            if (itemIndex >= siteList.Count) break;
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

            // var parsedContent = string.Empty;

            // if (listInfo.Layout == ELayout.None)
            // {
            //     var rptContents = new Repeater
            //     {
            //         ItemTemplate =
            //             new RepeaterTemplate(listInfo.ItemTemplate, listInfo.SelectedItems,
            //                 listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat,
            //                 pageInfo, EContextType.Site, contextInfo)
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
            //         rptContents.AlternatingItemTemplate = new RepeaterTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Site, contextInfo);
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

            //     pdlContents.ItemTemplate = new DataListTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Site, contextInfo);
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
            //         pdlContents.AlternatingItemTemplate = new DataListTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Site, contextInfo);
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

        private static List<SiteInfo> ParseEntity(ParseContext context, List<KeyValuePair<int, SiteInfo>> siteList)
        {
            var siteInfoList = new List<SiteInfo>();

            foreach (var site in siteList)
            {
                var siteInfo = context.SiteRepository.GetSiteInfo(site.Value.Id);
                if (siteInfo != null)
                {
                    siteInfoList.Add(siteInfo);
                }
            }

            return siteInfoList;
        }
    }
}
