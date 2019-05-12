using System;
using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Utils.Enumerations;
using SiteServer.CMS.StlParser.Template;
using System.Text;

namespace SiteServer.CMS.StlParser.StlElement
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

            var dataSource = GetDataSource(pageInfo, contextInfo, listInfo);
            var channelIdList = GetChannelIdList(pageInfo, contextInfo, listInfo);

            if (contextInfo.IsStlEntity)
            {
                return ParseEntity(pageInfo, dataSource, channelIdList);
            }

            return ParseElement(pageInfo, contextInfo, listInfo, dataSource, channelIdList);
        }

        public static DataSet GetDataSource(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var channelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, listInfo.UpLevel, listInfo.TopLevel);

            channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, channelId, listInfo.ChannelIndex, listInfo.ChannelName);

            var isTotal = TranslateUtils.ToBool(listInfo.Others.Get(IsTotal));

            if (TranslateUtils.ToBool(listInfo.Others.Get(IsAllChildren)))
            {
                listInfo.Scope = EScopeType.Descendant;
            }

            return StlDataUtility.GetChannelsDataSource(pageInfo.SiteId, channelId, listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.IsImageExists, listInfo.IsImage, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString, listInfo.Scope, isTotal, listInfo.Where);
        }

        public static List<int> GetChannelIdList(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var channelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, listInfo.UpLevel, listInfo.TopLevel);

            channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, channelId, listInfo.ChannelIndex, listInfo.ChannelName);

            var isTotal = TranslateUtils.ToBool(listInfo.Others.Get(IsTotal));

            if (TranslateUtils.ToBool(listInfo.Others.Get(IsAllChildren)))
            {
                listInfo.Scope = EScopeType.Descendant;
            }

            return StlDataUtility.GetChannelsChannelIdList(pageInfo.SiteId, channelId, listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.IsImageExists, listInfo.IsImage, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString, listInfo.Scope, isTotal, listInfo.Where);
        }

        private static string ParseElement(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo, DataSet dataSource, List<int> channelIdList)
        {
            if (channelIdList == null || channelIdList.Count == 0) return string.Empty;

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

                for (var i = 0; i < channelIdList.Count; i++)
                {
                    if (isSeparator && i % 2 != 0 && i != channelIdList.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorTemplate);
                    }

                    var channelId = channelIdList[i];

                    pageInfo.ChannelItems.Push(new ChannelItemInfo(channelId, i));
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
                                if (itemIndex < channelIdList.Count)
                                {
                                    var channelId = channelIdList[itemIndex];

                                    pageInfo.ChannelItems.Push(new ChannelItemInfo(channelId, itemIndex));
                                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                                    cellHtml = TemplateUtility.GetChannelsItemTemplateString(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, pageInfo, EContextType.Channel, contextInfo);
                                }
                                tr.AddCell(cellHtml, cellAttributes);
                                itemIndex++;
                            }
                            if (itemIndex >= channelIdList.Count) break;
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

        private static object ParseEntity(PageInfo pageInfo, DataSet dataSource, List<int> channelIdList)
        {
            var channelInfoList = new List<Dictionary<string, object>>();
            var table = dataSource.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                var channelId = Convert.ToInt32(row[nameof(ContentAttribute.Id)]);

                var channelInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, channelId);
                if (channelInfo != null)
                {
                    channelInfoList.Add(channelInfo.ToDictionary());
                }
            }

            return channelInfoList;
        }
    }
}
