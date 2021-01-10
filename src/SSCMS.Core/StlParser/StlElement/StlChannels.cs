using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Core.StlParser.Enums;
using SSCMS.Core.StlParser.Mocks;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Models;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "栏目列表", Description = "通过 stl:channels 标签在模板中显示栏目列表")]
    public class StlChannels : StlListBase
    {
        public const string ElementName = "stl:channels";

        [StlAttribute(Title = "从所有栏目中选择")] private const string IsTotal = nameof(IsTotal);

        [StlAttribute(Title = "显示所有级别的子栏目")] private const string IsAllChildren = nameof(IsAllChildren);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var listInfo = await ListInfo.GetListInfoAsync(parseManager, ParseType.Channel);

            var dataSource = await GetChannelsDataSourceAsync(parseManager, listInfo);

            if (parseManager.ContextInfo.IsStlEntity)
            {
                return ParseEntity(dataSource);
            }

            return await ParseAsync(parseManager, listInfo, dataSource);
        }

        protected static async Task<string> ParseAsync(IParseManager parseManager, ListInfo listInfo, List<KeyValuePair<int, Channel>> channels)
        {
            var pageInfo = parseManager.PageInfo;

            if (channels == null || channels.Count == 0) return string.Empty;

            var builder = new StringBuilder();
            if (listInfo.Layout == ListLayout.None)
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

                for (var i = 0; i < channels.Count; i++)
                {
                    var channel = channels[i];

                    pageInfo.ChannelItems.Push(channel);
                    var templateString = isAlternative && i % 2 == 1 ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                    var parsedString = await TemplateUtility.GetChannelsItemTemplateStringAsync(templateString,
                        listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, parseManager, ParseType.Channel);
                    builder.Append(parsedString);

                    if (isSeparator && i != channels.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorTemplate);
                    }
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
                        if (itemIndex < channels.Count)
                        {
                            var channel = channels[itemIndex];

                            pageInfo.ChannelItems.Push(channel);
                            var templateString = isAlternative && itemIndex % 2 == 1
                                ? listInfo.AlternatingItemTemplate
                                : listInfo.ItemTemplate;
                            cellHtml = await TemplateUtility.GetChannelsItemTemplateStringAsync(templateString,
                                listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, parseManager,
                                ParseType.Channel);
                        }

                        tr.AddCell(cellHtml, cellAttributes);
                        itemIndex++;
                    }

                    if (itemIndex >= channels.Count) break;
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

        protected static async Task<List<KeyValuePair<int, Channel>>> GetChannelsDataSourceAsync(IParseManager parseManager, ListInfo listInfo)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var dataManager = new StlDataManager(parseManager.DatabaseManager);
            var channelId = await dataManager.GetChannelIdByLevelAsync(pageInfo.SiteId, contextInfo.ChannelId, listInfo.UpLevel, listInfo.TopLevel);

            channelId = await dataManager.GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(pageInfo.SiteId, channelId, listInfo.ChannelIndex, listInfo.ChannelName);

            var isTotal = TranslateUtils.ToBool(listInfo.Others.Get(nameof(IsTotal)));

            if (TranslateUtils.ToBool(listInfo.Others.Get(nameof(IsAllChildren))))
            {
                listInfo.Scope = ScopeType.Descendant;
            }

            var taxisType = GetChannelTaxisTypeByOrder(listInfo.Order);

            return await parseManager.DatabaseManager.ChannelRepository.ParserGetChannelsAsync(pageInfo.SiteId, channelId,
                listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.IsImageExists, listInfo.IsImage,
                listInfo.StartNum, listInfo.TotalNum, taxisType, listInfo.Scope, isTotal);
        }

        private static TaxisType GetChannelTaxisTypeByOrder(string orderValue)
        {
            var taxisType = TaxisType.OrderByTaxis;
            if (!string.IsNullOrEmpty(orderValue))
            {
                if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderDefault))
                {
                    taxisType = TaxisType.OrderByTaxis;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderBack))
                {
                    taxisType = TaxisType.OrderByTaxisDesc;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderAddDate))
                {
                    taxisType = TaxisType.OrderByAddDateDesc;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderAddDateBack))
                {
                    taxisType = TaxisType.OrderByAddDate;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderHits))
                {
                    taxisType = TaxisType.OrderByHits;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderRandom))
                {
                    taxisType = TaxisType.OrderByRandom;
                }
            }

            return taxisType;
        }

        private static object ParseEntity(IEnumerable<KeyValuePair<int, Channel>> channels)
        {
            var channelInfoList = new List<IDictionary<string, object>>();

            foreach (var channel in channels)
            {
                channelInfoList.Add(channel.Value.ToDictionary());
            }

            return channelInfoList;
        }
    }
}
