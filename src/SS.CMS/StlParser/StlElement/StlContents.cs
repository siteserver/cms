using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Parse;
using SS.CMS.Core;
using SS.CMS.StlParser.Mock;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.Utility;

namespace SS.CMS.StlParser.StlElement
{
    [StlElement(Title = "内容列表", Description = "通过 stl:contents 标签在模板中显示内容列表")]
    public class StlContents : StlListBase
    {
        public const string ElementName = "stl:contents";

        [StlAttribute(Title = "显示相关内容列表")]
        public const string IsRelatedContents = nameof(IsRelatedContents);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var listInfo = await ListInfo.GetListInfoAsync(parseManager, ParseType.Content);
            var dataSource = await GetContentsDataSourceAsync(parseManager, listInfo);

            if (parseManager.ContextInfo.IsStlEntity)
            {
                return ParseEntity(dataSource);
            }

            return await ParseElementAsync(parseManager, listInfo, dataSource);
        }

        protected static async Task<string> ParseElementAsync(IParseManager parseManager, ListInfo listInfo, List<KeyValuePair<int, Content>> dataSource)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

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
                    builder.Append(await TemplateUtility.GetContentsItemTemplateStringAsync(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, parseManager, ParseType.Content));
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
                            cellHtml = await TemplateUtility.GetContentsItemTemplateStringAsync(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, parseManager, ParseType.Content);
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

        protected static async Task<List<KeyValuePair<int, Content>>> GetContentsDataSourceAsync(IParseManager parseManager, ListInfo listInfo)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var dataManager = new StlDataManager(parseManager.DatabaseManager);
            var channelId = await dataManager.GetChannelIdByLevelAsync(pageInfo.SiteId, contextInfo.ChannelId, listInfo.UpLevel, listInfo.TopLevel);

            channelId = await parseManager.DatabaseManager.ChannelRepository.GetChannelIdAsync(pageInfo.SiteId, channelId, listInfo.ChannelIndex, listInfo.ChannelName);
            var taxisType = GetTaxisType(listInfo.Order);

            return await parseManager.DatabaseManager.ContentRepository.ParserGetContentsDataSourceAsync(pageInfo.Site, channelId, contextInfo.ContentId, listInfo.GroupContent, listInfo.GroupContentNot, listInfo.Tags, listInfo.IsImageExists, listInfo.IsImage, listInfo.IsVideoExists, listInfo.IsVideo, listInfo.IsFileExists, listInfo.IsFile, listInfo.IsRelatedContents, listInfo.StartNum, listInfo.TotalNum, taxisType, listInfo.IsTopExists, listInfo.IsTop, listInfo.IsRecommendExists, listInfo.IsRecommend, listInfo.IsHotExists, listInfo.IsHot, listInfo.IsColorExists, listInfo.IsColor, listInfo.Scope, listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.Others);
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
