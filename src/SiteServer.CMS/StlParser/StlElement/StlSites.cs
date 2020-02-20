using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteServer.Abstractions;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using System.Threading.Tasks;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser.Mock;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "站点列表", Description = "通过 stl:sites 标签在模板中显示站点列表")]
    public class StlSites
    {
        public const string ElementName = "stl:sites";

        [StlAttribute(Title = "站点名称")]
        private const string SiteName = nameof(SiteName);

        [StlAttribute(Title = "站点文件夹")]
        private const string SiteDir = nameof(SiteDir);

        public static async Task<object> ParseAsync(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var listInfo = await ListInfo.GetListInfoAsync(pageInfo, contextInfo, ContextType.Site);
            var siteName = listInfo.Others.Get(SiteName);
            var siteDir = listInfo.Others.Get(SiteDir);

            var taxisType = GetTaxisTypeByOrder(listInfo.Order);
            var dataSource = await DataProvider.SiteRepository.ParserGetSitesAsync(siteName, siteDir, listInfo.StartNum, listInfo.TotalNum, listInfo.Scope, taxisType);

            if (contextInfo.IsStlEntity)
            {
                return ParseEntity(dataSource);
            }

            return await ParseElementAsync(pageInfo, contextInfo, listInfo, dataSource);
        }

        private static TaxisType GetTaxisTypeByOrder(string orderValue)
        {
            var taxisType = TaxisType.OrderByTaxis;
            if (!string.IsNullOrEmpty(orderValue))
            {
                if (orderValue.ToLower().Equals(StlParserUtility.OrderDefault.ToLower()))
                {
                    taxisType = TaxisType.OrderByTaxis;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderBack.ToLower()))
                {
                    taxisType = TaxisType.OrderByTaxisDesc;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderAddDate.ToLower()))
                {
                    taxisType = TaxisType.OrderByAddDateDesc;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderAddDateBack.ToLower()))
                {
                    taxisType = TaxisType.OrderByAddDate;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderRandom.ToLower()))
                {
                    taxisType = TaxisType.OrderByRandom;
                }
            }

            return taxisType;
        }

        private static async Task<string> ParseElementAsync(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo, List<KeyValuePair<int, Site>> sites)
        {
            if (sites == null || sites.Count == 0) return string.Empty;

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

                for (var i = 0; i < sites.Count; i++)
                {
                    if (isSeparator && i % 2 != 0 && i != sites.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorTemplate);
                    }

                    var site = sites[i];

                    pageInfo.SiteItems.Push(site);
                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                    var parsedString = await TemplateUtility.GetSitesTemplateStringAsync(templateString, string.Empty, pageInfo, ContextType.Site, contextInfo);
                    builder.Append(parsedString);
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
                    using var tr = table.AddRow(null);
                    for (var cell = 1; cell <= columns; cell++)
                    {
                        var cellHtml = string.Empty;
                        if (itemIndex < sites.Count)
                        {
                            var site = sites[itemIndex];

                            pageInfo.SiteItems.Push(site);
                            var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                            cellHtml = await TemplateUtility.GetSitesTemplateStringAsync(templateString, string.Empty, pageInfo, ContextType.Site, contextInfo);
                        }
                        tr.AddCell(cellHtml, cellAttributes);
                        itemIndex++;
                    }
                    if (itemIndex >= sites.Count) break;
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

        private static List<Site> ParseEntity(List<KeyValuePair<int, Site>> dataSource)
        {
            return dataSource.Select(x => x.Value).ToList();
        }
    }
}
