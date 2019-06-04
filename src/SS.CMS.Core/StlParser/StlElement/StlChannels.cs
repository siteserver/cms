using System.Collections.Generic;
using System.Text;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Stl;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Template;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "栏目列表", Description = "通过 stl:channels 标签在模板中显示栏目列表")]
    public class StlChannels : StlListBase
    {
        public const string ElementName = "stl:channels";

        [StlAttribute(Title = "从所有栏目中选择")]
        public const string IsTotal = nameof(IsTotal);

        [StlAttribute(Title = "显示所有级别的子栏目")]
        public const string IsAllChildren = nameof(IsAllChildren);

        public static object Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var listInfo = ListInfo.GetListInfo(pageInfo, contextInfo, EContextType.Channel);

            // var dataSource = GetDataSource(pageInfo, contextInfo, listInfo);
            var channelList = GetContainerChannelList(pageInfo, contextInfo, listInfo);

            if (contextInfo.IsStlEntity)
            {
                return ParseEntity(pageInfo, channelList);
            }

            return ParseElement(pageInfo, contextInfo, listInfo, channelList);
        }

        // public static DataSet GetDataSource(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        // {
        //     var channelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, listInfo.UpLevel, listInfo.TopLevel);

        //     channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, channelId, listInfo.ChannelIndex, listInfo.ChannelName);

        //     var isTotal = TranslateUtils.ToBool(listInfo.Others.Get(IsTotal));

        //     if (TranslateUtils.ToBool(listInfo.Others.Get(IsAllChildren)))
        //     {
        //         listInfo.Scope = EScopeType.Descendant;
        //     }

        //     return StlDataUtility.GetChannelsDataSource(pageInfo.SiteId, channelId, listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.IsImageExists, listInfo.IsImage, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString, listInfo.Scope, isTotal, listInfo.Where);
        // }

        public static IList<Container.Channel> GetContainerChannelList(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var channelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, listInfo.UpLevel, listInfo.TopLevel);

            channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, channelId, listInfo.ChannelIndex, listInfo.ChannelName);

            var isTotal = TranslateUtils.ToBool(listInfo.Others.Get(IsTotal));

            if (TranslateUtils.ToBool(listInfo.Others.Get(IsAllChildren)))
            {
                listInfo.Scope = EScopeType.Descendant;
            }

            var taxisType = StlDataUtility.GetChannelTaxisType(listInfo.Order, ETaxisType.OrderByTaxis);

            return StlChannelCache.GetContainerChannelList(pageInfo.SiteId, channelId, listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.IsImageExists, listInfo.IsImage, listInfo.StartNum, listInfo.TotalNum, taxisType, listInfo.Scope, isTotal);
        }

        public static string ParseElement(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo, IList<Container.Channel> channelList)
        {
            if (channelList == null || channelList.Count == 0) return string.Empty;

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

                for (var i = 0; i < channelList.Count; i++)
                {
                    if (isSeparator && i % 2 != 0 && i != channelList.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorTemplate);
                    }

                    var channel = channelList[i];

                    pageInfo.ChannelItems.Push(channel);
                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                    builder.Append(TemplateUtility.GetChannelsItemTemplateString(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, pageInfo, EContextType.Channel, contextInfo));
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
                                if (itemIndex < channelList.Count)
                                {
                                    var channel = channelList[itemIndex];

                                    pageInfo.ChannelItems.Push(channel);
                                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                                    cellHtml = TemplateUtility.GetChannelsItemTemplateString(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, pageInfo, EContextType.Channel, contextInfo);
                                }
                                tr.AddCell(cellHtml, cellAttributes);
                                itemIndex++;
                            }
                            if (itemIndex >= channelList.Count) break;
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
        }

        private static object ParseEntity(PageInfo pageInfo, IList<Container.Channel> channelList)
        {
            // var table = dataSource.Tables[0];
            // foreach (DataRow row in table.Rows)
            // {
            //     var channelId = Convert.ToInt32(row[nameof(ContentAttribute.Id)]);

            //     var channelInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, channelId);
            //     if (channelInfo != null)
            //     {
            //         channelInfoList.Add(channelInfo.ToDictionary());
            //     }
            // }

            var channelInfoList = new List<IDictionary<string, object>>();
            foreach (var channel in channelList)
            {
                var channelInfo = ChannelManager.GetChannelInfo(channel.SiteId, channel.Id);
                if (channelInfo != null)
                {
                    channelInfoList.Add(channelInfo.ToDictionary());
                }
            }

            return channelInfoList;
        }
    }
}
