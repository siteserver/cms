using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Enumerations;
using SiteServer.CMS.Model;
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
            var listInfo = await ListInfo.GetListInfoAsync(pageInfo, contextInfo, EContextType.Content);
            var dataSource = await GetDataSourceAsync(pageInfo, contextInfo, listInfo);

            if (contextInfo.IsStlEntity)
            {
                return await ParseEntityAsync(pageInfo, dataSource);
            }

            return ParseElement(pageInfo, contextInfo, listInfo, dataSource);
        }

        private static async Task<DataSet> GetDataSourceAsync(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var channelId = await StlDataUtility.GetChannelIdByLevelAsync(pageInfo.SiteId, contextInfo.ChannelId, listInfo.UpLevel, listInfo.TopLevel);

            channelId = await ChannelManager.GetChannelIdAsync(pageInfo.SiteId, channelId, listInfo.ChannelIndex, listInfo.ChannelName);

            return await StlDataUtility.GetContentsDataSourceAsync(pageInfo.Site, channelId, contextInfo.ContentId, listInfo.GroupContent, listInfo.GroupContentNot, listInfo.Tags, listInfo.IsImageExists, listInfo.IsImage, listInfo.IsVideoExists, listInfo.IsVideo, listInfo.IsFileExists, listInfo.IsFile, listInfo.IsRelatedContents, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString, listInfo.IsTopExists, listInfo.IsTop, listInfo.IsRecommendExists, listInfo.IsRecommend, listInfo.IsHotExists, listInfo.IsHot, listInfo.IsColorExists, listInfo.IsColor, listInfo.Where, listInfo.Scope, listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.Others);
        }

        private static string ParseElement(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo, DataSet dataSource)
        {
            var parsedContent = string.Empty;

            if (listInfo.Layout == ELayout.None)
            {
                var rptContents = new Repeater
                {
                    ItemTemplate =
                        new RepeaterTemplate(listInfo.ItemTemplate, listInfo.SelectedItems,
                            listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat,
                            pageInfo, EContextType.Content, contextInfo)
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
                    rptContents.AlternatingItemTemplate = new RepeaterTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Content, contextInfo);
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

                pdlContents.ItemTemplate = new DataListTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Content, contextInfo);
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
                    pdlContents.AlternatingItemTemplate = new DataListTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Content, contextInfo);
                }

                pdlContents.DataSource = dataSource;
                pdlContents.DataKeyField = ContentAttribute.Id;
                pdlContents.DataBind();

                if (pdlContents.Items.Count > 0)
                {
                    parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
                }
            }

            return parsedContent;
        }

        private static async Task<object> ParseEntityAsync(PageInfo pageInfo, DataSet dataSource)
        {
            var contentInfoList = new List<IDictionary<string, object>>();

            var table = dataSource.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                var contentId = Convert.ToInt32(row[nameof(ContentAttribute.Id)]);
                var channelId = Convert.ToInt32(row[nameof(ContentAttribute.ChannelId)]);

                var contentInfo = await DataProvider.ContentDao.GetAsync(pageInfo.Site, channelId, contentId);

                if (contentInfo != null)
                {
                    contentInfoList.Add(contentInfo.ToDictionary());
                }
            }

            return contentInfoList;
        }
    }
}
