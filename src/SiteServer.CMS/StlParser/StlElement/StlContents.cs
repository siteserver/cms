using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using SiteServer.Abstractions;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser.Mock;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "内容列表", Description = "通过 stl:contents 标签在模板中显示内容列表")]
    public class StlContents : StlListBase
    {
        public const string ElementName = "stl:contents";

        [StlAttribute(Title = "显示相关内容列表")]
        public const string IsRelatedContents = nameof(IsRelatedContents);

        public static async Task<object> ParseAsync(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var listInfo = await ListInfo.GetListInfoAsync(pageInfo, contextInfo, ContextType.Content);
            var dataSource = await GetContentsDataSourceAsync(pageInfo, contextInfo, listInfo);

            if (contextInfo.IsStlEntity)
            {
                return ParseEntity(dataSource);
            }

            return await ParseElementAsync(pageInfo, contextInfo, listInfo, dataSource);
        }

        protected static async Task<string> ParseElementAsync(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo, List<KeyValuePair<int, Content>> dataSource)
        {
            if (dataSource == null || dataSource.Count == 0) return string.Empty;

            var builder = new StringBuilder();
            if (listInfo.Layout == Model.Layout.None)
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

                for (var i = 0; i < dataSource.Count; i++)
                {
                    if (isSeparator && i % 2 != 0 && i != dataSource.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorTemplate);
                    }

                    var content = dataSource[i];

                    pageInfo.ContentItems.Push(content);
                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                    builder.Append(await TemplateUtility.GetContentsItemTemplateStringAsync(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, pageInfo, ContextType.Content, contextInfo));
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
                            var content = dataSource[itemIndex];

                            pageInfo.ContentItems.Push(content);

                            var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                            cellHtml = await TemplateUtility.GetContentsItemTemplateStringAsync(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, pageInfo, ContextType.Content, contextInfo);
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

        private static object ParseEntity(List<KeyValuePair<int, Content>> dataSource)
        {
            var contentInfoList = new List<IDictionary<string, object>>();

            foreach (var row in dataSource)
            {
                if (row.Value != null)
                {
                    contentInfoList.Add(row.Value.ToDictionary());
                }
            }

            return contentInfoList;
        }

        protected static async Task<List<KeyValuePair<int, Content>>> GetContentsDataSourceAsync(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var channelId = await StlDataUtility.GetChannelIdByLevelAsync(pageInfo.SiteId, contextInfo.ChannelId, listInfo.UpLevel, listInfo.TopLevel);

            channelId = await DataProvider.ChannelRepository.GetChannelIdAsync(pageInfo.SiteId, channelId, listInfo.ChannelIndex, listInfo.ChannelName);
            var taxisType = GetTaxisType(listInfo.Order);

            return await DataProvider.ContentRepository.ParserGetContentsDataSourceAsync(pageInfo.Site, channelId, contextInfo.ContentId, listInfo.GroupContent, listInfo.GroupContentNot, listInfo.Tags, listInfo.IsImageExists, listInfo.IsImage, listInfo.IsVideoExists, listInfo.IsVideo, listInfo.IsFileExists, listInfo.IsFile, listInfo.IsRelatedContents, listInfo.StartNum, listInfo.TotalNum, taxisType, listInfo.IsTopExists, listInfo.IsTop, listInfo.IsRecommendExists, listInfo.IsRecommend, listInfo.IsHotExists, listInfo.IsHot, listInfo.IsColorExists, listInfo.IsColor, listInfo.Scope, listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.Others);
        }

        public static TaxisType GetTaxisType(string order)
        {
            var taxisType = TaxisType.OrderByTaxisDesc;

            if (!string.IsNullOrEmpty(order))
            {
                if (StringUtils.EqualsIgnoreCase(order, StlParserUtility.OrderDefault))
                {
                    taxisType = TaxisType.OrderByTaxisDesc;
                }
                else if (StringUtils.EqualsIgnoreCase(order, StlParserUtility.OrderBack))
                {
                    taxisType = TaxisType.OrderByTaxis;
                }
                else if (StringUtils.EqualsIgnoreCase(order, StlParserUtility.OrderAddDate))
                {
                    taxisType = TaxisType.OrderByAddDateDesc;
                }
                else if (StringUtils.EqualsIgnoreCase(order, StlParserUtility.OrderAddDateBack))
                {
                    taxisType = TaxisType.OrderByAddDate;
                }
                else if (StringUtils.EqualsIgnoreCase(order, StlParserUtility.OrderLastEditDate))
                {
                    taxisType = TaxisType.OrderByLastEditDateDesc;
                }
                else if (StringUtils.EqualsIgnoreCase(order, StlParserUtility.OrderLastEditDateBack))
                {
                    taxisType = TaxisType.OrderByLastEditDate;
                }
                else if (StringUtils.EqualsIgnoreCase(order, StlParserUtility.OrderHits))
                {
                    taxisType = TaxisType.OrderByHits;
                }
                else if (StringUtils.EqualsIgnoreCase(order, StlParserUtility.OrderHitsByDay))
                {
                    taxisType = TaxisType.OrderByHitsByDay;
                }
                else if (StringUtils.EqualsIgnoreCase(order, StlParserUtility.OrderHitsByWeek))
                {
                    taxisType = TaxisType.OrderByHitsByWeek;
                }
                else if (StringUtils.EqualsIgnoreCase(order, StlParserUtility.OrderHitsByMonth))
                {
                    taxisType = TaxisType.OrderByHitsByMonth;
                }
                else if (StringUtils.EqualsIgnoreCase(order, StlParserUtility.OrderRandom))
                {
                    taxisType = TaxisType.OrderByRandom;
                }
            }

            return taxisType;
        }
    }
}
