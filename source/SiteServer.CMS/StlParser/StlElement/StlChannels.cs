using System;
using System.Collections;
using System.Collections.Generic;
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
    [Stl(Usage = "栏目列表", Description = "通过 stl:channels 标签在模板中显示栏目列表")]
    public class StlChannels
    {
        public const string ElementName = "stl:channels";

        public const string AttributeChannelIndex = "channelIndex";			    //栏目索引
        public const string AttributeChannelName = "channelName";				//栏目名称
        public const string AttributeUpLevel = "upLevel";						//上级栏目的级别
        public const string AttributeTopLevel = "topLevel";					    //从首页向下的栏目级别
        public const string AttributeIsTotal = "isTotal";						//是否从所有栏目中选择（包括首页）
        public const string AttributeIsAllChildren = "isAllChildren";			//是否显示所有级别的子栏目
        public const string AttributeGroupChannel = "groupChannel";		        //指定显示的栏目组
        public const string AttributeGroupChannelNot = "groupChannelNot";	    //指定不显示的栏目组
        public const string AttributeTotalNum = "totalNum";					    //显示栏目数目
        public const string AttributeStartNum = "startNum";					    //从第几条信息开始显示
        public const string AttributeTitleWordNum = "titleWordNum";			    //
        public const string AttributeOrder = "order";						    //排序
        public const string AttributeIsImage = "isImage";					    //仅显示图片栏目
        public const string AttributeWhere = "where";                           //获取栏目列表的条件判断
        public const string AttributeIsDynamic = "isDynamic";                   //是否动态显示
        public const string AttributeCellPadding = "cellPadding";
        public const string AttributeCellSpacing = "cellSpacing";
        public const string AttributeClass = "class";
        public const string AttributeColumns = "columns";
        public const string AttributeDirection = "direction";
        public const string AttributeHeight = "height";
        public const string AttributeWidth = "width";
        public const string AttributeAlign = "align";
        public const string AttributeLayout = "layout";
        public const string AttributeItemHeight = "itemHeight";
        public const string AttributeItemWidth = "itemWidth";
        public const string AttributeItemAlign = "itemAlign";
        public const string AttributeItemVerticalAlign = "itemVerticalAlign";
        public const string AttributeItemClass = "itemClass";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeChannelIndex, "栏目索引"},
            {AttributeChannelName, "栏目名称"},
            {AttributeUpLevel, "上级栏目的级别"},
            {AttributeTopLevel, "从首页向下的栏目级别"},
            {AttributeIsTotal, "是否从所有栏目中选择"},
            {AttributeIsAllChildren, "是否显示所有级别的子栏目"},
            {AttributeGroupChannel, "指定显示的栏目组"},
            {AttributeGroupChannelNot, "指定不显示的栏目组"},
            {AttributeTotalNum, "显示栏目数目"},
            {AttributeStartNum, "从第几条信息开始显示"},
            {AttributeTitleWordNum, "栏目名称文字数量"},
            {AttributeOrder, "排序"},
            {AttributeIsImage, "仅显示图片栏目"},
            {AttributeWhere, "获取栏目列表的条件判断"},
            {AttributeIsDynamic, "是否动态显示"},            
            {AttributeCellPadding, "填充"},
            {AttributeCellSpacing, "间距"},
            {AttributeClass, "Css类"},
            {AttributeColumns, "列数"},
            {AttributeDirection, "方向"},
            {AttributeHeight, "整体高度"},
            {AttributeWidth, "整体宽度"},
            {AttributeAlign, "整体对齐"},
            {AttributeLayout, "指定列表布局方式"},
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
                var listInfo = ListInfo.GetListInfoByXmlNode(node, pageInfo, contextInfo, EContextType.Channel);

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

            var isTotal = TranslateUtils.ToBool(listInfo.Others.Get(AttributeIsTotal));

            if (TranslateUtils.ToBool(listInfo.Others.Get(AttributeIsAllChildren)))
            {
                listInfo.Scope = EScopeType.Descendant;
            }

            return StlDataUtility.GetChannelsDataSource(pageInfo.PublishmentSystemId, channelId, listInfo.GroupChannel, listInfo.GroupChannelNot, listInfo.IsImageExists, listInfo.IsImage, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString, listInfo.Scope, isTotal, listInfo.Where);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var parsedContent = string.Empty;

            contextInfo.TitleWordNum = listInfo.TitleWordNum;

            var dataSource = GetDataSource(pageInfo, contextInfo, listInfo);

            if (listInfo.Layout == ELayout.None)
            {
                var rptContents = new Repeater();

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
                    rptContents.AlternatingItemTemplate = new RepeaterTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Channel, contextInfo);
                }

                rptContents.ItemTemplate = new RepeaterTemplate(listInfo.ItemTemplate, listInfo.SelectedItems,
                    listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat,
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
                TemplateUtility.PutListInfoToMyDataList(pdlContents, listInfo);

                //设置列表模板
                pdlContents.ItemTemplate = new DataListTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Channel, contextInfo);
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
                    pdlContents.AlternatingItemTemplate = new DataListTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.Channel, contextInfo);
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
