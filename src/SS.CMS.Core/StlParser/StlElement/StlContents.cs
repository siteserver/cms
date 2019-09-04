using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.Common.Enums;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Models;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "内容列表", Description = "通过 stl:contents 标签在模板中显示内容列表")]
    public class StlContents : StlListBase
    {
        public const string ElementName = "stl:contents";

        [StlAttribute(Title = "显示相关内容列表")]
        public const string IsRelatedContents = nameof(IsRelatedContents);

        public static async Task<object> ParseAsync(ParseContext parseContext)
        {
            parseContext.ContextType = EContextType.Content;
            var listInfo = await ListInfo.GetListInfoAsync(parseContext);
            var contentList = await GetContainerContentListAsync(parseContext, listInfo);

            if (parseContext.IsStlEntity)
            {
                return await ParseEntityAsync(parseContext, contentList);
            }

            return await ParseElementAsync(parseContext, listInfo, contentList);
        }

        // private static DataSet GetDataSource(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        // {
        //     var channelId = parseContext.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, listInfo.UpLevel, listInfo.TopLevel);

        //     channelId = ChannelManager.GetChannelId(pageInfo.SiteId, channelId, listInfo.ChannelIndex, listInfo.ChannelName);

        //     return parseContext.GetContentsDataSource(pageInfo.SiteInfo, channelId, contextInfo.ContentId, listInfo.GroupContent, listInfo.GroupContentNot, listInfo.Tags, listInfo.IsImageExists, listInfo.IsImage, listInfo.IsVideoExists, listInfo.IsVideo, listInfo.IsFileExists, listInfo.IsFile, listInfo.IsRelatedContents, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString, listInfo.IsTopExists, listInfo.IsTop, listInfo.IsRecommendExists, listInfo.IsRecommend, listInfo.IsHotExists, listInfo.IsHot, listInfo.IsColorExists, listInfo.IsColor, listInfo.Where, listInfo.Scope, listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.Others);
        // }

        private static async Task<List<KeyValuePair<int, Content>>> GetContainerContentListAsync(ParseContext parseContext, ListInfo listInfo)
        {
            var channelId = await parseContext.GetChannelIdByLevelAsync(parseContext.SiteId, parseContext.ChannelId, listInfo.UpLevel, listInfo.TopLevel);

            channelId = await parseContext.ChannelRepository.GetIdAsync(parseContext.SiteId, channelId, listInfo.ChannelIndex, listInfo.ChannelName);

            return await parseContext.GetContainerContentListAsync(parseContext.SiteInfo, channelId, parseContext.ContentId, listInfo.GroupContent, listInfo.GroupContentNot, listInfo.Tags, listInfo.IsImage, listInfo.IsVideo, listInfo.IsFile, listInfo.IsRelatedContents, listInfo.StartNum, listInfo.TotalNum, listInfo.Order, listInfo.IsTop, listInfo.IsRecommend, listInfo.IsHot, listInfo.IsColor, listInfo.Scope, listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.Others);
        }

        public static async Task<string> ParseElementAsync(ParseContext parseContext, ListInfo listInfo, List<KeyValuePair<int, Content>> contentList)
        {
            if (contentList == null || contentList.Count == 0) return string.Empty;

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

                for (var i = 0; i < contentList.Count; i++)
                {
                    if (isSeparator && i % 2 != 0 && i != contentList.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorTemplate);
                    }

                    var content = contentList[i];

                    parseContext.PageInfo.ContentItems.Push(content);
                    var context = parseContext.Clone(EContextType.Content);
                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                    builder.Append(await TemplateUtility.GetContentsItemTemplateStringAsync(context, templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty));
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
                                if (itemIndex < contentList.Count)
                                {
                                    var content = contentList[itemIndex];

                                    parseContext.PageInfo.ContentItems.Push(content);
                                    var context = parseContext.Clone(EContextType.Content);

                                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                                    cellHtml = await TemplateUtility.GetContentsItemTemplateStringAsync(context, templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty);
                                }
                                tr.AddCell(cellHtml, cellAttributes);
                                itemIndex++;
                            }
                            if (itemIndex >= contentList.Count) break;
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
            //                 pageInfo, EContextType.Content, contextInfo)
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
            //         rptContents.AlternatingItemTemplate = new RepeaterTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Content, contextInfo);
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

            //     pdlContents.ItemTemplate = new DataListTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Content, contextInfo);
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
            //         pdlContents.AlternatingItemTemplate = new DataListTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Content, contextInfo);
            //     }

            //     pdlContents.DataSource = dataSource;
            //     pdlContents.DataKeyField = ContentAttribute.Id;
            //     pdlContents.DataBind();

            //     if (pdlContents.Items.Count > 0)
            //     {
            //         parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
            //     }
            // }

            // return parsedContent;
        }

        private static async Task<object> ParseEntityAsync(ParseContext parseContext, List<KeyValuePair<int, Content>> contentList)
        {
            var contentInfoList = new List<IDictionary<string, object>>();

            // var table = dataSource.Tables[0];
            // foreach (DataRow row in table.Rows)
            // {
            //     var contentId = Convert.ToInt32(row[nameof(ContentAttribute.Id)]);
            //     var channelId = Convert.ToInt32(row[nameof(ContentAttribute.ChannelId)]);

            //     var contentInfo = ContentManager.GetContentInfo(pageInfo.SiteInfo, channelId, contentId);

            //     if (contentInfo != null)
            //     {
            //         contentInfoList.Add(contentInfo.ToDictionary());
            //     }
            // }

            foreach (var content in contentList)
            {
                var channelInfo = await parseContext.ChannelRepository.GetChannelAsync(content.Value.ChannelId);
                var contentRepository = parseContext.ChannelRepository.GetContentRepository(parseContext.SiteInfo, channelInfo);
                var contentInfo = await contentRepository.GetContentInfoAsync(content.Value.Id);

                if (contentInfo != null)
                {
                    contentInfoList.Add(contentInfo.ToDictionary());
                }
            }

            return contentInfoList;
        }
    }
}
