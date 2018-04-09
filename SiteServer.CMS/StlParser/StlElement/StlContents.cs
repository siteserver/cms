using System.Data;
using System.Web.UI.WebControls;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "内容列表", Description = "通过 stl:contents 标签在模板中显示内容列表")]
    public class StlContents
    {
        public const string ElementName = "stl:contents";

        public static readonly Attr ChannelIndex = new Attr("channelIndex", "栏目索引");
        public static readonly Attr ChannelName = new Attr("channelName", "栏目名称");
        public static readonly Attr UpLevel = new Attr("upLevel", "上级栏目的级别");
        public static readonly Attr TopLevel = new Attr("topLevel", "从首页向下的栏目级别");
        public static readonly Attr Scope = new Attr("scope", "内容范围");
        public static readonly Attr Group = new Attr("group", "指定显示的栏目组");
        public static readonly Attr GroupNot = new Attr("groupNot", "指定不显示的栏目组");
        public static readonly Attr GroupChannel = new Attr("groupChannel", "指定显示的内容组");
        public static readonly Attr GroupChannelNot = new Attr("groupChannelNot", "指定不显示的内容组");
        public static readonly Attr GroupContent = new Attr("groupContent", "指定显示的内容组");
        public static readonly Attr GroupContentNot = new Attr("groupContentNot", "指定不显示的内容组");
        public static readonly Attr Tags = new Attr("tags", "指定标签");
        public static readonly Attr IsTop = new Attr("isTop", "仅显示置顶内容");
        public static readonly Attr IsRecommend = new Attr("isRecommend", "仅显示推荐内容");
        public static readonly Attr IsHot = new Attr("isHot", "仅显示热点内容");
        public static readonly Attr IsColor = new Attr("isColor", "仅显示醒目内容");
        public static readonly Attr TotalNum = new Attr("totalNum", "显示内容数目");
        public static readonly Attr StartNum = new Attr("startNum", "从第几条信息开始显示");
        public static readonly Attr Order = new Attr("order", "排序");
        public static readonly Attr IsImage = new Attr("isImage", "仅显示图片内容");
        public static readonly Attr IsVideo = new Attr("isVideo", "仅显示视频内容");
        public static readonly Attr IsFile = new Attr("isFile", "仅显示附件内容");
        public static readonly Attr IsNoDup = new Attr("isNoDup", "不显示重复标题的内容");
        public static readonly Attr IsRelatedContents = new Attr("isRelatedContents", "显示相关内容列表");
        public static readonly Attr Where = new Attr("where", "获取内容列表的条件判断");
        public static readonly Attr CellPadding = new Attr("cellPadding", "填充");
        public static readonly Attr CellSpacing = new Attr("cellSpacing", "间距");
        public static readonly Attr Class = new Attr("class", "Css类");
        public static readonly Attr Columns = new Attr("columns", "列数");
        public static readonly Attr Direction = new Attr("direction", "方向");
        public static readonly Attr Height = new Attr("height", "指定列表布局方式");
        public static readonly Attr Width = new Attr("width", "整体高度");
        public static readonly Attr Align = new Attr("align", "整体宽度");
        public static readonly Attr ItemHeight = new Attr("itemHeight", "整体对齐");
        public static readonly Attr ItemWidth = new Attr("itemWidth", "项高度");
        public static readonly Attr ItemAlign = new Attr("itemAlign", "项宽度");
        public static readonly Attr ItemVerticalAlign = new Attr("itemVerticalAlign", "项水平对齐");
        public static readonly Attr ItemClass = new Attr("itemClass", "项垂直对齐");
        public static readonly Attr Layout = new Attr("layout", "项Css类");

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            // 如果是实体标签则返回空
            if (contextInfo.IsStlEntity)
            {
                return string.Empty;
            }

            var listInfo = ListInfo.GetListInfoByXmlNode(pageInfo, contextInfo, EContextType.Content);

            return ParseImpl(pageInfo, contextInfo, listInfo);
        }

        public static DataSet GetDataSource(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var channelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, listInfo.UpLevel, listInfo.TopLevel);

            channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, channelId, listInfo.ChannelIndex, listInfo.ChannelName);

            return StlDataUtility.GetContentsDataSource(pageInfo.SiteInfo, channelId, contextInfo.ContentId, listInfo.GroupContent, listInfo.GroupContentNot, listInfo.Tags, listInfo.IsImageExists, listInfo.IsImage, listInfo.IsVideoExists, listInfo.IsVideo, listInfo.IsFileExists, listInfo.IsFile, listInfo.IsNoDup, listInfo.IsRelatedContents, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString, listInfo.IsTopExists, listInfo.IsTop, listInfo.IsRecommendExists, listInfo.IsRecommend, listInfo.IsHotExists, listInfo.IsHot, listInfo.IsColorExists, listInfo.IsColor, listInfo.Where, listInfo.Scope, listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.Others);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var parsedContent = string.Empty;

            var dataSource = GetDataSource(pageInfo, contextInfo, listInfo);

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

    }
}
