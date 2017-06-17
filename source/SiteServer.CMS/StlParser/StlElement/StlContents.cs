using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "内容列表", Description = "通过 stl:contents 标签在模板中显示内容列表")]
    public class StlContents
    {
        public const string ElementName = "stl:contents";

        public const string AttributeChannelIndex = "channelIndex";
        public const string AttributeChannelName = "channelName";
        public const string AttributeUpLevel = "upLevel";
        public const string AttributeTopLevel = "topLevel";
        public const string AttributeScope = "scope";
        public const string AttributeGroup = "group";
        public const string AttributeGroupNot = "groupNot";
        public const string AttributeGroupChannel = "groupChannel";
        public const string AttributeGroupChannelNot = "groupChannelNot";
        public const string AttributeGroupContent = "groupContent";
        public const string AttributeGroupContentNot = "groupContentNot";
        public const string AttributeTags = "tags";
        public const string AttributeIsTop = "isTop";
        public const string AttributeIsRecommend = "isRecommend";
        public const string AttributeIsHot = "isHot";
        public const string AttributeIsColor = "isColor";
        public const string AttributeTotalNum = "totalNum";
        public const string AttributeStartNum = "startNum";
        public const string AttributeTitleWordNum = "titleWordNum";
        public const string AttributeOrder = "order";
        public const string AttributeIsImage = "isImage";
        public const string AttributeIsVideo = "isVideo";
        public const string AttributeIsFile = "isFile";
        public const string AttributeIsNoDup = "isNoDup";
        public const string AttributeIsRelatedContents = "isRelatedContents";
        public const string AttributeWhere = "where";
        public const string AttributeIsDynamic = "isDynamic";
        public const string AttributeCellPadding = "cellPadding";
        public const string AttributeCellSpacing = "cellSpacing";
        public const string AttributeClass = "class";
        public const string AttributeColumns = "columns";
        public const string AttributeDirection = "direction";
        public const string AttributeHeight = "height";
        public const string AttributeWidth = "width";
        public const string AttributeAlign = "align";
        public const string AttributeItemHeight = "itemHeight";
        public const string AttributeItemWidth = "itemWidth";
        public const string AttributeItemAlign = "itemAlign";
        public const string AttributeItemVerticalAlign = "itemVerticalAlign";
        public const string AttributeItemClass = "itemClass";
        public const string AttributeLayout = "layout";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeChannelIndex, "栏目索引"},
            {AttributeChannelName, "栏目名称"},
            {AttributeUpLevel, "上级栏目的级别"},
            {AttributeTopLevel, "从首页向下的栏目级别"},
            {AttributeScope, "内容范围"},
            {AttributeGroupChannel, "指定显示的栏目组"},
            {AttributeGroupChannelNot, "指定不显示的栏目组"},
            {AttributeGroup, "指定显示的内容组"},
            {AttributeGroupNot, "指定不显示的内容组"},
            {AttributeGroupContent, "指定显示的内容组"},
            {AttributeGroupContentNot, "指定不显示的内容组"},
            {AttributeTags, "指定标签"},
            {AttributeIsTop, "仅显示置顶内容"},
            {AttributeIsRecommend, "仅显示推荐内容"},
            {AttributeIsHot, "仅显示热点内容"},
            {AttributeIsColor, "仅显示醒目内容"},
            {AttributeTotalNum, "显示内容数目"},
            {AttributeStartNum, "从第几条信息开始显示"},
            {AttributeTitleWordNum, "内容标题文字数量"},
            {AttributeOrder, "排序"},
            {AttributeIsImage, "仅显示图片内容"},
            {AttributeIsVideo, "仅显示视频内容"},
            {AttributeIsFile, "仅显示附件内容"},
            {AttributeIsNoDup, "不显示重复标题的内容"},
            {AttributeIsRelatedContents, "显示相关内容列表"},
            {AttributeWhere, "获取内容列表的条件判断"},
            {AttributeIsDynamic, "是否动态显示"},
            {AttributeCellPadding, "填充"},
            {AttributeCellSpacing, "间距"},
            {AttributeClass, "Css类"},
            {AttributeColumns, "列数"},
            {AttributeDirection, "方向"},
            {AttributeLayout, "指定列表布局方式"},
            {AttributeHeight, "整体高度"},
            {AttributeWidth, "整体宽度"},
            {AttributeAlign, "整体对齐"},
            {AttributeItemHeight, "项高度"},
            {AttributeItemWidth, "项宽度"},
            {AttributeItemAlign, "项水平对齐"},
            {AttributeItemVerticalAlign, "项垂直对齐"},
            {AttributeItemClass, "项Css类"},
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var listInfo = ListInfo.GetListInfoByXmlNode(node, pageInfo, contextInfo, EContextType.Content);

                parsedContent = listInfo.IsDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, listInfo);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        public static IEnumerable GetDataSource(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelId, listInfo.UpLevel, listInfo.TopLevel);

            channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, listInfo.ChannelIndex, listInfo.ChannelName);

            return StlDataUtility.GetContentsDataSource(pageInfo.PublishmentSystemInfo, channelId, contextInfo.ContentId, listInfo.GroupContent, listInfo.GroupContentNot, listInfo.Tags, listInfo.IsImageExists, listInfo.IsImage, listInfo.IsVideoExists, listInfo.IsVideo, listInfo.IsFileExists, listInfo.IsFile, listInfo.IsNoDup, listInfo.IsRelatedContents, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString, listInfo.IsTopExists, listInfo.IsTop, listInfo.IsRecommendExists, listInfo.IsRecommend, listInfo.IsHotExists, listInfo.IsHot, listInfo.IsColorExists, listInfo.IsColor, listInfo.Where, listInfo.Scope, listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.Others);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var parsedContent = string.Empty;

            var titleWordNum = contextInfo.TitleWordNum;
            contextInfo.TitleWordNum = listInfo.TitleWordNum;

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

            contextInfo.TitleWordNum = titleWordNum;

            return parsedContent;
        }

    }
}
