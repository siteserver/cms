using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Mock;
using SSCMS.Core.StlParser.Model;
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

            return await ParseElementAsync(parseManager, listInfo, dataSource);
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
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderHits.ToLower()))
                {
                    taxisType = TaxisType.OrderByHits;
                }
                else if (orderValue.ToLower().Equals(StlParserUtility.OrderRandom.ToLower()))
                {
                    taxisType = TaxisType.OrderByRandom;
                }
            }

            return taxisType;
        }

        protected static async Task<string> ParseElementAsync(IParseManager parseManager, ListInfo listInfo, List<KeyValuePair<int, Channel>> channels)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            if (channels == null || channels.Count == 0) return string.Empty;

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

                for (var i = 0; i < channels.Count; i++)
                {
                    if (isSeparator && i % 2 != 0 && i != channels.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorTemplate);
                    }

                    var channel = channels[i];

                    pageInfo.ChannelItems.Push(channel);
                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                    var parsedString = await TemplateUtility.GetChannelsItemTemplateStringAsync(templateString,
                        listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, parseManager, ParseType.Channel);
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
                    using var tr = table.AddRow();
                    for (var cell = 1; cell <= columns; cell++)
                    {
                        var cellHtml = string.Empty;
                        if (itemIndex < channels.Count)
                        {
                            var channel = channels[itemIndex];

                            pageInfo.ChannelItems.Push(channel);
                            var templateString = isAlternative
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

        private static object ParseEntity(List<KeyValuePair<int, Channel>> channels)
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


//using System;
//using System.Collections.Generic;
//using System.Data;
//using SSCMS;
//using SS.CMS.StlParser.Model;
//using SS.CMS.StlParser.Utility;
//using System.Threading.Tasks;
//using SS.CMS;
//using SS.CMS.Repositories;

//namespace SS.CMS.StlParser.StlElement
//{
//    [StlElement(Title = "栏目列表", Description = "通过 stl:channels 标签在模板中显示栏目列表")]
//    public class StlChannels : StlListBase
//    {
//        public const string ElementName = "stl:channels";

//        [StlAttribute(Title = "从所有栏目中选择")]
//        public const string IsTotal = nameof(IsTotal);

//        [StlAttribute(Title = "显示所有级别的子栏目")]
//        public const string IsAllChildren = nameof(IsAllChildren);

//        public static async Task<object> ParseAsync(PageInfo pageInfo, ContextInfo contextInfo)
//        {
//            var listInfo = await ListInfo.GetListInfoAsync(pageInfo, contextInfo, ContextType.Channel);

//            var dataSource = await GetDataSourceAsync(pageInfo, contextInfo, listInfo);

//            if (contextInfo.IsStlEntity)
//            {
//                return await ParseEntityAsync(dataSource);
//            }

//            return ParseElement(pageInfo, contextInfo, listInfo, dataSource);
//        }

//        public static async Task<DataSet> GetDataSourceAsync(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
//        {
//            var channelId = await StlDataUtility.GetChannelIdByLevelAsync(pageInfo.SiteId, contextInfo.ChannelId, listInfo.UpLevel, listInfo.TopLevel);

//            channelId = await StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(pageInfo.SiteId, channelId, listInfo.ChannelIndex, listInfo.ChannelName);

//            var isTotal = TranslateUtils.ToBool(listInfo.Others.Get(IsTotal));

//            if (TranslateUtils.ToBool(listInfo.Others.Get(IsAllChildren)))
//            {
//                listInfo.Scope = ScopeType.Descendant;
//            }

//            return await StlDataUtility.GetChannelsDataSourceAsync(pageInfo.SiteId, channelId, listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.IsImageExists, listInfo.IsImage, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString, listInfo.Scope, isTotal, listInfo.Where);
//        }

//        private static string ParseElement(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo, DataSet dataSource)
//        {
//            var parsedContent = string.Empty;

//            if (listInfo.Layout == Model.Layout.None)
//            {
//                var rptContents = new Repeater();

//                if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
//                {
//                    rptContents.HeaderTemplate = new SeparatorTemplate(listInfo.HeaderTemplate);
//                }
//                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
//                {
//                    rptContents.FooterTemplate = new SeparatorTemplate(listInfo.FooterTemplate);
//                }
//                if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
//                {
//                    rptContents.SeparatorTemplate = new SeparatorTemplate(listInfo.SeparatorTemplate);
//                }
//                if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
//                {
//                    rptContents.AlternatingItemTemplate = new RepeaterTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, ContextType.Channel, contextInfo);
//                }

//                rptContents.ItemTemplate = new RepeaterTemplate(listInfo.ItemTemplate, listInfo.SelectedItems,
//                    listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat,
//                    pageInfo, ContextType.Channel, contextInfo);

//                rptContents.DataSource = dataSource;
//                rptContents.DataBind();

//                if (rptContents.Items.Count > 0)
//                {
//                    parsedContent = ControlUtils.GetControlRenderHtml(rptContents);
//                }
//            }
//            else
//            {
//                var pdlContents = new ParsedDataList();

//                //设置显示属性
//                TemplateUtility.PutListInfoToMyDataList(pdlContents, listInfo);

//                //设置列表模板
//                pdlContents.ItemTemplate = new DataListTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, ContextType.Channel, contextInfo);
//                if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
//                {
//                    pdlContents.HeaderTemplate = new SeparatorTemplate(listInfo.HeaderTemplate);
//                }
//                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
//                {
//                    pdlContents.FooterTemplate = new SeparatorTemplate(listInfo.FooterTemplate);
//                }
//                if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
//                {
//                    pdlContents.SeparatorTemplate = new SeparatorTemplate(listInfo.SeparatorTemplate);
//                }
//                if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
//                {
//                    pdlContents.AlternatingItemTemplate = new DataListTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, ContextType.Channel, contextInfo);
//                }

//                pdlContents.DataSource = dataSource;
//                pdlContents.DataKeyField = nameof(Channel.Id);
//                pdlContents.DataBind();

//                if (pdlContents.Items.Count > 0)
//                {
//                    parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
//                }
//            }

//            return parsedContent;
//        }

//        private static async Task<object> ParseEntityAsync(DataSet dataSource)
//        {
//            var channelInfoList = new List<IDictionary<string,object>>();
//            var table = dataSource.Tables[0];
//            foreach (DataRow row in table.Rows)
//            {
//                var channelId = Convert.ToInt32(row[nameof(ContentAttribute.Id)]);

//                var channelInfo = await parseManager.DatabaseManager.ChannelRepository.GetAsync(channelId);
//                if (channelInfo != null)
//                {
//                    channelInfoList.Add(channelInfo.ToDictionary());
//                }
//            }

//            return channelInfoList;
//        }
//    }
//}
