using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlChannels
    {
        public const string ElementName = "stl:channels";//栏目列表

        public const string AttributeChannelIndex = "channelindex";			//栏目索引
        public const string AttributeChannelName = "channelname";				//栏目名称
        public const string AttributeUpLevel = "uplevel";						//上级栏目的级别
        public const string AttributeTopLevel = "toplevel";					//从首页向下的栏目级别
        public const string AttributeIsTotal = "istotal";						//是否从所有栏目中选择（包括首页）
        public const string AttributeIsAllChildren = "isallchildren";			//是否显示所有级别的子栏目

        public const string AttributeGroupChannel = "groupchannel";		    //指定显示的栏目组
        public const string AttributeGroupChannelNot = "groupchannelnot";	    //指定不显示的栏目组
        public const string AttributeGroup = "group";		                    //指定显示的栏目组
        public const string AttributeGroupNot = "groupnot";	                //指定不显示的栏目组
        public const string AttributeTotalNum = "totalnum";					//显示栏目数目
        public const string AttributeStartNum = "startnum";					//从第几条信息开始显示
        public const string AttributeTitleWordNum = "titlewordnum";			//栏目名称文字数量
        public const string AttributeOrder = "order";						    //排序
        public const string AttributeIsImage = "isimage";					    //仅显示图片栏目
        public const string AttributeWhere = "where";                          //获取栏目列表的条件判断
        public const string AttributeIsDynamic = "isdynamic";              //是否动态显示

        public const string AttributeColumns = "columns";
        public const string AttributeDirection = "direction";
        public const string AttributeHeight = "height";
        public const string AttributeWidth = "width";
        public const string AttributeAlign = "align";
        public const string AttributeItemHeight = "itemheight";
        public const string AttributeItemWidth = "itemwidth";
        public const string AttributeItemAlign = "itemalign";
        public const string AttributeItemVerticalAlign = "itemverticalalign";
        public const string AttributeItemClass = "itemclass";
        public const string AttributeLayout = "layout";

        public static ListDictionary AttributeList => new ListDictionary
        {
            {"cellpadding", "填充"},
            {"cellspacing", "间距"},
            {"class", "Css类"},
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
            {AttributeGroupChannel, "指定显示的栏目组"},
            {AttributeGroupChannelNot, "指定不显示的栏目组"},
            {AttributeTotalNum, "显示栏目数目"},
            {AttributeStartNum, "从第几条信息开始显示"},
            {AttributeOrder, "排序"},
            {AttributeIsImage, "仅显示图片栏目"},
            {AttributeWhere, "获取栏目列表的条件判断"},
            {AttributeIsDynamic, "是否动态显示"},
            {AttributeChannelIndex, "栏目索引"},
            {AttributeChannelName, "栏目名称"},
            {AttributeUpLevel, "上级栏目的级别"},
            {AttributeTopLevel, "从首页向下的栏目级别"},
            {AttributeIsTotal, "是否从所有栏目中选择"},
            {AttributeIsAllChildren, "是否显示所有级别的子栏目"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var displayInfo = ContentsDisplayInfo.GetContentsDisplayInfoByXmlNode(node, pageInfo, contextInfo, EContextType.Channel);

                parsedContent = displayInfo.IsDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, displayInfo);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        public static IEnumerable GetDataSource(PageInfo pageInfo, ContextInfo contextInfo, ContentsDisplayInfo displayInfo)
        {
            var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelID, displayInfo.UpLevel, displayInfo.TopLevel);

            channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, displayInfo.ChannelIndex, displayInfo.ChannelName);

            var isTotal = TranslateUtils.ToBool(displayInfo.OtherAttributes[AttributeIsTotal]);

            if (TranslateUtils.ToBool(displayInfo.OtherAttributes[AttributeIsAllChildren]))
            {
                displayInfo.Scope = EScopeType.Descendant;
            }

            return StlDataUtility.GetChannelsDataSource(pageInfo.PublishmentSystemId, channelId, displayInfo.GroupChannel, displayInfo.GroupChannelNot, displayInfo.IsImageExists, displayInfo.IsImage, displayInfo.StartNum, displayInfo.TotalNum, displayInfo.OrderByString, displayInfo.Scope, isTotal, displayInfo.Where);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, ContentsDisplayInfo displayInfo)
        {
            var parsedContent = string.Empty;

            contextInfo.TitleWordNum = displayInfo.TitleWordNum;

            var dataSource = GetDataSource(pageInfo, contextInfo, displayInfo);

            if (displayInfo.Layout == ELayout.None)
            {
                var rptContents = new Repeater();

                if (!string.IsNullOrEmpty(displayInfo.HeaderTemplate))
                {
                    rptContents.HeaderTemplate = new SeparatorTemplate(displayInfo.HeaderTemplate);
                }
                if (!string.IsNullOrEmpty(displayInfo.FooterTemplate))
                {
                    rptContents.FooterTemplate = new SeparatorTemplate(displayInfo.FooterTemplate);
                }
                if (!string.IsNullOrEmpty(displayInfo.SeparatorTemplate))
                {
                    rptContents.SeparatorTemplate = new SeparatorTemplate(displayInfo.SeparatorTemplate);
                }
                if (!string.IsNullOrEmpty(displayInfo.AlternatingItemTemplate))
                {
                    rptContents.AlternatingItemTemplate = new RepeaterTemplate(displayInfo.AlternatingItemTemplate, displayInfo.SelectedItems, displayInfo.SelectedValues, displayInfo.SeparatorRepeatTemplate, displayInfo.SeparatorRepeat, pageInfo, EContextType.Channel, contextInfo);
                }

                rptContents.ItemTemplate = new RepeaterTemplate(displayInfo.ItemTemplate, displayInfo.SelectedItems,
                    displayInfo.SelectedValues, displayInfo.SeparatorRepeatTemplate, displayInfo.SeparatorRepeat,
                    pageInfo, EContextType.Channel, contextInfo);

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

                //设置显示属性
                TemplateUtility.PutContentsDisplayInfoToMyDataList(pdlContents, displayInfo);

                //设置列表模板
                pdlContents.ItemTemplate = new DataListTemplate(displayInfo.ItemTemplate, displayInfo.SelectedItems, displayInfo.SelectedValues, displayInfo.SeparatorRepeatTemplate, displayInfo.SeparatorRepeat, pageInfo, EContextType.Channel, contextInfo);
                if (!string.IsNullOrEmpty(displayInfo.HeaderTemplate))
                {
                    pdlContents.HeaderTemplate = new SeparatorTemplate(displayInfo.HeaderTemplate);
                }
                if (!string.IsNullOrEmpty(displayInfo.FooterTemplate))
                {
                    pdlContents.FooterTemplate = new SeparatorTemplate(displayInfo.FooterTemplate);
                }
                if (!string.IsNullOrEmpty(displayInfo.SeparatorTemplate))
                {
                    pdlContents.SeparatorTemplate = new SeparatorTemplate(displayInfo.SeparatorTemplate);
                }
                if (!string.IsNullOrEmpty(displayInfo.AlternatingItemTemplate))
                {
                    pdlContents.AlternatingItemTemplate = new DataListTemplate(displayInfo.AlternatingItemTemplate, displayInfo.SelectedItems, displayInfo.SelectedValues, displayInfo.SeparatorRepeatTemplate, displayInfo.SeparatorRepeat, pageInfo, EContextType.Channel, contextInfo);
                }

                pdlContents.DataSource = dataSource;
                pdlContents.DataKeyField = NodeAttribute.NodeId;
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
